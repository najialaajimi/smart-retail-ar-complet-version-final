#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SmartRetailAR.Products;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Éditeur personnalisé pour ProductDatabase
    /// </summary>
    [CustomEditor(typeof(ProductDatabase))]
    public class ProductDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ProductDatabase database = (ProductDatabase)target;

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Charger la base de données", GUILayout.Height(30)))
            {
                bool success = database.LoadFromJson();
                if (success)
                {
                    int count = database.GetAllProducts().Count;
                    EditorUtility.DisplayDialog("Succès", 
                        $"Base de données chargée: {count} produits", 
                        "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Erreur", 
                        "Échec du chargement de la base de données", 
                        "OK");
                }
            }

            if (database.IsInitialized())
            {
                GUILayout.Space(5);
                var products = database.GetAllProducts();
                EditorGUILayout.LabelField($"Produits chargés: {products.Count}", EditorStyles.helpBox);

                if (products.Count > 0 && GUILayout.Button("Afficher les produits"))
                {
                    ShowProductsWindow(products);
                }
            }
        }

        /// <summary>
        /// Affiche une fenêtre avec la liste des produits
        /// </summary>
        private void ShowProductsWindow(System.Collections.Generic.List<Product> products)
        {
            string message = "Produits dans la base de données:\n\n";
            
            foreach (var product in products)
            {
                message += $"• {product.id}: {product.name} ({product.brand}) - {product.price}€\n";
            }

            EditorUtility.DisplayDialog("Produits", message, "OK");
        }
    }
}
#endif
