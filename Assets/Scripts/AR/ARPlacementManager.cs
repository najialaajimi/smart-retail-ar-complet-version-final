using UnityEngine;
using SmartRetailAR.Utils;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Gestionnaire de placement d'objets AR dans l'environnement
    /// Permet de placer des produits virtuels sur des surfaces détectées
    /// </summary>
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameObject placementIndicator;
        [SerializeField] private LayerMask placementLayerMask;
        [SerializeField] private float maxPlacementDistance = 5f;

        [Header("Placement")]
        [SerializeField] private bool allowMultiplePlacements = false;
        [SerializeField] private float minDistanceBetweenPlacements = 0.5f;

        // État
        private bool m_IsPlacementMode = false;
        private GameObject m_CurrentPlacementIndicator;
        private Vector3 m_LastPlacementPosition;
        private bool m_HasPlacement = false;

        // Events
        public event System.Action<Vector3, Quaternion> OnObjectPlaced;

        private void Start()
        {
            // Créer l'indicateur de placement si nécessaire
            if (placementIndicator != null && m_CurrentPlacementIndicator == null)
            {
                m_CurrentPlacementIndicator = Instantiate(placementIndicator);
                m_CurrentPlacementIndicator.SetActive(false);
            }
        }

        private void Update()
        {
            if (!m_IsPlacementMode)
                return;

            UpdatePlacementIndicator();
            HandlePlacementInput();
        }

        /// <summary>
        /// Active le mode placement
        /// </summary>
        public void EnablePlacementMode()
        {
            m_IsPlacementMode = true;
            
            if (m_CurrentPlacementIndicator != null)
            {
                m_CurrentPlacementIndicator.SetActive(true);
            }

            DebugLogger.Log("Mode placement AR activé", LogLevel.Debug);
        }

        /// <summary>
        /// Désactive le mode placement
        /// </summary>
        public void DisablePlacementMode()
        {
            m_IsPlacementMode = false;
            
            if (m_CurrentPlacementIndicator != null)
            {
                m_CurrentPlacementIndicator.SetActive(false);
            }

            DebugLogger.Log("Mode placement AR désactivé", LogLevel.Debug);
        }

        /// <summary>
        /// Met à jour la position de l'indicateur de placement
        /// </summary>
        private void UpdatePlacementIndicator()
        {
            if (m_CurrentPlacementIndicator == null || Camera.main == null)
                return;

            // Raycast depuis le centre de l'écran
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            // Note: Dans une vraie implémentation AR, on utiliserait ARRaycastManager
            // pour détecter les plans AR au lieu d'un raycast physique standard

            if (Physics.Raycast(ray, out hit, maxPlacementDistance, placementLayerMask))
            {
                // Positionner l'indicateur sur la surface détectée
                m_CurrentPlacementIndicator.transform.position = hit.point;
                m_CurrentPlacementIndicator.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                
                if (!m_CurrentPlacementIndicator.activeSelf)
                {
                    m_CurrentPlacementIndicator.SetActive(true);
                }
            }
            else
            {
                // Aucune surface détectée
                if (m_CurrentPlacementIndicator.activeSelf)
                {
                    m_CurrentPlacementIndicator.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Gère l'input pour le placement d'objets
        /// </summary>
        private void HandlePlacementInput()
        {
            // Détection du tap/clic
            bool inputDetected = false;

            #if UNITY_EDITOR || UNITY_STANDALONE
            inputDetected = Input.GetMouseButtonDown(0);
            #elif UNITY_ANDROID || UNITY_IOS
            inputDetected = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
            #endif

            if (inputDetected && m_CurrentPlacementIndicator != null && m_CurrentPlacementIndicator.activeSelf)
            {
                TryPlaceObject();
            }
        }

        /// <summary>
        /// Tente de placer un objet à la position actuelle de l'indicateur
        /// </summary>
        private void TryPlaceObject()
        {
            if (!allowMultiplePlacements && m_HasPlacement)
            {
                DebugLogger.LogWarning("Placement multiple désactivé");
                return;
            }

            Vector3 placementPosition = m_CurrentPlacementIndicator.transform.position;
            Quaternion placementRotation = m_CurrentPlacementIndicator.transform.rotation;

            // Vérifier la distance minimale avec le dernier placement
            if (m_HasPlacement)
            {
                float distance = Vector3.Distance(placementPosition, m_LastPlacementPosition);
                if (distance < minDistanceBetweenPlacements)
                {
                    DebugLogger.LogWarning($"Distance insuffisante: {distance:F2}m (minimum: {minDistanceBetweenPlacements}m)");
                    return;
                }
            }

            // Placer l'objet
            m_LastPlacementPosition = placementPosition;
            m_HasPlacement = true;

            DebugLogger.Log($"Objet placé à la position: {placementPosition}", LogLevel.Info);
            OnObjectPlaced?.Invoke(placementPosition, placementRotation);

            // Désactiver le mode placement si un seul placement autorisé
            if (!allowMultiplePlacements)
            {
                DisablePlacementMode();
            }
        }

        /// <summary>
        /// Place un objet à une position spécifique
        /// </summary>
        public void PlaceObjectAt(Vector3 position, Quaternion rotation)
        {
            m_LastPlacementPosition = position;
            m_HasPlacement = true;

            DebugLogger.Log($"Objet placé manuellement à: {position}", LogLevel.Info);
            OnObjectPlaced?.Invoke(position, rotation);
        }

        /// <summary>
        /// Réinitialise tous les placements
        /// </summary>
        public void ClearPlacements()
        {
            m_HasPlacement = false;
            m_LastPlacementPosition = Vector3.zero;
            DebugLogger.Log("Placements réinitialisés", LogLevel.Debug);
        }

        /// <summary>
        /// Obtient la dernière position de placement
        /// </summary>
        public Vector3 GetLastPlacementPosition()
        {
            return m_LastPlacementPosition;
        }

        private void OnDestroy()
        {
            if (m_CurrentPlacementIndicator != null)
            {
                Destroy(m_CurrentPlacementIndicator);
            }
        }
    }
}
