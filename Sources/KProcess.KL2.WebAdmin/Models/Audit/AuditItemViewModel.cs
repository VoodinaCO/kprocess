using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Audit
{
    public class AuditItemViewModel
    {
        public int AuditId { get; set; }
        public int Number { get; set; }
        public string Question { get; set; }
        public bool? IsOK { get; set; }
        public bool HasPhoto { get; set; }
        public string Photo { get; set; }
        public string RawPhoto { get; set; }
        public string Comment { get; set; }
    }
}