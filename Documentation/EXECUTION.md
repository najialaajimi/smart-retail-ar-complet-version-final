# Ordre d'Exécution - Smart Retail AR

## 📋 Vue d'Ensemble

Ce document décrit l'ordre d'exécution, le flux de contrôle et la séquence de démarrage de l'application Smart Retail AR.

---

## 🚀 Séquence de Démarrage

### 1. Initialisation de l'Application

**Scène de départ** : `MainMenu.unity`

**Ordre d'initialisation des composants** :

1. **GameManager** (DontDestroyOnLoad)
   - Initialisation du singleton
   - Chargement des paramètres de l'application
   - Configuration de la persistance inter-scènes
   
2. **ProductDatabase** (Lazy initialization)
   - Chargement du fichier `Assets/Data/products.json`
   - Parsing et indexation des produits
   - Mise en cache des données

3. **PerformanceMonitor** (Optionnel)
   - Démarrage du monitoring si activé
   - Initialisation des compteurs de performance

**Script de contrôle** : `GameManager.cs`
```csharp
// Ordre d'exécution dans Awake() :
// 1. Vérifier/créer l'instance singleton
// 2. Marquer DontDestroyOnLoad
// 3. Initialiser AppSettings
// 4. Charger ProductDatabase
```

---

## 🔄 Flux de Navigation Entre Scènes

### Diagramme de Flux

```
MainMenu (Scène d'entrée)
    │
    ├──> QRScanner ──> ARProductView ──> Recommendations
    │                        │                  │
    │                        └──────────────────┘
    │                                 │
    └──> Settings <──────────────────┘
         (Retour au menu principal)
```

### Ordre d'Exécution par Scène

#### 1. MainMenu → QRScanner

**Déclencheur** : Clic sur bouton "Scanner QR"
**Script** : `MainMenuController.cs` → `SceneLoader.LoadScene("QRScanner")`

**Séquence** :
1. Désactivation du Canvas MainMenu
2. Chargement asynchrone de la scène QRScanner
3. Activation de la caméra dans QRScanner
4. Initialisation de `QRCodeScanner.cs`

#### 2. QRScanner → ARProductView

**Déclencheur** : QR Code détecté et validé
**Script** : `QRCodeScanner.cs` → Event `OnQRCodeScanned`

**Séquence** :
1. Extraction de l'ID produit du QR code
2. Validation de l'ID dans ProductDatabase
3. Stockage du produit sélectionné dans `ProductManager`
4. Transition vers ARProductView
5. Initialisation de la session AR

#### 3. ARProductView → Recommendations

**Déclencheur** : Clic sur bouton "Voir Alternatives"
**Script** : `ProductInfoPanel.cs`

**Séquence** :
1. Récupération du produit courant
2. Pré-calcul des recommandations via `RecommendationEngine`
3. Chargement de la scène Recommendations
4. Affichage des produits alternatifs

#### 4. N'importe quelle scène → Settings

**Déclencheur** : Clic sur bouton "Paramètres"
**Script** : Divers controllers UI

**Séquence** :
1. Sauvegarde de l'état actuel
2. Chargement de Settings
3. Affichage des préférences utilisateur

#### 5. Retour au MainMenu

**Déclencheur** : Clic sur bouton "Retour" ou "Menu Principal"
**Script** : `SceneLoader.LoadScene("MainMenu")`

**Séquence** :
1. Nettoyage de la scène actuelle
2. Arrêt des services (AR Session, Camera)
3. Retour au menu principal

---

## ⚙️ Ordre d'Exécution des Scripts Unity

### Scripts avec Execution Order Personnalisé

Unity exécute les scripts dans cet ordre (si configuré) :

1. **GameManager** (Execution Order: -100)
   - Doit s'initialiser avant tous les autres
   
2. **ProductDatabase** (Execution Order: -50)
   - Initialisation des données après GameManager
   
3. **Scripts par défaut** (Execution Order: 0)
   - Tous les autres MonoBehaviours

### Configuration dans Unity

