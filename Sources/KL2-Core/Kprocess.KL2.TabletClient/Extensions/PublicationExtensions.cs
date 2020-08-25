using Kprocess.KL2.FileTransfer;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Services;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.Extensions
{
    public static class PublicationExtensions
    {
        public static void SetRead(this Publication publication, int readerId, DateTime? readingDate = null)
        {
            DateTime now = readingDate ?? DateTime.Now;
            if (publication == null)
                return;
            var userReadPublication = publication.Readers.SingleOrDefault(_ => _.UserId == readerId);
            if (userReadPublication == null)
                publication.Readers.Add(new UserReadPublication { UserId = readerId, ReadDate = now });
            else if (userReadPublication.ReadDate == null)
                userReadPublication.ReadDate = now;
            foreach (var linkedPublication in publication.PublishedActions.Where(_ => _.LinkedPublication != null).Select(_ => _.LinkedPublication))
                linkedPublication.SetRead(readerId, now);
        }

        /// <summary>
        /// Méthode permettant de mettre les différents élement non sauvegardé lors de la sérialization 
        /// </summary>
        /// <param name="publication"></param>
        /// <param name="downloadFile"></param>
        public static Task<TaskResult> UpdatePublication(this Publication publication, bool downloadFile)
        {
            try
            {
                if (publication == null)
                    return Task.FromResult(TaskResult.Nok);

                // On met à jour IsGroup
                publication.PublishedActions.ForEach(action =>
                {
                    if (action.LinkedPublication != null)
                        action.LinkedPublication?.PublishedActions.ForEach(innerAction => UpdateAction(action.LinkedPublication, innerAction));
                    else
                        UpdateAction(publication, action);
                });

                if (downloadFile && Locator.APIManager.IsOnline == true)
                    return Locator.DownloadManager.StartDownload(publication);
                return Task.FromResult(TaskResult.Ok);
            }
            catch (Exception e)
            {
                Locator.TraceManager.TraceError(e, e.Message);
                return Task.FromResult(TaskResult.Nok);
            }
        }

        /// <summary>
        /// Méthode permettant de mettre les différents élement non sauvegardé lors de la sérialization 
        /// </summary>
        /// <param name="publications"></param>
        /// <param name="downloadFile"></param>
        public static Task<TaskResult> UpdatePublications(IEnumerable<Publication> publications, bool downloadFile)
        {
            try
            {
                if (publications == null || !publications.Any() || publications.All(p => p == null))
                    return Task.FromResult(TaskResult.Nok);

                // On met à jour IsGroup
                foreach (var publication in publications.Where(p => p != null))
                {
                    publication.PublishedActions.ForEach(action =>
                    {
                        if (action.LinkedPublication != null)
                            action.LinkedPublication?.PublishedActions.ForEach(innerAction => UpdateAction(action.LinkedPublication, innerAction));
                        else
                            UpdateAction(publication, action);
                    });
                }

                if (downloadFile && Locator.APIManager.IsOnline == true)
                    return Locator.DownloadManager.StartDownload(publications.Where(p => p != null));
                return Task.FromResult(TaskResult.Ok);
            }
            catch (Exception e)
            {
                Locator.TraceManager.TraceError(e, e.Message);
                return Task.FromResult(TaskResult.Nok);
            }
        }

        static void UpdateAction(Publication publication, PublishedAction action)
        {
            Dictionary<string, string> localizations;
            try
            {
                if (WBSHelper.HasChildren(action.WBS, publication.PublishedActions.Select(_ => _.WBS)))
                    action.IsGroup = true;

                localizations = publication.Localizations.ToDictionary(k => k.ResourceKey, v => v.Value);

                // On construit les détails de la tâche
                action.Refs = new TrackableCollection<RefsCollection>();

                (action, localizations)
                    .UpdateRefs(nameof(Ref1), 1)
                    .UpdateRefs(nameof(Ref2), 2)
                    .UpdateRefs(nameof(Ref3), 3)
                    .UpdateRefs(nameof(Ref4), 4)
                    .UpdateRefs(nameof(Ref5), 5)
                    .UpdateRefs(nameof(Ref6), 6)
                    .UpdateRefs(nameof(Ref7), 7);

                action.CustomLabels = new TrackableCollection<CustomLabel>();

                if (!string.IsNullOrEmpty(action.CustomTextValue))
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue), Label = localizations[nameof(Project.CustomTextLabel)], Value = action.CustomTextValue });
                if (!string.IsNullOrEmpty(action.CustomTextValue2))
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue2), Label = localizations[nameof(Project.CustomTextLabel2)], Value = action.CustomTextValue2 });
                if (!string.IsNullOrEmpty(action.CustomTextValue3))
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue3), Label = localizations[nameof(Project.CustomTextLabel3)], Value = action.CustomTextValue3 });
                if (!string.IsNullOrEmpty(action.CustomTextValue4))
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomTextValue4), Label = localizations[nameof(Project.CustomTextLabel4)], Value = action.CustomTextValue4 });
                if (action.CustomNumericValue != null)
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue), Label = localizations[nameof(Project.CustomNumericLabel)], Value = ((double)action.CustomNumericValue).ToString() });
                if (action.CustomNumericValue2 != null)
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue2), Label = localizations[nameof(Project.CustomNumericLabel2)], Value = ((double)action.CustomNumericValue2).ToString() });
                if (action.CustomNumericValue3 != null)
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue3), Label = localizations[nameof(Project.CustomNumericLabel3)], Value = ((double)action.CustomNumericValue3).ToString() });
                if (action.CustomNumericValue4 != null)
                    action.CustomLabels.Add(new CustomLabel { Field = nameof(PublishedAction.CustomNumericValue4), Label = localizations[nameof(Project.CustomNumericLabel4)], Value = ((double)action.CustomNumericValue4).ToString() });
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de l'ajout d'info dans les actions publiées.");
                throw;
            }
        }

        static (PublishedAction action, Dictionary<string, string> localizations) UpdateRefs(this (PublishedAction action, Dictionary<string, string> localizations) tuple, string refName, int refNumber)
        {
            if (tuple.action.PublishedReferentialActions.Any(_ => _.RefNumber == refNumber))
                tuple.action.Refs.Add(new RefsCollection { Field = $"Refs{refNumber}", Label = tuple.localizations[refName], Values = new TrackableCollection<PublishedReferentialAction>(tuple.action.PublishedReferentialActions.Where(_ => _.RefNumber == refNumber)) });
            return tuple;
        }

        public static async Task<KProcess.BulkObservableCollection<INode>> ReloadAllPublications(this PublishModeEnum mode)
        {
            var result = new KProcess.BulkObservableCollection<INode>();
            if (Locator.APIManager.IsOnline == true)
            {
                INode[] data = await Locator.GetService<IPrepareService>().GetPublicationsTree(mode);
                var root = new ProjectDir { Id = -1, Name = Locator.LocalizationManager.GetString("All_Publications_Label"), ParentId = null, IsExpanded = true };
                root.MarkAsUnchanged();
                root.StopTracking();
                root.Childs = new TrackableCollection<ProjectDir>(data.OfType<ProjectDir>());
                root.Processes = new TrackableCollection<Procedure>(data.OfType<Procedure>());
                root.StartMonitorNodesChanged();
                result.Add(root);

                // Start tracking
                foreach (INode node in result[0].Nodes)
                    StartTracking(node);

                // On exporte l'arbre de publication pour le mode hors ligne
                switch (mode)
                {
                    case PublishModeEnum.Formation:
                    case PublishModeEnum.Evaluation:
                        OfflineFile.TrainingsTree.SaveToJson(root);
                        break;
                    case PublishModeEnum.Inspection:
                        OfflineFile.InspectionsTree.SaveToJson(root);
                        break;
                }
            }
            else
            {
                await Task.Run(async () =>
                {
                    ProjectDir root = null;
                    switch (mode)
                    {
                        case PublishModeEnum.Formation:
                        case PublishModeEnum.Evaluation:
                            root = await OfflineFile.TrainingsTree.GetFromJson<ProjectDir>();
                            break;
                        case PublishModeEnum.Inspection:
                            root = await OfflineFile.InspectionsTree.GetFromJson<ProjectDir>();
                            break;
                    }
                    root.IsExpanded = true;
                    root.MarkAsUnchanged();
                    root.StopTracking();
                    root.StartMonitorNodesChanged();
                    result.Add(root);

                    // Start tracking
                    foreach (INode node in result[0].Nodes)
                        StartTracking(node);
                });
            }
            return result;
        }

        static void StartTracking(INode root)
        {
            if (root is ProjectDir projectDir)
            {
                projectDir.StartMonitorNodesChanged();
                projectDir.StartTracking();
                projectDir.Childs.ForEach(StartTracking);
                projectDir.Processes.ForEach(StartTracking);
            }
            else if (root is Procedure process)
            {
                process.StartMonitorNodesChanged();
                process.StartTracking();
            }
        }

        public static bool CheckAuditorHaveActiveAudit(this Publication publication, int auditorId)
        {
            return publication.Inspections.SelectMany(i => i.Audits).Any(a => a.AuditorId == auditorId && a.EndDate == null && !a.IsDeleted);
        }
    }
}