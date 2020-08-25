using KProcess.Business;
using KProcess.KL2.Business.Impl.Shared.ViewModel.Anomaly;
using KProcess.KL2.Business.Impl.Shared.ViewModel.Audit;
using KProcess.KL2.Business.Impl.Shared.ViewModel.Evaluation;
using KProcess.KL2.Business.Impl.Shared.ViewModel.Formation;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using MoreLinq;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Westwind.RazorHosting;

namespace KProcess.KL2.Business.Impl.Desktop
{
    public class NotificationService : IBusinessService, INotificationService
    {
        readonly ITraceManager _traceManager;
        readonly ISecurityContext _securityContext;
        readonly ILocalizationManager _localizationManager;

        //For render engine
        readonly string bodyTemplatePath = Directory.GetCurrentDirectory() + @"\App_Data\BodyTemplate\";
        readonly string pdfTemplatePath = Directory.GetCurrentDirectory() + @"\App_Data\PdfTemplate\";
        readonly string pdfBase = Directory.GetCurrentDirectory() + @"\App_Data\QtBinaries\";
        readonly string baseUrl = Directory.GetCurrentDirectory() + @"\App_Data\Style\";

        public NotificationService(
            ISecurityContext securityContext,
            ILocalizationManager localizationManager,
            ITraceManager traceManager)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
        }

