using Kprocess.KL2.FileTransfer;
using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion de la partie fonctionnelle "Analyser".
    /// </summary>
    public class AnalyzeService : IBusinessService, IAnalyzeService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public AnalyzeService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Prédit les scénarios qui seront impactés par les modifications en attente de sauvegarde.
        /// </summary>
        /// <param name="sourceModifiedScenario">Le scenério source modifié.</param>
        /// <param name="allScenarios">Tous les scénarios.</param>
        /// <param name="actionsToDelete">Les actions à supprimer.</param>
        public virtual Task<Scenario[]> PredictImpactedScenarios(
            Scenario sourceModifiedScenario, Scenario[] allScenarios, KAction[] actionsToDelete, KAction[] actionsWithUpdatedWBS) =>
            Task.Run(() =>
            {
                return ActionsRecursiveUpdate.PredictImpactedScenarios(sourceModifiedScenario, allScenarios, actionsToDelete, actionsWithUpdatedWBS);
            });

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<AcquireData> GetAcquireData(int projectId, bool getSyncedVideos = true)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<AcquireData>(KL2_Server.API, nameof(AnalyzeService), nameof(GetAcquireData), param);
            });
        }

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task<Scenario> SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario)
        {
            return await Task.Run(async () =>
            {
                // Avant l'appel de la fonction de sauvegarde, on doit envoyer tous les nouveaux thumbnails
                var actionCloudFiles = updatedScenario.Actions.Where(_ => _.Thumbnail != null && _.Thumbnail.IsMarkedAsAdded);
                foreach (KAction actionCloudFile in actionCloudFiles)
                {
                    CloudFile cloudFile = await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, "CloudFilesService", $"Get/{actionCloudFile.Thumbnail.Hash}{actionCloudFile.Thumbnail.Extension}", null, "GET");
                    if (cloudFile != null) // Le fichier est déjà en base
                        actionCloudFile.Thumbnail.MarkAsUnchanged();
                    if (!await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{actionCloudFile.Thumbnail.Hash}{actionCloudFile.Thumbnail.Extension}", null, "GET"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(Preferences.SyncDirectory, $"{actionCloudFile.Thumbnail.Hash}{actionCloudFile.Thumbnail.Extension}"));
                        var tusOperation = TusFileTransferManager.Instance.Upload($"{Preferences.FileServerUri}/files", fileInfo);
                        await tusOperation.WaitTransferFinished();
                    }
                }

                dynamic param = new ExpandoObject();
                param.allScenarios = allScenarios;
                param.updatedScenario = updatedScenario;
                return await _apiHttpClient.ServiceAsync<Scenario>(KL2_Server.API, nameof(AnalyzeService), nameof(SaveAcquireData), param);
            });
        }

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<BuildData> GetBuildData(int projectId, bool getSyncedVideos = true)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<BuildData>(KL2_Server.API, nameof(AnalyzeService), nameof(GetBuildData), param);
            });
        }


        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task<Scenario> SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.allScenarios = allScenarios;
                param.updatedScenario = updatedScenario;
                return await _apiHttpClient.ServiceAsync<Scenario>(KL2_Server.API, nameof(AnalyzeService), nameof(SaveBuildScenario), param);
            });
        }


        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<SimulateData> GetSimulateData(int projectId)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<SimulateData>(KL2_Server.API, nameof(AnalyzeService), nameof(GetSimulateData), param);
            });
        }

        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetRestitutionData(int projectId)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<RestitutionData>(KL2_Server.API, nameof(AnalyzeService), nameof(GetRestitutionData), param);
            });
        }

        /// <summary>
        /// Obtient toutes les données du projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetFullProjectDetails(int projectId)
        {
            return await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<RestitutionData>(KL2_Server.API, nameof(AnalyzeService), nameof(GetFullProjectDetails), param);
            });
        }
  
        /// <summary>
        /// Met à jour l'affichage dans la synthèse pour le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="isShownInSummary"><c>true</c> pour l'afficher dans le scénario.</param>
        public virtual async Task<Scenario> UpdateScenarioIsShownInSummary(int scenarioId, bool isShownInSummary)
        {
            dynamic param = new ExpandoObject();
            param.scenarioId = scenarioId;
            param.isShownInSummary = isShownInSummary;
            return await _apiHttpClient.ServiceAsync<Scenario>(KL2_Server.API, nameof(AnalyzeService), nameof(UpdateScenarioIsShownInSummary), param);
        }
    }
}