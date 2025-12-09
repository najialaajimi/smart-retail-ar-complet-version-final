using UnityEngine;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// ScriptableObject pour stocker les données d'un produit
    /// Permet de créer des assets de produits dans l'éditeur Unity
    /// </summary>
    [CreateAssetMenu(fileName = "NewProduct", menuName = "Smart Retail AR/Product", order = 1)]
    public class ProductScriptableObject : ScriptableObject
    {
        [Header("Informations de base")]
        public string productId;
        public string productName;
        public string brand;
        
        [TextArea(3, 5)]
        public string description;
        
        public float price;
        public string origin;
        public string category;

        [Header("Nutrition")]
        public NutritionInfo nutrition;

        [Header("Éco-responsabilité")]
        [Range(0, 100)]
        public float ecoScore = 50f;
        public bool isBio;
        public bool isEcoResponsible;

        [Header("Alternatives")]
        public ProductScriptableObject[] alternativeProducts;

        [Header("Médias")]
        public Sprite productImage;
        public string qrCodeData;

        /// <summary>
        /// Convertit le ScriptableObject en instance de Product
        /// </summary>
        public Product ToProduct()
        {
            Product product = new Product
            {
                id = productId,
                name = productName,
                brand = brand,
                description = description,
                price = price,
                origin = origin,
                category = category,
                nutrition = nutrition,
                ecoScore = ecoScore,
                isBio = isBio,
                isEcoResponsible = isEcoResponsible,
                qrCodeData = qrCodeData,
                imageUrl = productImage != null ? productImage.name : ""
            };

            if (alternativeProducts != null && alternativeProducts.Length > 0)
            {
                product.alternatives = new string[alternativeProducts.Length];
                for (int i = 0; i < alternativeProducts.Length; i++)
                {
                    if (alternativeProducts[i] != null)
                    {
                        product.alternatives[i] = alternativeProducts[i].productId;
                    }
                }
            }

            return product;
        }
    }
}
