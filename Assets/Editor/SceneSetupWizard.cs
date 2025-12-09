#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Assistant de configuration des scènes Smart Retail AR
    /// Menu: Tools > Smart Retail AR > Scene Setup Wizard
    /// </summary>
    public class SceneSetupWizard : EditorWindow
    {
        private string sceneName = "NewScene";
        private SceneType sceneType = SceneType.MainMenu;

        private enum SceneType
        {
            MainMenu,
            QRScanner,
            ARProductView,
            Recommendations,
            Settings
        }

        [MenuItem("Tools/Smart Retail AR/Scene Setup Wizard")]
        public static void ShowWindow()
        {
            SceneSetupWizard window = GetWindow<SceneSetupWizard>("Scene Setup Wizard");
            window.minSize = new Vector2(400, 300);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Smart Retail AR - Scene Setup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Créez et configurez rapidement une nouvelle scène", MessageType.Info);

            GUILayout.Space(10);

            sceneName = EditorGUILayout.TextField("Nom de la scène", sceneName);
            sceneType = (SceneType)EditorGUILayout.EnumPopup("Type de scène", sceneType);

            GUILayout.Space(20);

            if (GUILayout.Button("Créer la scène", GUILayout.Height(40)))
            {
                CreateScene();
            }

            GUILayout.Space(10);

            EditorGUILayout.HelpBox(GetSceneDescription(sceneType), MessageType.Info);
        }

        /// <summary>
        /// Crée une nouvelle scène configurée
        /// </summary>
        private void CreateScene()
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                EditorUtility.DisplayDialog("Erreur", "Veuillez spécifier un nom de scène", "OK");
                return;
            }

            // Créer une nouvelle scène
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Configurer selon le type
            ConfigureScene(sceneType);

            // Sauvegarder
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            
            // Créer le dossier Scenes s'il n'existe pas
            if (!System.IO.Directory.Exists("Assets/Scenes"))
            {
                System.IO.Directory.CreateDirectory("Assets/Scenes");
            }

            bool saved = EditorSceneManager.SaveScene(newScene, scenePath);

            if (saved)
            {
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Succès", 
                    $"Scène créée et configurée:\n{scenePath}", 
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Erreur", 
                    "Échec de la sauvegarde de la scène", 
                    "OK");
            }
        }

        /// <summary>
        /// Configure la scène selon son type
        /// </summary>
        private void ConfigureScene(SceneType type)
        {
            switch (type)
            {
                case SceneType.MainMenu:
                    CreateMainMenuSetup();
                    break;
                case SceneType.QRScanner:
                    CreateQRScannerSetup();
                    break;
                case SceneType.ARProductView:
                    CreateARViewSetup();
                    break;
                case SceneType.Recommendations:
                    CreateRecommendationsSetup();
                    break;
                case SceneType.Settings:
                    CreateSettingsSetup();
                    break;
            }
        }

        private void CreateMainMenuSetup()
        {
            GameObject canvas = CreateUICanvas("MainMenuCanvas");
            // Ajouter MainMenuController
            canvas.AddComponent<SmartRetailAR.UI.MainMenuController>();
        }

        private void CreateQRScannerSetup()
        {
            GameObject scanner = new GameObject("QRScanner");
            scanner.AddComponent<SmartRetailAR.QRCode.QRCodeScanner>();
        }

        private void CreateARViewSetup()
        {
            GameObject arSession = new GameObject("AR Session");
            // Note: Les composants AR nécessitent les packages AR Foundation
        }

        private void CreateRecommendationsSetup()
        {
            GameObject canvas = CreateUICanvas("RecommendationsCanvas");
            canvas.AddComponent<SmartRetailAR.Recommendations.RecommendationUI>();
        }

        private void CreateSettingsSetup()
        {
            GameObject canvas = CreateUICanvas("SettingsCanvas");
            canvas.AddComponent<SmartRetailAR.UI.SettingsPanel>();
        }

        private GameObject CreateUICanvas(string name)
        {
            GameObject canvas = new GameObject(name);
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            return canvas;
        }

        private string GetSceneDescription(SceneType type)
        {
            switch (type)
            {
                case SceneType.MainMenu:
                    return "Menu principal avec navigation vers les autres scènes";
                case SceneType.QRScanner:
                    return "Scène de scan de QR codes pour identifier les produits";
                case SceneType.ARProductView:
                    return "Vue AR avec overlay d'informations produit";
                case SceneType.Recommendations:
                    return "Affichage des recommandations et alternatives";
                case SceneType.Settings:
                    return "Paramètres de l'application";
                default:
                    return "";
            }
        }
    }
}
#endif
