using KProcess.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    public interface INotificationService : IBusinessService
    {
        Task<Notification> PushNotification(string body, int notificationTypeId, List<byte[]> attachments, List<string> attachmentNames = null);

        Task<IEnumerable<Notification>> NotificationProcessed(Notification[] notifications);

        string RenderViewToString<T>(T model, string templatePath, string templateName);

        byte[] RenderViewToPdf(string html, string basePath, string baseUrl);

        /// <summary>
        /// Get all notifications which is not send yet.
        /// </summary>
        Task<IEnumerable<Notification>> GetNotificationNotSend();

        /// <summary>
        /// Get all notification types.
        /// </summary>
        Task<IEnumerable<NotificationType>> GetNotificationTypes(NotificationCategory? notificationCategory = null);

        Task<IEnumerable<Inspection>> GetInspectionReport();

        Task<IEnumerable<Audit>> GetAuditReport();

        Task<IEnumerable<Qualification>> GetEvaluationReport();

        Task<NotificationType> SaveNotificationType(NotificationType notificationType);

        Task<Qualification> SendQualificationReport(int qualificationId);

        Task<IEnumerable<Training>> SendTrainingsReport(List<int> trainingIds);

        Task<Audit> SendAuditReport(int auditId);

        Task<IEnumerable<Anomaly>> SendAnomaliesReport(List<int> anomalyIds);
    }
}
