using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'une action d'une extension 
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IExtBarAction<TViewModel>
        where TViewModel : IViewModel
    {

        /// <summary>
        /// Obtient le Guid unique de l'extension.
        /// </summary>
        Guid ExtensionId { get; }

        /// <summary>
        /// Obtient le libellé de l'action.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Obtient l'uri de l'icône en petite taille.
        /// </summary>
        Uri SmallIconUri { get; }

        /// <summary>
        /// Obtient l'uri de l'icône en grande taille.
        /// </summary>
        Uri LargeIconUri { get; }

        /// <summary>
        /// Définit le view model associé.
        /// </summary>
        TViewModel ViewModel { set; }

        /// <summary>
        /// Obtient l'action à exécuter.
        /// </summary>
        ICommand Action { get; }

    }
}
