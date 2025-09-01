using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using GeoJsonImporter.Utils;

namespace GeoJsonImporter.Addin.Commands
{
    /// <summary>
    /// 🎯 WORK-DLL MANAGER: Zentrale Verwaltung für das Laden/Entladen der Work-DLL
    /// </summary>
    public static class WorkDllManager
    {
        // 🎯 STARKE REFERENZ: Verhindert Garbage Collection
        private static AssemblyLoadContext _workDllContext = null;
        private static string _workDllVersion = null;

        /// <summary>
        /// Entlädt die aktuelle Work-DLL und gibt das File frei
        /// </summary>
        public static bool UnloadWorkDll()
        {
            try
            {
                HotReloadLogger.Info("🔄 Starte Work-DLL Unload...");
                
                if (_workDllContext != null)
                {
                    var context = _workDllContext;
                    if (context != null)
                    {
                        HotReloadLogger.Info($"Unload Context: {context.Name}");
                        context.Unload();
                        HotReloadLogger.Info("Context.Unload() aufgerufen");
                    }
                }
                else
                {
                    HotReloadLogger.Info("Kein aktiver Context zum Unloaden");
                }

                // Starke Referenz zurücksetzen
                _workDllContext = null;
                _workDllVersion = null;

                // Garbage Collection erzwingen
                HotReloadLogger.Info("Force Garbage Collection...");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                HotReloadLogger.Info("✅ Work-DLL Unload abgeschlossen!");
                return true;
            }
            catch (Exception ex)
            {
                HotReloadLogger.Error("Work-DLL Unload Fehler", ex);
                return false;
            }
        }

        /// <summary>
        /// Lädt eine neue Work-DLL in einen separaten AssemblyLoadContext
        /// </summary>
        public static bool LoadWorkDll()
        {
            try
            {
                HotReloadLogger.Info("🚀 Starte Work-DLL Load...");

                // Work-DLL Pfad ermitteln
                string workDllPath = GetWorkDllPath();
                if (!File.Exists(workDllPath))
                {
                    HotReloadLogger.Error($"Work-DLL nicht gefunden: {workDllPath}");
                    return false;
                }

                HotReloadLogger.Info($"Work-DLL gefunden: {workDllPath}");

                // 🎯 NEUE AssemblyLoadContext mit isCollectible: true erstellen
                var contextName = "WorkDllContext_" + DateTime.Now.Ticks;
                var newContext = new WorkDllLoadContext(contextName, Path.GetDirectoryName(workDllPath));
                _workDllContext = newContext; // Strong reference

                HotReloadLogger.Info("AssemblyLoadContext erstellt");

                // Work-DLL in den Container laden
                var assembly = newContext.LoadFromAssemblyPath(workDllPath);
                HotReloadLogger.Info($"Assembly geladen: {assembly.FullName}");

                // Version speichern
                _workDllVersion = assembly.GetName().Version?.ToString() ?? "Unbekannt";
                
                HotReloadLogger.Info($"✅ Work-DLL Load erfolgreich! Version: {_workDllVersion}");
                return true;
            }
            catch (Exception ex)
            {
                HotReloadLogger.Error("Work-DLL Load Fehler", ex);
                return false;
            }
        }

        /// <summary>
        /// Führt einen Command in der geladenen Work-DLL aus
        /// </summary>
        public static Autodesk.Revit.UI.Result ExecuteWorkCommand(
            Autodesk.Revit.UI.ExternalCommandData commandData, 
            ref string message, 
            Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                // Prüfe ob Work-DLL geladen ist
                if (_workDllContext == null)
                {
                    HotReloadLogger.Info("Work-DLL ist nicht geladen - lade automatisch...");
                    if (!LoadWorkDll())
                    {
                        HotReloadLogger.Error("Automatisches Laden der Work-DLL fehlgeschlagen");
                        return Autodesk.Revit.UI.Result.Failed;
                    }
                }

                HotReloadLogger.Info("Suche ImportGeoJsonWorkCommand in Work-DLL...");

                // Suche die Work-Command Klasse
                var assemblies = _workDllContext.Assemblies.ToList();
                HotReloadLogger.Info($"Gefundene Assemblies: {assemblies.Count}");

                foreach (var assembly in assemblies)
                {
                    HotReloadLogger.Info($"Assembly: {assembly.FullName}");
                    var commandType = assembly.GetType("GeoJsonImporter.Work.Commands.ImportGeoJsonWorkCommand");
                    if (commandType != null)
                    {
                        HotReloadLogger.Info("ImportGeoJsonWorkCommand gefunden!");
                        
                        // Instanz erstellen und Execute aufrufen
                        var commandInstance = Activator.CreateInstance(commandType);
                        var executeMethod = commandType.GetMethod("Execute");
                        
                        if (executeMethod != null)
                        {
                            HotReloadLogger.Info("Execute Methode wird aufgerufen...");
                            var result = executeMethod.Invoke(commandInstance, new object[] { commandData, message, elements });
                            return (Autodesk.Revit.UI.Result)result;
                        }
                    }
                }

                HotReloadLogger.Error("ImportGeoJsonWorkCommand nicht gefunden in Work-DLL");
                return Autodesk.Revit.UI.Result.Failed;
            }
            catch (Exception ex)
            {
                // Zeige den ECHTEN Fehler - oft ist es InnerException
                var realEx = ex.InnerException ?? ex;
                HotReloadLogger.Error($"Work-Command Ausführung Fehler: {realEx.Message}", realEx);
                return Autodesk.Revit.UI.Result.Failed;
            }
        }

