using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente les valeurs initiales pré-zoom vertical
    /// </summary>
    public class ZoomYInitialValues
    {
        /// <summary>
        /// Obtient ou définit le ratio entre la hauteur des barres et celles des éléments.
        /// </summary>
        public double BarHeightRatio { get; set; }

        /// <summary>
        /// Obtient ou définit la hauteur initiale des éléments.
        /// </summary>
        public double ItemHeight { get; set; }

        /// <summary>
        /// Obtient ou définit la taille de police initiale.
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Obtient ou définit le conteneur des lignes.
        /// </summary>
        public DataGridRowsPresenter DataGridRowsPresenter { get; set; }
    }
}
