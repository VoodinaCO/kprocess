using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    public partial class DocumentationDraft
    {
        [DataMember]
        public bool IsFromPrevious { get; set; }
    }
}
