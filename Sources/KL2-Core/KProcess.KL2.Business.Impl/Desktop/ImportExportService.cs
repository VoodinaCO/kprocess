using KProcess.Business;
using KProcess.Common;
using KProcess.Data;
using KProcess.KL2.Business.Impl.Shared;
using KProcess.KL2.Business.Impl.Shared.ImportExport;
using KProcess.KL2.Business.Impl.Shared.ProjectImportMigration;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service d'importation/exportation.
    /// </summary>
    public class ImportExportService : IBusinessService, IImportExportService
    {
        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;

        public ImportExportService(ISecurityContext securityContext, ITraceManager traceManager)
        {
            _securityContext = securityContext;
            _traceManager = traceManager;
        }

        #region Export projet

        /// <summary>
        /// Exporte un projet.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<Stream> ExportProject(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    ProjectExport projectExport = await CreateProjectExport(context, projectId);

                    MemoryStream ms = new MemoryStream();
                    XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false);
                    NetDataContractSerializer ser = new NetDataContractSerializer();
                    ser.WriteObject(writer, projectExport);
                    writer.Close();

                    ms.Seek(0, SeekOrigin.Begin);

                    return ms;
                }
            });

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="data">Les données contenant l'export.</param>
        public virtual async Task<ProjectImport> PredictMergedReferentialsProject(byte[] data) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    ProjectMigration migration = new ProjectMigration(data);

                    ProjectExport project = await migration.Migrate();

                    // Charger les référentiels
                    Referentials dbStandardReferentials = await LoadAllStandardReferentials(context);

                    Dictionary<IActionReferentialProcess, IActionReferential> referentialsProject = new Dictionary<IActionReferentialProcess, IActionReferential>();
                    Dictionary<IActionReferential, IActionReferential> referentialsStd = new Dictionary<IActionReferential, IActionReferential>();

                    DetermineMergeCandidates(project.ReferentialsProject, dbStandardReferentials, referentialsProject);
                    DetermineMergeCandidates(project.ReferentialsStandard, dbStandardReferentials, referentialsStd);

                    return new ProjectImport()
                    {
                        ExportedProject = project,
                        ProjectReferentialsMergeCandidates = referentialsProject,
                        StandardReferentialsMergeCandidates = referentialsStd,
                    };
                }
            });

        /// <summary>
        /// Importe un projet.
        /// </summary>
        /// <param name="import">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        public virtual async Task<Project> ImportProject(ProjectImport import, bool mergeReferentials, string videosDirectory) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    return await ImportProject(context, import, mergeReferentials, videosDirectory);
                }
            });

        /// <summary>
        /// Crée une instance de <see cref="ProjectExport"/>.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <returns>Le <see cref="ProjectExport"/></returns>
        internal async Task<ProjectExport> CreateProjectExport(KsmedEntities context, int projectId)
        {
            var referentialsUsed = await SharedScenarioActionsOperations.GetReferentialsUse(context, projectId);
            await Queries.LoadAllReferentialsOfProject(context, projectId, referentialsUsed);

            var project = await context.Projects
                .Include("Videos")
                .Include("Referentials")
                .FirstAsync(s => s.ProjectId == projectId);

            var scenarios = await context.Scenarios
                .Where(s => s.ProjectId == projectId &&
                    (s.NatureCode == KnownScenarioNatures.Initial || s.NatureCode == KnownScenarioNatures.Target || s.NatureCode == KnownScenarioNatures.Realized))
                .ToArrayAsync();

            await Queries.LoadScenariosDetails(context, scenarios, referentialsUsed);

            return new ProjectExport()
            {
                AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Project = project,
                ReferentialsStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(project).ToArray(),
                ReferentialsProject = ReferentialsHelper.GetAllReferentialsProject(project).ToArray(),
            };
        }

        /// <summary>
        /// Importe le projet spécifié.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="import">Le projet exporté.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier où se situent les vidéos.</param>
        /// <returns>Le projet créé.</returns>
        private async Task<Project> ImportProject(KsmedEntities context, ProjectImport import, bool mergeReferentials, string videosDirectory)
        {
            // Projet
            var p = import.ExportedProject.Project;
            p.ProjectId = default(int);
            MarkAsAdded(p);

            // Ajouter l'utilisateur courant en tant qu'analyste créateur
            var owner = await context.Users.FirstAsync(u => !u.IsDeleted && u.Username == _securityContext.CurrentUser.Username);
            p.Process.Owner = owner;
            p.Process.UserRoleProcesses.Add(new UserRoleProcess()
            {
                User = owner,
                RoleCode = KnownRoles.Analyst,
            });

            // Videos
            foreach (var video in p.Process.Videos)
            {
                if (video.DefaultResourceId.HasValue && video.DefaultResource == null)
                {
                    // Bug présent dans certains exports de la version 2.5.0.0. Supprimer le lien vers la ressource par défaut.
                    video.DefaultResourceId = null;
                }

                video.VideoId = default(int);
                MarkAsAdded(video);
                video.FilePath = IOHelper.ChangeDirectory(video.FilePath, videosDirectory);
            }

            var referentialsToRemap = new Dictionary<IActionReferential, IActionReferential>();

            if (!mergeReferentials)
            {
                // Référentiels process
                foreach (var refe in import.ExportedProject.ReferentialsProject)
                    MarkAsAdded(refe);

                // Référentiels standard
                foreach (var refe in import.ExportedProject.ReferentialsStandard)
                {
                    var refProject = ReferentialsFactory.CopyToNewProject(refe);

                    // Associer au process
                    refProject.Process = p.Process;

                    referentialsToRemap[refe] = refProject;
                }
            }
            else
            {
                // Référentiels process
                foreach (var refe in import.ExportedProject.ReferentialsProject)
                {
                    // Ajouter au tableau de remap s'il y a une correspondance.
                    if (import.ProjectReferentialsMergeCandidates.ContainsKey(refe))
                    {
                        referentialsToRemap[refe] = import.ProjectReferentialsMergeCandidates[refe];
                    }
                    else
                        MarkAsAdded(refe);
                }

                // Référentiels standard
                foreach (var refe in import.ExportedProject.ReferentialsStandard)
                {
                    if (import.StandardReferentialsMergeCandidates.ContainsKey(refe))
                    {
                        referentialsToRemap[refe] = import.StandardReferentialsMergeCandidates[refe];
                    }
                    else
                    {
                        var refProject = ReferentialsFactory.CopyToNewProject(refe);

                        // Associer au process
                        refProject.Process = p.Process;

                        referentialsToRemap[refe] = refProject;
                    }
                }
            }


            // Scénarios
            foreach (var scenario in p.Scenarios.Where(s => s.OriginalScenarioId.HasValue))
            {
                // Remapper l'original
                scenario.Original = p.Scenarios.Single(s => s.ScenarioId == scenario.OriginalScenarioId);
            }

            foreach (var scenario in p.Scenarios)
            {
                foreach (var action in scenario.Actions.Where(a => a.OriginalActionId.HasValue))
                {
                    // Remapper l'original
                    action.Original = p.Scenarios.SelectMany(s => s.Actions).Single(a => a.ActionId == action.OriginalActionId);
                }
            }

            foreach (var scenario in p.Scenarios)
            {
                scenario.ScenarioId = default(int);
                MarkAsAdded(scenario);

                // Supprimer le WebPublicationGuid
                scenario.WebPublicationGuid = null;

                // Actions
                foreach (var action in scenario.Actions)
                {
                    action.ActionId = default(int);
                    MarkAsAdded(action);

                    // Actions réduites
                    if (action.IsReduced)
                    {
                        MarkAsAdded(action.Reduced);
                    }
                }
            }

            // Remapper les référentiels du projet, des actions et des vidéos
            foreach (var oldReferential in referentialsToRemap.Keys)
            {
                ReferentialsHelper.UpdateReferentialReferences(p, oldReferential, referentialsToRemap[oldReferential]);
            }

            foreach (var scenario in p.Scenarios)
                if (scenario.Original != null)
                {
                    context.Scenarios.ApplyChanges(scenario);
                    ObjectContextExt.SetRelationShipReferenceValue(context, scenario, scenario.Original, s => s.OriginalScenarioId);

                    foreach (var action in scenario.Actions)
                        if (action.Original != null)
                        {
                            context.KActions.ApplyChanges(action);
                            ObjectContextExt.SetRelationShipReferenceValue(context, action, action.Original, a => a.OriginalActionId);
                        }
                }

            var resources = p.Scenarios.SelectMany(s => s.Actions).Select(a => a.Resource).Distinct().ToArray();

            context.Projects.ApplyChanges(p);

            if (mergeReferentials)
            {
                ServicesDiagnosticsDebug.CheckNotInContext(context, referentialsToRemap.Keys);
                ServicesDiagnosticsDebug.CheckObjectStateManagerState(context, EntityState.Unchanged, referentialsToRemap.Values);
            }

            ServicesDiagnosticsDebug.CheckReferentialsState();

            await context.SaveChangesAsync();

            return p;
        }

        #endregion

        #region Export video decomposition

        /// <summary>
        /// Exporte la décomposition d'une vidéo d'un scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        public virtual async Task<Stream> ExportVideoDecomposition(int projectId, int scenarioId, int videoId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    var videoDecomposition = await new VideoDecompositionExporter(context, projectId, scenarioId, videoId).CreateExport();

                    var ms = new MemoryStream();
                    var writer = XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false);
                    var ser = new NetDataContractSerializer();
                    ser.WriteObject(writer, videoDecomposition);
                    writer.Close();

                    ms.Seek(0, SeekOrigin.Begin);

                    return ms;
                }
            });

        /// <summary>
        /// Prédit les référentiels potentiels à merger.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="stream">Le flux de données contenant l'export.</param>
        public virtual async Task<VideoDecompositionImport> PredictMergedReferentialsVideoDecomposition(int projectId, Stream stream) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    NetDataContractSerializer ser = new NetDataContractSerializer()
                    {
                        Binder = new SerializationOperations.AnyVersionSerializationBinder(),
                    };
                    VideoDecompositionExport videoDecomposition = (VideoDecompositionExport)ser.ReadObject(stream);

                    stream.Close();

                    // Charger les référentiels
                    Referentials dbStandardReferentials = await LoadAllStandardReferentials(context);
                    Referentials dbProjectReferentials = await LoadAllProjectReferentials(context, (await context.Projects.SingleAsync(p => p.ProjectId == projectId)).ProcessId);

                    Dictionary<IActionReferentialProcess, IActionReferential> referentialsProject = new Dictionary<IActionReferentialProcess, IActionReferential>();
                    Dictionary<IActionReferential, IActionReferential> referentialsStd = new Dictionary<IActionReferential, IActionReferential>();

                    DetermineMergeCandidates(videoDecomposition.ReferentialsProject, dbStandardReferentials, referentialsProject);
                    DetermineMergeCandidates(videoDecomposition.ReferentialsStandard, dbStandardReferentials, referentialsStd);

                    DetermineMergeCandidates(videoDecomposition.ReferentialsProject, dbProjectReferentials, referentialsProject);
                    DetermineMergeCandidates(videoDecomposition.ReferentialsStandard, dbProjectReferentials, referentialsStd);

                    return new VideoDecompositionImport()
                    {
                        ExportedVideoDecomposition = videoDecomposition,
                        ProjectReferentialsMergeCandidates = referentialsProject,
                        StandardReferentialsMergeCandidates = referentialsStd,
                    };
                }
            });

        /// <summary>
        /// Importe décomposition d'une vidéo d'un scénario dans le scénario initial existant d'un projet.
        /// </summary>
        /// <param name="videoDecomposition">Les données à importer.</param>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier qui contient les vidéos.</param>
        /// <param name="targetProjectId">L'identifiant du projet cible.</param>
        public virtual async Task<bool> ImportVideoDecomposition(VideoDecompositionImport videoDecomposition, bool mergeReferentials, string videosDirectory, int targetProjectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext())
                {
                    var scenario = await context.Scenarios
                        .Include("Project")
                        .FirstOrDefaultAsync(s => s.ProjectId == targetProjectId &&
                            s.StateCode == KnownScenarioStates.Draft &&
                            s.NatureCode == KnownScenarioNatures.Initial);

                    await Queries.LoadScenariosDetails(context, EnumerableExt.Concat(scenario), null);

                    if (scenario == null)
                        return false;

                    await new VideoDecompositionImporter(context, videoDecomposition, scenario).ImportVideoDecomposition(mergeReferentials, videosDirectory);

                    return true;
                }
            });

        #endregion

        /// <summary>
        /// Charge tous les référentiels standards.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <returns>Les référentiels chargés.</returns>
        private static async Task<Referentials> LoadAllStandardReferentials(KsmedEntities context)
        {

            var dbStandardReferentials = new Referentials
            {
                Categories = await context.ActionCategories.OfType<ActionCategory>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Equipments = await context.Resources.OfType<Equipment>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Operators = await context.Resources.OfType<Operator>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref1s = await context.Refs1.OfType<Ref1>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref2s = await context.Refs2.OfType<Ref2>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref3s = await context.Refs3.OfType<Ref3>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref4s = await context.Refs4.OfType<Ref4>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref5s = await context.Refs5.OfType<Ref5>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref6s = await context.Refs6.OfType<Ref6>().Where(r => r.ProcessId == null).ToArrayAsync(),
                Ref7s = await context.Refs7.OfType<Ref7>().Where(r => r.ProcessId == null).ToArrayAsync(),
            };
            return dbStandardReferentials;
        }

        /// <summary>
        /// Charge tous les référentiels projets du projet.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="projectId">L'ide du projet.</param>
        /// <returns>Les référentiels chargés.</returns>
        private static async Task<Referentials> LoadAllProjectReferentials(KsmedEntities context, int processId)
        {
            var dbProjectReferentials = new Referentials
            {
                Categories = await context.ActionCategories.OfType<ActionCategory>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Equipments = await context.Resources.OfType<Equipment>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Operators = await context.Resources.OfType<Operator>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref1s = await context.Refs1.OfType<Ref1>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref2s = await context.Refs2.OfType<Ref2>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref3s = await context.Refs3.OfType<Ref3>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref4s = await context.Refs4.OfType<Ref4>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref5s = await context.Refs5.OfType<Ref5>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref6s = await context.Refs6.OfType<Ref6>().Where(r => r.ProcessId == processId).ToArrayAsync(),
                Ref7s = await context.Refs7.OfType<Ref7>().Where(r => r.ProcessId == processId).ToArrayAsync(),
            };
            return dbProjectReferentials;
        }

        /// <summary>
        /// Détermine les candidats pour la fusion des référentiels.
        /// </summary>
        /// <typeparam name="TCandidateReferential">Le type de référentiel pour les candidats.</typeparam>
        /// <typeparam name="TReplacementReferential">Le type de réferentiel pour les remplacements.</typeparam>
        /// <param name="referentials">Les référentiels à tester.</param>
        /// <param name="dbStandardReferentials">Les référentiels standard en base.</param>
        /// <param name="candidatesCollection">La collection de candidats au merge.</param>
        private void DetermineMergeCandidates<TCandidateReferential, TReplacementReferential>(TCandidateReferential[] referentials, Referentials dbStandardReferentials,
            IDictionary<TCandidateReferential, TReplacementReferential> candidatesCollection)
            where TCandidateReferential : IActionReferential
        {
            foreach (var referential in referentials)
            {
                IEnumerable<IActionReferential> collection;

                if (referential is ActionCategory)
                    collection = dbStandardReferentials.Categories;
                else if (referential is Equipment)
                    collection = dbStandardReferentials.Equipments;
                else if (referential is Operator)
                    collection = dbStandardReferentials.Operators;
                else if (referential is Ref1)
                    collection = dbStandardReferentials.Ref1s;
                else if (referential is Ref2)
                    collection = dbStandardReferentials.Ref2s;
                else if (referential is Ref3)
                    collection = dbStandardReferentials.Ref3s;
                else if (referential is Ref4)
                    collection = dbStandardReferentials.Ref4s;
                else if (referential is Ref5)
                    collection = dbStandardReferentials.Ref5s;
                else if (referential is Ref6)
                    collection = dbStandardReferentials.Ref6s;
                else if (referential is Ref7)
                    collection = dbStandardReferentials.Ref7s;
                else
                    throw new ArgumentOutOfRangeException(nameof(referentials));

                IActionReferential matchingReferential = collection.FirstOrDefault(r => string.Compare(r.Label, referential.Label, StringComparison.CurrentCultureIgnoreCase) == 0);

                if (matchingReferential != null && !candidatesCollection.ContainsKey(referential))
                    candidatesCollection[referential] = (TReplacementReferential)matchingReferential;
            }
        }

        /// <summary>
        /// Marque l'entité comme nouvelle.
        /// </summary>
        /// <param name="entity">L'entité.</param>
        private void MarkAsAdded(IObjectWithChangeTracker entity)
        {
            entity.ChangeTracker = new ObjectChangeTracker
            {
                State = ObjectState.Added
            };
        }


    }
}