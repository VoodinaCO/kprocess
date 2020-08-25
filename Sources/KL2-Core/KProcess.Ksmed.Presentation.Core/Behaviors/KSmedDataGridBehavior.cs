using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Security;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Ajoute un ContextMenu qui liste les colonnes du DataGrid associé et permet de les afficher ou de les cacher.
    /// Stocke les colonnes sélectionnées dans un cache statique par utilisateur.
    /// </summary>
    public class KSmedDataGridBehavior : DataGridColumnPickerBehavior
    {
        private bool _isUpdatingVisibility = false;
        private bool _isUpdatingOrder = false;

        private Dictionary<GanttGridView, DataGridLayout> _columns;

        /// <summary>
        /// Obtient ou définit la vue courante
        /// </summary>
        public GanttGridView CurrentView
        {
            get { return (GanttGridView)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CurrentView"/>.
        /// </summary>
        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register("CurrentView", typeof(GanttGridView), typeof(KSmedDataGridBehavior),
            new UIPropertyMetadata(GanttGridView.WBS, OnCurrentViewChanged));

        private static void OnCurrentViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KSmedDataGridBehavior)d).RestoreLayout();
        }

        /// <summary>
        /// Obtient l'identifiant d'une colonne.
        /// </summary>
        /// <param name="obj">La colonne.</param>
        /// <returns>L'identifiant.</returns>
        public static string GetId(DataGridColumn obj)
        {
            return (string)obj.GetValue(IdProperty);
        }
        /// <summary>
        /// Définit l'identifiant d'une colonne.
        /// </summary>
        /// <param name="obj">La colonne.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="value">L'identifiant.</param>
        public static void SetId(DataGridColumn obj, string value)
        {
            obj.SetValue(IdProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Id"/>.
        /// </summary>
        public static readonly DependencyProperty IdProperty =
            DependencyProperty.RegisterAttached("Id", typeof(string), typeof(KSmedDataGridBehavior), new UIPropertyMetadata(null));


        /// <summary>
        /// Obtient une valeur indiquant si la visibilité de la colonne peut changer dynamiquement.
        /// </summary>
        /// <param name="obj">L'objet attaché.</param>
        /// <returns><c>true</c> si la visibilité de la colonne peut changer dynamiquement.</returns>
        public static bool GetCanChangeVisibility(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanChangeVisibilityProperty);
        }

        /// <summary>
        /// Obtient une valeur indiquant si la visibilité de la colonne peut changer dynamiquement.
        /// </summary>
        /// <param name="obj">L'objet attaché.</param>
        /// <param name="value"><c>true</c> si la visibilité de la colonne peut changer dynamiquement.</param>
        public static void SetCanChangeVisibility(DependencyObject obj, bool value)
        {
            obj.SetValue(CanChangeVisibilityProperty, value);
        }

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CanChangeVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty CanChangeVisibilityProperty =
            DependencyProperty.RegisterAttached("CanChangeVisibility", typeof(bool), typeof(KSmedDataGridBehavior), new UIPropertyMetadata(true));


        /// <summary>
        /// Obtient la liste des colonnes de la grille.
        /// </summary>
        protected override ObservableCollection<DataGridColumn> DataGridcolumns
        {
            get
            {
                if (base.AssociatedObject is GanttChartDataGrid)
                    return ((GanttChartDataGrid)base.AssociatedObject).Columns;
                else
                    return base.AssociatedObject.Columns;
            }
        }

        /// <summary>
        /// Obtient le DataGrid interne : dans le cas des Gantt, il est particulier.
        /// </summary>
        private DataGrid InternalDataGrid
        {
            get
            {
                if (base.AssociatedObject is GanttChartDataGrid)
                    return ((GanttChartDataGrid)base.AssociatedObject).DataTreeGrid;
                else
                    return base.AssociatedObject;
            }
        }

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.RetrieveLayout();

            IoC.Resolve<IEventBus>().Subscribe<ColumnLayoutChangedEvent>(e => OnColumnLayoutChangedEvent(e));
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.InternalDataGrid.ColumnDisplayIndexChanged -= new EventHandler<DataGridColumnEventArgs>(OnColumnDisplayIndexChanged);
            IoC.Resolve<IEventBus>().Unsubscribe(this);
        }

        /// <summary>
        /// Appelé lorsque l'index d'affichage a changé pour une des colonnes.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.Controls.DataGridColumnEventArgs"/> contenant les données de l'évènement.</param>
        private void OnColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            if (!_isUpdatingOrder)
            {
                SaveNewColumnOrder();
                PersistLayout();
            }
        }

        /// <summary>
        /// Sauvegarde le nouvel ordre des colonnes.
        /// </summary>
        private void SaveNewColumnOrder()
        {
            var order = new List<string>();
            foreach (var column in DataGridcolumns.OrderBy(c => c.DisplayIndex))
            {
                string columnName = GetId(column);
                if (columnName != null)
                    order.Add(columnName);
            }

            GetViewLayout(this.CurrentView).ColumnsOrder = order.ToArray();
        }

        private DataGridLayout GetViewLayout(GanttGridView view)
        {
            if (!_columns.ContainsKey(view))
                _columns[view] = new DataGridLayout
                {
                    ColumnsVisibilities = new Dictionary<string, bool>(),
                };

            return _columns[view];
        }

        /// <summary>
        /// Appelé lorsque la visibilité d'une colonne a changé.
        /// </summary>
        /// <param name="e">The e.</param>
        private void OnColumnLayoutChangedEvent(ColumnLayoutChangedEvent e)
        {
            if (e.Sender != this)
            {
                this.RetrieveLayout();
                this.RestoreLayout();
            }
        }

        /// <summary>
        /// Appelé lorsque le Behavior a été initialisé.
        /// </summary>
        protected override void OnInitialized()
        {
            if (base.AssociatedObject.Name == null)
                throw new ArgumentNullException("Le DataGrid associé doit obligatoirement avoir un nom (unique).");

            RestoreLayout();
            SaveNewColumnOrder();

            this.InternalDataGrid.ColumnDisplayIndexChanged += new EventHandler<DataGridColumnEventArgs>(OnColumnDisplayIndexChanged);
        }

        /// <summary>
        /// Réapplique les visibilités sauvegardées.
        /// </summary>
        private void RestoreLayout()
        {
            if (_columns == null)
                return;

            string[] order;

            if (_columns.ContainsKey(this.CurrentView))
                order = _columns[this.CurrentView].ColumnsOrder;
            else
                order = null;

            if (order != null)
            {
                int decrement = 0;
                for (int i = 0; i < order.Length; i++)
                {
                    var column = this.DataGridcolumns.FirstOrDefault(c => GetId(c) == order[i]);
                    if (column != null)
                    {
                        _isUpdatingOrder = true;
                        column.DisplayIndex = i + decrement;
                        _isUpdatingOrder = false;
                    }
                    else
                        decrement--;
                }
            }

            // restaurer les préférences
            foreach (var column in DataGridcolumns)
            {
                string columnName = GetId(column);
                if (columnName != null)
                {
                    if (GetCanChangeVisibility(column))
                    {
                        var vis = this.GetVisibility(columnName);
                        if (vis.HasValue)
                        {
                            _isUpdatingVisibility = true;
                            column.Visibility = vis.Value ? Visibility.Visible : Visibility.Collapsed;
                            _isUpdatingVisibility = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tente d'afficher la colonne spécifiée.
        /// L'affiche si l'utilisateur ne l'a pas cachée explicitement.
        /// </summary>
        /// <param name="column">La colonne.</param>
        /// <param name="view">La vue.</param>
        public void TryShow(DataGridColumn column, GanttGridView view)
        {
            bool canShow = true;

            if (!GetCanChangeVisibility(column))
                return;

            var columnName = GetId(column);
            var vis = GetVisibility(view, columnName);
            if (vis.HasValue && !vis.Value)
                canShow = false;

            _isUpdatingVisibility = true;
            column.Visibility = canShow ? Visibility.Visible : Visibility.Collapsed;
            _isUpdatingVisibility = false;
        }

        /// <summary>
        /// Tente de cacher la colonne spécifiée.
        /// L'affiche si l'utilisateur ne l'a pas affichée explicitement.
        /// </summary>
        /// <param name="column">La colonne.</param>
        /// <param name="view">La vue.</param>
        public void TryHide(DataGridColumn column, GanttGridView view)
        {
            bool canHide = true;

            if (!GetCanChangeVisibility(column))
                return;

            var columnName = GetId(column);
            var vis = GetVisibility(view, columnName);
            if (vis.HasValue && vis.Value)
                canHide = false;

            _isUpdatingVisibility = true;
            column.Visibility = canHide ? Visibility.Collapsed : Visibility.Visible;
            _isUpdatingVisibility = false;
        }

        /// <summary>
        /// Ajoute un élément dans le ContextMenu.
        /// </summary>
        /// <param name="column">La colonne associée.</param>
        protected override void AddMenuItem(DataGridColumn column)
        {
            if (GetId(column) != null)
                base.AddMenuItem(column);
        }

        /// <summary>
        /// Appelé lorsqu'un MenuItem a vu son état coché changé.
        /// </summary>
        /// <param name="mi">Le MenuItem.</param>
        /// <param name="column">La colonne associée.</param>
        /// <param name="isChecked">Son état coché.</param>
        protected override void OnMenuItemCheckStateChanged(MenuItem mi, DataGridColumn column, bool isChecked)
        {
            if (!_isUpdatingVisibility)
                ChangeCheckedState(column);
        }

        /// <summary>
        /// Change l'état coché dans le cache pour la colonne spécifiée.
        /// </summary>
        /// <param name="column">La colonne.</param>
        private void ChangeCheckedState(DataGridColumn column)
        {
            string columnName = GetId(column);
            SetVisibility(columnName, column.Visibility == Visibility.Visible);
            this.PersistLayout();
        }

        /// <summary>
        /// Obtient la visibilité de la colonne spécifiée dans la vue en cours.
        /// </summary>
        /// <param name="columnName">Le nom de la colonne.</param>
        /// <returns><c>null</c> si non défini.</returns>
        private bool? GetVisibility(string columnName)
        {
            return GetVisibility(this.CurrentView, columnName);
        }

        /// <summary>
        /// Obtient la visibilité de la colonne spécifiée dans la vue spécifiée.
        /// </summary>
        /// <param name="view">La vue.</param>
        /// <param name="columnName">Le nom de la colonne.</param>
        /// <returns>
        ///   <c>null</c> si non défini.
        /// </returns>
        private bool? GetVisibility(GanttGridView view, string columnName)
        {
            if (_columns.ContainsKey(view) && _columns[view].ColumnsVisibilities.ContainsKey(columnName))
                return _columns[view].ColumnsVisibilities[columnName];
            else
                return null;
        }

        /// <summary>
        /// Définit la visibilité de la colonne spécifiée dans la vue en cours.
        /// </summary>
        /// <param name="columnName">Le nom de la colonne.</param>
        /// <returns><c>null</c> si non défini.</returns>
        /// <param name="value">La visibilité.</param>
        private void SetVisibility(string columnName, bool value)
        {
            SetVisibility(this.CurrentView, columnName, value);
        }

        /// <summary>
        /// Définit la visibilité de la colonne spécifiée dans la vue spécifiée.
        /// </summary>
        /// <param name="view">La vue.</param>
        /// <param name="columnName">Le nom de la colonne.</param>
        /// <param name="value">La visibilité.</param>
        private void SetVisibility(GanttGridView view, string columnName, bool value)
        {
            GetViewLayout(view).ColumnsVisibilities[columnName] = value;
        }

        /// <summary>
        /// Récupère les visibilités déjà définies et les met en cache.
        /// </summary>
        private void RetrieveLayout()
        {
            var layout = IoC.Resolve<IServiceBus>().Get<ILayoutPersistanceService>().DataGridTryRetrieve();

            if (layout != null)
                _columns = new Dictionary<GanttGridView, DataGridLayout>(layout);
            else if (_columns == null)
                _columns = new Dictionary<GanttGridView, DataGridLayout>();
        }

        /// <summary>
        /// Persistes les visibilités spécifiées.
        /// </summary>
        private void PersistLayout()
        {
            var service = IoC.Resolve<IServiceBus>().Get<ILayoutPersistanceService>();

            if (_columns != null)
            {
                service.DataGridPersist(_columns);

                IoC.Resolve<IEventBus>().Publish(new ColumnLayoutChangedEvent(this));
            }
        }

        /// <summary>
        /// Survient lorsque le layout d'un DataGrid a changé.
        /// </summary>
        internal class ColumnLayoutChangedEvent : EventBase
        {
            public ColumnLayoutChangedEvent(object sender)
                : base(sender)
            {

            }
        }

    }
}
