using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Survey
{
    public class SurveyViewModel
    {   
        public int? SurveyId { get; set; }
        public string Name { get; set; }
        public List<SurveyItemViewModel> SurveyItems { get; set; }
    }
}