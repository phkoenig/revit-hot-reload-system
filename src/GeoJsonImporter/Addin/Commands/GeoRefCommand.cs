using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GeoRefCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🚀 PROXY: Delegiere an Work-DLL über WorkDllManager
                return WorkDllManager.ExecuteWorkCommand(commandData, ref message, elements);
            }
            catch (Exception ex)
            {
                message = $"Fehler: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}
