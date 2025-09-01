

# **Entwicklung eines C\#-Plugins für den GeoJSON-Import in Revit 2026: Ein Leitfaden für moderne und effiziente Workflows**

## **Einleitung**

Dieser Bericht dient als maßgeblicher technischer Leitfaden für die Entwicklung eines C\#-Plugins für Autodesk Revit 2026\. Das primäre Ziel ist der Import von GeoJSON-Daten und deren Umwandlung in native Revit-Geometrie unter Verwendung der DirectShape-Klasse. Ein zentrales Anliegen des modernen Entwicklers ist die Minimierung der Dauer des "inner loop"-Entwicklungszyklus – der Zeitspanne zwischen einer Codeänderung und dem Testen dieser Änderung. Daher wird eine detaillierte Analyse und eine definitive Empfehlung zur Optimierung des Test- und Debugging-Prozesses gegeben, um die Notwendigkeit ständiger Revit-Neustarts zu eliminieren oder drastisch zu reduzieren.

Der Bericht richtet sich an erfahrene C\#-Entwickler und technisch versierte BIM-Spezialisten, die eine robuste, wartbare und performante Revit-Integration erstellen möchten. Die Analyse und die bereitgestellten Codebeispiele basieren auf der technologischen Grundlage von Revit 2026, das auf.NET 8 aufbaut 1, und nutzen Cursor AI, einen Fork von Visual Studio Code, als moderne, KI-gestützte Entwicklungsumgebung. Es wird ein tiefgreifendes Verständnis der Revit-API, der Projektkonfiguration und der Best Practices für einen effizienten Entwicklungs-Workflow vermittelt.

## **1\. Fundament: Projekteinrichtung in Cursor AI für Revit 2026**

Eine solide und korrekt konfigurierte Projektgrundlage ist entscheidend für den Erfolg und die Wartbarkeit eines jeden Softwareprojekts. Dies gilt insbesondere für die Revit-Plugin-Entwicklung, bei der die Abhängigkeiten zur Host-Anwendung präzise verwaltet werden müssen. Die Wahl von Cursor AI (oder einer anderen VS Code-basierten IDE) repräsentiert einen Branchentrend hin zu leichteren, plattformübergreifenden und KI-gestützten Entwicklungsumgebungen. Mit den richtigen Erweiterungen lässt sich eine vollwertige Entwicklungserfahrung realisieren, die mit traditionellen IDEs wie Visual Studio konkurriert.3

### **1.1. Konfiguration der Entwicklungsumgebung**

Bevor mit der eigentlichen Projektentwicklung begonnen werden kann, muss die Entwicklungsumgebung korrekt eingerichtet werden. Die folgenden Komponenten sind unerlässlich:

1. **Cursor AI / Visual Studio Code:** Die Basis-IDE. Cursor AI integriert KI-Funktionen nativ, aber die grundlegenden Schritte sind identisch mit denen in Visual Studio Code.  
2. **.NET 8 SDK:** Da Revit 2026 auf.NET 8 basiert, ist das entsprechende Software Development Kit (SDK) zwingend erforderlich. Es enthält den Compiler, die Laufzeitbibliotheken und die Kommandozeilenwerkzeuge (dotnet CLI).2  
3. **C\# Dev Kit Extension:** Diese von Microsoft veröffentlichte Erweiterung für VS Code ist entscheidend. Sie transformiert den texteditor-zentrierten Ansatz von VS Code in eine vollwertige C\#-IDE, indem sie einen Projektmappen-Explorer, integrierte Test- und Debugging-Funktionen sowie erweiterte IntelliSense-Funktionen bereitstellt.3

Die Installation erfolgt über den Extension Marketplace innerhalb von Cursor AI/VS Code. Nach der Installation des C\# Dev Kit wird dieses typischerweise die Installation des.NET SDK anstoßen, falls es noch nicht vorhanden ist.4

### **1.2. Erstellung des C\#-Klassenbibliotheksprojekts**

Ein Revit-Plugin ist im Kern eine.NET-Klassenbibliothek (.dll), die von Revit zur Laufzeit geladen wird. Die Projekterstellung in Cursor AI erfolgt über die Befehlspalette:

1. Öffnen Sie die Befehlspalette mit Ctrl+Shift+P.  
2. Geben Sie .NET: New Project ein und wählen Sie den entsprechenden Befehl aus.4  
3. Wählen Sie aus der Liste der Vorlagen "Class Library" aus.  
4. Geben Sie einen Projektnamen (z.B. GeoJsonImporter) und einen Speicherort an.

Dadurch werden die grundlegenden Projektdateien erstellt: eine Projektmappendatei (.sln), eine Projektdatei (.csproj) und eine initiale C\#-Klassendatei (Class1.cs).

### **1.3. Anatomie der .csproj-Datei und API-Referenzierung**

Die .csproj-Datei ist das Herzstück des Projekts und definiert, wie es kompiliert wird. Für die Revit 2026-Entwicklung sind spezifische Anpassungen erforderlich. Die manuelle Bearbeitung dieser Datei vermittelt ein tiefes Verständnis der Projektabhängigkeiten, was für die Fehlersuche und fortgeschrittene Konfigurationen von unschätzbarem Wert ist.

Ein korrekt konfiguriertes .csproj-File für ein Revit 2026 Plugin sieht wie folgt aus:

XML

\<Project Sdk\="Microsoft.NET.Sdk"\>

  \<PropertyGroup\>  
    \<TargetFramework\>net8.0-windows\</TargetFramework\>  
    \<ImplicitUsings\>enable\</ImplicitUsings\>  
    \<Nullable\>enable\</Nullable\>  
    \<PlatformTarget\>x64\</PlatformTarget\>  
    \<LangVersion\>latest\</LangVersion\>  
  \</PropertyGroup\>

  \<ItemGroup\>  
    \<Reference Include\="RevitAPI"\>  
      \<HintPath\>C:\\Program Files\\Autodesk\\Revit 2026\\RevitAPI.dll\</HintPath\>  
      \<Private\>False\</Private\>  
    \</Reference\>  
    \<Reference Include\="RevitAPIUI"\>  
      \<HintPath\>C:\\Program Files\\Autodesk\\Revit 2026\\RevitAPIUI.dll\</HintPath\>  
      \<Private\>False\</Private\>  
    \</Reference\>  
  \</ItemGroup\>

\</Project\>

**Schlüsselelemente und ihre Bedeutung:**

* **\<TargetFramework\>net8.0-windows\</TargetFramework\>:** Definiert das Ziel-Framework. net8.0 ist für Revit 2026 erforderlich.1 Der Zusatz  
  \-windows ist notwendig, um auf Windows-spezifische APIs zuzugreifen, die von der Revit-API und insbesondere von UI-Komponenten benötigt werden.  
