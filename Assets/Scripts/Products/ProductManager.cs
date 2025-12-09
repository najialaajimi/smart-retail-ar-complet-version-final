using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using SmartRetailAR.Data;
using SmartRetailAR.Utils;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Gestionnaire central des produits de l'application
    /// </summary>
    public class ProductManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string productsJsonPath = "Data/Products/products";
        [SerializeField] private bool loadOnStart = true;

        // Base de données des produits
        private Dictionary<string, ProductData> m_ProductsById = new Dictionary<string, ProductData>();
        private Dictionary<string, List<ProductData>> m_ProductsByCategory = new Dictionary<string, List<ProductData>>();
        private List<ProductData> m_AllProducts = new List<ProductData>();

        // Produit actuellement sélectionné
        private ProductData m_CurrentProduct;

        // Events
        public event System.Action<ProductData> OnProductLoaded;
        public event System.Action<List<ProductData>> OnProductsLoaded;

        /// <summary>
        /// Produit actuellement sélectionné
        /// </summary>
        public ProductData CurrentProduct => m_CurrentProduct;

        /// <summary>
        /// Liste de tous les produits
        /// </summary>
        public List<ProductData> AllProducts => m_AllProducts;

        private void Start()
        {
            if (loadOnStart)
            {
                LoadProducts();
            }
        }

        /// <summary>
        /// Charge tous les produits depuis le fichier JSON
        /// </summary>
        public void LoadProducts()
        {
            DebugLogger.Log("Chargement des produits...", LogLevel.Info);

            ProductDataList productList = JsonDataLoader.LoadProducts(productsJsonPath);

            if (productList == null || productList.products == null || productList.products.Count == 0)
            {
                DebugLogger.LogError("Aucun produit chargé");
                return;
            }

            m_AllProducts = productList.products;
            m_ProductsById.Clear();
            m_ProductsByCategory.Clear();

            // Indexer les produits
            foreach (var product in m_AllProducts)
            {
                // Par ID
                if (!m_ProductsById.ContainsKey(product.id))
                {
                    m_ProductsById.Add(product.id, product);
                }

                // Par catégorie
                if (!m_ProductsByCategory.ContainsKey(product.category))
                {
                    m_ProductsByCategory[product.category] = new List<ProductData>();
                }
                m_ProductsByCategory[product.category].Add(product);
            }

            DebugLogger.Log($"✓ {m_AllProducts.Count} produits chargés avec succès", LogLevel.Info);
            OnProductsLoaded?.Invoke(m_AllProducts);
        }

        /// <summary>
        /// Récupère un produit par son ID
        /// </summary>
        public ProductData GetProductById(string productId)
        {
            if (m_ProductsById.ContainsKey(productId))
            {
                return m_ProductsById[productId];
            }

            DebugLogger.LogWarning($"Produit non trouvé: {productId}");
            return null;
        }

        /// <summary>
        /// Récupère un produit par son QR code ID
        /// </summary>
        public ProductData GetProductByQRCode(string qrCodeId)
        {
            ProductData product = m_AllProducts.FirstOrDefault(p => p.qrCodeId == qrCodeId);
            
            if (product != null)
            {
                DebugLogger.Log($"Produit trouvé via QR code {qrCodeId}: {product.name}", LogLevel.Info);
            }
            else
            {
                DebugLogger.LogWarning($"Aucun produit trouvé pour le QR code: {qrCodeId}");
            }

            return product;
        }

        /// <summary>
        /// Récupère tous les produits d'une catégorie
        /// </summary>
        public List<ProductData> GetProductsByCategory(string category)
        {
            if (m_ProductsByCategory.ContainsKey(category))
            {
                return m_ProductsByCategory[category];
            }

            return new List<ProductData>();
        }

        /// <summary>
        /// Recherche des produits par nom ou marque
        /// </summary>
        public List<ProductData> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return m_AllProducts;

            searchTerm = searchTerm.ToLower();

            return m_AllProducts.Where(p =>
                p.name.ToLower().Contains(searchTerm) ||
                p.brand.ToLower().Contains(searchTerm) ||
                p.description.ToLower().Contains(searchTerm)
            ).ToList();
        }

        /// <summary>
        /// Filtre les produits selon des critères
        /// </summary>
        public List<ProductData> FilterProducts(ProductFilter filter)
        {
            IEnumerable<ProductData> filtered = m_AllProducts;

            if (filter.onlyBio)
            {
                filtered = filtered.Where(p => p.isBio);
            }

            if (filter.onlyVegan)
            {
                filtered = filtered.Where(p => p.isVegan);
            }

            if (filter.maxPrice > 0)
            {
                filtered = filtered.Where(p => p.price <= filter.maxPrice);
            }

            if (!string.IsNullOrEmpty(filter.category))
            {
                filtered = filtered.Where(p => p.category == filter.category);
            }

            if (filter.minEcoScore != null)
            {
                filtered = filtered.Where(p => CompareEcoScore(p.ecoScore, filter.minEcoScore) >= 0);
            }

            return filtered.ToList();
        }

        /// <summary>
        /// Compare deux Eco-Scores (A > B > C > D > E)
        /// </summary>
        private int CompareEcoScore(string score1, string score2)
        {
            int GetScoreValue(string score)
            {
                switch (score.ToUpper())
                {
                    case "A": return 5;
                    case "B": return 4;
                    case "C": return 3;
                    case "D": return 2;
                    case "E": return 1;
                    default: return 0;
                }
            }

            return GetScoreValue(score1).CompareTo(GetScoreValue(score2));
        }

        /// <summary>
        /// Définit le produit actuellement sélectionné
        /// </summary>
        public void SetCurrentProduct(ProductData product)
        {
            m_CurrentProduct = product;
            
            if (product != null)
            {
                DebugLogger.Log($"Produit sélectionné: {product.name}", LogLevel.Info);
                OnProductLoaded?.Invoke(product);
            }
        }

        /// <summary>
        /// Définit le produit actuel par son ID
        /// </summary>
        public void SetCurrentProductById(string productId)
        {
            ProductData product = GetProductById(productId);
            SetCurrentProduct(product);
        }

        /// <summary>
        /// Récupère les produits alternatifs pour un produit donné
        /// </summary>
        public List<ProductData> GetAlternatives(ProductData product)
        {
            if (product == null || product.alternatives == null || product.alternatives.Count == 0)
            {
                return new List<ProductData>();
            }

            List<ProductData> alternatives = new List<ProductData>();
            
            foreach (string altId in product.alternatives)
            {
                ProductData alt = GetProductById(altId);
                if (alt != null)
                {
                    alternatives.Add(alt);
                }
            }

            return alternatives;
        }

        /// <summary>
        /// Récupère toutes les catégories disponibles
        /// </summary>
        public List<string> GetAllCategories()
        {
            return m_ProductsByCategory.Keys.ToList();
        }
    }

    /// <summary>
    /// Critères de filtrage des produits
    /// </summary>
    [System.Serializable]
    public class ProductFilter
    {
        public bool onlyBio = false;
        public bool onlyVegan = false;
        public float maxPrice = 0f;
        public string category = "";
        public string minEcoScore = null;
    }
}
