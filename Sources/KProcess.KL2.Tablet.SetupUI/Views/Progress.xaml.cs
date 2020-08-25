using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Tablet.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Progress.xaml
    /// </summary>
    public partial class Progress : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility SummaryTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility ProgressTabItemVisibility { get; } = Visibility.Visible;
        public Visibility FinishTabItemVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Visibilities of buttons

        public Visibility BackButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility NextButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility CancelButtonVisibility { get; } = Visibility.Visible;
        public Visibility InstallButtonVisibility { get; } = Visibility.Collapsed;
        public Visibility ExitButtonVisibility { get; } = Visibility.Collapsed;

        #endregion

        #region Commands of buttons

        public ICommand BackButtonCommand { get; } = null;

        public ICommand NextButtonCommand { get; } = null;

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip => null;

        public Progress()
        {
            InitializeComponent();

            MainViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.SelectedScreen) && MainViewModel.Instance.SelectedScreen.Content.GetType() == typeof(Progress))
                {
#if DEBUG
                    Task.Run(() =>
                    {
                        ManagedBA.Instance.ActionResult = ActionResult.Failure;
                        MainViewModel.Instance.NavigateFromOffset(1);
                    });
#else
                    ManagedBA.Instance.Engine.Apply(IntPtr.Zero);
#endif
                }
            };
        }
    }
}
