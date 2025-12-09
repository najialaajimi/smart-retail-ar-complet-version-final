using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Data;
using SmartRetailAR.AR;
using SmartRetailAR.Recommendations;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Main UI Manager for the Smart Retail AR application.
    /// Handles all screen transitions and UI state management.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }
                return _instance;
            }
        }
        
        [Header("Screens")]
        [SerializeField] private GameObject splashScreen;
        [SerializeField] private GameObject mainMenuScreen;
        [SerializeField] private GameObject scannerScreen;
        [SerializeField] private GameObject productDetailScreen;
        [SerializeField] private GameObject recommendationsScreen;
        [SerializeField] private GameObject searchScreen;
        [SerializeField] private GameObject settingsScreen;
        
        [Header("Loading")]
        [SerializeField] private GameObject loadingOverlay;
        [SerializeField] private TextMeshProUGUI loadingText;
        [SerializeField] private Slider loadingProgress;
        
        [Header("Notifications")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float notificationDuration = 3f;
        
        [Header("Product Detail UI")]
        [SerializeField] private TextMeshProUGUI detailProductName;
        [SerializeField] private TextMeshProUGUI detailBrand;
        [SerializeField] private TextMeshProUGUI detailPrice;
        [SerializeField] private TextMeshProUGUI detailOrigin;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private RawImage detailProductImage;
        [SerializeField] private Transform nutritionInfoContainer;
        [SerializeField] private Transform ecoInfoContainer;
        [SerializeField] private Transform alternativesContainer;
        
        [Header("Search UI")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Transform searchResultsContainer;
        [SerializeField] private GameObject searchResultItemPrefab;
        
        [Header("Settings UI")]
        [SerializeField] private Toggle bioPreferenceToggle;
        [SerializeField] private Toggle localPreferenceToggle;
        [SerializeField] private Toggle ecoPreferenceToggle;
        [SerializeField] private Toggle pricePreferenceToggle;
        [SerializeField] private Toggle nutriPreferenceToggle;
        
        [Header("Animation")]
        [SerializeField] private float screenTransitionDuration = 0.3f;
        
        private GameObject currentScreen;
        private Product currentProduct;
        private Stack<GameObject> screenHistory;
        private Coroutine notificationCoroutine;
        
        public event Action<string> OnScreenChanged;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            screenHistory = new Stack<GameObject>();
        }
        
        private void Start()
        {
            // Initialize UI state
            HideAllScreens();
            
            // Show splash screen
            ShowScreen(splashScreen);
            
            // Subscribe to settings toggles
            SetupSettingsListeners();
            
            // Auto-transition to main menu after splash
            StartCoroutine(SplashSequence());
        }
        
        /// <summary>
        /// Splash screen sequence before showing main menu.
        /// </summary>
        private IEnumerator SplashSequence()
        {
            yield return new WaitForSeconds(2f);
            ShowMainMenu();
        }
        
        /// <summary>
        /// Sets up listeners for settings toggles.
        /// </summary>
        private void SetupSettingsListeners()
        {
            if (bioPreferenceToggle != null)
            {
                bioPreferenceToggle.onValueChanged.AddListener(OnBioPreferenceChanged);
            }
            if (localPreferenceToggle != null)
            {
                localPreferenceToggle.onValueChanged.AddListener(OnLocalPreferenceChanged);
            }
            if (ecoPreferenceToggle != null)
            {
                ecoPreferenceToggle.onValueChanged.AddListener(OnEcoPreferenceChanged);
            }
            if (pricePreferenceToggle != null)
            {
                pricePreferenceToggle.onValueChanged.AddListener(OnPricePreferenceChanged);
            }
            if (nutriPreferenceToggle != null)
            {
                nutriPreferenceToggle.onValueChanged.AddListener(OnNutriPreferenceChanged);
            }
            
            // Load saved preferences
            LoadPreferencesUI();
        }
        
        /// <summary>
        /// Hides all screens.
        /// </summary>
        private void HideAllScreens()
        {
            if (splashScreen != null) splashScreen.SetActive(false);
            if (mainMenuScreen != null) mainMenuScreen.SetActive(false);
            if (scannerScreen != null) scannerScreen.SetActive(false);
            if (productDetailScreen != null) productDetailScreen.SetActive(false);
            if (recommendationsScreen != null) recommendationsScreen.SetActive(false);
            if (searchScreen != null) searchScreen.SetActive(false);
            if (settingsScreen != null) settingsScreen.SetActive(false);
            if (loadingOverlay != null) loadingOverlay.SetActive(false);
            if (notificationPanel != null) notificationPanel.SetActive(false);
        }
        
        /// <summary>
        /// Shows a specific screen with transition animation.
        /// </summary>
        private void ShowScreen(GameObject screen, bool addToHistory = true)
        {
            if (screen == null) return;
            
            if (currentScreen != null && addToHistory)
            {
                screenHistory.Push(currentScreen);
                currentScreen.SetActive(false);
            }
            else if (currentScreen != null)
            {
                currentScreen.SetActive(false);
            }
            
            currentScreen = screen;
            StartCoroutine(TransitionToScreen(screen));
            
            OnScreenChanged?.Invoke(screen.name);
        }
        
        /// <summary>
        /// Screen transition animation coroutine.
        /// </summary>
        private IEnumerator TransitionToScreen(GameObject screen)
        {
            screen.SetActive(true);
            
            CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = screen.AddComponent<CanvasGroup>();
            }
            
            canvasGroup.alpha = 0;
            
            float elapsed = 0;
            while (elapsed < screenTransitionDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = elapsed / screenTransitionDuration;
                yield return null;
            }
            
            canvasGroup.alpha = 1;
        }
        
        /// <summary>
        /// Navigates back to the previous screen.
        /// </summary>
        public void NavigateBack()
        {
            if (screenHistory.Count > 0)
            {
                GameObject previousScreen = screenHistory.Pop();
                ShowScreen(previousScreen, false);
            }
            else
            {
                ShowMainMenu();
            }
        }
        
        // Screen Navigation Methods
        
        public void ShowMainMenu()
        {
            screenHistory.Clear();
            ShowScreen(mainMenuScreen, false);
        }
        
        public void ShowScanner()
        {
            ShowScreen(scannerScreen);
        }
        
        public void ShowProductDetail(Product product)
        {
            currentProduct = product;
            UpdateProductDetailUI(product);
            ShowScreen(productDetailScreen);
        }
        
        public void ShowRecommendations(Product product)
        {
            currentProduct = product;
            ShowScreen(recommendationsScreen);
            LoadRecommendations(product);
        }
        
        public void ShowSearch()
        {
            ShowScreen(searchScreen);
            if (searchInput != null)
            {
                searchInput.Select();
            }
        }
        
        public void ShowSettings()
        {
            LoadPreferencesUI();
            ShowScreen(settingsScreen);
        }
        
        /// <summary>
        /// Updates the product detail UI with product data.
        /// </summary>
        private void UpdateProductDetailUI(Product product)
        {
            if (product == null) return;
            
            SetText(detailProductName, product.name);
            SetText(detailBrand, product.brand);
            SetText(detailPrice, $"{product.price:F2} {product.currency}");
            SetText(detailOrigin, $"Origin: {product.origin}");
            SetText(detailDescription, product.description);
            
            // Load image
            if (detailProductImage != null && !string.IsNullOrEmpty(product.imagePath))
            {
                Texture2D texture = Resources.Load<Texture2D>(product.imagePath);
                if (texture != null)
                {
                    detailProductImage.texture = texture;
                }
            }
            
            // Update nutrition info
            UpdateNutritionUI(product.nutrition);
            
            // Update eco info
            UpdateEcoUI(product.ecoInfo);
        }
        
        /// <summary>
        /// Updates the nutrition information UI.
        /// </summary>
        private void UpdateNutritionUI(NutritionalInfo nutrition)
        {
            if (nutrition == null || nutritionInfoContainer == null) return;
            
            // Clear existing items
            foreach (Transform child in nutritionInfoContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create nutrition items (using TextMeshProUGUI for each item)
            CreateNutritionItem($"Calories: {nutrition.calories:F0} kcal");
            CreateNutritionItem($"Proteins: {nutrition.proteins:F1}g");
            CreateNutritionItem($"Carbohydrates: {nutrition.carbohydrates:F1}g");
            CreateNutritionItem($"Sugars: {nutrition.sugars:F1}g");
            CreateNutritionItem($"Fats: {nutrition.fats:F1}g");
            CreateNutritionItem($"Saturated Fats: {nutrition.saturatedFats:F1}g");
            CreateNutritionItem($"Fiber: {nutrition.fiber:F1}g");
            CreateNutritionItem($"Salt: {nutrition.salt:F2}g");
            CreateNutritionItem($"Nutri-Score: {nutrition.nutriScore}");
        }
        
        /// <summary>
        /// Creates a nutrition item UI element.
        /// </summary>
        private void CreateNutritionItem(string text)
        {
            if (nutritionInfoContainer == null) return;
            
            GameObject item = new GameObject("NutritionItem");
            item.transform.SetParent(nutritionInfoContainer);
            
            TextMeshProUGUI tmpText = item.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 14;
            tmpText.color = Color.black;
            
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 30);
        }
        
        /// <summary>
        /// Updates the eco information UI.
        /// </summary>
        private void UpdateEcoUI(EcoInfo ecoInfo)
        {
            if (ecoInfo == null || ecoInfoContainer == null) return;
            
            // Clear existing items
            foreach (Transform child in ecoInfoContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create eco items
            CreateEcoItem($"Eco-Score: {ecoInfo.GetEcoGrade()} ({ecoInfo.ecoScore}/100)");
            if (ecoInfo.isBio) CreateEcoItem("✓ Organic/Bio Product");
            if (ecoInfo.isLocalProduct) CreateEcoItem("✓ Local Product");
            if (ecoInfo.isRecyclablePackaging) CreateEcoItem("✓ Recyclable Packaging");
            CreateEcoItem($"Carbon Footprint: {ecoInfo.carbonFootprint:F2} kg CO2");
            if (!string.IsNullOrEmpty(ecoInfo.certifications))
            {
                CreateEcoItem($"Certifications: {ecoInfo.certifications}");
            }
        }
        
        /// <summary>
        /// Creates an eco info item UI element.
        /// </summary>
        private void CreateEcoItem(string text)
        {
            if (ecoInfoContainer == null) return;
            
            GameObject item = new GameObject("EcoItem");
            item.transform.SetParent(ecoInfoContainer);
            
            TextMeshProUGUI tmpText = item.AddComponent<TextMeshProUGUI>();
            tmpText.text = text;
            tmpText.fontSize = 14;
            tmpText.color = new Color(0.1f, 0.5f, 0.1f);
            
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 30);
        }
        
        /// <summary>
        /// Loads and displays recommendations for a product.
        /// </summary>
        private void LoadRecommendations(Product product)
        {
            if (alternativesContainer == null) return;
            
            // Clear existing
            foreach (Transform child in alternativesContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Get recommendations
            var recommendations = RecommendationEngine.Instance.GetRecommendations(product);
            
            foreach (var rec in recommendations)
            {
                CreateRecommendationItem(rec);
            }
        }
        
        /// <summary>
        /// Creates a recommendation item UI element.
        /// </summary>
        private void CreateRecommendationItem(RecommendationResult recommendation)
        {
            if (alternativesContainer == null) return;
            
            GameObject item = new GameObject("RecommendationItem");
            item.transform.SetParent(alternativesContainer);
            
            // Add background
            Image bg = item.AddComponent<Image>();
            bg.color = recommendation.isBetterChoice ? new Color(0.9f, 1f, 0.9f) : Color.white;
            
            // Add text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(item.transform);
            
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            string reasonsStr = string.Join(", ", recommendation.reasons);
            tmpText.text = $"<b>{recommendation.product.name}</b>\n{recommendation.product.brand} - {recommendation.product.price:F2} {recommendation.product.currency}\n{reasonsStr}";
            tmpText.fontSize = 12;
            tmpText.color = Color.black;
            
            // Add button functionality
            Button btn = item.AddComponent<Button>();
            Product prod = recommendation.product;
            btn.onClick.AddListener(() => ShowProductDetail(prod));
            
            RectTransform rect = item.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(300, 80);
        }
        
        /// <summary>
        /// Performs a product search.
        /// </summary>
        public void PerformSearch()
        {
            if (searchInput == null || searchResultsContainer == null) return;
            
            string query = searchInput.text;
            var results = ProductManager.Instance.SearchProducts(query);
            
            // Clear existing results
            foreach (Transform child in searchResultsContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Display results
            foreach (var product in results)
            {
                CreateSearchResultItem(product);
            }
            
            if (results.Count == 0)
            {
                ShowNotification("No products found");
            }
        }
        
        /// <summary>
        /// Creates a search result item UI element.
        /// </summary>
        private void CreateSearchResultItem(Product product)
        {
            if (searchResultsContainer == null) return;
            
            GameObject item;
            if (searchResultItemPrefab != null)
            {
                item = Instantiate(searchResultItemPrefab, searchResultsContainer);
            }
            else
            {
                item = new GameObject("SearchResultItem");
                item.transform.SetParent(searchResultsContainer);
                
                Image bg = item.AddComponent<Image>();
                bg.color = Color.white;
                
                GameObject textObj = new GameObject("Text");
                textObj.transform.SetParent(item.transform);
                
                TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
                tmpText.text = $"{product.name} - {product.brand}\n{product.price:F2} {product.currency}";
                tmpText.fontSize = 14;
                tmpText.color = Color.black;
            }
            
            // Add button functionality
            Button btn = item.GetComponent<Button>();
            if (btn == null) btn = item.AddComponent<Button>();
            
            Product prod = product;
            btn.onClick.AddListener(() => ShowProductDetail(prod));
            
            RectTransform rect = item.GetComponent<RectTransform>();
            if (rect != null) rect.sizeDelta = new Vector2(350, 60);
        }
        
        // Loading UI Methods
        
        public void ShowLoading(string message = "Loading...")
        {
            if (loadingOverlay != null)
            {
                loadingOverlay.SetActive(true);
                SetText(loadingText, message);
                if (loadingProgress != null) loadingProgress.value = 0;
            }
        }
        
        public void UpdateLoading(float progress, string message = null)
        {
            if (loadingProgress != null) loadingProgress.value = progress;
            if (message != null) SetText(loadingText, message);
        }
        
        public void HideLoading()
        {
            if (loadingOverlay != null) loadingOverlay.SetActive(false);
        }
        
        // Notification Methods
        
        public void ShowNotification(string message)
        {
            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }
            notificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message));
        }
        
        private IEnumerator ShowNotificationCoroutine(string message)
        {
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(true);
                SetText(notificationText, message);
                
                yield return new WaitForSeconds(notificationDuration);
                
                notificationPanel.SetActive(false);
            }
        }
        
        // Settings Preference Handlers
        
        private void OnBioPreferenceChanged(bool value)
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            prefs.preferBio = value;
            RecommendationEngine.Instance.SetUserPreferences(prefs);
        }
        
        private void OnLocalPreferenceChanged(bool value)
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            prefs.preferLocal = value;
            RecommendationEngine.Instance.SetUserPreferences(prefs);
        }
        
        private void OnEcoPreferenceChanged(bool value)
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            prefs.preferEcoFriendly = value;
            RecommendationEngine.Instance.SetUserPreferences(prefs);
        }
        
        private void OnPricePreferenceChanged(bool value)
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            prefs.preferLowerPrice = value;
            RecommendationEngine.Instance.SetUserPreferences(prefs);
        }
        
        private void OnNutriPreferenceChanged(bool value)
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            prefs.preferBetterNutriScore = value;
            RecommendationEngine.Instance.SetUserPreferences(prefs);
        }
        
        private void LoadPreferencesUI()
        {
            var prefs = RecommendationEngine.Instance.GetUserPreferences();
            
            if (bioPreferenceToggle != null) bioPreferenceToggle.isOn = prefs.preferBio;
            if (localPreferenceToggle != null) localPreferenceToggle.isOn = prefs.preferLocal;
            if (ecoPreferenceToggle != null) ecoPreferenceToggle.isOn = prefs.preferEcoFriendly;
            if (pricePreferenceToggle != null) pricePreferenceToggle.isOn = prefs.preferLowerPrice;
            if (nutriPreferenceToggle != null) nutriPreferenceToggle.isOn = prefs.preferBetterNutriScore;
        }
        
        // Helper Methods
        
        private void SetText(TextMeshProUGUI textComponent, string value)
        {
            if (textComponent != null)
            {
                textComponent.text = value ?? "";
            }
        }
    }
}
