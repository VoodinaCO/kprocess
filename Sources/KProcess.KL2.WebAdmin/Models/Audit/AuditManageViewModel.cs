using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Audit
{
    public class AuditManageViewModel
    {
        public AuditManageViewModel()
        {
            Audits = new List<AuditViewModel>();
        }
        public IEnumerable<AuditViewModel> Audits { get; set; }
    }
}