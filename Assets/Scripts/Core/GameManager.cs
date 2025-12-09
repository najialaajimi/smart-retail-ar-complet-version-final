using UnityEngine;
using SmartRetailAR.Utils;
using SmartRetailAR.Products;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Gestionnaire principal de l'application Smart Retail AR
    /// Singleton qui gère l'état global et coordonne les différents systèmes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_Instance;

        [Header("Configuration")]
        [SerializeField] private AppConfig appConfig;
        
        [Header("Managers")]
        [SerializeField] private ProductManager productManager;
        [SerializeField] private PerformanceMonitor performanceMonitor;

        // État de l'application
        private bool m_IsInitialized = false;

        /// <summary>
        /// Instance singleton du GameManager
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = FindObjectOfType<GameManager>();
                    
                    if (s_Instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        s_Instance = go.AddComponent<GameManager>();
                    }
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// Configuration de l'application
        /// </summary>
        public AppConfig Config => appConfig;

        /// <summary>
        /// Gestionnaire des produits
        /// </summary>
        public ProductManager ProductManager => productManager;

        /// <summary>
        /// Moniteur de performances
        /// </summary>
        public PerformanceMonitor PerformanceMonitor => performanceMonitor;

        /// <summary>
        /// Indique si l'application est initialisée
        /// </summary>
        public bool IsInitialized => m_IsInitialized;

        private void Awake()
        {
            // Configuration du singleton
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            // Charger la configuration par défaut si non assignée
            if (appConfig == null)
            {
                appConfig = Resources.Load<AppConfig>("AppConfig");
                if (appConfig == null)
                {
                    DebugLogger.LogWarning("AppConfig non trouvé dans Resources, utilisation des valeurs par défaut");
                }
            }

            InitializeApplication();
        }

        /// <summary>
        /// Initialise l'application
        /// </summary>
        private void InitializeApplication()
        {
            DebugLogger.Log("=== Initialisation de Smart Retail AR ===", LogLevel.Info);

            // Appliquer la configuration
            if (appConfig != null)
            {
                appConfig.ApplyConfiguration();
            }

            // Initialiser le moniteur de performance
            if (performanceMonitor == null)
            {
                performanceMonitor = gameObject.AddComponent<PerformanceMonitor>();
            }

            // Initialiser le gestionnaire de produits
            if (productManager == null)
            {
                GameObject pmGo = new GameObject("ProductManager");
                pmGo.transform.SetParent(transform);
                productManager = pmGo.AddComponent<ProductManager>();
            }

            m_IsInitialized = true;
            DebugLogger.Log("✓ Application initialisée avec succès", LogLevel.Info);
        }

        /// <summary>
        /// Réinitialise l'application
        /// </summary>
        public void ResetApplication()
        {
            DebugLogger.Log("Réinitialisation de l'application...", LogLevel.Info);
            
            if (performanceMonitor != null)
            {
                performanceMonitor.ResetStats();
            }

            m_IsInitialized = false;
            InitializeApplication();
        }

        /// <summary>
        /// Quitte l'application proprement
        /// </summary>
        public void QuitApplication()
        {
            DebugLogger.Log("Fermeture de l'application...", LogLevel.Info);

            // Sauvegarder les données si nécessaire
            // TODO: Implémenter la sauvegarde des préférences utilisateur

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void OnApplicationQuit()
        {
            DebugLogger.Log("Application fermée", LogLevel.Info);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                DebugLogger.Log("Application en pause", LogLevel.Debug);
            }
            else
            {
                DebugLogger.Log("Application reprise", LogLevel.Debug);
            }
        }
    }
}
