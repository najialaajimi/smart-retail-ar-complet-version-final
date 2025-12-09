# Guide de Création des Scènes Unity - Smart Retail AR

Ce document détaille la création et la configuration de chaque scène Unity pour l'application Smart Retail AR.

---

## 📋 Table des Scènes

1. MainMenu.unity - Menu principal
2. QRScanner.unity - Scanner QR
3. ARProductView.unity - Vue AR du produit
4. Recommendations.unity - Recommandations
5. Settings.unity - Paramètres

---

## 🏠 Scene 1: MainMenu.unity

### Description
Menu principal de l'application avec navigation vers les différentes fonctionnalités.

### Hiérarchie d'objets recommandée

```
MainMenu
├── Canvas (Screen Space - Overlay)
│   ├── Background
│   │   └── Image (Fond dégradé ou couleur)
│   │
│   ├── Logo
│   │   └── Image (Logo Smart Retail AR)
│   │
│   ├── Title
│   │   └── Text (ou TextMeshPro) - "Smart Retail AR"
│   │
│   ├── ButtonPanel
│   │   ├── ScanButton
│   │   │   ├── Image (Fond du bouton)
│   │   │   ├── Icon (Icône caméra)
│   │   │   └── Text - "Scanner un produit"
│   │   │
│   │   ├── RecommendationsButton
│   │   │   ├── Image
│   │   │   ├── Icon (Icône étoile)
│   │   │   └── Text - "Mes recommandations"
│   │   │
│   │   ├── SettingsButton
│   │   │   ├── Image
│   │   │   ├── Icon (Icône engrenage)
│   │   │   └── Text - "Paramètres"
│   │   │
│   │   └── QuitButton
│   │       ├── Image
│   │       └── Text - "Quitter"
│   │
│   └── Footer
│       └── Text - "Version 1.0.0"
│
├── EventSystem
│
└── GameManager
    ├── GameManager.cs
    ├── PerformanceMonitor.cs
    └── SceneLoader.cs
```

### Configuration des composants

**Canvas**
- Render Mode: Screen Space - Overlay
- Canvas Scaler: Scale With Screen Size
- Reference Resolution: 1080x1920
- Match: Width or Height = 0.5

**Buttons**
- Navigation: Automatic
- Transition: Color Tint
- Target Graphic: Image du bouton

**GameManager GameObject**
- Ajouter le script `GameManager.cs`
- Assigner l'AppConfig ScriptableObject
- Ajouter `PerformanceMonitor.cs`
- Le SceneLoader sera automatiquement créé

### Scripts à attacher

```csharp
// Sur les boutons
ScanButton.onClick -> SceneLoader.Instance.LoadQRScanner()
RecommendationsButton.onClick -> SceneLoader.Instance.LoadRecommendations()
SettingsButton.onClick -> SceneLoader.Instance.LoadSettings()
QuitButton.onClick -> GameManager.Instance.QuitApplication()
```

---

## 📷 Scene 2: QRScanner.unity

### Description
Scène de scan de QR codes avec affichage de la caméra en temps réel.

### Hiérarchie d'objets recommandée

```
QRScanner
├── Canvas (Screen Space - Overlay)
│   ├── CameraPreview
│   │   └── RawImage (Affichage de la webcam texture)
│   │
│   ├── ScanGuide
│   │   ├── Frame (Image - Cadre de visée)
│   │   └── Instructions
│   │       └── Text - "Pointez vers un QR code produit"
│   │
│   ├── TopBar
│   │   ├── BackButton
│   │   │   ├── Image
│   │   │   └── Text - "←"
│   │   └── Title
│   │       └── Text - "Scanner QR Code"
│   │
│   └── ScanStatus
│       ├── StatusText - "En attente..."
│       └── LoadingIndicator (Animation rotation)
│
├── EventSystem
│
├── QRCodeScanner
│   └── QRCodeScanner.cs
│
├── ProductManager
│   └── ProductManager.cs
│
└── UIManager
    └── UIManager.cs
```

