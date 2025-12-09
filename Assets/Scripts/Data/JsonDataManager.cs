using UnityEngine;
using System.IO;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Gestionnaire de données JSON
    /// </summary>
    public class JsonDataManager : MonoBehaviour
    {
        /// <summary>
        /// Charge des données JSON depuis un fichier
        /// </summary>
        public static T LoadFromFile<T>(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"Fichier JSON introuvable: {filePath}");
                    return default(T);
                }

                string jsonData = File.ReadAllText(filePath);
                T data = JsonUtility.FromJson<T>(jsonData);
                Debug.Log($"Données chargées depuis: {filePath}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du chargement JSON: {e.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Sauvegarde des données JSON dans un fichier
        /// </summary>
        public static bool SaveToFile<T>(T data, string filePath, bool prettyPrint = true)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string jsonData = JsonUtility.ToJson(data, prettyPrint);
                File.WriteAllText(filePath, jsonData);
                Debug.Log($"Données sauvegardées dans: {filePath}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors de la sauvegarde JSON: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Charge des données JSON depuis Resources
        /// </summary>
        public static T LoadFromResources<T>(string resourcePath)
        {
            try
            {
                TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
                if (textAsset == null)
                {
                    Debug.LogWarning($"Resource JSON introuvable: {resourcePath}");
                    return default(T);
                }

                T data = JsonUtility.FromJson<T>(textAsset.text);
                Debug.Log($"Données chargées depuis Resources: {resourcePath}");
                return data;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du chargement JSON depuis Resources: {e.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Convertit un objet en chaîne JSON
        /// </summary>
        public static string ToJson<T>(T data, bool prettyPrint = false)
        {
            return JsonUtility.ToJson(data, prettyPrint);
        }

        /// <summary>
        /// Convertit une chaîne JSON en objet
        /// </summary>
        public static T FromJson<T>(string jsonData)
        {
            try
            {
                return JsonUtility.FromJson<T>(jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur lors du parsing JSON: {e.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Vérifie si un fichier JSON est valide
        /// </summary>
        public static bool ValidateJsonFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return false;
                }

                string jsonData = File.ReadAllText(filePath);
                // Tentative de parsing simple
                JsonUtility.FromJson<object>(jsonData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
