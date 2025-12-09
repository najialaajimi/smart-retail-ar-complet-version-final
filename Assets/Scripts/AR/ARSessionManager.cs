using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Gère la session AR Foundation (ARCore/ARKit)
    /// Vérifie la compatibilité et initialise les composants AR
    /// </summary>
    [RequireComponent(typeof(ARSession))]
    public class ARSessionManager : MonoBehaviour
    {
        private static ARSessionManager instance;
        public static ARSessionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ARSessionManager>();
                }
                return instance;
            }
        }

        [Header("Composants AR")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private UnityEngine.XR.ARFoundation.XROrigin xrOrigin;

        [Header("Événements")]
        public UnityEvent onARSessionInitialized = new UnityEvent();
        public UnityEvent onARSessionFailed = new UnityEvent();
        public UnityEvent<ARSessionState> onARSessionStateChanged = new UnityEvent<ARSessionState>();

        private bool isARSupported = false;
        private bool isInitialized = false;
        private ARSessionState currentState = ARSessionState.None;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // Récupérer les composants si non assignés
            if (arSession == null)
            {
                arSession = GetComponent<ARSession>();
            }

            if (xrOrigin == null)
            {
                xrOrigin = FindObjectOfType<UnityEngine.XR.ARFoundation.XROrigin>();
            }
        }

        private void Start()
        {
            CheckARSupport();
        }

        /// <summary>
        /// Vérifie si AR est supporté sur l'appareil
        /// </summary>
        private void CheckARSupport()
        {
            #if UNITY_EDITOR
            Debug.LogWarning("AR non disponible dans l'éditeur Unity");
            isARSupported = false;
            #else
            StartCoroutine(CheckARSupportCoroutine());
            #endif
        }

        /// <summary>
        /// Coroutine pour vérifier le support AR
        /// </summary>
        private System.Collections.IEnumerator CheckARSupportCoroutine()
        {
            // Vérifier le support AR
            var checkAvailability = ARSession.CheckAvailability();
            yield return checkAvailability;

            if (checkAvailability.IsCompleted)
            {
                ARSessionState availability = checkAvailability.Result;
                isARSupported = availability == ARSessionState.Ready || 
                                availability == ARSessionState.SessionInitializing;

                if (isARSupported)
                {
                    Debug.Log("AR supporté sur cet appareil");
                    InitializeARSession();
                }
                else
                {
                    Debug.LogError($"AR non supporté: {availability}");
                    onARSessionFailed?.Invoke();
                }
            }
        }

        /// <summary>
        /// Initialise la session AR
        /// </summary>
        public void InitializeARSession()
        {
            if (isInitialized)
            {
                Debug.LogWarning("Session AR déjà initialisée");
                return;
            }

            if (arSession == null)
            {
                Debug.LogError("ARSession non trouvé");
                onARSessionFailed?.Invoke();
                return;
            }

            try
            {
                // Configurer la session
                ARSession.stateChanged += OnARSessionStateChanged;

                isInitialized = true;
                Debug.Log("Session AR initialisée");
                onARSessionInitialized?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"Erreur lors de l'initialisation AR: {e.Message}");
                onARSessionFailed?.Invoke();
            }
        }

        /// <summary>
        /// Callback pour les changements d'état de la session AR
        /// </summary>
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            currentState = args.state;
            Debug.Log($"État AR changé: {currentState}");
            onARSessionStateChanged?.Invoke(currentState);

            switch (currentState)
            {
                case ARSessionState.Ready:
                    Debug.Log("Session AR prête");
                    break;
                case ARSessionState.SessionInitializing:
                    Debug.Log("Initialisation de la session AR...");
                    break;
                case ARSessionState.SessionTracking:
                    Debug.Log("Tracking AR actif");
                    break;
                case ARSessionState.Unsupported:
                    Debug.LogError("AR non supporté sur cet appareil");
                    onARSessionFailed?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// Réinitialise la session AR
        /// </summary>
        public void ResetARSession()
        {
            if (arSession != null)
            {
                arSession.Reset();
                Debug.Log("Session AR réinitialisée");
            }
        }

        /// <summary>
        /// Active/désactive la session AR
        /// </summary>
        public void SetAREnabled(bool enabled)
        {
            if (arSession != null)
            {
                arSession.enabled = enabled;
                Debug.Log($"Session AR {(enabled ? "activée" : "désactivée")}");
            }
        }

        /// <summary>
        /// Vérifie si AR est supporté
        /// </summary>
        public bool IsARSupported()
        {
            return isARSupported;
        }

        /// <summary>
        /// Vérifie si la session est initialisée
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        /// <summary>
        /// Récupère l'état actuel de la session
        /// </summary>
        public ARSessionState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Vérifie si le tracking est actif
        /// </summary>
        public bool IsTracking()
        {
            return currentState == ARSessionState.SessionTracking;
        }

        private void OnDestroy()
        {
            if (arSession != null)
            {
                ARSession.stateChanged -= OnARSessionStateChanged;
            }
        }
    }
}
