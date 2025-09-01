# 🗺️ GeoJSON Importer for Revit 2026 - Hot Reload Development System

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Revit](https://img.shields.io/badge/Revit-2026-orange.svg)](https://www.autodesk.com/products/revit)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Hot Reload](https://img.shields.io/badge/Hot%20Reload-✅%20Working-brightgreen.svg)](docs/README_HotReload_Architecture.md)
[![Status](https://img.shields.io/badge/Status-Development%20Phase%201-yellow.svg)]()

> **🔥 Revolutionary Hot Reload system for Revit 2026 C# plugins + GeoJSON Import functionality**

## 🎯 Project Overview

This project combines **two major innovations**:

1. **⚡ Hot Reload System** - 6x faster Revit plugin development
2. **🗺️ GeoJSON Importer** - Import geographic data as native Revit elements

### 🚀 Current Status - Phase 1 Complete ✅

**✅ Implemented:**
- **Hot Reload System** - Loader/Work DLL Architecture
- **Revit Ribbon Integration** - UTM Grid Setup Button
- **Revit File Validation Dialog** - Pre-check for Revit axioms
- **UTM Grid Setup Dialog** - Main UI with HTTP Server Map
- **MapServer Implementation** - Local HTTP Server for Leaflet
- **HTTP Server Solution** - Hot Reload-compatible mapping
- **Unit Converter Foundation** - Imperial/Metric conversion

**🚫 Abandoned:**
- **WebView2** - Incompatible with Hot Reload
- **CefSharp** - DLL locking issues
- **Embedded Browser** - Too complex for Hot Reload

**🎯 Next Steps:**
1. **Project Base Point & Survey Point Setup**
2. **Geographic North Correction**
3. **GeoJSON Import System**
4. **Shared Parameters Mapping UI**

## 🏗️ Architecture

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
│  │  │  - ExecuteWorkCommand() │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
│              │                         │
│              ▼ (Dynamic Loading)       │
│  ┌─────────────────────────────────┐   │
│  │        WORK-DLL                 │   │
│  │  (Hot-swappable)                │   │
│  │  ┌─────────────────────────┐    │   │
│  │  │ UTM Grid Setup          │    │   │
│  │  │ GeoJSON Import          │    │   │
│  │  │ Map Integration         │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

### Map Integration
```
┌─────────────────────────────────────────┐
│           REVIT PLUGIN                  │
├─────────────────────────────────────────┤
│  ┌─────────────────────────────────┐   │
│  │    UTM Grid Setup Dialog        │   │
│  │  ┌─────────────────────────┐    │   │
│  │  │   MapServer (HTTP)      │    │   │
│  │  │   - Local Server        │    │   │
│  │  │   - Leaflet/OSM         │    │   │
│  │  │   - Hot Reload Safe     │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
│              │                         │
│              ▼ (System Browser)        │
│  ┌─────────────────────────────────┐   │
│  │    System Browser               │   │
│  │  - Leaflet Map                  │   │
│  │  - OpenStreetMap Tiles          │   │
│  │  - Interactive Grid Selection   │   │
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
- You'll see buttons: **UTM Grid Setup**, **Unload Work-DLL**, **Load Work-DLL**

### 3. Test Hot Reload
```bash
# 1. In Revit: Click "Load Work-DLL"
# 2. In Revit: Click "UTM Grid Setup"
# 3. Make code changes in: src/GeoJsonImporter.Work/
# 4. In Revit: Click "Unload Work-DLL"
# 5. Build: cd src/GeoJsonImporter.Work && dotnet build
# 6. In Revit: Click "Load Work-DLL"
# 🎉 Changes are live - no Revit restart!
```

## ✅ Current Features

### ✅ Hot Reload System
- **6x faster development** - 5-10s vs 30-60s per iteration
- **True hot reload** - No Revit restarts needed
- **Clean architecture** - Loader/Work DLL separation
- **AssemblyLoadContext** - Proper unloading and isolation

### ✅ UTM Grid Setup
- **Revit File Validation** - Pre-check dialog for Revit axioms
- **Interactive Map** - HTTP Server with Leaflet/OpenStreetMap
- **Coordinate Display** - WGS84 and UTM coordinates
- **Grid Configuration** - Size and zone selection
- **Hot Reload Compatible** - HTTP Server solution

### ✅ Map Integration
- **Local HTTP Server** - `http://localhost:8080/map`
- **Leaflet/OpenStreetMap** - Interactive mapping
- **System Browser** - No embedded browser issues
- **Hot Reload Safe** - No DLL locking problems

## 🎯 Next Development Phase

### Phase 2: Project Setup Automation
1. **Project Base Point Setup** - Automatic positioning
2. **Survey Point Setup** - Geographic coordinate system
3. **Geographic North Correction** - True north alignment
4. **UTM Grid Generation** - Reference planes creation

### Phase 3: GeoJSON Import System
1. **GeoJSON Parser** - Feature extraction
2. **Shared Parameters Mapping** - Dynamic parameter creation
3. **Revit Element Creation** - Native Revit elements
4. **Coordinate Transformation** - UTM to Revit coordinates

## 🛠️ Development

### Project Structure
```
revit-geojson-importer/
├── src/
│   ├── GeoJsonImporter/           # Loader-DLL (stays in Revit)
│   │   ├── Addin/
│   │   │   ├── App.cs             # IExternalApplication
│   │   │   └── Commands/          # UI Commands & Management
│   │   └── Utils/                 # Logging & Utilities
│   └── GeoJsonImporter.Work/      # Work-DLL (hot-swappable)
│       ├── Commands/              # Plugin commands
│       ├── UI/                    # WPF dialogs
│       │   ├── RevitFileValidationForm.cs
│       │   └── UtmGridSetupDialog.xaml
│       └── Utils/                 # Business logic
│           └── MapServer.cs       # HTTP Server for maps
├── WorkDll/                       # Work-DLL output
├── Deploy/                        # Revit addin files
└── docs/                          # Documentation
```

### Building
```bash
# Build Loader-DLL (deploy to Revit)
dotnet build

# Build Work-DLL only (for hot reload)
cd src/GeoJsonImporter.Work
dotnet build
```

## 🐛 Troubleshooting

### Common Issues

**Problem: Build fails with "DLL locked by Revit"**
```bash
# Solution: Click "Unload Work-DLL" in Revit first
# The Work-DLL must be unloaded before building
```

**Problem: Map doesn't load**
```bash
# Solution: Check if HTTP Server started
# Look for "MapServer started on http://localhost:8080" in debug output
```

**Problem: Buttons not visible in Revit**
```bash
# Solution: Rebuild Loader-DLL and restart Revit
dotnet build  # In root directory
# Close and restart Revit
```

## 📊 Performance Metrics

| Metric | Traditional | Hot Reload | Improvement |
|--------|-------------|------------|-------------|
| **Development Cycle** | 30-60s | 5-10s | **6x faster** |
| **Revit Restarts** | Every change | Never | **∞x better** |
| **Productivity** | 1x baseline | 6x baseline | **500% increase** |

## 🤝 Contributing

This project represents a breakthrough in Revit plugin development. We welcome contributions!

### Ways to Contribute
- 🐛 **Bug Reports** - Found an issue? Let us know!
- 💡 **Feature Requests** - Ideas for improvements
- 📖 **Documentation** - Help improve our guides
- 🔧 **Code Contributions** - Submit pull requests

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**🚀 Happy Hot Reloading! Welcome to the future of Revit plugin development! 🚀**

*Made with ❤️ by developers who were tired of waiting for Revit to restart.*
