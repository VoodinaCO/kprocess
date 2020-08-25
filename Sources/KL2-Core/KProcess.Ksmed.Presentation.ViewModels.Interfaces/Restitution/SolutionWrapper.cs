using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels.Restitution
{
    /// <summary>
    /// Représente un conteneur autour d'une solution.
    /// </summary>
    public class SolutionWrapper : NotifiableObject
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SolutionWrapper"/>.
        /// </summary>
        /// <param name="solution">La solution.</param>
        public SolutionWrapper(Solution solution)
        {
            this.Solution = solution;
            this.Solution.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Solution_PropertyChanged);
        }

        /// <summary>
        /// Définit les actions liées à la solution et calcule le gain
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        public void SetRelatedActions(IEnumerable<KAction> allActions)
        {
            var actions = allActions.Where(a =>
                !a.IsGroup &&
                a.IsReduced &&
                a.Reduced.Solution == this.Solution.SolutionDescription &&

                !(this.Solution.IsEmpty && a.Reduced.ReductionRatio == 0.0d && a.Reduced.ActionTypeCode == KnownActionCategoryTypes.I)

                );
            RefreshSavings(allActions);

            var maxWBSLength = actions.Any() ? actions.Select(a => a.WBS.Length).Max() : 1;

            this.RelatedActions = string.Join(Environment.NewLine, actions.Select(a => string.Format("{0} - {1}", a.WBS.PadRight(maxWBSLength), a.Label ?? string.Empty)));
        }

        /// <summary>
        /// Met à jour les gains.
        /// </summary>
        /// <param name="allActions">Toutes les actions du scénario.</param>
        public void RefreshSavings(IEnumerable<KAction> allActions)
        {
            var actions = allActions.Where(a => a.IsReduced && !a.IsGroup && a.Reduced.Solution == this.Solution.SolutionDescription);
            this.Saving = actions.Sum(a =>
                   a.Reduced.ActionTypeCode != null && a.Reduced.ActionTypeCode == KnownActionCategoryTypes.E ?
                       // Si une action réduite est marquée E, le gain doit compter 100% de la tache
                       a.Reduced.OriginalBuildDuration :
                       // Sinon le gain est simplement la différence avec la valeur originale
                       a.Reduced.OriginalBuildDuration - a.BuildDuration);
            OnPropertyChanged("Saving");
        }

        /// <summary>
        /// Gère l'évènement PropertyChanged de la solution.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.ComponentModel.PropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        void Solution_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("IG");
            OnPropertyChanged("DCG");
        }

        /// <summary>
        /// Obtient la solution.
        /// </summary>
        public Solution Solution { get; private set; }

        /// <summary>
        /// Obtient les actions liées.
        /// </summary>
        public string RelatedActions { get; private set; }

        /// <summary>
        /// Obtient ou définit le gain en temps.
        /// </summary>
        public long Saving { get; set; }

        /// <summary>
        /// Obtient Investissement / Gain.
        /// </summary>
        public double? IG
        {
            get
            {
                if (Saving == 0)
                    return null;

                return this.Solution.Investment.GetValueOrDefault() /
                    TimeSpan.FromTicks(Saving).TotalMinutes;
            }
        }

        /// <summary>
        /// Obtient Difficulté * Cout / Gain.
        /// </summary>
        public double? DCG
        {
            get
            {
                if (Saving == 0)
                    return null;

                return this.Solution.Difficulty.GetValueOrDefault() * this.Solution.Cost.GetValueOrDefault() /
                    TimeSpan.FromTicks(Saving).TotalMinutes;
            }
        }

        /// <summary>
        /// Obtient ou définit l'index de visualisation.
        /// </summary>
        public int Index { get; set; }

        private bool _isNotReadOnly;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la solution n'est pas en lecture seule.
        /// </summary>
        public bool IsNotReadOnly
        {
            get { return _isNotReadOnly; }
            set
            {
                if (_isNotReadOnly != value)
                {
                    _isNotReadOnly = value;
                    OnPropertyChanged("IsNotReadOnly");
                }
            }
        }

    }
}
