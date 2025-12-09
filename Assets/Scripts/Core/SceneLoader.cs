using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Événement de chargement de scène
    /// </summary>
    [System.Serializable]
    public class SceneLoadEvent : UnityEvent<string> { }

    /// <summary>
    /// Gère le chargement et la navigation entre les scènes
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader instance;
        public static SceneLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SceneLoader>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SceneLoader");
                        instance = go.AddComponent<SceneLoader>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [Header("Événements")]
        public SceneLoadEvent onSceneLoadStarted = new SceneLoadEvent();
        public SceneLoadEvent onSceneLoadCompleted = new SceneLoadEvent();
        public UnityEvent<float> onLoadingProgress = new UnityEvent<float>();

        private string currentSceneName;
        private bool isLoading = false;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                currentSceneName = SceneManager.GetActiveScene().name;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Charge une scène de manière asynchrone
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (isLoading)
            {
                Debug.LogWarning("Un chargement de scène est déjà en cours");
                return;
            }

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("Nom de scène invalide");
                return;
            }

            StartCoroutine(LoadSceneAsync(sceneName));
        }

        /// <summary>
        /// Charge une scène par son index
        /// </summary>
        public void LoadSceneByIndex(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"Index de scène invalide: {sceneIndex}");
                return;
            }

            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            LoadScene(sceneName);
        }

        /// <summary>
        /// Charge la scène de menu principal
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene("MainMenu");
        }

        /// <summary>
        /// Charge la scène de scanner QR
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
        /// Charge la scène de recommandations
        /// </summary>
        public void LoadRecommendations()
        {
            LoadScene("Recommendations");
        }

        /// <summary>
        /// Charge la scène de paramètres
        /// </summary>
        public void LoadSettings()
        {
            LoadScene("Settings");
        }

        /// <summary>
        /// Recharge la scène actuelle
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadScene(currentSceneName);
        }

        /// <summary>
        /// Coroutine de chargement asynchrone
        /// </summary>
        private IEnumerator LoadSceneAsync(string sceneName)
        {
            isLoading = true;
            Debug.Log($"Chargement de la scène: {sceneName}");
            onSceneLoadStarted?.Invoke(sceneName);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Attendre que le chargement soit presque terminé
            while (asyncLoad.progress < 0.9f)
            {
                float progress = asyncLoad.progress / 0.9f;
                onLoadingProgress?.Invoke(progress);
                yield return null;
            }

            // Simuler un temps de chargement minimum pour la fluidité
            onLoadingProgress?.Invoke(1f);
            yield return new WaitForSeconds(0.5f);

            // Activer la scène
            asyncLoad.allowSceneActivation = true;

            // Attendre que la scène soit complètement chargée
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            currentSceneName = sceneName;
            isLoading = false;

            Debug.Log($"Scène chargée: {sceneName}");
            onSceneLoadCompleted?.Invoke(sceneName);
        }

        /// <summary>
        /// Récupère le nom de la scène actuelle
        /// </summary>
        public string GetCurrentSceneName()
        {
            return currentSceneName;
        }

        /// <summary>
        /// Vérifie si un chargement est en cours
        /// </summary>
        public bool IsLoading()
        {
            return isLoading;
        }

        /// <summary>
        /// Précharge une scène en arrière-plan
        /// </summary>
        public void PreloadScene(string sceneName)
        {
            StartCoroutine(PreloadSceneAsync(sceneName));
        }

        private IEnumerator PreloadSceneAsync(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Debug.Log($"Scène préchargée: {sceneName}");
        }
    }
}
