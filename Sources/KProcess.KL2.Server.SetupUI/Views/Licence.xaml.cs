using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace KProcess.KL2.Server.SetupUI.Views
{
    /// <summary>
    /// Logique d'interaction pour Licence.xaml
    /// </summary>
    public partial class Licence : InstallScreen
    {
        #region Visibilities of TabItems

        public Visibility WelcomeTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility LicenseTabItemVisibility { get; } = Visibility.Visible;
        public Visibility ConnectionTabItemVisibility { get; } = Visibility.Collapsed;
        public Visibility InstallPathTabItemVisibility { get; } = Visibility.Collapsed;
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
            e => MainViewModel.Instance.LicenseAccepted);

        public ICommand CancelButtonCommand { get; } = new DelegateCommand<object>(e => MainViewModel.Instance.AskCancelApplication(ActionResult.UserExit));

        public ICommand InstallButtonCommand { get; } = null;

        public ICommand ExitButtonCommand { get; } = null;

        #endregion

        public string NextButtonTooltip { get { return LocalizationExt.CurrentLanguage.GetLocalizedValue("LicenseTooltip"); } }

        public Licence()
        {
            InitializeComponent();
            LocalizationExt.StaticPropertyChanged += LocalizationExt_StaticPropertyChanged;
            LocalizationExt_StaticPropertyChanged(null, new System.ComponentModel.PropertyChangedEventArgs(nameof(LocalizationExt.CurrentLanguage)));

            MainViewModel.Instance.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.LicenseAccepted))
                    ((DelegateCommand<object>)NextButtonCommand).RaiseCanExecuteChanged();
            };
        }

        private void LocalizationExt_StaticPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LocalizationExt.CurrentLanguage))
            {
                var filePath = $"/KProcess.KL2.Server.SetupUI;component/Resources/License_{LocalizationExt.CurrentLanguage.ToCultureInfoString()}.xaml";
                FlowDocument content = XamlReader.Load(Application.GetResourceStream(new Uri(filePath, UriKind.Relative)).Stream) as FlowDocument;
                licenseReader.Document = content;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var filePath = $"/KProcess.KL2.Server.SetupUI;component/Resources/License_{LocalizationExt.CurrentLanguage.ToCultureInfoString()}.xaml";
            FlowDocument printedDoc = XamlReader.Load(Application.GetResourceStream(new Uri(filePath, UriKind.Relative)).Stream) as FlowDocument;
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                printedDoc.PageHeight = pd.PrintableAreaHeight;
                printedDoc.PageWidth = pd.PrintableAreaWidth;
                printedDoc.ColumnWidth = pd.PrintableAreaWidth;
                pd.PrintDocument(((IDocumentPaginatorSource)printedDoc).DocumentPaginator, LocalizationExt.CurrentLanguage.GetLocalizedValue("License"));
            }
        }
    }
}
