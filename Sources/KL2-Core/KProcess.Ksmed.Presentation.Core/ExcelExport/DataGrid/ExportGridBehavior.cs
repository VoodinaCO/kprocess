using DocumentFormat.OpenXml.Packaging;
using KProcess.Globalization;
using KProcess.Ksmed.Presentation.Core.ExcelExport;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet d'exporter une grille via un clic droit sur la grille.
    /// </summary>
    public class ExportGridBehavior : LoadedBehavior<DataGrid>
    {

        /// <summary>
        /// La clé de la ressource localisée.
        /// </summary>
        private const string _excelExportResourceKey = "DataGrid_ExcelExport";

        private MenuItem _menuItem;

        private DataGrid _target;

        /// <summary>
        /// Appelé lorsque l'objet attaché a été chargé.
        /// </summary>
        protected override void OnLoaded()
        {
            if (AssociatedObject is Controls.KGanttChartDataGrid)
                _target = ((Controls.KGanttChartDataGrid)AssociatedObject).DataTreeGrid;
            else
                _target = AssociatedObject;

            string header = LocalizationManager.GetString(_excelExportResourceKey);

            if (_target.ContextMenu != null)
                _menuItem = _target.ContextMenu.Items.OfType<MenuItem>().FirstOrDefault(mi => (mi.Header as string) == header);
            else
                _target.ContextMenu = new ContextMenu();

            if (_menuItem == null)
            {
                _menuItem = new MenuItem() { Header = header };
                _target.ContextMenu.Items.Add(_menuItem);
            }

            _menuItem.Click += OnExportExcelClick;
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_menuItem != null)
                _menuItem.Click -= OnExportExcelClick;
        }

        /// <summary>
        /// Appelé lorsque l'utilisateur clique sur le bouton d'export excel
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">Les <see cref="RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private async void OnExportExcelClick(object sender, RoutedEventArgs e)
        {
            var formats = new Dictionary<ColumnFormat, ExportFormatBehavior>();

            IEnumerable<DataGridColumn> columns;

            if (_target is DlhSoft.Windows.Controls.GanttChartDataGrid)
                columns = ((DlhSoft.Windows.Controls.GanttChartDataGrid)_target).Columns;
            else
                columns = _target.Columns;

            var sortedVisibileColumns = columns
                .Where(c => c.Visibility == Visibility.Visible)
                .OrderBy(c => c.DisplayIndex)
                .ToArray();

            // On crée les formats compatibles avec l'exporter.
            foreach (DataGridColumn column in sortedVisibileColumns)
            {
                ColumnFormat excelFormat = new ColumnFormat();
                ExportFormatBehavior format = Interaction.GetBehaviors(column).OfType<ExportFormatBehavior>().FirstOrDefault();

                if (format != null &&
                    format.Binding != null)
                {
                    excelFormat.Header = format.Header ?? (column.Header != null ? column.Header.ToString() : string.Empty);
                    formats[excelFormat] = format;
                }
            }

            var data = new List<CellContent[]>();

            // On récupère les données grâce aux bindings
            foreach (object item in _target.Items)
            {
                CellContent[] row = new CellContent[formats.Count];

                int i = 0;
                foreach (var kvp in formats)
                {
                    var excelFormat = kvp.Key;
                    var behavior = kvp.Value;
                    // Appliquer le binding 
                    var element = new FrameworkElement
                    {
                        DataContext = item
                    };
                    BindingOperations.SetBinding(element, CellContentProperty, behavior.Binding);
                    object value = element.GetValue(CellContentProperty);
                    BindingOperations.ClearBinding(element, CellContentProperty);

                    row[i] = value?.ToString();

                    i++;
                }

                data.Add(row);
            }

            // Affiche la fenêtre de confirmation
            var result = IoC.Resolve<IDialogFactory>().GetDialogView<IExportDialog>().ShowExportToExcel(ExcelFormat.Xlsm);

            // On sauvegarde
            if (result.Accepts)
            {
                try
                {
                    string fileName = ExcelExporter.GetFileNameWithExtension(result.Filename);
                    ExcelExporter file = await ExcelExporter.Create(fileName);
                    WorksheetPart sheet = file.CreateSheet(LocalizationManager.GetString("DataGrid_ExcelExportSheetName"));
                    file.AddTable(sheet, formats.Keys.ToArray(), data.ToArray());
                    file.SaveAndClose();

                    if (result.OpenWhenCreated)
                        System.Diagnostics.Process.Start(fileName);
                }
                catch(ExcelExporter.FileAlreadyInUseExeption ex)
                {
                    TraceManager.TraceError(ex, ex.Message);
                    // Une notification a déjà été affichée dans ce cas précis
                }
                catch (Exception ex)
                {
                    TraceManager.TraceError(ex, ex.Message);
                    IoC.Resolve<IDialogFactory>().GetDialogView<IErrorDialog>().Show(ex.Message, LocalizationManager.GetString("Common_Error"), ex);
                }
            }


        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CellContent"/>.
        /// </summary>
        private static readonly DependencyProperty CellContentProperty = DependencyProperty.RegisterAttached("CellContent", typeof(string), typeof(ExportGridBehavior));


    }
}
