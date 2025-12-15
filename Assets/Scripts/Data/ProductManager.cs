using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Manages the product database and provides access to product information.
    /// Implements singleton pattern for global access.
    /// </summary>
    public class ProductManager : MonoBehaviour
    {
        private static ProductManager _instance;
        public static ProductManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ProductManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ProductManager");
                        _instance = go.AddComponent<ProductManager>();
                    }
                }
                return _instance;
            }
        }
        
        [Header("Database Settings")]
        [SerializeField] private string databaseFileName = "products_database";
        [SerializeField] private bool loadOnStart = true;
        
        private ProductDatabase database;
        private Dictionary<string, Product> productById;
        private Dictionary<string, Product> productByQRCode;
        
        public event Action<ProductDatabase> OnDatabaseLoaded;
        public event Action<Product> OnProductFound;
        public event Action<string> OnProductNotFound;
        
        public bool IsLoaded => database != null;
        public int ProductCount => database?.products?.Count ?? 0;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            productById = new Dictionary<string, Product>();
            productByQRCode = new Dictionary<string, Product>();
            
            if (loadOnStart)
            {
                LoadDatabase();
            }
        }
        
        /// <summary>
        /// Loads the product database from JSON file in Resources.
        /// </summary>
        public void LoadDatabase()
        {
            try
            {
                TextAsset jsonFile = Resources.Load<TextAsset>(databaseFileName);
                
                if (jsonFile == null)
                {
                    Debug.LogWarning($"Product database file '{databaseFileName}' not found. Creating default database.");
                    CreateDefaultDatabase();
                    return;
                }
                
                database = JsonUtility.FromJson<ProductDatabase>(jsonFile.text);
                BuildIndexes();
                
                Debug.Log($"Product database loaded successfully. {ProductCount} products found.");
                OnDatabaseLoaded?.Invoke(database);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading product database: {ex.Message}");
                CreateDefaultDatabase();
            }
        }
        
        /// <summary>
        /// Creates a default database with sample products.
        /// </summary>
        private void CreateDefaultDatabase()
        {
            database = new ProductDatabase();
            database.products = GetSampleProducts();
            BuildIndexes();
            
            Debug.Log($"Default database created with {ProductCount} sample products.");
            OnDatabaseLoaded?.Invoke(database);
        }
        
        /// <summary>
        /// Builds lookup indexes for fast product retrieval.
        /// </summary>
        private void BuildIndexes()
        {
            productById.Clear();
            productByQRCode.Clear();
            
            if (database?.products == null) return;
            
            foreach (var product in database.products)
            {
                if (!string.IsNullOrEmpty(product.id))
                {
                    productById[product.id] = product;
                }
                
                if (!string.IsNullOrEmpty(product.qrCodeId))
                {
                    productByQRCode[product.qrCodeId] = product;
                }
            }
        }
        
        /// <summary>
        /// Gets a product by its ID.
        /// </summary>
        public Product GetProductById(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            
            productById.TryGetValue(id, out Product product);
            
            if (product != null)
            {
                OnProductFound?.Invoke(product);
            }
            else
            {
                OnProductNotFound?.Invoke(id);
            }
            
            return product;
        }
        
        /// <summary>
        /// Gets a product by its QR code ID.
        /// </summary>
        public Product GetProductByQRCode(string qrCodeId)
        {
            if (string.IsNullOrEmpty(qrCodeId)) return null;
            
            productByQRCode.TryGetValue(qrCodeId, out Product product);
            
            if (product != null)
            {
                OnProductFound?.Invoke(product);
            }
            else
            {
                OnProductNotFound?.Invoke(qrCodeId);
            }
            
            return product;
        }
        
        /// <summary>
        /// Gets all products in the database.
        /// </summary>
        public List<Product> GetAllProducts()
        {
            return database?.products ?? new List<Product>();
        }
        
        /// <summary>
        /// Gets products filtered by category.
        /// </summary>
        public List<Product> GetProductsByCategory(string category)
        {
            if (database?.products == null) return new List<Product>();
            
            return database.products
                .Where(p => p.category?.Equals(category, StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();
        }
        
        /// <summary>
        /// Gets products filtered by tags.
        /// </summary>
        public List<Product> GetProductsByTags(params string[] tags)
        {
            if (database?.products == null) return new List<Product>();
            
            return database.products
                .Where(p => p.tags != null && tags.Any(t => p.tags.Contains(t, StringComparer.OrdinalIgnoreCase)))
                .ToList();
        }
        
        /// <summary>
        /// Gets alternative products for a given product.
        /// </summary>
        public List<Product> GetAlternatives(Product product)
        {
            if (product?.alternativeProductIds == null) return new List<Product>();
            
            return product.alternativeProductIds
                .Select(id => GetProductById(id))
                .Where(p => p != null)
                .ToList();
        }
        
        /// <summary>
        /// Searches products by name or brand.
        /// </summary>
        public List<Product> SearchProducts(string query)
        {
            if (string.IsNullOrEmpty(query) || database?.products == null)
                return new List<Product>();
            
            query = query.ToLower();
            
            return database.products
                .Where(p => 
                    (p.name?.ToLower().Contains(query) ?? false) ||
                    (p.brand?.ToLower().Contains(query) ?? false) ||
                    (p.description?.ToLower().Contains(query) ?? false))
                .ToList();
        }
        
        /// <summary>
        /// Returns sample products for demonstration.
        /// </summary>
        private List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    id = "PROD001",
                    name = "Organic Apple Juice",
                    brand = "BioFresh",
                    description = "100% pure organic apple juice from local orchards",
                    price = 3.99f,
                    currency = "EUR",
                    origin = "France",
                    category = "Beverages",
                    qrCodeId = "QR001",
                    imagePath = "Products/apple_juice",
                    nutrition = new NutritionalInfo
                    {
                        calories = 46,
                        proteins = 0.1f,
                        carbohydrates = 11.3f,
                        sugars = 9.6f,
                        fats = 0.1f,
                        saturatedFats = 0f,
                        fiber = 0.1f,
                        salt = 0.01f,
                        nutriScore = "C",
                        allergens = new List<string>()
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 85,
                        isBio = true,
                        isLocalProduct = true,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.3f,
                        certifications = "AB, EU Bio"
                    },
                    alternativeProductIds = new List<string> { "PROD002", "PROD003" },
                    tags = new List<string> { "bio", "local", "vegan", "organic" }
                },
                new Product
                {
                    id = "PROD002",
                    name = "Premium Orange Juice",
                    brand = "SunnyFruit",
                    description = "Freshly squeezed orange juice with pulp",
                    price = 4.49f,
                    currency = "EUR",
                    origin = "Spain",
                    category = "Beverages",
                    qrCodeId = "QR002",
                    imagePath = "Products/orange_juice",
                    nutrition = new NutritionalInfo
                    {
                        calories = 45,
                        proteins = 0.7f,
                        carbohydrates = 10.4f,
                        sugars = 8.4f,
                        fats = 0.2f,
                        saturatedFats = 0f,
                        fiber = 0.2f,
                        salt = 0f,
                        nutriScore = "C",
                        allergens = new List<string>()
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 65,
                        isBio = false,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.8f,
                        certifications = ""
                    },
                    alternativeProductIds = new List<string> { "PROD001", "PROD003" },
                    tags = new List<string> { "premium", "natural", "vegan" }
                },
                new Product
                {
                    id = "PROD003",
                    name = "Tropical Mix Smoothie",
                    brand = "TropiBlend",
                    description = "Exotic blend of mango, passion fruit, and coconut",
                    price = 5.99f,
                    currency = "EUR",
                    origin = "Belgium",
                    category = "Beverages",
                    qrCodeId = "QR003",
                    imagePath = "Products/tropical_smoothie",
                    nutrition = new NutritionalInfo
                    {
                        calories = 68,
                        proteins = 0.5f,
                        carbohydrates = 15.2f,
                        sugars = 12.8f,
                        fats = 0.8f,
                        saturatedFats = 0.5f,
                        fiber = 0.5f,
                        salt = 0f,
                        nutriScore = "D",
                        allergens = new List<string>()
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 45,
                        isBio = false,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 1.5f,
                        certifications = ""
                    },
                    alternativeProductIds = new List<string> { "PROD001", "PROD002" },
                    tags = new List<string> { "exotic", "premium", "vegan" }
                },
                new Product
                {
                    id = "PROD004",
                    name = "Whole Wheat Bread",
                    brand = "ArtisanBake",
                    description = "Traditional whole wheat sourdough bread",
                    price = 2.99f,
                    currency = "EUR",
                    origin = "France",
                    category = "Bakery",
                    qrCodeId = "QR004",
                    imagePath = "Products/wheat_bread",
                    nutrition = new NutritionalInfo
                    {
                        calories = 247,
                        proteins = 13f,
                        carbohydrates = 41f,
                        sugars = 4f,
                        fats = 3.4f,
                        saturatedFats = 0.6f,
                        fiber = 7f,
                        salt = 1.1f,
                        nutriScore = "A",
                        allergens = new List<string> { "Gluten", "Wheat" }
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 80,
                        isBio = true,
                        isLocalProduct = true,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.5f,
                        certifications = "AB, EU Bio"
                    },
                    alternativeProductIds = new List<string> { "PROD005" },
                    tags = new List<string> { "bio", "local", "artisan", "wholesome" }
                },
                new Product
                {
                    id = "PROD005",
                    name = "Gluten-Free Rice Bread",
                    brand = "FreeFrom",
                    description = "Soft rice bread suitable for gluten-free diets",
                    price = 4.49f,
                    currency = "EUR",
                    origin = "Germany",
                    category = "Bakery",
                    qrCodeId = "QR005",
                    imagePath = "Products/rice_bread",
                    nutrition = new NutritionalInfo
                    {
                        calories = 260,
                        proteins = 4f,
                        carbohydrates = 52f,
                        sugars = 3f,
                        fats = 4f,
                        saturatedFats = 0.5f,
                        fiber = 2f,
                        salt = 0.9f,
                        nutriScore = "B",
                        allergens = new List<string>()
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 60,
                        isBio = false,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.7f,
                        certifications = "Gluten-Free Certified"
                    },
                    alternativeProductIds = new List<string> { "PROD004" },
                    tags = new List<string> { "gluten-free", "allergen-friendly" }
                },
                new Product
                {
                    id = "PROD006",
                    name = "Organic Free-Range Eggs",
                    brand = "HappyHens",
                    description = "Pack of 6 large organic free-range eggs",
                    price = 4.99f,
                    currency = "EUR",
                    origin = "France",
                    category = "Dairy & Eggs",
                    qrCodeId = "QR006",
                    imagePath = "Products/organic_eggs",
                    nutrition = new NutritionalInfo
                    {
                        calories = 155,
                        proteins = 13f,
                        carbohydrates = 1.1f,
                        sugars = 1.1f,
                        fats = 11f,
                        saturatedFats = 3.3f,
                        fiber = 0f,
                        salt = 0.4f,
                        nutriScore = "A",
                        allergens = new List<string> { "Eggs" }
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 75,
                        isBio = true,
                        isLocalProduct = true,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.4f,
                        certifications = "AB, EU Bio, Free-Range"
                    },
                    alternativeProductIds = new List<string>(),
                    tags = new List<string> { "bio", "local", "free-range", "organic" }
                },
                new Product
                {
                    id = "PROD007",
                    name = "Dark Chocolate 70%",
                    brand = "ChocoArt",
                    description = "Premium dark chocolate with 70% cocoa",
                    price = 3.49f,
                    currency = "EUR",
                    origin = "Belgium",
                    category = "Confectionery",
                    qrCodeId = "QR007",
                    imagePath = "Products/dark_chocolate",
                    nutrition = new NutritionalInfo
                    {
                        calories = 598,
                        proteins = 7.8f,
                        carbohydrates = 31f,
                        sugars = 24f,
                        fats = 43f,
                        saturatedFats = 25f,
                        fiber = 11f,
                        salt = 0.02f,
                        nutriScore = "D",
                        allergens = new List<string> { "Milk", "Soy" }
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 55,
                        isBio = false,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 1.2f,
                        certifications = "Fair Trade"
                    },
                    alternativeProductIds = new List<string> { "PROD008" },
                    tags = new List<string> { "premium", "fair-trade", "indulgence" }
                },
                new Product
                {
                    id = "PROD008",
                    name = "Organic Raw Cacao Bar",
                    brand = "RawPure",
                    description = "Organic raw cacao bar with natural sweeteners",
                    price = 5.99f,
                    currency = "EUR",
                    origin = "Switzerland",
                    category = "Confectionery",
                    qrCodeId = "QR008",
                    imagePath = "Products/raw_cacao",
                    nutrition = new NutritionalInfo
                    {
                        calories = 480,
                        proteins = 9f,
                        carbohydrates = 38f,
                        sugars = 18f,
                        fats = 32f,
                        saturatedFats = 19f,
                        fiber = 14f,
                        salt = 0.01f,
                        nutriScore = "C",
                        allergens = new List<string>()
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 82,
                        isBio = true,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.6f,
                        certifications = "AB, EU Bio, Raw, Fair Trade"
                    },
                    alternativeProductIds = new List<string> { "PROD007" },
                    tags = new List<string> { "bio", "raw", "vegan", "organic", "fair-trade" }
                },
                new Product
                {
                    id = "PROD009",
                    name = "Almond Milk Original",
                    brand = "PlantLife",
                    description = "Unsweetened almond milk, perfect dairy alternative",
                    price = 2.49f,
                    currency = "EUR",
                    origin = "Netherlands",
                    category = "Plant-Based",
                    qrCodeId = "QR009",
                    imagePath = "Products/almond_milk",
                    nutrition = new NutritionalInfo
                    {
                        calories = 13,
                        proteins = 0.4f,
                        carbohydrates = 0.1f,
                        sugars = 0f,
                        fats = 1.1f,
                        saturatedFats = 0.1f,
                        fiber = 0.2f,
                        salt = 0.1f,
                        nutriScore = "A",
                        allergens = new List<string> { "Tree Nuts" }
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 70,
                        isBio = false,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.4f,
                        certifications = "Vegan Certified"
                    },
                    alternativeProductIds = new List<string> { "PROD010" },
                    tags = new List<string> { "vegan", "plant-based", "dairy-free", "low-calorie" }
                },
                new Product
                {
                    id = "PROD010",
                    name = "Oat Milk Barista Edition",
                    brand = "OatlyPro",
                    description = "Oat milk specially formulated for coffee and frothing",
                    price = 3.29f,
                    currency = "EUR",
                    origin = "Sweden",
                    category = "Plant-Based",
                    qrCodeId = "QR010",
                    imagePath = "Products/oat_milk",
                    nutrition = new NutritionalInfo
                    {
                        calories = 59,
                        proteins = 1f,
                        carbohydrates = 6.6f,
                        sugars = 4f,
                        fats = 3f,
                        saturatedFats = 0.3f,
                        fiber = 0.8f,
                        salt = 0.1f,
                        nutriScore = "B",
                        allergens = new List<string> { "Oats" }
                    },
                    ecoInfo = new EcoInfo
                    {
                        ecoScore = 85,
                        isBio = true,
                        isLocalProduct = false,
                        isRecyclablePackaging = true,
                        carbonFootprint = 0.3f,
                        certifications = "AB, Vegan Certified"
                    },
                    alternativeProductIds = new List<string> { "PROD009" },
                    tags = new List<string> { "bio", "vegan", "plant-based", "dairy-free", "barista" }
                }
            };
        }
    }
}