* **\<PlatformTarget\>x64\</PlatformTarget\>:** Revit ist eine 64-Bit-Anwendung. Daher muss das Plugin zwingend für die x64-Plattform kompiliert werden, um Ladefehler zu vermeiden.5  
* **\<Reference Include="..."\>:** Dieser Abschnitt dient dem Hinzufügen von Referenzen auf externe Assemblies. Für die Revit-API sind RevitAPI.dll (Datenbank- und Kernfunktionalität) und RevitAPIUI.dll (Benutzeroberflächen-spezifische Funktionalität) unerlässlich.6  
* **\<HintPath\>...\</HintPath\>:** Da die Revit-API-DLLs nicht über NuGet-Pakete verteilt werden, muss der Pfad zu ihrer lokalen Installation explizit angegeben werden. Dieser Pfad muss an die jeweilige Revit-Installation angepasst werden.8  
* **\<Private\>False\</Private\>:** Dies ist eine kritische Einstellung, die dem Copy Local-Attribut in Visual Studio entspricht. Sie verhindert, dass die Revit-API-DLLs in das Ausgabe-Verzeichnis des Plugins (bin/Debug) kopiert werden. Würde man dies nicht tun, könnte es zu Versionskonflikten kommen, wenn Revit versucht, seine eigenen, bereits geladenen DLLs erneut aus dem Plugin-Verzeichnis zu laden.6

### **1.4. Empfohlene Projekt- und Ordnerstruktur**

Eine gut durchdachte Ordnerstruktur ist entscheidend für die Wartbarkeit und Skalierbarkeit eines Plugins. Sie fördert das Prinzip der "Separation of Concerns", bei dem Code mit unterschiedlichen Verantwortlichkeiten in getrennten Modulen organisiert wird.10

Eine bewährte Struktur für das GeoJSON-Importer-Projekt könnte wie folgt aussehen:

GeoJsonImporter/  
├── GeoJsonImporter.sln  
├── src/  
│   └── GeoJsonImporter/  
│       ├── GeoJsonImporter.csproj  
│       ├── Addin/  
│       │   ├── App.cs                      // IExternalApplication: UI-Setup, Ribbon-Erstellung  
│       │   └── Commands/  
│       │       └── ImportGeoJsonCommand.cs // IExternalCommand: Hauptlogik des Imports  
│       ├── Core/  
│       │   ├── GeoJsonParser.cs            // Logik zum Parsen der GeoJSON-Datei  
│       │   └── RevitGeometryCreator.cs     // Logik zur Erzeugung von Revit-Geometrie  
│       └── Resources/  
│           └── icon.png                    // Icon für den Ribbon-Button  
├── assets/  
│   └── GeoJsonImporter.addin               // Revit-Manifestdatei

**Verantwortlichkeiten der Komponenten:**

* **src/:** Enthält den gesamten C\#-Quellcode.  
* **assets/:** Beinhaltet nicht kompilierte, aber für das Deployment notwendige Dateien wie die .addin-Manifestdatei.  
* **Addin/:** Enthält die Klassen, die direkt mit der Revit-API-Schnittstelle interagieren (IExternalApplication, IExternalCommand).  
* **Core/:** Beinhaltet die Kernlogik des Plugins, die idealerweise unabhängig von der Revit-API ist (z.B. das Parsen von Daten). Dies erleichtert das Testen und die Wiederverwendbarkeit.

## **2\. Der Entwicklungszyklus: Optimierung des Reload-Workflows**

Die vom Benutzer gestellte Frage nach einer optimierten Möglichkeit, Änderungen zu testen, zielt auf einen der größten Effizienzkiller in der traditionellen Revit-Plugin-Entwicklung ab. Die technologische Entwicklung bietet hier eine klare Evolution von Workarounds hin zu einer nativen, robusten Lösung.

### **2.1. Die klassische Herausforderung**

Das grundlegende Problem liegt in der Art und Weise, wie das klassische.NET Framework Assemblies verwaltet. Wenn eine Assembly (eine .dll-Datei) in die standardmäßige AppDomain einer Anwendung geladen wird, sperrt die Laufzeitumgebung diese Datei auf dem Dateisystem. Ein Entladen einer einzelnen Assembly aus dieser AppDomain ist nicht vorgesehen. Um eine neue Version der DLL zu laden, muss die gesamte AppDomain – und damit die Host-Anwendung Revit – beendet und neu gestartet werden.11

### **2.2. Methode A: Der Add-In Manager (SDK-Tool) – Ein verlässlicher, aber manueller Ansatz**

Der Add-In Manager ist ein Werkzeug, das mit dem Revit SDK ausgeliefert wird und seit vielen Jahren als primärer Workaround für das Reload-Problem dient.

* **Installation:** Die AddInManager.dll und die zugehörige AddInManager.addin-Datei müssen aus dem SDK-Installationsverzeichnis in den Revit-Addins-Ordner (C:\\ProgramData\\Autodesk\\Revit\\Addins\\\<Jahr\>\\) kopiert werden. Gegebenenfalls muss der Pfad in der .addin-Datei angepasst werden.13  
* **Verwendung:** Anstatt das eigene Plugin direkt über den Addins-Ordner zu laden, startet man Revit und nutzt den Add-In Manager, um die Plugin-DLL manuell aus dem bin/Debug-Verzeichnis zu laden. Nach einer Codeänderung und Neukompilierung kann die DLL im Add-In Manager entladen und die neue Version geladen werden, ohne Revit neu zu starten.13  
* **Bewertung:** Diese Methode ist zuverlässig für einfache, isolierte IExternalCommand-Befehle. Sie hat jedoch Nachteile: Der Prozess ist manuell und erfordert mehrere Klicks. Zudem kann er bei komplexeren Plugins, insbesondere solchen mit WPF-Benutzeroberflächen oder IExternalApplication-Implementierungen, zu Instabilität und Abstürzen führen.16

### **2.3. Methode B: Visual Studio "Hot Reload" – Stärken und Limitierungen**

Moderne.NET-Versionen und IDEs bieten eine "Hot Reload"-Funktion, die es ermöglicht, Codeänderungen zur Laufzeit direkt in einen laufenden Prozess einzuspielen, ohne diesen neu zu starten.

* **Konzept:** Während einer Debugging-Sitzung (via "Attach to Process" an Revit) können Änderungen im Code vorgenommen werden. Ein Klick auf den "Hot Reload"-Button (oder das Speichern der Datei) versucht, diese Änderungen in die laufende Anwendung zu injizieren.12  
* **Anwendung und Limitierungen:** Hot Reload funktioniert gut für Änderungen innerhalb von Methodenrümpfen (z.B. das Ändern einer Berechnung oder eines Logikflusses). Es scheitert jedoch oft bei "unhöflichen Bearbeitungen" wie dem Ändern von Methodensignaturen, dem Hinzufügen neuer Methoden oder Klassen oder größeren Refactorings. In diesen Fällen ist ein Neustart der Debugging-Sitzung unumgänglich, was oft auch einen Neustart von Revit erfordert.12 Es ist ein nützliches Werkzeug für schnelle, iterative Anpassungen, aber keine umfassende Lösung für das grundlegende Problem.