```
Edit > Project Settings > Script Execution Order
│
├── GameManager : -100
├── ProductDatabase : -50
└── Default Time : 0
```

---

## 📱 Cycle de Vie d'une Scène AR (ARProductView)

### Ordre d'Initialisation AR

1. **ARSessionManager.Awake()**
   - Vérification de la compatibilité AR
   - Configuration du XROrigin

2. **ARSessionManager.OnEnable()**
   - Démarrage de la session AR
   - Activation du tracking

3. **ImageTrackingManager.OnEnable()**
   - Chargement de la bibliothèque d'images
   - Activation du tracking d'images

4. **ARPlacementManager.Start()**
   - Initialisation du gestionnaire de placement
   - Configuration des raycast AR

5. **ARProductOverlay.Update()**
   - Mise à jour continue de l'overlay
   - Repositionnement selon le tracking

### Ordre de Destruction AR

1. **ARSessionManager.OnDisable()**
   - Pause de la session AR
   
2. **ImageTrackingManager.OnDisable()**
   - Arrêt du tracking d'images
   
3. **ARSessionManager.OnDestroy()**
   - Nettoyage complet de la session AR

---

## 🧪 Ordre d'Exécution des Tests

### Tests EditMode

**Exécution** : `Window > General > Test Runner > EditMode`

**Ordre** :
1. `ProductDatabaseTests.cs`
   - Test de chargement JSON
   - Test de recherche/filtrage
   
2. `RecommendationEngineTests.cs`
   - Test des algorithmes de recommandation
   - Test des filtres

### Tests PlayMode

**Exécution** : `Window > General > Test Runner > PlayMode`

**Ordre** :
1. `QRScannerTests.cs`
   - Test d'initialisation de la caméra
   - Test de simulation de scan
   
2. `AROverlayTests.cs`
   - Test de création d'overlay
   - Test d'affichage produit

---

## 🔧 Outils d'Exécution Automatique

### AutoSceneSetup (Editor Tool)

**Accès** : `Tools > Smart Retail AR > Auto Scene Setup`

**Ordre d'exécution** :
1. Sélection de la scène cible
2. Création des GameObjects
3. Ajout des composants (AddComponent)
4. Configuration des références
5. Sauvegarde de la scène

### SceneAutoRunner (Validation)

**Script** : `Assets/Scripts/Utils/SceneAutoRunner.cs`

**Ordre d'exécution** :
1. Chargement de toutes les scènes
2. Validation de la hiérarchie
3. Vérification des composants requis
4. Génération du rapport

---

## 📊 Workflows Complets - Guide Step by Step

### Workflow 1 : Scan → Visualisation AR → Recommandations

#### Étape 1 : Démarrage de l'Application
**Durée** : 0.5-1 seconde

**Séquence d'exécution** :
```
1.1. Unity charge MainMenu.unity
     └─> Lecture du fichier de scène
     └─> Création de la hiérarchie GameObjects
     
1.2. GameManager.Awake() s'exécute en PREMIER (Execution Order: -100)
     └─> Vérification singleton : if (Instance == null) Instance = this;
     └─> DontDestroyOnLoad(gameObject)
     └─> Initialisation de AppSettings.Instance
     
1.3. ProductDatabase initialisation (Lazy Loading)
     └─> Premier accès déclenche LoadFromJson()
     └─> Lecture de Assets/Data/products.json
     └─> Parsing JSON → List<Product>
     └─> Indexation par ID pour accès O(1)
     
1.4. MainMenuController.Start() s'exécute
     └─> Configuration des boutons UI
     └─> Câblage des événements OnClick
     └─> Affichage du menu principal
```

**Validation** :
- ✅ Console : "GameManager initialized"
- ✅ Console : "ProductDatabase loaded: X products"
- ✅ UI : Menu principal visible avec 4 boutons

---

#### Étape 2 : Navigation vers Scanner QR
**Durée** : 0.3-0.5 seconde

