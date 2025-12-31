using FishFlingers.Scenes;
using FishFlingers.States;
using PrimeTween;
using PurrNet;
using PurrNet.Prediction;
using PurrNet.Transports;
using ShinyOwl.Common;
using System.Threading.Tasks;
using UnityEngine;

namespace FishFlingers.Networking.Predictions
{
    public interface IPredictionManagerListener
    { }

    public class PredictionManager : GameSystem<IPredictionManagerListener>, INetworkManagerListener, ISceneManagerListener
    {
        private PredictionManagerConfig _config;

        private NetworkManager _networkManager;
        private SceneManager _sceneManager;

        private PurrNet.Prediction.PredictionManager _purrnetPredictionManager;

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
            return _purrnetPredictionManager.hierarchy.Create(prefab, owner);
        }

        // Currently, this only gets called once for both the server and client
        public void OnNetworkSceneLoaded(EScene scene, bool asServer) 
        {
            if (scene == EScene.Game)
            {
                if (_purrnetPredictionManager != null)
                {
                    Debugger.LogError(this, "Tried to create or retrieve a prediction manager when one already exists");
                    return;
                }

                // Only the server needs to instantiate the manager, since it will be networked. PUrrdiction is setup
                // such that prediction managers need to exist within the scenes they manage
                _purrnetPredictionManager = asServer
                    ? Object.Instantiate(_config.PurrdictionPredictionManagerPrefab)
                    : Object.FindFirstObjectByType<PurrNet.Prediction.PredictionManager>();

                if (asServer)
                {
                    Spawn(_config.PlayerSpawnerPrefab.gameObject, PlayerID.Server);
                }
            }
        }

        public void OnSceneUnloaded(EScene scene) 
        {
            if (scene == EScene.Game)
            {
                _purrnetPredictionManager = null;
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
        public void OnActiveSceneChanged(EScene previous, EScene current) { }
    }
}
