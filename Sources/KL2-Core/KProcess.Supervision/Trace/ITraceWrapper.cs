using System;

namespace KProcess
{
  /// <summary>
  /// Définit le contrat d'un wrapper de traces
  /// </summary>
  public interface ITraceWrapper
  {
    #region Proprietes

    /// <summary>
    /// Obtient ou definit le nom du wrapper de trace
    /// </summary>
    string Name
    {
      get;
      set;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Indique si le wrapper accepte le niveau debug
    /// </summary>
    /// <returns>true si le niveau debug est actif, false sinon</returns>
    bool IsDebugEnabled();

    /// <summary>
    /// Effectue une trace de niveau debug
    /// </summary>
    /// <param name="message">message à tracer</param>
    void Debug(string message);

    /// <summary>
    /// Effectue une trace de niveau debug
    /// </summary>
    /// <param name="message">message à tracer</param>
    /// <param name="exception">exception à tracer</param>
    void Debug(string message, Exception exception);

    /// <summary>
    /// Effectue une trace de niveau debug
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    void Debug(Exception exception, string format, params object[] args);

    #endregion

    #region Info

    /// <summary>
    /// Indique si le wrapper accepte le niveau info
    /// </summary>
    /// <returns>true si le niveau info est actif, false sinon</returns>
    bool IsInfoEnabled();

    /// <summary>
    /// Effectue une trace de niveau info
    /// </summary>
    /// <param name="message">message à tracer</param>
    void Info(string message);

    /// <summary>
    /// Effectue une trace de niveau info
    /// </summary>
    /// <param name="message">message à tracer</param>
    /// <param name="exception">exception à tracer</param>
    void Info(string message, Exception exception);

    /// <summary>
    /// Effectue une trace de niveau info
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    void Info(Exception exception, string format, params object[] args);

    #endregion

    #region Warning

    /// <summary>
    /// Indique si le wrapper accepte le niveau warning
    /// </summary>
    /// <returns>true si le niveau warning est actif, false sinon</returns>
    bool IsWarningEnabled();

    /// <summary>
    /// Effectue une trace de niveau warning
    /// </summary>
    /// <param name="message">message à tracer</param>
    void Warning(string message);

    /// <summary>
    /// Effectue une trace de niveau warning
    /// </summary>
    /// <param name="message">message à tracer</param>
    /// <param name="exception">exception à tracer</param>
    void Warning(string message, Exception exception);

    /// <summary>
    /// Effectue une trace de niveau warning
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    void Warning(Exception exception, string format, params object[] args);

    #endregion

    #region Error

    /// <summary>
    /// Indique si le wrapper accepte le niveau error
    /// </summary>
    /// <returns>true si le niveau error est actif, false sinon</returns>
    bool IsErrorEnabled();

    /// <summary>
    /// Effectue une trace de niveau error
    /// </summary>
    /// <param name="message">message à tracer</param>
    void Error(string message);

    /// <summary>
    /// Effectue une trace de niveau error
    /// </summary>
    /// <param name="message">message à tracer</param>
    /// <param name="exception">exception à tracer</param>
    void Error(string message, Exception exception);

    /// <summary>
    /// Effectue une trace de niveau error
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    void Error(Exception exception, string format, params object[] args);

    #endregion

    #region Fatal

    /// <summary>
    /// Indique si le wrapper accepte le niveau fatal
    /// </summary>
    /// <returns>true si le niveau fatal est actif, false sinon</returns>
    bool IsFatalEnabled();

    /// <summary>
    /// Effectue une trace de niveau fatal
    /// </summary>
    /// <param name="message">message à tracer</param>
    void Fatal(string message);

    /// <summary>
    /// Effectue une trace de niveau fatal
    /// </summary>
    /// <param name="message">message à tracer</param>
    /// <param name="exception">exception à tracer</param>
    void Fatal(string message, Exception exception);

    /// <summary>
    /// Effectue une trace de niveau fatal
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    void Fatal(Exception exception, string format, params object[] args);

    #endregion
  }
}
