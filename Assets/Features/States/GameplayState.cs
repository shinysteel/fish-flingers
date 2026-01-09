using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Common.Framework;
using FishFlingers.UI.Transitions;
using FishFlingers.UI;
using FishFlingers.Networking;
using System;
using Steamworks;
using ShinyOwl.Common;
using FishFlingers.Scenes;
using System.Threading.Tasks;
using PurrNet.Transports;
using PurrNet;
using FishFlingers.Environments;
using FishFlingers.Entities;

using NetworkManager = FishFlingers.Networking.NetworkManager;

namespace FishFlingers.States
{
    public class GameplayState : MainState<EMainState, ENone>, ILobbyManagerListener, INetworkManagerListener
    {
        private TransitionManager _transitionManager;
        private UIManager _uiManager;
        private StateManager _stateManager;
        private NetworkManager _networkManager;
        private SceneManager _sceneManager;
        private LobbyManager _lobbyManager;

        private GameplayStateConfig _config;

        private GameplayScreen _gameplayScreen;

        public GameplayState(StateMachine<EMainState> parent) : base(parent)
        {
            _transitionManager = GameManager.Instance.Get<TransitionManager>();
            _uiManager = GameManager.Instance.Get<UIManager>();
            _stateManager = GameManager.Instance.Get<StateManager>();
            _networkManager = GameManager.Instance.Get<NetworkManager>();
            _sceneManager = GameManager.Instance.Get<SceneManager>();
            _lobbyManager = GameManager.Instance.Get<LobbyManager>();

            _networkManager.AddListener(this);
            _lobbyManager.AddListener(this);
        }

        ~GameplayState()
        {
            _networkManager?.RemoveListener(this);
            _lobbyManager?.RemoveListener(this);
        }

        public override void Initialise(StateManagerConfig config)
        {
            _config = config.GameplayStateConfig;
        }

        public override async Task EnterAsync()
        {
            try
            {
                if (_networkManager.IsServer)
                {
                    // Network the game scene
                    await _sceneManager.LoadSceneAsync(EScene.Game, LoadSceneMode.Single, LoadSceneContext.Networked);
                }
                else
                {
                    // Scenes are structs, so we need to keep requesting while awaiting
                    while (!_sceneManager.IsSceneActive(EScene.Game))
                    {
                        await Task.Yield();
                    }
                }

                await _sceneManager.LoadSceneAsync(EScene.EnvironmentGameplay, LoadSceneMode.Additive, LoadSceneContext.Local);

                // Only the server creates the raft
                if (_networkManager.IsServer)
                {
                    Raft raft = _networkManager.Spawn(_config.RaftPrefab);
                    WaveSpawner waveSpawner = _networkManager.Spawn(_config.WaveSpawnerPrefab);
                    SalvageSpawner salvageSpawner = _networkManager.Spawn(_config.SalvageSpawnerPrefab);

                    waveSpawner.Initialise(raft);
                    salvageSpawner.Initialise(raft);
                }

                _gameplayScreen = (GameplayScreen)await _uiManager.CreateUIElementAsync(_uiManager.Config.GameplayScreen, UILayer.Screens);
                _gameplayScreen.Show(null);

                _transitionManager.UncoverScreen(null);
            }
            catch (Exception ex)
            {
                Debugger.Log(this, ex);
            }
        }

        public override void Exit()
        {
            _uiManager.DestroyUIElement(_gameplayScreen, UILayer.Screens);
            _gameplayScreen = null;

            _lobbyManager.LeaveLobby();

            _sceneManager.LoadSceneAsync(EScene.Default, LoadSceneMode.Single, LoadSceneContext.Local);
        }

        public void OnLobbyEnter(Lobby lobby)
        {
            // This can happen from any state besides itself. Currently we 
            // assume you are 'ready' straight away and move to the GameplayState
            if (_parentStateMachine.CurrentEnum == EMainState.Gameplay)
            {
                return;
            }

            // Currently we have no lobby flow, and just start the lobby as soon as we create it
            if (_lobbyManager.IsLobbyOwner(lobby))
            {
                _lobbyManager.StartLobby();
            }
        }

        public void OnLobbyStart(Lobby lobby) 
        {
            if (_parentStateMachine.CurrentEnum == EMainState.Gameplay)
            {
                return;
            }

            _transitionManager.CoverScreen(() => _stateManager.ChangeState(EMainState.Gameplay));
        }

        public void OnNetworkShutdown(bool asServer)
        {
            if (_parentStateMachine.CurrentEnum != EMainState.Gameplay)
            {
                return;
            }

            // This will get called twice on the server, as they act as both the server and a client
            if (asServer)
            {
                return;
            }

            _transitionManager.CoverScreen(() => _stateManager.ChangeState(EMainState.Menus));
        }

        public void OnLobbyLeave()  { }
        public void OnLobbyCreated(Lobby lobby) { }
        public void OnPlayerJoined(PlayerID id, bool isReconnect, bool asServer) { }
        public void OnPlayerLeft(PlayerID id, bool asServer) { }
        public void OnClientConnectionState(ConnectionState state) { }
        public void OnNetworkStarted(bool asServer) { }
        public void OnNetworkSpawn() { }
        public void OnNetworkDespawn() { }
    }
}