**Séquence d'exécution** :
```
2.1. Utilisateur clique sur bouton "Scanner QR"
     └─> MainMenuController.OnScanQRButtonClick()
     └─> SceneLoader.LoadScene("QRScanner")
     
2.2. Unity décharge MainMenu (sauf DontDestroyOnLoad objects)
     └─> Destruction des GameObjects de la scène
     └─> GameManager PERSISTE (DontDestroyOnLoad)
     
2.3. Unity charge QRScanner.unity
     └─> Création de la hiérarchie de scène
     └─> Instanciation Camera, Canvas, QRCodeScanner
     
2.4. QRCodeScanner.Start() s'exécute
     └─> WebCamTexture.devices pour lister caméras
     └─> Sélection de la caméra arrière
     └─> webCamTexture = new WebCamTexture(selectedDevice.name)
     └─> webCamTexture.Play()
     
2.5. QRCodeScanner.Update() commence à s'exécuter
     └─> Chaque frame : lecture du webCamTexture
     └─> Tentative de décodage QR avec ZXing (si implémenté)
```

**Validation** :
- ✅ Console : "QRScanner scene loaded"
- ✅ UI : Flux caméra visible à l'écran
- ✅ UI : Zone de visée visible au centre
- ✅ UI : Instructions "Pointez vers un QR Code"

---

#### Étape 3 : Scan du QR Code
**Durée** : 1-3 secondes (selon conditions lumineuses)

**Séquence d'exécution** :
```
3.1. QR Code détecté dans le flux caméra
     └─> ZXing.BarcodeReader.Decode(pixels)
     └─> Result != null
     
3.2. Validation du format QR
     └─> Format attendu : "SMARTRETAIL:PROD001"
     └─> Split sur ":" pour extraire ID
     └─> productId = "PROD001"
     
3.3. Recherche du produit dans la base
     └─> Product product = ProductDatabase.Instance.GetProductById(productId)
     └─> if (product == null) → Afficher erreur "Produit inconnu"
     └─> if (product != null) → Continuer
     
3.4. Enregistrement du produit courant
     └─> ProductManager.Instance.SetCurrentProduct(product)
     └─> Event OnProductSelected déclenché
     
3.5. Arrêt de la caméra
     └─> webCamTexture.Stop()
     └─> Libération des ressources
     
3.6. Transition automatique vers AR
     └─> Délai de 0.5s pour feedback visuel
     └─> SceneLoader.LoadScene("ARProductView")
```

**Validation** :
- ✅ Console : "QR Code detected: SMARTRETAIL:PROD001"
- ✅ Console : "Product found: Lait Bio Entier"
- ✅ UI : Feedback visuel (flash vert ou animation)
- ✅ État : ProductManager.CurrentProduct != null

---

#### Étape 4 : Initialisation de la Session AR
**Durée** : 1-2 secondes

**Séquence d'exécution** :
```
4.1. Unity charge ARProductView.unity
     └─> Création XR Origin GameObject
     └─> Création AR Session GameObject
     
4.2. ARSessionManager.Awake()
     └─> Recherche XROrigin dans la scène
     └─> xrOrigin = GetComponent<XROrigin>()
     └─> Vérification compatibilité AR
     
4.3. ARSessionManager.OnEnable()
     └─> if (ARSession.state == ARSessionState.None)
     └─> Démarrage session : ARSession.enabled = true
     └─> Attente état SessionTracking
     
4.4. Boucle d'attente tracking AR
     └─> while (ARSession.state != ARSessionState.SessionTracking)
     └─> yield return new WaitForSeconds(0.1f)
     └─> Timeout après 10 secondes
     
4.5. ARSessionManager.OnSessionInitialized()
     └─> Event déclenché quand tracking actif
     └─> ARPlacementManager activé
     └─> ImageTrackingManager activé
```

**Validation** :
- ✅ Console : "AR Session initializing..."
- ✅ Console : "AR Session state: SessionTracking"
- ✅ UI : Flux caméra AR visible
- ✅ État : ARSession.state == ARSessionState.SessionTracking

---

