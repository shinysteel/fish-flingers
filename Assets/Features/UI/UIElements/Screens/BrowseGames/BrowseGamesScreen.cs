using FishFlingers.Networking;
using PurrLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class BrowseGamesScreen : UIElement, INetworkManagerListener
    {
        [SerializeField] private Button _closeButton;

        private NetworkManager _networkManager;

        public override void Load()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _networkManager.AddListener(this);

            _closeButton.onClick.AddListener(HandleCloseButtonClicked);
        }

        public override void Show()
        {
            base.Show();

            _networkManager.SearchLobbies();
        }

        public override void Unload()
        {
            _networkManager?.RemoveListener(this);
        }

        private void HandleCloseButtonClicked()
        {
            Hide();
        }

        public void OnLobbySearchResults(List<Lobby> results)
        {
        }
    }
}