### Configuration des composants

**QRCodeScanner GameObject**
- Script: `QRCodeScanner.cs`
- Scan Interval: 0.5
- Texture Width: 1280
- Texture Height: 720
- Auto Start Scanning: true
- Camera Preview: Référence au RawImage
- Scan Guide UI: Référence au GameObject ScanGuide

**Events à configurer**
```csharp
QRCodeScanner.OnQRCodeDetected += (qrData) => {
    ProductData product = ProductManager.GetProductById(qrData.productId);
    if (product != null) {
        SceneLoader.Instance.LoadScene("ARProductView");
        ProductManager.SetCurrentProduct(product);
    }
}
```

### Simulation en éditeur

En mode Play dans l'éditeur, appuyer sur **Espace** pour simuler un scan de QR code.

---

## 🥽 Scene 3: ARProductView.unity

### Description
Vue en réalité augmentée du produit avec overlay d'informations.

### Hiérarchie d'objets recommandée

```
ARProductView
├── XR Origin (Remplace AR Session Origin)
│   ├── Camera Offset
│   │   └── Main Camera
│   │       ├── Camera.cs
│   │       └── AR Camera Manager.cs
│   │
│   ├── AR Session.cs
│   ├── AR Input Manager.cs
│   └── XR Origin.cs
│
├── AR Managers
│   ├── AR Plane Manager.cs
│   ├── AR Raycast Manager.cs
│   └── AR Tracked Image Manager.cs
│
├── Canvas (World Space)
│   └── ARProductOverlay
│       ├── ARProductOverlay.cs
│       ├── ProductPanel
│       │   ├── ProductName (Text)
│       │   ├── ProductPrice (Text)
│       │   ├── NutriScorePanel
│       │   │   ├── Icon (Image)
│       │   │   └── Text
│       │   └── EcoScorePanel
│       │       ├── Icon (Image)
│       │       └── Text
│       └── InfoButton
│
├── Canvas (Screen Space - Overlay)
│   ├── TopBar
│   │   ├── BackButton
│   │   └── Title - "Vue AR"
│   │
│   └── BottomBar
│       ├── ShowInfoButton - "Voir détails"
│       └── RecommendationsButton - "Alternatives"
│
├── ARSessionManager
│   └── ARSessionManager.cs
│
├── ARPlacementManager
│   ├── ARPlacementManager.cs
│   └── Placement Indicator Prefab
│
└── ProductManager
    └── ProductManager.cs
```

### Configuration AR Foundation

**AR Session**
- Attempt Update: true
- Matching Frame Rate Requested: true

**AR Plane Manager**
- Detection Mode: Horizontal
- Plane Prefab: [Créer un plan simple avec material semi-transparent]

**AR Tracked Image Manager**
- Max Number of Moving Images: 2
- Reference Image Library: [Créer une librairie avec images de produits]

**Canvas World Space**
- Render Mode: World Space
- Event Camera: Main Camera
- Position: (0, 0.3, 0.5) - Devant la caméra
- Rotation: (0, 180, 0)
- Scale: (0.001, 0.001, 0.001)

### Scripts à attacher

```csharp
// Sur ARSessionManager
ARSessionManager arManager = gameObject.AddComponent<ARSessionManager>();
arManager.StartARSession();

// Sur ARProductOverlay
ProductData currentProduct = ProductManager.Instance.CurrentProduct;
ARProductOverlay overlay = GetComponent<ARProductOverlay>();
overlay.ShowProduct(currentProduct, productTransform);
```

---

## ⭐ Scene 4: Recommendations.unity

### Description
Affichage des recommandations de produits alternatifs.

### Hiérarchie d'objets recommandée

