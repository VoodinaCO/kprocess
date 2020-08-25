using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using System;
using System.Linq;

namespace KProcess.Ksmed.Presentation.ViewModels
{

    /// <summary>
    /// Représente le modèle de vue des écrans Restituer - Documents.
    /// </summary>
    class RestitutionRef4ViewModel : RestitutionViewByResourceViewModelBase<Ref4Wrapper, Ref4WrapperItem, Ref4>, IRestitutionRef4ViewModel
    {

        #region Méthodes privées

        /// <summary>
        /// Obtient un tableau des différents référentiels utilisés.
        /// </summary>
        /// <param name="leftActionFilter">Le filtre sur les actions de gauche.</param>
        /// <param name="rightActionFilter">Le filtre sur les actions de droite.</param>
        /// <returns>Les référentiels</returns>
        protected override Ref4[] GetDistinctReferentials(Func<KAction, bool> leftActionFilter, Func<KAction, bool> rightActionFilter)
        {
            return ParentViewModel.ScenariosToShow
                .SelectMany(s => s.Actions)
                .Where(a => leftActionFilter(a) || rightActionFilter(a))
                .SelectMany(a => a.Ref4.Select(al => al.Ref4))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Détermine si l'action spécifiée utilise le référentiel spécifié.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si l'action spécifiée utilise le référentiel spécifié.</returns>
        protected override bool MatchesReferential(KAction action, Ref4 referential) =>
            action.Ref4.Any(al => al.Referential == referential);

        #endregion

    }

    /// <summary>
    /// Représente un conteneur autour d'un document.
    /// </summary>
    public class Ref4Wrapper : ReferentialBase<Ref4WrapperItem>
    {
    }

    /// <summary>
    /// Représente un sous-conteneur autour d'un document.
    /// </summary>
    public class Ref4WrapperItem : ReferentialItemBase
    {

    }
}
