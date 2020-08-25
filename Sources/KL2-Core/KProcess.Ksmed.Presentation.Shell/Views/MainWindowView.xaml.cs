using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IMainWindowViewModel))]
    public partial class MainWindowView : Window, IModalWindowView
    {
        /// <summary>
        /// Initialise la classe <see cref="MainWindowView"/>.
        /// </summary>
        static MainWindowView()
        {
            EventManager.RegisterClassHandler(typeof(MainWindowView), KMediaElement.MediaFailedEvent, (RoutedMediaFailedEventHandler)MediaElement_MediaFailed);
        }

        /// <summary>
        /// Survient lorsqu'une vidéo n'a pas pu être chargée.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="KProcess.Presentation.Windows.Controls.RoutedMediaFailedEventArgs"/> contenant les données de l'évènement.</param>
        private static void MediaElement_MediaFailed(object sender, RoutedMediaFailedEventArgs e)
        {
            var dialogFactory = IoC.Resolve<System.ComponentModel.Composition.Hosting.CompositionContainer>()
                .GetExportedValue<IDialogFactory>();

            /*if (!File.Exists(e.FileSource))
            {
                TraceManager.TraceWarning(string.Format("Le fichier vidéo {0} n'existe pas.", e.FileSource));
                dialogFactory.GetDialogView<IMessageDialog>().Show(string.Format(LocalizationManager.GetString("View_MainWindow_VideoNotFound"), e.FileSource),LocalizationManager.GetString("Common_Error"));
                return;
            }*/

            TraceManager.TraceDebug(e.Exception, e.Exception.Message);

            // tenter de récupérer les infos du media
            MediaInfo info = null;
            try
            {
                info = MediaDetector.GetMediaInfo(e.FileSource);
            }
            catch (Exception) { }

            if (info != null)
            {
                var sb = new StringBuilder("MediaInfo :");
                sb.AppendLine(e.FileSource);
                foreach (System.ComponentModel.PropertyDescriptor prop in System.ComponentModel.TypeDescriptor.GetProperties(info.GetType()))
                {
                    sb.AppendFormat("{0} = {1}{2}", prop.Name, prop.GetValue(info), Environment.NewLine);
                }

                TraceManager.TraceDebug(sb.ToString());
            }

            /*dialogFactory.GetDialogView<IErrorDialog>()
                .Show(LocalizationManager.GetString("View_MainWindow_MediaFailed"),
                    LocalizationManager.GetString("Common_Error"), e.Exception);*/
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="MainWindowView"/>.
        /// </summary>
        public MainWindowView()
        {

            InitializeComponent();
            DataContextChanged += async (s, e) =>
            {
                await TryPublish();
            };

            Loaded += async (s, e) =>
            {
                await TryPublish();
            };

        }

        /// <summary>
        /// Tente de publier l'évènement <see cref="ViewLoadedEvent"/>.
        /// </summary>
        async Task TryPublish()
        {
            if (DataContext is IMainWindowViewModel _dataContext && IsLoaded)
                await _dataContext.OnViewLoaded();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel |= !((MainWindowViewModel)DataContext).CheckCanClose();
        }
    }
}
