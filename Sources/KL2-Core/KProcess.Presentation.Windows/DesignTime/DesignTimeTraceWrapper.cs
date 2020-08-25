using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.DesignTime
{
    /// <summary>
    /// Un moteur de traces utilisé en mode design.
    /// </summary>
    internal class DesignTraceWrapper : ITraceWrapper
    {
        public void Debug(Exception exception, string format, params object[] args) { }

        public void Debug(string message, Exception exception) { }

        public void Debug(string message) { }

        public void Error(Exception exception, string format, params object[] args) { }

        public void Error(string message, Exception exception) { }

        public void Error(string message) { }

        public void Fatal(Exception exception, string format, params object[] args) { }

        public void Fatal(string message, Exception exception) { }

        public void Fatal(string message) { }

        public void Info(Exception exception, string format, params object[] args) { }

        public void Info(string message, Exception exception) { }

        public void Info(string message) { }

        public bool IsDebugEnabled() { return false; }

        public bool IsErrorEnabled() { return false; }

        public bool IsFatalEnabled() { return false; }

        public bool IsInfoEnabled() { return false; }

        public bool IsWarningEnabled() { return false; }

        public string Name
        {
            get { return "Design"; }
            set { }
        }

        public void Warning(Exception exception, string format, params object[] args) { }

        public void Warning(string message, Exception exception) { }

        public void Warning(string message) { }
    }
}
