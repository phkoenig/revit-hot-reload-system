using Autodesk.Revit.UI;
using System.Reflection;

namespace GeoJsonImporter.Addin
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "GeoJSON Importer";
            application.CreateRibbonTab(tabName);

            // Import Panel
            RibbonPanel importPanel = application.CreateRibbonPanel(tabName, "Import");
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            var importButtonData = new PushButtonData(
                "ImportGeoJson",
                "Import\nGeoJSON",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.ImportGeoJsonCommand");

            // Import Button zum Panel hinzufügen
            importPanel.AddItem(importButtonData);

            // Development Panel mit separaten Unload/Load Buttons
            RibbonPanel devPanel = application.CreateRibbonPanel(tabName, "Development");
            
            // Unload Work-DLL Button
            var unloadButtonData = new PushButtonData(
                "UnloadWorkDll",
                "Unload\nWork-DLL",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.UnloadWorkDllCommand");

            // Load Work-DLL Button  
            var loadButtonData = new PushButtonData(
                "LoadWorkDll",
                "Load\nWork-DLL",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.LoadWorkDllCommand");

            // Buttons zum Panel hinzufügen
            devPanel.AddItem(unloadButtonData);
            devPanel.AddItem(loadButtonData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}