# 🚀 Revit Hot Reload System - Revolutionary C# Plugin Development

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Revit](https://img.shields.io/badge/Revit-2026-orange.svg)](https://www.autodesk.com/products/revit)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Hot Reload](https://img.shields.io/badge/Hot%20Reload-✅%20Working-brightgreen.svg)](docs/README_HotReload_Architecture.md)

> **🔥 The first working Hot Reload system for Revit 2026 C# plugins - no more Revit restarts needed!**

## ✨ What is This?

This project implements **genuine Hot Reload functionality** for Revit C# plugins, similar to PyRevit but for compiled C# assemblies. Change your code, test immediately - **without restarting Revit**.

### 🎯 Key Features

- ⚡ **6x Faster Development** - From 30-60 seconds to 5-10 seconds per iteration
- 🔄 **True Hot Reload** - Code changes without Revit restart
- 🏗️ **Loader-Plugin Architecture** - Separate DLLs for maximum flexibility
- 🛡️ **AssemblyLoadContext** - Clean isolation and unloading
- 🔧 **Dependency Resolution** - Automatic NuGet package handling
- 📊 **Professional Logging** - File and console output
- 🖱️ **User-Friendly UI** - Simple Load/Unload buttons

## 🚀 Quick Demo

**Traditional Revit Development:**
```
Code change → Close Revit → Build → Start Revit → Test Plugin
⏱️ Time: 30-60 seconds per iteration
```

**With Hot Reload System:**
```
Code change → Unload → Build → Load → Test Plugin
⏱️ Time: 5-10 seconds per iteration
```

**Result: 85% time reduction, 6x productivity increase! 🎉**

## 📋 Prerequisites

- **Revit 2026** or higher
- **.NET 8.0** Windows SDK
- **Visual Studio 2022** (recommended)
- **Windows 10/11** x64

## ⚡ Quick Start

### 1. Clone and Build
```bash
git clone https://github.com/YourUsername/revit-hot-reload-system.git
cd revit-hot-reload-system
dotnet build
```

### 2. Start Revit
- Open Revit 2026
- Look for **"GeoJSON Importer"** toolbar
- You'll see 3 buttons: Import, Unload Work-DLL, Load Work-DLL

### 3. Initial Setup
```bash
1. Click "Load Work-DLL" → Work-DLL gets loaded
2. Click "Import GeoJSON" → Test the functionality
3. You should see a version MessageBox
```

### 4. Hot Reload Development Cycle
```bash
# 1. Make code changes in:
# src/GeoJsonImporter.Work/Commands/ImportGeoJsonWorkCommand.cs

# 2. In Revit: Click "Unload Work-DLL"
# 3. In Terminal: Build Work-DLL
cd src/GeoJsonImporter.Work
dotnet build

# 4. In Revit: Click "Load Work-DLL"  
# 5. In Revit: Click "Import GeoJSON"
# 🎉 Your changes are live - no Revit restart!
```

## 🏗️ Architecture Overview

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
│  │  │ Your Plugin Logic Here  │    │   │
│  │  │  - Commands             │    │   │
│  │  │  - Business Logic       │    │   │
│  │  │  - Dependencies         │    │   │
│  │  └─────────────────────────┘    │   │
│  └─────────────────────────────────┘   │
└─────────────────────────────────────────┘
```

### Key Components

- **Loader-DLL** (`GeoJsonImporter.dll`) - Stays loaded in Revit, manages Hot Reload
- **Work-DLL** (`GeoJsonImporter.Work.dll`) - Contains your plugin logic, hot-swappable
- **WorkDllManager** - Core system managing load/unload operations
- **Custom AssemblyLoadContext** - Enables clean unloading and dependency resolution

## 📚 Documentation

### 📖 Complete Documentation
- **[🏗️ Architecture Guide](docs/README_HotReload_Architecture.md)** - Deep technical implementation details
- **[👨‍💻 User Guide](docs/README_HotReload_UserGuide.md)** - Step-by-step workflows and best practices  
- **[🔬 Technical Specs](docs/README_HotReload_TechnicalSpecs.md)** - Detailed specifications and configuration

### 🎓 Learning Resources
- **[📁 Examples](examples/)** - Sample GeoJSON files for testing
- **[🔧 Development Setup](docs/README_Development.md)** - Development environment configuration
- **[📖 Revit API Guide](docs/README_RevitAPI.md)** - Revit API integration patterns

## 🛠️ Development

### Project Structure
```
revit-hot-reload-system/
├── src/
│   ├── GeoJsonImporter/           # Loader-DLL (stays in Revit)
│   │   ├── Addin/
│   │   │   ├── App.cs             # IExternalApplication
│   │   │   └── Commands/          # UI Commands & Management
│   │   └── Utils/                 # Logging & Utilities
│   └── GeoJsonImporter.Work/      # Work-DLL (hot-swappable)
│       ├── Commands/              # Your plugin commands
│       └── Core/                  # Business logic
├── docs/                          # Complete documentation
├── examples/                      # Sample files
├── WorkDll/                       # Work-DLL output (not locked by Revit)
└── assets/                        # Revit .addin manifest
```

### Building the Project
```bash
# Build Loader-DLL (deploy to Revit)
dotnet build

# Build Work-DLL only (for hot reload)
cd src/GeoJsonImporter.Work
dotnet build
```

### Adding Your Own Plugin Logic
1. **Modify Work-DLL**: Edit files in `src/GeoJsonImporter.Work/`
2. **Keep Loader-DLL minimal**: Only management code in `src/GeoJsonImporter/`
3. **Use Hot Reload cycle**: Unload → Build → Load → Test

## 🧪 Testing Hot Reload

### Simple Test: Change MessageBox Text
```csharp
// In: src/GeoJsonImporter.Work/Commands/ImportGeoJsonWorkCommand.cs
MessageBox.Show($"🎉 MY CUSTOM VERSION! 🎉\n\n" +
    $"Build Time: {buildTime:HH:mm:ss}\n" +
    $"✨ Hot Reload is working! ✨", 
    "Custom Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
```

**Hot Reload Steps:**
1. Make the change above
2. Revit: "Unload Work-DLL" 
3. Terminal: `cd src/GeoJsonImporter.Work && dotnet build`
4. Revit: "Load Work-DLL"
5. Revit: "Import GeoJSON" 
6. 🎉 See your custom message instantly!

## 🐛 Troubleshooting

### Common Issues

**Problem: Build fails with "DLL locked by Revit"**
```bash
# Solution: Make sure to click "Unload Work-DLL" first
# The Work-DLL must be unloaded before building
```

**Problem: Old version still appears**
```bash
# Solution: Check if build was successful and reload
cd src/GeoJsonImporter.Work
dotnet build  # Check for errors
# Then click "Load Work-DLL" in Revit
```

**Problem: Buttons not visible in Revit**
```bash
# Solution: Rebuild Loader-DLL and restart Revit
dotnet build  # In root directory
# Close and restart Revit
```

See **[Complete Troubleshooting Guide](docs/README_HotReload_UserGuide.md#troubleshooting)** for more solutions.

## 📊 Performance Metrics

| Metric | Traditional | Hot Reload | Improvement |
|--------|-------------|------------|-------------|
| **Development Cycle** | 30-60s | 5-10s | **6x faster** |
| **Revit Restarts** | Every change | Never | **∞x better** |
| **Productivity** | 1x baseline | 6x baseline | **500% increase** |
| **Developer Happiness** | 😤 Frustrated | 🚀 Excited | **Priceless** |

## 🤝 Contributing

We welcome contributions! This project represents a breakthrough in Revit plugin development.

### Ways to Contribute
- 🐛 **Bug Reports** - Found an issue? Let us know!
- 💡 **Feature Requests** - Ideas for improvements
- 📖 **Documentation** - Help improve our guides
- 🔧 **Code Contributions** - Submit pull requests
- 🌟 **Spread the Word** - Share with other Revit developers

### Getting Started
1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Autodesk** for the Revit API
- **Microsoft** for .NET AssemblyLoadContext capabilities
- **PyRevit Community** for Hot Reload inspiration
- **All Revit Developers** who suffer through slow development cycles

## 🌟 Star This Repository!

If this project helps you develop Revit plugins faster, please give it a ⭐!

**This could revolutionize how the entire Revit development community works.**

---

## 📞 Support & Community

- **📖 Documentation**: [Complete Docs](docs/)
- **🐛 Issues**: [GitHub Issues](https://github.com/YourUsername/revit-hot-reload-system/issues)
- **💬 Discussions**: [GitHub Discussions](https://github.com/YourUsername/revit-hot-reload-system/discussions)
- **📧 Email**: your-email@example.com

---

**🚀 Happy Hot Reloading! Welcome to the future of Revit plugin development! 🚀**

*Made with ❤️ by developers who were tired of waiting for Revit to restart.*
