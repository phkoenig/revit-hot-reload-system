# 🗺️ GeoJSON Importer for Revit 2026 - Hot Reload Development System

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Revit](https://img.shields.io/badge/Revit-2026-orange.svg)](https://www.autodesk.com/products/revit)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Hot Reload](https://img.shields.io/badge/Hot%20Reload-✅%20Working-brightgreen.svg)]()
[![Status](https://img.shields.io/badge/Status-Development%20Phase%201-yellow.svg)]()

> **🔥 Revolutionary Hot Reload system for Revit 2026 C# plugins + GeoJSON Import functionality**

## 🎯 Project Overview

This project combines **two major innovations**:

1. **⚡ Hot Reload System** - 6x faster Revit plugin development
2. **🗺️ GeoJSON Importer** - Import geographic data as native Revit elements

### 🚀 Current Status - Phase 1 Complete ✅

**✅ Implemented:**
- **Hot Reload System** - Loader/Work DLL Architecture
- **Revit Ribbon Integration** - Abracadabra Test Button
- **UTM Grid Setup Dialog** - Main UI with HTTP Server Map
- **MapServer Implementation** - Local HTTP Server for Leaflet
- **HTTP Server Solution** - Hot Reload-compatible mapping
- **Unit Converter Foundation** - Imperial/Metric conversion

## 🏗️ Architecture

### 🚨 KRITISCHE ARCHITEKTUR-REGEL (NIEMALS VERLETZEN!)

**Die Loader-DLL (GeoJsonImporter.dll) ist NUR ein Dummy!**

#### ✅ **RICHTIG - Loader-DLL (GeoJsonImporter.dll):**
- **NUR Revit API Referenzen** (RevitAPI.dll, RevitAPIUI.dll)
- **KEINE Package References** (kein GeoJSON.Net, kein Newtonsoft.Json!)
- **NUR Dummy-Code:** App.cs, Proxy-Commands, WorkDllManager
- **Kann NICHT ausgetauscht werden** (statisch in Revit geladen)

#### ✅ **RICHTIG - Work-DLL (GeoJsonImporter.Work.dll):**
- **ALLE Package References** (GeoJSON.Net, Newtonsoft.Json, etc.)
- **ALLE echten Commands** (Business Logic)
- **Kann zur Laufzeit ausgetauscht werden** (Hot Reload fähig)

#### ❌ **FALSCH - NIEMALS TUN:**
- **Package References in Loader-DLL hinzufügen** (zerstört Architektur!)
- **Business Logic in Loader-DLL schreiben** (zerstört Hot Reload!)
- **Work-DLL Referenzen in Loader-DLL** (verursacht "Klasse nicht gefunden")

---

### Hot Reload System
```
┌─────────────────────────────────────────┐
│           REVIT 2026 PROCESS            │
├─────────────────────────────────────────┤
│  ┌─────────────────────────────────┐   │
│  │        LOADER-DLL               │   │
│  │  (Always loaded in Revit)       │   │
│  │  ┌─────────────────────────┐    │   │
│  │  │    WorkDllManager       │    │   │
│  │  │  - UnloadWorkDll()      │    │   │
│  │  │  - LoadWorkDll()        │    │   │
│  │  │  - ExecuteAbracadabra() │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
│              │                         │
│              ▼ (Dynamic Loading)       │
│  ┌─────────────────────────────────┐   │
│  │        WORK-DLL                 │   │
│  │  (Hot-swappable)                │   │
│  │  ┌─────────────────────────┐    │   │
│  │  │ Abracadabra Command     │    │   │
│  │  │ UTM Grid Setup          │    │   │
│  │  │ GeoJSON Import          │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

## ⚡ Quick Start

### 1. Build and Deploy
```bash
# Clone repository
git clone https://github.com/YourUsername/revit-geojson-importer.git
cd revit-geojson-importer

# Build both DLLs
dotnet build
```

### 2. Start Revit
- Open Revit 2026
- Look for **"GeoJSON Importer"** toolbar
- You'll see buttons: **Abracadabra**, **Unload Work-DLL**, **Load Work-DLL**

### 3. Test Hot Reload
```bash
# 1. In Revit: Click "Load Work-DLL"
# 2. In Revit: Click "Abracadabra" (shows "2")
# 3. Make code changes in: src/GeoJsonImporter.Work/
# 4. In Revit: Click "Unload Work-DLL"
# 5. Build: cd src/GeoJsonImporter.Work && dotnet build
# 6. In Revit: Click "Load Work-DLL"
# 7. In Revit: Click "Abracadabra" (shows "3")
# 🎉 Changes are live - no Revit restart!
```

## ✅ Current Features

### ✅ Hot Reload System
- **6x faster development** - 5-10s vs 30-60s per iteration
- **No Revit restart required** - Load/Unload Work-DLL
- **Assembly isolation** - Separate LoadContext for Work-DLL
- **Memory management** - Proper cleanup and garbage collection

### ✅ GeoJSON Import Foundation
- **UTM Grid Setup** - Geographic coordinate system
- **Map Integration** - HTTP Server + Leaflet maps
- **Unit Conversion** - Imperial/Metric support
- **Revit Integration** - Native Revit elements

## 🚀 Development Workflow

### 1. **Loader-DLL (Static)**
- **Never change** - Contains UI and Proxy-Commands
- **Revit API only** - No external packages
- **WorkDllManager** - Delegates to Work-DLL

### 2. **Work-DLL (Hot Reload)**
- **Change freely** - All business logic here
- **All packages** - GeoJSON.Net, Newtonsoft.Json, etc.
- **Commands** - AbracadabraWorkCommand, SetupWorkCommand, etc.

### 3. **Hot Reload Cycle**
```
Revit → Load Work-DLL → Test → Unload Work-DLL → Change Code → Build → Load Work-DLL → Test
```

## 🏆 Success Story

**After 8 hours of troubleshooting, we discovered the key to success:**

1. **Keep Loader-DLL simple** - Only Proxy-Commands and WorkDllManager
2. **Use specific methods** - `ExecuteAbracadabra()` instead of generic `ExecuteWorkCommand(string, ...)`
3. **Avoid over-engineering** - Simple Reflection is more stable than complex logic
4. **Follow the working pattern** - Don't reinvent what already works

**The Hot Reload now works perfectly: 1 → 2 → 3 without Revit restarts!** 🎉

## 📁 Project Structure

```
GeoJson_Importer/
├── src/
│   ├── GeoJsonImporter/           # Loader-DLL (Static)
│   │   ├── Addin/
│   │   │   ├── App.cs            # Revit Ribbon
│   │   │   └── Commands/         # Proxy-Commands
│   │   └── Utils/
│   │       └── HotReloadLogger.cs
│   └── GeoJsonImporter.Work/     # Work-DLL (Hot Reload)
│       ├── Commands/              # Real Commands
│       ├── UI/                    # Dialogs and Forms
│       └── Utils/                 # Business Logic
├── WorkDll/                       # Deployed Work-DLL
└── Deploy/                        # Deployed Loader-DLL
```

## 🔧 Technical Details

### Loader-DLL (GeoJsonImporter.dll)
- **Target:** .NET 8.0 Windows
- **References:** RevitAPI.dll, RevitAPIUI.dll
- **Purpose:** UI Management + Proxy-Commands + WorkDllManager

### Work-DLL (GeoJsonImporter.Work.dll)
- **Target:** .NET 8.0 Windows
- **References:** GeoJSON.Net, Newtonsoft.Json, etc.
- **Purpose:** All Business Logic + Commands

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**🎯 Remember: The Loader-DLL is a Dummy - never add Business Logic there!**