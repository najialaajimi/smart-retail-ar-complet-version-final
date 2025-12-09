using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Conteneur pour la sérialisation JSON de la liste de produits
    /// </summary>
    [Serializable]
    public class ProductList
    {
        public List<Product> products;
    }

    /// <summary>
    /// Gère la base de données des produits
    /// Charge, stocke et recherche les produits
    /// </summary>
    public class ProductDatabase : MonoBehaviour
    {
        private static ProductDatabase instance;
        public static ProductDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ProductDatabase>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ProductDatabase");
                        instance = go.AddComponent<ProductDatabase>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        private TextAsset jsonDatabase;

        private Dictionary<string, Product> productDictionary = new Dictionary<string, Product>();
        private List<Product> productList = new List<Product>();

        private bool isInitialized = false;

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
                return;
            }
        }

        /// <summary>
        /// Charge les produits depuis un fichier JSON
        /// </summary>
        public bool LoadFromJson()
        {
            try
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("Data/products");
                if (jsonFile == null)
                {
                    Debug.LogError("Fichier products.json introuvable dans Resources/Data/");
                    return false;
                }

                return LoadFromJsonString(jsonFile.text);
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors du chargement de la base de données: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge les produits depuis une chaîne JSON
        /// </summary>
        public bool LoadFromJsonString(string jsonString)
        {
            try
            {
                ProductList productListData = JsonUtility.FromJson<ProductList>(jsonString);
                
                if (productListData == null || productListData.products == null)
                {
                    Debug.LogError("Format JSON invalide");
                    return false;
                }

                productList = productListData.products;
                productDictionary.Clear();

                foreach (var product in productList)
                {
                    if (!string.IsNullOrEmpty(product.id))
                    {
                        productDictionary[product.id] = product;
                    }
                }

                isInitialized = true;
                Debug.Log($"Base de données chargée avec {productList.Count} produits");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors du parsing JSON: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Récupère un produit par son identifiant
        /// </summary>
        public Product GetProductById(string id)
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            if (productDictionary.TryGetValue(id, out Product product))
            {
                return product;
            }

            Debug.LogWarning($"Produit avec l'ID {id} introuvable");
            return null;
        }

        /// <summary>
        /// Récupère un produit par les données de son QR code
        /// </summary>
        public Product GetProductByQRCode(string qrCodeData)
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            return productList.FirstOrDefault(p => p.qrCodeData == qrCodeData);
        }

        /// <summary>
        /// Recherche des produits par nom ou marque
        /// </summary>
        public List<Product> SearchProducts(string searchTerm)
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            if (string.IsNullOrEmpty(searchTerm))
            {
                return productList;
            }

            searchTerm = searchTerm.ToLower();
            return productList.Where(p =>
                p.name.ToLower().Contains(searchTerm) ||
                p.brand.ToLower().Contains(searchTerm) ||
                p.category.ToLower().Contains(searchTerm)
            ).ToList();
        }

        /// <summary>
        /// Récupère tous les produits
        /// </summary>
        public List<Product> GetAllProducts()
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            return new List<Product>(productList);
        }

        /// <summary>
        /// Filtre les produits selon des critères
        /// </summary>
        public List<Product> FilterProducts(float maxPrice = float.MaxValue, 
            float minEcoScore = 0f, 
            bool requireBio = false, 
            bool requireEcoResponsible = false)
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            return productList.Where(p => p.MatchesFilter(maxPrice, minEcoScore, requireBio, requireEcoResponsible)).ToList();
        }

        /// <summary>
        /// Récupère les produits d'une catégorie spécifique
        /// </summary>
        public List<Product> GetProductsByCategory(string category)
        {
            if (!isInitialized)
            {
                LoadFromJson();
            }

            return productList.Where(p => p.category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Vérifie si la base de données est initialisée
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized;
        }
    }
}
