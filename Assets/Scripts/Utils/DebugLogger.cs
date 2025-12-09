using UnityEngine;
using System.Collections.Generic;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Logger de debug avec niveau de log configurable
    /// </summary>
    public class DebugLogger : MonoBehaviour
    {
        public enum LogLevel
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
            Verbose = 4
        }

        private static DebugLogger instance;
        public static DebugLogger Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("DebugLogger");
                    instance = go.AddComponent<DebugLogger>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [SerializeField] private LogLevel currentLogLevel = LogLevel.Info;
        [SerializeField] private bool logToFile = false;
        [SerializeField] private int maxLogEntries = 1000;

        private List<string> logHistory = new List<string>();
        private string logFilePath;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            logFilePath = Application.persistentDataPath + "/smart_retail_ar.log";
            
            if (logToFile)
            {
                System.IO.File.WriteAllText(logFilePath, $"=== Smart Retail AR Log - {System.DateTime.Now} ===\n");
            }
        }

        /// <summary>
        /// Log d'information
        /// </summary>
        public static void LogInfo(string message, Object context = null)
        {
            Instance.Log(message, LogLevel.Info, context);
        }

        /// <summary>
        /// Log de warning
        /// </summary>
        public static void LogWarning(string message, Object context = null)
        {
            Instance.Log(message, LogLevel.Warning, context);
        }

        /// <summary>
        /// Log d'erreur
        /// </summary>
        public static void LogError(string message, Object context = null)
        {
            Instance.Log(message, LogLevel.Error, context);
        }

        /// <summary>
        /// Log verbose (détaillé)
        /// </summary>
        public static void LogVerbose(string message, Object context = null)
        {
            Instance.Log(message, LogLevel.Verbose, context);
        }

        /// <summary>
        /// Log interne
        /// </summary>
        private void Log(string message, LogLevel level, Object context = null)
        {
            if (level > currentLogLevel)
            {
                return;
            }

            string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
            string formattedMessage = $"[{timestamp}] [{level}] {message}";

            // Ajouter à l'historique
            AddToHistory(formattedMessage);

            // Log Unity
            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError(formattedMessage, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(formattedMessage, context);
                    break;
                default:
                    Debug.Log(formattedMessage, context);
                    break;
            }

            // Log fichier
            if (logToFile)
            {
                WriteToFile(formattedMessage);
            }
        }

        /// <summary>
        /// Ajoute un message à l'historique
        /// </summary>
        private void AddToHistory(string message)
        {
            logHistory.Add(message);

            if (logHistory.Count > maxLogEntries)
            {
                logHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// Écrit dans le fichier de log
        /// </summary>
        private void WriteToFile(string message)
        {
            try
            {
                System.IO.File.AppendAllText(logFilePath, message + "\n");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur écriture log: {e.Message}");
            }
        }

        /// <summary>
        /// Récupère l'historique des logs
        /// </summary>
        public List<string> GetLogHistory()
        {
            return new List<string>(logHistory);
        }

        /// <summary>
        /// Efface l'historique
        /// </summary>
        public void ClearHistory()
        {
            logHistory.Clear();
        }

        /// <summary>
        /// Définit le niveau de log
        /// </summary>
        public void SetLogLevel(LogLevel level)
        {
            currentLogLevel = level;
        }

        /// <summary>
        /// Active/désactive le log fichier
        /// </summary>
        public void SetLogToFile(bool enabled)
        {
            logToFile = enabled;
        }
    }
}
