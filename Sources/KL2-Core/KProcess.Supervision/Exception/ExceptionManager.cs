using System;

namespace KProcess
{
  /// <summary>
  /// Gère les exceptions à lever en permettant leur traçage
  /// </summary>
  public static class ExceptionManager
  {
    #region Methode Pass

    /// <summary>
    /// Fait passer l'exception telle quelle
    /// </summary>
    /// <param name="exception">exception à passer</param>
    public static Exception Pass(Exception exception)
    {
      Trace(exception);
      return exception;
    }

    #endregion

    #region Methodes Create

    /// <summary>
    /// Crée une nouvelle exception
    /// </summary>
    /// <param name="message">message de l'exception</param>
    /// <typeparam name="TException">type de l'exception à créer</typeparam>
    public static Exception Create<TException>(string message)
        where TException : ExceptionBase
    {
      try
      {
        TException exception = (TException)Activator.CreateInstance(typeof(TException), new object[] { message });
        Trace(exception);
        return exception;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    /// <summary>
    /// Crée une nouvelle exception avec un message formaté
    /// </summary>
    /// <typeparam name="TException">type de l'exception à créer</typeparam>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    public static Exception Create<TException>(string format, params object[] args)
        where TException : ExceptionBase
    {
      try
      {
        TException exception = (TException)Activator.CreateInstance(typeof(TException), new object[] { String.Format(format, args) });
        Trace(exception);
        return exception;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    #endregion

    #region Methodes Replace

    /// <summary>
    /// Remplace une exception par une ExceptionBase
    /// </summary>
    /// <typeparam name="TException">type de l'exception</typeparam>
    /// <param name="exception">exception à remplacer</param>
    /// <remarks>Le message de la nouvelle exception sera le meme que celui de l'exception fournie</remarks>
    public static Exception Replace<TException>(Exception exception)
        where TException : ExceptionBase
    {
      try
      {
        if (exception == null)
          return Wrap<FWTException>(new ArgumentNullException("exception"));

        TException newException = (TException)Activator.CreateInstance(typeof(TException), new object[] { exception.Message });
        Trace(newException);
        return newException;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    /// <summary>
    /// Remplace une exception par une ExceptionBase
    /// </summary>
    /// <typeparam name="TException">type de l'exception</typeparam>
    /// <param name="exception">exception à remplacer</param>
    /// <param name="message">message de l'exception</param>
    public static Exception Replace<TException>(Exception exception, string message)
        where TException : ExceptionBase
    {
      try
      {
        if (exception == null)
          return Wrap<FWTException>(new ArgumentNullException("exception"));

        TException newException = (TException)Activator.CreateInstance(typeof(TException), new object[] { message });
        Trace(newException);
        throw newException;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    /// <summary>
    /// Remplace une exception par une ExceptionBase
    /// </summary>
    /// <typeparam name="TException">type de l'exception</typeparam>
    /// <param name="exception">exception à remplacer</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    public static Exception Replace<TException>(Exception exception, string format, params object[] args)
        where TException : ExceptionBase
    {
      try
      {
        if (exception == null)
          return Wrap<FWTException>(new ArgumentNullException("exception"));

        TException newException = (TException)Activator.CreateInstance(typeof(TException), new object[] { String.Format(format, args) });
        Trace(newException);
        return newException;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    #endregion

    #region Methodes Wrap

    /// <summary>
    /// Encapsule une exception dans une autre exception
    /// </summary>
    /// <typeparam name="TException">type de l'exception à créer</typeparam>
    /// <param name="innerException">exception à encapsuler</param>
    /// <remarks>Le message de la nouvelle exception sera le meme que celui de l'exception fournie</remarks>
    public static Exception Wrap<TException>(Exception innerException)
        where TException : ExceptionBase
    {
      try
      {
        if (innerException == null)
          return Wrap<FWTException>(new ArgumentNullException("innerException"));

        TException exception = (TException)Activator.CreateInstance(typeof(TException), new object[] { innerException.Message, innerException });
        Trace(exception);
        return exception;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    /// <summary>
    /// Encapsule une exception dans une autre exception
    /// </summary>
    /// <typeparam name="TException">type de l'exception à créer</typeparam>
    /// <param name="innerException">exception à encapsuler</param>
    /// <param name="message">message de l'exception à créer</param>
    public static Exception Wrap<TException>(Exception innerException, string message)
        where TException : ExceptionBase
    {
      try
      {
        if (innerException == null)
          return Wrap<FWTException>(new ArgumentNullException("innerException"));

        TException exception = (TException)Activator.CreateInstance(typeof(TException), new object[] { message, innerException });
        Trace(exception);
        throw exception;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    /// <summary>
    /// Encapsule une exception dans une autre exception
    /// </summary>
    /// <typeparam name="TException">type de l'exception à créer</typeparam>
    /// <param name="innerException">exception à encapsuler</param>
    /// <param name="format">format du message</param>
    /// <param name="args">parametres du message</param>
    public static Exception Wrap<TException>(Exception innerException, string format, params object[] args)
        where TException : ExceptionBase
    {
      try
      {
        if (innerException == null)
          return Wrap<FWTException>(new ArgumentNullException("innerException"));

        TException exception = (TException)Activator.CreateInstance(typeof(TException), new object[] { String.Format(format, args), innerException });
        Trace(exception);
        return exception;
      }
      catch (ExceptionBase ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        return Wrap<FWTException>(ex);
      }
    }

    #endregion

    #region Methodes privées

    /// <summary>
    /// Trace l'exception fournie
    /// </summary>
    /// <param name="exception">exception à tracer</param>
    private static void Trace(Exception exception)
    {
      try
      {
        if (exception == null)
          throw new ArgumentNullException("exception");

        TraceManager.TraceError(exception, exception.Message);
      }
      catch
      {
        throw;
      }
    }

    #endregion
  }
}
