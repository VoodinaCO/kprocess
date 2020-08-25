using KProcess.Ksmed.Models;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class UpdateActionDispositionDocumentationDraftModel
    {
        public DocumentationDraft Draft { get; set; }
        public string ActionDisposition { get; set; }
        public int PublishMode { get; set; }
    }
}