#### Étape 5 : Affichage Overlay AR Produit
**Durée** : 0.5-1 seconde

**Séquence d'exécution** :
```
5.1. ARProductOverlay.Start()
     └─> Product currentProduct = ProductManager.Instance.CurrentProduct
     └─> if (currentProduct == null) → Erreur et retour menu
     
5.2. Création de l'UI Overlay
     └─> Canvas worldSpace créé devant caméra
     └─> Position initiale : 1 mètre devant caméra
     └─> Rotation billboard : face à la caméra
     
5.3. Peuplement des données UI
     └─> productNameText.text = currentProduct.name
     └─> priceText.text = $"{currentProduct.price:C2}"
     └─> brandText.text = currentProduct.brand
     └─> originText.text = $"Origine: {currentProduct.origin}"
     
5.4. Affichage NutriScore
     └─> NutriScoreDisplay.UpdateScore(currentProduct.nutrition.nutriScore)
     └─> Couleur selon score : A=Vert, E=Rouge
     └─> Animation d'apparition
     
5.5. Affichage EcoScore
     └─> EcoScoreDisplay.UpdateScore(currentProduct.ecoScore)
     └─> Barre de progression 0-100
     └─> Couleur gradient selon score
     
5.6. ARProductOverlay.LateUpdate() en boucle
     └─> Chaque frame : mise à jour position overlay
     └─> Billboard rotation : toujours face caméra
     └─> Distance maintenue : 1-2 mètres
```

**Validation** :
- ✅ UI : Panel AR visible avec infos produit
- ✅ UI : NutriScore affiché avec bonne couleur
- ✅ UI : EcoScore visible (0-100)
- ✅ Comportement : Panel suit le mouvement caméra

---

#### Étape 6 : Interaction Utilisateur en AR
**Durée** : Variable (exploration utilisateur)

**Options disponibles** :
```
6.1. Bouton "Voir Alternatives"
     └─> ProductInfoPanel.OnAlternativesButtonClick()
     └─> Transition vers Recommendations
     
6.2. Bouton "Plus d'Infos"
     └─> ProductInfoPanel.OnMoreInfoButtonClick()
     └─> Affichage panel détaillé (nutrition complète)
     
6.3. Bouton "Retour"
     └─> SceneLoader.LoadScene("MainMenu")
     └─> Nettoyage session AR
```

---

#### Étape 7 : Transition vers Recommandations
**Durée** : 0.5-1 seconde

**Séquence d'exécution** :
```
7.1. Utilisateur clique "Voir Alternatives"
     └─> ProductInfoPanel.OnAlternativesButtonClick()
     └─> Product currentProduct = ProductManager.Instance.CurrentProduct
     
7.2. ARSessionManager.OnDisable()
     └─> Pause session AR : ARSession.enabled = false
     └─> Arrêt tracking images
     
7.3. Chargement Recommendations.unity
     └─> Unity décharge ARProductView
     └─> Destruction objets AR (XROrigin, ARSession)
     └─> GameManager PERSISTE
     
7.4. RecommendationEngine.Start()
     └─> Récupération produit courant
     └─> Calcul des alternatives
```

**Validation** :
- ✅ Console : "AR Session stopped"
- ✅ Console : "Recommendations scene loaded"
- ✅ État : Session AR proprement fermée

---

#### Étape 8 : Calcul des Recommandations
**Durée** : 0.2-0.5 seconde

**Séquence d'exécution** :
```
8.1. RecommendationEngine.CalculateRecommendations(currentProduct)
     └─> Récupération alternatives du produit
     └─> alternatives = currentProduct.alternatives (IDs)
     
8.2. Chargement des produits alternatifs
     └─> foreach (string altId in alternatives)
     └─> Product alt = ProductDatabase.Instance.GetProductById(altId)
     └─> recommendedProducts.Add(alt)
     
8.3. Application des filtres utilisateur
     └─> RecommendationFilter.ApplyFilters(recommendedProducts)
     └─> Filtrage par prix (si slider activé)
     └─> Filtrage par EcoScore (si toggle actif)
     └─> Filtrage par Bio (si toggle actif)
     
8.4. Tri des recommandations
     └─> Calcul score combiné pour chaque produit
     └─> score = 0.3*ecoScore + 0.3*nutriScore + 0.2*priceScore + 0.2*bioBonus
     └─> Tri décroissant par score
     
8.5. Limitation du nombre de résultats
     └─> Garder les 5 meilleurs
     └─> return topRecommendations
```

