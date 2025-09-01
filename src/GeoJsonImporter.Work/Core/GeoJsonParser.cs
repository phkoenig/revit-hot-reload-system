using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System.IO;

namespace GeoJsonImporter.Core
{
    public static class GeoJsonParser
    {
        public static FeatureCollection? ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                // Fehlerbehandlung: Datei nicht gefunden
                return null;
            }

            string jsonContent = File.ReadAllText(filePath);

            try
            {
                var featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(jsonContent);
                return featureCollection;
            }
            catch (JsonException ex)
            {
                // Fehlerbehandlung: Ungültiges JSON-Format
                System.Diagnostics.Debug.WriteLine($"JSON parsing error: {ex.Message}");
                return null;
            }
        }
    }
}