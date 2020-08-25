using Syncfusion.Windows.Tools.Controls;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell.Views.Wizard
{
    /// <summary>
    /// Logique d'interaction pour AddVideo_ChoosingResource.xaml
    /// </summary>
    public partial class AddVideo_ChoosingResource : WizardPage, IGotFocus
    {
        public AddVideo_ChoosingResource()
        {
            InitializeComponent();
        }

        async Task IGotFocus.GotFocus()
        {
            while (!ResourcesCombo.IsFocused)
            {
                ResourcesCombo.Focus();
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
