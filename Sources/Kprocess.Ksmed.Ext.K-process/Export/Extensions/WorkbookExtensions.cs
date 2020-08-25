using Microsoft.Office.Interop.Excel;
using System.Linq;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class WorkbookExtensions
    {
        public static Worksheet GetWorksheetByName(this Workbook workbook, string name) =>
            workbook.Worksheets.OfType<Worksheet>().FirstOrDefault(ws => ws.Name == name);
    }
}
