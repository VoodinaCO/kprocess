using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Interactivity;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Ajoute un ContextMenu qui liste les colonnes du DataGrid associé et permet de les afficher ou de les cacher.
    /// </summary>
    [Description("Ajoute un ContextMenu qui liste les colonnes du DataGrid associé et permet de les afficher ou de les cacher.")]
    public class DataGridColumnPickerBehavior : BehaviorBase<DataGrid>
    {
        private ContextMenu _contextMenu;

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (base.AssociatedObject.IsLoaded)
                Initialize();
            else
                base.AssociatedObject.Loaded += new System.Windows.RoutedEventHandler(AssociatedObject_Loaded);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.Loaded -= new System.Windows.RoutedEventHandler(AssociatedObject_Loaded);
            DataGridcolumns.CollectionChanged -= new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);

            if (_contextMenu != null)
            {
                foreach (MenuItem menuItem in _contextMenu.Items)
                {
                    menuItem.Checked -= new RoutedEventHandler(MenuItem_CheckedUnChecked);
                    menuItem.Unchecked -= new RoutedEventHandler(MenuItem_CheckedUnChecked);
                }
            }

            _contextMenu = null;
        }

        /// <summary>
        /// Appelé lorsque l'objet associé est chargé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            base.AssociatedObject.Loaded -= new System.Windows.RoutedEventHandler(AssociatedObject_Loaded);
            Initialize();
        }

        /// <summary>
        /// Obtient la liste des colonnes de la grille.
        /// </summary>
        protected virtual ObservableCollection<DataGridColumn> DataGridcolumns
        {
            get { return base.AssociatedObject.Columns; }
        }

        /// <summary>
        /// Initialise l'élément.
        /// </summary>
        private void Initialize()
        {
            var columnsHeaderPresenter = base.AssociatedObject.FindFirstChild<DataGridColumnHeadersPresenter>();

            if (columnsHeaderPresenter != null)
            {
                _contextMenu = new ContextMenu();

                foreach (var column in DataGridcolumns)
                    AddMenuItem(column);

                columnsHeaderPresenter.ContextMenu = _contextMenu;

                DataGridcolumns.CollectionChanged += new NotifyCollectionChangedEventHandler(Columns_CollectionChanged);

                OnInitialized();
            }
        }

        /// <summary>
        /// Appelé lorsque le Behavior a été initialisé.
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// Appelé lorsque la liste des colonnes a changé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (DataGridColumn column in e.NewItems)
                    AddMenuItem(column);

            if (e.OldItems != null)
                foreach (DataGridColumn column in e.OldItems)
                    RemoveMenuItem(column);
        }

        /// <summary>
        /// Ajoute un élément dans le ContextMenu.
        /// </summary>
        /// <param name="column">La colonne associée.</param>
        protected virtual void AddMenuItem(DataGridColumn column)
        {
            var mi = new MenuItem()
            {
                IsCheckable = true,
                DataContext = column,
            };

            var headerBinding = new Binding("Header") { Mode = BindingMode.OneWay };
            mi.SetBinding(MenuItem.HeaderProperty, headerBinding);

            var headerTemplateBinding = new Binding("HeaderTemplate") { Mode = BindingMode.OneWay };
            mi.SetBinding(MenuItem.HeaderTemplateProperty, headerTemplateBinding);

            var isCheckedBinding = new Binding("Visibility") { Mode = BindingMode.TwoWay, Converter = new VisibilityToBooleanConverter() };
            mi.SetBinding(MenuItem.IsCheckedProperty, isCheckedBinding);

            _contextMenu.Items.Add(mi);

            mi.Checked += new RoutedEventHandler(MenuItem_CheckedUnChecked);
            mi.Unchecked += new RoutedEventHandler(MenuItem_CheckedUnChecked);
        }

        /// <summary>
        /// Supprime l'élément correspondant à la colonne spécifiée du ContextMenu.
        /// </summary>
        /// <param name="column">La colonne associée.</param>
        private void RemoveMenuItem(DataGridColumn column)
        {
            var item = _contextMenu.Items.OfType<MenuItem>().FirstOrDefault(mi => mi.DataContext == column);
            if (item != null)
                _contextMenu.Items.Remove(item);
        }

        /// <summary>
        /// Appelé lorsque un élément du menu est coché ou décoché.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void MenuItem_CheckedUnChecked(object sender, RoutedEventArgs e)
        {
            var mi = (MenuItem)sender;
            OnMenuItemCheckStateChanged(mi, (DataGridColumn)mi.DataContext, mi.IsChecked);
        }

        /// <summary>
        /// Appelé lorsqu'un MenuItem a vu son état coché changé.
        /// </summary>
        /// <param name="mi">Le MenuItem.</param>
        /// <param name="column">La colonne associée.</param>
        /// <param name="isChecked">Son état coché.</param>
        protected virtual void OnMenuItemCheckStateChanged(MenuItem mi, DataGridColumn column, bool isChecked)
        {
        }

        /// <summary>
        /// Convertit un booléen en Visibilité (c'est un BooleanToVisibilityConverter inversé).
        /// </summary>
        private class VisibilityToBooleanConverter : IValueConverter
        {
            private BooleanToVisibilityConverter _btvc;

            public VisibilityToBooleanConverter()
            {
                _btvc = new BooleanToVisibilityConverter();
            }

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return _btvc.ConvertBack(value, targetType, parameter, culture);
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return _btvc.Convert(value, targetType, parameter, culture);
            }
        }

    }
}
