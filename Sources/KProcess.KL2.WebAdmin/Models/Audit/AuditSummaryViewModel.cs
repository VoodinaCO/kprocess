using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Audit
{
    public class AuditSummaryViewModel
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
        public string stringTotal { get; set; }
        public string Color { get; set; }
    }
}