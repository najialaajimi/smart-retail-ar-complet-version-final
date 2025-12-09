# Quick Start Guide - Smart Retail AR

## 🚀 Getting Started (5 minutes)

### 1. Open Project in Unity
```bash
# Make sure Unity 2022.3.62f3 is installed
unity-hub://2022.3.62f3
```

### 2. Install Required Packages
In Unity Editor:
- Window → Package Manager
- Install: AR Foundation, ARCore XR Plugin, ARKit XR Plugin

### 3. Open Main Scene
- Navigate to `Assets/Scenes/`
- Open `MainMenu.unity` (once created)

### 4. Test in Editor
- Press Play
- Navigate to QR Scanner
- Press **Space** to simulate QR scan

---

## 📁 Key Files

### Configuration
- `Assets/Resources/AppConfig.asset` - App settings (create this)

### Data
- `Assets/Data/Products/products.json` - 13 sample products
- `Assets/Data/Categories/categories.json` - 6 categories

### Main Scripts
- `GameManager.cs` - Core application manager
- `QRCodeScanner.cs` - QR code scanning
- `ProductManager.cs` - Product database
- `ProductRecommendationEngine.cs` - Smart recommendations

---

## 🛠️ Editor Tools

### QR Code Generator
**Menu:** Tools → Smart Retail AR → QR Code Generator
- Generate QR codes for all products
- Output: `Assets/Data/QRCodes/`

### Scene Auto Runner
**Menu:** Tools → Smart Retail AR → Scene Auto Runner
- Test all scenes automatically

---

## 📝 Quick Commands

### Generate QR Codes
1. Tools → Smart Retail AR → QR Code Generator
2. Click "Generate All QR Codes"

### Run Tests
1. Window → General → Test Runner
2. Select PlayMode or EditMode
3. Click "Run All"

### Build for Android
1. File → Build Settings
2. Platform: Android
3. Add Scenes in order
4. Build and Run

---

## 🐛 Common Issues

**Camera doesn't open**
- Check Camera permissions in Android/iOS settings
- In Editor, press Space to simulate

**AR not working**
- Verify AR Foundation packages installed
- Check XR Plug-in Management settings
- Test on ARCore/ARKit compatible device

**No products loaded**
- Copy products.json to Assets/Resources/Data/Products/
- Check console for JSON loading errors

---

## 📚 Documentation Files

1. **README.md** - Complete project guide (read first!)
2. **SCENE_SETUP_GUIDE.md** - How to create Unity scenes
3. **PROJECT_SUMMARY.md** - Project overview

---

## 🎯 Next Steps

### For Developers
1. Read README.md completely
2. Follow SCENE_SETUP_GUIDE.md to create scenes
3. Test each scene individually
4. Create prefabs for UI elements
5. Build and test on device

### For Testing
1. Generate QR codes using the editor tool
2. Print QR codes or display on screen
3. Build app to device
4. Test scanning and AR features

---

## 💡 Tips

- **Simulation Mode:** Press Space in Editor to simulate QR scan
- **Performance:** Check PerformanceMonitor for FPS and latency
- **Logs:** DebugLogger outputs to Unity Console
- **Prefabs:** Create reusable UI prefabs for consistency

---

## 📞 Need Help?

1. Check troubleshooting in README.md
2. Review console logs (DebugLogger)
3. Check GitHub Issues

---

**Version:** 1.0.0  
**Unity:** 2022.3.62f3  
**Platform:** Android (ARCore) / iOS (ARKit)

🎉 **Ready to build amazing AR retail experiences!**
