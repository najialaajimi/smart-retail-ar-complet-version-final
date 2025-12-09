using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using SmartRetailAR.Utils;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Gestionnaire de chargement des scènes avec transitions
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader s_Instance;

        [Header("Configuration")]
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private bool showLoadingScreen = true;

        private bool m_IsLoading = false;

        /// <summary>
        /// Instance singleton du SceneLoader
        /// </summary>
        public static SceneLoader Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    s_Instance = go.AddComponent<SceneLoader>();
                    DontDestroyOnLoad(go);
                }
                return s_Instance;
            }
        }

        /// <summary>
        /// Indique si une scène est en cours de chargement
        /// </summary>
        public bool IsLoading => m_IsLoading;

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Charge une scène par son nom
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (m_IsLoading)
            {
                DebugLogger.LogWarning($"Chargement déjà en cours, impossible de charger {sceneName}");
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>
        /// Charge une scène par son index
        /// </summary>
        public void LoadScene(int sceneIndex)
        {
            if (m_IsLoading)
            {
                DebugLogger.LogWarning($"Chargement déjà en cours, impossible de charger la scène {sceneIndex}");
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneIndex));
        }

        /// <summary>
        /// Recharge la scène actuelle
        /// </summary>
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            LoadScene(currentScene.name);
        }

        /// <summary>
        /// Charge le menu principal
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene("MainMenu");
        }

        /// <summary>
        /// Charge la scène du scanner QR
        /// </summary>
        public void LoadQRScanner()
        {
            LoadScene("QRScanner");
        }

        /// <summary>
        /// Charge la scène de vue AR du produit
        /// </summary>
        public void LoadARProductView()
        {
            LoadScene("ARProductView");
        }

        /// <summary>
        /// Charge la scène des recommandations
        /// </summary>
        public void LoadRecommendations()
        {
            LoadScene("Recommendations");
        }

        /// <summary>
        /// Charge la scène des paramètres
        /// </summary>
        public void LoadSettings()
        {
            LoadScene("Settings");
        }

        /// <summary>
        /// Coroutine pour charger une scène de manière asynchrone
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            m_IsLoading = true;
            DebugLogger.Log($"Chargement de la scène: {sceneName}", LogLevel.Info);

            // Transition de sortie
            if (showLoadingScreen)
            {
                // TODO: Afficher l'écran de chargement
                yield return new WaitForSeconds(transitionDuration * 0.5f);
            }

            // Charger la scène
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Attendre que la scène soit chargée
            while (asyncLoad.progress < 0.9f)
            {
                DebugLogger.LogDebug($"Progression du chargement: {asyncLoad.progress * 100}%");
                yield return null;
            }

            // Activer la scène
            asyncLoad.allowSceneActivation = true;

            // Attendre que la scène soit activée
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Transition d'entrée
            if (showLoadingScreen)
            {
                // TODO: Masquer l'écran de chargement
                yield return new WaitForSeconds(transitionDuration * 0.5f);
            }

            m_IsLoading = false;
            DebugLogger.Log($"✓ Scène {sceneName} chargée avec succès", LogLevel.Info);
        }

        /// <summary>
        /// Coroutine pour charger une scène par index de manière asynchrone
        /// </summary>
        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            m_IsLoading = true;
            DebugLogger.Log($"Chargement de la scène index: {sceneIndex}", LogLevel.Info);

            // Transition de sortie
            if (showLoadingScreen)
            {
                yield return new WaitForSeconds(transitionDuration * 0.5f);
            }

            // Charger la scène
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            // Attendre que la scène soit chargée
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Activer la scène
            asyncLoad.allowSceneActivation = true;

            // Attendre que la scène soit activée
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Transition d'entrée
            if (showLoadingScreen)
            {
                yield return new WaitForSeconds(transitionDuration * 0.5f);
            }

            m_IsLoading = false;
            DebugLogger.Log($"✓ Scène index {sceneIndex} chargée avec succès", LogLevel.Info);
        }
    }
}
