# GeoJSON Importer für Revit 2026

Ein C#-Plugin für Autodesk Revit 2026, das GeoJSON-Daten als native Revit-Elemente importiert.

## 🏗️ Projektstruktur

```
GeoJson_Importer/
├── src/                    # Quellcode
│   ├── Commands/          # Revit-Commands (IExternalCommand)
│   ├── Services/          # Business Logic (GeoJSON-Parsing, Element-Erstellung)
│   ├── Models/            # Datenmodelle (GeoJSON-Strukturen)
│   └── Utils/             # Hilfsfunktionen (Koordinaten-Transformation)
├── docs/                  # Dokumentation
│   ├── api/               # API-Dokumentation
│   └── tutorials/         # Anleitungen und Tutorials
├── examples/              # Beispiel-Dateien
│   ├── geojson/           # GeoJSON-Testdateien
│   └── revit/             # Revit-Testmodelle
└── temp/                  # Temporäre Dateien für Tests
```

## 🚀 Features

- Import von GeoJSON-Dateien in Revit 2026
- Automatische Koordinaten-Transformation
- Erstellung von Wänden, Linien und anderen Revit-Elementen
- Unterstützung verschiedener GeoJSON-Geometrietypen

## 🛠️ Entwicklungsumgebung

- **IDE:** Cursor AI
- **Framework:** .NET 8
- **Revit Version:** 2026
- **API:** Revit 2026 API SDK

## 📋 Voraussetzungen

- Revit 2026 installiert
- Revit 2026 API SDK heruntergeladen
- .NET 8 SDK installiert
- Cursor AI als Entwicklungsumgebung

## 🔧 Installation

1. SDK herunterladen von [Autodesk Platform Services](https://aps.autodesk.com/developer/overview/revit)
2. Projekt in Cursor AI öffnen
3. NuGet-Pakete installieren
4. Revit-Referenzen hinzufügen

## 📚 Dokumentation

- [Installation & Tests](README_Installation.md) ← **NEU!**
- [Entwicklungsrichtlinien](README_Development.md)
- [Revit API Referenz](README_RevitAPI.md)
- [GeoJSON Dokumentation](README_GeoJSON.md)
- [API-Referenz](api/)
- [Tutorials](tutorials/)
- [Beispiele](../examples/)

## 🤝 Beitragen

Dieses Projekt folgt dem "Do One Thing and Do It Right" Prinzip. Alle Module sind klein und übersichtlich gehalten.

## 📄 Lizenz

MIT License