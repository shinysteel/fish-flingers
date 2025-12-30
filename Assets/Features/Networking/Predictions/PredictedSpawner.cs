using FishFlingers.Entities;
using FishFlingers.Scenes;
using ParrelSync;
using PurrNet;
using PurrNet.Prediction;
using ShinyOwl.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    /// <summary>
    /// Purrdiction provides PredictedPlayerSpawner to spawn predicted identities. It's not flexible
    /// at all since it's designed to just spawn a prefab for each player once they join. This script
    /// should allow us to instruct when to do so and any other feature that comes up later
    /// </summary>
    
    public class PredictedSpawner : PredictedPlayerSpawner
    {
        // Just making this public since PredictedPlayerSpawner made it's prefab field private :)
        [Header("IGNORE FIELDS ABOVE THIS")]
        public GameObject PlayerPrefab;

        // We don't want to inherit these methods
        protected override void LateAwake()
        {
            if (predictionManager.players)
            {
                IReadOnlyList<PlayerID> players = predictionManager.players.players;
                for (int i = 0; i < players.Count; i++)
                {
                    OnPlayerLoadedScene(players[i]);
                }

                predictionManager.players.onPlayerAdded += OnPlayerLoadedScene;
            }
        }

        protected override void Destroyed()
        {
            if (predictionManager && predictionManager.players)
            {
                predictionManager.players.onPlayerAdded -= OnPlayerLoadedScene;
            }
        }

        private void OnPlayerLoadedScene(PlayerID player)
        {
            if (!isServer)
            {
                return;
            }

            if (currentState.ContainsKey(player))
            {
                return;
            }

            PredictedObjectID? obj = hierarchy.Create(PlayerPrefab.gameObject, player);
            currentState[player] = obj.Value;
            predictionManager.SetOwnership(obj, player);
        }
    }
}