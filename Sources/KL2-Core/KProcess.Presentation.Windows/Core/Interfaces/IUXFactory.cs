// -----------------------------------------------------------------------
// <copyright file="IUXFactory.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface de la factory des views et des viewModels
    /// </summary>
    [InheritedExport]
    public interface IUXFactory
    {
        /// <summary>
        /// Retourne l'instance du viewModel demandée
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <returns>l'instance du viewModel demandée</returns>
        TViewModel GetViewModel<TViewModel>() 
            where TViewModel : IViewModel;

        /// <summary>
        /// Retourne l'instance de la vue correspondant au viewModel
        /// </summary>
        /// <param name="viewModel">instance du viewModel demandée</param>
        /// <returns>l'instance de la vue correspondant au viewModel</returns>
        IView GetView(IViewModel viewModel);

        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        IView GetView<TViewModel>()
            where TViewModel : IViewModel;

        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <param name="viewModel">instance du viewModel créée</param>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        IView GetView<TViewModel>(out TViewModel viewModel)
            where TViewModel : IViewModel;

        /// <summary>
        /// Retourne l'instance de la vue correspondant au TViewModel
        /// </summary>
        /// <typeparam name="TViewModel">type de viewModel demandé</typeparam>
        /// <param name="onCreated">délégué à exécuter lors de la création du viewModel</param>
        /// <returns>l'instance de la vue correspondant au TViewModel</returns>
        IView GetView<TViewModel>(Action<TViewModel> onCreated)
            where TViewModel : IViewModel;       
    }
}
