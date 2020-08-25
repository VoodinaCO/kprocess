using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Défines a blank action or an action with no video
    /// </summary>
    internal class BlankAction : IAction
    {

        /// <summary>
        /// L'intervalle du timer en millisecondes.
        /// </summary>
        public const int TimerIntervalMilliseconds = 10;

        private DispatcherTimer _timeline;
        private KMiniPlayer _player;
        private double _speedRatio = 1d;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BlankAction"/> class.
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public BlankAction(KMiniPlayer player, long start, long end)
            : this()
        {
            _player = player;
            Start = start;
            End = end;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlankAction"/> class.
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="associatedItem">The associated item.</param>
        public BlankAction(KMiniPlayer player, IActionPath associatedItem = null)
            : this()
        {
            _player = player;
            AssociatedItem = associatedItem;
            Start = AssociatedItem.Start;
            End = AssociatedItem.Finish;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlankAction"/> class.
        /// </summary>
        private BlankAction()
        {
            _timeline = new DispatcherTimer(TimeSpan.FromMilliseconds(TimerIntervalMilliseconds), DispatcherPriority.DataBind, OnTimeLineUpdate, Dispatcher.CurrentDispatcher);
            _timeline.IsEnabled = false;
        }

        #endregion

        #region IAction members

        /// <summary>
        /// Gets the associated item.
        /// </summary>
        public IActionPath AssociatedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the start.
        /// </summary>
        public long Start
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the end.
        /// </summary>
        public long End
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the ressources and synchronizes the position.
        /// </summary>
        public void LoadAndSync()
        {
            _timeline.IsEnabled = false;
            _player.Source = null;
        }

        /// <summary>
        /// Runs the action.
        /// </summary>
        public void Run()
        {
            _timeline.IsEnabled = false;
            _player.Source = null;
            _timeline.IsEnabled = true;
        }

        /// <summary>
        /// Freezes the action.
        /// </summary>
        public void Freeze()
        {
            _timeline.IsEnabled = false;
            _player.Source = null;
        }

        /// <summary>
        /// Releases all ressources that have been created by <see cref="Run"/>.
        /// </summary>
        public void Release()
        {
            _timeline.IsEnabled = false;
        }

        /// <summary>
        /// Sets the speed ratio.
        /// </summary>
        /// <param name="speedRatio">The speed ratio.</param>
        public void SetSpeedRatio(double speedRatio)
        {
        }

        /// <summary>
        /// Indicates wheter the action is the last one.
        /// </summary>
        public bool IsLast
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the speed ratio that has been originally applied to the video.
        /// </summary>
        public double InitialSpeedRatio
        {
            get { return 1; }
        }

        /// <summary>
        /// Occurs when the current action has completed.
        /// </summary>
        public event EventHandler Completed;

        #endregion

        private void OnTimeLineUpdate(object sender, EventArgs e)
        {
            if (_player != null && _player.IsPlaying && _player.CurrentAction == this)
            {
                var globalPosition = _player.InternalPosition + Convert.ToInt64(_timeline.Interval.Ticks * _player.SpeedRatio);
                
                if (globalPosition > End)
                {
                    _timeline.IsEnabled = false;
                    _player.InternalPosition = End;
                    RaiseCompleted();
                }
                else
                {
                    _player.InternalPosition += Convert.ToInt64(_timeline.Interval.Ticks * _player.SpeedRatio);

                    var speedRatio = _player.GetCurrentActionActualSpeedRatio();
                    _speedRatio = speedRatio;
                }
            }
        }

        private void RaiseCompleted()
        {
            if (this.Completed != null)
                this.Completed(this, EventArgs.Empty);
        }

    }
}
