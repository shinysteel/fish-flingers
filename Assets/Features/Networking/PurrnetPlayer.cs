using FishFlingers.Cameras;
using FishFlingers.Scenes;
using PurrLobby;
using PurrNet;
using PurrNet.Transports;
using ShinyOwl.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using FishFlingers.Networking.Predictions;
using System.Threading;

namespace FishFlingers.Networking
{
    public class PurrnetPlayer : NetworkBehaviour, INetworkManagerListener
    {
        private NetworkManager _networkManager;

        protected override void OnInitializeModules()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
        }

        protected override void OnSpawned()
        {
            // If we've missed the OnLobbyStart event, let's invoke it here
            if (_networkManager.CurrentLobby.Properties[LobbyService.StartedKey] == true.ToString())
            {
                OnLobbyStart(_networkManager.CurrentLobby);
            }

            // We deliberately subscribe after invoking missed events
            _networkManager.AddListener(this);
        }

        protected override void OnDespawned()
        {
            _networkManager?.RemoveListener(this);
        }

        protected override void OnOwnerDisconnected(PlayerID ownerId)
        {
            Destroy(gameObject);
        }

        public void OnLobbyStart(Lobby lobby)
        {
            // There used to be code for spawning a 'human' to control here.
            // Since we moved to Purrdiction, that's handled separately from
            // Purrnet. I'm leaving the implementation here since it's a nice
            // reference to look back on
        }

        public void OnLobbyEnter(Lobby lobby) { }
        public void OnLobbyCreated(Lobby lobby) { }
        public void OnLobbyLeave() { }
        public void OnNetworkStarted(bool asServer) { }
        public void OnNetworkShutdown(bool asServer) { }
        public void OnClientConnectionState(ConnectionState state) { }
        public void OnPlayerJoined(PlayerID id, bool isReconnect, bool asServer) { }
        public void OnPlayerLeft(PlayerID id, bool asServer) { }
        public void OnNetworkSceneLoaded(EScene scene, bool asServer) { }
        public void OnNetworkSceneUnloaded(EScene scene, bool asServer) { }
    }
}