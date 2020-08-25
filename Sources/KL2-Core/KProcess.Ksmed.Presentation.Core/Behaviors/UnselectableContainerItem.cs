using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Controls.Primitives;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de déselectionner un élément en cliquant une nouvelle fois dessus.
    /// </summary>
    public class UnselectableContainerItem : Behavior<FrameworkElement>
    {

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            base.AssociatedObject.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            base.AssociatedObject.PreviewMouseLeftButtonDown -= new System.Windows.Input.MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown);
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selector.GetIsSelected(base.AssociatedObject))
            {
                Selector.SetIsSelected(base.AssociatedObject, false);
                e.Handled = true;
            }
        }
    }
}
