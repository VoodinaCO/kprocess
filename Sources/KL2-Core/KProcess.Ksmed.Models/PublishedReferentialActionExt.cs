using System.Collections.Generic;

namespace KProcess.Ksmed.Models
{
    partial class PublishedReferentialAction
    {
        public PublishedReferentialAction(IReferentialActionLink refActionLink, Dictionary<ProcessReferentialIdentifier, bool> refHasQuantity)
        {
            RefNumber = (byte)refActionLink.Referential.ProcessReferentialId - 3;
            if (refHasQuantity.ContainsKey(refActionLink.Referential.ProcessReferentialId) && refHasQuantity[refActionLink.Referential.ProcessReferentialId])
                Quantity = refActionLink.Quantity;
            else
                Quantity = null;
            //PublishedReferential = new PublishedReferential(refActionLink.Referential);
        }
    }
}
