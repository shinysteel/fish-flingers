using UnityEngine;
using UnityEngine.Assertions;

namespace ShinyOwl.Common
{
    public static class Log
    {
        public static void Info(object context, object message) => Debug.Log(Format(context, message?.ToString()));
        public static void Warning(object context, object message) => Debug.LogWarning(Format(context, message?.ToString()));
        public static void Error(object context, object message) => Debug.LogError(Format(context, message?.ToString()));

        private static string Format(object context, string message)
        {
            return $"[{context?.GetType().Name}] {message}";
        }
    }
}