using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Affichage visuel du Nutri-Score (A-E)
    /// </summary>
    public class NutriScoreDisplay : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image[] scoreIndicators;

        [Header("Couleurs")]
        [SerializeField] private Color colorA = new Color(0.04f, 0.52f, 0.27f); // Vert foncé
        [SerializeField] private Color colorB = new Color(0.53f, 0.76f, 0.29f); // Vert clair
        [SerializeField] private Color colorC = new Color(1f, 0.80f, 0f);       // Jaune
        [SerializeField] private Color colorD = new Color(0.93f, 0.58f, 0.25f); // Orange
        [SerializeField] private Color colorE = new Color(0.90f, 0.30f, 0.24f); // Rouge

        private string currentScore = "C";

        /// <summary>
        /// Définit le Nutri-Score à afficher
        /// </summary>
        public void SetNutriScore(string score)
        {
            if (string.IsNullOrEmpty(score))
            {
                score = "C";
            }

            currentScore = score.ToUpper();

            if (scoreText != null)
            {
                scoreText.text = currentScore;
            }

            UpdateColor();
            UpdateIndicators();
        }

        /// <summary>
        /// Met à jour la couleur selon le score
        /// </summary>
        private void UpdateColor()
        {
            Color targetColor = GetColorForScore(currentScore);

            if (backgroundImage != null)
            {
                backgroundImage.color = targetColor;
            }

            if (scoreText != null)
            {
                scoreText.color = Color.white;
            }
        }

        /// <summary>
        /// Met à jour les indicateurs visuels
        /// </summary>
        private void UpdateIndicators()
        {
            if (scoreIndicators == null || scoreIndicators.Length == 0)
            {
                return;
            }

            int scoreIndex = GetScoreIndex(currentScore);

            for (int i = 0; i < scoreIndicators.Length; i++)
            {
                if (scoreIndicators[i] != null)
                {
                    scoreIndicators[i].color = i == scoreIndex ? 
                        GetColorForScore(currentScore) : 
                        new Color(0.8f, 0.8f, 0.8f, 0.3f);
                }
            }
        }

        /// <summary>
        /// Récupère la couleur associée au score
        /// </summary>
        private Color GetColorForScore(string score)
        {
            switch (score)
            {
                case "A": return colorA;
                case "B": return colorB;
                case "C": return colorC;
                case "D": return colorD;
                case "E": return colorE;
                default: return colorC;
            }
        }

        /// <summary>
        /// Récupère l'index du score (A=0, E=4)
        /// </summary>
        private int GetScoreIndex(string score)
        {
            switch (score)
            {
                case "A": return 0;
                case "B": return 1;
                case "C": return 2;
                case "D": return 3;
                case "E": return 4;
                default: return 2;
            }
        }

        /// <summary>
        /// Récupère le score actuel
        /// </summary>
        public string GetCurrentScore()
        {
            return currentScore;
        }
    }
}
