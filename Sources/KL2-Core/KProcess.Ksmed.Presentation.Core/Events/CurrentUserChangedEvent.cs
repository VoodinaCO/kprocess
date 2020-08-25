using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Survient lorsque l'utilisateur courant a changé.
    /// </summary>
    public class CurrentUserChangedEvent : EventBase
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CurrentUserChangedEvent"/>.
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public CurrentUserChangedEvent(object sender)
            : base(sender)
        {

        }

    }
}
