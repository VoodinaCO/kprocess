using System;
using System.Collections.Generic;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Evaluation
{
    [Serializable]
    public class EvaluationReportViewModel
    {
        public string ProcessLabel { get; set; }
        public string EvaluatedName { get; set; }
        public string EvaluatedTenured { get; set; }
        public List<string> Teams { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReferentName { get; set; }
        public List<string> QualifiersName { get; set; }
        public string Result { get; set; }
        public string Decision { get; set; }
        public string Comment { get; set; }

        public List<ObservationViewModel> Observations { get; set; }
    }
}
