using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Demande une action spécifique sur le lecteur multimédia.
    /// </summary>
    public class MediaPlayerActionEvent : EventBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="action">L'action à exécuter.</param>
        public MediaPlayerActionEvent(object sender, MediaPlayerAction action)
            :base(sender)
        {
            Action = action;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="actionWhenPlayerDispatcherReady">le délégué à exécuter lorsque le dispatcher du player est prêt.</param>
        public MediaPlayerActionEvent(object sender, Action actionWhenPlayerDispatcherReady)
            :base(sender)
        {
            Action = MediaPlayerAction.ExecuteWhenDispatcherReady;
            ActionWhenPlayerDispatcherReady = actionWhenPlayerDispatcherReady;
            SendWhenPlayerDispatcherReady = true;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        /// <param name="playerActionWhenPlayerDispatcherReady">le délégué à exécuter lorsque le dispatcher du player est prêt.</param>
        public MediaPlayerActionEvent(object sender, Action<IMediaPlayer> playerActionWhenPlayerDispatcherReady)
            : base(sender)
        {
            Action = MediaPlayerAction.ExecuteOnPlayerWhenDispatcherReady;
            ActionOnPlayerWhenPlayerDispatcherReady = playerActionWhenPlayerDispatcherReady;
            SendWhenPlayerDispatcherReady = true;
        }

        /// <summary>
        /// Obtient l'action à exécuter.
        /// </summary>
        public MediaPlayerAction Action { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// Obtient ou définit le début lors de l'utilisation de l'action ShowTimeline.
        /// </summary>
        public long ShowTimelineStart { get; set; }

        /// <summary>
        /// Obtient ou définit la fin lors de l'utilisation de l'action ShowTimeline.
        /// </summary>
        public long ShowTimelineEnd { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action doit être exécutée lorsque le dispatcher du player est prêt.
        /// </summary>
        public bool SendWhenPlayerDispatcherReady { get; set; }

        /// <summary>
        /// Obtient le délégué à exécuter lorsque le dispatcher du player est prêt.
        /// Fonctionne avec <see cref="MediaPlayerAction"/>.ExecuteWhenDispatcherReady.
        /// </summary>
        public Action ActionWhenPlayerDispatcherReady { get; private set; }

        /// <summary>
        /// Obtient le délégué à exécuter lorsque le dispatcher du player est prêt.
        /// Fonctionne avec <see cref="MediaPlayerAction"/>.ExecuteWhenDispatcherReady.
        /// </summary>
        public Action<IMediaPlayer> ActionOnPlayerWhenPlayerDispatcherReady { get; private set; }
    }

    /// <summary>
    /// Une action sur le lecteur multimédia.
    /// </summary>
    public enum MediaPlayerAction
    {
        Play,
        Pause,
        PlayPause,
        Stop,
        Mute,
        Unmute,
        ToggleMute,
        StepBackward,
        StepForward,
        ShowTimeline,
        ResetTimeline,
        ActivateMediaPlayerService,
        UnactivateMediaPlayerService,
        ExecuteWhenDispatcherReady,
        ExecuteOnPlayerWhenDispatcherReady,
        ToggleScreenMode,
        ShowAnnotations,
        HideAnnotations
    }
}
