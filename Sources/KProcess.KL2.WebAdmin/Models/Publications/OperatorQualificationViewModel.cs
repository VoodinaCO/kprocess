using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class OperatorQualificationViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public int Success { get; set; }
        public int Failed { get; set; }
        public int Total { get; set; }
        public double PercentageRate { get; set; }
    }
}