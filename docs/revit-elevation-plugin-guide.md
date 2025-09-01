# CursorAI Prompt: Revit 2026 Plugin mit WCS-Höhendaten Integration

## Projektübersicht

Entwickle ein Revit 2026 Plugin in C#, das Höhendaten über Web Coverage Service (WCS) von Berlin-Brandenburg abruft. Das Plugin soll Smart Batching und Redis-Caching implementieren für optimale Performance bei Höhenabfragen.

## Technische Anforderungen

### Hauptkomponenten:
1. **WCS Client** - Höhendaten von Brandenburg WCS Service
2. **Redis Cache** - High-Performance Caching für Höhenwerte  
3. **Smart Batching** - Effiziente Gruppierung von Höhenabfragen
4. **Revit Integration** - Seamless integration in Revit 2026 workflow
5. **UTM33 Koordinaten** - Native UTM33 support für Berlin-Brandenburg

### NuGet Packages benötigt:
```xml
<PackageReference Include="RestSharp" Version="110.2.0" />
<PackageReference Include="StackExchange.Redis" Version="2.7.33" />
<PackageReference Include="NetTopologySuite" Version="2.5.0" />
<PackageReference Include="OSGeo.GDAL" Version="3.8.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
<PackageReference Include="System.Drawing.Common" Version="7.0.0" />
```

## 1. WCS Service Client implementieren

Erstelle eine `WCSElevationService` Klasse für Berlin-Brandenburg WCS Integration:

```csharp
using RestSharp;
using System;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using OSGeo.GDAL;

public class WCSElevationService
{
    private const string WCS_ENDPOINT = "https://isk.geobasis-bb.de/ows/dgm_wcs";
    private const string COVERAGE_ID = "BB_DGM1";
    private const string VERSION = "2.0.1";
    private const string CRS = "EPSG:25833"; // UTM33
    
    private readonly RestClient _client;
    
    public WCSElevationService()
    {
        _client = new RestClient(WCS_ENDPOINT);
        
        // GDAL initialisieren
        GdalConfiguration.ConfigureGdal();
        GdalConfiguration.ConfigureOgr();
    }
    
    public async Task<double?> GetElevationAsync(double easting, double northing, int buffer = 50)
    {
        try
        {
            // WCS GetCoverage Request erstellen
            var request = new RestRequest("", Method.Get);
            request.AddParameter("SERVICE", "WCS");
            request.AddParameter("VERSION", VERSION);
            request.AddParameter("REQUEST", "GetCoverage");
            request.AddParameter("COVERAGEID", COVERAGE_ID);
            request.AddParameter("FORMAT", "image/tiff");
            request.AddParameter("SUBSET", $"E({easting - buffer},{easting + buffer})");
            request.AddParameter("SUBSET", $"N({northing - buffer},{northing + buffer})");
            
            var response = await _client.ExecuteAsync(request);
            
            if (response.IsSuccessful && response.RawBytes != null)
            {
                return ExtractElevationFromGeoTiff(response.RawBytes, easting, northing);
            }
            
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"WCS Request failed: {ex.Message}");
            return null;
        }
    }
    
    private double? ExtractElevationFromGeoTiff(byte[] tiffData, double targetX, double targetY)
    {
        // Create in-memory GDAL dataset
        string vsiPath = $"/vsimem/temp_elevation_{Guid.NewGuid()}.tif";
        
        try
        {
            // Write to GDAL virtual file system
            Gdal.FileFromMemBuffer(vsiPath, tiffData);
            
            using (var dataset = Gdal.Open(vsiPath, Access.GA_ReadOnly))
            {
                if (dataset == null) return null;
                
                // Get geotransformation
                double[] geoTransform = new double[6];
                dataset.GetGeoTransform(geoTransform);
                
                // Calculate pixel coordinates
                int px = (int)((targetX - geoTransform[0]) / geoTransform[1]);
                int py = (int)((targetY - geoTransform[3]) / geoTransform[5]);
                
                // Check bounds
                if (px < 0 || py < 0 || px >= dataset.RasterXSize || py >= dataset.RasterYSize)
                    return null;
                
                // Read elevation value
                using (var band = dataset.GetRasterBand(1))
                {
                    double[] buffer = new double[1];
                    band.ReadRaster(px, py, 1, 1, buffer, 1, 1, 0, 0);
                    
                    double elevation = buffer[0];
                    
                    // Check for NoData
                    double noDataValue;
                    int hasNoData;
                    band.GetNoDataValue(out noDataValue, out hasNoData);
                    
                    if (hasNoData == 1 && Math.Abs(elevation - noDataValue) < 0.001)
                        return null;
                    
                    return elevation;
                }
            }
        }
        finally
        {
            // Cleanup virtual file
            Gdal.Unlink(vsiPath);
        }
    }
}

public static class GdalConfiguration
{
    public static void ConfigureGdal()
    {
        if (GdalBase.HasGdal)
        {
            Gdal.AllRegister();
        }
    }
    
    public static void ConfigureOgr()
    {
        if (GdalBase.HasGdal)
        {
            Ogr.RegisterAll();
        }
    }
}
```

