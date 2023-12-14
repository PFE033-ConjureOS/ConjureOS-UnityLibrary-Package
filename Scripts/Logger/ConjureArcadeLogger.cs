using UnityEngine;

namespace ConjureOS.Logger
{
    public static class ConjureArcadeLogger
    {
        private const string header = "ConjureOS: ";
        public static void Log(string message)
        {
            Debug.Log($"{header}{message}");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($"{header}{message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"{header}{message}");
        }
    }

    public enum LogMessageType
    {
        Log,
        Warning,
        Error
    }
}