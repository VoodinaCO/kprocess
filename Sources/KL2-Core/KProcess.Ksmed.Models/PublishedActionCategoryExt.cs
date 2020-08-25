namespace KProcess.Ksmed.Models
{
    partial class PublishedActionCategory
    {
        public PublishedActionCategory(ActionCategory actionC)
        {
            Label = actionC.Label;
            Description = actionC.Description;
            ActionTypeCode = actionC.ActionTypeCode;
            ActionValueCode = actionC.ActionValueCode;
            FileHash = actionC.Hash;
        }
    }
}
