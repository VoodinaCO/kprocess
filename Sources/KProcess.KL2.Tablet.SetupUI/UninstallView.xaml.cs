using MahApps.Metro.Controls;
using Microsoft.Deployment.WindowsInstaller;

namespace KProcess.KL2.Tablet.SetupUI
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