**Validation** :
- ✅ Console : "Calculating recommendations for PROD001"
- ✅ Console : "Found X alternatives"
- ✅ Console : "After filtering: Y products"

---

#### Étape 9 : Affichage des Recommandations
**Durée** : 0.3-0.5 seconde

**Séquence d'exécution** :
```
9.1. RecommendationUI.DisplayRecommendations(recommendations)
     └─> Effacement du contenu précédent du ScrollView
     └─> foreach (Product product in recommendations)
     
9.2. Pour chaque produit recommandé
     └─> Instanciation RecommendationCard prefab
     └─> SetParent(scrollViewContent)
     └─> Peuplement des données :
         - card.productNameText = product.name
         - card.priceText = product.price
         - card.nutriScoreImage = GetNutriScoreSprite(product.nutrition.nutriScore)
         - card.ecoScoreFill = product.ecoScore / 100f
         
9.3. Configuration des callbacks
     └─> card.button.onClick.AddListener(() => OnProductCardClick(product))
     └─> Animation d'apparition (fade in + scale)
     
9.4. Mise à jour layout
     └─> LayoutRebuilder.ForceRebuildLayoutImmediate(scrollViewContent)
     └─> Canvas.ForceUpdateCanvases()
```

**Validation** :
- ✅ UI : Liste de produits alternatifs visible
- ✅ UI : Chaque carte affiche nom, prix, scores
- ✅ UI : Cartes cliquables
- ✅ Comportement : Scroll fonctionnel

---

#### Étape 10 : Retour au Menu Principal
**Durée** : 0.3-0.5 seconde

**Séquence d'exécution** :
```
10.1. Utilisateur clique "Retour Menu"
      └─> RecommendationUI.OnBackButtonClick()
      └─> SceneLoader.LoadScene("MainMenu")
      
10.2. Nettoyage de la scène Recommendations
      └─> Destruction des cartes produits
      └─> Libération des ressources UI
      
10.3. Rechargement MainMenu.unity
      └─> GameManager toujours présent
      └─> ProductDatabase toujours en mémoire
      └─> MainMenuController.Start() réinitialise UI
      
10.4. Réinitialisation de l'état
      └─> ProductManager.CurrentProduct peut être gardé ou null
      └─> Option : conserver historique navigation
```

**Validation** :
- ✅ UI : Menu principal affiché
- ✅ État : Retour à l'état initial
- ✅ Console : Pas d'erreurs ou warnings
- ✅ Mémoire : Pas de memory leaks

---

### Workflow 2 : Configuration des Paramètres

#### Étape 1 : Accès aux Paramètres
**Durée** : 0.3 seconde

**Séquence d'exécution** :
```
1.1. Depuis n'importe quelle scène, clic sur "⚙️ Paramètres"
     └─> Current scene controller : OnSettingsButtonClick()
     └─> Sauvegarde état actuel (optionnel)
     
1.2. Chargement Settings.unity
     └─> SceneLoader.LoadScene("Settings")
     └─> GameManager persiste
     
1.3. AppSettings.Awake()
     └─> if (Instance == null) Instance = this;
     └─> DontDestroyOnLoad(gameObject)
     
1.4. AppSettings.LoadSettings()
     └─> Lecture PlayerPrefs :
         - arMode = PlayerPrefs.GetInt("ARMode", 0)
         - scanFrequency = PlayerPrefs.GetFloat("ScanFrequency", 30f)
         - enableSound = PlayerPrefs.GetInt("EnableSound", 1) == 1
         - enableVibration = PlayerPrefs.GetInt("EnableVibration", 1) == 1
```

