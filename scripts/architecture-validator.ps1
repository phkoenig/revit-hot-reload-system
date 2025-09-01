# 🚀 REVIT HOT RELOAD ARCHITEKTUR-VALIDATOR
# GeoJSON Importer - Automatische Architektur-Prüfung
# Erstellt: 2024-12-19
# AKTUALISIERT: 2024-12-19 (Funktionierende Architektur validiert!)

param(
    [string]$ProjectRoot = $PSScriptRoot
)

Write-Host "Pruefe Hot Reload Architektur..." -ForegroundColor Cyan

# 🏗️ ARCHITEKTUR-REGELN (FUNKTIONIERT!)
# Loader-DLL MUSS Proxy-Commands enthalten!
# App.cs MUSS auf Loader-DLL Proxy-Commands zeigen!

$errors = @()
$warnings = @()

# 📁 Pfade definieren
$loaderCommandsPath = Join-Path $ProjectRoot "src\GeoJsonImporter\Addin\Commands"
$workCommandsPath = Join-Path $ProjectRoot "src\GeoJsonImporter.Work\Commands"
$appCsPath = Join-Path $ProjectRoot "src\GeoJsonImporter\Addin\App.cs"
$workDllManagerPath = Join-Path $ProjectRoot "src\GeoJsonImporter\Addin\Commands\WorkDllManager.cs"

# 🔍 1. Loader-DLL prüfen
Write-Host "  Pruefe Loader-DLL..." -ForegroundColor Yellow

if (Test-Path $loaderCommandsPath) {
    $loaderCommands = Get-ChildItem $loaderCommandsPath -Filter "*.cs" | ForEach-Object { $_.BaseName }
    
    # ✅ ERFORDERLICHE Commands in Loader-DLL:
    # - WorkDllManager.cs (immer erforderlich)
    # - LoadWorkDllCommand.cs (immer erforderlich) 
    # - UnloadWorkDllCommand.cs (immer erforderlich)
    # - Proxy-Commands sind ERFORDERLICH!
    
    $requiredLoaderCommands = @("WorkDllManager", "LoadWorkDllCommand", "UnloadWorkDllCommand")
    $expectedProxyCommands = @("GeoRefCommand", "SetupCommand", "ImportGeoJsonCommand")
    
    # Prüfe erforderliche Commands
    foreach ($required in $requiredLoaderCommands) {
        if ($required -notin $loaderCommands) {
            $errors += "FEHLT: Erforderlicher Command '$required.cs' fehlt in Loader-DLL!"
        } else {
            Write-Host "    ✅ Erforderlicher Command gefunden: $required.cs" -ForegroundColor Green
        }
    }
    
    # Prüfe Proxy-Commands
    foreach ($proxy in $expectedProxyCommands) {
        if ($proxy -notin $loaderCommands) {
            $errors += "FEHLT: Proxy-Command '$proxy.cs' fehlt in Loader-DLL!"
        } else {
            Write-Host "    ✅ Proxy-Command gefunden: $proxy.cs" -ForegroundColor Green
        }
    }
}

# 🔍 2. App.cs prüfen
Write-Host "  Pruefe App.cs..." -ForegroundColor Yellow

if (Test-Path $appCsPath) {
    $appContent = Get-Content $appCsPath -Raw
    
    # ✅ KORREKTE ARCHITEKTUR: App.cs zeigt auf Loader-DLL Proxy-Commands!
    # Das ist ERFORDERLICH und funktioniert!
    
    # Prüfe auf Work-DLL Referenzen (sollten NICHT da sein)
    $workDllReferences = [regex]::Matches($appContent, '"GeoJsonImporter\.Work\.Commands\.(\w+)"')
    
    if ($workDllReferences.Count -gt 0) {
        foreach ($match in $workDllReferences) {
            $commandName = $match.Groups[1].Value
            $errors += "VERBOTEN: App.cs zeigt direkt auf Work-DLL Command '$commandName'! Sollte auf Loader-DLL Proxy-Command zeigen!"
        }
    }
    
    # Prüfe auf Loader-DLL Proxy-Command Referenzen (sind ERFORDERLICH!)
    $loaderDllReferences = [regex]::Matches($appContent, '"GeoJsonImporter\.Addin\.Commands\.(\w+)"')
    
    if ($loaderDllReferences.Count -gt 0) {
        foreach ($match in $loaderDllReferences) {
            $commandName = $match.Groups[1].Value
            Write-Host "    ✅ Proxy-Command Referenz gefunden: $commandName (korrekt)" -ForegroundColor Green
        }
    } else {
        $errors += "FEHLT: App.cs zeigt auf keine Loader-DLL Proxy-Commands!"
    }
}

