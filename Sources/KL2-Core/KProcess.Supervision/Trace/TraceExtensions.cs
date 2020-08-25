using System;

namespace KProcess
{
    /// <summary>
    /// Fournit des méthodes d'extension à Object pour permettre de tracer de n'importe où
    /// </summary>
    public static class TraceExtensions
    {
        #region Méthodes Debug

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="message">message à tracer</param>
        public static void TraceDebug(this object value, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Debug(message));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceDebug(this object value, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Debug(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>        
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceDebug(this object value, Exception exception, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Debug(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceDebug(this object value, Exception exception, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Debug(exception, format, args));
        }

        #endregion

        #region Méthodes Info

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="message">message à tracer</param>
        public static void TraceInfo(this object value, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Info(message));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceInfo(this object value, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Info(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>        
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceInfo(this object value, Exception exception, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Info(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceInfo(this object value, Exception exception, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Info(exception, format, args));
        }

        #endregion

        #region Méthodes Warning

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="message">message à tracer</param>
        public static void TraceWarning(this object value, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Warning(message));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceWarning(this object value, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Warning(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>        
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceWarning(this object value, Exception exception, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Warning(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceWarning(this object value, Exception exception, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Warning(exception, format, args));
        }

        #endregion

        #region Méthodes Error

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="message">message à tracer</param>
        public static void TraceError(this object value, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Error(message));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceError(this object value, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Error(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>        
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceError(this object value, Exception exception, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Error(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceError(this object value, Exception exception, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Error(exception, format, args));
        }

        #endregion

        #region Méthodes Fatal

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="message">message à tracer</param>
        public static void TraceFatal(this object value, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Fatal(message));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceFatal(this object value, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Fatal(String.Format(format, args)));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>        
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        public static void TraceFatal(this object value, Exception exception, string message)
        {
            DoTrace(value.GetType().FullName, trace => trace.Fatal(message, exception));
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="value">L'objet source de la trace.</param>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public static void TraceFatal(this object value, Exception exception, string format, params object[] args)
        {
            DoTrace(value.GetType().FullName, trace => trace.Fatal(exception, format, args));
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Effectue une trace.
        /// </summary>
        /// <param name="name">Le nom de la source de la trace.</param>
        /// <param name="execute">L'action à exécuter pour tracer.</param>
        private static void DoTrace(string name, Action<ITraceWrapper> execute)
        {
            ITraceWrapper trace = IoC.Resolve<ITraceWrapper>();
            if (trace != null)
            {
                trace.Name = name;
                execute(trace);
            }
        }

        #endregion
    }
}
