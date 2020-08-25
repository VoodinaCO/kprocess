using Kprocess.KL2.TabletClient.Behaviors;
using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Views
{
    public partial class Formation : UserControl
    {
        static double DefaultGroupHeight = 178;
        static double DefaultNotGroupHeight = 178;

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        #region Constructors

        public Formation()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// Méthode permettant de définir la hauteur de
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DataGrid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var visualContainer = DataGrid.GetVisualContainer();

            try
            {
                var items = (TrackableCollection<PublishedAction>)DataGrid.ItemsSource;
                int nbRows = visualContainer.RowCount;
                int? nbItems = items?.Count;

                if (nbItems.HasValue)
                {
                    for (int i = 0; i < nbItems; i++)
                    {
                        if (items[i].IsGroup)
                        {
                            if (DataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[2 * i + 1] = Math.Max(autoHeight, DefaultGroupHeight);
                            else
                                visualContainer.RowHeights[2 * i + 1] = DefaultGroupHeight;
                        }
                        else
                        {
                            if (DataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[2 * i + 1] = Math.Max(autoHeight, DefaultNotGroupHeight);
                            else
                                visualContainer.RowHeights[2 * i + 1] = DefaultNotGroupHeight;
                        }
                        if (items[i].LinkedPublication != null)
                            DataGrid.UpdateDataRow(DataGrid.ResolveToRowIndex(items[i]));
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
            CustomColumnSizerBehavior.SetIsEnabled(e.DetailsViewDataGrid, true);
            var visualContainer = e.DetailsViewDataGrid.GetVisualContainer();

            try
            {
                var items = (TrackableCollection<PublishedAction>)e.DetailsViewDataGrid.ItemsSource;
                int nbRows = visualContainer.RowCount;
                int? nbItems = items?.Count;

                if (nbItems.HasValue)
                {
                    for (int i = 0; i < nbItems; i++)
                    {
                        if (items[i].IsGroup)
                        {
                            if (e.DetailsViewDataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[i + 1] = Math.Max(autoHeight, DefaultGroupHeight);
                            else
                                visualContainer.RowHeights[i + 1] = DefaultGroupHeight;
                        }
                        else
                        {
                            if (e.DetailsViewDataGrid.GridColumnSizer.GetAutoRowHeight(i, gridRowResizingOptions, out double autoHeight))
                                visualContainer.RowHeights[i + 1] = Math.Max(autoHeight, DefaultNotGroupHeight);
                            else
                                visualContainer.RowHeights[i + 1] = DefaultNotGroupHeight;
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

        #endregion

        void DataGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        {
            if (DataContext is FormationViewModel context)
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
                }
                else
                {
                    Locator.Main.IsLoading = true;

                    try
                    {
                        PublishedAction action = context.Publication.PublishedActions.FirstOrDefault(p => p.PublishedActionId == pAction.PublishedActionId);

                        context.IndexParent = null;
                        context.Index = context.Publication.PublishedActions.IndexOf(action);

                        context.PublishedAction = action;

                        await Locator.Navigation.PushDialog<FormationActionDetailsDialog, FormationViewModel>(context);
                    }
                    catch (Exception ex)
                    {
                        Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans la formation.");
                    }
                    finally
                    {
                        Locator.Main.IsLoading = false;
                    }
                }*/
                context.PublishedAction = pAction;
                context.ShowStepCommand.Execute(pAction);
            }
        }

        void DetailsViewDataGrid_CellTapped(object sender, GridCellTappedEventArgs e)
        {
            if (DataContext is FormationViewModel context)
            {
                PublishedAction pAction = e.Record as PublishedAction;
                if (pAction == null)
                    return;
                /*if (pAction.IsGroup)
                    return;*/

                /*Locator.Main.IsLoading = true;

                try
                {
                    PublishedAction action = null;
                    PublishedAction parent = context.Publication.PublishedActions.FirstOrDefault(p => p.LinkedPublicationId == pAction.PublicationId);
                    if (parent != null)
                    {
                        action = parent.LinkedPublication.PublishedActions.FirstOrDefault(p => p.PublishedActionId == pAction.PublishedActionId);
                        context.IndexParent = context.Publication.PublishedActions.IndexOf(parent);
                        context.Index = parent.LinkedPublication.PublishedActions.IndexOf(action);
                    }

                    context.PublishedAction = action;

                    await Locator.Navigation.PushDialog<FormationActionDetailsDialog, FormationViewModel>(context);
                }
                catch (Exception ex)
                {
                    Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans la formation.");
                }
                finally
                {
                    Locator.Main.IsLoading = false;
                }*/
                context.PublishedAction = pAction;
                context.ShowStepCommand.Execute(pAction);
            }
        }
    }
}
