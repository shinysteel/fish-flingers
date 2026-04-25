using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;

namespace ShinyOwl.Common
{
    public class StateAnimationEvent : IComparable<StateAnimationEvent>
    {
        private float _normalisedTime;
        private Action _method;

        public float NormalisedTime => _normalisedTime;
        public Action Method => _method;

        public StateAnimationEvent(float normalisedTime, Action method)
        {
            SetNormalisedTime(normalisedTime);
            _method = method;
        }

        public void SetNormalisedTime(float time)
        {
            _normalisedTime = Mathf.Clamp(time, 0f, 1f);
        }

        public int CompareTo(StateAnimationEvent other)
        {
            return _normalisedTime.CompareTo(other._normalisedTime);
        }
    }

    public class StateAnimationEvents : IEnumerable<StateAnimationEvent>
    {
        private string _stateName;

        private List<StateAnimationEvent> _events = new();

        private int _index;

        public StateAnimationEvents(string stateName)
        {
            _stateName = stateName;
        }

        // Implementing IEnumerator allows us to initialise the class with values
        public IEnumerator<StateAnimationEvent> GetEnumerator()
        {
            return _events.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(StateAnimationEvent animationEvent)
        {
            _events.Add(animationEvent);

            // Ensure the collection is in ascending order
            _events.Sort();
        }

        public void Tick(AnimatorStateInfo info)
        {
            if (!info.IsName(_stateName))
            {
                _index = 0;
                return;
            }

            // Executes events once their normalised time has exceeded
            while (_index < _events.Count && _events[_index].NormalisedTime <= info.normalizedTime)
            {
                _events[_index].Method?.Invoke();
                _index++;
            }
        }
    }
}