//#define DEBUG_QUEUEING
using AnnotationsLib;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet de contrôler le media player associé à l'aide d'évènements.
    /// </summary>
    public class MediaPlayerBehavior : Behavior<FrameworkElement>, IMediaPlayerService
    {
        private Queue<MediaPlayerActionEvent> WaitingActions = new Queue<MediaPlayerActionEvent>();

        /// <summary>
        /// Obtient le MediaPlayer générique.
        /// </summary>
        private IMediaPlayer MediaPlayer
        {
            get { return (IMediaPlayer)AssociatedObject; }
        }

        /// <summary>
        /// Obtient le FrameworkElement générique.
        /// </summary>
        private FrameworkElement FrameworkElement
        {
            get { return AssociatedObject as FrameworkElement; }
        }

        /// <summary>
        /// Obtient le KMediaPlayer spécifique.
        /// </summary>
        private KMediaPlayer KMediaPlayer
        {
            get { return AssociatedObject as KMediaPlayer; }
        }

        /// <summary>
        /// Obtient le KMiniPlayer spécifique.
        /// </summary>
        private KMiniPlayer KMiniPlayer
        {
            get { return AssociatedObject as KMiniPlayer; }
        }

#if DEBUG_QUEUEING
        int counterId = 0;
#endif

        /// <summary>
        /// Appelé une fois que le comportement est attaché à un AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            if (!(AssociatedObject is IMediaPlayer))
                throw new InvalidOperationException("Ce behavior ne peut être attaché qu'à un IMediaPlayer");

            FrameworkElement.Loaded += OnMediaPlayerLoaded;

            if (KMediaPlayer != null)
            {
                KMediaPlayer.SpeedRatioChanged += new RoutedPropertyChangedEventHandler<double>(OnPlayerSpeedRatioChanged);
                KMediaPlayer.MediaOpened += new RoutedEventHandler(OnPlayerMediaOpened);
            }
            else if (KMiniPlayer != null)
            {
                KMiniPlayer.SpeedRatioChanged += new RoutedPropertyChangedEventHandler<double>(OnPlayerSpeedRatioChanged);
                KMiniPlayer.CurrentActionChanged += new RoutedEventHandler(OnMiniPlayerCurrentActionChanged);
            }

            var timeService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();
            MediaPlayer.MoveStepDuration = timeService.CurrentTimeScale;
            if (KMediaPlayer != null)
            {
                KMediaPlayer.TimeToStringFormatter = timeService;
            }

            IoC.Resolve<IEventBus>().Subscribe<MediaPlayerActionEvent>(e =>
            {
                if (e.Action == MediaPlayerAction.ActivateMediaPlayerService || e.Action == MediaPlayerAction.UnactivateMediaPlayerService)
                    OnMediaPlayerActionEvent(e);
                else if (MediaPlayer.IsLoaded && WaitingActions.Count == 0)
                {
#if DEBUG_QUEUEING
                    e.Id = counterId++;
                    TraceManager.TraceDebug($"Demand : {e.Id} - {e.Action}");
#endif
                    OnMediaPlayerActionEvent(e);
                }
                else
                {
#if DEBUG_QUEUEING
                    e.Id = counterId++;
                    TraceManager.TraceDebug($"Demand : {e.Id} - {e.Action}");
#endif
                    WaitingActions.Enqueue(e);
                }
            });
        }

        private void OnMediaPlayerLoaded(object sender, RoutedEventArgs e)
        {
            while (WaitingActions.Count > 0)
                OnMediaPlayerActionEvent(WaitingActions.Dequeue());
        }

        private void OnMediaPlayerActionEvent(MediaPlayerActionEvent e)
        {
            if (e.Sender == AssociatedObject.DataContext || e.Sender == AssociatedObject)
            {
#if DEBUG_QUEUEING
                TraceManager.TraceDebug($"Execute : {e.Id} - {e.Action}");
#endif
                Action action = null;

                switch (e.Action)
                {
                    case MediaPlayerAction.Play:
                        action = () => MediaPlayer.Play();
                        break;

                    case MediaPlayerAction.Pause:
                        action = () => MediaPlayer.Pause();
                        break;

                    case MediaPlayerAction.PlayPause:
                        if (MediaPlayer.IsPlaying)
                            action = () => MediaPlayer.Pause();
                        else
                            action = () => MediaPlayer.Play();
                        break;

                    case MediaPlayerAction.Stop:
                        action = () => MediaPlayer.Stop();
                        break;

                    case MediaPlayerAction.Mute:
                        if (KMediaPlayer != null)
                            action = () => KMediaPlayer.Mute();
                        break;

                    case MediaPlayerAction.Unmute:
                        if (KMediaPlayer != null)
                            action = () => KMediaPlayer.Unmute();
                        break;

                    case MediaPlayerAction.ToggleMute:
                        if (KMediaPlayer != null)
                            action = () => KMediaPlayer.ToggleMute();
                        break;

                    case MediaPlayerAction.StepBackward:
                        action = () => MediaPlayer.StepBackward();
                        break;

                    case MediaPlayerAction.StepForward:
                        action = () => MediaPlayer.StepForward();
                        break;

                    case MediaPlayerAction.ShowTimeline:
                        if (KMediaPlayer != null)
                            action = () => KMediaPlayer.TimelineShow(e.ShowTimelineStart, e.ShowTimelineEnd, true);
                        break;

                    case MediaPlayerAction.ResetTimeline:
                        if (KMediaPlayer != null)
                            action = () => KMediaPlayer.TimelineReset();
                        break;

                    case MediaPlayerAction.ActivateMediaPlayerService:
                        action = () => IoC.Resolve<IServiceBus>().Register<IMediaPlayerService>(this);
                        break;

                    case MediaPlayerAction.UnactivateMediaPlayerService:
                        action = () => IoC.Resolve<IServiceBus>().Unregister<IMediaPlayerService>();
                        break;

                    case MediaPlayerAction.ExecuteWhenDispatcherReady:
                        action = e.ActionWhenPlayerDispatcherReady;
                        break;

                    case MediaPlayerAction.ExecuteOnPlayerWhenDispatcherReady:
                        action = () => e.ActionOnPlayerWhenPlayerDispatcherReady(MediaPlayer);
                        break;

                    case MediaPlayerAction.ToggleScreenMode:
                        action = () =>
                        {
                            if (KMediaPlayer.ScreenModeCommand != null && KMediaPlayer.ScreenModeCommand.CanExecute(null))
                                KMediaPlayer.ScreenModeCommand.Execute(null);
                        };
                        break;

                    case MediaPlayerAction.ShowAnnotations:
                        action = () =>
                        {
                            AnnotationsAdornment.SetAnnotationsIsVisible(KMediaPlayer.MediaElement.VideoImage, true);
                            AnnotationsAdornment.SetIsInEditMode(KMediaPlayer.MediaElement.VideoImage, true);
                        };
                        break;

                    case MediaPlayerAction.HideAnnotations:
                        action = () =>
                        {
                            if (AnnotationsAdornment.GetIsVisible(KMediaPlayer.MediaElement.VideoImage))
                                AnnotationsAdornment.DestroyAnnotations(KMediaPlayer.MediaElement.VideoImage);
                        };
                        break;

                    default:
                        break;
                }

                if (action != null)
                {
                    if (e.SendWhenPlayerDispatcherReady)
                    {
                        Action<Action> onDispatcherAction = (a) =>
                        {
                            if (MediaPlayer is KMediaPlayer)
                            {
                                if (KMediaPlayer.MediaElement != null)
                                    KMediaPlayer.MediaElement.BeginInvokeOnMediaPlayerDispatcher((Action)delegate
                                    {
                                        Dispatcher.Invoke(a);
                                    });
                                else
                                    a();
                            }
                            else if (MediaPlayer is KMiniPlayer)
                            {
                                if (KMiniPlayer.MediaElement != null)
                                    KMiniPlayer.MediaElement.BeginInvokeOnMediaPlayerDispatcher((Action)delegate
                                    {
                                        Dispatcher.Invoke(a);
                                    });
                                else
                                    a();
                            }
                        };

                        if (MediaPlayer is KMediaPlayer)
                        {
                            if (KMediaPlayer.MediaElement == null)
                                Dispatcher.BeginInvoke(onDispatcherAction, System.Windows.Threading.DispatcherPriority.Loaded, action);
                            else
                                onDispatcherAction(action);
                        }
                        else if (MediaPlayer is KMiniPlayer)
                        {
                            if (KMiniPlayer.MediaElement == null)
                                Dispatcher.BeginInvoke(onDispatcherAction, System.Windows.Threading.DispatcherPriority.Loaded, action);
                            else
                                onDispatcherAction(action);
                        }
                    }
                    else
                        action();
                }
            }
        }

        /// <summary>
        /// Appelé lorsque le comportement est détaché de son AssociatedObject, mais avant qu'il ne se soit produit réellement.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            FrameworkElement.Loaded -= OnMediaPlayerLoaded;

            IoC.Resolve<IEventBus>().Unsubscribe(this);

            if (KMediaPlayer != null)
            {
                KMediaPlayer.SpeedRatioChanged -= new RoutedPropertyChangedEventHandler<double>(OnPlayerSpeedRatioChanged);
                KMediaPlayer.MediaOpened -= new RoutedEventHandler(OnPlayerMediaOpened);
            }
            else if (KMiniPlayer != null)
            {
                KMiniPlayer.SpeedRatioChanged -= new RoutedPropertyChangedEventHandler<double>(OnPlayerSpeedRatioChanged);
                KMiniPlayer.CurrentActionChanged -= new RoutedEventHandler(OnMiniPlayerCurrentActionChanged);
            }
        }

        #region IMediaPlayerService Members

        /// <summary>
        /// Obtient le format de la vidéo en cours de lecture.
        /// </summary>
        /// <returns>
        /// Le format ou null s'il est inaccessible ou si aucune vidéo n'est chargée.
        /// </returns>
        public MediaInfo GetFormat()
        {
            if (KMediaPlayer != null)
            {
                KMediaPlayer.UpdateMediaInfo();
                return KMediaPlayer.CurrentMediaInfo;
            }
            else
                return null;
        }

        /// <summary>
        /// Obtient la durée de la vidéo en cours de lecture.
        /// </summary>
        /// <returns>
        /// la durée ou null si aucune vidéo n'est chargée.
        /// </returns>
        public TimeSpan? GetDuration()
        {
            if (KMediaPlayer != null)
                return KMediaPlayer.Source != null && KMediaPlayer.MediaElement != null ? KMediaPlayer.MediaElement.Duration : (TimeSpan?)null;
            else
                return null;
        }

        /// <summary>
        /// Obtient une capture de la vidéo en cours sans annotations
        /// </summary>
        /// <param name="stream"></param>
        public void GetThumbnail(Stream stream)
        {
            var pixelWidth = ((D3DImage)KMediaPlayer.MediaElement.VideoImage.Source).PixelWidth;
            var pixelHeight = ((D3DImage)KMediaPlayer.MediaElement.VideoImage.Source).PixelHeight;

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder { QualityLevel = 90 };
            RenderTargetBitmap renderTarget = new RenderTargetBitmap(pixelWidth, pixelHeight, 96, 96, PixelFormats.Pbgra32);

            VisualBrush sourceBrush = new VisualBrush(KMediaPlayer.MediaElement.VideoImage);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.PushTransform((Transform)KMediaPlayer.MediaElement.VideoImage.LayoutTransform.Inverse);
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(pixelWidth, pixelHeight)));
            }
            renderTarget.Render(drawingVisual);
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
            jpgEncoder.Save(stream);
        }

        /// <summary>
        /// Obtient une capture de la vidéo en cours avec des annotations
        /// </summary>
        /// <param name="stream"></param>
        public void GetThumbnailWithAnnotations(Stream stream) => AnnotationsAdornment.GetSaveCommand(KMediaPlayer.MediaElement.VideoImage)?.Execute(stream);

        public void ResetAnnotations() =>
            AnnotationsAdornment.GetClearAnnotationsCommand(KMediaPlayer.MediaElement.VideoImage)?.Execute(null);

        public bool IsPlaying() => KMediaPlayer.IsPlaying;

        #endregion

        #region Gestion du speed ratio

        /// <summary>
        /// Appelé lorsqu'un fichier a été ouvert sur le player.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnPlayerMediaOpened(object sender, RoutedEventArgs e)
        {
            var service = IoC.Resolve<IServiceBus>().Get<IVideoSpeedRatioPersistanceService>();

            double speedRatio;

            if (this.KMediaPlayer != null)
            {
                if (this.KMediaPlayer.Source != null)
                    speedRatio = service.GetSpeedRatio(this.KMediaPlayer.Source) ?? 1.0d;
                else
                    speedRatio = 1.0d;

                var mediaElement = (KMediaElement)e.OriginalSource;
                mediaElement.SpeedRatio = speedRatio;
            }
        }

        /// <summary>
        /// Appelé lorsque l'action courante du mini player a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnMiniPlayerCurrentActionChanged(object sender, RoutedEventArgs e)
        {
            if (this.KMiniPlayer != null)
            {
                var service = IoC.Resolve<IServiceBus>().Get<IVideoSpeedRatioPersistanceService>();
                double speedRatio;
                if (this.KMiniPlayer.MediaElement != null && this.KMiniPlayer.MediaElement.Source != null)
                    speedRatio = service.GetSpeedRatio(this.KMiniPlayer.MediaElement.Source) ?? 1.0d;
                else
                    speedRatio = 1.0d;

                this.KMiniPlayer.SpeedRatio = speedRatio;
            }
        }

        /// <summary>
        /// Appelé lorsque le coefficient de vitesse a changé sur le player.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.RoutedPropertyChangedEventArgs&lt;System.Double&gt;"/> contenant les données de l'évènement.</param>
        private void OnPlayerSpeedRatioChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var service = IoC.Resolve<IServiceBus>().Get<IVideoSpeedRatioPersistanceService>();

            Uri source = null;

            if (this.KMediaPlayer != null)
                source = this.KMediaPlayer.Source;
            else if (this.KMiniPlayer != null && this.KMiniPlayer.MediaElement != null)
                source = this.KMiniPlayer.MediaElement.Source;

            if (source != null)
                service.PersistSpeedRatio(source, e.NewValue);
        }

        #endregion
    }
}