```
Recommendations
├── Canvas (Screen Space - Overlay)
│   ├── TopBar
│   │   ├── BackButton
│   │   └── Title - "Recommandations"
│   │
│   ├── CurrentProductPanel
│   │   ├── Title - "Produit actuel"
│   │   ├── ProductCard
│   │   │   ├── ProductImage
│   │   │   ├── ProductName
│   │   │   ├── ProductPrice
│   │   │   └── Scores (Nutri, Eco)
│   │   └── Separator
│   │
│   ├── FilterBar
│   │   ├── BioToggle
│   │   ├── VeganToggle
│   │   ├── PriceSlider
│   │   └── ApplyFiltersButton
│   │
│   ├── RecommendationsScrollView
│   │   ├── Viewport
│   │   │   └── Content
│   │   │       └── [Recommandation Cards - Générées dynamiquement]
│   │   │
│   │   └── Scrollbar Vertical
│   │
│   └── NoRecommendationsPanel (Caché par défaut)
│       ├── Icon
│       └── Text - "Aucune recommandation disponible"
│
├── EventSystem
│
├── ProductManager
│   └── ProductManager.cs
│
├── RecommendationEngine
│   └── ProductRecommendationEngine.cs
│
└── UIManager
    ├── UIManager.cs
    ├── RecommendationPanel.cs
    └── FilterPanel.cs
```

### Configuration des composants

**RecommendationsScrollView**
- Scroll Rect: Horizontal = false, Vertical = true
- Movement Type: Elastic
- Inertia: true
- Scroll Sensitivity: 10

**Content (dans Viewport)**
- Layout: Vertical Layout Group
- Spacing: 10
- Child Force Expand: Width = true

**Recommendation Card Prefab**
Créer un prefab avec:
- Background (Image)
- Product Image (Image)
- Product Name (Text)
- Brand (Text)
- Price (Text)
- Scores (NutriScore, EcoScore)
- Labels (Bio, Vegan)
- Compare Button

### Scripts à attacher

```csharp
// Au démarrage de la scène
void Start() {
    ProductData currentProduct = ProductManager.Instance.CurrentProduct;
    
    // Générer les recommandations
    List<ProductData> recommendations = RecommendationEngine.GetRecommendations(currentProduct);
    
    // Afficher
    RecommendationPanel panel = FindObjectOfType<RecommendationPanel>();
    panel.ShowRecommendations(currentProduct, recommendations);
}

// Sur les filtres
FilterPanel.OnFilterApplied += (filter) => {
    List<ProductData> filtered = ProductManager.FilterProducts(filter);
    UpdateRecommendations(filtered);
}
```

---

## ⚙️ Scene 5: Settings.unity

### Description
Écran de paramètres de l'application.

### Hiérarchie d'objets recommandée

```
Settings
├── Canvas (Screen Space - Overlay)
│   ├── TopBar
│   │   ├── BackButton
│   │   └── Title - "Paramètres"
│   │
│   ├── SettingsScrollView
│   │   └── Content
│   │       ├── ARSection
│   │       │   ├── SectionTitle - "Réalité Augmentée"
│   │       │   ├── EnableARToggle
│   │       │   ├── PlaneDetectionToggle
│   │       │   └── ImageTrackingToggle
│   │       │
│   │       ├── PreferencesSection
│   │       │   ├── SectionTitle - "Préférences"
│   │       │   ├── PreferBioToggle
│   │       │   ├── PreferVeganToggle
│   │       │   ├── PreferLocalToggle
│   │       │   └── MaxPriceSlider
│   │       │
│   │       ├── AllergiesSection
│   │       │   ├── SectionTitle - "Allergies"
│   │       │   ├── LactoseToggle
│   │       │   ├── GlutenToggle
│   │       │   ├── NutsToggle
│   │       │   └── SoyToggle
│   │       │
│   │       ├── PerformanceSection
│   │       │   ├── SectionTitle - "Performance"
│   │       │   ├── ShowFPSToggle
│   │       │   ├── EnableMonitoringToggle
│   │       │   └── OptimizeForMobileToggle
│   │       │
│   │       └── AboutSection
│   │           ├── SectionTitle - "À propos"
│   │           ├── VersionText - "Version 1.0.0"
│   │           ├── UnityVersionText
│   │           └── ResetButton
│   │
│   └── SaveButton
│
├── EventSystem
│
└── UIManager
    └── UIManager.cs
```

