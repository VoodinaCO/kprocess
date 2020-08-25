using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Qualification
{
    public class QualificationManageViewModel
    {
        public QualificationManageViewModel()
        {
            Qualifications = new List<QualificationViewModel>();
        }
        public IEnumerable<QualificationViewModel> Qualifications { get; set; }
    }
}