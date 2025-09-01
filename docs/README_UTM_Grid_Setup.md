# UTM Grid Setup Tool - Project Requirements

## 📋 Übersicht
Der "UTM Grid Setup" Button erstellt ein geodätisch korrektes Revit-Environment-File für den Import von Geodaten mit präziser UTM-Koordinatenreferenz.

## 🎯 Ziele
- Revit-File für Geodaten-Import vorbereiten
- UTM-Koordinatensystem korrekt in Revit einrichten
- Visuelles UTM-Grid als Orientierungshilfe erstellen
- Environment-File für Projekt-Referenzierung bereitstellen

## 🏗️ Revit-File Axiome

### Koordinatensystem-Setup:
- **Project North = Geographic North** (True North)
- **Interner Nullpunkt (0,0,0)** = UTM-Verschiebungsvektor-Ziel
- **Z = 0** entspricht Höhe über Meer
- **Project Base Point** auf (0,0,0)
- **Survey Base Point** auf (0,0,0)
- **Einheiten:** Meter mit 0.0001m Präzision

### Verwendung:
- File als **Revit Environment File**
- Referenzierung in Hauptprojekte
- Basis für alle Geodaten-Imports

## 🔄 Workflow

### 1. Pre-Check & Validation
```
1.1 Prüfe aktuelle Revit-File Settings:
    - Project North Alignment
    - Base Point Positionen
    - Einheiten-System
    
1.2 Bei Abweichungen:
    - User-Dialog: "Einstellungen korrigieren?"
    - Optionen: [Ja, korrigieren] [Abbrechen]
    - Bei Abbruch: Workflow beenden
```

### 2. UTM Grid Configuration Dialog
```
2.1 Dialog öffnen mit folgenden Elementen:
    - Google Maps Integration
    - Adress-Eingabe & Lokalisierung
    - UTM-System Auswahl
    - Koordinaten-Anzeige & Rundung
    - Grid-Größe Konfiguration
    - Grid-Erstellung
```

### 3. Adress-Lokalisierung
```
3.1 User gibt Postadresse ein
3.2 "Platzieren" Button → Google Maps API Geocoding
3.3 Karte zentriert auf gefundene Position
3.4 Wiederholbar für verschiedene Adressen
```

### 4. UTM Grid Visualization
```
4.1 WSG84-Koordinaten → UTM-Koordinaten konvertieren
4.2 UTM-Koordinaten auf 100m runden
4.3 Grid-Overlay auf Google Maps:
    - 100x100m Maschenweite
    - Zentriert auf gerundete UTM-Koordinate
    - Größe gemäß Schieberegler (100-5000m)
```

### 5. Revit Grid Creation
```
5.1 "Grid herstellen" Button → Dialog schließen
5.2 Neuen Grid Type "UTM" erstellen (falls nicht vorhanden)
5.3 Rasterlinien zeichnen gemäß Konfiguration
5.4 Rasterlinien-Benennung: [EPSG-Code][X/Y][100er-Koordinate]
```

## 🎨 UI-Elemente Spezifikation

### Haupt-Button (Revit Ribbon):
- **Name:** "UTM Grid Setup"
- **Icon:** Grid/Koordinatensystem-Symbol
- **Tooltip:** "Revit-File für Geodaten-Import vorbereiten"

### Pre-Check Dialog:
- **Titel:** "Revit File Validation"
- **Checkboxes:** 
  - ☐ Project North = Geographic North
  - ☐ Base Points auf (0,0,0)
  - ☐ Einheiten: Meter (0.0001 Präzision)
- **Buttons:** [Korrigieren] [Abbrechen]

### UTM Grid Configuration Dialog:
- **Größe:** 800x600px (resizable)
- **Titel:** "UTM Grid Configuration"

#### Sektion 1: Adress-Lokalisierung
- **Label:** "Standort-Suche"
- **TextBox:** Postadresse-Eingabe
- **Button:** "Platzieren" (Geocoding)
- **Google Maps:** Embedded Map Control

