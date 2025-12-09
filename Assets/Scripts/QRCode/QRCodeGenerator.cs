using UnityEngine;
using System.IO;
using SmartRetailAR.Utils;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// Générateur de QR codes pour les produits
    /// Utilise une bibliothèque externe ou génère des images de test
    /// </summary>
    public class QRCodeGenerator : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int qrCodeSize = 256;
        [SerializeField] private string outputFolder = "Assets/Data/QRCodes";

        /// <summary>
        /// Génère un QR code pour un produit
        /// </summary>
        public Texture2D GenerateQRCode(string productId)
        {
            string qrData = QRCodeData.GenerateQRData(productId);
            return GenerateQRCodeFromData(qrData, qrCodeSize);
        }

        /// <summary>
        /// Génère un QR code à partir de données textuelles
        /// </summary>
        public Texture2D GenerateQRCodeFromData(string data, int size)
        {
            DebugLogger.Log($"Génération d'un QR code: {data} (taille: {size}x{size})", LogLevel.Info);

            // NOTE: Cette implémentation nécessite ZXing.Net ou une bibliothèque similaire
            // Pour la démo, on génère une texture de test
            
            #if UNITY_EDITOR
            // En mode éditeur, on peut générer une texture de test
            return GenerateTestQRCode(data, size);
            #else
            // En production, utiliser ZXing:
            // BarcodeWriter writer = new BarcodeWriter
            // {
            //     Format = BarcodeFormat.QR_CODE,
            //     Options = new QrCodeEncodingOptions
            //     {
            //         Width = size,
            //         Height = size,
            //         Margin = 1
            //     }
            // };
            // Color32[] pixels = writer.Write(data);
            // Texture2D texture = new Texture2D(size, size);
            // texture.SetPixels32(pixels);
            // texture.Apply();
            // return texture;
            
            // Pour l'instant, version de test également
            return GenerateTestQRCode(data, size);
            #endif
        }

        /// <summary>
        /// Génère une texture de test simulant un QR code
        /// </summary>
        private Texture2D GenerateTestQRCode(string data, int size)
        {
            Texture2D texture = new Texture2D(size, size);
            
            // Créer un pattern simple en noir et blanc
            Color[] pixels = new Color[size * size];
            
            // Fond blanc
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            // Bordure noire
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (x < 10 || x >= size - 10 || y < 10 || y >= size - 10)
                    {
                        pixels[y * size + x] = Color.black;
                    }
                }
            }

            // Pattern basé sur le hash des données
            int hash = data.GetHashCode();
            System.Random rnd = new System.Random(hash);
            
            for (int i = 0; i < 100; i++)
            {
                int x = rnd.Next(20, size - 20);
                int y = rnd.Next(20, size - 20);
                int boxSize = rnd.Next(5, 15);
                
                for (int dx = 0; dx < boxSize && x + dx < size - 20; dx++)
                {
                    for (int dy = 0; dy < boxSize && y + dy < size - 20; dy++)
                    {
                        pixels[(y + dy) * size + (x + dx)] = Color.black;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            DebugLogger.Log($"✓ QR code de test généré pour: {data}", LogLevel.Debug);
            return texture;
        }

        /// <summary>
        /// Sauvegarde un QR code en tant que fichier PNG
        /// </summary>
        public bool SaveQRCode(Texture2D qrCode, string filename)
        {
            try
            {
                // Créer le dossier si nécessaire
                if (!Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                // Encoder en PNG
                byte[] bytes = qrCode.EncodeToPNG();
                
                // Construire le chemin complet
                string filepath = Path.Combine(outputFolder, filename + ".png");
                
                // Sauvegarder le fichier
                File.WriteAllBytes(filepath, bytes);

                DebugLogger.Log($"✓ QR code sauvegardé: {filepath}", LogLevel.Info);
                return true;
            }
            catch (System.Exception e)
            {
                DebugLogger.LogException(e, "SaveQRCode");
                return false;
            }
        }

        /// <summary>
        /// Génère et sauvegarde un QR code pour un produit
        /// </summary>
        public bool GenerateAndSaveQRCode(string productId)
        {
            Texture2D qrCode = GenerateQRCode(productId);
            if (qrCode == null)
            {
                DebugLogger.LogError($"Impossible de générer le QR code pour {productId}");
                return false;
            }

            string filename = $"QR_{productId}";
            return SaveQRCode(qrCode, filename);
        }
    }
}
