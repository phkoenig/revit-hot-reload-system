using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class LoadWorkDllCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Delegiere an den WorkDllManager für Load
                bool success = WorkDllManager.LoadWorkDll();
                
                if (success)
                {
                    string version = WorkDllManager.GetWorkDllVersion();
                    MessageBox.Show($"🚀 Work-DLL erfolgreich geladen!\n\n" +
                        $"Version: {version}\n\n" +
                        "Du kannst jetzt das Import Tool testen!", 
                        "Work-DLL Load Erfolgreich", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return Result.Succeeded;
                }
                else
                {
                    MessageBox.Show("⚠️ Work-DLL Load Fehler!\n\n" +
                        "Siehe Log für Details.", 
                        "Work-DLL Load Fehler", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Fehler beim Work-DLL Load:\n\n{ex.Message}", 
                    "Load Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Result.Failed;
            }
        }
    }
}
