using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Evènement servant à indiquer qu'un rafraichissement est nécessaire.
    /// </summary>
    public class RefreshRequestedEvent : EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public RefreshRequestedEvent(object sender)
            : base(sender)
        {
        }

    }
}
