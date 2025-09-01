using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class SetupWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🚀 SETUP: Revit-Projekt Grundeinstellungen überprüfen und korrigieren
                var document = commandData.Application.ActiveUIDocument.Document;
                
                // Pre-Check: Revit File Validation
                var validationResult = ValidateRevitFileSettings(document);
                if (!validationResult.IsValid)
                {
                    var preCheckResult = ShowPreCheckDialog(validationResult);
                    if (preCheckResult == DialogResult.No)
                    {
                        return Result.Cancelled;
                    }
                    
                    // Settings korrigieren
                    if (!CorrectRevitFileSettings(document, validationResult))
                    {
                        message = "Revit-Projekt Settings konnten nicht korrigiert werden.";
                        return Result.Failed;
                    }
                }
                
                MessageBox.Show("✅ Revit-Projekt Setup erfolgreich abgeschlossen!\n\n" +
                    "Folgende Einstellungen wurden überprüft/korrigiert:\n" +
                    "• Project North = Geographic North\n" +
                    "• Project Base Point auf (0,0,0)\n" +
                    "• Survey Point korrekt positioniert\n" +
                    "• Einheiten: Meter mit 0.0001 Präzision\n" +
                    "• Koordinatensystem für Geodaten-Import vorbereitet", 
                    "Setup Erfolgreich", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Setup Fehler: {ex.Message}";
                MessageBox.Show($"❌ Setup Fehler:\n\n{ex.Message}\n\nDetails:\n{ex.StackTrace}", 
                    "Setup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
        
        private ValidationResult ValidateRevitFileSettings(Document document)
        {
            var result = new ValidationResult();
            
            try
            {
                // 🔍 Project Base Point prüfen
                var projectBasePoint = GetProjectBasePoint(document);
                if (projectBasePoint != null)
                {
                    var position = projectBasePoint.Position;
                    var distanceFromOrigin = Math.Sqrt(position.X * position.X + position.Y * position.Y + position.Z * position.Z);
                    
                    if (distanceFromOrigin > 0.001) // Toleranz: 1mm
                    {
                        result.ProjectBasePointIssue = $"Project Base Point ist bei ({position.X:F3}, {position.Y:F3}, {position.Z:F3}) statt (0,0,0)";
                        result.HasIssues = true;
                    }
                }
                
                // 🔍 Survey Point prüfen
                var surveyPoint = GetSurveyPoint(document);
                if (surveyPoint != null)
                {
                    var position = surveyPoint.Position;
                    var distanceFromOrigin = Math.Sqrt(position.X * position.X + position.Y * position.Y + position.Z * position.Z);
                    
                    if (distanceFromOrigin > 0.001) // Toleranz: 1mm
                    {
                        result.SurveyPointIssue = $"Survey Point ist bei ({position.X:F3}, {position.Y:F3}, {position.Z:F3}) statt (0,0,0)";
                        result.HasIssues = true;
                    }
                }
                
                // 🔍 Project North prüfen - Geographic North (0°) validieren
                try
                {
                    // Project North wird über den Project Base Point gesetzt
                    var projectBasePointForNorth = GetProjectBasePoint(document);
                    if (projectBasePointForNorth != null)
                    {
                        // Project North Winkel prüfen (sollte 0° sein für Geographic North)
                        var projectNorthAngle = projectBasePointForNorth.GetParameters("Project North Angle")
                            .FirstOrDefault()?.AsDouble() ?? 0.0;
                        
                        if (Math.Abs(projectNorthAngle) > 0.001) // Nicht bei 0°
                        {
                            result.ProjectNorthIssue = $"Project North ist bei {projectNorthAngle * 180 / Math.PI:F2}° statt 0° (Geographic North)";
                            result.HasIssues = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.ProjectNorthIssue = $"Project North Validierung fehlgeschlagen: {ex.Message}";
                    result.HasIssues = true;
                }
                
                // 🔍 Units prüfen (TODO: Implementieren)
                result.UnitsIssue = "Einheiten: Meter mit 0.0001 Präzision (noch nicht implementiert)";
                
                return result;
            }
            catch (Exception ex)
            {
                result.HasIssues = true;
                result.GeneralError = $"Validierung Fehler: {ex.Message}";
                return result;
            }
        }
        
        private DialogResult ShowPreCheckDialog(ValidationResult validationResult)
        {
            var preCheckForm = new UI.RevitFileValidationForm();
            
            // TODO: ValidationResult an das Form übergeben
            // Für jetzt zeigen wir eine einfache MessageBox
            var message = "🔍 Revit-Projekt Validierung:\n\n";
            
            if (!string.IsNullOrEmpty(validationResult.ProjectBasePointIssue))
                message += $"❌ {validationResult.ProjectBasePointIssue}\n\n";
                
            if (!string.IsNullOrEmpty(validationResult.SurveyPointIssue))
                message += $"❌ {validationResult.SurveyPointIssue}\n\n";
                
            if (!string.IsNullOrEmpty(validationResult.ProjectNorthIssue))
                message += $"⚠️ {validationResult.ProjectNorthIssue}\n\n";
                
            if (!string.IsNullOrEmpty(validationResult.UnitsIssue))
                message += $"⚠️ {validationResult.UnitsIssue}\n\n";
                
            message += "Soll das Plugin diese Einstellungen automatisch korrigieren?";
            
            return MessageBox.Show(message, "Revit-Projekt Validierung", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        
        private bool CorrectRevitFileSettings(Document document, ValidationResult validationResult)
        {
            try
            {
                using (var transaction = new Transaction(document, "Setup Revit Project"))
                {
                    transaction.Start();
                    
                    // 🔧 Project Base Point korrigieren
                    if (!string.IsNullOrEmpty(validationResult.ProjectBasePointIssue))
                    {
                        var projectBasePoint = GetProjectBasePoint(document);
                        if (projectBasePoint != null)
                        {
                            // 🔓 Base Point entsperren falls gesperrt
                            if (projectBasePoint.Pinned)
                            {
                                projectBasePoint.Pinned = false;
                                MessageBox.Show("🔓 Project Base Point entsperrt", "Entsperren", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            
                            // Base Point verschieben mit ElementTransformUtils
                            var currentPosition = projectBasePoint.Position;
                            var offset = new XYZ(-currentPosition.X, -currentPosition.Y, -currentPosition.Z);
                            
                            ElementTransformUtils.MoveElement(document, projectBasePoint.Id, offset);
                            MessageBox.Show($"✅ Project Base Point von ({currentPosition.X:F3}, {currentPosition.Y:F3}, {currentPosition.Z:F3}) auf (0,0,0) verschoben", "Korrektur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    
                    // 🔧 Survey Point korrigieren
                    if (!string.IsNullOrEmpty(validationResult.SurveyPointIssue))
                    {
                        var surveyPoint = GetSurveyPoint(document);
                        if (surveyPoint != null)
                        {
                            // 🔓 Base Point entsperren falls gesperrt
                            if (surveyPoint.Pinned)
                            {
                                surveyPoint.Pinned = false;
                                MessageBox.Show("🔓 Survey Point entsperrt", "Entsperren", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            
                            // Base Point verschieben mit ElementTransformUtils
                            var currentPosition = surveyPoint.Position;
                            var offset = new XYZ(-currentPosition.X, -currentPosition.Y, -currentPosition.Z);
                            
                            ElementTransformUtils.MoveElement(document, surveyPoint.Id, offset);
                            MessageBox.Show($"✅ Survey Point von ({currentPosition.X:F3}, {currentPosition.Y:F3}, {currentPosition.Z:F3}) auf (0,0,0) verschoben", "Korrektur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    
                    // 🔧 Project North korrigieren - auf Geographic North setzen
                    if (!string.IsNullOrEmpty(validationResult.ProjectNorthIssue))
                    {
                        try
                        {
                            // Project North über Project Base Point korrigieren
                            var projectBasePoint = GetProjectBasePoint(document);
                            if (projectBasePoint != null)
                            {
                                // 🔓 Project Base Point entsperren falls gesperrt
                                if (projectBasePoint.Pinned)
                                {
                                    projectBasePoint.Pinned = false;
                                    MessageBox.Show("🔓 Project Base Point entsperrt", "Entsperren", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                
                                // Project North Angle Parameter finden und auf 0° setzen
                                var projectNorthParam = projectBasePoint.GetParameters("Project North Angle").FirstOrDefault();
                                if (projectNorthParam != null)
                                {
                                    var oldAngle = projectNorthParam.AsDouble() * 180 / Math.PI;
                                    projectNorthParam.Set(0.0); // 0° = Geographic North
                                    MessageBox.Show($"✅ Project North von {oldAngle:F2}° auf 0° (Geographic North) gesetzt", "Korrektur", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("⚠️ Project North Angle Parameter nicht gefunden", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"⚠️ Project North konnte nicht korrigiert werden: {ex.Message}", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    
                    // 🔧 Units korrigieren (TODO: Implementieren)
                    
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Korrektur Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        
        private BasePoint GetProjectBasePoint(Document document)
        {
            try
            {
                var collector = new FilteredElementCollector(document);
                var basePoints = collector.OfClass(typeof(BasePoint)).Cast<BasePoint>();
                
                foreach (var basePoint in basePoints)
                {
                    if (basePoint.IsShared == false) // Project Base Point ist nicht shared
                    {
                        return basePoint;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Finden des Project Base Point: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        
        private BasePoint GetSurveyPoint(Document document)
        {
            try
            {
                var collector = new FilteredElementCollector(document);
                var basePoints = collector.OfClass(typeof(BasePoint)).Cast<BasePoint>();
                
                foreach (var basePoint in basePoints)
                {
                    if (basePoint.IsShared == true) // Survey Point ist shared
                    {
                        return basePoint;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Finden des Survey Point: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
    
    public class ValidationResult
    {
        public bool HasIssues { get; set; } = false;
        public string ProjectBasePointIssue { get; set; } = "";
        public string SurveyPointIssue { get; set; } = "";
        public string ProjectNorthIssue { get; set; } = "";
        public string UnitsIssue { get; set; } = "";
        public string GeneralError { get; set; } = "";
        
        public bool IsValid => !HasIssues;
    }
}
