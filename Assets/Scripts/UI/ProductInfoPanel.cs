using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Data;
using SmartRetailAR.Utils;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel d'affichage des informations détaillées d'un produit
    /// </summary>
    public class ProductInfoPanel : MonoBehaviour
    {
        [Header("Informations de base")]
        [SerializeField] private Text productNameText;
        [SerializeField] private Text brandText;
        [SerializeField] private Text priceText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text originText;
        [SerializeField] private Text categoryText;
        [SerializeField] private Image productImage;

        [Header("Nutrition")]
        [SerializeField] private Text caloriesText;
        [SerializeField] private Text proteinsText;
        [SerializeField] private Text carbsText;
        [SerializeField] private Text fatText;
        [SerializeField] private Text fiberText;
        [SerializeField] private Text sugarText;
        [SerializeField] private Text saltText;

        [Header("Scores")]
        [SerializeField] private Text nutriScoreText;
        [SerializeField] private Image nutriScoreIcon;
        [SerializeField] private Text ecoScoreText;
        [SerializeField] private Image ecoScoreIcon;

        [Header("Labels")]
        [SerializeField] private GameObject bioLabel;
        [SerializeField] private GameObject veganLabel;

        [Header("Allergènes")]
        [SerializeField] private Text allergensText;
        [SerializeField] private Transform allergensContainer;

        // Produit actuel
        private ProductData m_CurrentProduct;

        /// <summary>
        /// Affiche les informations d'un produit
        /// </summary>
        public void ShowProduct(ProductData product)
        {
            if (product == null)
            {
                DebugLogger.LogWarning("Produit null, impossible d'afficher les informations");
                return;
            }

            m_CurrentProduct = product;
            UpdateDisplay();

            DebugLogger.Log($"Affichage des informations pour: {product.name}", LogLevel.Info);
        }

        /// <summary>
        /// Met à jour l'affichage avec les données du produit
        /// </summary>
        private void UpdateDisplay()
        {
            if (m_CurrentProduct == null)
                return;

            // Informations de base
            SetText(productNameText, m_CurrentProduct.name);
            SetText(brandText, m_CurrentProduct.brand);
            SetText(priceText, $"{m_CurrentProduct.price:F2}€");
            SetText(descriptionText, m_CurrentProduct.description);
            SetText(originText, $"Origine: {m_CurrentProduct.origin}");
            SetText(categoryText, m_CurrentProduct.category);

            // Nutrition
            if (m_CurrentProduct.nutrition != null)
            {
                SetText(caloriesText, $"{m_CurrentProduct.nutrition.calories} kcal");
                SetText(proteinsText, $"Protéines: {m_CurrentProduct.nutrition.proteins}g");
                SetText(carbsText, $"Glucides: {m_CurrentProduct.nutrition.carbs}g");
                SetText(fatText, $"Lipides: {m_CurrentProduct.nutrition.fat}g");
                SetText(fiberText, $"Fibres: {m_CurrentProduct.nutrition.fiber}g");
                SetText(sugarText, $"Sucres: {m_CurrentProduct.nutrition.sugar}g");
                SetText(saltText, $"Sel: {m_CurrentProduct.nutrition.salt}g");

                // Nutri-Score
                SetText(nutriScoreText, $"Nutri-Score: {m_CurrentProduct.nutrition.nutriscore}");
                if (nutriScoreIcon != null)
                {
                    nutriScoreIcon.color = GetScoreColor(m_CurrentProduct.nutrition.nutriscore);
                }
            }

            // Eco-Score
            SetText(ecoScoreText, $"Eco-Score: {m_CurrentProduct.ecoScore}");
            if (ecoScoreIcon != null)
            {
                ecoScoreIcon.color = GetScoreColor(m_CurrentProduct.ecoScore);
            }

            // Labels
            if (bioLabel != null)
            {
                bioLabel.SetActive(m_CurrentProduct.isBio);
            }

            if (veganLabel != null)
            {
                veganLabel.SetActive(m_CurrentProduct.isVegan);
            }

            // Allergènes
            if (m_CurrentProduct.allergens != null && m_CurrentProduct.allergens.Count > 0)
            {
                string allergensStr = "Allergènes: " + string.Join(", ", m_CurrentProduct.allergens);
                SetText(allergensText, allergensStr);
            }
            else
            {
                SetText(allergensText, "Aucun allergène");
            }
        }

        /// <summary>
        /// Définit le texte d'un composant Text (avec vérification null)
        /// </summary>
        private void SetText(Text textComponent, string value)
        {
            if (textComponent != null)
            {
                textComponent.text = value ?? "";
            }
        }

        /// <summary>
        /// Obtient la couleur correspondant à un score
        /// </summary>
        private Color GetScoreColor(string score)
        {
            switch (score?.ToUpper())
            {
                case "A": return new Color(0.0f, 0.6f, 0.0f); // Vert foncé
                case "B": return new Color(0.5f, 0.8f, 0.0f); // Vert clair
                case "C": return new Color(1.0f, 0.8f, 0.0f); // Jaune
                case "D": return new Color(1.0f, 0.5f, 0.0f); // Orange
                case "E": return new Color(0.8f, 0.0f, 0.0f); // Rouge
                default: return Color.gray;
            }
        }

        /// <summary>
        /// Récupère le produit actuellement affiché
        /// </summary>
        public ProductData GetCurrentProduct()
        {
            return m_CurrentProduct;
        }
    }
}
