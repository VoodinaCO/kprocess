using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un référentiel.
    /// </summary>
    [TemplatePart(Name = ReferentialPickerItem.PART_Label, Type = typeof(TextBlock))]
    [TemplatePart(Name = ReferentialPickerItem.PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = ReferentialPickerItem.PART_Quantity, Type = typeof(TextBox))]
    public class ReferentialPickerItem : ListBoxItem
    {
        #region Constantes

        private const string PART_Label = "PART_Label";
        private const string PART_TextBox = "PART_TextBox";
        private const string PART_Quantity = "PART_Quantity";

        #endregion

        #region Champs privés

        private TextBlock _label;
        private TextBox _textBox;
        private TextBox _quantity;

        #endregion

        static ReferentialPickerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReferentialPickerItem), new FrameworkPropertyMetadata(typeof(ReferentialPickerItem)));
        }

        #region DPs

        /// <summary>
        /// Obtient ou définit le référentiel associé.
        /// </summary>
        public IActionReferential Referential
        {
            get { return (IActionReferential)GetValue(ReferentialProperty); }
            set { SetValue(ReferentialProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Referential"/>.
        /// </summary>
        public static readonly DependencyProperty ReferentialProperty =
            DependencyProperty.Register("Referential", typeof(IActionReferential), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(null, OnReferentialChanged));

        private static void OnReferentialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(IsStandardProperty);
        }

        /// <summary>
        /// Obtient une valeur indiquant si le référentiel est standard.
        /// </summary>
        public bool IsStandard
        {
            get { return (bool)GetValue(IsStandardProperty); }
            private set { SetValue(IsStandardPropertyKey, value); }
        }
        /// <summary>
        /// Identifie la clé de la propriété de dépendance <see cref="IsStandard"/>.
        /// </summary>
        private static readonly DependencyPropertyKey IsStandardPropertyKey =
            DependencyProperty.RegisterReadOnly("IsStandard", typeof(bool), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(false, null, OnCoerceIsStandard));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsStandard"/>.
        /// </summary>
        public static readonly DependencyProperty IsStandardProperty = IsStandardPropertyKey.DependencyProperty;

        private static object OnCoerceIsStandard(DependencyObject d, object baseValue)
        {
            var rpi = (ReferentialPickerItem)d;
            return rpi.Referential != null && (rpi.Referential as IActionReferentialProcess)?.ProcessId == null;
        }


        /// <summary>
        /// Obtient ou définit une valeur indiquant si le référentiel est modifiable.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsEditable"/>.
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(false, OnIsEditableChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="IsEditable"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (ReferentialPickerItem)d;
            if (source.IsEditable)
                source.BeginEdit();
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant l'instance est utilisée pour représenter une nouveau référentiel.
        /// </summary>
        public bool IsNewReferential
        {
            get { return (bool)GetValue(IsNewReferentialProperty); }
            set { SetValue(IsNewReferentialProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsNewReferential"/>.
        /// </summary>
        public static readonly DependencyProperty IsNewReferentialProperty =
            DependencyProperty.Register("IsNewReferential", typeof(bool), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(false));

        /// <summary>
        /// Obtient ou définit la quantité.
        /// </summary>
        public int Quantity
        {
            get { return (int)GetValue(QuantityProperty); }
            set { SetValue(QuantityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Quantity"/>.
        /// </summary>
        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(int), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(0));


        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Label"/>.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ReferentialPickerItem),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si de nouveaux référentiels peuvent être créés.
        /// </summary>
        public bool CanCreate
        {
            get { return (bool)GetValue(CanCreateProperty); }
            set { SetValue(CanCreateProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CanCreate"/>.
        /// </summary>
        public static readonly DependencyProperty CanCreateProperty = ReferentialPicker.CanCreateProperty.AddOwner(typeof(ReferentialPickerItem));

        #endregion

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _label = base.GetTemplateChild(PART_Label) as TextBlock;
            _textBox = base.GetTemplateChild(PART_TextBox) as TextBox;
            _quantity = base.GetTemplateChild(PART_Quantity) as TextBox;

            if (this.IsEditable)
                BeginEdit();

            UpdateQuantityVisibility();
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (e.Source == _quantity)
                return;

            if (this.Referential != null && Selector.GetIsSelected(this))
            {
                Selector.SetIsSelected(this, false);
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            UpdateQuantityVisibility();
        }

        /// <inheritdoc />
        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            UpdateQuantityVisibility();
        }

        /// <summary>
        /// Met à jour la visibilité de la TextBox de visibilité.
        /// </summary>
        private void UpdateQuantityVisibility()
        {
            if (_quantity != null)
                _quantity.Visibility = HasQuantity && (IsSelected || IsEditable) ? Visibility.Visible : Visibility.Collapsed;
        }

        #region Gestion édition

        private bool HasQuantity
        {
            get 
            { 
                var picker = this.TryFindParent<ReferentialPicker>();
                if( picker != null)
                    return picker.HasQuantity; 
                else
                    return false;
            }
        }

        /// <summary>
        /// Démarre la modification.
        /// </summary>
        private void BeginEdit()
        {
            if (_textBox != null && _label != null)
            {
                this.KeyDown -= new System.Windows.Input.KeyEventHandler(TextBox_KeyDown);

                _label.Visibility = Visibility.Collapsed;

                _textBox.Visibility = Visibility.Visible;
                _quantity.Visibility = HasQuantity ? Visibility.Visible : Visibility.Collapsed;

                _textBox.Focus();

                this.KeyDown += new System.Windows.Input.KeyEventHandler(TextBox_KeyDown);
            }
        }

        /// <summary>
        /// Arrête la modification.
        /// </summary>
        private void EndEdit()
        {
            if (_textBox != null && _label != null)
            {
                _label.Visibility = Visibility.Visible;

                _textBox.Visibility = Visibility.Collapsed;

                this.KeyDown -= new System.Windows.Input.KeyEventHandler(TextBox_KeyDown);

                this.IsEditable = false;
            }
        }

        /// <inheritdoc />
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsEditable && !this.IsKeyboardFocusWithin)
                EndEdit();
        }

        /// <summary>
        /// Gère l'évènement KeyDown sur l'objet associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.KeyEventArgs"/> contenant les données de l'évènement.</param>
        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                EndEdit();
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                _textBox.SetCurrentValue(TextBox.TextProperty, string.Empty);
                if (HasQuantity)
                    _quantity.SetCurrentValue(TextBox.TextProperty, "1");

                EndEdit();
            }
        }

        #endregion
    }
}
