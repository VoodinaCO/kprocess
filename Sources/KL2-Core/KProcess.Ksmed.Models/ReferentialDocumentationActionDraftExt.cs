namespace KProcess.Ksmed.Models
{
    public partial class ReferentialDocumentationActionDraft
    {
        public ReferentialDocumentationActionDraft(ReferentialDocumentationActionDraft reference)
        {
            ReferentialId = reference.ReferentialId;
            RefNumber = reference.RefNumber;
            Quantity = reference.Quantity;
        }
    }
}
