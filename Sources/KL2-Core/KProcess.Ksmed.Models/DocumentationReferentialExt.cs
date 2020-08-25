using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    public partial class DocumentationReferential
    {
        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public bool IsEditable { get; set; }

        [DataMember]
        public ReferentialCategory Category { get; set; }
    }
}
