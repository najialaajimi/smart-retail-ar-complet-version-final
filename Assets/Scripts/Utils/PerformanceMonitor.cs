using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Moniteur de performance pour mesurer les KPIs
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        private static PerformanceMonitor instance;
        public static PerformanceMonitor Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("PerformanceMonitor");
                    instance = go.AddComponent<PerformanceMonitor>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private float updateInterval = 1f;

        [Header("KPI Targets")]
        [Tooltip("Latence maximale d'affichage en secondes")]
        [SerializeField] private float targetDisplayLatency = 1f;

        [Tooltip("Taux de reconnaissance minimum (%)")]
        [SerializeField] private float targetRecognitionRate = 95f;

        [Header("Statistics")]
        [SerializeField] private float currentFPS;
        [SerializeField] private float averageFrameTime;
        [SerializeField] private int totalScans;
        [SerializeField] private int successfulScans;
        [SerializeField] private float recognitionRate;

        private Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();
        private Dictionary<string, List<float>> metrics = new Dictionary<string, List<float>>();
        private float lastUpdateTime;
        private float deltaTime;

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
            }
        }

        private void Update()
        {
            if (!enableMonitoring)
            {
                return;
            }

            // Calcul FPS
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            currentFPS = 1.0f / deltaTime;
            averageFrameTime = deltaTime * 1000f;

            // Mise à jour périodique
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                lastUpdateTime = Time.time;
                UpdateStatistics();
            }
        }

        /// <summary>
        /// Démarre un chronomètre pour mesurer une opération
        /// </summary>
        public void StartTimer(string operationName)
        {
            if (!timers.ContainsKey(operationName))
            {
                timers[operationName] = new Stopwatch();
            }

            timers[operationName].Restart();
        }

        /// <summary>
        /// Arrête un chronomètre et enregistre le temps
        /// </summary>
        public float StopTimer(string operationName)
        {
            if (!timers.ContainsKey(operationName))
            {
                UnityEngine.Debug.LogWarning($"Timer '{operationName}' n'existe pas");
                return 0f;
            }

            timers[operationName].Stop();
            float elapsedSeconds = timers[operationName].ElapsedMilliseconds / 1000f;

            RecordMetric(operationName, elapsedSeconds);

            return elapsedSeconds;
        }

        /// <summary>
        /// Enregistre une métrique
        /// </summary>
        public void RecordMetric(string metricName, float value)
        {
            if (!metrics.ContainsKey(metricName))
            {
                metrics[metricName] = new List<float>();
            }

            metrics[metricName].Add(value);

            // Limiter la taille de l'historique
            if (metrics[metricName].Count > 1000)
            {
                metrics[metricName].RemoveAt(0);
            }
        }

        /// <summary>
        /// Enregistre un scan réussi
        /// </summary>
        public void RecordSuccessfulScan()
        {
            totalScans++;
            successfulScans++;
            UpdateRecognitionRate();
        }

        /// <summary>
        /// Enregistre un scan échoué
        /// </summary>
        public void RecordFailedScan()
        {
            totalScans++;
            UpdateRecognitionRate();
        }

        /// <summary>
        /// Met à jour le taux de reconnaissance
        /// </summary>
        private void UpdateRecognitionRate()
        {
            if (totalScans > 0)
            {
                recognitionRate = (float)successfulScans / totalScans * 100f;
            }
        }

        /// <summary>
        /// Met à jour les statistiques
        /// </summary>
        private void UpdateStatistics()
        {
            // Vérifier si les KPIs sont atteints
            bool latencyMet = GetAverageMetric("DisplayLatency") <= targetDisplayLatency;
            bool recognitionMet = recognitionRate >= targetRecognitionRate;

            if (enableMonitoring)
            {
                UnityEngine.Debug.Log($"[Performance] FPS: {currentFPS:F1} | Recognition: {recognitionRate:F1}% | Latency Met: {latencyMet}");
            }
        }

        /// <summary>
        /// Récupère la moyenne d'une métrique
        /// </summary>
        public float GetAverageMetric(string metricName)
        {
            if (!metrics.ContainsKey(metricName) || metrics[metricName].Count == 0)
            {
                return 0f;
            }

            float sum = 0f;
            foreach (float value in metrics[metricName])
            {
                sum += value;
            }

            return sum / metrics[metricName].Count;
        }

        /// <summary>
        /// Récupère le minimum d'une métrique
        /// </summary>
        public float GetMinMetric(string metricName)
        {
            if (!metrics.ContainsKey(metricName) || metrics[metricName].Count == 0)
            {
                return 0f;
            }

            float min = float.MaxValue;
            foreach (float value in metrics[metricName])
            {
                if (value < min)
                {
                    min = value;
                }
            }

            return min;
        }

        /// <summary>
        /// Récupère le maximum d'une métrique
        /// </summary>
        public float GetMaxMetric(string metricName)
        {
            if (!metrics.ContainsKey(metricName) || metrics[metricName].Count == 0)
            {
                return 0f;
            }

            float max = float.MinValue;
            foreach (float value in metrics[metricName])
            {
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        /// <summary>
        /// Génère un rapport de performance
        /// </summary>
        public string GeneratePerformanceReport()
        {
            string report = "=== Performance Report ===\n";
            report += $"FPS: {currentFPS:F1}\n";
            report += $"Average Frame Time: {averageFrameTime:F2}ms\n";
            report += $"Total Scans: {totalScans}\n";
            report += $"Successful Scans: {successfulScans}\n";
            report += $"Recognition Rate: {recognitionRate:F1}%\n";
            report += $"Target Recognition Rate: {targetRecognitionRate}%\n";
            report += $"Recognition Rate Met: {recognitionRate >= targetRecognitionRate}\n\n";

            foreach (var metric in metrics)
            {
                report += $"{metric.Key}:\n";
                report += $"  Average: {GetAverageMetric(metric.Key):F3}s\n";
                report += $"  Min: {GetMinMetric(metric.Key):F3}s\n";
                report += $"  Max: {GetMaxMetric(metric.Key):F3}s\n";
            }

            return report;
        }

        /// <summary>
        /// Réinitialise toutes les statistiques
        /// </summary>
        public void ResetStatistics()
        {
            totalScans = 0;
            successfulScans = 0;
            recognitionRate = 0f;
            metrics.Clear();
            timers.Clear();
            UnityEngine.Debug.Log("Statistiques réinitialisées");
        }

        /// <summary>
        /// Active/désactive le monitoring
        /// </summary>
        public void SetMonitoringEnabled(bool enabled)
        {
            enableMonitoring = enabled;
        }

        /// <summary>
        /// Récupère le FPS actuel
        /// </summary>
        public float GetCurrentFPS()
        {
            return currentFPS;
        }

        /// <summary>
        /// Récupère le taux de reconnaissance
        /// </summary>
        public float GetRecognitionRate()
        {
            return recognitionRate;
        }
    }
}
