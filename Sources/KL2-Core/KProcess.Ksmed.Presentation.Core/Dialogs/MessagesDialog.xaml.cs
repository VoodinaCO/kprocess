#define ACTIVE_FRESHDESK
using KProcess.Ksmed.Security.Activation.Providers;
using KProcess.Presentation.Windows;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MessageDialog"/>.
        /// </summary>
        public MessageDialog()
        {
            InitializeComponent();

            try
            {
                UserInformationProvider userinfo = new UserInformationProvider();
                UsernameTB.Text = userinfo.Username;
                CompanyTB.Text = userinfo.Company;
                EmailTB.Text = userinfo.Email;
            }
            catch (Exception)
            {

            }

        }


        /// <summary>
        /// Obtient ou définit l'exception de départ.
        /// </summary>
        public Exception InitialException { get; set; }

        /// <summary>
        /// Obtient ou définit l'entête.
        /// </summary>
        public object Caption
        {
            get { return ChildWindow.Title; }
            set { ChildWindow.Title = value; }
        }

        /// <summary>
        /// Obtient ou définit le message.
        /// </summary>
        public string Message
        {
            get { return MessageTextTB.Text; }
            set { MessageTextTB.Text = value; }
        }

        /// <summary>
        /// Obtient ou définit la visibilité de la partie "Rapport".
        /// </summary>
        public Visibility ReportVisibility
        {
            get { return report.Visibility; }
            set { report.Visibility = value; }
        }

        MessageDialogButton _buttons;
        /// <summary>
        /// Obtient ou définit les boutons à afficher.
        /// </summary>
        public MessageDialogButton Buttons
        {
            get { return _buttons; }
            set
            {
                _buttons = value;
                switch (_buttons)
                {
                    case MessageDialogButton.CloseApp:
                        OkButton.Visibility = Visibility.Collapsed;
                        YesButton.Visibility = Visibility.Collapsed;
                        NoButton.Visibility = Visibility.Collapsed;
                        CancelButton.Visibility = Visibility.Collapsed;
                        CloseButton.Visibility = Visibility.Visible;
                        break;
                    case MessageDialogButton.OK:
                        OkButton.Visibility = Visibility.Visible;
                        YesButton.Visibility = Visibility.Collapsed;
                        NoButton.Visibility = Visibility.Collapsed;
                        CancelButton.Visibility = Visibility.Collapsed;
                        CloseButton.Visibility = Visibility.Collapsed;
                        break;
                    case MessageDialogButton.OKCancel:
                        OkButton.Visibility = Visibility.Visible;
                        YesButton.Visibility = Visibility.Collapsed;
                        NoButton.Visibility = Visibility.Collapsed;
                        CancelButton.Visibility = Visibility.Visible;
                        CloseButton.Visibility = Visibility.Collapsed;
                        break;
                    case MessageDialogButton.YesNoCancel:
                        OkButton.Visibility = Visibility.Collapsed;
                        YesButton.Visibility = Visibility.Visible;
                        NoButton.Visibility = Visibility.Visible;
                        CancelButton.Visibility = Visibility.Visible;
                        CloseButton.Visibility = Visibility.Collapsed;
                        break;
                    case MessageDialogButton.YesNo:
                        OkButton.Visibility = Visibility.Collapsed;
                        YesButton.Visibility = Visibility.Visible;
                        NoButton.Visibility = Visibility.Visible;
                        CancelButton.Visibility = Visibility.Collapsed;
                        CloseButton.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }

        MessageDialogImage _image;
        /// <summary>
        /// Obtient ou définit l'image à afficher.
        /// </summary>
        public MessageDialogImage Image
        {
            get { return _image; }
            set
            {
                _image = value;

                string imageName;

                switch (_image)
                {
                    case MessageDialogImage.None:
                        imageName = null;
                        break;
                    case MessageDialogImage.Error:
                        imageName = "stop32.png";
                        break;
                    case MessageDialogImage.Hand:
                        imageName = "stop32.png";
                        break;
                    case MessageDialogImage.Stop:
                        imageName = "stop32.png";
                        break;
                    case MessageDialogImage.Question:
                        imageName = "questionbook32.png";
                        break;
                    case MessageDialogImage.Exclamation:
                        imageName = "warning32.png";
                        break;
                    case MessageDialogImage.Warning:
                        imageName = "warning32.png";
                        break;
                    case MessageDialogImage.Information:
                        imageName = "info_16x32.png";
                        break;
                    case MessageDialogImage.Asterisk:
                        imageName = "info_16x32.png";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value));
                }

                MessageBoxImage.Source = imageName == null ? null :
                    (ImageSource)new ImageSourceConverter().ConvertFrom("pack://siteoforigin:,,,/Resources/Images/" + imageName);
            }
        }


        MessageDialogResult _defaultButton;
        /// <summary>
        /// Obtient ou définit le bouton activé par défaut.
        /// </summary>
        public MessageDialogResult DefaultButton
        {
            get { return _defaultButton; }
            set
            {
                _defaultButton = value;
                switch (_defaultButton)
                {
                    case MessageDialogResult.None:
                        break;
                    case MessageDialogResult.OK:
                        OkButton.IsDefault = true;
                        OkButton.Focus();
                        break;
                    case MessageDialogResult.Cancel:
                        CancelButton.IsDefault = true;
                        CancelButton.Focus();
                        break;
                    case MessageDialogResult.Yes:
                        YesButton.IsDefault = true;
                        YesButton.Focus();
                        break;
                    case MessageDialogResult.No:
                        NoButton.IsDefault = true;
                        NoButton.Focus();
                        break;
                }
            }
        }

        /// <summary>
        /// Obtient le résultat du dialogue.
        /// </summary>
        public MessageDialogResult MessageDialogResult { get; private set; }

        /// <summary>
        /// Gère l'évènement Click du contrôle OkButton.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        async void OkButton_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Pas de rapport d'erreur en mode DEBUG");
            await Task.CompletedTask;
