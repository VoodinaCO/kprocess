using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Associe l'attache associée au ViewModel présent dans le DataContext.
    /// </summary>
    [System.ComponentModel.Description("Associe l'attache associée au ViewModel présent dans le DataContext.")]
    public class ViewHandleBehavior : Behavior<FrameworkElement>
    {

        /// <summary>
        /// Appelé après que le Behavior ait été attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.DataContextChanged += new DependencyPropertyChangedEventHandler(OnDataContextChanged);
            UpdateViewHandle();
        }

        /// <summary>
        /// Appelé lorsque le data context a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateViewHandle();
        }

        /// <summary>
        /// Met à jour l'association avec l'attache.
        /// </summary>
        private void UpdateViewHandle()
        {
            var vm = this.AssociatedObject.DataContext as IViewModel;
            if (vm != null)
            {
                IoC.Resolve<IServiceBus>().Get<IViewHandleService>().Register(vm, base.AssociatedObject);
            }
            else
                IoC.Resolve<IServiceBus>().Get<IViewHandleService>().Release(base.AssociatedObject);
        }

        /// <summary>
        /// Appelé lorsque le Behavior est en train d'être détaché de l'AssociatedObject.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.DataContextChanged -= new DependencyPropertyChangedEventHandler(OnDataContextChanged);

            IoC.Resolve<IServiceBus>().Get<IViewHandleService>().Release(base.AssociatedObject);
        }

    }
}
