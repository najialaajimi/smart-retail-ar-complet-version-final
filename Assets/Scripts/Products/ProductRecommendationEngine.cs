using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SmartRetailAR.Data;
using SmartRetailAR.Utils;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Moteur de recommandations pour proposer des alternatives aux produits
    /// Sprint 3 - Système intelligent basé sur multiples critères
    /// </summary>
    public class ProductRecommendationEngine : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int maxRecommendations = 5;
        [SerializeField] private float scoreThreshold = 5.0f;
        [SerializeField] private bool prioritizeBio = true;
        [SerializeField] private bool prioritizeLocal = true;
        [SerializeField] private bool prioritizePrice = true;

        [Header("Poids des critères")]
        [Range(0f, 1f)] [SerializeField] private float categoryWeight = 0.3f;
        [Range(0f, 1f)] [SerializeField] private float priceWeight = 0.2f;
        [Range(0f, 1f)] [SerializeField] private float ecoScoreWeight = 0.25f;
        [Range(0f, 1f)] [SerializeField] private float nutriScoreWeight = 0.15f;
        [Range(0f, 1f)] [SerializeField] private float bioWeight = 0.1f;

        private ProductManager m_ProductManager;

        private void Awake()
        {
            m_ProductManager = FindObjectOfType<ProductManager>();
            
            if (m_ProductManager == null)
            {
                DebugLogger.LogWarning("ProductManager non trouvé, certaines fonctionnalités seront limitées");
            }
        }

        /// <summary>
        /// Génère des recommandations pour un produit donné
        /// </summary>
        public List<ProductData> GetRecommendations(ProductData product, UserPreferences userPreferences = null)
        {
            if (product == null)
            {
                DebugLogger.LogWarning("Produit null, impossible de générer des recommandations");
                return new List<ProductData>();
            }

            if (m_ProductManager == null || m_ProductManager.AllProducts == null)
            {
                DebugLogger.LogWarning("ProductManager ou produits non disponibles");
                return new List<ProductData>();
            }

            DebugLogger.Log($"Génération de recommandations pour: {product.name}", LogLevel.Info);

            // Récupérer tous les produits sauf le produit actuel
            List<ProductData> candidateProducts = m_ProductManager.AllProducts
                .Where(p => p.id != product.id)
                .ToList();

            // Calculer les scores de recommandation
            List<RecommendationScore> scoredProducts = new List<RecommendationScore>();

            foreach (var candidate in candidateProducts)
            {
                float score = CalculateRecommendationScore(product, candidate, userPreferences);
                
                if (score >= scoreThreshold)
                {
                    scoredProducts.Add(new RecommendationScore
                    {
                        product = candidate,
                        score = score
                    });
                }
            }

            // Trier par score décroissant
            scoredProducts = scoredProducts.OrderByDescending(s => s.score).ToList();

            // Prioriser les alternatives explicites
            List<ProductData> explicitAlternatives = m_ProductManager.GetAlternatives(product);
            List<ProductData> recommendations = new List<ProductData>();

            // Ajouter d'abord les alternatives explicites
            foreach (var alt in explicitAlternatives)
            {
                if (!recommendations.Contains(alt) && recommendations.Count < maxRecommendations)
                {
                    recommendations.Add(alt);
                }
            }

            // Compléter avec les recommandations calculées
            foreach (var scored in scoredProducts)
            {
                if (!recommendations.Contains(scored.product) && recommendations.Count < maxRecommendations)
                {
                    recommendations.Add(scored.product);
                }
            }

            DebugLogger.Log($"✓ {recommendations.Count} recommandations générées", LogLevel.Info);

            return recommendations;
        }

        /// <summary>
        /// Calcule le score de recommandation entre deux produits
        /// </summary>
        private float CalculateRecommendationScore(ProductData reference, ProductData candidate, UserPreferences prefs)
        {
            float totalScore = 0f;

            // 1. Score de catégorie (même catégorie = bonus)
            float categoryScore = reference.category == candidate.category ? 10f : 0f;
            totalScore += categoryScore * categoryWeight * 10f;

            // 2. Score de prix (similaire ou moins cher = bonus)
            float priceDiff = Mathf.Abs(candidate.price - reference.price);
            float priceScore = candidate.price <= reference.price ? 10f - Mathf.Min(priceDiff * 2f, 5f) : 5f - Mathf.Min(priceDiff, 5f);
            totalScore += priceScore * priceWeight * 10f;

            // 3. Score écologique (meilleur = bonus)
            float ecoScore = GetEcoScoreValue(candidate.ecoScore) - GetEcoScoreValue(reference.ecoScore);
            totalScore += Mathf.Max(0, ecoScore) * ecoScoreWeight * 10f;

            // 4. Score nutritionnel (meilleur = bonus)
            float nutriScore = GetNutriScoreValue(candidate.nutrition.nutriscore) - GetNutriScoreValue(reference.nutrition.nutriscore);
            totalScore += Mathf.Max(0, nutriScore) * nutriScoreWeight * 10f;

            // 5. Bonus bio
            if (candidate.isBio && !reference.isBio)
            {
                totalScore += 10f * bioWeight * 10f;
            }

            // 6. Bonus selon les préférences utilisateur
            if (prefs != null)
            {
                if (prefs.preferBio && candidate.isBio)
                    totalScore += 5f;

                if (prefs.preferVegan && candidate.isVegan)
                    totalScore += 5f;

                if (prefs.maxPrice > 0 && candidate.price <= prefs.maxPrice)
                    totalScore += 3f;

                // Pénalité pour les allergènes
                if (prefs.allergyRestrictions != null)
                {
                    foreach (var allergen in candidate.allergens)
                    {
                        if (prefs.allergyRestrictions.Contains(allergen))
                        {
                            totalScore -= 20f; // Forte pénalité
                        }
                    }
                }
            }

            // 7. Bonus prioritaires
            if (prioritizeBio && candidate.isBio && !reference.isBio)
                totalScore += 5f;

            if (prioritizePrice && candidate.price < reference.price)
                totalScore += 5f;

            return totalScore;
        }

        /// <summary>
        /// Obtient la valeur numérique d'un Eco-Score
        /// </summary>
        private float GetEcoScoreValue(string score)
        {
            switch (score?.ToUpper())
            {
                case "A": return 15f;
                case "B": return 10f;
                case "C": return 5f;
                case "D": return 2f;
                case "E": return 0f;
                default: return 0f;
            }
        }

        /// <summary>
        /// Obtient la valeur numérique d'un Nutri-Score
        /// </summary>
        private float GetNutriScoreValue(string score)
        {
            switch (score?.ToUpper())
            {
                case "A": return 10f;
                case "B": return 7f;
                case "C": return 5f;
                case "D": return 2f;
                case "E": return 0f;
                default: return 0f;
            }
        }

        /// <summary>
        /// Filtre les recommandations selon les préférences utilisateur
        /// </summary>
        public List<ProductData> FilterRecommendations(List<ProductData> recommendations, UserPreferences prefs)
        {
            if (prefs == null)
                return recommendations;

            return recommendations.Where(p =>
            {
                // Exclure si contient un allergène
                if (prefs.allergyRestrictions != null)
                {
                    foreach (var allergen in p.allergens)
                    {
                        if (prefs.allergyRestrictions.Contains(allergen))
                            return false;
                    }
                }

                // Filtrer par prix max
                if (prefs.maxPrice > 0 && p.price > prefs.maxPrice)
                    return false;

                // Filtrer par préférence bio
                if (prefs.preferBio && !p.isBio)
                    return false;

                // Filtrer par préférence vegan
                if (prefs.preferVegan && !p.isVegan)
                    return false;

                return true;
            }).ToList();
        }

        /// <summary>
        /// Structure pour stocker un produit avec son score
        /// </summary>
        private class RecommendationScore
        {
            public ProductData product;
            public float score;
        }
    }
}
