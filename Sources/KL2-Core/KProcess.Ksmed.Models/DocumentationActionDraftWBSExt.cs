using System.Linq;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    public partial class DocumentationActionDraftWBS
    {
        [DataMember]
        public bool IsDocumentation =>
            DocumentationActionDraftId != null;

        [DataMember]
        public int TreeId { get; set; }

        [DataMember]
        public int? ParentId { get; set; }

        [DataMember]
        public bool IsGroup { get; set; }

        [DataMember]
        public bool IsExpanded { get; set; }

        /// <summary>
        /// Obtient les parties du WBS.
        /// </summary>
        public int[] WBSParts
        {
            get
            {
                if (WBS != null)
                    return WBS.Split('.').Select(str => int.Parse(str)).ToArray();
                else
                    return null;
            }
        }

        public void Delete()
        {
            ChangeTracker.ChangeTrackingEnabled = true;
            DocumentationActionDraft?.Delete();
            this.MarkAsDeleted();
        }

        public DocumentationActionDraftWBS(DocumentationActionDraftWBS reference)
        {
            ActionId = reference.IsDocumentation ? null : reference.ActionId;
            DocumentationActionDraft = reference.IsDocumentation ? new DocumentationActionDraft(reference.DocumentationActionDraft) : null;
            WBS = reference.WBS;
            DocumentationPublishMode = reference.DocumentationPublishMode;
        }

        public void Update(DocumentationActionDraftWBS reference)
        {
            ChangeTracker.ChangeTrackingEnabled = true;
            WBS = reference.WBS;

            if (DocumentationActionDraft != null)
                DocumentationActionDraft.Update(reference.DocumentationActionDraft);
        }
    }
}
