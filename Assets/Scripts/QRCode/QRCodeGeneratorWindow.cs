#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SmartRetailAR.QRCode;
using SmartRetailAR.Products;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Fenêtre Editor pour générer des QR codes pour les produits
    /// Menu: Tools > Smart Retail AR > QR Code Generator
    /// </summary>
    public class QRCodeGeneratorWindow : EditorWindow
    {
        private string productId = "PROD001";
        private int qrCodeSize = 256;
        private string savePath = "Assets/Resources/QRCodes/";
        private Vector2 scrollPosition;
        
        private QRCodeGenerator generator;
        private Texture2D previewTexture;

        [MenuItem("Tools/Smart Retail AR/QR Code Generator")]
        public static void ShowWindow()
        {
            QRCodeGeneratorWindow window = GetWindow<QRCodeGeneratorWindow>("QR Code Generator");
            window.minSize = new Vector2(400, 600);
        }

        private void OnEnable()
        {
            if (generator == null)
            {
                GameObject go = new GameObject("TempQRGenerator");
                generator = go.AddComponent<QRCodeGenerator>();
            }
        }

        private void OnDisable()
        {
            if (generator != null)
            {
                DestroyImmediate(generator.gameObject);
            }
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Smart Retail AR - QR Code Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Générez des QR codes pour vos produits", MessageType.Info);

            GUILayout.Space(10);

            // Configuration
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            productId = EditorGUILayout.TextField("Product ID", productId);
            qrCodeSize = EditorGUILayout.IntSlider("Taille QR Code", qrCodeSize, 128, 1024);
            savePath = EditorGUILayout.TextField("Chemin de sauvegarde", savePath);

            GUILayout.Space(10);

            // Boutons de génération
            if (GUILayout.Button("Générer QR Code", GUILayout.Height(30)))
            {
                GenerateQRCode();
            }

            if (GUILayout.Button("Générer pour tous les produits", GUILayout.Height(30)))
            {
                GenerateAllQRCodes();
            }

            GUILayout.Space(10);

            // Aperçu
            if (previewTexture != null)
            {
                EditorGUILayout.LabelField("Aperçu", EditorStyles.boldLabel);
                GUILayout.Label(previewTexture, GUILayout.Width(256), GUILayout.Height(256));
            }

            GUILayout.Space(10);

            // Liste des produits
            EditorGUILayout.LabelField("Produits disponibles", EditorStyles.boldLabel);
            if (GUILayout.Button("Charger la liste des produits"))
            {
                LoadProductDatabase();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Génère un QR code pour le produit spécifié
        /// </summary>
        private void GenerateQRCode()
        {
            if (string.IsNullOrEmpty(productId))
            {
                EditorUtility.DisplayDialog("Erreur", "Veuillez spécifier un Product ID", "OK");
                return;
            }

            if (generator == null)
            {
                EditorUtility.DisplayDialog("Erreur", "Générateur non initialisé", "OK");
                return;
            }

            // Générer le QR code
            previewTexture = generator.GenerateProductQRCode(productId);

            if (previewTexture != null)
            {
                // Sauvegarder
                string fileName = $"QR_{productId}.png";
                string fullPath = savePath + fileName;

                // Créer le dossier si nécessaire
                if (!System.IO.Directory.Exists(savePath))
                {
                    System.IO.Directory.CreateDirectory(savePath);
                }

                bool success = generator.SaveQRCodeToFile(previewTexture, fullPath);

                if (success)
                {
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Succès", $"QR code généré et sauvegardé:\n{fullPath}", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Erreur", "Échec de la sauvegarde du QR code", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Erreur", "Échec de la génération du QR code", "OK");
            }
        }

        /// <summary>
        /// Génère des QR codes pour tous les produits
        /// </summary>
        private void GenerateAllQRCodes()
        {
            if (!EditorUtility.DisplayDialog("Confirmation", 
                "Générer des QR codes pour tous les produits de la base de données?", 
                "Oui", "Annuler"))
            {
                return;
            }

            // Charger la base de données
            var products = ProductDatabase.Instance.GetAllProducts();

            if (products == null || products.Count == 0)
            {
                EditorUtility.DisplayDialog("Erreur", 
                    "Aucun produit trouvé. Chargez d'abord la base de données.", 
                    "OK");
                return;
            }

            // Créer le dossier si nécessaire
            if (!System.IO.Directory.Exists(savePath))
            {
                System.IO.Directory.CreateDirectory(savePath);
            }

            int successCount = 0;

            for (int i = 0; i < products.Count; i++)
            {
                Product product = products[i];
                EditorUtility.DisplayProgressBar("Génération des QR codes", 
                    $"Génération pour {product.name}...", 
                    (float)i / products.Count);

                Texture2D qrTexture = generator.GenerateProductQRCode(product.id);
                
                if (qrTexture != null)
                {
                    string fileName = $"QR_{product.id}.png";
                    string fullPath = savePath + fileName;

                    if (generator.SaveQRCodeToFile(qrTexture, fullPath))
                    {
                        successCount++;
                    }
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Terminé", 
                $"{successCount}/{products.Count} QR codes générés avec succès", 
                "OK");
        }

        /// <summary>
        /// Charge la base de données de produits
        /// </summary>
        private void LoadProductDatabase()
        {
            bool success = ProductDatabase.Instance.LoadFromJson();
            
            if (success)
            {
                int count = ProductDatabase.Instance.GetAllProducts().Count;
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
    }
}
#endif
