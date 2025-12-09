using System;
using System.IO;
using UnityEngine;
using SmartRetailAR.Data;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Utilitaire pour charger des données JSON depuis les fichiers ou Resources
    /// </summary>
    public static class JsonDataLoader
    {
        /// <summary>
        /// Charge la liste de produits depuis le fichier JSON
        /// </summary>
        public static ProductDataList LoadProducts(string filePath = "Data/Products/products")
        {
            try
            {
                // Essayer de charger depuis Resources en premier
                TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
                
                if (jsonFile != null)
                {
                    ProductDataList productList = JsonUtility.FromJson<ProductDataList>(jsonFile.text);
                    
                    if (productList != null && productList.products != null)
                    {
                        DebugLogger.Log($"Produits chargés avec succès: {productList.products.Count} produits", LogLevel.Info);
                        return productList;
                    }
                }
                else
                {
                    // Essayer de charger depuis le chemin absolu du fichier
                    string fullPath = Path.Combine(Application.dataPath, filePath + ".json");
                    if (File.Exists(fullPath))
                    {
                        string jsonContent = File.ReadAllText(fullPath);
                        ProductDataList productList = JsonUtility.FromJson<ProductDataList>(jsonContent);
                        
                        if (productList != null && productList.products != null)
                        {
                            DebugLogger.Log($"Produits chargés depuis fichier: {productList.products.Count} produits", LogLevel.Info);
                            return productList;
                        }
                    }
                }

                DebugLogger.LogError($"Impossible de charger les produits depuis {filePath}");
                return new ProductDataList { products = new System.Collections.Generic.List<ProductData>() };
            }
            catch (Exception e)
            {
                DebugLogger.LogError($"Erreur lors du chargement des produits: {e.Message}");
                return new ProductDataList { products = new System.Collections.Generic.List<ProductData>() };
            }
        }

        /// <summary>
        /// Charge la liste de catégories depuis le fichier JSON
        /// </summary>
        public static CategoryDataList LoadCategories(string filePath = "Data/Categories/categories")
        {
            try
            {
                // Essayer de charger depuis Resources
                TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
                
                if (jsonFile != null)
                {
                    CategoryDataList categoryList = JsonUtility.FromJson<CategoryDataList>(jsonFile.text);
                    
                    if (categoryList != null && categoryList.categories != null)
                    {
                        DebugLogger.Log($"Catégories chargées avec succès: {categoryList.categories.Count} catégories", LogLevel.Info);
                        return categoryList;
                    }
                }
                else
                {
                    // Essayer de charger depuis le chemin absolu du fichier
                    string fullPath = Path.Combine(Application.dataPath, filePath + ".json");
                    if (File.Exists(fullPath))
                    {
                        string jsonContent = File.ReadAllText(fullPath);
                        CategoryDataList categoryList = JsonUtility.FromJson<CategoryDataList>(jsonContent);
                        
                        if (categoryList != null && categoryList.categories != null)
                        {
                            DebugLogger.Log($"Catégories chargées depuis fichier: {categoryList.categories.Count} catégories", LogLevel.Info);
                            return categoryList;
                        }
                    }
                }

                DebugLogger.LogError($"Impossible de charger les catégories depuis {filePath}");
                return new CategoryDataList { categories = new System.Collections.Generic.List<CategoryData>() };
            }
            catch (Exception e)
            {
                DebugLogger.LogError($"Erreur lors du chargement des catégories: {e.Message}");
                return new CategoryDataList { categories = new System.Collections.Generic.List<CategoryData>() };
            }
        }

        /// <summary>
        /// Sauvegarde les produits dans un fichier JSON
        /// </summary>
        public static bool SaveProducts(ProductDataList productList, string filePath)
        {
            try
            {
                string json = JsonUtility.ToJson(productList, true);
                string fullPath = Path.Combine(Application.dataPath, filePath);
                
                // Créer le dossier si nécessaire
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(fullPath, json);
                DebugLogger.Log($"Produits sauvegardés avec succès dans {fullPath}", LogLevel.Info);
                return true;
            }
            catch (Exception e)
            {
                DebugLogger.LogError($"Erreur lors de la sauvegarde des produits: {e.Message}");
                return false;
            }
        }
    }
}
