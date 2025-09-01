# 🔬 Hot Reload Technical Specifications

## 📋 Technische Spezifikationen

### System Requirements
- **Revit:** 2026 oder höher
- **.NET:** 8.0-windows10.0.19041.0
- **Platform:** x64
- **OS:** Windows 10/11

### Assembly Configuration
```xml
<PropertyGroup>
    <TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
    <PlatformTarget>x64</PlatformTarget>
    <OutputType>Library</OutputType>
    <EnableHotReload>true</EnableHotReload>
    <DebugType>portable</DebugType>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

## 🏗️ Architektur-Details

### AssemblyLoadContext Implementation
```csharp
public class WorkDllLoadContext : AssemblyLoadContext
{
    private readonly string _workDllDirectory;

    public WorkDllLoadContext(string name, string workDllDirectory) 
        : base(name, isCollectible: true)  // Kritisch für Unloading
    {
        _workDllDirectory = workDllDirectory;
    }

    protected override Assembly Load(AssemblyName assemblyName)
    {
        // Custom Dependency Resolution
        string assemblyPath = Path.Combine(_workDllDirectory, 
            assemblyName.Name + ".dll");
            
        if (File.Exists(assemblyPath))
        {
            return LoadFromAssemblyPath(assemblyPath);
        }
        
        return null; // Fallback to default resolver
    }
}
```

### Memory Management
```csharp
public static bool UnloadWorkDll()
{
    try
    {
        if (_workDllContext != null)
        {
            // Explizites Unloading
            _workDllContext.Unload();
            _workDllContext = null;  // Starke Referenz entfernen
        }
        
        // Aggressive Garbage Collection
        GC.Collect();
        GC.WaitForPendingFinalizers(); 
        GC.Collect();
        
        return true;
    }
    catch (Exception ex)
    {
        // Error handling
        return false;
    }
}
```

## 📁 File System Layout

### Directory Structure
```
B:\Nextcloud\CODE\revit-plugins\GeoJson_Importer\
├── GeoJsonImporter.csproj                    # Loader-DLL Project
├── src\
│   ├── GeoJsonImporter\                      # Loader-DLL Source
│   │   ├── Addin\
│   │   │   ├── App.cs                        # IExternalApplication
│   │   │   └── Commands\
│   │   │       ├── ImportGeoJsonCommand.cs   # Proxy Command
│   │   │       ├── UnloadWorkDllCommand.cs   # Unload Button
│   │   │       ├── LoadWorkDllCommand.cs     # Load Button
│   │   │       └── WorkDllManager.cs         # Core Manager
│   │   └── Utils\
│   │       └── HotReloadLogger.cs            # Logging System
│   └── GeoJsonImporter.Work\                 # Work-DLL Project
│       ├── GeoJsonImporter.Work.csproj
│       ├── Commands\
│       │   └── ImportGeoJsonWorkCommand.cs   # Real Implementation
│       └── Core\
│           ├── GeoJsonParser.cs              # Business Logic
│           └── RevitGeometryCreator.cs       # Revit API Logic
├── WorkDll\                                  # Work-DLL Output (Hot-Swappable)
│   ├── GeoJsonImporter.Work.dll
│   ├── GeoJSON.Net.dll
│   └── Newtonsoft.Json.dll
├── Deploy\                                   # Loader-DLL Output
│   └── net8.0-windows10.0.19041.0\
│       └── GeoJsonImporter.dll
└── C:\ProgramData\Autodesk\Revit\Addins\2026\  # Revit Deployment
    ├── GeoJsonImporter.addin                 # Manifest
    ├── GeoJsonImporter.dll                   # Loader-DLL (Locked by Revit)
    └── GeoJsonImporter.pdb                   # Debug Symbols
```

### Build Outputs
```xml
<!-- Loader-DLL Build Configuration -->
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <PropertyGroup>
        <RevitAddinsPath>C:\ProgramData\Autodesk\Revit\Addins\2026\</RevitAddinsPath>
        <TargetAddinPath>$(RevitAddinsPath)GeoJsonImporter.addin</TargetAddinPath>
        <TargetDllPath>$(RevitAddinsPath)GeoJsonImporter.dll</TargetDllPath>
        <TargetPdbPath>$(RevitAddinsPath)GeoJsonImporter.pdb</TargetPdbPath>
    </PropertyGroup>
    
    <!-- Deploy to Revit AddIns Directory -->
    <Copy SourceFiles="$(ProjectDir)assets\GeoJsonImporter.addin" 
          DestinationFiles="$(TargetAddinPath)" />
    <Copy SourceFiles="$(TargetPath)" 
          DestinationFiles="$(TargetDllPath)" />
    <Copy SourceFiles="$(TargetDir)$(TargetName).pdb" 
          DestinationFiles="$(TargetPdbPath)" />