**Validation** :
- ✅ UI : Scène Settings chargée
- ✅ Console : "Settings loaded"
- ✅ État : AppSettings.Instance != null

---

#### Étape 2 : Affichage des Options
**Durée** : 0.1 seconde

**Séquence d'exécution** :
```
2.1. SettingsPanel.Start()
     └─> Récupération des paramètres actuels
     └─> AppSettings settings = AppSettings.Instance
     
2.2. Initialisation des contrôles UI
     └─> arModeDropdown.value = settings.arMode
     └─> scanFrequencySlider.value = settings.scanFrequency
     └─> soundToggle.isOn = settings.enableSound
     └─> vibrationToggle.isOn = settings.enableVibration
     
2.3. Câblage des événements
     └─> arModeDropdown.onValueChanged.AddListener(OnARModeChanged)
     └─> scanFrequencySlider.onValueChanged.AddListener(OnScanFrequencyChanged)
     └─> soundToggle.onValueChanged.AddListener(OnSoundToggled)
     └─> vibrationToggle.onValueChanged.AddListener(OnVibrationToggled)
```

**Validation** :
- ✅ UI : Tous les contrôles affichent valeurs actuelles
- ✅ UI : Dropdowns, sliders, toggles fonctionnels

---

#### Étape 3 : Modification Interactive
**Durée** : Variable (interaction utilisateur)

**Séquence pour chaque modification** :
```
3.1. Utilisateur modifie un paramètre
     └─> Event onValueChanged déclenché
     
3.2. Exemple : Toggle Son activé
     └─> OnSoundToggled(bool value)
     └─> AppSettings.Instance.enableSound = value
     └─> Feedback visuel (animation toggle)
     
3.3. Exemple : Slider Fréquence Scan
     └─> OnScanFrequencyChanged(float value)
     └─> AppSettings.Instance.scanFrequency = value
     └─> scanFrequencyValueText.text = $"{value:F1} Hz"
     
3.4. Sauvegarde automatique en temps réel (optionnel)
     └─> PlayerPrefs.SetInt("EnableSound", value ? 1 : 0)
     └─> PlayerPrefs.Save()
```

**Validation** :
- ✅ UI : Changements visibles immédiatement
- ✅ État : AppSettings.Instance mis à jour
- ✅ UI : Labels affichent nouvelles valeurs

---

#### Étape 4 : Sauvegarde Explicite
**Durée** : 0.1 seconde

**Séquence d'exécution** :
```
4.1. Utilisateur clique "Sauvegarder"
     └─> SettingsPanel.OnSaveButtonClick()
     └─> AppSettings.Instance.SaveSettings()
     
4.2. AppSettings.SaveSettings()
     └─> PlayerPrefs.SetInt("ARMode", arMode)
     └─> PlayerPrefs.SetFloat("ScanFrequency", scanFrequency)
     └─> PlayerPrefs.SetInt("EnableSound", enableSound ? 1 : 0)
     └─> PlayerPrefs.SetInt("EnableVibration", enableVibration ? 1 : 0)
     └─> PlayerPrefs.Save()
     
4.3. Feedback visuel
     └─> Affichage message "✓ Paramètres sauvegardés"
     └─> Animation fade out après 2 secondes
```

**Validation** :
- ✅ Console : "Settings saved successfully"
- ✅ UI : Message de confirmation
- ✅ Persistance : PlayerPrefs.HasKey("ARMode") == true

---

#### Étape 5 : Retour à la Scène Précédente
**Durée** : 0.3 seconde

**Séquence d'exécution** :
```
5.1. Utilisateur clique "Retour" ou "← Précédent"
     └─> SettingsPanel.OnBackButtonClick()
     
5.2. Détermination de la scène de retour
     └─> Option A : SceneLoader.LoadPreviousScene()
     └─> Option B : SceneLoader.LoadScene("MainMenu")
     
5.3. Application des nouveaux paramètres
     └─> Les composants lisent AppSettings au prochain Start()
     └─> Exemple : QRCodeScanner lit scanFrequency
     
5.4. Retour à la scène
     └─> Unity charge la scène de destination
     └─> AppSettings persiste (DontDestroyOnLoad)
```

