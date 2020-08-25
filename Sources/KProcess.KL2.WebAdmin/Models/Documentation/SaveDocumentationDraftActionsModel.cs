using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class SaveDocumentationDraftActionsModel
    {
        public DocumentationDraft Draft { get; set; }
        public List<DocumentationActionDraftWBS> TrainingActions { get; set; }
        public List<DocumentationActionDraftWBS> EvaluationActions { get; set; }
        public List<DocumentationActionDraftWBS> InspectionActions { get; set; }
    }
}