using PurrNet;
using PurrNet.Pooling;
using PurrNet.Prediction;
using ShinyOwl.Common;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    public class PredictedPlayerSpawner : DeterministicIdentity<PredictedPlayerSpawner.State>
    {
        [SerializeField] private PurrdictionPlayer _playerPrefab;

        public struct State : IPredictedData<State>
        {
            public DisposableDictionary<PlayerID, PredictedObjectID> PlayerLookup;
            public DisposableList<PlayerID> ToAdd;
            public DisposableList<PlayerID> ToRemove;

            public void Dispose()
            {
                PlayerLookup.Dispose();
                ToAdd.Dispose();
                ToRemove.Dispose();
            }
        }

        protected override State GetInitialState()
        {
            base.GetInitialState();

            return new State()
            {
                PlayerLookup = DisposableDictionary<PlayerID, PredictedObjectID>.Create(),
                ToAdd = DisposableList<PlayerID>.Create(),
                ToRemove = DisposableList<PlayerID>.Create()
            };
        }

        protected override void SimulationStart()
        {
            foreach (PlayerID id in predictionManager.players.players)
            {
                HandlePlayerAdded(id);
            }

            predictionManager.players.onPlayerAdded += HandlePlayerAdded;
            predictionManager.players.onPlayerRemoved += HandlePlayerRemoved;
        }

        protected override void Destroyed()
        {
            if (predictionManager?.players != null)
            {
                predictionManager.players.onPlayerAdded -= HandlePlayerAdded;
                predictionManager.players.onPlayerRemoved -= HandlePlayerRemoved;
            }
        }

        private void HandlePlayerAdded(PlayerID id)
        {
            currentState.ToAdd.Add(id);
        }

        private void HandlePlayerRemoved(PlayerID id)
        {
            currentState.ToRemove.Add(id);
        }

        protected override void Simulate(ref State state, sfloat delta)
        {
            foreach (PlayerID id in state.ToAdd)
            {
                AddPlayer(id);
            }

            foreach (PlayerID id in state.ToRemove)
            {
                RemovePlayer(id);
            }

            state.ToAdd.Clear();
            state.ToRemove.Clear();
        }

        private void AddPlayer(PlayerID playerId)
        {
            if (currentState.PlayerLookup.ContainsKey(playerId))
            {
                Debugger.LogError(this, "This player is already assigned a predicted object");
                return;
            }

            PredictedObjectID objectId = hierarchy.Create(_playerPrefab.gameObject, playerId).Value;
            currentState.PlayerLookup[playerId] = objectId;
        }

        private void RemovePlayer(PlayerID playerId)
        {
            if (!currentState.PlayerLookup.TryGetValue(playerId, out PredictedObjectID objectId))
            {
                return;
            }

            hierarchy.Delete(objectId);
            currentState.PlayerLookup.Remove(playerId);
        }

        protected override State Interpolate(State from, State to, float t)
        {
            return to;
        }
    }
}