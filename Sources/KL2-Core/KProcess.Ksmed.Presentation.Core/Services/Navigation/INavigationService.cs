using KProcess.Presentation.Windows;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service gérant la navigation dans l'application.
    /// </summary>
    public interface INavigationService : IPresentationService
    {

        /// <summary>
        /// Tente d'afficher le ViewModel spécifié.
        /// </summary>
        /// <typeparam name="TViewModel">Le type du ViewModel.</typeparam>
        /// <param name="initialization">L'action à exécuter une fois que le VM est créé.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        Task<bool> TryShow<TViewModel>(Action<TViewModel> initialization = null) where TViewModel : IFrameContentViewModel;

        /// <summary>
        /// Tente de naviguer vers le menu spécifié.
        /// </summary>
        /// <param name="menuCode">Le code du menu.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        Task<bool> TryNavigate(string menuCode);

        /// <summary>
        /// Tente de naviguer vers le menu spécifié.
        /// </summary>
        /// <param name="menuItemCode">Le code du menu.</param>
        /// <param name="subMenuCode">Le code du sous-menu.</param>
        /// <returns><c>true</c> si l'opération a réussi.</returns>
        Task<bool> TryNavigate(string menuCode, string subMenuCode);

        /// <summary>
        /// Obtient les préférences de navigation.
        /// </summary>
        NavigationSharedPreferences Preferences { get; }

        /// <summary>
        /// Rafraîchie l'état des menus
        /// </summary>
        void InvalidateMenus();

    }
}
