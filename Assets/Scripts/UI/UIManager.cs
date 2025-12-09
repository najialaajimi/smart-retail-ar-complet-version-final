using UnityEngine;
using SmartRetailAR.Core;
using SmartRetailAR.Utils;

namespace SmartRetailAR.UI
{
    /// <summary>
    /// Gestionnaire principal de l'interface utilisateur
    /// Coordonne tous les panels et gère les transitions
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject scannerPanel;
        [SerializeField] private GameObject productInfoPanel;
        [SerializeField] private GameObject recommendationsPanel;
        [SerializeField] private GameObject settingsPanel;

        [Header("Composants UI")]
        [SerializeField] private ProductInfoPanel productInfo;
        [SerializeField] private RecommendationPanel recommendations;

        [Header("Configuration")]
        [SerializeField] private bool useTransitions = true;
        [SerializeField] private float transitionDuration = 0.3f;

        // État actuel
        private GameObject m_CurrentPanel;

        private void Start()
        {
            // Afficher le menu principal au démarrage
            ShowMainMenu();
        }

        /// <summary>
        /// Affiche le menu principal
        /// </summary>
        public void ShowMainMenu()
        {
            ShowPanel(mainMenuPanel);
            DebugLogger.Log("Menu principal affiché", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche le scanner QR
        /// </summary>
        public void ShowScanner()
        {
            ShowPanel(scannerPanel);
            DebugLogger.Log("Scanner affiché", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche les informations produit
        /// </summary>
        public void ShowProductInfo()
        {
            ShowPanel(productInfoPanel);
            DebugLogger.Log("Informations produit affichées", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche les recommandations
        /// </summary>
        public void ShowRecommendations()
        {
            ShowPanel(recommendationsPanel);
            DebugLogger.Log("Recommandations affichées", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche les paramètres
        /// </summary>
        public void ShowSettings()
        {
            ShowPanel(settingsPanel);
            DebugLogger.Log("Paramètres affichés", LogLevel.Debug);
        }

        /// <summary>
        /// Affiche un panel spécifique
        /// </summary>
        private void ShowPanel(GameObject panel)
        {
            if (panel == null)
            {
                DebugLogger.LogWarning("Panel null, impossible d'afficher");
                return;
            }

            // Cacher le panel actuel
            if (m_CurrentPanel != null && m_CurrentPanel != panel)
            {
                if (useTransitions)
                {
                    StartCoroutine(TransitionPanel(m_CurrentPanel, panel));
                }
                else
                {
                    m_CurrentPanel.SetActive(false);
                    panel.SetActive(true);
                }
            }
            else
            {
                panel.SetActive(true);
            }

            m_CurrentPanel = panel;
        }

        /// <summary>
        /// Coroutine de transition entre panels
        /// </summary>
        private System.Collections.IEnumerator TransitionPanel(GameObject fromPanel, GameObject toPanel)
        {
            // Fade out
            CanvasGroup fromGroup = fromPanel.GetComponent<CanvasGroup>();
            if (fromGroup == null)
            {
                fromGroup = fromPanel.AddComponent<CanvasGroup>();
            }

            float elapsed = 0f;
            while (elapsed < transitionDuration / 2f)
            {
                elapsed += Time.deltaTime;
                fromGroup.alpha = 1f - (elapsed / (transitionDuration / 2f));
                yield return null;
            }

            fromPanel.SetActive(false);
            toPanel.SetActive(true);

            // Fade in
            CanvasGroup toGroup = toPanel.GetComponent<CanvasGroup>();
            if (toGroup == null)
            {
                toGroup = toPanel.AddComponent<CanvasGroup>();
            }

            elapsed = 0f;
            while (elapsed < transitionDuration / 2f)
            {
                elapsed += Time.deltaTime;
                toGroup.alpha = elapsed / (transitionDuration / 2f);
                yield return null;
            }

            toGroup.alpha = 1f;
        }

        /// <summary>
        /// Retour au panel précédent
        /// </summary>
        public void GoBack()
        {
            // Logique de navigation simple
            if (m_CurrentPanel == productInfoPanel || m_CurrentPanel == recommendationsPanel)
            {
                ShowScanner();
            }
            else if (m_CurrentPanel == scannerPanel || m_CurrentPanel == settingsPanel)
            {
                ShowMainMenu();
            }

            DebugLogger.Log("Navigation arrière", LogLevel.Debug);
        }

        /// <summary>
        /// Charge une scène
        /// </summary>
        public void LoadScene(string sceneName)
        {
            SceneLoader.Instance.LoadScene(sceneName);
        }

        /// <summary>
        /// Quitte l'application
        /// </summary>
        public void QuitApplication()
        {
            GameManager.Instance.QuitApplication();
        }
    }
}
