using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Represents the behavior of a service that manages the themes.
    /// </summary>
    public interface IThemeManagerService : IPresentationService
    {

        /// <summary>
        /// Gets the available themes.
        /// </summary>
        IThemeDescription[] AvailableThemes { get; }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        IThemeDescription CurrentTheme { get; set; }

    }
}
