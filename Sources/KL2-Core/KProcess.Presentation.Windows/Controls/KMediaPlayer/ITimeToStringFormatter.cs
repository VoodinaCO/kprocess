using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Définit le comportement d'un formatteur de temps vers une chaîne de caractères
    /// </summary>
    public interface ITimeToStringFormatter
    {

        /// <summary>
        /// Obtient le convertisseur de position courante
        /// </summary>
        IMultiValueConverter CurrentPositionConverter { get; }

        /// <summary>
        /// Formatte un temps.
        /// </summary>
        /// <param name="ticks">Des ticks.</param>
        /// <returns>Le temps formatté</returns>
        string FormatTime(long ticks);

    }
}
