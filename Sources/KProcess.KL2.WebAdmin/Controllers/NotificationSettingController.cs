using KProcess.KL2.JWT;
using KProcess.KL2.Languages;
using KProcess.KL2.WebAdmin.Authentication;
using KProcess.KL2.WebAdmin.Identity;
using KProcess.KL2.WebAdmin.Mapper;
using KProcess.KL2.WebAdmin.Models.NotificationSetting;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Syncfusion.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace KProcess.KL2.WebAdmin.Controllers
{
    [CustomAuthorize(KnownRoles.Administrator)]
    [SettingUserContextFilter]
    public class NotificationSettingController : LocalizedController
    {
        readonly INotificationService _notificationService;
        readonly IPrepareService _prepareService;

        public NotificationSettingController(INotificationService notificationService,
            IPrepareService prepareService,
            ILocalizationManager localizationManager)
            : base(localizationManager)
        {
            _notificationService = notificationService;
            _prepareService = prepareService;
        }
        // GET: NotificationSetting
        public async Task<ActionResult> Index(int? type, int? to, bool partial = false)
        {
            var model = await GetNotificationManage(type, to);
            model.Type = type;
            model.To = to;
            if (partial)
                return PartialView(model);
            return View(model);
        }

        public async Task<ActionResult> Address(int? type, int? to)
        {
            var model = await GetNotificationManage(type.Value, to.Value);
            model.Type = type;
            model.To = to;
            return PartialView(model);
        }

        public async Task<NotificationManageViewModel> GetNotificationManage(int? type, int? to)
        {
            var notificationTypes = await _notificationService.GetNotificationTypes();
            var model = NotificationTypeMapper.ToNotificationManageViewModel(notificationTypes, type, to, JwtTokenProvider.GetUserModelCurrentLanguage(Request.Cookies["token"].Value));
            var appSettings = await _prepareService.GetAllAppSettings();
            model.CanSendMail = true;
            try
            {
                model.UseAnonymMode = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode").Value)) : false;
                model.SenderEmail = appSettings.Any(_ => _.Key == "Email_Sender_Address") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Address").Value) : string.Empty;
                model.Password = appSettings.Any(_ => _.Key == "Email_Sender_Password") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Password").Value) : string.Empty;
                model.ServerAddress = appSettings.Any(_ => _.Key == "SMTP_Client") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Client").Value) : string.Empty;
                model.ServerPort = appSettings.Any(_ => _.Key == "SMTP_Port") ? int.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Port").Value)) : 25;
                model.UseSSL = appSettings.Any(_ => _.Key == "SMTP_EnableSsl") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_EnableSsl").Value)) : true;
                if (string.IsNullOrEmpty(model.SenderEmail))
                    model.CanSendMail = false;
                else if (!model.UseAnonymMode && string.IsNullOrEmpty(model.Password))
                    model.CanSendMail = false;
                else if (string.IsNullOrEmpty(model.ServerAddress))
                    model.CanSendMail = false;
            }
            catch
            {
                model.CanSendMail = false;
            }
            if (type != null)
            {
                var notifTypeSetting = notificationTypes.SingleOrDefault(nt => (int)nt.NotificationCategory == type.Value)?.NotificationTypeSetting;
                if (notifTypeSetting != null)
                    model.CanSendMail &= !string.IsNullOrEmpty(notifTypeSetting.Recipients);
            }
            return model;
        }

        public async Task<ActionResult> SMTP()
        {
            var appSettings = await _prepareService.GetAllAppSettings();
            var model = new NotificationManageViewModel
            {
                CanSendMail = true
            };
            try
            {
                model.UseAnonymMode = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode").Value)) : false;
                model.SenderEmail = appSettings.Any(_ => _.Key == "Email_Sender_Address") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Address").Value) : string.Empty;
                model.Password = appSettings.Any(_ => _.Key == "Email_Sender_Password") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Password").Value) : string.Empty;
                model.ServerAddress = appSettings.Any(_ => _.Key == "SMTP_Client") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Client").Value) : string.Empty;
                model.ServerPort = appSettings.Any(_ => _.Key == "SMTP_Port") ? int.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Port").Value)) : 25;
                model.UseSSL = appSettings.Any(_ => _.Key == "SMTP_EnableSsl") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_EnableSsl").Value)) : true;
                if (string.IsNullOrEmpty(model.SenderEmail))
                    model.CanSendMail = false;
                else if (!model.UseAnonymMode && string.IsNullOrEmpty(model.Password))
                    model.CanSendMail = false;
                else if (string.IsNullOrEmpty(model.ServerAddress))
                    model.CanSendMail = false;
            }
            catch
            {
                model.CanSendMail = false;
            }
            return PartialView(model);
        }

        public async Task<ActionResult> UpdateSMTP(CRUDModel<SMTPViewModel> smtp)
        {
            var appSettings = await _prepareService.GetAllAppSettings();
            var editSetting = appSettings.FirstOrDefault(s => s.Key == smtp.Value.Key);
            editSetting.MarkAsModified();
            editSetting.Value = Encoding.UTF8.GetBytes(smtp.Value.Value);
            await _prepareService.SaveAppSettings(appSettings);
            smtp.Value.IsPassword = smtp.Value.Key == "Email_Sender_Password" ? true : false;
            return Json(smtp.Value, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateAddress(CRUDModel<AddressViewModel> address)
        {
            var to = Convert.ToInt16(address.Params["To"]);
            var type = Convert.ToInt32(address.Params["Type"]);
            var notificationTypes = await _notificationService.GetNotificationTypes((NotificationCategory)type);
            var notificationType = notificationTypes.FirstOrDefault();
            notificationType.NotificationTypeSetting.MarkAsModified();
            switch (to)
            {
                case (int)ToType.Recipients:
                    var recipientList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    recipientList.FirstOrDefault(t => t.Id == address.Value.Id).Address = address.Value.Address;
                    notificationType.NotificationTypeSetting.Recipients = string.Join(",", recipientList.Select(l => l.Address));
                    await _notificationService.SaveNotificationType(notificationType);
                    break;
                case (int)ToType.RecipientCcs:
                    var ccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    ccsList.FirstOrDefault(t => t.Id == address.Value.Id).Address = address.Value.Address;
                    notificationType.NotificationTypeSetting.RecipientCcs = string.Join(",", ccsList.Select(l => l.Address));
                    await _notificationService.SaveNotificationType(notificationType);
                    break;
                case (int)ToType.RecipientBccs:
                    var bccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    bccsList.FirstOrDefault(t => t.Id == address.Value.Id).Address = address.Value.Address;
                    notificationType.NotificationTypeSetting.RecipientBccs = string.Join(",", bccsList.Select(l => l.Address));
                    await _notificationService.SaveNotificationType(notificationType);
                    break;
                default:
                    break;
            }
            return Json(address, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> InsertAddress(CRUDModel<AddressViewModel> address)
        {
            var to = Convert.ToInt16(address.Params["To"]);
            var type = Convert.ToInt32(address.Params["Type"]);
            var notificationTypes = await _notificationService.GetNotificationTypes((NotificationCategory)type);
            var notificationType = notificationTypes.FirstOrDefault();
            notificationType.NotificationTypeSetting.MarkAsModified();
            switch (to)
            {
                case (int)ToType.Recipients:
                    var recipientList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    recipientList.Add(new AddressViewModel { Address = address.Value.Address });
                    notificationType.NotificationTypeSetting.Recipients = string.Join(",", recipientList.Select(l => l.Address));
                    var newRecipient = await _notificationService.SaveNotificationType(notificationType);
                    address.Value.Id = recipientList.Last().Id + 1;
                    break;
                case (int)ToType.RecipientCcs:
                    var ccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    ccsList.Add(new AddressViewModel { Address = address.Value.Address });
                    notificationType.NotificationTypeSetting.RecipientCcs = string.Join(",", ccsList.Select(l => l.Address));
                    var newCc = await _notificationService.SaveNotificationType(notificationType);
                    address.Value.Id = ccsList.Last().Id + 1;
                    break;
                case (int)ToType.RecipientBccs:
                    var bccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                    bccsList.Add(new AddressViewModel { Address = address.Value.Address });
                    notificationType.NotificationTypeSetting.RecipientBccs = string.Join(",", bccsList.Select(l => l.Address));
                    var newBcc = await _notificationService.SaveNotificationType(notificationType);
                    address.Value.Id = bccsList.Last().Id + 1;
                    break;
                default:
                    break;
            }
            return Json(address.Value, JsonRequestBehavior.AllowGet);
        }

        //public async Task<ActionResult> DeleteAddress(int key)
        //{
        //    var to = Convert.ToInt16(Request.Headers.GetValues("To")[0]);
        //    var type = Convert.ToInt32(Request.Headers.GetValues("Type")[0]);
        //    var address = Request.Headers.GetValues("Address")[0];
        //    var notificationTypes = await _notificationService.GetNotificationTypes((NotificationCategory)type);
        //    var notificationType = notificationTypes.FirstOrDefault();
        //    notificationType.NotificationTypeSetting.MarkAsModified();
        //    switch (to)
        //    {
        //        case (int)ToType.Recipients:
        //            var recipientList = NotificationTypeMapper.toAddressViewModels(notificationType, to);
        //            var recipientToRemove = recipientList.First(r => r.Address == address);
        //            recipientList.Remove(recipientToRemove);
        //            notificationType.NotificationTypeSetting.Recipients = recipientList.Any() ? string.Join(",", recipientList.Select(l => l.Address)) : null;
        //            await _notificationService.SaveNotificationType(notificationType);
        //            break;
        //        case (int)ToType.RecipientCcs:
        //            var ccsList = NotificationTypeMapper.toAddressViewModels(notificationType, to);
        //            var ccToRemove = ccsList.First(r => r.Address == address);
        //            ccsList.Remove(ccToRemove);
        //            notificationType.NotificationTypeSetting.RecipientCcs = ccsList.Any() ? string.Join(",", ccsList.Select(l => l.Address)) : null;
        //            await _notificationService.SaveNotificationType(notificationType);
        //            break;
        //        case (int)ToType.RecipientBccs:
        //            var bccsList = NotificationTypeMapper.toAddressViewModels(notificationType, to);
        //            var bccToRemove = bccsList.First(r => r.Address == address);
        //            bccsList.Remove(bccToRemove);
        //            notificationType.NotificationTypeSetting.RecipientBccs = bccsList.Any() ? string.Join(",", bccsList.Select(l => l.Address)) : null;
        //            await _notificationService.SaveNotificationType(notificationType);
        //            break;
        //        default:
        //            break;
        //    }
        //    return Json(key, JsonRequestBehavior.AllowGet);
        //}

        public async Task<ActionResult> DeleteAddress(int key, int To, int Type)
        {
            try
            {
                int to = To;
                int type = Type;
                var addressId = key;
                var notificationTypes = await _notificationService.GetNotificationTypes((NotificationCategory)type);
                var notificationType = notificationTypes.FirstOrDefault();
                notificationType.NotificationTypeSetting.MarkAsModified();
                switch (to)
                {
                    case (int)ToType.Recipients:
                        var recipientList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                        var recipientToRemove = recipientList.First(r => r.Id == addressId);
                        recipientList.Remove(recipientToRemove);
                        notificationType.NotificationTypeSetting.Recipients = recipientList.Any() ? string.Join(",", recipientList.Select(l => l.Address)) : null;
                        await _notificationService.SaveNotificationType(notificationType);
                        break;
                    case (int)ToType.RecipientCcs:
                        var ccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                        var ccToRemove = ccsList.First(r => r.Id == addressId);
                        ccsList.Remove(ccToRemove);
                        notificationType.NotificationTypeSetting.RecipientCcs = ccsList.Any() ? string.Join(",", ccsList.Select(l => l.Address)) : null;
                        await _notificationService.SaveNotificationType(notificationType);
                        break;
                    case (int)ToType.RecipientBccs:
                        var bccsList = NotificationTypeMapper.ToAddressViewModels(notificationType, to);
                        var bccToRemove = bccsList.First(r => r.Id == addressId);
                        bccsList.Remove(bccToRemove);
                        notificationType.NotificationTypeSetting.RecipientBccs = bccsList.Any() ? string.Join(",", bccsList.Select(l => l.Address)) : null;
                        await _notificationService.SaveNotificationType(notificationType);
                        break;
                    default:
                        break;
                }
                var model = await GetNotificationManage(type, to);
                return Json(model.Addresses, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendReportMailTest(int notifTypeId)
        {
            try
            {
                await _notificationService.PushNotification("Test email", notifTypeId, new List<byte[]>());
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> TestSMTP(string to)
        {
            var appSettings = await _prepareService.GetAllAppSettings();
            var useAnonymMode = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode").Value)) : false;
            try
            {
                using (var smtp = new SmtpClient())
                using (MailMessage message = new MailMessage())
                {
                    smtp.EnableSsl = Convert.ToBoolean(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_EnableSsl").Value));
                    smtp.Host = Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_Client").Value);
                    smtp.Port = Convert.ToInt32(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_Port").Value));
                    if (!useAnonymMode)
                        smtp.Credentials = new NetworkCredential(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Address").Value), Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Password").Value));

                    MailAddress fromAddr = new MailAddress(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Address").Value));
                    message.From = fromAddr;
                    message.Subject = "Test SMTP";
                    message.IsBodyHtml = false;
                    string htmlString = "Test SMTP";
                    message.Body = htmlString;

                    message.To.Add(to);

                    smtp.Send(message);
                }
                HttpContext.Response.StatusDescription = $"{HttpStatusCode.OK}";
                return Content($"{HttpStatusCode.OK}");
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusDescription = $"{HttpStatusCode.InternalServerError}";
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> GetSmtpParameters()
        {
            var appSettings = await _prepareService.GetAllAppSettings(); 
            bool useAnonymMode = false;
            string senderEmail = string.Empty;
            string password = string.Empty;
            string serverAddress = string.Empty;
            int serverPort = 25;
            bool useSSL = true;
            try
            {
                useAnonymMode = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode").Value)) : false;
                senderEmail = appSettings.Any(_ => _.Key == "Email_Sender_Address") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Address").Value) : string.Empty;
                password = appSettings.Any(_ => _.Key == "Email_Sender_Password") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "Email_Sender_Password").Value) : string.Empty;
                serverAddress = appSettings.Any(_ => _.Key == "SMTP_Client") ? Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Client").Value) : string.Empty;
                serverPort = appSettings.Any(_ => _.Key == "SMTP_Port") ? int.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_Port").Value)) : 25;
                useSSL = appSettings.Any(_ => _.Key == "SMTP_EnableSsl") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_EnableSsl").Value)) : true;
            }
            catch
            {
            }
            return Json(new
            {
                useAnonymMode,
                senderEmail,
                password,
                serverAddress,
                serverPort,
                useSSL
            }, JsonRequestBehavior.AllowGet);
        }

        public class SMTPParameters
        {
            public bool useAnonymMode { get; set; }
            public string senderEmail { get; set; }
            public string password { get; set; }
            public string serverAddress { get; set; }
            public int serverPort { get; set; }
            public bool useSSL { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult> SaveSmtpParameters(SMTPParameters parameters)
        {
            bool useAnonymMode = parameters.useAnonymMode;
            string senderEmail = parameters.senderEmail;
            string password = parameters.password;
            string serverAddress = parameters.serverAddress;
            int serverPort = parameters.serverPort;
            bool useSSL = parameters.useSSL;

            try
            {
                if (string.IsNullOrEmpty(senderEmail))
                    throw new Exception(LocalizedStrings.GetString("Web_Controller_Notification_SenderEmailCantBeNullOrEmpty"));
                if (string.IsNullOrEmpty(serverAddress))
                    throw new Exception(LocalizedStrings.GetString("Web_Controller_Notification_ServerAddressCantBeNullOrEmpty"));
                if (serverPort == 0)
                    throw new Exception(LocalizedStrings.GetString("Web_Controller_Notification_ServerPortCantBeNullOrEmpty"));
                if (!useAnonymMode && string.IsNullOrEmpty(password))
                    throw new Exception(LocalizedStrings.GetString("Web_Controller_Notification_PasswordCantBeNullOrEmptyWhenNotUsingAnonymMode"));
                if (string.IsNullOrEmpty(password))
                    password = string.Empty;

                var editedSettings = new List<AppSetting>();
                var appSettings = await _prepareService.GetAllAppSettings();

                var editSetting = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode") : new AppSetting { Key = "SMTP_UseAnonymMode", Value = Encoding.UTF8.GetBytes(useAnonymMode.ToString()) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(useAnonymMode.ToString());
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                editSetting = appSettings.Any(_ => _.Key == "Email_Sender_Address") ? appSettings.Single(_ => _.Key == "Email_Sender_Address") : new AppSetting { Key = "Email_Sender_Address", Value = Encoding.UTF8.GetBytes(senderEmail) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(senderEmail);
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                editSetting = appSettings.Any(_ => _.Key == "Email_Sender_Password") ? appSettings.Single(_ => _.Key == "Email_Sender_Password") : new AppSetting { Key = "Email_Sender_Password", Value = Encoding.UTF8.GetBytes(password) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(password);
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                editSetting = appSettings.Any(_ => _.Key == "SMTP_Client") ? appSettings.Single(_ => _.Key == "SMTP_Client") : new AppSetting { Key = "SMTP_Client", Value = Encoding.UTF8.GetBytes(serverAddress) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(serverAddress);
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                editSetting = appSettings.Any(_ => _.Key == "SMTP_Port") ? appSettings.Single(_ => _.Key == "SMTP_Port") : new AppSetting { Key = "SMTP_Port", Value = Encoding.UTF8.GetBytes(serverPort.ToString()) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(serverPort.ToString());
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                editSetting = appSettings.Any(_ => _.Key == "SMTP_EnableSsl") ? appSettings.Single(_ => _.Key == "SMTP_EnableSsl") : new AppSetting { Key = "SMTP_EnableSsl", Value = Encoding.UTF8.GetBytes(useSSL.ToString()) };
                if (!editSetting.IsMarkedAsAdded)
                {
                    editSetting.Value = Encoding.UTF8.GetBytes(useSSL.ToString());
                    editSetting.MarkAsModified();
                }
                editedSettings.Add(editSetting);

                await _prepareService.SaveAppSettings(editedSettings.ToArray());

                HttpContext.Response.StatusDescription = $"{HttpStatusCode.OK}";
                return Content($"{HttpStatusCode.OK}");
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusDescription = $"{HttpStatusCode.InternalServerError}";
                return Content(ex.Message);
            }
        }
    }
}