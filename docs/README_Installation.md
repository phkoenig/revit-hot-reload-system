# Revit 2026 API SDK Installation & Tests

## 📍 **Installationspfade**

### **Revit 2026 Hauptinstallation**
- **Pfad:** `C:\Program Files\Autodesk\Revit 2026\`
- **Status:** ✅ Installiert und funktionsfähig
- **Version:** 2026 (aktuellste)

### **Wichtige API-DLLs**
- **RevitAPI.dll:** `C:\Program Files\Autodesk\Revit 2026\RevitAPI.dll`
- **RevitAPIUI.dll:** `C:\Program Files\Autodesk\Revit 2026\RevitAPIUI.dll`
- **Status:** ✅ Verfügbar für Referenzierung

### **AddIns-Ordner**
- **Pfad:** `C:\ProgramData\Autodesk\Revit\Addins\2026\`
- **Status:** ✅ Bereits konfiguriert
- **Bestehende AddIns:** Enscape, BatchPrint, eTransmit, etc.

## 🧪 **Test-Skripte**

### **Test-Skripte Ordner**
- **Pfad:** `b:\Nextcloud\CODE\revit-plugins\GeoJson_Importer\temp\`
- **Zweck:** Alle Test-Skripte, Debugging-Code und temporäre Entwicklungsdateien

### **Aktuelle Test-Skripte**

#### **1. test_revit_api.cs**
- **Pfad:** `temp/test_revit_api.cs`
- **Zweck:** Testet, ob die Revit DLLs korrekt geladen werden können
- **Funktionen:**
  - Prüft Existenz der Revit DLLs
  - Versucht Assembly-Loading
  - Zeigt Version und Architektur an
  - Fehlerbehandlung bei Problemen

#### **2. test_script.cs**
- **Pfad:** `temp/test_script.cs`
- **Zweck:** Allgemeines Test-Skript für GeoJSON-Import
- **Funktionen:**
  - Lädt Beispiel-GeoJSON-Dateien
  - Grundlegende Funktionalität testen

## 🔧 **Projekt-Konfiguration**

### **C#-Projektdatei**
- **Datei:** `GeoJsonImporter.csproj`
- **Framework:** .NET 8
- **Plattform:** x64
- **Revit-Referenzen:** Korrekt konfiguriert

### **Wichtige Einstellungen**
```xml
<PropertyGroup>
  <PlatformTarget>x64</PlatformTarget>
  <ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch>None</ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch>
</PropertyGroup>

<Reference Include="RevitAPI">
  <HintPath>C:\Program Files\Autodesk\Revit 2026\RevitAPI.dll</HintPath>
  <Private>False</Private>
  <EmbedInteropTypes>False</EmbedInteropTypes>
</Reference>
```

## 🚀 **Nächste Schritte**

### **1. Projekt in Cursor AI öffnen**
- C#-Projekt öffnen
- NuGet-Pakete installieren (`dotnet restore`)

### **2. API-Tests durchführen**
- Test-Skripte ausführen
- Revit DLLs testen
- Kompilierung testen

### **3. Erste C#-Klassen implementieren**
- IExternalCommand Interface
- GeoJSON-Parsing
- Revit-Element-Erstellung

## 📋 **Test-Checkliste**

- [ ] Revit 2026 läuft ohne Fehler
- [ ] RevitAPI.dll kann geladen werden
- [ ] RevitAPIUI.dll kann geladen werden
- [ ] Projekt kompiliert ohne Fehler
- [ ] Test-Skripte funktionieren
- [ ] AddIns-Ordner ist beschreibbar

## 🔍 **Fehlerbehebung**

### **Häufige Probleme**
1. **Architecture Mismatch:** Projekt auf x64 konfigurieren
2. **DLL nicht gefunden:** Pfade in .csproj prüfen
3. **Copy Local Fehler:** Auf `False` setzen
4. **Permission Fehler:** Als Administrator ausführen

### **Debugging-Tools**
- **Test-Skripte:** Im `temp/` Ordner
- **Logs:** Über Console.WriteLine in Test-Skripten
- **Assembly-Info:** Über Reflection in Test-Skripten