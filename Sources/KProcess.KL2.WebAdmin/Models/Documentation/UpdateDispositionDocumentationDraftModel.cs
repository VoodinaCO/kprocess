using KProcess.Ksmed.Models;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class UpdateDispositionDocumentationDraftModel
    {
        public DocumentationDraft Draft { get; set; }
        public string JsonDisposition { get; set; }
        public int PublishMode { get; set; }
    }
}