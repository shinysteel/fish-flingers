using NUnit.Framework;
using PurrNet.Pooling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Pools
{
    public class SortedSetPool<T> : GenericPool<SortedSet<T>>
    {
#if UNITY_EDITOR
        [ThreadStatic]
#endif
        private static SortedSetPool<T> _instance;

        static SortedSetPool() => _instance = new SortedSetPool<T>();

        private static SortedSet<T> Factory()
        {
            return new SortedSet<T>();
        }

        private static void Reset(SortedSet<T> set)
        {
            set.Clear();
        }

        public SortedSetPool() : base(Factory, Reset) 
        { }

        public static int GetCount()
        {
            return _instance.count;
        }

        public static SortedSet<T> Instantiate()
        {
#if UNITY_EDITOR
            _instance ??= new SortedSetPool<T>();
#endif

            SortedSet<T> set = _instance.Allocate();

#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.Track(set);
#endif
            return set;
        }

        public static void Destroy(SortedSet<T> set)
        {
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.UnTrack(set);
#endif

            _instance.Delete(set);
        }
    }
}