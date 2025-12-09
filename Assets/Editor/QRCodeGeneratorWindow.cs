using UnityEngine;
using UnityEditor;
using SmartRetailAR.QRCode;
using SmartRetailAR.Utils;
using SmartRetailAR.Data;
using System.IO;

namespace SmartRetailAR.Editor
{
    /// <summary>
    /// Fenêtre d'éditeur Unity pour générer des QR codes pour les produits
    /// Menu: Tools > Smart Retail AR > QR Code Generator
    /// </summary>
    public class QRCodeGeneratorWindow : EditorWindow
    {
        private string productId = "PROD001";
        private int qrCodeSize = 256;
        private string outputFolder = "Assets/Data/QRCodes";
        private bool generateForAllProducts = false;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Smart Retail AR/QR Code Generator")]
        public static void ShowWindow()
        {
            QRCodeGeneratorWindow window = GetWindow<QRCodeGeneratorWindow>("QR Code Generator");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Générateur de QR Codes", EditorStyles.boldLabel);
            GUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Configuration
            EditorGUILayout.LabelField("Configuration", EditorStyles.boldLabel);
            qrCodeSize = EditorGUILayout.IntSlider("Taille du QR Code", qrCodeSize, 128, 1024);
            outputFolder = EditorGUILayout.TextField("Dossier de sortie", outputFolder);

            GUILayout.Space(10);

            // Génération pour un produit unique
            EditorGUILayout.LabelField("Générer pour un produit", EditorStyles.boldLabel);
            productId = EditorGUILayout.TextField("ID du produit", productId);

            if (GUILayout.Button("Générer QR Code"))
            {
                GenerateSingleQRCode();
            }

            GUILayout.Space(10);

            // Génération pour tous les produits
            EditorGUILayout.LabelField("Génération en masse", EditorStyles.boldLabel);
            generateForAllProducts = EditorGUILayout.Toggle("Générer pour tous les produits", generateForAllProducts);

            if (GUILayout.Button("Générer tous les QR Codes"))
            {
                GenerateAllQRCodes();
            }

            GUILayout.Space(10);

            // Informations
            EditorGUILayout.HelpBox(
                "Ce générateur crée des QR codes pour les produits de l'application Smart Retail AR.\n\n" +
                "- Format: PNG\n" +
                "- Contenu: PRODUCT:{ID_PRODUIT}\n" +
                "- Note: Nécessite ZXing.Net pour une génération réelle",
                MessageType.Info
            );

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Génère un QR code pour un seul produit
        /// </summary>
        private void GenerateSingleQRCode()
        {
            if (string.IsNullOrEmpty(productId))
            {
                EditorUtility.DisplayDialog("Erreur", "Veuillez spécifier un ID de produit", "OK");
                return;
            }

            // Créer un GameObject temporaire avec le générateur
            GameObject tempObj = new GameObject("TempQRGenerator");
            QRCodeGenerator generator = tempObj.AddComponent<QRCodeGenerator>();

            try
            {
                // Générer le QR code
                Texture2D qrCode = generator.GenerateQRCode(productId);

                if (qrCode != null)
                {
                    // Créer le dossier si nécessaire
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }

                    // Sauvegarder
                    string filename = $"QR_{productId}.png";
                    string filepath = Path.Combine(outputFolder, filename);

                    byte[] bytes = qrCode.EncodeToPNG();
                    File.WriteAllBytes(filepath, bytes);

                    // Rafraîchir l'Asset Database
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("Succès", 
                        $"QR Code généré avec succès:\n{filepath}", "OK");

                    Debug.Log($"✓ QR Code généré: {filepath}");
                }
                else
                {
                    EditorUtility.DisplayDialog("Erreur", 
                        "Impossible de générer le QR code", "OK");
                }
            }
            finally
            {
                // Nettoyer
                DestroyImmediate(tempObj);
            }
        }

        /// <summary>
        /// Génère des QR codes pour tous les produits du fichier JSON
        /// </summary>
        private void GenerateAllQRCodes()
        {
            // Charger les produits
            ProductDataList productList = JsonDataLoader.LoadProducts("Data/Products/products");

            if (productList == null || productList.products == null || productList.products.Count == 0)
            {
                EditorUtility.DisplayDialog("Erreur", 
                    "Impossible de charger les produits depuis le fichier JSON", "OK");
                return;
            }

            // Créer un GameObject temporaire avec le générateur
            GameObject tempObj = new GameObject("TempQRGenerator");
            QRCodeGenerator generator = tempObj.AddComponent<QRCodeGenerator>();

            try
            {
                // Créer le dossier si nécessaire
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                int successCount = 0;
                int totalCount = productList.products.Count;

                // Générer pour chaque produit
                for (int i = 0; i < totalCount; i++)
                {
                    ProductData product = productList.products[i];

                    // Afficher la progression
                    EditorUtility.DisplayProgressBar("Génération des QR Codes",
                        $"Génération pour {product.id} ({i + 1}/{totalCount})",
                        (float)i / totalCount);

                    try
                    {
                        // Générer le QR code
                        Texture2D qrCode = generator.GenerateQRCode(product.id);

                        if (qrCode != null)
                        {
                            // Sauvegarder
                            string filename = $"QR_{product.id}.png";
                            string filepath = Path.Combine(outputFolder, filename);

                            byte[] bytes = qrCode.EncodeToPNG();
                            File.WriteAllBytes(filepath, bytes);

                            successCount++;
                            Debug.Log($"✓ QR Code généré: {product.id}");
                        }
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Erreur lors de la génération du QR code pour {product.id}: {e.Message}");
                    }
                }

                // Rafraîchir l'Asset Database
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Génération terminée",
                    $"{successCount}/{totalCount} QR codes générés avec succès dans:\n{outputFolder}", "OK");
            }
            finally
            {
                // Nettoyer
                DestroyImmediate(tempObj);
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
