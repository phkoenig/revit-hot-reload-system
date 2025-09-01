using System;
using System.IO;
using System.Diagnostics;

namespace GeoJsonImporter.Utils
{
    public static class HotReloadLogger
    {
        private static readonly string LogFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "GeoJsonImporter_HotReload.log");

        public static void Info(string message)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] INFO: {message}";
            
            // 🎯 CONSOLE OUTPUT (sichtbar in Visual Studio Output)
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
            
            // 📝 FILE LOGGING (persistent)
            try
            {
                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
            catch { /* Ignore file errors */ }
        }

        public static void Error(string message, Exception ex = null)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}";
            if (ex != null)
                logMessage += $" | Exception: {ex.Message}";
            
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
            
            try
            {
                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
            catch { /* Ignore file errors */ }
        }

        public static void Success(string message)
        {
            var logMessage = $"[{DateTime.Now:HH:mm:ss}] SUCCESS: {message}";
            
            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
            
            try
            {
                File.AppendAllText(LogFile, logMessage + Environment.NewLine);
            }
            catch { /* Ignore file errors */ }
        }

        public static void ClearLog()
        {
            try
            {
                if (File.Exists(LogFile))
                    File.Delete(LogFile);
            }
            catch { /* Ignore file errors */ }
        }

        public static string GetLogPath() => LogFile;
    }
}