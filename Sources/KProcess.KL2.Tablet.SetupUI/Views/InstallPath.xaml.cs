using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;
using System.Windows.Input;
using WPF.Dialogs;

namespace KProcess.KL2.Tablet.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour InstallPath.xaml
    /// </summary>
    public partial class InstallPath : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Visible;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Visible;
        public Visibility SummaryTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ProgressTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility FinishTabItemVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Visibilities of buttons

        public Visibility BackButtonVisibility { get; } = Visibility.Visible;
        public Visibility NextButtonVisibility { get; } = Visibility.Visible;
        public Visibility CancelButtonVisibility { get; } = Visibility.Visible;
        public Visibility InstallButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility ExitButtonVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Commands of buttons

        public ICommand BackButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.NavigateFromOffset(-1));

        public ICommand NextButtonCommand { get; } = new DelegateCommand<object>(e =>
        {
            if (string.IsNullOrEmpty(MainViewModel.Instance.KL2InstallPath))
                MainViewModel.Instance.KL2InstallPath = MainViewModel.Instance.Default_KL2InstallPath;
            MainViewModel.Instance.NavigateFromOffset(1);
        });

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip => null;

        public ICommand KL2InstallPathButtonCommand { get; } = new DelegateCommand<object>(e =>
        {
            var folderDialog = new OpenFolderDialog
            {
                Description = LocalizationExt.CurrentLanguage.GetLocalizedValue("SelectFolder"),
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = string.IsNullOrEmpty(MainViewModel.Instance.KL2InstallPath) ? MainViewModel.Instance.Default_KL2InstallPath : MainViewModel.Instance.FormattedKL2InstallPath,
                ShowNewFolderButton = true
            };
            if (folderDialog.ShowDialog() == true)
                MainViewModel.Instance.KL2InstallPath = folderDialog.SelectedPath;
        });

        public ICommand KL2SyncPathButtonCommand { get; } = new DelegateCommand<object>(e =>
        {
            var folderDialog = new OpenFolderDialog
            {
                Description = LocalizationExt.CurrentLanguage.GetLocalizedValue("SelectFolder"),
                RootFolder = Environment.SpecialFolder.MyComputer,
                SelectedPath = string.IsNullOrEmpty(MainViewModel.Instance.KL2SyncPath) ? MainViewModel.Instance.Default_KL2SyncPath : MainViewModel.Instance.KL2SyncPath,
                ShowNewFolderButton = true
            };
            if (folderDialog.ShowDialog() == true)
                MainViewModel.Instance.KL2SyncPath = folderDialog.SelectedPath;
        });

        public InstallPath()
        {
            InitializeComponent();
        }
    }
}
