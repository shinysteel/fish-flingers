using FishFlingers.Scenes;
using FishFlingers.States;
using NUnit.Framework;
using PrimeTween;
using PurrNet;
using PurrNet.Prediction;
using PurrNet.Transports;
using ShinyOwl.Common;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using PurrNet.Pooling;

namespace FishFlingers.Networking.Predictions
{
    public interface IPredictionManagerListener
    { }

    public class PredictionManager : GameSystem<IPredictionManagerListener>, INetworkManagerListener, ISceneManagerListener
    {
        private PredictionManagerConfig _config;

        private NetworkManager _networkManager;
        private SceneManager _sceneManager;

        private PurrdictionPredictionManager _purrdictionPredictionManager;
        private PredictedPlayerSpawner _spawner;

        // This is currently unreliable for clients
        public ref DisposableDictionary<PlayerID, PredictedObjectID> PlayerMap => ref _spawner.currentState.PlayerMap;

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.PredictionManagerConfig;

            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _sceneManager = GameManager.Instance.Get<SceneManager>();

            _networkManager.AddListener(this);
            _sceneManager.AddListener(this);

            base.Initialise(config);
        }

        public override void Shutdown()
        {
            _networkManager?.RemoveListener(this);
            _sceneManager?.RemoveListener(this);

            base.Shutdown();
        }

        public PredictedObjectID? Spawn(GameObject prefab, PlayerID? owner)
        {
            return _purrdictionPredictionManager.hierarchy.Create(prefab, owner);
        }

        // Intended for clients to have their prediction systems self report once they are ready, since they 
        // don't have the benefit of spawning it themselves
        public void RegisterPurrdictionPredictionManager(PurrdictionPredictionManager manager)
        {
            _purrdictionPredictionManager = manager;
        }

        public void RegisterPredictedPlayerSpawner(PredictedPlayerSpawner spawner)
        {
            _spawner = spawner;
        }

        public void UnregisterPurrdictionPredictionManager(PurrdictionPredictionManager manager)
        {
            if (_purrdictionPredictionManager == manager)
            {
                _purrdictionPredictionManager = null;
            }
        }

        public void UnregisterPredictedPlayerSpawner(PredictedPlayerSpawner spawner)
        {
            if (_spawner == spawner)
            {
                _spawner = null;
            }
        }

        // Currently, this only gets called once for both the server and client
        public void OnNetworkSceneLoaded(EScene scene, bool asServer) 
        {
            if (scene == EScene.Game)
            {
                if (_purrdictionPredictionManager != null || _spawner != null)
                {
                    Debugger.LogError(this, "Tried to create or retrieve a prediction system that is already assigned");
                    return;
                }

                if (asServer)
                {
                    // Only the server needs to instantiate the manager, since it will be networked. Purrdiction is setup
                    // such that prediction managers need to exist within the scenes they manage

                    _purrdictionPredictionManager = Object.Instantiate(_config.PurrdictionPredictionManagerPrefab);
                    PredictedObjectID spawnerId = Spawn(_config.PlayerSpawnerPrefab.gameObject, PlayerID.Server).Value;
                    _spawner = spawnerId.GetGameObject(_purrdictionPredictionManager).GetComponent<PredictedPlayerSpawner>();
                }
            }
        }

        public void OnLobbyCreated(Lobby lobby) { }
        public void OnLobbyEnter(Lobby lobby) { }
        public void OnLobbyStart(Lobby lobby) { }
        public void OnLobbyLeave() { }
        public void OnNetworkStarted(bool asServer) { }
        public void OnNetworkShutdown(bool asServer) { }
        public void OnClientConnectionState(ConnectionState state) { }
        public void OnPlayerJoined(PlayerID id, bool isReconnect, bool asServer) { }
        public void OnPlayerLeft(PlayerID id, bool asServer) { }
        public void OnNetworkSceneUnloaded(EScene scene, bool asServer) { }
        public void OnSceneLoaded(EScene scene, LoadSceneMode mode) { }
        public void OnSceneUnloaded(EScene scene) { }
        public void OnActiveSceneChanged(EScene previous, EScene current) { }
    }
}
