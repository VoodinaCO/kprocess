using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Qualification
{
    public class QualificationStepViewModel
    {
        public int QualificationStepId { get; set; }
        public string Wbs { get; set; }
        public string Date { get; set; }
        public string Comment { get; set; }
        public int QualificationId { get; set; }
        public int PublishedActionId { get; set; }
        public int QualifierId { get; set; }
        public bool? IsQualified { get; set; }
        public string ActionLabel { get; set; }
        public string QualifierName { get; set; }
        public int Level { get; set; }
        public bool? IsParent { get; set; }
        public string colorCondition { get; set; }

    }
}