using FreshDeskLib;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace KProcess.KL2.SetupUI
{
    public class UninstallViewModel : INotifyPropertyChanged
    {
        #region FreshDesk

        private const string DevEmail = "pierre-yves.cassard@k-process.com";
        private const long email_config_id = 16000018357;
        private const long group_id = 16000053569;
        private const string API_KEY = "Q2SIdFBsNAxYJ2Xhew0D";
        private const string API_FRESHDESK_OUTBOUND_EMAIL = "https://kprocess.freshdesk.com/api/v2/tickets/outbound_email";
        private const string API_FRESHDESK_TICKETS = "https://kprocess.freshdesk.com/api/v2/tickets";

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public IDialogCoordinator DialogService { get; private set; }

        private static UninstallViewModel _instance = null;
        public static UninstallViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UninstallViewModel { DialogService = DialogCoordinator.Instance };
                    FreshdeskClient.Instance.Initialize(DevEmail, API_KEY, API_FRESHDESK_TICKETS, API_FRESHDESK_OUTBOUND_EMAIL);
                }
                return _instance;
            }
        }

        public static MetroWindow mainWindow { get; set; }
        public static bool ForceClose { get; set; } = false;

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

        public ICommand SendLogCommand { get; } = new DelegateCommand<object>(async e =>
        {
            await Instance.DialogService.ShowMetroDialogAsync(Instance, new SendReportDialog());
        });

        public void NavigateFromOffset(int offset) => SelectedIndexScreen += offset;

        public void ExitApplication(ActionResult arg)
        {
#if !DEBUG
            ManagedBA.BootstrapperDispatcher.Invoke(() => ManagedBA.Instance.Engine.Quit((int)arg));
#endif
            mainWindow.Close();
        }

        public bool BackupConfigFile()
        {
            try
            {
                string configPath = $"{ManagedBA.Instance.Engine.StringVariables["INSTALLDIR"]}KL².exe.config";
                var backupConfig = XDocument.Load(configPath);
                var connectionString = backupConfig.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities").Attribute("connectionString").Value;
                var dataSource = connectionString.Substring(connectionString.IndexOf("Data Source=") + 12).Split(';')[0];
                ManagedBA.Instance.Engine.StringVariables["INSTANCE"] = dataSource;
                ManagedBA.Instance.Engine.StringVariables["SENDREPORT"] = backupConfig.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "SendReport").Element("value").Value == "True" ? "yes" : "no";
                ManagedBA.Instance.Engine.StringVariables["MUTE"] = backupConfig.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "Mute").Element("value").Value == "True" ? "yes" : "no";
                ManagedBA.Instance.Log("DEBUG : Backup config file");
            }
            catch (Exception e)
            {
                ManagedBA.Instance.LogExceptionWithInner(e);
                return false;
            }
            return true;
        }
    }
}
