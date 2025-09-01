using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class GeoRefWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // 🧪 EINFACHER TEST: Nur MessageBox mit "1"
                MessageBox.Show("1", "Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Fehler: {ex.Message}";
                return Result.Failed;
            }
        }
    }
}
