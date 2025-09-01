using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class UtmGridSetupWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🚀 UTM GRID SETUP: Revit-File für Geodaten-Import vorbereiten
                var document = commandData.Application.ActiveUIDocument.Document;
                
                // Pre-Check: Revit File Validation
                if (!ValidateRevitFileSettings(document))
                {
                    var preCheckResult = ShowPreCheckDialog();
                    if (preCheckResult == DialogResult.Cancel)
                    {
                        return Result.Cancelled;
                    }
                    
                    // Settings korrigieren
                    if (!CorrectRevitFileSettings(document))
                    {
                        message = "Revit-File Settings konnten nicht korrigiert werden.";
                        return Result.Failed;
                    }
                }
                
                // Haupt-Dialog: UTM Grid Configuration
                var utmGridDialog = new UI.UtmGridSetupDialog();
                var dialogResult = utmGridDialog.ShowDialog();
                
                if (dialogResult == true) // WPF Dialog Result
                {
                    // Grid in Revit erstellen
                    CreateUtmGridInRevit(document, utmGridDialog.GridConfiguration);
                    return Result.Succeeded;
                }
                
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = $"UTM Grid Setup Fehler: {ex.Message}";
                MessageBox.Show($"❌ UTM Grid Setup Fehler:\n\n{ex.Message}\n\nDetails:\n{ex.StackTrace}", 
                    "UTM Grid Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
        
        private bool ValidateRevitFileSettings(Document document)
        {
            // TODO: Implementiere Validation Logic
            // - Project North = Geographic North
            // - Base Points auf (0,0,0)
            // - Einheiten: Meter mit 0.0001 Präzision
            return false; // Für jetzt immer Pre-Check Dialog zeigen
        }
        
        private DialogResult ShowPreCheckDialog()
        {
            var preCheckForm = new UI.RevitFileValidationForm();
            return preCheckForm.ShowDialog();
        }
        
        private bool CorrectRevitFileSettings(Document document)
        {
            // TODO: Implementiere Settings Correction
            // - Set Project North = Geographic North
            // - Move Base Points to (0,0,0)
            // - Set Units to Meters with 0.0001 precision
            return true; // Für jetzt immer erfolgreich
        }
        
        private void CreateUtmGridInRevit(Document document, object gridConfiguration)
        {
            // TODO: Implementiere Grid Creation
            // - Create UTM Grid Type
            // - Create Grid Lines
            // - Name Grid Lines with EPSG codes
            MessageBox.Show("🗺️ UTM Grid würde jetzt erstellt werden!\n\n" +
                "Grid Configuration empfangen und verarbeitet.", 
                "UTM Grid Creation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
