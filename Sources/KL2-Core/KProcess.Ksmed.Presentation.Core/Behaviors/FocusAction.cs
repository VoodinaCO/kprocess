using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Met le focus sur la cible.
    /// </summary>
    public class FocusAction : TargetedTriggerAction<FrameworkElement>
    {
        /// <summary>
        /// Invoque l'action.
        /// </summary>
        /// <param name="parameter">Paramètre de l'action. Si l'action ne nécessite pas de paramètre, le paramètre peut être défini sur une référence null.</param>
        protected override void Invoke(object parameter)
        {
            base.Dispatcher.BeginInvoke((Func<bool>)base.Target.Focus, System.Windows.Threading.DispatcherPriority.Loaded);
        }
    }
}
