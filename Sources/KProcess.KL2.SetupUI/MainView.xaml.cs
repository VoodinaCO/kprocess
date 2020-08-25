using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using System.Linq;

namespace KProcess.KL2.SetupUI
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            Loaded += async (o, e) =>
            {
                MainViewModel.MainWindow = this;

                while (Process.GetProcesses().Any(p => p.ProcessName == "KL²" && p.MainModule.FileVersionInfo.FileMajorPart == 3))
                {
                    var result = await MainViewModel.Instance.DialogService.ShowMessageAsync(MainViewModel.Instance,
                        string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("Title"), LocalizationExt.ProductName),
                        string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("IsRunningMessage"), LocalizationExt.ProductName, LocalizationExt.ProductVersion),
                        MessageDialogStyle.AffirmativeAndNegative,
                        new MetroDialogSettings
                        {
                            AffirmativeButtonText = LocalizationExt.CurrentLanguage.GetLocalizedValue("Retry"),
                            NegativeButtonText = LocalizationExt.CurrentLanguage.GetLocalizedValue("ExitButton"),
                            DefaultButtonFocus = MessageDialogResult.Affirmative
                        });
                    if (result == MessageDialogResult.Negative)
                    {
                        MainViewModel.ForceClose = true;
                        MainViewModel.Instance.ExitApplication(ActionResult.UserExit);
                    }
                    else
                        MainViewModel.ForceClose = false;
                }
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
