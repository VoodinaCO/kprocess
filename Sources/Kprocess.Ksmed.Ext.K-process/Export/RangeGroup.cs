using Microsoft.Office.Interop.Excel;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public class RangeGroup
    {
        public Range RangeStart { get; set; }

        public Range RangeFinish { get; set; }

        public string WBSStart { get; set; }

        public string WBSFinish { get; set; }

        public int StartLineNumber { get; set; }

        public int FinishLineNumber { get; set; }
    }
}
