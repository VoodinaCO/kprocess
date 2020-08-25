using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un élément de la barre de notifications.
    /// </summary>
    [TemplatePart(Name = NotificationBarItem.PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = NotificationBarItem.PART_ToggleButton, Type = typeof(ToggleButton))]
    public class NotificationBarItem : ListBoxItem
    {

        #region Constantes

        private const string PART_Popup = "PART_Popup";
        private const string PART_ToggleButton = "PART_ToggleButton";

        #endregion

        #region Champs privés

        private Popup _popup;
        private ToggleButton _toggleButton;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Initialise la classe <see cref="NotificationBarItem"/>.
        /// </summary>
        static NotificationBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NotificationBarItem), new FrameworkPropertyMetadata(typeof(NotificationBarItem)));
        }

        #endregion

        #region Propriétés de dépendance

        /// <summary>
        /// Obtient ou définit l'entête.
        /// </summary>
        [Localizability(LocalizationCategory.Label), Bindable(true), Category("Content")]
        public object Header
        {
            get { return base.GetValue(HeaderProperty); }
            set { base.SetValue(HeaderProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object),
            typeof(NotificationBarItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le StringFormat du contenu.
        /// </summary>
        [Bindable(true), Category("Content")]
        public string HeaderStringFormat
        {
            get { return (string)base.GetValue(HeaderStringFormatProperty); }
            set { base.SetValue(HeaderStringFormatProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HeaderStringFormat"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderStringFormatProperty = DependencyProperty.Register("HeaderStringFormat",
            typeof(string), typeof(NotificationBarItem), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le DataTemplate de l'entête.
        /// </summary>
        [Bindable(true), Category("Content")]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)base.GetValue(HeaderTemplateProperty); }
            set { base.SetValue(HeaderTemplateProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HeaderTemplate"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate",
            typeof(DataTemplate), typeof(NotificationBarItem), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit le DataTemplateSelector de l'entête.
        /// </summary>
        [Bindable(true), Category("Content")]
        public DataTemplateSelector HeaderTemplateSelector
        {
            get { return (DataTemplateSelector)base.GetValue(HeaderTemplateSelectorProperty); }
            set { base.SetValue(HeaderTemplateSelectorProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HeaderTemplateSelector"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateSelectorProperty = DependencyProperty.Register("HeaderTemplateSelector",
            typeof(DataTemplateSelector), typeof(NotificationBarItem), new FrameworkPropertyMetadata(null));

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le <see cref="Selector"/> auquel l'élément appartient.
        /// </summary>
        internal Selector ParentSelector
        {
            get { return (ItemsControl.ItemsControlFromItemContainer(this) as Selector); }
        }

        #endregion

        #region Surcharges

        /// <summary>
        /// Appelé lorsque la Template est appliquée.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _popup = base.GetTemplateChild(PART_Popup) as Popup;

            if (_popup != null)
                _popup.CustomPopupPlacementCallback = null;

            if (_popup != null)
                _popup.CustomPopupPlacementCallback = CustomPopupPlacementCallback;

            if (_toggleButton != null)
            {
                _toggleButton.Checked -= new RoutedEventHandler(OnChecked);
                _toggleButton.Unchecked -= new RoutedEventHandler(OnUnchecked);
            }
            _toggleButton = base.GetTemplateChild(PART_ToggleButton) as ToggleButton;
            if (_toggleButton != null)
            {
                _toggleButton.Checked += new RoutedEventHandler(OnChecked);
                _toggleButton.Unchecked += new RoutedEventHandler(OnUnchecked);

                if (this.IsSelected)
                    _toggleButton.IsChecked = true;
            }
        }

        /// <summary>
        /// Appelé lorsque l'instance est sélectionnée.
        /// </summary>
        /// <param name="e">Les données de l'évènement.</param>
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            if (_toggleButton != null)
                _toggleButton.IsChecked = true;
        }

        /// <summary>
        /// Appelé lorsque l'instance est désélectionnée.
        /// </summary>
        /// <param name="e">Les données de l'évènement.</param>
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            if (_toggleButton != null)
                _toggleButton.IsChecked = false;
        }

        /// <summary>
        /// Appelé lorsque le focus du clavier a changé dans l'élément ou un de ses enfants.
        /// </summary>
        /// <param name="e">Les données de l'évènement.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsSelected && !base.IsKeyboardFocusWithin)
                this.IsSelected = false;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque le bouton est décoché.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            this.IsSelected = true;
            this.Focus();
        }

        /// <summary>
        /// Appelé lorsque le bouton est coché.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            this.IsSelected = false;
        }

        /// <summary>
        /// Fournit le placement du popup.
        /// </summary>
        /// <param name="popupSize">La taille du popup.</param>
        /// <param name="targetSize">La taille de la cible.</param>
        /// <param name="offset">Le décalage.</param>
        /// <returns>Les placements du Popup.</returns>
        private CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            var left = targetSize.Width - popupSize.Width;
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(left, targetSize.Height), PopupPrimaryAxis.Horizontal),
            };
        }

        #endregion

    }
}
