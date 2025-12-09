using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Data;
using SmartRetailAR.UI;

namespace SmartRetailAR.QRCode
{
    /// <summary>
    /// QR Code Scanner component that uses the device camera to scan QR codes.
    /// Sprint 1: Basic QR scanning functionality.
    /// </summary>
    public class QRCodeScanner : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private RawImage cameraPreview;
        [SerializeField] private int requestedWidth = 1280;
        [SerializeField] private int requestedHeight = 720;
        [SerializeField] private int requestedFPS = 30;
        
        [Header("Scan Settings")]
        [SerializeField] private float scanInterval = 0.3f;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private RectTransform scanArea;
        
        [Header("UI Feedback")]
        [SerializeField] private Image scanFrameImage;
        [SerializeField] private Color idleColor = Color.white;
        [SerializeField] private Color scanningColor = Color.green;
        [SerializeField] private Color errorColor = Color.red;
        
        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip scanSuccessSound;
        [SerializeField] private AudioClip scanErrorSound;
        
        private WebCamTexture webCamTexture;
        private bool isScanning;
        private bool isCameraRunning;
        private string lastScannedCode;
        private float lastScanTime;
        
        // QR Code decoder - In production, you would use a library like ZXing
        // For this implementation, we'll simulate QR decoding
        
        public event Action<string> OnQRCodeScanned;
        public event Action<Product> OnProductScanned;
        public event Action<string> OnScanError;
        public event Action OnCameraStarted;
        public event Action OnCameraStopped;
        
        public bool IsScanning => isScanning;
        public bool IsCameraRunning => isCameraRunning;
        
        private void Start()
        {
            if (autoStart)
            {
                StartScanning();
            }
        }
        
        private void OnDestroy()
        {
            StopScanning();
        }
        
        /// <summary>
        /// Starts the camera and begins scanning for QR codes.
        /// </summary>
        public void StartScanning()
        {
            if (isCameraRunning) return;
            StartCoroutine(InitializeCamera());
        }
        
        /// <summary>
        /// Stops the camera and scanning process.
        /// </summary>
        public void StopScanning()
        {
            isScanning = false;
            
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                webCamTexture.Stop();
            }
            
            isCameraRunning = false;
            OnCameraStopped?.Invoke();
            
