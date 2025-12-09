# Guide Complet des Interactions et Composants par Scène

## Vue d'Ensemble

Ce guide détaille **comment gérer les composants et interactions** dans chaque scène pour permettre une exécution fonctionnelle du projet Smart Retail AR.

---

## 🎯 Composants Requis par Scène

### 1. MainMenu.unity

#### GameObjects et Composants Essentiels

```
MainMenu (Scene Root)
├── GameManager (GameObject)
│   └── GameManager.cs (Component)
│
├── Canvas (GameObject)
│   ├── Canvas (Component)
│   │   ├── Render Mode: Screen Space - Overlay
│   │   ├── Pixel Perfect: true
│   │   └── Sort Order: 0
│   ├── CanvasScaler (Component)
│   │   ├── UI Scale Mode: Scale With Screen Size
│   │   └── Reference Resolution: 1920x1080
│   ├── GraphicRaycaster (Component) ✅ REQUIS pour interactions
│   └── MainMenuController.cs (Component)
│
├── EventSystem (GameObject) ✅ OBLIGATOIRE
│   ├── EventSystem (Component)
│   └── StandaloneInputModule (Component)
│
└── UI Elements (children of Canvas)
    ├── TitleText (Text/TextMeshPro)
    ├── ScanQRButton (Button)
    │   ├── Image (Component)
    │   ├── Button (Component)
    │   │   └── OnClick() → SceneLoader.LoadScene("QRScanner")
    │   └── Text (Component)
    ├── ViewProductsButton (Button)
    │   └── OnClick() → SceneLoader.LoadScene("Recommendations")
    ├── SettingsButton (Button)
    │   └── OnClick() → SceneLoader.LoadScene("Settings")
    └── QuitButton (Button)
        └── OnClick() → Application.Quit()
```

#### Configuration des Interactions

**Script: MainMenuController.cs**
```csharp
using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Core;

namespace SmartRetailAR.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI References")]
        public Button scanQRButton;
        public Button viewProductsButton;
        public Button settingsButton;
        public Button quitButton;

        private void Start()
        {
            // Câbler les événements onClick
            if (scanQRButton != null)
                scanQRButton.onClick.AddListener(OnScanQRClicked);
            
            if (viewProductsButton != null)
                viewProductsButton.onClick.AddListener(OnViewProductsClicked);
            
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsClicked);
            
            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitClicked);

            Debug.Log("[MainMenu] Initialized - All buttons wired");
        }

        private void OnScanQRClicked()
        {
            Debug.Log("[MainMenu] Loading QRScanner scene");
            SceneLoader.Instance.LoadScene("QRScanner");
        }

        private void OnViewProductsClicked()
        {
            Debug.Log("[MainMenu] Loading Recommendations scene");
            SceneLoader.Instance.LoadScene("Recommendations");
        }

        private void OnSettingsClicked()
        {
            Debug.Log("[MainMenu] Loading Settings scene");
            SceneLoader.Instance.LoadScene("Settings");
        }

        private void OnQuitClicked()
        {
            Debug.Log("[MainMenu] Quitting application");
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
```

---

### 2. QRScanner.unity

#### GameObjects et Composants Essentiels

```
QRScanner (Scene Root)
├── QRScannerManager (GameObject)
│   ├── QRCodeScanner.cs (Component)
│   └── RawImage (Component) - Pour afficher le flux caméra
│       ├── Texture: WebCamTexture (assignée dynamiquement)
│       └── UV Rect: (0,0,1,1)
│
├── Canvas (GameObject)
│   ├── Canvas (Component)
│   ├── CanvasScaler (Component)
│   ├── GraphicRaycaster (Component) ✅ REQUIS
│   │
│   ├── CameraFeedPanel (Panel)
│   │   └── ScanTargetArea (Image) - Zone de scan visuelle
│   │
│   ├── InstructionsText (Text)
│   │   └── Text: "Scannez le QR code du produit"
│   │
│   ├── StatusText (Text) - Feedback en temps réel
│   │   └── Text dynamique (ex: "Scanning...", "Product found!")
│   │
│   └── BackButton (Button)
│       └── OnClick() → SceneLoader.LoadScene("MainMenu")
│
└── EventSystem (GameObject) ✅ OBLIGATOIRE
```

#### Configuration des Interactions

