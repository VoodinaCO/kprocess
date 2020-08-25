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
    public class NavigationRequestedEvent : EventBase
    {
        /// <summary>
        /// code du menu
        /// </summary>
        public string MenuCode { get; private set; }

        /// <summary>
        /// code du sous menu
        /// </summary>
        public string SubMenuCode { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CultureChangedEvent"/>.
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public NavigationRequestedEvent(object sender, string menuCode, string subMenuCode)
            : base(sender)
        {
            this.MenuCode = menuCode;
            this.SubMenuCode = subMenuCode;
        }

    }
}
