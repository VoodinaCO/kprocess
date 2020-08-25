﻿using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Survey
{
    public class SurveyItemViewModel
    {
        public int SurveyId { get; set; }
        public int Number { get; set; }
        public string Query { get; set; }
    }
}