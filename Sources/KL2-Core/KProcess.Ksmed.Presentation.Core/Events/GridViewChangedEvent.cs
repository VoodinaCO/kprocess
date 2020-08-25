using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Notifie un changement de vue sur la grille.
    /// </summary>
    public class GridViewChangedEvent : EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="action">L'action à exécuter.</param>
        public GridViewChangedEvent(object sender, GanttGridView view)
            :base(sender)
        {
            this.View = view;
        }

        /// <summary>
        /// Obtient la vue.
        /// </summary>
        public GanttGridView View { get; private set; }
    }
}
