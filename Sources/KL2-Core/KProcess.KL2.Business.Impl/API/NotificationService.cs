using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    public class NotificationService : IBusinessService, INotificationService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public NotificationService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        public async Task<Notification> PushNotification(string body, int notificationTypeId, List<byte[]> attachments, List<string> attachmentNames = null)
        {
            dynamic param = new ExpandoObject();
            param.notificationTypeId = notificationTypeId;
            param.body = body;
            param.attachments = attachments;
            param.attachmentNames = attachmentNames;
            return await _apiHttpClient.ServiceAsync<Notification>(KL2_Server.API, nameof(NotificationService), nameof(PushNotification), param);
        }

        public string RenderViewToString<T>(T model, string templatePath, string templateName)
        {
            dynamic param = new ExpandoObject();
            param.model = model;
            param.templatePath = templatePath;
            param.templateName = templateName;
            return _apiHttpClient.ServiceAsync<string>(KL2_Server.API, nameof(NotificationService), nameof(RenderViewToString), param);
        }

        public byte[] RenderViewToPdf(string html, string basePath, string baseUrl)
        {
            dynamic param = new ExpandoObject();
            param.html = html;
            param.basePath = basePath;
            param.baseUrl = baseUrl;
            return _apiHttpClient.ServiceAsync<byte[]>(KL2_Server.API, nameof(NotificationService), nameof(RenderViewToPdf), param);
        }



        /// <summary>
        /// Get all notification types.
        /// </summary>
        public async Task<IEnumerable<NotificationType>> GetNotificationTypes(NotificationCategory? notificationTypeId = null)
        {
            dynamic param = new ExpandoObject();
            param.notificationTypeId = notificationTypeId;
            return await _apiHttpClient.ServiceAsync<IEnumerable<NotificationType>>(KL2_Server.API, nameof(NotificationService), nameof(GetNotificationTypes), param);
        }



        /// <summary>
        /// Get all notifications which is not send yet.
        /// </summary>
        public async Task<IEnumerable<Notification>> GetNotificationNotSend()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Notification>>(KL2_Server.API, nameof(NotificationService), nameof(GetNotificationNotSend), param);
        }

        public async Task<IEnumerable<Notification>> NotificationProcessed(Notification[] notifications)
        {
            dynamic param = new ExpandoObject();
            param.notifications = notifications;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Notification>>(KL2_Server.API, nameof(NotificationService), nameof(NotificationProcessed), param);
        }

        public async Task<IEnumerable<Inspection>> GetInspectionReport()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Inspection>>(KL2_Server.API, nameof(NotificationService), nameof(GetInspectionReport), param);
        }

        public async Task<IEnumerable<Audit>> GetAuditReport()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Audit>>(KL2_Server.API, nameof(NotificationService), nameof(GetAuditReport), param);
        }

        public async Task<IEnumerable<Qualification>> GetEvaluationReport()
        {
            dynamic param = new ExpandoObject();
            return await _apiHttpClient.ServiceAsync<IEnumerable<Qualification>>(KL2_Server.API, nameof(NotificationService), nameof(GetEvaluationReport), param);
        }

        public async Task<NotificationType> SaveNotificationType(NotificationType notificationType)
        {
            dynamic param = new ExpandoObject();
            param.notificationType = notificationType;
            return await _apiHttpClient.ServiceAsync<NotificationType>(KL2_Server.API, nameof(NotificationService), nameof(SaveNotificationType), param);
        }

        public async Task<Qualification> SendQualificationReport(int qualificationId)
        {
            dynamic param = new ExpandoObject();
            param.qualificationId = qualificationId;
            return await _apiHttpClient.ServiceAsync<Qualification>(KL2_Server.API, nameof(NotificationService), nameof(SendQualificationReport), param);
        }

        public async Task<IEnumerable<Training>> SendTrainingsReport(List<int> trainingIds)
        {
            dynamic param = new ExpandoObject();
            param.trainingIds = trainingIds;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Training>>(KL2_Server.API, nameof(NotificationService), nameof(SendTrainingsReport), param);
        }

        public async Task<Audit> SendAuditReport(int auditId)
        {
            dynamic param = new ExpandoObject();
            param.auditId = auditId;
            return await _apiHttpClient.ServiceAsync<Audit>(KL2_Server.API, nameof(NotificationService), nameof(SendAuditReport), param);
        }

        public async Task<IEnumerable<Anomaly>> SendAnomaliesReport(List<int> anomalyIds)
        {
            dynamic param = new ExpandoObject();
            param.anomalyIds = anomalyIds;
            return await _apiHttpClient.ServiceAsync<IEnumerable<Anomaly>>(KL2_Server.API, nameof(NotificationService), nameof(SendAnomaliesReport), param);
        }
    }
}
