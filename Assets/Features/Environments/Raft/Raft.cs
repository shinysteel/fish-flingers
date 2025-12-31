using FishFlingers.Pools;
using NUnit.Framework;
using PurrNet;
using System.Timers;
using UnityEngine;
using System.Collections.Generic;
using ShinyOwl.Common;
using PurrNet.Prediction;
using PurrNet.Pooling;
using System.Linq;
using System.Text;

namespace FishFlingers.Environments
{
    public class Raft : PredictedIdentity<Raft.State>
    {
        [SerializeField] private Transform _tilesContainer;

        [SerializeField] private GameObject _tilePrefab;

        public struct State : IPredictedData<State>
        {
            public DisposableDictionary<Vector2Int, PredictedObjectID> TileIds;

            public void Dispose() 
            {
                TileIds.Dispose();
            }
        }

        protected override State GetInitialState()
        {
            return new State()
            {
                TileIds = DisposableDictionary<Vector2Int, PredictedObjectID>.Create()
            };
        }

        protected override void SimulationStart()
        {
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    AddTile(new Vector2Int(i, j));
                }
            }
        }
        
        [SimulationOnly]
        public void AddTile(Vector2Int cell)
        {
            if (currentState.TileIds.ContainsKey(cell))
            {
                return;
            }

            PredictedObjectID tileId = hierarchy.Create(_tilePrefab, PlayerID.Server).Value;

            Tile tile = tileId.GetComponent<Tile>(predictionManager);
            tile.SetCell(cell);
            tile.SetHealth(Tile.DefaultHealth);

            currentState.TileIds.Add(cell, tileId);
        }

        [SimulationOnly]
        public void DamageTile(Vector2Int cell, int damage)
        {
            if (!currentState.TileIds.TryGetValue(cell, out PredictedObjectID tileId))
            {
                return;
            }

            Tile tile = tileId.GetComponent<Tile>(predictionManager);
            tile.SetHealth(tile.currentState.Health - 1);

            if (tile.currentState.Health <= 0)
            {
                hierarchy.Delete(tileId);
                currentState.TileIds.Remove(cell);
            }
        }
    }
}