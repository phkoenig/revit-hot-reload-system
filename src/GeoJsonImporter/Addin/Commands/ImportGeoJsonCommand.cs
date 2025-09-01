using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ImportGeoJsonCommand : IExternalCommand
    {
        // 🎯 LOADER-PROXY: Delegiert an die Work-DLL über den WorkDllManager
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Delegiere an die Work-DLL über den neuen WorkDllManager
            return WorkDllManager.ExecuteWorkCommand(commandData, ref message, elements);
        }
    }
}