using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des catégories d'action.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IAdminReferentialsViewModel : IFrameContentViewModel
    {
        
        /// <summary>
        /// Obtient ou définit l'élément courant.
        /// </summary>
        IActionReferential CurrentItem { get; set; }
        
        /// <summary>
        /// Crée la CollectionView pour les éléments.
        /// </summary>
        void SetItemsSource(IActionReferential[] items, Procedure[] processes);

        Procedure[] Processes { get; }
    }
}
