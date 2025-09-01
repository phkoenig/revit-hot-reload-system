# Hot Reload Setup für Revit 2026 C# Plugin

Dieses Dokument beschreibt die erfolgreiche Implementierung eines Hot Reload-Setups für das GeoJSON Importer Plugin in Revit 2026.

## ✅ **Was funktioniert**

### **1. Hot Reload Konfiguration**
- **AssemblyLoadContext="Separate"**: Ermöglicht das dynamische Neuladen von Plugins ohne Revit-Neustart
- **Automatische Bereitstellung**: Post-Build-Events kopieren Plugin-Dateien in den Revit AddIns-Ordner
- **Schnelle Entwicklung**: Code-Änderungen → Build → Plugin lädt automatisch neu

### **2. Projektstruktur**
```
GeoJson_Importer/
├── src/GeoJsonImporter/
│   ├── Addin/
│   │   ├── App.cs                    # IExternalApplication
│   │   └── Commands/
│   │       └── ImportGeoJsonCommand.cs # IExternalCommand
│   └── Core/
│       ├── GeoJsonParser.cs          # GeoJSON-Parsing
│       └── RevitGeometryCreator.cs   # Revit-Geometrie-Erstellung
├── assets/
│   └── GeoJsonImporter.addin         # Revit Add-In Manifest
├── docs/                             # Dokumentation
├── examples/                          # Beispiel-Dateien
└── temp/                             # Test-Skripte
```

### **3. Erfolgreiche Bereitstellung**
Alle Dateien sind korrekt im Revit AddIns-Ordner platziert:
- `C:\ProgramData\Autodesk\Revit\Addins\2026\GeoJsonImporter.dll`
- `C:\ProgramData\Autodesk\Revit\Addins\2026\GeoJsonImporter.addin`
- `C:\ProgramData\Autodesk\Revit\Addins\2026\GeoJSON.Net.dll`
- `C:\ProgramData\Autodesk\Revit\Addins\2026\Newtonsoft.Json.dll`

## 🔧 **Wichtige Konfigurationen**

### **1. Projektdatei (GeoJsonImporter.csproj)**
```xml
<PropertyGroup>
  <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
  <PlatformTarget>x64</PlatformTarget>
  <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
</PropertyGroup>
```

### **2. Add-In Manifest (.addin)**
```xml
<AddIn Type="Application">
  <Assembly>GeoJsonImporter.dll</Assembly>
  <FullClassName>GeoJsonImporter.Addin.App</FullClassName>
  <AssemblyLoadContext>Separate</AssemblyLoadContext>
</AddIn>
```

## ⚠️ **Wichtige Hinweise**

### **1. NuGet-Abhängigkeiten**
**WICHTIG**: NuGet-Pakete werden NICHT automatisch in den Revit AddIns-Ordner kopiert. Sie müssen manuell kopiert werden:

```cmd
# GeoJSON.Net kopieren
copy "C:\Users\[USERNAME]\.nuget\packages\geojson.net\1.4.1\lib\netstandard2.0\*.dll" "C:\ProgramData\Autodesk\Revit\Addins\2026\"

# Newtonsoft.Json kopieren  
copy "C:\Users\[USERNAME]\.nuget\packages\newtonsoft.json\13.0.3\lib\netstandard2.0\*.dll" "C:\ProgramData\Autodesk\Revit\Addins\2026\"
```

### **2. Revit muss geschlossen sein**
Beim ersten Kopieren der DLLs muss Revit geschlossen sein, da es die Dateien sperrt.

### **3. Assembly-Versionen**
Stelle sicher, dass die kopierten DLL-Versionen mit den im Projekt referenzierten Versionen übereinstimmen.

## 🚀 **Entwicklungsworkflow**

### **1. Code ändern**
- Bearbeite die C#-Dateien in `src/GeoJsonImporter/`

### **2. Projekt bauen**
```cmd
dotnet build
```

### **3. Automatische Bereitstellung**
- Plugin-DLL wird automatisch in Revit AddIns-Ordner kopiert
- NuGet-Abhängigkeiten müssen nur einmal manuell kopiert werden

### **4. Plugin testen**
- Starte Revit 2026
- Gehe zu "GeoTools" → "Import GeoJSON"
- Plugin lädt automatisch neu bei Code-Änderungen

## 🐛 **Fehlerbehebung**

### **Problem: "Could not load file or assembly 'GeoJSON.Net'**
**Lösung**: Kopiere die NuGet-DLLs manuell in den Revit AddIns-Ordner

### **Problem: "File is being used by another process"**
**Lösung**: Schließe Revit vor dem Kopieren der DLLs

### **Problem: Plugin erscheint nicht in Revit**
**Lösung**: Überprüfe die `.addin`-Datei und stelle sicher, dass alle Pfade korrekt sind

## 📚 **Nützliche Befehle**

```cmd
# Projekt bauen
dotnet build

# NuGet-Pakete auflisten
dotnet list package

# NuGet-Cache-Pfade anzeigen
dotnet nuget locals all --list

# Revit AddIns-Ordner überprüfen
dir "C:\ProgramData\Autodesk\Revit\Addins\2026\"
```

---

**Status**: ✅ Hot Reload Setup erfolgreich implementiert und getestet!
**Nächster Schritt**: GeoJSON-Import-Logik implementieren und testen.
