using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un évènement levé lorsque le thème c ourant a changé.
    /// </summary>
    public class ThemeChangedEvent : EventBase
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ThemeChangedEvent"/>.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="oldTheme">L'ancien thème.</param>
        /// <param name="newTheme">Le nouveau thème.</param>
        public ThemeChangedEvent(object sender, IThemeDescription oldTheme, IThemeDescription newTheme)
            : base(sender)
        {
            OldTheme = oldTheme;
            NewTheme = newTheme;
        }

        /// <summary>
        /// Obtient l'ancien thème.
        /// </summary>
        public IThemeDescription OldTheme { get; private set; }

        /// <summary>
        /// Obtient le nouveau thème.
        /// </summary>
        public IThemeDescription NewTheme { get; private set; }
    }
}
