using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Defines a KMiniPlayer internal action
    /// </summary>
    internal interface IAction
    {
        /// <summary>
        /// Gets the associated item.
        /// </summary>
        IActionPath AssociatedItem { get; }

        /// <summary>
        /// Gets the start.
        /// </summary>
        long Start { get; }

        /// <summary>
        /// Gets the end.
        /// </summary>
        long End { get; }

        /// <summary>
        /// Loads the ressources and synchronizes the position.
        /// </summary>
        void LoadAndSync();

        /// <summary>
        /// Runs the action.
        /// </summary>
        void Run();

        /// <summary>
        /// Freezes the action.
        /// </summary>
        void Freeze();

        /// <summary>
        /// Releases all ressources that have been created by <see cref="Run"/>.
        /// </summary>
        void Release();

        /// <summary>
        /// Sets the speed ratio.
        /// </summary>
        /// 
        /// <param name="speedRatio">The speed ratio.</param>
        void SetSpeedRatio(double speedRatio);

        /// <summary>
        /// Indicates wheter the action is the last one.
        /// </summary>
        bool IsLast { get; set; }

        /// <summary>
        /// Gets the speed ratio that has been originally applied to the video.
        /// </summary>
        double InitialSpeedRatio { get; }

        /// <summary>
        /// Occurs when the current action has completed.
        /// </summary>
        event EventHandler Completed;
    }
}
