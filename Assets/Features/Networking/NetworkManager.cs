using PurrLobby;
using ShinyOwl.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Networking
{
    public interface INetworkManagerListener
    {
        void OnLobbySearchResults(List<Lobby> results);
        void OnLobbyJoined(Lobby lobby);
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
            _purrnetLobbyManager.OnRoomJoined.AddListener(HandleRoomJoined);

            base.Initialise(gameManagerConfig);
        }

        public override void Shutdown()
        {
            _purrnetLobbyManager?.OnRoomSearchResults.RemoveListener(HandleRoomSearchResults);
            _purrnetLobbyManager?.OnRoomJoined.RemoveListener(HandleRoomJoined);

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

        private void HandleRoomSearchResults(List<Lobby> results) => Listeners.Dispatch(NotifyOnLobbySearchResults, results);
        private void HandleRoomJoined(Lobby lobby) => Listeners.Dispatch(NotifyOnLobbyJoined, lobby);

        private static void NotifyOnLobbySearchResults(INetworkManagerListener listener, List<Lobby> results) => listener.OnLobbySearchResults(results);
        private static void NotifyOnLobbyJoined(INetworkManagerListener listener, Lobby lobby) => listener.OnLobbyJoined(lobby);
    }
}