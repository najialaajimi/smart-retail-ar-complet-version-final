using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SmartRetailAR.Data;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// AR Manager handles all augmented reality functionality.
    /// Sprint 2: AR integration with data overlay on products.
    /// Works with AR Foundation for ARCore/ARKit support.
    /// </summary>
    public class ARManager : MonoBehaviour
    {
        private static ARManager _instance;
        public static ARManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ARManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ARManager");
                        _instance = go.AddComponent<ARManager>();
                    }
                }
                return _instance;
            }
        }
        
        [Header("AR Settings")]
        [SerializeField] private bool enablePlaneDetection = true;
        [SerializeField] private bool enableImageTracking = true;
        [SerializeField] private float overlayDistance = 0.5f;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject productInfoOverlayPrefab;
        [SerializeField] private GameObject nutritionPanelPrefab;
        [SerializeField] private GameObject recommendationsPanelPrefab;
        [SerializeField] private GameObject scanIndicatorPrefab;
        
        [Header("Performance")]
        [SerializeField] private int maxOverlays = 5;
        [SerializeField] private float overlayUpdateInterval = 0.1f;
        
        // Active overlays tracking
        private Dictionary<string, ARProductOverlay> activeOverlays;
        private Queue<ARProductOverlay> overlayPool;
        
        // AR state
        private bool isARSessionRunning;
        private bool isTracking;
        
        // Events
        public event Action OnARSessionStarted;
        public event Action OnARSessionPaused;
        public event Action<Product> OnProductTracked;
        public event Action<Product> OnProductLost;
        
        public bool IsARSessionRunning => isARSessionRunning;
        public bool IsTracking => isTracking;
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            activeOverlays = new Dictionary<string, ARProductOverlay>();
            overlayPool = new Queue<ARProductOverlay>();
        }
        
        private void Start()
        {
            InitializeAR();
        }
        
        /// <summary>
        /// Initializes the AR session and components.
        /// In production, this would set up AR Foundation components.
        /// </summary>
        private void InitializeAR()
        {
            Debug.Log("Initializing AR Manager...");
            
            // In production with AR Foundation:
            // - Initialize ARSession
            // - Set up ARSessionOrigin
            // - Configure ARPlaneManager if plane detection enabled
            // - Configure ARTrackedImageManager if image tracking enabled
            
            isARSessionRunning = true;
            OnARSessionStarted?.Invoke();
            
            Debug.Log("AR Manager initialized successfully");
        }
        
        /// <summary>
        /// Displays product information as an AR overlay.
        /// </summary>
        public ARProductOverlay ShowProductOverlay(Product product, Vector3 position, Quaternion rotation)
        {
            if (product == null)
            {
                Debug.LogError("Cannot show overlay for null product");
                return null;
            }
            
            // Check if overlay already exists for this product
            if (activeOverlays.TryGetValue(product.id, out ARProductOverlay existingOverlay))
            {
                existingOverlay.UpdatePosition(position, rotation);
                return existingOverlay;
            }
            
            // Check max overlays limit
            if (activeOverlays.Count >= maxOverlays)
            {
                RemoveOldestOverlay();
            }
            
            // Get or create overlay
            ARProductOverlay overlay = GetOrCreateOverlay();
            overlay.Initialize(product, position, rotation);
            activeOverlays[product.id] = overlay;
            
            OnProductTracked?.Invoke(product);
            
            return overlay;
        }
        
        /// <summary>
        /// Shows product overlay at a screen position (converts to world position).
        /// </summary>
        public ARProductOverlay ShowProductOverlayAtScreen(Product product, Vector2 screenPosition)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found");
                return null;
            }
            
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, overlayDistance));
            Quaternion rotation = Quaternion.LookRotation(mainCamera.transform.forward);
            
            return ShowProductOverlay(product, worldPosition, rotation);
        }
        
        /// <summary>
        /// Hides the overlay for a specific product.
        /// </summary>
        public void HideProductOverlay(string productId)
        {
            if (activeOverlays.TryGetValue(productId, out ARProductOverlay overlay))
            {
                var product = overlay.Product;
                overlay.Hide();
                ReturnOverlayToPool(overlay);
                activeOverlays.Remove(productId);
                
                OnProductLost?.Invoke(product);
            }
        }
        
        /// <summary>
        /// Hides all active overlays.
        /// </summary>
        public void HideAllOverlays()
        {
            foreach (var kvp in activeOverlays)
            {
                kvp.Value.Hide();
                ReturnOverlayToPool(kvp.Value);
            }
            activeOverlays.Clear();
        }
        
        /// <summary>
        /// Gets an overlay from the pool or creates a new one.
        /// </summary>
        private ARProductOverlay GetOrCreateOverlay()
        {
            ARProductOverlay overlay;
            
            if (overlayPool.Count > 0)
            {
                overlay = overlayPool.Dequeue();
                overlay.gameObject.SetActive(true);
            }
            else
            {
                GameObject overlayObject;
                if (productInfoOverlayPrefab != null)
                {
                    overlayObject = Instantiate(productInfoOverlayPrefab, transform);
                }
                else
                {
                    overlayObject = new GameObject("ARProductOverlay");
                    overlayObject.transform.SetParent(transform);
                }
                
                overlay = overlayObject.GetComponent<ARProductOverlay>();
                if (overlay == null)
                {
                    overlay = overlayObject.AddComponent<ARProductOverlay>();
                }
            }
            
            return overlay;
        }
        
        /// <summary>
        /// Returns an overlay to the pool for reuse.
        /// </summary>
        private void ReturnOverlayToPool(ARProductOverlay overlay)
        {
            overlay.gameObject.SetActive(false);
            overlayPool.Enqueue(overlay);
        }
        
        /// <summary>
        /// Removes the oldest overlay to make room for new ones.
        /// </summary>
        private void RemoveOldestOverlay()
        {
            if (activeOverlays.Count == 0) return;
            
            string oldestId = null;
            float oldestTime = float.MaxValue;
            
            foreach (var kvp in activeOverlays)
            {
                if (kvp.Value.CreationTime < oldestTime)
                {
                    oldestTime = kvp.Value.CreationTime;
                    oldestId = kvp.Key;
                }
            }
            
            if (oldestId != null)
            {
                HideProductOverlay(oldestId);
            }
        }
        
        /// <summary>
        /// Updates overlay positions based on camera movement.
        /// Called by AR Foundation's ARSession update in production.
        /// </summary>
        public void UpdateOverlayPositions()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
            
            foreach (var kvp in activeOverlays)
            {
                kvp.Value.FaceCamera(mainCamera);
            }
        }
        
        private void Update()
        {
            if (isARSessionRunning)
            {
                UpdateOverlayPositions();
            }
        }
        
        /// <summary>
        /// Pauses the AR session.
        /// </summary>
        public void PauseARSession()
        {
            isARSessionRunning = false;
            OnARSessionPaused?.Invoke();
        }
        
        /// <summary>
        /// Resumes the AR session.
        /// </summary>
        public void ResumeARSession()
        {
            isARSessionRunning = true;
            OnARSessionStarted?.Invoke();
        }
        
        /// <summary>
        /// Gets tracking quality status.
        /// </summary>
        public string GetTrackingStatus()
        {
            if (!isARSessionRunning) return "AR Session Paused";
            if (!isTracking) return "Looking for surfaces...";
            return "Tracking Active";
        }
        
        private void OnDestroy()
        {
            HideAllOverlays();
        }
    }
}
