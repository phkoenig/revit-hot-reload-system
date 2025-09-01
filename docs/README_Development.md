# Entwicklungsdokumentation

## 🎯 Projektziele

Dieses Plugin soll GeoJSON-Daten als native Revit-Elemente importieren, wobei der Fokus auf:
- Einfachheit und Wartbarkeit liegt
- Kleine, übersichtliche Module folgt dem "Do One Thing and Do It Right" Prinzip
- Maximale Dateigröße: 200-300 Zeilen pro Modul

## 🏗️ Architektur

### Commands (src/Commands/)
- **GeoJsonImportCommand.cs**: Haupt-Command für den Import
- **GeoJsonExportCommand.cs**: Export von Revit-Daten als GeoJSON

### Services (src/Services/)
- **GeoJsonParserService.cs**: Parsing von GeoJSON-Dateien
- **RevitElementService.cs**: Erstellung von Revit-Elementen
- **CoordinateTransformService.cs**: Koordinaten-Transformation

### Models (src/Models/)
- **GeoJsonFeature.cs**: GeoJSON Feature-Struktur
- **GeoJsonGeometry.cs**: GeoJSON Geometrie-Typen
- **RevitElementMapping.cs**: Mapping zwischen GeoJSON und Revit

### Utils (src/Utils/)
- **Constants.cs**: Konstanten und Konfiguration
- **Logger.cs**: Logging-Funktionalität
- **ValidationHelper.cs**: Validierung von Eingabedaten

## 🔧 Entwicklungsumgebung

### Cursor AI Setup
- C#-Projekt mit .NET 8
- Revit API Referenzen (Copy Local: false)
- NuGet-Pakete: Newtonsoft.Json, NUnit (für Tests)

### Revit API Integration
- IExternalCommand für Commands
- IExternalApplication für App-Initialisierung
- Transaction-Management für alle Datenbank-Änderungen

## 📋 Entwicklungsregeln

1. **Modulgröße**: Maximal 200-300 Zeilen pro Datei
2. **Einzelverantwortlichkeit**: Jede Klasse hat eine klare Aufgabe
3. **Fehlerbehandlung**: Robuste Fehlerbehandlung mit Logging
4. **Dokumentation**: XML-Dokumentation für alle öffentlichen Methoden
5. **Tests**: Unit-Tests für alle Services und Utils

## 📁 Test-Skripte

**Wichtiger Hinweis:** Alle Test-Skripte befinden sich im `temp/` Ordner:
- **Pfad:** `b:\Nextcloud\CODE\revit-plugins\GeoJson_Importer\temp\`
- **Aktuelles Test-Skript:** `temp/test_script.cs`
- **Verwendung:** Für schnelle Tests und Debugging in Cursor AI

## 🚀 Deployment

### Add-In Datei
- Manifest-Datei im Revit Addins-Ordner
- DLL-Referenzen korrekt konfiguriert
- Versionierung und Update-Mechanismus

### Testing
- Test-Skripte im temp/-Ordner
- Beispiel-GeoJSON-Dateien in examples/geojson/
- Revit-Testmodelle in examples/revit/