using UnityEngine;
using SmartRetailAR.Products;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Charge les données produits depuis différentes sources
    /// </summary>
    public class ProductDataLoader : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private TextAsset jsonDataAsset;
        [SerializeField] private string jsonFilePath = "Data/products";
        [SerializeField] private bool loadOnStart = true;

        private void Start()
        {
            if (loadOnStart)
            {
                LoadProductData();
            }
        }

        /// <summary>
        /// Charge les données produits
        /// </summary>
        public bool LoadProductData()
        {
            // Essayer de charger depuis l'asset assigné
            if (jsonDataAsset != null)
            {
                return LoadFromTextAsset(jsonDataAsset);
            }

            // Essayer de charger depuis Resources
            return LoadFromResources();
        }

        /// <summary>
        /// Charge depuis un TextAsset
        /// </summary>
        private bool LoadFromTextAsset(TextAsset textAsset)
        {
            if (textAsset == null)
            {
                Debug.LogError("TextAsset est null");
                return false;
            }

            Debug.Log("Chargement des produits depuis TextAsset");
            return ProductDatabase.Instance.LoadFromJsonString(textAsset.text);
        }

        /// <summary>
        /// Charge depuis Resources
        /// </summary>
        private bool LoadFromResources()
        {
            Debug.Log($"Chargement des produits depuis Resources: {jsonFilePath}");
            return ProductDatabase.Instance.LoadFromJson();
        }

        /// <summary>
        /// Charge depuis un fichier externe
        /// </summary>
        public bool LoadFromFile(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    Debug.LogError($"Fichier introuvable: {filePath}");
                    return false;
                }

                string jsonData = System.IO.File.ReadAllText(filePath);
                Debug.Log($"Chargement des produits depuis: {filePath}");
                return ProductDatabase.Instance.LoadFromJsonString(jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du chargement du fichier: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge depuis une URL (pour usage futur)
        /// </summary>
        public void LoadFromURL(string url)
        {
            StartCoroutine(LoadFromURLCoroutine(url));
        }

        private System.Collections.IEnumerator LoadFromURLCoroutine(string url)
        {
            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    string jsonData = request.downloadHandler.text;
                    ProductDatabase.Instance.LoadFromJsonString(jsonData);
                    Debug.Log("Produits chargés depuis URL");
                }
                else
                {
                    Debug.LogError($"Erreur chargement URL: {request.error}");
                }
            }
        }
    }
}
