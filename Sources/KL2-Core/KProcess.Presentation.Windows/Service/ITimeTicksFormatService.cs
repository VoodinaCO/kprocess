using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit le comportement d'un service permettant de formatter un temps ou une durée.
    /// </summary>
    public interface ITimeTicksFormatService : IPresentationService, Controls.ITimeToStringFormatter
    {
        /// <summary>
        /// Obtient l'échelle de temps du projet en cours.
        /// </summary>
        long CurrentTimeScale { get; }
        
        /// <summary>
        /// Effectue un arrondi sur les ticks.
        /// </summary>
        /// <param name="ticks">Les ticks.</param>
        /// <param name="scale">L'arrondi.</param>
        /// <returns>Les ticks arrondis.</returns>
        long RoundTime(long ticks, long scale);

        /// <summary>
        /// Convertit des ticks en string.
        /// </summary>
        /// <param name="ticks">les ticks.</param>
        /// <param name="timeScale">L'échelle de temps. -1 par défaut.</param>
        /// <returns>La chaîne formattée.</returns>
        string TicksToString(long ticks, long timeScale = -1);

        /// <summary>
        /// Parse une chaîne et la convertit en ticks.
        /// </summary>
        /// <param name="input">La chaîne.</param>
        /// <returns>Les ticks, ou 0 si le contenu est invalide.</returns>
        long ParseToTicks(string input);


    }
}
