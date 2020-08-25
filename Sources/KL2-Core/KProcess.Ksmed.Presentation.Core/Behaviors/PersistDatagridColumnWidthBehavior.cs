using KProcess.Ksmed.Business;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    public class PersistDatagridColumnWidthBehavior : Behavior<DataGrid>
    {
        private readonly IUISettingsService _uiSettingsService = null;
        private readonly PropertyDescriptor _propertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        private string _screenHostAutoKey = null;

        public string DataGridKey
        {
            get { return (string)GetValue(DataGridKeyProperty); }
            set { SetValue(DataGridKeyProperty, value); }
        }

        public static readonly DependencyProperty DataGridKeyProperty = DependencyProperty.Register("DataGridKey", typeof(string), typeof(PersistDatagridColumnWidthBehavior), new PropertyMetadata());

        public PersistDatagridColumnWidthBehavior()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            var serviceBus = IoC.Resolve<IServiceBus>();
            _uiSettingsService = serviceBus.Get<IUISettingsService>();
            _timer.Interval = TimeSpan.FromSeconds(1);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.Loaded += OnLoaded;
            this.AssociatedObject.Unloaded += OnUnloaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            UnregisterLoad();
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.Unloaded -= OnUnloaded;
        }

        private async Task Persist()
        {
            if (_screenHostAutoKey == null && string.IsNullOrWhiteSpace(DataGridKey))
                return;

            string uiPartId = $"{_screenHostAutoKey}/{DataGridKey}";
            var columnWidths = AssociatedObject.Columns
                .Where(column => !string.IsNullOrWhiteSpace(GetColumnKey(column)))
                .ToDictionary(column => GetColumnKey(column), column => column.ActualWidth);

            await _uiSettingsService.SaveColumnsInfo(uiPartId, columnWidths);
        }

        private async Task<bool> Recover()
        {
            if (_screenHostAutoKey == null && string.IsNullOrWhiteSpace(DataGridKey))
                return false;

            string uiPartId = $"{_screenHostAutoKey}/{DataGridKey}";

            try
            {
                Dictionary<string, double> data = await _uiSettingsService.GetColumnsInfo(uiPartId);

                ApplyColumnInfo(data); // Mettre en dictionnaire
                return true;
            }
            catch (Exception e)
            {
                TraceManager.TraceError(e, "PersistDatagridColumnWidthBehavior: {0} Column recovery failed", uiPartId);
                return false;
            }
        }

        /*private void ApplyColumnInfo(KeyValuePair<string, double>[] columnData)
        {
            if (!AssociatedObject.IsLoaded) return;

            foreach (var columnInfo in columnData)
            {
                var targetedColumn = AssociatedObject.Columns.FirstOrDefault(column => columnInfo.Key == GetColumnKey(column));

                if (targetedColumn != null)
                    targetedColumn.Width = columnInfo.Value;
            }
        }*/
        private void ApplyColumnInfo(Dictionary<string, double> columnData)
        {
            if (!AssociatedObject.IsLoaded) return;

            foreach (var columnInfo in columnData)
            {
                var targetedColumn = AssociatedObject.Columns.FirstOrDefault(column => columnInfo.Key == GetColumnKey(column));

                if (targetedColumn != null)
                    targetedColumn.Width = columnInfo.Value;
            }
        }

        private void GenerateHostKey()
        {
            var host = FindHost(AssociatedObject) ?? FindSubstituteHost(AssociatedObject);
            if (host == null)
            {
                _screenHostAutoKey = null;
                TraceManager.TraceWarning("PersistDatagridColumnWidthBehavior: Unable to find a host for current DataGrid {0}. Column width cannot be persisted", this.AssociatedObject.Name ?? "#NoName#");
            }
            else
                _screenHostAutoKey = host.GetType().Name;
        }

        private static DependencyObject FindHost(DependencyObject from)
        {
            if (from == null)
                return null;

            var parent = VisualTreeHelper.GetParent(from);

            if (parent is UserControl && parent is IView && parent.GetType().GetCustomAttributes(typeof(ViewExportAttribute), true).Any())
                return parent;

            return FindHost(parent);
        }

        private static DependencyObject FindSubstituteHost(DependencyObject from)
        {
            if (from == null)
                return null;

            var parent = VisualTreeHelper.GetParent(from);

            if (parent is UserControl && parent is IView)
                return parent;

            return FindHost(parent);
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            TraceManager.TraceDebug("PersistDatagridColumnWidthBehavior: Saving column widths...");
            await Persist();
        }

        private async void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GenerateHostKey();
            if (await Recover())
                await Dispatcher.BeginInvoke(new Action(RegisterLoad), DispatcherPriority.Loaded);
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UnregisterLoad();
        }

        private void RegisterLoad()
        {
            if (!AssociatedObject.IsLoaded || string.IsNullOrWhiteSpace(_screenHostAutoKey)) return;

            _timer.Tick += OnTimerTick;
            foreach (var column in AssociatedObject.Columns)
            {
                _propertyDescriptor.AddValueChanged(column, new EventHandler(OnColumnWidthPropertyChanged));
            }
        }

        private void UnregisterLoad()
        {
            _timer.Tick -= OnTimerTick;
            foreach (var column in AssociatedObject.Columns)
            {
                _propertyDescriptor.RemoveValueChanged(column, new EventHandler(OnColumnWidthPropertyChanged));
            }
        }

        private void OnColumnWidthPropertyChanged(object sender, EventArgs e)
        {
            _timer.Stop();
            _timer.Start();
        }


        /// <summary>
        /// Obtient la clef de la colonne utilisée pour identifier l'information de la colonne persistée
        /// </summary>
        /// <param name="obj">L'instance sur laquelle est définie la propriété attachée</param>
        /// <returns>La valeur de la clef</returns>
        public static string GetColumnKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnKeyProperty);
        }

        /// <summary>
        /// Définit la clef de la colonne utilisée pour identifier l'information de la colonne persistée
        /// </summary>
        /// <param name="obj">L'instance sur laquelle est définie la propriété attachée</param>
        /// <param name="value">La valeur de la clef</param>
        public static void SetColumnKey(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnKeyProperty, value);
        }

        public static readonly DependencyProperty ColumnKeyProperty = DependencyProperty.RegisterAttached("ColumnKey", typeof(string), typeof(PersistDatagridColumnWidthBehavior), new PropertyMetadata());


    }
}