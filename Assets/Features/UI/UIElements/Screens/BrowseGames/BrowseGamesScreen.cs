using FishFlingers.Networking;
using PurrLobby;
using ShinyOwl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BrowseGamesScreen : UIElement, INetworkManagerListener
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _lobbyEntryContainer;
        [SerializeField] private LobbyEntry _lobbyEntryPrefab;

        private NetworkManager _networkManager;

        private List<LobbyEntry> _lobbyEntries = new();
        private float _searchInterval = 3f;
        private float _searchTimer;

        public override void Load()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _networkManager.AddListener(this);

            _closeButton.onClick.AddListener(CloseClicked);
        }

        public override void Show(Action onComplete)
        {
            base.Show(onComplete);

            Search();
        }

        public override void Unload()
        {
            _networkManager?.RemoveListener(this);
        }

        private void Update()
        {
            AutoSearchUpdate();
        }

        private void AutoSearchUpdate()
        {
            if (!_isVisible)
            {
                return;
            }

            _searchTimer += Time.deltaTime;
            if (_searchTimer < _searchInterval)
            {
                return;
            }

            Search();
        }

        private void Search()
        {
            _networkManager.SearchLobbies();
            _searchTimer = 0f;
        }

        private void CloseClicked()
        {
            Hide(null);
        }

        // Use pooling once we allow the scroll rect to display only what is on screen
        public void OnLobbySearchResults(List<Lobby> results)
        {
            if (!_isVisible)
            {
                return;
            }

            for (int i = _lobbyEntries.Count; i < results.Count; i++)
            {
                _lobbyEntries.Add(Instantiate(_lobbyEntryPrefab, _lobbyEntryContainer));
            }

            for (int i = 0; i < results.Count; i++)
            {
                _lobbyEntries[i].Setup(results[i]);
            }

            for (int i = _lobbyEntries.Count - 1; i >= results.Count; i--)
            {
                _lobbyEntries.RemoveAt(i);
            }
        }

        public void OnLobbyJoined(Lobby lobby) { }
    }
}