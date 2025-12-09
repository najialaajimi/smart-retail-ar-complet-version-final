using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script éditeur pour configurer automatiquement les scènes avec GameObjects et composants
/// Menu: Tools > Smart Retail AR > Auto Scene Setup
/// </summary>
public class AutoSceneSetup : EditorWindow
{
    private bool setupMainMenu = true;
    private bool setupQRScanner = true;
    private bool setupARProductView = true;
    private bool setupRecommendations = true;
    private bool setupSettings = true;

    [MenuItem("Tools/Smart Retail AR/Auto Scene Setup")]
    public static void ShowWindow()
    {
        GetWindow<AutoSceneSetup>("Auto Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Configuration Automatique des Scènes", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Sélectionnez les scènes à configurer:", EditorStyles.label);
        setupMainMenu = EditorGUILayout.Toggle("MainMenu", setupMainMenu);
        setupQRScanner = EditorGUILayout.Toggle("QRScanner", setupQRScanner);
        setupARProductView = EditorGUILayout.Toggle("ARProductView", setupARProductView);
        setupRecommendations = EditorGUILayout.Toggle("Recommendations", setupRecommendations);
        setupSettings = EditorGUILayout.Toggle("Settings", setupSettings);

        GUILayout.Space(20);

        if (GUILayout.Button("Configurer les Scènes Sélectionnées", GUILayout.Height(40)))
        {
            SetupSelectedScenes();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Configurer Toutes les Scènes", GUILayout.Height(30)))
        {
            setupMainMenu = setupQRScanner = setupARProductView = setupRecommendations = setupSettings = true;
            SetupSelectedScenes();
        }
    }

    private void SetupSelectedScenes()
    {
        if (setupMainMenu) SetupMainMenuScene();
        if (setupQRScanner) SetupQRScannerScene();
        if (setupARProductView) SetupARProductViewScene();
        if (setupRecommendations) SetupRecommendationsScene();
        if (setupSettings) SetupSettingsScene();

        Debug.Log("✅ Configuration des scènes terminée!");
        EditorUtility.DisplayDialog("Succès", "Configuration des scènes terminée avec succès!", "OK");
    }

    /// <summary>
    /// Configure la scène MainMenu avec tous les GameObjects et composants nécessaires
    /// </summary>
    private void SetupMainMenuScene()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        // Créer EventSystem si nécessaire
        CreateEventSystemIfNeeded();

        // Créer GameManager
        GameObject gameManager = CreateOrGetGameObject("GameManager");
        AddComponentIfMissing<SmartRetailAR.Core.GameManager>(gameManager);

        // Créer Canvas principal
        GameObject canvas = CreateOrGetCanvas("MainMenuCanvas");
        
        // Créer panneau de fond
        GameObject backgroundPanel = CreateOrGetGameObject("BackgroundPanel", canvas.transform);
        AddComponentIfMissing<Image>(backgroundPanel);
        SetRectTransform(backgroundPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // Créer titre
        GameObject title = CreateOrGetGameObject("TitleText", canvas.transform);
        TextMeshProUGUI titleText = AddComponentIfMissing<TextMeshProUGUI>(title);
        titleText.text = "Smart Retail AR";
        titleText.fontSize = 48;
        titleText.alignment = TextAlignmentOptions.Center;
        SetRectTransform(title, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), new Vector2(0, 0), new Vector2(400, 100));

        // Créer boutons de menu
        CreateMenuButton(canvas.transform, "ScanButton", "Scanner Produit", new Vector2(0.5f, 0.6f), "QRScanner");
        CreateMenuButton(canvas.transform, "ARViewButton", "Vue AR", new Vector2(0.5f, 0.5f), "ARProductView");
        CreateMenuButton(canvas.transform, "RecommendationsButton", "Recommandations", new Vector2(0.5f, 0.4f), "Recommendations");
        CreateMenuButton(canvas.transform, "SettingsButton", "Paramètres", new Vector2(0.5f, 0.3f), "Settings");

        // Ajouter MainMenuController
        GameObject controller = CreateOrGetGameObject("MainMenuController");
        AddComponentIfMissing<SmartRetailAR.UI.MainMenuController>(controller);

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ MainMenu scene configured");
    }

    /// <summary>
    /// Configure la scène QRScanner
    /// </summary>
    private void SetupQRScannerScene()
    {
        string scenePath = "Assets/Scenes/QRScanner.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        CreateEventSystemIfNeeded();

        // Créer QRScanner Manager
        GameObject scannerManager = CreateOrGetGameObject("QRScannerManager");
        AddComponentIfMissing<SmartRetailAR.QRCode.QRCodeScanner>(scannerManager);

        // Créer Canvas
        GameObject canvas = CreateOrGetCanvas("QRScannerCanvas");

        // Panneau de scan
        GameObject scanPanel = CreateOrGetGameObject("ScanPanel", canvas.transform);
        AddComponentIfMissing<Image>(scanPanel);
        SetRectTransform(scanPanel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // Zone de visée pour le scan
        GameObject targetArea = CreateOrGetGameObject("TargetArea", scanPanel.transform);
        Image targetImage = AddComponentIfMissing<Image>(targetArea);
        targetImage.color = new Color(1, 1, 1, 0.3f);
        SetRectTransform(targetArea, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(300, 300));

        // Instructions
        GameObject instructions = CreateOrGetGameObject("InstructionsText", scanPanel.transform);
        TextMeshProUGUI instructionsText = AddComponentIfMissing<TextMeshProUGUI>(instructions);
        instructionsText.text = "Placez le QR code dans le cadre";
        instructionsText.fontSize = 24;
        instructionsText.alignment = TextAlignmentOptions.Center;
        SetRectTransform(instructions, new Vector2(0.5f, 0.2f), new Vector2(0.5f, 0.2f), new Vector2(0, 0), new Vector2(500, 60));

        // Bouton retour
        CreateMenuButton(canvas.transform, "BackButton", "Retour", new Vector2(0.1f, 0.9f), "MainMenu");

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ QRScanner scene configured");
    }

    /// <summary>
    /// Configure la scène ARProductView
    /// </summary>
    private void SetupARProductViewScene()
    {
        string scenePath = "Assets/Scenes/ARProductView.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        CreateEventSystemIfNeeded();

        // Créer AR Session Origin (XR Origin sera ajouté via le package)
        GameObject arSessionOrigin = CreateOrGetGameObject("XR Origin");
        
        // Créer AR Session
        GameObject arSession = CreateOrGetGameObject("AR Session");

        // Créer AR Manager
        GameObject arManager = CreateOrGetGameObject("ARSessionManager");
        AddComponentIfMissing<SmartRetailAR.AR.ARSessionManager>(arManager);

        // Créer Placement Manager
        GameObject placementManager = CreateOrGetGameObject("ARPlacementManager");
        AddComponentIfMissing<SmartRetailAR.AR.ARPlacementManager>(placementManager);

        // Créer Canvas AR Overlay
        GameObject canvas = CreateOrGetCanvas("AROverlayCanvas");

        // Panneau d'informations produit
        GameObject productPanel = CreateOrGetGameObject("ProductInfoPanel", canvas.transform);
        AddComponentIfMissing<Image>(productPanel);
        AddComponentIfMissing<SmartRetailAR.UI.ProductInfoPanel>(productPanel);
        SetRectTransform(productPanel, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), new Vector2(0, 0), new Vector2(400, 200));

        // Boutons d'action
        CreateMenuButton(canvas.transform, "RecommendationsButton", "Voir Alternatives", new Vector2(0.5f, 0.2f), "Recommendations");
        CreateMenuButton(canvas.transform, "BackButton", "Retour", new Vector2(0.1f, 0.9f), "MainMenu");

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ ARProductView scene configured");
    }

    /// <summary>
    /// Configure la scène Recommendations
    /// </summary>
    private void SetupRecommendationsScene()
    {
        string scenePath = "Assets/Scenes/Recommendations.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        CreateEventSystemIfNeeded();

        // Créer Recommendation Engine
        GameObject engine = CreateOrGetGameObject("RecommendationEngine");
        AddComponentIfMissing<SmartRetailAR.Recommendations.RecommendationEngine>(engine);

        // Créer Canvas
        GameObject canvas = CreateOrGetCanvas("RecommendationsCanvas");

        // Titre
        GameObject title = CreateOrGetGameObject("TitleText", canvas.transform);
        TextMeshProUGUI titleText = AddComponentIfMissing<TextMeshProUGUI>(title);
        titleText.text = "Recommandations";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        SetRectTransform(title, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0, 0), new Vector2(400, 60));

        // Panneau de filtres
        GameObject filterPanel = CreateOrGetGameObject("FilterPanel", canvas.transform);
        AddComponentIfMissing<Image>(filterPanel);
        AddComponentIfMissing<SmartRetailAR.UI.FilterPanel>(filterPanel);
        SetRectTransform(filterPanel, new Vector2(0.5f, 0.75f), new Vector2(0.5f, 0.75f), new Vector2(0, 0), new Vector2(700, 100));

        // Scroll View pour les recommandations
        GameObject scrollView = CreateScrollView(canvas.transform, "RecommendationsScrollView");
        AddComponentIfMissing<SmartRetailAR.UI.RecommendationUI>(scrollView);

        // Bouton retour
        CreateMenuButton(canvas.transform, "BackButton", "Retour", new Vector2(0.1f, 0.05f), "MainMenu");

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ Recommendations scene configured");
    }

