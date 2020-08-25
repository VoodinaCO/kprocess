using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    public partial class PublicationHistory
    {
        [DataMember]
        public string TrainingPublicationVersion { get; set; }

        [DataMember]
        public string EvaluationPublicationVersion { get; set; }

        [DataMember]
        public string InspectionPublicationVersion { get; set; }
    }
}
