using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SmartRetailAR.Data;
using SmartRetailAR.Core;
using SmartRetailAR.QRCode;

namespace SmartRetailAR.Utils
{
    /// <summary>
    /// Scene automation utility that executes all GameObjects in a scene.
    /// Used for automated testing and demonstration purposes.
    /// </summary>
    public class SceneAutomation : MonoBehaviour
    {
        [Header("Automation Settings")]
        [SerializeField] private bool autoRunOnStart = false;
        [SerializeField] private float executionDelay = 0.5f;
        [SerializeField] private bool logProgress = true;
        
        [Header("Scene Configuration")]
        [SerializeField] private List<AutomationStep> automationSteps;
        
        private int currentStepIndex;
        private bool isRunning;
        
        public event Action OnAutomationStarted;
        public event Action OnAutomationCompleted;
        public event Action<AutomationStep> OnStepCompleted;
        
        private void Start()
        {
            if (autoRunOnStart)
            {
                StartAutomation();
            }
        }
        
        /// <summary>
        /// Starts the automation sequence.
        /// </summary>
        public void StartAutomation()
        {
            if (isRunning)
            {
                Debug.LogWarning("Automation already running");
                return;
            }
            
            StartCoroutine(RunAutomation());
        }
        
        /// <summary>
        /// Stops the automation sequence.
        /// </summary>
        public void StopAutomation()
        {
            isRunning = false;
            StopAllCoroutines();
        }
        
        /// <summary>
        /// Runs the automation sequence.
        /// </summary>
        private IEnumerator RunAutomation()
        {
            isRunning = true;
            currentStepIndex = 0;
            
            Log("=== Starting Scene Automation ===");
            OnAutomationStarted?.Invoke();
            
            // Initialize all managers
            yield return InitializeManagers();
            
            // Execute automation steps
            if (automationSteps != null && automationSteps.Count > 0)
            {
                foreach (var step in automationSteps)
                {
                    if (!isRunning) break;
                    
                    Log($"Executing step: {step.stepName}");
                    yield return ExecuteStep(step);
                    OnStepCompleted?.Invoke(step);
                    
                    currentStepIndex++;
                    yield return new WaitForSeconds(executionDelay);
                }
            }
            else
            {
                // Default automation if no steps defined
                yield return DefaultAutomation();
            }
            
            Log("=== Automation Complete ===");
            isRunning = false;
            OnAutomationCompleted?.Invoke();
        }
        
        /// <summary>
        /// Initializes all required managers.
        /// </summary>
        private IEnumerator InitializeManagers()
        {
            Log("Initializing managers...");
            
            // Ensure ProductManager is loaded
            while (!ProductManager.Instance.IsLoaded)
            {
                yield return new WaitForSeconds(0.1f);
            }
            
            Log($"ProductManager loaded: {ProductManager.Instance.ProductCount} products");
            
            // Ensure AppController is initialized
            if (AppController.Instance != null)
            {
                while (!AppController.Instance.IsInitialized)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                Log("AppController initialized");
            }
            
            yield return null;
        }
        
        /// <summary>
        /// Executes a single automation step.
        /// </summary>
        private IEnumerator ExecuteStep(AutomationStep step)
        {
            switch (step.stepType)
            {
                case AutomationStepType.ScanProduct:
                    yield return ScanProduct(step.productId);
                    break;
                    
                case AutomationStepType.ShowProductDetail:
                    yield return ShowProductDetail(step.productId);
                    break;
                    
                case AutomationStepType.ShowRecommendations:
                    yield return ShowRecommendations(step.productId);
                    break;
                    
                case AutomationStepType.Navigate:
                    yield return Navigate(step.targetScreen);
                    break;
                    
                case AutomationStepType.Wait:
                    yield return new WaitForSeconds(step.waitDuration);
                    break;
                    
                case AutomationStepType.RunTests:
                    yield return RunTests();
                    break;
                    
                case AutomationStepType.Custom:
                    step.customAction?.Invoke();
                    break;
            }
        }
        
