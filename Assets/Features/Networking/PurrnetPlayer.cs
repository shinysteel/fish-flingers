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
using System.Threading;
using FishFlingers.Entities;

namespace FishFlingers.Networking
{
    public class PurrnetPlayer : NetworkBehaviour, ILobbyManagerListener
    {
        [SerializeField] private RaftPlayer _playerPrefab;

        private NetworkManager _networkManager;
        private SceneManager _sceneManager;
        private LobbyManager _lobbyManager;

        protected override void OnInitializeModules()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _sceneManager = GameManager.Instance.Get<SceneManager>();
            _lobbyManager = GameManager.Instance.Get<LobbyManager>();
        }

        protected override void OnSpawned()
        {
            // If we've missed the OnLobbyStart event, let's invoke it here
            if (_lobbyManager.CurrentLobby.Properties[LobbyService.StartedKey] == true.ToString())
            {
                OnLobbyStart(_lobbyManager.CurrentLobby);
            }

            // We deliberately subscribe after invoking missed events
            _lobbyManager.AddListener(this);
        }

        protected override void OnDespawned()
        {
            _lobbyManager?.RemoveListener(this);
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

            if (!isServer)
            {
                return;
            }

            _ = SpawnPlayerAsync();
        }

        private async Task SpawnPlayerAsync()
        {
            while (!_sceneManager.IsSceneActive(EScene.Game))
            {
                await Task.Yield();
            }

            RaftPlayer player = _networkManager.Spawn(_playerPrefab, NetworkManager.HiddenSpawnPosition);
            player.GiveOwnership(owner);
        }

        public void OnLobbyEnter(Lobby lobby) { }
        public void OnLobbyCreated(Lobby lobby) { }
        public void OnLobbyLeave() { }
    }
}