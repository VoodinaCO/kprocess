using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un service capable d'enregistrer une association Vue-ViewModel afin de pouvoir fournir un
    /// DependencyObject à un view model (utile pour les ChildWindowService par exemple).
    /// Ne fonctionne qu'avec des références faibles.
    /// </summary>
    public interface IViewHandleService : IPresentationService
    {

        /// <summary>
        /// Enregistre l'association spécifiée.
        /// </summary>
        /// <param name="viewModel">Le VM.</param>
        /// <param name="handle">L'attache.</param>
        void Register(IViewModel viewModel, DependencyObject handle);

        /// <summary>
        /// Libère les associations donc l'attache est celle spécifiée
        /// </summary>
        /// <param name="handle">L'attache.</param>
        void Release(DependencyObject handle);

        /// <summary>
        /// Obtient l'attache pour le VM spécifié.
        /// </summary>
        /// <param name="viewModel">Le vm.</param>
        /// <returns>L'attache ou null.</returns>
        DependencyObject Resolve(IViewModel viewModel);

    }
}
