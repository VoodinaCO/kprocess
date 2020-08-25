using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Publications
{
    public class CompetenciesViewModel
    {
        public IEnumerable<UserCompetencyViewModel> UserCompetencyViewModel { get; set; }
        public IEnumerable<ProcessCompetencyViewModel> ProcessCompetencyViewModel { get; set; }
        public IEnumerable<TaskCompetencyViewModel> TaskCompetencyViewModel { get; set; }
        public int selectedIndexTeam { get; set; }
        public int selectedIndexPosition { get; set; }
    }
}