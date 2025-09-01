using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class UnloadWorkDllCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🚀 Entlade Work-DLL über WorkDllManager
                bool success = WorkDllManager.UnloadWorkDll();
                
                if (success)
                {
                    TaskDialog.Show("Hot Reload", "✅ Work-DLL erfolgreich entladen!");
                    return Result.Succeeded;
                }
                else
                {
                    message = "Fehler beim Entladen der Work-DLL";
                    TaskDialog.Show("Hot Reload Fehler", "❌ Work-DLL konnte nicht entladen werden!");
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = $"UnloadWorkDll Fehler: {ex.Message}";
                TaskDialog.Show("UnloadWorkDll Fehler", $"❌ Fehler beim Entladen der Work-DLL:\n\n{ex.Message}");
                return Result.Failed;
            }
        }
    }
}
