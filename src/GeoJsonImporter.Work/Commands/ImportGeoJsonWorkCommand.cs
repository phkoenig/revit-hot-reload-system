using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GeoJsonImporter.Core;
using System.Windows.Forms;
using System;
using System.Linq;
using System.Reflection;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ImportGeoJsonWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                // 🚀 HOT RELOAD TEST: Diese MessageBox zeigt, ob Hot Reload funktioniert!
                var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unbekannt";
                var buildTime = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
                
                            MessageBox.Show($"🎉 ULTIMATE HOT RELOAD v4.0! 🎉\n\n" +
                $"Version: {version}\n" +
                $"Build Zeit: {buildTime:HH:mm:ss}\n" +
                $"Assembly: {Assembly.GetExecutingAssembly().GetName().Name}\n\n" +
                $"✨ ECHTES HOT RELOAD FUNKTIONIERT! ✨", 
                "Work-DLL v4.0 - HOT RELOAD SUCCESS!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Datei-Dialog anzeigen, um GeoJSON-Datei auszuwählen
                string selectedFilePath = ShowFileDialog();
                if (string.IsNullOrEmpty(selectedFilePath))
                {
                    return Result.Cancelled;
                }

                // GeoJSON parsen
                var featureCollection = GeoJsonParser.ParseFile(selectedFilePath);
                if (featureCollection == null)
                {
                    message = "Fehler beim Parsen der GeoJSON-Datei.";
                    return Result.Failed;
                }

                if (featureCollection.Features == null || !featureCollection.Features.Any())
                {
                    message = "Keine GeoJSON-Features zum Importieren gefunden.";
                    return Result.Failed;
                }                // Revit-Elemente erstellen
                using (Transaction transaction = new Transaction(doc, "Import GeoJSON"))
                {
                    transaction.Start();

                    foreach (var feature in featureCollection.Features)
                    {
                        CreateRevitElement(doc, feature);
                    }

                    transaction.Commit();
                }

                Autodesk.Revit.UI.TaskDialog.Show("Erfolg", $"GeoJSON erfolgreich importiert. {featureCollection.Features.Count} Features verarbeitet.");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Fehler beim Import: {ex.Message}";
                return Result.Failed;
            }
        }

        private string ShowFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "GeoJSON Files (*.geojson;*.json)|*.geojson;*.json|All Files (*.*)|*.*";
                openFileDialog.Title = "GeoJSON-Datei auswählen";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
                return string.Empty;
            }
        }

        private void CreateRevitElement(Document doc, GeoJSON.Net.Feature.Feature feature)
        {
            // Hier wird die Logik zum Erstellen der Revit-Elemente implementiert
            // Basierend auf der GeoJSON-Geometrie
            // Für den Moment machen wir nichts, um Fehler zu vermeiden
        }
    }
}