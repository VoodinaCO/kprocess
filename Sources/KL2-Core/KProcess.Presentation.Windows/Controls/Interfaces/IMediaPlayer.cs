namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Describes a media player.
    /// </summary>
    public interface IMediaPlayer
    {
        /// <summary>
        /// Plays the media.
        /// </summary>
        void Play();

        /// <summary>
        /// Plays the media.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stops the player.
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Obtient une valeur indiquant si la lecture est en cours.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// Moves to a step forward.
        /// </summary>
        void StepForward();

        /// <summary>
        /// Moves to a step backward.
        /// </summary>
        void StepBackward();

        /// <summary>
        /// Gets or sets the duration used when moving by steps.
        /// </summary>
        long MoveStepDuration { get; set; }

        /// <summary>
        /// Permet de savoir si le player a été chargé.
        /// </summary>
        bool IsLoaded { get; }
    }
}
