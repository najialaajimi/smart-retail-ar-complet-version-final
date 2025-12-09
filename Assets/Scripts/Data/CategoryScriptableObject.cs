using UnityEngine;

namespace SmartRetailAR.Data
{
    /// <summary>
    /// Données d'une catégorie de produits
    /// </summary>
    [System.Serializable]
    public class CategoryData
    {
        public string id;
        public string name;
        public string description;
        public string icon;
        public string color;
    }

    /// <summary>
    /// Wrapper pour la désérialisation JSON des catégories
    /// </summary>
    [System.Serializable]
    public class CategoryDataList
    {
        public System.Collections.Generic.List<CategoryData> categories;
    }

    /// <summary>
    /// ScriptableObject pour stocker les catégories de produits
    /// </summary>
    [CreateAssetMenu(fileName = "NewCategory", menuName = "Smart Retail AR/Category", order = 2)]
    public class CategoryScriptableObject : ScriptableObject
    {
        [Header("Informations de la catégorie")]
        public string categoryId;
        public string categoryName;
        [TextArea(2, 4)]
        public string description;

        [Header("Visuels")]
        public Sprite icon;
        public Color color = Color.white;

        /// <summary>
        /// Convertit le ScriptableObject en CategoryData
        /// </summary>
        public CategoryData ToCategoryData()
        {
            CategoryData data = new CategoryData
            {
                id = categoryId,
                name = categoryName,
                description = description,
                icon = icon != null ? icon.name : "",
                color = "#" + ColorUtility.ToHtmlStringRGB(color)
            };

            return data;
        }

        /// <summary>
        /// Remplit le ScriptableObject depuis un CategoryData
        /// </summary>
        public void FromCategoryData(CategoryData data)
        {
            categoryId = data.id;
            categoryName = data.name;
            description = data.description;
            
            if (ColorUtility.TryParseHtmlString(data.color, out Color parsedColor))
            {
                color = parsedColor;
            }
        }
    }
}
