using PurrLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Networking
{
    public interface INetworkManagerListener
    {
        void OnLobbySearchResults(List<Lobby> results);
    }

    public class NetworkManager : GameSystem<INetworkManagerListener>
    {
        private NetworkManagerConfig _config;
        private PurrNet.NetworkManager _purrnetNetworkManager;
        private PurrLobby.LobbyManager _purrnetLobbyManager;

        public override void Initialise(GameManagerConfig gameManagerConfig)
        {
            _config = gameManagerConfig.NetworkManagerConfig;

            _purrnetNetworkManager = Object.Instantiate(_config.PurrnetNetworkManagerPrefab);
            _purrnetLobbyManager = Object.Instantiate(_config.PurrentLobbyManagerPrefab);

            _purrnetLobbyManager.OnRoomSearchResults.AddListener(HandleRoomSearchResults);

            base.Initialise(gameManagerConfig);
        }

        public override void Shutdown()
        {
            _purrnetLobbyManager?.OnRoomSearchResults.RemoveListener(HandleRoomSearchResults);

            _purrnetNetworkManager = null;
            _purrnetLobbyManager = null;

            base.Shutdown();
        }

        public void CreateLobby()
        {
            _purrnetLobbyManager.CreateRoom();
        }

        public void SearchLobbies()
        {
            _purrnetLobbyManager.SearchLobbies();
        }

        private void HandleRoomSearchResults(List<Lobby> results)
        {
            Listeners.Dispatch(NotifyOnLobbySearchResults, results);
        }

        private static void NotifyOnLobbySearchResults(INetworkManagerListener listener, List<Lobby> results)
        {
            listener.OnLobbySearchResults(results);
        }
    }
}