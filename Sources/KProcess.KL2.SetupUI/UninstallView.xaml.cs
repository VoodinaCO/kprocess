using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.Linq;

namespace KProcess.KL2.SetupUI
{
    /// <summary>
    /// Logique d'interaction pour UninstallView.xaml
    /// </summary>
    public partial class UninstallView : MetroWindow
    {
        public UninstallView()
        {
            InitializeComponent();
            Loaded += (o, e) => UninstallViewModel.mainWindow = this;
            Closing += MainView_Closing;
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (UninstallViewModel.Instance.SelectedScreen.Content.GetType() == typeof(Views.UninstallProgress))
                e.Cancel = true;
            else
                UninstallViewModel.Instance.ExitApplication(ActionResult.UserExit);
        }
    }
}
