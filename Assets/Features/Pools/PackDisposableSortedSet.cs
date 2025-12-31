using PurrNet.Modules;
using PurrNet.Packing;
using PurrNet.Pooling;
using System;
using UnityEngine;

namespace FishFlingers.Pools
{
    public static class PackDisposableSortedSet
    {
        [UsedByIL]
        public static void Write<T>(this BitPacker packer, DisposableSortedSet<T> value)
        {
            bool hasValue = !value.IsDisposed && value.Set != null;
            Packer<bool>.Write(packer, hasValue);

            if (!hasValue)
            {
                return;
            }

            int count = value.Count;
            packer.WriteInteger(count, 31);

            foreach (var item in value)
            {
                Packer<T>.Write(packer, item);
            }
        }

        [UsedByIL]
        public static void Read<T>(this BitPacker packer, ref DisposableSortedSet<T> value)
        {
            value.Dispose();
            bool hasValue = default;
            packer.Read(ref hasValue);

            if (!hasValue)
            {
                return;
            }

            long count = default;
            packer.ReadInteger(ref count, 31);
            value = DisposableSortedSet<T>.Create();

            for (int i = 0; i < count; i++)
            {
                T item = default;
                Packer<T>.Read(packer, ref item);
                value.Add(item);
            }
        }

        [UsedByIL]
        public static bool WriteDelta<T>(this BitPacker packer, DisposableSortedSet<T> old, DisposableSortedSet<T> value)
        {
            bool hasChanged;

            if (old.Set == null || value.Set == null)
            {
                hasChanged = old.IsDisposed != value.IsDisposed;
            }
            else
            {
                hasChanged = !old.Set.SetEquals(value.Set);
            }

            Packer<bool>.Write(packer, hasChanged);

            if (hasChanged)
            {
                Write(packer, value);
            }

            return hasChanged;
        }

        [UsedByIL]
        public static void ReadDelta<T>(this BitPacker packer, DisposableSortedSet<T> old, ref DisposableSortedSet<T> value)
        {
            bool hasChanged = default;
            packer.Read(ref hasChanged);

            if (!hasChanged)
            {
                value = old;
                return;
            }

            Read(packer, ref value);
        }
    }
}