using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Framework;
using FishFlingers.UI;

namespace FishFlingers.States
{
    public enum EMenusState { }

    public class MenusState : State<EMainState, EMenusState>
    {
        private UIManager _uiManager;

        private MainMenuScreen _mainMenuScreen;
        private BrowseGamesScreen _browseGamesScreen;

        public MenusState(StateMachine<EMainState> parent) : base(parent)
        {
            _uiManager = GameManager.Instance.Get<UIManager>();
        }

        public override void Enter()
        {
            _browseGamesScreen = _uiManager.CreateUIElementInLayer(_uiManager.Config.BrowseGamesScreen, UILayer.Screens);

            _mainMenuScreen = _uiManager.CreateUIElementInLayer(_uiManager.Config.MainMenuScreen, UILayer.Screens, UILayerInsertMode.FirstSibling);
            _mainMenuScreen.Configure(_browseGamesScreen);
            _mainMenuScreen.Show();
        }

        public override void Exit()
        {
            _uiManager.DestroyUIElementInLayer(_mainMenuScreen, UILayer.Screens);
            _mainMenuScreen = null;

            _uiManager.DestroyUIElementInLayer(_browseGamesScreen, UILayer.Screens);
            _browseGamesScreen = null;
        }
    }
}