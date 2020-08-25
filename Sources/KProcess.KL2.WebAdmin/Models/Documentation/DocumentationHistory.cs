using KProcess.Ksmed.Models;
using System;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class DocumentationHistory
    {
        public int ProcessId { get; set; }
        public int PublicationHistoryId { get; set; }
        public string ProcessLabel { get; set; }
        public string Description { get; set; }
        public string TrainingVersion { get; set; }
        public string EvaluationVersion { get; set; }
        public string InspectionVersion { get; set; }
        public string StateAsLabel { get; set; }
        public string StateAsString { get; set; }
        public PublicationStatus State{ get; set; }
        public string ErrorMessage {get;set;}
        public string ProgressBar { get; set; }
        public string Publisher { get; set; }
        public DateTime Timestamp{ get; set; }
        public string TimestampAsString {
            get
            {
                return $"{Timestamp.ToShortDateString()} {Timestamp.ToShortTimeString()}";
            }
        }
    }
}