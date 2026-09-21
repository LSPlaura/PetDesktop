# 🐾 PetDesktop

An interactive virtual desktop pet application built with **Avalonia UI (.NET 10)**. Features smooth sprite animations, autonomous walking behavior, screen edge detection, drag-and-drop support, and configurable pet switching.

---

## ✨ Key Features

* **Configurable Pets:** Switch between **Eevee** and **Pingu** by configuring the `appsettings.json` file.
* **Autonomous Movement:** Screen edge detection and real-time walking/idle behaviors.
* **Interactive Floating Window:** Transparent, Always-on-Top desktop window with full drag-and-drop support.
* **Optimized Rendering:** Smooth sprite rendering using Avalonia's native `CroppedBitmap` to minimize memory usage and Garbage Collector overhead.
* **Standalone Binary:** Self-contained executable support — no pre-installed .NET runtime required.

---

## 🛠️ Architecture & Technologies

PetDesktop follows a clean, decoupled architecture separating UI logic from domain models:

* **UI Framework:** Avalonia UI (C# / XAML)
* **Design Pattern:** MVVM (Model-View-ViewModel) with `CommunityToolkit.Mvvm`
* **Persistence & Database:** SQLite with Entity Framework Core
* **Logging:** Serilog for console and file logging
* **Asset Pipeline:** Native SkiaSharp pipeline for automated SpriteSheet generation

---

## ⚙️ Configuration (`appsettings.json`)

You can change your active pet before launching the app by editing the `"DefaultPet"` block in `appsettings.json`:

### 🦊 Eevee
```json
"DefaultPet": {
  "Name": "Evee",
  "InitialAnimation": "DefaultRight",
  "Route": "Assets/EveeDefault",
  "FrameWidth": 64,
  "FrameHeight": 64
}
```

### 🐧 Pingu
```json
"DefaultPet": {
  "Name": "Pingu",
  "InitialAnimation": "DefaultRight",
  "Route": "Assets/DefaultPet",
  "FrameWidth": 128,
  "FrameHeight": 128
}
```
---

## 🎨 Credits & Asset Attributions

The sprite assets and icons used in this project were gathered from publicly available sources created by talented community artists:

* **Pingu Assets:** Derived from the [Penguin Skin Sprite Sheet](https://spelunky.fyi/mods/m/penguin-skin-sprite-sheet/) on Spelunky.fyi.
* **Eevee Walk Assets:** Derived from [Eevee Walk Sprite (F2U)](https://www.deviantart.com/starwolff-nyota/art/Eevee-Walk-Sprite-F2U-1112113629) created by Starwolff-Nyota on DeviantArt.
* **Eevee Icon:** Derived from [Eevee Pixel Art Icon](https://dinopixel.com/eevee-pixel-art-pixel-art-13697) on DinoPixel.

> **Note for Content Creators:** All assets are used for non-commercial, educational, and showcase purposes. If you are the original owner/creator of any asset used here and have concerns or would like your work removed or credited differently, please open an Issue or contact me directly, and I will be glad to resolve it immediately.
>
---
## 📜 License

This project is licensed under the **MIT License** — you are free to use, modify, and distribute this software as long as you provide attribution to the original author.