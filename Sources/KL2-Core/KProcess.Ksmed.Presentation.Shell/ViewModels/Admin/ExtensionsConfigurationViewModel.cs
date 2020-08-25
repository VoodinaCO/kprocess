using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de configuration des extensions.
    /// </summary>
    class ExtensionsConfigurationViewModel : FrameContentViewModelBase, IExtensionsConfigurationViewModel
    {

        #region Champs privés

        IExtensionsManager _extensionsManager;
        private IExtensionConfigurationViewModel _currentExtensionConfigurationViewModel;

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading()
        {
            _extensionsManager = ServiceBus.Get<IExtensionsManager>();

            // Récupérer toutes les extensions
            CompositionContainer container = IoC.Resolve<CompositionContainer>();

            IEnumerable<ExtensionDescription> extensions = _extensionsManager.GetExtensions();

            Extensions = extensions.Select(ext =>
                new ExtensionViewModel(ext.Extension, ext.IsEnabled, this)
                {
                    IsVersionValid = ext.IsVersionValid,
                }).ToArray();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Extensions = new ExtensionViewModel[]
            {
                new ExtensionViewModel(new DesignExtension() { Label = "Ext1"}, true, this) { IsVersionValid = true },
                new ExtensionViewModel(new DesignExtension() { Label = "Ext2"}, false, this) { IsVersionValid = false },
                new ExtensionViewModel(new DesignExtension() { Label = "Ext3"}, true, this) { IsVersionValid = true },
                new ExtensionViewModel(new DesignExtension() { Label = "Ext3"}, true, this) { IsVersionValid = false },
            };

            CurrentExtension = Extensions[0];

            return Task.CompletedTask;
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            _currentExtensionConfigurationViewModel?.OnNavigatingAway();

            return Task.FromResult(true);
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            // Sauvegarde l'état désactivé

            _extensionsManager.SaveEnabledStates();
        }

        #endregion

        #region Propriétés

        private ExtensionViewModel[] _extensions;
        /// <summary>
        /// Obtient les extensions.
        /// </summary>
        public ExtensionViewModel[] Extensions
        {
            get { return _extensions; }
            private set
            {
                if (_extensions != value)
                {
                    _extensions = value;
                    OnPropertyChanged("Extensions");
                }
            }
        }

        private ExtensionViewModel _currentExtension;
        /// <summary>
        /// Obtient ou définit l'extension en cours.
        /// </summary>
        public ExtensionViewModel CurrentExtension
        {
            get { return _currentExtension; }
            set
            {
                if (_currentExtension != value)
                {
                    var old = _currentExtension;
                    _currentExtension = value;
                    OnPropertyChanged("CurrentExtension");
                    OnCurrentExtensionChanged(old, value);
                }
            }
        }

        private IView _currentExtensionConfigurationView;
        /// <summary>
        /// Obtient la vue de l'extension actuellement affichée.
        /// </summary>
        public IView CurrentExtensionConfigurationView
        {
            get { return _currentExtensionConfigurationView; }
            private set
            {
                if (_currentExtensionConfigurationView != value)
                {
                    _currentExtensionConfigurationView = value;
                    OnPropertyChanged("CurrentExtensionConfigurationView");
                }
            }
        }

        #endregion

        #region Commandes

        #endregion

        #region Public methods

        /// <summary>
        /// Appelé lorsqu'une extension est activée ou désactivée.
        /// </summary>
        /// <param name="extension">L'extension.</param>
        public void OnIsEnabledChanged(ExtensionViewModel extension)
        {
            _extensionsManager.SetExtensionEnabled(extension.Extension.ExtensionId, extension.IsEnabled);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Appelé lorsque l'extension courante est changée.
        /// </summary>
        /// <param name="previousExtension">L'ancienne extension.</param>
        /// <param name="newExtension">La nouvelle extension.</param>
        private void OnCurrentExtensionChanged(ExtensionViewModel previousExtension, ExtensionViewModel newExtension)
        {
            if (previousExtension != null && _currentExtensionConfigurationViewModel != null)
                _currentExtensionConfigurationViewModel.OnNavigatingAway();

            if (this.CurrentExtension != null && this.CurrentExtension.Extension.HasConfiguration)
            {
                IView view;
                _currentExtensionConfigurationViewModel = this.CurrentExtension.Extension.GetExtensionConfiguration(out view);
                this.CurrentExtensionConfigurationView = view;
            }
            else
            {
                this.CurrentExtensionConfigurationView = null;
                _currentExtensionConfigurationViewModel = null;
            }
        }

        #endregion

        /// <summary>
        /// Représente une extension pour le mode design.
        /// </summary>
        [System.ComponentModel.Composition.PartNotDiscoverable]
        private class DesignExtension : IExtension
        {

            public Guid ExtensionId
            {
                get { return Guid.NewGuid(); }
            }

            public string Label { get; set; }

            public bool HasConfiguration
            {
                get { return false; }
            }

            public IExtensionConfigurationViewModel GetExtensionConfiguration(out IView extensionConfigurationView)
            {
                throw new NotSupportedException();
            }

            public Version MinimumApplicationVersion
            {
                get { return new Version("0.0.0.1"); }
            }
        }

    }
}