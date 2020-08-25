using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{
    /// <summary>
    /// Le type de données d'une cellule.
    /// </summary>
    public enum CellDataType
    {
        /// <summary>
        /// Uen chaîne de caractères.
        /// </summary>
        String,

        /// <summary>
        /// Un nombre.
        /// </summary>
        Number,

        /// <summary>
        /// Un pourcentage
        /// </summary>
        Percentage,

        /// <summary>
        /// Un temps.
        /// </summary>
        TimeSpan,

        /// <summary>
        /// Une date
        /// </summary>
        Date,

        /// <summary>
        /// Un hyperlien
        /// </summary>
        Hyperlink
    }
}
