using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    [Serializable]
    public class InspectionViewModel
    {
        public int InspectionId { get; set; }
        public Guid PublicationId { get; set; }
        public string ProcessName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime Date { get; set; }
        public bool IsOK { get; set; }
        public int AnomalyNumber{ get; set; }
        public bool AuditExists { get; set; }

        public List<InspectionStepViewModel> Steps { get; set; }
        public List<AnomalyViewModel> Anomalies { get; set; }
        
        public string InspectionDate { get; set; }
        public List<string> Teams { get; set; }
        public List<string> Inspectors { get; set; }
        public bool DoneByDeactivated { get; set; }
    }
}