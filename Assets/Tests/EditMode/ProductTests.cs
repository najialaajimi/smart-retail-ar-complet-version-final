using NUnit.Framework;
using UnityEngine;
using SmartRetailAR.Data;
using SmartRetailAR.Products;
using System.Collections.Generic;

namespace SmartRetailAR.Tests.EditMode
{
    /// <summary>
    /// Tests unitaires pour le système de produits
    /// </summary>
    public class ProductTests
    {
        private ProductData testProduct;
        private ProductData alternativeProduct;

        [SetUp]
        public void Setup()
        {
            // Créer un produit de test
            testProduct = new ProductData
            {
                id = "TEST001",
                name = "Test Product",
                brand = "Test Brand",
                price = 2.50f,
                category = "Test Category",
                nutrition = new NutritionData
                {
                    calories = 100,
                    proteins = 5,
                    carbs = 20,
                    fat = 3,
                    nutriscore = "A"
                },
                ecoScore = "A",
                isBio = true,
                isVegan = false,
                allergens = new List<string> { "Lactose" }
            };

            // Créer un produit alternatif
            alternativeProduct = new ProductData
            {
                id = "TEST002",
                name = "Alternative Product",
                brand = "Alt Brand",
                price = 3.00f,
                category = "Test Category",
                nutrition = new NutritionData
                {
                    calories = 80,
                    proteins = 6,
                    carbs = 15,
                    fat = 2,
                    nutriscore = "A"
                },
                ecoScore = "A",
                isBio = true,
                isVegan = true,
                allergens = new List<string>()
            };
        }

        [Test]
        public void ProductData_Creation_IsValid()
        {
            Assert.IsNotNull(testProduct);
            Assert.AreEqual("TEST001", testProduct.id);
            Assert.AreEqual("Test Product", testProduct.name);
            Assert.AreEqual(2.50f, testProduct.price);
        }

        [Test]
        public void ProductData_NutritionData_IsValid()
        {
            Assert.IsNotNull(testProduct.nutrition);
            Assert.AreEqual(100, testProduct.nutrition.calories);
            Assert.AreEqual("A", testProduct.nutrition.nutriscore);
        }

        [Test]
        public void ProductData_CalculateRecommendationScore_SameCategory_ReturnsHigherScore()
        {
            float score = alternativeProduct.CalculateRecommendationScore(testProduct);
            
            // Le produit devrait avoir un score positif car même catégorie
            Assert.Greater(score, 0);
        }

        [Test]
        public void ProductData_CalculateRecommendationScore_BioBonus()
        {
            ProductData nonBioProduct = new ProductData
            {
                id = "TEST003",
                name = "Non Bio",
                category = "Test Category",
                price = 2.50f,
                nutrition = new NutritionData { nutriscore = "B" },
                ecoScore = "B",
                isBio = false
            };

            float bioScore = testProduct.CalculateRecommendationScore(nonBioProduct);
            float nonBioScore = nonBioProduct.CalculateRecommendationScore(testProduct);

            // Le produit bio devrait avoir un meilleur score
            Assert.Greater(bioScore, nonBioScore);
        }

        [Test]
        public void ProductFilter_OnlyBio_FiltersCorrectly()
        {
            List<ProductData> products = new List<ProductData>
            {
                testProduct,  // Bio
                alternativeProduct,  // Bio
                new ProductData { id = "TEST003", isBio = false }  // Non bio
            };

            ProductFilter filter = new ProductFilter { onlyBio = true };
            List<ProductData> filtered = products.FindAll(p => 
                (!filter.onlyBio || p.isBio)
            );

            Assert.AreEqual(2, filtered.Count);
            Assert.IsTrue(filtered.TrueForAll(p => p.isBio));
        }

        [Test]
        public void ProductFilter_MaxPrice_FiltersCorrectly()
        {
            List<ProductData> products = new List<ProductData>
            {
                new ProductData { id = "P1", price = 1.50f },
                new ProductData { id = "P2", price = 2.50f },
                new ProductData { id = "P3", price = 5.00f }
            };

            ProductFilter filter = new ProductFilter { maxPrice = 3.00f };
            List<ProductData> filtered = products.FindAll(p => 
                (filter.maxPrice <= 0 || p.price <= filter.maxPrice)
            );

            Assert.AreEqual(2, filtered.Count);
            Assert.IsTrue(filtered.TrueForAll(p => p.price <= 3.00f));
        }

        [Test]
        public void UserPreferences_AllergyRestrictions_WorksCorrectly()
        {
            UserPreferences prefs = new UserPreferences
            {
                allergyRestrictions = new List<string> { "Lactose" }
            };

            // Le produit test contient du lactose, devrait être filtré
            bool hasAllergen = false;
            foreach (var allergen in testProduct.allergens)
            {
                if (prefs.allergyRestrictions.Contains(allergen))
                {
                    hasAllergen = true;
                    break;
                }
            }

            Assert.IsTrue(hasAllergen);

            // Le produit alternatif n'a pas d'allergènes
            hasAllergen = false;
            foreach (var allergen in alternativeProduct.allergens)
            {
                if (prefs.allergyRestrictions.Contains(allergen))
                {
                    hasAllergen = true;
                    break;
                }
            }

            Assert.IsFalse(hasAllergen);
        }

        [Test]
        public void QRCodeData_ParseProductId_Valid()
        {
            string qrData1 = "PRODUCT:PROD001";
            string qrData2 = "PROD002";

            string id1 = SmartRetailAR.QRCode.QRCodeData.ParseProductId(qrData1);
            string id2 = SmartRetailAR.QRCode.QRCodeData.ParseProductId(qrData2);

            Assert.AreEqual("PROD001", id1);
            Assert.AreEqual("PROD002", id2);
        }

        [Test]
        public void QRCodeData_ParseProductId_Invalid()
        {
            string invalidData = "INVALID_DATA";
            string result = SmartRetailAR.QRCode.QRCodeData.ParseProductId(invalidData);

            Assert.IsNull(result);
        }

        [Test]
        public void QRCodeData_GenerateQRData_Valid()
        {
            string productId = "PROD001";
            string qrData = SmartRetailAR.QRCode.QRCodeData.GenerateQRData(productId);

            Assert.AreEqual("PRODUCT:PROD001", qrData);
        }

        [TearDown]
        public void Teardown()
        {
            testProduct = null;
            alternativeProduct = null;
        }
    }
}
