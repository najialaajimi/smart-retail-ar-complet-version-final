# Guide d'Exécution - Smart Retail AR

## 📋 Table des Matières
1. [Prérequis](#prérequis)
2. [Configuration Initiale](#configuration-initiale)
3. [Exécution dans l'Éditeur Unity](#exécution-dans-léditeur-unity)
4. [Guide des Scènes](#guide-des-scènes)
5. [Tests et Validation](#tests-et-validation)
6. [Déploiement sur Appareil](#déploiement-sur-appareil)
7. [Workflows Complets](#workflows-complets)
8. [Outils de Développement](#outils-de-développement)
9. [Dépannage](#dépannage)

---

## Prérequis

### Logiciels Requis
- **Unity 2022.3.62f3** (version exacte requise)
- **Unity Hub** (dernière version)
- Pour Android : **Android SDK** (API Level 24+)
- Pour iOS : **Xcode 12+** (macOS uniquement)

### Packages Unity Requis
Tous les packages sont déjà configurés dans `Packages/manifest.json` :
- AR Foundation 5.1.0
- ARCore XR Plugin 5.1.0 (Android)
- ARKit XR Plugin 5.1.0 (iOS)
- XR Core Utils 2.2.0
- TextMeshPro 3.0.6
- Input System 1.5.1
- Test Framework 1.3.4

### Matériel Requis
- **Pour le développement** : PC/Mac capable de faire tourner Unity
- **Pour les tests AR** : 
  - Smartphone Android avec support ARCore
  - iPhone/iPad avec iOS 11+ (support ARKit)

---

## Configuration Initiale

### 1. Ouverture du Projet

```bash
# Cloner le repository
git clone https://github.com/najialaajimi/smart-retail-ar-complet-version-final.git
cd smart-retail-ar-complet-version-final
```

### 2. Ouvrir avec Unity Hub
1. Lancez Unity Hub
2. Cliquez sur "Add" → "Add project from disk"
3. Sélectionnez le dossier du projet
4. Vérifiez que Unity 2022.3.62f3 est installé
5. Ouvrez le projet

### 3. Première Configuration
Lors de la première ouverture, Unity va :
- Importer tous les packages
- Compiler les scripts C#
- Générer les fichiers de projet
- **Temps estimé : 5-10 minutes**

### 4. Vérification de la Configuration
Une fois le projet ouvert :
1. Vérifiez qu'il n'y a pas d'erreurs dans la Console
2. Allez dans `Window > Package Manager` pour vérifier les packages
3. Testez l'ouverture d'une scène : `Assets/Scenes/MainMenu.unity`

---

## Exécution dans l'Éditeur Unity

### Mode Éditeur (Testing sans AR)

#### Lancement Rapide
1. Ouvrez la scène `Assets/Scenes/MainMenu.unity`
2. Cliquez sur le bouton **Play ▶** en haut de l'éditeur
3. La scène du menu principal s'affiche

#### Navigation entre les Scènes
Dans l'éditeur, vous pouvez tester chaque scène individuellement :

**Scène par Scène :**
```
MainMenu.unity → Point d'entrée de l'application
QRScanner.unity → Simulation du scan QR (sans caméra réelle)
ARProductView.unity → Aperçu AR (limité sans appareil)
Recommendations.unity → Affichage des recommandations
Settings.unity → Configuration de l'application
```

#### Mode Simulation AR
Unity ne peut pas simuler complètement l'AR sans appareil, mais vous pouvez :
1. Tester les scripts et la logique
2. Vérifier les UI
3. Déboguer les interactions

---

## Guide des Scènes

### 🏠 Scène 1 : MainMenu.unity

**Objectif :** Menu principal de navigation

**Comment exécuter :**
1. Ouvrez `Assets/Scenes/MainMenu.unity`
2. Appuyez sur **Play ▶**
3. Le menu principal apparaît

**Composants Requis :**
- `GameObject` : MainMenuCanvas
  - Composant : `MainMenuController.cs`
- `GameObject` : Camera
- `GameObject` : GameManager (avec `GameManager.cs`)

**Actions Disponibles :**
- Bouton "Scanner Produit" → Charge QRScanner.unity
- Bouton "Voir Recommandations" → Charge Recommendations.unity
- Bouton "Paramètres" → Charge Settings.unity
- Bouton "Quitter" → Ferme l'application

**Test dans l'Éditeur :**
```csharp
// Dans MainMenuController.cs, les boutons appellent :
SceneLoader.Instance.LoadScene("QRScanner");
SceneLoader.Instance.LoadScene("Recommendations");
SceneLoader.Instance.LoadScene("Settings");
```

---

### 📷 Scène 2 : QRScanner.unity

**Objectif :** Scanner des QR codes produits

**Comment exécuter :**
1. Ouvrez `Assets/Scenes/QRScanner.unity`
2. Appuyez sur **Play ▶**
3. La caméra démarre (simulation dans l'éditeur)

**Composants Requis :**
- `GameObject` : QRScannerCanvas
  - Composant : `QRCodeScanner.cs`
- `GameObject` : Camera (WebCamTexture)
- `GameObject` : UIPanel (affichage du scan)

**Test Sans Caméra (Éditeur) :**
Utilisez la fonction de simulation :
```csharp
// Dans l'Inspector, sur le composant QRCodeScanner
// Ou via script de test :
QRCodeScanner scanner = FindObjectOfType<QRCodeScanner>();
scanner.SimulateScan("SMARTRETAIL:PROD001");
```

**Formats QR Supportés :**
- `SMARTRETAIL:PROD001` → Produit ID PROD001
- `SMARTRETAIL:PROD002` → Produit ID PROD002
- etc.

**Actions Après Scan :**
- Le scanner détecte le QR
- Charge le produit depuis `ProductDatabase`
- Transition automatique vers `ARProductView.unity`

**Test Avec Appareil Réel :**
1. Générez des QR codes via `Tools > Smart Retail AR > QR Code Generator`
2. Imprimez ou affichez les QR codes
3. Lancez l'app sur smartphone
4. Pointez la caméra vers le QR code

---

### 🔍 Scène 3 : ARProductView.unity

**Objectif :** Affichage AR des informations produit

**Comment exécuter :**
1. Ouvrez `Assets/Scenes/ARProductView.unity`
2. Appuyez sur **Play ▶**
3. **Note :** L'AR ne fonctionne pas dans l'éditeur, utilisez un appareil

**Composants Requis :**
- `GameObject` : AR Session
  - Composant : `ARSession.cs`
  - Composant : `ARSessionManager.cs`
- `GameObject` : XR Origin
  - Composant : `XROrigin.cs`
  - Enfant : AR Camera
  - Composant : `ARPlacementManager.cs`
- `GameObject` : ARProductOverlay (Prefab)
  - Composant : `ARProductOverlay.cs`
- `GameObject` : AR Plane Manager
- `GameObject` : AR Tracked Image Manager
  - Composant : `ImageTrackingManager.cs`

**Workflow AR :**
1. L'app initialise la session AR
2. Détecte les plans horizontaux (tables, sol)
3. L'utilisateur tape sur un plan pour placer l'overlay
4. Les informations produit s'affichent en 3D

**Informations Affichées :**
- Nom du produit
- Prix
- Nutri-Score (A-E) avec code couleur
- Éco-Score (0-100) avec barre de progression
- Origine
- Bouton "Voir Alternatives"

**Test sur Appareil :**
1. Buildez pour Android/iOS
2. Installez sur l'appareil
3. Autorisez l'accès caméra
4. Pointez vers une surface plane
5. Tapez pour placer l'overlay

---

### 💡 Scène 4 : Recommendations.unity

**Objectif :** Afficher les recommandations produits

**Comment exécuter :**
1. Ouvrez `Assets/Scenes/Recommendations.unity`
2. Appuyez sur **Play ▶**
3. Les recommandations apparaissent

**Composants Requis :**
- `GameObject` : RecommendationsCanvas
  - Composant : `RecommendationUI.cs`
- `GameObject` : ScrollView (liste de produits)
- `GameObject` : FilterPanel
  - Composant : `FilterPanel.cs`
- `GameObject` : RecommendationEngine
  - Composant : `RecommendationEngine.cs`

**Fonctionnalités :**
- Affichage des alternatives au produit actuel
- Filtres multi-critères :
  - Prix (slider)
  - Éco-Score (slider)
  - Bio (toggle)
  - Origine (dropdown)
  - Nutri-Score (A-E)

**Algorithme de Recommandation :**
```
Score Final = 0.3 × Éco-Score + 0.3 × Nutri-Score + 0.2 × Score Prix + 0.2 × Bonus Bio
```

**Test dans l'Éditeur :**
```csharp
// Simuler un produit actuel
ProductManager.Instance.SetCurrentProduct(productId);

// Obtenir les recommandations
List<Product> recommendations = RecommendationEngine.Instance.GetAlternatives(
    currentProduct, 
    maxResults: 5
);
```

---

### ⚙️ Scène 5 : Settings.unity

**Objectif :** Configuration de l'application

**Comment exécuter :**
1. Ouvrez `Assets/Scenes/Settings.unity`
2. Appuyez sur **Play ▶**
3. Les paramètres sont affichés

**Composants Requis :**
- `GameObject` : SettingsCanvas
  - Composant : `SettingsPanel.cs`
- `GameObject` : AppSettings
  - Composant : `AppSettings.cs`

**Paramètres Disponibles :**
- **Mode AR** : ARCore / ARKit / Simulation
- **Fréquence de Scan** : Continue / On Demand
- **Qualité Vidéo** : Basse / Moyenne / Haute
- **Son** : Activé / Désactivé
- **Notifications** : Activé / Désactivé
- **Langue** : Français / English

**Persistance :**
Les paramètres sont sauvegardés via `PlayerPrefs` :
```csharp
AppSettings.Instance.SetARMode(ARMode.ARCore);
AppSettings.Instance.SetScanFrequency(ScanFrequency.Continuous);
AppSettings.Instance.SaveSettings();
```

---

## Tests et Validation

### Tests Unitaires (EditMode)

**Exécuter les tests EditMode :**
1. Ouvrez `Window > General > Test Runner`
2. Onglet **EditMode**
3. Cliquez sur "Run All"

**Tests Disponibles :**
- `ProductDatabaseTests.cs` : Test de la base de données
  - Chargement JSON
  - Recherche de produits
  - Filtres
- `RecommendationEngineTests.cs` : Test du moteur de recommandations
  - Calcul de scores
  - Filtrage multi-critères
  - Tri des résultats

**Commande CLI :**
```bash
# Depuis la ligne de commande
Unity -runTests -batchmode -projectPath . -testPlatform EditMode -testResults results.xml
```

---

### Tests d'Intégration (PlayMode)

**Exécuter les tests PlayMode :**
1. Ouvrez `Window > General > Test Runner`
2. Onglet **PlayMode**
3. Cliquez sur "Run All"

**Tests Disponibles :**
- `QRScannerTests.cs` : Test du scanner QR
  - Initialisation caméra
  - Simulation de scan
  - Parsing des données
- `AROverlayTests.cs` : Test de l'overlay AR
  - Affichage produit
  - Visibilité des éléments
  - Interaction utilisateur

**Commande CLI :**
```bash
Unity -runTests -batchmode -projectPath . -testPlatform PlayMode -testResults results.xml
```

---

### Validation Manuelle

**Checklist de Validation :**

✅ **Navigation**
- [ ] Le menu principal s'affiche correctement
- [ ] Tous les boutons sont cliquables
- [ ] Les transitions de scènes fonctionnent
- [ ] Le bouton retour fonctionne

✅ **Scanner QR**
- [ ] La caméra s'initialise
- [ ] Le scan détecte les QR codes
- [ ] Les données sont parsées correctement
- [ ] La transition vers AR fonctionne

✅ **Vue AR**
- [ ] La session AR s'initialise
- [ ] Les plans sont détectés
- [ ] L'overlay est placé correctement
- [ ] Les informations sont lisibles

✅ **Recommandations**
- [ ] Les alternatives s'affichent
- [ ] Les filtres fonctionnent
- [ ] Le tri est correct
- [ ] Les cartes sont cliquables

✅ **Paramètres**
- [ ] Les paramètres se chargent
- [ ] Les modifications sont sauvegardées
- [ ] Les valeurs persistent après redémarrage

---

## Déploiement sur Appareil

### Build Android (ARCore)

**1. Configuration du Build**
1. `File > Build Settings`
2. Sélectionnez **Android**
3. Cliquez sur "Switch Platform"

**2. Player Settings**
1. `Edit > Project Settings > Player`
2. **Company Name** : Votre nom
3. **Product Name** : Smart Retail AR
4. **Package Name** : `com.smartretail.ar`
5. **Minimum API Level** : Android 7.0 (API 24)
6. **Target API Level** : Automatic (highest installed)

**3. XR Settings**
1. `Edit > Project Settings > XR Plug-in Management`
2. Onglet **Android**
3. Cochez **ARCore**

**4. Build**
1. `File > Build Settings`
2. Ajoutez toutes les scènes dans l'ordre :
   - MainMenu
   - QRScanner
   - ARProductView
   - Recommendations
   - Settings
3. Cliquez sur "Build And Run"
4. Nommez le fichier : `SmartRetailAR.apk`

**5. Installation**
- Via câble USB : L'app s'installe automatiquement
- Ou partagez l'APK

---

### Build iOS (ARKit)

**1. Configuration du Build**
1. `File > Build Settings`
2. Sélectionnez **iOS**
3. Cliquez sur "Switch Platform"

**2. Player Settings**
1. `Edit > Project Settings > Player`
2. **Company Name** : Votre nom
3. **Product Name** : Smart Retail AR
4. **Bundle Identifier** : `com.smartretail.ar`
5. **Minimum iOS Version** : 11.0
6. **Target SDK** : Device SDK
7. **Architecture** : ARM64

**3. XR Settings**
1. `Edit > Project Settings > XR Plug-in Management`
2. Onglet **iOS**
3. Cochez **ARKit**

**4. Permissions**
Ajoutez dans `Info.plist` :
```xml
<key>NSCameraUsageDescription</key>
<string>L'app a besoin de la caméra pour scanner les produits et afficher l'AR</string>
```

**5. Build**
1. `File > Build Settings`
2. Ajoutez toutes les scènes
3. Cliquez sur "Build"
4. Sélectionnez un dossier (ex: `Builds/iOS`)
5. Unity génère un projet Xcode

**6. Compilation Xcode**
1. Ouvrez le projet `.xcodeproj` dans Xcode
2. Sélectionnez votre équipe de développement
3. Connectez votre iPhone/iPad
4. Cliquez sur "Build and Run" ▶

---

## Workflows Complets

### Workflow 1 : Scan Produit → Vue AR → Recommandations

**Étapes :**
1. **Démarrage** : Lancement de l'app → MainMenu.unity
2. **Navigation** : Clic sur "Scanner Produit" → QRScanner.unity
3. **Scan** : Pointer la caméra vers un QR code
4. **Détection** : Le scanner lit `SMARTRETAIL:PROD001`
5. **Transition** : Chargement automatique → ARProductView.unity
6. **Initialisation AR** : La session AR démarre
7. **Placement** : Détection d'un plan, l'utilisateur tape dessus
8. **Affichage** : L'overlay AR apparaît avec les infos produit
9. **Action** : Clic sur "Voir Alternatives"
10. **Recommandations** : Transition → Recommendations.unity
11. **Filtrage** : Application de filtres (Bio, Éco-Score > 80)
12. **Sélection** : Clic sur un produit alternatif
13. **Retour AR** : Le nouveau produit s'affiche en AR

---

### Workflow 2 : Configuration et Test

**Étapes :**
1. **Démarrage** : Lancement → MainMenu.unity
2. **Paramètres** : Clic sur "Paramètres" → Settings.unity
3. **Configuration** :
   - Mode AR : ARCore
   - Fréquence Scan : Continue
   - Qualité : Haute
4. **Sauvegarde** : Clic sur "Enregistrer"
5. **Retour** : Clic sur "Retour" → MainMenu.unity
6. **Test** : Lancement du scan pour tester les nouveaux paramètres

---

### Workflow 3 : Test de Performance

**Étapes :**
1. Activez le `PerformanceMonitor` dans la scène
2. Lancez l'app
3. Effectuez un scan de produit
4. Vérifiez les métriques :
   - Latence de scan : ≤ 1 seconde
   - Taux de reconnaissance : ≥ 95%
   - FPS en AR : ≥ 30
5. Consultez les logs via `DebugLogger`

**Accès aux Métriques :**
```csharp
PerformanceMonitor.Instance.StartTimer("ScanLatency");
// ... opération ...
float latency = PerformanceMonitor.Instance.StopTimer("ScanLatency");
Debug.Log($"Latence: {latency}s");
```

---

## Outils de Développement

### 1. Générateur de QR Codes

**Accès :** `Tools > Smart Retail AR > QR Code Generator`

**Utilisation :**
1. Ouvrez la fenêtre
2. Sélectionnez un produit dans la liste
3. Cliquez sur "Generate QR Code"
4. Le QR code est sauvegardé dans `Assets/Resources/QRCodes/`

**Génération en Batch :**
- Cliquez sur "Generate All" pour créer tous les QR codes

---

### 2. Éditeur de Base de Données Produits

**Accès :** Sélectionnez `Assets/Data/products.json` dans l'Inspector

**Utilisation :**
1. Cliquez sur "Add Product"
2. Remplissez les champs
3. Cliquez sur "Save"
4. Le fichier JSON est mis à jour

---

### 3. Scene Setup Wizard

**Accès :** `Tools > Smart Retail AR > Scene Setup Wizard`

**Utilisation :**
1. Choisissez un type de scène
2. Cliquez sur "Create Scene"
3. La scène est créée avec tous les GameObjects nécessaires

---

### 4. Auto Runner de Scènes

**Utilisation :**
```csharp
// Ajouter SceneAutoRunner à un GameObject
SceneAutoRunner runner = gameObject.AddComponent<SceneAutoRunner>();
runner.RunAllScenes();
```

**Validation :**
- Charge chaque scène
- Vérifie les composants requis
- Log les erreurs éventuelles

---

## Dépannage

### Problème : "No scenes in build"

**Solution :**
1. `File > Build Settings`
2. Cliquez sur "Add Open Scenes" pour chaque scène
3. Ou glissez-déposez les scènes depuis le Project

---

### Problème : "AR session failed to initialize"

**Causes possibles :**
- Appareil non compatible ARCore/ARKit
- Permissions caméra refusées
- Google Play Services for AR non installé (Android)

**Solution :**
1. Vérifiez la compatibilité de l'appareil
2. Accordez les permissions caméra
3. Android : Installez ARCore depuis le Play Store

---

### Problème : "QR code not detected"

**Causes possibles :**
- QR code trop petit/trop grand
- Mauvais éclairage
- QR code flou

**Solution :**
1. Assurez un bon éclairage
2. Gardez le QR code stable
3. Distance optimale : 20-30 cm

---

### Problème : "Product not found"

**Causes possibles :**
- Fichier `products.json` manquant
- ID produit invalide dans le QR code
- Erreur de parsing JSON

**Solution :**
1. Vérifiez que `Assets/Data/products.json` existe
2. Validez le format JSON
3. Vérifiez l'ID dans le QR code : `SMARTRETAIL:PROD001`

---

### Problème : "Compilation errors"

**Solution :**
1. `Assets > Reimport All`
2. `Edit > Preferences > External Tools` → Regenerate project files
3. Redémarrez Unity

---

## Annexe : Commandes Utiles

### Tests
```bash
# Tous les tests EditMode
Unity -runTests -batchmode -projectPath . -testPlatform EditMode

# Tous les tests PlayMode
Unity -runTests -batchmode -projectPath . -testPlatform PlayMode

# Tests spécifiques
Unity -runTests -batchmode -projectPath . -testPlatform EditMode -testFilter ProductDatabaseTests
```

### Build
```bash
# Build Android
Unity -quit -batchmode -projectPath . -buildTarget Android -executeMethod BuildScript.BuildAndroid

# Build iOS
Unity -quit -batchmode -projectPath . -buildTarget iOS -executeMethod BuildScript.BuildiOS
```

---

## Support

Pour toute question ou problème :
1. Consultez `TROUBLESHOOTING.md`
2. Vérifiez les logs Unity (`Window > Console`)
3. Consultez la documentation AR Foundation
4. Ouvrez une issue sur GitHub

---

**Version :** 1.0.0  
**Dernière mise à jour :** Décembre 2025  
**Compatibilité :** Unity 2022.3.62f3