    /// <summary>
    /// Configure la scène Settings
    /// </summary>
    private void SetupSettingsScene()
    {
        string scenePath = "Assets/Scenes/Settings.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath);

        CreateEventSystemIfNeeded();

        // Créer AppSettings
        GameObject appSettings = CreateOrGetGameObject("AppSettings");
        AddComponentIfMissing<SmartRetailAR.Core.AppSettings>(appSettings);

        // Créer Canvas
        GameObject canvas = CreateOrGetCanvas("SettingsCanvas");

        // Titre
        GameObject title = CreateOrGetGameObject("TitleText", canvas.transform);
        TextMeshProUGUI titleText = AddComponentIfMissing<TextMeshProUGUI>(title);
        titleText.text = "Paramètres";
        titleText.fontSize = 36;
        titleText.alignment = TextAlignmentOptions.Center;
        SetRectTransform(title, new Vector2(0.5f, 0.9f), new Vector2(0.5f, 0.9f), new Vector2(0, 0), new Vector2(400, 60));

        // Panneau de paramètres
        GameObject settingsPanel = CreateOrGetGameObject("SettingsPanel", canvas.transform);
        AddComponentIfMissing<Image>(settingsPanel);
        AddComponentIfMissing<SmartRetailAR.UI.SettingsPanel>(settingsPanel);
        SetRectTransform(settingsPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(600, 400));

        // Boutons
        CreateMenuButton(canvas.transform, "SaveButton", "Sauvegarder", new Vector2(0.7f, 0.1f), null);
        CreateMenuButton(canvas.transform, "BackButton", "Retour", new Vector2(0.3f, 0.1f), "MainMenu");

        EditorSceneManager.SaveScene(scene);
        Debug.Log("✅ Settings scene configured");
    }