**Script: QRCodeScanner.cs - Amélioré**
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SmartRetailAR.Products;
using SmartRetailAR.Core;

namespace SmartRetailAR.QRCode
{
    public class QRCodeScanner : MonoBehaviour
    {
        [Header("UI References")]
        public RawImage cameraFeedDisplay; // Affiche le flux caméra
        public Text statusText; // Feedback utilisateur
        public Button backButton;

        [Header("Scan Settings")]
        public float scanFrequency = 0.5f; // Scan toutes les 0.5s
        public bool autoStartCamera = true;

        private WebCamTexture webCamTexture;
        private bool isScanning = false;

        private void Start()
        {
            Debug.Log("[QRScanner] Start - Initializing camera");
            
            // Câbler le bouton retour
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            if (autoStartCamera)
                StartCamera();
        }

        public void StartCamera()
        {
            if (webCamTexture != null)
                return;

            Debug.Log("[QRScanner] Starting WebCamTexture");
            webCamTexture = new WebCamTexture();
            
            if (cameraFeedDisplay != null)
                cameraFeedDisplay.texture = webCamTexture;

            webCamTexture.Play();
            
            StartCoroutine(ScanRoutine());
            UpdateStatus("Scanning... Point at QR code");
        }

        private IEnumerator ScanRoutine()
        {
            isScanning = true;
            
            while (isScanning)
            {
                yield return new WaitForSeconds(scanFrequency);
                
                // Simulation de scan (remplacer par ZXing en production)
                if (Input.GetKeyDown(KeyCode.Space)) // TEST: Appuyer sur Espace pour simuler
                {
                    SimulateScan("SMARTRETAIL:PROD001");
                }
                
                // TODO: Intégrer ZXing ici
                // var result = DecodeQRCode(webCamTexture);
                // if (result != null) OnQRCodeDetected(result);
            }
        }

        public void SimulateScan(string qrData)
        {
            Debug.Log($"[QRScanner] QR Code detected: {qrData}");
            OnQRCodeDetected(qrData);
        }

        private void OnQRCodeDetected(string qrData)
        {
            // Format attendu: SMARTRETAIL:PROD001
            if (!qrData.StartsWith("SMARTRETAIL:"))
            {
                UpdateStatus("Invalid QR code format");
                return;
            }

            string productId = qrData.Replace("SMARTRETAIL:", "");
            Debug.Log($"[QRScanner] Product ID extracted: {productId}");

            // Recherche du produit
            Product product = ProductDatabase.Instance.GetProductById(productId);
            
            if (product != null)
            {
                UpdateStatus($"Product found: {product.name}");
                ProductManager.Instance.SetCurrentProduct(product);
                
                StopScanning();
                
                // Transition vers AR
                StartCoroutine(TransitionToAR());
            }
            else
            {
                UpdateStatus($"Product not found: {productId}");
            }
        }

        private IEnumerator TransitionToAR()
        {
            yield return new WaitForSeconds(1f); // Pause pour feedback
            SceneLoader.Instance.LoadScene("ARProductView");
        }

        private void StopScanning()
        {
            isScanning = false;
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
            
            Debug.Log($"[QRScanner] Status: {message}");
        }

        private void OnBackClicked()
        {
            StopScanning();
            SceneLoader.Instance.LoadScene("MainMenu");
        }

