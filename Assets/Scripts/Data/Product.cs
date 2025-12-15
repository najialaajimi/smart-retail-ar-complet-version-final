using System;
using System.Collections.Generic;
using UnityEngine;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Represents a product in the Smart Retail AR system.
    /// Contains all product information including nutrition, origin, and eco-score.
    /// </summary>
    [Serializable]
    public class Product
    {
        public string id;
        public string name;
        public string brand;
        public string description;
        public float price;
        public string currency;
        public string origin;
        public string category;
        public string imagePath;
        public string qrCodeId;
        
        // Nutritional Information
        public NutritionalInfo nutrition;
        
        // Eco-responsibility
        public EcoInfo ecoInfo;
        
        // Alternative products IDs
        public List<string> alternativeProductIds;
        
        // Tags for filtering (e.g., "bio", "vegan", "local")
        public List<string> tags;
        
        public Product()
        {
            alternativeProductIds = new List<string>();
            tags = new List<string>();
            nutrition = new NutritionalInfo();
            ecoInfo = new EcoInfo();
        }
    }
    
    /// <summary>
    /// Nutritional information for a product.
    /// Based on standard food labeling requirements.
    /// </summary>
    [Serializable]
    public class NutritionalInfo
    {
        public float calories;          // kcal per 100g
        public float proteins;          // grams per 100g
        public float carbohydrates;     // grams per 100g
        public float sugars;            // grams per 100g
        public float fats;              // grams per 100g
        public float saturatedFats;     // grams per 100g
        public float fiber;             // grams per 100g
        public float salt;              // grams per 100g
        public string nutriScore;       // A, B, C, D, E
        
        // Allergens
        public List<string> allergens;
        
        public NutritionalInfo()
        {
            allergens = new List<string>();
            nutriScore = "C";
        }
        
        /// <summary>
        /// Gets the color associated with the NutriScore.
        /// </summary>
        public Color GetNutriScoreColor()
        {
            switch (nutriScore?.ToUpper())
            {
                case "A": return new Color(0.03f, 0.55f, 0.27f); // Dark Green
                case "B": return new Color(0.53f, 0.77f, 0.25f); // Light Green
                case "C": return new Color(1f, 0.82f, 0f);       // Yellow
                case "D": return new Color(0.94f, 0.50f, 0.18f); // Orange
                case "E": return new Color(0.88f, 0.23f, 0.16f); // Red
                default: return Color.gray;
            }
        }
    }
    
    /// <summary>
    /// Eco-responsibility information for a product.
    /// </summary>
    [Serializable]
    public class EcoInfo
    {
        public int ecoScore;                // 0-100
        public bool isBio;
        public bool isLocalProduct;
        public bool isRecyclablePackaging;
        public float carbonFootprint;       // kg CO2 equivalent
        public string certifications;       // e.g., "AB, EU Bio, Fair Trade"
        
        public EcoInfo()
        {
            ecoScore = 50;
            isBio = false;
            isLocalProduct = false;
            isRecyclablePackaging = false;
            carbonFootprint = 0f;
            certifications = "";
        }
        
        /// <summary>
        /// Gets the eco-score grade (A-E).
        /// </summary>
        public string GetEcoGrade()
        {
            if (ecoScore >= 80) return "A";
            if (ecoScore >= 60) return "B";
            if (ecoScore >= 40) return "C";
            if (ecoScore >= 20) return "D";
            return "E";
        }
        
        /// <summary>
        /// Gets the color associated with the eco-score.
        /// </summary>
        public Color GetEcoScoreColor()
        {
            if (ecoScore >= 80) return new Color(0.03f, 0.55f, 0.27f);
            if (ecoScore >= 60) return new Color(0.53f, 0.77f, 0.25f);
            if (ecoScore >= 40) return new Color(1f, 0.82f, 0f);
            if (ecoScore >= 20) return new Color(0.94f, 0.50f, 0.18f);
            return new Color(0.88f, 0.23f, 0.16f);
        }
    }
    
    /// <summary>
    /// Container for the product database JSON.
    /// </summary>
    [Serializable]
    public class ProductDatabase
    {
        public List<Product> products;
        public string version;
        public string lastUpdated;
        
        public ProductDatabase()
        {
            products = new List<Product>();
            version = "1.0.0";
            lastUpdated = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
