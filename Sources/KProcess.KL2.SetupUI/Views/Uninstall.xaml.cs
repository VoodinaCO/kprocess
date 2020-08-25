using Microsoft.Deployment.WindowsInstaller;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Uninstall.xaml
    /// </summary>
    public partial class Uninstall : InstallScreen
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

        public Visibility UninstallButtonVisibility { get; } = Visibility.Visible;
        public Visibility CancelButtonVisibility { get; } = Visibility.Visible;
        public Visibility ExitButtonVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Commands of buttons

        public ICommand UninstallButtonCommand { get; } = new DelegateCommand<object>(e => UninstallViewModel.Instance.NavigateFromOffset(1));

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => UninstallViewModel.Instance.ExitApplication(ActionResult.UserExit));

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public Uninstall()
        {
            InitializeComponent();
        }
    }
}
