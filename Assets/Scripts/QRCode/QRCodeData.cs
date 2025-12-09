using System;
using UnityEngine;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// Données associées à un QR Code
    /// </summary>
    [Serializable]
    public class QRCodeData
    {
        public string qrCodeId;
        public string productId;
        public string data;
        public DateTime scannedAt;

        public QRCodeData()
        {
            scannedAt = DateTime.Now;
        }

        public QRCodeData(string qrId, string prodId)
        {
            qrCodeId = qrId;
            productId = prodId;
            data = $"PRODUCT:{prodId}";
            scannedAt = DateTime.Now;
        }

        /// <summary>
        /// Parse les données d'un QR code pour extraire l'ID du produit
        /// </summary>
        public static string ParseProductId(string qrData)
        {
            if (string.IsNullOrEmpty(qrData))
                return null;

            // Format attendu: "PRODUCT:PROD001" ou simplement "PROD001"
            if (qrData.StartsWith("PRODUCT:"))
            {
                return qrData.Substring(8);
            }

            // Si c'est déjà un ID de produit
            if (qrData.StartsWith("PROD"))
            {
                return qrData;
            }

            return null;
        }

        /// <summary>
        /// Génère les données d'un QR code à partir d'un ID de produit
        /// </summary>
        public static string GenerateQRData(string productId)
        {
            return $"PRODUCT:{productId}";
        }

        public override string ToString()
        {
            return $"QRCode [{qrCodeId}] -> Product [{productId}] scanned at {scannedAt:HH:mm:ss}";
        }
    }
}
