using Microsoft.Office.Interop.Excel;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public static class WorksheetExtensions
    {
        public static Worksheet CopyBefore(this Worksheet source, Worksheet before)
        {
            source.Copy(Before: before);
            return (Worksheet)before.Previous;
        }
    }
}
