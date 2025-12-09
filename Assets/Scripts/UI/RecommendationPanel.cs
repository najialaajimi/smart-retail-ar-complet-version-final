using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SmartRetailAR.Data;
using SmartRetailAR.Products;
using SmartRetailAR.Utils;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel d'affichage des recommandations de produits alternatifs
    /// </summary>
    public class RecommendationPanel : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform recommendationsContainer;
        [SerializeField] private GameObject recommendationCardPrefab;
        [SerializeField] private Text titleText;
        [SerializeField] private Button closeButton;

        [Header("Configuration")]
        [SerializeField] private int maxRecommendations = 5;

        // État
        private List<GameObject> m_RecommendationCards = new List<GameObject>();
        private ProductData m_ReferenceProduct;

        // Events
        public event System.Action<ProductData> OnProductSelected;

        private void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        /// <summary>
        /// Affiche les recommandations pour un produit
        /// </summary>
        public void ShowRecommendations(ProductData product, List<ProductData> recommendations)
        {
            if (product == null)
            {
                DebugLogger.LogWarning("Produit null, impossible d'afficher les recommandations");
                return;
            }

            m_ReferenceProduct = product;

            // Mettre à jour le titre
            if (titleText != null)
            {
                titleText.text = $"Alternatives pour {product.name}";
            }

            // Nettoyer les recommandations précédentes
            ClearRecommendations();

            // Créer les cartes de recommandation
            if (recommendations != null && recommendations.Count > 0)
            {
                int count = Mathf.Min(recommendations.Count, maxRecommendations);
                for (int i = 0; i < count; i++)
                {
                    CreateRecommendationCard(recommendations[i]);
                }

                DebugLogger.Log($"✓ {count} recommandations affichées", LogLevel.Info);
            }
            else
            {
                DebugLogger.LogWarning("Aucune recommandation disponible");
                CreateNoRecommendationsMessage();
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// Crée une carte de recommandation
        /// </summary>
        private void CreateRecommendationCard(ProductData product)
        {
            GameObject card = null;

            // Si un prefab est défini, l'instancier
            if (recommendationCardPrefab != null && recommendationsContainer != null)
            {
                card = Instantiate(recommendationCardPrefab, recommendationsContainer);
            }
            else if (recommendationsContainer != null)
            {
                // Sinon, créer une carte simple
                card = CreateSimpleCard(product);
            }

            if (card != null)
            {
                // Configurer la carte avec les données du produit
                ConfigureCard(card, product);
                m_RecommendationCards.Add(card);
            }
        }

        /// <summary>
        /// Crée une carte simple de recommandation
        /// </summary>
        private GameObject CreateSimpleCard(ProductData product)
        {
            GameObject card = new GameObject($"Card_{product.id}");
            card.transform.SetParent(recommendationsContainer, false);

            // Ajouter un layout
            RectTransform rectTransform = card.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300, 100);

            // Ajouter un background
            Image background = card.AddComponent<Image>();
            background.color = new Color(0.9f, 0.9f, 0.9f);

            // Ajouter un texte
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(card.transform, false);
            Text text = textObj.AddComponent<Text>();
            text.text = $"{product.brand}\n{product.name}\n{product.price:F2}€";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;

            RectTransform textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            // Ajouter un bouton
            Button button = card.AddComponent<Button>();
            button.onClick.AddListener(() => SelectProduct(product));

            return card;
        }

        /// <summary>
        /// Configure une carte avec les données du produit
        /// </summary>
        private void ConfigureCard(GameObject card, ProductData product)
        {
            // Rechercher les composants Text dans la carte
            Text[] texts = card.GetComponentsInChildren<Text>();
            
            foreach (var text in texts)
            {
                if (text.name.Contains("Name"))
                {
                    text.text = product.name;
                }
                else if (text.name.Contains("Brand"))
                {
                    text.text = product.brand;
                }
                else if (text.name.Contains("Price"))
                {
                    text.text = $"{product.price:F2}€";
                }
            }

            // Configurer le bouton
            Button button = card.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectProduct(product));
            }
        }

        /// <summary>
        /// Crée un message "Aucune recommandation"
        /// </summary>
        private void CreateNoRecommendationsMessage()
        {
            if (recommendationsContainer == null)
                return;

            GameObject messageObj = new GameObject("NoRecommendations");
            messageObj.transform.SetParent(recommendationsContainer, false);

            Text text = messageObj.AddComponent<Text>();
            text.text = "Aucune recommandation disponible pour ce produit.";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.gray;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 18;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(400, 100);

            m_RecommendationCards.Add(messageObj);
        }

        /// <summary>
        /// Sélectionne un produit recommandé
        /// </summary>
        private void SelectProduct(ProductData product)
        {
            DebugLogger.Log($"Produit sélectionné: {product.name}", LogLevel.Info);
            OnProductSelected?.Invoke(product);
        }

        /// <summary>
        /// Nettoie toutes les recommandations affichées
        /// </summary>
        private void ClearRecommendations()
        {
            foreach (var card in m_RecommendationCards)
            {
                if (card != null)
                {
                    Destroy(card);
                }
            }

            m_RecommendationCards.Clear();
        }

        /// <summary>
        /// Cache le panel
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            DebugLogger.Log("Panel de recommandations caché", LogLevel.Debug);
        }

        private void OnDestroy()
        {
            ClearRecommendations();
        }
    }
}
