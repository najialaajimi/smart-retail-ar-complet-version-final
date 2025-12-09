using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Data;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// AR Product Overlay displays product information in augmented reality.
    /// This component renders nutrition info, origin, eco-score, and alternatives.
    /// </summary>
    public class ARProductOverlay : MonoBehaviour
    {
        [Header("Main Panel")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Canvas worldSpaceCanvas;
        
        [Header("Product Info")]
        [SerializeField] private TextMeshProUGUI productNameText;
        [SerializeField] private TextMeshProUGUI brandText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI originText;
        [SerializeField] private RawImage productImage;
        
        [Header("Nutrition Info")]
        [SerializeField] private GameObject nutritionPanel;
        [SerializeField] private TextMeshProUGUI caloriesText;
        [SerializeField] private TextMeshProUGUI proteinsText;
        [SerializeField] private TextMeshProUGUI carbsText;
        [SerializeField] private TextMeshProUGUI fatsText;
        [SerializeField] private TextMeshProUGUI nutriScoreText;
        [SerializeField] private Image nutriScoreBackground;
        
        [Header("Eco Info")]
        [SerializeField] private GameObject ecoPanel;
        [SerializeField] private TextMeshProUGUI ecoScoreText;
        [SerializeField] private Image ecoScoreBackground;
        [SerializeField] private GameObject bioIcon;
        [SerializeField] private GameObject localIcon;
        [SerializeField] private GameObject recyclableIcon;
        
        [Header("Allergens")]
        [SerializeField] private GameObject allergensPanel;
        [SerializeField] private TextMeshProUGUI allergensText;
        
        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float scaleAnimationDuration = 0.2f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Header("Behavior")]
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private float smoothFollowSpeed = 5f;
        
        private Product currentProduct;
        private float creationTime;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private bool isInitialized;
        
        public Product Product => currentProduct;
        public float CreationTime => creationTime;
        
        private void Awake()
        {
            // Ensure we have a canvas group for fading
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            // Get or create world space canvas
            if (worldSpaceCanvas == null)
            {
                worldSpaceCanvas = GetComponentInChildren<Canvas>();
            }
        }
        
        /// <summary>
        /// Initializes the overlay with product data.
        /// </summary>
        public void Initialize(Product product, Vector3 position, Quaternion rotation)
        {
            currentProduct = product;
            creationTime = Time.time;
            targetPosition = position;
            targetRotation = rotation;
            
            transform.position = position;
            transform.rotation = rotation;
            
            UpdateDisplay();
            Show();
            
            isInitialized = true;
        }
        
        /// <summary>
        /// Updates the display with current product data.
        /// </summary>
        private void UpdateDisplay()
        {
            if (currentProduct == null) return;
            
            // Basic info
            SetText(productNameText, currentProduct.name);
            SetText(brandText, currentProduct.brand);
            SetText(priceText, $"{currentProduct.price:F2} {currentProduct.currency}");
            SetText(originText, $"Origin: {currentProduct.origin}");
            
            // Load product image
            LoadProductImage();
            
            // Nutrition info
            UpdateNutritionDisplay();
            
            // Eco info
            UpdateEcoDisplay();
            
            // Allergens
            UpdateAllergensDisplay();
        }
        
        /// <summary>
        /// Updates the nutrition panel display.
        /// </summary>
        private void UpdateNutritionDisplay()
        {
            if (currentProduct?.nutrition == null) return;
            
            var nutrition = currentProduct.nutrition;
            
            SetText(caloriesText, $"{nutrition.calories:F0} kcal");
            SetText(proteinsText, $"Proteins: {nutrition.proteins:F1}g");
            SetText(carbsText, $"Carbs: {nutrition.carbohydrates:F1}g");
            SetText(fatsText, $"Fats: {nutrition.fats:F1}g");
            SetText(nutriScoreText, nutrition.nutriScore);
            
            if (nutriScoreBackground != null)
            {
                nutriScoreBackground.color = nutrition.GetNutriScoreColor();
            }
        }
        
        /// <summary>
        /// Updates the eco-responsibility panel display.
        /// </summary>
        private void UpdateEcoDisplay()
        {
            if (currentProduct?.ecoInfo == null) return;
            
            var ecoInfo = currentProduct.ecoInfo;
            
            SetText(ecoScoreText, ecoInfo.GetEcoGrade());
            
            if (ecoScoreBackground != null)
            {
                ecoScoreBackground.color = ecoInfo.GetEcoScoreColor();
            }
            
            // Toggle icons
            if (bioIcon != null) bioIcon.SetActive(ecoInfo.isBio);
            if (localIcon != null) localIcon.SetActive(ecoInfo.isLocalProduct);
            if (recyclableIcon != null) recyclableIcon.SetActive(ecoInfo.isRecyclablePackaging);
        }
        
        /// <summary>
        /// Updates the allergens display.
        /// </summary>
        private void UpdateAllergensDisplay()
        {
            if (currentProduct?.nutrition?.allergens == null || currentProduct.nutrition.allergens.Count == 0)
            {
                if (allergensPanel != null)
                {
                    allergensPanel.SetActive(false);
                }
                return;
            }
            
            if (allergensPanel != null)
            {
                allergensPanel.SetActive(true);
            }
            
            string allergensList = string.Join(", ", currentProduct.nutrition.allergens);
            SetText(allergensText, $"⚠️ Contains: {allergensList}");
        }
        
        /// <summary>
        /// Loads the product image from resources.
        /// </summary>
        private void LoadProductImage()
        {
            if (productImage == null || string.IsNullOrEmpty(currentProduct?.imagePath)) return;
            
            Texture2D texture = Resources.Load<Texture2D>(currentProduct.imagePath);
            if (texture != null)
            {
                productImage.texture = texture;
            }
        }
        
        /// <summary>
        /// Shows the overlay with animation.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowAnimation());
        }
        
        /// <summary>
        /// Hides the overlay with animation.
        /// </summary>
        public void Hide()
        {
            StartCoroutine(HideAnimation());
        }
        
        /// <summary>
        /// Animation coroutine for showing the overlay.
        /// </summary>
        private IEnumerator ShowAnimation()
        {
            // Initial state
            canvasGroup.alpha = 0;
            transform.localScale = Vector3.zero;
            
            // Animate
            float elapsed = 0;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;
                
                canvasGroup.alpha = t;
                transform.localScale = Vector3.one * scaleCurve.Evaluate(t);
                
                yield return null;
            }
            
            // Final state
            canvasGroup.alpha = 1;
            transform.localScale = Vector3.one;
        }
        
        /// <summary>
        /// Animation coroutine for hiding the overlay.
        /// </summary>
        private IEnumerator HideAnimation()
        {
            float elapsed = 0;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1 - (elapsed / fadeInDuration);
                
                canvasGroup.alpha = t;
                transform.localScale = Vector3.one * t;
                
                yield return null;
            }
            
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Updates the overlay position with smooth interpolation.
        /// </summary>
        public void UpdatePosition(Vector3 position, Quaternion rotation)
        {
            targetPosition = position;
            targetRotation = rotation;
        }
        
        /// <summary>
        /// Makes the overlay face the camera.
        /// </summary>
        public void FaceCamera(Camera camera)
        {
            if (!faceCamera || camera == null) return;
            
            Vector3 directionToCamera = camera.transform.position - transform.position;
            targetRotation = Quaternion.LookRotation(-directionToCamera);
        }
        
        private void Update()
        {
            if (!isInitialized) return;
            
            // Smooth position and rotation interpolation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothFollowSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothFollowSpeed);
        }
        
        /// <summary>
        /// Helper method to safely set text.
        /// </summary>
        private void SetText(TextMeshProUGUI textComponent, string value)
        {
            if (textComponent != null)
            {
                textComponent.text = value ?? "";
            }
        }
        
        /// <summary>
        /// Expands or collapses the nutrition panel.
        /// </summary>
        public void ToggleNutritionPanel()
        {
            if (nutritionPanel != null)
            {
                nutritionPanel.SetActive(!nutritionPanel.activeSelf);
            }
        }
        
        /// <summary>
        /// Expands or collapses the eco panel.
        /// </summary>
        public void ToggleEcoPanel()
        {
            if (ecoPanel != null)
            {
                ecoPanel.SetActive(!ecoPanel.activeSelf);
            }
        }
    }
}
