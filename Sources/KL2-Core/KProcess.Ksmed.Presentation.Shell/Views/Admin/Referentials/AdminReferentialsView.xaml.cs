using KProcess.Common;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Presentation.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de gestion des catégories d'action.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IAdminReferentialsViewModel))]
    public partial class AdminReferentialsView : UserControl, IView
    {
        private ToggleButton _menuToggleButtonReference = null;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AdminReferentialsView"/>.
        /// </summary>
        public AdminReferentialsView()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                this.DataContext = new KProcess.Ksmed.Presentation.ViewModels.AdminReferentialsViewModel();
            }

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.DisposeMenuToggleBehavior();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.InitializeMenuToggleBehavior();
        }

        #region MenuToggle behavior (User Story 4370)
        /*
         * CHE - On choisit ici une implémentation extremement spécifique étant donné que ce comportement fait l'objet d'une demande très spécifique et ne respectant qui plus est pas le model de données.
         * Un gain de temps conséquent est également à l'origine de ce choix à la demande de Mr. SAVARINO.
         */
        private void InitializeMenuToggleBehavior()
        {
            menu.AddHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(ToggleButtonCheckHandler));
            var vm = this.DataContext as AdminReferentialsViewModel;
            if (vm != null)
            {
                vm.PropertyChanged += VMPropertyChanged;
            }

            RefreshMenuToggleReference();
        }

        private void DisposeMenuToggleBehavior()
        {
            menu.RemoveHandler(ToggleButton.CheckedEvent, new RoutedEventHandler(ToggleButtonCheckHandler));
            var vm = this.DataContext as AdminReferentialsViewModel;
            if (vm != null)
            {
                vm.PropertyChanged -= VMPropertyChanged;
            }
        }

        private void RefreshMenuToggleReference()
        {
            _menuToggleButtonReference = VisualTreeHelperExtensions.FindFirstChild<ToggleButton>(this.menu);
        }

        private void VMPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var vm = this.DataContext as AdminReferentialsViewModel;
                if (vm != null
                    && e.PropertyName == ExtractFrom<AdminReferentialsViewModel>.MemberName(_ => _.SelectedView))
                {
                    if (_menuToggleButtonReference != null && !_menuToggleButtonReference.IsLoaded)
                    {
                        RefreshMenuToggleReference();
                    }

                    if (
                        !string.IsNullOrWhiteSpace(vm.SelectedView)
                        && !vm.Views.First(view => view.Key == vm.SelectedView).IsResource
                        && _menuToggleButtonReference != null
                        && _menuToggleButtonReference.IsLoaded)
                    {
                        _menuToggleButtonReference.IsChecked = false;
                    }
                }
            }));
        }

        private void ToggleButtonCheckHandler(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as AdminReferentialsViewModel;
            if (vm != null && vm.Views.Any() && _menuToggleButtonReference != null && e.OriginalSource == _menuToggleButtonReference)
            {
                vm.SelectedView = vm.Views.First().Key;
            }
        }
        #endregion
    }

}

