using UnityEngine;
using UnityEngine.UI;
using SmartRetailAR.Core;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Contrôleur du menu principal
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button scanButton;
        [SerializeField] private Button arViewButton;
        [SerializeField] private Button recommendationsButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            SetupButtonListeners();
        }

        /// <summary>
        /// Configure les listeners des boutons
        /// </summary>
        private void SetupButtonListeners()
        {
            if (scanButton != null)
            {
                scanButton.onClick.AddListener(OnScanButtonClicked);
            }

            if (arViewButton != null)
            {
                arViewButton.onClick.AddListener(OnARViewButtonClicked);
            }

            if (recommendationsButton != null)
            {
                recommendationsButton.onClick.AddListener(OnRecommendationsButtonClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        /// <summary>
        /// Ouvre le scanner QR
        /// </summary>
        private void OnScanButtonClicked()
        {
            Debug.Log("Navigation vers le scanner QR");
            SceneLoader.Instance.LoadQRScanner();
        }

        /// <summary>
        /// Ouvre la vue AR
        /// </summary>
        private void OnARViewButtonClicked()
        {
            Debug.Log("Navigation vers la vue AR");
            SceneLoader.Instance.LoadARProductView();
        }

        /// <summary>
        /// Ouvre les recommandations
        /// </summary>
        private void OnRecommendationsButtonClicked()
        {
            Debug.Log("Navigation vers les recommandations");
            SceneLoader.Instance.LoadRecommendations();
        }

        /// <summary>
        /// Ouvre les paramètres
        /// </summary>
        private void OnSettingsButtonClicked()
        {
            Debug.Log("Navigation vers les paramètres");
            SceneLoader.Instance.LoadSettings();
        }

        /// <summary>
        /// Quitte l'application
        /// </summary>
        private void OnQuitButtonClicked()
        {
            Debug.Log("Fermeture de l'application");
            GameManager.Instance.QuitApplication();
        }

        private void OnDestroy()
        {
            if (scanButton != null) scanButton.onClick.RemoveAllListeners();
            if (arViewButton != null) arViewButton.onClick.RemoveAllListeners();
            if (recommendationsButton != null) recommendationsButton.onClick.RemoveAllListeners();
            if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
            if (quitButton != null) quitButton.onClick.RemoveAllListeners();
        }
    }
}
