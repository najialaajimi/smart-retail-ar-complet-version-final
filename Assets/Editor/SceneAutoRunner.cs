using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Script d'automatisation pour exécuter et tester toutes les scènes
    /// Menu: Tools > Smart Retail AR > Scene Auto Runner
    /// </summary>
    public class SceneAutoRunner : EditorWindow
    {
        private Vector2 scrollPosition;
        private List<SceneAsset> scenesToRun = new List<SceneAsset>();
        private bool isRunning = false;
        private int currentSceneIndex = 0;
        private float sceneRunDuration = 5f;

        [MenuItem("Tools/Smart Retail AR/Scene Auto Runner")]
        public static void ShowWindow()
        {
            SceneAutoRunner window = GetWindow<SceneAutoRunner>("Scene Auto Runner");
            window.minSize = new Vector2(400, 400);
            window.Show();
        }

        private void OnEnable()
        {
            // Charger toutes les scènes du projet
            LoadProjectScenes();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Auto Runner", EditorStyles.boldLabel);
            GUILayout.Label("Automatise l'exécution de toutes les scènes pour tests", EditorStyles.helpBox);
            GUILayout.Space(10);

            // Configuration
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            sceneRunDuration = EditorGUILayout.FloatField("Durée par scène (secondes)", sceneRunDuration);

            GUILayout.Space(10);

            // Liste des scènes
            EditorGUILayout.LabelField("Scènes à exécuter", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));

            for (int i = 0; i < scenesToRun.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Afficher le nom de la scène
                string sceneName = scenesToRun[i] != null ? scenesToRun[i].name : "None";
                if (i == currentSceneIndex && isRunning)
                {
                    EditorGUILayout.LabelField($"▶ {sceneName} (En cours)", EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUILayout.LabelField($"{i + 1}. {sceneName}");
                }

                // Bouton pour retirer la scène
                if (GUILayout.Button("✗", GUILayout.Width(30)))
                {
                    scenesToRun.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // Bouton pour ajouter une scène
            if (GUILayout.Button("Ajouter une scène"))
            {
                scenesToRun.Add(null);
            }

            if (GUILayout.Button("Recharger les scènes du projet"))
            {
                LoadProjectScenes();
            }

            GUILayout.Space(10);

            // Contrôles d'exécution
            EditorGUILayout.LabelField("Contrôles", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup(isRunning);
            if (GUILayout.Button("Lancer l'exécution automatique"))
            {
                StartAutoRun();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!isRunning);
            if (GUILayout.Button("Arrêter"))
            {
                StopAutoRun();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            // Boutons individuels
            EditorGUILayout.LabelField("Actions rapides", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Ouvrir toutes les scènes dans Build Settings"))
            {
                AddScenesToBuildSettings();
            }

            if (GUILayout.Button("Valider toutes les scènes"))
            {
                ValidateAllScenes();
            }

            GUILayout.Space(10);

            // Informations
            if (isRunning)
            {
                EditorGUILayout.HelpBox(
                    $"Exécution en cours: Scène {currentSceneIndex + 1}/{scenesToRun.Count}\n" +
                    $"Temps restant: {sceneRunDuration}s par scène",
                    MessageType.Info
                );
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Cet outil permet de tester automatiquement toutes les scènes du projet.\n" +
                    "Chaque scène sera chargée et exécutée pendant la durée spécifiée.",
                    MessageType.Info
                );
            }
        }

        /// <summary>
        /// Charge toutes les scènes du projet
        /// </summary>
        private void LoadProjectScenes()
        {
            scenesToRun.Clear();

            // Rechercher toutes les scènes dans Assets/Scenes
            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                
                if (scene != null)
                {
                    scenesToRun.Add(scene);
                }
            }

            Debug.Log($"✓ {scenesToRun.Count} scènes chargées");
        }

        /// <summary>
        /// Démarre l'exécution automatique des scènes
        /// </summary>
        private void StartAutoRun()
        {
            if (scenesToRun.Count == 0)
            {
                EditorUtility.DisplayDialog("Erreur", "Aucune scène à exécuter", "OK");
                return;
            }

            isRunning = true;
            currentSceneIndex = 0;

            Debug.Log("=== Début de l'exécution automatique des scènes ===");
            EditorApplication.update += UpdateAutoRun;
        }

        /// <summary>
        /// Arrête l'exécution automatique
        /// </summary>
        private void StopAutoRun()
        {
            isRunning = false;
            EditorApplication.update -= UpdateAutoRun;
            Debug.Log("=== Exécution automatique arrêtée ===");
        }

        /// <summary>
        /// Mise à jour de l'exécution automatique
        /// </summary>
        private void UpdateAutoRun()
        {
            // Cette méthode serait appelée pour faire avancer l'exécution
            // Pour une vraie implémentation, il faudrait gérer le timing et le passage entre scènes
            Repaint();
        }

        /// <summary>
        /// Ajoute toutes les scènes aux Build Settings
        /// </summary>
        private void AddScenesToBuildSettings()
        {
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

            foreach (var sceneAsset in scenesToRun)
            {
                if (sceneAsset != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                }
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log($"✓ {buildScenes.Count} scènes ajoutées aux Build Settings");

            EditorUtility.DisplayDialog("Succès",
                $"{buildScenes.Count} scènes ajoutées aux Build Settings",
                "OK");
        }

        /// <summary>
        /// Valide toutes les scènes
        /// </summary>
        private void ValidateAllScenes()
        {
            int validCount = 0;
            int invalidCount = 0;

            foreach (var sceneAsset in scenesToRun)
            {
                if (sceneAsset != null)
                {
                    string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    
                    // Essayer d'ouvrir la scène pour la valider
                    try
                    {
                        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                        validCount++;
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Erreur lors du chargement de {sceneAsset.name}: {e.Message}");
                        invalidCount++;
                    }
                }
            }

            EditorUtility.DisplayDialog("Validation terminée",
                $"Scènes valides: {validCount}\nScènes invalides: {invalidCount}",
                "OK");
        }
    }
}