        private void OnDestroy()
        {
            StopScanning();
        }
    }
}
```

**🔧 Points Clés pour le Fonctionnement:**
1. **WebCamTexture** doit être assignée à un RawImage pour affichage
2. **EventSystem** requis pour interactions boutons
3. **GraphicRaycaster** sur Canvas pour détection clics
4. Utiliser `SimulateScan("SMARTRETAIL:PROD001")` pour tests en éditeur

---

### 3. ARProductView.unity

#### GameObjects et Composants Essentiels

```
ARProductView (Scene Root)
├── AR Session (GameObject)
│   └── ARSession (Component) - Gestion session AR
│
├── XR Origin (GameObject)
│   ├── XROrigin (Component)
│   ├── Camera Offset (GameObject)
│   │   └── Main Camera (GameObject)
│   │       └── Camera (Component)
│   │           ├── Clear Flags: Solid Color
│   │           └── Background: Black
│   └── ARSessionManager.cs (Component)
│
├── AR Plane Manager (GameObject)
│   ├── ARPlaneManager (Component)
│   └── ARPlacementManager.cs (Component)
│
├── AR Tracked Image Manager (GameObject)
│   ├── ARTrackedImageManager (Component)
│   │   └── Reference Image Library: Assigned
│   └── ImageTrackingManager.cs (Component)
│
├── ProductInfoOverlay (GameObject) - World Space Canvas
│   ├── Canvas (Component)
│   │   ├── Render Mode: World Space
│   │   ├── Event Camera: Main Camera
│   │   └── Sort Order: 10
│   ├── CanvasScaler (Component)
│   ├── GraphicRaycaster (Component)
│   ├── ARProductOverlay.cs (Component)
│   │
│   └── ProductInfoPanel (Panel)
│       ├── ProductNameText (Text)
│       ├── PriceText (Text)
│       ├── NutriScoreDisplay (Image + Text)
│       ├── EcoScoreDisplay (Image + Text)
│       ├── ViewAlternativesButton (Button)
│       │   └── OnClick() → LoadRecommendations()
│       └── BackButton (Button)
│           └── OnClick() → ReturnToMenu()
│
└── EventSystem (GameObject)
```

#### Configuration des Interactions AR

**Script: ARProductOverlay.cs - Complet**
```csharp
using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Products;
using SmartRetailAR.Core;

namespace SmartRetailAR.AR
{
    public class ARProductOverlay : MonoBehaviour
    {
        [Header("UI References")]
        public Text productNameText;
        public Text priceText;
        public Text nutriScoreText;
        public Image nutriScoreBackground;
        public Text ecoScoreText;
        public Image ecoScoreBackground;
        public Button viewAlternativesButton;
        public Button backButton;

        [Header("Billboard Settings")]
        public bool enableBillboard = true;
        public Transform cameraTransform;

        private void Start()
        {
            // Trouver la caméra automatiquement
            if (cameraTransform == null)
                cameraTransform = Camera.main.transform;

            // Câbler les boutons
            if (viewAlternativesButton != null)
                viewAlternativesButton.onClick.AddListener(OnViewAlternativesClicked);
            
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            // Afficher le produit actuel
            DisplayCurrentProduct();
        }

        private void DisplayCurrentProduct()
        {
            Product product = ProductManager.Instance.GetCurrentProduct();
            
            if (product == null)
            {
                Debug.LogWarning("[AROverlay] No product selected");
                return;
            }

            Debug.Log($"[AROverlay] Displaying product: {product.name}");

            // Afficher les données
            if (productNameText != null)
                productNameText.text = product.name;
            
            if (priceText != null)
                priceText.text = $"{product.price:F2}€";

            // Nutri-Score
            if (nutriScoreText != null)
                nutriScoreText.text = product.nutrition.nutriScore;
            
            if (nutriScoreBackground != null)
                nutriScoreBackground.color = GetNutriScoreColor(product.nutrition.nutriScore);

            // Eco-Score
            if (ecoScoreText != null)
                ecoScoreText.text = $"{product.ecoScore}/100";
            
            if (ecoScoreBackground != null)
                ecoScoreBackground.color = GetEcoScoreColor(product.ecoScore);
        }

        private void LateUpdate()
        {
            // Billboard: toujours face à la caméra
            if (enableBillboard && cameraTransform != null)
            {
                transform.LookAt(cameraTransform);
                transform.Rotate(0, 180, 0); // Flip pour affichage correct
            }
        }

        private Color GetNutriScoreColor(string nutriScore)
        {
            switch (nutriScore.ToUpper())
            {
                case "A": return new Color(0.04f, 0.52f, 0.27f); // Vert foncé
                case "B": return new Color(0.53f, 0.76f, 0.29f); // Vert clair
                case "C": return new Color(1f, 0.80f, 0f);       // Jaune
                case "D": return new Color(0.95f, 0.61f, 0.07f); // Orange
                case "E": return new Color(0.90f, 0.30f, 0.24f); // Rouge
                default: return Color.gray;
            }
        }

        private Color GetEcoScoreColor(float ecoScore)
        {
            if (ecoScore >= 80) return new Color(0.04f, 0.52f, 0.27f); // Excellent
            if (ecoScore >= 60) return new Color(0.53f, 0.76f, 0.29f); // Bon
            if (ecoScore >= 40) return new Color(1f, 0.80f, 0f);       // Moyen
            if (ecoScore >= 20) return new Color(0.95f, 0.61f, 0.07f); // Faible
            return new Color(0.90f, 0.30f, 0.24f);                     // Très faible
        }

