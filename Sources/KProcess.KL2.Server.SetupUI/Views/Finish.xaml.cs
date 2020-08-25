using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Server.SetupUI.Views
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
                if (MainViewModel.Instance.LaunchKL2OnExit && ManagedBA.Instance.ActionResult == ActionResult.Success)
                {
                    Process.Start(GetDefaultBrowserPath(), $"{MainViewModel.Instance.WebUrl.TrimEnd('/')}:8080");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MainViewModel.Instance.ExitApplication(ManagedBA.Instance.ActionResult);
        });

        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            RegistryKey userChoiceKey = null;
            string browserPath = "";

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey = Registry.CurrentUser.OpenSubKey(urlAssociation, false);
                    }
                    var path = CleanifyBrowserPath(browserKey.GetValue(null) as string);
                    browserKey.Close();
                    return path;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);
                    browserPath = CleanifyBrowserPath(kp.GetValue(null) as string);
                    kp.Close();
                    return browserPath;
                }
            }
            catch
            {
                return "";
            }
        }

        private static string CleanifyBrowserPath(string p)
        {
            string[] url = p.Split('"');
            string clean = url[1];
            return clean;
        }

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

        public static void StartHideProcess(string proc, string args)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p = Process.Start(proc, args, null, null, null);
            p.WaitForExit();
        }
    }
}
