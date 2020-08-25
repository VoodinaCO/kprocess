using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Représente les filtres définis pour une extension de fichier.
    /// </summary>
    public class ExtensionFiltersSource
    {

        /// <summary>
        /// Obtient ou définit l'extension de fichier (avec le point).
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Obtient ou définit la vitesse de lecture minimale supportée.
        /// </summary>
        public double? MinSpeedRatio { get; set; }

        /// <summary>
        /// Obtient ou définit la vitesse de lecture maximale supportée.
        /// </summary>
        public double? MaxSpeedRatio { get; set; }

        /// <summary>
        /// Obtient ou définit le Splitter.
        /// </summary>
        public FilterSource Splitter { get; set; }

        /// <summary>
        /// Obtient ou définit le décodeur vidéo.
        /// </summary>
        public FilterSource VideoDecoder { get; set; }

        /// <summary>
        /// Obtient ou définit le décodeur audio.
        /// </summary>
        public FilterSource AudioDecoder { get; set; }

    }
}
