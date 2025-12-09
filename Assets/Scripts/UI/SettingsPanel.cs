using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SmartRetailAR.Core;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Panel de paramètres de l'application
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] private Toggle tutorialToggle;
        [SerializeField] private TMP_Dropdown arModeDropdown;
        [SerializeField] private Slider scanFrequencySlider;
        [SerializeField] private TextMeshProUGUI scanFrequencyText;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button closeButton;

        private void Start()
        {
            LoadCurrentSettings();
            SetupListeners();
        }

        /// <summary>
        /// Charge les paramètres actuels
        /// </summary>
        private void LoadCurrentSettings()
        {
            if (soundToggle != null)
            {
                soundToggle.isOn = AppSettings.Instance.SoundEnabled;
            }

            if (vibrationToggle != null)
            {
                vibrationToggle.isOn = AppSettings.Instance.VibrationEnabled;
            }

            if (tutorialToggle != null)
            {
                tutorialToggle.isOn = AppSettings.Instance.ShowTutorial;
            }

            if (arModeDropdown != null)
            {
                arModeDropdown.value = (int)AppSettings.Instance.TrackingMode;
            }

            if (scanFrequencySlider != null)
            {
                scanFrequencySlider.value = AppSettings.Instance.ScanFrequency;
                UpdateScanFrequencyText(AppSettings.Instance.ScanFrequency);
            }
        }

        /// <summary>
        /// Configure les listeners
        /// </summary>
        private void SetupListeners()
        {
            if (scanFrequencySlider != null)
            {
                scanFrequencySlider.onValueChanged.AddListener(UpdateScanFrequencyText);
            }

            if (saveButton != null)
            {
                saveButton.onClick.AddListener(SaveSettings);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(ResetSettings);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Close);
            }
        }

        /// <summary>
        /// Met à jour le texte de la fréquence de scan
        /// </summary>
        private void UpdateScanFrequencyText(float value)
        {
            if (scanFrequencyText != null)
            {
                scanFrequencyText.text = $"{value:F1} Hz";
            }
        }

        /// <summary>
        /// Sauvegarde les paramètres
        /// </summary>
        private void SaveSettings()
        {
            if (soundToggle != null)
            {
                AppSettings.Instance.SoundEnabled = soundToggle.isOn;
            }

            if (vibrationToggle != null)
            {
                AppSettings.Instance.VibrationEnabled = vibrationToggle.isOn;
            }

            if (tutorialToggle != null)
            {
                AppSettings.Instance.ShowTutorial = tutorialToggle.isOn;
            }

            if (arModeDropdown != null)
            {
                AppSettings.Instance.TrackingMode = (AppSettings.ARTrackingMode)arModeDropdown.value;
            }

            if (scanFrequencySlider != null)
            {
                AppSettings.Instance.ScanFrequency = scanFrequencySlider.value;
            }

            AppSettings.Instance.SaveSettings();
            Debug.Log("Paramètres sauvegardés");
        }

        /// <summary>
        /// Réinitialise les paramètres par défaut
        /// </summary>
        private void ResetSettings()
        {
            AppSettings.Instance.ResetToDefaults();
            LoadCurrentSettings();
            Debug.Log("Paramètres réinitialisés");
        }

        /// <summary>
        /// Ferme le panel
        /// </summary>
        private void Close()
        {
            // Retourner au menu principal
            SceneLoader.Instance.LoadMainMenu();
        }

        private void OnDestroy()
        {
            if (scanFrequencySlider != null)
            {
                scanFrequencySlider.onValueChanged.RemoveAllListeners();
            }

            if (saveButton != null)
            {
                saveButton.onClick.RemoveAllListeners();
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveAllListeners();
            }

            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
            }
        }
    }
}
