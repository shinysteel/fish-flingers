using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Common.Framework;
using FishFlingers.UI;
using FishFlingers.Networking;
using PurrLobby;
using ShinyOwl.Common;
using UnityEngine.SceneManagement;
using FishFlingers.Cameras;
using FishFlingers.UI.Transitions;

namespace FishFlingers.States
{
    public enum EMenusState { }

    public class MenusState : State<MainState, EMenusState>, INetworkManagerListener
    {
        private UIManager _uiManager;
        private NetworkManager _networkManager;
        private CameraManager _cameraManager;
        private TransitionManager _transitionManager;

        private MainMenuScreen _mainMenuScreen;
        private BrowseGamesScreen _browseGamesScreen;

        public MenusState(StateMachine<MainState> parent) : base(parent)
        {
            _uiManager = GameManager.Instance.Get<UIManager>();
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _cameraManager = GameManager.Instance.Get<CameraManager>();
            _transitionManager = GameManager.Instance.Get<TransitionManager>();

            _networkManager.AddListener(this);
        }

        ~MenusState()
        {
            _networkManager?.RemoveListener(this);
        }

        public override void Enter()
        {
            _browseGamesScreen = _uiManager.CreateUIElementInLayer(_uiManager.Config.BrowseGamesScreen, UILayer.Screens);

            _mainMenuScreen = _uiManager.CreateUIElementInLayer(_uiManager.Config.MainMenuScreen, UILayer.Screens, UILayerInsertMode.FirstSibling);
            _mainMenuScreen.Configure(_browseGamesScreen);
            _mainMenuScreen.Show(null);

            AsyncOperation op = SceneManager.LoadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentMainMenu), LoadSceneMode.Additive);
            op.completed += _ =>
            {
                SceneManager.SetActiveScene(SceneRegistry.GetScene(EScene.EnvironmentMainMenu));
                _transitionManager.UncoverScreen(null);
            };

            _cameraManager.SetMode(new OrbitCameraMode(Vector3.zero, 5f, 3f, 0.1f));
        }

        public override void Exit()
        {
            _uiManager.DestroyUIElementInLayer(_mainMenuScreen, UILayer.Screens);
            _mainMenuScreen = null;

            _uiManager.DestroyUIElementInLayer(_browseGamesScreen, UILayer.Screens);
            _browseGamesScreen = null;

            SceneManager.UnloadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentMainMenu));
        }

        public void OnLobbyJoined(Lobby lobby)
        {
            if (_parentStateMachine.CurrentState != this)
            {
                return;
            }

            _transitionManager.CoverScreen(() => _parentStateMachine.ChangeState(MainState.Gameplay));
        }

        public void OnLobbySearchResults(List<Lobby> results) { }
    }
}