using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de configuration des extensions.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IExtensionsConfigurationViewModel : IFrameContentViewModel
    {
        /// <summary>
        /// Obtient les extensions.
        /// </summary>
        ExtensionViewModel[] Extensions { get; }

        /// <summary>
        /// Obtient ou définit l'extension en cours.
        /// </summary>
        ExtensionViewModel CurrentExtension { get; }

        /// <summary>
        /// Obtient la vue de l'extension actuellement affichée.
        /// </summary>
        IView CurrentExtensionConfigurationView { get; }

        /// <summary>
        /// Appelé lorsqu'une extension est activée ou désactivée.
        /// </summary>
        /// <param name="extension">L'extension.</param>
        void OnIsEnabledChanged(ExtensionViewModel extension);
    }


    /// <summary>
    /// Représente un wrapper autour d'une extension.
    /// </summary>
    public class ExtensionViewModel : NotifiableObject
    {
        private IExtensionsConfigurationViewModel _parent;
        public ExtensionViewModel(IExtension extension, bool isEnabled, IExtensionsConfigurationViewModel parent)
        {
            Extension = extension;
            _parent = parent;
            _isEnabled = isEnabled;
        }

        public IExtension Extension { get; private set; }

        public string Label
        {
            get { return Extension.Label; }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                    _parent.OnIsEnabledChanged(this);
                }
            }
        }

        public bool IsVersionValid { get; set; }
    }
}