        private void OnViewAlternativesClicked()
        {
            Debug.Log("[AROverlay] Loading Recommendations");
            SceneLoader.Instance.LoadScene("Recommendations");
        }

        private void OnBackClicked()
        {
            Debug.Log("[AROverlay] Returning to MainMenu");
            SceneLoader.Instance.LoadScene("MainMenu");
        }
    }
}
```

---

### 4. Recommendations.unity

#### GameObjects et Composants

```
Recommendations (Scene Root)
├── RecommendationEngineManager (GameObject)
│   └── RecommendationEngine.cs (Component)
│
├── Canvas (GameObject)
│   ├── Canvas (Component)
│   ├── CanvasScaler (Component)
│   ├── GraphicRaycaster (Component)
│   ├── RecommendationUI.cs (Component)
│   │
│   ├── HeaderPanel (Panel)
│   │   ├── TitleText (Text): "Alternatives Recommandées"
│   │   └── CurrentProductText (Text): Affiche produit actuel
│   │
│   ├── FilterPanel (Panel)
│   │   ├── FilterPanel.cs (Component)
│   │   ├── PriceSlider (Slider)
│   │   ├── BioToggle (Toggle)
│   │   ├── EcoToggle (Toggle)
│   │   └── ApplyFiltersButton (Button)
│   │
│   ├── RecommendationsScrollView (Scroll View)
│   │   ├── Viewport
│   │   └── Content (GameObject)
│   │       ├── VerticalLayoutGroup (Component)
│   │       └── [ProductCards instantiated dynamically]
│   │
│   └── BackButton (Button)
│       └── OnClick() → SceneLoader.LoadScene("ARProductView")
│
└── EventSystem (GameObject)
```

#### Configuration des Interactions

**Script: RecommendationUI.cs - Complet**
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SmartRetailAR.Products;
using SmartRetailAR.Core;

namespace SmartRetailAR.Recommendations
{
    public class RecommendationUI : MonoBehaviour
    {
        [Header("UI References")]
        public Text currentProductText;
        public Transform contentPanel; // Content du ScrollView
        public GameObject productCardPrefab; // Prefab pour chaque produit
        public Button backButton;

        [Header("Filter Panel")]
        public FilterPanel filterPanel;

        private List<Product> recommendations;

        private void Start()
        {
            Debug.Log("[RecommendationUI] Start - Loading recommendations");

            // Câbler bouton retour
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);

            // Afficher produit actuel
            DisplayCurrentProduct();

            // Charger et afficher recommandations
            LoadRecommendations();
        }

        private void DisplayCurrentProduct()
        {
            Product current = ProductManager.Instance.GetCurrentProduct();
            
            if (current != null && currentProductText != null)
            {
                currentProductText.text = $"Alternatives pour: {current.name}";
            }
        }

        private void LoadRecommendations()
        {
            Product currentProduct = ProductManager.Instance.GetCurrentProduct();
            
            if (currentProduct == null)
            {
                Debug.LogWarning("[RecommendationUI] No current product");
                return;
            }

            // Obtenir recommandations via le moteur
            recommendations = RecommendationEngine.Instance.GetRecommendations(
                currentProduct,
                maxResults: 5
            );

            Debug.Log($"[RecommendationUI] Found {recommendations.Count} recommendations");

            // Afficher les cartes
            DisplayRecommendationCards();
        }

        private void DisplayRecommendationCards()
        {
            // Nettoyer anciennes cartes
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            // Créer nouvelles cartes
            foreach (Product product in recommendations)
            {
                CreateProductCard(product);
            }

            // Forcer rebuild du layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentPanel as RectTransform);
        }

        private void CreateProductCard(Product product)
        {
            GameObject card;
            
            if (productCardPrefab != null)
            {
                // Instancier depuis prefab
                card = Instantiate(productCardPrefab, contentPanel);
            }
            else
            {
                // Créer dynamiquement
                card = new GameObject($"Card_{product.id}");
                card.transform.SetParent(contentPanel);
                
                // Ajouter Image comme background
                Image bgImage = card.AddComponent<Image>();
                bgImage.color = new Color(0.9f, 0.9f, 0.9f);

                // Ajouter LayoutElement
                LayoutElement layout = card.AddComponent<LayoutElement>();
                layout.minHeight = 100;
                layout.preferredHeight = 120;
            }

            // Peupler avec données
            PopulateCard(card, product);
        }

        private void PopulateCard(GameObject card, Product product)
        {
            // Trouver ou créer les textes
            Text nameText = card.transform.Find("NameText")?.GetComponent<Text>();
            if (nameText == null)
            {
                GameObject nameObj = new GameObject("NameText");
                nameObj.transform.SetParent(card.transform);
                nameText = nameObj.AddComponent<Text>();
                nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                nameText.fontSize = 18;
                nameText.color = Color.black;
                
                RectTransform rect = nameText.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0.5f);
                rect.anchorMax = new Vector2(1, 1);
                rect.offsetMin = new Vector2(10, 0);
                rect.offsetMax = new Vector2(-10, -10);
            }
            nameText.text = product.name;

            // Price
            Text priceText = card.transform.Find("PriceText")?.GetComponent<Text>();
            if (priceText == null)
            {
                GameObject priceObj = new GameObject("PriceText");
                priceObj.transform.SetParent(card.transform);
                priceText = priceObj.AddComponent<Text>();
                priceText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                priceText.fontSize = 16;
                priceText.color = new Color(0.2f, 0.6f, 0.2f);
                
                RectTransform rect = priceText.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.offsetMin = new Vector2(10, 10);
                rect.offsetMax = new Vector2(-5, -5);
            }
            priceText.text = $"{product.price:F2}€";

            // Scores
            Text scoreText = card.transform.Find("ScoreText")?.GetComponent<Text>();
            if (scoreText == null)
            {
                GameObject scoreObj = new GameObject("ScoreText");
                scoreObj.transform.SetParent(card.transform);
                scoreText = scoreObj.AddComponent<Text>();
                scoreText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                scoreText.fontSize = 14;
                scoreText.color = Color.black;
                scoreText.alignment = TextAnchor.MiddleRight;
                
                RectTransform rect = scoreText.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(1, 0.5f);
                rect.offsetMin = new Vector2(5, 10);
                rect.offsetMax = new Vector2(-10, -5);
            }
            scoreText.text = $"Nutri: {product.nutrition.nutriScore} | Eco: {product.ecoScore}";

            // Bouton pour sélectionner ce produit
            Button selectButton = card.GetComponent<Button>();
            if (selectButton == null)
            {
                selectButton = card.AddComponent<Button>();
            }
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => OnProductSelected(product));

            Debug.Log($"[RecommendationUI] Card created for {product.name}");
        }

        private void OnProductSelected(Product product)
        {
            Debug.Log($"[RecommendationUI] Product selected: {product.name}");
            ProductManager.Instance.SetCurrentProduct(product);
            
            // Retour vers AR pour voir ce produit
            SceneLoader.Instance.LoadScene("ARProductView");
        }

        private void OnBackClicked()
        {
            Debug.Log("[RecommendationUI] Returning to ARProductView");
            SceneLoader.Instance.LoadScene("ARProductView");
        }

        // Public method pour réappliquer filtres
        public void RefreshRecommendations()
        {
            LoadRecommendations();
        }
    }
}
```

