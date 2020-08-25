using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using System;
using System.Linq;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue des écrans Restituer - Référentiels supplémentaires 1.
    /// </summary>
    class RestitutionRef5ViewModel : RestitutionViewByResourceViewModelBase<Ref5Wrapper, Ref5WrapperItem, Ref5>, IRestitutionRef5ViewModel
    {

        #region Méthodes privées

        /// <summary>
        /// Obtient un tableau des différents référentiels utilisés.
        /// </summary>
        /// <param name="leftActionFilter">Le filtre sur les actions de gauche.</param>
        /// <param name="rightActionFilter">Le filtre sur les actions de droite.</param>
        /// <returns>Les référentiels</returns>
        protected override Ref5[] GetDistinctReferentials(Func<KAction, bool> leftActionFilter, Func<KAction, bool> rightActionFilter)
        {
            return ParentViewModel.ScenariosToShow
                .SelectMany(s => s.Actions)
                .Where(a => leftActionFilter(a) || rightActionFilter(a))
                .SelectMany(a => a.Ref5.Select(al => al.Ref5))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Détermine si l'action spécifiée utilise le référentiel spécifié.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si l'action spécifiée utilise le référentiel spécifié.</returns>
        protected override bool MatchesReferential(KAction action, Ref5 referential) =>
            action.Ref5.Any(al => al.Referential == referential);

        #endregion

    }

    /// <summary>
    /// Représente un conteneur autour d'un référentiel supplémentaire 1.
    /// </summary>
    public class Ref5Wrapper : ReferentialBase<Ref5WrapperItem>
    {
    }

    /// <summary>
    /// Représente un sous-conteneur autour d'un référentiel supplémentaire 1.
    /// </summary>
    public class Ref5WrapperItem : ReferentialItemBase
    {
    }
}
