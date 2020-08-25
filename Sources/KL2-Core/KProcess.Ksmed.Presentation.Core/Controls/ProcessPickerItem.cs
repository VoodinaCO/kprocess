using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un process.
    /// </summary>
    [TemplatePart(Name = PART_Label, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class ProcessPickerItem : ListBoxItem
    {
        #region Constantes

        const string PART_Label = "PART_Label";
        const string PART_TextBox = "PART_TextBox";

        #endregion

        #region Champs privés

        TextBlock _label;
        TextBox _textBox;

        #endregion

        static ProcessPickerItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessPickerItem), new FrameworkPropertyMetadata(typeof(ProcessPickerItem)));
        }

        #region DPs

        /// <summary>
        /// Obtient ou définit le process associé.
        /// </summary>
        public Procedure Process
        {
            get { return (Procedure)GetValue(ProcessProperty); }
            set { SetValue(ProcessProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Process"/>.
        /// </summary>
        public static readonly DependencyProperty ProcessProperty =
            DependencyProperty.Register(nameof(Process), typeof(Procedure), typeof(ProcessPickerItem));

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
            DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(ProcessPickerItem),
            new UIPropertyMetadata(false, OnIsEditableChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="IsEditable"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnIsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (ProcessPickerItem)d;
            if (source.IsEditable)
                source.BeginEdit();
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant l'instance est utilisée pour représenter une nouveau process.
        /// </summary>
        public bool IsNewProcess
        {
            get { return (bool)GetValue(IsNewProcessProperty); }
            set { SetValue(IsNewProcessProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsNewProcess"/>.
        /// </summary>
        public static readonly DependencyProperty IsNewProcessProperty =
            DependencyProperty.Register(nameof(IsNewProcess), typeof(bool), typeof(ProcessPickerItem),
            new UIPropertyMetadata(false));

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
            DependencyProperty.Register(nameof(Label), typeof(string), typeof(ProcessPickerItem),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si de nouveaux process peuvent être créés.
        /// </summary>
        public bool CanCreate
        {
            get { return (bool)GetValue(CanCreateProperty); }
            set { SetValue(CanCreateProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CanCreate"/>.
        /// </summary>
        public static readonly DependencyProperty CanCreateProperty = ProcessPicker.CanCreateProperty.AddOwner(typeof(ProcessPickerItem));

        #endregion

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _label = GetTemplateChild(PART_Label) as TextBlock;
            _textBox = GetTemplateChild(PART_TextBox) as TextBox;

            if (IsEditable)
                BeginEdit();
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (Process != null && Selector.GetIsSelected(this))
            {
                Selector.SetIsSelected(this, false);
                e.Handled = true;
            }
        }

        #region Gestion édition

        /// <summary>
        /// Démarre la modification.
        /// </summary>
        void BeginEdit()
        {
            if (_textBox != null && _label != null)
            {
                KeyDown -= TextBox_KeyDown;

                _label.Visibility = Visibility.Collapsed;

                _textBox.Visibility = Visibility.Visible;

                _textBox.Focus();

                KeyDown += TextBox_KeyDown;
            }
        }

        /// <summary>
        /// Arrête la modification.
        /// </summary>
        void EndEdit()
        {
            if (_textBox != null && _label != null)
            {
                _label.Visibility = Visibility.Visible;

                _textBox.Visibility = Visibility.Collapsed;

                KeyDown -= TextBox_KeyDown;

                IsEditable = false;
            }
        }

        /// <inheritdoc />
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsEditable && !IsKeyboardFocusWithin)
                EndEdit();
        }

        /// <summary>
        /// Gère l'évènement KeyDown sur l'objet associé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.Input.KeyEventArgs"/> contenant les données de l'évènement.</param>
        void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EndEdit();
            else if (e.Key == Key.Escape)
            {
                _textBox.SetCurrentValue(TextBox.TextProperty, string.Empty);

                EndEdit();
            }
        }

        #endregion
    }
}
