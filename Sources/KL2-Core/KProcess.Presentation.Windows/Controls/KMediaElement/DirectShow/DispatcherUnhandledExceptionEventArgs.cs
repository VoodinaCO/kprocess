using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls.DirectShow
{
    /// <summary>
    /// Contient les données de l'évènement DispatcherUnhandledException.
    /// </summary>
    public class DispatcherUnhandledExceptionEventArgs : EventArgs
    {
        private Exception _exception;
        private bool _handled;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DispatcherUnhandledExceptionEventArgs"/>.
        /// </summary>
        /// <param name="exception">L'exception.</param>
        /// <param name="handled">l'état initial</param>
        internal DispatcherUnhandledExceptionEventArgs(Exception exception, bool handled)
        {
            _exception = exception;
            _handled = handled;
        }

        /// <summary>
        /// Obtient l'exception.
        /// </summary>
        public Exception Exception
        {
            get { return this._exception; }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'exception a été gérée.
        /// </summary>
        public bool Handled
        {
            get { return this._handled; }
            set
            {
                if (value)
                {
                    this._handled = value;
                }
            }
        }
    }



}
