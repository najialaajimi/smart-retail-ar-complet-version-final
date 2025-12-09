using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SmartRetailAR.Products;

namespace SmartRetailAR.Recommendations
{
    /// <summary>
    /// Type de critère de recommandation
    /// </summary>
    public enum RecommendationCriteria
    {
        Price,              // Prix similaire ou inférieur
        EcoScore,          // Meilleur score écologique
        NutriScore,        // Meilleur score nutritionnel
        Bio,               // Produits bio
        Origin,            // Même origine
        Category,          // Même catégorie
        Combined           // Combinaison de critères
    }

    /// <summary>
    /// Moteur de recommandations de produits
    /// Analyse et suggère des alternatives basées sur différents critères
    /// </summary>
    public class RecommendationEngine : MonoBehaviour
    {
        private static RecommendationEngine instance;
        public static RecommendationEngine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RecommendationEngine>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("RecommendationEngine");
                        instance = go.AddComponent<RecommendationEngine>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [Header("Configuration")]
        [SerializeField] private int maxRecommendations = 5;
        [SerializeField] private float priceTolerancePercent = 20f;
        [SerializeField] private float ecoScoreWeight = 0.3f;
        [SerializeField] private float nutriScoreWeight = 0.3f;
        [SerializeField] private float priceWeight = 0.2f;
        [SerializeField] private float bioWeight = 0.2f;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Obtient les alternatives pour un produit donné
        /// </summary>
        public List<Product> GetAlternatives(Product product, RecommendationCriteria criteria = RecommendationCriteria.Combined)
        {
            if (product == null)
            {
                Debug.LogWarning("Produit null passé à GetAlternatives");
                return new List<Product>();
            }

            List<Product> allProducts = ProductDatabase.Instance.GetAllProducts();
            List<Product> alternatives = new List<Product>();

            // Retirer le produit actuel de la liste
            allProducts = allProducts.Where(p => p.id != product.id).ToList();

            switch (criteria)
            {
                case RecommendationCriteria.Price:
                    alternatives = GetPriceBasedAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.EcoScore:
                    alternatives = GetEcoScoreBasedAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.NutriScore:
                    alternatives = GetNutriScoreBasedAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.Bio:
                    alternatives = GetBioAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.Origin:
                    alternatives = GetOriginBasedAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.Category:
                    alternatives = GetCategoryBasedAlternatives(product, allProducts);
                    break;
                case RecommendationCriteria.Combined:
                    alternatives = GetCombinedAlternatives(product, allProducts);
                    break;
            }

            return alternatives.Take(maxRecommendations).ToList();
        }

        /// <summary>
        /// Alternatives basées sur le prix
        /// </summary>
        private List<Product> GetPriceBasedAlternatives(Product product, List<Product> allProducts)
        {
            float maxPrice = product.price * (1 + priceTolerancePercent / 100f);
            
            return allProducts
                .Where(p => p.category == product.category && p.price <= maxPrice)
                .OrderBy(p => p.price)
                .ToList();
        }

        /// <summary>
        /// Alternatives avec meilleur eco-score
        /// </summary>
        private List<Product> GetEcoScoreBasedAlternatives(Product product, List<Product> allProducts)
        {
            return allProducts
                .Where(p => p.category == product.category && p.ecoScore >= product.ecoScore)
                .OrderByDescending(p => p.ecoScore)
                .ToList();
        }

        /// <summary>
        /// Alternatives avec meilleur nutri-score
        /// </summary>
        private List<Product> GetNutriScoreBasedAlternatives(Product product, List<Product> allProducts)
        {
            int currentNutriScore = ConvertNutriScoreToInt(product.nutrition.nutriScore);

            return allProducts
                .Where(p => p.category == product.category)
                .OrderBy(p => ConvertNutriScoreToInt(p.nutrition.nutriScore))
                .Where(p => ConvertNutriScoreToInt(p.nutrition.nutriScore) <= currentNutriScore)
                .ToList();
        }

        /// <summary>
        /// Alternatives bio
        /// </summary>
        private List<Product> GetBioAlternatives(Product product, List<Product> allProducts)
        {
            return allProducts
                .Where(p => p.category == product.category && p.isBio)
                .OrderByDescending(p => p.ecoScore)
                .ToList();
        }

