using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShinyOwl.Framework
{
    public class ListenerHandler<TListener>
    {
        private List<TListener> _listeners = new();

        public void AddListener(TListener listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(TListener listener)
        {
            _listeners.Remove(listener);
        }

        private void ForEachListener(Action<TListener> call)
        {
            foreach (TListener listener in _listeners)
            {
                call(listener);
            }
        }

        public void Dispatch(Action<TListener> call) => ForEachListener(call);
        public void Dispatch<T1>(Action<TListener, T1> call, T1 arg1) => ForEachListener(listener => call(listener, arg1));
        public void Dispatch<T1, T2>(Action<TListener, T1, T2> call, T1 arg1, T2 arg2) => ForEachListener(listener => call(listener, arg1, arg2));
        public void Dispatch<T1, T2, T3>(Action<TListener, T1, T2, T3> call, T1 arg1, T2 arg2, T3 arg3) => ForEachListener(listener => call(listener, arg1, arg2, arg3));
    }
}