using Microsoft.Deployment.WindowsInstaller;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Server.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour InstallPath.xaml
    /// </summary>
    public partial class Parameters : InstallScreen
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
        }, e => !string.IsNullOrEmpty(MainViewModel.Instance.WebUrl) && MainViewModel.Instance.WebUrl.StartsWith("http"));

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip => null;

        public Parameters()
        {
            InitializeComponent();
            Loaded += (o, e) => MainViewModel.Instance.IntervalNotification = "3";
        }
    }
}
