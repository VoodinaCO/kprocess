using log4net;
using System;
using System.Reflection;

namespace KProcess.Supervision.Log4net
{
    /// <summary>
    /// Wrapper de trace basé sur Log4net
    /// </summary>
    public class Log4netWrapper : ITraceWrapper
    {
        #region Attributs

        private string _name;
        private ILog _logger;

        #endregion

        #region Proprietes

        /// <summary>
        /// Obtient ou définit le nom du wrapper de trace Log4Net
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _logger = LogManager.GetLogger(Assembly.GetExecutingAssembly(), _name);
            }
        }

        #endregion

        #region Constructeur statique

        /// <summary>
        /// Constructeur statique
        /// </summary>
        static Log4netWrapper()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            logRepository.RendererMap.Put(ReflectionTypeLoadExceptionRenderer.RendereredType, new ReflectionTypeLoadExceptionRenderer());

            log4net.Config.XmlConfigurator.Configure(logRepository);
        }

        #endregion

        #region Debug

        /// <summary>
        /// Indique si le wrapper accepte le niveau debug
        /// </summary>
        /// <returns>true si le niveau debug est actif, false sinon</returns>
        public bool IsDebugEnabled()
        {
            return _logger.IsDebugEnabled;
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void Debug(string message)
        {
            if (IsDebugEnabled())
                _logger.Debug(message);
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="message">message à tracer</param>
        /// <param name="exception">exception à tracer</param>
        public void Debug(string message, Exception exception)
        {
            if (IsDebugEnabled())
                _logger.Debug(message, exception);
        }

        /// <summary>
        /// Effectue une trace de niveau debug
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void Debug(Exception exception, string format, params object[] args)
        {
            if (IsDebugEnabled())
                _logger.DebugFormat(String.Format(format, args), exception);
        }

        #endregion

        #region Info

        /// <summary>
        /// Indique si le wrapper accepte le niveau info
        /// </summary>
        /// <returns>true si le niveau info est actif, false sinon</returns>
        public bool IsInfoEnabled()
        {
            return _logger.IsInfoEnabled;
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void Info(string message)
        {
            if (IsInfoEnabled())
                _logger.Info(message);
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="message">message à tracer</param>
        /// <param name="exception">exception à tracer</param>
        public void Info(string message, Exception exception)
        {
            if (IsInfoEnabled())
                _logger.Info(message, exception);
        }

        /// <summary>
        /// Effectue une trace de niveau info
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void Info(Exception exception, string format, params object[] args)
        {
            if (IsInfoEnabled())
                _logger.Info(String.Format(format, args), exception);
        }

        #endregion

        #region Warning

        /// <summary>
        /// Indique si le wrapper accepte le niveau warning
        /// </summary>
        /// <returns>true si le niveau warning est actif, false sinon</returns>
        public bool IsWarningEnabled()
        {
            return _logger.IsWarnEnabled;
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void Warning(string message)
        {
            if (IsWarningEnabled())
                _logger.Warn(message);
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="message">message à tracer</param>
        /// <param name="exception">exception à tracer</param>
        public void Warning(string message, Exception exception)
        {
            if (IsWarningEnabled())
                _logger.Warn(message, exception);
        }

        /// <summary>
        /// Effectue une trace de niveau warning
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void Warning(Exception exception, string format, params object[] args)
        {
            if (IsWarningEnabled())
                _logger.Warn(String.Format(format, args), exception);
        }

        #endregion

        #region Error

        /// <summary>
        /// Indique si le wrapper accepte le niveau error
        /// </summary>
        /// <returns>true si le niveau error est actif, false sinon</returns>
        public bool IsErrorEnabled()
        {
            return _logger.IsErrorEnabled;
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void Error(string message)
        {
            if (IsErrorEnabled())
                _logger.Error(message);
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="message">message à tracer</param>
        /// <param name="exception">exception à tracer</param>
        public void Error(string message, Exception exception)
        {
            if (IsErrorEnabled())
                _logger.Error(message, exception);
        }

        /// <summary>
        /// Effectue une trace de niveau error
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void Error(Exception exception, string format, params object[] args)
        {
            if (IsErrorEnabled())
                _logger.Error(String.Format(format, args), exception);
        }

        #endregion

        #region Fatal

        /// <summary>
        /// Indique si le wrapper accepte le niveau fatal
        /// </summary>
        /// <returns>true si le niveau fatal est actif, false sinon</returns>
        public bool IsFatalEnabled()
        {
            return _logger.IsFatalEnabled;
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="message">message à tracer</param>
        public void Fatal(string message)
        {
            if (IsFatalEnabled())
                _logger.Fatal(message);
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="message">message à tracer</param>
        /// <param name="exception">exception à tracer</param>
        public void Fatal(string message, Exception exception)
        {
            if (IsFatalEnabled())
                _logger.Fatal(message, exception);
        }

        /// <summary>
        /// Effectue une trace de niveau fatal
        /// </summary>
        /// <param name="exception">exception à tracer</param>
        /// <param name="format">format du message</param>
        /// <param name="args">parametres du message</param>
        public void Fatal(Exception exception, string format, params object[] args)
        {
            if (IsFatalEnabled())
                _logger.Fatal(String.Format(format, args), exception);
        }

        #endregion
    }
}
