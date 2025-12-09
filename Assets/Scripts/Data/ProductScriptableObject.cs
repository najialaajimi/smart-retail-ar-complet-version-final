using UnityEngine;
using System.Collections.Generic;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// ScriptableObject pour stocker les données produits dans l'éditeur Unity
    /// Permet de créer des assets réutilisables pour chaque produit
    /// </summary>
    [CreateAssetMenu(fileName = "NewProduct", menuName = "Smart Retail AR/Product", order = 1)]
    public class ProductScriptableObject : ScriptableObject
    {
        [Header("Informations de base")]
        public string productId;
        public string productName;
        public string brand;
        [TextArea(3, 6)]
        public string description;
        public float price;
        public string origin;
        public string category;

        [Header("Nutrition")]
        public float calories;
        public float proteins;
        public float carbs;
        public float fat;
        public float fiber;
        public float sugar;
        public float salt;
        [Tooltip("A, B, C, D ou E")]
        public string nutriScore = "A";

        [Header("Labels")]
        [Tooltip("A, B, C, D ou E")]
        public string ecoScore = "A";
        public bool isBio;
        public bool isVegan;

        [Header("Allergènes")]
        public List<string> allergens = new List<string>();

        [Header("Alternatives")]
        public List<string> alternativeProductIds = new List<string>();

        [Header("Visuels")]
        public Sprite productImage;
        public string imageUrl;

        [Header("QR Code")]
        public string qrCodeId;

        /// <summary>
        /// Convertit le ScriptableObject en ProductData
        /// </summary>
        public ProductData ToProductData()
        {
            ProductData data = new ProductData
            {
                id = productId,
                name = productName,
                brand = brand,
                description = description,
                price = price,
                origin = origin,
                category = category,
                nutrition = new NutritionData
                {
                    calories = calories,
                    proteins = proteins,
                    carbs = carbs,
                    fat = fat,
                    fiber = fiber,
                    sugar = sugar,
                    salt = salt,
                    nutriscore = nutriScore
                },
                ecoScore = ecoScore,
                isBio = isBio,
                isVegan = isVegan,
                allergens = new List<string>(allergens),
                alternatives = new List<string>(alternativeProductIds),
                imageUrl = imageUrl,
                qrCodeId = qrCodeId
            };

            return data;
        }

        /// <summary>
        /// Remplit le ScriptableObject depuis un ProductData
        /// </summary>
        public void FromProductData(ProductData data)
        {
            productId = data.id;
            productName = data.name;
            brand = data.brand;
            description = data.description;
            price = data.price;
            origin = data.origin;
            category = data.category;
            
            calories = data.nutrition.calories;
            proteins = data.nutrition.proteins;
            carbs = data.nutrition.carbs;
            fat = data.nutrition.fat;
            fiber = data.nutrition.fiber;
            sugar = data.nutrition.sugar;
            salt = data.nutrition.salt;
            nutriScore = data.nutrition.nutriscore;
            
            ecoScore = data.ecoScore;
            isBio = data.isBio;
            isVegan = data.isVegan;
            allergens = new List<string>(data.allergens);
            alternativeProductIds = new List<string>(data.alternatives);
            imageUrl = data.imageUrl;
            qrCodeId = data.qrCodeId;
        }
    }
}
