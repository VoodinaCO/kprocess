using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un contrôle permettant de sélectionner un process.
    /// </summary>
    [TemplatePart(Name = PART_NewProcess, Type = typeof(ProcessPickerItem))]
    public class ProcessPicker : ListBox
    {
        const string PART_NewProcess = "PART_NewProcess";
        ProcessPickerItem _newProcessItem;

        static ProcessPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessPicker), new FrameworkPropertyMetadata(typeof(ProcessPicker)));
        }

        #region DPs

        /// <summary>
        /// Obtient ou définit .
        /// </summary>
        public ICommand CreateProcessCommand
        {
            get { return (ICommand)GetValue(CreateProcessCommandProperty); }
            set { SetValue(CreateProcessCommandProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CreateProcessCommand"/>.
        /// </summary>
        public static readonly DependencyProperty CreateProcessCommandProperty =
            DependencyProperty.Register(nameof(CreateProcessCommand), typeof(ICommand), typeof(ProcessPicker),
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
        public static readonly DependencyProperty CanCreateProperty =
            DependencyProperty.Register(nameof(CanCreate), typeof(bool), typeof(ProcessPicker),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        #endregion

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ProcessPickerItem;
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ProcessPickerItem();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (_newProcessItem != null)
                _newProcessItem.PreviewMouseLeftButtonDown -= OnNewProcessItemPreviewMouseLeftButtonDown;

            _newProcessItem = GetTemplateChild(PART_NewProcess) as ProcessPickerItem;

            _newProcessItem.PreviewMouseLeftButtonDown += OnNewProcessItemPreviewMouseLeftButtonDown;
        }

        void OnNewProcessItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CreateProcessCommand != null && CreateProcessCommand.CanExecute(ItemsSource))
                CreateProcessCommand.Execute(ItemsSource);
        }

    }
}
