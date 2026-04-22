using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShinyOwl.Common
{
    [Serializable]
    public class WeightedPick<T>
    {
        [SerializeField] private T _value;
        [SerializeField] private int _count;

        public T Value => _value;
        public int Count => _count;

        public WeightedPick(T value, int count)
        {
            _value = value;
            _count = count;
        }
    }

    [Serializable]
    public class WeightedEntry<T>
    {
        [SerializeField] private T _value;
        [SerializeField] private float _weight = 1f;
        [SerializeField] private int _minCount = 1;
        [SerializeField] private int _maxCount = 1;

        public T Value => _value;
        public float Weight => _weight;
        public int MinCount => _minCount;
        public int MaxCount => _maxCount;
    }

    public class WeightedPicker<T>
    {
        private List<WeightedEntry<T>> _entries = new();

        public void Set(WeightedEntry<T>[] entries)
        {
            Clear();
            _entries.AddRange(entries);
        }

        public void Clear()
        {
            _entries.Clear();
        }
        
        public WeightedPick<T> Pick()
        {
            float total = 0f;

            foreach (WeightedEntry<T> item in _entries)
            {
                total += item.Weight;
            }

            float random = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (WeightedEntry<T> item in _entries)
            {
                cumulative += item.Weight;

                if (random < cumulative)
                {
                    return new WeightedPick<T>(item.Value, Random.Range(item.MinCount, item.MaxCount + 1));
                }
            }

            return default;
        }
    }
}