### **2.4. Die moderne Lösung für Revit 2026: Add-In-Abhängigkeitsisolierung**

Mit Revit 2026 führt Autodesk eine native Lösung ein, die das Problem an der Wurzel packt: die "Option for Add-in Dependency Isolation".18 Dies ist keine oberflächliche Verbesserung, sondern eine fundamentale Änderung der Lade-Architektur von Add-Ins.

* **Technischer Hintergrund:** Diese Funktion basiert auf dem modernen.NET-Konzept des AssemblyLoadContext. Im Gegensatz zur monolithischen AppDomain ist ein AssemblyLoadContext eine leichtgewichtige, isolierte Umgebung, die vollständig entladen werden kann. Jedes Add-In, das diese Funktion nutzt, wird in seinen eigenen AssemblyLoadContext geladen.11 Dies ermöglicht es Revit, ein Add-In und alle seine Abhängigkeiten sauber zu entladen und bei Bedarf eine neue Version zu laden.  
* **Vorteile:**  
  1. **Echtes Hot Reload:** Ermöglicht ein robustes Neuladen des gesamten Plugins, einschließlich IExternalApplication-Logik, ohne Revit-Neustart.  
  2. **DLL-Konfliktlösung:** Löst das berüchtigte "DLL Hell"-Problem. Wenn zwei verschiedene Add-Ins unterschiedliche Versionen derselben Bibliothek (z.B. Newtonsoft.Json) benötigen, kann jeder AssemblyLoadContext seine eigene Version laden, ohne die anderen zu stören. Dies erhöht die Stabilität des gesamten Revit-Ökosystems erheblich.11

### **2.5. Implementierung der Isolation über die .addin-Manifestdatei**

Die Aktivierung dieser neuen Funktion ist unkompliziert und erfolgt über einen neuen XML-Tag in der .addin-Manifestdatei.

XML

\<?xml version="1.0" encoding="utf-8"?\>  
\<RevitAddIns\>  
  \<AddIn Type\="Application"\>  
    \<AssemblyLoadContext\>Separate\</AssemblyLoadContext\>  
  \</AddIn\>  
\</RevitAddIns\>

Wichtig ist der Hinweis, dass das Standardverhalten für Add-Ins in Revit 2026, bei denen dieser Tag nicht spezifiziert ist, true (also isoliert) ist.18 Entwickler müssen sicherstellen, dass ihre Add-Ins alle benötigten Abhängigkeiten selbst mitliefern, da sie sich nicht mehr darauf verlassen können, dass Revit oder ein anderes Add-In eine benötigte DLL bereits geladen hat.

**Vergleich der Reload-Methoden für die Revit-Plugin-Entwicklung**

| Methode | Funktionsprinzip | Revit-Kompatibilität | Vorteile | Nachteile | Empfehlung für 2026 |
| :---- | :---- | :---- | :---- | :---- | :---- |
| **Manueller Neustart** | Revit beenden, neu kompilieren, Revit starten | Alle Versionen | Einfachster Prozess, immer zuverlässig | Extrem langsam, unterbricht den Workflow | Nur als letzter Ausweg. |
| **Add-In Manager** | Manuelles Entladen/Laden der DLL in Revit | Alle Versionen mit SDK | Kein Revit-Neustart, gut für isolierte Befehle | Manuelle Schritte erforderlich, kann bei UI instabil sein, lädt keine IExternalApplication neu | Gute Fallback-Lösung oder für ältere Revit-Versionen. |
| **Visual Studio Hot Reload** | Laufzeit-Patching des Codes durch den Debugger | VS2019+ mit.NET Core/5+ | Sehr schnell für kleine Änderungen, nahtlose IDE-Integration | Limitiert auf methodeninterne Änderungen, unzuverlässig bei API-Änderungen | Nützlich für schnelle, iterative Logik-Anpassungen, aber keine Komplettlösung. |
| **Revit 2026 Dependency Isolation** | Lädt Add-In in einen separaten, entladbaren AssemblyLoadContext | Revit 2026+ | Nativ, robust, löst auch DLL-Konflikte, unterstützt IExternalApplication | Erfordert korrekte Konfiguration, Abhängigkeiten müssen vollständig sein | **Die empfohlene Standardmethode.** |

## **3\. Plugin-Architektur: UI-Integration und Befehlsstruktur**

Eine saubere Trennung zwischen der Initialisierung der Benutzeroberfläche und der Ausführung der eigentlichen Geschäftslogik ist ein entscheidendes Architekturprinzip. Es verbessert die Lesbarkeit, Wartbarkeit und Testbarkeit des Codes erheblich. Die Revit-API unterstützt dieses Prinzip durch die Schnittstellen IExternalApplication und IExternalCommand.

### **3.1. Der IExternalApplication-Einstiegspunkt**

Die IExternalApplication-Schnittstelle ist der Haupteinstiegspunkt für Plugins, die beim Start von Revit initialisiert werden müssen, typischerweise um UI-Elemente wie Ribbon-Tabs und Buttons zu erstellen.10

* **OnStartup(UIControlledApplication application):** Diese Methode wird einmalig beim Start von Revit aufgerufen. Hier wird die Benutzeroberfläche des Plugins aufgebaut.  
* **OnShutdown(UIControlledApplication application):** Diese Methode wird beim Beenden von Revit aufgerufen und kann für Aufräumarbeiten genutzt werden.

Das folgende Codebeispiel zeigt die Implementierung in der App.cs-Klasse, die einen neuen Tab "GeoTools" und ein Panel "Import" erstellt.

C\#

using Autodesk.Revit.UI;  
using System.Reflection;

namespace GeoJsonImporter.Addin  
{  
    public class App : IExternalApplication  
    {  
        public Result OnStartup(UIControlledApplication application)  
        {  
            string tabName \= "GeoTools";  
            application.CreateRibbonTab(tabName);

            RibbonPanel panel \= application.CreateRibbonPanel(tabName, "Import");

            string assemblyPath \= Assembly.GetExecutingAssembly().Location;

            var importButtonData \= new PushButtonData(  
                "ImportGeoJson",  
                "Import\\nGeoJSON",  
                assemblyPath,  
                "GeoJsonImporter.Addin.Commands.ImportGeoJsonCommand");

            // Weitere Konfiguration des Buttons (Icon, Tooltip) folgt hier...  
              
            panel.AddItem(importButtonData);

            return Result.Succeeded;  
        }

        public Result OnShutdown(UIControlledApplication application)  
        {  
            return Result.Succeeded;  
        }  
    }  
}

