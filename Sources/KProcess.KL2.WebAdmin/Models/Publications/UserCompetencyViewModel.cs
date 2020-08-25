using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class UserCompetencyViewModel
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public List<bool> HasCompetency { get; set; }
        public List<bool> HasCompetencyPreviousVersion { get; set; }
        public List<bool> HasCompetencyPreviousMajorVersion { get; set; }
        public List<string> HasCompetencyData { get; set; }
        public List<int> PercentageResult { get; set; }
    }
}