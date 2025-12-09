using UnityEngine;
using System.IO;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Gestionnaire de stockage local
    /// </summary>
    public class LocalStorageManager : MonoBehaviour
    {
        private static LocalStorageManager instance;
        public static LocalStorageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("LocalStorageManager");
                    instance = go.AddComponent<LocalStorageManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private string storagePath;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                storagePath = Application.persistentDataPath + "/SmartRetailAR/";
                EnsureStorageDirectory();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// S'assure que le répertoire de stockage existe
        /// </summary>
        private void EnsureStorageDirectory()
        {
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
                Debug.Log($"Répertoire de stockage créé: {storagePath}");
            }
        }

        /// <summary>
        /// Sauvegarde des données
        /// </summary>
        public bool SaveData<T>(string key, T data)
        {
            try
            {
                string filePath = storagePath + key + ".json";
                return JsonDataManager.SaveToFile(data, filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors de la sauvegarde: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge des données
        /// </summary>
        public T LoadData<T>(string key)
        {
            try
            {
                string filePath = storagePath + key + ".json";
                return JsonDataManager.LoadFromFile<T>(filePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du chargement: {e.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Vérifie si des données existent
        /// </summary>
        public bool HasData(string key)
        {
            string filePath = storagePath + key + ".json";
            return File.Exists(filePath);
        }

        /// <summary>
        /// Supprime des données
        /// </summary>
        public bool DeleteData(string key)
        {
            try
            {
                string filePath = storagePath + key + ".json";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"Données supprimées: {key}");
                    return true;
                }
                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors de la suppression: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Efface toutes les données
        /// </summary>
        public void ClearAllData()
        {
            try
            {
                if (Directory.Exists(storagePath))
                {
                    Directory.Delete(storagePath, true);
                    EnsureStorageDirectory();
                    Debug.Log("Toutes les données effacées");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors de l'effacement: {e.Message}");
            }
        }

        /// <summary>
        /// Sauvegarde une chaîne de texte
        /// </summary>
        public bool SaveString(string key, string value)
        {
            try
            {
                string filePath = storagePath + key + ".txt";
                File.WriteAllText(filePath, value);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors de la sauvegarde de la chaîne: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge une chaîne de texte
        /// </summary>
        public string LoadString(string key, string defaultValue = "")
        {
            try
            {
                string filePath = storagePath + key + ".txt";
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
                return defaultValue;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du chargement de la chaîne: {e.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Récupère le chemin de stockage
        /// </summary>
        public string GetStoragePath()
        {
            return storagePath;
        }
    }
}
