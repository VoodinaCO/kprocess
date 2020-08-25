using KProcess.Globalization;
using KProcess.KL2.Business.Impl.Desktop;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Réprésente le conteneur d'un élément d'un menu de l'application.
    /// </summary>
    [TemplateVisualState(GroupName = VisualStates.SelectionStates, Name = VisualStates.SelectionSelected)]
    [TemplateVisualState(GroupName = VisualStates.SelectionStates, Name = VisualStates.SelectionSelectedUnfocused)]
    public class MenuItem : ContentControl
    {

        #region Propriétés de dépendance

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est sélectionné.
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsSelected"/>.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété de dépendance <see cref="IsSelected"/> a changé.
        /// </summary>
        /// <param name="d">L'objet de dépendance où la propriété a changé.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)d).UpdateVisualState(true);
        }

        /// <summary>
        /// Obtient ou définit la commande à exécuter lorsque cette instance est sélectionnée.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(MenuItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit la visibilité du séparateur.
        /// </summary>
        public Visibility SeparatorVisibility
        {
            get { return (Visibility)GetValue(SeparatorVisibilityProperty); }
            set { SetValue(SeparatorVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SeparatorVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorVisibilityProperty =
            DependencyProperty.Register("SeparatorVisibility", typeof(Visibility), typeof(MenuItem), new UIPropertyMetadata(Visibility.Visible));

        #endregion

        public MenuItem()
        {
            DataContextChanged += (o, e) =>
            {
                var appMenuItem = e.NewValue as ApplicationMenuItem;
                if (appMenuItem == null)
                    return;
                if (appMenuItem.Code == "AdminBackupRestore")
                {
                    // On désactive le menu si on est en distant
                    IsEnabled = DataBaseService.IsLocalDbStatic();
                    // On active un tooltip si on désactive le MenuItem
                    if (!IsEnabled)
                    {
                        ToolTipService.SetShowOnDisabled(this, true);
                        ToolTipService.SetToolTip(this, LocalizationManager.GetString("VM_MainWindowView_BackupRestoreOnlyOnLocal"));
                    }
                }
            };
        }

        /// <summary>
        /// Survient lors d'un clic gauche.
        /// </summary>
        /// <param name="e">Les <see cref="T:System.Windows.Input.MouseButtonEventArgs"/>.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var control = ItemsControl.ItemsControlFromItemContainer(this);
            if (control == null)
                return;

            var item = control.ItemContainerGenerator.ItemFromContainer(this) ?? this;

            if (this.Command != null && this.Command.CanExecute(item))
            {
                this.Command.Execute(item);
            }
        }

        /// <summary>
        /// Met à jour le VisualState?
        /// </summary>
        /// <param name="useTransitions"><c>true</c> pour utiliser les transitions.</param>
        private void UpdateVisualState(bool useTransitions)
        {
            if (this.IsSelected)
                VisualStateManager.GoToState(this, "Selected", useTransitions);
            else
                VisualStateManager.GoToState(this, "Unselected", useTransitions);
        }

    }
}
