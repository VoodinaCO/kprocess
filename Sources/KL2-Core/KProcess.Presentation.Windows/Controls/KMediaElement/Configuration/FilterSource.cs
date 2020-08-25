using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Représente la source d'un filtre.
    /// </summary>
    public class FilterSource
    {

        /// <summary>
        /// Obtient ou définit le nom du filtre.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtient ou définit le type de source.
        /// </summary>
        public FilterSourceTypeEnum SourceType { get; set; }

        /// <summary>
        /// Obtient ou définit le CLSID du filtre externe.
        /// </summary>
        public string ExternalCLSID { get; set; }

    }
}
