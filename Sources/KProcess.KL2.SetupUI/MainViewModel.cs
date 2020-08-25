using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.KL2.SetupUI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string Manufacturer = "K-process";
        public const string ApplicationName = "KL2VideoAnalyst";

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public IDialogCoordinator DialogService { get; private set; }

        private static MainViewModel _instance = null;
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainViewModel { DialogService = DialogCoordinator.Instance };
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["MSIINSTALLPERUSER"] = "0";
                    ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = _instance.Default_AllUsers_KL2InstallPath;
                    ManagedBA.Instance.Engine.StringVariables["SYNCPATH"] = _instance.Default_AllUsers_KL2SyncPath;
#endif
                }
                return _instance;
            }
        }

        public static MetroWindow MainWindow { get; set; }
        public static bool ForceClose { get; set; } = false;

        #region Variables for installation

        private bool _LicenseAccepted = false;
        public bool LicenseAccepted
        {
            get { return _LicenseAccepted; }
            set
            {
                if (_LicenseAccepted != value)
                {
                    _LicenseAccepted = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _SendReportAccepted = true;
        public bool SendReportAccepted
        {
            get { return _SendReportAccepted; }
            set
            {
                if (_SendReportAccepted != value)
                {
                    _SendReportAccepted = value;
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["SENDREPORT"] = value ? "yes" : "no";
#endif
                    RaisePropertyChanged();
                }
            }
        }

        private bool _InstallForJustMe = true;
        public bool InstallForJustMe
        {
            get { return _InstallForJustMe; }
            set
            {
                _InstallForJustMe = value;
                if (_InstallForJustMe)
                {
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["MSIINSTALLPERUSER"] = "1";
#endif
                    KL2InstallPath = Default_CurrentUser_KL2InstallPath;
                    KL2SyncPath = Default_CurrentUser_KL2SyncPath;
                }
                RaisePropertyChanged();
            }
        }

        private bool _InstallForAllUsers = false;
        public bool InstallForAllUsers
        {
            get { return _InstallForAllUsers; }
            set
            {
                _InstallForAllUsers = value;
                if (_InstallForAllUsers)
                {
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["MSIINSTALLPERUSER"] = "0";
#endif
                    KL2InstallPath = Default_AllUsers_KL2InstallPath;
                    KL2SyncPath = Default_AllUsers_KL2SyncPath;
                }
                RaisePropertyChanged();
            }
        }

        private bool _CreateShorcutOnDesktop = true;
        public bool CreateShorcutOnDesktop
        {
            get { return _CreateShorcutOnDesktop; }
            set
            {
                if (_CreateShorcutOnDesktop != value)
                {
                    _CreateShorcutOnDesktop = value;
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["INSTALLDESKTOPSHORTCUT"] = value ? "yes" : "no";
#endif
                    RaisePropertyChanged();
                }
            }
        }

        private bool _CreateShortcutOnStartMenu = true;
        public bool CreateShortcutOnStartMenu
        {
            get { return _CreateShortcutOnStartMenu; }
            set
            {
                if (_CreateShortcutOnStartMenu != value)
                {
                    _CreateShortcutOnStartMenu = value;
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["INSTALLSTARTMENUSHORTCUT"] = value ? "yes" : "no";
#endif
                    RaisePropertyChanged();
                }
            }
        }

        private bool _LaunchKL2OnExit = true;
        public bool LaunchKL2OnExit
        {
            get { return _LaunchKL2OnExit; }
            set
            {
                if (_LaunchKL2OnExit != value)
                {
                    _LaunchKL2OnExit = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Default_CurrentUser_KL2InstallPath { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{Manufacturer}\{ApplicationName}";
        public string Default_AllUsers_KL2InstallPath { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)}\{Manufacturer}\{ApplicationName}";
        private string _KL2InstallPath = null;
        public string KL2InstallPath
        {
            get { return _KL2InstallPath; }
            set
            {
                if (value.EndsWith($@"\{Manufacturer}\{ApplicationName}", StringComparison.InvariantCultureIgnoreCase))
                    _KL2InstallPath = value;
                else if (value.EndsWith($@"\{Manufacturer}", StringComparison.InvariantCultureIgnoreCase))
                    _KL2InstallPath = Path.Combine(value, ApplicationName);
                else
                    _KL2InstallPath = Path.Combine(value, Manufacturer, ApplicationName);
#if !DEBUG
                ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = _KL2InstallPath;
#endif
                RaisePropertyChanged();
                KL2SyncPath = Path.Combine(_KL2InstallPath, "SyncFiles");
            }
        }

        public string Default_CurrentUser_KL2SyncPath { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\{Manufacturer}\{ApplicationName}\SyncFiles";
        public string Default_AllUsers_KL2SyncPath { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments)}\{Manufacturer}\{ApplicationName}\SyncFiles";
        private string _KL2SyncPath = null;
        public string KL2SyncPath
        {
            get { return string.IsNullOrEmpty(_KL2SyncPath) ? Default_CurrentUser_KL2SyncPath : _KL2SyncPath; }
            set
            {
                _KL2SyncPath = value;
#if !DEBUG
                ManagedBA.Instance.Engine.StringVariables["SYNCPATH"] = _KL2SyncPath;
#endif
                RaisePropertyChanged();
            }
        }

        #endregion

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;

                    RaisePropertyChanged();
                }
            }
        }

        private bool _ConnectionTestOK = false;
        public bool ConnectionTestOK
        {
            get { return _ConnectionTestOK; }
            private set
            {
                _ConnectionTestOK = value;
                RaisePropertyChanged();
                if (value)
                {
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["API_LOCATION"] = APILocation;
                    ManagedBA.Instance.Engine.StringVariables["FILESERVER_LOCATION"] = FileServerLocation;
#endif
                }
            }
        }

        private string _APILocation;
        public string APILocation
        {
            get { return _APILocation; }
            set
            {
                _APILocation = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _FileServerLocation;
        public string FileServerLocation
        {
            get { return _FileServerLocation; }
            set
            {
                _FileServerLocation = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private int _SelectedIndexScreen = 0;
        public int SelectedIndexScreen
        {
            get { return _SelectedIndexScreen; }
            set
            {
                if (_SelectedIndexScreen != value)
                {
                    _SelectedIndexScreen = value;
                    RaisePropertyChanged();
                }
            }
        }

        private TabItem _SelectedScreen;
        public TabItem SelectedScreen
        {
            get { return _SelectedScreen; }
            set
            {
                if (_SelectedScreen != value)
                {
                    _SelectedScreen = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void NavigateFromOffset(int offset) => SelectedIndexScreen += offset;

        public async void AskCancelApplication(ActionResult arg)
        {
            var result = await DialogService.ShowMessageAsync(this,
                string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("Title"), LocalizationExt.ProductName),
                string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("CancelAskMessage"), LocalizationExt.ProductName, LocalizationExt.ProductVersion),
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = LocalizationExt.CurrentLanguage.GetLocalizedValue("Yes"),
                    NegativeButtonText = LocalizationExt.CurrentLanguage.GetLocalizedValue("No"),
                    DefaultButtonFocus = MessageDialogResult.Negative
                });
            if (result == MessageDialogResult.Affirmative)
            {
                ForceClose = true;
                ExitApplication(arg);
            }
        }

        public void ExitApplication(ActionResult arg)
        {
#if !DEBUG
            ManagedBA.BootstrapperDispatcher.Invoke(() => ManagedBA.Instance.Engine.Quit((int)arg));
#endif
            MainWindow.Close();
        }

        public ICommand SendLogCommand { get; } = new DelegateCommand<object>(async e =>
        {
            await Instance.DialogService.ShowMetroDialogAsync(Instance, new SendReportDialog());
        });

        public ICommand TestConnectionsCommand { get; } = new DelegateCommand(async () =>
        {
            Instance.ConnectionTestOK = false;
            List<string> exceptionsMessages = new List<string>();

            var progressController = await Instance.DialogService.ShowProgressAsync(Instance,
                LocalizationExt.CurrentLanguage.GetLocalizedValue("ConnectionsTestTitle"),
                LocalizationExt.CurrentLanguage.GetLocalizedValue("APIConnectionTestMessage"));
            progressController.SetIndeterminate();

            //Test API connection
            try
            {
                WebRequest webRequest = WebRequest.Create($"{Instance.APILocation}/Ping");
                WebResponse webResp = await webRequest.GetResponseAsync();
                using (Stream stream = webResp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string responseString = reader.ReadToEnd().Replace("\"", "");
                    if (!responseString.EndsWith("API is running."))
                        exceptionsMessages.Add(string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("NotCorrectAPILocationMessage"), Instance.APILocation));
                }
            }
            catch (Exception)
            {
                exceptionsMessages.Add(string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("UnableConnectToMessage"), Instance.APILocation));
            }

            //Test File server connection
            progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("FileConnectionTestMessage"));
            try
            {
                WebRequest webRequest = WebRequest.Create($"{Instance.FileServerLocation}/Ping");
                WebResponse webResp = await webRequest.GetResponseAsync();
                using (Stream stream = webResp.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string responseString = reader.ReadToEnd().Replace("\"", "");
                    if (!responseString.EndsWith("File Server is running."))
                        exceptionsMessages.Add(string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("NotCorrectFileLocationMessage"), Instance.FileServerLocation));
                }
            }
            catch (Exception)
            {
                exceptionsMessages.Add(string.Format(LocalizationExt.CurrentLanguage.GetLocalizedValue("UnableConnectToMessage"), Instance.FileServerLocation));
            }

            await progressController.CloseAsync();

            if (exceptionsMessages.Any())
            {
                await Instance.DialogService.ShowMessageAsync(Instance,
                    LocalizationExt.CurrentLanguage.GetLocalizedValue("ConnectionsErrorTitle"),
                    string.Join("\n", exceptionsMessages),
                    MessageDialogStyle.Affirmative,
                    new MetroDialogSettings
                    {
                        AffirmativeButtonText = LocalizationExt.CurrentLanguage.GetLocalizedValue("Ok")
                    });
                Instance.ConnectionTestOK = false;
            }
            else
                Instance.ConnectionTestOK = true;
        }, () => !string.IsNullOrEmpty(Instance.APILocation) && !string.IsNullOrEmpty(Instance.FileServerLocation));
    }
}
