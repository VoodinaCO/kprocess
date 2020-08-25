using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Associe la fermeture d'une <see cref="ChildWindow"/> avec l'évènement ShuttingDown d'un ViewModel.
    /// </summary>
    public class CloseChildWindowOnShutdownBehavior : Behavior<ChildWindow>
    {

        private IViewModel _viewModel;

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (base.AssociatedObject.DataContext != null)
                RegisterEvents();
            else
                base.AssociatedObject.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
        }

        /// <summary>
        /// Appelé lorsque le DataContext a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.AssociatedObject.DataContextChanged -= new DependencyPropertyChangedEventHandler(OnDataContextChanged);
            RegisterEvents();
        }

        /// <summary>
        /// S'abonner aux évènements du ViewModel.
        /// </summary>
        private void RegisterEvents()
        {
            _viewModel = base.AssociatedObject.DataContext as IViewModel;
            if (_viewModel != null)
                _viewModel.Shuttingdown += new EventHandler(ViewModel_Shuttingdown);
        }

        /// <summary>
        /// Appelé lorsque l'évènement ShuttingDown du view model est levé.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void ViewModel_Shuttingdown(object sender, EventArgs e)
        {
            base.AssociatedObject.Close();
            _viewModel.Shuttingdown -= new EventHandler(ViewModel_Shuttingdown);
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.DataContextChanged -= new DependencyPropertyChangedEventHandler(OnDataContextChanged);
            if (_viewModel != null)
                _viewModel.Shuttingdown -= new EventHandler(ViewModel_Shuttingdown);
            _viewModel = null;
        }

    }
}
