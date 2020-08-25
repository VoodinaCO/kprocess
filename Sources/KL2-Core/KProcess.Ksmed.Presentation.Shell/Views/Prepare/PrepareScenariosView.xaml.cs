using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KProcess.Globalization;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran Preparation - Scénarios.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IPrepareScenariosViewModel))]
    public partial class PrepareScenariosView : UserControl, IView
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PrepareScenariosView"/>.
        /// </summary>
        public PrepareScenariosView()
        {
            InitializeComponent();
            
            this.DataContextChanged += (s, e) =>
            {
                IoC.Resolve<IEventBus>().Unsubscribe(this);
                if (this.DataContext != null)
                    IoC.Resolve<IEventBus>()
                        .Subscribe<RefreshRequestedEvent>(ev =>
                        {
                            scenariosDataGrid.ItemsSource = null;
                            operatorsDataGrid.ItemsSource = null;
                            equipmentsDataGrid.ItemsSource = null;
                        });
            };
        }

        /// <summary>
        /// Gère l'évènement SelectionChanged du contrôle TabControl.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == tabControl && tabControl.SelectedIndex == 1)
            {
                scenariosDataGrid.ItemsSource = null;
                operatorsDataGrid.ItemsSource = null;
                equipmentsDataGrid.ItemsSource = null;
            }
        }

        /// <summary>
        /// Gère l'évènement Loaded du contrôle scenariosDataGrid.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void scenariosDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (scenariosDataGrid.ItemsSource == null)
            {
                var vm = this.DataContext as IPrepareScenariosViewModel;

                if (vm != null && vm.Summary != null)
                {
                    SummaryBuilder.BuildScenarios(vm.Summary, scenariosDataGrid);
                }
            }
        }

        /// <summary>
        /// Gère l'évènement Loaded du contrôle operatorsDataGrid.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void operatorsDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (operatorsDataGrid.ItemsSource == null)
            {
                var vm = this.DataContext as IPrepareScenariosViewModel;

                if (vm != null && vm.Summary != null)
                {
                    SummaryBuilder.BuildResources(vm.Summary, operatorsDataGrid, false);
                }
            }
        }

        /// <summary>
        /// Gère l'évènement Loaded du contrôle equipmentsDataGrid.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void equipmentsDataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (equipmentsDataGrid.ItemsSource == null)
            {
                var vm = this.DataContext as IPrepareScenariosViewModel;

                if (vm != null && vm.Summary != null)
                {
                    SummaryBuilder.BuildResources(vm.Summary, equipmentsDataGrid, true);
                }
            }
        }
    }

}