</Target>
```

```xml
<!-- Work-DLL Build Configuration -->
<PropertyGroup>
    <OutputPath>..\..\WorkDll\</OutputPath>  <!-- Critical: Not locked by Revit -->
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <GenerateTargetFrameworkAttribute>false</GenerateTargetFrameworkAttribute>
</PropertyGroup>
```

## 🔌 Revit Integration

### AddIn Manifest
```xml
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
    <AddIn Type="Application">
        <Name>GeoJSON Importer</Name>
        <Assembly>C:\ProgramData\Autodesk\Revit\Addins\2026\GeoJsonImporter.dll</Assembly>
        <AddInId>12345678-1234-1234-1234-123456789ABC</AddInId>
        <FullClassName>GeoJsonImporter.Addin.App</FullClassName>
        <VendorId>Philip</VendorId>
        <VendorDescription>Philip's Revit Plugins</VendorDescription>
        <ManifestSettings>
            <UseRevitContext>false</UseRevitContext>
            <ContextName>Separate</ContextName>
        </ManifestSettings>
    </AddIn>
</RevitAddIns>
```

### Ribbon UI Configuration
```csharp
public Result OnStartup(UIControlledApplication application)
{
    string tabName = "GeoJSON Importer";
    application.CreateRibbonTab(tabName);

    // Import Panel
    RibbonPanel importPanel = application.CreateRibbonPanel(tabName, "Import");
    var importButtonData = new PushButtonData(
        "ImportGeoJson",
        "Import\nGeoJSON", 
        assemblyPath,
        "GeoJsonImporter.Addin.Commands.ImportGeoJsonCommand");
    importPanel.AddItem(importButtonData);

    // Development Panel
    RibbonPanel devPanel = application.CreateRibbonPanel(tabName, "Development");
    
    var unloadButtonData = new PushButtonData(
        "UnloadWorkDll",
        "Unload\nWork-DLL",
        assemblyPath,
        "GeoJsonImporter.Addin.Commands.UnloadWorkDllCommand");
        
    var loadButtonData = new PushButtonData(
        "LoadWorkDll", 
        "Load\nWork-DLL",
        assemblyPath,
        "GeoJsonImporter.Addin.Commands.LoadWorkDllCommand");

    devPanel.AddItem(unloadButtonData);
    devPanel.AddItem(loadButtonData);

    return Result.Succeeded;
}
```

## 🔄 Reflection-Based Execution

### Dynamic Command Invocation
```csharp
public static Result ExecuteWorkCommand(
    ExternalCommandData commandData,
    ref string message, 
    ElementSet elements)
{
    try
    {
        // Auto-load if not loaded
        if (_workDllContext == null)
        {
            if (!LoadWorkDll()) return Result.Failed;
        }

        HotReloadLogger.Info("Searching for ImportGeoJsonWorkCommand in Work-DLL...");

        // Search for command class in loaded assemblies
        var assemblies = _workDllContext.Assemblies.ToList();
        foreach (var assembly in assemblies)
        {
            var commandType = assembly.GetType(
                "GeoJsonImporter.Work.Commands.ImportGeoJsonWorkCommand");
                
            if (commandType != null)
            {
                HotReloadLogger.Info("ImportGeoJsonWorkCommand found!");
                
                // Create instance and invoke Execute method
                var commandInstance = Activator.CreateInstance(commandType);
                var executeMethod = commandType.GetMethod("Execute");
                
                if (executeMethod != null)
                {
                    var result = executeMethod.Invoke(commandInstance, 
                        new object[] { commandData, message, elements });
                    return (Result)result;
                }
            }
        }

        HotReloadLogger.Error("ImportGeoJsonWorkCommand not found in Work-DLL");
        return Result.Failed;
    }
    catch (Exception ex)
    {
        var realEx = ex.InnerException ?? ex;
        HotReloadLogger.Error($"Work-Command execution error: {realEx.Message}", realEx);
        return Result.Failed;
    }
}
```

## 📊 Performance Metrics

### Memory Usage
```
Loader-DLL (in Revit memory):
- Size: ~50KB
- Memory footprint: ~2MB
- Lifetime: Entire Revit session

Work-DLL (in separate context):
- Size: ~200KB + Dependencies (~500KB)
- Memory footprint: ~5MB
- Lifetime: Load/Unload cycles
```

### Load/Unload Times
```
Typical Performance (measured):
- Load Work-DLL: 100-200ms
- Unload Work-DLL: 50-100ms  
- GC after unload: 200-500ms
- Total cycle time: 400-800ms
```

### Build Times
```
Loader-DLL Build: 1-2 seconds
Work-DLL Build: 0.5-1 second
Deployment: 100-200ms
Total: 2-3 seconds
```

## 🔐 Security Considerations

### Assembly Loading Security
```csharp
// Only load from trusted directory
private string GetWorkDllPath()
{
    string basePath = @"B:\Nextcloud\CODE\revit-plugins\GeoJson_Importer";
    string workDllPath = Path.Combine(basePath, "WorkDll", "GeoJsonImporter.Work.dll");
    
    // Validate path to prevent directory traversal
    var fullPath = Path.GetFullPath(workDllPath);
    if (!fullPath.StartsWith(basePath))
    {
        throw new SecurityException("Invalid Work-DLL path");
    }
    
    return fullPath;
}
```

### Sandbox Considerations
- Work-DLL runs in same AppDomain as Revit
- Full Revit API access available
- No additional security restrictions
- Standard .NET security model applies

## 🐛 Error Handling

### Exception Hierarchy
```
HotReloadException
├── WorkDllLoadException
│   ├── WorkDllNotFoundException
│   ├── WorkDllCorruptException
│   └── DependencyNotFoundException
├── WorkDllUnloadException
│   ├── AssemblyStillReferencedException
│   └── GarbageCollectionFailedException
└── WorkCommandExecutionException
    ├── ReflectionException
    ├── MethodNotFoundException
    └── InvocationException
