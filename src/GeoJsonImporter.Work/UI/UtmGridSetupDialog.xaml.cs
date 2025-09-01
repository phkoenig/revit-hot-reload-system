using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using GeoJsonImporter.Work.Utils;

namespace GeoJsonImporter.Work.UI
{
    public partial class UtmGridSetupDialog : Window, IDisposable
    {
        public object GridConfiguration { get; private set; }
        private bool _disposed = false;
        private MapServer mapServer;
        
        private double currentLatitude = 52.5370; // Berlin Default
        private double currentLongitude = 13.3500; // Berlin Default
        private int currentGridSize = 1000; // Default 1000m
        
        public UtmGridSetupDialog()
        {
            InitializeComponent();
        }
        

        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                mapServer = new MapServer();
                string mapUrl = await mapServer.StartServer();
                
                // ✅ System-Browser öffnen
                Process.Start(new ProcessStartInfo(mapUrl) 
                { 
                    UseShellExecute = true 
                });
                
                System.Windows.MessageBox.Show($"✅ Map Server gestartet: {mapUrl}\nBrowser sollte sich öffnen!", 
                    "Server Running");
                
                UpdateCoordinateDisplays();
                UpdateGridInfo();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Server-Fehler: {ex.Message}", "Fehler");
            }
        }
        

        

        
        // OnMapMessageReceived wurde durch MapCallbackHandler.OnMapClick ersetzt
        
        private void PlaceButton_Click(object sender, RoutedEventArgs e)
        {
            var address = AddressTextBox.Text.Trim();
            if (string.IsNullOrEmpty(address)) return;
            
            // TODO: Implement Geocoding
            // For now, just show message
            System.Windows.MessageBox.Show($"Geocoding für Adresse: '{address}'\n\n" +
                "Diese Funktion wird implementiert:\n" +
                "1. Nominatim API Geocoding\n" +
                "2. Karte auf Ergebnis zentrieren\n" +
                "3. Koordinaten aktualisieren", 
                "Geocoding", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void GridSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentGridSize = (int)e.NewValue;
            if (GridSizeLabel != null)
            {
                GridSizeLabel.Text = $"Aktuell: {currentGridSize}m x {currentGridSize}m";
            }
            
            UpdateGridInfo();
            UpdateMapGrid();
        }
        
        private void ShowGridOverlayCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateMapGrid();
        }
        
                private void UpdateMapGrid()
        {
            // ✅ CefSharp temporär deaktiviert
            // Map Grid Update wird später mit CefSharp implementiert
            var showGrid = ShowGridOverlayCheckBox?.IsChecked == true;
            // ✅ CefSharp temporär deaktiviert - Grid Update wird später implementiert
            System.Diagnostics.Debug.WriteLine($"Grid Update: Lat={currentLatitude:F4}, Lng={currentLongitude:F4}, Grid={showGrid}");
        }
        
        /// <summary>
        /// Wird vom MapCallbackHandler aufgerufen wenn auf Karte geklickt wird
        /// </summary>
        public void UpdateCoordinatesFromMap(double latitude, double longitude)
        {
            currentLatitude = latitude;
            currentLongitude = longitude;
            
            // UI auf Main Thread aktualisieren
            Dispatcher.Invoke(() =>
            {
                UpdateCoordinateDisplays();
                UpdateGridInfo();
                UpdateMapGrid();
            });
        }
        
        private void UpdateCoordinateDisplays()
        {
            // Update WGS84 coordinates
            WgsCoordinatesTextBox.Text = $"{currentLatitude:F6}, {currentLongitude:F6}";
            
            // TODO: Implement proper UTM conversion
            // For now, use approximation
            var utmEasting = 372000 + (currentLongitude - 13.3500) * 70000; // Rough approximation
            var utmNorthing = 5814000 + (currentLatitude - 52.5370) * 111000; // Rough approximation
            
            UtmCoordinatesTextBox.Text = $"{utmEasting:F0}, {utmNorthing:F0}";
            
            // Round to 100m
            var roundedEasting = Math.Round(utmEasting / 100) * 100;
            var roundedNorthing = Math.Round(utmNorthing / 100) * 100;
            UtmRoundedTextBox.Text = $"{roundedEasting:F0}, {roundedNorthing:F0}";
        }
        
        private void UpdateGridInfo()
        {
            if (GridInfoTextBlock != null)
            {
                var gridLines = (currentGridSize / 100) + 1;
                GridInfoTextBlock.Text = $"Grid: {gridLines} x {gridLines} Linien\n" +
                                       $"Abstand: 100m\n" +
                                       $"Bereich: {currentGridSize}m x {currentGridSize}m";
            }
        }
        
        private void CreateGridButton_Click(object sender, RoutedEventArgs e)
        {
            // Sammle Grid-Konfiguration
            GridConfiguration = new
            {
                CenterLatitude = currentLatitude,
                CenterLongitude = currentLongitude,
                UtmZone = UtmZoneComboBox.SelectedIndex == 0 ? "EPSG:25832" : "EPSG:25833",
                GridSize = currentGridSize,
                Address = AddressTextBox.Text
            };
            
            this.DialogResult = true;
            this.Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        
        private void Window_Loaded_Original(object sender, RoutedEventArgs e)
        {
            UpdateCoordinateDisplays();
            UpdateGridInfo();
        }
        

        
        // ✅ Hot Reload-sicheres Cleanup
        public void Dispose()
        {
            mapServer?.Dispose();
        }
        
        protected override void OnClosed(EventArgs e)
        {
            Dispose();
            base.OnClosed(e);
        }
    }
}
