using System;

namespace KProcess
{
    /// <summary>
    /// Take all exceptions from application
    /// </summary>
    public interface IExceptionHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="showMessage"></param>
        /// <param name="msgTitle"></param>
        void HandleException(Exception ex, bool showMessage = true, string msgTitle = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="showMessage"></param>
        /// <param name="msgTitle"></param>
        void HandleFatalException(Exception ex, bool showMessage = true, string msgTitle = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="showMessage"></param>
        /// <param name="msgTitle"></param>
        void HandleTaskException(Exception ex, bool showMessage = true, string msgTitle = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        void RaiseTaskException(Exception ex);
    }
}
