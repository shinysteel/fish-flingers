using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ShinyOwl.Framework;

namespace FishFlingers.States
{
    public enum EGameplayState { }

    public class GameplayState : State<EMainState, EGameplayState>
    {
        public GameplayState(StateMachine<EMainState> parent) : base(parent)
        { }
    }
}