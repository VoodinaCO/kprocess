using Syncfusion.Windows.Tools.Controls;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell.Views.Wizard
{
    /// <summary>
    /// Logique d'interaction pour AddVideo_CameraName.xaml
    /// </summary>
    public partial class AddVideo_CameraName : WizardPage, IGotFocus
    {
        public AddVideo_CameraName()
        {
            InitializeComponent();
        }

        async Task IGotFocus.GotFocus()
        {
            while (!cameraNameTextBox.IsFocused)
            {
                cameraNameTextBox.Focus();
                await Task.Delay(TimeSpan.FromMilliseconds(10));
            }
        }
    }
}
