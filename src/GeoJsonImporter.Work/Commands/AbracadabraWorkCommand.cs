using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace GeoJsonImporter.Work.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class AbracadabraWorkCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 🧙‍♂️ HOT RELOAD TEST - Zeigt "3" nach dem Reload!
            MessageBox.Show("3", "Hot Reload Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            return Result.Succeeded;
        }
    }
}
