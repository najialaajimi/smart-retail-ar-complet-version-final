using NUnit.Framework;
using SmartRetailAR.Recommendations;
using SmartRetailAR.Products;
using System.Collections.Generic;

namespace SmartRetailAR.Tests.EditMode
{
    /// <summary>
    /// Tests unitaires pour RecommendationEngine
    /// </summary>
    public class RecommendationEngineTests
    {
        private RecommendationEngine engine;
        private ProductDatabase database;

        [SetUp]
        public void Setup()
        {
            // Créer les instances de test
            UnityEngine.GameObject engineGo = new UnityEngine.GameObject("TestEngine");
            engine = engineGo.AddComponent<RecommendationEngine>();

            UnityEngine.GameObject dbGo = new UnityEngine.GameObject("TestDatabase");
            database = dbGo.AddComponent<ProductDatabase>();

            // Charger des données de test
            LoadTestData();
        }

        [TearDown]
        public void Teardown()
        {
            if (engine != null)
            {
                UnityEngine.Object.DestroyImmediate(engine.gameObject);
            }
            if (database != null)
            {
                UnityEngine.Object.DestroyImmediate(database.gameObject);
            }
        }

        [Test]
        public void GetAlternatives_WithValidProduct_ReturnsAlternatives()
        {
            // Arrange
            Product product = database.GetProductById("TEST001");

            // Act
            List<Product> alternatives = engine.GetAlternatives(product, RecommendationCriteria.Combined);

            // Assert
            Assert.IsNotNull(alternatives);
        }

        [Test]
        public void GetAlternatives_WithNullProduct_ReturnsEmptyList()
        {
            // Act
            List<Product> alternatives = engine.GetAlternatives(null);

            // Assert
            Assert.IsNotNull(alternatives);
            Assert.AreEqual(0, alternatives.Count);
        }

        [Test]
        public void GetEcoFriendlyAlternatives_ReturnsEcoProducts()
        {
            // Arrange
            Product product = database.GetProductById("TEST001");

            // Act
            List<Product> ecoAlternatives = engine.GetEcoFriendlyAlternatives(product);

            // Assert
            Assert.IsNotNull(ecoAlternatives);
            foreach (var alt in ecoAlternatives)
            {
                Assert.IsTrue(alt.isEcoResponsible);
                Assert.GreaterOrEqual(alt.ecoScore, 70);
            }
        }

        [Test]
        public void GetSimilarProducts_WithinPriceRange_ReturnsFilteredProducts()
        {
            // Arrange
            Product product = database.GetProductById("TEST001");

            // Act
            List<Product> similar = engine.GetSimilarProducts(product, 1f, 5f);

            // Assert
            Assert.IsNotNull(similar);
            foreach (var prod in similar)
            {
                Assert.GreaterOrEqual(prod.price, 1f);
                Assert.LessOrEqual(prod.price, 5f);
            }
        }

        private void LoadTestData()
        {
            string testJson = @"{
                ""products"": [
                    {
                        ""id"": ""TEST001"",
                        ""name"": ""Product 1"",
                        ""brand"": ""Brand A"",
                        ""price"": 3.50,
                        ""origin"": ""France"",
                        ""category"": ""TestCategory"",
                        ""nutrition"": {
                            ""calories"": 100,
                            ""proteins"": 5,
                            ""carbohydrates"": 10,
                            ""fats"": 2,
                            ""fiber"": 1,
                            ""salt"": 0.5,
                            ""nutriScore"": ""A""
                        },
                        ""ecoScore"": 85,
                        ""isBio"": true,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    },
                    {
                        ""id"": ""TEST002"",
                        ""name"": ""Product 2"",
                        ""brand"": ""Brand B"",
                        ""price"": 4.20,
                        ""origin"": ""France"",
                        ""category"": ""TestCategory"",
                        ""nutrition"": {
                            ""calories"": 120,
                            ""proteins"": 6,
                            ""carbohydrates"": 12,
                            ""fats"": 3,
                            ""fiber"": 2,
                            ""salt"": 0.6,
                            ""nutriScore"": ""B""
                        },
                        ""ecoScore"": 75,
                        ""isBio"": false,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    },
                    {
                        ""id"": ""TEST003"",
                        ""name"": ""Product 3"",
                        ""brand"": ""Brand C"",
                        ""price"": 2.80,
                        ""origin"": ""Italy"",
                        ""category"": ""TestCategory"",
                        ""nutrition"": {
                            ""calories"": 90,
                            ""proteins"": 4,
                            ""carbohydrates"": 9,
                            ""fats"": 1,
                            ""fiber"": 1,
                            ""salt"": 0.4,
                            ""nutriScore"": ""A""
                        },
                        ""ecoScore"": 80,
                        ""isBio"": true,
                        ""isEcoResponsible"": true,
                        ""alternatives"": []
                    }
                ]
            }";

            database.LoadFromJsonString(testJson);
        }
    }
}
