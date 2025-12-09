using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Recommendations;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel de filtrage interactif des produits
    /// </summary>
    public class FilterPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private Slider priceSlider;
        [SerializeField] private TextMeshProUGUI priceValueText;
        [SerializeField] private Slider ecoScoreSlider;
        [SerializeField] private TextMeshProUGUI ecoScoreValueText;
        [SerializeField] private Toggle bioToggle;
        [SerializeField] private Toggle ecoResponsibleToggle;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;

        [Header("Configuration")]
        [SerializeField] private float maxPrice = 20f;

        private RecommendationFilter currentFilter;

        private void Awake()
        {
            currentFilter = new RecommendationFilter();
            SetupListeners();

            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Configure les listeners des contrôles
        /// </summary>
        private void SetupListeners()
        {
            if (priceSlider != null)
            {
                priceSlider.maxValue = maxPrice;
                priceSlider.onValueChanged.AddListener(OnPriceChanged);
            }

            if (ecoScoreSlider != null)
            {
                ecoScoreSlider.maxValue = 100f;
                ecoScoreSlider.onValueChanged.AddListener(OnEcoScoreChanged);
            }

            if (applyButton != null)
            {
                applyButton.onClick.AddListener(OnApplyFilters);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetFilters);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }
        }

        /// <summary>
        /// Affiche le panel de filtres
        /// </summary>
        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
            }

            UpdateUI();
        }

        /// <summary>
        /// Cache le panel de filtres
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Met à jour l'interface avec les valeurs du filtre
        /// </summary>
        private void UpdateUI()
        {
            if (priceSlider != null)
            {
                priceSlider.value = currentFilter.maxPrice;
            }

            if (ecoScoreSlider != null)
            {
                ecoScoreSlider.value = currentFilter.minEcoScore;
            }

            if (bioToggle != null)
            {
                bioToggle.isOn = currentFilter.requireBio;
            }

            if (ecoResponsibleToggle != null)
            {
                ecoResponsibleToggle.isOn = currentFilter.requireEcoResponsible;
            }

            UpdatePriceText(currentFilter.maxPrice);
            UpdateEcoScoreText(currentFilter.minEcoScore);
        }

        /// <summary>
        /// Callback pour le changement de prix
        /// </summary>
        private void OnPriceChanged(float value)
        {
            currentFilter.maxPrice = value;
            currentFilter.filterByPrice = true;
            UpdatePriceText(value);
        }

        /// <summary>
        /// Callback pour le changement d'eco-score
        /// </summary>
        private void OnEcoScoreChanged(float value)
        {
            currentFilter.minEcoScore = value;
            currentFilter.filterByEcoScore = true;
            UpdateEcoScoreText(value);
        }

        /// <summary>
        /// Met à jour le texte du prix
        /// </summary>
        private void UpdatePriceText(float price)
        {
            if (priceValueText != null)
            {
                priceValueText.text = $"Max: {price:F2}€";
            }
        }

        /// <summary>
        /// Met à jour le texte de l'eco-score
        /// </summary>
        private void UpdateEcoScoreText(float score)
        {
            if (ecoScoreValueText != null)
            {
                ecoScoreValueText.text = $"Min: {score:F0}";
            }
        }

        /// <summary>
        /// Applique les filtres
        /// </summary>
        private void OnApplyFilters()
        {
            if (bioToggle != null)
            {
                currentFilter.requireBio = bioToggle.isOn;
            }

            if (ecoResponsibleToggle != null)
            {
                currentFilter.requireEcoResponsible = ecoResponsibleToggle.isOn;
            }

            Debug.Log($"Filtres appliqués: {currentFilter.GetActiveFilterCount()} actifs");
            
            // Ici vous pouvez déclencher un événement ou appeler une fonction
            // pour mettre à jour la liste de produits affichée

            Hide();
        }

        /// <summary>
        /// Réinitialise les filtres
        /// </summary>
        private void OnResetFilters()
        {
            currentFilter.Reset();
            UpdateUI();
            Debug.Log("Filtres réinitialisés");
        }

        /// <summary>
        /// Récupère le filtre actuel
        /// </summary>
        public RecommendationFilter GetCurrentFilter()
        {
            return currentFilter.Clone();
        }

        /// <summary>
        /// Définit un filtre personnalisé
        /// </summary>
        public void SetFilter(RecommendationFilter filter)
        {
            if (filter != null)
            {
                currentFilter = filter.Clone();
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            if (priceSlider != null)
            {
                priceSlider.onValueChanged.RemoveAllListeners();
            }

            if (ecoScoreSlider != null)
            {
                ecoScoreSlider.onValueChanged.RemoveAllListeners();
            }

            if (applyButton != null)
            {
                applyButton.onClick.RemoveAllListeners();
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }
    }
}
