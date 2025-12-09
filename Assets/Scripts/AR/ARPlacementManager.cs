using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Gère le placement d'objets en AR
    /// Détecte les plans et place des objets sur ceux-ci
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARPlacementManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Prefab à placer en AR")]
        [SerializeField] private GameObject placementPrefab;

        [Tooltip("Indicateur de placement")]
        [SerializeField] private GameObject placementIndicator;

        [Header("Paramètres")]
        [SerializeField] private bool autoPlacement = false;
        [SerializeField] private float placementHeight = 0f;

        private ARRaycastManager arRaycastManager;
        private ARPlaneManager arPlaneManager;
        private List<ARRaycastHit> hits = new List<ARRaycastHit>();
        
        private GameObject currentPlacedObject;
        private bool isPlacementValid = false;
        private Vector3 placementPosition;
        private Quaternion placementRotation;

        private void Awake()
        {
            arRaycastManager = GetComponent<ARRaycastManager>();
            arPlaneManager = GetComponent<ARPlaneManager>();

            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
        }

        private void Update()
        {
            UpdatePlacementIndicator();

            if (autoPlacement && isPlacementValid && currentPlacedObject == null)
            {
                PlaceObject();
            }
        }

        /// <summary>
        /// Met à jour l'indicateur de placement
        /// </summary>
        private void UpdatePlacementIndicator()
        {
            if (arRaycastManager == null)
            {
                return;
            }

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                placementPosition = hitPose.position + Vector3.up * placementHeight;
                placementRotation = hitPose.rotation;
                isPlacementValid = true;

                if (placementIndicator != null && !placementIndicator.activeSelf)
                {
                    placementIndicator.SetActive(true);
                }

                if (placementIndicator != null)
                {
                    placementIndicator.transform.SetPositionAndRotation(placementPosition, placementRotation);
                }
            }
            else
            {
                isPlacementValid = false;

                if (placementIndicator != null && placementIndicator.activeSelf)
                {
                    placementIndicator.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Place un objet à la position actuelle
        /// </summary>
        public GameObject PlaceObject()
        {
            if (!isPlacementValid || placementPrefab == null)
            {
                Debug.LogWarning("Placement invalide ou prefab manquant");
                return null;
            }

            if (currentPlacedObject != null)
            {
                Destroy(currentPlacedObject);
            }

            currentPlacedObject = Instantiate(placementPrefab, placementPosition, placementRotation);
            Debug.Log($"Objet placé à: {placementPosition}");

            return currentPlacedObject;
        }

        /// <summary>
        /// Place un objet personnalisé
        /// </summary>
        public GameObject PlaceObject(GameObject prefab)
        {
            if (!isPlacementValid || prefab == null)
            {
                return null;
            }

            GameObject obj = Instantiate(prefab, placementPosition, placementRotation);
            return obj;
        }

        /// <summary>
        /// Supprime l'objet actuellement placé
        /// </summary>
        public void RemoveCurrentObject()
        {
            if (currentPlacedObject != null)
            {
                Destroy(currentPlacedObject);
                currentPlacedObject = null;
                Debug.Log("Objet supprimé");
            }
        }

        /// <summary>
        /// Définit le prefab à placer
        /// </summary>
        public void SetPlacementPrefab(GameObject prefab)
        {
            placementPrefab = prefab;
        }

        /// <summary>
        /// Vérifie si le placement est valide
        /// </summary>
        public bool IsPlacementValid()
        {
            return isPlacementValid;
        }

        /// <summary>
        /// Récupère la position de placement actuelle
        /// </summary>
        public Vector3 GetPlacementPosition()
        {
            return placementPosition;
        }

        /// <summary>
        /// Récupère l'objet actuellement placé
        /// </summary>
        public GameObject GetCurrentPlacedObject()
        {
            return currentPlacedObject;
        }

        /// <summary>
        /// Active/désactive la détection de plans
        /// </summary>
        public void SetPlaneDetectionEnabled(bool enabled)
        {
            if (arPlaneManager != null)
            {
                arPlaneManager.enabled = enabled;
            }
        }
    }
}
