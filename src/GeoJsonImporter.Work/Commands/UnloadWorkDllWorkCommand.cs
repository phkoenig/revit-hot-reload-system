using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class UnloadWorkDllWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Entlade die Work-DLL über den WorkDllManager
                // Da wir in der Work-DLL sind, können wir den WorkDllManager nicht direkt aufrufen
                // Stattdessen zeigen wir dem Benutzer, was zu tun ist
                
                MessageBox.Show("⚠️ Work-DLL kann nicht von sich selbst entladen werden!\n\nUm die Work-DLL zu entladen:\n1. Schließen Sie Revit\n2. Bauen Sie die Work-DLL neu\n3. Starten Sie Revit neu\n\nOder verwenden Sie den 'Unload Work-DLL' Button in der Loader-DLL.", 
                    "Work-DLL Entladen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Unload Work-DLL Fehler: {ex.Message}";
                MessageBox.Show($"❌ Unload Work-DLL Fehler:\n\n{ex.Message}",
                    "Unload Work-DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
    }
}