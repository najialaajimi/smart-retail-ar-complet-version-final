using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Affichage visuel de l'Eco-Score (0-100)
    /// </summary>
    public class EcoScoreDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image fillBar;
        [SerializeField] private Image backgroundBar;
        [SerializeField] private Slider scoreSlider;

        [Header("Couleurs graduées")]
        [SerializeField] private Gradient scoreGradient;

        private float currentScore = 50f;

        private void Awake()
        {
            // Créer un gradient par défaut si non défini
            if (scoreGradient == null)
            {
                scoreGradient = CreateDefaultGradient();
            }
        }

        /// <summary>
        /// Définit l'Eco-Score à afficher
        /// </summary>
        public void SetEcoScore(float score)
        {
            currentScore = Mathf.Clamp(score, 0f, 100f);

            if (scoreText != null)
            {
                scoreText.text = $"{currentScore:F0}/100";
            }

            UpdateVisuals();
        }

        /// <summary>
        /// Met à jour les éléments visuels
        /// </summary>
        private void UpdateVisuals()
        {
            float normalizedScore = currentScore / 100f;

            // Mise à jour de la barre de remplissage
            if (fillBar != null)
            {
                fillBar.fillAmount = normalizedScore;
                fillBar.color = scoreGradient.Evaluate(normalizedScore);
            }

            // Mise à jour du slider
            if (scoreSlider != null)
            {
                scoreSlider.value = normalizedScore;
            }
        }

        /// <summary>
        /// Crée un gradient par défaut pour l'Eco-Score
        /// </summary>
        private Gradient CreateDefaultGradient()
        {
            Gradient gradient = new Gradient();

            GradientColorKey[] colorKeys = new GradientColorKey[5];
            colorKeys[0] = new GradientColorKey(new Color(0.90f, 0.30f, 0.24f), 0f);    // Rouge (0-20)
            colorKeys[1] = new GradientColorKey(new Color(0.93f, 0.58f, 0.25f), 0.25f); // Orange (20-40)
            colorKeys[2] = new GradientColorKey(new Color(1f, 0.80f, 0f), 0.5f);        // Jaune (40-60)
            colorKeys[3] = new GradientColorKey(new Color(0.53f, 0.76f, 0.29f), 0.75f); // Vert clair (60-80)
            colorKeys[4] = new GradientColorKey(new Color(0.04f, 0.52f, 0.27f), 1f);    // Vert foncé (80-100)

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);

            gradient.SetKeys(colorKeys, alphaKeys);

            return gradient;
        }

        /// <summary>
        /// Récupère le score actuel
        /// </summary>
        public float GetCurrentScore()
        {
            return currentScore;
        }

        /// <summary>
        /// Récupère la couleur pour un score donné
        /// </summary>
        public Color GetColorForScore(float score)
        {
            float normalizedScore = Mathf.Clamp01(score / 100f);
            return scoreGradient.Evaluate(normalizedScore);
        }
    }
}
