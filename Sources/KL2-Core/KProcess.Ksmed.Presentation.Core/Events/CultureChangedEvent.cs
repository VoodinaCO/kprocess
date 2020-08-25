using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Survient lorsque la langue de l'application a changé.
    /// </summary>
    public class CultureChangedEvent : EventBase
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CultureChangedEvent"/>.
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public CultureChangedEvent(object sender)
            : base(sender)
        {

        }

    }
}
