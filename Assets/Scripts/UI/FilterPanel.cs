using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Products;
using SmartRetailAR.Data;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel de filtrage des produits
    /// Sprint 3 - Filtres de recherche
    /// </summary>
    public class FilterPanel : MonoBehaviour
    {
        [Header("Filtres")]
        [SerializeField] private Toggle bioToggle;
        [SerializeField] private Toggle veganToggle;
        [SerializeField] private Toggle localToggle;
        [SerializeField] private Slider maxPriceSlider;
        [SerializeField] private Text maxPriceText;
        [SerializeField] private Dropdown categoryDropdown;
        [SerializeField] private Dropdown ecoScoreDropdown;

        [Header("Boutons")]
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;

        // Filtres actifs
        private ProductFilter m_CurrentFilter = new ProductFilter();

        // Events
        public event System.Action<ProductFilter> OnFilterApplied;
        public event System.Action OnFilterReset;

        private void Start()
        {
            // Configurer les listeners
            if (applyButton != null)
            {
                applyButton.onClick.AddListener(ApplyFilters);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(ResetFilters);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (maxPriceSlider != null)
            {
                maxPriceSlider.onValueChanged.AddListener(UpdateMaxPriceText);
                UpdateMaxPriceText(maxPriceSlider.value);
            }

            // Initialiser les dropdowns
            InitializeDropdowns();

            // Charger les filtres sauvegardés
            LoadSavedFilters();
        }

        /// <summary>
        /// Initialise les dropdowns
        /// </summary>
        private void InitializeDropdowns()
        {
            // Catégories
            if (categoryDropdown != null)
            {
                categoryDropdown.ClearOptions();
                categoryDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Toutes les catégories",
                    "Produits laitiers",
                    "Boissons végétales",
                    "Boulangerie",
                    "Desserts végétaux",
                    "Épicerie",
                    "Fruits & Légumes"
                });
            }

            // Eco-Score
            if (ecoScoreDropdown != null)
            {
                ecoScoreDropdown.ClearOptions();
                ecoScoreDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Tous les scores",
                    "A minimum",
                    "B minimum",
                    "C minimum"
                });
            }
        }

        /// <summary>
        /// Applique les filtres
        /// </summary>
        public void ApplyFilters()
        {
            // Construire le filtre
            m_CurrentFilter.onlyBio = bioToggle != null && bioToggle.isOn;
            m_CurrentFilter.onlyVegan = veganToggle != null && veganToggle.isOn;
            m_CurrentFilter.maxPrice = maxPriceSlider != null ? maxPriceSlider.value : 0f;

            // Catégorie
            if (categoryDropdown != null && categoryDropdown.value > 0)
            {
                m_CurrentFilter.category = categoryDropdown.options[categoryDropdown.value].text;
            }
            else
            {
                m_CurrentFilter.category = "";
            }

            // Eco-Score
            if (ecoScoreDropdown != null && ecoScoreDropdown.value > 0)
            {
                switch (ecoScoreDropdown.value)
                {
                    case 1: m_CurrentFilter.minEcoScore = "A"; break;
                    case 2: m_CurrentFilter.minEcoScore = "B"; break;
                    case 3: m_CurrentFilter.minEcoScore = "C"; break;
                    default: m_CurrentFilter.minEcoScore = null; break;
                }
            }
            else
            {
                m_CurrentFilter.minEcoScore = null;
            }

            // Sauvegarder les filtres
            SaveFilters();

            // Déclencher l'événement
            OnFilterApplied?.Invoke(m_CurrentFilter);

            Utils.DebugLogger.Log("Filtres appliqués", Utils.LogLevel.Info);
        }

        /// <summary>
        /// Réinitialise les filtres
        /// </summary>
        public void ResetFilters()
        {
            m_CurrentFilter = new ProductFilter();

            // Réinitialiser l'UI
            if (bioToggle != null) bioToggle.isOn = false;
            if (veganToggle != null) veganToggle.isOn = false;
            if (localToggle != null) localToggle.isOn = false;
            if (maxPriceSlider != null) maxPriceSlider.value = 0f;
            if (categoryDropdown != null) categoryDropdown.value = 0;
            if (ecoScoreDropdown != null) ecoScoreDropdown.value = 0;

            // Sauvegarder
            SaveFilters();

            // Déclencher l'événement
            OnFilterReset?.Invoke();

            Utils.DebugLogger.Log("Filtres réinitialisés", Utils.LogLevel.Info);
        }

        /// <summary>
        /// Met à jour le texte du prix maximum
        /// </summary>
        private void UpdateMaxPriceText(float value)
        {
            if (maxPriceText != null)
            {
                if (value > 0)
                {
                    maxPriceText.text = $"Prix max: {value:F2}€";
                }
                else
                {
                    maxPriceText.text = "Prix: Tous";
                }
            }
        }

        /// <summary>
        /// Sauvegarde les filtres dans PlayerPrefs
        /// </summary>
        private void SaveFilters()
        {
            PlayerPrefs.SetInt("Filter_Bio", m_CurrentFilter.onlyBio ? 1 : 0);
            PlayerPrefs.SetInt("Filter_Vegan", m_CurrentFilter.onlyVegan ? 1 : 0);
            PlayerPrefs.SetFloat("Filter_MaxPrice", m_CurrentFilter.maxPrice);
            PlayerPrefs.SetString("Filter_Category", m_CurrentFilter.category ?? "");
            PlayerPrefs.SetString("Filter_EcoScore", m_CurrentFilter.minEcoScore ?? "");
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Charge les filtres depuis PlayerPrefs
        /// </summary>
        private void LoadSavedFilters()
        {
            if (PlayerPrefs.HasKey("Filter_Bio"))
            {
                m_CurrentFilter.onlyBio = PlayerPrefs.GetInt("Filter_Bio") == 1;
                if (bioToggle != null) bioToggle.isOn = m_CurrentFilter.onlyBio;
            }

            if (PlayerPrefs.HasKey("Filter_Vegan"))
            {
                m_CurrentFilter.onlyVegan = PlayerPrefs.GetInt("Filter_Vegan") == 1;
                if (veganToggle != null) veganToggle.isOn = m_CurrentFilter.onlyVegan;
            }

            if (PlayerPrefs.HasKey("Filter_MaxPrice"))
            {
                m_CurrentFilter.maxPrice = PlayerPrefs.GetFloat("Filter_MaxPrice");
                if (maxPriceSlider != null) maxPriceSlider.value = m_CurrentFilter.maxPrice;
            }
        }

        /// <summary>
        /// Cache le panel
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Affiche le panel
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Récupère le filtre actuel
        /// </summary>
        public ProductFilter GetCurrentFilter()
        {
            return m_CurrentFilter;
        }
    }
}
