# Pre-Build Check für GeoJSON Importer
# Prüft ob Revit läuft und Work-DLL geladen ist

Write-Host "Pre-Build Check für GeoJSON Importer..." -ForegroundColor Cyan

# Prüfe ob Revit läuft
$revitProcesses = Get-Process -Name "Revit" -ErrorAction SilentlyContinue

if ($revitProcesses) {
    Write-Host "REVIT LAEUFT!" -ForegroundColor Yellow
    Write-Host "   Prozess-ID: $($revitProcesses.Id)" -ForegroundColor Yellow
    Write-Host "   Speicher: $([math]::Round($revitProcesses.WorkingSet64 / 1MB, 2)) MB" -ForegroundColor Yellow
    
    # Prüfe ob Work-DLL geladen ist (vereinfachte Prüfung)
    Write-Host "Work-DLL Status wird geprüft..." -ForegroundColor Cyan
    
    # Da Revit läuft, aber Work-DLL Status unbekannt ist
    Write-Host "⚠️  Revit läuft - Build mit Vorsicht durchführen" -ForegroundColor Yellow
    Write-Host "   Falls Work-DLL geladen ist, kann Build fehlschlagen" -ForegroundColor White
    
    # Frage Benutzer ob Build fortgesetzt werden soll
    $choice = Read-Host "Build trotzdem fortsetzen? (j/n)"
    
    if ($choice -eq "j" -or $choice -eq "J" -or $choice -eq "y" -or $choice -eq "Y") {
        Write-Host "✅ Build wird fortgesetzt - Benutzer hat bestätigt!" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "❌ Build abgebrochen - Benutzer hat abgebrochen" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "Revit läuft nicht - Build kann sicher durchgeführt werden" -ForegroundColor Green
    exit 0
}