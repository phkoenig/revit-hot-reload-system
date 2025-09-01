# WebView2 & Hot Reload Inkompatibilität - Technische Analyse

## 🚨 Problem Summary

**Hot Reload Mechanismus funktioniert nicht**, wenn WebView2-Controls in WPF-Dialogen verwendet werden. Die Work-DLL kann nicht entladen werden, obwohl explizit `AssemblyLoadContext.Unload()` aufgerufen wird.

## 🏗️ Architektur-Kontext

### **Hot Reload Setup:**
```csharp
// Loader-DLL (GeoJsonImporter.dll) - bleibt in Revit geladen
public class WorkDllLoadContext : AssemblyLoadContext
{
    public WorkDllLoadContext() : base(isCollectible: true) { }
    // Custom dependency resolution für Work-DLL
}

// Work-DLL Manager
public static class WorkDllManager 
{
    private static WorkDllLoadContext _workDllContext;
    
    public static bool UnloadWorkDll()
    {
        if (_workDllContext != null)
        {
            _workDllContext.Unload();  // ❌ FUNKTIONIERT NICHT mit WebView2
            _workDllContext = null;
            return true;
        }
        return false;
    }
}
```

### **WebView2 Integration:**
```csharp
// WPF Dialog mit WebView2 (in Work-DLL)
public partial class UtmGridSetupDialog : Window, IDisposable
{
    // XAML: <wv2:WebView2 Name="MapWebView" Height="500"/>
    
    private async void InitializeWebView2()
    {
        await MapWebView.EnsureCoreWebView2Async();
        MapWebView.CoreWebView2.WebMessageReceived += OnMapMessageReceived;
        // Leaflet.js Map wird geladen
    }
    
    public void Dispose()
    {
        if (MapWebView?.CoreWebView2 != null)
        {
            MapWebView.CoreWebView2.WebMessageReceived -= OnMapMessageReceived;
            MapWebView.Dispose();  // ❓ Reicht das?
        }
    }
}
```

## 🔥 Symptome

1. **Build-Fehler nach WebView2-Dialog Nutzung:**
   ```
   error MSB3021: Unable to copy file "obj\Debug\net8.0-windows\GeoJsonImporter.Work.dll" 
   to "...\WorkDll\GeoJsonImporter.Work.dll". The process cannot access the file 
   because it is being used by another process.
   ```

2. **Hot Reload "Unload" Button scheinbar erfolgreich:**
   - Keine Exception beim `_workDllContext.Unload()`
   - Logger zeigt "Work-DLL unloaded" 
   - **ABER**: DLL-File bleibt gesperrt

3. **Ohne WebView2 funktioniert Hot Reload perfekt:**
   - Einfache WPF-Dialogs: ✅ Hot Reload OK
   - Windows Forms: ✅ Hot Reload OK  
   - **Mit WebView2**: ❌ DLL-Lock bleibt

## 🔬 Technische Hypothesen

### **1. Native Handle Leaks**
```csharp
// WebView2 erstellt native Browser-Prozesse
// Microsoft Edge WebView2 Runtime (msedgewebview2.exe)
// Diese Prozesse halten möglicherweise Handles zur Work-DLL offen
```

### **2. COM/Interop References**
```csharp
// WebView2 basiert auf COM-Interop mit Edge-Browser
// COM-Objects werden möglicherweise nicht ordentlich released
// RCW (Runtime Callable Wrapper) hält Assembly-References
```

### **3. Event Handler Registrations**
```csharp
// JavaScript ↔ C# Bridge
MapWebView.CoreWebView2.WebMessageReceived += OnMapMessageReceived;

// Mögliche weitere interne Event-Registrierungen:
// - NavigationCompleted
// - DOMContentLoaded  
// - ScriptException
// Alle könnten Assembly-References halten
```

### **4. WebView2 User Data Folder**
```csharp
// WebView2 erstellt temporäre Dateien und Cache
// Default: %LOCALAPPDATA%\Microsoft\EdgeWebView\
// Möglicherweise werden DLL-Pfade in Cache/Metadaten gespeichert
```

## 📊 Vergleich: Funktioniert vs. Funktioniert Nicht

| Szenario | Hot Reload | DLL-Lock |
|----------|------------|----------|
| Nur Windows Forms | ✅ OK | ❌ Kein Lock |
| WPF ohne WebView2 | ✅ OK | ❌ Kein Lock |
| **WPF + WebView2** | **❌ FAIL** | **🔒 DLL gesperrt** |

## 🛠️ Bisherige Lösungsversuche

### **1. IDisposable Implementation:**
```csharp
public void Dispose()
{
    if (MapWebView?.CoreWebView2 != null)
    {
        MapWebView.CoreWebView2.WebMessageReceived -= OnMapMessageReceived;
        MapWebView.Dispose();
    }
}
```
**❌ Resultat**: DLL bleibt gesperrt

### **2. Explicit GC.Collect():**
```csharp
public static bool UnloadWorkDll()
{
    _workDllContext?.Unload();
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
    // ❌ Resultat: DLL bleibt gesperrt
}
```

## ❓ Spezifische Fragen für Perplexity

1. **Ist WebView2 + AssemblyLoadContext.Unload() eine bekannte Inkompatibilität?**

2. **Wie kann man WebView2-Controls vollständig "detachen" von einer Assembly?**

3. **Gibt es WebView2-spezifische Dispose-Patterns für Hot Reload Szenarien?**

4. **Alternative: Kann man WebView2 in einem separaten AppDomain/Process isolieren?**

5. **Workaround: Ist eine "WebView2-freie" Leaflet-Alternative für WPF verfügbar?**

## 🔧 Technische Umgebung

- **Revit**: 2026
- **WebView2**: Microsoft.Web.WebView2 v1.0.2739.15
- **.NET**: 8.0-windows
- **WPF**: Framework-dependent
- **AssemblyLoadContext**: isCollectible: true
- **Hot Reload**: Custom Implementation

## 💡 Mögliche Workarounds

1. **WebView2 in separatem Process**
2. **Alternative HTML-Rendering (CefSharp, etc.)**  
3. **Native Win32 WebBrowser Control**
4. **Server-basierte Map-Lösung**
5. **Hot Reload nur für Non-WebView2 Components**

---
*Erstellt für Perplexity-Recherche zur WebView2 & Hot Reload Inkompatibilität*
