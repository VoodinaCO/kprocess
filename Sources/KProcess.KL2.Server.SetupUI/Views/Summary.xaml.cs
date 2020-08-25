using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Server.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Summary.xaml
    /// </summary>
    public partial class Summary : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility SummaryTabItemVisibility { get; } = Visibility.Visible;
        public Visibility ProgressTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility FinishTabItemVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Visibilities of buttons

        public Visibility BackButtonVisibility { get; } = Visibility.Visible;
        public Visibility NextButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility CancelButtonVisibility { get; } = Visibility.Visible;
        public Visibility InstallButtonVisibility { get; } = Visibility.Visible;
        public Visibility ExitButtonVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Commands of buttons

        public ICommand BackButtonCommand { get; } = new DelegateCommand<object>(e =>
            MainViewModel.Instance.NavigateFromOffset(-1));

        public ICommand NextButtonCommand { get; } = null;

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.NavigateFromOffset(1));

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip => null;

        public Summary()
        {
            InitializeComponent();

            MainViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.SelectedScreen) && MainViewModel.Instance.SelectedScreen.Content.GetType() == typeof(Summary))
                {
                    MainViewModel.Instance.IsLoading = true;

                    MainViewModel.Instance.KL2InstallPath = MainViewModel.Instance.Default_KL2InstallPath;

                    MainViewModel.Instance.IsLoading = false;
#if !DEBUG
                    ManagedBA.Instance.Engine.StringVariables["LANGUAGE"] = LocalizationExt.CurrentLanguage.ToCultureInfoString();
                    ManagedBA.Instance.Engine.Plan(LaunchAction.Install);
#endif
                }
            };
        }
    }
}
