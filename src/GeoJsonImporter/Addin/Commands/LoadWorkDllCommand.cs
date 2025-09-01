using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class LoadWorkDllCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🚀 Lade Work-DLL über WorkDllManager
                bool success = WorkDllManager.LoadWorkDll();
                
                if (success)
                {
                    TaskDialog.Show("Hot Reload", "✅ Work-DLL erfolgreich geladen!");
                    return Result.Succeeded;
                }
                else
                {
                    message = "Fehler beim Laden der Work-DLL";
                    TaskDialog.Show("Hot Reload Fehler", "❌ Work-DLL konnte nicht geladen werden!");
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = $"LoadWorkDll Fehler: {ex.Message}";
                TaskDialog.Show("LoadWorkDll Fehler", $"❌ Fehler beim Laden der Work-DLL:\n\n{ex.Message}");
                return Result.Failed;
            }
        }
    }
}
