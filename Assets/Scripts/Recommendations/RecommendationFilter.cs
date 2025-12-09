using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SmartRetailAR.Products;

namespace SmartRetailAR.Recommendations
{
    /// <summary>
    /// Type de filtre
    /// </summary>
    public enum FilterType
    {
        Price,
        EcoScore,
        Bio,
        Origin,
        NutriScore,
        Category,
        EcoResponsible
    }

    /// <summary>
    /// Gère les filtres de recherche de produits
    /// </summary>
    [Serializable]
    public class RecommendationFilter
    {
        [Header("Filtres de prix")]
        public bool filterByPrice = false;
        public float minPrice = 0f;
        public float maxPrice = 100f;

        [Header("Filtres écologiques")]
        public bool filterByEcoScore = false;
        public float minEcoScore = 0f;
        public bool requireBio = false;
        public bool requireEcoResponsible = false;

        [Header("Filtres nutritionnels")]
        public bool filterByNutriScore = false;
        public List<string> allowedNutriScores = new List<string> { "A", "B", "C", "D", "E" };

        [Header("Filtres géographiques")]
        public bool filterByOrigin = false;
        public List<string> allowedOrigins = new List<string>();

        [Header("Filtres de catégorie")]
        public bool filterByCategory = false;
        public List<string> allowedCategories = new List<string>();

        /// <summary>
        /// Applique les filtres à une liste de produits
        /// </summary>
        public List<Product> Apply(List<Product> products)
        {
            if (products == null || products.Count == 0)
            {
                return new List<Product>();
            }

            IEnumerable<Product> filtered = products;

            // Filtre de prix
            if (filterByPrice)
            {
                filtered = filtered.Where(p => p.price >= minPrice && p.price <= maxPrice);
            }

            // Filtre eco-score
            if (filterByEcoScore)
            {
                filtered = filtered.Where(p => p.ecoScore >= minEcoScore);
            }

            // Filtre bio
            if (requireBio)
            {
                filtered = filtered.Where(p => p.isBio);
            }

            // Filtre éco-responsable
            if (requireEcoResponsible)
            {
                filtered = filtered.Where(p => p.isEcoResponsible);
            }

            // Filtre nutri-score
            if (filterByNutriScore && allowedNutriScores.Count > 0)
            {
                filtered = filtered.Where(p => 
                    p.nutrition != null && 
                    allowedNutriScores.Contains(p.nutrition.nutriScore.ToUpper()));
            }

            // Filtre origine
            if (filterByOrigin && allowedOrigins.Count > 0)
            {
                filtered = filtered.Where(p => allowedOrigins.Contains(p.origin));
            }

            // Filtre catégorie
            if (filterByCategory && allowedCategories.Count > 0)
            {
                filtered = filtered.Where(p => allowedCategories.Contains(p.category));
            }

            return filtered.ToList();
        }

        /// <summary>
        /// Réinitialise tous les filtres
        /// </summary>
        public void Reset()
        {
            filterByPrice = false;
            minPrice = 0f;
            maxPrice = 100f;

            filterByEcoScore = false;
            minEcoScore = 0f;
            requireBio = false;
            requireEcoResponsible = false;

            filterByNutriScore = false;
            allowedNutriScores = new List<string> { "A", "B", "C", "D", "E" };

            filterByOrigin = false;
            allowedOrigins.Clear();

            filterByCategory = false;
            allowedCategories.Clear();
        }

        /// <summary>
        /// Active un filtre spécifique
        /// </summary>
        public void EnableFilter(FilterType filterType, bool enable = true)
        {
            switch (filterType)
            {
                case FilterType.Price:
                    filterByPrice = enable;
                    break;
                case FilterType.EcoScore:
                    filterByEcoScore = enable;
                    break;
                case FilterType.Bio:
                    requireBio = enable;
                    break;
                case FilterType.Origin:
                    filterByOrigin = enable;
                    break;
                case FilterType.NutriScore:
                    filterByNutriScore = enable;
                    break;
                case FilterType.Category:
                    filterByCategory = enable;
                    break;
                case FilterType.EcoResponsible:
                    requireEcoResponsible = enable;
                    break;
            }
        }

        /// <summary>
        /// Compte le nombre de filtres actifs
        /// </summary>
        public int GetActiveFilterCount()
        {
            int count = 0;

            if (filterByPrice) count++;
            if (filterByEcoScore) count++;
            if (requireBio) count++;
            if (requireEcoResponsible) count++;
            if (filterByNutriScore) count++;
            if (filterByOrigin) count++;
            if (filterByCategory) count++;

            return count;
        }

        /// <summary>
        /// Vérifie si des filtres sont actifs
        /// </summary>
        public bool HasActiveFilters()
        {
            return GetActiveFilterCount() > 0;
        }

        /// <summary>
        /// Clone le filtre
        /// </summary>
        public RecommendationFilter Clone()
        {
            RecommendationFilter clone = new RecommendationFilter
            {
                filterByPrice = this.filterByPrice,
                minPrice = this.minPrice,
                maxPrice = this.maxPrice,
                filterByEcoScore = this.filterByEcoScore,
                minEcoScore = this.minEcoScore,
                requireBio = this.requireBio,
                requireEcoResponsible = this.requireEcoResponsible,
                filterByNutriScore = this.filterByNutriScore,
                allowedNutriScores = new List<string>(this.allowedNutriScores),
                filterByOrigin = this.filterByOrigin,
                allowedOrigins = new List<string>(this.allowedOrigins),
                filterByCategory = this.filterByCategory,
                allowedCategories = new List<string>(this.allowedCategories)
            };

            return clone;
        }
    }
}
