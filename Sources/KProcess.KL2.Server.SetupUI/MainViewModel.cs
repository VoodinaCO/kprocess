using KProcess.KL2.ConnectionSecurity;
using KProcess.KL2.Server.SetupUI.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Deployment.WindowsInstaller;
using Ookii.Dialogs.Wpf;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace KProcess.KL2.Server.SetupUI
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public const string Manufacturer = "K-process";
        public const string ApplicationName = "KL2WebAdmin";

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
                    ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = _instance.Default_KL2InstallPath;
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

        private string _FileProvider = "SFtp";
        public string FileProvider
        {
            get { return _FileProvider; }
            set
            {
                if (_FileProvider != value)
                {
                    _FileProvider = value;
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["FILE_PROVIDER"] = value;
#endif
                    RaisePropertyChanged();
                }
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

        //public string  Default_KL2InstallPath { get; } = $@"{Environment.GetFolderPath(Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles)}\{Manufacturer}\{ApplicationName}v4\";
        public string Default_KL2InstallPath { get; } = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\{Manufacturer}\{ApplicationName}v4\";
        private string _KL2InstallPath = null;
        public string KL2InstallPath
        {
            get { return _KL2InstallPath; }
            set
            {
                if (value.EndsWith($@"\{Manufacturer}\{ApplicationName}v4\"))
                    _KL2InstallPath = value;
                else
                    _KL2InstallPath = $@"{value}\{Manufacturer}\{ApplicationName}v4\";
#if !DEBUG
                ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"] = _KL2InstallPath;
#endif
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FormattedKL2InstallPath));
            }
        }

        public string FormattedKL2InstallPath => string.IsNullOrEmpty(KL2InstallPath) ? null : KL2InstallPath;

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
                    ManagedBA.Instance.Engine.StringVariables["DATASOURCE"] = DataSource;
                    ManagedBA.Instance.Engine.StringVariables["FILE_PROVIDER"] = FileProvider;
                    if (FileProvider == "SFtp")
                    {
                        ManagedBA.Instance.Engine.StringVariables["SERVER"] = Server;
                        ManagedBA.Instance.Engine.StringVariables["PORT"] = Port;
                        ManagedBA.Instance.Engine.StringVariables["USER"] = Username;
                        ManagedBA.Instance.Engine.StringVariables["PASSWORD"] = Password;
                        ManagedBA.Instance.Engine.StringVariables["PUBLISHED_DIR"] = SFtp_PublishedFiles;
                        ManagedBA.Instance.Engine.StringVariables["UPLOADED_DIR"] = SFtp_UploadedFiles;
                    }
                    else
                    {
                        ManagedBA.Instance.Engine.StringVariables["PUBLISHED_DIR"] = Local_PublishedFiles;
                        ManagedBA.Instance.Engine.StringVariables["UPLOADED_DIR"] = Local_UploadedFiles;
                    }
#endif
                }
            }
        }
        public void SetConnectionTestOK(bool value) => ConnectionTestOK = value;

        private string _DataSource;
        public string DataSource
        {
            get { return _DataSource; }
            set
            {
                _DataSource = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Server;
        public string Server
        {
            get { return _Server; }
            set
            {
                _Server = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Port;
        public string Port
        {
            get { return _Port; }
            set
            {
                _Port = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Username;
        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _SFtp_PublishedFiles;
        public string SFtp_PublishedFiles
        {
            get { return _SFtp_PublishedFiles; }
            set
            {
                _SFtp_PublishedFiles = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _SFtp_UploadedFiles;
        public string SFtp_UploadedFiles
        {
            get { return _SFtp_UploadedFiles; }
            set
            {
                _SFtp_UploadedFiles = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Local_PublishedFiles;
        public string Local_PublishedFiles
        {
            get { return _Local_PublishedFiles; }
            set
            {
                _Local_PublishedFiles = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _Local_UploadedFiles;
        public string Local_UploadedFiles
        {
            get { return _Local_UploadedFiles; }
            set
            {
                _Local_UploadedFiles = value;
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = false;
            }
        }

        private string _IntervalNotification;
        public string IntervalNotification
        {
            get { return _IntervalNotification; }
            set
            {
                if (int.TryParse(value, out int result))
                {
                    _IntervalNotification = result.ToString();
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["INTERVAL"] = _IntervalNotification;
#endif
                }
                RaisePropertyChanged();
                ((DelegateCommand)TestConnectionsCommand).RaiseCanExecuteChanged();
                ConnectionTestOK = !string.IsNullOrEmpty(_IntervalNotification);
            }
        }

        private string _WebUrl;
        public string WebUrl
        {
            get { return _WebUrl; }
            set
            {
                _WebUrl = value;
                WebApiUrl = $"{_WebUrl.TrimEnd('/')}:8081";
                WebFileServerUrl = $"{_WebUrl.TrimEnd('/')}:8082";
#if !DEBUG
                ManagedBA.Instance.Engine.StringVariables["API_LOCATION"] = WebApiUrl;
                ManagedBA.Instance.Engine.StringVariables["FILESERVER_LOCATION"] = WebFileServerUrl;
#endif
                RaisePropertyChanged();
                if (SelectedScreen.Content is Parameters currentScreen)
                {
                    ((DelegateCommand<object>)currentScreen.NextButtonCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _WebApiUrl;
        public string WebApiUrl
        {
            get => _WebApiUrl;
            set
            {
                if (_WebApiUrl != value)
                {
                    _WebApiUrl = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _WebFileServerUrl;
        public string WebFileServerUrl
        {
            get => _WebFileServerUrl;
            set
            {
                if (_WebFileServerUrl != value)
                {
                    _WebFileServerUrl = value;
                    RaisePropertyChanged();
                }
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

        public ICommand SaveLogCommand { get; } = new DelegateCommand<object>(async e =>
        {
            await Instance.DialogService.ShowMetroDialogAsync(Instance, new SendReportDialog());
        });

        public ICommand BrowsePublishedFilesDirectory { get; } = new DelegateCommand(() =>
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true)
                Instance.Local_PublishedFiles = dialog.SelectedPath;
        });

        public ICommand BrowseUploadedFilesDirectory { get; } = new DelegateCommand(() =>
        {
            var dialog = new VistaFolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == true)
                Instance.Local_UploadedFiles = dialog.SelectedPath;
        });

        public ICommand TestConnectionsCommand { get; } = new DelegateCommand(async () =>
        {
            Instance.ConnectionTestOK = false;
            List<string> exceptionsMessages = new List<string>();

            var progressController = await Instance.DialogService.ShowProgressAsync(Instance,
                LocalizationExt.CurrentLanguage.GetLocalizedValue("ConnectionsTestTitle"),
                LocalizationExt.CurrentLanguage.GetLocalizedValue("DatabaseConnectionTestMessage"));
            progressController.SetIndeterminate();

            //Test database connection
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = Instance.DataSource,
                UserID = Const.DataBaseUser,
                Password = ConnectionStringsSecurity.DecryptPassword(Const.DataBaseUserCryptedPassword)
            };
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                try
                {
                    await sqlConn.OpenAsync();
                    try
                    {
                        Version dbVersion = null;

                        SqlCommand sqlCommand = new SqlCommand($"USE [{Const.DataBaseName_v3}]", sqlConn);
                        await sqlCommand.ExecuteNonQueryAsync();
                        sqlCommand = new SqlCommand("SELECT [value] FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", sqlConn);
                        using (SqlDataReader result = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await result.ReadAsync())
                                dbVersion = Version.Parse(result.GetString(0));
                        }
                        if (dbVersion == null)
                            exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("CantGetDatabaseVersionMessage"));
                        else if (dbVersion.Major != 4)
                            exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("InvalidDatabaseVersionMessage"));
                    }
                    catch
                    {
                        exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("CantGetDatabaseVersionMessage"));
                    }
                }
                catch
                {
                    exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("CantConnectToDatabaseMessage"));
                }
                finally
                {
                    sqlConn.Close();
                }
            }

            if (Instance.FileProvider == "SFtp")
            {
                //Test File provider connection
                progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("FileConnectionTestMessage"));
                await Task.Run(() =>
                {
                    try
                    {
                        var _sftpClient = new SftpClient(Instance.Server, int.Parse(Instance.Port), Instance.Username, Instance.Password);
                        _sftpClient.Connect();
                        if (!_sftpClient.Exists(Path.Combine(_sftpClient.WorkingDirectory, Instance.SFtp_PublishedFiles)))
                            exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("SFTP_PublishedFilesDoesntExistMessage"));
                        if (!_sftpClient.Exists(Path.Combine(_sftpClient.WorkingDirectory, Instance.SFtp_UploadedFiles)))
                            exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("SFTP_UploadedFilesDoesntExistMessage"));
                    }
                    catch (Exception)
                    {
                        exceptionsMessages.Add(LocalizationExt.CurrentLanguage.GetLocalizedValue("CantConnectToSFTPMessage"));
                    }
                });
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
        }, () =>
        {
            if (string.IsNullOrEmpty(Instance.DataSource))
                return false;
            if (Instance.FileProvider == "SFtp")
                return !string.IsNullOrEmpty(Instance.Server)
                    && !string.IsNullOrEmpty(Instance.Port)
                    && !string.IsNullOrEmpty(Instance.Username)
                    && !string.IsNullOrEmpty(Instance.Password)
                    && !string.IsNullOrEmpty(Instance.SFtp_PublishedFiles)
                    && !string.IsNullOrEmpty(Instance.SFtp_UploadedFiles);
            return !string.IsNullOrEmpty(Instance.Local_PublishedFiles) && !string.IsNullOrEmpty(Instance.Local_UploadedFiles);
        });
    }
}
