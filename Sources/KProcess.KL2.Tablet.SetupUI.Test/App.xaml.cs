using System.Windows;

namespace KProcess.KL2.Tablet.SetupUI.Test
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LocalizationExt.ProductVersion = "4.0.0.0";
            ManagedBA.Instance = new ManagedBA();

            MainView view = new MainView();

            //ManagedBA.Instance.launchAction = Microsoft.Tools.WindowsInstallerXml.Bootstrapper.LaunchAction.Uninstall;
            //UninstallView view = new UninstallView();

            view.Show();
        }
    }
}