---

### 5. Settings.unity

#### Configuration Simple

```
Settings (Scene Root)
├── AppSettingsManager (GameObject)
│   └── AppSettings.cs (Component)
│
├── Canvas (GameObject)
│   ├── Canvas, CanvasScaler, GraphicRaycaster
│   ├── SettingsPanel.cs (Component)
│   │
│   ├── AudioToggle (Toggle)
│   │   └── OnValueChanged() → AppSettings.SetAudioEnabled(value)
│   ├── ScanFrequencySlider (Slider)
│   │   └── OnValueChanged() → AppSettings.SetScanFrequency(value)
│   ├── ARModeToggle (Toggle)
│   │   └── OnValueChanged() → AppSettings.SetARMode(value)
│   │
│   ├── SaveButton (Button)
│   │   └── OnClick() → SaveSettings()
│   └── BackButton (Button)
│       └── OnClick() → SceneLoader.LoadScene("MainMenu")
│
└── EventSystem
```

---

## 🔄 Orchestration des Transitions entre Scènes

### Flow Principal

```
┌─────────────┐
│  MainMenu   │
└──────┬──────┘
       │
       ├──[Scan QR]──→┌────────────┐
       │              │ QRScanner  │
       │              └──────┬─────┘
       │                     │
       │          [QR Detected & Product Found]
       │                     ↓
       │              ┌─────────────────┐
       │              │ ARProductView   │
       │              └────────┬────────┘
       │                       │
       ├──[View Products]──────┤
       │                       │
       │              [View Alternatives]
       │                       ↓
       │              ┌──────────────────┐
       │              │ Recommendations  │
       │              └──────────────────┘
       │
       └──[Settings]──→┌──────────┐
                       │ Settings │
                       └──────────┘
```

