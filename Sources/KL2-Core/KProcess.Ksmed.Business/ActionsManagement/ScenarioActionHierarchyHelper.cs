using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    /// <summary>
    /// Un helper afin de déterminer les Parents ou Enfant (originaux ou dérivés) des actions et scénarios.
    /// </summary>
    public static class ScenarioActionHierarchyHelper
    {

        /// <summary>
        /// Détermine si l'ancêtre présumée spécifiée est bien l'ancêtre de l'action spécifiée.
        /// </summary>
        /// <param name="ancestor">La présumée ancêtre.</param>
        /// <param name="action">L'action.</param>
        /// <returns>
        ///   <c>true</c> si l'ancêtre présumée spécifiée est bien l'ancêtre de l'action spécifiée; sinon, <c>false</c>.
        /// </returns>
        public static bool IsAncestor(KAction ancestor, KAction action)
        {
            var current = action.Original;
            while (current != null)
            {
                if (current == ancestor)
                    return true;
                current = current.Original;
            }

            return false;
        }

        /// <summary>
        /// Obtient les scénarios dérivés du scénario spécifié
        /// </summary>
        /// <param name="mainScenario">le scénario source.</param>
        /// <param name="allScenarios">Tous les scénarios.</param>
        /// <returns>Les scénarios dérivés, triés par ordre d'héritage.</returns>
        public static Scenario[] GetDerivedScenarios(Scenario mainScenario, IEnumerable<Scenario> allScenarios)
        {
            var derivedScenarios = new List<Scenario>();

            foreach (var scenario in allScenarios.Where(s => s != mainScenario))
            {
                if (derivedScenarios.Contains(scenario))
                    continue;

                bool found = false;
                var currentScenario = scenario;

                while (currentScenario.Original != null)
                {
                    if (currentScenario.Original == mainScenario)
                    {
                        found = true;
                        break;
                    }
                    currentScenario = currentScenario.Original;
                }

                if (found)
                {
                    // Parcourir à nouveau tous les parents pour les ajouter en même temps à la liste.
                    // Cela permettra d'effectuer un tri particulier
                    var hierarchy = new List<Scenario>();
                    currentScenario = scenario;

                    while (currentScenario != mainScenario)
                    {
                        if (!derivedScenarios.Contains(currentScenario))
                            hierarchy.Add(currentScenario);
                        currentScenario = currentScenario.Original;
                    }

                    hierarchy.Reverse();
                    foreach (var s in hierarchy)
                        derivedScenarios.Add(s);
                }
            }

            return derivedScenarios.ToArray();
        }

        /// <summary>
        /// Obtient l'action dérivée de l'action spécifiée dans le scénario dérivé spécifié.
        /// </summary>
        /// <param name="sourceAction">L'action source.</param>
        /// <param name="derivedScenario">Le scénario dérivé.</param>
        /// <returns>L'action dérivée, ou null si elle n'a pas été trouvée.</returns>
        public static KAction GetDerivedAction(KAction sourceAction, Scenario derivedScenario)
        {
            if (sourceAction == null)
                return null;

            foreach (var action in derivedScenario.Actions)
            {
                bool found = false;
                var currentAction = action;
                while (currentAction.Original != null)
                {
                    if (currentAction.Original == sourceAction)
                    {
                        found = true;
                        break;
                    }
                    currentAction = currentAction.Original;
                }

                if (found)
                    return action;
            }

            return null;
        }

        /// <summary>
        /// Obtient l'action ancêtre de l'action spécifiée dans le scénario ancêtre spécifié.
        /// </summary>
        /// <param name="sourceAction">L'action source.</param>
        /// <param name="ancestorScenario">Le scénario ancetre.</param>
        /// <returns>
        /// L'action dérivée, ou null si elle n'a pas été trouvée.
        /// </returns>
        public static KAction GetAncestorAction(KAction sourceAction, Scenario ancestorScenario)
        {
            if (sourceAction == null)
                return null;

            var original = sourceAction.Original;
            while (original != null)
            {
                if (ancestorScenario.Actions.Contains(original))
                    return original;
                original = original.Original;
            }

            return null;
        }

        /// <summary>
        /// Fait correspondre les originaux de scénarios et d'actions.
        /// </summary>
        /// <param name="scenarios">Tous les scénarios.</param>
        public static void MapScenariosActionsOriginals(Scenario[] scenarios)
        {
            // Mapper les originaux à la main
            foreach (var scenario in scenarios)
            {
                if (scenario.OriginalScenarioId.HasValue)
                {
                    scenario.Original = scenarios.FirstOrDefault(s => s.ScenarioId == scenario.OriginalScenarioId);

                    if (scenario.Original != null)
                        foreach (var action in scenario.Actions.Where(a => a.OriginalActionId.HasValue))
                            action.Original = scenario.Original.Actions.FirstOrDefault(a => a.ActionId == action.OriginalActionId);
                }
            }
        }

    }
}
