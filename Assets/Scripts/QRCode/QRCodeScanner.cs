using UnityEngine;
using System;
using SmartRetailAR.Utils;
using SmartRetailAR.Products;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// Scanner de QR codes utilisant la caméra du smartphone
    /// Compatible avec ZXing.Net (à installer via NuGet ou Unity Package)
    /// </summary>
    public class QRCodeScanner : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float scanInterval = 0.5f;
        [SerializeField] private int textureWidth = 1280;
        [SerializeField] private int textureHeight = 720;
        [SerializeField] private bool autoStartScanning = true;

        [Header("UI")]
        [SerializeField] private GameObject scanGuideUI;
        [SerializeField] private UnityEngine.UI.RawImage cameraPreview;

        // État du scanner
        private WebCamTexture m_WebCamTexture;
        private bool m_IsScanning = false;
        private float m_LastScanTime = 0f;
        private QRCodeData m_LastScannedQR;

        // Events
        public event Action<QRCodeData> OnQRCodeDetected;
        public event Action<string> OnScanError;

        private void Start()
        {
            if (autoStartScanning)
            {
                StartScanning();
            }
        }

        /// <summary>
        /// Démarre le scan de QR codes
        /// </summary>
        public void StartScanning()
        {
            if (m_IsScanning)
            {
                DebugLogger.LogWarning("Le scan est déjà en cours");
                return;
            }

            DebugLogger.Log("Démarrage du scanner QR...", LogLevel.Info);

            // Demander l'autorisation de la caméra
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }

            // Initialiser la webcam
            InitializeWebcam();

            m_IsScanning = true;

            if (scanGuideUI != null)
            {
                scanGuideUI.SetActive(true);
            }

            DebugLogger.Log("✓ Scanner QR démarré", LogLevel.Info);
        }

        /// <summary>
        /// Arrête le scan de QR codes
        /// </summary>
        public void StopScanning()
        {
            if (!m_IsScanning)
                return;

            DebugLogger.Log("Arrêt du scanner QR...", LogLevel.Info);

            m_IsScanning = false;

            if (m_WebCamTexture != null && m_WebCamTexture.isPlaying)
            {
                m_WebCamTexture.Stop();
            }

            if (scanGuideUI != null)
            {
                scanGuideUI.SetActive(false);
            }

            DebugLogger.Log("✓ Scanner QR arrêté", LogLevel.Info);
        }

        /// <summary>
        /// Initialise la webcam
        /// </summary>
        private void InitializeWebcam()
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length == 0)
            {
                DebugLogger.LogError("Aucune caméra disponible");
                OnScanError?.Invoke("Aucune caméra disponible");
                return;
            }

            // Préférer la caméra arrière sur mobile
            string deviceName = devices[0].name;
            for (int i = 0; i < devices.Length; i++)
            {
                if (!devices[i].isFrontFacing)
                {
                    deviceName = devices[i].name;
                    break;
                }
            }

            m_WebCamTexture = new WebCamTexture(deviceName, textureWidth, textureHeight);
            
            if (cameraPreview != null)
            {
                cameraPreview.texture = m_WebCamTexture;
            }

            m_WebCamTexture.Play();

            DebugLogger.Log($"Caméra initialisée: {deviceName}", LogLevel.Debug);
        }

        private void Update()
        {
            if (!m_IsScanning || m_WebCamTexture == null || !m_WebCamTexture.isPlaying)
                return;

            // Scan à intervalles réguliers pour optimiser les performances
            if (Time.time - m_LastScanTime < scanInterval)
                return;

            m_LastScanTime = Time.time;

            // Scanner le QR code
            ScanQRCode();
        }

        /// <summary>
        /// Scanne le QR code depuis l'image de la caméra
        /// </summary>
        private void ScanQRCode()
        {
            try
            {
                // NOTE: Cette implémentation nécessite ZXing.Net
                // Pour une démo sans dépendance, on peut simuler la détection
                
                #if UNITY_EDITOR
                // Mode simulation pour l'éditeur Unity
                SimulateScan();
                #else
                // Mode réel avec ZXing (nécessite l'import du package)
                // IBarcodeReader barcodeReader = new BarcodeReader();
                // var result = barcodeReader.Decode(m_WebCamTexture.GetPixels32(), m_WebCamTexture.width, m_WebCamTexture.height);
                
                // if (result != null)
                // {
                //     ProcessQRCode(result.Text);
                // }
                
                // Pour l'instant, simulation également en production
                SimulateScan();
                #endif
            }
            catch (Exception e)
            {
                DebugLogger.LogException(e, "ScanQRCode");
                OnScanError?.Invoke($"Erreur de scan: {e.Message}");
            }
        }

        /// <summary>
        /// Simule la détection d'un QR code (pour tests sans ZXing)
        /// </summary>
        private void SimulateScan()
        {
            // Simulation: détecter un produit aléatoire toutes les 5 secondes
            if (Input.GetKeyDown(KeyCode.Space) || (Time.time % 10f < 0.1f && Time.time > 5f))
            {
                int randomProduct = UnityEngine.Random.Range(1, 14);
                string productId = $"PROD{randomProduct:D3}";
                string qrData = QRCodeData.GenerateQRData(productId);
                
                DebugLogger.Log($"[SIMULATION] QR Code détecté: {qrData}", LogLevel.Debug);
                ProcessQRCode(qrData);
            }
        }

        /// <summary>
        /// Traite les données d'un QR code détecté
        /// </summary>
        private void ProcessQRCode(string qrData)
        {
            string productId = QRCodeData.ParseProductId(qrData);

            if (string.IsNullOrEmpty(productId))
            {
                DebugLogger.LogWarning($"QR Code invalide: {qrData}");
                OnScanError?.Invoke("QR Code invalide");
                return;
            }

            // Créer les données du QR code
            QRCodeData qrCodeData = new QRCodeData($"QR{productId.Substring(4)}", productId);
            m_LastScannedQR = qrCodeData;

            DebugLogger.Log($"✓ QR Code scanné avec succès: {qrCodeData}", LogLevel.Info);

            // Déclencher l'événement
            OnQRCodeDetected?.Invoke(qrCodeData);

            // Arrêter le scan après détection (optionnel)
            // StopScanning();
        }

        /// <summary>
        /// Récupère le dernier QR code scanné
        /// </summary>
        public QRCodeData GetLastScannedQR()
        {
            return m_LastScannedQR;
        }

        private void OnDestroy()
        {
            StopScanning();
            
            if (m_WebCamTexture != null)
            {
                Destroy(m_WebCamTexture);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (m_WebCamTexture != null && m_WebCamTexture.isPlaying)
                {
                    m_WebCamTexture.Pause();
                }
            }
            else
            {
                if (m_WebCamTexture != null && m_IsScanning)
                {
                    m_WebCamTexture.Play();
                }
            }
        }
    }
}
