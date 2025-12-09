using UnityEngine;
using UnityEngine.Events;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Gestionnaire principal de l'application Smart Retail AR
    /// Singleton qui persiste entre les scènes
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private bool debugMode = false;

        [Header("Événements")]
        public UnityEvent onApplicationStarted = new UnityEvent();
        public UnityEvent onApplicationPaused = new UnityEvent();
        public UnityEvent onApplicationResumed = new UnityEvent();

        private bool isInitialized = false;
        private bool isPaused = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initialise le gestionnaire de jeu
        /// </summary>
        private void Initialize()
        {
            if (isInitialized)
            {
                return;
            }

            Debug.Log("=== Smart Retail AR - Initialisation ===");

            // Configuration des paramètres de l'application
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Charger les paramètres
            AppSettings.Instance.LoadSettings();

            // Initialiser la base de données des produits
            Products.ProductDatabase.Instance.LoadFromJson();

            isInitialized = true;
            onApplicationStarted?.Invoke();

            if (debugMode)
            {
                Debug.Log("Mode Debug activé");
            }

            Debug.Log("=== Initialisation terminée ===");
        }

        /// <summary>
        /// Récupère le statut d'initialisation
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        /// <summary>
        /// Active/désactive le mode debug
        /// </summary>
        public void SetDebugMode(bool enabled)
        {
            debugMode = enabled;
            Debug.Log($"Mode Debug: {(enabled ? "Activé" : "Désactivé")}");
        }

        /// <summary>
        /// Vérifie si le mode debug est activé
        /// </summary>
        public bool IsDebugMode()
        {
            return debugMode;
        }

        /// <summary>
        /// Quitte l'application
        /// </summary>
        public void QuitApplication()
        {
            Debug.Log("Fermeture de l'application");

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private void OnApplicationPause(bool pause)
        {
            isPaused = pause;

            if (pause)
            {
                Debug.Log("Application mise en pause");
                onApplicationPaused?.Invoke();
            }
            else
            {
                Debug.Log("Application reprise");
                onApplicationResumed?.Invoke();
            }
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Application en cours de fermeture");
            
            // Sauvegarder les paramètres
            AppSettings.Instance.SaveSettings();
        }

        /// <summary>
        /// Vérifie si l'application est en pause
        /// </summary>
        public bool IsPaused()
        {
            return isPaused;
        }
    }
}
