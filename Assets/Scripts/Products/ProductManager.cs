using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartRetailAR.Products
{
    /// <summary>
    /// Événement déclenché lors de la sélection d'un produit
    /// </summary>
    [Serializable]
    public class ProductSelectedEvent : UnityEvent<Product> { }

    /// <summary>
    /// Gère l'état actuel du produit et les interactions
    /// </summary>
    public class ProductManager : MonoBehaviour
    {
        private static ProductManager instance;
        public static ProductManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ProductManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ProductManager");
                        instance = go.AddComponent<ProductManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        [Header("Événements")]
        public ProductSelectedEvent onProductSelected = new ProductSelectedEvent();
        public UnityEvent onProductDeselected = new UnityEvent();

        private Product currentProduct;
        private Product previousProduct;

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
            }
        }

        /// <summary>
        /// Sélectionne un produit et déclenche l'événement associé
        /// </summary>
        public void SelectProduct(Product product)
        {
            if (product == null)
            {
                Debug.LogWarning("Tentative de sélection d'un produit null");
                return;
            }

            previousProduct = currentProduct;
            currentProduct = product;

            Debug.Log($"Produit sélectionné: {product.name}");
            onProductSelected?.Invoke(product);
        }

        /// <summary>
        /// Sélectionne un produit par son ID
        /// </summary>
        public void SelectProductById(string productId)
        {
            Product product = ProductDatabase.Instance.GetProductById(productId);
            if (product != null)
            {
                SelectProduct(product);
            }
            else
            {
                Debug.LogWarning($"Produit avec l'ID {productId} introuvable");
            }
        }

        /// <summary>
        /// Désélectionne le produit actuel
        /// </summary>
        public void DeselectProduct()
        {
            if (currentProduct != null)
            {
                Debug.Log($"Désélection du produit: {currentProduct.name}");
                previousProduct = currentProduct;
                currentProduct = null;
                onProductDeselected?.Invoke();
            }
        }

        /// <summary>
        /// Récupère le produit actuellement sélectionné
        /// </summary>
        public Product GetCurrentProduct()
        {
            return currentProduct;
        }

        /// <summary>
        /// Récupère le produit précédemment sélectionné
        /// </summary>
        public Product GetPreviousProduct()
        {
            return previousProduct;
        }

        /// <summary>
        /// Vérifie si un produit est actuellement sélectionné
        /// </summary>
        public bool HasSelectedProduct()
        {
            return currentProduct != null;
        }

        /// <summary>
        /// Compare le produit actuel avec un autre produit
        /// </summary>
        public void CompareWithCurrentProduct(Product otherProduct)
        {
            if (currentProduct == null)
            {
                Debug.LogWarning("Aucun produit sélectionné pour la comparaison");
                return;
            }

            Debug.Log($"Comparaison: {currentProduct.name} vs {otherProduct.name}");
            Debug.Log($"Prix: {currentProduct.price}€ vs {otherProduct.price}€");
            Debug.Log($"Eco-score: {currentProduct.ecoScore} vs {otherProduct.ecoScore}");
            Debug.Log($"Nutri-score: {currentProduct.nutrition.nutriScore} vs {otherProduct.nutrition.nutriScore}");
        }
    }
}
