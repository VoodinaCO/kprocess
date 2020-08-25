using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Common;
using KProcess.Ksmed.Models;
using System.Diagnostics;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    /// <summary>
    /// Permet de gérer le timing et le déplacement d'actions.
    /// </summary>
    public static class ActionsTimingsMoveManagement
    {

        #region Timing

        /// <summary>
        /// Met à jour le début, la durée et la fin VIDEO sur les éléments de groupe
        /// </summary>
        /// <param name="actions">Les actions.</param>
        public static void UpdateVideoGroupsTiming(KAction[] actions)
        {
            var orderedActions = WBSHelper.OrderByWBS(actions).Reverse();
            foreach(var action in orderedActions)
            {
                var children = WBSHelper.GetChildren(action, actions);
                if (children.Any())
                {
                    var minStart = children.Min(c => c.Start);
                    var totalDuration = children.Sum(c => c.Duration);

                    action.Finish = minStart + totalDuration;
                    action.Start = minStart;
                }
            }
        }

        /// <summary>
        /// Met à jour le début, la durée et la fin PROCESS sur les éléments de groupe
        /// </summary>
        /// <param name="actions">Les actions.</param>
        public static void UpdateBuildGroupsTiming(KAction[] actions)
        {
            foreach (var action in actions)
            {
                var children = WBSHelper.GetDescendants(action, actions);
                if (children.Any())
                {
                    var minBuildStart = children.Min(c => c.BuildStart);
                    var maxBuildFinish = children.Max(c => c.BuildFinish);

                    action.BuildStart = minBuildStart;
                    action.BuildFinish = maxBuildFinish;
                }
            }
        }


        /// <summary>
        /// Met à jour le début, la durée et la fin sur les éléments de groupe
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="buildStartGetter">Le délégué qui obtient le début.</param>
        /// <param name="buildFinishGetter">Le délégué qui obtient la fin.</param>
        /// <param name="buildStartSetter">Le délégué qui définit le début.</param>
        /// <param name="buildFinishSetter">Le délégué qui définit la fin.</param>
        public static void UpdateBuildGroupsTiming(KAction[] actions,
            Func<KAction, long> buildStartGetter, Func<KAction, long> buildFinishGetter,
            Action<KAction, long> buildStartSetter, Action<KAction, long> buildFinishSetter)
        {
            // trier les actions de façon à ce que ce soient celles les plus indentées qui soient calculées en premier
            // Cela permet à des groupes imbriqués d'être correctement mis à jour
            var ac = actions.OrderByDescending(a => WBSHelper.IndentationFromWBS(a.WBS));

            foreach (var action in ac)
            {
                var children = WBSHelper.GetDescendants(action, actions);
                if (children.Any())
                {
                    var minBuildStart = children.Min(c => buildStartGetter(c));
                    var maxBuildFinish = children.Max(c => buildFinishGetter(c));

                    buildStartSetter(action, minBuildStart);
                    buildFinishSetter(action, maxBuildFinish);
                }
            }
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="useManagedPredSucc"><c>true</c> pour utiliser les collections managées de prédécesseurs et successeurs.</param>
        public static void FixPredecessorsSuccessorsTimings(KAction[] actions, bool useManagedPredSucc)
        {
            // Calculer le temps critique de chaque action
            var actionTimings = CreateActionTimings(actions, useManagedPredSucc, a => a.BuildStart, a => a.BuildFinish);
            var critical = CalculateCriticalTimes(actionTimings);

            FixPredecessorsSuccessorsTimings(critical, a => a.BuildStart, a => a.BuildFinish,
                (a, t) => a.BuildStart = t, (a, t) => a.BuildFinish = t);
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        /// <param name="actions">The actions.</param>
        /// <param name="useManagedPredSucc"><c>true</c> pour utiliser les collections managées de prédécesseurs et successeurs.</param>
        /// <param name="buildStartGetter">Le délégué qui obtient le début.</param>
        /// <param name="buildFinishGetter">Le délégué qui obtient la fin.</param>
        /// <param name="buildStartSetter">Le délégué qui définit le début.</param>
        /// <param name="buildFinishSetter">Le délégué qui définit la fin.</param>
        public static void FixPredecessorsSuccessorsTimings(KAction[] actions, bool useManagedPredSucc,
            Func<KAction, long> buildStartGetter, Func<KAction, long> buildFinishGetter,
            Action<KAction, long> buildStartSetter, Action<KAction, long> buildFinishSetter)
        {
            // Calculer le temps critique de chaque action
            var actionTimings = CreateActionTimings(actions, useManagedPredSucc, buildStartGetter, buildFinishGetter);
            var critical = CalculateCriticalTimes(actionTimings);

            FixPredecessorsSuccessorsTimings(critical, buildStartGetter, buildFinishGetter, buildStartSetter, buildFinishSetter);
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        /// <param name="critical">Les actions critiques.</param>
        /// <param name="buildStartGetter">Le délégué qui obtient le début.</param>
        /// <param name="buildFinishGetter">Le délégué qui obtient la fin.</param>
        /// <param name="buildStartSetter">Le délégué qui définit le début.</param>
        /// <param name="buildFinishSetter">Le délégué qui définit la fin.</param>
        private static void FixPredecessorsSuccessorsTimings(CriticalAction[] critical,
            Func<KAction, long> buildStartGetter, Func<KAction, long> buildFinishGetter,
            Action<KAction, long> buildStartSetter, Action<KAction, long> buildFinishSetter)
        {
            // Partir des noeuds de début
            foreach (var action in critical)
            {
                if (action.Predecessors.Any())
                {
                    var max = action.Predecessors.MaxWithValue(s => buildFinishGetter(s.Action));

                    var d = buildFinishGetter(action.Action) - buildStartGetter(action.Action);

                    buildStartSetter(action.Action, buildFinishGetter(max.Action));
                    buildFinishSetter(action.Action, buildFinishGetter(max.Action) + d);
                }
            }
        }

        /// <summary>
        /// Corrige les temps en fonction des prédécesseurs et successeurs de chaque action.
        /// </summary>
        /// <param name="actions">The actions.</param>
        public static void FixPredecessorsSuccessorsTimings(ActionTiming[] actions)
        {
            // Calculer le temps critique de chaque action
            var critical = CalculateCriticalTimes(actions);

            var actionsDic = actions.ToDictionary(at => at.Action);

            // Partir des noeuds de début
            foreach (var action in critical)
            {
                if (action.Predecessors.Any())
                {
                    var max = action.Predecessors.MaxWithValue(s => actionsDic[s.Action].Finish);

                    var d = actionsDic[action.Action].Finish - actionsDic[action.Action].Start;

                    actionsDic[action.Action].Start = actionsDic[max.Action].Finish;
                    actionsDic[action.Action].Finish = actionsDic[max.Action].Finish + d;
                }
            }
        }

        /// <summary>
        /// Crée des action timings standards autour des actions spécifiées.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <param name="useManagedPredSucc"><c>true</c> pour utiliser les collections managées de prédécesseurs et successeurs.</param>
        /// <param name="buildStartGetter">Le délégué qui obtient le début.</param>
        /// <param name="buildFinishGetter">Le délégué qui obtient la fin.</param>
        /// <returns>Les ActionTiming créés</returns>
        public static ActionTiming[] CreateActionTimings(IEnumerable<KAction> actions, bool useManagedPredSucc,
            Func<KAction, long> buildStartGetter, Func<KAction, long> buildFinishGetter)
        {
            var actionTimings = actions
                .ToDictionary(a => a,
                a => new ActionTiming
                {
                    Action = a,
                    Start = buildStartGetter(a),
                    Finish = buildFinishGetter(a),
                });

            foreach (var kvp in actionTimings)
            {
                var action = kvp.Key;
                var at = kvp.Value;

                if (useManagedPredSucc)
                {
                    at.Predecessors = action.PredecessorsManaged
                        .Where(p => actionTimings.ContainsKey(p))
                        .Select(p => actionTimings[p])
                        .ToList();

                    at.Successors = action.SuccessorsManaged
                        .Where(p => actionTimings.ContainsKey(p))
                        .Select(p => actionTimings[p])
                        .ToList();
                }
                else
                {
                    at.Predecessors = action.Predecessors
                        .Where(p => actionTimings.ContainsKey(p))
                        .Select(p => actionTimings[p])
                        .ToList();

                    at.Successors = action.Successors
                        .Where(p => actionTimings.ContainsKey(p))
                        .Select(p => actionTimings[p])
                        .ToList();
                }
            }

            return actionTimings.Values.ToArray();
        }

        #endregion

        #region Chemin critique

        /// <summary>
        /// Calcule les temps critiques des actions.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        /// <returns>
        /// Les temps critiques.
        /// </returns>
        public static CriticalAction[] CalculateCriticalTimes(IEnumerable<ActionTiming> actions)
        {
            var actionsCache = actions.Select(a => a.Action).ToArray();
            actions = actions.Where(a => !WBSHelper.HasChildren(a.Action, actionsCache));

            if (!actions.Any())
                return new CriticalAction[] { };

            var CPMActions = new List<CriticalAction>();
            foreach (var action in actions)
                CriticalAction.GetOrCreate(action, actions, CPMActions);

            // Ajouter un début pour tous les débuts
            var firstActions = CPMActions.Where(l => !l.Predecessors.Any());
            if (firstActions.Count() > 1)
            {
                var action = new CriticalAction();
                foreach (var first in firstActions)
                {
                    action.Successors.Add(first);
                    first.Predecessors.Add(action);
                }
                CPMActions.Insert(0, action);
            }

            // Ajouter une fin pour toutes les fins
            var lastActions = CPMActions.Where(l => !l.Successors.Any());
            if (lastActions.Count() > 1)
            {
                var action = new CriticalAction();
                foreach (var last in lastActions)
                {
                    last.Successors.Add(action);
                    action.Predecessors.Add(last);
                }
                CPMActions.Add(action);
            }

            CalcCriticalTime(CPMActions.Single(a => !a.Successors.Any()));

            var ordered = CPMActions
                .Where(a => a.Action != null)
                .OrderBy(a => a, new CriticalAction());

            var fakes = CPMActions.Where(a => a.Action == null);
            foreach (var item in ordered)
            {
                foreach (var fake in fakes)
                {
                    item.Predecessors.Remove(fake);
                    item.Successors.Remove(fake);
                }
            }

            return ordered.ToArray();
        }

        /// <summary>
        /// Calcule le temps critique de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        private static void CalcCriticalTime(CriticalAction action)
        {
            if (!action.Successors.Any())
                action.CriticalTime = 0;

            foreach (var p in action.Predecessors)
                p.CriticalTime = Math.Max(p.CriticalTime, action.CriticalTime + p.Duration);

            foreach (var p in action.Predecessors)
                CalcCriticalTime(p);
        }

        /// <summary>
        /// Calcule la durée du chemin critique d'un scénario avec un filtre I.
        /// </summary>
        /// <param name="s">Le scénario.</param>
        /// <returns>
        /// La durée du chemin critique.
        /// </returns>
        public static long GetInternalCriticalPathDuration(Scenario s)
        {
            if (!s.Actions.Any())
                return 0;

            var actionTimings = CreateActionTimingsI(s.Actions);
            FixPredecessorsSuccessorsTimings(actionTimings);

            if (actionTimings.Any())
            {
                return
                    actionTimings.Max(a => a.Finish) -
                    actionTimings.Min(a => a.Start);
            }
            else
                return 0;
        }

        /// <summary>
        /// Met à jour les prédécesseurs et successeurs managés dans le cadre de l'utilisation d'un filtre I.
        /// </summary>
        /// <param name="allActions">Les actions.</param>
        /// <returns>Les ActionTimings créés.</returns>
        public static ActionTiming[] CreateActionTimingsI(IEnumerable<KAction> allActions)
        {
            var actionsSorted = allActions
                .OrderBy(a => a.WBSParts, new WBSHelper.WBSComparer())
                .ToArray();

            var actionsTimings = actionsSorted
                .ToDictionary(a => a,
                a => new ActionTiming
                {
                    Action = a,
                    Start = a.BuildStart,
                    Finish = a.BuildFinish,
                    Predecessors = new List<ActionTiming>(),
                    Successors = new List<ActionTiming>(),
                });

            var actionsTimingsFinal = new List<ActionTiming>();

                var actionsTimingsByWBS = actionsSorted
                .ToDictionary(a => a.WBS,
                a => new ActionTiming
                {
                    Action = a,
                    Start = a.BuildStart,
                    Finish = a.BuildFinish,
                    Predecessors = new List<ActionTiming>(),
                    Successors = new List<ActionTiming>(),
                });
            // Commencer par mapper les prédécesseurs et les successeurs
            foreach (var kvp in actionsTimings)
            {
                    var action = kvp.Key;
                    var at = kvp.Value;

                    foreach (var pred in action.Predecessors)
                        at.Predecessors.Add(actionsTimingsByWBS[pred.WBS]);
                    foreach (var succ in action.Successors)
                        at.Successors.Add(actionsTimingsByWBS[succ.WBS]);
            }


            foreach (var kvp in actionsTimings)
            {
                var action = kvp.Key;
                var at = kvp.Value;

                if (IsActionExternal(action) || IsActionDeleted(action))
                {
                    // Déplacer tous les pred vers ses succ
                    MapAllPredToSuccManaged(at, a => IsActionInternal(a.Action));
                }
                else if (IsActionInternal(action))
                {
                    actionsTimingsFinal.Add(at);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(action.IsGroup);
                }
            }

            // Supprimer les liens pour les actions non présentes dans la collection
            foreach (var at in actionsTimingsFinal)
            {
                foreach (var pred in at.Predecessors.ToArray())
                {
                    if (!actionsTimingsFinal.Contains(pred))
                    {
                        at.Predecessors.Remove(pred);
                        pred.Successors.Remove(at);
                    }
                }

                foreach (var succ in at.Successors.ToArray())
                {
                    if (!actionsTimingsFinal.Contains(succ))
                    {
                        at.Successors.Remove(succ);
                        succ.Predecessors.Remove(at);
                    }
                }
            }

            return actionsTimingsFinal.ToArray();
        }

        /// <summary>
        /// Mappe les prédécesseurs et successeurs de l'ActioNTiming spécifiée en appliquant le filtre;
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="filter">Le filtre.</param>
        private static void MapAllPredToSuccManaged(ActionTiming action, Func<ActionTiming, bool> filter)
        {
            var finalPredecessors = new List<ActionTiming>();
            var finalSuccessors = new List<ActionTiming>();

            SearchPredSuccHierarchy(true, action, finalPredecessors, filter);
            SearchPredSuccHierarchy(false, action, finalSuccessors, filter);

            foreach (var pred in finalPredecessors)
            {
                foreach (var succ in finalSuccessors)
                {
                    pred.Successors.AddNew(succ);
                    succ.Predecessors.AddNew(pred);
                }
            }

        }

        /// <summary>
        /// Cherche un prédécesseur ou un successeur dans toute la hiérarchie 
        /// </summary>
        /// <param name="useManagedPredSucc"><c>true</c> pour utiliser les collections managées de prédécesseurs et successeurs.</param>
        /// <param name="currentAction">L'action.</param>
        /// <param name="actions">Toutes les actions.</param>
        /// <param name="filter">Le filtre définissant quand s'arrêter.</param>
        private static void SearchPredSuccHierarchy(bool usePredecessors, ActionTiming currentAction, List<ActionTiming> actions, Func<ActionTiming, bool> filter)
        {
            var col = usePredecessors ? currentAction.Predecessors : currentAction.Successors;
            foreach (var pred in col)
            {
                if (filter(pred))
                    actions.Add(pred);
                else
                    SearchPredSuccHierarchy(usePredecessors, pred, actions, filter);
            }
        }

        /// <summary>
        /// Détermine si une action est I.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>
        ///   <c>true</c> si l'action est I.; sinon, <c>false</c>.
        /// </returns>
        public static bool IsActionInternal(KAction action)
        {
            if (action.IsGroup)
                return false;

            if (!action.IsReduced)
                return true;

            var isApproved = GetIsSolutionApproved(action);
            if (isApproved.HasValue && !isApproved.Value)
                return true;

            return action.Reduced.ActionTypeCode == KnownActionCategoryTypes.I;
        }

        /// <summary>
        /// Détermine si une action est E.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>
        ///   <c>true</c> si l'action est E.; sinon, <c>false</c>.
        /// </returns>
        public static bool IsActionExternal(KAction action)
        {
            if (action.IsGroup)
                return false;

            if (!action.IsReduced)
                return false;

            if (!action.Reduced.Approved)
            {
                var isApproved = GetIsSolutionApproved(action);
                if (isApproved.HasValue && !isApproved.Value)
                    return false;
            }

            return action.Reduced.ActionTypeCode == KnownActionCategoryTypes.E;
        }

        /// <summary>
        /// Détermine si une action est S.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>
        ///   <c>true</c> si l'action est S.; sinon, <c>false</c>.
        /// </returns>
        public static bool IsActionDeleted(KAction action)
        {
            if (action.IsGroup)
                return false;

            if (action.Scenario.NatureCode == KnownScenarioNatures.Initial ||
                action.Scenario.NatureCode == KnownScenarioNatures.Realized)
                return false;

            if (!action.IsReduced)
                return false;

            var isApproved = GetIsSolutionApproved(action);
            if (isApproved.HasValue && !isApproved.Value)
                return false;

            return action.Reduced.ActionTypeCode == KnownActionCategoryTypes.S;
        }

        #endregion

        #region WBS

        /// <summary>
        /// Met à jour les WBS en vue d'une insertion d'une action.
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        /// <param name="newAction">La nouvelle action.</param>
        /// <param name="parent">le parent de la nouvelle action ou <c>null</c> si indentation 0.</param>
        /// <param name="targetWbsLevel">Le niveau WBS souhaité. -1 pour l'ajouter à la fin du niveau.</param>
        /// <param name="callbackBefore">Une action éventuelle à exécuter avant de modifier chaque WBS.</param>
        public static void InsertUpdateWBS(KAction[] allActions, KAction newAction, KAction parent,
            int targetWbsLevel = -1,
            Action<KAction, string> callbackBefore = null)
        {
            int insertIndentation;
            int[] finalWBS;

            if (parent != null)
                insertIndentation = WBSHelper.IndentationFromWBS(parent.WBS) + 1;
            else
                insertIndentation = 0;

            // Prendre au maximum le niveau max possible en fonction des actions existantes
            var levelsFirstValues = parent != null ? WBSHelper.GetParts(parent.WBS) : new int[] { };

            var atSameLevel = WBSHelper.GetActionsAtIndentationLevel(insertIndentation, levelsFirstValues, allActions);
            if (atSameLevel.Any())
            {
                var max = atSameLevel.Max(a => WBSHelper.GetParts(a.WBS).Last()) + 1;

                if (targetWbsLevel == -1)
                    targetWbsLevel = max;
                else
                    targetWbsLevel = Math.Min(targetWbsLevel, max);
            }

            if (targetWbsLevel == -1)
                targetWbsLevel = 1;

            if (parent != null)
            {
                var parentParts = WBSHelper.GetParts(parent.WBS);
                finalWBS = parentParts.Concat(new int[] { targetWbsLevel }).ToArray();
            }
            else
                finalWBS = new int[] { targetWbsLevel };

            var previousAction = WBSHelper.GetLastPrecedingSibling(WBSHelper.LevelsToWBS(finalWBS), allActions);
            if (previousAction != null)
            {
                // Décaler vers le bas tous les éléments à partir de l'index spécifié
                var successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(previousAction.WBS, allActions);
                foreach (var a in successiveSiblings)
                {
                    var wbs = WBSHelper.MoveDown(a.WBS, insertIndentation);
                    if (callbackBefore != null)
                        callbackBefore(a, wbs);
                    a.WBS = wbs;
                }
            }

            var w = WBSHelper.LevelsToWBS(finalWBS);
            if (callbackBefore != null)
                callbackBefore(newAction, w);
            newAction.WBS = w;
        }

        /// <summary>
        /// Met à jour les WBS en vue d'une suppression d'une action.
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        /// <param name="action">L'action à supprimer.</param>
        /// <param name="callbackBefore">Une action éventuelle à exécuter avant de modifier chaque WBS.</param>
        public static void DeleteUpdateWBS(KAction[] allActions, KAction action, Action<KAction, string> callbackBefore = null)
        {
            var actionIndex = Array.IndexOf(allActions, action);
            var actionIndentation = WBSHelper.IndentationFromWBS(action.WBS);

            var nextActionIndex = actionIndex + 1;
            if (nextActionIndex < allActions.Length)
            {
                var nextAction = allActions[nextActionIndex];
                if (nextAction != null)
                {
                    var nextItemIndentation = WBSHelper.IndentationFromWBS(nextAction.WBS);

                    // Déterminer si à l'index + 1, l'élément pourra garder son indentation
                    // Les else if ont un ordre bien particulier à respecter
                    bool shouldDecreaseIndentation;
                    if (nextItemIndentation == 0)
                        shouldDecreaseIndentation = false;
                    else if (actionIndex - 1 < 0)
                        shouldDecreaseIndentation = true;
                    else if (nextItemIndentation <= actionIndentation)
                        shouldDecreaseIndentation = false;
                    else
                        shouldDecreaseIndentation = true;

                    // Désindenter l'action suivant l'action à supprimer
                    if (shouldDecreaseIndentation)
                        Unindent(allActions, nextAction, UnindentationBehavior.PutInline);
                }
            }

            var children = WBSHelper.GetDescendants(action, allActions).ToArray();
            var successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(action, allActions).ToArray();

            // On détermine, pour les enfants actuels, à partir de combien il faudra reprendre la numérotation
            int startIndex = 0;
            var nextParent = WBSHelper.GetLastPrecedingSibling(action.WBS, allActions);
            if (nextParent != null)
            {
                var lastChild = WBSHelper.GetLastChild(nextParent, allActions);
                if (lastChild != null)
                    startIndex = WBSHelper.GetNumberAtLevel(lastChild.WBS, actionIndentation + 1);
            }

            foreach (var child in children)
            {
                var wbs = WBSHelper.MoveUp(child.WBS, actionIndentation);
                wbs = WBSHelper.SetNumberAtLevel(wbs, actionIndentation + 1, WBSHelper.GetNumberAtLevel(wbs, actionIndentation + 1) + startIndex);

                if (callbackBefore != null)
                    callbackBefore(child, wbs);
                child.WBS = wbs;
            }

            foreach (var sibling in successiveSiblings)
            {
                var wbs = WBSHelper.MoveUp(sibling.WBS, actionIndentation);
                if (callbackBefore != null)
                    callbackBefore(sibling, wbs);
                sibling.WBS = wbs;
            }
        }

        /// <summary>
        /// Désindente l'action spécifié.
        /// Elle n'est pas censée avoir d'enfants.
        /// </summary>
        /// <param name="allActions">Les actions.</param>
        /// <param name="action">L'action.</param>
        /// <param name="behavior">The behavior.</param>
        public static void Unindent(KAction[] allActions, KAction action, UnindentationBehavior behavior)
        {
            var actionIndentation = WBSHelper.IndentationFromWBS(action.WBS);

            // toutes les actions qui sont au niveau de l'élément mais en dessous, doivent être remontées d'un cran

            KAction[] futureSiblings = null;

            switch (behavior)
            {
                case UnindentationBehavior.PutAbove:

                    var successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(action, allActions);
                    foreach (var a in successiveSiblings)
                        a.WBS = WBSHelper.MoveUp(a.WBS, actionIndentation);

                    action.WBS = WBSHelper.Unindent(action.WBS);
                    break;

                case UnindentationBehavior.PutBelow:

                    successiveSiblings = WBSHelper.GetSucessiveSliblingsAndDescendents(action, allActions);
                    foreach (var a in successiveSiblings)
                        a.WBS = WBSHelper.MoveUp(a.WBS, actionIndentation);

                    action.WBS = WBSHelper.Unindent(action.WBS);
                    action.WBS = WBSHelper.MoveDown(action.WBS, actionIndentation - 1);
                    break;

                case UnindentationBehavior.PutInline:

                    var children = WBSHelper.GetDescendants(action, allActions).ToArray();

                    var successiveSiblingsCache = WBSHelper.GetSucessiveSliblingsAndDescendents(action, allActions).ToArray();

                    var actionWBSExceptLastPart = WBSHelper.GetParts(action.WBS);
                    actionWBSExceptLastPart = actionWBSExceptLastPart.Take(actionWBSExceptLastPart.Length - 2).ToArray();

                    futureSiblings = allActions.Where(a =>
                        WBSHelper.IndentationFromWBS(a.WBS) >= actionIndentation - 1 &&
                        WBSHelper.GetNumberAtLevel(a.WBS, actionIndentation - 1) > WBSHelper.GetNumberAtLevel(action.WBS, actionIndentation - 1) &&
                        WBSHelper.StartsWith(WBSHelper.GetParts(a.WBS), actionWBSExceptLastPart))
                        .ToArray();

                    action.WBS = WBSHelper.Unindent(action.WBS);
                    action.WBS = WBSHelper.MoveDown(action.WBS, actionIndentation - 1);

                    // Pour tous les enfants, désindenter
                    // Ex: la désindentation de T2 entraîne la désindentation de T21 et T22

                    //  G1 1
                    //      T1 1.1
                    //      T2 1.2
                    //          T21 1.2.1
                    //          T22 1.2.2
                    //      T3 1.3

                    foreach (var child in children)
                    {
                        child.WBS = WBSHelper.Unindent(child.WBS, WBSHelper.IndentationFromWBS(action.WBS) + 1);
                        for (int i = 0; i < WBSHelper.IndentationFromWBS(action.WBS) + 1; i++)
                            child.WBS = WBSHelper.CopyNumberAtLevel(action.WBS, child.WBS, i);
                    }

                    int upDownDiff = -1;
                    if (successiveSiblingsCache.Any())
                    {
                        var firstSibling = successiveSiblingsCache.First();
                        var childrenIndentation = WBSHelper.IndentationFromWBS(action.WBS) + 1;
                        if (children.Any())
                        {
                            var lastChildWithSameIndentation = children.LastOrDefault(c =>
                                WBSHelper.IndentationFromWBS(c.WBS) == childrenIndentation);
                            var expectedNumber = WBSHelper.GetNumberAtLevel(lastChildWithSameIndentation.WBS, childrenIndentation) + 1;
                            upDownDiff = expectedNumber - WBSHelper.GetNumberAtLevel(firstSibling.WBS, childrenIndentation);
                        }
                        else
                            upDownDiff = 1 - WBSHelper.GetNumberAtLevel(firstSibling.WBS, childrenIndentation);
                    }

                    // En suivant l'exemple précédent, il faut que T3 suive T21 et T22
                    foreach (var a in successiveSiblingsCache)
                    {
                        if (upDownDiff < 0)
                            for (int i = 0; i < Math.Abs(upDownDiff); i++)
                                a.WBS = WBSHelper.MoveUp(a.WBS, actionIndentation);
                        else
                            for (int i = 0; i < Math.Abs(upDownDiff); i++)
                                a.WBS = WBSHelper.MoveDown(a.WBS, actionIndentation);

                        a.WBS = WBSHelper.MoveDown(a.WBS, actionIndentation - 1);
                    }

                    break;

                default:
                    break;
            }

            // L'action prennant un WBS, tous les WBS en dessous doivent être modifiés
            if (behavior == UnindentationBehavior.PutInline)
                IncreaseWBSFor(actionIndentation - 1, 1, futureSiblings);
            else
            {
                IncreaseWbsForAllDown(allActions, actionIndentation - 1, WBSHelper.GetNumberAtLevel(action.WBS, actionIndentation - 1), 1);
                actionIndentation--;
                action.WBS = WBSHelper.MoveUp(action.WBS, actionIndentation);
            }
        }

        /// <summary>
        /// Augmente le WBS pour tous les éléments spécifiés.
        /// </summary>
        /// <param name="level">Le niveau d'indentation.</param>
        /// <param name="increment">L'incrément.</param>
        /// <param name="targettedActions">Les actions cibles.</param>
        private static void IncreaseWBSFor(int level, int increment, IEnumerable<KAction> targettedActions)
        {
            foreach (var action in targettedActions)
            {
                if (increment < 0)
                {
                    for (int i = 0; i > increment; i--)
                        action.WBS = WBSHelper.MoveUp(action.WBS, level);
                }
                else
                {
                    for (int i = 0; i < increment; i++)
                        action.WBS = WBSHelper.MoveDown(action.WBS, level);
                }
            }
        }

        /// <summary>
        /// Augmente le WBS pour tous les éléments se trouvant en dessous de celui spécifié.
        /// </summary>
        /// <param name="level">Le niveau d'indentation.</param>
        /// <param name="start">Le début.</param>
        /// <param name="increment">L'incrément.</param>
        private static void IncreaseWbsForAllDown(KAction[] allActions, int level, int start, int increment)
        {
            var targettedActions = allActions.Where(a =>
                WBSHelper.IndentationFromWBS(a.WBS) >= level && WBSHelper.GetNumberAtLevel(a.WBS, level) >= start);
            IncreaseWBSFor(level, increment, targettedActions);
        }

        /// <summary>
        /// Trace le WBS des actions chargées.
        /// </summary>
        public static void TraceAllWBS(IEnumerable<KAction> actions)
        {
            var sb = new StringBuilder();
            foreach (var action in actions)
            {
                var indentation = WBSHelper.IndentationFromWBS(action.WBS);
                for (int i = 0; i < indentation; i++)
                    sb.Append("  ");
                sb.Append(action.Label ?? string.Empty);
                sb.Append(" ");
                sb.Append(action.WBS);
                sb.AppendLine();
            }
            TraceManager.TraceDebug(sb.ToString());
        }

        /// <summary>
        /// Corrige les WBS. A n'utiliser que s'ils sont incorrects.
        /// </summary>
        public static void FixAllWBS(KAction[] actions)
        {
            if (!actions.Any())
                return;

            // Corriger par les items et leurs indentations
            var maxIndentation = actions.Select(a => WBSHelper.IndentationFromWBS(a.WBS)).Max() + 1;
            var currentLevels = new int[maxIndentation];

            foreach (var action in actions)
            {
                var indentation = WBSHelper.IndentationFromWBS(action.WBS);
                var itemWBS = new int[indentation + 1];

                for (int i = 0; i <= indentation; i++)
                {
                    if (i == indentation)
                        currentLevels[i] = currentLevels[i] + 1;

                    itemWBS[i] = currentLevels[i];
                }

                for (int i = indentation + 1; i < currentLevels.Length; i++)
                {
                    currentLevels[i] = 0;
                }

                action.WBS = WBSHelper.LevelsToWBS(itemWBS);
            }
        }

        #endregion

        #region Prédecesseurs / Successeurs

        /// <summary>
        /// Vérifie si un prédecesseur peut être ajouté.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">L'action prédécesseur.</param>
        /// <returns>
        ///   <c>true</c> si l'ajout est possible.
        /// </returns>
        public static bool CheckCanAddPredecessor(KAction[] allActions, KAction action, KAction predecessor)
        {
            Debug.Assert(allActions != null);
            Debug.Assert(action != null);
            Debug.Assert(predecessor != null);

            // impossible qu'un groupe soit prédécesseur ou successeur.
            if (predecessor == null || action == null || allActions == null || predecessor.IsGroup || action.IsGroup)
                return false;

            // Déterminer si le lien créerait des références cycliques.
            var predecessorLinks = new List<(KAction Predecessor, KAction Successor)>();

            // Créer les liens entre éléments
            foreach (var a in allActions)
            {
                Debug.Assert(a.Predecessors != null);
                if (a.Predecessors != null)
                {
                    foreach (var p in a.Predecessors)
                        predecessorLinks.Add((p, a));
                }
            }

            predecessorLinks.Add((predecessor, action));

            while (predecessorLinks.Any())
            {
                var leafs = predecessorLinks.Where(tuple =>
                    !predecessorLinks.Select(p => p.Predecessor).Contains(tuple.Successor) ||
                    !predecessorLinks.Select(p => p.Successor).Contains(tuple.Predecessor)
                    ).ToList();
                if (leafs.Any())
                    predecessorLinks.RemoveRange(leafs);
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Ajoute un prédécesseur à l'action.
        /// </summary>
        /// <param name="allActions">Toutes les actions.</param>
        /// <param name="action">L'action successeur.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        /// <returns><c>true</c> si l'ajout est possible.</returns>
        public static bool AddPredecessor(KAction[] allActions, KAction action, KAction predecessor)
        {
            if (!CheckCanAddPredecessor(allActions, action, predecessor))
                return false;

            action.Predecessors.Add(predecessor);
            return true;
        }

        /// <summary>
        /// Supprime une prédécesseur de l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="predecessor">Le prédécesseur.</param>
        public static void RemovePredecessor(KAction action, KAction predecessor)
        {
            action.Predecessors.Remove(predecessor);
        }

        /// <summary>
        /// Remappe les prédécesseurs et successeurs de l'action en fonction du filter d'inclusion filter.
        /// </summary>
        /// <param name="action">L'action</param>
        /// <param name="filter">Le filtre</param>
        /// <param name="newTimings">La collection de timings nouveaux à alimenter</param>
        /// <param name="useManaged"><c>true</c> pour utiliser les collection managées.</param>
        public static void MapAllPredToSucc(KAction action, Func<KAction, bool> filter, List<ActionTiming> newTimings, bool useManaged)
        {
            var finalPredecessors = new List<KAction>();
            var finalSuccessors = new List<KAction>();

            SearchPredSuccHierarchy(true, action, finalPredecessors, filter);
            SearchPredSuccHierarchy(false, action, finalSuccessors, filter);

            foreach (var pred in finalPredecessors)
            {
                foreach (var succ in finalSuccessors)
                {
                    ActionsTimingsMoveManagement.GetSuccessors(pred, useManaged).AddNew(succ);
                    ActionsTimingsMoveManagement.GetPredecessors(succ, useManaged).AddNew(pred);
                }
            }

            // Déplacer le début des successeurs en remplacant la valeur par le début de cette tâche.
            // Le Fix Pred Succ timings corrigera (si nécessaire) les timings par la suite
            if (!finalPredecessors.Any())
            {
                foreach (var succ in finalSuccessors)
                {
                    ActionTiming newTiming = newTimings.FirstOrDefault(t => t.Action == succ);
                    if (newTiming == null)
                    {
                        var duration = succ.BuildDuration;
                        newTimings.Add(new ActionTiming()
                        {
                            Action = succ,
                            Start = action.BuildStart,
                            Finish = action.BuildStart + duration
                        });
                    }
                    else if (newTiming.Start > action.BuildStart)
                    {
                        var duration = succ.BuildDuration;
                        newTiming.Start = action.BuildStart;
                        newTiming.Finish = action.BuildStart + duration;
                    }
                }
            }
        }

        /// <summary>
        /// Recherche un prédécesseur ou successeur récursivement qui passe le filtre spécifié.
        /// </summary>
        /// <param name="usePredecessors"><c>true</c> pour chercher dans les prédécesseurs, <c>false</c> pour les successeurs</param>
        /// <param name="currentAction">L'action de laquelle partir</param>
        /// <param name="actions">Toutes les actions</param>
        /// <param name="filter">Le filtre pour arrêter la recherche.</param>
        private static void SearchPredSuccHierarchy(bool usePredecessors, KAction currentAction, List<KAction> actions, Func<KAction, bool> filter)
        {
            var col = usePredecessors ? currentAction.Predecessors : currentAction.Successors;
            foreach (var pred in col)
            {
                if (filter(pred))
                    actions.Add(pred);
                else
                    SearchPredSuccHierarchy(usePredecessors, pred, actions, filter);
            }
        }

        #endregion

        #region Réduction

        /// <summary>
        /// Applique les changements du type de l'action réduite.
        /// </summary>
        /// <param name="action">L'action.</param>
        public static void ApplyReducedType(KAction action, string oldCode)
        {
            if (action == null || action.Reduced == null || action.Reduced.ActionTypeCode == null)
                return;

            var newCode = action.Reduced.ActionTypeCode;

            if (newCode == KnownActionCategoryTypes.S)
            {
                // Cas ou le Reduced.OriginalBuildDuration a été mis à 0 par la category -> retour à la "normal": on remet à 0
                //if (action.Category != null && action.Category.ActionTypeCode == KnownActionCategoryTypes.S && action.Original != null && action.Reduced != null)
                //{
                //    action.Reduced.OriginalBuildDuration = 0;
                //}

                // Passer la durée à 0 si on devient S
                action.BuildDuration = 0;
                action.Reduced.ReductionRatio = 1;
                ApplyReducedApproved(action, false);
            }
            else if (newCode != KnownActionCategoryTypes.S && oldCode == KnownActionCategoryTypes.S
                && action.Reduced.Approved)
            {

                //// Cas ou le Reduced.OriginalBuildDuration a été mis à 0 par la category
                if (action.Category != null /*&& action.Category.ActionTypeCode == KnownActionCategoryTypes.S*/ && action.Original != null && action.Reduced != null)
                {
                    action.Reduced.OriginalBuildDuration = action.Original.BuildDuration;
                }

                // Passer la durée à l'action original si on devient autre chose que S
                action.BuildDuration = action.Reduced.OriginalBuildDuration;
                ApplyReducedApproved(action, false);
            }
        }

        /// <summary>
        /// Applique le changement de l'état approuvé de l'action
        /// </summary>
        /// <param name="action">The action.</param>
        public static void ApplyReducedApproved(KAction action, bool refreshReducedType)
        {
            if (action.Reduced.Approved)
            {
                // rétablir le temps de l'action d'origine
                if (refreshReducedType)
                    ApplyReducedType(action, null);
                UpdateTimingsFromReducedReduction(action);
            }
            else
            {
                // Sauvegarder le temps d'origine
                action.BuildDuration = action.Reduced.OriginalBuildDuration;
            }
        }

        /// <summary>
        /// Met à jour le pourcentage de réduction à partir des timings.
        /// </summary>
        /// <param name="action">L'action.</param>
        public static void UpdateReducedReductionFromTimings(KAction action)
        {
            if (action.IsReduced && action.Reduced.Approved)
            {
                if (action.Reduced.OriginalBuildDuration == default(long))
                {
                    // Ne rien modifier
                }
                else if (action.Reduced.Approved)
                    action.Reduced.ReductionRatio = 1.0 - (double)action.BuildDuration / (double)action.Reduced.OriginalBuildDuration;
                else
                    action.Reduced.ReductionRatio = 0;
            }
        }

        /// <summary>
        /// Met à jour les timings à partir du pourcentage de réduction.
        /// </summary>
        /// <param name="action">L'action.</param>
        public static void UpdateTimingsFromReducedReduction(KAction action)
        {
            if (action.Reduced.Approved)
            {
                action.BuildDuration = Convert.ToInt64(action.Reduced.OriginalBuildDuration * (1.0 - (action.Reduced.ReductionRatio)));
            }
        }

        /// <summary>
        /// Obtient la solution associée à une action.
        /// </summary>
        /// <param name="a">L'action.</param>
        /// <returns>La solution ou <c>null</c> si introuvable.</returns>
        public static Solution GetSolution(KAction a)
        {
            if (a.Scenario != null && a.Reduced.Solution != null)
            {
                Solution solution;
                if (string.IsNullOrWhiteSpace(a.Reduced.Solution))
                    solution = a.Scenario.Solutions.FirstOrDefault(s => s.IsEmpty);
                else
                    solution = a.Scenario.Solutions.FirstOrDefault(s => a.Reduced.Solution == s.SolutionDescription) ??
                               new Solution();

                return solution;
            }
            else
                return null;
        }

        /// <summary>
        /// Obtient une valeur indiquant si la solution de l'action est approuvée.
        /// </summary>
        /// <param name="a">L'action.</param>
        /// <returns><c>true</c> si la solution de l'action est approuvée.</returns>
        public static bool? GetIsSolutionApproved(KAction a)
        {
            var solution = GetSolution(a);
            return solution != null ? solution.Approved : (bool?)null;
        }

        #endregion

        #region Helpers

        public const string KActionStartPropertyName = "Start";
        public const string KActionFinishPropertyName = "Finish";
        public const string KActionBuildStartPropertyName = "BuildStart";
        public const string KActionBuildFinishPropertyName = "BuildFinish";

        /// <summary>
        /// Obtient les durées Process d'origine et modifié.
        /// </summary>
        /// <param name="originalAction">L'action.</param>
        /// <param name="originalDuration">La durée d'origine.</param>
        /// <param name="modifiedDuration">La durée modifiée.</param>
        public static void GetOrignalModifiedBuildDurations(KAction originalAction, out long originalDuration, out long modifiedDuration)
        {
            GetOrignalModifiedDurations(KActionBuildStartPropertyName, KActionBuildFinishPropertyName, originalAction, out originalDuration, out modifiedDuration);
        }

        /// <summary>
        /// Obtient les durées Video d'origine et modifié.
        /// </summary>
        /// <param name="originalAction">L'action.</param>
        /// <param name="originalDuration">La durée d'origine.</param>
        /// <param name="modifiedDuration">La durée modifiée.</param>
        public static void GetOrignalModifiedVideoDurations(KAction originalAction, out long originalDuration, out long modifiedDuration)
        {
            GetOrignalModifiedDurations(KActionStartPropertyName, KActionFinishPropertyName, originalAction, out originalDuration, out modifiedDuration);
        }

        /// <summary>
        /// Obtient les durées d'origine et modifié.
        /// </summary>
        /// <param name="startPropertyName">Le nom de la propriété de début.</param>
        /// <param name="finishPropertyName">Le nom de la propriété de fin.</param>
        /// <param name="originalAction">L'action.</param>
        /// <param name="originalDuration">La durée d'origine.</param>
        /// <param name="modifiedDuration">La durée modifiée.</param>
        private static void GetOrignalModifiedDurations(string startPropertyName, string finishPropertyName, KAction originalAction, out long originalDuration, out long modifiedDuration)
        {
            // Vérifier si les reduced doit être impacté également
            long originalStart, originalFinish, start, finish;

            var modifiedValues = originalAction.ChangeTracker.ModifiedValues;
            var originalValues = originalAction.ChangeTracker.OriginalValues;

            if (modifiedValues.ContainsKey(startPropertyName))
            {
                originalStart = (long)originalValues[startPropertyName];
                start = (long)modifiedValues[startPropertyName];
            }
            else
                originalStart = start = (long)originalAction.GetCurrentValues()[startPropertyName];

            if (modifiedValues.ContainsKey(finishPropertyName))
            {
                originalFinish = (long)originalValues[finishPropertyName];
                finish = (long)modifiedValues[finishPropertyName];
            }
            else
                originalFinish = finish = (long)originalAction.GetCurrentValues()[finishPropertyName]; ;

            originalDuration = originalFinish - originalStart;
            modifiedDuration = finish - start;
        }

        /// <summary>
        /// Obtient la collection de prédécesseurs, managés ou non.
        /// </summary>
        /// <param name="action">L'action</param>
        /// <param name="managed"><c>true</c> pour utiliser la collection managé.</param>
        /// <returns>La collection de prédécesseurs.</returns>
        public static ICollection<KAction> GetPredecessors(KAction action, bool managed)
        {
            if (managed)
                return action.PredecessorsManaged;
            else
                return action.Predecessors;
        }

        /// <summary>
        /// Obtient la collection de successeurs, managés ou non.
        /// </summary>
        /// <param name="action">L'action</param>
        /// <param name="managed"><c>true</c> pour utiliser la collection managé.</param>
        /// <returns>La collection de successeurs.</returns>
        public static ICollection<KAction> GetSuccessors(KAction action, bool managed)
        {
            if (managed)
                return action.SuccessorsManaged;
            else
                return action.Successors;
        }

        #endregion

        #region Vérifications - DEBUG

        /// <summary>
        /// Vérifie si les prédécesseurs et successors managés sont bien construits.
        /// </summary>
        /// <param name="actions">Les actions.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugCheckPredSucc(IEnumerable<KAction> actions, bool managed)
        {
            foreach (var action in actions)
            {
                foreach (var pred in action.PredecessorsManaged)
                {
                    System.Diagnostics.Debug.Assert(GetSuccessors(pred, managed).Contains(action));
                    System.Diagnostics.Debug.Assert(actions.Contains(pred));
                }

                foreach (var succ in action.SuccessorsManaged)
                {
                    System.Diagnostics.Debug.Assert(GetPredecessors(succ, managed).Contains(action));
                    System.Diagnostics.Debug.Assert(actions.Contains(succ));
                }
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        /// <summary>
        /// Vérifie l'intégrité de tous les WBS
        /// </summary>
        public static void DebugCheckAllWBS(IEnumerable<Scenario> scenarios)
        {
            foreach (var scenario in scenarios)
                DebugCheckAllWBS(scenario.Actions);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        /// <summary>
        /// Vérifie l'intégrité de tous les WBS
        /// </summary>
        public static void DebugCheckAllWBS(Scenario scenario)
        {
            DebugCheckAllWBS(scenario.Actions);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        /// <summary>
        /// Vérifie l'intégrité de tous les WBS
        /// </summary>
        public static void DebugCheckAllWBS(IEnumerable<KAction> acts)
        {
            var actions = acts.OrderByWBS().ToArray();

            if (!actions.Any())
                return;

            var maxIndentation = actions.Select(a => WBSHelper.IndentationFromWBS(a.WBS)).Max();

            var currentLevels = new int[maxIndentation];

            bool areValid = true;
            string previous = null;
            foreach (var action in actions)
            {
                if (previous != null && !WBSHelper.CanBeSuccessive(previous, action.WBS))
                {
                    areValid = false;
                    break;
                }

                previous = action.WBS;
            }

            if (!areValid)
            {
#if DEBUG
                throw new InvalidOperationException("Les WBS sont invalides");
#else
                    TraceManager.TraceDebug("Les WBS sont invalides");
                    //ActionsTimingsMoveManagement.TraceAllWBS(actions);
                    ActionsTimingsMoveManagement.FixAllWBS(actions);
#endif
            }
        }

        #endregion

    }

    /// <summary>
    /// Représente le timing d'une action.
    /// </summary>
    public class ActionTiming
    {
        /// <summary>
        /// Obtient ou définit l'action.
        /// </summary>
        public KAction Action { get; set; }

        /// <summary>
        /// Obtient ou définit le début.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// Obtient ou définit la fin.
        /// </summary>
        public long Finish { get; set; }

        /// <summary>
        /// Obtient ou définit les prédécesseurs.
        /// </summary>
        public List<ActionTiming> Predecessors { get; set; }

        /// <summary>
        /// Obtient ou définit les successeurs.
        /// </summary>
        public List<ActionTiming> Successors { get; set; }
    }
}
