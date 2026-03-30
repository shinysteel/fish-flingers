using FishFlingers.Networking;
using FishFlingers.States;
using ShinyOwl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FishFlingers.UI
{
    public class MainMenuScreen : ScreenUI
    {
        [SerializeField] private Button _browseGamesButton;
        [SerializeField] private Button _hostGameButton;
        [SerializeField] private Button _quitButton;

        private LobbyManager _lobbyManager;
        private UIManager _uiManager;

        private BrowseGamesPanel _browseGamesPanel;
        private HostGamePanel _hostGamePanel;
        private bool _creatingBrowseGames;
        private bool _creatingHostGame;

        public override void Load(Canvas canvas)
        {
            base.Load(canvas);

            _lobbyManager = GameManager.Instance.Get<LobbyManager>();
            _uiManager = GameManager.Instance.Get<UIManager>();

            _browseGamesButton.onClick.AddListener(BrowseGamesPressed);
            _hostGameButton.onClick.AddListener(HostGamePressed);
            _quitButton.onClick.AddListener(QuitPressed);
        }

        private void BrowseGamesPressed()
        {
            if (_browseGamesPanel != null)
            {
                _browseGamesPanel.SimulateClosePressed();
                return;
            }

            if (_uiManager.IsLayerInUse(UILayer.Panels))
            {
                return;
            }

            if (!_creatingBrowseGames)
            {
                _creatingBrowseGames = true;

                _uiManager.CreateScreenUIAsync(_uiManager.Config.BrowseGamesPanelPrefab, UILayer.Panels).completed += (BrowseGamesPanel panel) =>
                {
                    _browseGamesPanel = panel;
                    _browseGamesPanel.Show(null);

                    _creatingBrowseGames = false;
                };
            }
        }

        private void HostGamePressed()
        {
            if (_hostGamePanel != null)
            {
                _hostGamePanel.SimulateClosePressed();
                return;
            }

            if (_uiManager.IsLayerInUse(UILayer.Panels))
            {
                return;
            }

            if (!_creatingHostGame)
            {
                _creatingHostGame = true;

                _uiManager.CreateScreenUIAsync(_uiManager.Config.HostGamePanelPrefab, UILayer.Panels).completed += (HostGamePanel panel) =>
                {
                    _hostGamePanel = panel;
                    _hostGamePanel.Show(null);

                    _creatingHostGame = false;
                };
            }
        }

        private void QuitPressed()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}