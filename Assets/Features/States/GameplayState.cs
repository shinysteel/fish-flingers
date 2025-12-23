using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Common.Framework;
using UnityEngine.SceneManagement;

namespace FishFlingers.States
{
    public enum EGameplayState { }

    public class GameplayState : State<MainState, EGameplayState>
    {
        public GameplayState(StateMachine<MainState> parent) : base(parent)
        { }

        public override void Enter()
        {
            SceneManager.LoadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentGameplay), LoadSceneMode.Additive);
        }

        public override void Exit()
        {
            SceneManager.UnloadSceneAsync(SceneRegistry.GetSceneName(EScene.EnvironmentGameplay));
        }
    }
}