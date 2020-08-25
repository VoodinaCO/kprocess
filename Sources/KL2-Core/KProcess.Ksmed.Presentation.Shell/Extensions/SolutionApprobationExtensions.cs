using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation
{
    public static class SolutionApprobationExtensions
    {
        /// <summary>
        /// Définit la ressource à afficher dans Reduced pour chacune des actions du scénario spécifié
        /// </summary>
        /// <param name="scenario">Le scénario contenant les actions à mapper</param>
        /// <returns>Le scénario spécifié</returns>
        public static Scenario MapApprovedReduction(this Scenario scenario)
        {
            if (scenario == null) return null;

            foreach (var action in scenario.Actions.Where(a => a.IsReduced))
            {
               // var solution = scenario.Solutions.FirstOrDefault(s => s.SolutionDescription == action.Reduced.Solution);
               // action.Reduced.Approved = solution == null || solution.Approved;
                action.MapApprovedReducedResource();
            }

            return scenario;
        }

        /// <summary>
        /// Définit la ressource à afficher dans Reduced pour chacune des actions des scénarios spécifié
        /// </summary>
        /// <param name="scenarios">Les scénarios contenant les actions à mapper</param>
        /// <returns>Les scénarios spécifiés</returns>
        public static IEnumerable<Scenario> MapReducedResources(this IEnumerable<Scenario> scenarios)
        {
            foreach (var scenario in scenarios)
            {
                scenario.MapApprovedReduction();
            }

            return scenarios;
        }

        /// <summary>
        /// Récupère la resource à prendre en compte pour une action spécifiée
        /// </summary>
        /// <param name="action">L'action à partir de laquelle la resource est récupérée</param>
        /// <returns>La resource correspondant à l'action spécifiée</returns>
        public static Resource GetApprovedResource(this KAction action)
        {
            return action.IsReduced ? action.Reduced.Resource : action.Resource;
        }

        /// <summary>
        /// Définit la ressource à afficher dans Reduced pour l'action spécifiée
        /// </summary>
        /// <param name="action">l'action à mapper</param>
        /// <returns></returns>
        public static KAction MapApprovedReducedResource(this KAction action)
        {
            if (action.IsReduced)
            {
                if (action.Reduced.Approved)
                {
                    action.Reduced.Resource = action.Resource;
                }
                else if (action.OriginalActionId.HasValue)
                {
                    action.Reduced.Resource = action.Original.Resource;
                }
                else // Pourrait être mergé avec la première condition mais les cas métiers apparaissent plus clairement ainsi
                {
                    action.Reduced.Resource = action.Resource;
                }
            }

            return action;
        }

        /// <summary>
        /// Définit la ressource à afficher dans Reduced pour les actions spécifiées
        /// </summary>
        /// <param name="actions">les action à mapper</param>
        /// <returns></returns>
        public static IEnumerable<KAction> MapApprovedReducedResource(this IEnumerable<KAction> actions)
        {
            foreach (var action in actions)
            {
                action.MapApprovedReducedResource();
            }

            return actions;
        }
    }
}