**Validation** :
- ✅ UI : Retour à la scène attendue
- ✅ État : Paramètres appliqués dans nouvelle scène
- ✅ Console : "Returned from Settings"

---

### Workflow 3 : Gestion d'Erreur - Produit Non Trouvé

#### Séquence Complète
**Durée** : 3-5 secondes

```
1. QR Code scanné avec ID invalide
   └─> QRCodeScanner détecte "SMARTRETAIL:PROD999"
   
2. Recherche dans la base
   └─> Product product = ProductDatabase.Instance.GetProductById("PROD999")
   └─> Result : product == null
   
3. Gestion de l'erreur
   └─> Debug.LogWarning("Product PROD999 not found")
   └─> Affichage message UI : "Produit non reconnu"
   └─> Son d'erreur (si enableSound == true)
   
4. Options utilisateur
   └─> Bouton "Réessayer" → Reste sur QRScanner
   └─> Bouton "Menu" → Retour MainMenu
   
5. Pas de transition vers AR
   └─> ARProductView n'est PAS chargé
   └─> Utilisateur peut scanner un autre code
```

**Validation** :
- ✅ UI : Message d'erreur visible
- ✅ Comportement : App ne crash pas
- ✅ UX : Options de récupération disponibles

---

### Workflow 4 : Mode Débogage - Simulation Sans Caméra

#### Étapes pour Test sans Appareil AR

```
1. Activer le mode DEBUG dans GameManager
   └─> GameManager.IsDebugMode = true
   └─> Console : "DEBUG MODE ENABLED"
   
2. Bypass vérification caméra
   └─> QRCodeScanner.Start()
   └─> if (IsDebugMode) SkipCameraInitialization()
   
3. Utiliser SimulateScan()
   └─> Dans Unity Console :
       QRCodeScanner.Instance.SimulateScan("SMARTRETAIL:PROD001")
   
4. Bypass vérification AR
   └─> ARSessionManager.Start()
   └─> if (IsDebugMode) SimulateARSession()
   
5. Tester workflows complets sans matériel
   └─> Navigation complète possible
   └─> UI testable dans Unity Editor
```

**Validation** :
- ✅ Console : "DEBUG: Simulating QR scan"
- ✅ Console : "DEBUG: Simulating AR session"
- ✅ Comportement : Workflows fonctionnent sans caméra

---

## 🎯 Points de Contrôle d'Exécution

### Vérifications Avant Exécution

**Avant de lancer l'application** :
1. ✅ Vérifier que MainMenu est la première scène dans Build Settings
2. ✅ S'assurer que products.json est dans `Assets/Data/`
3. ✅ Vérifier que tous les packages AR sont installés
4. ✅ Confirmer que les scènes ont des GUIDs valides

### Points de Validation Runtime

**Pendant l'exécution** :
1. **GameManager.Instance != null** - Singleton initialisé
2. **ProductDatabase.IsLoaded** - Données chargées
3. **ARSession.state == ARSessionState.SessionTracking** - AR fonctionnel
4. **ProductManager.CurrentProduct != null** - Produit sélectionné

---

## 🚨 Gestion des Erreurs d'Exécution

### Erreurs Critiques (Application s'arrête)

1. **GameManager non initialisé**
   - Cause : Script manquant ou désactivé
   - Solution : Ajouter GameManager GameObject dans MainMenu
   
2. **products.json introuvable**
   - Cause : Fichier manquant ou chemin incorrect
   - Solution : Vérifier `Assets/Data/products.json`

3. **AR Session non supportée**
   - Cause : Appareil non compatible
   - Solution : Vérifier ARSessionManager.CheckARSupport()

### Erreurs Non-Critiques (Fonctionnement dégradé)

1. **Produit non trouvé dans la base**
   - Comportement : Message d'erreur affiché
   - Continuation : Retour au menu principal
   
