using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Affiche des filtres afin de filtre un ItemsControl.
    /// </summary>
    [TemplatePart(Name = ListFilter.PART_Label, Type = typeof(RadioButton))]
    [TemplatePart(Name = ListFilter.PART_Date, Type = typeof(RadioButton))]
    public class ListFilter : Control
    {

        #region Constantes

        private const string PART_Label = "PART_Label";
        private const string PART_Date = "PART_Date";

        #endregion

        #region Champs privés

        private RadioButton _label;
        private RadioButton _date;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Initialise la classe <see cref="ListFilter"/>.
        /// </summary>
        static ListFilter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListFilter), new FrameworkPropertyMetadata(typeof(ListFilter)));
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Gets or sets the ItemsControl.
        /// </summary>
        public ItemsControl ItemsControl
        {
            get { return (ItemsControl)GetValue(ItemsControlProperty); }
            set { SetValue(ItemsControlProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ItemsControl"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsControlProperty =
            DependencyProperty.Register("ItemsControl", typeof(ItemsControl), typeof(ListFilter), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the LabelPropertyName.
        /// </summary>
        public string LabelPropertyName
        {
            get { return (string)GetValue(LabelPropertyNameProperty); }
            set { SetValue(LabelPropertyNameProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LabelPropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelPropertyNameProperty =
            DependencyProperty.Register("LabelPropertyName", typeof(string), typeof(ListFilter), new UIPropertyMetadata("Label"));

        /// <summary>
        /// Gets or sets the DatePropertyName.
        /// </summary>
        public string DatePropertyName
        {
            get { return (string)GetValue(DatePropertyNameProperty); }
            set { SetValue(DatePropertyNameProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="DatePropertyName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DatePropertyNameProperty =
            DependencyProperty.Register("DatePropertyName", typeof(string), typeof(ListFilter), new UIPropertyMetadata("LastModificationDate"));

        /// <summary>
        /// Gets the CurrentSort.
        /// </summary>
        public string CurrentSort
        {
            get { return (string)GetValue(CurrentSortProperty); }
            private set { SetValue(CurrentSortPropertyKey, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CurrentSort"/> dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey CurrentSortPropertyKey =
            DependencyProperty.RegisterReadOnly("CurrentSort", typeof(string), typeof(ListFilter), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifies the <see cref="CurrentSort"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSortProperty = CurrentSortPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets the CurrentSortDirection.
        /// </summary>
        public ListSortDirection CurrentSortDirection
        {
            get { return (ListSortDirection)GetValue(CurrentSortDirectionProperty); }
            private set { SetValue(CurrentSortDirectionPropertyKey, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CurrentSortDirection"/> dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey CurrentSortDirectionPropertyKey =
            DependencyProperty.RegisterReadOnly("CurrentSortDirection", typeof(ListSortDirection), typeof(ListFilter), new UIPropertyMetadata(ListSortDirection.Ascending));
        /// <summary>
        /// Identifies the <see cref="CurrentSortDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentSortDirectionProperty = CurrentSortDirectionPropertyKey.DependencyProperty;

        #endregion

        #region Surcharges

        /// <summary>
        /// Surcharge d'OnApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_label != null)
                _label.Click -= new RoutedEventHandler(OnButtonClick);
            if (_date != null)
                _date.Click -= new RoutedEventHandler(OnButtonClick);

            _date = base.GetTemplateChild(PART_Date) as RadioButton;
            _label = base.GetTemplateChild(PART_Label) as RadioButton;

            if (_label != null)
                _label.Click += new RoutedEventHandler(OnButtonClick);
            if (_date != null)
                _date.Click += new RoutedEventHandler(OnButtonClick);
        }

        /// <summary>
        /// Appelé lorsqu'un bouton de tri a été cliqué.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Sort((ButtonBase)sender);
        }

        #endregion

        #region Méthode privées

        /// <summary>
        /// Applique le tri.
        /// </summary>
        /// <param name="source">La source de l'opération.</param>
        private void Sort(ButtonBase source)
        {
            if (ItemsControl == null || ItemsControl.ItemsSource == null)
                return;

            var cv = CollectionViewSource.GetDefaultView(ItemsControl.ItemsSource);
            if (cv == null)
                return;

            var currentSort = cv.SortDescriptions.FirstOrDefault();

            string nextSort;

            if (source == _label)
            {
                _label.IsChecked = true;
                _date.IsChecked = false;
                nextSort = this.LabelPropertyName;
            }
            else if (source == _date)
            {
                _label.IsChecked = false;
                _date.IsChecked = true;
                nextSort = this.DatePropertyName;
            }
            else
                throw new ArgumentOutOfRangeException("source");

            SortDescription sd;

            if (currentSort == null || !string.Equals(currentSort.PropertyName, nextSort, StringComparison.InvariantCultureIgnoreCase))
            {
                // On part sur un tri ascendant sur une nouvelle propriété
                sd = new SortDescription(nextSort, ListSortDirection.Ascending);
            }
            else
            {
                // On inverse le tri
                sd = new SortDescription(nextSort, currentSort.Direction == ListSortDirection.Ascending ?
                    ListSortDirection.Descending :
                    ListSortDirection.Ascending);
            }

            cv.SortDescriptions.Clear();
            cv.SortDescriptions.Add(sd);

            this.CurrentSort = source.Name;
            this.CurrentSortDirection = sd.Direction;
        }

        #endregion

    }
}
