using MahApps.Metro.Controls;
using Microsoft.Deployment.WindowsInstaller;

namespace KProcess.KL2.Server.SetupUI
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += (o, e) =>
            {
                MainViewModel.MainWindow = this;
            };
            Closing += MainView_Closing;
        }

        private void MainView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainViewModel.ForceClose)
            {
                e.Cancel = true;
                MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit);
            }
        }
    }
}
