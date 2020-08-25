using KProcess.Ksmed.Models;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.NotificationSetting
{
    public class NotificationTypeViewModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int NotificationTypeSettingId { get; set; }
        public NotificationCategory Category { get; set; }
        public List<string> Recipients { get; set; }
        public List<string> RecipientCcs { get; set; }
        public List<string> RecipientBccs { get; set; }

    }

    public class NotificationTypeList
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}