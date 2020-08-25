using System;
using System.Collections.Generic;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Audit
{
    [Serializable]
    public class AuditReportViewModel
    {
        public string ProcessLabel { get; set; }
        public List<string> Teams { get; set; }
        public string AuditorName { get; set; }
        public string AuditeeName { get; set; }
        public bool AuditeeHasTenured { get; set; }
        public bool AuditeeTenured { get; set; }
        public List<AuditItemViewModel> AuditItemsViewModel { get; set; }
        public List<AnomalyViewModel> AnomaliesViewModel { get; set; }
    }
}
