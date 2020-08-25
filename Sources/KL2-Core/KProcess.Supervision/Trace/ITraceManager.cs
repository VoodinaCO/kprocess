using System;

namespace KProcess
{
    /// <summary>
    /// Permet de tracer depuis une méthode statique
    /// </summary>
    public interface ITraceManager
    {
        #region Méthodes Debug

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="message">message à tracer</param>
        void TraceDebug(string message);

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceDebug(string format, params object[] args);

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        void TraceDebug(Exception exception, string message);

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceDebug(Exception exception, string format, params object[] args);

        #endregion

        #region Méthodes Info

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="message">message à tracer</param>
        void TraceInfo(string message);

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceInfo(string format, params object[] args);

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        void TraceInfo(Exception exception, string message);

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceInfo(Exception exception, string format, params object[] args);

        #endregion

        #region Méthodes Warning

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="message">message à tracer</param>
        void TraceWarning(string message);

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceWarning(string format, params object[] args);

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        void TraceWarning(Exception exception, string message);

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceWarning(Exception exception, string format, params object[] args);

        #endregion

        #region Méthodes Error

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="message">message à tracer</param>
        void TraceError(string message);

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceError(string format, params object[] args);

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        void TraceError(Exception exception, string message);

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceError(Exception exception, string format, params object[] args);

        #endregion

        #region Méthodes Fatal

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="message">message à tracer</param>
        void TraceFatal(string message);

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceFatal(string format, params object[] args);

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>        
        /// <param name="exception">exception à tracer</param>
        /// <param name="message">message à tracer</param>
        void TraceFatal(Exception exception, string message);

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        void TraceFatal(Exception exception, string format, params object[] args);

        #endregion

    }
}
