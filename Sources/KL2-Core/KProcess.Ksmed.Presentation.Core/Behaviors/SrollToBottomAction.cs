using System.Windows.Controls;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Permet au scrollViewer d'être positionné en bas sur l'evenement selectionné
    /// </summary>
    public class SrollToBottomAction: TargetedTriggerAction<ScrollViewer>
    {
        protected override void Invoke(object parameter)
        {
            if (this.Target != null)
            {
                this.Target.ScrollToBottom();
            }
        }
    }
}
