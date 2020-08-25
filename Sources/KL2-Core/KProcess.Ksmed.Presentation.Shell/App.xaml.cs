using KProcess.Ksmed.Presentation.Core;
using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
#pragma warning disable CS0649
        [Import]
        IExceptionHandler _exceptionHandler;
#pragma warning restore CS0649

        private Controller _controller;

        private bool _isShowingDispatcherExceptionDialog;

        /// <summary>
        /// Initialise la classe <see cref="App"/>.
        /// </summary>
        static App()
        {
            GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();

            FrameworkElement.LanguageProperty.OverrideMetadata(
              typeof(FrameworkElement),
              new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag))
            );

            ControlEnterBindingUpdate.ActivateTextBox();
            ControlForbiddenCharacters.ActivateInputElement();
            ControlSpellChecking.ActivateInputElement();
        }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA3NjkwQDMxMzcyZTMxMmUzMGRpNUF0QThQTklxRFlMeDhXR3NRZFlHcnU4VjN5Z2tGS3FaT2sxM1hER1E9;MTA3NjkxQDMxMzcyZTMxMmUzMFVqQ0tFMGcxclFlNUVKVUJtdWpGcWlmQ1gvbE9PMGxZYWxHTFZYSmxHZDQ9;MTA3NjkyQDMxMzcyZTMxMmUzMEZMTENrQjNCYjUvNmtKTHFNRHBGRkxXVlFXNGZReDVrbWl2WXZsczFORlk9;MTA3NjkzQDMxMzcyZTMxMmUzME5MY3VCaXEydmZyS085RmhzdWtMK2pjNE4wQmY1RUZxMHorMHhHZGlpOFU9;MTA3Njk0QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89;MTA3Njk1QDMxMzcyZTMxMmUzMFM5YmYyTDhoL1N4Z2RIeEZ4S1p6KzF5eGxkdkp2VDNIMFpkZHBwYWVndFE9;MTA3Njk2QDMxMzcyZTMxMmUzMEtFZTNrcnBaelBHUXg4TC9kdGE0TjVQTHNIVVloTDZ3N1I4VHQ0NysvTEE9;MTA3Njk3QDMxMzcyZTMxMmUzMExlamxrNmhmL0RyR3Y2QzBoYitxOXhyazRnR0JZUUJBZThJZmhENG1oVGs9;MTA3Njk4QDMxMzcyZTMxMmUzMGRrTFo4clJIR29kZEZrdlRSUlFsQ3VvckJGRjNiZCtscFNYbE0zRThQSGc9;MTA3Njk5QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89");
        }

        /// <summary>
        /// Lève l'évènement <see cref="E:System.Windows.Application.Startup"/>.
        /// </summary>
        /// <param name="e">Un <see cref="T:System.Windows.StartupEventArgs"/> contenant les données de l'évènement.</param>
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                if (StartupUri != null)
                    throw new Exception("L'application ne doit pas avoir de StartupUri");

                base.OnStartup(e);
                RegisterUnhandledExceptionEvents();

                this.TraceInfo("Lancement application. Version {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                _controller = new Controller();
                await _controller.Start();
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                    ex = (ex as AggregateException).Flatten().InnerException;

                Current.Shutdown();
            }
        }

        /// <summary>
        /// Lève l'évènement <see cref="E:System.Windows.Application.Exit"/>.
        /// </summary>
        /// <param name="e">Un <see cref="T:System.Windows.ExitEventArgs"/> contenant les données de l'évènement.</param>
        protected override async void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            await _controller?.Stop();
        }

        /// <summary>
        /// Active la première fenêtre.
        /// </summary>
        internal void Activate()
        {
            if (Windows.Count >= 0)
                Windows[0].Activate();
        }

        #region Private Members

        /// <summary>
        /// Appelé lorsqu'une exception non gérée remonte jusqu'au dispatcher.
        /// </summary>
        /// <param name="sender">L'instance source de l'évènement.</param>
        /// <param name="e">Les données de l'évènement.</param>
        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            TraceManager.TraceError(e.Exception, "Exception non gérée dans le dispatcher.");

            if (_isShowingDispatcherExceptionDialog)
            {
                HandleException(e.Exception, ExceptionType.AppDomain);
                return;
            }

            _isShowingDispatcherExceptionDialog = true;

            // Lever l'exception en mode débug dans le débugger
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();

            HandleException(e.Exception, ExceptionType.Dispatcher);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject, ExceptionType.AppDomain);
        }

        void HandleException(Exception exception, ExceptionType type)
        {
            if (exception is AggregateException aggregateException)
                foreach (var e in aggregateException.Flatten().InnerExceptions)
                    HandleException(e, type);

            switch (type)
            {
                case ExceptionType.Dispatcher:
                    {
                        _exceptionHandler.HandleFatalException(exception);
                        break;
                    }
                case ExceptionType.TaskScheduler:
                    {
                        _exceptionHandler.HandleTaskException(exception);
                        break;
                    }
                case ExceptionType.AppDomain:
                    {
                        _exceptionHandler.HandleException(exception);
                        break;
                    }
            }
        }

        void RegisterUnhandledExceptionEvents()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            HandleException(e.Exception, ExceptionType.TaskScheduler);
        }

        #endregion
    }
}
