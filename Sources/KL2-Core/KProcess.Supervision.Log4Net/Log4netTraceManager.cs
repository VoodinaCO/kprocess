using System;

namespace KProcess
{
    /// <summary>
    /// Permet de tracer depuis une méthode statique
    /// </summary>
    public class Log4netTraceManager : ITraceManager
    {
        private readonly ITraceWrapper _traceWrapper;

        public Log4netTraceManager(ITraceWrapper traceWrapper)
        {
            _traceWrapper = traceWrapper;
        }

        #region Méthodes Debug

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void TraceDebug(string message)
        {
            DoTrace(trace => trace.Debug(message));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceDebug(string format, params object[] args)
        {
            DoTrace(trace => trace.Debug(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public void TraceDebug(Exception exception, string message)
        {
            DoTrace(trace => trace.Debug(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceDebug(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Debug(exception, format, args));
        }

        #endregion

        #region Méthodes Info

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void TraceInfo(string message)
        {
            DoTrace(trace => trace.Info(message));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceInfo(string format, params object[] args)
        {
            DoTrace(trace => trace.Info(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public void TraceInfo(Exception exception, string message)
        {
            DoTrace(trace => trace.Info(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceInfo(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Info(exception, format, args));
        }

        #endregion

        #region Méthodes Warning

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void TraceWarning(string message)
        {
            DoTrace(trace => trace.Warning(message));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceWarning(string format, params object[] args)
        {
            DoTrace(trace => trace.Warning(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public void TraceWarning(Exception exception, string message)
        {
            DoTrace(trace => trace.Warning(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceWarning(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Warning(exception, format, args));
        }

        #endregion

        #region Méthodes Error

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void TraceError(string message)
        {
            DoTrace(trace => trace.Error(message));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceError(string format, params object[] args)
        {
            DoTrace(trace => trace.Error(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public void TraceError(Exception exception, string message)
        {
            DoTrace(trace => trace.Error(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceError(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Error(exception, format, args));
        }

        #endregion

        #region Méthodes Fatal

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void TraceFatal(string message)
        {
            DoTrace(trace => trace.Fatal(message));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceFatal(string format, params object[] args)
        {
            DoTrace(trace => trace.Fatal(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public void TraceFatal(Exception exception, string message)
        {
            DoTrace(trace => trace.Fatal(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void TraceFatal(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Fatal(exception, format, args));
        }

        #endregion

        #region Méthodes privées

        private void DoTrace(Action<ITraceWrapper> execute)
        {
            ITraceWrapper trace = _traceWrapper;
            if (trace != null)
            {
                _traceWrapper.Name = ReflectionHelper.GetCallingFullTypeName(typeof(ITraceManager), typeof(ExceptionManager), typeof(KProcess.Log4netTraceManager));
                execute(trace);
            }
        }

        #endregion
    }
}
