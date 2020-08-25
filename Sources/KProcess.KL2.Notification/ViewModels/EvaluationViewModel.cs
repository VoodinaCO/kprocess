using System;

namespace KProcess.KL2.Notification.ViewModels
{
    [Serializable]
    public class EvaluationViewModel
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }
}
