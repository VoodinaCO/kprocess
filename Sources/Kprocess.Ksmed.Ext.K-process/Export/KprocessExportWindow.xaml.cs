using KProcess.Globalization;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    /// <summary>
    /// Interaction logic for KprocessExportWindow.xaml
    /// </summary>
    public partial class KprocessExportWindow : Window
    {
        private static Task<KprocessExportWindow> _windowInstance = null;

        private bool _preventClosing = true;

        public static int nbVideos = 0;
        private static CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Gets the cancellation token
        /// </summary>
        public static CancellationToken CancellationToken { get { return _cancellationTokenSource.Token; } }

        public void DoClose()
        {
            _preventClosing = false;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Des effets de bord: lorsque l'application ferme avant la window, impossible de fermer la window......
            //if (_preventClosing)
            //{
            //    e.Cancel = true;
            //    return;
            //}
            RequestCancellation();
            base.OnClosing(e); 
        }

        public KprocessExportWindow()
        {
            InitializeComponent();
        }

        public static void IncrLogVideoProgress(int total)
        {
            try
            {
                if (_windowInstance != null)
                {
                    _windowInstance.ContinueWith(task =>
                    {
                        var window = task.Result;
                        if (window != null)
                        {

                            window.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    if (nbVideos == 0)
                                        window.btn_stop.Visibility = Visibility.Visible;

                                    window.Details.Text = string.Format(LocalizationManager.GetString("ExtKp_Dlg_CreatingVideoFiles"), ++nbVideos, total);//"Création des fichiers videos {0}/{1}"
                                }
                                catch { }
                            }));
                        }
                    });
                }
            }
            catch { }
        }

        public static void Log(string message)
        {
            try
            {
                if (_windowInstance != null)
                {
                    _windowInstance.ContinueWith(task =>
                    {
                        var window = task.Result;
                        if (window != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    window.Details.Text = message;
                                }
                                catch { }
                            }));
                        }
                    });
                }
            }
            catch { }
        }

        /// <summary>
        /// Stops the current window awaiter instance
        /// </summary>
        public static void StopCurrent()
        {
            if (_windowInstance != null)
            {
                _windowInstance.ContinueWith(windowTask =>
                {
                    windowTask.Result.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        windowTask.Result.DoClose();
                        windowTask.Result.Dispatcher.Thread.Abort();
                    }), priority: System.Windows.Threading.DispatcherPriority.Loaded);
                });
            }            
        }

        /// <summary>
        /// Starts a new window awaiter
        /// </summary>
        /// <returns></returns>
        public static Task<KprocessExportWindow> StartNew()
        {
            var continuationTaskSource = new TaskCompletionSource<KprocessExportWindow>();
            _windowInstance = continuationTaskSource.Task;
            _cancellationTokenSource = new CancellationTokenSource();

            var newWindowThread = new Thread(new ThreadStart(() =>
            {
                var window = new KprocessExportWindow();
                continuationTaskSource.SetResult(window);
                window.Show();
                System.Windows.Threading.Dispatcher.Run();
            }));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            newWindowThread.IsBackground = true;
            newWindowThread.Start();

            return _windowInstance;
        }

        /// <summary>
        /// Requests cancellation of the current export
        /// </summary>
        public static void RequestCancellation() =>
            _cancellationTokenSource?.Cancel();

        private void Button_Click(object sender, RoutedEventArgs e) =>
            RequestCancellation();
    }
}
