using UnityEngine;
using System.Collections.Generic;
using SmartRetailAR.Data;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Base de données ScriptableObject pour stocker les produits
    /// Permet de gérer les produits directement dans l'éditeur Unity
    /// </summary>
    [CreateAssetMenu(fileName = "ProductDatabase", menuName = "Smart Retail AR/Product Database", order = 3)]
    public class ProductDatabase : ScriptableObject
    {
        [Header("Produits")]
        [SerializeField] private List<ProductScriptableObject> products = new List<ProductScriptableObject>();

        [Header("Configuration")]
        [SerializeField] private bool autoLoadFromJson = true;
        [SerializeField] private string jsonFilePath = "Data/Products/products";

        /// <summary>
        /// Liste des produits ScriptableObject
        /// </summary>
        public List<ProductScriptableObject> Products => products;

        /// <summary>
        /// Récupère tous les produits sous forme de ProductData
        /// </summary>
        public List<ProductData> GetAllProductData()
        {
            List<ProductData> productDataList = new List<ProductData>();

            foreach (var product in products)
            {
                if (product != null)
                {
                    productDataList.Add(product.ToProductData());
                }
            }

            return productDataList;
        }

        /// <summary>
        /// Trouve un produit par son ID
        /// </summary>
        public ProductScriptableObject FindProductById(string productId)
        {
            return products.Find(p => p != null && p.productId == productId);
        }

        /// <summary>
        /// Trouve un produit par son QR code ID
        /// </summary>
        public ProductScriptableObject FindProductByQRCode(string qrCodeId)
        {
            return products.Find(p => p != null && p.qrCodeId == qrCodeId);
        }

        /// <summary>
        /// Ajoute un produit à la base de données
        /// </summary>
        public void AddProduct(ProductScriptableObject product)
        {
            if (product != null && !products.Contains(product))
            {
                products.Add(product);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        /// <summary>
        /// Supprime un produit de la base de données
        /// </summary>
        public void RemoveProduct(ProductScriptableObject product)
        {
            if (product != null && products.Contains(product))
            {
                products.Remove(product);
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }

        /// <summary>
        /// Vide la base de données
        /// </summary>
        public void Clear()
        {
            products.Clear();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Charge les produits depuis le fichier JSON (éditeur uniquement)
        /// </summary>
        [ContextMenu("Load from JSON")]
        public void LoadFromJson()
        {
            Utils.DebugLogger.Log("Chargement des produits depuis JSON...", Utils.LogLevel.Info);

            var productList = Utils.JsonDataLoader.LoadProducts(jsonFilePath);

            if (productList == null || productList.products == null)
            {
                Utils.DebugLogger.LogError("Impossible de charger les produits");
                return;
            }

            Clear();

            foreach (var productData in productList.products)
            {
                // Créer un nouveau ScriptableObject pour chaque produit
                ProductScriptableObject productSO = ScriptableObject.CreateInstance<ProductScriptableObject>();
                productSO.FromProductData(productData);
                productSO.name = productData.id;

                // Sauvegarder en tant qu'asset
                string path = $"Assets/Resources/Products/{productData.id}.asset";
                UnityEditor.AssetDatabase.CreateAsset(productSO, path);

                products.Add(productSO);
            }

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();

            Utils.DebugLogger.Log($"✓ {products.Count} produits chargés depuis JSON", Utils.LogLevel.Info);
        }

        /// <summary>
        /// Exporte la base de données vers JSON (éditeur uniquement)
        /// </summary>
        [ContextMenu("Export to JSON")]
        public void ExportToJson()
        {
            ProductDataList productList = new ProductDataList
            {
                products = GetAllProductData()
            };

            Utils.JsonDataLoader.SaveProducts(productList, jsonFilePath + ".json");
        }
#endif
    }
}