### Configuration des composants

**Toggle Groups**
Regrouper les toggles par section pour une meilleure organisation.

**Save Button**
```csharp
SaveButton.onClick -> SaveAllSettings()

void SaveAllSettings() {
    // Sauvegarder dans PlayerPrefs
    PlayerPrefs.SetInt("PreferBio", preferBioToggle.isOn ? 1 : 0);
    PlayerPrefs.SetInt("PreferVegan", preferVeganToggle.isOn ? 1 : 0);
    // ... etc
    PlayerPrefs.Save();
    
    // Afficher confirmation
    ShowNotification("Paramètres sauvegardés");
}
```

---

## 🎨 Thème Visuel Global

### Palette de couleurs

**Couleurs principales:**
- Primary: #4CAF50 (Vert - Bio/Eco)
- Secondary: #2196F3 (Bleu - Technologie)
- Accent: #FF9800 (Orange - Action)
- Background: #FAFAFA (Gris très clair)
- Surface: #FFFFFF (Blanc)
- Error: #F44336 (Rouge)

**Scores:**
- Nutri-Score A: #008000 (Vert foncé)
- Nutri-Score B: #85BB2F (Vert clair)
- Nutri-Score C: #FFAA00 (Jaune)
- Nutri-Score D: #FF8000 (Orange)
- Nutri-Score E: #CC0000 (Rouge)

### Typography

**Recommandations:**
- Headers: Bold, 24-32pt
- Body: Regular, 14-16pt
- Captions: Regular, 12pt
- Buttons: Medium, 16pt

### Animations

**Transitions recommandées:**
- Fade In/Out: 0.3s
- Scale: 0.2s avec ease-out
- Slide: 0.25s

---

## 🔧 Configuration Post-Création

### 1. Build Settings

Ajouter toutes les scènes dans l'ordre:
1. MainMenu
2. QRScanner
3. ARProductView
4. Recommendations
5. Settings

### 2. Tags et Layers

**Tags:**
- MainCamera
- Player
- GameController

**Layers:**
- Default
- UI
- AR

### 3. Quality Settings

**Mobile:**
- Pixel Light Count: 1
- Texture Quality: Medium
- Anisotropic Textures: Per Texture
- Anti Aliasing: 2x
- Shadows: Hard Only
- V Sync: Don't Sync

### 4. Player Settings

**Android:**
- Minimum API Level: 24 (Android 7.0)
- Target API Level: 33
- Scripting Backend: IL2CPP
- Target Architectures: ARM64

**iOS:**
- Target minimum iOS Version: 11.0
- Camera Usage Description: "Nécessaire pour scanner les QR codes"
- Architecture: ARM64

---

## ✅ Checklist de Validation

Avant de construire l'application, vérifier:

- [ ] Toutes les scènes sont créées et fonctionnelles
- [ ] Navigation entre scènes fonctionne
- [ ] GameManager est présent dans toutes les scènes
- [ ] Tous les boutons ont des événements assignés
- [ ] Les références aux scripts sont assignées
- [ ] Les prefabs sont créés et assignés
- [ ] Les images et sprites sont importés
- [ ] AR Foundation est configuré
- [ ] Les paramètres de build sont corrects
- [ ] Le projet compile sans erreur
- [ ] Les tests unitaires passent
- [ ] La performance est acceptable (30+ FPS)

---

## 📝 Notes

- Toutes les scènes doivent avoir un EventSystem
- Le GameManager doit persister entre les scènes (DontDestroyOnLoad)
- Utiliser des pools d'objets pour les listes de recommandations
- Implémenter un système de chargement asynchrone pour les scènes
- Tester sur appareils réels, pas seulement en éditeur

---

**Prochaines étapes:**
1. Créer les scènes dans Unity
2. Configurer les prefabs
3. Assigner tous les scripts
4. Tester chaque scène individuellement
5. Tester le flux complet
6. Optimiser les performances
7. Build et test sur appareil mobile
