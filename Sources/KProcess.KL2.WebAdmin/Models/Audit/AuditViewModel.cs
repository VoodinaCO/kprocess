using KProcess.KL2.WebAdmin.Models.Inspection;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Audit
{
    [Serializable]
    public class AuditViewModel
    {
        public int? AuditId { get; set; }
        public int SurveyId { get; set; }
        public string SurveyName { get; set; }
        public int AuditorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<AuditItemViewModel> AuditItems { get; set; }
        public int AuditeeId { get; set; }


        public int InspectionId { get; set; }
        public string ProcessName { get; set; }
        public string AuditorName { get; set; }
        public string AuditeeName { get; set; }
        public List<string> AuditorTeams { get; set; }
        public bool IsOK { get; set; }
        public InspectionViewModel Inspection { get; set; }
        public int AnomalyNumber { get; set; }
        
        public List<string> SurveyQuestions { get; set; }
    }
}