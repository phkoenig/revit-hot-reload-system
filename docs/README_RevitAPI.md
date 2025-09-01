# Revit 2026 API Dokumentation

## 📚 Wichtige Ressourcen

### Offizielle Dokumentation
- [Autodesk Platform Services - Revit APIs](https://aps.autodesk.com/developer/overview/revit)
- [Revit 2026 API SDK Download](https://aps.autodesk.com/developer/overview/revit)
- [Revit .NET SDK Dokumentation](https://aps.autodesk.com/developer/overview/revit)

### Training Material
- [ADN-DevTech/RevitTrainingMaterial](https://github.com/ADN-DevTech/RevitTrainingMaterial)
- Über 100 Code-Beispiele in C# und VB.NET
- Hands-on Übungen für Revit API Entwicklung

## 🔑 Schlüssel-Konzepte

### IExternalCommand Interface
```csharp
[Transaction(TransactionMode.Manual)]
public class GeoJsonImportCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, 
                         ref string message, 
                         ElementSet elements)
    {
        // Implementierung hier
        return Result.Succeeded;
    }
}
```

### IExternalApplication Interface
```csharp
public class GeoJsonImporterApp : IExternalApplication
{
    public Result OnStartup(UIControlledApplication application)
    {
        // App-Initialisierung
        return Result.Succeeded;
    }
    
    public Result OnShutdown(UIControlledApplication application)
    {
        // Cleanup
        return Result.Succeeded;
    }
}
```

## 🏗️ Element-Erstellung

### Wände erstellen
```csharp
// Aus zwei Punkten eine Linie erstellen
Line baseCurve = Line.CreateBound(startPoint, endPoint);

// Wand aus der Linie erstellen
Wall wall = Wall.Create(document, baseCurve, levelId, isStructural);
```

### Linien erstellen
```csharp
// Linie zwischen zwei Punkten
Line line = Line.CreateBound(point1, point2);

// Als ModelLine in das Dokument einfügen
ModelLine modelLine = ModelLine.Create(document, line, sketchPlane);
```

## 📍 Koordinaten-System

### XYZ-Punkte
```csharp
// 3D-Punkt erstellen
XYZ point = new XYZ(x, y, z);

// Punkt-Operationen
XYZ midPoint = (point1 + point2) / 2.0;
double distance = point1.DistanceTo(point2);
```

### Transformation
```csharp
// Koordinaten-Transformation
Transform transform = Transform.CreateTranslation(offset);
XYZ transformedPoint = transform.OfPoint(originalPoint);
```

## 🔄 Transaction-Management

### Wichtig für alle Datenbank-Änderungen
```csharp
using (Transaction transaction = new Transaction(document))
{
    transaction.Start("GeoJSON Import");
    
    // Alle Revit-Änderungen hier
    
    transaction.Commit();
}
```

## 📁 Projekt-Struktur

### DLL-Referenzen
- RevitAPI.dll (Copy Local: false)
- RevitAPIUI.dll (Copy Local: false)
- Ziel-Plattform: x64

### Build-Konfiguration
```xml
<PropertyGroup>
  <ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch>None</ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch>
</PropertyGroup>
```

## 🚨 Häufige Fehler

### Processor Architecture Mismatch
- Projekt auf x64 konfigurieren
- Revit-DLLs nicht kopieren (Copy Local: false)

### Transaction-Fehler
- Alle Datenbank-Änderungen in Transaction-Blöcken
- Regenerate() nach umfangreichen Änderungen

### Koordinaten-Fehler
- Einheiten beachten (Revit verwendet Feet)
- Transformationen korrekt anwenden