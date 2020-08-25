using KProcess.KL2.Notification.ViewModels;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Notification.Jobs
{
    class CreateEmailJob : IJob
    {
        readonly INotificationService _notificationService;

        public CreateEmailJob(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var notificationTypes = await _notificationService.GetNotificationTypes();
                var notification = new Ksmed.Models.Notification();
                var evaluation = notificationTypes.FirstOrDefault(_ => _.NotificationCategory == NotificationCategory.Evaluation);
                
                var model = new EvaluationViewModel{
                    Title = "Hi title",
                    Message = "Hey there, just wanna say hi one more time!"
                };
                string bodyTemplatePath = Directory.GetCurrentDirectory() + @"\BodyTemplates\";
                string pdfTemplatePath = Directory.GetCurrentDirectory() + @"\PdfTemplates\";
                string pdfBase = Directory.GetCurrentDirectory() + @"\QtBinaries\";
                string baseUrl = Directory.GetCurrentDirectory() + @"\Style\";
                string body = _notificationService.RenderViewToString(model, bodyTemplatePath, evaluation.NotificationTypeSetting.BodyTemplate);
                string pdfString = _notificationService.RenderViewToString(model, pdfTemplatePath, "PdfEvaluation.cshtml");
                var pdfAttachment = _notificationService.RenderViewToPdf(pdfString, pdfBase, baseUrl);
                List<byte[]> attachments = new List<byte[]>
                {
                    pdfAttachment
                };
                await _notificationService.PushNotification(body, evaluation.Id, attachments);
                Console.WriteLine("Notification creation succeed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
