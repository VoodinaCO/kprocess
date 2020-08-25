using System;

namespace KProcess
{
    /// <summary>
    /// Permet de tracer depuis une méthode statique
    /// </summary>
    public static class TraceManager
    {
        #region Méthodes Debug

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="message">message à tracer</param>
        public static void TraceDebug(string message)
        {
            DoTrace(trace => trace.Debug(message));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceDebug(string format, params object[] args)
        {
            DoTrace(trace => trace.Debug(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceDebug(Exception exception, string message)
        {
            DoTrace(trace => trace.Debug(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceDebug(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Debug(exception, format, args));
        }

        #endregion

        #region Méthodes Info

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="message">message à tracer</param>
        public static void TraceInfo(string message)
        {
            DoTrace(trace => trace.Info(message));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceInfo(string format, params object[] args)
        {
            DoTrace(trace => trace.Info(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceInfo(Exception exception, string message)
        {
            DoTrace(trace => trace.Info(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceInfo(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Info(exception, format, args));
        }

        #endregion

        #region Méthodes Warning

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="message">message à tracer</param>
        public static void TraceWarning(string message)
        {
            DoTrace(trace => trace.Warning(message));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceWarning(string format, params object[] args)
        {
            DoTrace(trace => trace.Warning(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceWarning(Exception exception, string message)
        {
            DoTrace(trace => trace.Warning(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceWarning(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Warning(exception, format, args));
        }

        #endregion

        #region Méthodes Error

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="message">message à tracer</param>
        public static void TraceError(string message)
        {
            DoTrace(trace => trace.Error(message));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceError(string format, params object[] args)
        {
            DoTrace(trace => trace.Error(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceError(Exception exception, string message)
        {
            DoTrace(trace => trace.Error(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceError(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Error(exception, format, args));
        }

        #endregion

        #region Méthodes Fatal

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="message">message à tracer</param>
        public static void TraceFatal(string message)
        {
            DoTrace(trace => trace.Fatal(message));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceFatal(string format, params object[] args)
        {
            DoTrace(trace => trace.Fatal(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceFatal(Exception exception, string message)
        {
            DoTrace(trace => trace.Fatal(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceFatal(Exception exception, string format, params object[] args)
        {
            DoTrace(trace => trace.Fatal(exception, format, args));
        }

        #endregion

        #region Méthodes privées

        private static void DoTrace(Action<ITraceWrapper> execute)
        {
            ITraceWrapper trace = IoC.Resolve<ITraceWrapper>();
            if (trace != null)
            {
                trace.Name = ReflectionHelper.GetCallingFullTypeName(typeof(TraceManager), typeof(ExceptionManager));
                execute(trace);
            }
        }

        #endregion
    }
}
