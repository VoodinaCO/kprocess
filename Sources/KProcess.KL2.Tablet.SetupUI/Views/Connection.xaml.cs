using Microsoft.Deployment.WindowsInstaller;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Tablet.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Connection.xaml
    /// </summary>
    public partial class Connection : InstallScreen
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

        public ICommand NextButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.NavigateFromOffset(1),
            e => MainViewModel.Instance.ConnectionTestOK);

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip { get { return LocalizationExt.CurrentLanguage.GetLocalizedValue("TestConnectionTooltip"); } }

        public Connection()
        {
            InitializeComponent();

            MainViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.ConnectionTestOK))
                    ((DelegateCommand<object>)NextButtonCommand).RaiseCanExecuteChanged();
                else if (e.PropertyName == nameof(MainViewModel.Instance.SelectedScreen) && MainViewModel.Instance.SelectedScreen.Content.GetType() == typeof(Connection))
                {
                }
            };
            LocalizationExt.StaticPropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(LocalizationExt.CurrentLanguage))
                {
                    RaisePropertyChanged(nameof(NextButtonTooltip));
                }
            };
        }
    }
}
