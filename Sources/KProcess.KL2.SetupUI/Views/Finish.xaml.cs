using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Finish.xaml
    /// </summary>
    public partial class Finish : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility SummaryTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ProgressTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility FinishTabItemVisibility { get; } = Visibility.Visible;

        #endregion

        #region Visibilities of buttons

        public Visibility BackButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility NextButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility CancelButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility ExitButtonVisibility { get; } = Visibility.Visible;

        #endregion

        #region Commands of buttons

        public ICommand BackButtonCommand { get; } = null;

        public ICommand NextButtonCommand { get; } = null;

        public ICommand CancelButtonCommand { get; } = null;

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = new DelegateCommand<object>(e =>
        {
            try
            {
                string exePath = MainViewModel.Instance.KL2InstallPath;
                if (MainViewModel.Instance.LaunchKL2OnExit && ManagedBA.Instance.ActionResult == ActionResult.Success)
                {
                    Process startProcess = new Process();
                    startProcess.StartInfo.FileName = Path.Combine(exePath, "KL².exe");
                    startProcess.StartInfo.WorkingDirectory = Directory.GetParent(startProcess.StartInfo.FileName).FullName;
                    startProcess.StartInfo.UseShellExecute = true;

                    startProcess.Start();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MainViewModel.Instance.ExitApplication(ManagedBA.Instance.ActionResult);
        });

        #endregion

        public Finish()
        {
            InitializeComponent();

            MainViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.SelectedScreen) && MainViewModel.Instance.SelectedScreen.Content.GetType() == typeof(Finish))
                {
                    MainViewModel.ForceClose = true;

#if !DEBUG
                    // Test if zip created correctly
                    //File.WriteAllBytes(@"c:\Temp\Logs.zip", SendReportDialog.CreateLogZip(ManagedBA.Instance.Engine.StringVariables["WixBundleLog"]));
#endif
                }
            };
        }
    }
}
