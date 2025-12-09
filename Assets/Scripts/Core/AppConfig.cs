using UnityEngine;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Configuration globale de l'application Smart Retail AR
    /// </summary>
    [CreateAssetMenu(fileName = "AppConfig", menuName = "Smart Retail AR/App Config", order = 0)]
    public class AppConfig : ScriptableObject
    {
        [Header("Version")]
        public string appVersion = "1.0.0";
        public string unityVersion = "2022.3.62f3";

        [Header("Configuration AR")]
        public bool enableARCore = true;
        public bool enableARKit = true;
        public float arTrackingDistance = 5.0f;
        public float arPlacementHeight = 0.0f;

        [Header("Configuration QR Code")]
        public float qrScanInterval = 0.5f;
        public int qrImageWidth = 1280;
        public int qrImageHeight = 720;

        [Header("Configuration Produits")]
        public string productsJsonPath = "Data/Products/products";
        public string categoriesJsonPath = "Data/Categories/categories";
        public bool useLocalCache = true;
        public float cacheExpirationHours = 24f;

        [Header("Configuration UI")]
        public float transitionDuration = 0.3f;
        public bool enableAnimations = true;
        public bool enableHapticFeedback = true;

        [Header("Configuration Performance")]
        public bool enablePerformanceMonitoring = true;
        public float targetLatencyMs = 1000f; // KPI: <= 1 seconde
        public int targetFPS = 30;
        public bool optimizeForMobile = true;

        [Header("Configuration Debug")]
        public bool enableDebugLogs = true;
        public bool showPerformanceOnScreen = false;
        public bool enableTestMode = false;

        [Header("Configuration Recommandations")]
        public int maxRecommendations = 5;
        public float recommendationScoreThreshold = 5.0f;
        public bool prioritizeBio = true;
        public bool prioritizeLocal = true;

        /// <summary>
        /// Applique la configuration à l'application
        /// </summary>
        public void ApplyConfiguration()
        {
            // Configuration du logger
            if (enableDebugLogs)
            {
                SmartRetailAR.Utils.DebugLogger.ConfigureForDevelopment();
            }
            else
            {
                SmartRetailAR.Utils.DebugLogger.ConfigureForProduction();
            }

            // Configuration de la qualité graphique pour mobile
            if (optimizeForMobile)
            {
                Application.targetFrameRate = targetFPS;
                QualitySettings.vSyncCount = 0;
            }

            SmartRetailAR.Utils.DebugLogger.Log($"Configuration appliquée - Version {appVersion}", Utils.LogLevel.Info);
        }

        private void OnValidate()
        {
            // Valider les paramètres
            qrScanInterval = Mathf.Max(0.1f, qrScanInterval);
            targetLatencyMs = Mathf.Max(100f, targetLatencyMs);
            targetFPS = Mathf.Clamp(targetFPS, 15, 120);
            maxRecommendations = Mathf.Clamp(maxRecommendations, 1, 10);
        }
    }
}
