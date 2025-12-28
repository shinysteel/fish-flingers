using PurrNet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishFlingers.Entities;
using FishFlingers.Cameras;
using ShinyOwl.Common;
using FishFlingers.Scenes;
using PurrNet.Transports;

namespace FishFlingers.Networking
{
    public class Player : NetworkBehaviour, INetworkManagerListener
    {
        [SerializeField] private Character _humanPrefab;

        private CameraManager _cameraManager;
        private NetworkManager _networkManager;

        private Character _human;

        protected override void OnEarlySpawn()
        {
            _cameraManager = GameManager.Instance.Get<CameraManager>();
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _networkManager.AddListener(this);
        }

        protected override void OnDespawned()
        {
            _networkManager.RemoveListener(this);
        }

        protected override void OnOwnerDisconnected(PlayerID ownerId)
        {
            Destroy(gameObject);
        }

        public void OnLobbyStart()
        { 
            if (!isOwner)
            {
                return;
            }

            _human = Instantiate(_humanPrefab);

            _cameraManager.SetMode(new FollowCameraMode(_human.transform, new Vector3(0f, 3f, -5f)));
        }

        public void OnLobbyCreated(Lobby lobby) { }
        public void OnLobbyEnter(Lobby lobby) { }
        public void OnLobbyLeave() { }
        public void OnNetworkStarted(bool asServer) { }
        public void OnNetworkShutdown(bool asServer) { }
        public void OnClientConnectionState(ConnectionState state) { }
        public void OnPlayerJoined(PlayerID id, bool isReconnect) { }
        public void OnPlayerLeft(PlayerID id) { }
    }
}