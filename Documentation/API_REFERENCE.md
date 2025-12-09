# Référence API - Smart Retail AR

## Namespaces

- `SmartRetailAR.Core` - Gestionnaires principaux
- `SmartRetailAR.Products` - Modèles de produits
- `SmartRetailAR.QRCode` - Scanner QR
- `SmartRetailAR.AR` - Fonctionnalités AR
- `SmartRetailAR.Recommendations` - Recommandations
- `SmartRetailAR.UI` - Interface utilisateur
- `SmartRetailAR.Data` - Gestion des données
- `SmartRetailAR.Utils` - Utilitaires

---

## Core

### GameManager

**Singleton** - Gestionnaire principal de l'application.

```csharp
GameManager.Instance.IsInitialized(); // bool
GameManager.Instance.SetDebugMode(bool enabled);
GameManager.Instance.QuitApplication();
```

### SceneLoader

**Singleton** - Gestion du chargement de scènes.

```csharp
SceneLoader.Instance.LoadScene(string sceneName);
SceneLoader.Instance.LoadMainMenu();
SceneLoader.Instance.LoadQRScanner();
SceneLoader.Instance.LoadARProductView();
SceneLoader.Instance.LoadRecommendations();
SceneLoader.Instance.LoadSettings();
SceneLoader.Instance.ReloadCurrentScene();
```

**Événements** :
- `onSceneLoadStarted` : `UnityEvent<string>`
- `onSceneLoadCompleted` : `UnityEvent<string>`
- `onLoadingProgress` : `UnityEvent<float>`

### AppSettings

**Singleton** - Paramètres persistants.

```csharp
AppSettings.Instance.SoundEnabled; // bool get/set
AppSettings.Instance.VibrationEnabled; // bool get/set
AppSettings.Instance.TrackingMode; // ARTrackingMode get/set
AppSettings.Instance.ScanFrequency; // float get/set
AppSettings.Instance.SaveSettings();
AppSettings.Instance.LoadSettings();
AppSettings.Instance.ResetToDefaults();
```

---

## Products

### Product

**Classe de données** représentant un produit.

```csharp
public class Product
{
    public string id;
    public string name;
    public string brand;
    public string description;
    public float price;
    public string origin;
    public string category;
    public NutritionInfo nutrition;
    public string[] alternatives;
    public float ecoScore;
    public bool isBio;
    public bool isEcoResponsible;
    public string imageUrl;
    public string qrCodeData;
    
    public bool MatchesFilter(float maxPrice, float minEcoScore, 
                              bool requireBio, bool requireEcoResponsible);
}
```

### ProductDatabase

**Singleton** - Base de données de produits.

```csharp
ProductDatabase.Instance.LoadFromJson(); // bool
ProductDatabase.Instance.LoadFromJsonString(string json); // bool
ProductDatabase.Instance.GetProductById(string id); // Product
ProductDatabase.Instance.GetProductByQRCode(string qrData); // Product
ProductDatabase.Instance.SearchProducts(string searchTerm); // List<Product>
ProductDatabase.Instance.GetAllProducts(); // List<Product>
ProductDatabase.Instance.FilterProducts(float maxPrice, float minEcoScore, 
                                         bool requireBio, bool requireEcoResponsible); // List<Product>
```

### ProductManager

**Singleton** - Gestion du produit actuel.

```csharp
ProductManager.Instance.SelectProduct(Product product);
ProductManager.Instance.SelectProductById(string productId);
ProductManager.Instance.DeselectProduct();
ProductManager.Instance.GetCurrentProduct(); // Product
ProductManager.Instance.HasSelectedProduct(); // bool
```

**Événements** :
- `onProductSelected` : `UnityEvent<Product>`
- `onProductDeselected` : `UnityEvent`

---

## QRCode

### QRCodeScanner

Scanner de QR codes.

```csharp
scanner.StartScanning();
scanner.StopScanning();
scanner.IsScanning(); // bool
scanner.SimulateScan(string qrData); // Pour les tests
scanner.GetCameraTexture(); // WebCamTexture
```

**Événements** :
- `onQRCodeScanned` : `UnityEvent<string>`
- `onScanStarted` : `UnityEvent`
- `onScanStopped` : `UnityEvent`
- `onScanError` : `UnityEvent<string>`

### QRCodeGenerator

Générateur de QR codes.

```csharp
generator.GenerateQRCode(string data); // Texture2D
generator.GenerateProductQRCode(string productId); // Texture2D
generator.SaveQRCodeToFile(Texture2D texture, string filePath); // bool
QRCodeGenerator.ValidateQRCodeData(string qrData); // static bool
QRCodeGenerator.ExtractProductId(string qrData); // static string
```

---

## AR

### ARSessionManager

**Singleton** - Gestion de la session AR.

