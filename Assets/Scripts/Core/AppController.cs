using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using SmartRetailAR.Data;
using SmartRetailAR.AR;
using SmartRetailAR.QRCode;
using SmartRetailAR.Recommendations;
using SmartRetailAR.UI;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Main Application Controller for Smart Retail AR.
    /// Manages app lifecycle, initializes managers, and coordinates between components.
    /// </summary>
    public class AppController : MonoBehaviour
    {
        private static AppController _instance;
        public static AppController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AppController>();
                }
                return _instance;
            }
        }
        
        [Header("Configuration")]
        [SerializeField] private string appVersion = "1.0.0";
        [SerializeField] private bool enableDebugMode = false;
        [SerializeField] private bool autoInitialize = true;
        
        [Header("Target Performance")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private float maxLatencyMs = 1000f; // KPI: ≤ 1 second
        
        [Header("References")]
        [SerializeField] private ProductManager productManager;
        [SerializeField] private ARManager arManager;
        [SerializeField] private QRCodeScanner qrScanner;
        [SerializeField] private RecommendationEngine recommendationEngine;
        [SerializeField] private UIManager uiManager;
        
        // App state
        private bool isInitialized;
        private bool isARSupported;
        private float initializationStartTime;
        
        // Events
        public event Action OnAppInitialized;
        public event Action<string> OnAppError;
        public event Action<Product> OnProductScanned;
        
        public string AppVersion => appVersion;
        public bool IsInitialized => isInitialized;
        public bool IsARSupported => isARSupported;
        public bool DebugMode => enableDebugMode;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Set target frame rate
            Application.targetFrameRate = targetFrameRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        
        private void Start()
        {
            if (autoInitialize)
            {
                StartCoroutine(Initialize());
            }
        }
        
        /// <summary>
        /// Initializes all app components.
        /// </summary>
        public IEnumerator Initialize()
        {
            initializationStartTime = Time.realtimeSinceStartup;
            
            Debug.Log($"Smart Retail AR v{appVersion} - Initializing...");
            
            // Check AR support
            yield return CheckARSupport();
            
            // Initialize managers
            yield return InitializeManagers();
            
            // Setup event listeners
            SetupEventListeners();
            
            // Initialization complete
            float initTime = (Time.realtimeSinceStartup - initializationStartTime) * 1000f;
            Debug.Log($"Initialization complete in {initTime:F0}ms");
            
            isInitialized = true;
            OnAppInitialized?.Invoke();
        }
        
        /// <summary>
        /// Checks if AR is supported on the device.
        /// </summary>
        private IEnumerator CheckARSupport()
        {
            // In production, you would check AR Foundation's ARSession.state
            // For demo purposes, we'll check platform
            
            #if UNITY_ANDROID || UNITY_IOS
            isARSupported = true;
            Debug.Log("AR is supported on this device");
            #else
            isARSupported = false;
            Debug.LogWarning("AR is not supported on this platform. Running in simulation mode.");
            #endif
            
            yield return null;
        }
        
        /// <summary>
        /// Initializes all manager components.
        /// </summary>
        private IEnumerator InitializeManagers()
        {
            // Find or create managers
            if (productManager == null)
            {
                productManager = ProductManager.Instance;
            }
            
            if (arManager == null)
            {
                arManager = ARManager.Instance;
            }
            
            if (recommendationEngine == null)
            {
                recommendationEngine = RecommendationEngine.Instance;
            }
            
            // Wait for product database to load
            while (!ProductManager.Instance.IsLoaded)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Debug.Log($"Product database loaded: {ProductManager.Instance.ProductCount} products");
            
            yield return null;
        }
        
        /// <summary>
        /// Sets up event listeners between components.
        /// </summary>
        private void SetupEventListeners()
        {
            // QR Scanner events
            if (qrScanner != null)
            {
                qrScanner.OnProductScanned += HandleProductScanned;
                qrScanner.OnScanError += HandleScanError;
            }
            
            // Product Manager events
            ProductManager.Instance.OnProductFound += HandleProductFound;
            ProductManager.Instance.OnProductNotFound += HandleProductNotFound;
            
            // AR Manager events
            if (arManager != null)
            {
                arManager.OnProductTracked += HandleProductTracked;
                arManager.OnProductLost += HandleProductLost;
            }
        }
        
        /// <summary>
        /// Handles a product being scanned via QR code.
        /// </summary>
        private void HandleProductScanned(Product product)
        {
            float scanLatency = (Time.realtimeSinceStartup - initializationStartTime) * 1000f;
            
            if (scanLatency > maxLatencyMs)
            {
                Debug.LogWarning($"Scan latency ({scanLatency:F0}ms) exceeds target ({maxLatencyMs}ms)");
            }
            
            OnProductScanned?.Invoke(product);
            
            // Show product in AR
            if (arManager != null && isARSupported)
            {
                Vector3 position = Camera.main != null 
                    ? Camera.main.transform.position + Camera.main.transform.forward * 0.5f 
                    : Vector3.zero;
                arManager.ShowProductOverlay(product, position, Quaternion.identity);
            }
            
            // Show product detail UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowProductDetail(product);
            }
            
            // Log for analytics
            LogProductScan(product);
        }
        
        /// <summary>
        /// Handles scan errors.
        /// </summary>
        private void HandleScanError(string error)
        {
            Debug.LogWarning($"Scan error: {error}");
            OnAppError?.Invoke(error);
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification(error);
            }
        }
        
        /// <summary>
        /// Handles product found in database.
        /// </summary>
        private void HandleProductFound(Product product)
        {
            if (enableDebugMode)
            {
                Debug.Log($"Product found: {product.name} ({product.id})");
            }
        }
        
        /// <summary>
        /// Handles product not found in database.
        /// </summary>
        private void HandleProductNotFound(string id)
        {
            Debug.LogWarning($"Product not found: {id}");
            
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowNotification($"Product not found: {id}");
            }
        }
        
        /// <summary>
        /// Handles product being tracked in AR.
        /// </summary>
        private void HandleProductTracked(Product product)
        {
            if (enableDebugMode)
            {
                Debug.Log($"Product tracked in AR: {product.name}");
            }
        }
        
        /// <summary>
        /// Handles product tracking lost in AR.
        /// </summary>
        private void HandleProductLost(Product product)
        {
            if (enableDebugMode)
            {
                Debug.Log($"Product tracking lost: {product.name}");
            }
        }
        
        /// <summary>
        /// Logs product scan for analytics (KPI tracking).
        /// </summary>
        private void LogProductScan(Product product)
        {
            // In production, send to analytics service
            // For demo, just log locally
            
            int totalScans = PlayerPrefs.GetInt("TotalScans", 0) + 1;
            PlayerPrefs.SetInt("TotalScans", totalScans);
            PlayerPrefs.SetString("LastScanTime", DateTime.Now.ToString());
            PlayerPrefs.Save();
            
            if (enableDebugMode)
            {
                Debug.Log($"Analytics: Product scanned - {product.name} (Total: {totalScans})");
            }
        }
        
        /// <summary>
        /// Simulates scanning a product (for testing without camera).
        /// </summary>
        public void SimulateScan(string qrCodeId)
        {
            if (qrScanner != null)
            {
                qrScanner.SimulateQRScan(qrCodeId);
            }
            else
            {
                Product product = ProductManager.Instance.GetProductByQRCode(qrCodeId);
                if (product != null)
                {
                    HandleProductScanned(product);
                }
            }
        }
        
        /// <summary>
        /// Gets app performance metrics (for Sprint 4 testing).
        /// </summary>
        public PerformanceMetrics GetPerformanceMetrics()
        {
            return new PerformanceMetrics
            {
                frameRate = 1f / Time.deltaTime,
                memoryUsageMB = GC.GetTotalMemory(false) / (1024f * 1024f),
                productCount = ProductManager.Instance.ProductCount,
                isARRunning = arManager != null && arManager.IsARSessionRunning
            };
        }
        
        /// <summary>
        /// Reloads the application.
        /// </summary>
        public void ReloadApp()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        /// <summary>
        /// Quits the application.
        /// </summary>
        public void QuitApp()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
        
        private void OnDestroy()
        {
            // Cleanup event listeners
            if (qrScanner != null)
            {
                qrScanner.OnProductScanned -= HandleProductScanned;
                qrScanner.OnScanError -= HandleScanError;
            }
        }
    }
    
    /// <summary>
    /// Performance metrics for monitoring and testing.
    /// </summary>
    [Serializable]
    public struct PerformanceMetrics
    {
        public float frameRate;
        public float memoryUsageMB;
        public int productCount;
        public bool isARRunning;
        
        public override string ToString()
        {
            return $"FPS: {frameRate:F1}, Memory: {memoryUsageMB:F1}MB, Products: {productCount}, AR: {isARRunning}";
        }
    }
}
