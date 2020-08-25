using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement des modèles de vue des sous-écrans de Admin/Referentials;
    /// </summary>
    public interface IAdminSubReferentialViewModel : IViewModel
    {
        /// <summary>
        /// Obtient ou définit le VM parent.
        /// </summary>
        IAdminReferentialsViewModel ParentViewModel { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si l'écran possède des fonctionnalités supplémentaires.
        /// </summary>
        bool HasExtraFeatures { get; }

        /// <summary>
        /// Charge les données.
        /// </summary>
        Task LoadItems();

        /// <summary>
        /// Sauvegarde les élements.
        /// A surcharger.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        Task SaveItems(IEnumerable<IActionReferential> items);

        /// <summary>
        /// Crée un nouveau référentiel standard.
        /// </summary>
        /// <returns>Le référentiel créé.</returns>
        IActionReferential CreateStandardReferential();

        /// <summary>
        /// Crée un nouveau référentiel process.
        /// </summary>
        /// <returns>Le référentiel créé.</returns>
        IActionReferentialProcess CreateProcessReferential();

        /// <summary>
        /// A lieu lorsque l'élément courant a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        void OnCurrentItemChanged(IActionReferential oldValue, IActionReferential newValue);

        /// <summary>
        /// Dans cette méthode, désinitialiser les propriétés de navigation de l'élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        void UninitializeRemovedItem(IActionReferential item);
    }
}