#### Sektion 2: UTM-System
- **Label:** "UTM Koordinatensystem"
- **ComboBox:** UTM-Zone Auswahl
  - EPSG:25832 (UTM Zone 32N)
  - EPSG:25833 (UTM Zone 33N) [Default für Berlin]
  - Weitere nach Bedarf

#### Sektion 3: Koordinaten-Anzeige
- **GroupBox:** "Koordinaten-Transformation"
- **Label + TextBox:** "WGS84 Lat/Lon:" (read-only)
- **Label + TextBox:** "UTM Easting/Northing:" (read-only)
- **Label + TextBox:** "UTM gerundet (100m):" (read-only)

#### Sektion 4: Grid-Konfiguration
- **GroupBox:** "Grid-Einstellungen"
- **Label:** "Betrachtungsraum:"
- **Slider:** 100-5000m (Default: 1000m)
- **Label:** Dynamic "Aktuell: XXXXm x XXXXm"
- **CheckBox:** "Grid-Overlay auf Karte anzeigen" (Default: checked)

#### Sektion 5: Aktionen
- **Button:** "Grid herstellen" (Primary, grün)
- **Button:** "Abbrechen" (Secondary)

## 🔧 Technische Spezifikation

### Google Maps Integration:
```csharp
// WebView2 für Google Maps
// Alternative: Bing Maps Control
// Geocoding API für Adress → Koordinaten
```

### Koordinaten-Transformation:
```csharp
// WGS84 ↔ UTM Transformation
// Library: ProjNet4GeoAPI oder DotSpatial
// EPSG-Codes: 4326 (WGS84) → 25833 (UTM33N)
```

### Revit API Integration:
```csharp
// Grid Type Creation
// Grid Line Creation & Naming
// Base Point Manipulation
// Project Settings Validation
```

## 📊 Grid-Naming Convention

### Beispiel für EPSG:25833, UTM-Koordinate 372400/5814200:
```
Ost-West Linien (Y-Konstant):
- 25833Y58142 (Y = 5814200)
- 25833Y58143 (Y = 5814300)

Nord-Süd Linien (X-Konstant):
- 25833X3724 (X = 372400)
- 25833X3725 (X = 372500)
```

## ⚠️ Error Handling

### Mögliche Fehlerquellen:
- Google Maps API nicht verfügbar
- Geocoding fehlgeschlagen
- Ungültige UTM-Zone für Standort
- Revit API Permissions
- Grid Type bereits vorhanden (unterschiedliche Parameter)

### Fallback-Strategien:
- Offline-Koordinaten-Eingabe
- Manuelle UTM-Zone Auswahl
- Grid Type Überschreibung mit User-Bestätigung

## 🔄 Testing Scenarios

### Test Case 1: Berlin-Spandau
- **Adresse:** "Spandau, Berlin"
- **Erwartete UTM-Zone:** EPSG:25833
- **Erwartete Koordinaten:** ~372000/5814000

### Test Case 2: München
- **Adresse:** "München, Bayern"
- **Erwartete UTM-Zone:** EPSG:25832
- **Erwartete Koordinaten:** ~692000/5334000

## 📈 Future Enhancements
- Höhendaten-Integration (hoehendaten.de API)
- Topographische Karten-Overlay
- Batch-Adress-Verarbeitung
- Export/Import von Grid-Konfigurationen
- Integration mit WFS-Services

## 🎯 Success Criteria
1. ✅ Revit-File korrekt für Geodaten vorbereitet
2. ✅ UTM-Grid visuell korrekt in Revit erstellt
3. ✅ Grid-Linien korrekt benannt
4. ✅ Environment-File für Projekt-Referenz nutzbar
5. ✅ Workflow intuitiv und fehlerresistent

---
*Erstellt: [Datum]*
*Version: 1.0*
*Status: Requirements Definition*
