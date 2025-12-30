using ShinyOwl.Common.Framework;
using System;
using UnityEngine;

namespace FishFlingers.States
{
    public interface IFishFlingersState
    {
        void Initialise(StateManagerConfig config);
    }

    public abstract class FishFlingersState<TParentStateEnum, TSubStateEnum> : State<TParentStateEnum, TSubStateEnum>, IFishFlingersState
        where TParentStateEnum : Enum
        where TSubStateEnum : Enum
    {
        public FishFlingersState(StateMachine<TParentStateEnum> parent) : base(parent) 
        { }

        public virtual void Initialise(StateManagerConfig config)
        { }
    }
}