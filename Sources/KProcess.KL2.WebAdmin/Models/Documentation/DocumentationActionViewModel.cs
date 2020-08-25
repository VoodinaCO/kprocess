using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class ReferentialField
    {
        public ProcessReferentialIdentifier ReferentialFieldId { get; set; }
        public string ReferentialFieldName { get; set; }
        public bool HasMultipleSelection { get; set; }
        public bool HasQuantity { get; set; }
        public IList<ReferentialFieldElement> ReferentialsFieldElements { get; set; }
    }

    public class ReferentialFieldElement
    {
        public int Id { get; set; }
        public int RefId { get; set; }
        public string Label { get; set; }
        public bool HasQuantity { get; set; }
        public string CategoryAsLabel { get; set; }
        public int Category { get; set; }
        public CloudFile CloudFile { get; set; }
        public string Description { get; set; }
    }

    public class ReferentialFieldValues
    {
        public ProcessReferentialIdentifier ReferentialFieldId { get; set; }
        public string ReferentialFieldName { get; set; }
        public IList<ReferentialFieldValue> Values { get; set; }
    }
    public static class ReferentialFieldValuesExt
    {
        public static List<TRefAction> GetDocumentationRefs<TRefAction>(this List<ReferentialFieldValues> referentialFieldValues) where TRefAction : IReferentialActionLink, new()
        {
            ProcessReferentialIdentifier refIdentifier = ProcessReferentialIdentifier.Skill;
            if (typeof(TRefAction) == typeof(Ref1Action))
                refIdentifier = ProcessReferentialIdentifier.Ref1;
            else if (typeof(TRefAction) == typeof(Ref2Action))
                refIdentifier = ProcessReferentialIdentifier.Ref2;
            else if (typeof(TRefAction) == typeof(Ref3Action))
                refIdentifier = ProcessReferentialIdentifier.Ref3;
            else if (typeof(TRefAction) == typeof(Ref4Action))
                refIdentifier = ProcessReferentialIdentifier.Ref4;
            else if (typeof(TRefAction) == typeof(Ref5Action))
                refIdentifier = ProcessReferentialIdentifier.Ref5;
            else if (typeof(TRefAction) == typeof(Ref6Action))
                refIdentifier = ProcessReferentialIdentifier.Ref6;
            else if (typeof(TRefAction) == typeof(Ref7Action))
                refIdentifier = ProcessReferentialIdentifier.Ref7;

            return referentialFieldValues
                .Where(u => u.ReferentialFieldId == refIdentifier)
                .SelectMany(reference => reference.Values,
                    (reference, fieldValue) => new TRefAction
                    {
                        ReferentialId = fieldValue.ReferentialId,
                        Quantity = fieldValue.Quantity ?? 1
                    })
                .ToList();
        }
    }

    public class ReferentialFieldValue
    {
        public int ReferentialId{ get; set; }
        public int? Quantity{ get; set; }
    }
}