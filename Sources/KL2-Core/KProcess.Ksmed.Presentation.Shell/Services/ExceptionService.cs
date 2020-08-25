using KProcess.KL2.Languages;
using KProcess.Presentation.Windows;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.Shell
{
    public enum ExceptionType
    {
        Dispatcher = 0,
        TaskScheduler = 1,
        AppDomain = 3,
    }

    [Export(typeof(IExceptionHandler))]
    public class ExceptionService : IExceptionHandler
    {
#pragma warning disable CS0649
        [Import]
        IDialogFactory _dialogFactory;
#pragma warning restore CS0649

        public ExceptionService()
        {
            IoC.RegisterInstance<IExceptionHandler>(this);
        }

        public void HandleException(Exception ex, bool showMessage = true, string msgTitle = null)
        {
            if (ex == null || !showMessage)
                return;
            TraceManager.TraceDebug(ex.ToString());

            if (ex is AggregateException aggregateException)
            {
                foreach (var e in aggregateException.Flatten().InnerExceptions)
                    HandleException(e);
            }

            msgTitle = string.IsNullOrEmpty(msgTitle) ? Resources.LocalizationResources.Error : msgTitle;
            string mess = string.Empty;

            //Unhandled exception custom message
            if (ex is OutOfMemoryException)
                mess = IoC.Resolve<ILocalizationManager>().GetString("Common_Error_OutOfMemoryException");

            //defince more exception...


            ShowDialog(mess, msgTitle, ex);
        }

        public void HandleFatalException(Exception ex, bool showMessage = true, string msgTitle = null)
        {
            string mess = Resources.LocalizationResources.Exception_GenericMessage;
            msgTitle = string.IsNullOrEmpty(msgTitle) ? Resources.LocalizationResources.Error : msgTitle;

            bool forceClose = false;
            if (ex is ServerNotReacheableException)
            {
                mess = ex.Message;
                forceClose = true;
            }

            ShowDialog(mess, msgTitle, ex, forceClose);
        }

        public void HandleTaskException(Exception ex, bool showMessage = true, string msgTitle = null)
        {
            string mess = Resources.LocalizationResources.Exception_GenericMessage;
            msgTitle = string.IsNullOrEmpty(msgTitle) ? Resources.LocalizationResources.Error : msgTitle;
            if (ex is TaskCanceledException || ex is ServerNotReacheableException)
                mess = Resources.LocalizationResources.Exception_CannotInitializeLocalizationConnectToDatabase;

            ShowDialog(mess, msgTitle, ex);
        }

        void ShowDialog(string mess, string title, Exception ex, bool forceClose = false)
        {
            if (Application.Current?.Dispatcher?.CheckAccess() == false)
            {
                Application.Current.Dispatcher.Invoke
                (
                    new Func<string, string, Exception, bool, bool>
                    (
                        (m, c, mbb, mbi) =>
                        {
                            ShowDialog(m, c, mbb, mbi);
                            return true;
                        }
                    ), mess, title, ex, forceClose
                );
                return;
            }

            _dialogFactory.GetDialogView<IErrorDialog>().Show(mess, title, ex, forceClose);
        }

        public void RaiseTaskException(Exception ex)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            var currentTask = Task.CompletedTask;
            if (currentTask.IsCompleted)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => throw ex), DispatcherPriority.Normal);
                return;
            }

            throw currentTask.Exception ?? throw ex;
        }
    }
}
