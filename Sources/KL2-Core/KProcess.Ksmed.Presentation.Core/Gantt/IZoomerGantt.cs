using DlhSoft.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Définit le comportement d'un élément capable d'effectuer des zooms sur des gantts.
    /// </summary>
    public interface IZoommerGantt
    {
        /// <summary>
        /// Obtient ou définit un délégué exécuté lorsque l'auto scale aura lieu.
        /// </summary>
        Action AutoScaleEnqueued { get; set; }

        /// <summary>
        /// Met en attente un auto-scale qui sera exécuté dès que le gantt sera prêt.
        /// </summary>
        /// <param name="horizontal"><c>true</c> pour scaler horizontalement.</param>
        /// <param name="vertical"><c>true</c> pour scaler verticalement.</param>
        /// <param name="resetZooms"><c>true</c> pour  RAZ le zoom.</param>
        void EnqueueAutoScale(bool horizontal, bool vertical, bool resetZooms);

        /// <summary>
        /// Obtient ou définit le zoom.
        /// </summary>
        double ZoomX { get; set; }

        /// <summary>
        /// Obtient ou définit le zoom.
        /// </summary>
        double ZoomY { get; set; }
    }
}
