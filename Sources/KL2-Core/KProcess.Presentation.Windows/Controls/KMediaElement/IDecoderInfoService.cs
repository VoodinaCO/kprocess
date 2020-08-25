using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows.Controls;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Représente le comportement d'un service permettant de fournir des informations sur les codec utilisé.
    /// </summary>
    public interface IDecoderInfoService : IPresentationService
    {

        /// <summary>
        /// Obtient ou définit les profils de décodage.
        /// </summary>
        FiltersConfiguration FiltersConfiguration { get; set; }

        /// <summary>
        /// Initialize la configuration des filtres.
        /// </summary>
        /// <returns><c>true</c> si l'initialisation a réussi.</returns>
        bool InitializeFiltersConfiguration();

    }
}