        /// <summary>
        /// Gibt die Version der aktuell geladenen Work-DLL zurück
        /// </summary>
        public static string GetWorkDllVersion()
        {
            return _workDllVersion ?? "Nicht geladen";
        }

        /// <summary>
        /// Prüft ob eine Work-DLL geladen ist
        /// </summary>
        public static bool IsWorkDllLoaded()
        {
            return _workDllContext != null && _workDllContext.Assemblies.Any();
        }

        /// <summary>
        /// Führt UTM Grid Setup Command in der Work-DLL aus
        /// </summary>
        public static Autodesk.Revit.UI.Result ExecuteUtmGridSetup(
            Autodesk.Revit.UI.ExternalCommandData commandData, 
            ref string message, 
            Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                HotReloadLogger.Info("🗺️ UTM Grid Setup Command aufgerufen");

                // Work-DLL laden falls nicht geladen
                if (!IsWorkDllLoaded())
                {
                    HotReloadLogger.Info("Work-DLL nicht geladen - lade automatisch...");
                    if (!LoadWorkDll())
                    {
                        message = "Work-DLL konnte nicht geladen werden!";
                        return Autodesk.Revit.UI.Result.Failed;
                    }
                }

                // Work-DLL Assembly holen
                var assembly = _workDllContext.Assemblies.FirstOrDefault();
                if (assembly != null)
                {
                    // Nach UtmGridSetupWorkCommand suchen
                    var commandType = assembly.GetTypes()
                        .FirstOrDefault(t => t.Name == "UtmGridSetupWorkCommand");

                    if (commandType != null)
                    {
                        HotReloadLogger.Info($"UtmGridSetupWorkCommand gefunden: {commandType.FullName}");
                        
                        var commandInstance = Activator.CreateInstance(commandType);
                        var executeMethod = commandType.GetMethod("Execute");
                        
                        if (executeMethod != null)
                        {
                            HotReloadLogger.Info("Execute-Methode wird aufgerufen...");
                            var result = executeMethod.Invoke(commandInstance, new object[] { commandData, message, elements });
                            HotReloadLogger.Info($"UTM Grid Setup Command ausgeführt: {result}");
                            return (Autodesk.Revit.UI.Result)result;
                        }
                    }
                }

                HotReloadLogger.Error("UtmGridSetupWorkCommand nicht gefunden in Work-DLL");
                return Autodesk.Revit.UI.Result.Failed;
            }
            catch (Exception ex)
            {
                var realEx = ex.InnerException ?? ex;
                HotReloadLogger.Error($"UTM Grid Setup Ausführung Fehler: {realEx.Message}", realEx);
                return Autodesk.Revit.UI.Result.Failed;
            }
        }

        /// <summary>
        /// Ermittelt den Pfad zur Work-DLL
        /// </summary>
        private static string GetWorkDllPath()
        {
            // 🎯 WORK-DLL wird aus dem WorkDll-Ordner geladen (NICHT gesperrt von Revit!)
            string basePath = @"B:\Nextcloud\CODE\revit-plugins\GeoJson_Importer";
            string workDllPath = Path.Combine(basePath, "WorkDll", "GeoJsonImporter.Work.dll");
            
            return Path.GetFullPath(workDllPath);
        }
    }

    /// <summary>
    /// 🎯 Custom AssemblyLoadContext für Work-DLL Dependencies
    /// </summary>
    public class WorkDllLoadContext : AssemblyLoadContext
    {
        private readonly string _workDllDirectory;

        public WorkDllLoadContext(string name, string workDllDirectory) : base(name, true)
        {
            _workDllDirectory = workDllDirectory;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Suche Dependencies im WorkDll-Ordner
            string assemblyPath = Path.Combine(_workDllDirectory, assemblyName.Name + ".dll");
            if (File.Exists(assemblyPath))
            {
                return LoadFromAssemblyPath(assemblyPath);
            }
            return null; // Lass den Standard-Resolver versuchen
        }
    }
}
