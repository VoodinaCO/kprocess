using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.ActionsManagement
{
    // Documentation utilisée pour le calcul des temps critiques :
    // http://www.ctl.ua.edu/math103/scheduling/cpaprelim.htm#The Backflow Algorithm
    // http://www.ctl.ua.edu/math103/scheduling/scheduling_algorithms.htm

    /// <summary>
    /// Représente une action et son temps critique.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Action.Label} {CriticalTime}")]
    public class CriticalAction : IComparer<CriticalAction>
    {
        /// <summary>
        /// Obtient ou crée une <see cref="CriticalAction"/> contenant l'action spécifiée.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="allActions">La totalité des actions.</param>
        /// <param name="createdActions">Les actions déjà créées.</param>
        /// <returns>
        /// L'instance de <see cref="CriticalAction"/>.
        /// </returns>
        public static CriticalAction GetOrCreate(ActionTiming action, IEnumerable<ActionTiming> allActions, List<CriticalAction> createdActions)
        {

            var cpma = createdActions.FirstOrDefault(a => a.Action == action.Action);
            if (cpma == null)
            {
                cpma = new CriticalAction()
                {
                    Action = action.Action,
                    Duration = action.Finish - action.Start,
                };

                createdActions.Add(cpma);

                if (!action.Predecessors.Any())
                    cpma.Duration += action.Start;

                foreach (var p in action.Predecessors)
                {
                    var cpmPredecessor = GetOrCreate(p, allActions, createdActions);
                    if (!cpma.Predecessors.Any(a => a.Action == p.Action))
                        cpma.Predecessors.Add(cpmPredecessor);
                }

                foreach (var s in action.Successors)
                {
                    var cpmSuccessor = GetOrCreate(s, allActions, createdActions);
                    if (!cpma.Successors.Any(a => a.Action == s.Action))
                        cpma.Successors.Add(cpmSuccessor);
                }
            }

            return cpma;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CriticalAction"/>.
        /// </summary>
        public CriticalAction()
        {
            this.Predecessors = new List<CriticalAction>();
            this.Successors = new List<CriticalAction>();
        }

        /// <summary>
        /// Obtient l'action.
        /// </summary>
        public KAction Action { get; private set; }

        /// <summary>
        /// Obtient la durée de l'action.
        /// </summary>
        public long Duration { get; private set; }

        /// <summary>
        /// Obtient les prédécesseurs.
        /// </summary>
        public List<CriticalAction> Predecessors { get; private set; }

        /// <summary>
        /// Obtient les successeurs.
        /// </summary>
        public List<CriticalAction> Successors { get; private set; }

        /// <summary>
        /// Obtient le temps critique.
        /// </summary>
        public long CriticalTime { get; set; }

        public int Compare(CriticalAction x, CriticalAction y)
        {
            if (x.CriticalTime != y.CriticalTime)
            {
                // Order descendant sur CriticalTime
                if (y.CriticalTime > x.CriticalTime)
                    return 1;
                else
                    return -1;
            }
            else
            {
                // Avec cette succession de tâches :
                // x
                //  y
                // où y est le successeur de X et X a une durée de 0 (et donc X et Y ont le même critical time)
                // x doit apparaitre avant Y

                if (x.Successors.Contains(y))
                {
                    return -1;
                }
                else if (y.Successors.Contains(x))
                {
                    return 1;
                }
                else
                    return 0;
            }
        }
    }
}
