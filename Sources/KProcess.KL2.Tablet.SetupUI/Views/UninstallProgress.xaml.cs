using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Tablet.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour UninstallProgress.xaml
    /// </summary>
    public partial class UninstallProgress : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility SummaryTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ProgressTabItemVisibility { get; } = Visibility.Visible;
        public Visibility FinishTabItemVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Visibilities of buttons

        public Visibility UninstallButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility CancelButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility ExitButtonVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Commands of buttons

        public ICommand UninstallButtonCommand { get; } = null;

        public ICommand CancelButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public UninstallProgress()
        {
            InitializeComponent();

            UninstallViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(UninstallViewModel.Instance.SelectedScreen) && UninstallViewModel.Instance.SelectedScreen.Content.GetType() == typeof(UninstallProgress))
                {
#if DEBUG
                    Task.Run(async () =>
                    {
                        ManagedBA.Instance.CurrentlyProcessingPackageName = "Package 1";
                        while (ManagedBA.Instance.GlobalProgress < 100)
                        {
                            if (ManagedBA.Instance.PackageProgress == 100)
                            {
                                ManagedBA.Instance.CurrentlyProcessingPackageName = "Package 2";
                                ManagedBA.Instance.PackageProgress = 0;
                            }
                            await Task.Delay(500);
                            ManagedBA.Instance.PackageProgress += 10;
                            ManagedBA.Instance.GlobalProgress += 5;
                        }
                        ManagedBA.Instance.ActionResult = ActionResult.Success;
                        UninstallViewModel.Instance.NavigateFromOffset(1);
                    });
#else
                    if (ManagedBA.Instance.launchAction == LaunchAction.UpdateReplace)
                    {
                        ManagedBA.Instance.Engine.StringVariables["CREATEINSTANCE"] = "no";
                        ManagedBA.Instance.Engine.StringVariables["CREATEDATABASE"] = "no";
                        ManagedBA.Instance.Engine.StringVariables["MIGRATEDATA"] = "no";
                        try
                        {
                            RegistryKey valKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\K-process\\KL² Tablet\\Install\\v4");
                            try
                            {
                                ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = (string)valKey.GetValue("InstallLocation");
                            }
                            catch
                            {
                                ManagedBA.Instance.Log("Erreur lors de la récupération du répertoire d'installation");
                                ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = MainViewModel.Instance.Default_KL2InstallPath;
                            }
                            try
                            {
                                string regValue = (string)valKey.GetValue("DesktopShortcut");
                                ManagedBA.Instance.Engine.StringVariables["INSTALLDESKTOPSHORTCUT"] = string.IsNullOrEmpty(regValue) ? "yes" : regValue;
                            }
                            catch
                            {
                                ManagedBA.Instance.Log("Erreur lors de la récupération du raccourci du bureau");
                                ManagedBA.Instance.Engine.StringVariables["INSTALLDESKTOPSHORTCUT"] = "yes";
                            }
                            try
                            {
                                string regValue = (string)valKey.GetValue("StartMenuShortcut");
                                ManagedBA.Instance.Engine.StringVariables["INSTALLSTARTMENUSHORTCUT"] = string.IsNullOrEmpty(regValue) ? "yes" : regValue;
                            }
                            catch
                            {
                                ManagedBA.Instance.Log("Erreur lors de la récupération du raccourci du menu démarrer");
                                ManagedBA.Instance.Engine.StringVariables["INSTALLSTARTMENUSHORTCUT"] = "yes";
                            }
                        }
                        catch (Exception ex)
                        {
                            ManagedBA.Instance.Log(ex.Message);
                        }
                        UninstallViewModel.Instance.BackupConfigFile();
                        ManagedBA.Instance.Engine.Plan(LaunchAction.Install);
                    }
                    else
                    {
                        ManagedBA.Instance.Engine.Plan(ManagedBA.Instance.launchAction);
                    }
                    ManagedBA.Instance.Engine.Apply(IntPtr.Zero);
#endif
                }
            };
        }
    }
}