        /// <summary>
        /// Default automation sequence when no steps are defined.
        /// </summary>
        private IEnumerator DefaultAutomation()
        {
            Log("Running default automation sequence...");
            
            var products = ProductManager.Instance.GetAllProducts();
            
            // Scan first product
            if (products.Count > 0)
            {
                Log($"Scanning product: {products[0].name}");
                yield return ScanProduct(products[0].qrCodeId);
                yield return new WaitForSeconds(2f);
                
                // Show recommendations
                Log("Showing recommendations...");
                yield return ShowRecommendations(products[0].id);
                yield return new WaitForSeconds(2f);
            }
            
            // Run automated tests
            Log("Running automated tests...");
            yield return RunTests();
        }
        
        /// <summary>
        /// Simulates scanning a product.
        /// </summary>
        private IEnumerator ScanProduct(string qrCodeOrProductId)
        {
            if (AppController.Instance != null)
            {
                AppController.Instance.SimulateScan(qrCodeOrProductId);
            }
            else
            {
                var product = ProductManager.Instance.GetProductByQRCode(qrCodeOrProductId);
                if (product == null)
                {
                    product = ProductManager.Instance.GetProductById(qrCodeOrProductId);
                }
                
                if (product != null)
                {
                    Log($"Scanned: {product.name}");
                    UI.UIManager.Instance?.ShowProductDetail(product);
                }
            }
            
            yield return new WaitForSeconds(1f);
        }
        
        /// <summary>
        /// Shows product detail screen.
        /// </summary>
        private IEnumerator ShowProductDetail(string productId)
        {
            var product = ProductManager.Instance.GetProductById(productId);
            if (product != null && UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.ShowProductDetail(product);
            }
            
            yield return null;
        }
        
        /// <summary>
        /// Shows recommendations for a product.
        /// </summary>
        private IEnumerator ShowRecommendations(string productId)
        {
            var product = ProductManager.Instance.GetProductById(productId);
            if (product != null && UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.ShowRecommendations(product);
            }
            
            yield return null;
        }
        
        /// <summary>
        /// Navigates to a screen.
        /// </summary>
        private IEnumerator Navigate(string screenName)
        {
            if (UI.UIManager.Instance == null) yield break;
            
            switch (screenName.ToLower())
            {
                case "mainmenu":
                case "main":
                    UI.UIManager.Instance.ShowMainMenu();
                    break;
                case "scanner":
                case "scan":
                    UI.UIManager.Instance.ShowScanner();
                    break;
                case "search":
                    UI.UIManager.Instance.ShowSearch();
                    break;
                case "settings":
                    UI.UIManager.Instance.ShowSettings();
                    break;
                case "back":
                    UI.UIManager.Instance.NavigateBack();
                    break;
            }
            
            yield return null;
        }
        
        /// <summary>
        /// Runs automated tests.
        /// </summary>
        private IEnumerator RunTests()
        {
            var testRunner = FindObjectOfType<Tests.TestRunner>();
            if (testRunner != null)
            {
                yield return testRunner.RunAllTests();
            }
            else
            {
                Log("No TestRunner found in scene");
            }
        }
        
        private void Log(string message)
        {
            if (logProgress)
            {
                Debug.Log($"[SceneAutomation] {message}");
            }
        }
    }
    
    /// <summary>
    /// Types of automation steps.
    /// </summary>
    public enum AutomationStepType
    {
        ScanProduct,
        ShowProductDetail,
        ShowRecommendations,
        Navigate,
        Wait,
        RunTests,
        Custom
    }
    
    /// <summary>
    /// Represents a single automation step.
    /// </summary>
    [Serializable]
    public class AutomationStep
    {
        public string stepName;
        public AutomationStepType stepType;
        public string productId;
        public string targetScreen;
        public float waitDuration = 1f;
        public UnityEvent customAction;
    }
}