        public async Task<Notification> PushNotification(string body, int notificationTypeId, List<byte[]> attachments, List<string> attachmentNames = null)
        {
            Notification newNotification = null;
            var now = DateTime.Now;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var notificationType = await context.NotificationTypes
                    .AsNoTracking()
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .SingleOrDefaultAsync(_ => _.NotificationCategory == (NotificationCategory)notificationTypeId);
                if (notificationType == null)
                    return null;
                newNotification = new Notification
                {
                    NotificationTypeId = notificationType.Id,
                    CreatedAt = now,
                    IsProcessed = false,
                    RecipientTo = notificationType.NotificationTypeSetting.Recipients,
                    RecipientCc = notificationType.NotificationTypeSetting.RecipientCcs,
                    RecipientBcc = notificationType.NotificationTypeSetting.RecipientBccs,
                    ScheduledSendingDate = now,
                    Subject = $"{notificationType.Label} - {now.ToShortDateString()}",
                    Body = body
                };
                if (newNotification != null)
                {
                    context.Notifications.ApplyChanges(newNotification);

                    await context.SaveChangesAsync();
                }
                if (attachments.Count != 0)
                {
                    int count = 0;
                    foreach (var attachment in attachments)
                    {
                        var newAttachment = new NotificationAttachment();
                        if (attachmentNames != null)
                            newAttachment.Name = attachmentNames[count];
                        else
                            newAttachment.Name = newNotification.Subject;
                        newAttachment.Attachment = attachment;
                        newAttachment.NotificationId = newNotification.NotificationId;

                        context.NotificationAttachments.ApplyChanges(newAttachment);
                        await context.SaveChangesAsync();
                        count++;
                    }
                }
            }
            return newNotification;
        }

        public string RenderViewToString<T>(T model, string templatePath, string templateName)
        {
            var host = new RazorFolderHostContainer()
            {
                TemplatePath = templatePath,
                BaseBinaryFolder = Environment.CurrentDirectory
            };
            host.TemplatePath = templatePath;
            host.BaseBinaryFolder = Environment.CurrentDirectory;
            host.AddAssemblyFromType(typeof(T));
            host.UseAppDomain = true;
            host.Start();
            string result = host.RenderTemplate("~/" + templateName, model);
            host.Stop();
            return result;
        }

        public byte[] RenderViewToPdf(string html, string basePath, string baseUrl)
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA3NjkwQDMxMzcyZTMxMmUzMGRpNUF0QThQTklxRFlMeDhXR3NRZFlHcnU4VjN5Z2tGS3FaT2sxM1hER1E9;MTA3NjkxQDMxMzcyZTMxMmUzMFVqQ0tFMGcxclFlNUVKVUJtdWpGcWlmQ1gvbE9PMGxZYWxHTFZYSmxHZDQ9;MTA3NjkyQDMxMzcyZTMxMmUzMEZMTENrQjNCYjUvNmtKTHFNRHBGRkxXVlFXNGZReDVrbWl2WXZsczFORlk9;MTA3NjkzQDMxMzcyZTMxMmUzME5MY3VCaXEydmZyS085RmhzdWtMK2pjNE4wQmY1RUZxMHorMHhHZGlpOFU9;MTA3Njk0QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89;MTA3Njk1QDMxMzcyZTMxMmUzMFM5YmYyTDhoL1N4Z2RIeEZ4S1p6KzF5eGxkdkp2VDNIMFpkZHBwYWVndFE9;MTA3Njk2QDMxMzcyZTMxMmUzMEtFZTNrcnBaelBHUXg4TC9kdGE0TjVQTHNIVVloTDZ3N1I4VHQ0NysvTEE9;MTA3Njk3QDMxMzcyZTMxMmUzMExlamxrNmhmL0RyR3Y2QzBoYitxOXhyazRnR0JZUUJBZThJZmhENG1oVGs9;MTA3Njk4QDMxMzcyZTMxMmUzMGRrTFo4clJIR29kZEZrdlRSUlFsQ3VvckJGRjNiZCtscFNYbE0zRThQSGc9;MTA3Njk5QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89");

            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);
            WebKitConverterSettings settings = new WebKitConverterSettings
            {
                WebKitPath = basePath,
                SinglePageLayout = Syncfusion.Pdf.HtmlToPdf.SinglePageLayout.FitWidth
            };
            htmlConverter.ConverterSettings = settings;
            
            PdfDocument document = htmlConverter.Convert(html, baseUrl);
            MemoryStream memoryStream = new MemoryStream();
            document.Save(memoryStream);
            
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            document.Close();
            return bytes;
        }

        /// <summary>
        /// Get all notifications which is not send yet.
        /// </summary>
        public async Task<IEnumerable<Notification>> GetNotificationNotSend()
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var notifications = await context.Notifications
                    .Include(nameof(Notification.NotificationType))
                    .Include($"{nameof(Notification.NotificationType)}.{nameof(NotificationType.NotificationTypeSetting)}")
                    .Include(nameof(Notification.NotificationAttachments))
                    .Where(_ => _.IsProcessed != true)
                    .ToArrayAsync();
                return notifications;
            }
        }

        /// <summary>
        /// Get all notification types.
        /// </summary>
        public async Task<IEnumerable<NotificationType>> GetNotificationTypes(NotificationCategory? notificationCategory = null)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (notificationCategory == null)
                    return await context.NotificationTypes
                        .Include(nameof(NotificationType.NotificationTypeSetting))
                        .ToArrayAsync();
                return await context.NotificationTypes
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .Where(_ => _.NotificationCategory == notificationCategory)
                    .ToArrayAsync();
            }
        }

        public async Task<IEnumerable<Notification>> NotificationProcessed(Notification[] notifications)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (notifications?.Any() == true)
                {
                    foreach (var notification in notifications)
                        context.Notifications.ApplyChanges(notification);
                    await context.SaveChangesAsync();
                }
            }
            return notifications;
        }

        public async Task<IEnumerable<Inspection>> GetInspectionReport()
        {
            // Récupère les inspections de la veille
            var today = DateTime.Now.Date.AddHours(7);
            var yesterday = today.AddDays(-1);

            // Récupère les inspections du jour (POUR TEST)
            /*var today = DateTime.Now.Date.AddDays(1).AddHours(7);
            var yesterday = today.AddDays(-1);*/

            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Inspections
                    .Include(nameof(Inspection.InspectionSteps))
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}.{nameof(User.Teams)}")
                    .Include($"{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")
                    .Include(nameof(Inspection.Publication))
                    .Include(nameof(Inspection.Audits))
                    .Include(nameof(Inspection.Anomalies))
                    .Include($"{nameof(Inspection.Anomalies)}.{ nameof(Anomaly.Inspection)}")
                    .Include($"{nameof(Inspection.Anomalies)}.{ nameof(Anomaly.Inspection)}.{ nameof(Inspection.Publication)}")
                    .Include($"{nameof(Inspection.Anomalies)}.{ nameof(Anomaly.Inspection)}.{ nameof(Inspection.Publication)}.{ nameof(Publication.Process)}")
                    .Include($"{nameof(Inspection.Publication)}.{ nameof(Publication.Process)}")
                    .Where(_ => !_.IsDeleted
                                && ((yesterday <= _.StartDate && _.StartDate <= today)
                                   || (_.EndDate != null && yesterday <= _.EndDate && _.EndDate <= today)))
                    .ToArrayAsync();
            }   
        }

        public async Task<IEnumerable<Audit>> GetAuditReport()
        {
            var today = DateTime.Now.Date.AddHours(7);
            var yesterday = today.AddDays(-1);
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Audits
                    .Include(nameof(Audit.AuditItems))
                    .Include(nameof(Audit.Survey))
                    .Include(nameof(Audit.Inspection))
                    .Include($"{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                    .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Anomalies)}")
                    .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}")
                    .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                    .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}")
                    .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}.{nameof(Publication.Process)}")
                    .Include(nameof(Audit.Auditor))
                    .Include($"{nameof(Audit.Auditor)}.{nameof(User.Teams)}")
                    .Where(_ => _.EndDate != null && yesterday <= _.EndDate && _.EndDate <= today)
                    .ToArrayAsync();
            }   
        }

        public async Task<IEnumerable<Qualification>> GetEvaluationReport()
        {
            var today = DateTime.Now.Date.AddHours(7);
            var yesterday = today.AddDays(-1);
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                return await context.Qualifications
                    .Include(nameof(Qualification.Publication))
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Process)}")
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Trainings)}")
                    .Include(nameof(Qualification.User))
                    .Include($"{nameof(Qualification.User)}.{nameof(User.Teams)}")
                    .Include(nameof(Qualification.QualificationSteps))
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}.{nameof(User.Roles)}")
                    .Where(_ => _.EndDate != null && yesterday <= _.EndDate && _.EndDate <= today)
                    .ToArrayAsync();
            }
        }

        public async Task<NotificationType> SaveNotificationType(NotificationType notificationType)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                if (notificationType != null)
                {
                    context.NotificationTypes.ApplyChanges(notificationType);

                    await context.SaveChangesAsync();
                }
            }
            return notificationType;
        }

        public async Task<IEnumerable<Training>> SendTrainingsReport(List<int> trainingIds)
        {
            var trainings = new List<Training>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var notificationType = await context.NotificationTypes
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .FirstOrDefaultAsync(_ => _.NotificationCategory == NotificationCategory.Formation);
                if (notificationType?.NotificationTypeSetting.Recipients == null)
                {
                    //Report can't be created without recipient address
                    return trainings;
                }
                //collect attachments
                List<byte[]> attachments = new List<byte[]>();
                List<string> attachmentsNames = new List<string>();
                foreach (var trainingId in trainingIds)
                {
                    var training = await context.Trainings
                        .Include(nameof(Training.Publication))
                        .Include($"{nameof(Training.Publication)}.{nameof(Publication.Process)}")
                        .Include(nameof(Training.User))
                        .Include($"{nameof(Training.User)}.{nameof(User.Teams)}")
                        .Include(nameof(Training.ValidationTrainings))
                        .Include($"{nameof(Training.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                        .SingleOrDefaultAsync(_ => _.TrainingId == trainingId);
                    if (training == null)
                        continue;
                    trainings.Add(training);

                    var model = ToFormationReportViewModel(training);

                    string pdfString = RenderViewToString(model, pdfTemplatePath, notificationType.NotificationTypeSetting.PdfTemplate);
                    var pdfAttachment = RenderViewToPdf(pdfString, pdfBase, baseUrl);
                    attachments.Add(pdfAttachment);
                    // T - YYMMDD - process(version) - Trainee
                    var fileName = $"T - {DateTime.Today.ToString("yyMMdd")} - {training.Publication.Process.Label} (v{training.Publication.Version}) - {training.User.FullName}";
                    foreach (char c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c.ToString(), "_");
                    fileName = fileName.Substring(0, Math.Min(fileName.Length, 100));
                    attachmentsNames.Add(fileName);
                }
                string bodyString = RenderViewToString(new AnomalyReportViewModel(), bodyTemplatePath, notificationType.NotificationTypeSetting.BodyTemplate);

                await PushNotification(bodyString, notificationType.Id, attachments, attachmentsNames);

                return trainings;
            }
        }

        public async Task<Qualification> SendQualificationReport(int qualificationId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var evaluationType = await context.NotificationTypes
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .FirstOrDefaultAsync(_ => _.NotificationCategory == NotificationCategory.Evaluation);
                if (evaluationType?.NotificationTypeSetting.Recipients == null)
                {
                    //Report can't be created without recipient address
                    return null;
                }
                var qualification = await context.Qualifications
                    .Include(nameof(Qualification.User))
                    .Include(nameof(Qualification.QualificationSteps))
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.PublishedAction)}")
                    .Include($"{nameof(Qualification.QualificationSteps)}.{nameof(QualificationStep.User)}")
                    .Include(nameof(Qualification.Publication))
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.Process)}")
                    .Include($"{nameof(Qualification.Publication)}.{nameof(Publication.PublishedActions)}")
                    .FirstOrDefaultAsync(_ => _.QualificationId == qualificationId);
                if (qualification == null)
                    return null;

                var model = await ToEvaluationReportViewModel(qualification);
                
                string pdfString = RenderViewToString(model, pdfTemplatePath, evaluationType.NotificationTypeSetting.PdfTemplate);
                List<byte[]> attachments = new List<byte[]>
                {
                    RenderViewToPdf(pdfString, pdfBase, baseUrl)
                };
                // E - YYMMDD - process(version) - Trainee - result (OK ou NOK)
                var fileName = $"E - {DateTime.Today.ToString("yyMMdd")} - {qualification.Publication.Process.Label} (v{qualification.Publication.Version}) - {qualification.User.FullName} - {(qualification.IsQualified == true ? "OK" : "NOK")}";
                foreach (char c in Path.GetInvalidFileNameChars())
                    fileName = fileName.Replace(c.ToString(), "_");
                fileName = fileName.Substring(0, Math.Min(fileName.Length, 100));
                List<string> attachmentsNames = new List<string> { fileName };
                string bodyString = RenderViewToString(model, bodyTemplatePath, evaluationType.NotificationTypeSetting.BodyTemplate);

                await PushNotification(bodyString, evaluationType.Id, attachments, attachmentsNames);

                return qualification;
            }
        }

        public async Task<Audit> SendAuditReport(int auditId)
        {
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var auditType = await context.NotificationTypes
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .FirstOrDefaultAsync(_ => _.NotificationCategory == NotificationCategory.Audit);
                if (auditType?.NotificationTypeSetting.Recipients == null)
                {
                    //Report can't be created without recipient address
                    return null;
                }
                var audit = await context.Audits
                        .Include(nameof(Audit.AuditItems))
                        .Include(nameof(Audit.Survey))
                        .Include(nameof(Audit.Inspection))
                        .Include($"{nameof(Audit.Survey)}.{nameof(Survey.SurveyItems)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Anomalies)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.InspectionSteps)}.{nameof(InspectionStep.Inspector)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}")
                        .Include($"{nameof(Audit.Inspection)}.{nameof(Inspection.Publication)}.{nameof(Publication.Process)}")
                        .Include(nameof(Audit.Auditor))
                        .Include($"{nameof(Audit.Auditor)}.{nameof(User.Teams)}")
                        .FirstOrDefaultAsync(_ => _.Id == auditId);
                if (audit == null)
                    return null;

                var model = ToAuditReportViewModel(audit);
                
                string pdfString = RenderViewToString(model, pdfTemplatePath, auditType.NotificationTypeSetting.PdfTemplate);
                List<byte[]> attachments = new List<byte[]>
                {
                    RenderViewToPdf(pdfString, pdfBase, baseUrl)
                };
                // Au - YYMMDD - process(version) - Survey - Auditor - Nb NOK)
                var fileName = $"Au - {DateTime.Today.ToString("yyMMdd")} - {audit.Inspection.Publication.Process.Label} (v{audit.Inspection.Publication.Version}) - {audit.Survey.Name} - {audit.Auditor.FullName} - {audit.AuditItems.Count(ai => ai.IsOk == false)} NOK";
                foreach (char c in Path.GetInvalidFileNameChars())
                    fileName = fileName.Replace(c.ToString(), "_");
                fileName = fileName.Substring(0, Math.Min(fileName.Length, 100));
                List<string> attachmentsNames = new List<string> { fileName };
                string bodyString = RenderViewToString(model, bodyTemplatePath, auditType.NotificationTypeSetting.BodyTemplate);

                await PushNotification(bodyString, auditType.Id, attachments, attachmentsNames);

                return audit;
            }
        }

        public async Task<IEnumerable<Anomaly>> SendAnomaliesReport(List<int> anomalyIds)
        {
            var anomalies = new List<Anomaly>();
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                var anomalyType = await context.NotificationTypes
                    .Include(nameof(NotificationType.NotificationTypeSetting))
                    .FirstOrDefaultAsync(_ => _.NotificationCategory == NotificationCategory.Anomaly);
                if (anomalyType?.NotificationTypeSetting.Recipients == null)
                {
                    //Report can't be created without recipient address
                    return anomalies;
                }
                //collect attachments
                List<byte[]> attachments = new List<byte[]>();
                List<string> attachmentNames = new List<string>();
                foreach (var anomalyId in anomalyIds)
                {
                    var anomaly = await context.Anomalies
                        .Include(nameof(Anomaly.Inspection))
                        .Include($"{nameof(Anomaly.Inspection)}.{nameof(Inspection.Publication)}")
                        .Include($"{nameof(Anomaly.Inspection)}.{nameof(Inspection.Publication)}.{nameof(Publication.Process)}")
                        .Include(nameof(Anomaly.Inspector))
                        .Include(nameof(Anomaly.InspectionSteps))
                        .Include($"{nameof(Anomaly.InspectionSteps)}.{nameof(InspectionStep.PublishedAction)}")
                        .SingleOrDefaultAsync(_ => _.Id == anomalyId);
                    if (anomaly == null)
                        continue;
                    anomalies.Add(anomaly);

                    var model = ToAnomalyReportViewModel(anomaly);
                    var taskAnomaly = anomaly.InspectionSteps.FirstOrDefault();
                    string anomalyFrontLabel = $"Anomalie tâche {(taskAnomaly == null ? "HV" : $"tâche {taskAnomaly.PublishedAction.Label}")}";
                    string inspectorName = anomaly.Inspector.FullName;
                    string inspectionName = anomaly.Inspection.Publication.Process.Label
                        .Substring(0, Math.Min(15, anomaly.Inspection.Publication.Process.Label.Length));
                    string anomalyDate = anomaly.Date.ToShortDateString();
                    // A - YYMMDD - (Ligne - Machine) - process(version) - Task ID (- Task) /HV - Inspector
                    var stringBuilder = new StringBuilder($"A - {DateTime.Today.ToString("yyMMdd")} - "); // A - YYMMDD - 
                    if (!string.IsNullOrEmpty(anomaly.Line) && !string.IsNullOrEmpty(anomaly.Machine)) // (Ligne-Machine) - 
                        stringBuilder.Append($"{anomaly.Line}-{anomaly.Machine} - ");
                    else if (!string.IsNullOrEmpty(anomaly.Line)) // (Ligne) - 
                        stringBuilder.Append($"{anomaly.Line} - ");
                    else if (!string.IsNullOrEmpty(anomaly.Machine)) // (Machine) - 
                        stringBuilder.Append($"{anomaly.Machine} - ");
                    stringBuilder.Append($"{anomaly.Inspection.Publication.Process.Label} (v{anomaly.Inspection.Publication.Version}) - "); // process (version) - 
                    stringBuilder.Append($"{(taskAnomaly == null ? "HV" : $"{taskAnomaly.PublishedAction.WBS} - {taskAnomaly.PublishedAction.Label}")} - "); // HV or TaskID - Task - 
                    stringBuilder.Append(inspectorName); // Inspector
                    string fileName = stringBuilder.ToString();
                    foreach (char c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c.ToString(), "_");
                    fileName = fileName.Substring(0, Math.Min(fileName.Length, 100));

                    string pdfString = RenderViewToString(model, pdfTemplatePath, anomalyType.NotificationTypeSetting.PdfTemplate);
                    var pdfAttachment = RenderViewToPdf(pdfString, pdfBase, baseUrl);
                    attachments.Add(pdfAttachment);
                    attachmentNames.Add(fileName);
                }
                string bodyString = RenderViewToString(new AnomalyReportViewModel(), bodyTemplatePath, anomalyType.NotificationTypeSetting.BodyTemplate);

                await PushNotification(bodyString, anomalyType.Id, attachments, attachmentNames);

                return anomalies;
            }   
        }

        public AuditReportViewModel ToAuditReportViewModel(Audit audit)
        {
            var auditee = audit.Inspection.InspectionSteps
                .Where(iS => !iS.IsDeleted)
                .MaxBy(iS => iS.Date)
                .Select(iS => iS.Inspector)
                .FirstOrDefault();
            return new AuditReportViewModel
            {
                ProcessLabel = audit.Inspection.Publication.Process.Label,
                Teams = audit.Auditor.Teams
                    .Select(t => t.Name)
                    .ToList(),
                AuditorName = audit.Auditor.FullName,
                AuditeeName = auditee?.FullName,
                AuditeeHasTenured = auditee.Tenured.HasValue,
                AuditeeTenured = auditee.Tenured ?? false,
                AuditItemsViewModel = audit.AuditItems
                    .OrderBy(ai => ai.Number)
                    .Select(ai => new AuditItemViewModel
                    {
                        Question = audit.Survey.SurveyItems
                            .FirstOrDefault(s => s.Number == ai.Number)
                            ?.Query,
                        HasResult = ai.IsOk.HasValue,
                        Result = ai.IsOk ?? false,
                        Comment = ai.Comment
                    }).ToList(),
                AnomaliesViewModel = audit.Inspection.Anomalies
                    .Select(a => new AnomalyViewModel
                    {
                        TypeLabel = a.Type.AnomalyTypeToStringReport(),
                        TypeColor = a.Type == AnomalyType.Security
                        ? "#5cb85c"
                        : a.Type == AnomalyType.Maintenance
                            ? "#d9534f"
                            : a.Type == AnomalyType.Operator
                                ? "lightskyblue"
                                : "grey",
                        Comment = a.Description,
                        HasPhoto = a.Photo != null,
                        Photo = a.Photo == null
                            ? string.Empty
                            : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(a.Photo))
                    }).ToList()
            };
        }

        public async Task<EvaluationReportViewModel> ToEvaluationReportViewModel(Qualification qualification)
        {
            var qualifiers = qualification.QualificationSteps
                .Where(qs => !qs.IsDeleted)
                .OrderBy(qs => qs.Date)
                .Select(qs => qs.User.FullName)
                .Distinct()
                .ToList();
            Publication trainingPublication = null;
            using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            {
                trainingPublication = await context.Publications
                    .Include(nameof(Publication.Trainings))
                    .Include($"{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}")
                    .Include($"{nameof(Publication.Trainings)}.{nameof(Training.ValidationTrainings)}.{nameof(ValidationTraining.User)}")
                    .Where(tp => tp.PublishedDate == qualification.Publication.PublishedDate
                                 && tp.PublishMode == PublishModeEnum.Formation)
                    .SingleAsync();
            }
            var referentName = trainingPublication.Trainings
                .SingleOrDefault(t => !t.IsDeleted && t.UserId == qualification.UserId)
                ?.ValidationTrainings
                .Where(vt => !vt.IsDeleted)
                .MaxBy(vt => vt.EndDate)
                .FirstOrDefault()
                ?.User
                ?.FullName;
            var model = new EvaluationReportViewModel
            {
                ProcessLabel = qualification.Publication.Process.Label,
                EvaluatedName = qualification.User.FullName,
                EvaluatedTenured = qualification.User.Tenured.HasValue
                    ? qualification.User.Tenured.Value
                        ? "Titulaire"
                        : "Intérimaire"
                    : string.Empty,
                Teams = qualification.User.Teams
                    .Select(t => t.Name)
                    .ToList(),
                StartDate = qualification.StartDate,
                EndDate = qualification.EndDate.Value,
                QualifiersName = qualifiers,
                Result = qualification.Result.HasValue
                    ? $"{qualification.Result.Value}%"
                    : string.Empty,
                Decision = qualification.IsQualified.HasValue
                    ? qualification.IsQualified.Value
                        ? "Passé"
                        : "Échoué"
                    : string.Empty,
                Comment = qualification.Comment,
                ReferentName = referentName,
                Observations = qualification.QualificationSteps
                    .Where(qs => !qs.IsDeleted && qs.IsQualified == false)
                    .Select(qs => new ObservationViewModel
                    {
                        WBS = qs.PublishedAction.WBS,
                        Action = qs.PublishedAction.Label,
                        Question = qs.PublishedAction.CustomTextValue2,
                        Comment = qs.Comment
                    }).ToList()
            };

            return model;
        }

        public FormationReportViewModel ToFormationReportViewModel(Training training)
        {
            var trainers = training.ValidationTrainings
                    .Where(vt => !vt.IsDeleted)
                    .OrderBy(vt => vt.EndDate)
                    .Select(vt => vt.User.FullName)
                    .Distinct()
                    .ToList();
            var referent = training.ValidationTrainings
                    .Where(vt => !vt.IsDeleted)
                    .MaxBy(vt => vt.EndDate)
                    .Select(vt => vt.User.FullName)
                    .FirstOrDefault();
            return new FormationReportViewModel
            {
                ProcessLabel = training.Publication.Process.Label,
                TrainedName = training.User.FullName,
                TrainedTenured = training.User.Tenured.HasValue
                    ? training.User.Tenured.Value
                        ? "Titulaire"
                        : "Intérimaire"
                    : string.Empty,
                Teams = training.User.Teams
                    .Select(t => t.Name)
                    .ToList(),
                StartDate = training.StartDate,
                EndDate = training.EndDate.Value,
                TrainersName = trainers,
                ReferentName = referent
            };
        }

        public AnomalyReportViewModel ToAnomalyReportViewModel(Anomaly anomaly)
        {
            var kindItems = Anomalies.GetPossibleAnomalies(anomaly.Type);
            return new AnomalyReportViewModel
            {
                AnomalyId = anomaly.Id,
                Category = anomaly.Category,
                Date = anomaly.Date,
                Description = anomaly.Description,
                HasPhoto = anomaly.Photo != null,
                Photo = anomaly.Photo == null
                    ? string.Empty
                    : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(anomaly.Photo)),
                Label = anomaly.Label,
                Line = anomaly.Line,
                Machine = anomaly.Machine,
                Priority = (int)anomaly.Priority.Value,
                Type = (int)anomaly.Type,
                TypeLabel = anomaly.Type.AnomalyTypeToString(),
                TypeColor = anomaly.Type == AnomalyType.Security
                    ? "green"
                    : anomaly.Type == AnomalyType.Maintenance
                        ? "red"
                        : anomaly.Type == AnomalyType.Operator
                            ? "blue"
                            : "gray",
                PriorityLists = new List<Tuple<int, string>>
                {
                    new Tuple<int, string>(1, "A"),
                    new Tuple<int, string>(2, "B"),
                    new Tuple<int, string>(3, "C")
                },
                KindItems = kindItems,
                Items = kindItems
                    .Where(ki => ki is AnomalyKindItem)
                    .Select(ki => new AnomalyKindItemViewModel
                    {
                        Label = ((AnomalyKindItem)ki).Label,
                        Category = ((AnomalyKindItem)ki).Category,
                        Number = ((AnomalyKindItem)ki).Number
                    }).ToList()
            };
        }
    }
}
