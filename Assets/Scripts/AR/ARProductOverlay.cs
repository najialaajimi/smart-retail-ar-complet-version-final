using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Products;

namespace SmartRetailAR.AR
{
    /// <summary>
    /// Affiche les informations d'un produit en overlay AR
    /// Panel flottant avec informations clés
    /// </summary>
    public class ARProductOverlay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas overlayCanvas;
        [SerializeField] private TextMeshProUGUI productNameText;
        [SerializeField] private TextMeshProUGUI productBrandText;
        [SerializeField] private TextMeshProUGUI productPriceText;
        [SerializeField] private TextMeshProUGUI productOriginText;
        [SerializeField] private TextMeshProUGUI nutriScoreText;
        [SerializeField] private Image nutriScoreBackground;
        [SerializeField] private Image ecoScoreBar;
        [SerializeField] private Image productImage;

        [Header("Configuration")]
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.2f, 0);
        [SerializeField] private float animationDuration = 0.3f;

        private Product currentProduct;
        private Camera mainCamera;
        private bool isVisible = false;
        private float currentScale = 0f;
        private Vector3 targetScale = Vector3.one;

        private void Awake()
        {
            mainCamera = Camera.main;

            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (isVisible && faceCamera && mainCamera != null)
            {
                // Faire face à la caméra
                transform.LookAt(mainCamera.transform);
                transform.Rotate(0, 180, 0);
            }

            // Animation de scaling
            if (currentScale < 1f && isVisible)
            {
                currentScale = Mathf.Min(currentScale + Time.deltaTime / animationDuration, 1f);
                transform.localScale = targetScale * currentScale;
            }
        }

        /// <summary>
        /// Affiche les informations d'un produit
        /// </summary>
        public void ShowProduct(Product product)
        {
            if (product == null)
            {
                Debug.LogWarning("Produit null passé à ShowProduct");
                return;
            }

            currentProduct = product;
            UpdateUI();
            Show();
        }

        /// <summary>
        /// Met à jour l'interface avec les données du produit
        /// </summary>
        private void UpdateUI()
        {
            if (currentProduct == null)
            {
                return;
            }

            // Nom et marque
            if (productNameText != null)
            {
                productNameText.text = currentProduct.name;
            }

            if (productBrandText != null)
            {
                productBrandText.text = currentProduct.brand;
            }

            // Prix
            if (productPriceText != null)
            {
                productPriceText.text = $"{currentProduct.price:F2}€";
            }

            // Origine
            if (productOriginText != null)
            {
                productOriginText.text = $"Origine: {currentProduct.origin}";
            }

            // Nutri-Score
            if (nutriScoreText != null && currentProduct.nutrition != null)
            {
                nutriScoreText.text = currentProduct.nutrition.nutriScore;
                UpdateNutriScoreColor(currentProduct.nutrition.nutriScore);
            }

            // Eco-Score
            if (ecoScoreBar != null)
            {
                ecoScoreBar.fillAmount = currentProduct.ecoScore / 100f;
                UpdateEcoScoreColor(currentProduct.ecoScore);
            }
        }

        /// <summary>
        /// Met à jour la couleur du Nutri-Score
        /// </summary>
        private void UpdateNutriScoreColor(string nutriScore)
        {
            if (nutriScoreBackground == null)
            {
                return;
            }

            Color color = Color.gray;

            switch (nutriScore.ToUpper())
            {
                case "A":
                    color = new Color(0.04f, 0.52f, 0.27f); // Vert foncé
                    break;
                case "B":
                    color = new Color(0.53f, 0.76f, 0.29f); // Vert clair
                    break;
                case "C":
                    color = new Color(1f, 0.80f, 0f); // Jaune
                    break;
                case "D":
                    color = new Color(0.93f, 0.58f, 0.25f); // Orange
                    break;
                case "E":
                    color = new Color(0.90f, 0.30f, 0.24f); // Rouge
                    break;
            }

            nutriScoreBackground.color = color;
        }

        /// <summary>
        /// Met à jour la couleur de l'Eco-Score
        /// </summary>
        private void UpdateEcoScoreColor(float ecoScore)
        {
            if (ecoScoreBar == null)
            {
                return;
            }

            Color color;

            if (ecoScore >= 80)
            {
                color = new Color(0.04f, 0.52f, 0.27f); // Vert
            }
            else if (ecoScore >= 60)
            {
                color = new Color(0.53f, 0.76f, 0.29f); // Vert clair
            }
            else if (ecoScore >= 40)
            {
                color = new Color(1f, 0.80f, 0f); // Jaune
            }
            else if (ecoScore >= 20)
            {
                color = new Color(0.93f, 0.58f, 0.25f); // Orange
            }
            else
            {
                color = new Color(0.90f, 0.30f, 0.24f); // Rouge
            }

            ecoScoreBar.color = color;
        }

        /// <summary>
        /// Affiche l'overlay
        /// </summary>
        public void Show()
        {
            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(true);
            }

            isVisible = true;
            currentScale = 0f;
            transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// Cache l'overlay
        /// </summary>
        public void Hide()
        {
            if (overlayCanvas != null)
            {
                overlayCanvas.gameObject.SetActive(false);
            }

            isVisible = false;
            currentScale = 0f;
        }

        /// <summary>
        /// Positionne l'overlay par rapport à un point
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            transform.position = position + offset;
        }

        /// <summary>
        /// Récupère le produit actuellement affiché
        /// </summary>
        public Product GetCurrentProduct()
        {
            return currentProduct;
        }

        /// <summary>
        /// Vérifie si l'overlay est visible
        /// </summary>
        public bool IsVisible()
        {
            return isVisible;
        }
    }
}