```csharp
ARSessionManager.Instance.InitializeARSession();
ARSessionManager.Instance.ResetARSession();
ARSessionManager.Instance.SetAREnabled(bool enabled);
ARSessionManager.Instance.IsARSupported(); // bool
ARSessionManager.Instance.IsInitialized(); // bool
ARSessionManager.Instance.IsTracking(); // bool
```

**Événements** :
- `onARSessionInitialized` : `UnityEvent`
- `onARSessionFailed` : `UnityEvent`
- `onARSessionStateChanged` : `UnityEvent<ARSessionState>`

### ARProductOverlay

Affichage AR des informations produit.

```csharp
overlay.ShowProduct(Product product);
overlay.Show();
overlay.Hide();
overlay.SetPosition(Vector3 position);
overlay.IsVisible(); // bool
overlay.GetCurrentProduct(); // Product
```

### ARPlacementManager

Placement d'objets en AR.

```csharp
manager.PlaceObject(); // GameObject
manager.PlaceObject(GameObject prefab); // GameObject
manager.RemoveCurrentObject();
manager.SetPlacementPrefab(GameObject prefab);
manager.IsPlacementValid(); // bool
manager.GetPlacementPosition(); // Vector3
```

---

## Recommendations

### RecommendationEngine

**Singleton** - Moteur de recommandations.

```csharp
RecommendationEngine.Instance.GetAlternatives(Product product, 
    RecommendationCriteria criteria); // List<Product>
RecommendationEngine.Instance.GetEcoFriendlyAlternatives(Product product); // List<Product>
RecommendationEngine.Instance.GetSimilarProducts(Product product, 
    float minPrice, float maxPrice); // List<Product>
RecommendationEngine.Instance.SetMaxRecommendations(int max);
```

**Critères disponibles** :
- `RecommendationCriteria.Price`
- `RecommendationCriteria.EcoScore`
- `RecommendationCriteria.NutriScore`
- `RecommendationCriteria.Bio`
- `RecommendationCriteria.Origin`
- `RecommendationCriteria.Category`
- `RecommendationCriteria.Combined` (défaut)

### RecommendationFilter

Filtrage de produits.

```csharp
filter.Apply(List<Product> products); // List<Product>
filter.Reset();
filter.EnableFilter(FilterType filterType, bool enable);
filter.GetActiveFilterCount(); // int
filter.HasActiveFilters(); // bool
filter.Clone(); // RecommendationFilter
```

---

## Utils

### DebugLogger

**Singleton** - Logger personnalisé.

```csharp
DebugLogger.LogInfo(string message, Object context);
DebugLogger.LogWarning(string message, Object context);
DebugLogger.LogError(string message, Object context);
DebugLogger.LogVerbose(string message, Object context);
DebugLogger.Instance.SetLogLevel(LogLevel level);
DebugLogger.Instance.SetLogToFile(bool enabled);
```

### PerformanceMonitor

**Singleton** - Monitoring de performance.

```csharp
PerformanceMonitor.Instance.StartTimer(string operationName);
PerformanceMonitor.Instance.StopTimer(string operationName); // float
PerformanceMonitor.Instance.RecordMetric(string metricName, float value);
PerformanceMonitor.Instance.RecordSuccessfulScan();
PerformanceMonitor.Instance.RecordFailedScan();
PerformanceMonitor.Instance.GetCurrentFPS(); // float
PerformanceMonitor.Instance.GetRecognitionRate(); // float
PerformanceMonitor.Instance.GeneratePerformanceReport(); // string
```

---

## Exemples d'Utilisation

### Scanner un QR et afficher le produit

```csharp
// Dans le scanner
scanner.onQRCodeScanned.AddListener((qrData) => {
    string productId = QRCodeGenerator.ExtractProductId(qrData);
    Product product = ProductDatabase.Instance.GetProductById(productId);
    
    if (product != null) {
        ProductManager.Instance.SelectProduct(product);
        SceneLoader.Instance.LoadARProductView();
    }
});
```

### Afficher les recommandations

```csharp
// Dans la scène de recommandations
Product currentProduct = ProductManager.Instance.GetCurrentProduct();
if (currentProduct != null) {
    List<Product> alternatives = 
        RecommendationEngine.Instance.GetAlternatives(
            currentProduct, 
            RecommendationCriteria.EcoScore
        );
    recommendationUI.ShowRecommendations(currentProduct);
}
```

### Mesurer les performances

```csharp
// Démarrer un timer
PerformanceMonitor.Instance.StartTimer("ProductLoad");

// ... opération ...

// Arrêter et enregistrer
float elapsed = PerformanceMonitor.Instance.StopTimer("ProductLoad");
Debug.Log($"Temps de chargement: {elapsed}s");

// Générer un rapport
string report = PerformanceMonitor.Instance.GeneratePerformanceReport();
Debug.Log(report);
```

---

Pour plus d'exemples, consulter les tests dans `Assets/Tests/`.
