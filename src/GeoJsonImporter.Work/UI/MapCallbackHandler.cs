using System;

namespace GeoJsonImporter.Work.UI
{
    /// <summary>
    /// JavaScript ↔ C# Bridge für CefSharp Map-Interaktion
    /// </summary>
    public class MapCallbackHandler
    {
        private readonly UtmGridSetupDialog _dialog;

        public MapCallbackHandler(UtmGridSetupDialog dialog)
        {
            _dialog = dialog;
        }

        /// <summary>
        /// Wird von JavaScript aufgerufen wenn auf die Karte geklickt wird
        /// </summary>
        public void OnMapClick(double latitude, double longitude)
        {
            try
            {
                // Koordinaten an Dialog weiterleiten
                _dialog.UpdateCoordinatesFromMap(latitude, longitude);
            }
            catch (Exception ex)
            {
                // Fehler ignorieren - Map-Interaktion soll nicht crashen
                System.Diagnostics.Debug.WriteLine($"MapCallback Error: {ex.Message}");
            }
        }
    }
}
