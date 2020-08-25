using System;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Audit
{
    [Serializable]
    public class AuditItemViewModel
    {
        public string Question { get; set; }
        public bool HasResult { get; set; }
        public bool Result { get; set; }
        public string Comment { get; set; }
    }
}
