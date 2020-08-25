using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Survey
{
    public class SurveyManageViewModel
    {
        public SurveyManageViewModel()
        {
            Surveys = new List<SurveyViewModel>();
        }
        public IEnumerable<SurveyViewModel> Surveys { get; set; }
    }
}