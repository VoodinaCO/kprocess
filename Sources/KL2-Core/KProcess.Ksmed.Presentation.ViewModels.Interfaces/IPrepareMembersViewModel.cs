using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des membres du projet.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IPrepareMembersViewModel : IFrameContentViewModel
    {
        /// <summary>
        /// Obtient uen valeur indiquant si la sélection peut être changée.
        /// </summary>
        bool CanChange { get; }

        /// <summary>
        /// Obtient ou définit le membre courant.
        /// </summary>
        User CurrentMember { get; set; }

        /// <summary>
        /// Obtient les utilisateurs.
        /// </summary>
        User[] Users { get; }

    }
}
