using System;
using UnityEngine;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Représente les informations nutritionnelles d'un produit
    /// </summary>
    [Serializable]
    public class NutritionInfo
    {
        [Tooltip("Calories pour 100g/100ml")]
        public float calories;
        
        [Tooltip("Protéines en grammes")]
        public float proteins;
        
        [Tooltip("Glucides en grammes")]
        public float carbohydrates;
        
        [Tooltip("Lipides en grammes")]
        public float fats;
        
        [Tooltip("Fibres en grammes")]
        public float fiber;
        
        [Tooltip("Sel en grammes")]
        public float salt;
        
        [Tooltip("Score nutritionnel (A, B, C, D, E)")]
        public string nutriScore;

        public NutritionInfo()
        {
            nutriScore = "C";
        }
    }

    /// <summary>
    /// Représente un produit dans le système Smart Retail AR
    /// </summary>
    [Serializable]
    public class Product
    {
        [Tooltip("Identifiant unique du produit")]
        public string id;
        
        [Tooltip("Nom du produit")]
        public string name;
        
        [Tooltip("Marque du produit")]
        public string brand;
        
        [Tooltip("Description détaillée")]
        [TextArea(3, 5)]
        public string description;
        
        [Tooltip("Prix en euros")]
        public float price;
        
        [Tooltip("Pays d'origine")]
        public string origin;
        
        [Tooltip("Catégorie du produit")]
        public string category;
        
        [Tooltip("Informations nutritionnelles")]
        public NutritionInfo nutrition;
        
        [Tooltip("Identifiants des produits alternatifs")]
        public string[] alternatives;
        
        [Tooltip("Score écologique (0-100)")]
        [Range(0, 100)]
        public float ecoScore;
        
        [Tooltip("Produit issu de l'agriculture biologique")]
        public bool isBio;
        
        [Tooltip("Produit éco-responsable")]
        public bool isEcoResponsible;
        
        [Tooltip("URL ou chemin de l'image du produit")]
        public string imageUrl;
        
        [Tooltip("Données du QR code associé")]
        public string qrCodeData;

        public Product()
        {
            nutrition = new NutritionInfo();
            alternatives = new string[0];
            ecoScore = 50f;
        }

        /// <summary>
        /// Retourne une représentation textuelle du produit
        /// </summary>
        public override string ToString()
        {
            return $"{name} ({brand}) - {price}€";
        }

        /// <summary>
        /// Vérifie si le produit correspond aux critères de filtre
        /// </summary>
        public bool MatchesFilter(float maxPrice, float minEcoScore, bool requireBio, bool requireEcoResponsible)
        {
            if (price > maxPrice) return false;
            if (ecoScore < minEcoScore) return false;
            if (requireBio && !isBio) return false;
            if (requireEcoResponsible && !isEcoResponsible) return false;
            return true;
        }
    }
}
