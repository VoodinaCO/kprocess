using KProcess.Ksmed.Models.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace KProcess.Ksmed.Tests.Helper
{
    /// <summary>
    /// Un helper pour les tests sur la couche business.
    /// </summary>
    [TestClass] // Requis pour [AssemblyInitialize]
    public static partial class Initialization
    {
        /// <summary>
        /// Initialise les contexte de tests.
        /// </summary>
        /// <param name="tc">Le contexte de tests.</param>
        [AssemblyInitialize]
        public static void Initialize(TestContext tc)
        {
            SetTaskScheduler();
            InitLocalization();
            SetCurrentUser();
            InitDiagnostics(tc);
        }

        /// <summary>
        /// Définit le <see cref="TaskScheduler"/> correct pour les opérations asyncrhones.
        /// </summary>
        public static void SetTaskScheduler()
        {
            KProcess.Business.AsyncFactory.TaskSchedulerFactory = () => System.Threading.Tasks.TaskScheduler.Current;
        }

        /// <summary>
        /// Initialise la localisation.
        /// </summary>
        public static void InitLocalization()
        {
            InitLocalizationImpl();
        }

        static partial void InitLocalizationImpl();

        /// <summary>
        /// Définit un utilisateur courant.
        /// </summary>
        /// <param name="username">le nom d'utilisateur.</param>
        public static void SetCurrentUser(string username = "admin")
        {
            Security.SecurityContext.CurrentUser = new SecurityUser(new Models.User { Username = username });
        }

        /// <summary>
        /// Initialise les outils de diagnostics.
        /// </summary>
        /// <param name="tc">Le contexte de tests.</param>
        public static void InitDiagnostics(TestContext tc)
        {
            IoC.RegisterInstance<ITraceWrapper>(new TestsTraceWrapper(tc));

            if (Debugger.IsAttached)
                return;

            var dtl = Debug.Listeners.OfType<DefaultTraceListener>().FirstOrDefault();
            if (dtl != null)
                Debug.Listeners.Remove(dtl);

            var failL = Debug.Listeners.OfType<FailOnAssertListener>().FirstOrDefault();
            if (failL == null)
            {
                failL = new FailOnAssertListener(tc);
                Debug.Listeners.Add(failL);
            }
        }

        /// <summary>
        /// Listener effectuant un Assert.Fail lorsqu'un Debug.Assert échoue
        /// </summary>
        private class FailOnAssertListener : TraceListener
        {
            private TestContext _tc;
            public FailOnAssertListener(TestContext tc)
            {
                _tc = tc;
            }

            public override void Fail(string message)
            {
                this.Fail(message, null);
            }

            public override void Fail(string message, string detailMessage)
            {
                var stackTrace = new StackTrace(true);

                string stackTrace2;
                try
                {
                    stackTrace2 = stackTrace.ToString();
                }
                catch
                {
                    stackTrace2 = "";
                }

                var longMessage = string.Concat(
                    "Assert Failed: ",
                    Environment.NewLine,
                    message,
                    Environment.NewLine,
                    "Detailed message: ",
                    Environment.NewLine,
                    detailMessage,
                    Environment.NewLine,
                    stackTrace
                );
                this.Write(longMessage);
                Assert.Fail(longMessage);
            }

            public override void Write(string message)
            {
                _tc.WriteLine(message);
            }

            public override void WriteLine(string message)
            {
                _tc.WriteLine(message);
            }
        }

        private class TestsTraceWrapper : ITraceWrapper
        {

            private TestContext _testContext;

            public TestsTraceWrapper(TestContext tc)
            {
                _testContext = tc;
            }

            #region ITraceWrapper Members

            public string Name { get; set; }

            private void Write(string level, string message)
            {
                _testContext.WriteLine("{0}: {1}", level, message);
            }

            private void Write(string level, string message, Exception exception)
            {
                _testContext.WriteLine("{0}: {1}\r\nException : {2}", level, message, exception);
            }

            private void Write(string level, Exception exception, string format, object[] args)
            {
                _testContext.WriteLine("{0}: {1}\r\nException : {2}", level, string.Format(format, args), exception);
            }

            public bool IsDebugEnabled()
            {
                return true;
            }

            public void Debug(string message)
            {
                Write("Debug", message);
            }

            public void Debug(string message, Exception exception)
            {
                Write("Debug", message, exception);
            }

            public void Debug(Exception exception, string format, params object[] args)
            {
                Write("Debug", exception, format, args);
            }

            public bool IsInfoEnabled()
            {
                return true;
            }

            public void Info(string message)
            {
                Write("Info", message);
            }

            public void Info(string message, Exception exception)
            {
                Write("Info", message, exception);
            }

            public void Info(Exception exception, string format, params object[] args)
            {
                Write("Info", exception, format, args);
            }

            public bool IsWarningEnabled()
            {
                return true;
            }

            public void Warning(string message)
            {
                Write("Warning", message);
            }

            public void Warning(string message, Exception exception)
            {
                Write("Warning", message, exception);
            }

            public void Warning(Exception exception, string format, params object[] args)
            {
                Write("Warning", exception, format, args);
            }

            public bool IsErrorEnabled()
            {
                return true;
            }

            public void Error(string message)
            {
                Write("Error", message);
            }

            public void Error(string message, Exception exception)
            {
                Write("Error", message, exception);
            }

            public void Error(Exception exception, string format, params object[] args)
            {
                Write("Error", exception, format, args);
            }

            public bool IsFatalEnabled()
            {
                return true;
            }

            public void Fatal(string message)
            {
                Write("Fatal", message);
            }

            public void Fatal(string message, Exception exception)
            {
                Write("Fatal", message, exception);
            }

            public void Fatal(Exception exception, string format, params object[] args)
            {
                Write("Fatal", exception, format, args);
            }

            #endregion
        }

    }

}
