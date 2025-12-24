using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Common.Framework;
using UnityEngine.SceneManagement;
using FishFlingers.UI.Transitions;

namespace FishFlingers.States
{
    public enum EGameplayState { }

    public class GameplayState : State<MainState, EGameplayState>
    {
        private TransitionManager _transitionManager;

        public GameplayState(StateMachine<MainState> parent) : base(parent)
        {
            _transitionManager = GameManager.Instance.Get<TransitionManager>();
        }

        public override void Enter()
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentGameplay), LoadSceneMode.Additive);
            op.completed += _ =>
            {
                SceneManager.SetActiveScene(SceneRegistry.GetScene(EScene.EnvironmentGameplay));
                _transitionManager.UncoverScreen(null);
            };
        }

        public override void Exit()
        {
            SceneManager.UnloadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentGameplay));
        }
    }
}