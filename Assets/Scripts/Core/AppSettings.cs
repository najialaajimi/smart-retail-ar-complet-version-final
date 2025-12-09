using UnityEngine;

namespace SmartRetailAR.Core
{
    /// <summary>
    /// Gère les paramètres de l'application
    /// Persistance avec PlayerPrefs
    /// </summary>
    public class AppSettings : MonoBehaviour
    {
        private static AppSettings instance;
        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AppSettings>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("AppSettings");
                        instance = go.AddComponent<AppSettings>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        // Clés PlayerPrefs
        private const string KEY_SOUND_ENABLED = "SoundEnabled";
        private const string KEY_VIBRATION_ENABLED = "VibrationEnabled";
        private const string KEY_AR_TRACKING_MODE = "ARTrackingMode";
        private const string KEY_SHOW_TUTORIAL = "ShowTutorial";
        private const string KEY_LANGUAGE = "Language";
        private const string KEY_SCAN_FREQUENCY = "ScanFrequency";

        [Header("Paramètres Audio")]
        [SerializeField] private bool soundEnabled = true;
        [SerializeField] private bool vibrationEnabled = true;

        [Header("Paramètres AR")]
        [SerializeField] private ARTrackingMode arTrackingMode = ARTrackingMode.ImageTracking;
        [SerializeField] private float scanFrequency = 2f;

        [Header("Paramètres Interface")]
        [SerializeField] private bool showTutorial = true;
        [SerializeField] private string language = "fr";

        public enum ARTrackingMode
        {
            ImageTracking,
            PlaneDetection,
            Both
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Charge les paramètres depuis PlayerPrefs
        /// </summary>
        public void LoadSettings()
        {
            soundEnabled = PlayerPrefs.GetInt(KEY_SOUND_ENABLED, 1) == 1;
            vibrationEnabled = PlayerPrefs.GetInt(KEY_VIBRATION_ENABLED, 1) == 1;
            arTrackingMode = (ARTrackingMode)PlayerPrefs.GetInt(KEY_AR_TRACKING_MODE, (int)ARTrackingMode.ImageTracking);
            showTutorial = PlayerPrefs.GetInt(KEY_SHOW_TUTORIAL, 1) == 1;
            language = PlayerPrefs.GetString(KEY_LANGUAGE, "fr");
            scanFrequency = PlayerPrefs.GetFloat(KEY_SCAN_FREQUENCY, 2f);

            Debug.Log("Paramètres chargés");
        }

        /// <summary>
        /// Sauvegarde les paramètres dans PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetInt(KEY_SOUND_ENABLED, soundEnabled ? 1 : 0);
            PlayerPrefs.SetInt(KEY_VIBRATION_ENABLED, vibrationEnabled ? 1 : 0);
            PlayerPrefs.SetInt(KEY_AR_TRACKING_MODE, (int)arTrackingMode);
            PlayerPrefs.SetInt(KEY_SHOW_TUTORIAL, showTutorial ? 1 : 0);
            PlayerPrefs.SetString(KEY_LANGUAGE, language);
            PlayerPrefs.SetFloat(KEY_SCAN_FREQUENCY, scanFrequency);
            PlayerPrefs.Save();

            Debug.Log("Paramètres sauvegardés");
        }

        /// <summary>
        /// Réinitialise les paramètres par défaut
        /// </summary>
        public void ResetToDefaults()
        {
            soundEnabled = true;
            vibrationEnabled = true;
            arTrackingMode = ARTrackingMode.ImageTracking;
            showTutorial = true;
            language = "fr";
            scanFrequency = 2f;

            SaveSettings();
            Debug.Log("Paramètres réinitialisés");
        }

        // Getters et Setters
        public bool SoundEnabled
        {
            get => soundEnabled;
            set
            {
                soundEnabled = value;
                SaveSettings();
            }
        }

        public bool VibrationEnabled
        {
            get => vibrationEnabled;
            set
            {
                vibrationEnabled = value;
                SaveSettings();
            }
        }

        public ARTrackingMode TrackingMode
        {
            get => arTrackingMode;
            set
            {
                arTrackingMode = value;
                SaveSettings();
            }
        }

        public bool ShowTutorial
        {
            get => showTutorial;
            set
            {
                showTutorial = value;
                SaveSettings();
            }
        }

        public string Language
        {
            get => language;
            set
            {
                language = value;
                SaveSettings();
            }
        }

        public float ScanFrequency
        {
            get => scanFrequency;
            set
            {
                scanFrequency = Mathf.Clamp(value, 0.5f, 10f);
                SaveSettings();
            }
        }
    }
}
