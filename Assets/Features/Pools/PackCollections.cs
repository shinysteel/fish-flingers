using PurrNet.Modules;
using PurrNet.Packing;
using System.Collections.Generic;
using UnityEngine;

namespace FishFlingers.Pools
{
    public static class PackCollections
    {
        [UsedByIL]
        public static void RegisterDisposableSortedSet<T>()
        {
            Packer<DisposableSortedSet<T>>.RegisterWriter(PackDisposableSortedSet.Write);
            Packer<DisposableSortedSet<T>>.RegisterReader(PackDisposableSortedSet.Read);

            DeltaPacker<DisposableSortedSet<T>>.RegisterWriter(PackDisposableSortedSet.WriteDelta);
            DeltaPacker<DisposableSortedSet<T>>.RegisterReader(PackDisposableSortedSet.ReadDelta);

            PurrNet.Packing.PackCollections.RegisterDisposableList<T>();
        }

        [UsedByIL]
        public static void RegisterSortedSet<T>()
        {
            Packer<SortedSet<T>>.RegisterWriter(WriteSortedSet);
            Packer<SortedSet<T>>.RegisterReader(ReadSortedSet);

            PurrNet.Packing.PackCollections.RegisterDisposableList<T>();
        }

        [UsedByIL]
        private static void WriteSortedSet<T>(BitPacker packer, SortedSet<T> value)
        {
            if (value == null)
            {
                Packer<bool>.Write(packer, false);
                return;
            }

            Packer<bool>.Write(packer, true);
            Packer<int>.Write(packer, value.Count);

            foreach (T item in value)
            {
                Packer<T>.Write(packer, item);
            }
        }

        [UsedByIL]
        private static void ReadSortedSet<T>(BitPacker packer, ref SortedSet<T> value)
        {
            bool hasValue = false;
            packer.Read(ref hasValue);

            if (!hasValue)
            {
                value = null;
                return;
            }

            int count = 0;
            Packer<int>.Read(packer, ref count);
            value = new SortedSet<T>();

            for (int i = 0; i < count; i++)
            {
                T item = default;
                Packer<T>.Read(packer, ref item);
                value.Add(item);
            }
        }
    }
}