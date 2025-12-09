# Guide des Scènes - Smart Retail AR

## Table des Matières

1. [MainMenu](#mainmenu)
2. [QRScanner](#qrscanner)
3. [ARProductView](#arproductview)
4. [Recommendations](#recommendations)
5. [Settings](#settings)

## MainMenu

### Description
Menu principal de l'application permettant la navigation vers toutes les fonctionnalités.

### GameObjects Requis

```
MainMenuCanvas (Canvas)
├── MainMenuController (Script)
├── ButtonsPanel
│   ├── ScanButton (Button)
│   ├── ARViewButton (Button)
│   ├── RecommendationsButton (Button)
│   ├── SettingsButton (Button)
│   └── QuitButton (Button)
├── TitleText (TextMeshPro)
└── LogoImage (Image)
```

### Configuration

1. Créer un Canvas
2. Ajouter `MainMenuController`
3. Assigner les boutons dans l'Inspector
4. Configurer les événements onClick

### Scripts Nécessaires
- `MainMenuController.cs`
- `SceneLoader.cs`

---

## QRScanner

### Description
Scène de scan de QR codes pour identifier les produits.

### GameObjects Requis

```
QRScanner (GameObject)
├── QRCodeScanner (Script)
├── Camera (Camera)
├── Canvas
│   ├── CameraFeedRawImage (RawImage)
│   ├── ScanInfoText (TextMeshPro)
│   └── BackButton (Button)
└── ProductDatabase (Script)
```

### Configuration

1. Créer un GameObject vide "QRScanner"
2. Ajouter `QRCodeScanner` script
3. Configurer :
   - Auto Start: true
   - Scan Frequency: 2 Hz
   - Camera Resolution: 1920x1080

### Workflow

1. **Initialisation** : La caméra s'active automatiquement
2. **Scan** : Détection continue des QR codes
3. **Détection** : Événement `onQRCodeScanned` déclenché
4. **Affichage** : Produit trouvé et affiché

### Scripts Nécessaires
- `QRCodeScanner.cs`
- `ProductDatabase.cs`
- `ProductManager.cs`

---

## ARProductView

### Description
Vue AR avec overlay d'informations produit en réalité augmentée.

### GameObjects Requis

```
AR Session (GameObject)
├── ARSessionManager (Script)
├── ARSession (AR Foundation)
├── ARSessionOrigin
│   ├── ARCamera
│   ├── ARPlacementManager (Script)
│   └── ImageTrackingManager (Script)
├── ProductOverlay
│   └── ARProductOverlay (Script)
└── UI Canvas
    ├── ARStatusText (TextMeshPro)
    └── BackButton (Button)
```

### Configuration AR Session

1. Créer "AR Session" GameObject
2. Ajouter composants AR Foundation :
   - AR Session
   - AR Session Origin
   - AR Camera
3. Ajouter scripts personnalisés :
   - ARSessionManager
   - ARPlacementManager
   - ImageTrackingManager

### Configuration Overlay

1. Créer un prefab "ARProductOverlay"
2. Ajouter le script `ARProductOverlay`
3. Configurer les éléments UI :
   - Product Name Text
   - Price Text
   - Nutri-Score Display
   - Eco-Score Display

### Workflow

1. **Initialisation AR** : Vérification compatibilité
2. **Tracking** : Détection de plans ou images
3. **Placement** : Placement automatique de l'overlay
4. **Affichage** : Informations produit affichées

### Scripts Nécessaires
- `ARSessionManager.cs`
- `ARPlacementManager.cs`
- `ARProductOverlay.cs`
- `ImageTrackingManager.cs`

---

## Recommendations

### Description
Affichage des recommandations et alternatives de produits.

### GameObjects Requis

```
RecommendationsCanvas (Canvas)
├── RecommendationUI (Script)
├── TitleText (TextMeshPro)
├── ScrollView
│   └── Content
│       └── (RecommendationCards - générés dynamiquement)
├── FilterPanel
│   ├── FilterPanel (Script)
│   ├── PriceSlider
│   ├── EcoScoreSlider
│   ├── BioToggle
│   └── EcoToggle
└── BackButton (Button)
```

### Configuration

1. Créer un Canvas
2. Ajouter `RecommendationUI`
3. Créer un prefab "RecommendationCard"
4. Assigner le prefab dans l'Inspector
5. Configurer FilterPanel

### Prefab RecommendationCard

```
RecommendationCard (Button)
├── ProductImage (Image)
├── ProductNameText (TextMeshPro)
├── PriceText (TextMeshPro)
├── NutriScoreDisplay
└── EcoScoreDisplay
```

### Workflow

1. **Chargement** : Récupération du produit actuel
2. **Calcul** : Moteur de recommandations trouve des alternatives
3. **Affichage** : Cartes de produits créées dynamiquement
4. **Sélection** : Click sur une carte pour voir les détails

### Scripts Nécessaires
- `RecommendationUI.cs`
- `RecommendationEngine.cs`
- `RecommendationFilter.cs`
- `FilterPanel.cs`

---

## Settings

### Description
Paramètres et configuration de l'application.

### GameObjects Requis

```
SettingsCanvas (Canvas)
├── SettingsPanel (Script)
├── TitleText (TextMeshPro)
├── SettingsScrollView
│   └── Content
│       ├── AudioSection
│       │   ├── SoundToggle
│       │   └── VibrationToggle
│       ├── ARSection
│       │   ├── ARModeDropdown
│       │   └── ScanFrequencySlider
│       ├── GeneralSection
│       │   └── TutorialToggle
│       └── ButtonsSection
│           ├── SaveButton
│           ├── ResetButton
│           └── BackButton
└── AppSettings (Script)
```

### Configuration

1. Créer un Canvas
2. Ajouter `SettingsPanel`
3. Créer les sections de paramètres
4. Assigner tous les contrôles dans l'Inspector

### Paramètres Disponibles

- **Audio**
  - Sound Enabled
  - Vibration Enabled

- **AR**
  - Tracking Mode (Image / Plane / Both)
  - Scan Frequency (0.5 - 10 Hz)

- **Général**
  - Show Tutorial
  - Language (fr/en)

### Workflow

1. **Chargement** : Paramètres chargés depuis PlayerPrefs
2. **Modification** : Utilisateur modifie les valeurs
3. **Sauvegarde** : Click sur "Save" pour persister
4. **Reset** : Restauration des valeurs par défaut

### Scripts Nécessaires
- `SettingsPanel.cs`
- `AppSettings.cs`

---

## Navigation entre Scènes

### Utilisation de SceneLoader

```csharp
// Charger une scène
SceneLoader.Instance.LoadMainMenu();
SceneLoader.Instance.LoadQRScanner();
SceneLoader.Instance.LoadARProductView();
SceneLoader.Instance.LoadRecommendations();
SceneLoader.Instance.LoadSettings();

// Recharger la scène actuelle
SceneLoader.Instance.ReloadCurrentScene();
```

### Événements de Chargement

```csharp
SceneLoader.Instance.onSceneLoadStarted.AddListener((sceneName) => {
    Debug.Log($"Chargement de {sceneName}");
});

SceneLoader.Instance.onSceneLoadCompleted.AddListener((sceneName) => {
    Debug.Log($"Scène {sceneName} chargée");
});

SceneLoader.Instance.onLoadingProgress.AddListener((progress) => {
    Debug.Log($"Progression: {progress * 100}%");
});
```

---

## Conseils de Développement

### Tester une Scène Individuellement

1. Ouvrir la scène dans Unity
2. S'assurer que GameManager existe (DontDestroyOnLoad)
3. Play

### Débogage

Activer les logs détaillés :

```csharp
GameManager.Instance.SetDebugMode(true);
DebugLogger.SetLogLevel(DebugLogger.LogLevel.Verbose);
```

### Optimisation

- Utiliser Object Pooling pour les cartes de recommandations
- Limiter la fréquence de scan QR si performance faible
- Désactiver AR quand non nécessaire

---

Voir aussi : [API_REFERENCE.md](API_REFERENCE.md) pour les détails des scripts.
