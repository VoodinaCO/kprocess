using KProcess.KL2.WebAdmin.Models.Inspection;
using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.WebAdmin.Mapper
{
    public static class InspectionMapper
    {
        public async static Task<InspectionManageViewModel> ToInspectionManageViewModel(
            IEnumerable<Inspection> inspections
            )
        {
            var inspectionManageViewModel = new InspectionManageViewModel();
            var inspectionsFiltered = new List<Inspection>();
            var userPools = await LicenseMapper.GetUserPools();
            inspectionsFiltered = inspections.Where(u => u.EndDate != null).OrderByDescending(s => s.EndDate).ToList();
            inspectionManageViewModel.Inspections = inspectionsFiltered.Select(u => new InspectionViewModel
            {
                InspectionId = u.InspectionId,
                StartDate = u.StartDate,
                EndDate = u.EndDate,
                Date = u.EndDate.Value,
                PublicationId = u.PublicationId,
                ProcessName = $"{u.Publication.Process.Label} (v{u.Publication.Version})",
                IsOK = !u.InspectionSteps.Any(s => s.IsOk == false),
                AnomalyNumber = u.InspectionSteps.Count(s => s.IsOk.HasValue && !s.IsOk.Value),
                Teams = u.InspectionSteps.SelectMany(s => s.Inspector.Teams.Select(t => t.Name)).Distinct().ToList(),
                Inspectors = u.InspectionSteps.Select(s => s.Inspector.FullName).Distinct().ToList(),
                DoneByDeactivated = !u.InspectionSteps.Select(s => s.Inspector.UserId).Distinct().ToList().Any(i => userPools.Any(p => p == i)),
                Steps = u.InspectionSteps.Select(s => new InspectionStepViewModel
                {
                    Id = s.InspectionStepId,
                    Wbs = s.PublishedAction.WBS,
                    Date = s.Date,
                    InspectionDate = s.Date.ToShortDateString(),
                    InspectorId = s.InspectorId,
                    Inspector = s.Inspector.FullName,
                    ActionLabel = s.PublishedAction.Label,
                    Teams = s.Inspector.Teams.Select(t => t.Name).ToList(),
                    IsOK = s.IsOk,
                    ThumbnailHash = s.PublishedAction.Thumbnail?.Hash,
                    ThumbnailExt = s.PublishedAction.Thumbnail?.Extension,
                    Action = new Models.Action.GenericActionViewModel
                    {
                        ActionId = s.PublishedAction.PublishedActionId,
                        Label = s.PublishedAction.Label
                    }
                }).ToList()
            });
            return inspectionManageViewModel;
        }
        public async static Task<InspectionManageViewModel> ToInspectionManageViewModel(
            IEnumerable<Inspection> inspections,
            string team,
            string userId,
            string publicationId
            )
        {
            var inspectionManageViewModel = new InspectionManageViewModel();
            var inspectionsFiltered = new List<Inspection>();
            var userPools = await LicenseMapper.GetUserPools();
            int teamId = team == "0" ? 0 : Convert.ToInt16(team);
            int uid = userId == "0" ? 0 : Convert.ToInt16(userId);
            Guid pid = publicationId == "0" ? new Guid() : new Guid(publicationId);

            inspectionsFiltered = inspections
                    .Where(u =>
                    (u.EndDate != null)
                    &&
                    (team == "0" || u.InspectionSteps.Any(s => s.Inspector.Teams.Any(t => t.Id == teamId)))
                    &&
                    (userId == "0" || u.InspectionSteps.Any(s => s.Inspector.UserId == uid))
                    &&
                    (publicationId == "0" || u.PublicationId == pid)
                    ).OrderByDescending(s => s.EndDate)
                    .ToList();

            inspectionManageViewModel.Inspections = inspectionsFiltered.Select(u => new InspectionViewModel
            {
                InspectionId = u.InspectionId,
                StartDate = u.StartDate,
                EndDate = u.EndDate,
                Date = u.EndDate.Value,
                PublicationId = u.PublicationId,
                ProcessName = $"{u.Publication.Process.Label} (v{u.Publication.Version})",
                IsOK = !u.InspectionSteps.Any(s => s.IsOk == false),
                AnomalyNumber = u.InspectionSteps.Count(s => s.IsOk.HasValue && !s.IsOk.Value),
                Teams = u.InspectionSteps.SelectMany(s => s.Inspector.Teams.Select(t => t.Name)).Distinct().ToList(),
                Inspectors = u.InspectionSteps.Select(s => s.Inspector.FullName).Distinct().ToList(),
                DoneByDeactivated = !u.InspectionSteps.Select(s => s.Inspector.UserId).Distinct().ToList().Any(i => userPools.Any(p => p == i)),
                Steps = u.InspectionSteps.Select(s => new InspectionStepViewModel
                {
                    Id = s.InspectionStepId,
                    Date = s.Date,
                    Wbs = s.PublishedAction.WBS,
                    InspectionDate = s.Date.ToShortDateString(),
                    InspectorId = s.InspectorId,
                    Inspector = s.Inspector.FullName,
                    ActionLabel = s.PublishedAction.Label,
                    Teams = s.Inspector.Teams.Select(t => t.Name).ToList(),
                    IsOK = s.IsOk,
                    ThumbnailHash = s.PublishedAction.Thumbnail?.Hash,
                    ThumbnailExt = s.PublishedAction.Thumbnail?.Extension,
                    Action = new Models.Action.GenericActionViewModel
                    {
                        ActionId = s.PublishedAction.PublishedActionId,
                        Label = s.PublishedAction.Label
                    }
                }).ToList()
            });
            return inspectionManageViewModel;
        }
        public static InspectionViewModel ToInspectionViewModel(
            Inspection inspection,
            IList<InspectionStep> steps,
            IList<Anomaly> anomalies
            )
        {
            var model = new InspectionViewModel {
                InspectionId = inspection.InspectionId,
                StartDate = inspection.StartDate,
                EndDate = inspection.EndDate,
                Date = inspection.EndDate.Value,
                PublicationId = inspection.PublicationId,
                ProcessName = $"{inspection.Publication.Process.Label} (v{inspection.Publication.Version})",
                IsOK = !inspection.InspectionSteps.Any(s => s.IsOk == false),
                Teams = inspection.InspectionSteps.SelectMany(s => s.Inspector.Teams.Select(t => t.Name)).Distinct().ToList(),
                Inspectors = inspection.InspectionSteps.Select(s => s.Inspector.FullName).Distinct().ToList(),
                Steps = steps.Select(s => new InspectionStepViewModel
                {
                    Id = s.InspectionStepId,
                    // If inspection step id, it means we are addin a parent task so we do not have any date
                    Date = s.Date,
                    Wbs = s.PublishedAction.WBS,
                    InspectionDate = s.InspectionId != 0 ? s.Date.ToShortDateString() : "",
                    InspectorId = s.InspectorId,
                    Inspector = s.Inspection != null ? s.Inspector.FullName : "",
                    ActionLabel = s.PublishedAction.Label,
                    ThumbnailHash = s.PublishedAction.Thumbnail?.Hash,
                    ThumbnailExt = s.PublishedAction.Thumbnail?.Extension,
                    HasThumbnail = s.PublishedAction.ThumbnailHash != null && !string.IsNullOrEmpty(s.PublishedAction.ThumbnailHash),
                    Teams = s.Inspector != null ? s.Inspector.Teams.Select(t => t.Name).ToList() : new List<string>(),
                    IsOK = s.IsOk,
                    IsParent = s.IsParent != null ? s.IsParent : false,
                    ChildTask = (s.IsOk != null ? s.IsOk.Value : false) && !(s.IsParent != null ? s.IsParent.Value : false),
                    Level = s.Level,
                    IsKeyTask = s.PublishedAction.IsKeyTask,
                    colorCondition = s.IsOk == true && s.IsParent != true ? "readCell" : s.IsOk == false && s.IsParent != true ? "notReadCell" : "",
                    AnomalyId = s.AnomalyId,
                    TypeLabel = s.AnomalyId != null ? s.Anomaly.Type.AnomalyTypeToString() : "",
                    Description = s.AnomalyId != null ? s.Anomaly.Description : "",
                    Action = new Models.Action.GenericActionViewModel
                    {
                        ActionId = s.PublishedAction.PublishedActionId,
                        Label = s.PublishedAction.Label,
                    }
                }).ToList(),
                Anomalies = anomalies != null ? anomalies.Select(a => new AnomalyViewModel
                {
                    AnomalyId = a.Id,
                    AnomalyType = a.InspectionSteps.Count > 0 ? "Standard" : "Non standard",
                    AnomalyTypeIdentifier = a.InspectionSteps.Count > 0 ? 1 : 2,
                    Description = a.Description,
                    HasPhoto = a.Photo != null,
                    InspectionId = a.InspectionId,
                    InspectorId = a.InspectorId,
                    InspectorName = a.Inspector.FullName,
                    TypeLabel = a.Type.AnomalyTypeToString(),
                    Photo = a.Photo == null ? "" : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(a.Photo)),
                    RawPhoto = a.Photo == null ? "" : Convert.ToBase64String(a.Photo),
                    ThumbnailHash = a.InspectionSteps.Count > 0
                                    && a.InspectionSteps[0].PublishedAction != null 
                                    ? a.InspectionSteps[0].PublishedAction.Thumbnail?.Hash
                                    : null,
                    ThumbnailExt = a.InspectionSteps.Count > 0
                                   && a.InspectionSteps[0].PublishedAction != null
                                   ? a.InspectionSteps[0].PublishedAction.Thumbnail?.Extension
                                   : null,
                    HasThumbnail = a.InspectionSteps.Count > 0
                                   && a.InspectionSteps[0].PublishedAction != null
                                   && !string.IsNullOrEmpty(a.InspectionSteps[0].PublishedAction.ThumbnailHash),
                }).ToList() : new List<AnomalyViewModel>()
            };

            return model;
        }

        public static AnomalyViewModel ToAnomalyViewModel(Anomaly anomaly)
        {
            return new AnomalyViewModel
            {
                AnomalyId = anomaly.Id,
                Category = anomaly.Category,
                Date = anomaly.Date,
                Description = anomaly.Description,
                HasPhoto = anomaly.Photo != null,
                Photo = anomaly.Photo == null ? "" : string.Format("data:image/gif;base64,{0}", Convert.ToBase64String(anomaly.Photo)),
                RawPhoto = anomaly.Photo == null ? "" : Convert.ToBase64String(anomaly.Photo),
                Label = anomaly.Label,
                Line = anomaly.Line,
                Machine = anomaly.Machine,
                Priority = (int)anomaly.Priority.Value,
                Type = (int)anomaly.Type,
                TypeLabel = anomaly.Type.AnomalyTypeToString(),
                TypeColor = (int)anomaly.Type == 0 ? "green" : (int)anomaly.Type == 1 ? "red" : (int)anomaly.Type == 2 ? "blue" : "gray"
            };
        }

        public static List<InspectionViewModel> ToInspectionViewModels(IEnumerable<Inspection> inspections)
        {
            var model = new List<InspectionViewModel>();
            foreach (var inspection in inspections)
            {
                model.Add(new InspectionViewModel {
                    InspectionId = inspection.InspectionId,
                    StartDate = inspection.StartDate,
                    EndDate = inspection.EndDate,
                    PublicationId = inspection.PublicationId,
                    ProcessName = $"{inspection.Publication.Process.Label} (v{inspection.Publication.Version})",
                    IsOK = !inspection.InspectionSteps.Any(s => s.IsOk.Value == false),
                    AnomalyNumber = inspection.InspectionSteps.Count(s => s.IsOk.HasValue && !s.IsOk.Value),
                    Teams = inspection.InspectionSteps.SelectMany(s => s.Inspector.Teams.Select(t => t.Name)).Distinct().ToList(),
                    Inspectors = inspection.InspectionSteps.Select(s => s.Inspector.FullName).Distinct().ToList(),
                    AuditExists = inspection.Audits.Count > 0
                });
            }
            return model;
        }
        public static List<AnomalyViewModel> ToAnomalyViewModels(IEnumerable<Anomaly> anomalies)
        {
            var model = new List<AnomalyViewModel>();
            foreach (var anomaly in anomalies)
            {
                model.Add(new AnomalyViewModel {
                    AnomalyId = anomaly.Id,
                    Category = anomaly.Category,
                    Date = anomaly.Date,
                    Description = anomaly.Description,
                    HasPhoto = anomaly.Photo != null ? true : false,
                    Photo = anomaly.Photo == null ? "" : String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(anomaly.Photo)),
                    RawPhoto = anomaly.Photo == null ? "" : Convert.ToBase64String(anomaly.Photo),
                    Label = anomaly.Label,
                    Line = anomaly.Line,
                    Machine = anomaly.Machine,
                    Priority = (int)anomaly.Priority.Value,
                    Type = (int)anomaly.Type,
                    TypeLabel = anomaly.Type.AnomalyTypeToStringReport(),
                    TypeColor = (int)anomaly.Type == 0 ? "#5cb85c" : (int)anomaly.Type == 1 ? "#d9534f" : (int)anomaly.Type == 2 ? "lightskyblue" : "grey",
                    ProcessLabel = anomaly.Inspection.Publication.Process.Label
                });
            }
            return model;
        }

    }
}