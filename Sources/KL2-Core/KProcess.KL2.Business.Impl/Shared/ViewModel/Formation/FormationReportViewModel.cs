using System;
using System.Collections.Generic;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Formation
{
    [Serializable]
    public class FormationReportViewModel
    {
        public string ProcessLabel { get; set; }
        public string TrainedName { get; set; }
        public string TrainedTenured { get; set; }
        public List<string> Teams { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> TrainersName { get; set; }
        public string ReferentName { get; set; }
    }
}