        /// <summary>
        /// Alternatives de même origine
        /// </summary>
        private List<Product> GetOriginBasedAlternatives(Product product, List<Product> allProducts)
        {
            return allProducts
                .Where(p => p.category == product.category && p.origin == product.origin)
                .OrderByDescending(p => p.ecoScore)
                .ToList();
        }

        /// <summary>
        /// Alternatives de même catégorie
        /// </summary>
        private List<Product> GetCategoryBasedAlternatives(Product product, List<Product> allProducts)
        {
            return allProducts
                .Where(p => p.category == product.category)
                .OrderByDescending(p => p.ecoScore)
                .ToList();
        }

        /// <summary>
        /// Alternatives basées sur plusieurs critères combinés
        /// </summary>
        private List<Product> GetCombinedAlternatives(Product product, List<Product> allProducts)
        {
            // Filtrer par catégorie
            var candidates = allProducts.Where(p => p.category == product.category).ToList();

            // Calculer un score pour chaque alternative
            var scoredProducts = candidates.Select(p => new
            {
                Product = p,
                Score = CalculateCombinedScore(product, p)
            }).OrderByDescending(x => x.Score);

            return scoredProducts.Select(x => x.Product).ToList();
        }

        /// <summary>
        /// Calcule un score combiné pour un produit alternatif
        /// </summary>
        private float CalculateCombinedScore(Product reference, Product candidate)
        {
            float score = 0f;

            // Eco-score (plus c'est haut, mieux c'est)
            float ecoScoreDiff = (candidate.ecoScore - reference.ecoScore) / 100f;
            score += ecoScoreDiff * ecoScoreWeight;

            // Nutri-score (A=1, E=5, plus c'est bas, mieux c'est)
            int refNutri = ConvertNutriScoreToInt(reference.nutrition.nutriScore);
            int candNutri = ConvertNutriScoreToInt(candidate.nutrition.nutriScore);
            float nutriScoreDiff = (refNutri - candNutri) / 5f;
            score += nutriScoreDiff * nutriScoreWeight;

            // Prix (moins cher, mieux c'est)
            float priceDiff = (reference.price - candidate.price) / reference.price;
            score += priceDiff * priceWeight;

            // Bio bonus
            if (candidate.isBio && !reference.isBio)
            {
                score += bioWeight;
            }

            return score;
        }

        /// <summary>
        /// Obtient les produits éco-responsables similaires
        /// </summary>
        public List<Product> GetEcoFriendlyAlternatives(Product product)
        {
            List<Product> allProducts = ProductDatabase.Instance.GetAllProducts();

            return allProducts
                .Where(p => p.id != product.id &&
                           p.category == product.category &&
                           p.isEcoResponsible &&
                           p.ecoScore >= 70)
                .OrderByDescending(p => p.ecoScore)
                .Take(maxRecommendations)
                .ToList();
        }

        /// <summary>
        /// Obtient les produits similaires dans une gamme de prix
        /// </summary>
        public List<Product> GetSimilarProducts(Product product, float minPrice, float maxPrice)
        {
            List<Product> allProducts = ProductDatabase.Instance.GetAllProducts();

            return allProducts
                .Where(p => p.id != product.id &&
                           p.category == product.category &&
                           p.price >= minPrice &&
                           p.price <= maxPrice)
                .OrderBy(p => Mathf.Abs(p.price - product.price))
                .Take(maxRecommendations)
                .ToList();
        }

        /// <summary>
        /// Convertit le nutri-score en valeur numérique (A=1, E=5)
        /// </summary>
        private int ConvertNutriScoreToInt(string nutriScore)
        {
            if (string.IsNullOrEmpty(nutriScore))
            {
                return 3; // Valeur par défaut C
            }

            switch (nutriScore.ToUpper())
            {
                case "A": return 1;
                case "B": return 2;
                case "C": return 3;
                case "D": return 4;
                case "E": return 5;
                default: return 3;
            }
        }

        /// <summary>
        /// Configure le nombre maximum de recommandations
        /// </summary>
        public void SetMaxRecommendations(int max)
        {
            maxRecommendations = Mathf.Max(1, max);
        }
    }
}
