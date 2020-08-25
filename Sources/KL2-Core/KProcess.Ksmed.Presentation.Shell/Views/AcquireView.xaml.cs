using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using DlhSoft.Windows.Data;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using KProcess.Common;
using Syncfusion.Windows.Tools.Controls;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran d'acquisition du scénario principal.
    /// </summary>
    public partial class AcquireView : UserControl, IView
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AnalyzeAcquireView"/>.
        /// </summary>
        public AcquireView()
        {
            InitializeComponent();

            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            this.Loaded += (s, e) =>
            {
                var vm = (IAcquireViewModel)base.DataContext;
                vm.IsViewLoaded = true;
                this.SetupUnlinkMarkersMode();
                vm.PropertyChanged +=  OnVmPropertyChanged;
            };
            this.Unloaded += (s, e) =>
                {
                    var vm = (IAcquireViewModel)base.DataContext;
                    vm.PropertyChanged -= OnVmPropertyChanged;
                };
            this.MediaPlayer.Loaded += new RoutedEventHandler(MediaPlayer_Loaded);

            base.DataContextChanged += (s, e) =>
            {
                ((NotifiableObject)base.DataContext).PropertyChanged += OnPropertyChanged;

                IoC.Resolve<IEventBus>()
                    .Subscribe<GridViewChangedEvent>(ev =>
                    {
                        if (ev.Sender == base.DataContext)
                        {
                            UdpateColumnsVisibility(ev.View);
                            SetupUnlinkMarkersMode();
                        }
                    })
                    .Subscribe<FocusDefaultFieldWhenCreatingEvent>(ev =>
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            this.labelTB.Focus();
                        }), DispatcherPriority.Loaded);
                    })
                    .Subscribe<PlayerScreenModeChangeRequestedEvent>(ev =>
                    {
                        TogglePlayerScreenMode();
                    });
            };
            AnalyzeAcquireDataGrid.ItemContainerGenerator.StatusChanged += new EventHandler(ItemContainerGenerator_StatusChanged);

            this.DetailsGrid.PreviewKeyDown += new KeyEventHandler(DetailsGrid_PreviewKeyDown);

            this.ChangeVideoButton.Click += (sender, e) =>
            {
                var vm = ((IAcquireViewModel)base.DataContext);
                if(vm.UnselectItemCommand.CanExecute(null) && this.MediaPlayer.ChangeVideoCommand.CanExecute(null))
                {
                    vm.UnselectItemCommand.Execute(null);
                    this.Dispatcher.BeginInvoke(new Action(() => this.MediaPlayer.ChangeVideoCommand.Execute(null)));
                }
            };
        }

        private void OnVmPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == ExtractFrom<IAcquireViewModel>.MemberName(vm => vm.IsMarkersLinkedModeEnabled))
            {
                this.SetupUnlinkMarkersMode();
            }
        }

        /// <summary>
        /// Met à jour les colonnes relatives au mode de liaison des marqueurs
        /// </summary>
        private void SetupUnlinkMarkersMode()
        {
            var vm = (IAcquireViewModel)base.DataContext;
            if (!vm.IsMarkersLinkedModeEnabled)
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.ColumnPredecessors.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
        }

        /// <summary>
        /// Met à jour la visibilité des colonnes en fonction de la vue.
        /// </summary>
        /// <param name="view">La vue.</param>
        private void UdpateColumnsVisibility(GanttGridView view)
        {
            switch (view)
            {
                case GanttGridView.WBS:
                    GridBehavior.TryShow(ColumnVideo, view);
                    GridBehavior.TryShow(ColumnTask, view);
                    GridBehavior.TryShow(ColumnDuration, view);
                    GridBehavior.TryShow(ColumnCategory, view);
                    GridBehavior.TryShow(ColumnWBS, view);
                    GridBehavior.TryShow(ColumnResource, view);
                    GridBehavior.TryShow(ColumnPredecessors, view);
                    break;

                default:
                    GridBehavior.TryShow(ColumnVideo, view);
                    GridBehavior.TryShow(ColumnTask, view);
                    GridBehavior.TryShow(ColumnDuration, view);
                    GridBehavior.TryShow(ColumnCategory, view);
                    GridBehavior.TryHide(ColumnWBS, view);
                    GridBehavior.TryHide(ColumnResource, view);
                    GridBehavior.TryHide(ColumnPredecessors, view);
                    break;
            }

            SetupUnlinkMarkersMode();
        }

        /// <summary>
        /// Gère l'évènement Loaded du MediaPlayer.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void MediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            var b = new Binding("CurrentVideoPosition") { Mode = BindingMode.TwoWay };
            this.MediaPlayer.MediaElement.SetBinding(KMediaElement.MediaPositionProperty, b);
        }

        /// <summary>
        /// Appelé lorsque la valeur d'une propriété a changé sur le VM.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.ComponentModel.PropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CanChange":
                    Dispatcher.BeginInvoke((Action)RefreshRowsEnabled, DispatcherPriority.Background);
                    break;
            }
        }

        /// <summary>
        /// Gère l'évènement StatusChanged de l'ItemContainerGenerator de la grille.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (AnalyzeAcquireDataGrid.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                RefreshRowsEnabled();
        }

        /// <summary>
        /// Rafraichit l'état Activé des lignes de la grille.
        /// </summary>
        private void RefreshRowsEnabled()
        {
            bool enable = false;

            if (base.DataContext == null)
                enable = true;
            else
            {
                enable = (((IAcquireViewModel)base.DataContext).CanChange);
                foreach (var tuple in GetRows())
                {
                    if (tuple.DataGridRow != null)
                        tuple.DataGridRow.IsEnabled = enable || tuple.IsEnabled;
                }
            }
        }

        /// <summary>
        /// Obtient les lignes de la grille..
        /// </summary>
        /// <returns>Les lignes de la grille.</returns>
        private IEnumerable<(DataGridRow DataGridRow, bool IsEnabled)> GetRows()
        {
            var items = new List<(DataGridRow DataGridRow, bool IsEnabled)>();

            for (int i = 0; i < AnalyzeAcquireDataGrid.Items.Count; i++)
            {
                var item = (IGridItem)AnalyzeAcquireDataGrid.Items[i];
                items.Add((GetRow(AnalyzeAcquireDataGrid, i), item.IsEnabled));
            }

            return items;
        }

        /// <summary>
        /// Obtient le DataGridRow à l'index spécifié.
        /// </summary>
        /// <param name="index">L'index de la ligne.</param>
        private DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            return (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
        }

        /// <summary>
        /// Gère l'évènement PreviewKeyDown sur le contrôle DetailsGrid.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.KeyEventArgs"/> contenant les données de l'évènement.</param>
        private void DetailsGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = (IAcquireViewModel)base.DataContext;

            (Key FirstKey, Key LastKey)[] excludedRanges =
            {
                (Key.F1, Key.F24)
            };

            if (vm != null && vm.AutoPause && !excludedRanges.Any(range => (int)e.Key > (int)range.FirstKey && (int)e.Key < (int)range.LastKey))
                IoC.Resolve<IEventBus>().Publish(new MediaPlayerActionEvent(MediaPlayer, MediaPlayerAction.Pause));
        }

        private void OperatorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!OperatorsListBox.IsLoaded)
                return;
            CloseMultipleDropDownButtonOnSelectionChanged(OperatorsListBox, EquipmentsListBox);
        }

        private void EquipmentsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!EquipmentsListBox.IsLoaded)
                return;
            CloseMultipleDropDownButtonOnSelectionChanged(OperatorsListBox, EquipmentsListBox);
        }

        private void OperatorsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            CloseMultipleDropDownButtonOnSelectionChanged(OperatorsListBox, EquipmentsListBox);
        }

        private void EquipmentsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            CloseMultipleDropDownButtonOnSelectionChanged(OperatorsListBox, EquipmentsListBox);
        }

        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.CategoriesListBox);
        }

        private void SkillsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CloseDropDownButtonOnSelectionChanged(this.SkillsListBox);
        }

        private void CloseMultipleDropDownButtonOnSelectionChanged(params ReferentialPicker[] refPickers)
        {
            if (refPickers.Any(e => !e.IsLoaded))
                return;

            // Ne pas fermer si on est en mode saisie
            foreach(var refPicker in refPickers)
                for (int i = 0; i < refPicker.Items.Count; i++)
                {
                    var item = (ReferentialPickerItem)refPicker.ItemContainerGenerator.ContainerFromIndex(i);
                    if (item != null && item.IsEditable)
                        return;
                }

            var ddb = VisualTreeHelperExtensions.TryFindParent<Core.Controls.DropDownButton>(refPickers.First());
            if (ddb != null)
            {
                ddb.IsOpen = false;
            }
        }

        private void CloseDropDownButtonOnSelectionChanged(ReferentialPicker e)
        {
            if (!e.IsLoaded)
                return;

            // Ne pas fermer si on est en mode saisie
            for (int i = 0; i < e.Items.Count; i++)
            {
                var item = (ReferentialPickerItem)e.ItemContainerGenerator.ContainerFromIndex(i);
                if (item != null && item.IsEditable)
                    return;
            }

            var ddb = VisualTreeHelperExtensions.TryFindParent<Core.Controls.DropDownButton>(e);
            if (ddb != null)
            {
                ddb.IsOpen = false;
            }
        }

        void TogglePlayerScreenMode()
        {
            var minHeight = MediaPlayer.MinHeight + DetailsGrid.ActualHeight;
            bool isReduced = Math.Abs(VideoPlayerRowDefinition.ActualHeight - minHeight) < 10;

            if (isReduced)
                VideoPlayerRowDefinition.Height = new GridLength(GlobalRow.ActualHeight - 90, GridUnitType.Pixel);
            else
            {
                VideoPlayerRowDefinition.Height = new GridLength(minHeight, GridUnitType.Pixel);
                ReferentialsRowDefinition.Height = new GridLength(1, GridUnitType.Star);
            }
        }

        void ProcessesTreeview_Collapsing(object sender, ExpandingCollapsingEventArgs e)
        {
            if ((e.OriginalSource as TreeViewItemAdv)?.DataContext is ProjectDir folder && folder.Id == -1)
                e.Cancel = true;
        }
    }
}

