using KProcess.Ksmed.Models;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class UpdateReleaseNotesDocumentationDraftModel
    {
        public DocumentationDraft Draft { get; set; }
        public bool IsMajor { get; set; }
        public string ReleaseNotes { get; set; }
        public int PublishMode { get; set; }
    }
}