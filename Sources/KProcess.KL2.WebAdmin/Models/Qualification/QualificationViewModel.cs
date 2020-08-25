using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Qualification
{
    [Serializable]
    public class QualificationViewModel
    {
        public int QualificationId { get; set; }
        public string Folder { get; set; }
        public string FolderPath { get; set; }
        public string ProcessName { get; set; }
        public string Trainer { get; set; }
        public string Operator { get; set; }
        public string Qualifier { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public DateTime? TrainingEndDate { get; set; }
        public DateTime QualificationDate { get; set; }
        public string PublicationName { get; set; }
        public bool Result { get; set; }
        public int PercentageResult { get; set; }
        public string Notes { get; set; }
        public List<string> Teams { get; set; }
        public List<QualificationStepViewModel> Steps { get; set; }
    }
}