using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Defines an action playing a video
    /// </summary>
    internal class PlayVideoAction : IAction
    {
        #region Attributes

        private KMiniPlayer _player;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayVideoAction"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="associatedItem">The associated item.</param>
        public PlayVideoAction(KMiniPlayer player, IActionPath associatedItem)
        {
            _player = player;
            AssociatedItem = associatedItem;
            Start = AssociatedItem.Start;
            End = AssociatedItem.Finish;
            if (this.Duration == 0)
                this.InitialSpeedRatio = 1;
            else
                this.InitialSpeedRatio = (double)(AssociatedItem.VideoFinish - AssociatedItem.VideoStart) / (AssociatedItem.Finish - AssociatedItem.Start);
        }

        #endregion

        #region IAction members

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
        /// Gets the duration.
        /// </summary>
        private long Duration
        {
            get { return End - Start; }
        }

        /// <summary>
        /// Gets the associated item.
        /// </summary>
        public IActionPath AssociatedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Loads the ressources and synchronizes the position.
        /// </summary>
        public void LoadAndSync()
        {
            if (_player.Source != AssociatedItem)
                _player.Source = AssociatedItem;

            _player.MediaElement.Pause();

            _player.MediaElement.MediaStartingPosition = AssociatedItem.VideoStart;
            _player.MediaElement.MediaPosition = AssociatedItem.VideoStart + Convert.ToInt64((_player.Position - AssociatedItem.Start) * this.InitialSpeedRatio);

            TrySetSpeedRatio(_player.GetCurrentActionActualSpeedRatio());
        }

        /// <summary>
        /// Runs the action.
        /// </summary>
        public void Run()
        {
            if (this.Duration != 0)
            {
                var speedRatio = _player.GetCurrentActionActualSpeedRatio();
                if (TrySetSpeedRatio(speedRatio))
                {
                    _player.MediaElement.Play();
                }
                else
                {
                    _player.MediaElement.Stop();
                    _player.Source = null;
                    RaiseCompleted();
                }


                _player.MediaElement.MediaPositionChanged -= MediaElement_MediaPositionChanged;
                _player.MediaElement.MediaEnded -= MediaElement_MediaEnded;

                _player.MediaElement.MediaPositionChanged += MediaElement_MediaPositionChanged;
                _player.MediaElement.MediaEnded += MediaElement_MediaEnded;
            }
        }

        /// <summary>
        /// Freezes the action.
        /// </summary>
        public void Freeze()
        {
            _player.Source = AssociatedItem;
            _player.MediaElement.MediaStartingPosition = AssociatedItem.VideoStart;
            if (this.Duration != 0)
            {
                _player.MediaElement.MediaPosition = AssociatedItem.VideoStart + Convert.ToInt64((_player.Position - AssociatedItem.Start) * this.InitialSpeedRatio);
                _player.MediaElement.Pause();
            }
        }

        /// <summary>
        /// Releases all ressources that have been created by <see cref="Run"/>.
        /// </summary>
        public void Release()
        {
            _player.MediaElement.MediaPositionChanged -= MediaElement_MediaPositionChanged;
            _player.MediaElement.MediaEnded -= MediaElement_MediaEnded;
        }

        /// <summary>
        /// Sets the speed ratio.
        /// </summary>
        /// <param name="speedRatio">The speed ratio.</param>
        public void SetSpeedRatio(double speedRatio)
        {
            if (TrySetSpeedRatio(speedRatio))
            {
                // La vitesse est gérée, lancer la vidéo si elle avait été supprimée
                if (_player.Source != AssociatedItem)
                {
                    if (_player.Source != AssociatedItem)
                        _player.Source = AssociatedItem;

                    _player.MediaElement.MediaStartingPosition = AssociatedItem.VideoStart;
                    _player.MediaElement.MediaPosition = AssociatedItem.VideoStart + Convert.ToInt64((_player.Position - AssociatedItem.Start) * this.InitialSpeedRatio);

                    _player.MediaElement.Play();
                }
            }
            else
            {
                // La vitesse n'est pas gérée, on va donc supprimer la source vidéo
                _player.MediaElement.Stop();
                _player.Source = null;
                RaiseCompleted();
            }
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
        /// Gets or sets the speed ratio that has been originally applied to the video.
        /// </summary>
        public double InitialSpeedRatio { get; private set; }

        /// <summary>
        /// Occurs when the current action has completed.
        /// </summary>
        public event EventHandler Completed;

        private bool TrySetSpeedRatio(double speedRatio)
        {
            if (_player.MediaElement.CanSetSpeedRatio(speedRatio))
            {
                _player.MediaElement.SpeedRatio = speedRatio;
                return true;
            }
            else
                return false;
        }

        private void MediaElement_MediaPositionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_player.IsPlaying && _player.CurrentAction == this)
            {
                var globalCurrentVideoPosition = this.Start + (long)((_player.MediaElement.MediaPosition - AssociatedItem.VideoStart) / this.InitialSpeedRatio);

                if (globalCurrentVideoPosition >= this.End && _player.InternalPosition != AssociatedItem.VideoFinish)
                {
                    MediaCompleted();
                }
                else
                {
                    _player.InternalPosition = globalCurrentVideoPosition;

                    var speedRatio = _player.GetCurrentActionActualSpeedRatio();
                    if (_player.MediaElement.SpeedRatio != speedRatio)
                        this.SetSpeedRatio(speedRatio);
                }
            }
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            MediaCompleted();
        }

        private void MediaCompleted()
        {
            _player.MediaElement.Stop();
            _player.MediaElement.MediaPosition = AssociatedItem.VideoFinish;
            _player.InternalPosition = this.End;
            this.RaiseCompleted();
        }

        private void RaiseCompleted()
        {
            if (this.Completed != null)
                this.Completed(this, EventArgs.Empty);
        }

        #endregion
    }
}
