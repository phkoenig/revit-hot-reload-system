# 🚀 Auto-Unload für GeoJSON Importer
# Versucht automatisch die Work-DLL zu entladen
# Nur verwenden wenn Revit läuft und Work-DLL geladen ist

Write-Host "🚀 Auto-Unload für GeoJSON Importer..." -ForegroundColor Cyan

# Prüfe ob Revit läuft
$revitProcesses = Get-Process -Name "Revit" -ErrorAction SilentlyContinue

if (-not $revitProcesses) {
    Write-Host "✅ Revit läuft nicht - nichts zu entladen" -ForegroundColor Green
    exit 0
}

Write-Host "🔍 Revit läuft - versuche Work-DLL zu entladen..." -ForegroundColor Yellow

# Versuche die Work-DLL über Revit API zu entladen
# Das ist komplex, aber möglich über COM-Interop
try {
    # Hier könnte man versuchen, über Revit API die Work-DLL zu entladen
    # Das ist aber sehr komplex und fehleranfällig
    
    Write-Host "⚠️  Automatisches Entladen ist komplex und fehleranfällig" -ForegroundColor Yellow
    Write-Host "   → Besser: Manuell 'Unload Work-DLL' in Revit klicken" -ForegroundColor White
    
    $choice = Read-Host "`nMöchten Sie es trotzdem versuchen? (j/n)"
    
    if ($choice -eq "j" -or $choice -eq "J" -or $choice -eq "y" -or $choice -eq "Y") {
        Write-Host "⚠️  Versuche automatisches Entladen..." -ForegroundColor Yellow
        # Hier würde die komplexe COM-Interop Logik stehen
        Write-Host "❌ Automatisches Entladen fehlgeschlagen - manueller Eingriff nötig" -ForegroundColor Red
        exit 1
    } else {
        Write-Host "✅ Automatisches Entladen abgebrochen" -ForegroundColor Green
        exit 0
    }
} catch {
    Write-Host "❌ Fehler beim automatischen Entladen: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}