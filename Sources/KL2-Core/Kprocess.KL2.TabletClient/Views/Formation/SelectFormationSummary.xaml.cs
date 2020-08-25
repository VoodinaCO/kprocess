using Kprocess.KL2.TabletClient.Behaviors;
using Kprocess.KL2.TabletClient.Common;
using Kprocess.KL2.TabletClient.Converter;
using Kprocess.KL2.TabletClient.Extensions;
using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Kprocess.KL2.TabletClient.Views
{
    /// <summary>
    /// Logique d'interaction pour SelectFormationSummary.xaml
    /// </summary>
    public partial class SelectFormationSummary : UserControl
    {
        static double DefaultGroupHeight = 178;
        static double DefaultNotGroupHeight = 178;

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        #region Constructors

        public SelectFormationSummary()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var visualContainer = DataGrid.GetVisualContainer();

            try
            {
                var items = (TrackableCollection<PublishedAction>)DataGrid.ItemsSource;
                int? nbItems = items?.Count;
                int multiple = DataGrid.DetailsViewDefinition.Count == 0 ? 1 : 2;

                if (nbItems.HasValue && visualContainer.RowHeights.GetLineCount() >= multiple * nbItems + 1)
                {
                    for (int i = 0; i < nbItems; i++)
                    {
                        if (items[i].IsGroup)
                        {
                            if (DataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[multiple * i + 1] = Math.Max(autoHeight, DefaultGroupHeight);
                            else
                                visualContainer.RowHeights[multiple * i + 1] = DefaultGroupHeight;
                        }
                        else
                        {
                            if (DataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[multiple * i + 1] = Math.Max(autoHeight, DefaultNotGroupHeight);
                            else
                                visualContainer.RowHeights[multiple * i + 1] = DefaultNotGroupHeight;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la modification de la hauteur des lignes du tableau.");
            }

            visualContainer.InvalidateMeasure();
        }

        void DataGrid_DetailsViewLoading(object sender, DetailsViewLoadingAndUnloadingEventArgs e)
        {
            Publication linkedPublication = null;
            TrackableCollection<PublishedAction> publishedActions = null;
            if (e.DetailsViewDataGrid.ItemsSource is TrackableCollection<PublishedAction> source && source.Any())
            {
                publishedActions = source;
                linkedPublication = source.First().Publication;
            }

            if (e.DetailsViewDataGrid.SerializationController != null && !(e.DetailsViewDataGrid.SerializationController is LinkedPublicationDataGridSerialization))
            {
                e.DetailsViewDataGrid.SerializationController = new LinkedPublicationDataGridSerialization(e.DetailsViewDataGrid);
                e.DetailsViewDataGrid.CellTapped += DetailsViewDataGrid_CellTapped;
                if (linkedPublication?.Formation_Disposition != null && linkedPublication.Formation_Disposition.Length > 1)
                {
                    e.DetailsViewDataGrid.CustomDeserialize(linkedPublication.Formation_Disposition);

                    // On supprime les colonnes inutiles
                    e.DetailsViewDataGrid.RemoveHiddenColumns();
                    // On renomme les colonnes
                    e.DetailsViewDataGrid.RenameColumns();
                }
                e.DetailsViewDataGrid.SortComparers.Add(new SortComparer { Comparer = new WBSComparer(), PropertyName = nameof(PublishedAction.WBS) });
                e.DetailsViewDataGrid.SortColumnDescriptions.Clear();
                e.DetailsViewDataGrid.SortColumnDescriptions.Add(new SortColumnDescription { ColumnName = nameof(PublishedAction.WBS), SortDirection = ListSortDirection.Ascending });
                e.DetailsViewDataGrid.ExpanderColumnWidth = 0;
                CustomColumnSizerBehavior.SetIsEnabled(e.DetailsViewDataGrid, true);

                //var columnSizer = (CustomColumnSizer)e.DetailsViewDataGrid.GridColumnSizer;
                /*try
                {
                    e.DetailsViewDataGrid.Columns["WBS"].Width = publishedActions.Select(_ => columnSizer.MeasureText(IndentWBSConverter.Instance.Convert(_.WBS), DataGrid.Columns["WBS"], true).Width).Max() + 10;
                }
                catch { }*/
                e.DetailsViewDataGrid.Columns[nameof(PublishedAction.WBS)].DisplayBinding = new Binding(nameof(PublishedAction.WBS)) { Mode = BindingMode.OneWay, Converter = IndentWBSConverter.Instance };
                e.DetailsViewDataGrid.Columns[nameof(PublishedAction.Label)].CellTemplateSelector = Resources["LabelStyleSelector"] as DataTemplateSelector;
                if (e.DetailsViewDataGrid.Columns.Any(_ => _.MappingName == nameof(PublishedAction.PublishedResource)))
                    e.DetailsViewDataGrid.Columns[nameof(PublishedAction.PublishedResource)].CellTemplateSelector = Resources["PublishedResourceDataGridTemplateSelector"] as DataTemplateSelector;
                if (e.DetailsViewDataGrid.Columns.Any(_ => _.MappingName == nameof(PublishedAction.PublishedActionCategory)))
                    e.DetailsViewDataGrid.Columns[nameof(PublishedAction.PublishedActionCategory)].CellTemplateSelector = Resources["PublishedCategoryDataGridTemplateSelector"] as DataTemplateSelector;
            }

            var visualContainer = e.DetailsViewDataGrid.GetVisualContainer();

            try
            {
                var items = (TrackableCollection<PublishedAction>)e.DetailsViewDataGrid.ItemsSource;
                int? nbItems = items?.Count;

                if (nbItems.HasValue)
                {
                    for (int i = 0; i < nbItems; i++)
                    {
                        if (items[i].IsGroup)
                        {
                            if (e.DetailsViewDataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[2 * i + 1] = Math.Max(autoHeight, DefaultGroupHeight);
                            else
                                visualContainer.RowHeights[2 * i + 1] = DefaultGroupHeight;
                        }
                        else
                        {
                            if (e.DetailsViewDataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[2 * i + 1] = Math.Max(autoHeight, DefaultNotGroupHeight);
                            else
                                visualContainer.RowHeights[2 * i + 1] = DefaultNotGroupHeight;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la modification de la hauteur des lignes du tableau.");
            }

            visualContainer.InvalidateMeasure();
        }

        /// <summary>
        /// Méthode appellé lorsque la source de donnée de la grille change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_ItemsSourceChanged(object sender, GridItemsSourceChangedEventArgs e)
        {
            if (DataContext is SelectFormationSummaryViewModel context && e.NewItemsSource != null && context?.Publication?.Formation_Disposition != null)
            {
                DataGrid.CustomDeserialize(context.Publication.Formation_Disposition, true);

                // On supprime les colonnes inutiles
                DataGrid.RemoveHiddenColumns();
                // On renomme les colonnes
                DataGrid.RenameColumns();

                DataGrid.DetailsViewPadding = new Thickness(0, 10, 10, 10);
                CustomColumnSizerBehavior.SetIsEnabled(DataGrid, true);

                //var columnSizer = (CustomColumnSizer)DataGrid.GridColumnSizer;
                //DataGrid.Columns["WBS"].Width = context.Publication.PublishedActions.Select(_ => columnSizer.MeasureText(IndentWBSConverter.Instance.Convert(_.WBS), DataGrid.Columns["WBS"]).Width).Max() + 10;
                DataGrid.Columns[nameof(PublishedAction.WBS)].DisplayBinding = new Binding(nameof(PublishedAction.WBS)) { Mode = BindingMode.OneWay, Converter = IndentWBSConverter.Instance };
                DataGrid.Columns[nameof(PublishedAction.Label)].CellTemplateSelector = Resources["LabelStyleSelector"] as DataTemplateSelector;
                if (DataGrid.Columns.Any(_ => _.MappingName == nameof(PublishedAction.PublishedResource)))
                    DataGrid.Columns[nameof(PublishedAction.PublishedResource)].CellTemplateSelector = Resources["PublishedResourceDataGridTemplateSelector"] as DataTemplateSelector;
                if (DataGrid.Columns.Any(_ => _.MappingName == nameof(PublishedAction.PublishedActionCategory)))
                    DataGrid.Columns[nameof(PublishedAction.PublishedActionCategory)].CellTemplateSelector = Resources["PublishedCategoryDataGridTemplateSelector"] as DataTemplateSelector;

                if (context != null)
                    context.DataGrid = DataGrid;
            }

            DataGrid_Loaded(sender, null);
        }

        void DataGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        {
            if (DataContext is SelectFormationSummaryViewModel context)
            {
                PublishedAction pAction = e.Record as PublishedAction;
                if (pAction == null)
                    return;
                /*if (pAction.IsGroup)
                    return;*/         
                /*if (pAction.LinkedPublication != null)
                {
                    int rowIndex = DataGrid.ResolveToRowIndex(pAction);
                    int recordIndex = DataGrid.ResolveToRecordIndex(rowIndex);
                    if (recordIndex < 0)
                        return;
                    if (DataGrid.View.Records[recordIndex].IsExpanded)
                        DataGrid.CollapseDetailsViewAt(recordIndex);
                    else
                        DataGrid.ExpandDetailsViewAt(recordIndex);
                    return;
                }
                context.IndexParent = null;*/
                context.PublishedAction = pAction;
                context.ShowStepCommand.Execute(pAction);
            }
        }

        void DetailsViewDataGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        {
            if (DataContext is SelectFormationSummaryViewModel context)
            {
                PublishedAction pAction = e.Record as PublishedAction;
                if (pAction == null)
                    return;
                /*if (pAction.IsGroup)
                    return;*/
                /*var parentPublishedAction = context.Publication.PublishedActions.First(_ => _.LinkedPublication != null && _.LinkedPublication.PublishedActions.Contains(pAction));
                context.IndexParent = context.Publication.PublishedActions.IndexOf(parentPublishedAction);*/
                context.PublishedAction = pAction;
                context.ShowStepCommand.Execute(pAction);
            }
        }

        #endregion
    }
}
