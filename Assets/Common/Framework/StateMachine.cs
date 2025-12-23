using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ShinyOwl.Framework
{
    /// <summary>
    /// Goals:
    /// 1. We achieve a HFSM where states can implement sub state machines
    /// 2. States are aware of their 'parent', and can request to change to their related sub states
    /// 3. The state machine is generic and does not require extra top-level 'state' classes
    /// 4. The systems needs to be safe and not allow state machines change to states outside their scope
    /// </summary>

    public interface IState
    {
        void Enter();
        void Update();
        void Exit();
    }

    public class State<TParent, TSub> : IState
        where TParent : Enum
        where TSub    : Enum
    {
        protected StateMachine<TParent> _parent;
        protected StateMachine<TSub> _sub;

        public State(StateMachine<TParent> parent)
        {
            _parent = parent;
        }

        public virtual void Enter()
        {
            _sub?.Enter();
        }

        public virtual void Update()
        {
            _sub?.Update();
        }

        public virtual void Exit()
        {
            _sub?.Exit();
        }

        protected void ChangeState(TSub newStateEnum)
        {
            _sub?.ChangeState(newStateEnum);   
        }
    }

    public class StateMachine<TState> 
        where TState : Enum
    {
        private Dictionary<TState, IState> _enumStateMap = new();
        private IState _currentState;

        public StateMachine()
        {
            // Start off every enum with null. Allows us to skip assigning null to Enum.None
            foreach (TState stateEnum in Enum.GetValues(typeof(TState)).Cast<TState>())
            {
                _enumStateMap.Add(stateEnum, null);
            }
        }

        public void AddState(TState stateEnum, IState state)
        {
            _enumStateMap[stateEnum] = state;
        }

        public void ChangeState(TState stateEnum)
        {
            if (!_enumStateMap.TryGetValue(stateEnum, out IState state))
            {
                return;
            }

            if (_currentState == state)
            {
                return;
            }

            _currentState?.Exit();
            _currentState = state;
            _currentState?.Enter();
        }

        public void Enter()
        {
            _currentState?.Enter();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Exit()
        {
            _currentState?.Exit();
        }
    }
}