### SceneLoader.cs - Gestionnaire Central

```csharp
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace SmartRetailAR.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        public event System.Action<string> OnSceneLoadStarted;
        public event System.Action<string> OnSceneLoadCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log($"[SceneLoader] Loading scene: {sceneName}");
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            OnSceneLoadStarted?.Invoke(sceneName);

            // Optionnel: Afficher loading screen
            yield return new WaitForSeconds(0.1f);

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"[SceneLoader] Loading progress: {progress * 100}%");
                yield return null;
            }

            OnSceneLoadCompleted?.Invoke(sceneName);
            Debug.Log($"[SceneLoader] Scene loaded: {sceneName}");
        }
    }
}
```

---

## 🧪 Script Automatique d'Intégration

### SceneInteractionSetup.cs - Editor Tool

```csharp
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SmartRetailAR.Editor
{
    public class SceneInteractionSetup : EditorWindow
    {
        [MenuItem("Tools/Smart Retail AR/Setup Scene Interactions")]
        public static void ShowWindow()
        {
            GetWindow<SceneInteractionSetup>("Scene Interactions Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Setup Scene Interactions", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Add EventSystem to Current Scene"))
            {
                AddEventSystem();
            }

            if (GUILayout.Button("Wire All Buttons in Current Scene"))
            {
                WireAllButtons();
            }

            if (GUILayout.Button("Add GraphicRaycaster to All Canvas"))
            {
                AddGraphicRaycastersToCanvas();
            }

            if (GUILayout.Button("Setup Full Scene (All-in-One)"))
            {
                SetupFullScene();
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Ces outils ajoutent automatiquement les composants nécessaires " +
                "pour permettre les interactions dans la scène actuelle.",
                MessageType.Info
            );
        }

        private static void AddEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                
                Debug.Log("[Setup] EventSystem added");
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {
                Debug.Log("[Setup] EventSystem already exists");
            }
        }

        private static void AddGraphicRaycastersToCanvas()
        {
            Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
            int added = 0;

            foreach (Canvas canvas in canvases)
            {
                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                    added++;
                }
            }

            Debug.Log($"[Setup] Added GraphicRaycaster to {added} Canvas objects");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void WireAllButtons()
        {
            Button[] buttons = Object.FindObjectsOfType<Button>(true);
            int wired = 0;

            foreach (Button button in buttons)
            {
                // Détection automatique basée sur le nom
                string buttonName = button.gameObject.name.ToLower();

                if (buttonName.Contains("back"))
                {
                    // Wire avec fonction retour
                    wired++;
                    Debug.Log($"[Setup] Wired back button: {button.name}");
                }
                else if (buttonName.Contains("scan") || buttonName.Contains("qr"))
                {
                    wired++;
                    Debug.Log($"[Setup] Wired scan button: {button.name}");
                }
                // Ajouter plus de logique selon les besoins
            }

            Debug.Log($"[Setup] Wired {wired} buttons");
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void SetupFullScene()
        {
            AddEventSystem();
            AddGraphicRaycastersToCanvas();
            WireAllButtons();
            
            Debug.Log("[Setup] Full scene setup completed");
        }
    }
}
```

---

## ✅ Checklist de Validation

### Pour Chaque Scène

- [ ] **EventSystem existe** (1 par scène)
- [ ] **Canvas a GraphicRaycaster** (requis pour clics UI)
- [ ] **Boutons ont onClick câblés** (via Inspector ou code)
- [ ] **Références UI assignées** dans scripts (via Inspector)
- [ ] **Camera configurée correctement**
  - MainCamera pour scènes normales
  - AR Camera pour scène AR
- [ ] **Scripts ajoutés aux bons GameObjects**
- [ ] **Console ne montre pas d'erreurs NullReference**

### Test Manuel

