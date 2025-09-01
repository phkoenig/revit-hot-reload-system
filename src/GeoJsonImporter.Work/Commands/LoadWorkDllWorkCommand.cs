using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;
using System;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class LoadWorkDllWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Lade die Work-DLL über den WorkDllManager
                // Da wir in der Work-DLL sind, können wir den WorkDllManager nicht direkt aufrufen
                // Stattdessen zeigen wir dem Benutzer, was zu tun ist
                
                MessageBox.Show("✅ Work-DLL ist bereits geladen!\n\nDa Sie diesen Button aus der Work-DLL aufrufen, ist sie bereits aktiv.\n\nUm die Work-DLL neu zu laden:\n1. Klicken Sie 'Unload Work-DLL'\n2. Bauen Sie die Work-DLL neu\n3. Klicken Sie 'Load Work-DLL'", 
                    "Work-DLL Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Load Work-DLL Fehler: {ex.Message}";
                MessageBox.Show($"❌ Load Work-DLL Fehler:\n\n{ex.Message}",
                    "Load Work-DLL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
    }
}