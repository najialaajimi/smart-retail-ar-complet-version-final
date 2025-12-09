using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Data;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Affichage détaillé des informations nutritionnelles
    /// </summary>
    public class NutritionInfoDisplay : MonoBehaviour
    {
        [Header("UI Nutrition")]
        [SerializeField] private Text caloriesText;
        [SerializeField] private Text proteinsText;
        [SerializeField] private Text carbsText;
        [SerializeField] private Text fatText;
        [SerializeField] private Text fiberText;
        [SerializeField] private Text sugarText;
        [SerializeField] private Text saltText;

        [Header("Nutri-Score")]
        [SerializeField] private Text nutriScoreText;
        [SerializeField] private Image nutriScoreBackground;
        [SerializeField] private GameObject nutriScorePanel;

        [Header("Barres de progression")]
        [SerializeField] private Slider proteinsSlider;
        [SerializeField] private Slider carbsSlider;
        [SerializeField] private Slider fatSlider;
        [SerializeField] private Slider fiberSlider;

        [Header("Valeurs de référence (pour 100g)")]
        [SerializeField] private float maxProteins = 30f;
        [SerializeField] private float maxCarbs = 100f;
        [SerializeField] private float maxFat = 30f;
        [SerializeField] private float maxFiber = 10f;

        /// <summary>
        /// Affiche les informations nutritionnelles
        /// </summary>
        public void ShowNutrition(NutritionData nutrition)
        {
            if (nutrition == null)
                return;

            // Textes
            SetText(caloriesText, $"{nutrition.calories} kcal");
            SetText(proteinsText, $"Protéines: {nutrition.proteins}g");
            SetText(carbsText, $"Glucides: {nutrition.carbs}g");
            SetText(fatText, $"Lipides: {nutrition.fat}g");
            SetText(fiberText, $"Fibres: {nutrition.fiber}g");
            SetText(sugarText, $"Sucres: {nutrition.sugar}g");
            SetText(saltText, $"Sel: {nutrition.salt}g");

            // Nutri-Score
            SetText(nutriScoreText, nutrition.nutriscore);
            if (nutriScoreBackground != null)
            {
                nutriScoreBackground.color = GetNutriScoreColor(nutrition.nutriscore);
            }

            // Barres de progression
            SetSliderValue(proteinsSlider, nutrition.proteins, maxProteins);
            SetSliderValue(carbsSlider, nutrition.carbs, maxCarbs);
            SetSliderValue(fatSlider, nutrition.fat, maxFat);
            SetSliderValue(fiberSlider, nutrition.fiber, maxFiber);
        }

        /// <summary>
        /// Définit le texte d'un composant Text
        /// </summary>
        private void SetText(Text textComponent, string value)
        {
            if (textComponent != null)
            {
                textComponent.text = value ?? "";
            }
        }

        /// <summary>
        /// Définit la valeur d'un slider
        /// </summary>
        private void SetSliderValue(Slider slider, float value, float max)
        {
            if (slider != null)
            {
                slider.maxValue = max;
                slider.value = Mathf.Min(value, max);
            }
        }

        /// <summary>
        /// Obtient la couleur du Nutri-Score
        /// </summary>
        private Color GetNutriScoreColor(string score)
        {
            switch (score?.ToUpper())
            {
                case "A": return new Color(0.0f, 0.6f, 0.0f); // Vert foncé
                case "B": return new Color(0.5f, 0.8f, 0.0f); // Vert clair
                case "C": return new Color(1.0f, 0.8f, 0.0f); // Jaune
                case "D": return new Color(1.0f, 0.5f, 0.0f); // Orange
                case "E": return new Color(0.8f, 0.0f, 0.0f); // Rouge
                default: return Color.gray;
            }
        }

        /// <summary>
        /// Affiche ou cache le panel
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (nutriScorePanel != null)
            {
                nutriScorePanel.SetActive(visible);
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }
    }
}
