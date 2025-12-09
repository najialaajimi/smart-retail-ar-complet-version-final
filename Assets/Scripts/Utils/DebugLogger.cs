using UnityEngine;
using System;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Niveaux de log pour le système de debug
    /// </summary>
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    /// <summary>
    /// Système de logging centralisé pour l'application Smart Retail AR
    /// Permet d'activer/désactiver les logs facilement pour les builds de production
    /// </summary>
    public static class DebugLogger
    {
        private static bool s_EnableLogs = true;
        private static bool s_EnableDebugLogs = true;
        private static bool s_ShowTimestamp = true;
        private static string s_LogPrefix = "[SmartRetailAR]";

        /// <summary>
        /// Active ou désactive tous les logs
        /// </summary>
        public static bool EnableLogs
        {
            get { return s_EnableLogs; }
            set { s_EnableLogs = value; }
        }

        /// <summary>
        /// Active ou désactive les logs de niveau Debug
        /// </summary>
        public static bool EnableDebugLogs
        {
            get { return s_EnableDebugLogs; }
            set { s_EnableDebugLogs = value; }
        }

        /// <summary>
        /// Active ou désactive l'affichage du timestamp dans les logs
        /// </summary>
        public static bool ShowTimestamp
        {
            get { return s_ShowTimestamp; }
            set { s_ShowTimestamp = value; }
        }

        /// <summary>
        /// Log un message d'information
        /// </summary>
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (!s_EnableLogs)
                return;

            if (level == LogLevel.Debug && !s_EnableDebugLogs)
                return;

            string formattedMessage = FormatMessage(message, level);

            switch (level)
            {
                case LogLevel.Info:
                case LogLevel.Debug:
                    Debug.Log(formattedMessage);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage);
                    break;
                case LogLevel.Error:
                    Debug.LogError(formattedMessage);
                    break;
            }
        }

        /// <summary>
        /// Log un message d'erreur
        /// </summary>
        public static void LogError(string message)
        {
            Log(message, LogLevel.Error);
        }

        /// <summary>
        /// Log un message d'avertissement
        /// </summary>
        public static void LogWarning(string message)
        {
            Log(message, LogLevel.Warning);
        }

        /// <summary>
        /// Log un message de debug (désactivable facilement)
        /// </summary>
        public static void LogDebug(string message)
        {
            Log(message, LogLevel.Debug);
        }

        /// <summary>
        /// Log une exception
        /// </summary>
        public static void LogException(Exception exception, string context = "")
        {
            if (!s_EnableLogs)
                return;

            string message = string.IsNullOrEmpty(context)
                ? $"Exception: {exception.Message}\n{exception.StackTrace}"
                : $"Exception dans {context}: {exception.Message}\n{exception.StackTrace}";

            Debug.LogException(exception);
            LogError(message);
        }

        /// <summary>
        /// Formate le message avec prefix et timestamp
        /// </summary>
        private static string FormatMessage(string message, LogLevel level)
        {
            string timestamp = s_ShowTimestamp ? $"[{DateTime.Now:HH:mm:ss}] " : "";
            string levelStr = level == LogLevel.Debug ? "[DEBUG] " : "";
            return $"{timestamp}{s_LogPrefix} {levelStr}{message}";
        }

        /// <summary>
        /// Configure le logger pour les builds de production
        /// </summary>
        public static void ConfigureForProduction()
        {
            s_EnableLogs = false;
            s_EnableDebugLogs = false;
        }

        /// <summary>
        /// Configure le logger pour le développement
        /// </summary>
        public static void ConfigureForDevelopment()
        {
            s_EnableLogs = true;
            s_EnableDebugLogs = true;
            s_ShowTimestamp = true;
        }
    }
}
