using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace GeoJsonImporter.Addin.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class ImportGeoJsonCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 🚀 PROXY: Delegiere an Work-DLL über WorkDllManager
            return WorkDllManager.ExecuteWorkCommand(commandData, ref message, elements);
        }
    }
}
