using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.NotificationSetting
{
    public class NotificationManageViewModel
    {
        public NotificationTypeViewModel NotificationTypeViewModel { get; set; }
        public List<NotificationTypeList> NotificationTypeList { get; set; }
        public int? Type { get; set; }
        public int? To { get; set; }
        public List<AddressViewModel> Addresses { get; set; }
        public bool CanSendMail { get; set; }

        // SMTP parameters
        public bool UseAnonymMode { get; set; }
        public string SenderEmail { get; set; }
        public string Password { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public bool UseSSL { get; set; }
    }
    public class AddressViewModel
    {
        public int Id { get; set; }
        public int To { get; set; }
        public string Address { get; set; }
    }

    public class SMTPViewModel
    {
        public string Key { get; set; }
        public string Label { get; set; }
        public string Value { get; set; }
        public bool IsPassword { get; set; }
    }
}