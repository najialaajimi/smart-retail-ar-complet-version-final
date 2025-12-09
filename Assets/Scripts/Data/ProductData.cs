using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Données nutritionnelles d'un produit
    /// </summary>
    [Serializable]
    public class NutritionData
    {
        public float calories;
        public float proteins;
        public float carbs;
        public float fat;
        public float fiber;
        public float sugar;
        public float salt;
        public string nutriscore;
    }

    /// <summary>
    /// Représente un produit dans l'application Smart Retail AR
    /// </summary>
    [Serializable]
    public class ProductData
    {
        public string id;
        public string name;
        public string brand;
        public string description;
        public float price;
        public string origin;
        public string category;
        public NutritionData nutrition;
        public string ecoScore;
        public bool isBio;
        public bool isVegan;
        public List<string> allergens;
        public List<string> alternatives;
        public string imageUrl;
        public string qrCodeId;

        /// <summary>
        /// Calcule un score de recommandation par rapport à un autre produit
        /// </summary>
        public float CalculateRecommendationScore(ProductData referenceProduct, UserPreferences preferences = null)
        {
            float score = 0f;

            // Score basé sur la catégorie (même catégorie = +10 points)
            if (category == referenceProduct.category)
                score += 10f;

            // Score basé sur le prix (similaire ou moins cher = +5 à +10 points)
            float priceDiff = Mathf.Abs(price - referenceProduct.price);
            if (price <= referenceProduct.price)
                score += 10f - Mathf.Min(priceDiff * 2f, 5f);

            // Score écologique (meilleur = +15 points)
            score += GetEcoScoreValue(ecoScore) - GetEcoScoreValue(referenceProduct.ecoScore);

            // Bonus bio (+10 points)
            if (isBio && !referenceProduct.isBio)
                score += 10f;

            // Bonus vegan si préférence utilisateur (+5 points)
            if (preferences != null && preferences.preferVegan && isVegan)
                score += 5f;

            // Bonus nutrition (meilleur Nutri-Score = +10 points)
            score += GetNutriScoreValue(nutrition.nutriscore) - GetNutriScoreValue(referenceProduct.nutrition.nutriscore);

            return Mathf.Max(0, score);
        }

        private float GetEcoScoreValue(string score)
        {
            switch (score.ToUpper())
            {
                case "A": return 15f;
                case "B": return 10f;
                case "C": return 5f;
                case "D": return 2f;
                case "E": return 0f;
                default: return 0f;
            }
        }

        private float GetNutriScoreValue(string score)
        {
            switch (score.ToUpper())
            {
                case "A": return 10f;
                case "B": return 7f;
                case "C": return 5f;
                case "D": return 2f;
                case "E": return 0f;
                default: return 0f;
            }
        }

        public override string ToString()
        {
            return $"{brand} - {name} ({price:F2}€)";
        }
    }

    /// <summary>
    /// Wrapper pour la désérialisation JSON
    /// </summary>
    [Serializable]
    public class ProductDataList
    {
        public List<ProductData> products;
    }

    /// <summary>
    /// Préférences utilisateur pour les recommandations
    /// </summary>
    [Serializable]
    public class UserPreferences
    {
        public bool preferBio = false;
        public bool preferVegan = false;
        public bool preferLocal = false;
        public float maxPrice = 999f;
        public List<string> allergyRestrictions = new List<string>();
    }
}
