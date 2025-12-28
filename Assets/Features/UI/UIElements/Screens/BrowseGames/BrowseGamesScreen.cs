using FishFlingers.Networking;
using ShinyOwl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BrowseGamesScreen : UIElement
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private LobbyEntry _lobbyEntryPrefab;

        [SerializeField] private Transform _lanContainer;
        [SerializeField] private Transform _steamContainer;

        private NetworkManager _networkManager;

        private Dictionary<eLobbyService, LobbyEntryContainer> _entryContainers = new();

        private float _searchTimer;

        private const float SearchInterval = 2.5f;

        private class LobbyEntryContainer
        {
            public Transform Container { get; private set; }
            public List<LobbyEntry> Entries { get; private set; }

            public LobbyEntryContainer(Transform container)
            {
                Container = container;
                Entries = new();
            }
        }

        public override void Load()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _entryContainers.Add(eLobbyService.LAN, new LobbyEntryContainer(_lanContainer));
            _entryContainers.Add(eLobbyService.Steam, new LobbyEntryContainer(_steamContainer));

            _closeButton.onClick.AddListener(CloseClicked);
        }

        public override void Show(Action onComplete)
        {
            base.Show(onComplete);

            _ = SearchAsync();
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
            if (_searchTimer < SearchInterval)
            {
                return;
            }

            _ = SearchAsync();
        }

        // Use pooling once we allow the scroll rect to display only what is on screen
        private async Task SearchAsync()
        {
            _searchTimer = 0f;

            Dictionary<eLobbyService, Lobby[]> lobbies = await _networkManager.SearchLobbies();

            foreach (eLobbyService service in Enum.GetValues(typeof(eLobbyService)))
            {
                SyncLobbyEntries(_entryContainers[service], lobbies[service]);
            }
        }

        private void SyncLobbyEntries(LobbyEntryContainer container, Lobby[] lobbies)
        {
            for (int i = container.Entries.Count; i < lobbies.Length; i++)
            {
                container.Entries.Add(Instantiate(_lobbyEntryPrefab, container.Container));
            }

            for (int i = 0; i < lobbies.Length; i++)
            {
                container.Entries[i].Setup(lobbies[i]);
            }

            for (int i = container.Entries.Count - 1; i >= lobbies.Length; i--)
            {
                container.Entries.RemoveAt(i);
            }
        }

        private void CloseClicked()
        {
            Hide(null);
        }
    }
}