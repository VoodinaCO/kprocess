using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Demande un auto redimensionnement du Gantt.
    /// </summary>
    public class GanttAutoScaleEvent : EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public GanttAutoScaleEvent(object sender)
            : base(sender)
        {
        }
    }
}