## 2. Redis Cache Service implementieren

Erstelle `ElevationCacheService` für hochperformantes Caching:

```csharp
using StackExchange.Redis;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public class ElevationCacheService
{
    private readonly IDatabase _database;
    private readonly ConnectionMultiplexer _redis;
    private const int CACHE_TTL_SECONDS = 3600; // 1 Stunde
    private const int GRID_SIZE = 10; // 10m Raster für Caching
    
    public ElevationCacheService(string connectionString = "localhost:6379")
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }
    
    public async Task<double?> GetCachedElevationAsync(double easting, double northing)
    {
        string cacheKey = GenerateCacheKey(easting, northing);
        
        try
        {
            var cachedData = await _database.StringGetAsync(cacheKey);
            
            if (cachedData.HasValue)
            {
                var elevationData = JsonConvert.DeserializeObject<CachedElevation>(cachedData);
                return elevationData.Elevation;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Cache read error: {ex.Message}");
        }
        
        return null;
    }
    
    public async Task CacheElevationAsync(double easting, double northing, double elevation)
    {
        string cacheKey = GenerateCacheKey(easting, northing);
        
        var cacheData = new CachedElevation
        {
            Elevation = elevation,
            CachedAt = DateTime.UtcNow,
            Precision = "10m_grid",
            Source = "BB_WCS_DGM1"
        };
        
        try
        {
            string jsonData = JsonConvert.SerializeObject(cacheData);
            await _database.StringSetAsync(cacheKey, jsonData, TimeSpan.FromSeconds(CACHE_TTL_SECONDS));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Cache write error: {ex.Message}");
        }
    }
    
    private string GenerateCacheKey(double easting, double northing)
    {
        // Round to 10m grid for cache efficiency
        int gridX = (int)(easting / GRID_SIZE) * GRID_SIZE;
        int gridY = (int)(northing / GRID_SIZE) * GRID_SIZE;
        
        return $"elevation:utm33:{gridX}:{gridY}";
    }
    
    public async Task<CacheStatistics> GetStatisticsAsync()
    {
        try
        {
            var info = await _redis.GetServer(_redis.GetEndPoints()[0]).InfoAsync("stats");
            var keyspaceInfo = info.ToString();
            
            // Parse keyspace hits/misses from info string
            // Simplified - in production, parse actual values
            return new CacheStatistics
            {
                CacheHits = 0, // Parse from keyspaceInfo
                CacheMisses = 0,
                HitRate = 0.0
            };
        }
        catch
        {
            return new CacheStatistics();
        }
    }
    
    public void Dispose()
    {
        _redis?.Dispose();
    }
}

public class CachedElevation
{
    public double Elevation { get; set; }
    public DateTime CachedAt { get; set; }
    public string Precision { get; set; }
    public string Source { get; set; }
}

public class CacheStatistics
{
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double HitRate { get; set; }
}
```

## 3. Smart Batching Service implementieren

