using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SmartRetailAR.AR;
using SmartRetailAR.Products;

namespace SmartRetailAR.Tests.PlayMode
{
    /// <summary>
    /// Tests PlayMode pour l'overlay AR
    /// </summary>
    public class AROverlayTests
    {
        private GameObject overlayObject;
        private ARProductOverlay overlay;

        [SetUp]
        public void Setup()
        {
            overlayObject = new GameObject("TestAROverlay");
            overlay = overlayObject.AddComponent<ARProductOverlay>();
        }

        [TearDown]
        public void Teardown()
        {
            if (overlayObject != null)
            {
                Object.Destroy(overlayObject);
            }
        }

        [UnityTest]
        public IEnumerator ShowProduct_ValidProduct_DisplaysOverlay()
        {
            // Arrange
            Product testProduct = CreateTestProduct();

            // Act
            overlay.ShowProduct(testProduct);
            yield return null;

            // Assert
            Assert.IsTrue(overlay.IsVisible());
            Assert.AreEqual(testProduct, overlay.GetCurrentProduct());
        }

        [UnityTest]
        public IEnumerator Hide_HidesOverlay()
        {
            // Arrange
            Product testProduct = CreateTestProduct();
            overlay.ShowProduct(testProduct);
            yield return null;

            // Act
            overlay.Hide();
            yield return null;

            // Assert
            Assert.IsFalse(overlay.IsVisible());
        }

        [UnityTest]
        public IEnumerator SetPosition_UpdatesPosition()
        {
            // Arrange
            Vector3 testPosition = new Vector3(1, 2, 3);

            // Act
            overlay.SetPosition(testPosition);
            yield return null;

            // Assert
            // La position devrait être mise à jour avec l'offset
            Assert.IsNotNull(overlay.transform.position);
        }

        private Product CreateTestProduct()
        {
            return new Product
            {
                id = "TEST001",
                name = "Test Product",
                brand = "Test Brand",
                price = 9.99f,
                origin = "France",
                category = "Test",
                nutrition = new NutritionInfo
                {
                    calories = 100,
                    proteins = 5,
                    carbohydrates = 10,
                    fats = 2,
                    fiber = 1,
                    salt = 0.5f,
                    nutriScore = "A"
                },
                ecoScore = 85,
                isBio = true,
                isEcoResponsible = true
            };
        }
    }
}
