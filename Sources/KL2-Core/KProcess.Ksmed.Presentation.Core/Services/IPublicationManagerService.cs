using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service de gestion d'une publication en cours.
    /// </summary>
    public interface IPublicationManagerService : IPresentationService
    {

        /// <summary>
        /// Obtient ou définit la publication courante.
        /// </summary>
        Publication CurrentPublication { get; set; }

        /// <summary>
        /// Définit la publication courante.
        /// </summary>
        /// <param name="s">Le scénario.</param>
        void SetCurrentPublication(Scenario s);

        /// <summary>
        /// Obtient l'état de la publication en cours.
        /// </summary>
        

        /// <summary>
        /// Annule la publication en cours.
        /// </summary>

    }
}
