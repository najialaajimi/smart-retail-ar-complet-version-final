using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Products;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel d'affichage des informations détaillées d'un produit
    /// </summary>
    public class ProductInfoPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI productNameText;
        [SerializeField] private TextMeshProUGUI productBrandText;
        [SerializeField] private TextMeshProUGUI productDescriptionText;
        [SerializeField] private TextMeshProUGUI productPriceText;
        [SerializeField] private TextMeshProUGUI productOriginText;
        [SerializeField] private TextMeshProUGUI productCategoryText;
        [SerializeField] private Image productImage;
        [SerializeField] private NutriScoreDisplay nutriScoreDisplay;
        [SerializeField] private EcoScoreDisplay ecoScoreDisplay;
        [SerializeField] private Toggle bioToggle;
        [SerializeField] private Toggle ecoResponsibleToggle;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button viewAlternativesButton;

        [Header("Nutrition Details")]
        [SerializeField] private TextMeshProUGUI caloriesText;
        [SerializeField] private TextMeshProUGUI proteinsText;
        [SerializeField] private TextMeshProUGUI carbsText;
        [SerializeField] private TextMeshProUGUI fatsText;

        private Product currentProduct;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (viewAlternativesButton != null)
            {
                viewAlternativesButton.onClick.AddListener(OnViewAlternatives);
            }

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Affiche le panel avec les informations du produit
        /// </summary>
        public void ShowProduct(Product product)
        {
            if (product == null)
            {
                Debug.LogWarning("Produit null passé à ShowProduct");
                return;
            }

            currentProduct = product;
            UpdateUI();

            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// Met à jour l'interface avec les données du produit
        /// </summary>
        private void UpdateUI()
        {
            if (currentProduct == null)
            {
                return;
            }

            // Informations de base
            SetText(productNameText, currentProduct.name);
            SetText(productBrandText, currentProduct.brand);
            SetText(productDescriptionText, currentProduct.description);
            SetText(productPriceText, $"{currentProduct.price:F2} €");
            SetText(productOriginText, $"Origine: {currentProduct.origin}");
            SetText(productCategoryText, $"Catégorie: {currentProduct.category}");

            // Scores
            if (nutriScoreDisplay != null && currentProduct.nutrition != null)
            {
                nutriScoreDisplay.SetNutriScore(currentProduct.nutrition.nutriScore);
            }

            if (ecoScoreDisplay != null)
            {
                ecoScoreDisplay.SetEcoScore(currentProduct.ecoScore);
            }

            // Toggles
            if (bioToggle != null)
            {
                bioToggle.isOn = currentProduct.isBio;
            }

            if (ecoResponsibleToggle != null)
            {
                ecoResponsibleToggle.isOn = currentProduct.isEcoResponsible;
            }

            // Informations nutritionnelles
            if (currentProduct.nutrition != null)
            {
                SetText(caloriesText, $"Calories: {currentProduct.nutrition.calories} kcal");
                SetText(proteinsText, $"Protéines: {currentProduct.nutrition.proteins}g");
                SetText(carbsText, $"Glucides: {currentProduct.nutrition.carbohydrates}g");
                SetText(fatsText, $"Lipides: {currentProduct.nutrition.fats}g");
            }
        }

        /// <summary>
        /// Méthode helper pour définir le texte
        /// </summary>
        private void SetText(TextMeshProUGUI textComponent, string text)
        {
            if (textComponent != null)
            {
                textComponent.text = text;
            }
        }

        /// <summary>
        /// Cache le panel
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Affiche les alternatives du produit
        /// </summary>
        private void OnViewAlternatives()
        {
            if (currentProduct != null)
            {
                Debug.Log($"Affichage des alternatives pour: {currentProduct.name}");
                // L'interface de recommandations sera gérée par RecommendationUI
            }
        }

        /// <summary>
        /// Récupère le produit actuellement affiché
        /// </summary>
        public Product GetCurrentProduct()
        {
            return currentProduct;
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }

            if (viewAlternativesButton != null)
            {
                viewAlternativesButton.onClick.RemoveAllListeners();
            }
        }
    }
}
