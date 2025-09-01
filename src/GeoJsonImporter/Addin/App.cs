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

            // 📁 IMPORT BUTTON (dritter Button) - Zeigt auf Loader-DLL Proxy-Command!
            var importButtonData = new PushButtonData(
                "ImportGeoJson",
                "Import\nGeoJSON",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.ImportGeoJsonCommand"); // ✅ Proxy-Command in Loader-DLL

            // Import Button zum Panel hinzufügen
            importPanel.AddItem(importButtonData);

            // Setup Panel für Geodaten-Vorbereitung
            RibbonPanel setupPanel = application.CreateRibbonPanel(tabName, "Setup");
            
            // ⚙️ SETUP BUTTON (erster Button) - Zeigt auf Loader-DLL Proxy-Command!
            var setupButtonData = new PushButtonData(
                "Setup",
                "Setup",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.SetupCommand"); // ✅ Proxy-Command in Loader-DLL
                
            setupButtonData.ToolTip = "Revit-Projekt Grundeinstellungen überprüfen und korrigieren";
            setupButtonData.LongDescription = "Überprüft und korrigiert Project Base Point, Survey Point, Project North und Einheiten für Geodaten-Import";
            
            setupPanel.AddItem(setupButtonData);
            
            // 🧙‍♂️ ABRACADABRA Panel für einfache Tests
            RibbonPanel abracadabraPanel = application.CreateRibbonPanel(tabName, "Abracadabra");
            
            // 🧙‍♂️ ABRACADABRA BUTTON (einfacher Button ohne Proxy!)
            var abracadabraButtonData = new PushButtonData(
                "Abracadabra",
                "Abracadabra",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.AbracadabraCommand"); // ✅ Direkter Command in Loader-DLL
                
            abracadabraButtonData.ToolTip = "Einfacher Test-Button ohne komplexe Logik";
            abracadabraButtonData.LongDescription = "Zeigt einfach eine MessageBox - nichts weiter!";
            
            abracadabraPanel.AddItem(abracadabraButtonData);

            // Development Panel mit separaten Unload/Load Buttons
            RibbonPanel devPanel = application.CreateRibbonPanel(tabName, "Development");
            
            // Unload Work-DLL Button - Zeigt auf Loader-DLL Command!
            var unloadButtonData = new PushButtonData(
                "UnloadWorkDll",
                "Unload\nWork-DLL",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.UnloadWorkDllCommand"); // ✅ Loader-DLL Command

            // Load Work-DLL Button - Zeigt auf Loader-DLL Command!
            var loadButtonData = new PushButtonData(
                "LoadWorkDll",
                "Load\nWork-DLL",
                assemblyPath,
                "GeoJsonImporter.Addin.Commands.LoadWorkDllCommand"); // ✅ Loader-DLL Command

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