# 🔍 3. WorkDllManager prüfen
Write-Host "  Pruefe WorkDllManager..." -ForegroundColor Yellow

if (Test-Path $workDllManagerPath) {
    $managerContent = Get-Content $workDllManagerPath -Raw
    
    # ❌ VERBOTEN: Neue Delegations-Methoden
    $forbiddenMethods = @("ExecuteSetup", "ExecuteUtmGridSetup", "ExecuteGeoRef")
    
    foreach ($method in $forbiddenMethods) {
        if ($managerContent -match "Execute$method") {
            $errors += "VERBOTEN: Neue Delegations-Methode 'Execute$method' im WorkDllManager gefunden! Nur ExecuteWorkCommand() ist erlaubt!"
        }
    }
    
    # ✅ ERFORDERLICH: ExecuteWorkCommand() Methode
    if ($managerContent -match "ExecuteWorkCommand") {
        Write-Host "    ✅ ExecuteWorkCommand() gefunden (korrekt)" -ForegroundColor Green
    } else {
        $errors += "FEHLT: ExecuteWorkCommand() Methode im WorkDllManager nicht gefunden!"
    }
}

# 🔍 4. Work-DLL Command-Referenzen prüfen
Write-Host "  Pruefe Work-DLL Command-Referenzen..." -ForegroundColor Yellow

if (Test-Path $workCommandsPath) {
    $workCommands = Get-ChildItem $workCommandsPath -Filter "*.cs" | ForEach-Object { $_.BaseName }
    
    # Alle Work-DLL Commands müssen IExternalCommand implementieren
    foreach ($command in $workCommands) {
        $commandFile = Join-Path $workCommandsPath "$command.cs"
        if (Test-Path $commandFile) {
            $commandContent = Get-Content $commandFile -Raw
            
            # Prüfe auf IExternalCommand Interface
            if ($commandContent -match "IExternalCommand") {
                Write-Host "    ✅ $command implementiert IExternalCommand" -ForegroundColor Green
            } else {
                $warnings += "WARNUNG: $command implementiert möglicherweise nicht IExternalCommand"
            }
            
            # ❌ VERBOTEN: Referenzen auf Loader-DLL
            if ($commandContent -match "GeoJsonImporter\.Addin") {
                $errors += "VERBOTEN: $command referenziert Loader-DLL! Work-DLL darf nicht auf Loader-DLL verweisen!"
            }
        }
    }
}

# 📊 Ergebnisse ausgeben
Write-Host ""
Write-Host "ARCHITEKTUR-PRUEFUNG ABGESCHLOSSEN" -ForegroundColor Cyan

if ($errors.Count -eq 0) {
    Write-Host "✅ KEINE ARCHITEKTUR-FEHLER GEFUNDEN!" -ForegroundColor Green
    Write-Host "🚀 Hot Reload System ist intakt und funktioniert!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "🚨 ARCHITEKTUR-FEHLER GEFUNDEN!" -ForegroundColor Red
    Write-Host "❌ Hot Reload System ist zerstört!" -ForegroundColor Red
    
    foreach ($error in $errors) {
        Write-Host "   ❌ $error" -ForegroundColor Red
    }
    
    if ($warnings.Count -gt 0) {
        Write-Host ""
        Write-Host "⚠️  WARNUNGEN:" -ForegroundColor Yellow
        foreach ($warning in $warnings) {
            Write-Host "   ⚠️  $warning" -ForegroundColor Yellow
        }
    }
    
    Write-Host ""
    Write-Host "SOFORT REPARIEREN:" -ForegroundColor Red
    Write-Host "   1. Architektur-Fehler beheben" -ForegroundColor Red
    Write-Host "   2. Beide DLLs neu bauen" -ForegroundColor Red
    Write-Host "   3. Hot Reload System testen" -ForegroundColor Red
    
    exit 1
}