Erstelle `SmartBatchingService` für effiziente Höhenabfragen:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SmartBatchingService
{
    private readonly WCSElevationService _wcsService;
    private readonly ElevationCacheService _cacheService;
    private const int TILE_SIZE = 1000; // 1km Tiles für Batching
    
    public SmartBatchingService(WCSElevationService wcsService, ElevationCacheService cacheService)
    {
        _wcsService = wcsService;
        _cacheService = cacheService;
    }
    
    public async Task<List<ElevationResult>> GetElevationsBatchAsync(List<UTMCoordinate> coordinates)
    {
        var results = new List<ElevationResult>();
        var uncachedCoordinates = new List<UTMCoordinate>();
        
        // Phase 1: Check cache for all coordinates
        foreach (var coord in coordinates)
        {
            var cachedElevation = await _cacheService.GetCachedElevationAsync(coord.Easting, coord.Northing);
            
            if (cachedElevation.HasValue)
            {
                results.Add(new ElevationResult
                {
                    Coordinate = coord,
                    Elevation = cachedElevation.Value,
                    Source = "Cache",
                    ResponseTime = TimeSpan.FromMilliseconds(1)
                });
            }
            else
            {
                uncachedCoordinates.Add(coord);
            }
        }
        
        if (uncachedCoordinates.Any())
        {
            // Phase 2: Group uncached coordinates by spatial tiles
            var tiles = GroupCoordinatesByTiles(uncachedCoordinates);
            
            // Phase 3: Process each tile with WCS
            var wcsResults = await ProcessTilesAsync(tiles);
            results.AddRange(wcsResults);
        }
        
        return results.OrderBy(r => coordinates.IndexOf(r.Coordinate)).ToList();
    }
    
    private Dictionary<string, List<UTMCoordinate>> GroupCoordinatesByTiles(List<UTMCoordinate> coordinates)
    {
        var tiles = new Dictionary<string, List<UTMCoordinate>>();
        
        foreach (var coord in coordinates)
        {
            int tileX = (int)(coord.Easting / TILE_SIZE) * TILE_SIZE;
            int tileY = (int)(coord.Northing / TILE_SIZE) * TILE_SIZE;
            string tileKey = $"{tileX}_{tileY}";
            
            if (!tiles.ContainsKey(tileKey))
            {
                tiles[tileKey] = new List<UTMCoordinate>();
            }
            
            tiles[tileKey].Add(coord);
        }
        
        return tiles;
    }
    
    private async Task<List<ElevationResult>> ProcessTilesAsync(Dictionary<string, List<UTMCoordinate>> tiles)
    {
        var allResults = new List<ElevationResult>();
        
        // Process tiles in parallel (limit concurrency to avoid overwhelming WCS)
        var semaphore = new SemaphoreSlim(3, 3); // Max 3 concurrent requests
        var tasks = tiles.Select(async tile =>
        {
            await semaphore.WaitAsync();
            try
            {
                return await ProcessSingleTileAsync(tile.Value);
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        var results = await Task.WhenAll(tasks);
        
        foreach (var result in results)
        {
            allResults.AddRange(result);
        }
        
        return allResults;
    }
    
    private async Task<List<ElevationResult>> ProcessSingleTileAsync(List<UTMCoordinate> coordinates)
    {
        var results = new List<ElevationResult>();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        foreach (var coord in coordinates)
        {
            var startTime = DateTime.UtcNow;
            var elevation = await _wcsService.GetElevationAsync(coord.Easting, coord.Northing);
            var responseTime = DateTime.UtcNow - startTime;
            
            if (elevation.HasValue)
            {
                // Cache the result
                await _cacheService.CacheElevationAsync(coord.Easting, coord.Northing, elevation.Value);
                
                results.Add(new ElevationResult
                {
                    Coordinate = coord,
                    Elevation = elevation.Value,
                    Source = "WCS",
                    ResponseTime = responseTime
                });
            }
            else
            {
                results.Add(new ElevationResult
                {
                    Coordinate = coord,
                    Elevation = null,
                    Source = "WCS_ERROR",
                    ResponseTime = responseTime,
                    Error = "No elevation data found"
                });
            }
        }
        
        stopwatch.Stop();
        System.Diagnostics.Debug.WriteLine($"Processed tile with {coordinates.Count} points in {stopwatch.ElapsedMilliseconds}ms");
        
        return results;
    }
}

public class UTMCoordinate
{
    public double Easting { get; set; }
    public double Northing { get; set; }
    public int Zone { get; set; } = 33; // UTM Zone 33 for Berlin-Brandenburg
    
    public UTMCoordinate(double easting, double northing)
    {
        Easting = easting;
        Northing = northing;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is UTMCoordinate other)
        {
            return Math.Abs(Easting - other.Easting) < 0.001 && 
                   Math.Abs(Northing - other.Northing) < 0.001;
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Math.Round(Easting, 3), Math.Round(Northing, 3));
    }
}

public class ElevationResult
{
    public UTMCoordinate Coordinate { get; set; }
    public double? Elevation { get; set; }
    public string Source { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public string Error { get; set; }
    
    public bool IsSuccess => Elevation.HasValue && string.IsNullOrEmpty(Error);
}
```

## 4. Revit Plugin Integration

Erstelle die Hauptklasse für das Revit 2026 Plugin:

```csharp
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class ElevationQueryCommand : IExternalCommand
{
    private static WCSElevationService _wcsService;
    private static ElevationCacheService _cacheService;
    private static SmartBatchingService _batchingService;
    
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        UIApplication uiapp = commandData.Application;
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Application app = uiapp.Application;
        Document doc = uidoc.Document;
        
        try
        {
            // Initialize services if not already done
            InitializeServices();
            
            // Get selected elements or all topography
            var selection = uidoc.Selection;
            var elementIds = selection.GetElementIds();
            
            List<Element> elementsToProcess;
            
            if (elementIds.Any())
            {
                elementsToProcess = elementIds.Select(id => doc.GetElement(id)).ToList();
            }
            else
            {
                // Get all topography surfaces if nothing selected
                elementsToProcess = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Topography)
                    .WhereElementIsNotElementType()
                    .ToList();
            }
            
            if (!elementsToProcess.Any())
            {
                TaskDialog.Show("No Elements", "No topography elements found. Please select elements or create topography first.");
                return Result.Cancelled;
            }
            
            // Show progress dialog
            using (var progressForm = new ProgressForm())
            {
                progressForm.Show();
                
                // Process elevation queries
                var task = ProcessElevationQueriesAsync(doc, elementsToProcess, progressForm);
                var results = task.GetAwaiter().GetResult();
                
                // Apply results to Revit elements
                ApplyElevationResults(doc, results);
                
                progressForm.Close();
            }
            
            TaskDialog.Show("Success", $"Elevation data updated successfully!");
            
            return Result.Succeeded;
        }
        catch (Exception ex)
        {
            message = ex.Message;
            return Result.Failed;
        }
    }
    
    private void InitializeServices()
    {
        if (_wcsService == null)
        {
            _wcsService = new WCSElevationService();
            _cacheService = new ElevationCacheService();
            _batchingService = new SmartBatchingService(_wcsService, _cacheService);
        }
    }
    
    private async Task<List<ElevationResult>> ProcessElevationQueriesAsync(
        Document doc, 
        List<Element> elements, 
        ProgressForm progressForm)
    {
        var coordinates = ExtractCoordinatesFromElements(doc, elements);
        
        progressForm.SetProgress(0, $"Processing {coordinates.Count} coordinate queries...");
        
        // Use smart batching for efficient processing
        var results = await _batchingService.GetElevationsBatchAsync(coordinates);
        
        progressForm.SetProgress(100, "Elevation queries completed!");
        
        return results;
    }
    
    private List<UTMCoordinate> ExtractCoordinatesFromElements(Document doc, List<Element> elements)
    {
        var coordinates = new List<UTMCoordinate>();
        
        foreach (var element in elements)
        {
            if (element is TopographySurface topo)
            {
                // Get points from topography surface
                var points = GetTopographyPoints(topo);
                coordinates.AddRange(ConvertRevitPointsToUTM(doc, points));
            }
            else if (element.Location is LocationPoint locationPoint)
            {
                // Single point element
                var utm = ConvertRevitPointToUTM(doc, locationPoint.Point);
                coordinates.Add(utm);
            }
            else if (element.Location is LocationCurve locationCurve)
            {
                // Sample points along curve
                var curve = locationCurve.Curve;
                var samplePoints = SamplePointsAlongCurve(curve, 10.0); // 10 unit intervals
                coordinates.AddRange(ConvertRevitPointsToUTM(doc, samplePoints));
            }
        }
        
        return coordinates.Distinct().ToList();
    }
    
    private List<XYZ> GetTopographyPoints(TopographySurface topo)
    {
        var points = new List<XYZ>();
        
        // Get points from topography mesh
        var mesh = topo.get_Geometry(new Options()).OfType<Mesh>().FirstOrDefault();
        if (mesh != null)
        {
            for (int i = 0; i < mesh.NumTriangles; i++)
            {
                var triangle = mesh.get_Triangle(i);
                points.Add(triangle.get_Vertex(0));
                points.Add(triangle.get_Vertex(1));
                points.Add(triangle.get_Vertex(2));
            }
        }
        
        return points.Distinct().ToList();
    }
    
    private UTMCoordinate ConvertRevitPointToUTM(Document doc, XYZ revitPoint)
    {
        // Get project location and transform
        var projectLocation = doc.ActiveProjectLocation;
        var transform = projectLocation.GetTotalTransform();
        
        // Transform from Revit internal coordinates to project coordinates
        var projectPoint = transform.OfPoint(revitPoint);
        
        // Convert from project coordinates to UTM33
        // Note: This assumes project is already set up with UTM33 coordinate system
        // In production, you may need additional coordinate transformation
        
        var utm = new UTMCoordinate(
            UnitUtils.ConvertFromInternalUnits(projectPoint.X, UnitTypeId.Meters),
            UnitUtils.ConvertFromInternalUnits(projectPoint.Y, UnitTypeId.Meters)
        );
        
        return utm;
    }
    
    private List<UTMCoordinate> ConvertRevitPointsToUTM(Document doc, List<XYZ> revitPoints)
    {
        return revitPoints.Select(point => ConvertRevitPointToUTM(doc, point)).ToList();
    }
    
    private List<XYZ> SamplePointsAlongCurve(Curve curve, double interval)
    {
        var points = new List<XYZ>();
        double length = curve.Length;
        int numSamples = (int)(length / interval) + 1;
        
        for (int i = 0; i <= numSamples; i++)
        {
            double parameter = curve.GetEndParameter(0) + 
                             (curve.GetEndParameter(1) - curve.GetEndParameter(0)) * i / numSamples;
            points.Add(curve.Evaluate(parameter, false));
        }
        
        return points;
    }
    
    private void ApplyElevationResults(Document doc, List<ElevationResult> results)
    {
        using (Transaction trans = new Transaction(doc, "Apply Elevation Data"))
        {
            trans.Start();
            
            try
            {
                foreach (var result in results.Where(r => r.IsSuccess))
                {
                    // Apply elevation data to Revit elements
                    // Implementation depends on specific use case:
                    // - Update topography points
                    // - Set shared parameters with elevation data  
                    // - Create adaptive components at elevations
                    // - Update family instance parameters
                    
                    // Example: Set shared parameter on elements
                    // SetElevationParameter(element, result.Elevation.Value);
                }
                
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.RollBack();
                throw;
            }
        }
    }
}

// Progress form for user feedback
public partial class ProgressForm : Form
{
    private ProgressBar progressBar;
    private Label statusLabel;
    
    public ProgressForm()
    {
        InitializeComponent();
    }
    
    public void SetProgress(int percentage, string status)
    {
        if (InvokeRequired)
        {
            Invoke(new Action<int, string>(SetProgress), percentage, status);
            return;
        }
        
        progressBar.Value = Math.Min(percentage, 100);
        statusLabel.Text = status;
        Application.DoEvents();
    }
    
    private void InitializeComponent()
    {
        // Initialize Windows Forms components
        this.progressBar = new ProgressBar();
        this.statusLabel = new Label();
        
        // Form setup
        this.Text = "Processing Elevation Data";
        this.Size = new System.Drawing.Size(400, 120);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        
        // Progress bar setup
        this.progressBar.Location = new System.Drawing.Point(20, 30);
        this.progressBar.Size = new System.Drawing.Size(350, 25);
        this.progressBar.Style = ProgressBarStyle.Continuous;
        
        // Status label setup
        this.statusLabel.Location = new System.Drawing.Point(20, 65);
        this.statusLabel.Size = new System.Drawing.Size(350, 20);
        this.statusLabel.Text = "Initializing...";
        
        this.Controls.Add(this.progressBar);
        this.Controls.Add(this.statusLabel);
    }
}
```

## 5. Revit Plugin Manifest und Setup

Erstelle `ElevationPlugin.addin` für Revit Plugin Registry:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Command">
    <Text>Get Elevation Data</Text>
    <Description>Query elevation data from Berlin-Brandenburg WCS service with smart caching</Description>
    <Assembly>ElevationPlugin.dll</Assembly>
    <FullClassName>ElevationQueryCommand</FullClassName>
    <CommandId>B8C23C26-1B2E-4F7F-9A47-8F6E12345678</CommandId>
    <VendorId>YourCompany</VendorId>
    <VendorDescription>Your Company Name</VendorDescription>
  </AddIn>
  
  <AddIn Type="Application">
    <Name>Elevation Data Plugin</Name>
    <Assembly>ElevationPlugin.dll</Assembly>
    <FullClassName>ElevationApplication</FullClassName>
    <ClientId>A7D14F85-2C3D-4B6E-8E58-7G9F23456789</ClientId>
    <VendorId>YourCompany</VendorId>
    <VendorDescription>Your Company Name</VendorDescription>
  </AddIn>
</RevitAddIns>
```

## 6. Docker Setup für Redis

Erstelle `docker-compose.yml` für lokale Entwicklung:

```yaml
version: '3.8'

services:
  redis:
    image: redis:7-alpine
    container_name: elevation-cache
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    command: redis-server --appendonly yes --maxmemory 1gb --maxmemory-policy allkeys-lru
    restart: unless-stopped

volumes:
  redis_data:
```

## 7. Konfiguration und Deployment

Erstelle `appsettings.json` für Konfiguration:

```json
{
  "ElevationService": {
    "WCS": {
      "Endpoint": "https://isk.geobasis-bb.de/ows/dgm_wcs",
      "CoverageId": "BB_DGM1",
      "Version": "2.0.1",
      "BufferSize": 50,
      "TimeoutSeconds": 30
    },
    "Cache": {
      "RedisConnectionString": "localhost:6379",
      "TTLSeconds": 3600,
      "GridSizeMeters": 10,
      "MaxMemoryMB": 1024
    },
    "Batching": {
      "TileSizeMeters": 1000,
      "MaxConcurrentRequests": 3,
      "BatchSizeLimit": 1000
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Implementierungsreihenfolge

1. **Start mit WCS Client** - Teste einzelne Höhenabfragen
2. **Redis Cache hinzufügen** - Implementiere Basis-Caching  
3. **Smart Batching entwickeln** - Optimiere für Batch-Verarbeitung
4. **Revit Integration** - Verbinde mit Revit-Objekten
5. **UI und UX polish** - Progress feedback und Error handling
6. **Testing und Optimization** - Performance-Tuning

## Erwartete Performance

- **Einzelabfrage ohne Cache:** ~500ms
- **Einzelabfrage mit Cache Hit:** ~1ms  
- **Batch 100 Punkte ohne Cache:** ~15 Sekunden
- **Batch 100 Punkte mit 90% Cache Hit:** ~2 Sekunden

## Key Implementation Notes

- Verwende `async/await` für alle WCS-Requests
- Implementiere proper `IDisposable` für Redis connections
- Handle GDAL memory management sorgfältig
- Cache auf 10m-Raster für Balance zwischen Hits und Genauigkeit
- Limit concurrent WCS requests um Service nicht zu überlasten
- Proper error handling und user feedback in Revit UI

Das Plugin wird Revit mit hochpräzisen Höhendaten aus offiziellen Quellen erweitern und durch intelligente Optimierung auch bei großen Datenmengen performant arbeiten.