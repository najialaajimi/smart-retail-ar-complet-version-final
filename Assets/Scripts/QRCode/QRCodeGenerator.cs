using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Data;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// QR Code Generator for creating QR codes for products.
    /// Generates visual QR codes that can be used for testing.
    /// </summary>
    public class QRCodeGenerator : MonoBehaviour
    {
        [Header("QR Code Settings")]
        [SerializeField] private int qrCodeSize = 256;
        [SerializeField] private Color foregroundColor = Color.black;
        [SerializeField] private Color backgroundColor = Color.white;
        [SerializeField] private int quietZone = 4;
        
        [Header("Output")]
        [SerializeField] private RawImage previewImage;
        
        private static QRCodeGenerator _instance;
        public static QRCodeGenerator Instance => _instance;
        
        private void Awake()
        {
            _instance = this;
        }
        
        /// <summary>
        /// Generates a QR code texture for the given content.
        /// Uses a simple encoding algorithm for demonstration.
        /// In production, you would use a library like ZXing.
        /// </summary>
        public Texture2D GenerateQRCode(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogError("Cannot generate QR code for empty content");
                return null;
            }
            
            // Generate QR code matrix
            bool[,] matrix = GenerateQRMatrix(content);
            int size = matrix.GetLength(0);
            
            // Create texture
            int textureSize = qrCodeSize;
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            
            // Calculate module size
            int moduleSize = textureSize / (size + quietZone * 2);
            int offset = (textureSize - moduleSize * size) / 2;
            
            // Fill background
            Color[] colors = new Color[textureSize * textureSize];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = backgroundColor;
            }
            texture.SetPixels(colors);
            
            // Draw QR modules
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (matrix[x, y])
                    {
                        DrawModule(texture, offset + x * moduleSize, offset + y * moduleSize, moduleSize, foregroundColor);
                    }
                }
            }
            
            texture.Apply();
            
            return texture;
        }
        
        /// <summary>
        /// Generates a simple QR-like matrix for the content.
        /// This is a simplified version for demonstration purposes.
        /// </summary>
        private bool[,] GenerateQRMatrix(string content)
        {
            // Determine size based on content length
            int version = Mathf.Max(1, Mathf.CeilToInt(content.Length / 10f));
            int size = 21 + (version - 1) * 4;
            
            bool[,] matrix = new bool[size, size];
            
            // Add finder patterns (top-left, top-right, bottom-left)
            AddFinderPattern(matrix, 0, 0);
            AddFinderPattern(matrix, size - 7, 0);
            AddFinderPattern(matrix, 0, size - 7);
            
            // Add timing patterns
            for (int i = 8; i < size - 8; i++)
            {
                matrix[i, 6] = i % 2 == 0;
                matrix[6, i] = i % 2 == 0;
            }
            
            // Add alignment pattern for larger versions
            if (version >= 2)
            {
                int alignPos = size - 9;
                AddAlignmentPattern(matrix, alignPos, alignPos);
            }
            
            // Encode data (simplified)
            EncodeData(matrix, content);
            
            return matrix;
        }
        
        /// <summary>
        /// Adds a finder pattern to the matrix.
        /// </summary>
        private void AddFinderPattern(bool[,] matrix, int x, int y)
        {
            int size = matrix.GetLength(0);
            
            for (int dy = 0; dy < 7; dy++)
            {
                for (int dx = 0; dx < 7; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    
                    if (px >= 0 && px < size && py >= 0 && py < size)
                    {
                        // Outer border
                        if (dx == 0 || dx == 6 || dy == 0 || dy == 6)
                        {
                            matrix[px, py] = true;
                        }
                        // Inner square
                        else if (dx >= 2 && dx <= 4 && dy >= 2 && dy <= 4)
                        {
                            matrix[px, py] = true;
                        }
                    }
                }
            }
            
            // Add separator (white space around finder pattern)
            // This is handled by leaving surrounding cells as false
        }
        
        /// <summary>
        /// Adds an alignment pattern to the matrix.
        /// </summary>
        private void AddAlignmentPattern(bool[,] matrix, int x, int y)
        {
            int size = matrix.GetLength(0);
            
            for (int dy = -2; dy <= 2; dy++)
            {
                for (int dx = -2; dx <= 2; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    
                    if (px >= 0 && px < size && py >= 0 && py < size)
                    {
                        // Outer border or center
                        if (dx == -2 || dx == 2 || dy == -2 || dy == 2 || (dx == 0 && dy == 0))
                        {
                            matrix[px, py] = true;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Encodes data into the QR matrix.
        /// </summary>
        private void EncodeData(bool[,] matrix, string content)
        {
            int size = matrix.GetLength(0);
            int hash = content.GetHashCode();
            
            // Simple data encoding for demonstration
            // Fill remaining space with pattern based on content hash
            System.Random random = new System.Random(hash);
            
            for (int y = 8; y < size - 8; y++)
            {
                for (int x = 8; x < size - 8; x++)
                {
                    // Skip timing pattern columns/rows
                    if (x == 6 || y == 6) continue;
                    
                    // Skip alignment pattern area
                    if (x >= size - 11 && x <= size - 7 && y >= size - 11 && y <= size - 7) continue;
                    
                    // Encode based on content
                    int charIndex = ((x - 8) + (y - 8) * (size - 16)) % content.Length;
                    if (charIndex < content.Length)
                    {
                        matrix[x, y] = (content[charIndex] + x + y) % 2 == 0 || random.NextDouble() < 0.3;
                    }
                }
            }
        }
        
        /// <summary>
        /// Draws a module (single square) on the texture.
        /// </summary>
        private void DrawModule(Texture2D texture, int x, int y, int size, Color color)
        {
            for (int dy = 0; dy < size; dy++)
            {
                for (int dx = 0; dx < size; dx++)
                {
                    int px = x + dx;
                    int py = y + dy;
                    
                    if (px >= 0 && px < texture.width && py >= 0 && py < texture.height)
                    {
                        texture.SetPixel(px, py, color);
                    }
                }
            }
        }
        
        /// <summary>
        /// Generates a QR code for a product and displays it in the preview.
        /// </summary>
        public Texture2D GenerateProductQRCode(Product product)
        {
            if (product == null)
            {
                Debug.LogError("Cannot generate QR code for null product");
                return null;
            }
            
            Texture2D qrTexture = GenerateQRCode(product.qrCodeId);
            
            if (previewImage != null && qrTexture != null)
            {
                previewImage.texture = qrTexture;
            }
            
            return qrTexture;
        }
        
        /// <summary>
        /// Generates QR codes for all products in the database.
        /// </summary>
        public Dictionary<string, Texture2D> GenerateAllProductQRCodes()
        {
            Dictionary<string, Texture2D> qrCodes = new Dictionary<string, Texture2D>();
            
            var products = ProductManager.Instance.GetAllProducts();
            
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.qrCodeId))
                {
                    Texture2D qrTexture = GenerateQRCode(product.qrCodeId);
                    if (qrTexture != null)
                    {
                        qrCodes[product.id] = qrTexture;
                    }
                }
            }
            
            Debug.Log($"Generated {qrCodes.Count} QR codes for products");
            return qrCodes;
        }
        
        /// <summary>
        /// Saves a QR code texture to a PNG file.
        /// </summary>
        public void SaveQRCodeToFile(Texture2D qrTexture, string filePath)
        {
            if (qrTexture == null)
            {
                Debug.LogError("Cannot save null texture");
                return;
            }
            
            byte[] pngData = qrTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(filePath, pngData);
            
            Debug.Log($"QR code saved to: {filePath}");
        }
    }
}
