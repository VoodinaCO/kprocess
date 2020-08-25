using KProcess.KL2.WebAdmin.Models.Audit;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class AuditMapper
    {
        public static AuditManageViewModel ToAuditManageViewModel(IEnumerable<Audit> audits)
        {
            var auditManageViewModel = new AuditManageViewModel
            {
                Audits = audits.Select(a => new AuditViewModel
                {
                    AuditId = a.Id,
                    AuditorId = a.AuditorId,
                    AuditeeId = a.Inspection.InspectionSteps.Select(s => s.InspectorId).FirstOrDefault(),
                    AuditorName = a.Auditor.FullName,
                    AuditeeName = a.Inspection.InspectionSteps.Select(s => s.Inspector.FullName).FirstOrDefault(),
                    AuditorTeams = a.Auditor.Teams.Select(t => t.Name).ToList(),
                    //AnomalyNumber = a.Inspection.Anomalies.Count(anomalie => anomalie.Origin == AnomalyOrigin.Audit),
                    AnomalyNumber = a.AuditItems.Where(i => i.IsOk == false).Count(),
                    SurveyId = a.SurveyId,
                    SurveyName = a.Survey.Name,
                    StartDate = a.StartDate.Date,
                    EndDate = a.EndDate.HasValue ? a.EndDate.Value.Date : a.EndDate,
                    IsOK = a.AuditItems.All(i => i.IsOk == true),
                    InspectionId = a.InspectionId,
                    ProcessName = $"{a.Inspection.Publication.Process.Label} (v{a.Inspection.Publication.Version})",
                    AuditItems = a.AuditItems.Select(i => new AuditItemViewModel
                    {
                        AuditId = i.AuditId,
                        Number = i.Number,
                        IsOK = i.IsOk,
                        Comment = i.Comment,
                        HasPhoto = i.Photo != null,
                        Photo = i.Photo == null ? null : String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(i.Photo)),
                        RawPhoto = i.Photo == null ? "" : Convert.ToBase64String(i.Photo)
                    }).ToList()
                })
            };
            return auditManageViewModel;
        }
        
        public static AuditViewModel ToAuditViewModel(Audit audit)
        {
            var model = new AuditViewModel {
                AuditId = audit.Id,
                AuditorId = audit.AuditorId,
                AuditeeId = audit.Inspection.InspectionSteps.Select(s => s.InspectorId).FirstOrDefault(),
                AuditorName = audit.Auditor.FullName,
                AuditeeName = audit.Inspection.InspectionSteps.Select(s => s.Inspector.FullName).FirstOrDefault(),
                AuditorTeams = audit.Auditor.Teams.Select(t => t.Name).ToList(),
                //AnomalyNumber = audit.Inspection.Anomalies.Count(anomalie => anomalie.Origin == AnomalyOrigin.Audit),
                AnomalyNumber = audit.AuditItems.Where(i => i.IsOk == false).Count(),
                SurveyId = audit.SurveyId,
                SurveyName = audit.Survey.Name,
                StartDate = audit.StartDate.Date,
                EndDate = audit.EndDate.HasValue ? audit.EndDate.Value.Date : audit.EndDate,
                IsOK = audit.AuditItems.All(i => i.IsOk == true),
                InspectionId = audit.InspectionId,
                ProcessName = $"{audit.Inspection.Publication.Process.Label} (v{audit.Inspection.Publication.Version})",
                AuditItems = audit.AuditItems.Select(i => new AuditItemViewModel
                {
                    AuditId = i.AuditId,
                    Number = i.Number,
                    Question = audit.Survey.SurveyItems.FirstOrDefault(s => s.Number == i.Number).Query,
                    IsOK = i.IsOk,
                    Comment = i.Comment,
                    HasPhoto = i.Photo != null,
                    Photo = i.Photo == null ? null : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(i.Photo)),
                    RawPhoto = i.Photo == null ? "" : Convert.ToBase64String(i.Photo)
                }).ToList()
            };

            return model;
        }

        public static List<AuditViewModel> ToAuditViewModels(IEnumerable<Audit> audits)
        {
            var model = new List<AuditViewModel>();
            foreach (var audit in audits)
            {
                model.Add(new AuditViewModel {
                    AuditId = audit.Id,
                    AuditorId = audit.AuditorId,
                    AuditeeId = audit.Inspection.InspectionSteps.Select(s => s.InspectorId).FirstOrDefault(),
                    AuditorName = audit.Auditor.FullName,
                    AuditeeName = audit.Inspection.InspectionSteps.Select(s => s.Inspector.FullName).FirstOrDefault(),
                    AuditorTeams = audit.Auditor.Teams.Select(t => t.Name).ToList(),
                    AnomalyNumber = audit.Inspection.Anomalies.Count(anomalie => anomalie.Origin == AnomalyOrigin.Audit),
                    SurveyId = audit.SurveyId,
                    SurveyName = audit.Survey.Name,
                    StartDate = audit.StartDate.Date,
                    EndDate = audit.EndDate.HasValue ? audit.EndDate.Value.Date : audit.EndDate,
                    IsOK = audit.AuditItems.All(i => i.IsOk == true),
                    InspectionId = audit.InspectionId,
                    ProcessName = $"{audit.Inspection.Publication.Process.Label} (v{audit.Inspection.Publication.Version})"
                });
            }
            return model;
        }
    }
}