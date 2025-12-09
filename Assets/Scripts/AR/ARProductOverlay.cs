using UnityEngine;
using SmartRetailAR.Data;
using SmartRetailAR.Utils;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Overlay AR pour afficher les informations du produit en réalité augmentée
    /// Sprint 2 - Superposition des données sur le produit
    /// </summary>
    public class ARProductOverlay : MonoBehaviour
    {
        [Header("Références UI")]
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private UnityEngine.UI.Text productNameText;
        [SerializeField] private UnityEngine.UI.Text productPriceText;
        [SerializeField] private UnityEngine.UI.Text nutriScoreText;
        [SerializeField] private UnityEngine.UI.Text ecoScoreText;
        [SerializeField] private UnityEngine.UI.Image nutriScoreIcon;
        [SerializeField] private UnityEngine.UI.Image ecoScoreIcon;
        [SerializeField] private GameObject infoPanel;

        [Header("Configuration")]
        [SerializeField] private float displayDistance = 0.3f;
        [SerializeField] private bool followProduct = true;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private bool autoHide = true;
        [SerializeField] private float autoHideDelay = 5f;

        [Header("Animation")]
        [SerializeField] private bool useAnimation = true;
        [SerializeField] private float animationDuration = 0.5f;

        // État
        private ProductData m_CurrentProduct;
        private Transform m_TargetTransform;
        private bool m_IsVisible = false;
        private float m_ShowTime = 0f;
        private Vector3 m_TargetScale;
        private Vector3 m_InitialScale;

        // Couleurs des scores
        private readonly Color[] m_ScoreColors = new Color[]
        {
            new Color(0.0f, 0.6f, 0.0f), // A - Vert foncé
            new Color(0.5f, 0.8f, 0.0f), // B - Vert clair
            new Color(1.0f, 0.8f, 0.0f), // C - Jaune
            new Color(1.0f, 0.5f, 0.0f), // D - Orange
            new Color(0.8f, 0.0f, 0.0f)  // E - Rouge
        };

        private void Awake()
        {
            if (overlayCanvas != null)
            {
                m_InitialScale = overlayCanvas.transform.localScale;
                m_TargetScale = m_InitialScale;
            }

            // Cacher par défaut
            SetVisibility(false, false);
        }

        private void Update()
        {
            if (!m_IsVisible)
                return;

            // Suivre la cible si activé
            if (followProduct && m_TargetTransform != null)
            {
                UpdatePosition();
            }

            // Animation de l'échelle
            if (useAnimation && overlayCanvas != null)
            {
                overlayCanvas.transform.localScale = Vector3.Lerp(
                    overlayCanvas.transform.localScale,
                    m_TargetScale,
                    Time.deltaTime * smoothSpeed
                );
            }

            // Auto-hide après délai
            if (autoHide && Time.time - m_ShowTime > autoHideDelay)
            {
                Hide();
            }
        }

        /// <summary>
        /// Affiche les informations d'un produit
        /// </summary>
        public void ShowProduct(ProductData product, Transform targetPosition = null)
        {
            if (product == null)
            {
                DebugLogger.LogWarning("Produit null, impossible d'afficher l'overlay");
                return;
            }

            m_CurrentProduct = product;
            m_TargetTransform = targetPosition;
            m_ShowTime = Time.time;

            UpdateProductInfo();
            SetVisibility(true, useAnimation);

            DebugLogger.Log($"Affichage overlay AR pour: {product.name}", LogLevel.Info);
        }

        /// <summary>
        /// Met à jour les informations affichées
        /// </summary>
        private void UpdateProductInfo()
        {
            if (m_CurrentProduct == null)
                return;

            // Nom du produit
            if (productNameText != null)
            {
                productNameText.text = $"{m_CurrentProduct.brand}\n{m_CurrentProduct.name}";
            }

            // Prix
            if (productPriceText != null)
            {
                productPriceText.text = $"{m_CurrentProduct.price:F2}€";
            }

            // Nutri-Score
            if (nutriScoreText != null)
            {
                nutriScoreText.text = $"Nutri-Score: {m_CurrentProduct.nutrition.nutriscore}";
            }

            if (nutriScoreIcon != null)
            {
                nutriScoreIcon.color = GetScoreColor(m_CurrentProduct.nutrition.nutriscore);
            }

            // Eco-Score
            if (ecoScoreText != null)
            {
                ecoScoreText.text = $"Eco-Score: {m_CurrentProduct.ecoScore}";
            }

            if (ecoScoreIcon != null)
            {
                ecoScoreIcon.color = GetScoreColor(m_CurrentProduct.ecoScore);
            }
        }

        /// <summary>
        /// Met à jour la position de l'overlay
        /// </summary>
        private void UpdatePosition()
        {
            if (m_TargetTransform == null)
                return;

            // Positionner l'overlay au-dessus du produit
            Vector3 targetPosition = m_TargetTransform.position + Vector3.up * displayDistance;
            
            if (followProduct)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
            }
            else
            {
                transform.position = targetPosition;
            }

            // Faire face à la caméra
            if (Camera.main != null)
            {
                Vector3 lookDirection = Camera.main.transform.position - transform.position;
                lookDirection.y = 0; // Garder l'overlay horizontal
                
                if (lookDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
                }
            }
        }

        /// <summary>
        /// Cache l'overlay
        /// </summary>
        public void Hide()
        {
            SetVisibility(false, useAnimation);
            DebugLogger.Log("Overlay AR caché", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche ou cache l'overlay
        /// </summary>
        private void SetVisibility(bool visible, bool animate)
        {
            m_IsVisible = visible;

            if (animate && overlayCanvas != null)
            {
                m_TargetScale = visible ? m_InitialScale : Vector3.zero;
            }
            else
            {
                if (overlayCanvas != null)
                {
                    overlayCanvas.gameObject.SetActive(visible);
                }
                
                if (infoPanel != null)
                {
                    infoPanel.SetActive(visible);
                }
            }
        }

        /// <summary>
        /// Obtient la couleur correspondant à un score
        /// </summary>
        private Color GetScoreColor(string score)
        {
            switch (score?.ToUpper())
            {
                case "A": return m_ScoreColors[0];
                case "B": return m_ScoreColors[1];
                case "C": return m_ScoreColors[2];
                case "D": return m_ScoreColors[3];
                case "E": return m_ScoreColors[4];
                default: return Color.gray;
            }
        }

        /// <summary>
        /// Définit la distance d'affichage
        /// </summary>
        public void SetDisplayDistance(float distance)
        {
            displayDistance = distance;
        }

        /// <summary>
        /// Active/désactive le suivi de la cible
        /// </summary>
        public void SetFollowProduct(bool follow)
        {
            followProduct = follow;
        }
    }
}
