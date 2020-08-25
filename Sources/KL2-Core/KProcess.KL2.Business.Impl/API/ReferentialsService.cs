using Kprocess.KL2.FileTransfer;
using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion des référentiels d'actions.
    /// </summary>
    public class ReferentialsService : IBusinessService, IReferentialsService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public ReferentialsService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        public string GetLabel(ProcessReferentialIdentifier refe)
        {
            dynamic param = new ExpandoObject();
            param.refe = refe;
            return _apiHttpClient.Service<string>(KL2_Server.API, nameof(ReferentialsService), nameof(GetLabel), param);
        }

        public string GetLabelPlural(ProcessReferentialIdentifier refe)
        {
            dynamic param = new ExpandoObject();
            param.refe = refe;
            return _apiHttpClient.Service<string>(KL2_Server.API, nameof(ReferentialsService), nameof(GetLabelPlural), param);
        }

        /// <summary>
        /// Obtient la configuration des réferentiels.
        /// </summary>
        /// <returns>Lse référentiels.</returns>
        public Task<Referential[]> GetApplicationReferentials() =>
            _apiHttpClient.ServiceAsync<Referential[]>(KL2_Server.API, nameof(ReferentialsService), nameof(GetApplicationReferentials));

        /// <inheritdoc />
        public async Task UpdateReferentialLabel(ProcessReferentialIdentifier refId, string label) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.refId = refId;
                param.label = label;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(ReferentialsService), nameof(UpdateReferentialLabel), param);
            });

        #region Catégories

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les catégories d'actions de tous les projets
        /// Tous les types d'actions 
        /// Toutes les valorisations d'actions.
        /// </summary>
        public async Task<(ActionCategory[] Categories, ActionValue[] ActionValues, ActionType[] ActionTypes, Procedure[] Processes)> LoadCategories() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<(ActionCategory[] Categories, ActionValue[] ActionValues, ActionType[] ActionTypes, Procedure[] Processes)>(KL2_Server.API, nameof(ReferentialsService), nameof(LoadCategories));
            });

        /// <summary>
        /// Sauvegarde les catégories.
        /// </summary>
        /// <param name="categories">Les catégories.</param>
        public async Task<IEnumerable<ActionCategory>> SaveCategories(IEnumerable<ActionCategory> categories) =>
            await Task.Run(async () =>
            {
                // Avant l'appel de la fonction de sauvegarde, on doit envoyer tous les nouveaux fichiers
                var categoryCloudFiles = categories.Where(_ => _.CloudFile != null && _.CloudFile.IsMarkedAsAdded);
                foreach (var categoryCloudFile in categoryCloudFiles)
                {
                    CloudFile cloudFile = await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, "CloudFilesService", $"Get/{categoryCloudFile.CloudFile.Hash}{categoryCloudFile.CloudFile.Extension}", null, "GET");
                    if (cloudFile != null) // Le fichier est déjà en base
                        categoryCloudFile.CloudFile.MarkAsUnchanged();
                    if (!await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{categoryCloudFile.CloudFile.Hash}{categoryCloudFile.CloudFile.Extension}", null, "GET"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(Preferences.SyncDirectory, $"{categoryCloudFile.CloudFile.Hash}{categoryCloudFile.CloudFile.Extension}"));
                        var tusOperation = TusFileTransferManager.Instance.Upload($"{Preferences.FileServerUri}/files", fileInfo);
                        await tusOperation.WaitTransferFinished();
                    }
                }

                dynamic param = new ExpandoObject();
                param.categories = categories.ToList();
                return await _apiHttpClient.ServiceAsync<IEnumerable<ActionCategory>>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveCategories), param);
            });

        /// <summary>
        /// Obtient un CloudFile
        /// <param name="hash">Hash généré comme identifiant</param>
        /// </summary>
        public async Task<CloudFile> GetCloudFile(string hash) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.hash = hash;
                return await _apiHttpClient.ServiceAsync<IEnumerable<ActionCategory>>(KL2_Server.API, nameof(ReferentialsService), nameof(GetCloudFile), param);
            });

        /// <summary>
        /// Sauvegarde un CloudFile
        /// </summary>
        public async Task<CloudFile> SaveCloudFile(CloudFile file) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.file = file;
                return await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveCloudFile), param);
            });

        #endregion

        #region Compétences

        /// <summary>
        /// Obtient les compétences d'actions de tous les projets
        /// </summary>
        public async Task<Skill[]> LoadSkills(bool allInfos = false) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.allInfos = allInfos;
                return await _apiHttpClient.ServiceAsync<Skill[]>(KL2_Server.API, nameof(ReferentialsService), nameof(LoadSkills), param);
            });

        /// <summary>
        /// Sauvegarde les compétences.
        /// </summary>
        /// <param name="skills">Les compétences.</param>
        public async Task<IEnumerable<Skill>> SaveSkills(IEnumerable<Skill> skills) =>
            await Task.Run(async () =>
            {
                // Avant l'appel de la fonction de sauvegarde, on doit envoyer tous les nouveaux fichiers
                var skillCloudFiles = skills.Where(_ => _.CloudFile != null && _.CloudFile.IsMarkedAsAdded);
                foreach (var skillCloudFile in skillCloudFiles)
                {
                    CloudFile cloudFile = await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, "CloudFilesService", $"Get/{skillCloudFile.CloudFile.Hash}{skillCloudFile.CloudFile.Extension}", null, "GET");
                    if (cloudFile != null) // Le fichier est déjà en base
                        skillCloudFile.CloudFile.MarkAsUnchanged();
                    if (!await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{skillCloudFile.CloudFile.Hash}{skillCloudFile.CloudFile.Extension}", null, "GET"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(Preferences.SyncDirectory, $"{skillCloudFile.CloudFile.Hash}{skillCloudFile.CloudFile.Extension}"));
                        var tusOperation = TusFileTransferManager.Instance.Upload($"{Preferences.FileServerUri}/files", fileInfo);
                        await tusOperation.WaitTransferFinished();
                    }
                }

                dynamic param = new ExpandoObject();
                param.skills = skills.ToList();
                return await _apiHttpClient.ServiceAsync<IEnumerable<Skill>>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveSkills), param);
            });

        #endregion

        #region Référentiels multiples

        /// <summary>
        /// Obtient les référentiels standards et projets du type spécifié.
        /// </summary>
        /// <param name="refId">L'identifiant du référentiel.</param>
        public async Task<(IMultipleActionReferential[] Referentials, Procedure[] Processes)> GetReferentials(ProcessReferentialIdentifier refId, int? processId = null) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.refId = refId;
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<(IMultipleActionReferential[] Referentials, Procedure[] Processes)>(KL2_Server.API, nameof(ReferentialsService), nameof(GetReferentials), param);
            });

        public async Task<(IActionReferential[] Referentials, Procedure[] Processes)> GetAllReferentials(ProcessReferentialIdentifier refId, int? processId = null) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.refId = refId;
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<(IActionReferential[] Referentials, Procedure[] Processes)>(KL2_Server.API, nameof(ReferentialsService), nameof(GetAllReferentials), param);
            });

        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <param name="referentials">Les référentiels.</param>
        public async Task<IEnumerable<TReferential>> SaveReferentials<TReferential>(IEnumerable<TReferential> referentials) where TReferential : class, IActionReferential =>
            await Task.Run(async () =>
            {
                // Avant l'appel de la fonction de sauvegarde, on doit envoyer tous les nouveaux fichiers
                var referentialCloudFiles = referentials.Where(_ => _.CloudFile != null && _.CloudFile.IsMarkedAsAdded);
                foreach (var referentialCloudFile in referentialCloudFiles)
                {
                    CloudFile cloudFile = await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, "CloudFilesService", $"Get/{referentialCloudFile.CloudFile.Hash}{referentialCloudFile.CloudFile.Extension}", null, "GET");
                    if (cloudFile != null) // Le fichier est déjà en base
                        referentialCloudFile.CloudFile.MarkAsUnchanged();
                    if (!await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{referentialCloudFile.CloudFile.Hash}{referentialCloudFile.CloudFile.Extension}", null, "GET"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(Preferences.SyncDirectory, $"{referentialCloudFile.CloudFile.Hash}{referentialCloudFile.CloudFile.Extension}"));
                        var tusOperation = TusFileTransferManager.Instance.Upload($"{Preferences.FileServerUri}/files", fileInfo);
                        await tusOperation.WaitTransferFinished();
                    }
                }

                dynamic param = new ExpandoObject();
                param.type = typeof(TReferential);
                param.referentials = referentials.ToList();
                return await _apiHttpClient.ServiceAsync<IEnumerable<TReferential>>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveReferentials), param);
            });

        #endregion

        #region Ressources

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les équipements de tous les projets.
        /// </summary>
        public async Task<(Equipment[] Equipments, Procedure[] Processes)> LoadEquipments() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<(Equipment[] Equipments, Procedure[] Processes)>(KL2_Server.API, nameof(ReferentialsService), nameof(LoadEquipments));
            });

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les opérateurs de tous les projets.
        /// </summary>
        public async Task<(Operator[] Operators, Procedure[] Processes)> LoadOperators() =>
            await Task.Run(async () =>
            {
                return await _apiHttpClient.ServiceAsync<(Operator[] Operators, Procedure[] Processes)>(KL2_Server.API, nameof(ReferentialsService), nameof(LoadOperators));
            });

        /// <summary>
        /// Sauvegarde les ressources.
        /// </summary>
        /// <param name="resources">Les ressources.</param>
        public async Task<IEnumerable<Resource>> SaveResources(IEnumerable<Resource> resources) =>
            await Task.Run(async () =>
            {
                // Avant l'appel de la fonction de sauvegarde, on doit envoyer tous les nouveaux fichiers
                var resourceCloudFiles = resources.Where(_ => _.CloudFile != null && _.CloudFile.IsMarkedAsAdded);
                foreach (var resourceCloudFile in resourceCloudFiles)
                {
                    CloudFile cloudFile = await _apiHttpClient.ServiceAsync<CloudFile>(KL2_Server.API, "CloudFilesService", $"Get/{resourceCloudFile.CloudFile.Hash}{resourceCloudFile.CloudFile.Extension}", null, "GET");
                    if (cloudFile != null) // Le fichier est déjà en base
                        resourceCloudFile.CloudFile.MarkAsUnchanged();
                    if (!await _apiHttpClient.ServiceAsync<bool>(KL2_Server.File, null, $"Exists/{resourceCloudFile.CloudFile.Hash}{resourceCloudFile.CloudFile.Extension}", null, "GET"))
                    {
                        var fileInfo = new FileInfo(Path.Combine(Preferences.SyncDirectory, $"{resourceCloudFile.CloudFile.Hash}{resourceCloudFile.CloudFile.Extension}"));
                        var tusOperation = TusFileTransferManager.Instance.Upload($"{Preferences.FileServerUri}/files", fileInfo);
                        await tusOperation.WaitTransferFinished();
                    }
                }

                dynamic param = new ExpandoObject();
                param.resources = resources.ToList();
                return await _apiHttpClient.ServiceAsync<IEnumerable<Resource>>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveResources), param);
            });

        #endregion

        /// <summary>
        /// Fusionne des référentiels
        /// </summary>
        /// <param name="master">Le référentiel maître.</param>
        /// <param name="slaves">Les référentiels esclaves.</param>
        public async Task MergeReferentials(IActionReferential master, IActionReferential[] slaves) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.master = master;
                param.slaves = slaves;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(ReferentialsService), nameof(MergeReferentials), param);
            });

        public async Task<bool> ReferentialUsed(ProcessReferentialIdentifier processReferentialId, int referentialId)
        {
            dynamic param = new ExpandoObject();
            param.processReferentialId = processReferentialId;
            param.referentialId = referentialId;
            return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(ReferentialsService), nameof(ReferentialUsed), param);
        }

        /// <summary>
        /// Obtient les référentiels du projet spécifié.
        /// </summary>
        public async Task<List<ProjectReferential>> GetProjectReferentials(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<List<ProjectReferential>>(KL2_Server.API, nameof(ReferentialsService), nameof(GetProjectReferentials), param);
            });

        /// <summary>
        /// Obtient si le Custom Field est utilisé dans le scénario spécifié.
        /// </summary>
        public async Task<bool> CustomFieldIsUsed(int scenarioId, ProcessReferentialIdentifier customFieldId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.scenarioId = scenarioId;
                param.customFieldId = customFieldId;
                return await _apiHttpClient.ServiceAsync<bool>(KL2_Server.API, nameof(ReferentialsService), nameof(CustomFieldIsUsed), param);
            });

        /// <summary>
        /// Obtient les référentiels de documentation du process spécifié.
        /// </summary>
        public async Task<List<DocumentationReferential>> GetDocumentationReferentials(int processId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.processId = processId;
                return await _apiHttpClient.ServiceAsync<List<DocumentationReferential>>(KL2_Server.API, nameof(ReferentialsService), nameof(GetDocumentationReferentials), param);
            });

        /// <summary>
        /// Sauvegarde les référentiels de documentation.
        /// </summary>
        public async Task<List<DocumentationReferential>> SaveDocumentationReferentials(DocumentationReferential[] documentationReferentials) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.documentationReferentials = documentationReferentials.ToArray();
                return await _apiHttpClient.ServiceAsync<List<DocumentationReferential>>(KL2_Server.API, nameof(ReferentialsService), nameof(SaveDocumentationReferentials), param);
            });
    }
}