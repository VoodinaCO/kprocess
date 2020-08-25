using KProcess.Common;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared.ImportExport
{
    /// <summary>
    /// Représente un composant capable d'importer une décomposition vidéo.
    /// </summary>
    internal class VideoDecompositionImporter
    {
        private KsmedEntities _context;
        private VideoDecompositionImport _import;
        private Scenario _targetScenario;

        private IDictionary<IActionReferential, IActionReferential> _referentialsToRemap;
        private KAction[] _actions;

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="import">La décomposition vidéo à importer.</param>
        /// <param name="targetScenario">Le scénario cible.</param>
        public VideoDecompositionImporter(KsmedEntities context, VideoDecompositionImport import, Scenario targetScenario)
        {
            _context = context;
            _import = import;
            _targetScenario = targetScenario;

            _referentialsToRemap = new Dictionary<IActionReferential, IActionReferential>();
        }

        /// <summary>
        /// Importe la décomposition vidéo spécifiée spécifié.
        /// </summary>
        /// <param name="mergeReferentials"><c>true</c> pour fusionner les référentiels.</param>
        /// <param name="videosDirectory">Le dossier où se situent les vidéos.</param>
        public async Task ImportVideoDecomposition(bool mergeReferentials, string videosDirectory)
        {
            var videoDecomposition = _import.ExportedVideoDecomposition;

            // Videos
            PrepareVideo(videosDirectory);

            // Referentials
            ImportReferentials(mergeReferentials);

            // Actions
            ImportActions();

            // Remapper les référentiels du projet, des actions et des vidéos
            RemapReferentials();

            FixupImportedActions();

            // Custom text & Numeric labels
            ImportCustomFieldsLabels();

            _context.DetectChanges();

            // Passer les entités mappées à non modifiées
            FixupReferentialsTrackingState();

            if (mergeReferentials)
            {
                ServicesDiagnosticsDebug.CheckNotInContext(_context, _referentialsToRemap.Keys);
                ServicesDiagnosticsDebug.CheckObjectStateManagerState(_context, System.Data.Entity.EntityState.Unchanged, _referentialsToRemap.Values);
            }

            await _context.SaveChangesAsync();

            ServicesDiagnosticsDebug.CheckReferentialsState();
        }

        void PrepareVideo(string videosDirectory)
        {
            var video = _import.ExportedVideoDecomposition.Video;
            video.VideoId = default(int);
            video.Process = _targetScenario.Project.Process;
            video.FilePath = IOHelper.ChangeDirectory(video.FilePath, videosDirectory);
        }

        private void ImportReferentials(bool mergeReferentials)
        {
            if (!mergeReferentials)
                ImportReferentials();
            else
                MergeReferentials();
        }

        private void ImportReferentials()
        {
            // Référentiels process
            foreach (var refe in _import.ExportedVideoDecomposition.ReferentialsProject)
            {
                refe.Process = _targetScenario.Project.Process;
            }

            // Référentiels standard
            foreach (var refe in _import.ExportedVideoDecomposition.ReferentialsStandard)
            {
                var refProject = ReferentialsFactory.CopyToNewProject(refe);
                _referentialsToRemap[refe] = refProject;

                // Associer au process
                refProject.Process = _targetScenario.Project.Process;
            }
        }

        private void MergeReferentials()
        {
            // Référentiels projet
            foreach (var refe in _import.ExportedVideoDecomposition.ReferentialsProject)
            {
                // Ajouter au tableau de remap s'il y a une correspondance.
                if (_import.ProjectReferentialsMergeCandidates.ContainsKey(refe))
                {
                    _referentialsToRemap[refe] = _import.ProjectReferentialsMergeCandidates[refe];
                }
                else
                {
                    refe.Process = _targetScenario.Project.Process;
                }
            }

            // Référentiels standard
            foreach (var refe in _import.ExportedVideoDecomposition.ReferentialsStandard)
            {
                if (_import.StandardReferentialsMergeCandidates.ContainsKey(refe))
                {
                    _referentialsToRemap[refe] = _import.StandardReferentialsMergeCandidates[refe];
                }
                else
                {
                    var refProject = ReferentialsFactory.CopyToNewProject(refe);

                    // Associer au projet
                    refProject.Process = _targetScenario.Project.Process;

                    _referentialsToRemap[refe] = refProject;
                }
            }
        }

        private void ImportActions()
        {
            // Trier les actions par WBS
            _actions = _import.ExportedVideoDecomposition.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();

            // Actions
            var parents = new Dictionary<KAction, KAction>();

            // Faire une liste des parents
            foreach (var action in _actions)
            {
                var parent = WBSHelper.GetParent(action, _actions);
                if (parent != null)
                    parents[action] = parent;
            }

            foreach (var action in _actions)
            {
                action.ActionId = default(int);

                action.OriginalActionId = null;
                SharedScenarioActionsOperations.ApplyNewReduced(action, KnownActionCategoryTypes.I);

                // Réinitialiser les timings
                // Ils seront ensuite automatiquement recalculés en fonction des pred/succ
                action.BuildStart = 0;
                action.BuildDuration = action.Duration;

                var parent = parents.ContainsKey(action) ? parents[action] : null;

                ActionsTimingsMoveManagement.InsertUpdateWBS(_targetScenario.Actions.ToArray(), action, parent, -1);
                _targetScenario.Actions.Add(action);
            }
        }

        private void RemapReferentials()
        {
            foreach (var oldReferential in _referentialsToRemap.Keys)
            {
                // Remapper la res par défaut de la vidéo
                if (oldReferential is Resource)
                    ReferentialsHelper.UpdateReferentialReferences(new Video[] { _import.ExportedVideoDecomposition.Video }, oldReferential, _referentialsToRemap[oldReferential]);

                // Remapper les actions
                ReferentialsHelper.UpdateReferentialReferences(_actions, oldReferential, _referentialsToRemap[oldReferential]);
            }
        }

        private void ImportCustomFieldsLabels()
        {
            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomNumericLabel))
                _targetScenario.Project.CustomNumericLabel = _import.ExportedVideoDecomposition.CustomNumericLabel;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomNumericLabel2))
                _targetScenario.Project.CustomNumericLabel2 = _import.ExportedVideoDecomposition.CustomNumericLabel2;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomNumericLabel3))
                _targetScenario.Project.CustomNumericLabel2 = _import.ExportedVideoDecomposition.CustomNumericLabel3;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomNumericLabel4))
                _targetScenario.Project.CustomNumericLabel4 = _import.ExportedVideoDecomposition.CustomNumericLabel4;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomTextLabel))
                _targetScenario.Project.CustomTextLabel = _import.ExportedVideoDecomposition.CustomTextLabel;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomTextLabel2))
                _targetScenario.Project.CustomTextLabel2 = _import.ExportedVideoDecomposition.CustomTextLabel2;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomTextLabel3))
                _targetScenario.Project.CustomTextLabel3 = _import.ExportedVideoDecomposition.CustomTextLabel3;

            if (!string.IsNullOrEmpty(_import.ExportedVideoDecomposition.CustomTextLabel4))
                _targetScenario.Project.CustomTextLabel4 = _import.ExportedVideoDecomposition.CustomTextLabel4;
        }

        private void FixupImportedActions()
        {
            var sortedActions = _targetScenario.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();
            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(sortedActions, false);
            ActionsTimingsMoveManagement.UpdateVideoGroupsTiming(sortedActions);
            ActionsTimingsMoveManagement.UpdateBuildGroupsTiming(sortedActions);
            _targetScenario.CriticalPathIDuration = ActionsTimingsMoveManagement.GetInternalCriticalPathDuration(_targetScenario);

            ActionsTimingsMoveManagement.DebugCheckAllWBS(_targetScenario);

        }

        private void FixupReferentialsTrackingState()
        {
            foreach (var newReferential in _referentialsToRemap.Values.Where(referential => referential.Id > 0))
                _context.ObjectStateManager.GetObjectStateEntry(newReferential).ChangeState(System.Data.Entity.EntityState.Unchanged);

        }

    }
}
