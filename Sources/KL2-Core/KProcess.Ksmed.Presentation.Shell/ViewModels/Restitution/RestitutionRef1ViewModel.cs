using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using System;
using System.Linq;

namespace KProcess.Ksmed.Presentation.ViewModels
{

    /// <summary>
    /// Représente le modèle de vue des écrans Restituer - Outils.
    /// </summary>
    class RestitutionRef1ViewModel : RestitutionViewByResourceViewModelBase<Ref1Wrapper, Ref1WrapperItem, Ref1>, IRestitutionRef1ViewModel
    {

        #region Méthodes privées

        /// <summary>
        /// Obtient un tableau des différents référentiels utilisés.
        /// </summary>
        /// <param name="leftActionFilter">Le filtre sur les actions de gauche.</param>
        /// <param name="rightActionFilter">Le filtre sur les actions de droite.</param>
        /// <returns>Les référentiels</returns>
        protected override Ref1[] GetDistinctReferentials(Func<KAction, bool> leftActionFilter, Func<KAction, bool> rightActionFilter)
        {
            return ParentViewModel.ScenariosToShow
                .SelectMany(s => s.Actions)
                .Where(a => leftActionFilter(a) || rightActionFilter(a))
                .SelectMany(a => a.Ref1.Select(al => al.Ref1))
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Détermine si l'action spécifiée utilise le référentiel spécifié.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si l'action spécifiée utilise le référentiel spécifié.</returns>
        protected override bool MatchesReferential(KAction action, Ref1 referential) =>
            action.Ref1.Any(al => al.Referential == referential);

        #endregion

    }

    /// <summary>
    /// Représente un conteneur autour d'un outil.
    /// </summary>
    public class Ref1Wrapper : ReferentialBase<Ref1WrapperItem>
    {

    }

    /// <summary>
    /// Représente un sous-conteneur autour d'un outil.
    /// </summary>
    public class Ref1WrapperItem : ReferentialItemBase
    {

    }
}
