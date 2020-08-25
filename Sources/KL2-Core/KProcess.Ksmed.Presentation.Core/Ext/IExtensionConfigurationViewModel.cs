using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation
{
    /// <summary>
    /// Définit le comprotement d'un VM de configuration d'extension
    /// </summary>
    public interface IExtensionConfigurationViewModel : IViewModel
    {
        
        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        void OnNavigatingAway();

    }
}
