using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.KL2.Business.Impl.Shared
{
    /// <summary>
    /// Gère les applications de modifications sur des scénarios.
    /// </summary>
    public static class ActionsRecursiveUpdate
    {

        #region Champs privés

        private static readonly string[] _kActionPropertyNamesToCopy =
        {
            "Video",
            "Label",
            "Start",
            "Finish",
            "IsRandom",
            "CustomNumericValue",
            "CustomNumericValue2",
            "CustomNumericValue3",
            "CustomNumericValue4",
            "CustomTextValue",
            "CustomTextValue2",
            "CustomTextValue3",
            "CustomTextValue4",
        };

        private const string _predecessorsPropertyName = "Predecessors";

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Prédit les scénarios qui seront impactés par les modifications en attente de sauvegarde.
        /// </summary>
        /// <param name="sourceModifiedScenario">Le scenério source modifié.</param>
        /// <param name="allScenarios">Tous les scénarios.</param>
        /// <param name="actionsToDelete">Les actions à supprimer.</param>
        /// <returns>
        /// Les scénarios impactés.
        /// </returns>
        internal static Scenario[] PredictImpactedScenarios(Scenario sourceModifiedScenario, Scenario[] allScenarios, KAction[] actionsToDelete, KAction[] actionsWithUpdatedWBS)
        {
            var derivedScenarios = ScenarioActionHierarchyHelper.GetDerivedScenarios(sourceModifiedScenario, allScenarios);
            var scenariosToInspect = new List<Scenario>(derivedScenarios);
            var actions = GetActionsSortedWBS(sourceModifiedScenario);

            if (actions.Any(a => a.IsMarkedAsAdded))
            {
                scenariosToInspect.Clear();
            }
            else
            {
                foreach (var originalAction in actions)
                {
                    bool hasUpdatedWBS = actionsWithUpdatedWBS != null && actionsWithUpdatedWBS.Contains(originalAction);
                    if (originalAction.IsMarkedAsModified || hasUpdatedWBS)
                    {
                        var modifiedValues = originalAction.ChangeTracker.ModifiedValues;

                        // S'il y a une correspondance entre le nom d'une propriété modifiée et les propriétés qui sont impactantes
                        if (hasUpdatedWBS || modifiedValues.Keys.Intersect(_kActionPropertyNamesToCopy).Any())
                        {
                            // rechercher tous les scénarios qui possèdent ces actions dérivées
                            foreach (var derivedScenario in scenariosToInspect.ToArray())
                            {
                                var derivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario);
                                if (derivedAction != null)
                                    scenariosToInspect.Remove(derivedScenario);

                                if (!scenariosToInspect.Any())
                                    break;
                            }
                        }

                        if (modifiedValues.ContainsKey(ActionsTimingsMoveManagement.KActionBuildStartPropertyName) || modifiedValues.ContainsKey(ActionsTimingsMoveManagement.KActionBuildFinishPropertyName))
                        {
                            ActionsTimingsMoveManagement.GetOrignalModifiedBuildDurations(originalAction, out long originalDuration, out long modifiedDuration);

                            if (modifiedDuration != originalDuration)
                            {
                                // rechercher tous les scénarios qui possèdent ces actions dérivées
                                foreach (var derivedScenario in scenariosToInspect.ToArray())
                                {
                                    var derivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario);
                                    if (derivedAction != null && derivedAction.IsReduced)
                                        scenariosToInspect.Remove(derivedScenario);

                                    if (!scenariosToInspect.Any())
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            if (actionsToDelete != null)
            {
                foreach (var originalAction in actionsToDelete)
                {
                    // rechercher tous les scénarios qui possèdent ces actions dérivées
                    foreach (var derivedScenario in scenariosToInspect.ToArray())
                    {
                        var derivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario);
                        if (derivedAction != null)
                            scenariosToInspect.Remove(derivedScenario);
                    }
                    if (!scenariosToInspect.Any())
                        break;
                }
            }

            return derivedScenarios.Except(scenariosToInspect).ToArray();
        }

        /// <summary>
        /// Met à jour les actions récursivement sur les scénarios dérivés de celui spécifié.
        /// </summary>
        /// <param name="context">Le contexte EF.</param>
        /// <param name="sourceScenario">Le scénario source.</param>
        /// <param name="allScenarios">Tous les scénarios qui peuvent être impactés.</param>
        /// <param name="actionsToRemove">Les actions à supprimer manuellement.</param>
        internal static void UpdateActions(KsmedEntities context, Scenario sourceScenario, Scenario[] allScenarios,
            out KAction[] actionsToRemove, out IList<KAction> actionsWithOriginal)
        {
            var derivedScenarios = ScenarioActionHierarchyHelper.GetDerivedScenarios(sourceScenario, allScenarios);

            var actions = GetActionsSortedWBS(sourceScenario);
            actionsWithOriginal = new List<KAction>();

            foreach (var scenario in derivedScenarios)
            {
                // Mettre à jour IsGroup
                foreach (var action in scenario.Actions)
                    action.IsGroup = WBSHelper.HasChildren(action, scenario.Actions);
            }

            foreach (var originalAction in actions)
            {
                // J'enlève le IsMArkedAsModified car les 2 références sont la sauvegarde des actions depuis la construction et depuis l'optimisation
                // Or depuis la construction, en modification, le bout de code cidessous est déjà appelé 
                // Et depuis l'optimisation, il n'y a pas de changement de temps video
                if (originalAction.IsMarkedAsAdded /*|| originalAction.IsMarkedAsModified*/)
                {
                    var originalValues = originalAction.ChangeTracker.OriginalValues;
                    var modifiedValues = originalAction.ChangeTracker.ModifiedValues;

                    if (originalAction.IsMarkedAsAdded || modifiedValues.ContainsKey(ActionsTimingsMoveManagement.KActionStartPropertyName) || modifiedValues.ContainsKey(ActionsTimingsMoveManagement.KActionFinishPropertyName))
                    {
                        // Vérifier si le temps vidéo a changé
                        ActionsTimingsMoveManagement.GetOrignalModifiedVideoDurations(originalAction, out long originalDuration, out long modifiedDuration);

                        bool hasVideoDurationChanged = originalDuration != modifiedDuration;

                        // Si c'est une tâche créée et non dupliquée, le buildDuration est à 0, donc on doit le mettre à jour
                        //Sinon, si c'est une tâche dupliquée, on le laisse tel quel.
                        if (originalAction.BuildDuration == 0)
                        {
                            var paceRating = originalAction.Resource != null ? originalAction.Resource.PaceRating : 1d;
                            originalAction.BuildDuration = Convert.ToInt64(modifiedDuration * paceRating);
                        }
                    }
                }

                if (originalAction.IsMarkedAsAdded)
                {
                    // Si l'action est une action nouvelle dans un scénario cible, définir automatiquement la partie réduite
                    if (sourceScenario.NatureCode == KnownScenarioNatures.Target && originalAction.Reduced == null)
                    {
                        SharedScenarioActionsOperations.ApplyNewReduced(originalAction);
                    }

                    var originalActionKey = context.CreateEntityKey(KsmedEntities.KActionsEntitySetName, originalAction);
                    var parentOriginalAction = WBSHelper.GetParent(originalAction, actions);

                    foreach (var derivedScenario in derivedScenarios)
                    {
                        var derivedActions = GetActionsSortedWBS(derivedScenario);

                        // Rechercher le parent dans le scénario dérivé
                        var parentDerivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(parentOriginalAction, derivedScenario);

                        // Cloner l'action originale
                        var newAction = ScenarioCloneManager.CloneAction(originalAction, ActionCloneBehavior.Cascade);

                        // Assigner l'original
                        var originalActionForCurrentDerivedScenario = derivedScenario.Original == sourceScenario ? originalAction :
                            ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario.Original);
                        newAction.Original = originalActionForCurrentDerivedScenario;
                        actionsWithOriginal.Add(newAction);

                        // Insérer l'action clonée dans le scénario dérivé
                        ActionsTimingsMoveManagement.InsertUpdateWBS(
                            derivedActions, newAction, parentDerivedAction, WBSHelper.GetParts(originalAction.WBS).Last(),
                            (a, wbs) => EnsureTracking(a));

                        // Rafraichir les actions
                        derivedScenario.Actions.Add(newAction);
                        derivedActions = GetActionsSortedWBS(derivedScenario);

                        // Ajouter les mêmes prédécesseurs et successeurs
                        foreach (var originalPredecessor in originalAction.Predecessors)
                        {
                            var derivedPredecessor = ScenarioActionHierarchyHelper.GetDerivedAction(originalPredecessor, derivedScenario);
                            if (derivedPredecessor != null)
                            {
                                EnsureTracking(derivedPredecessor);
                                ActionsTimingsMoveManagement.AddPredecessor(derivedActions, newAction, derivedPredecessor);
                            }
                        }

                        foreach (var originalSuccessor in originalAction.Successors)
                        {
                            var derivedSuccessor = ScenarioActionHierarchyHelper.GetDerivedAction(originalSuccessor, derivedScenario);
                            if (derivedSuccessor != null)
                            {
                                EnsureTracking(derivedSuccessor);
                                ActionsTimingsMoveManagement.AddPredecessor(derivedActions, derivedSuccessor, newAction);
                            }
                        }

                        EnsureTracking(derivedScenario);
                        SharedScenarioActionsOperations.EnsureEmptySolutionExists(derivedScenario);
                        SharedScenarioActionsOperations.UdpateSolutionsApprovedState(derivedScenario);

                        ActionsTimingsMoveManagement.DebugCheckAllWBS(derivedActions);
                    }

                }
                else if (originalAction.IsMarkedAsModified)
                {
                    var originalValues = originalAction.ChangeTracker.OriginalValues;
                    var modifiedValues = originalAction.ChangeTracker.ModifiedValues;
                    var propertiesToCopyValues = new Dictionary<string, object>();

                    foreach (var propertyName in _kActionPropertyNamesToCopy)
                    {
                        if (modifiedValues.ContainsKey(propertyName))
                            propertiesToCopyValues[propertyName] = modifiedValues[propertyName];
                    }

                    // Vérifier si les reduced doit être impactés également
                    ActionsTimingsMoveManagement.GetOrignalModifiedBuildDurations(originalAction, out long originalDuration, out long modifiedDuration);

                    bool hasBuildDurationChanged = originalDuration != modifiedDuration;


                    foreach (var derivedScenario in derivedScenarios)
                    {
                        var derivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario);
                        if (derivedAction != null)
                        {
                            EnsureTracking(derivedAction);
                            foreach (var kvp in propertiesToCopyValues)
                                derivedAction.SetPropertyValue(kvp.Key, kvp.Value);

                            if (hasBuildDurationChanged)
                            {
                                if (derivedAction.IsReduced)
                                {
                                    // Modifier l'original duration et recalculer le temps final en fonction du gain
                                    EnsureTracking(derivedAction.Reduced);
                                    derivedAction.Reduced.OriginalBuildDuration = modifiedDuration;

                                    ActionsTimingsMoveManagement.UpdateTimingsFromReducedReduction(derivedAction);
                                }
                                else
                                {
                                    // Simplement recopier la durée
                                    derivedAction.BuildDuration = modifiedDuration;
                                }
                            }
                        }
                    }

                }
            }

            var toRemove = new List<KAction>();

            // Gérer les actions supprimées
            // EF gérant mal l'ordre des suppressions, ça créer une ConstraintException sur la FK OriginalActionId
            // Malheureusement un CascadeDelete est impossible puisque la FK est sur un même table
            if (sourceScenario.ChangeTracker.ObjectsRemovedFromCollectionProperties.ContainsKey("Actions"))
            {
                var removedActions = sourceScenario.ChangeTracker.ObjectsRemovedFromCollectionProperties["Actions"].ToArray();
                foreach (KAction originalAction in removedActions)
                {
                    EnsureTracking(originalAction);
                    toRemove.Add(originalAction);
                    originalAction.MarkAsUnchanged();

                    foreach (var derivedScenario in derivedScenarios)
                    {
                        var derivedAction = ScenarioActionHierarchyHelper.GetDerivedAction(originalAction, derivedScenario);
                        if (derivedAction != null)
                        {
                            var derivedActions = GetActionsSortedWBS(derivedScenario);

                            // Mettre à jour les WBS des autres actions
                            ActionsTimingsMoveManagement.DeleteUpdateWBS(derivedActions, derivedAction,
                                (a, wbs) => EnsureTracking(a));
                            EnsureTracking(derivedAction);
                            toRemove.Add(derivedAction);
                        }
                    }
                }

                // Il faut maintenant trier les actions à supprimer pour que la suppression se fasse dans le bon ordre
                toRemove.Reverse();
                actionsToRemove = toRemove.ToArray();
            }
            else
                actionsToRemove = new KAction[] { };

            sourceScenario.CriticalPathIDuration = ActionsTimingsMoveManagement.GetInternalCriticalPathDuration(sourceScenario);
            foreach (var scenario in derivedScenarios)
            {
                EnsureTracking(scenario);
                ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(scenario.Actions.ToArray(), false);
                ActionsTimingsMoveManagement.UpdateVideoGroupsTiming(scenario.Actions.ToArray());
                ActionsTimingsMoveManagement.UpdateBuildGroupsTiming(scenario.Actions.ToArray());
                scenario.CriticalPathIDuration = ActionsTimingsMoveManagement.GetInternalCriticalPathDuration(scenario);
            }
        }


        /// <summary>
        /// Supprime les actions dont la durée est 0 du scénario.
        /// Supprime également les groupes vides qui pourraient résulter des suppressions précédentes.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        internal static void RemoveEmptyDurationActionsAndGroupsFromNewScenario(Scenario scenario)
        {
            var emptyDurationActionsToDelete = new List<KAction>();
            var newTimings = new List<ActionTiming>();

            var derivedActions = GetActionsSortedWBS(scenario);

            // Mettre à jour IsGroup
            foreach (var action in derivedActions)
                action.IsGroup = WBSHelper.HasChildren(action, derivedActions);

            foreach (var action in derivedActions)
            {

                //On ne supprime pas si: la catégorie de type à supprimer &&  pas de parent dans le scenario n && originalBuildDuration!=0

                if (action.BuildDuration == 0 && !action.IsGroup &&
                    !(action.Category != null && action.Category.ActionTypeCode != null &&
                    action.Category.ActionTypeCode == KnownActionCategoryTypes.S &&
                    action.Original.Original == null &&
                    action.Reduced != null &&
                    action.Reduced.OriginalBuildDuration != 0))
                {
                    // Déplacer tous les pred vers ses succ
                    ActionsTimingsMoveManagement.MapAllPredToSucc(action, a => a.BuildDuration != 0 && !action.IsGroup, newTimings, false);

                    emptyDurationActionsToDelete.Add(action);
                    scenario.Actions.Remove(action);
                    action.Predecessors.Clear();
                    action.Successors.Clear();
                    action.MarkAsDeleted();
                }
            }

            //foreach (var action in actionsToDelete)
            //{
            //    // Mettre à jour les WBS des autres actions
            //    ActionsTimingsMoveManagement.DeleteUpdateWBS(derivedActions, action);
            //}

            if (scenario.NatureCode == KnownScenarioNatures.Realized)
            {
                foreach (var timing in newTimings)
                {
                    timing.Action.Start = timing.Start;
                    timing.Action.Finish = timing.Finish;
                }
            }

            //actionsToDelete.Clear();
            var emptyGroupActionsToDelete = new List<KAction>();

            var actionsFiltered = GetActionsSortedWBS(scenario);
            foreach (var action in actionsFiltered)
            {
                if (action.IsGroup && !WBSHelper.HasChildren(action, actionsFiltered))
                {
                    emptyGroupActionsToDelete.Add(action);
                    scenario.Actions.Remove(action);
                    action.MarkAsDeleted();
                }
            }

            //foreach (var action in emptyGroupActionsToDelete)
            //{
            //    // Mettre à jour les WBS des autres actions
            //    ActionsTimingsMoveManagement.DeleteUpdateWBS(derivedActions, action);
            //}

            // Mettre à jour les WBS des autres actions
            var tree = derivedActions.VirtualizeTree();
            emptyDurationActionsToDelete
                .Union(emptyGroupActionsToDelete)
                .ForEach(_ => tree.Remove(_));
            tree.ApplyWBS();


            ActionsTimingsMoveManagement.DebugCheckAllWBS(EnumerableExt.Concat(scenario));
            ActionsTimingsMoveManagement.DebugCheckPredSucc(scenario.Actions, false);
        }

        /// <summary>
        /// S'assure que le tracking est activé pour l'entité spécifiée.
        /// </summary>
        /// <param name="entity">L'entité.</param>
        private static void EnsureTracking(IObjectWithChangeTracker entity) =>
            entity.StartTracking();

        /// <summary>
        /// Obtient les actions d'un scénario triées par WBS.
        /// </summary>
        /// <param name="scenario">Le scenario.</param>
        /// <returns>Les actions triées.</returns>
        private static KAction[] GetActionsSortedWBS(Scenario scenario) =>
            scenario.Actions.OrderBy(a => a.WBSParts, new WBSHelper.WBSComparer()).ToArray();

        #endregion

    }
}