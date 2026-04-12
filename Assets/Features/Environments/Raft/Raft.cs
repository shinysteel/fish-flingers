using FishFlingers.Entities;
using FishFlingers.Networking;
using FishFlingers.Pools;
using FishFlingers.Saving;
using FishFlingers.Scenes;
using FishFlingers.States;
using Newtonsoft.Json;
using PurrNet;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using EntityId = FishFlingers.Entities.EntityId;
using Random = UnityEngine.Random;

namespace FishFlingers.Environments
{
    public class RaftSave
    {
        [JsonProperty] public List<TileSave> Tiles { get; private set; } = new();
        [JsonProperty] public List<StructureSave> Structures { get; private set; } = new();

        public void LoadTo(Raft raft)
        {
            foreach (TileSave save in Tiles)
            {
                raft.AddNetTileRpc(save.Cell, save.Health);
            }

            foreach (StructureSave save in Structures)
            {
                raft.AddStructureRpc(save.Cell, save.StructureId);

                // Since we are the server, we can assume it exists straight away
                raft.Tiles[save.Cell].Structure.LoadJsonData(save.JsonData);
            }
        }

        public void SaveFrom(Raft raft)
        {
            Tiles.Clear();
            Structures.Clear();

            foreach (Tile tile in raft.Tiles.Values)
            {
                Tiles.Add(new TileSave(tile));

                if (tile.Structure != null)
                {
                    Structures.Add(new StructureSave(tile.Structure));
                }
            }
        }

        public void ApplyDefaults()
        {
            // Start with a 3x3 grid
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int health = NetTile.MaxHealth;

                    // 33% chance to have one less health
                    if (Random.value < 1f / 3f)
                    {
                        health--;
                    }

                    Tiles.Add(new TileSave(new Vector2Int(x, y), health));
                }
            }

            // Start with a wave sign
            Structures.Add(new StructureSave(new Vector2Int(0, 1), EntityId.WaveSign, string.Empty));
        }
    }

    public class NetTile
    {
        public int Health { get; private set; }

        public const int MaxHealth = 3;

        public Structure Structure { get; private set; }

        public NetTile(int health)
        {
            SetHealth(health);
        }

        public void SetHealth(int health)
        {
            Health = Mathf.Clamp(health, 0, MaxHealth);
        }

        public void SetStructure(Structure structure)
        {
            Structure = structure;
        }
    }

    public class Raft : GameplayBehaviour
    {
        [SerializeField] private Transform _tilesContainer;

        private SyncDictionaryWrapper<Vector2Int, NetTile> _netTiles = new SyncDictionaryWrapper<Vector2Int, NetTile>(ownerAuth: true);

        private Dictionary<Vector2Int, Tile> _tiles = new();
        public IReadOnlyDictionary<Vector2Int, Tile> Tiles => _tiles;

        public event Action<Vector2Int, Tile> OnTileChanged;

        private RaftQueries _queries;
        public RaftQueries Queries => _queries;

        public override void Initialise(GameplayContext context)
        {
            base.Initialise(context);

            _instantiateManager.RaiseComponentInstantiated(this);

            _netTiles.onChanged += HandleNetTilesChanged;

            _queries = new RaftQueries(this);

            if (isOwner)
            {
                return;
            }

            // Clients need to manually handle changes that have happened before we joined
            foreach (KeyValuePair<Vector2Int, NetTile> kvp in _netTiles)
            {
                SyncDictionaryChange<Vector2Int, NetTile> change = new(SyncDictionaryOperation.Added, kvp.Key, kvp.Value);
                HandleNetTilesChanged(change);
            }
        }

        protected override void OnDespawned()
        {
            _instantiateManager.RaiseComponentDestroyed(this);
        }

        [ServerRpc(requireOwnership: false)]
        public void AddNetTileRpc(Vector2Int cell, int health)
        {
            _netTiles.TryAdd(cell, new NetTile(health));
        }

        [ServerRpc(requireOwnership: false)]
        public void AddStructureRpc(Vector2Int cell, EntityId structureId)
        {
            AddStructure(cell, structureId);
        }

        // No tile param is good here, since it lets callers request to damage a cell without having to worry
        // if it exists anymore
        public void ChangeNetTileHealth(Vector2Int cell, int change)
        {
            if (!isOwner)
            {
                return;
            }

            if (change == 0)
            {
                return;
            }

            if (!_netTiles.TryGetValue(cell, out NetTile netTile))
            {
                return;
            }

            netTile.SetHealth(netTile.Health + change);

            if (netTile.Health > 0)
            {
                _netTiles.SetDirty(cell);
            }
            else
            {
                _netTiles.Remove(cell);
            }
        }

        private void AddStructure(Vector2Int cell, EntityId structureId)
        {
            if (!isOwner)
            {
                return;
            }

            if (_entityManager.GetEntityPrefab(structureId) is not Structure)
            {
                return;
            }

            if (!_netTiles.TryGetValue(cell, out NetTile netTile))
            {
                return;
            }

            if (netTile.Structure != null)
            {
                return;
            }

            if (!_tiles.TryGetValue(cell, out Tile tile))
            {
                return;
            }

            Structure structure = (Structure)_entityManager.Spawn(structureId, new SpawnParams() { Parent = tile.transform, Position = new Vector3(tile.transform.position.x, tile.GetSurfaceY(), tile.transform.position.z) });
            structure.SetCell(cell);
            
            netTile.SetStructure(structure);
            _netTiles.SetDirty(cell);
        }

        private void HandleNetTilesChanged(SyncDictionaryChange<Vector2Int, NetTile> change)
        {
            // Tile exists
            if (change.operation != SyncDictionaryOperation.Removed && change.value != null)
            {
                SetTile(change.key, change.value);
            }
            // Tile no longer exists
            else
            {
                RemoveTile(change.key);
            }
        }

        // Adds a new tile, or updates an existing one
        private void SetTile(Vector2Int cell, NetTile netTile)
        {
            // Retrieve from pool
            if (!_tiles.ContainsKey(cell))
            {
                _tiles[cell] = (Tile)_entityManager.Spawn(EntityId.Tile, new SpawnParams() { Parent = _tilesContainer });
                _tiles[cell].Initialise(_context);
            }

            Tile tile = _tiles[cell];

            tile.SetHealth(netTile.Health);
            tile.SetCell(cell);
            tile.SetStructure(netTile.Structure);

            OnTileChanged?.Invoke(cell, tile);
        }

        private void RemoveTile(Vector2Int cell)
        {
            if (!_tiles.TryGetValue(cell, out Tile tile))
            {
                return;
            }

            // Return to pool
            _entityManager.Despawn(tile);

            _tiles.Remove(tile.Cell);

            OnTileChanged?.Invoke(cell, null);
        }
    }
}