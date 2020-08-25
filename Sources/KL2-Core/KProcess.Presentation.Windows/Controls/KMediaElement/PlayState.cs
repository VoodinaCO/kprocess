using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// L'état de lecture.
    /// </summary>
    public enum PlayState
    {
        /// <summary>
        /// En cours de lecture.
        /// </summary>
        Playing,

        /// <summary>
        /// Mis en pause.
        /// </summary>
        Paused,

        /// <summary>
        /// Arrêté.
        /// </summary>
        Stopped
    }
}
