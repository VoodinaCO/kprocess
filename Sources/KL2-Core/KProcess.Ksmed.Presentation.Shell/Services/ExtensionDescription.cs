using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Décrit une extension.
    /// </summary>
    internal class ExtensionDescription
    {

        public ExtensionDescription(IExtension extension, bool isVersionValid, bool isEnabled)
        {
            this.Extension = extension;
            this.IsVersionValid = isVersionValid;
            this.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Obtient ou définit l'extension.
        /// </summary>
        public IExtension Extension { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'extension a une version valide.
        /// </summary>
        public bool IsVersionValid { get; private set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'extension est activée.
        /// </summary>
        public bool IsEnabled { get; private set; }

    }
}
