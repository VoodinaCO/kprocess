using KProcess.KL2.WebAdmin.Mapper;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Notification.Jobs
{
    class CollectReportsJob : IJob
    {
        readonly ISecurityContext _securityContext;
        readonly INotificationService _notificationService;
        readonly IApplicationUsersService _applicationUsersService;
        readonly IPrepareService _prepareService;

        //For render engine
        readonly string bodyTemplatePath = Directory.GetCurrentDirectory() + @"\App_Data\BodyTemplate\";
        readonly string pdfTemplatePath = Directory.GetCurrentDirectory() + @"\App_Data\PdfTemplate\";
        readonly string pdfBase = Directory.GetCurrentDirectory() + @"\App_Data\QtBinaries\";
        readonly string baseUrl = Directory.GetCurrentDirectory() + @"\App_Data\Style\";

        public CollectReportsJob(ISecurityContext securityContext,
            INotificationService notificationService,
            IApplicationUsersService applicationUsersService,
            IPrepareService prepareService)
        {
            _securityContext = securityContext;
            _notificationService = notificationService;
            _applicationUsersService = applicationUsersService;
            _prepareService = prepareService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _securityContext.AutoLogAsDefaultAdmin();

            if (await CollectAndSendInspection())
                Console.WriteLine($"Inspection report for : {DateTime.Today.AddDays(-1).ToShortDateString()} was sent at : {DateTime.Now}");
            else
                Console.WriteLine($"Inspection report for : {DateTime.Today.AddDays(-1).ToShortDateString()} is empty. No inspection report is being sent.");
        }

        // NOT USED
        public async Task<bool> CollectAndSendAudit()
        {
            var auditTypes = await _notificationService.GetNotificationTypes(NotificationCategory.Audit);
            var auditType = auditTypes.FirstOrDefault();
            var audits = await _notificationService.GetAuditReport();
            if (!audits.Any())
                return false;
            var auditsModel = AuditMapper.ToAuditViewModels(audits);

            string pdfString = _notificationService.RenderViewToString(auditsModel, pdfTemplatePath, auditType.NotificationTypeSetting.PdfTemplate);
            _notificationService.RenderViewToPdf(pdfString, pdfBase, baseUrl);
            return true;
        }

        // NOT USED
        public async Task<bool> CollectAndSendEvaluation()
        {
            var evaluationTypes = await _notificationService.GetNotificationTypes(NotificationCategory.Evaluation);
            var evaluationType = evaluationTypes.FirstOrDefault();
            var evaluations = await _notificationService.GetEvaluationReport();
            if (!evaluations.Any())
                return false;
            var (Users, _, _, _) = await _applicationUsersService.GetUsersAndRolesAndLanguages();
            var trainingPublications = await _prepareService.GetTrainingPublications(evaluations.Select(e => e.PublicationId).Distinct().ToArray());
            var evaluationsModel = QualificationMapper.ToQualificationViewModels(evaluations, Users.ToList(), trainingPublications);
            
            //string body = _notificationService.RenderViewToString(evaluationsModel, bodyTemplatePath, evaluationType.NotificationTypeSetting.BodyTemplate);

            string pdfString = _notificationService.RenderViewToString(evaluationsModel, pdfTemplatePath, evaluationType.NotificationTypeSetting.PdfTemplate);
            _notificationService.RenderViewToPdf(pdfString, pdfBase, baseUrl);
            return true;
        }

        public async Task<bool> CollectAndSendInspection()
        {
            try
            {
                var inspectionTypes = await _notificationService.GetNotificationTypes(NotificationCategory.Inspection);
                var inspectionType = inspectionTypes.FirstOrDefault(_ => _.NotificationCategory == NotificationCategory.Inspection);
                if (inspectionType == null)
                    return false;
                var inspections = await _notificationService.GetInspectionReport();
                var anomalies = inspections.SelectMany(i => i.Anomalies).Distinct();
                if (!inspections.Any())
                    return false;
                var inspectionsModel = InspectionMapper.ToInspectionViewModels(inspections);
                var anomaliesModel = InspectionMapper.ToAnomalyViewModels(anomalies);
                var model = (inspectionsViewModel: inspectionsModel, anomaliesViewModel: anomaliesModel);
                List<byte[]> attachments = new List<byte[]>();

                string pdfString = _notificationService.RenderViewToString(model, pdfTemplatePath, inspectionType.NotificationTypeSetting.PdfTemplate);
                var pdfAttachment = _notificationService.RenderViewToPdf(pdfString, pdfBase, baseUrl);
                attachments.Add(pdfAttachment);
                string bodyString = _notificationService.RenderViewToString(model, bodyTemplatePath, inspectionType.NotificationTypeSetting.BodyTemplate);

                await _notificationService.PushNotification(bodyString, inspectionType.Id, attachments, new List<string> { $"I - {DateTime.Today.AddDays(-1).ToString("yyMMdd")}" });
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }   
        }

        // NOT USED
        public async Task<bool> CollectAndSendAnomaly()
        {
            var anomaly = await _prepareService.GetAnomaly(1023);
            if (anomaly == null)
                return false;
            var model = new WebAdmin.Models.Inspection.AnomalyViewModel
            {
                AnomalyId = anomaly.Id,
                Category = anomaly.Category,
                Date = anomaly.Date,
                Description = anomaly.Description,
                HasPhoto = anomaly.Photo != null,
                Photo = anomaly.Photo == null ? string.Empty : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(anomaly.Photo)),
                Label = anomaly.Label,
                Line = anomaly.Line,
                Machine = anomaly.Machine,
                Priority = (int)anomaly.Priority.Value,
                Type = (int)anomaly.Type,
                TypeLabel = anomaly.Type.AnomalyTypeToString(),
                TypeColor = anomaly.Type == AnomalyType.Security ? "green" : anomaly.Type == AnomalyType.Maintenance ? "red" : anomaly.Type == AnomalyType.Operator ? "blue" : "gray"
            };
            model.PriorityLists = new List<Tuple<int, string>>
            {
                new Tuple<int, string>(1, "A"),
                new Tuple<int, string>(2, "B"),
                new Tuple<int, string>(3, "C")
            };

            model.KindItems = new List<IAnomalyKindItem>();
            model.KindItems = Anomalies.GetPossibleAnomalies(anomaly.Type);

            model.Items = new List<WebAdmin.Models.Inspection.AnomalyKindItemViewModel>();
            foreach (var item in model.KindItems)
            {
                if (item.GetType().Name == "AnomalyKindItem")
                {
                    var tempItem = (AnomalyKindItem)item;
                    model.Items.Add(new WebAdmin.Models.Inspection.AnomalyKindItemViewModel
                    {
                        Label = tempItem.Label,
                        Category = tempItem.Category,
                        Number = tempItem.Number
                    });
                }
            }

            string pdfString = _notificationService.RenderViewToString(model, pdfTemplatePath, "PdfAnomaly.cshtml");
            _notificationService.RenderViewToPdf(pdfString, pdfBase, baseUrl);
            return true;
        }
    }
}