2. **Caméra non accessible**
   - Comportement : Message de permissions
   - Continuation : Demande de permissions utilisateur

---

## 📝 Commandes de Debug pour Contrôler l'Exécution

### Console Unity

```csharp
// Forcer le chargement d'une scène
SceneLoader.LoadScene("MainMenu");

// Simuler un scan QR
QRCodeScanner.Instance.SimulateScan("SMARTRETAIL:PROD001");

// Vérifier l'état du GameManager
Debug.Log($"GameManager initialized: {GameManager.Instance != null}");

// Afficher le produit courant
Debug.Log($"Current product: {ProductManager.Instance.CurrentProduct?.name}");

// Vérifier l'état AR
Debug.Log($"AR Session state: {ARSession.state}");
```

### Raccourcis Clavier (À implémenter si nécessaire)

- `F1` : Retour au MainMenu
- `F2` : Recharger la scène courante
- `F5` : Toggle PerformanceMonitor
- `F12` : Afficher les logs de debug

---

## 🔄 Ordre de Mise à Jour (Update Loop)

### Frame Update Order

Dans chaque frame Unity, l'ordre d'exécution est :

1. **FixedUpdate()** (physique, 50 FPS fixe)
2. **Update()** (logique de jeu, chaque frame)
3. **LateUpdate()** (ajustements après Update)

**Scripts utilisant Update()** :
- `QRCodeScanner.Update()` - Lecture frame caméra
- `ARProductOverlay.LateUpdate()` - Repositionnement UI après tracking
- `PerformanceMonitor.Update()` - Collecte métriques

---

## 📋 Checklist de Validation d'Exécution

### Avant le Déploiement

- [ ] Toutes les scènes se chargent sans erreur
- [ ] GameManager persiste entre les scènes
- [ ] ProductDatabase charge correctement
- [ ] Navigation entre scènes fonctionne
- [ ] Session AR démarre sur appareil compatible
- [ ] QR Code Scanner détecte les codes
- [ ] Recommandations s'affichent correctement
- [ ] Paramètres se sauvegardent
- [ ] Retour au menu depuis n'importe quelle scène
- [ ] Pas de memory leaks entre transitions

### Tests de Flux Complet

1. **Test Séquence Complète**
   - MainMenu → QRScanner → ARProductView → Recommendations → MainMenu
   - Durée attendue : < 30 secondes
   
2. **Test Interruption**
   - Navigation vers Settings depuis chaque scène
   - Retour à la scène d'origine sans perte de données
   
3. **Test Performance**
   - Latence affichage produit : ≤ 1 seconde
   - Taux de reconnaissance QR : ≥ 95%
   - FPS en AR : ≥ 30 FPS

---

## 🎓 Bonnes Pratiques d'Exécution

### 1. Gestion de l'État

- Toujours utiliser `ProductManager` pour partager les données entre scènes
- Ne pas stocker d'état dans des scripts de scène
- Utiliser DontDestroyOnLoad pour les gestionnaires globaux

### 2. Transitions de Scène

- Utiliser `SceneLoader.LoadScene()` pour toutes les transitions
- Attendre la fin du chargement asynchrone avant d'accéder aux objets
- Nettoyer les ressources avant de changer de scène

### 3. Gestion AR

- Toujours vérifier `ARSession.state` avant d'utiliser les features AR
- Désactiver la session AR quand on quitte ARProductView
- Gérer les cas où AR n'est pas supporté

### 4. Performance

- Limiter les appels dans Update() aux opérations nécessaires
- Utiliser des coroutines pour les opérations longues
- Mettre en cache les références aux composants dans Awake()

---

## 📞 Support

Pour plus d'informations :
- **Guide complet** : `Documentation/GUIDE_EXECUTION.md`
- **Configuration** : `Documentation/SETUP_GUIDE.md`
- **Architecture** : `Documentation/SCENE_GUIDE.md`
- **Dépannage** : `Documentation/TROUBLESHOOTING.md`
