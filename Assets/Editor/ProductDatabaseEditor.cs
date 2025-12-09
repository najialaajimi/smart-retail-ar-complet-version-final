using UnityEngine;
using UnityEditor;
using SmartRetailAR.Products;
using SmartRetailAR.Data;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Éditeur personnalisé pour la ProductDatabase
    /// Facilite la gestion des produits dans l'éditeur Unity
    /// </summary>
    [CustomEditor(typeof(ProductDatabase))]
    public class ProductDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ProductDatabase database = (ProductDatabase)target;

            DrawDefaultInspector();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            // Bouton pour charger depuis JSON
            if (GUILayout.Button("Load Products from JSON"))
            {
                database.LoadFromJson();
            }

            // Bouton pour exporter vers JSON
            if (GUILayout.Button("Export Products to JSON"))
            {
                database.ExportToJson();
            }

            GUILayout.Space(10);

            // Afficher les statistiques
            EditorGUILayout.LabelField("Statistics", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Products: {database.Products.Count}");

            if (database.Products.Count > 0)
            {
                int bioCount = database.Products.FindAll(p => p != null && p.isBio).Count;
                int veganCount = database.Products.FindAll(p => p != null && p.isVegan).Count;

                EditorGUILayout.LabelField($"Bio Products: {bioCount}");
                EditorGUILayout.LabelField($"Vegan Products: {veganCount}");
            }

            GUILayout.Space(10);

            // Bouton pour valider les données
            if (GUILayout.Button("Validate All Products"))
            {
                ValidateAllProducts(database);
            }

            // Bouton pour nettoyer
            if (GUILayout.Button("Clear All Products"))
            {
                if (EditorUtility.DisplayDialog("Confirm",
                    "Are you sure you want to clear all products?",
                    "Yes", "No"))
                {
                    database.Clear();
                    EditorUtility.SetDirty(database);
                }
            }
        }

        /// <summary>
        /// Valide tous les produits de la base de données
        /// </summary>
        private void ValidateAllProducts(ProductDatabase database)
        {
            int errorCount = 0;
            int warningCount = 0;

            foreach (var product in database.Products)
            {
                if (product == null)
                {
                    Debug.LogWarning("Null product found in database");
                    warningCount++;
                    continue;
                }

                // Valider les champs obligatoires
                if (string.IsNullOrEmpty(product.productId))
                {
                    Debug.LogError($"Product missing ID: {product.name}");
                    errorCount++;
                }

                if (string.IsNullOrEmpty(product.productName))
                {
                    Debug.LogError($"Product {product.productId} missing name");
                    errorCount++;
                }

                if (product.price <= 0)
                {
                    Debug.LogWarning($"Product {product.productId} has invalid price: {product.price}");
                    warningCount++;
                }

                if (string.IsNullOrEmpty(product.qrCodeId))
                {
                    Debug.LogWarning($"Product {product.productId} missing QR Code ID");
                    warningCount++;
                }
            }

            if (errorCount == 0 && warningCount == 0)
            {
                EditorUtility.DisplayDialog("Validation Complete",
                    "All products are valid!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Validation Complete",
                    $"Found {errorCount} errors and {warningCount} warnings.\nCheck console for details.",
                    "OK");
            }
        }
    }
}
