using KProcess.Presentation.Windows.SingleInstance;
using System;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Shell
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                using (SingleInstanceManager manager = SingleInstanceManager.Initialize(GetSingleInstanceManagerSetup()))
                {
                    App app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
            }
            catch (ApplicationInstanceAlreadyExistsException)
            {
            }
        }

        private static SingleInstanceManagerSetup GetSingleInstanceManagerSetup()
        {
            return new SingleInstanceManagerSetup("KL² Video Analyst")
            {
                ArgumentsHandler = args => ((App)Application.Current).Activate(),
                ArgumentsHandlerInvoker = new ApplicationDispatcherInvoker(),
                DelivaryFailureNotification = ex => MessageBox.Show(Resources.LocalizationResources.Exception_GenericMessage),
                TerminationOption = TerminationOption.Throw
            };
        }
    }
}
