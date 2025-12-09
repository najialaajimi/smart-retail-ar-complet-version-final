using UnityEngine;
using System.Diagnostics;
using System.Collections.Generic;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Moniteur de performances pour l'application Smart Retail AR
    /// Permet de surveiller les KPIs critiques (latence, FPS, etc.)
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private bool displayOnScreen = false;
        [SerializeField] private float updateInterval = 0.5f;

        [Header("Seuils d'alerte")]
        [SerializeField] private float targetLatencyMs = 1000f; // KPI: <= 1 seconde
        [SerializeField] private int targetFPS = 30;

        // Statistiques FPS
        private float m_DeltaTime = 0.0f;
        private float m_Fps = 0.0f;
        private float m_UpdateTimer = 0.0f;

        // Statistiques de latence
        private Dictionary<string, Stopwatch> m_ActiveTimers = new Dictionary<string, Stopwatch>();
        private Dictionary<string, float> m_LastLatencies = new Dictionary<string, float>();

        // Style GUI
        private GUIStyle m_Style;
        private Rect m_DisplayRect = new Rect(10, 10, 300, 100);

        private void Awake()
        {
            // Initialiser le style GUI
            m_Style = new GUIStyle();
            m_Style.alignment = TextAnchor.UpperLeft;
            m_Style.fontSize = 18;
            m_Style.normal.textColor = Color.white;
        }

        private void Update()
        {
            if (!enableMonitoring)
                return;

            // Calculer le FPS
            m_DeltaTime += (Time.unscaledDeltaTime - m_DeltaTime) * 0.1f;
            m_UpdateTimer += Time.unscaledDeltaTime;

            if (m_UpdateTimer >= updateInterval)
            {
                m_Fps = 1.0f / m_DeltaTime;
                m_UpdateTimer = 0.0f;

                // Vérifier les KPIs
                CheckPerformanceKPIs();
            }
        }

        private void OnGUI()
        {
            if (!enableMonitoring || !displayOnScreen)
                return;

            string displayText = $"FPS: {m_Fps:F1}\n";
            
            foreach (var latency in m_LastLatencies)
            {
                displayText += $"{latency.Key}: {latency.Value:F0}ms\n";
            }

            GUI.Label(m_DisplayRect, displayText, m_Style);
        }

        /// <summary>
        /// Démarre la mesure d'une opération
        /// </summary>
        public void StartTimer(string operationName)
        {
            if (!enableMonitoring)
                return;

            if (m_ActiveTimers.ContainsKey(operationName))
            {
                m_ActiveTimers[operationName].Restart();
            }
            else
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                m_ActiveTimers[operationName] = sw;
            }

            DebugLogger.LogDebug($"Début du timer: {operationName}");
        }

        /// <summary>
        /// Arrête la mesure d'une opération et retourne la latence en ms
        /// </summary>
        public float StopTimer(string operationName)
        {
            if (!enableMonitoring)
                return 0f;

            if (!m_ActiveTimers.ContainsKey(operationName))
            {
                DebugLogger.LogWarning($"Timer non trouvé: {operationName}");
                return 0f;
            }

            Stopwatch sw = m_ActiveTimers[operationName];
            sw.Stop();
            float latencyMs = sw.ElapsedMilliseconds;

            m_LastLatencies[operationName] = latencyMs;

            // Vérifier si on respecte le KPI de latence
            if (latencyMs > targetLatencyMs)
            {
                DebugLogger.LogWarning($"⚠️ KPI latence dépassé pour {operationName}: {latencyMs:F0}ms (cible: {targetLatencyMs}ms)");
            }
            else
            {
                DebugLogger.LogDebug($"✓ {operationName}: {latencyMs:F0}ms");
            }

            return latencyMs;
        }

        /// <summary>
        /// Récupère la dernière latence mesurée pour une opération
        /// </summary>
        public float GetLastLatency(string operationName)
        {
            return m_LastLatencies.ContainsKey(operationName) ? m_LastLatencies[operationName] : 0f;
        }

        /// <summary>
        /// Vérifie les KPIs de performance
        /// </summary>
        private void CheckPerformanceKPIs()
        {
            // Vérifier le FPS
            if (m_Fps < targetFPS)
            {
                DebugLogger.LogWarning($"⚠️ FPS sous la cible: {m_Fps:F1} FPS (cible: {targetFPS} FPS)");
            }
        }

        /// <summary>
        /// Active/désactive le monitoring
        /// </summary>
        public void SetMonitoringEnabled(bool enabled)
        {
            enableMonitoring = enabled;
        }

        /// <summary>
        /// Active/désactive l'affichage à l'écran
        /// </summary>
        public void SetDisplayEnabled(bool enabled)
        {
            displayOnScreen = enabled;
        }

        /// <summary>
        /// Réinitialise toutes les statistiques
        /// </summary>
        public void ResetStats()
        {
            m_ActiveTimers.Clear();
            m_LastLatencies.Clear();
            m_DeltaTime = 0.0f;
            m_Fps = 0.0f;
        }

        /// <summary>
        /// Retourne un rapport de performance
        /// </summary>
        public string GetPerformanceReport()
        {
            string report = $"=== Rapport de Performance ===\n";
            report += $"FPS actuel: {m_Fps:F1}\n";
            report += $"Latences mesurées:\n";

            foreach (var latency in m_LastLatencies)
            {
                string status = latency.Value <= targetLatencyMs ? "✓" : "✗";
                report += $"  {status} {latency.Key}: {latency.Value:F0}ms\n";
            }

            return report;
        }
    }
}
