using KProcess.Ksmed.Models;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class UpdateVideoDocumentationDraftModel
    {
        public DocumentationDraft Draft { get; set; }
        public bool ActiveVideoExport { get; set; }
        public bool SlowMotion { get; set; }
        public double SlowMotionDuration { get; set; }
        public bool WaterMarking { get; set; }
        public string WaterMarkingText { get; set; }
        public int WaterMarkingVAlign { get; set; }
        public int WaterMarkingHAlign { get; set; }
    }
}