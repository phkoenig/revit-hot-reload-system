using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class UnloadWorkDllCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Delegiere an den WorkDllManager für Unload
                bool success = WorkDllManager.UnloadWorkDll();
                
                if (success)
                {
                    MessageBox.Show("✅ Work-DLL erfolgreich entladen!\n\n" +
                        "Die Work-DLL ist jetzt freigegeben.\n" +
                        "Du kannst jetzt ein neues Build machen!", 
                        "Work-DLL Unload Erfolgreich", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return Result.Succeeded;
                }
                else
                {
                    MessageBox.Show("⚠️ Work-DLL Unload Fehler!\n\n" +
                        "Siehe Log für Details.", 
                        "Work-DLL Unload Fehler", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Fehler beim Work-DLL Unload:\n\n{ex.Message}", 
                    "Unload Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
    }
}
