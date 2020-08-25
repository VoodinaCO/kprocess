using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared.Migration
{
    public class MigratedScenarioCloneManager : IScenarioCloneManager
    {
        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;
        private readonly ISharedScenarioActionsOperations _sharedScenarioActionsOperations;
        public MigratedScenarioCloneManager(
            ISecurityContext securityContext, 
            ILocalizationManager localizationManager, 
            ITraceManager traceManager,
            ISharedScenarioActionsOperations sharedScenarioActionsOperations)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
            _sharedScenarioActionsOperations = sharedScenarioActionsOperations;
        }


        /// <summary>
        /// Crée un scénario cible à partir d'un autre scénario, initial ou cible.
        /// </summary>
        /// <param name="tempContext">Le contexte.</param>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="sourceScenarioId">L'identifiant du scénario source.</param>
        /// <param name="natureCode">Le code de la nature.</param>
        /// <param name="save"><c>true</c> pour sauvegarder le scénario créé.</param>
        /// <returns>
        /// Le scénario créé
        /// </returns>
        public async Task<Scenario> CreateDerivatedScenario(KsmedEntities tempContext, int projectId, int sourceScenarioId, string natureCode, bool save)
        {
            Scenario fromScenario;
           // using (var tempContext = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
            //{
                // Charger les référentiels
                var referentialsUsed = await _sharedScenarioActionsOperations.GetReferentialsUse(tempContext, projectId);
                await Queries.LoadAllReferentialsOfProject(tempContext, projectId, referentialsUsed);
                var videos = await tempContext.Projects
                    .Include(nameof(Project.Process))
                    .Include($"{nameof(Project.Process)}.{nameof(Procedure.Videos)}")
                    .Where(p => p.ProjectId == projectId)
                    .SelectMany(p => p.Process.Videos)
                    .ToArrayAsync();
                fromScenario = await tempContext.Scenarios.FirstAsync(s => s.ScenarioId == sourceScenarioId);
                await Queries.LoadScenariosDetails(tempContext, EnumerableExt.Concat(fromScenario), referentialsUsed);
           // }
            return await CreateDerivatedScenario(tempContext, fromScenario, natureCode, save);
        }

        /// <summary>
        /// Crée un scénario cible à partir d'un autre scénario, initial ou cible.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="sourceScenario">Le scenario source.</param>
        /// <param name="natureCode">Le code de la nature.</param>
        /// <param name="save"><c>true</c> pour sauvegarder le scénario créé.</param>
        /// <returns>
        /// Le scénario créé
        /// </returns>
        public async Task<Scenario> CreateDerivatedScenario(KsmedEntities context, Scenario sourceScenario, string natureCode, bool save)
        {
            Scenario derivatedScenario;
          //  using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
          //  {
                var targetNumber = await context.Scenarios.Where(s =>
                s.ProjectId == sourceScenario.ProjectId &&
                s.NatureCode == natureCode)
                .CountAsync() + 1;

                derivatedScenario = await CreateDerivatedScenario(context, sourceScenario, natureCode, save, targetNumber);
         //   }
            return derivatedScenario;
        }

        /// <summary>
        /// Crée un scénario cible à partir d'un autre scénario, initial ou cible.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        /// <param name="sourceScenario">Le scenario source.</param>
        /// <param name="natureCode">Le code de la nature.</param>
        /// <param name="save"><c>true</c> pour sauvegarder le scénario créé.</param>
        /// <param name="targetNumber">Le numéro cible.</param>
        /// <returns>
        /// Le scénario créé
        /// </returns>
        private async Task<Scenario> CreateDerivatedScenario(KsmedEntities context, Scenario sourceScenario, string natureCode, bool save, int targetNumber)
        {
            // Charger les données du scénario source
            var newScenario = new Scenario();

            ActionCloneBehavior cloneBehavior;
            if (sourceScenario.NatureCode == KnownScenarioNatures.Initial && natureCode == KnownScenarioNatures.Target)
                cloneBehavior = ActionCloneBehavior.InitialToTarget;
            else if (sourceScenario.NatureCode == KnownScenarioNatures.Target && natureCode == KnownScenarioNatures.Target)
                cloneBehavior = ActionCloneBehavior.TargetToTarget;
            else if (sourceScenario.NatureCode == KnownScenarioNatures.Target && natureCode == KnownScenarioNatures.Realized)
                cloneBehavior = ActionCloneBehavior.TargetToRealized;
            else if (sourceScenario.NatureCode == KnownScenarioNatures.Realized && natureCode == KnownScenarioNatures.Initial)
                cloneBehavior = ActionCloneBehavior.RealizedToNewInitial;
            else if (sourceScenario.NatureCode == KnownScenarioNatures.Target && natureCode == KnownScenarioNatures.Initial)
                cloneBehavior = ActionCloneBehavior.TargetToNewInitial;
            else if (sourceScenario.NatureCode == KnownScenarioNatures.Initial && natureCode == KnownScenarioNatures.Initial)
                cloneBehavior = ActionCloneBehavior.InitialToNewInitial;
            else
                throw new InvalidOperationException("Conversion impossible pour ces scénarios");
            switch (natureCode)
            {
                case KnownScenarioNatures.Target:
                    newScenario.Label = _localizationManager.GetString("Business_AnalyzeService_TargetScenarioLabel") + " " + targetNumber;
                    break;
                case KnownScenarioNatures.Realized:
                    newScenario.Label = _localizationManager.GetString("Business_AnalyzeService_ValidationScenarioLabel");
                    break;
                case KnownScenarioNatures.Initial:
                    newScenario.Label = _localizationManager.GetString("Business_AnalyzeService_InitialScenarioLabel");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(natureCode));
            }

            newScenario.StateCode = KnownScenarioStates.Draft;
            newScenario.NatureCode = natureCode;
            if (cloneBehavior != ActionCloneBehavior.RealizedToNewInitial
                && cloneBehavior != ActionCloneBehavior.TargetToNewInitial
                && cloneBehavior != ActionCloneBehavior.InitialToNewInitial)
            {
                newScenario.ProjectId = sourceScenario.ProjectId;
                newScenario.Original = sourceScenario;
                newScenario.OriginalScenarioId = sourceScenario.ScenarioId;
            }
            newScenario.IsShownInSummary = true;
            newScenario.CriticalPathIDuration = sourceScenario.CriticalPathIDuration;

            string[] scenarioLAbels = await EnsureCanShowScenarioInSummary(newScenario, true);

            // Copier toutes les actions
            foreach (var action in sourceScenario.Actions.ToArray())
            {
                var newAction = CloneAction(action, cloneBehavior);

                newAction.OriginalActionId = action.ActionId;
                newAction.Original = action;
                if (newAction.Reduced != null)
                    newAction.Reduced.OriginalBuildDuration = action.BuildDuration;

                // S'il s'agit d'un scénario validé, utiliser les temps process en tant que temps vidéo
                if (cloneBehavior == ActionCloneBehavior.TargetToRealized)
                {
                    newAction.Start = newAction.BuildStart;
                    newAction.Finish = newAction.BuildFinish;
                }

                newScenario.Actions.Add(newAction);
            }

            _sharedScenarioActionsOperations.EnsureEmptySolutionExists(newScenario);
            _sharedScenarioActionsOperations.UdpateSolutionsApprovedState(newScenario);

            // Copier les liens prédécesseurs successeurs
            foreach (var action in sourceScenario.Actions.ToArray())
            {
                var newAction = newScenario.Actions.FirstOrDefault(a => a.OriginalActionId == action.ActionId);
                if (newAction != null)
                    foreach (var predecessor in action.Predecessors)
                    {
                        var newPredecessor = newScenario.Actions.FirstOrDefault(a => a.OriginalActionId == predecessor.ActionId);
                        if (newPredecessor != null)
                            newAction.Predecessors.Add(newPredecessor);
                    }
            }

            //Suppression des actions avec durée = 0
            if (cloneBehavior == ActionCloneBehavior.TargetToRealized || cloneBehavior == ActionCloneBehavior.TargetToTarget)
                ActionsRecursiveUpdate.RemoveEmptyDurationActionsAndGroupsFromNewScenario(newScenario);

            if (cloneBehavior != ActionCloneBehavior.TargetToRealized           // ToDelete ne s'applique pas aux scenarios validés
                && cloneBehavior != ActionCloneBehavior.InitialToNewInitial     // ToDelete ne s'applique pas aux scenarios initiaux
                && cloneBehavior != ActionCloneBehavior.TargetToNewInitial      // ToDelete ne s'applique pas aux scenarios initiaux
                && cloneBehavior != ActionCloneBehavior.RealizedToNewInitial)   // ToDelete ne s'applique pas aux scenarios initiaux
            {
                foreach (var newAction in newScenario.Actions)
                {
                    // Si la category associée est dite "à supprimer", modifier la tache optimisée à "à supprimer"
                    if (newAction.Category != null && newAction.Category.ActionTypeCode == KnownActionCategoryTypes.S)
                        _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.S);
                }
            }

            if (save)
            {
                context.Scenarios.ApplyChanges(newScenario);
                await context.SaveChangesAsync();
            }

            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(newScenario.Actions.ToArray(), false);
            ActionsTimingsMoveManagement.UpdateVideoGroupsTiming(newScenario.Actions.ToArray());
            ActionsTimingsMoveManagement.UpdateBuildGroupsTiming(newScenario.Actions.ToArray());
            newScenario.CriticalPathIDuration = ActionsTimingsMoveManagement.GetInternalCriticalPathDuration(newScenario);

            // Supprimer les liens vers les originaux car dans un autre projet
            if (cloneBehavior == ActionCloneBehavior.RealizedToNewInitial
                || cloneBehavior == ActionCloneBehavior.TargetToNewInitial
                || cloneBehavior == ActionCloneBehavior.InitialToNewInitial)
            {
                foreach (var action in newScenario.Actions)
                {
                    action.Original = null;
                    action.OriginalActionId = null;
                }

                newScenario.Original = null;
                newScenario.OriginalScenarioId = null;
            }

            return newScenario;
        }

        /// <summary>
        /// S'assure que la scénario spécifié puisse être affiché dans la synthèse.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="updateValue">si <c>true</c> met à jour la valeur de <see cref="Scenario.IsShownInSummary"/>.</param>
        /// <returns>
        ///   Un tableau contenant les noms des scénarii.
        /// </returns>
        public async Task<string[]> EnsureCanShowScenarioInSummary(Scenario scenario, bool updateValue)
        {
            using (var context = ContextFactory.GetNewContext())
            {
                string[] sc;

                if (updateValue || scenario.StateCode != KnownScenarioStates.Validated)
                {
                    sc = await context
                        .Scenarios
                        .Where(s =>
                            s.IsShownInSummary &&
                            s.StateCode != KnownScenarioStates.Validated &&
                            s.ScenarioId != scenario.ScenarioId &&
                            s.ProjectId == scenario.ProjectId)
                        .Select(s => s.Label)
                        .ToArrayAsync();
                }
                else
                    sc = new string[] { };

                if (sc.Length >= ServiceConst.DefaultMaxScenariosInSummary)
                    scenario.IsShownInSummary &= !updateValue;
                else
                    sc = null;
                return sc;
            }
        }



        /// <summary>
        /// Clone une action pour qu'elle soit utilisée dans un nouveau scénario.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cloneBehavior">The clone behavior.</param>
        /// <returns></returns>
        public KAction CloneAction(KAction action, ActionCloneBehavior cloneBehavior)
        {
            var newAction = new KAction();

            var originalValues = action.GetCurrentValues();

            // Ignorer certaines propriétés
            var excludedPropertyNames = new List<string> { "ActionId", "ScenarioId", "OriginalActionId", "Predecessors", "Successors", "Reduced" };

            // Quand on passe vers un scénario de validation, il ne faut pas copier la vignette car on supprime les leins avec les vidéos
            if (cloneBehavior == ActionCloneBehavior.TargetToRealized)
            {
                excludedPropertyNames.Add("Thumbnail");
                excludedPropertyNames.Add("IsThumbnailSpecific");
                excludedPropertyNames.Add("ThumbnailPosition");
            }

            // Vérifier que ces noms de propriétés soient corrects
            if (excludedPropertyNames.Except(originalValues.Keys).Any())
                throw new InvalidOperationException("Les noms de propriétés présents dans excludedPropertyNames ne sont pas valides.");

            foreach (var kvp in originalValues)
            {
                if (!excludedPropertyNames.Contains(kvp.Key))
                    newAction.SetPropertyValue(kvp.Key, kvp.Value);
            }

            // Copier les liens actions / référentiel
            CloneReferentialActionsLinks(action, newAction);

            // Copier la partie Reduced
            switch (cloneBehavior)
            {
                case ActionCloneBehavior.InitialToTarget:
                    {
                        // S'il existe un prétypage par la catégorie, il est prioritaire. Sinon, utiliser celui sur l'action s'il y en a.
                        string actionTypeCode;

                        if (action.Category != null && action.Category.ActionTypeCode != null)
                            actionTypeCode = action.Category.ActionTypeCode;
                        else
                            actionTypeCode = action.IsReduced ? action.Reduced.ActionTypeCode : KnownActionCategoryTypes.I;

                        _sharedScenarioActionsOperations.ApplyNewReduced(newAction, actionTypeCode);
                    }
                    break;

                case ActionCloneBehavior.TargetToTarget:

                    if (action.Reduced.ActionTypeCode == null)
                        throw new InvalidOperationException("Une action cible doit toujours être réduite avec un type");

                    if (!ActionsTimingsMoveManagement.GetIsSolutionApproved(action).GetValueOrDefault(true))
                    {
                        // Tache I provenant du grand parent, on n'applique pas le prétype mais on applique la réduc
                        _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.I);
                    }
                    else
                    {
                        switch (action.Reduced.ActionTypeCode)
                        {
                            case KnownActionCategoryTypes.I:
                                // Déterminer si la tâche provient d'un scénario ancêtre
                                if (action.OriginalActionId != null)
                                {
                                    // Tache I provenant du grand parent, on n'applique pas le prétype mais on applique la réduc
                                    _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.I);
                                }
                                else
                                {
                                    // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a

                                    string actionTypeCode;

                                    if (action.Category != null && action.Category.ActionTypeCode != null)
                                        actionTypeCode = action.Category.ActionTypeCode;
                                    else
                                        actionTypeCode = KnownActionCategoryTypes.I;

                                    _sharedScenarioActionsOperations.ApplyNewReduced(newAction, actionTypeCode);
                                }

                                break;

                            case KnownActionCategoryTypes.E:
                                // Tâche E, on conserve E et on applique la réduc
                                _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.E);
                                break;

                            case KnownActionCategoryTypes.S:
                                _sharedScenarioActionsOperations.ApplyReduced(action, newAction);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(action), new ArgumentOutOfRangeException(nameof(KAction.Reduced), new ArgumentOutOfRangeException(nameof(KActionReduced.ActionTypeCode))));
                        }
                    }

                    break;

                case ActionCloneBehavior.TargetToRealized:

                    // Pour les tâches externes, il faut les faire réapparaitre comme externes
                    // Pour les tâches internes, il faut qu'elle aient une partie réduite pour pouvoir les repasser à exerne.
                    switch (action.Reduced.ActionTypeCode)
                    {
                        case KnownActionCategoryTypes.I:
                            _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.I);
                            break;

                        case KnownActionCategoryTypes.E:
                            _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.E);
                            break;
                    }

                    break;


                case ActionCloneBehavior.Cascade:
                    _sharedScenarioActionsOperations.ApplyReduced(action, newAction);
                    break;

                case ActionCloneBehavior.RealizedToNewInitial:
                case ActionCloneBehavior.TargetToNewInitial:
                case ActionCloneBehavior.InitialToNewInitial:

                    // Pour les tâches externes, il faut les faire réapparaitre comme externes
                    // Pour les tâches internes, il faut qu'elle aient une partie réduite pour pouvoir les repasser à exerne.
                    if (action.Reduced != null)
                    {
                        switch (action.Reduced.ActionTypeCode)
                        {
                            case KnownActionCategoryTypes.I:
                                _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.I);
                                break;

                            case KnownActionCategoryTypes.E:
                                _sharedScenarioActionsOperations.ApplyNewReduced(newAction, KnownActionCategoryTypes.E);
                                break;
                        }
                    }

                    break;


                default:
                    throw new ArgumentOutOfRangeException(nameof(cloneBehavior));
            }

            return newAction;
        }

        /// <summary>
        /// Clone les liens Référentiel - Action de l'ancienne tâche pour les mettre dans la nouvelle tâche.
        /// </summary>
        /// <param name="oldAction">L'ancienne tâche.</param>
        /// <param name="newAction">La nouvelle tâche.</param>
        private void CloneReferentialActionsLinks(KAction oldAction, KAction newAction)
        {
            // Ref1
            foreach (var actionLink in oldAction.Ref1)
                newAction.Ref1.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref2
            foreach (var actionLink in oldAction.Ref2)
                newAction.Ref2.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref3
            foreach (var actionLink in oldAction.Ref3)
                newAction.Ref3.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref4
            foreach (var actionLink in oldAction.Ref4)
                newAction.Ref4.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref5
            foreach (var actionLink in oldAction.Ref5)
                newAction.Ref5.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref6
            foreach (var actionLink in oldAction.Ref6)
                newAction.Ref6.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));

            // Ref7
            foreach (var actionLink in oldAction.Ref7)
                newAction.Ref7.Add(ReferentialsFactory.CloneReferentialActionsLink(actionLink, true, false));
        }

 
    }
}