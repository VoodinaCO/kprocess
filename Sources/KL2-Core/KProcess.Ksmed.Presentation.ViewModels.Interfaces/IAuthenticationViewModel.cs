using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{

    /// <summary>
    /// Définit le comportement du VM d'authentification.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IAuthenticationViewModel : IViewModel
    {
        
        /// <summary>
        /// Obtient la langue sélectionnée.
        /// </summary>
        Language SelectedLanguage { get; }

    }

}
