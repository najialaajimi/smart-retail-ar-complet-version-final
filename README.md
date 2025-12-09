# Smart Retail AR - Application de Réalité Augmentée pour Commerce de Détail

[![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f3-blue)](https://unity.com/)
[![AR Foundation](https://img.shields.io/badge/AR%20Foundation-5.1.0-green)](https://unity.com/unity/features/arfoundation)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📱 Description

Smart Retail AR est une application de réalité augmentée innovante conçue pour améliorer l'expérience client en magasin. Elle permet de scanner des produits (via QR code ou reconnaissance d'image) et d'afficher en temps réel des informations enrichies :

- **Informations nutritionnelles** : Nutri-Score, calories, protéines, lipides, etc.
- **Score écologique** : Impact environnemental du produit
- **Origine** : Provenance du produit
- **Alternatives intelligentes** : Recommandations de produits similaires plus sains ou plus écologiques

## ✨ Fonctionnalités Principales

### 🔍 Scan de Produits
- Scanner QR codes avec la caméra
- Reconnaissance d'images de produits
- Détection rapide et précise (≥ 95% de taux de reconnaissance)

### 🌟 Affichage AR
- Overlay d'informations en réalité augmentée
- Nutri-Score visuel (A-E)
- Éco-Score en barre de progression
- Badges Bio et Éco-responsable

### 🎯 Recommandations Intelligentes
- Algorithme multi-critères (prix, nutrition, écologie)
- Filtres personnalisables
- Alternatives bio et éco-responsables
- Comparaison de produits

### ⚙️ Paramètres
- Configuration AR (tracking, fréquence de scan)
- Préférences utilisateur
- Mode debug

## 🛠️ Prérequis

### Logiciels
- **Unity 2022.3.62f3** (obligatoire)
- Android Studio (pour build Android) ou Xcode (pour build iOS)
- Git

### Matériel
- Appareil Android compatible ARCore ou iOS compatible ARKit
- Caméra fonctionnelle
- Minimum 2 GB RAM

## 📦 Installation

### 1. Cloner le Repository

```bash
git clone https://github.com/najialaajimi/smart-retail-ar-complet-version-final.git
cd smart-retail-ar-complet-version-final
```

### 2. Ouvrir avec Unity Hub

1. Ouvrir Unity Hub
2. Cliquer sur "Add" et sélectionner le dossier du projet
3. S'assurer que Unity 2022.3.62f3 est installé
4. Ouvrir le projet

### 3. Importer les Packages

Les packages sont définis dans `Packages/manifest.json` et seront automatiquement importés :
- AR Foundation 5.1.0
- ARCore XR Plugin 5.1.0
- ARKit XR Plugin 5.1.0
- TextMeshPro 3.0.6
- Input System 1.5.1
- Test Framework 1.3.4

### 4. Configurer les Player Settings

#### Pour Android
1. **File > Build Settings > Android**
2. **Player Settings**:
   - Minimum API Level: Android 7.0 (API 24)
   - Target API Level: Android 13 (API 33)
   - Scripting Backend: IL2CPP
   - Target Architectures: ARM64
3. **XR Settings**:
   - Activer AR Foundation
   - Ajouter ARCore Support

#### Pour iOS
1. **File > Build Settings > iOS**
2. **Player Settings**:
   - Minimum iOS Version: 11.0
   - Target SDK: Device SDK
   - Architecture: ARM64
3. **XR Settings**:
   - Activer AR Foundation
   - Ajouter ARKit Support

## 🎮 Guide des Scènes

### MainMenu.unity
**Menu principal de l'application**
- Navigation vers toutes les fonctionnalités
- Composants requis : MainMenuController

### QRScanner.unity
**Scène de scan de QR codes**
- Active la caméra
- Détecte et décode les QR codes
- Composants requis : QRCodeScanner, ProductDatabase

### ARProductView.unity
**Vue AR avec informations produit**
- Affiche l'overlay AR
- Tracking d'images
- Composants requis : ARSessionManager, ARProductOverlay, ImageTrackingManager

### Recommendations.unity
**Affichage des recommandations**
- Liste des alternatives
- Filtres de recherche
- Composants requis : RecommendationUI, RecommendationEngine

### Settings.unity
**Paramètres de l'application**
- Configuration AR
- Préférences utilisateur
- Composants requis : SettingsPanel, AppSettings

## 🏗️ Architecture du Projet

```
Assets/
├── Scenes/              # Scènes Unity
├── Scripts/
│   ├── Core/            # Gestionnaires principaux
│   ├── QRCode/          # Scanner QR
│   ├── AR/              # Fonctionnalités AR
│   ├── Products/        # Modèles de produits
│   ├── Recommendations/ # Moteur de recommandations
│   ├── UI/              # Contrôleurs d'interface
│   ├── Data/            # Gestion des données
│   └── Utils/           # Utilitaires
├── Data/                # Fichiers JSON
├── Resources/           # Ressources chargées dynamiquement
├── Prefabs/             # Prefabs UI et AR
├── Editor/              # Scripts éditeur
└── Tests/               # Tests unitaires
```

## 💾 Base de Données Produits

### Format JSON

Le fichier `Assets/Data/products.json` contient la base de données avec 12 produits variés.

### Ajouter un Nouveau Produit

1. Éditer `Assets/Data/products.json`
2. Ajouter un nouvel objet produit
3. Générer le QR code via **Tools > Smart Retail AR > QR Code Generator**

## 🔧 Outils Éditeur

### QR Code Generator
**Menu : Tools > Smart Retail AR > QR Code Generator**

Génère des QR codes pour vos produits :
- Génération individuelle ou en masse
- Sauvegarde automatique dans `Assets/Resources/QRCodes/`
- Aperçu en temps réel

### Scene Setup Wizard
**Menu : Tools > Smart Retail AR > Scene Setup Wizard**

Assistant de création de scènes préconfigurées.

## 🧪 Tests

### Exécuter les Tests

#### Tests EditMode (tests unitaires)
```
Window > General > Test Runner > EditMode > Run All
```

Tests disponibles :
- `ProductDatabaseTests` : Tests de la base de données
- `RecommendationEngineTests` : Tests du moteur de recommandations

#### Tests PlayMode (tests d'intégration)
```
Window > General > Test Runner > PlayMode > Run All
```

Tests disponibles :
- `QRScannerTests` : Tests du scanner QR
- `AROverlayTests` : Tests de l'overlay AR

## 📊 KPIs et Performance

### Objectifs de Performance

- **Latence d'affichage** : ≤ 1 seconde
- **Taux de reconnaissance** : ≥ 95%
- **Framerate** : ≥ 30 FPS

### Monitoring

Le composant `PerformanceMonitor` mesure en temps réel les métriques clés.

## 📚 Documentation Additionnelle

- [SETUP_GUIDE.md](Documentation/SETUP_GUIDE.md) - Guide d'installation détaillé
- [SCENE_GUIDE.md](Documentation/SCENE_GUIDE.md) - Guide des scènes
- [API_REFERENCE.md](Documentation/API_REFERENCE.md) - Référence API
- [TROUBLESHOOTING.md](Documentation/TROUBLESHOOTING.md) - Dépannage

## 👥 Auteurs

- **Najia Laajimi** - [GitHub](https://github.com/najialaajimi)

---

**Version** : 1.0.0  
**Dernière mise à jour** : Décembre 2024  
**Status** : Production Ready ✅