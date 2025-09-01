# GeoJSON Dokumentation

## 📖 Was ist GeoJSON?

GeoJSON ist ein Format zur Darstellung geografischer Daten in JSON-Struktur. Es wird häufig für:
- GIS-Anwendungen
- Web-Mapping
- Geodaten-Austausch
- Stadtplanung und BIM

## 🏗️ GeoJSON-Struktur

### Feature Collection
```json
{
  "type": "FeatureCollection",
  "features": [
    {
      "type": "Feature",
      "geometry": { ... },
      "properties": { ... }
    }
  ]
}
```

### Feature
```json
{
  "type": "Feature",
  "geometry": {
    "type": "Polygon",
    "coordinates": [[[x1,y1], [x2,y2], ...]]
  },
  "properties": {
    "name": "Gebäude A",
    "height": 25.5,
    "material": "Beton"
  }
}
```

## 📐 Geometrie-Typen

### Point
```json
{
  "type": "Point",
  "coordinates": [x, y]
}
```

### LineString
```json
{
  "type": "LineString",
  "coordinates": [[x1,y1], [x2,y2], [x3,y3]]
}
```

### Polygon
```json
{
  "type": "Polygon",
  "coordinates": [
    [[x1,y1], [x2,y2], [x3,y3], [x1,y1]]
  ]
}
```

### MultiPolygon
```json
{
  "type": "MultiPolygon",
  "coordinates": [
    [[[x1,y1], [x2,y2], [x3,y3], [x1,y1]]],
    [[[x4,y4], [x5,y5], [x6,y6], [x4,y4]]]
  ]
}
```

## 🌍 Koordinaten-Systeme

### WGS84 (Standard)
- Breitengrad: -90 bis +90
- Längengrad: -180 bis +180
- Einheit: Dezimalgrad

### UTM (Universal Transverse Mercator)
- Zonensystem
- Einheit: Meter
- Präzise für lokale Projekte

### Lokale Systeme
- Stadtkoordinaten
- Projektkoordinaten
- Einheit: Meter

## 🔄 Transformation zu Revit

### Koordinaten-Umrechnung
1. **Quell-System identifizieren** (aus GeoJSON oder Metadaten)
2. **Ziel-System definieren** (Revit-Projektkoordinaten)
3. **Transformation berechnen** (Offset, Rotation, Skalierung)

### Einheiten-Umrechnung
- GeoJSON: Oft Meter
- Revit: Feet (Standard)
- Umrechnungsfaktor: 1 Meter = 3.28084 Feet

### Beispiel-Transformation
```csharp
// WGS84 zu lokalen Projektkoordinaten
double localX = (longitude - referenceLongitude) * metersPerDegree;
double localY = (latitude - referenceLatitude) * metersPerDegree;

// Meter zu Feet
double feetX = localX * 3.28084;
double feetY = localY * 3.28084;
```

## 📊 Eigenschaften (Properties)

### Standard-Eigenschaften
- **name**: Bezeichnung des Elements
- **height**: Höhe (für 3D-Elemente)
- **material**: Material-Information
- **type**: Element-Typ

### Benutzerdefinierte Eigenschaften
- **building_id**: Gebäude-ID
- **floor_number**: Stockwerk
- **construction_year**: Baujahr
- **architect**: Architekt

## 🚀 Import-Strategien

### 1. Direkte Mapping
- Point → Revit Point
- LineString → Revit Line
- Polygon → Revit Wall/Floor

### 2. Intelligente Interpretation
- Gebäude-Umrisse → Wände
- Wege → Linien
- Flächen → Böden/Dächer

### 3. Hierarchische Struktur
- FeatureCollection → Revit-Kategorien
- Feature → Revit-Familien
- Properties → Revit-Parameter

## 🔧 Tools und Bibliotheken

### C# Libraries
- **Newtonsoft.Json**: JSON-Parsing
- **NetTopologySuite**: Geometrie-Operationen
- **ProjNet4GeoAPI**: Koordinaten-Transformation

### Validierung
- GeoJSON-Schema-Validierung
- Koordinaten-Bereich-Prüfung
- Geometrie-Integritäts-Check