    // ==================== MÉTHODES UTILITAIRES ====================

    private GameObject CreateOrGetGameObject(string name, Transform parent = null)
    {
        GameObject obj = GameObject.Find(name);
        if (obj == null)
        {
            obj = new GameObject(name);
            if (parent != null)
            {
                obj.transform.SetParent(parent, false);
            }
        }
        return obj;
    }

    private GameObject CreateOrGetCanvas(string name)
    {
        GameObject canvasObj = GameObject.Find(name);
        if (canvasObj == null)
        {
            canvasObj = new GameObject(name);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        return canvasObj;
    }

    private void CreateEventSystemIfNeeded()
    {
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }

    private T AddComponentIfMissing<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
        }
        return component;
    }

    private void SetRectTransform(GameObject obj, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = obj.AddComponent<RectTransform>();
        }
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
    }

    private void CreateMenuButton(Transform parent, string name, string text, Vector2 position, string targetScene)
    {
        GameObject buttonObj = CreateOrGetGameObject(name, parent);
        
        Button button = AddComponentIfMissing<Button>(buttonObj);
        AddComponentIfMissing<Image>(buttonObj);
        
        SetRectTransform(buttonObj, position, position, Vector2.zero, new Vector2(300, 60));

        // Créer le texte du bouton
        GameObject textObj = CreateOrGetGameObject("Text", buttonObj.transform);
        TextMeshProUGUI buttonText = AddComponentIfMissing<TextMeshProUGUI>(textObj);
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        SetRectTransform(textObj, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // Ajouter l'action de navigation si targetScene est spécifié
        if (!string.IsNullOrEmpty(targetScene))
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => LoadScene(targetScene));
        }
    }

    private GameObject CreateScrollView(Transform parent, string name)
    {
        GameObject scrollView = CreateOrGetGameObject(name, parent);
        
        AddComponentIfMissing<ScrollRect>(scrollView);
        AddComponentIfMissing<Image>(scrollView);
        SetRectTransform(scrollView, new Vector2(0.1f, 0.1f), new Vector2(0.9f, 0.65f), Vector2.zero, Vector2.zero);

        // Viewport
        GameObject viewport = CreateOrGetGameObject("Viewport", scrollView.transform);
        AddComponentIfMissing<Image>(viewport);
        AddComponentIfMissing<Mask>(viewport);
        SetRectTransform(viewport, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        // Content
        GameObject content = CreateOrGetGameObject("Content", viewport.transform);
        AddComponentIfMissing<VerticalLayoutGroup>(content);
        ContentSizeFitter fitter = AddComponentIfMissing<ContentSizeFitter>(content);
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        SetRectTransform(content, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 0));

        // Configurer ScrollRect
        ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
        scrollRect.content = content.GetComponent<RectTransform>();
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.vertical = true;
        scrollRect.horizontal = false;

        return scrollView;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

/// <summary>
/// Namespace pour éviter les conflits avec les scripts existants
/// </summary>
namespace SmartRetailAR
{
    // Les classes référencées existent déjà dans le projet
}
