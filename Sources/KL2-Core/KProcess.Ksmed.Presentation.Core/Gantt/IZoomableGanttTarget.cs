using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente une cible qui effectuera acceptera un zoom..
    /// </summary>
    public interface IZoomableGanttTarget
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le Gantt est prêt à être auto-scalé.
        /// </summary>
        bool IsReadyForAutoScale { get; set; }

        /// <summary>
        /// Survient lorsque l'élément est prêt pour l'auto scale.
        /// </summary>
        event EventHandler ReadyForAutoScale;

        /// <summary>
        /// Obtient ou définit les valeurs initiales, avant le zoom vertical.
        /// </summary>
        ZoomYInitialValues ZoomYInitialValues { get; set; }
    }
}
