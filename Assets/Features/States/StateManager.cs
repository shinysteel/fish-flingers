using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Common.Framework;
using ShinyOwl.Common;
using FishFlingers.Scenes;

namespace FishFlingers.States
{
    public interface IStateManagerListener
    {
        void OnStateChanged(MainState previous, MainState current);
    }

    public enum MainState
    {
        None,
        Menus,
        Gameplay
    }

    public class StateManager : GameSystem<IStateManagerListener>, ISceneManagerListener
    {
        private StateManagerConfig _config;

        private SceneManager _sceneManager;

        private StateMachine<MainState> _stateMachine;
        private MenusState _menusState;
        private GameplayState _gameplayState;

        public override void Initialise(GameManagerConfig config)
        {
            _config = config.StateManagerConfig;

            _sceneManager = GameManager.Instance.Get<SceneManager>();

            _sceneManager.AddListener(this);

            _stateMachine = new();
            _menusState = new MenusState(_stateMachine);
            _gameplayState = new GameplayState(_stateMachine);

            List<IFishFlingersState> states = new() { _menusState, _gameplayState };
            foreach (IFishFlingersState state in states)
            {
                state.Initialise(_config);
            }

            _stateMachine.AddState(MainState.Menus, _menusState);
            _stateMachine.AddState(MainState.Gameplay, _gameplayState);

            base.Initialise(config);
        }

        public override void Shutdown()
        {
            _sceneManager?.RemoveListener(this);

            base.Shutdown();
        }

        public override void Update()
        {
            _stateMachine.Update();
        }

        public void ChangeState(MainState state)
        {
            MainState previous = _stateMachine.CurrentEnum;
            _stateMachine.ChangeState(state);
            Listeners.Dispatch(NotifyOnStateChanged, previous, state);
        }

        private void NotifyOnStateChanged(IStateManagerListener listener, MainState previous, MainState current)
        {
            listener.OnStateChanged(previous, current);
        }

        public void OnSceneUnloaded(EScene scene)
        { 
            // Only once do we listen for the startup scene to unload before starting the state machine
            if (scene == EScene.Startup)
            {
                _sceneManager.RemoveListener(this);
                _stateMachine.ChangeState(MainState.Menus);
            }
        }

        public void OnSceneLoaded(EScene scene, LoadSceneMode mode) { }
        public void OnSceneSetActive(EScene previous, EScene current) { }
    }
}