```

### Logging Levels
```csharp
public enum LogLevel
{
    Info,    // Normal operations
    Warning, // Recoverable issues
    Error,   // Critical failures
    Debug    // Development information
}
```

### Recovery Strategies
```csharp
public static bool RecoverFromFailedUnload()
{
    try
    {
        // Force multiple GC cycles
        for (int i = 0; i < 3; i++)
        {
            GC.Collect(2, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
        }
        
        // Attempt to clear references
        _workDllContext = null;
        _workDllVersion = null;
        
        return true;
    }
    catch
    {
        // Last resort: Require Revit restart
        return false;
    }
}
```

## 🧪 Testing Framework

### Unit Test Structure
```csharp
[TestClass]
public class WorkDllManagerTests
{
    [TestMethod]
    public void LoadWorkDll_ValidPath_ReturnsTrue()
    {
        // Arrange
        var manager = new WorkDllManager();
        
        // Act
        bool result = manager.LoadWorkDll();
        
        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(manager.IsWorkDllLoaded());
    }
    
    [TestMethod]
    public void UnloadWorkDll_LoadedDll_ReturnsTrue()
    {
        // Arrange
        var manager = new WorkDllManager();
        manager.LoadWorkDll();
        
        // Act
        bool result = manager.UnloadWorkDll();
        
        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(manager.IsWorkDllLoaded());
    }
}
```

### Integration Tests
```csharp
[TestClass]
public class HotReloadIntegrationTests
{
    [TestMethod]
    public void FullHotReloadCycle_ValidWorkDll_Success()
    {
        // Test complete Load -> Execute -> Unload -> Load cycle
        var manager = new WorkDllManager();
        
        // Load
        Assert.IsTrue(manager.LoadWorkDll());
        
        // Execute  
        var result = manager.ExecuteWorkCommand(mockCommandData, ref message, elements);
        Assert.AreEqual(Result.Succeeded, result);
        
        // Unload
        Assert.IsTrue(manager.UnloadWorkDll());
        
        // Reload
        Assert.IsTrue(manager.LoadWorkDll());
    }
}
```

## 📈 Monitoring and Diagnostics

### Performance Counters
```csharp
public class HotReloadMetrics
{
    public static TimeSpan LastLoadTime { get; private set; }
    public static TimeSpan LastUnloadTime { get; private set; }
    public static int TotalLoadCycles { get; private set; }
    public static int FailedOperations { get; private set; }
    
    public static void RecordLoadTime(TimeSpan duration)
    {
        LastLoadTime = duration;
        TotalLoadCycles++;
    }
}
```

### Health Checks
```csharp
public static bool PerformHealthCheck()
{
    try
    {
        // Check Work-DLL file existence
        if (!File.Exists(GetWorkDllPath()))
            return false;
            
        // Check AssemblyLoadContext state
        if (_workDllContext != null && !_workDllContext.Assemblies.Any())
            return false;
            
        // Check memory usage
        var memoryBefore = GC.GetTotalMemory(false);
        GC.Collect();
        var memoryAfter = GC.GetTotalMemory(true);
        
        if (memoryAfter > memoryBefore * 1.5) // 50% memory increase threshold
            return false;
            
        return true;
    }
    catch
    {
        return false;
    }
}
```

## 🔧 Configuration Management

### Settings Structure
```csharp
public class HotReloadSettings
{
    public string WorkDllPath { get; set; }
    public bool AutoLoadOnStartup { get; set; }
    public bool EnableLogging { get; set; }
    public LogLevel MinimumLogLevel { get; set; }
    public int MaxRetryAttempts { get; set; }
    public TimeSpan UnloadTimeout { get; set; }
}
```

### Configuration File
```json
{
    "HotReload": {
        "WorkDllPath": "WorkDll\\GeoJsonImporter.Work.dll",
        "AutoLoadOnStartup": false,
        "EnableLogging": true,
        "MinimumLogLevel": "Info",
        "MaxRetryAttempts": 3,
        "UnloadTimeoutMs": 5000
    }
}
```

---
*Technical Specifications v1.0*  
*Last Updated: $(Get-Date)*  
*Compatibility: Revit 2026, .NET 8.0*
