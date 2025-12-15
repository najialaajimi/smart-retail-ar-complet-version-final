using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SmartRetailAR.Data;

namespace SmartRetailAR.Recommendations
{
    /// <summary>
    /// Criteria for filtering and recommending products.
    /// </summary>
    [Serializable]
    public class RecommendationCriteria
    {
        public bool preferBio = false;
        public bool preferLocal = false;
        public bool preferEcoFriendly = false;
        public bool preferLowerPrice = false;
        public bool preferBetterNutriScore = false;
        public float maxPrice = float.MaxValue;
        public int minEcoScore = 0;
        public List<string> requiredTags = new List<string>();
        public List<string> excludedAllergens = new List<string>();
        public List<string> preferredCategories = new List<string>();
    }
    
    /// <summary>
    /// Result of a product recommendation with score and reasons.
    /// </summary>
    [Serializable]
    public class RecommendationResult
    {
        public Product product;
        public float score;
        public List<string> reasons;
        public bool isBetterChoice;
        
        public RecommendationResult(Product product)
        {
            this.product = product;
            this.score = 0;
            this.reasons = new List<string>();
            this.isBetterChoice = false;
        }
    }
    
    /// <summary>
    /// Recommendation Engine for finding alternative products.
    /// Sprint 3: Recommendation system with search filters.
    /// </summary>
    public class RecommendationEngine : MonoBehaviour
    {
        private static RecommendationEngine _instance;
        public static RecommendationEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<RecommendationEngine>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("RecommendationEngine");
                        _instance = go.AddComponent<RecommendationEngine>();
                    }
                }
                return _instance;
            }
        }
        
        [Header("Scoring Weights")]
        [SerializeField] private float bioWeight = 1.5f;
        [SerializeField] private float localWeight = 1.3f;
        [SerializeField] private float ecoScoreWeight = 1.2f;
        [SerializeField] private float nutriScoreWeight = 1.4f;
        [SerializeField] private float priceWeight = 1.0f;
        
        [Header("Settings")]
        [SerializeField] private int maxRecommendations = 5;
        [SerializeField] private float minScoreThreshold = 0.3f;
        
        // User preferences (can be loaded from PlayerPrefs)
        private RecommendationCriteria userPreferences;
        
        public event Action<List<RecommendationResult>> OnRecommendationsGenerated;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            LoadUserPreferences();
        }
        
        /// <summary>
        /// Gets alternative product recommendations for a given product.
        /// </summary>
        public List<RecommendationResult> GetRecommendations(Product sourceProduct, RecommendationCriteria criteria = null)
        {
            if (sourceProduct == null)
            {
                Debug.LogWarning("Cannot get recommendations for null product");
                return new List<RecommendationResult>();
            }
            
            criteria = criteria ?? userPreferences ?? new RecommendationCriteria();
            
            // Get all products in the same category
            var candidates = ProductManager.Instance.GetProductsByCategory(sourceProduct.category)
                .Where(p => p.id != sourceProduct.id)
                .ToList();
            
            // Add explicitly defined alternatives
            var definedAlternatives = ProductManager.Instance.GetAlternatives(sourceProduct);
            foreach (var alt in definedAlternatives)
            {
                if (!candidates.Any(c => c.id == alt.id))
                {
                    candidates.Add(alt);
                }
            }
            
            // Score and rank candidates
            var results = new List<RecommendationResult>();
            
            foreach (var candidate in candidates)
            {
                if (!PassesFilters(candidate, criteria)) continue;
                
                var result = ScoreProduct(candidate, sourceProduct, criteria);
                
                if (result.score >= minScoreThreshold)
                {
                    results.Add(result);
                }
            }
            
            // Sort by score descending and take top recommendations
            results = results.OrderByDescending(r => r.score).Take(maxRecommendations).ToList();
            
            OnRecommendationsGenerated?.Invoke(results);
            
            return results;
        }
        
        /// <summary>
        /// Checks if a product passes the filter criteria.
        /// </summary>
        private bool PassesFilters(Product product, RecommendationCriteria criteria)
        {
            // Price filter
            if (product.price > criteria.maxPrice) return false;
            
            // Eco score filter
            if (product.ecoInfo != null && product.ecoInfo.ecoScore < criteria.minEcoScore) return false;
            
            // Required tags filter
            if (criteria.requiredTags != null && criteria.requiredTags.Count > 0)
            {
                if (product.tags == null) return false;
                if (!criteria.requiredTags.All(t => product.tags.Contains(t, StringComparer.OrdinalIgnoreCase)))
                {
                    return false;
                }
            }
            
            // Excluded allergens filter
            if (criteria.excludedAllergens != null && criteria.excludedAllergens.Count > 0)
            {
                if (product.nutrition?.allergens != null)
                {
                    if (criteria.excludedAllergens.Any(a => 
                        product.nutrition.allergens.Contains(a, StringComparer.OrdinalIgnoreCase)))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Scores a product based on comparison with source and criteria.
        /// </summary>
        private RecommendationResult ScoreProduct(Product candidate, Product source, RecommendationCriteria criteria)
        {
            var result = new RecommendationResult(candidate);
            float totalScore = 0;
            
            // Bio preference scoring
            if (criteria.preferBio && candidate.ecoInfo != null)
            {
                if (candidate.ecoInfo.isBio && (source.ecoInfo == null || !source.ecoInfo.isBio))
                {
                    totalScore += bioWeight;
                    result.reasons.Add("Certified organic/bio product");
                    result.isBetterChoice = true;
                }
                else if (candidate.ecoInfo.isBio)
                {
                    totalScore += bioWeight * 0.5f;
                }
            }
            
            // Local preference scoring
            if (criteria.preferLocal && candidate.ecoInfo != null)
            {
                if (candidate.ecoInfo.isLocalProduct && (source.ecoInfo == null || !source.ecoInfo.isLocalProduct))
                {
                    totalScore += localWeight;
                    result.reasons.Add("Locally sourced product");
                    result.isBetterChoice = true;
                }
                else if (candidate.ecoInfo.isLocalProduct)
                {
                    totalScore += localWeight * 0.5f;
                }
            }
            
            // Eco score comparison
            if (criteria.preferEcoFriendly && candidate.ecoInfo != null && source.ecoInfo != null)
            {
                int scoreDiff = candidate.ecoInfo.ecoScore - source.ecoInfo.ecoScore;
                if (scoreDiff > 0)
                {
                    totalScore += ecoScoreWeight * (scoreDiff / 100f) * 2;
                    result.reasons.Add($"Better eco-score ({candidate.ecoInfo.GetEcoGrade()} vs {source.ecoInfo.GetEcoGrade()})");
                    result.isBetterChoice = true;
                }
            }
            
            // Nutri-score comparison
            if (criteria.preferBetterNutriScore && candidate.nutrition != null && source.nutrition != null)
            {
                int candidateScore = GetNutriScoreValue(candidate.nutrition.nutriScore);
                int sourceScore = GetNutriScoreValue(source.nutrition.nutriScore);
                
                if (candidateScore > sourceScore)
                {
                    totalScore += nutriScoreWeight * (candidateScore - sourceScore) * 0.5f;
                    result.reasons.Add($"Better nutrition score ({candidate.nutrition.nutriScore} vs {source.nutrition.nutriScore})");
                    result.isBetterChoice = true;
                }
            }
            
            // Price comparison
            if (criteria.preferLowerPrice)
            {
                float priceDiff = source.price - candidate.price;
                if (priceDiff > 0)
                {
                    totalScore += priceWeight * (priceDiff / source.price);
                    result.reasons.Add($"Lower price ({candidate.price:F2} vs {source.price:F2} {source.currency})");
                }
            }
            
            // Base score for being an alternative
            totalScore += 0.5f;
            
            result.score = Mathf.Clamp01(totalScore / 5f);
            
            return result;
        }
        
        /// <summary>
        /// Converts nutri-score letter to numeric value for comparison.
        /// </summary>
        private int GetNutriScoreValue(string nutriScore)
        {
            switch (nutriScore?.ToUpper())
            {
                case "A": return 5;
                case "B": return 4;
                case "C": return 3;
                case "D": return 2;
                case "E": return 1;
                default: return 0;
            }
        }
        
        /// <summary>
        /// Gets bio/organic alternatives for a product.
        /// </summary>
        public List<RecommendationResult> GetBioAlternatives(Product product)
        {
            var criteria = new RecommendationCriteria
            {
                preferBio = true,
                preferEcoFriendly = true,
                requiredTags = new List<string> { "bio" }
            };
            
            return GetRecommendations(product, criteria);
        }
        
        /// <summary>
        /// Gets local alternatives for a product.
        /// </summary>
        public List<RecommendationResult> GetLocalAlternatives(Product product)
        {
            var criteria = new RecommendationCriteria
            {
                preferLocal = true,
                requiredTags = new List<string> { "local" }
            };
            
            return GetRecommendations(product, criteria);
        }
        
        /// <summary>
        /// Gets eco-friendly alternatives for a product.
        /// </summary>
        public List<RecommendationResult> GetEcoFriendlyAlternatives(Product product)
        {
            var criteria = new RecommendationCriteria
            {
                preferBio = true,
                preferLocal = true,
                preferEcoFriendly = true,
                minEcoScore = 60
            };
            
            return GetRecommendations(product, criteria);
        }
        
        /// <summary>
        /// Gets healthier alternatives based on nutrition.
        /// </summary>
        public List<RecommendationResult> GetHealthierAlternatives(Product product)
        {
            var criteria = new RecommendationCriteria
            {
                preferBetterNutriScore = true
            };
            
            return GetRecommendations(product, criteria);
        }
        
        /// <summary>
        /// Gets allergen-free alternatives.
        /// </summary>
        public List<RecommendationResult> GetAllergenFreeAlternatives(Product product, params string[] allergens)
        {
            var criteria = new RecommendationCriteria
            {
                excludedAllergens = allergens.ToList()
            };
            
            return GetRecommendations(product, criteria);
        }
        
        /// <summary>
        /// Searches products with filters.
        /// Sprint 3: Search filters functionality.
        /// </summary>
        public List<Product> SearchWithFilters(string query, RecommendationCriteria filters)
        {
            var allProducts = ProductManager.Instance.GetAllProducts();
            
            // Apply text search
            IEnumerable<Product> results = allProducts;
            
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                results = results.Where(p =>
                    (p.name?.ToLower().Contains(query) ?? false) ||
                    (p.brand?.ToLower().Contains(query) ?? false) ||
                    (p.description?.ToLower().Contains(query) ?? false) ||
                    (p.tags?.Any(t => t.ToLower().Contains(query)) ?? false)
                );
            }
            
            // Apply filters
            if (filters != null)
            {
                results = results.Where(p => PassesFilters(p, filters));
                
                // Additional preference-based filtering
                if (filters.preferBio)
                {
                    results = results.Where(p => p.ecoInfo?.isBio ?? false);
                }
                
                if (filters.preferLocal)
                {
                    results = results.Where(p => p.ecoInfo?.isLocalProduct ?? false);
                }
                
                if (filters.preferredCategories != null && filters.preferredCategories.Count > 0)
                {
                    results = results.Where(p => filters.preferredCategories.Contains(p.category));
                }
            }
            
            return results.ToList();
        }
        
        /// <summary>
        /// Updates user preferences.
        /// </summary>
        public void SetUserPreferences(RecommendationCriteria preferences)
        {
            userPreferences = preferences;
            SaveUserPreferences();
        }
        
        /// <summary>
        /// Gets current user preferences.
        /// </summary>
        public RecommendationCriteria GetUserPreferences()
        {
            return userPreferences ?? new RecommendationCriteria();
        }
        
        /// <summary>
        /// Loads user preferences from PlayerPrefs.
        /// </summary>
        private void LoadUserPreferences()
        {
            userPreferences = new RecommendationCriteria
            {
                preferBio = PlayerPrefs.GetInt("Pref_Bio", 0) == 1,
                preferLocal = PlayerPrefs.GetInt("Pref_Local", 0) == 1,
                preferEcoFriendly = PlayerPrefs.GetInt("Pref_Eco", 0) == 1,
                preferLowerPrice = PlayerPrefs.GetInt("Pref_Price", 0) == 1,
                preferBetterNutriScore = PlayerPrefs.GetInt("Pref_Nutri", 0) == 1
            };
        }
        
        /// <summary>
        /// Saves user preferences to PlayerPrefs.
        /// </summary>
        private void SaveUserPreferences()
        {
            if (userPreferences == null) return;
            
            PlayerPrefs.SetInt("Pref_Bio", userPreferences.preferBio ? 1 : 0);
            PlayerPrefs.SetInt("Pref_Local", userPreferences.preferLocal ? 1 : 0);
            PlayerPrefs.SetInt("Pref_Eco", userPreferences.preferEcoFriendly ? 1 : 0);
            PlayerPrefs.SetInt("Pref_Price", userPreferences.preferLowerPrice ? 1 : 0);
            PlayerPrefs.SetInt("Pref_Nutri", userPreferences.preferBetterNutriScore ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
