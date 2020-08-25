using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Models.NotificationSetting;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class NotificationTypeMapper
    {
        public static NotificationManageViewModel ToNotificationManageViewModel(IEnumerable<NotificationType> notificationTypes, int? type, int? to, string language)
        {
            var model = new NotificationManageViewModel();
            NotificationTypeViewModel typeViewModel = new NotificationTypeViewModel();
            List<NotificationTypeList> typeList = new List<NotificationTypeList>();
            model.Addresses = new List<AddressViewModel>();
            foreach (var notificationType in notificationTypes)
            {
                if ((int)notificationType.NotificationCategory == type)
                {
                    typeViewModel = new NotificationTypeViewModel
                    {
                        Id = notificationType.Id,
                        Label = notificationType.Label,
                        NotificationTypeSettingId = notificationType.NotificationTypeSettingId,
                        Category = notificationType.NotificationCategory
                    };
                    model.Addresses = ToAddressViewModels(notificationType, to);
                }
                typeList.Add(new NotificationTypeList
                {
                    Id = (int)notificationType.NotificationCategory,
                    Label = notificationType.Label,
                    Description = GetNotificationTypeLabelDescription(notificationType.Label, language)
                });
            }
            model.NotificationTypeViewModel = typeViewModel;
            model.NotificationTypeList = typeList;
            return model;
        }

        public static List<AddressViewModel> ToAddressViewModels(NotificationType notificationType, int? to)
        {
            List<string> addresses = new List<string>();
            List<AddressViewModel> model = new List<AddressViewModel>();
            switch (to)
            {
                case (int)ToType.Recipients:
                    addresses = notificationType.NotificationTypeSetting.Recipients?.Split(',').ToList();
                    break;
                case (int)ToType.RecipientCcs:
                    addresses = notificationType.NotificationTypeSetting.RecipientCcs?.Split(',').ToList();
                    break;
                case (int)ToType.RecipientBccs:
                    addresses = notificationType.NotificationTypeSetting.RecipientBccs?.Split(',').ToList();
                    break;
                default:
                    break;
            }
            if (addresses != null)
            {
                int count = 1;
                foreach (var address in addresses)
                {
                    model.Add(new AddressViewModel { Address = address, Id = count });
                    count++;
                }
            }
            return model;
        }

        public static string GetNotificationTypeLabelDescription(string label, string language)
        {
            var localizedStrings = DependencyResolver.Current.GetService<ILocalizedStrings>();
            switch (label)
            {
                case "Evaluation":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_LiveReportOfQualifications", language);
                case "Inspection":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_ReportOfInspectionsFromPreviousDay", language);
                case "Audit":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_LiveReportOfAudits", language);
                case "Anomaly":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_LiveReportOfAnomalies", language);
                case "Formation":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_LiveReportOfTrainings", language);
                default:
                    return "undefined";
            }
        }

        public static string GetAppSettingLabel(string key, string language)
        {
            var localizedStrings = DependencyResolver.Current.GetService<ILocalizedStrings>();
            switch (key)
            {
                case "Email_Sender_Address":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_EmailSenderAddress", language);
                case "Email_Sender_Password":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_EmailSenderPassword", language);
                case "SMTP_Client":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_SMTPClient", language);
                case "SMTP_EnableSsl":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_SMTPEnableSsl", language);
                case "SMTP_Port":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_SMTPPort", language);
                case "SMTP_UseDefaultCredentials":
                    return localizedStrings.GetLanguageValue("Web_Controller_Notification_SMTPUseDefaultCredentials", language);
                default:
                    return key;
            }
        }
    }
}