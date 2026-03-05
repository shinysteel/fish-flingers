using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ShinyOwl.Common.Utils
{
    public static partial class Utils
    {
        public static class Collections
        {
            public static void ResizeList<T>(List<T> list, int newLength, Func<T> createElement, Action<T> removeElement, Action<T, int> processElement)
            {
                for (int i = list.Count; i < newLength; i++)
                {
                    list.Add(createElement());
                }

                for (int i = list.Count - 1; i >= newLength; i--)
                {
                    removeElement(list[i]);
                    list.RemoveAt(i);
                }

                for (int i = 0; i < newLength; i++)
                {
                    processElement(list[i], i);
                }
            }
        }
    }
}