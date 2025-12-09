using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Événement de détection d'image
    /// </summary>
    [Serializable]
    public class ImageTrackedEvent : UnityEvent<ARTrackedImage> { }

    /// <summary>
    /// Gère la reconnaissance et le tracking d'images en AR
    /// Détecte les images de produits et déclenche des événements
    /// </summary>
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class ImageTrackingManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Bibliothèque d'images de référence")]
        [SerializeField] private XRReferenceImageLibrary imageLibrary;

        [Tooltip("Nombre maximum d'images trackées simultanément")]
        [SerializeField] private int maxNumberOfMovingImages = 3;

        [Header("Événements")]
        public ImageTrackedEvent onImageDetected = new ImageTrackedEvent();
        public ImageTrackedEvent onImageTracking = new ImageTrackedEvent();
        public ImageTrackedEvent onImageLost = new ImageTrackedEvent();

        private ARTrackedImageManager trackedImageManager;
        private Dictionary<string, ARTrackedImage> trackedImages = new Dictionary<string, ARTrackedImage>();
        private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

        private void Awake()
        {
            trackedImageManager = GetComponent<ARTrackedImageManager>();

            if (trackedImageManager == null)
            {
                Debug.LogError("ARTrackedImageManager non trouvé");
                return;
            }

            // Configuration
            trackedImageManager.requestedMaxNumberOfMovingImages = maxNumberOfMovingImages;

            if (imageLibrary != null)
            {
                trackedImageManager.referenceLibrary = imageLibrary;
            }
        }

        private void OnEnable()
        {
            if (trackedImageManager != null)
            {
                trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
            }
        }

        private void OnDisable()
        {
            if (trackedImageManager != null)
            {
                trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            }
        }

        /// <summary>
        /// Callback pour les changements d'images trackées
        /// </summary>
        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            // Images nouvellement détectées
            foreach (var trackedImage in eventArgs.added)
            {
                OnImageAdded(trackedImage);
            }

            // Images mises à jour
            foreach (var trackedImage in eventArgs.updated)
            {
                OnImageUpdated(trackedImage);
            }

            // Images perdues
            foreach (var trackedImage in eventArgs.removed)
            {
                OnImageRemoved(trackedImage);
            }
        }

        /// <summary>
        /// Appelé quand une nouvelle image est détectée
        /// </summary>
        private void OnImageAdded(ARTrackedImage trackedImage)
        {
            string imageName = trackedImage.referenceImage.name;
            
            if (!trackedImages.ContainsKey(imageName))
            {
                trackedImages.Add(imageName, trackedImage);
            }

            Debug.Log($"Image détectée: {imageName}");
            onImageDetected?.Invoke(trackedImage);
        }

        /// <summary>
        /// Appelé quand une image trackée est mise à jour
        /// </summary>
        private void OnImageUpdated(ARTrackedImage trackedImage)
        {
            string imageName = trackedImage.referenceImage.name;

            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                // Image activement trackée
                if (trackedImages.ContainsKey(imageName))
                {
                    trackedImages[imageName] = trackedImage;
                }

                onImageTracking?.Invoke(trackedImage);
            }
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                // Tracking limité
                Debug.LogWarning($"Tracking limité pour: {imageName}");
            }
        }

        /// <summary>
        /// Appelé quand une image n'est plus trackée
        /// </summary>
        private void OnImageRemoved(ARTrackedImage trackedImage)
        {
            string imageName = trackedImage.referenceImage.name;

            if (trackedImages.ContainsKey(imageName))
            {
                trackedImages.Remove(imageName);
            }

            Debug.Log($"Image perdue: {imageName}");
            onImageLost?.Invoke(trackedImage);
        }

        /// <summary>
        /// Attache un prefab à une image trackée
        /// </summary>
        public GameObject AttachPrefabToImage(ARTrackedImage trackedImage, GameObject prefab)
        {
            if (trackedImage == null || prefab == null)
            {
                return null;
            }

            string imageName = trackedImage.referenceImage.name;

            // Supprimer l'ancien prefab s'il existe
            if (spawnedPrefabs.ContainsKey(imageName))
            {
                Destroy(spawnedPrefabs[imageName]);
                spawnedPrefabs.Remove(imageName);
            }

            // Instancier le nouveau prefab
            GameObject obj = Instantiate(prefab, trackedImage.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            spawnedPrefabs[imageName] = obj;

            return obj;
        }

        /// <summary>
        /// Détache le prefab d'une image
        /// </summary>
        public void DetachPrefabFromImage(string imageName)
        {
            if (spawnedPrefabs.ContainsKey(imageName))
            {
                Destroy(spawnedPrefabs[imageName]);
                spawnedPrefabs.Remove(imageName);
            }
        }

        /// <summary>
        /// Récupère une image trackée par son nom
        /// </summary>
        public ARTrackedImage GetTrackedImage(string imageName)
        {
            if (trackedImages.TryGetValue(imageName, out ARTrackedImage image))
            {
                return image;
            }

            return null;
        }

        /// <summary>
        /// Récupère toutes les images actuellement trackées
        /// </summary>
        public List<ARTrackedImage> GetAllTrackedImages()
        {
            return new List<ARTrackedImage>(trackedImages.Values);
        }

        /// <summary>
        /// Vérifie si une image est actuellement trackée
        /// </summary>
        public bool IsImageTracked(string imageName)
        {
            return trackedImages.ContainsKey(imageName);
        }

        /// <summary>
        /// Active/désactive le tracking d'images
        /// </summary>
        public void SetTrackingEnabled(bool enabled)
        {
            if (trackedImageManager != null)
            {
                trackedImageManager.enabled = enabled;
            }
        }

        /// <summary>
        /// Change la bibliothèque d'images de référence
        /// </summary>
        public void SetReferenceLibrary(XRReferenceImageLibrary library)
        {
            if (trackedImageManager != null && library != null)
            {
                imageLibrary = library;
                trackedImageManager.referenceLibrary = library;
                Debug.Log("Bibliothèque d'images mise à jour");
            }
        }
    }
}
