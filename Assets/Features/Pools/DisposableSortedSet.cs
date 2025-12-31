using PurrNet.Modules;
using PurrNet.Packing;
using PurrNet.Pooling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FishFlingers.Pools
{
    // Enforcing T : IComparable allows sorted sets to be deterministic
    public struct DisposableSortedSet<T> : ISet<T>, IDisposable, IDuplicate<DisposableSortedSet<T>>
    {
        private bool _isAllocated;
        private bool _shouldDispose;

        public SortedSet<T> Set;

        public bool IsDisposed => !_isAllocated;

        public T Min
        {
            get
            {
                if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
                NotifyUsage();
                return Set.Min;
            }
        }

        public T Max
        {
            get
            {
                if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
                NotifyUsage();
                return Set.Max;
            }
        }

        public int Count
        {
            get
            {
                if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
                NotifyUsage();
                return Set.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
                NotifyUsage();
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NotifyUsage()
        {
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            AllocationTracker.UpdateUsage(Set);   
#endif
        }

        public static DisposableSortedSet<T> Create()
        {
            DisposableSortedSet<T> set = new();

            set.Set = SortedSetPool<T>.Instantiate();
            set._isAllocated = true;
            set._shouldDispose = true;

            return set;
        }

        public static DisposableSortedSet<T> Create(ISet<T> copyFrom)
        {
            DisposableSortedSet<T> set = new();

            set.Set = SortedSetPool<T>.Instantiate();
            set._isAllocated = true;
            set._shouldDispose = true;

            if (copyFrom != null)
            {
                set.UnionWith(copyFrom);
            }

            return set;
        }

        public static DisposableSortedSet<T> Create(IEnumerable<T> copyFrom)
        {
            DisposableSortedSet<T> set = new();

            set.Set = SortedSetPool<T>.Instantiate();
            set._isAllocated = true;
            set._shouldDispose = true;

            if (copyFrom != null)
            {
                set.UnionWith(copyFrom);
            }

            return set;
        }

        public DisposableSortedSet<T> Duplicate()
        {
            if (!_isAllocated)
            {
                return default;
            }

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                DisposableSortedSet<T> deepClone = Create();

                foreach (T item in Set)
                {
                    deepClone.Add(Packer.Copy(item));
                }

                return deepClone;
            }

            return Create(this);
        }

        public void Dispose()
        {
            if (!_isAllocated)
            {
                return;
            }

            if (_shouldDispose && Set != null)
            {
                SortedSetPool<T>.Destroy(Set);
            }

            _isAllocated = false;
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public bool Add(T item)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.Add(item);
        }

        public bool Remove(T item)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.Remove(item);
        }

        public void Clear()
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.Clear();
        }

        public bool Contains(T item)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.ExceptWith(other);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return Set.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            Set.UnionWith(other);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_isAllocated) throw new ObjectDisposedException(nameof(DisposableSortedSet<T>));
            NotifyUsage();
            return GetEnumerator();
        }
    }
}