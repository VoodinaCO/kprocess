using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using System;
using System.Linq;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue des écrans Restituer - Lieux.
    /// </summary>
    class RestitutionRef3ViewModel : RestitutionViewByResourceViewModelBase<Ref3Wrapper, Ref3WrapperItem, Ref3>, IRestitutionRef3ViewModel
    {

        #region Méthodes privées

        /// <summary>
        /// Obtient un tableau des différents référentiels utilisés.
        /// </summary>
        /// <param name="leftActionFilter">Le filtre sur les actions de gauche.</param>
        /// <param name="rightActionFilter">Le filtre sur les actions de droite.</param>
        /// <returns>Les référentiels</returns>
        protected override Ref3[] GetDistinctReferentials(Func<KAction, bool> leftActionFilter, Func<KAction, bool> rightActionFilter)
        {
            return ParentViewModel.ScenariosToShow
                .SelectMany(s => s.Actions)
                .Where(a => leftActionFilter(a) || rightActionFilter(a))
                .SelectMany(a => a.Ref3.Select(al => al.Ref3))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Détermine si l'action spécifiée utilise le référentiel spécifié.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si l'action spécifiée utilise le référentiel spécifié.</returns>
        protected override bool MatchesReferential(KAction action, Ref3 referential) =>
            action.Ref3.Any(al => al.Referential == referential);

        #endregion

    }

    public class Ref3Wrapper : ReferentialBase<Ref3WrapperItem>
    {
    }

    public class Ref3WrapperItem : ReferentialItemBase
    {
    }
}
