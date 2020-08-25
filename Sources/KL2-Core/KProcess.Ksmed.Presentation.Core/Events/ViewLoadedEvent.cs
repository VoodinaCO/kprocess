using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Notifie une vue chargée
    /// </summary>
    public class ViewLoadedEvent : EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="view">la vue à l'origine de l'événement</param>
        /// <param name="viewModel">Le VM associé.</param>
        public ViewLoadedEvent(IView view, IViewModel viewModel)
            : base(view)
        {
            this.ViewModel = viewModel;
        }

        /// <summary>
        /// Obtient le vm associé
        /// </summary>
        public IViewModel ViewModel { get; private set; }
    }
}
