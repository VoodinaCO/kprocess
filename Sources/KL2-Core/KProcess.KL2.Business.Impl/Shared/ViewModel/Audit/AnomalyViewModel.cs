using System;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Audit
{
    [Serializable]
    public class AnomalyViewModel
    {
        public string TypeLabel { get; set; }
        public string TypeColor { get; set; }
        public string Comment { get; set; }
        public bool HasPhoto { get; set; }
        public string Photo { get; set; }
    }
}
