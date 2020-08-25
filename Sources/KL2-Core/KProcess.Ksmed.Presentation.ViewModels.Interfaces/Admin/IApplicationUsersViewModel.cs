using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des utilisateurs de l'application.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IApplicationUsersViewModel : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient ou définit l'utilisateur courant.
        /// </summary>
        User CurrentUser { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si une des éléments de la liste a changé.
        /// </summary>
        bool HasChanged { get; }

        /// <summary>
        /// Obtient les langues.
        /// </summary>
        Language[] Languages { get; }

        /// <summary>
        /// Obtient les roles.
        /// </summary>
        IPresentationRole[] Roles { get; }

        /// <summary>
        /// Obtient les utilisateurs.
        /// </summary>
        BulkObservableCollection<User> Users { get; }

        /// <summary>
        /// Appelé lorsqu'un rôle est coché ou décoché.
        /// </summary>
        /// <param name="role">Le rôle.</param>
        /// <param name="isChecked">L'état.</param>
        void OnRoleChecked(Role role, bool isChecked);
    }


    public interface IPresentationRole : IComparable<IPresentationRole>
    {
        /// <summary>
        /// Obtient le rôle associé.
        /// </summary>
        Role Role { get; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le rôle est coché.
        /// </summary>
        bool IsChecked { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si un séparateur est présent avant cet élément.
        /// </summary>
        bool HasSeparator { get; }
    }
}
