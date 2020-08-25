using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Quartz;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.KL2.Notification.Jobs
{
    class SendEmailsJob : IJob
    {
        readonly ISecurityContext _securityContext;
        readonly IPrepareService _prepareService;
        readonly INotificationService _notificationService;

        public SendEmailsJob(ISecurityContext securityContext,
            IPrepareService prepareService,
            INotificationService notificationService)
        {
            _securityContext = securityContext;
            _prepareService = prepareService;
            _notificationService = notificationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _securityContext.AutoLogAsDefaultAdmin();

            var notifications = await _notificationService.GetNotificationNotSend();
            if (notifications?.Any() != true)
                return;
            var appSettings = await _prepareService.GetAllAppSettings();
            var useAnonymMode = appSettings.Any(_ => _.Key == "SMTP_UseAnonymMode") ? bool.Parse(Encoding.UTF8.GetString(appSettings.Single(_ => _.Key == "SMTP_UseAnonymMode").Value)) : false;
            ConcurrentBag<Ksmed.Models.Notification> updatedNotifications = new ConcurrentBag<Ksmed.Models.Notification>();
            try
            {
                var basicCredential = new NetworkCredential(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Address").Value), Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Password").Value));
                notifications.AsParallel().ForAll(notification =>
                {
                    using (var smtp = new SmtpClient())
                    using (MailMessage message = new MailMessage())
                    {
                        smtp.EnableSsl = Convert.ToBoolean(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_EnableSsl").Value));
                        smtp.Host = Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_Client").Value);
                        smtp.Port = Convert.ToInt32(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "SMTP_Port").Value));
                        if (!useAnonymMode)
                            smtp.Credentials = basicCredential;

                        MailAddress fromAddr = new MailAddress(Encoding.UTF8.GetString(appSettings.FirstOrDefault(s => s.Key == "Email_Sender_Address").Value));
                        message.From = fromAddr;
                        message.Subject = notification.Subject;
                        message.IsBodyHtml = true;
                        string htmlString = notification.Body;
                        message.Body = htmlString;
                        if (notification.NotificationAttachments?.Any() == true)
                            message.Attachments.AddRange(notification.NotificationAttachments.Select(notificationAttachment =>
                                new Attachment(new MemoryStream(notificationAttachment.Attachment), $"{notificationAttachment.Name.Replace("/", "")}.pdf")));

                        message.To.Add(string.Join(",", notification.RecipientTo.Split(',').Distinct()));

                        if (!string.IsNullOrEmpty(notification.RecipientCc))
                            message.CC.Add(string.Join(",", notification.RecipientCc.Split(',').Distinct()));

                        if (!string.IsNullOrEmpty(notification.RecipientBcc))
                            message.Bcc.Add(string.Join(",", notification.RecipientBcc.Split(',').Distinct()));

                        try
                        {
                            smtp.Send(message);
                            notification.ActualSendingDate = DateTime.Now;
                            notification.IsProcessed = true;
                            notification.MarkAsModified();
                            updatedNotifications.Add(notification);
                            Console.WriteLine($"Email has been sent ! time is : {DateTime.Now}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                });
                if (updatedNotifications.Any())
                    await _notificationService.NotificationProcessed(updatedNotifications.ToArray());
                else
                    Console.WriteLine("No email to be send. Time :" + DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
