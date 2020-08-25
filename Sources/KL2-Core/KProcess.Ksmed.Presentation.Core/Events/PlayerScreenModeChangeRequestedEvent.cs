using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Survient lorsque un changement du mode d'affichage du player est demandé.
    /// </summary>
    public class PlayerScreenModeChangeRequestedEvent : EventBase
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PlayerScreenModeChangeRequestedEvent"/>.
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public PlayerScreenModeChangeRequestedEvent(object sender)
            : base(sender)
        {

        }
    }
}
