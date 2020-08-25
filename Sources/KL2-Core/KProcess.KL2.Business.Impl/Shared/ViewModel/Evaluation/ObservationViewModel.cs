using System;

namespace KProcess.KL2.Business.Impl.Shared.ViewModel.Evaluation
{
    [Serializable]
    public class ObservationViewModel
    {
        public string WBS { get; set; }
        public string Action { get; set; }
        public string Question { get; set; }
        public string Comment { get; set; }
    }
}
