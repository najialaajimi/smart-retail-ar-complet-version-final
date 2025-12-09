using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// Événement déclenché lors de la détection d'un QR code
    /// </summary>
    [Serializable]
    public class QRCodeScannedEvent : UnityEvent<string> { }

    /// <summary>
    /// Scanner de QR codes utilisant la caméra du dispositif
    /// Supporte plusieurs formats: QR, EAN, UPC
    /// Note: Nécessite l'intégration de ZXing.Net pour fonctionner complètement
    /// </summary>
    public class QRCodeScanner : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Activer le scanner au démarrage")]
        [SerializeField] private bool autoStart = true;

        [Tooltip("Fréquence de scan en images par seconde")]
        [SerializeField] private float scanFrequency = 2f;

        [Tooltip("Résolution de la caméra")]
        [SerializeField] private Vector2Int cameraResolution = new Vector2Int(1920, 1080);

        [Header("Événements")]
        public QRCodeScannedEvent onQRCodeScanned = new QRCodeScannedEvent();
        public UnityEvent onScanStarted = new UnityEvent();
        public UnityEvent onScanStopped = new UnityEvent();
        public UnityEvent<string> onScanError = new UnityEvent<string>();

        private WebCamTexture webCamTexture;
        private bool isScanning = false;
        private float lastScanTime = 0f;
        private string lastScannedCode = "";

        private void Start()
        {
            if (autoStart)
            {
                StartScanning();
            }
        }

        /// <summary>
        /// Démarre le scanner de QR code
        /// </summary>
        public void StartScanning()
        {
            if (isScanning)
            {
                Debug.LogWarning("Le scanner est déjà actif");
                return;
            }

            // Vérifier si une caméra est disponible
            if (WebCamTexture.devices.Length == 0)
            {
                string error = "Aucune caméra détectée sur l'appareil";
                Debug.LogError(error);
                onScanError?.Invoke(error);
                return;
            }

            try
            {
                // Initialiser la caméra (préférence pour la caméra arrière)
                WebCamDevice[] devices = WebCamTexture.devices;
                string deviceName = devices[0].name;

                for (int i = 0; i < devices.Length; i++)
                {
                    if (!devices[i].isFrontFacing)
                    {
                        deviceName = devices[i].name;
                        break;
                    }
                }

                webCamTexture = new WebCamTexture(deviceName, cameraResolution.x, cameraResolution.y);
                webCamTexture.Play();

                isScanning = true;
                lastScanTime = Time.time;

                Debug.Log($"Scanner démarré avec la caméra: {deviceName}");
                onScanStarted?.Invoke();
            }
            catch (Exception e)
            {
                string error = $"Erreur lors du démarrage du scanner: {e.Message}";
                Debug.LogError(error);
                onScanError?.Invoke(error);
            }
        }

        /// <summary>
        /// Arrête le scanner de QR code
        /// </summary>
        public void StopScanning()
        {
            if (!isScanning)
            {
                return;
            }

            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
                Destroy(webCamTexture);
                webCamTexture = null;
            }

            isScanning = false;
            Debug.Log("Scanner arrêté");
            onScanStopped?.Invoke();
        }

        private void Update()
        {
            if (!isScanning || webCamTexture == null || !webCamTexture.isPlaying)
            {
                return;
            }

            // Limiter la fréquence de scan
            if (Time.time - lastScanTime < 1f / scanFrequency)
            {
                return;
            }

            lastScanTime = Time.time;

            // Simuler la détection de QR code
            // NOTE: Dans une implémentation réelle, utiliser ZXing.Net ici
            // TryDecodeQRCode(webCamTexture);
        }

        /// <summary>
        /// Méthode pour décoder le QR code (placeholder pour ZXing)
        /// </summary>
        private void TryDecodeQRCode(WebCamTexture texture)
        {
            // TODO: Implémenter avec ZXing.Net
            // var reader = new BarcodeReader();
            // var result = reader.Decode(texture.GetPixels32(), texture.width, texture.height);
            // if (result != null)
            // {
            //     OnQRCodeDetected(result.Text);
            // }
        }

        /// <summary>
        /// Appelé lorsqu'un QR code est détecté
        /// </summary>
        private void OnQRCodeDetected(string qrData)
        {
            if (string.IsNullOrEmpty(qrData) || qrData == lastScannedCode)
            {
                return;
            }

            lastScannedCode = qrData;
            Debug.Log($"QR Code détecté: {qrData}");
            onQRCodeScanned?.Invoke(qrData);
        }

        /// <summary>
        /// Simule la détection d'un QR code (pour les tests)
        /// </summary>
        public void SimulateScan(string qrData)
        {
            if (isScanning)
            {
                OnQRCodeDetected(qrData);
            }
            else
            {
                Debug.LogWarning("Le scanner n'est pas actif");
            }
        }

        /// <summary>
        /// Récupère la texture de la caméra
        /// </summary>
        public WebCamTexture GetCameraTexture()
        {
            return webCamTexture;
        }

        /// <summary>
        /// Vérifie si le scanner est actif
        /// </summary>
        public bool IsScanning()
        {
            return isScanning;
        }

        private void OnDestroy()
        {
            StopScanning();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                StopScanning();
            }
            else if (autoStart)
            {
                StartScanning();
            }
        }
    }
}