            if (scanFrameImage != null)
            {
                scanFrameImage.color = idleColor;
            }
        }
        
        /// <summary>
        /// Initializes the device camera.
        /// </summary>
        private IEnumerator InitializeCamera()
        {
            // Request camera permission
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                Debug.LogError("Camera permission denied");
                OnScanError?.Invoke("Camera permission denied. Please enable camera access.");
                yield break;
            }
            
            // Get available cameras
            WebCamDevice[] devices = WebCamTexture.devices;
            
            if (devices.Length == 0)
            {
                Debug.LogError("No camera found");
                OnScanError?.Invoke("No camera found on this device.");
                yield break;
            }
            
            // Find back-facing camera (preferred for scanning)
            string cameraName = devices[0].name;
            foreach (var device in devices)
            {
                if (!device.isFrontFacing)
                {
                    cameraName = device.name;
                    break;
                }
            }
            
            // Create and start webcam texture
            webCamTexture = new WebCamTexture(cameraName, requestedWidth, requestedHeight, requestedFPS);
            
            if (cameraPreview != null)
            {
                cameraPreview.texture = webCamTexture;
            }
            
            webCamTexture.Play();
            
            // Wait for camera to initialize
            float timeout = 5f;
            while (!webCamTexture.didUpdateThisFrame && timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }
            
            if (!webCamTexture.isPlaying)
            {
                Debug.LogError("Failed to start camera");
                OnScanError?.Invoke("Failed to start camera.");
                yield break;
            }
            
            // Adjust preview for camera orientation
            AdjustCameraPreview();
            
            isCameraRunning = true;
            isScanning = true;
            OnCameraStarted?.Invoke();
            
            // Start scanning coroutine
            StartCoroutine(ScanForQRCodes());
            
            Debug.Log($"Camera started: {cameraName} ({webCamTexture.width}x{webCamTexture.height})");
        }
        
        /// <summary>
        /// Adjusts the camera preview for proper orientation.
        /// </summary>
        private void AdjustCameraPreview()
        {
            if (cameraPreview == null || webCamTexture == null) return;
            
            // Get the rotation angle
            float angle = -webCamTexture.videoRotationAngle;
            cameraPreview.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
            
            // Adjust aspect ratio
            float ratio = (float)webCamTexture.width / webCamTexture.height;
            
            // Handle mirroring for front camera
            if (webCamTexture.videoVerticallyMirrored)
            {
                cameraPreview.rectTransform.localScale = new Vector3(1, -1, 1);
            }
        }
        
        /// <summary>
        /// Main scanning coroutine that periodically checks for QR codes.
        /// </summary>
        private IEnumerator ScanForQRCodes()
        {
            while (isScanning)
            {
                yield return new WaitForSeconds(scanInterval);
                
                if (!webCamTexture.didUpdateThisFrame) continue;
                
                // Update scan frame color to indicate scanning
                if (scanFrameImage != null)
                {
                    scanFrameImage.color = scanningColor;
                }
                
                // Attempt to decode QR code from camera frame
                string qrContent = TryDecodeQRCode();
                
                if (!string.IsNullOrEmpty(qrContent))
                {
                    ProcessScannedCode(qrContent);
                }
            }
        }
        
        /// <summary>
        /// Attempts to decode a QR code from the current camera frame.
        /// In a production environment, this would use a library like ZXing.
        /// For demonstration, this simulates QR code detection.
        /// </summary>
        private string TryDecodeQRCode()
        {
            // In production, you would use a QR decoding library here
            // For example with ZXing.Net:
            // var barcodeReader = new BarcodeReader();
            // var result = barcodeReader.Decode(webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height);
            // return result?.Text;
            
            // For demonstration purposes, we'll return null
            // The SimulateQRScan method can be used for testing
            return null;
        }
        
        /// <summary>
        /// Processes a scanned QR code content.
        /// </summary>
        private void ProcessScannedCode(string content)
        {
            // Prevent duplicate scans of the same code
            if (content == lastScannedCode && Time.time - lastScanTime < 2f)
            {
                return;
            }
            
            lastScannedCode = content;
            lastScanTime = Time.time;
            
            // Provide haptic feedback (if available)
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
            
            // Notify listeners
            OnQRCodeScanned?.Invoke(content);
            
            // Try to find the product
            Product product = ProductManager.Instance.GetProductByQRCode(content);
            
            if (product != null)
            {
                PlaySound(scanSuccessSound);
                UpdateScanFrameColor(scanningColor);
                OnProductScanned?.Invoke(product);
                Debug.Log($"Product scanned: {product.name}");
            }
            else
            {
                PlaySound(scanErrorSound);
                UpdateScanFrameColor(errorColor);
                OnScanError?.Invoke($"Product not found for QR code: {content}");
                Debug.LogWarning($"Product not found for QR code: {content}");
            }
        }
        
        /// <summary>
        /// Simulates scanning a QR code (for testing without camera).
        /// </summary>
        public void SimulateQRScan(string qrContent)
        {
            Debug.Log($"Simulating QR scan: {qrContent}");
            ProcessScannedCode(qrContent);
        }
        
        /// <summary>
        /// Plays an audio clip if available.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Updates the scan frame color for visual feedback.
        /// </summary>
        private void UpdateScanFrameColor(Color color)
        {
            if (scanFrameImage != null)
            {
                scanFrameImage.color = color;
            }
        }
        
        /// <summary>
        /// Pauses scanning without stopping the camera.
        /// </summary>
        public void PauseScanning()
        {
            isScanning = false;
            if (scanFrameImage != null)
            {
                scanFrameImage.color = idleColor;
            }
        }
        
        /// <summary>
        /// Resumes scanning.
        /// </summary>
        public void ResumeScanning()
        {
            if (!isCameraRunning)
            {
                StartScanning();
                return;
            }
            
            isScanning = true;
            StartCoroutine(ScanForQRCodes());
        }
        
        /// <summary>
        /// Toggles the flashlight/torch on supported devices.
        /// </summary>
        public void ToggleFlashlight()
        {
            if (webCamTexture != null)
            {
                // Note: Unity doesn't have built-in flashlight support
                // In production, you would use native plugins for this
                Debug.Log("Flashlight toggle requested");
            }
        }
    }
}
