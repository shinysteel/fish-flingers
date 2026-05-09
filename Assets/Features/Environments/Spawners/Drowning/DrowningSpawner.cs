using FishFlingers.Entities;
using FishFlingers.Networking;
using System.Collections.Generic;
using UnityEngine;
using EntityId = FishFlingers.Entities.EntityId;
using System.Linq;
using ShinyOwl.Common;
using ShinyOwl.Common.Utils;

namespace FishFlingers.Environments
{
    public class DrowningSpawner : GameplayBehaviour, IEntityManagerListener
    {
        private Dictionary<RaftPlayer, Drowning> _playerDrowningMap = new();

        protected override void OnSpawned()
        {
            _entityManager.AddListener(this);

            base.OnSpawned();
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();

            _entityManager?.RemoveListener(this);
        }

        private void Update()
        {
            if (!isOwner)
            {
                return;
            }

            if (!_isInitialised)
            {
                return;
            }

            SpawnUpdate();
        }

        // Manage the collection of drownings based on players who are alive and in the water for long enough
        private void SpawnUpdate()
        {
            foreach (RaftPlayer player in _context.Players)
            {
                if (!player.EntityDefeatModule.IsDefeated && player.RaftPlayerPhysicsModule.TimeInWater >= 0.5f)
                {
                    AddDrowning(player);
                }
                else
                {
                    RemoveDrowning(player);
                }
            }
        }

        // Create a drowning to target a valid player if one doesn't already exist
        private void AddDrowning(RaftPlayer player)
        {
            if (_playerDrowningMap.ContainsKey(player))
            {
                return;
            }

            Drowning drowning = (Drowning)_entityManager.Spawn(EntityId.Drowning, new SpawnParams() { Position = NetworkManager.HiddenSpawnPosition });
            drowning.SetTarget(player);
            _playerDrowningMap.Add(player, drowning);
        }

        // Cleanup a drowning that no longer needs to exist
        private void RemoveDrowning(RaftPlayer player)
        {
            if (!_playerDrowningMap.TryGetValue(player, out Drowning drowning))
            {
                return;
            }

            _playerDrowningMap.Remove(player);

            // The flow that exists can cause this to be called for a drowning that has already despawned, since we are listening to OnEntityDespawned
            if (drowning.isSpawned)
            {
                _entityManager.Despawn(drowning);
            }
        }

        void IEntityManagerListener.OnEntityDespawned(IEntity entity)
        {
            if (!isOwner)
            {
                return;
            }

            // When players disconnect, we still need to remove any drowning associated with them
            if (entity is RaftPlayer player)
            {
                RemoveDrowning(player);
            }
            // When a drowning despawns itself, we need to track that
            else if (entity is Drowning drowning)
            {
                Utils.Collections.RemoveDictionaryKeys(_playerDrowningMap, (KeyValuePair<RaftPlayer, Drowning> kvp) => kvp.Value == drowning);
            }
        }
    }
}