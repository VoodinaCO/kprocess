using KProcess;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using MoreLinq;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kprocess.KL2.SyncService.Jobs
{
    class SyncingJob : IJob
    {
        readonly string OfflineFilesPath = "Offline";
        const string TabletAppName = "Kprocess.KL2.TabletClient";
        const string PublicationFile = "publication.json";
        static readonly object PublicationFileLock = new object();

        readonly ITraceManager _traceManager;
        readonly IAPIHttpClient _apiHttpClient;
        readonly IPrepareService _prepareService;

        public string GetOfflinePath =>
            Path.GetFullPath(Path.Combine(FilesHelper.GetSyncFilesLocation(), OfflineFilesPath));

        public SyncingJob(ITraceManager traceManager, IApplicationUsersService applicationUsersService, IAPIHttpClient apiHttpClient, IPrepareService prepareService)
        {
#if DEBUG
            OfflineFilesPath = @"..\..\..\..\Kprocess.KL2.TabletClient\bin\Debug\SyncFiles\Offline";
#endif

            _traceManager = traceManager;
            _apiHttpClient = apiHttpClient;
            _prepareService = prepareService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var tokenFilePath = Path.Combine(GetOfflinePath, "token.json");

            // Detect if Tablet app is running
            if (TabletAppIsRunning())
            {
                _traceManager.TraceDebug("Don't sync because tablet app is running.");
                return;
            }
            // Detect if token.json exists
            if (!File.Exists(tokenFilePath))
            {
                _traceManager.TraceDebug($"Don't sync because '{tokenFilePath}' doesn't exist.");
                return;
            }

            // Authenticate
            string token = null;
            using (var file = File.OpenText(tokenFilePath))
            {
                var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                token = (string)serializer.Deserialize(file, typeof(string));
            }
            if (!await _apiHttpClient.Relogon(token))
            {
                _traceManager.TraceDebug("Don't sync because can't relogon.");
                return;
            }

            // Sync
            List<string> jsonFiles = new List<string>
            {
                Path.Combine(GetOfflinePath, "training.json"),
                Path.Combine(GetOfflinePath, "evaluation.json"),
                Path.Combine(GetOfflinePath, "inspection.json")
            };
            foreach (var jsonFile in jsonFiles)
            {
                var publication = GetPublicationFromJson(jsonFile);
                if (publication == null)
                {
                    _traceManager.TraceDebug($"Don't sync '{jsonFile}' because publication is null.");
                    continue;
                }
                var hasChanges = publication.HasChanges();
                if (!hasChanges)
                {
                    _traceManager.TraceDebug($"Don't sync '{jsonFile}' because publication hasn't changes.");
                    continue;
                }
                try
                {
                    Publication result = await _prepareService.SavePublication(publication);
                    if (result != null)
                        await UpdatePublication(result, false);
                    _traceManager.TraceDebug($"Sync database OK for '{jsonFile}'.");
                    SavePublicationToJson(result, jsonFile);
                    _traceManager.TraceDebug($"Sync json OK for '{jsonFile}'.");
                }
                catch (Exception e)
                {
                    _traceManager.TraceError(e, e.Message);
                }
            }
        }

        Publication GetPublicationFromJson(string filePath)
        {
            /*if (!File.Exists(Path.Combine(GetOfflinePath, PublicationFile)))
                return null;*/
            if (!File.Exists(filePath))
                return null;
            try
            {
                lock (PublicationFileLock)
                {
                    using (var file = File.OpenText(filePath))
                    {
                        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                        return serializer.Deserialize(file, typeof(Publication)) as Publication;
                    }
                }
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, e.Message);
                return null;
            }
        }

        void SavePublicationToJson(Publication publication, string filePath)
        {
            try
            {
                lock (PublicationFileLock)
                {
                    Directory.CreateDirectory(GetOfflinePath);
                    using (var file = File.CreateText(filePath))
                    {
                        var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };
                        serializer.Serialize(file, publication);
                    }
                }
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, e.Message);
            }
        }

        bool TabletAppIsRunning()
        {
            var processesList = Process.GetProcesses().ToList();
            return processesList.Any(_ => _.ProcessName == TabletAppName);
        }

        public Task<bool> UpdatePublication(Publication publication, bool downloadFile)
        {
            try
            {
                if (publication == null)
                    return Task.FromResult(false);

                // On met à jour IsGroup
                publication.PublishedActions.ForEach(action =>
                {
                    if (action.LinkedPublication != null)
                        action.LinkedPublication?.PublishedActions.ForEach(innerAction => UpdateAction(action.LinkedPublication, innerAction));
                    else
                        UpdateAction(publication, action);
                });

                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, e.Message);
                return Task.FromResult(false);
            }
        }

        void UpdateAction(Publication publication, PublishedAction action)
        {
            Dictionary<string, string> localizations;
            try
            {
                if (WBSHelper.HasChildren(action.WBS, publication.PublishedActions.Select(_ => _.WBS)))
                    action.IsGroup = true;

                localizations = publication.Localizations.ToDictionary(k => k.ResourceKey, v => v.Value);

                // On construit les détails de la tâche
                action.Refs = new TrackableCollection<RefsCollection>();

                UpdateRefs((action, localizations), nameof(Ref1), 1);
                UpdateRefs((action, localizations), nameof(Ref2), 2);
                UpdateRefs((action, localizations), nameof(Ref3), 3);
                UpdateRefs((action, localizations), nameof(Ref4), 4);
                UpdateRefs((action, localizations), nameof(Ref5), 5);
                UpdateRefs((action, localizations), nameof(Ref6), 6);
                UpdateRefs((action, localizations), nameof(Ref7), 7);

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
                _traceManager.TraceError(ex, "Erreur lors de l'ajout d'info dans les actions publiées.");
                throw;
            }
        }

        (PublishedAction action, Dictionary<string, string> localizations) UpdateRefs((PublishedAction action, Dictionary<string, string> localizations) tuple, string refName, int refNumber)
        {
            if (tuple.action.PublishedReferentialActions.Any(_ => _.RefNumber == refNumber))
                tuple.action.Refs.Add(new RefsCollection { Field = $"Refs{refNumber}", Label = tuple.localizations[refName], Values = new TrackableCollection<PublishedReferentialAction>(tuple.action.PublishedReferentialActions.Where(_ => _.RefNumber == refNumber)) });
            return tuple;
        }
    }
}
