using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Framework;
using UnityEngine.SceneManagement;

namespace FishFlingers.States
{
    public interface IStateManagerListener
    { }

    public enum EMainState
    {
        None,
        Menus,
        Gameplay
    }

    public class StateManager : GameSystem<IStateManagerListener>
    {
        private StateManagerConfig _config;

        private StateMachine<EMainState> _stateMachine;
        private MenusState _menusState;
        private GameplayState _gameplayState;

        public override void Initialise(GameManagerConfig gameManagerConfig)
        {
            _config = gameManagerConfig.StateManagerConfig;

            _stateMachine = new();
            _menusState = new MenusState(_stateMachine);
            _gameplayState = new GameplayState(_stateMachine);

            _stateMachine.AddState(EMainState.Menus, _menusState);
            _stateMachine.AddState(EMainState.Gameplay, _gameplayState);

            base.Initialise(gameManagerConfig);

            SceneManager.sceneUnloaded += HandleSceneUnloaded;
            void HandleSceneUnloaded(Scene scene)
            {
                if (scene.name != SceneRegistry.GetSceneName(EScene.Startup))
                {
                    return;
                }

                _stateMachine.ChangeState(EMainState.Menus);
                SceneManager.sceneUnloaded -= HandleSceneUnloaded;
            }
        }

        public override void Update()
        {
            _stateMachine.Update();
        }
    }
}