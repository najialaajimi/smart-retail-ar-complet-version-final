using NUnit.Framework;
using SmartRetailAR.Products;
using System.Collections.Generic;

namespace SmartRetailAR.Tests.EditMode
{
    /// <summary>
    /// Tests unitaires pour ProductDatabase
    /// </summary>
    public class ProductDatabaseTests
    {
        private ProductDatabase database;

        [SetUp]
        public void Setup()
        {
            // Créer une instance de test
            UnityEngine.GameObject go = new UnityEngine.GameObject("TestDatabase");
            database = go.AddComponent<ProductDatabase>();
        }

        [TearDown]
        public void Teardown()
        {
            if (database != null)
            {
                UnityEngine.Object.DestroyImmediate(database.gameObject);
            }
        }

        [Test]
        public void LoadFromJsonString_ValidJson_ReturnsTrue()
        {
            // Arrange
            string validJson = @"{
                ""products"": [
                    {
                        ""id"": ""TEST001"",
                        ""name"": ""Test Product"",
                        ""brand"": ""Test Brand"",
                        ""price"": 1.99,
                        ""origin"": ""France"",
                        ""category"": ""Test"",
                        ""nutrition"": {
                            ""calories"": 100,
                            ""proteins"": 5,
                            ""carbohydrates"": 10,
                            ""fats"": 2,
                            ""fiber"": 1,
                            ""salt"": 0.5,
                            ""nutriScore"": ""A""
                        },
                        ""ecoScore"": 75,
                        ""isBio"": true,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    }
                ]
            }";

            // Act
            bool result = database.LoadFromJsonString(validJson);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(database.IsInitialized());
        }

        [Test]
        public void GetProductById_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            string json = CreateTestJsonWithProduct("PROD001", "Test Product");
            database.LoadFromJsonString(json);

            // Act
            Product product = database.GetProductById("PROD001");

            // Assert
            Assert.IsNotNull(product);
            Assert.AreEqual("PROD001", product.id);
            Assert.AreEqual("Test Product", product.name);
        }

        [Test]
        public void GetProductById_NonExistingProduct_ReturnsNull()
        {
            // Arrange
            string json = CreateTestJsonWithProduct("PROD001", "Test Product");
            database.LoadFromJsonString(json);

            // Act
            Product product = database.GetProductById("NONEXISTENT");

            // Assert
            Assert.IsNull(product);
        }

        [Test]
        public void SearchProducts_WithMatchingName_ReturnsProducts()
        {
            // Arrange
            string json = CreateTestJsonWithMultipleProducts();
            database.LoadFromJsonString(json);

            // Act
            List<Product> results = database.SearchProducts("Test");

            // Assert
            Assert.IsNotNull(results);
            Assert.Greater(results.Count, 0);
        }

        [Test]
        public void FilterProducts_WithPriceFilter_ReturnsFilteredProducts()
        {
            // Arrange
            string json = CreateTestJsonWithMultipleProducts();
            database.LoadFromJsonString(json);

            // Act
            List<Product> results = database.FilterProducts(maxPrice: 5f);

            // Assert
            Assert.IsNotNull(results);
            foreach (var product in results)
            {
                Assert.LessOrEqual(product.price, 5f);
            }
        }

        private string CreateTestJsonWithProduct(string id, string name)
        {
            return $@"{{
                ""products"": [
                    {{
                        ""id"": ""{id}"",
                        ""name"": ""{name}"",
                        ""brand"": ""Test Brand"",
                        ""price"": 2.99,
                        ""origin"": ""France"",
                        ""category"": ""Test"",
                        ""nutrition"": {{
                            ""calories"": 100,
                            ""proteins"": 5,
                            ""carbohydrates"": 10,
                            ""fats"": 2,
                            ""fiber"": 1,
                            ""salt"": 0.5,
                            ""nutriScore"": ""B""
                        }},
                        ""ecoScore"": 70,
                        ""isBio"": false,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    }}
                ]
            }}";
        }

        private string CreateTestJsonWithMultipleProducts()
        {
            return @"{
                ""products"": [
                    {
                        ""id"": ""TEST001"",
                        ""name"": ""Test Product 1"",
                        ""brand"": ""Brand A"",
                        ""price"": 2.99,
                        ""origin"": ""France"",
                        ""category"": ""Category1"",
                        ""nutrition"": {
                            ""calories"": 100,
                            ""proteins"": 5,
                            ""carbohydrates"": 10,
                            ""fats"": 2,
                            ""fiber"": 1,
                            ""salt"": 0.5,
                            ""nutriScore"": ""A""
                        },
                        ""ecoScore"": 80,
                        ""isBio"": true,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    },
                    {
                        ""id"": ""TEST002"",
                        ""name"": ""Test Product 2"",
                        ""brand"": ""Brand B"",
                        ""price"": 8.99,
                        ""origin"": ""Germany"",
                        ""category"": ""Category2"",
                        ""nutrition"": {
                            ""calories"": 150,
                            ""proteins"": 8,
                            ""carbohydrates"": 15,
                            ""fats"": 5,
                            ""fiber"": 2,
                            ""salt"": 1.0,
                            ""nutriScore"": ""C""
                        },
                        ""ecoScore"": 60,
                        ""isBio"": false,
                        ""isEcoResponsible"": false,
                        ""alternatives"": []
                    }
                ]
            }";
        }
    }
}
