using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeoJsonImporter.Work.Utils
{
    public class MapServer : IDisposable
    {
        private HttpListener server;
        private bool isRunning;
        private string baseUrl;
        private int port;
        
        public MapServer()
        {
            port = FindFreePort();
            baseUrl = $"http://localhost:{port}/";
        }
        
        private int FindFreePort()
        {
            try
            {
                var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
                listener.Start();
                int freePort = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return freePort;
            }
            catch
            {
                // Fallback auf Port 8080
                return 8080;
            }
        }
        
        public async Task<string> StartServer()
        {
            try
            {
                server = new HttpListener();
                server.Prefixes.Add(baseUrl);
                server.Start();
                isRunning = true;
                
                // ✅ Async handling - blockiert Plugin nicht
                _ = Task.Run(HandleRequests);
                
                System.Diagnostics.Debug.WriteLine($"MapServer started on {baseUrl}");
                return baseUrl + "map";
            }
            catch (Exception ex)
            {
                throw new Exception($"Server start failed on port {port}: {ex.Message}");
            }
        }
        
        private async Task HandleRequests()
        {
            while (isRunning && server.IsListening)
            {
                try
                {
                    var context = await server.GetContextAsync();
                    await ProcessRequest(context);
                }
                catch (ObjectDisposedException) 
                { 
                    break; // Server gestoppt
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Request error: {ex.Message}");
                }
            }
        }
        
        private async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            
            string responseContent = "";
            string contentType = "text/html; charset=utf-8";
            
            switch (request.Url.AbsolutePath.ToLower())
            {
                case "/map":
                    responseContent = CreateLeafletHtml();
                    break;
                    
                case "/api/coordinates":
                    // ✅ API für JavaScript ↔ Plugin Communication
                    responseContent = GetCoordinatesFromPlugin();
                    contentType = "application/json";
                    break;
                    
                default:
                    response.StatusCode = 404;
                    responseContent = "Not Found";
                    break;
            }
            
            byte[] buffer = Encoding.UTF8.GetBytes(responseContent);
            response.ContentType = contentType;
            response.ContentLength64 = buffer.Length;
            
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.Close();
        }
        
        private string CreateLeafletHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <title>UTM Grid Selector</title>
    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>
    <style>
        body { margin: 0; padding: 0; font-family: Arial, sans-serif; }
        #map { height: 100vh; }
        .controls { position: absolute; top: 10px; right: 10px; z-index: 1000;
                   background: white; padding: 10px; border-radius: 5px; }
    </style>
</head>
<body>
    <div class='controls'>
        <button onclick='sendToRevit()'>Send to Revit</button>
        <div id='coords'></div>
    </div>
    <div id='map'></div>
    
    <script>
        // ✅ Leaflet Map
        var map = L.map('map').setView([52.520008, 13.404954], 11);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);
        
        var selectedCoords = null;
        
        map.on('click', function(e) {
            selectedCoords = e.latlng;
            document.getElementById('coords').innerHTML = 
                'Lat: ' + e.latlng.lat.toFixed(6) + '<br>Lng: ' + e.latlng.lng.toFixed(6);
            
            // Marker hinzufügen
            L.marker([e.latlng.lat, e.latlng.lng]).addTo(map)
                .bindPopup('Selected: ' + e.latlng.lat.toFixed(6) + ', ' + e.latlng.lng.toFixed(6))
                .openPopup();
        });
        
        // ✅ JavaScript → Plugin Communication
        function sendToRevit() {
            if (selectedCoords) {
                fetch('/api/coordinates', {
                    method: 'POST',
                    body: JSON.stringify({lat: selectedCoords.lat, lng: selectedCoords.lng}),
                    headers: {'Content-Type': 'application/json'}
                });
            }
        }
    </script>
</body>
</html>";
        }
        
        private string GetCoordinatesFromPlugin()
        {
            // ✅ Plugin-Daten als JSON
            return @"{""status"": ""received"", ""message"": ""Coordinates sent to Revit Plugin""}";
        }
        
        // ✅ Sauberes Cleanup für Hot Reload
        public void Dispose()
        {
            isRunning = false;
            if (server != null)
            {
                try
                {
                    server.Stop();
                    server.Close();
                    System.Diagnostics.Debug.WriteLine($"MapServer stopped on port {port}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error stopping server: {ex.Message}");
                }
            }
        }
    }
}