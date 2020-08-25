using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un contrôle permettant de sélectionner un ou plusieurs référentiels.
    /// </summary>
    [TemplatePart(Name = PART_NewReferential, Type = typeof(ReferentialPickerItem))]
    public class ReferentialPicker : ListBox
    {
        private const string PART_NewReferential = "PART_NewReferential";
        private ReferentialPickerItem _newReferentialItem;

        static ReferentialPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ReferentialPicker), new FrameworkPropertyMetadata(typeof(ReferentialPicker)));
        }

        #region DPs

        /// <summary>
        /// Obtient ou définit .
        /// </summary>
        public ICommand CreateReferentialCommand
        {
            get { return (ICommand)GetValue(CreateReferentialCommandProperty); }
            set { SetValue(CreateReferentialCommandProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CreateReferentialCommand"/>.
        /// </summary>
        public static readonly DependencyProperty CreateReferentialCommandProperty =
            DependencyProperty.Register("CreateReferentialCommand", typeof(ICommand), typeof(ReferentialPicker),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la sélection multiple est activée.
        /// </summary>
        public bool CanSelectMultiple
        {
            get { return (bool)GetValue(CanSelectMultipleProperty); }
            set { SetValue(CanSelectMultipleProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="CanSelectMultiple"/>.
        /// </summary>
        public static readonly DependencyProperty CanSelectMultipleProperty =
            DependencyProperty.Register("CanSelectMultiple", typeof(bool), typeof(ReferentialPicker),
            new UIPropertyMetadata(false, null));

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        public ProcessReferentialIdentifier? ReferentialId
        {
            get { return (ProcessReferentialIdentifier?)GetValue(ReferentialIdProperty); }
            set { SetValue(ReferentialIdProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ReferentialId"/>.
        /// </summary>
        public static readonly DependencyProperty ReferentialIdProperty =
            DependencyProperty.Register("ReferentialId", typeof(ProcessReferentialIdentifier?), typeof(ReferentialPicker),
            new UIPropertyMetadata(null, OnReferentialIdChanged));

        /// <summary>
        /// Appelé lorsque la valeur de la propriété de dépendance <see cref="ReferentialId"/> a changé.
        /// </summary>
        /// <param name="d">L'instance où la propriété a changé.</param>
        /// <param name="e">Les arguments de l'évènement <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>.</param>
        private static void OnReferentialIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (ReferentialPicker)d;
            var referentialId = ((ProcessReferentialIdentifier?)e.NewValue).Value;

            if (DesignMode.IsInDesignMode)
                return;

            var referentialsInfo = IoC.Resolve<IReferentialsUseService>().Referentials[referentialId];
            if (!referentialsInfo.IsEnabled)
            {
                source.Visibility = Visibility.Collapsed;
            }
            else
            {
                source.Visibility = Visibility.Visible;
                source.Header = IoC.Resolve<IReferentialsUseService>().GetLabel(referentialId);
                source.CanSelectMultiple = referentialsInfo.HasMultipleSelection;
                source.HasQuantity = referentialsInfo.HasQuantity;
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant le composant gère les quantités.
        /// </summary>
        public bool HasQuantity
        {
            get { return (bool)GetValue(HasQuantityProperty); }
            set { SetValue(HasQuantityProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HasQuantity"/>.
        /// </summary>
        public static readonly DependencyProperty HasQuantityProperty =
            DependencyProperty.Register("HasQuantity", typeof(bool), typeof(ReferentialPicker),
            new UIPropertyMetadata(false));

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
            DependencyProperty.Register("CanCreate", typeof(bool), typeof(ReferentialPicker),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits));

        #region Header

        /// <summary>
        /// Obtient ou définit l'entête.
        /// </summary>
        [Localizability(LocalizationCategory.Label), Bindable(true), Category("Content")]
        public string Header
        {
            get { return (string)base.GetValue(HeaderProperty); }
            set { base.SetValue(HeaderProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string),
            typeof(ReferentialPicker),
            new FrameworkPropertyMetadata(null));

        #endregion

        #endregion

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ReferentialPickerItem;
        }

        /// <inheritdoc />
        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new ReferentialPickerItem();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (_newReferentialItem != null)
                _newReferentialItem.PreviewMouseLeftButtonDown -= OnNewReferentialItemPreviewMouseLeftButtonDown;

            _newReferentialItem = base.GetTemplateChild(PART_NewReferential) as ReferentialPickerItem;

            _newReferentialItem.PreviewMouseLeftButtonDown += OnNewReferentialItemPreviewMouseLeftButtonDown;
        }

        private void OnNewReferentialItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.CreateReferentialCommand != null && this.CreateReferentialCommand.CanExecute(this.ItemsSource))
                this.CreateReferentialCommand.Execute(this.ItemsSource);
        }

    }
}