#else
            if (InitialException != null)
                await SendReportAsync();
#endif

            MessageDialogResult = MessageDialogResult.OK;
            Close();
        }

        /// <summary>
        /// Gère l'évènement Click du contrôle OkButton.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void YesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult = MessageDialogResult.Yes;
            Close();
        }

        /// <summary>
        /// Gère l'évènement Click du contrôle OkButton.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void NoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult = MessageDialogResult.No;
            Close();
        }

        /// <summary>
        /// Gère l'évènement Click du contrôle OkButton.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult = MessageDialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// Gère l'évènement Click du contrôle SendReportButton.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        async void SendReportButton_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Pas de rapport d'erreur en mode DEBUG");
            await Task.CompletedTask;
#else
            await SendReportAsync();
#endif

            MessageDialogResult = MessageDialogResult.OK;
            Close();
        }

        /// <summary>
        /// Gère l'évènement Closed du contrôle childWindow.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        async void childWindow_Closed(object sender, EventArgs e)
        {
#if DEBUG
            Console.WriteLine("Pas de rapport d'erreur en mode DEBUG");
            await Task.CompletedTask;
#else
            if (InitialException != null)
                await SendReportAsync();
#endif

            MessageDialogResult = MessageDialogResult.None;
            Close();
        }

        async Task SendReportAsync()
        {
            if (!IoC.Resolve<IServiceBus>().Get<ISettingsService>().SendReport)
            {
                Console.WriteLine("L'envoi de rapport d'erreur n'est pas autorisé.");
                return;
            }
#if (!ACTIVE_FRESHDESK)
            Console.WriteLine("Freshdesk est désactivé.");
#else
            await IoC.Resolve<IServiceBus>().Get<IFreshdeskService>().SendErrorReportTicketAsync(InitialException,
                UsernameTB.Text, CompanyTB.Text, EmailTB.Text, AdditionalInfoTB.Text);
#endif
        }

        void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
