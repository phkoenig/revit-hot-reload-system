using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class AbracadabraCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🧙‍♂️ PROXY-COMMAND: Delegiert an Work-DLL über WorkDllManager
                return WorkDllManager.ExecuteAbracadabra(commandData, ref message, elements);
            }
            catch (Exception ex)
            {
                // Fallback: Zeigt "1" wenn Work-DLL nicht geladen ist
                message = $"Work-DLL Fehler: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}
