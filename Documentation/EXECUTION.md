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

## 📊 Workflow Complet Utilisateur

### Workflow 1 : Scan → Visualisation AR → Recommandations

```
1. Démarrage application
   └─> MainMenu.unity chargée
       └─> GameManager initialisé
       
2. Utilisateur clique "Scanner QR"
   └─> QRScanner.unity chargée
       └─> QRCodeScanner activé
       └─> Caméra démarrée
       
3. QR Code scanné
   └─> Produit extrait (ex: PROD001)
       └─> ProductManager.SetCurrentProduct()
       
4. Transition automatique vers ARProductView
   └─> ARSessionManager démarre session AR
       └─> ARProductOverlay affiche infos produit
       └─> ImageTrackingManager suit le produit
       
5. Utilisateur clique "Voir Alternatives"
   └─> Recommendations.unity chargée
       └─> RecommendationEngine calcule alternatives
       └─> RecommendationUI affiche les cartes produits
       
6. Utilisateur clique "Retour Menu"
   └─> MainMenu.unity rechargée
       └─> Cycle complet terminé
```

### Workflow 2 : Configuration

```
1. Depuis n'importe quelle scène
   └─> Clic sur bouton "Paramètres"
   
2. Settings.unity chargée
   └─> AppSettings charge les préférences
       └─> SettingsPanel affiche les options
       
3. Modifications utilisateur
   └─> Toggles/Sliders modifiés
       └─> Changements sauvegardés en temps réel
       
4. Clic sur "Sauvegarder"
   └─> AppSettings.SaveSettings()
       └─> Persistance dans PlayerPrefs
       
5. Retour à la scène précédente
   └─> Paramètres appliqués
```

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
