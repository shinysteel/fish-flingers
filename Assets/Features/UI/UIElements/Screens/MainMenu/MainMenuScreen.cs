using FishFlingers.Networking;
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
        private UIManager _uiManager;

        private BrowseGamesScreen _browseGamesScreen;

        public void Configure(BrowseGamesScreen browseGamesScreen)
        {
            _browseGamesScreen = browseGamesScreen;
        }

        public override void Load()
        {
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _uiManager = GameManager.Instance.Get<UIManager>();

            _browseGamesButton.onClick.AddListener(HandleBrowseGamesButtonClicked);
            _hostGameButton.onClick.AddListener(HandleHostGameButtonClicked);
            _quitButton.onClick.AddListener(HandleQuitButtonClicked);
        }

        private void HandleBrowseGamesButtonClicked()
        {
            _browseGamesScreen.Show();
        }

        private void HandleHostGameButtonClicked()
        {
            _networkManager.CreateLobby();
        }

        private void HandleQuitButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}