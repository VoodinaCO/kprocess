using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared.ImportExport
{
    /// <summary>
    /// Représente un composant capable de créer une décomposition vidéo exportable.
    /// </summary>
    internal class VideoDecompositionExporter
    {

        private KsmedEntities _context;
        private int _projectId;
        private int _scenarioId;
        private int _videoId;

        private KAction[] _actions;
        private Video _video;
        private Project _project;
        private IEnumerable<IActionReferential> _refStandard;
        private IEnumerable<IActionReferentialProcess> _refProject;

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        public VideoDecompositionExporter(KsmedEntities context, int projectId, int scenarioId, int videoId)
        {
            _context = context;
            _projectId = projectId;
            _scenarioId = scenarioId;
            _videoId = videoId;
        }

        /// <summary>
        /// Crée la décomposition exportable.
        /// </summary>
        /// <returns>La décomposition vidéo.</returns>
        public async Task<VideoDecompositionExport> CreateExport()
        {
            await LoadGlobalData();

            var targetActions = FilterActionsForVideo();
            LoadReferentials(targetActions);
            return CreateExport(targetActions);
        }

        /// <summary>
        /// Charge les données globales relatives à la décomposition.
        /// </summary>
        private async Task LoadGlobalData()
        {
            var referentialsUsed = await SharedScenarioActionsOperations.GetReferentialsUse(_context, _projectId);
            _actions = await _context.KActions
                .Include("Predecessors")
                .Where(a => a.ScenarioId == _scenarioId)
                .ToArrayAsync();

            await Queries.LoadActionsReferentials(_context, _actions, referentialsUsed);

            _video = await _context.Videos
                .Include("DefaultResource")
                .SingleAsync(v => v.VideoId == _videoId);

            _project = (await _context.Scenarios
                .Include("Project")
                .FirstAsync(s => s.ScenarioId == _scenarioId))
                .Project;
        }

        /// <summary>
        /// Filtre les actions pour la vidéo spécifiée.
        /// </summary>
        /// <returns>Les actions filtrées.</returns>
        private IList<KAction> FilterActionsForVideo()
        {
            var targetActions = _actions.Where(a => a.VideoId == _videoId).ToList();

            // Inclure également les actions qui sont simplement des groupes au dessus des actions ci-dessus
            foreach (var action in targetActions.ToArray())
            {
                var parent = WBSHelper.GetParent(action, _actions);
                while (parent != null)
                {
                    targetActions.AddNew(parent);
                    parent = WBSHelper.GetParent(parent, _actions);
                }
            }

            // Filtrer les actions : supprimer les prédécesseurs et successeurs qui ne sont pas dans la liste
            foreach (var action in targetActions)
            {
                action.Predecessors.RemoveWhere(p => !targetActions.Contains(p));
                action.Successors.RemoveWhere(p => !targetActions.Contains(p));
            }

            return targetActions;
        }

        /// <summary>
        /// Charge les référentiels pour les tâches.
        /// </summary>
        /// <param name="actions">Les tâches.</param>
        private void LoadReferentials(IEnumerable<KAction> actions)
        {
            Queries.LoadAllReferentials(_context, actions);

            _refStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(actions);
            _refProject = ReferentialsHelper.GetAllReferentialsProject(actions);

            if (_video.DefaultResource != null)
            {
                if ((_video.DefaultResource as IActionReferentialProcess)?.ProcessId != null)
                    _refProject = _refProject.Concat((IActionReferentialProcess)_video.DefaultResource);
                else if ((_video.DefaultResource as IActionReferentialProcess)?.ProcessId == null)
                    _refStandard = _refStandard.Concat((IActionReferential)_video.DefaultResource);
            }

            var allResources = _refStandard.OfType<Resource>().Union(_refProject.OfType<Resource>());

            // Filtrer les actions : supprimer les actions liés aux ressources qui ne sont pas dans la liste
            foreach (var resource in allResources)
                resource.Actions.RemoveWhere(p => !actions.Contains(p));
        }

        /// <summary>
        /// Crée l'élément exportable.
        /// </summary>
        /// <param name="actions">Les tâches.</param>
        /// <returns>La décomposition.</returns>
        private VideoDecompositionExport CreateExport(IList<KAction> actions)
        {
            var projectExport = new VideoDecompositionExport()
            {
                AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                Video = _video,
                Actions = actions.ToArray(),
                ReferentialsStandard = _refStandard.ToArray(),
                ReferentialsProject = _refProject.ToArray(),
                CustomTextLabel = _project.CustomTextLabel,
                CustomTextLabel2 = _project.CustomTextLabel2,
                CustomTextLabel3 = _project.CustomTextLabel3,
                CustomTextLabel4 = _project.CustomTextLabel4,
                CustomNumericLabel = _project.CustomNumericLabel,
                CustomNumericLabel2 = _project.CustomNumericLabel2,
                CustomNumericLabel3 = _project.CustomNumericLabel3,
                CustomNumericLabel4 = _project.CustomNumericLabel4,
            };

            return projectExport;
        }

    }
}