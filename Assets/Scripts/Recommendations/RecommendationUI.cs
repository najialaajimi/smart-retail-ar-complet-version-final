using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Products;

namespace SmartRetailAR.Recommendations
{
    /// <summary>
    /// Gère l'interface utilisateur des recommandations
    /// </summary>
    public class RecommendationUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject recommendationPanel;
        [SerializeField] private Transform recommendationContainer;
        [SerializeField] private GameObject recommendationCardPrefab;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button closeButton;

        [Header("Configuration")]
        [SerializeField] private RecommendationCriteria displayCriteria = RecommendationCriteria.Combined;

        private List<GameObject> instantiatedCards = new List<GameObject>();
        private Product currentProduct;

        private void Awake()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (recommendationPanel != null)
            {
                recommendationPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Affiche les recommandations pour un produit
        /// </summary>
        public void ShowRecommendations(Product product, RecommendationCriteria criteria = RecommendationCriteria.Combined)
        {
            if (product == null)
            {
                Debug.LogWarning("Produit null passé à ShowRecommendations");
                return;
            }

            currentProduct = product;
            displayCriteria = criteria;

            // Obtenir les recommandations
            List<Product> recommendations = RecommendationEngine.Instance.GetAlternatives(product, criteria);

            // Afficher l'UI
            DisplayRecommendations(recommendations);

            if (recommendationPanel != null)
            {
                recommendationPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Affiche la liste de recommandations
        /// </summary>
        private void DisplayRecommendations(List<Product> recommendations)
        {
            // Nettoyer les cartes existantes
            ClearRecommendations();

            if (titleText != null)
            {
                titleText.text = GetTitleForCriteria(displayCriteria);
            }

            // Créer les cartes de recommandations
            foreach (var product in recommendations)
            {
                CreateRecommendationCard(product);
            }

            Debug.Log($"{recommendations.Count} recommandations affichées");
        }

        /// <summary>
        /// Crée une carte de recommandation pour un produit
        /// </summary>
        private void CreateRecommendationCard(Product product)
        {
            if (recommendationCardPrefab == null || recommendationContainer == null)
            {
                Debug.LogWarning("Prefab ou container manquant");
                return;
            }

            GameObject card = Instantiate(recommendationCardPrefab, recommendationContainer);
            instantiatedCards.Add(card);

            // Configurer la carte (à personnaliser selon votre prefab)
            // Exemple simplifié:
            var nameText = card.GetComponentInChildren<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = $"{product.name}\n{product.price:F2}€";
            }

            // Ajouter un listener pour sélectionner le produit
            var button = card.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnRecommendationSelected(product));
            }
        }

        /// <summary>
        /// Appelé quand une recommandation est sélectionnée
        /// </summary>
        private void OnRecommendationSelected(Product product)
        {
            Debug.Log($"Recommandation sélectionnée: {product.name}");
            ProductManager.Instance.SelectProduct(product);
            Hide();
        }

        /// <summary>
        /// Nettoie les recommandations affichées
        /// </summary>
        private void ClearRecommendations()
        {
            foreach (var card in instantiatedCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            instantiatedCards.Clear();
        }

        /// <summary>
        /// Cache le panel de recommandations
        /// </summary>
        public void Hide()
        {
            if (recommendationPanel != null)
            {
                recommendationPanel.SetActive(false);
            }

            ClearRecommendations();
        }

        /// <summary>
        /// Retourne le titre approprié selon le critère
        /// </summary>
        private string GetTitleForCriteria(RecommendationCriteria criteria)
        {
            switch (criteria)
            {
                case RecommendationCriteria.Price:
                    return "Alternatives similaires en prix";
                case RecommendationCriteria.EcoScore:
                    return "Alternatives plus écologiques";
                case RecommendationCriteria.NutriScore:
                    return "Alternatives plus saines";
                case RecommendationCriteria.Bio:
                    return "Alternatives bio";
                case RecommendationCriteria.Origin:
                    return "Produits de même origine";
                case RecommendationCriteria.Category:
                    return "Produits similaires";
                case RecommendationCriteria.Combined:
                default:
                    return "Recommandations";
            }
        }

        /// <summary>
        /// Affiche les alternatives éco-responsables
        /// </summary>
        public void ShowEcoFriendlyAlternatives(Product product)
        {
            if (product == null)
            {
                return;
            }

            List<Product> ecoAlternatives = RecommendationEngine.Instance.GetEcoFriendlyAlternatives(product);
            DisplayRecommendations(ecoAlternatives);

            if (titleText != null)
            {
                titleText.text = "Alternatives éco-responsables";
            }

            if (recommendationPanel != null)
            {
                recommendationPanel.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }
    }
}
