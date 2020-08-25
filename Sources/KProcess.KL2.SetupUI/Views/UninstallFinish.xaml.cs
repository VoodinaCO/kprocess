using Microsoft.Deployment.WindowsInstaller;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour UninstallFinish.xaml
    /// </summary>
    public partial class UninstallFinish : InstallScreen
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

        public Visibility UninstallButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility CancelButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility ExitButtonVisibility { get; } = Visibility.Visible;

        #endregion

        #region Commands of buttons

        public ICommand UninstallButtonCommand { get; } = null;

        public ICommand CancelButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = new DelegateCommand<object>(e => UninstallViewModel.Instance.ExitApplication(ActionResult.Success));

        #endregion

        public UninstallFinish()
        {
            InitializeComponent();
        }
    }
}