### **3.2. Implementierung eines PushButton**

Ein PushButton ist der Standard-Button im Revit-Ribbon. Er wird über ein PushButtonData-Objekt konfiguriert, das alle notwendigen Informationen enthält.10

* **name:** Ein interner, eindeutiger Name für den Button.  
* **text:** Der für den Benutzer sichtbare Text. Ein \\n erzeugt einen Zeilenumbruch.  
* **assemblyName:** Der vollständige Pfad zur DLL, die den auszuführenden Befehl enthält. Assembly.GetExecutingAssembly().Location ist hier die Standardmethode.  
* **className:** Der vollqualifizierte Name der Klasse (inklusive Namespace), die IExternalCommand implementiert.

Zusätzlich können Icons und Tooltips zugewiesen werden, um die Benutzerfreundlichkeit zu verbessern.19

### **3.3. Die IExternalCommand-Implementierung**

Die IExternalCommand-Schnittstelle definiert den Code, der ausgeführt wird, wenn der Benutzer auf den zugehörigen PushButton klickt.7

* **Execute(ExternalCommandData commandData, ref string message, ElementSet elements):** Dies ist die zentrale Methode. Sie bietet Zugriff auf die Revit-Anwendung (commandData.Application), das aktive Dokument und die Benutzeroberfläche.22  
* **\`\`:** Dieses Attribut ist für Befehle, die das Revit-Modell verändern, unerlässlich. Es deklariert, dass Transaktionen manuell vom Entwickler gesteuert werden. Jede Änderung an der Revit-Datenbank muss innerhalb eines Transaction-Blocks erfolgen, der explizit gestartet (t.Start()) und abgeschlossen (t.Commit()) wird.10

Ein Grundgerüst für die ImportGeoJsonCommand.cs sieht so aus:

C\#

using Autodesk.Revit.Attributes;  
using Autodesk.Revit.DB;  
using Autodesk.Revit.UI;

namespace GeoJsonImporter.Addin.Commands  
{  
     
    public class ImportGeoJsonCommand : IExternalCommand  
    {  
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)  
        {  
            UIDocument uidoc \= commandData.Application.ActiveUIDocument;  
            Document doc \= uidoc.Document;

            // Hier wird die Kernlogik aufgerufen:  
            // 1\. Datei-Dialog anzeigen, um GeoJSON-Datei auszuwählen.  
            // 2\. GeoJsonParser aufrufen, um die Datei zu parsen.  
            // 3\. RevitGeometryCreator aufrufen, um die Geometrie zu erzeugen.  
              
            using (Transaction t \= new Transaction(doc, "Import GeoJSON Geometry"))  
            {  
                t.Start();  
                // Code zur Geometrieerzeugung hier...  
                t.Commit();  
            }

            return Result.Succeeded;  
        }  
    }  
}

### **3.4. Die .addin-Manifestdatei im Detail**

Die .addin-Datei ist eine XML-Datei, die Revit beim Start mitteilt, welche Plugins geladen werden sollen. Sie ist der Klebstoff, der das Plugin mit der Revit-Anwendung verbindet.24

* **Type="Application" vs. Type="Command":** Application wird für Plugins verwendet, die IExternalApplication implementieren und beim Start geladen werden müssen. Command ist für einfache Befehle, die Revit direkt in das "External Tools"-Menü lädt.25 Für dieses Projekt ist  
  Type="Application" korrekt.  
* **Obligatorische Tags:**  
  * **\<Assembly\>:** Der Pfad zur Plugin-DLL.  
  * **\<FullClassName\>:** Der vollqualifizierte Name der Klasse, die IExternalApplication oder IExternalCommand implementiert.  
  * **\<AddInId\>:** Eine eindeutige GUID, die das Plugin identifiziert. Diese muss für jedes Add-In einzigartig sein. GUIDs können in Visual Studio/Cursor AI über entsprechende Werkzeuge generiert werden.7  
* **Neuer Tag für Revit 2026:**  
  * **\<AssemblyLoadContext\>:** Wie in Abschnitt 2.5 beschrieben, wird hier Separate eingetragen, um die moderne Abhängigkeitsisolierung zu aktivieren.

Eine vollständige .addin-Datei für das Projekt würde so aussehen:

XML

\<?xml version="1.0" encoding="utf-8" standalone="no"?\>  
\<RevitAddIns\>  
  \<AddIn Type\="Application"\>  
    \<Name\>GeoJSON Importer\</Name\>  
    \<Assembly\>GeoJsonImporter.dll\</Assembly\>  
    \<AddInId\>PASTE-A-NEW-GUID-HERE\</AddInId\>  
    \<FullClassName\>GeoJsonImporter.Addin.App\</FullClassName\>  
    \<VendorId\>YourCompany\</VendorId\>  
    \<VendorDescription\>Your Company Description\</VendorDescription\>  
    \<AssemblyLoadContext\>Separate\</AssemblyLoadContext\>  
  \</AddIn\>  
\</RevitAddIns\>

## **4\. Kernfunktionalität: Parsen von GeoJSON-Daten in C\#**

Nachdem die Plugin-Struktur steht, folgt die Implementierung der Kernfunktionalität: das Einlesen und Verstehen der GeoJSON-Daten.

### **4.1. Auswahl der richtigen Bibliothek**

Für die Verarbeitung von GeoJSON in.NET hat sich die Bibliothek **GeoJSON.Net** als De-facto-Standard etabliert.26

* **Funktionalität:** Sie bietet stark typisierte C\#-Klassen, die die gesamte GeoJSON-Spezifikation (RFC 7946\) abbilden, einschließlich Point, Polygon, Feature und FeatureCollection.  
* **Abhängigkeit:** GeoJSON.Net hat eine feste Abhängigkeit von der populären JSON-Bibliothek Newtonsoft.Json (auch bekannt als Json.NET).26 Obwohl moderne.NET-Projekte oft das eingebaute  
  System.Text.Json bevorzugen 28, ist die Wahl von  
  Newtonsoft.Json im Revit-Kontext pragmatisch und sicher. Revit und viele etablierte Add-Ins verwenden es seit Jahren, was die Kompatibilität erhöht. Die in Revit 2026 eingeführte Abhängigkeitsisolierung mindert zudem das Risiko von Versionskonflikten, sollte Revit intern eine andere Newtonsoft.Json-Version verwenden.  
* **Installation:** Die Bibliothek wird als NuGet-Paket zum Projekt hinzugefügt. In Cursor AI kann dies über die Befehlspalette (.NET: Add Package) oder durch direktes Bearbeiten der .csproj-Datei geschehen.

### **4.2. Implementierung des Parsers**

Die Deserialisierung einer GeoJSON-Datei in C\#-Objekte ist mit GeoJSON.Net und Newtonsoft.Json sehr einfach. Der Prozess umfasst das Einlesen des Datei-Inhalts als String und die anschließende Konvertierung.

Das folgende Codebeispiel zeigt eine Methode in der hypothetischen GeoJsonParser.cs-Klasse:

C\#

using GeoJSON.Net.Feature;  
using Newtonsoft.Json;  
using System.IO;

namespace GeoJsonImporter.Core  
{  
    public static class GeoJsonParser  
    {  
        public static FeatureCollection? ParseFile(string filePath)  
        {  
            if (\!File.Exists(filePath))  
            {  
                // Fehlerbehandlung: Datei nicht gefunden  
                return null;  
            }

            string jsonContent \= File.ReadAllText(filePath);

            try  
            {  
                var featureCollection \= JsonConvert.DeserializeObject\<FeatureCollection\>(jsonContent);  
                return featureCollection;  
            }  
            catch (JsonException ex)  
            {  
                // Fehlerbehandlung: Ungültiges JSON-Format  
                // Log the exception...  
                return null;  
            }  
        }  
    }  
}

Die Methode JsonConvert.DeserializeObject\<FeatureCollection\>(jsonContent) ist der Kern des Vorgangs. Sie nimmt den JSON-String entgegen und verwendet die von GeoJSON.Net bereitgestellten Konverter, um ihn in ein FeatureCollection-Objekt zu überführen.26

### **4.3. Navigation durch die Datenstruktur**

Nach der erfolgreichen Deserialisierung liegt die GeoJSON-Struktur als C\#-Objekt vor, durch das einfach navigiert werden kann.

C\#

// In der Execute-Methode des IExternalCommand...  
FeatureCollection? featureCollection \= GeoJsonParser.ParseFile(selectedFilePath);

if (featureCollection \== null)  
{  
    // Fehlermeldung an den Benutzer  
    return Result.Failed;  
}

foreach (var feature in featureCollection.Features)  
{  
    // 1\. Zugriff auf Metadaten (Properties)  
    if (feature.Properties.TryGetValue("name", out object? nameValue))  
    {  
        string name \= nameValue?.ToString()?? "Unnamed";  
        //...  
    }

    // 2\. Zugriff auf Geometrie und Typ-Prüfung  
    if (feature.Geometry is GeoJSON.Net.Geometry.Polygon polygon)  
    {  
        // Polygon-Geometrie verarbeiten  
        var exteriorRing \= polygon.Coordinates; // Äußerer Ring  
        var coordinates \= exteriorRing.Coordinates; // Liste von IPosition-Objekten

        //...  
    }  
    else if (feature.Geometry is GeoJSON.Net.Geometry.Point point)  
    {  
        // Punkt-Geometrie verarbeiten  
        var position \= point.Coordinates;  
        //...  
    }  
    // Weitere Geometrietypen (LineString, etc.) können hier behandelt werden.  
}

Dieser Code-Abschnitt demonstriert, wie man durch die Liste der Features iteriert, auf die Properties (ein Dictionary\<string, object\>) zugreift und mittels Pattern Matching den Typ der Geometry prüft, um an die eigentlichen Koordinaten zu gelangen.

## **5\. Geometrieerzeugung in Revit: Von GeoJSON zu nativen Elementen**

Der letzte Schritt der Kernfunktionalität ist die Umwandlung der geparsten GeoJSON-Koordinaten in physische Geometrie innerhalb des Revit-Modells.

### **5.1. Einführung in DirectShape**

Für den Import von "dummer" Geometrie, die keine parametrischen Eigenschaften oder Revit-spezifisches Verhalten (wie Wände, die andere Elemente hosten) benötigt, ist die DirectShape-Klasse die ideale Wahl. Sie fungiert als leichtgewichtiger Container für explizite geometrische Formen, die aus externen Formaten wie IFC, STEP oder eben GeoJSON stammen.29

### **5.2. Von Koordinaten zu Revit-Geometrie mit TessellatedShapeBuilder**

Der kritischste Schritt ist die Umwandlung der GeoJSON-Koordinaten in eine für Revit verständliche 3D-Form. Der TessellatedShapeBuilder ist ein leistungsfähiges Werkzeug der Revit-API, um eine 3D-Form aus einer Sammlung von planaren, polygonalen Flächen zu konstruieren.31

**Wichtiger Hinweis zur Koordinatentransformation:** Ein häufiger Fehler ist die Annahme, dass GeoJSON-Koordinaten direkt als Revit-XYZ-Koordinaten verwendet werden können. GeoJSON verwendet typischerweise ein geodätisches Koordinatensystem wie WGS 84 (Längen- und Breitengrad), während Revit in einem kartesischen Projektkoordinatensystem arbeitet.33 Eine direkte Übernahme der Werte würde zu massiven Skalierungs- und Positionierungsfehlern führen. Für eine praxistaugliche Anwendung ist eine Koordinatentransformation unerlässlich. Dies kann eine komplexe Projektion (z.B. in ein UTM-System) oder – für eine einfachere Implementierung – die Definition eines lokalen Ursprungs und die Umrechnung aller Koordinaten relativ zu diesem Punkt sein. Das folgende Beispiel geht vereinfachend davon aus, dass die Koordinaten bereits in einem kompatiblen kartesischen System vorliegen.

Die Implementierung in der RevitGeometryCreator.cs könnte die folgende Methode enthalten:

C\#

using Autodesk.Revit.DB;  
using GeoJSON.Net.Geometry;  
using System.Collections.Generic;  
using System.Linq;

namespace GeoJsonImporter.Core  
{  
    public static class RevitGeometryCreator  
    {  
        public static IList\<GeometryObject\> CreateSolidFromPolygon(Polygon polygon)  
        {  
            // Annahme: Koordinaten sind bereits in Revit-Einheiten und \-System.  
            // Äußeren Ring des Polygons extrahieren.  
            var exteriorRing \= polygon.Coordinates.Coordinates;  
              
            List\<XYZ\> profilePoints \= exteriorRing  
               .Select(pos \=\> new XYZ(pos.Longitude, pos.Latitude, pos.Altitude?? 0))  
               .ToList();

            // Revit-API erfordert geschlossene Schleifen.  
            if (\!profilePoints.IsAlmostEqualTo(profilePoints\[profilePoints.Count \- 1\]))  
            {  
                profilePoints.Add(profilePoints);  
            }

            var builder \= new TessellatedShapeBuilder();  
            builder.OpenConnectedFaceSet(true);

            // TessellatedFace benötigt eine Liste von Listen von Punkten (für Löcher).  
            var faceLoops \= new List\<IList\<XYZ\>\> { profilePoints };  
            var face \= new TessellatedFace(faceLoops, ElementId.InvalidElementId);

            if (builder.DoesFaceHaveEnoughLoopsAndVertices(face))  
            {  
                builder.AddFace(face);  
            }  
              
            builder.CloseConnectedFaceSet();  
              
            builder.Build();  
              
            TessellatedShapeBuilderResult result \= builder.GetBuildResult();  
            return result.GetGeometricalObjects();  
        }  
    }  
}

Dieser Code extrahiert die Koordinaten, erstellt eine TessellatedFace und verwendet den TessellatedShapeBuilder, um daraus eine Liste von GeometryObject zu erzeugen, die die 3D-Form repräsentieren.31

### **5.3. Erstellung des DirectShape-Elements**

Die erzeugten Geometrieobjekte müssen nun in einem DirectShape-Element im Revit-Modell platziert werden. Dieser Vorgang muss innerhalb einer Transaktion stattfinden.

C\#

// In der Execute-Methode des IExternalCommand, innerhalb des Transaction-Blocks...

//... Code zum Parsen und Aufrufen von CreateSolidFromPolygon...  
IList\<GeometryObject\> geometryObjects \= RevitGeometryCreator.CreateSolidFromPolygon(polygon);

if (geometryObjects\!= null && geometryObjects.Count \> 0)  
{  
    // Eine Kategorie für das DirectShape festlegen.  
    ElementId categoryId \= new ElementId(BuiltInCategory.OST\_GenericModel);  
      
    DirectShape ds \= DirectShape.CreateElement(doc, categoryId);  
    ds.Name \= "Imported GeoJSON Shape";  
      
    // Die erzeugte Geometrie dem DirectShape zuweisen.  
    ds.SetShape(geometryObjects);  
}

Hier wird mit DirectShape.CreateElement(doc, categoryId) das Element erstellt und anschließend mit ds.SetShape(geometryObjects) die Geometrie zugewiesen.30

### **5.4. Übertragung von Metadaten**

Die im GeoJSON properties-Objekt enthaltenen Metadaten können als Parameter an das DirectShape-Element geschrieben werden. Dies erfordert das Erstellen oder Abrufen von Parametern am Element und das Zuweisen der Werte.

## **6\. Automatisierung des Deployments: Nahtlose Bereitstellung mit Post-Build-Events**

Ein effizienter Entwicklungs-Workflow endet nicht bei der Kompilierung. Die Kombination aus der optimierten Reload-Methode (Dependency Isolation) und einem automatisierten Deployment-Prozess schafft einen nahtlosen Kreislauf: Code ändern, kompilieren, und das Plugin ist sofort in Revit zum Testen bereit. Dies wird durch Post-Build-Events in der .csproj-Datei erreicht.

### **6.1. Konfiguration der .csproj-Datei**

MSBuild, das Build-System von.NET, ermöglicht die Definition von Aktionen, die nach einem erfolgreichen Build ausgeführt werden. Dies geschieht durch Hinzufügen eines \<Target\>-Elements in der .csproj-Datei.36

### **6.2. Vollständiges Post-Build-Skript**

Das folgende Skript, eingefügt in die .csproj-Datei, kopiert automatisch die notwendigen Dateien (.dll, .pdb für Debugging und die .addin-Datei) in den Revit-Addins-Ordner für Revit 2026\.

XML

\<Project Sdk\="Microsoft.NET.Sdk"\>  
  \<Target Name\="CopyAddinFiles" AfterTargets\="Build"\>  
    \<PropertyGroup\>  
      \<RevitAddinsFolder\>$(AppData)\\Autodesk\\Revit\\Addins\\2026\</RevitAddinsFolder\>  
    \</PropertyGroup\>

    \<ItemGroup\>  
      \<AddinFiles Include\="$(ProjectDir)assets\\\*.addin" /\>  
      \<AssemblyFiles Include\="$(TargetDir)\\$(TargetName).dll" /\>  
      \<SymbolFiles Include\="$(TargetDir)\\$(TargetName).pdb" /\>  
    \</ItemGroup\>

    \<Message Text\="Kopiere Add-In-Dateien nach $(RevitAddinsFolder)" Importance\="high" /\>  
      
    \<Copy SourceFiles\="@(AddinFiles)" DestinationFolder\="$(RevitAddinsFolder)" SkipUnchangedFiles\="true" /\>  
    \<Copy SourceFiles\="@(AssemblyFiles)" DestinationFolder\="$(RevitAddinsFolder)" SkipUnchangedFiles\="true" /\>  
    \<Copy SourceFiles\="@(SymbolFiles)" DestinationFolder\="$(RevitAddinsFolder)" SkipUnchangedFiles\="true" Condition\="Exists('%(FullPath)')" /\>  
  \</Target\>

\</Project\>

**Erläuterung des Skripts:**

* **\<Target Name="CopyAddinFiles" AfterTargets="Build"\>:** Definiert ein neues Ziel namens CopyAddinFiles, das nach dem standardmäßigen Build-Ziel ausgeführt wird.  
* **\<PropertyGroup\>:** Definiert eine Variable RevitAddinsFolder, die den Zielpfad unter Verwendung der vordefinierten $(AppData)-Eigenschaft dynamisch zusammensetzt.38  
* **\<ItemGroup\>:** Definiert Sammlungen von Dateien, die kopiert werden sollen: die .addin-Datei aus dem assets-Ordner, die kompilierte .dll und die .pdb-Symboldatei aus dem Ausgabeordner (bin/Debug/net8.0-windows/).  
* **\<Copy...\>:** Die Copy-Aufgabe führt den eigentlichen Kopiervorgang durch. SkipUnchangedFiles="true" sorgt für eine bessere Performance, da nur geänderte Dateien kopiert werden.36

## **Zusammenfassung und Empfehlungen**

Dieser Bericht hat einen umfassenden Weg für die Entwicklung eines C\#-Plugins für Revit 2026 aufgezeigt, von der initialen Projekteinrichtung in einer modernen IDE wie Cursor AI bis hin zur Implementierung der Kernfunktionalität und der Automatisierung des Deployments.

**Zusammenfassung der Kernpunkte:**

1. **Projekteinrichtung:** Die Basis ist ein.NET 8 Klassenbibliotheksprojekt, das explizit für die x64-Plattform konfiguriert und korrekt auf die RevitAPI.dll und RevitAPIUI.dll referenziert ist.  
2. **Entwicklungs-Workflow:** Die traditionellen Hürden des ständigen Neustarts von Revit können und sollten mit den modernen Werkzeugen überwunden werden.  
3. **Plugin-Architektur:** Eine saubere Trennung von UI-Initialisierung (IExternalApplication) und Befehlsausführung (IExternalCommand) ist für die Wartbarkeit entscheidend.  
4. **Datenverarbeitung:** Die GeoJSON.Net-Bibliothek bietet eine robuste und einfache Möglichkeit, GeoJSON-Daten zu deserialisieren und in stark typisierte C\#-Objekte umzuwandeln.  
5. **Geometrieerzeugung:** DirectShape in Kombination mit dem TessellatedShapeBuilder ist der empfohlene Weg, um importierte Polygon-Geometrien in Revit darzustellen. Eine korrekte Koordinatentransformation ist dabei unerlässlich.  
6. **Deployment:** Ein automatisierter Post-Build-Prozess ist der Schlüssel zur Maximierung der Entwicklungseffizienz.

**Definitive Empfehlung für Revit 2026:**

Für die Entwicklung von Plugins für Revit 2026 ist die native **Add-In Dependency Isolation** die mit Abstand überlegene Methode zur Optimierung des Entwicklungszyklus. Sie löst nicht nur das Problem des Neuladens von Assemblies auf eine robuste und von Autodesk unterstützte Weise, sondern verhindert auch proaktiv komplexe DLL-Konflikte zwischen verschiedenen Add-Ins. Diese Funktion sollte als Standard für alle neuen Entwicklungsprojekte für Revit 2026 und darüber hinaus angesehen werden. Ältere Methoden wie der Add-In Manager bleiben als Fallback für die Arbeit mit älteren Revit-Versionen relevant, sind aber für die moderne Entwicklung nicht mehr die erste Wahl.

**Ausblick:**

Auf der hier geschaffenen Grundlage können weiterführende Themen aufgebaut werden. Dazu gehören die Erstellung einer anspruchsvolleren Benutzeroberfläche mit WPF (Windows Presentation Foundation), die Implementierung von asynchronen Programmiermustern zur Verarbeitung großer GeoJSON-Dateien ohne Einfrieren der Revit-UI und die Vorbereitung des Plugins für die Veröffentlichung im Autodesk App Store.

#### **Referenzen**

1. Autodesk.Revit.SDK 2026.0.0.9999 \- NuGet, Zugriff am August 31, 2025, [https://www.nuget.org/packages/Autodesk.Revit.SDK](https://www.nuget.org/packages/Autodesk.Revit.SDK)  
2. API Changes 2025 \- Revit API Docs, Zugriff am August 31, 2025, [https://www.revitapidocs.com/2025/news](https://www.revitapidocs.com/2025/news)  
3. Installing C\# support \- Visual Studio Code, Zugriff am August 31, 2025, [https://code.visualstudio.com/docs/languages/csharp](https://code.visualstudio.com/docs/languages/csharp)  
4. Getting Started with C\# in VS Code, Zugriff am August 31, 2025, [https://code.visualstudio.com/docs/csharp/get-started](https://code.visualstudio.com/docs/csharp/get-started)  
5. Lesson 1: The Basic Plug-in \- Autodesk, Zugriff am August 31, 2025, [https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7JiiE4UoWHaTjxzuF754mL.html](https://www.autodesk.com/support/technical/article/caas/tsarticles/ts/7JiiE4UoWHaTjxzuF754mL.html)  
6. building revit plug-ins with visual studio: part one | archi-lab, Zugriff am August 31, 2025, [https://archi-lab.net/building-revit-plug-ins-with-visual-studio-part-one/](https://archi-lab.net/building-revit-plug-ins-with-visual-studio-part-one/)  
7. Walkthrough: Hello World \- Autodesk product documentation, Zugriff am August 31, 2025, [https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit\_API\_Revit\_API\_Developers\_Guide\_Introduction\_Getting\_Started\_Walkthrough\_Hello\_World\_html](https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Introduction_Getting_Started_Walkthrough_Hello_World_html)  
8. Update API Assembly References and Wizards \- The Building Coder \- TypePad, Zugriff am August 31, 2025, [https://thebuildingcoder.typepad.com/blog/2012/06/update-api-assembly-references-and-wizards.html](https://thebuildingcoder.typepad.com/blog/2012/06/update-api-assembly-references-and-wizards.html)  
9. Multi-Year Revit Addin: A Better Way | by EGeer \- Level Up Coding, Zugriff am August 31, 2025, [https://levelup.gitconnected.com/multi-year-revit-addin-a-better-way-54ef441b5b4b](https://levelup.gitconnected.com/multi-year-revit-addin-a-better-way-54ef441b5b4b)  
10. Create a Plugin in Revit 2026 with .NET8 — AYDrafting, Zugriff am August 31, 2025, [https://www.aydrafting.com/blog/starting-a-project-in-revit-2026-net8-api](https://www.aydrafting.com/blog/starting-a-project-in-revit-2026-net8-api)  
11. How to best handle dll conflicts with revit addins \- Autodesk Community, Zugriff am August 31, 2025, [https://forums.autodesk.com/t5/revit-api-forum/how-to-best-handle-dll-conflicts-with-revit-addins/td-p/13285022](https://forums.autodesk.com/t5/revit-api-forum/how-to-best-handle-dll-conflicts-with-revit-addins/td-p/13285022)  
12. How to reload a new build without restarting Revit \- McNeel Forum, Zugriff am August 31, 2025, [https://discourse.mcneel.com/t/how-to-reload-a-new-build-without-restarting-revit/131853](https://discourse.mcneel.com/t/how-to-reload-a-new-build-without-restarting-revit/131853)  
13. debugging revit add-ins | archi-lab, Zugriff am August 31, 2025, [https://archi-lab.net/debugging-revit-add-ins/](https://archi-lab.net/debugging-revit-add-ins/)  
14. Revit 2015 Add-In Manager \- AEC DevBlog \- TypePad, Zugriff am August 31, 2025, [https://adndevblog.typepad.com/aec/2014/05/revit-2015-add-in-manager.html](https://adndevblog.typepad.com/aec/2014/05/revit-2015-add-in-manager.html)  
15. AddInManager \- The Building Coder, Zugriff am August 31, 2025, [https://thebuildingcoder.typepad.com/blog/2010/03/addinmanager.html](https://thebuildingcoder.typepad.com/blog/2010/03/addinmanager.html)  
16. Reload Add-in for Debug Without Restart \- The Building Coder \- TypePad, Zugriff am August 31, 2025, [https://thebuildingcoder.typepad.com/blog/2012/12/reload-add-in-for-debug-without-restart.html](https://thebuildingcoder.typepad.com/blog/2012/12/reload-add-in-for-debug-without-restart.html)  
17. The Building Coder: .NET Core, C4R Views and Interactive Hot Reloading \- TypePad, Zugriff am August 31, 2025, [https://thebuildingcoder.typepad.com/blog/2024/02/net-core-c4r-views-and-interactive-hot-reload.html](https://thebuildingcoder.typepad.com/blog/2024/02/net-core-c4r-views-and-interactive-hot-reload.html)  
18. What's New in Revit API 2026 \- RVTDocs.com, Zugriff am August 31, 2025, [https://rvtdocs.com/2026/whatsnew?utm\_source=youtube\&utm\_medium=video\&utm\_campaign=tutorial\&utm\_content=whats\_new\_in\_API\_2026](https://rvtdocs.com/2026/whatsnew?utm_source=youtube&utm_medium=video&utm_campaign=tutorial&utm_content=whats_new_in_API_2026)  
19. create your own tab and buttons in revit | archi-lab, Zugriff am August 31, 2025, [https://archi-lab.net/create-your-own-tab-and-buttons-in-revit/](https://archi-lab.net/create-your-own-tab-and-buttons-in-revit/)  
20. Programming Buttons in Revit \- BIM365, Zugriff am August 31, 2025, [https://www.bim365.tech/blog/programming-buttons-in-revit](https://www.bim365.tech/blog/programming-buttons-in-revit)  
21. building revit plug-ins with visual studio: part two | archi-lab, Zugriff am August 31, 2025, [https://archi-lab.net/building-revit-plug-ins-with-visual-studio-part-two/](https://archi-lab.net/building-revit-plug-ins-with-visual-studio-part-two/)  
22. IExternalCommand Interface \- Revit API Docs, Zugriff am August 31, 2025, [https://www.revitapidocs.com/2022/ad99887e-db50-bf8f-e4e6-2fb86082b5fb.htm](https://www.revitapidocs.com/2022/ad99887e-db50-bf8f-e4e6-2fb86082b5fb.htm)  
23. Add a New External Command (IExternalCommand) Using the RevitAddinWizard, Zugriff am August 31, 2025, [https://spiderinnet.typepad.com/blog/2011/03/add-a-new-external-command-iexternalcommand-using-the-revitaddinwizard.html](https://spiderinnet.typepad.com/blog/2011/03/add-a-new-external-command-iexternalcommand-using-the-revitaddinwizard.html)  
24. Creating the RpsAddin manifest · Scripting Autodesk Revit with RevitPythonShell, Zugriff am August 31, 2025, [https://daren-thomas.gitbooks.io/scripting-autodesk-revit-with-revitpythonshell/content/deploying\_rpsaddins/creating\_the\_rpsaddin\_manifest.html](https://daren-thomas.gitbooks.io/scripting-autodesk-revit-with-revitpythonshell/content/deploying_rpsaddins/creating_the_rpsaddin_manifest.html)  
25. Add-In Manifest and Guidize \- The Building Coder, Zugriff am August 31, 2025, [https://thebuildingcoder.typepad.com/blog/2010/04/addin-manifest-and-guidize.html](https://thebuildingcoder.typepad.com/blog/2010/04/addin-manifest-and-guidize.html)  
26. GeoJSON-Net/GeoJSON.Net: .Net library for GeoJSON ... \- GitHub, Zugriff am August 31, 2025, [https://github.com/GeoJSON-Net/GeoJSON.Net](https://github.com/GeoJSON-Net/GeoJSON.Net)  
27. GeoJson c\# example parse countries in the world and generate Geojson for each country, Zugriff am August 31, 2025, [https://stackoverflow.com/questions/39057828/geojson-c-sharp-example-parse-countries-in-the-world-and-generate-geojson-for-ea](https://stackoverflow.com/questions/39057828/geojson-c-sharp-example-parse-countries-in-the-world-and-generate-geojson-for-ea)  
28. How to serialize JSON in C\# \- .NET | Microsoft Learn, Zugriff am August 31, 2025, [https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to)  
29. DirectShape Class \- Revit API Docs, Zugriff am August 31, 2025, [https://www.revitapidocs.com/2017.1/bfbd137b-c2c2-71bb-6f4a-992d0dcf6ea8.htm](https://www.revitapidocs.com/2017.1/bfbd137b-c2c2-71bb-6f4a-992d0dcf6ea8.htm)  
30. Help | DirectShape | Autodesk, Zugriff am August 31, 2025, [https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit\_API\_Revit\_API\_Developers\_Guide\_Revit\_Geometric\_Elements\_DirectShape\_html](https://help.autodesk.com/view/RVT/2025/ENU/?guid=Revit_API_Revit_API_Developers_Guide_Revit_Geometric_Elements_DirectShape_html)  
31. TessellatedShapeBuilder Class \- Revit API Docs, Zugriff am August 31, 2025, [https://www.revitapidocs.com/2017.1/a144b0e3-c997-eac1-5c00-51c56d9e66f2.htm](https://www.revitapidocs.com/2017.1/a144b0e3-c997-eac1-5c00-51c56d9e66f2.htm)  
32. TessellatedShapeBuilder \- RVTDocs.com, Zugriff am August 31, 2025, [https://rvtdocs.com/2025/a144b0e3-c997-eac1-5c00-51c56d9e66f2](https://rvtdocs.com/2025/a144b0e3-c997-eac1-5c00-51c56d9e66f2)  
33. GeoJSON4EntityFramework, Zugriff am August 31, 2025, [https://alatas.org/GeoJSON4EntityFramework/](https://alatas.org/GeoJSON4EntityFramework/)  
34. TessellatedShapeBuilder \- RVTDocs.com, Zugriff am August 31, 2025, [https://rvtdocs.com/2023/a144b0e3-c997-eac1-5c00-51c56d9e66f2](https://rvtdocs.com/2023/a144b0e3-c997-eac1-5c00-51c56d9e66f2)  
35. Solid as OpenShell DirectShape \- Autodesk Community, Zugriff am August 31, 2025, [https://forums.autodesk.com/t5/revit-api-forum/solid-as-openshell-directshape/td-p/8782521](https://forums.autodesk.com/t5/revit-api-forum/solid-as-openshell-directshape/td-p/8782521)  
36. Specify build events (C\#) \- Visual Studio (Windows) | Microsoft Learn, Zugriff am August 31, 2025, [https://learn.microsoft.com/en-us/visualstudio/ide/how-to-specify-build-events-csharp?view=vs-2022](https://learn.microsoft.com/en-us/visualstudio/ide/how-to-specify-build-events-csharp?view=vs-2022)  
37. Migration to .NET 7 breaks post build event \- Microsoft Q\&A, Zugriff am August 31, 2025, [https://learn.microsoft.com/en-us/answers/questions/1326444/migration-to-net-7-breaks-post-build-event](https://learn.microsoft.com/en-us/answers/questions/1326444/migration-to-net-7-breaks-post-build-event)  
38. Visual Studio Post Build Event \- Copy to Relative Directory Location \- Stack Overflow, Zugriff am August 31, 2025, [https://stackoverflow.com/questions/834270/visual-studio-post-build-event-copy-to-relative-directory-location](https://stackoverflow.com/questions/834270/visual-studio-post-build-event-copy-to-relative-directory-location)  
39. Post-Build Events and .NET Core \- Jeremy Bytes, Zugriff am August 31, 2025, [https://jeremybytes.blogspot.com/2020/05/post-build-events-and-net-core.html](https://jeremybytes.blogspot.com/2020/05/post-build-events-and-net-core.html)