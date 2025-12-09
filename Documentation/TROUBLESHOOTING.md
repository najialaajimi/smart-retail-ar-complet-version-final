# Dépannage - Smart Retail AR

## Table des Matières

1. [Problèmes d'Installation](#problèmes-dinstallation)
2. [Problèmes AR](#problèmes-ar)
3. [Problèmes QR Scanner](#problèmes-qr-scanner)
4. [Problèmes Base de Données](#problèmes-base-de-données)
5. [Problèmes de Performance](#problèmes-de-performance)
6. [Problèmes de Build](#problèmes-de-build)

---

## Problèmes d'Installation

### Unity ne trouve pas les packages AR Foundation

**Symptômes** : Erreurs de compilation liées à AR Foundation

**Solutions** :
1. Vérifier `Packages/manifest.json` :
   ```json
   "com.unity.xr.arfoundation": "5.1.0"
   ```
2. Window > Package Manager > AR Foundation > Install
3. Redémarrer Unity

### Les scripts ne compilent pas

**Symptômes** : Erreurs de compilation C#

**Solutions** :
1. Vérifier la version Unity : **2022.3.62f3**
2. Assets > Reimport All
3. Edit > Preferences > External Tools > Regenerate project files
4. Redémarrer Unity et votre IDE

### Erreur "Assembly not found"

**Solutions** :
1. Assets > Reimport All
2. Supprimer le dossier `Library/`
3. Rouvrir le projet dans Unity

---

## Problèmes AR

### AR ne s'initialise pas

**Symptômes** : Message "AR non supporté" ou session ne démarre pas

**Diagnostic** :
```csharp
Debug.Log($"AR Supported: {ARSessionManager.Instance.IsARSupported()}");
Debug.Log($"AR State: {ARSessionManager.Instance.GetCurrentState()}");
```

**Solutions** :

#### Sur Android
1. Vérifier ARCore installé sur l'appareil
2. Player Settings > XR Plug-in Management > Cocher "ARCore"
3. Vérifier permissions caméra dans AndroidManifest.xml

#### Sur iOS
1. Vérifier compatibilité ARKit (iPhone 6s minimum, iOS 11+)
2. Player Settings > XR Plug-in Management > Cocher "ARKit"
3. Vérifier NSCameraUsageDescription dans Info.plist

### Tracking AR instable

**Symptômes** : L'overlay AR bouge de manière saccadée

**Solutions** :
1. Améliorer l'éclairage de la pièce
2. Pointer vers des surfaces avec des détails visuels
3. Réduire la fréquence de mise à jour dans les paramètres
4. Vérifier les performances (FPS < 30 cause l'instabilité)

### Plans AR non détectés

**Symptômes** : Aucun plan détecté en AR

**Solutions** :
1. Vérifier que ARPlaneManager est activé
2. Pointer la caméra vers une surface horizontale
3. Bouger lentement la caméra pour scanner la zone
4. Vérifier l'éclairage

**Code de diagnostic** :
```csharp
var planeManager = FindObjectOfType<ARPlaneManager>();
Debug.Log($"Planes detected: {planeManager.trackables.count}");
```

---

## Problèmes QR Scanner

### Caméra ne s'active pas

**Symptômes** : Écran noir dans le scanner QR

**Solutions** :
1. Vérifier permissions caméra accordées
2. Redémarrer l'application
3. Vérifier qu'aucune autre app n'utilise la caméra

**Code de diagnostic** :
```csharp
Debug.Log($"Camera devices: {WebCamTexture.devices.Length}");
foreach (var device in WebCamTexture.devices) {
    Debug.Log($"Camera: {device.name}");
}
```

### QR codes non détectés

**Symptômes** : Scanner ne lit pas les QR codes

**Solutions** :
1. **Éclairage** : Assurer un bon éclairage
2. **Distance** : Tenir le QR code à 20-30 cm
3. **Qualité** : Vérifier la qualité du QR code (pas flou, pas abîmé)
4. **Format** : Vérifier le format (SMARTRETAIL:PRODXXX)
5. **Fréquence** : Augmenter la fréquence de scan dans les paramètres

**Test manuel** :
```csharp
scanner.SimulateScan("SMARTRETAIL:PROD001");
```

### Faux positifs ou mauvaises détections

**Solutions** :
1. Améliorer la validation du format QR
2. Réduire la fréquence de scan
3. Ajouter une confirmation utilisateur

---

## Problèmes Base de Données

### Produits non chargés

**Symptômes** : GetAllProducts() retourne une liste vide

**Diagnostic** :
```csharp
bool loaded = ProductDatabase.Instance.LoadFromJson();
Debug.Log($"Database loaded: {loaded}");
Debug.Log($"Products count: {ProductDatabase.Instance.GetAllProducts().Count}");
```

**Solutions** :
1. Vérifier que `products.json` existe dans `Assets/Data/`
2. Créer un dossier Resources et y placer le fichier :
   `Assets/Resources/Data/products.json`
3. Vérifier le format JSON (utiliser un validateur JSON)
4. Charger manuellement :
   ```csharp
   string json = Resources.Load<TextAsset>("Data/products").text;
   ProductDatabase.Instance.LoadFromJsonString(json);
   ```

### JSON invalide

**Symptômes** : Erreur lors du parsing JSON

**Solutions** :
1. Valider le JSON sur https://jsonlint.com/
2. Vérifier les virgules et accolades
3. S'assurer que les chaînes sont entre guillemets
4. Vérifier l'encodage UTF-8

### Produit non trouvé par ID

**Diagnostic** :
```csharp
string id = "PROD001";
Product product = ProductDatabase.Instance.GetProductById(id);
Debug.Log(product == null ? $"Product {id} not found" : $"Found: {product.name}");
```

**Solutions** :
1. Vérifier l'ID exact (case-sensitive)
2. Vérifier que le produit existe dans products.json
3. Recharger la base de données

---

## Problèmes de Performance

### FPS bas (< 30)

**Diagnostic** :
```csharp
float fps = PerformanceMonitor.Instance.GetCurrentFPS();
Debug.Log($"Current FPS: {fps}");
```

**Solutions** :
1. Réduire la résolution de la caméra
2. Limiter le nombre d'objets AR affichés
3. Optimiser les textures (compression, taille)
4. Désactiver les fonctionnalités AR non utilisées
5. Réduire la fréquence de scan QR

### Latence élevée

**Symptômes** : Délai entre le scan et l'affichage

**Mesure** :
```csharp
PerformanceMonitor.Instance.StartTimer("ScanToDisplay");
// ... opération ...
float elapsed = PerformanceMonitor.Instance.StopTimer("ScanToDisplay");
Debug.Log($"Latency: {elapsed}s (target: ≤1s)");
```

**Solutions** :
1. Précharger les ressources
2. Utiliser l'asynchrone pour les chargements
3. Optimiser les requêtes base de données
4. Mettre en cache les produits fréquents

### Mémoire saturée

**Symptômes** : App crash ou ralentissements

**Solutions** :
1. Libérer les textures non utilisées :
   ```csharp
   Resources.UnloadUnusedAssets();
   System.GC.Collect();
   ```
2. Utiliser Object Pooling pour les UI
3. Limiter le nombre d'objets instanciés

---

## Problèmes de Build

### Build Android échoue

**Erreur** : "Unable to find tools.jar"

**Solution** :
1. Edit > Preferences > External Tools
2. Vérifier le chemin JDK (Java 11)
3. Redémarrer Unity

**Erreur** : "IL2CPP error"

**Solutions** :
1. Installer Visual Studio avec C++ tools
2. Build Settings > Player Settings > Other Settings > IL2CPP
3. Nettoyer le projet : Assets > Clean All

**Erreur** : "Gradle build failed"

**Solutions** :
1. Augmenter la heap size Gradle
2. File > Build Settings > Player Settings > Publishing Settings
3. Cocher "Custom Gradle Template"
4. Éditer `gradle.properties` : `org.gradle.jvmargs=-Xmx4096m`

### Build iOS échoue

**Erreur** : "Code signing error"

**Solutions** :
1. Ouvrir le projet Xcode
2. Signing & Capabilities > Team > Sélectionner votre équipe
3. Automatically manage signing

**Erreur** : "Missing framework"

**Solutions** :
1. Build Settings > Link Framework > Ajouter le framework manquant
2. Rebuild depuis Xcode

### APK trop volumineux

**Solutions** :
1. Build Settings > Player Settings > Publishing Settings
2. Cocher "Split Application Binary"
3. Activer la compression des textures (ASTC pour Android)
4. Supprimer les assets non utilisés

---

## Logs de Diagnostic

### Activer les logs détaillés

```csharp
GameManager.Instance.SetDebugMode(true);
DebugLogger.Instance.SetLogLevel(DebugLogger.LogLevel.Verbose);
DebugLogger.Instance.SetLogToFile(true);
```

### Localiser les fichiers de log

**Android** :
```
/storage/emulated/0/Android/data/com.yourcompany.smartretailar/files/smart_retail_ar.log
```

**iOS** :
```
Application.persistentDataPath/smart_retail_ar.log
```

**Editor** :
- Windows : `%APPDATA%/../Local/Unity/Editor/Editor.log`
- Mac : `~/Library/Logs/Unity/Editor.log`
- Linux : `~/.config/unity3d/Editor.log`

### Générer un rapport de performance

```csharp
string report = PerformanceMonitor.Instance.GeneratePerformanceReport();
Debug.Log(report);
// Ou sauvegarder dans un fichier
System.IO.File.WriteAllText(Application.persistentDataPath + "/perf_report.txt", report);
```

---

## Support

Si le problème persiste :

1. **Vérifier les logs** : Chercher les erreurs dans la console Unity et les logs
2. **Tester sur un autre appareil** : Isoler les problèmes matériels
3. **Version propre** : Cloner à nouveau le repository
4. **Ouvrir une issue** : Sur GitHub avec les logs et détails

### Informations à fournir

Lors d'un rapport de bug, inclure :
- Version Unity
- Plateforme (Android/iOS) et version OS
- Modèle d'appareil
- Logs d'erreur complets
- Étapes pour reproduire

---

✅ La plupart des problèmes se résolvent avec un redémarrage de Unity ou de l'appareil !
