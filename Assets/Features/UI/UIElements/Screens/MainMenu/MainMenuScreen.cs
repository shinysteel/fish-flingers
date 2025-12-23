using FishFlingers.Networking;
using FishFlingers.States;
using PurrLobby;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class MainMenuScreen : UIElement
    {
        [SerializeField] private Button _browseGamesButton;
        [SerializeField] private Button _hostGameButton;
        [SerializeField] private Button _quitButton;

        private NetworkManager _networkManager;

        private BrowseGamesScreen _browseGamesScreen;

        public void Configure(BrowseGamesScreen browseGamesScreen)
        {
            _browseGamesScreen = browseGamesScreen;
        }

        public override void Load()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();

            _browseGamesButton.onClick.AddListener(BrowseGamesClicked);
            _hostGameButton.onClick.AddListener(HostGameClicked);
            _quitButton.onClick.AddListener(QuitClicked);
        }

        private void BrowseGamesClicked()
        {
            _browseGamesScreen.Show();
        }

        private void HostGameClicked()
        {
            _networkManager.CreateLobby();
        }

        private void QuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}