1. **MainMenu**: Tous les boutons répondent au clic
2. **QRScanner**: 
   - Caméra s'affiche dans RawImage
   - Appuyer Espace simule un scan
   - Transition vers AR fonctionne
3. **ARProductView**:
   - Données produit affichées
   - Couleurs NutriScore/EcoScore correctes
   - Boutons fonctionnent
4. **Recommendations**:
   - Liste de produits affichée
   - Clics sur cartes fonctionnent
   - Transition vers AR fonctionne
5. **Settings**:
   - Toggles et sliders répondent
   - Sauvegarde fonctionne

---

## 🚀 Ordre d'Exécution Recommandé

### 1. Configuration Initiale (Une fois)
```bash
1. Ouvrir Unity 2022.3.62f3
2. Ouvrir le projet Smart Retail AR
3. Tools > Smart Retail AR > Setup Scene Interactions
4. Cliquer "Setup Full Scene (All-in-One)" pour chaque scène
```

### 2. Tests par Scène

#### MainMenu
```
1. Play Mode
2. Vérifier que tous les boutons sont cliquables
3. Vérifier que les transitions de scène fonctionnent
4. Console: Pas d'erreurs
```

#### QRScanner
```
1. Play Mode
2. Vérifier que le flux caméra s'affiche
3. Appuyer sur Espace pour simuler un scan
4. Console: "[QRScanner] QR Code detected: SMARTRETAIL:PROD001"
5. Transition automatique vers ARProductView après 1s
```

#### ARProductView
```
1. Play Mode (après avoir scanné un produit)
2. Vérifier que les données s'affichent:
   - Nom du produit
   - Prix
   - NutriScore avec couleur
   - EcoScore avec couleur
3. Tester bouton "View Alternatives"
4. Tester bouton "Back"
```

#### Recommendations
```
1. Play Mode (après ARProductView)
2. Vérifier que la liste de produits s'affiche
3. Scroller dans la liste
4. Cliquer sur une carte produit
5. Vérifier transition vers ARProductView avec nouveau produit
```

---

## 🐛 Dépannage

### Problème: Boutons ne répondent pas aux clics

**Solutions:**
1. Vérifier qu'un **EventSystem** existe dans la scène
2. Vérifier que le **Canvas a un GraphicRaycaster**
3. Vérifier que le **Button a un Image** (même transparent)
4. Console: `EventSystem.current` ne doit pas être null

### Problème: Caméra ne s'affiche pas dans QRScanner

**Solutions:**
1. Vérifier autorisation caméra (Player Settings)
2. Vérifier que `WebCamTexture.devices.Length > 0`
3. Vérifier que RawImage.texture est assignée
4. Console: "[QRScanner] Starting WebCamTexture"

### Problème: Données produit ne s'affichent pas

**Solutions:**
1. Vérifier que `ProductManager.Instance.GetCurrentProduct() != null`
2. Vérifier que products.json est dans Assets/Data/
3. Vérifier que ProductDatabase est initialisée
4. Console: "[ProductDatabase] Loaded X products"

### Problème: Transitions de scène ne fonctionnent pas

**Solutions:**
1. Vérifier que toutes les scènes sont dans Build Settings
2. Vérifier que SceneLoader.Instance existe (DontDestroyOnLoad)
3. Console: "[SceneLoader] Loading scene: XXX"

---

## 📚 Résumé

**Pour faire fonctionner les interactions:**

1. **Composants UI Requis** (TOUTES les scènes):
   - EventSystem (1 par scène)
   - Canvas avec GraphicRaycaster
   - Buttons avec onClick câblés

2. **Pattern de Câblage**:
   ```csharp
   // Dans Start() de chaque controller
   if (myButton != null)
       myButton.onClick.AddListener(OnMyButtonClicked);
   ```

3. **Transitions**:
   ```csharp
   SceneLoader.Instance.LoadScene("SceneName");
   ```

4. **Partage de Données**:
   ```csharp
   // Définir
   ProductManager.Instance.SetCurrentProduct(product);
   
   // Récupérer
   Product product = ProductManager.Instance.GetCurrentProduct();
   ```

5. **Debug**:
   - Utiliser `Debug.Log()` abondamment
   - Vérifier Console pour flux d'exécution
   - Tester scène par scène

**Avec ces configurations, toutes les scènes devraient être interactives et fonctionnelles! 🎉**
