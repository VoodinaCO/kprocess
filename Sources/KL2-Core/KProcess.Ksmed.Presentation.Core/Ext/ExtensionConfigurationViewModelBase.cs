using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un VM de base pour les écrans de configuration des extensions.
    /// </summary>
    public abstract class ExtensionConfigurationViewModelBase : ViewModelBase, IExtensionConfigurationViewModel
    {
        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        public virtual void OnNavigatingAway()
        {
        }
    }
}
