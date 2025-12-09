using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Utilitaire pour exécuter automatiquement toutes les scènes
    /// Parcourt les GameObjects et valide les composants
    /// </summary>
    public class SceneAutoRunner : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool runOnStart = false;
        [SerializeField] private float sceneLoadDelay = 2f;
        [SerializeField] private bool validateComponents = true;
        [SerializeField] private bool logDetails = true;

        private List<string> sceneResults = new List<string>();
        private int currentSceneIndex = 0;
        private bool isRunning = false;

        private void Start()
        {
            if (runOnStart)
            {
                StartAutoRun();
            }
        }

        /// <summary>
        /// Démarre l'exécution automatique des scènes
        /// </summary>
        public void StartAutoRun()
        {
            if (isRunning)
            {
                Debug.LogWarning("Auto-run déjà en cours");
                return;
            }

            Debug.Log("=== Démarrage Auto-Run des scènes ===");
            sceneResults.Clear();
            currentSceneIndex = 0;
            isRunning = true;

            StartCoroutine(RunAllScenesCoroutine());
        }

        /// <summary>
        /// Coroutine pour parcourir toutes les scènes
        /// </summary>
        private IEnumerator RunAllScenesCoroutine()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                currentSceneIndex = i;
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                Debug.Log($"--- Chargement de la scène {i + 1}/{sceneCount}: {sceneName} ---");

                // Charger la scène
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(i);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                // Attendre que la scène soit prête
                yield return new WaitForSeconds(sceneLoadDelay);

                // Valider la scène
                ValidateScene(sceneName);

                yield return new WaitForSeconds(0.5f);
            }

            // Afficher le résumé
            GenerateReport();

            isRunning = false;
            Debug.Log("=== Auto-Run terminé ===");
        }

        /// <summary>
        /// Valide une scène chargée
        /// </summary>
        private void ValidateScene(string sceneName)
        {
            Scene scene = SceneManager.GetActiveScene();
            
            if (scene.name != sceneName)
            {
                string error = $"[ERREUR] Scène attendue: {sceneName}, obtenue: {scene.name}";
                sceneResults.Add(error);
                Debug.LogError(error);
                return;
            }

            // Compter les GameObjects
            GameObject[] rootObjects = scene.GetRootGameObjects();
            int totalObjects = 0;
            int componentsCount = 0;
            List<string> missingComponents = new List<string>();

            foreach (GameObject obj in rootObjects)
            {
                totalObjects++;
                ValidateGameObject(obj, ref componentsCount, missingComponents);
            }

            // Résultat de la validation
            string result = $"[OK] {sceneName} - {totalObjects} objets racines, {componentsCount} composants";
            
            if (missingComponents.Count > 0)
            {
                result += $" - {missingComponents.Count} composants manquants!";
                Debug.LogWarning($"{result}\nComposants manquants: {string.Join(", ", missingComponents)}");
            }
            else if (logDetails)
            {
                Debug.Log(result);
            }

            sceneResults.Add(result);
        }

        /// <summary>
        /// Valide un GameObject et ses enfants
        /// </summary>
        private void ValidateGameObject(GameObject obj, ref int componentsCount, List<string> missingComponents)
        {
            if (obj == null)
            {
                return;
            }

            if (validateComponents)
            {
                Component[] components = obj.GetComponents<Component>();
                
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        missingComponents.Add($"{obj.name}");
                    }
                    else
                    {
                        componentsCount++;
                    }
                }
            }

            // Valider les enfants
            foreach (Transform child in obj.transform)
            {
                ValidateGameObject(child.gameObject, ref componentsCount, missingComponents);
            }
        }

        /// <summary>
        /// Génère un rapport de validation
        /// </summary>
        private void GenerateReport()
        {
            Debug.Log("\n=== RAPPORT DE VALIDATION ===");
            
            int successCount = 0;
            int errorCount = 0;

            foreach (string result in sceneResults)
            {
                if (result.Contains("[OK]"))
                {
                    successCount++;
                }
                else if (result.Contains("[ERREUR]"))
                {
                    errorCount++;
                }

                Debug.Log(result);
            }

            Debug.Log($"\nRésumé: {successCount} scènes validées, {errorCount} erreurs");
            Debug.Log("=== FIN DU RAPPORT ===\n");
        }

        /// <summary>
        /// Récupère le rapport sous forme de texte
        /// </summary>
        public string GetReportText()
        {
            string report = "=== RAPPORT DE VALIDATION ===\n";
            
            foreach (string result in sceneResults)
            {
                report += result + "\n";
            }

            return report;
        }

        /// <summary>
        /// Vérifie si l'auto-run est en cours
        /// </summary>
        public bool IsRunning()
        {
            return isRunning;
        }

        /// <summary>
        /// Arrête l'auto-run
        /// </summary>
        public void Stop()
        {
            StopAllCoroutines();
            isRunning = false;
            Debug.Log("Auto-run arrêté");
        }
    }
}
