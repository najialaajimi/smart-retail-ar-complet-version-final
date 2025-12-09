# Guide d'Installation - Smart Retail AR

## Table des Matières

1. [Configuration Système](#configuration-système)
2. [Installation Unity](#installation-unity)
3. [Configuration du Projet](#configuration-du-projet)
4. [Build Android](#build-android)
5. [Build iOS](#build-ios)
6. [Vérification](#vérification)

## Configuration Système

### Minimum Requis

- **OS** : Windows 10 64-bit, macOS 10.13+, ou Linux Ubuntu 18.04+
- **RAM** : 8 GB minimum (16 GB recommandé)
- **Espace disque** : 10 GB disponible
- **Processeur** : Intel Core i5 ou équivalent

### Pour Build Android

- Android Studio 2021.1.1+
- Android SDK Platform API 24 minimum
- Android NDK (installé via Android Studio)
- Java JDK 11

### Pour Build iOS

- macOS 10.15+
- Xcode 13+
- Compte développeur Apple (pour tester sur appareil)

## Installation Unity

### 1. Télécharger Unity Hub

Télécharger depuis : https://unity.com/download

### 2. Installer Unity 2022.3.62f3

1. Ouvrir Unity Hub
2. Aller dans "Installs"
3. Cliquer sur "Install Editor"
4. Sélectionner la version **2022.3.62f3**
5. Cocher les modules :
   - Android Build Support
   - iOS Build Support (si sur macOS)
   - Documentation

## Configuration du Projet

### 1. Cloner le Repository

```bash
git clone https://github.com/najialaajimi/smart-retail-ar-complet-version-final.git
cd smart-retail-ar-complet-version-final
```

### 2. Ouvrir le Projet

1. Lancer Unity Hub
2. Cliquer sur "Add" > Sélectionner le dossier du projet
3. Ouvrir le projet avec Unity 2022.3.62f3

### 3. Vérifier les Packages

Unity va automatiquement importer les packages définis dans `Packages/manifest.json`.

Vérifier dans **Window > Package Manager** :
- ✅ AR Foundation 5.1.0
- ✅ ARCore XR Plugin 5.1.0
- ✅ ARKit XR Plugin 5.1.0
- ✅ TextMeshPro 3.0.6

## Build Android

### 1. Configurer Android SDK

1. **Edit > Preferences > External Tools**
2. Définir les chemins :
   - Android SDK
   - Android NDK
   - JDK

### 2. Player Settings

1. **File > Build Settings > Android**
2. **Player Settings** :

```
Company Name: YourCompany
Product Name: Smart Retail AR
Bundle Identifier: com.yourcompany.smartretailar
Version: 1.0.0
Minimum API Level: Android 7.0 (API 24)
Target API Level: Android 13 (API 33)
```

3. **Other Settings** :
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64 (cocher)
   - Internet Access: Require

4. **XR Plug-in Management** :
   - Cocher "ARCore"

### 3. Permissions Android

Le fichier `AndroidManifest.xml` doit inclure :

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-feature android:name="android.hardware.camera.ar" android:required="true"/>
```

### 4. Build et Run

1. Connecter un appareil Android via USB
2. Activer le "Mode Développeur" sur l'appareil
3. **File > Build Settings > Build And Run**

## Build iOS

### 1. Player Settings

1. **File > Build Settings > iOS**
2. **Player Settings** :

```
Company Name: YourCompany
Product Name: Smart Retail AR
Bundle Identifier: com.yourcompany.smartretailar
Version: 1.0.0
iOS Version: 11.0
Target SDK: Device SDK
Architecture: ARM64
```

3. **XR Plug-in Management** :
   - Cocher "ARKit"

### 2. Info.plist

Ajouter la description d'usage de la caméra :

```xml
<key>NSCameraUsageDescription</key>
<string>Cette app nécessite la caméra pour scanner les produits en AR</string>
```

### 3. Build Xcode Project

1. **File > Build Settings > Build**
2. Choisir un dossier de destination
3. Ouvrir le projet Xcode généré
4. Connecter un iPhone/iPad
5. Sélectionner l'appareil dans Xcode
6. Build and Run depuis Xcode

## Vérification

### Test de Base

1. Lancer l'application
2. Vérifier que le menu principal s'affiche
3. Accéder au scanner QR
4. Autoriser l'accès caméra
5. Scanner un QR code de test

### Test AR

1. Aller dans ARProductView
2. Vérifier que la session AR s'initialise
3. Pointer la caméra vers une surface plane
4. Vérifier la détection de plans

### Test Base de Données

Dans la console Unity :

```csharp
ProductDatabase.Instance.LoadFromJson();
Debug.Log($"Produits chargés: {ProductDatabase.Instance.GetAllProducts().Count}");
```

Devrait afficher : "Produits chargés: 12"

## Résolution de Problèmes

### Erreur : "AR Foundation not found"

**Solution** : Réinstaller le package AR Foundation
```
Window > Package Manager > AR Foundation > Install
```

### Erreur : "Android SDK not found"

**Solution** : Configurer le chemin SDK dans External Tools

### Erreur : "IL2CPP error"

**Solution** : Installer Visual Studio avec support C++

### Build iOS échoue

**Solution** : 
1. Vérifier la version Xcode (13+)
2. Nettoyer le build folder
3. Rebuild depuis Xcode

## Support

En cas de problème persistant :
1. Consulter [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
2. Ouvrir une issue sur GitHub
3. Vérifier les logs Unity (`Editor.log`)

---

✅ Installation terminée ! Voir [README.md](../README.md) pour l'utilisation.
