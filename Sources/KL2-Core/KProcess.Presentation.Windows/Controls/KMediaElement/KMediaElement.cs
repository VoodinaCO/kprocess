using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KProcess.Presentation.Windows.Controls.DirectShow;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls
{
    public class KMediaElement : FrameworkElement
    {
        #region Attributes

        /// <summary>
        /// The D3DImage used to render video
        /// </summary>
        private D3DImage _d3dImage;

        /// <summary>
        /// The Image control that has the source
        /// to the D3DImage
        /// </summary>
        private Image _videoImage;

        /// <summary>
        /// We keep reference to the D3D surface so
        /// we can delay loading it to avoid a black flicker
        /// when loading new media
        /// </summary>
        private IntPtr _backBuffer = IntPtr.Zero;

        /// <summary>
        /// Flag to tell us if we have a new D3D
        /// Surface available
        /// </summary>
        private bool _newSurfaceAvailable;

        /// <summary>
        /// A weak reference of D3DRenderers that have been cloned
        /// </summary>
        private readonly List<WeakReference> _clonedD3Drenderers = new List<WeakReference>();

        /// <summary>
        /// Backing field for the RenderOnCompositionTargetRendering flag. 
        /// </summary>
        private bool _renderOnCompositionTargetRendering;

        /// <summary>
        /// Temporary storage for the RenderOnCompositionTargetRendering flag.
        /// This is used to remember the value for when the control is loaded and unloaded.
        /// </summary>
        private bool _renderOnCompositionTargetRenderingTemp;

        /// <summary>
        /// This flag is used to ignore PropertyChangedCallbacks
        /// for when a DependencyProperty is needs to be updated
        /// from the media player thread
        /// </summary>
        private bool _ignorePropertyChangedCallback;

        private Window _currentWindow;
        private bool _windowHooked;
        private FiltersConfiguration _filtersConfig;

        #endregion

        #region DPs

        #region Stretch
        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register("Stretch", typeof(Stretch), typeof(KMediaElement),
                new FrameworkPropertyMetadata(Stretch.Uniform,
                    new PropertyChangedCallback(OnStretchChanged)));

        /// <summary>
        /// Defines what rules are applied to the stretching of the video
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnStretchChanged(e);
        }

        private void OnStretchChanged(DependencyPropertyChangedEventArgs e)
        {
            _videoImage.Stretch = (Stretch)e.NewValue;
        }
        #endregion

        #region StretchDirection

        public static readonly DependencyProperty StretchDirectionProperty =
            DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(KMediaElement),
                new FrameworkPropertyMetadata(StretchDirection.Both,
                    new PropertyChangedCallback(OnStretchDirectionChanged)));

        /// <summary>
        /// Gets or Sets the value that indicates how the video is scaled.  This is a dependency property.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get { return (StretchDirection)GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnStretchDirectionChanged(e);
        }

        protected virtual void OnStretchDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            _videoImage.StretchDirection = (StretchDirection)e.NewValue;
        }

        #endregion

        #region IsRenderingEnabled

        public static readonly DependencyProperty IsRenderingEnabledProperty =
            DependencyProperty.Register("IsRenderingEnabled", typeof(bool), typeof(KMediaElement),
                new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Enables or disables rendering of the video
        /// </summary>
        public bool IsRenderingEnabled
        {
            get { return (bool)GetValue(IsRenderingEnabledProperty); }
            set { SetValue(IsRenderingEnabledProperty, value); }
        }

        #endregion

        #region NaturalVideoHeight

        private static readonly DependencyPropertyKey NaturalVideoHeightPropertyKey
            = DependencyProperty.RegisterReadOnly("NaturalVideoHeight", typeof(int), typeof(KMediaElement),
                new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty NaturalVideoHeightProperty
            = NaturalVideoHeightPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the natural pixel height of the current media.  
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoHeight
        {
            get { return (int)GetValue(NaturalVideoHeightProperty); }
        }

        /// <summary>
        /// Internal method to set the read-only NaturalVideoHeight DP
        /// </summary>
        protected void SetNaturalVideoHeight(int value)
        {
            SetValue(NaturalVideoHeightPropertyKey, value);
        }

        #endregion

        #region NaturalVideoWidth

        private static readonly DependencyPropertyKey NaturalVideoWidthPropertyKey
            = DependencyProperty.RegisterReadOnly("NaturalVideoWidth", typeof(int), typeof(KMediaElement),
                new FrameworkPropertyMetadata(0));

        public static readonly DependencyProperty NaturalVideoWidthProperty
            = NaturalVideoWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the natural pixel width of the current media.
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoWidth
        {
            get { return (int)GetValue(NaturalVideoWidthProperty); }
        }

        /// <summary>
        /// Internal method to set the read-only NaturalVideoWidth DP
        /// </summary>
        protected void SetNaturalVideoWidth(int value)
        {
            SetValue(NaturalVideoWidthPropertyKey, value);
        }

        #endregion

        #region HasVideo

        private static readonly DependencyPropertyKey HasVideoPropertyKey
            = DependencyProperty.RegisterReadOnly("HasVideo", typeof(bool), typeof(KMediaElement),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasVideoProperty
            = HasVideoPropertyKey.DependencyProperty;

        /// <summary>
        /// Is true if the media contains renderable video
        /// </summary>
        public bool HasVideo
        {
            get { return (bool)GetValue(HasVideoProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only HasVideo DP
        /// </summary>
        protected void SetHasVideo(bool value)
        {
            SetValue(HasVideoPropertyKey, value);
        }
        #endregion

        #region IsLoading

        private static readonly DependencyPropertyKey IsLoadingPropertyKey
            = DependencyProperty.RegisterReadOnly("IsLoading", typeof(bool), typeof(KMediaElement),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsLoadingProperty
            = IsLoadingPropertyKey.DependencyProperty;

        /// <summary>
        /// Is true if the player is loading
        /// </summary>
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
        }

        /// <summary>
        /// Internal method for setting the read-only IsLoading DP
        /// </summary>
        protected void SetIsLoading(bool value)
        {
            SetValue(IsLoadingPropertyKey, value);
        }
        #endregion

        #region DeepColor

        public static readonly DependencyProperty DeepColorProperty =
            DependencyProperty.Register("DeepColor", typeof(bool), typeof(KMediaElement),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnDeepColorChanged)));

        public bool DeepColor
        {
            get { return (bool)GetValue(DeepColorProperty); }
            set { SetValue(DeepColorProperty, value); }
        }

        private static void OnDeepColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnDeepColorChanged(e);
        }

        protected virtual void OnDeepColorChanged(DependencyPropertyChangedEventArgs e)
        {
            ToggleDeepColorEffect((bool)e.NewValue);
        }

        #endregion

        #region VideoRenderer

        public static readonly DependencyProperty VideoRendererProperty =
            DependencyProperty.Register("VideoRenderer", typeof(VideoRendererType), typeof(KMediaElement),
                new FrameworkPropertyMetadata(VideoRendererType.VideoMixingRenderer9,
                    new PropertyChangedCallback(OnVideoRendererChanged)));

        public VideoRendererType VideoRenderer
        {
            get { return (VideoRendererType)GetValue(VideoRendererProperty); }
            set { SetValue(VideoRendererProperty, value); }
        }

        private static void OnVideoRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnVideoRendererChanged(e);
        }

        protected void OnVideoRendererChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetVideoRenderer();
        }

        void PlayerSetVideoRenderer()
        {
            VideoRendererType videoRendererType = VideoRendererType.EnhancedVideoRenderer;
            try
            {
                videoRendererType = (VideoRendererType)Enum.Parse(typeof(VideoRendererType), System.Configuration.ConfigurationManager.AppSettings["VideoRenderer"]);
            }
            catch { }
            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.VideoRenderer = videoRendererType;
            });
        }

        #endregion

        #region AudioRenderer

        public static readonly DependencyProperty AudioRendererProperty =
            DependencyProperty.Register("AudioRenderer", typeof(string), typeof(KMediaElement),
                new FrameworkPropertyMetadata("Default DirectSound Device",
                    new PropertyChangedCallback(OnAudioRendererChanged)));

        /// <summary>
        /// The name of the audio renderer device to use
        /// </summary>
        public string AudioRenderer
        {
            get { return (string)GetValue(AudioRendererProperty); }
            set { SetValue(AudioRendererProperty, value); }
        }

        private static void OnAudioRendererChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnAudioRendererChanged(e);
        }

        protected void OnAudioRendererChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetAudioRenderer();
        }

        private void PlayerSetAudioRenderer()
        {
            var audioDevice = AudioRenderer;

            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                /* Sets the audio device to use with the player */
                MediaPlayer.AudioRenderer = audioDevice;
            });
        }

        #endregion

        #region Source

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(KMediaElement),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnSourceChanged)));

        /// <summary>
        /// The Uri source to the media.  This can be a file path or a
        /// URL source
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnSourceChanged(e);
        }

        protected void OnSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetSource();
        }

        private void PlayerSetSource()
        {
            var source = Source;
            var rendererType = VideoRenderer;

            if (MediaPlayer.Source != source)
                SetIsMediaOpened(false);

            if (DesignMode.IsInDesignMode)
                return;

            this.MediaPosition = 0;

            base.CoerceValue(SpeedRatioProperty);

            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.Source = source;

                Dispatcher.BeginInvoke((Action)delegate
                {
                    if (IsLoaded)
                    {
                        ExecuteMediaState(LoadedBehavior);
                    }
                    //else
                    //    ExecuteMediaState(UnloadedBehavior);
                });
            });
        }
        #endregion

        #region Loop

        public static readonly DependencyProperty LoopProperty =
            DependencyProperty.Register("Loop", typeof(bool), typeof(KMediaElement),
                new FrameworkPropertyMetadata(false,
                    new PropertyChangedCallback(OnLoopChanged)));

        /// <summary>
        /// Gets or sets whether the media should return to the begining
        /// once the end has reached
        /// </summary>
        public bool Loop
        {
            get { return (bool)GetValue(LoopProperty); }
            set { SetValue(LoopProperty, value); }
        }

        private static void OnLoopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnLoopChanged(e);
        }

        protected void OnLoopChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetLoop();
        }

        private void PlayerSetLoop()
        {
            var loop = Loop;
            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.Loop = loop;
            });
        }
        #endregion

        #region MediaStartingPosition

        public long MediaStartingPosition
        {
            get { return (long)GetValue(MediaStartingPositionProperty); }
            set { SetValue(MediaStartingPositionProperty, value); }
        }

        public static readonly DependencyProperty MediaStartingPositionProperty =
            DependencyProperty.Register("MediaStartingPosition", typeof(long), typeof(KMediaElement), new UIPropertyMetadata(0L, OnMediaStartingPositionChanged));

        private static void OnMediaStartingPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnMediaStartingPositionChanged((long)e.NewValue);
        }

        private void OnMediaStartingPositionChanged(long position)
        {
            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate { MediaPlayer.MediaStartingPosition = position; });
        }

        #endregion

        #region MediaEndingPosition

        public long MediaEndingPosition
        {
            get { return (long)GetValue(MediaEndingPositionProperty); }
            set { SetValue(MediaEndingPositionProperty, value); }
        }

        public static readonly DependencyProperty MediaEndingPositionProperty =
            DependencyProperty.Register("MediaEndingPosition", typeof(long), typeof(KMediaElement), new UIPropertyMetadata(-1L, OnMediaEndingPositionChanged));

        private static void OnMediaEndingPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnMediaEndingPositionChanged((long)e.NewValue);
        }

        private void OnMediaEndingPositionChanged(long position)
        {
            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate { MediaPlayer.MediaEndingPosition = position; });
        }

        #endregion

        #region MediaPosition

        public static readonly DependencyProperty MediaPositionProperty =
            DependencyProperty.Register("MediaPosition", typeof(long), typeof(KMediaElement),
                                        new FrameworkPropertyMetadata((long)0,
                                                                      new PropertyChangedCallback(OnMediaPositionChanged)));

        /// <summary>
        /// Gets or sets the media position in units of CurrentPositionFormat
        /// </summary>
        public long MediaPosition
        {
            get { return (long)GetValue(MediaPositionProperty); }
            set { SetValue(MediaPositionProperty, value); }
        }

        private static void OnMediaPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnMediaPositionChanged(e);
        }

        protected void OnMediaPositionChanged(DependencyPropertyChangedEventArgs e)
        {
            RaiseMediaPositionChanged();

            /* If the change came from within our class,
             * ignore this callback */
            if ((_ignorePropertyChangedCallback))
            {
                _ignorePropertyChangedCallback = false;
                return;
            }

            PlayerSetMediaPosition();
            CoerceValue(PositionProperty);
        }

        /// <summary>
        /// Used to set the MediaPosition without firing the
        /// PropertyChanged callback
        /// </summary>
        /// <param name="value">The value to set the MediaPosition to</param>
        protected void SetMediaPositionInternal(long value)
        {
            CoerceValue(PositionProperty);

            /* Flag that we want to ignore the next
             * PropertyChangedCallback */
            _ignorePropertyChangedCallback = true;

            MediaPosition = value;
        }

        private void PlayerSetMediaPosition()
        {
            var position = MediaPosition;
            if (MediaPlayer.Dispatcher.Shutdown || MediaPlayer.Dispatcher.ShuttingDown)
                return;

            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate { MediaPlayer.MediaPosition = position; });
            InvalidateVideoImage();
        }

        #endregion

        #region Position

        private static readonly DependencyPropertyKey PositionPropertyKey
           = DependencyProperty.RegisterReadOnly("Position", typeof(TimeSpan), typeof(KMediaElement),
                                                 new FrameworkPropertyMetadata(TimeSpan.Zero, null, OnCoercePosition));

        private static object OnCoercePosition(DependencyObject d, object baseValue)
        {
            var mediaElement = (KMediaElement)d;
            return TimeSpan.FromTicks(mediaElement.MediaPosition);
        }

        public static readonly DependencyProperty PositionProperty
            = PositionPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the position of the media in TimeSpan
        /// </summary>
        public TimeSpan Position
        {
            get { return (TimeSpan)GetValue(PositionProperty); }
        }

        #endregion

        #region MediaDuration

        private static readonly DependencyPropertyKey MediaDurationPropertyKey
           = DependencyProperty.RegisterReadOnly("MediaDuration", typeof(long), typeof(KMediaElement),
                                                 new FrameworkPropertyMetadata((long)0));

        public static readonly DependencyProperty MediaDurationProperty
            = MediaDurationPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the duration of the media in the units of CurrentPositionFormat
        /// </summary>
        public long MediaDuration
        {
            get { return (long)GetValue(MediaDurationProperty); }
        }

        /// <summary>
        /// Internal method to set the read-only MediaDuration
        /// </summary>
        protected void SetMediaDuration(long value)
        {
            SetValue(MediaDurationPropertyKey, value);
            SetValue(DurationPropertyKey, TimeSpan.FromTicks(value));
        }

        #endregion

        #region Duration

        private static readonly DependencyPropertyKey DurationPropertyKey
           = DependencyProperty.RegisterReadOnly("Duration", typeof(TimeSpan), typeof(KMediaElement),
                                                 new FrameworkPropertyMetadata(TimeSpan.Zero));

        public static readonly DependencyProperty DurationProperty
            = DurationPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the duration of the media in TimeSpan
        /// </summary>
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
        }

        #endregion

        #region CurrentPositionFormat

        private static readonly DependencyPropertyKey CurrentPositionFormatPropertyKey
            = DependencyProperty.RegisterReadOnly("CurrentPositionFormat", typeof(MediaPositionFormat), typeof(KMediaElement),
                new FrameworkPropertyMetadata(MediaPositionFormat.None));

        public static readonly DependencyProperty CurrentPositionFormatProperty
            = CurrentPositionFormatPropertyKey.DependencyProperty;

        /// <summary>
        /// The current position format that the media is currently using
        /// </summary>
        public MediaPositionFormat CurrentPositionFormat
        {
            get { return (MediaPositionFormat)GetValue(CurrentPositionFormatProperty); }
        }

        protected void SetCurrentPositionFormat(MediaPositionFormat value)
        {
            SetValue(CurrentPositionFormatPropertyKey, value);
        }

        #endregion

        #region PreferedPositionFormat

        public static readonly DependencyProperty PreferedPositionFormatProperty =
            DependencyProperty.Register("PreferedPositionFormat", typeof(MediaPositionFormat), typeof(KMediaElement),
                new FrameworkPropertyMetadata(MediaPositionFormat.MediaTime,
                    new PropertyChangedCallback(OnPreferedPositionFormatChanged)));

        /// <summary>
        /// The MediaPositionFormat that is prefered to be used
        /// </summary>
        public MediaPositionFormat PreferedPositionFormat
        {
            get { return (MediaPositionFormat)GetValue(PreferedPositionFormatProperty); }
            set { SetValue(PreferedPositionFormatProperty, value); }
        }

        private static void OnPreferedPositionFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnPreferedPositionFormatChanged(e);
        }

        /// <summary>
        /// Executes when a the prefered position format has changed
        /// </summary>
        protected void OnPreferedPositionFormatChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetPreferedPositionFormat();
        }

        private void PlayerSetPreferedPositionFormat()
        {
            var format = PreferedPositionFormat;
            MediaPositionFormat currentFormat;
            long duration;

            /* We use BeginInvoke here to avoid what seems to be a deadlock */
            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.PreferedPositionFormat = format;
                currentFormat = MediaPlayer.CurrentPositionFormat;
                duration = MediaPlayer.Duration;

                Dispatcher.BeginInvoke((Action)delegate
                {
                    SetCurrentPositionFormat(currentFormat);
                    SetMediaDuration(duration);
                });
            });
        }

        #endregion

        #region SpeedRatio

        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(KMediaElement),
                new FrameworkPropertyMetadata(1.0d, OnSpeedRatioChanged, OnCoerceSpeedRatio));

        /// <summary>
        /// Gets or sets the rate the media is played back
        /// </summary>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }

        private static void OnSpeedRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnSpeedRatioChanged(e);
        }

        private static object OnCoerceSpeedRatio(DependencyObject d, object baseValue)
        {
            var source = (KMediaElement)d;
            if (source.Source != null)
            {
                var sourceFile = source.Source.OriginalString;
                var sourceFileExtension = System.IO.Path.GetExtension(sourceFile);
                var filterConfig = source.FiltersConfiguration[sourceFileExtension];
                if (filterConfig != null && filterConfig.MinSpeedRatio.HasValue && filterConfig.MaxSpeedRatio.HasValue)
                {
                    var currentSpeed = (double)baseValue;
                    currentSpeed = Math.Max(currentSpeed, filterConfig.MinSpeedRatio.Value);
                    currentSpeed = Math.Min(currentSpeed, filterConfig.MaxSpeedRatio.Value);
                    return currentSpeed;
                }
            }
            return baseValue;
        }

        protected void OnSpeedRatioChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                PlayerSetSpeedRatio();

            RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, SpeedRatioChangedEvent));
        }

        public bool CanSetSpeedRatio(double speedRatio)
        {
            if (Source != null)
            {
                var filterConfig = GetExtensionFiltersSource();
                if (filterConfig != null && filterConfig.MinSpeedRatio.HasValue && filterConfig.MaxSpeedRatio.HasValue)
                {
                    if (speedRatio > filterConfig.MaxSpeedRatio || speedRatio < filterConfig.MinSpeedRatio)
                        return false;
                }
            }

            return true;
        }

        private void PlayerSetSpeedRatio()
        {
            var speedRatio = SpeedRatio;

            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.SpeedRatio = speedRatio;
            });
        }


        #endregion

        #region UnloadedBehavior

        public static readonly DependencyProperty UnloadedBehaviorProperty =
            DependencyProperty.Register("UnloadedBehavior", typeof(MediaState), typeof(KMediaElement),
                                        new FrameworkPropertyMetadata(MediaState.Close));

        /// <summary>
        /// Defines the behavior of the control when it is unloaded
        /// </summary>
        public MediaState UnloadedBehavior
        {
            get { return (MediaState)GetValue(UnloadedBehaviorProperty); }
            set { SetValue(UnloadedBehaviorProperty, value); }
        }

        #endregion

        #region LoadedBehavior

        public static readonly DependencyProperty LoadedBehaviorProperty =
            DependencyProperty.Register("LoadedBehavior", typeof(MediaState), typeof(KMediaElement),
                                        new FrameworkPropertyMetadata(MediaState.Play));

        /// <summary>
        /// Defines the behavior of the control when it is loaded
        /// </summary>
        public MediaState LoadedBehavior
        {
            get { return (MediaState)GetValue(LoadedBehaviorProperty); }
            set { SetValue(LoadedBehaviorProperty, value); }
        }

        #endregion

        #region Volume

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(KMediaElement),
                new FrameworkPropertyMetadata(1.0d,
                    new PropertyChangedCallback(OnVolumeChanged)));

        /// <summary>
        /// Gets or sets the audio volume.  Specifies the volume, as a 
        /// number from 0 to 1.  Full volume is 1, and 0 is silence.
        /// </summary>
        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnVolumeChanged(e);
        }

        protected void OnVolumeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
                {
                    MediaPlayer.Volume = (double)e.NewValue;
                });
        }

        #endregion

        #region Balance

        public static readonly DependencyProperty BalanceProperty =
            DependencyProperty.Register("Balance", typeof(double), typeof(KMediaElement),
                new FrameworkPropertyMetadata(0d,
                    new PropertyChangedCallback(OnBalanceChanged)));

        /// <summary>
        /// Gets or sets the balance on the audio.
        /// The value can range from -1 to 1. The value -1 means the right channel is attenuated by 100 dB 
        /// and is effectively silent. The value 1 means the left channel is silent. The neutral value is 0, 
        /// which means that both channels are at full volume. When one channel is attenuated, the other 
        /// remains at full volume.
        /// </summary>
        public double Balance
        {
            get { return (double)GetValue(BalanceProperty); }
            set { SetValue(BalanceProperty, value); }
        }

        private static void OnBalanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((KMediaElement)d).OnBalanceChanged(e);
        }

        protected void OnBalanceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (HasInitialized)
                MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
                {
                    MediaPlayer.Balance = (double)e.NewValue;
                });
        }

        #endregion

        #region IsPlaying

        /// <summary>
        /// Identifie la clé de la propriété de dépendance <see cref="PlayState"/>.
        /// </summary>
        private static readonly DependencyPropertyKey PlayStatePropertyKey
            = DependencyProperty.RegisterReadOnly("PlayState", typeof(PlayState), typeof(KMediaElement),
                new FrameworkPropertyMetadata(PlayState.Stopped));

        /// <summary>
        /// Identifie la propriété de dépendance <see cref="PlayState"/>.
        /// </summary>
        public static readonly DependencyProperty PlayStateProperty = PlayStatePropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient l'état de lecture du composant.
        /// </summary>
        public PlayState PlayState
        {
            get { return (PlayState)GetValue(PlayStateProperty); }
            protected set { SetValue(PlayStatePropertyKey, value); }
        }

        #endregion

        #region IsMediaOpened

        /// <summary>
        /// Gets a value indicating whether a media is currently opened.
        /// </summary>
        public bool IsMediaOpened
        {
            get { return (bool)GetValue(IsMediaOpenedProperty); }
        }
        /// <summary>
        /// Identifies the <see cref="IsMediaOpened"/> dependency property key.
        /// </summary>
        private static readonly DependencyPropertyKey IsMediaOpenedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsMediaOpened", typeof(bool), typeof(KMediaElement), new UIPropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="IsMediaOpened"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMediaOpenedProperty = IsMediaOpenedPropertyKey.DependencyProperty;

        /// <summary>
        /// Sets the <see cref="IsMediaOpened"/> value.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void SetIsMediaOpened(bool value)
        {
            SetValue(IsMediaOpenedPropertyKey, value);
        }

        #endregion

        #endregion

        #region RoutedEvents

        #region MediaOpened

        public static readonly RoutedEvent MediaOpenedEvent = EventManager.RegisterRoutedEvent("MediaOpened",
                                                                                               RoutingStrategy.Bubble,
                                                                                               typeof(RoutedEventHandler
                                                                                                   ),
                                                                                               typeof(KMediaElement));

        /// <summary>
        /// Fires when media has successfully been opened
        /// </summary>
        public event RoutedEventHandler MediaOpened
        {
            add { AddHandler(MediaOpenedEvent, value); }
            remove { RemoveHandler(MediaOpenedEvent, value); }
        }

        #endregion

        #region MediaClosed

        public static readonly RoutedEvent MediaClosedEvent = EventManager.RegisterRoutedEvent("MediaClosed",
                                                                                               RoutingStrategy.Bubble,
                                                                                               typeof(RoutedEventHandler),
                                                                                               typeof(KMediaElement));

        /// <summary>
        /// Fires when media has been closed
        /// </summary>
        public event RoutedEventHandler MediaClosed
        {
            add { AddHandler(MediaClosedEvent, value); }
            remove { RemoveHandler(MediaClosedEvent, value); }
        }

        #endregion

        #region MediaEnded

        public static readonly RoutedEvent MediaEndedEvent = EventManager.RegisterRoutedEvent("MediaEnded",
                                                                                              RoutingStrategy.Bubble,
                                                                                              typeof(RoutedEventHandler),
                                                                                              typeof(KMediaElement));

        /// <summary>
        /// Fires when media has completed playing
        /// </summary>
        public event RoutedEventHandler MediaEnded
        {
            add { AddHandler(MediaEndedEvent, value); }
            remove { RemoveHandler(MediaEndedEvent, value); }
        }

        #endregion

        #region MediaFailed

        public static readonly RoutedEvent MediaFailedEvent = EventManager.RegisterRoutedEvent("MediaFailed",
                                                                                              RoutingStrategy.Bubble,
                                                                                              typeof(RoutedMediaFailedEventHandler),
                                                                                              typeof(KMediaElement));

        /// <summary>
        /// Fires when media has completed playing
        /// </summary>
        public event RoutedMediaFailedEventHandler MediaFailed
        {
            add { AddHandler(MediaFailedEvent, value); }
            remove { RemoveHandler(MediaFailedEvent, value); }
        }

        #endregion

        #region SpeedRatioChanged

        /// <summary>
        /// Occurs when the speed ratio has changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SpeedRatioChanged
        {
            add { AddHandler(SpeedRatioChangedEvent, value); }
            remove { RemoveHandler(SpeedRatioChangedEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SpeedRatioChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent SpeedRatioChangedEvent = EventManager.RegisterRoutedEvent(
            "SpeedRatioChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(KMediaElement));

        #endregion

        #region MediaStateChanged

        public static readonly RoutedEvent MediaStateChangedEvent = EventManager.RegisterRoutedEvent("MediaStateChanged",
                                                                                               RoutingStrategy.Bubble,
                                                                                               typeof(RoutedEventHandler
                                                                                                   ),
                                                                                               typeof(KMediaElement));

        /// <summary>
        /// Fires when media has successfully been opened
        /// </summary>
        public event RoutedEventHandler MediaStateChanged
        {
            add { AddHandler(MediaStateChangedEvent, value); }
            remove { RemoveHandler(MediaStateChangedEvent, value); }
        }

        #endregion

        #region MediaPositionChanged

        /// <summary>
        /// Occurs when the MediaPosition has changed.
        /// </summary>
        public event RoutedEventHandler MediaPositionChanged
        {
            add { AddHandler(MediaPositionChangedEvent, value); }
            remove { RemoveHandler(MediaPositionChangedEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MediaPositionChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent MediaPositionChangedEvent = EventManager.RegisterRoutedEvent(
            "MediaPositionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(KMediaElement));

        /// <summary>
        /// Lève l'évènement <see cref="MediaPositionChangedEvent"/>.
        /// </summary>
        protected virtual void RaiseMediaPositionChanged()
        {
            base.RaiseEvent(new RoutedEventArgs(MediaPositionChangedEvent, this));
        }

        #endregion

        #endregion

        #region Constructor

        public KMediaElement()
        {
            DefaultApartmentState = ApartmentState.MTA;

            OnInitializeD3DVideo();
            OnInitializeMediaPlayer();

            this.Loaded += new RoutedEventHandler(KMediaElement_Loaded);
            this.Unloaded += new RoutedEventHandler(KMediaElement_Unloaded);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The Image control that has the source
        /// to the D3DImage
        /// </summary>
        public Image VideoImage
        {
            get { return _videoImage; }
        }

        protected DxMediaPlayer MediaPlayer { get; private set; }

        protected ApartmentState DefaultApartmentState { get; private set; }

        public bool HasInitialized
        {
            get;
            protected set;
        }

        /// <summary>
        /// Renders the video with WPF's rendering using the CompositionTarget.Rendering event
        /// </summary>
        protected bool RenderOnCompositionTargetRendering
        {
            get
            {
                return _renderOnCompositionTargetRendering;
            }
            set
            {
                /* If it is being set to true and it was previously false
                 * then hook into the event */
                if (value && !_renderOnCompositionTargetRendering)
                    CompositionTarget.Rendering += CompositionTargetRendering;
                else if (!value)
                    CompositionTarget.Rendering -= CompositionTargetRendering;

                _renderOnCompositionTargetRendering = value;
                _renderOnCompositionTargetRenderingTemp = value;
            }
        }

        /// <summary>
        /// Gets the filter configuration
        /// </summary>
        protected FiltersConfiguration FiltersConfiguration
        {
            get
            {
                if (_filtersConfig == null)
                    if (DesignMode.IsInDesignMode)
                        _filtersConfig = new Controls.FiltersConfiguration();
                    else
                        _filtersConfig = IoC.Resolve<IServiceBus>().Get<IDecoderInfoService>().FiltersConfiguration;
                return _filtersConfig;
            }
        }

        #endregion

        #region Overrides

        public override void BeginInit()
        {
            HasInitialized = false;

            base.BeginInit();
        }

        public override void EndInit()
        {
            PlayerSetVideoRenderer();
            PlayerSetAudioRenderer();
            PlayerSetLoop();
            PlayerSetSource();
            PlayerSetMediaPosition();
            PlayerSetPreferedPositionFormat();
            PlayerSetSpeedRatio();

            double balance = Balance;
            double volume = Volume;

            MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                MediaPlayer.Balance = balance;
                MediaPlayer.Volume = volume;
            });

            HasInitialized = true;

            base.EndInit();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _videoImage.Measure(availableSize);
            return _videoImage.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _videoImage.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index > 0)
                throw new IndexOutOfRangeException();

            return _videoImage;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Plays the media
        /// </summary>
        public void Play()
        {
            MediaPlayer.EnsureThread(DefaultApartmentState);
            MediaPlayer.Dispatcher.BeginInvoke((Action)(delegate
            {
                MediaPlayer.Play();
            }));
            this.PlayState = PlayState.Playing;
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        public void Pause()
        {
            MediaPlayer.EnsureThread(DefaultApartmentState);
            MediaPlayer.Dispatcher.BeginInvoke((Action)(() => MediaPlayer.Pause()));
            this.PlayState = PlayState.Paused;
        }

        /// <summary>
        /// Closes the media
        /// </summary>
        public void Close()
        {
            MediaPlayer.MediaOpened -= new EventHandler(MediaPlayer_MediaOpened);
            MediaPlayer.MediaClosed -= new EventHandler(MediaPlayer_MediaClosed);
            MediaPlayer.MediaFailed -= new EventHandler<MediaFailedEventArgs>(MediaPlayer_MediaFailed);
            MediaPlayer.MediaEnded -= new EventHandler(MediaPlayer_MediaEnded);
            MediaPlayer.MediaPositionChanged -= new EventHandler(MediaPlayer_MediaPositionChanged);
            MediaPlayer.IsLoadingChanged -= new EventHandler(MediaPlayer_IsLoadingChanged);

            MediaPlayer.NewAllocatorFrame -= new EventHandler(MediaPlayer_NewAllocatorFrame);
            MediaPlayer.NewAllocatorSurface -= new EventHandler<NewAllocatorSurfaceEventArgs>(MediaPlayer_NewAllocatorSurface);

            SetBackBuffer(IntPtr.Zero);
            InvalidateVideoImage();

            if (!MediaPlayer.Dispatcher.Shutdown || !MediaPlayer.Dispatcher.ShuttingDown)
                MediaPlayer.Dispatcher.BeginInvoke((Action)(delegate
                {
                    MediaPlayer.Close();
                    MediaPlayer.Dispose();
                }));

            this.PlayState = PlayState.Stopped;
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        public void Stop()
        {
            if (!MediaPlayer.Dispatcher.Shutdown || !MediaPlayer.Dispatcher.ShuttingDown)
                MediaPlayer.Dispatcher.BeginInvoke((Action)(() => MediaPlayer.Stop()));

            this.PlayState = PlayState.Stopped;
        }

        /// <summary>
        /// Get the current rendered image
        /// </summary>
        /// <returns></returns>
        public BitmapSource GetCurrentImage()
        {
            if (_backBuffer == IntPtr.Zero) return null;

            int width = _d3dImage.PixelWidth;
            int height = _d3dImage.PixelHeight;

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
                dc.DrawImage(_d3dImage, new Rect(0, 0, width, height));

            RenderTargetBitmap rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);

            return BitmapFrame.Create(rtb);
        }

        /// <summary>
        /// Begins an async action on the MediaPlayer's dispatcher.
        /// </summary>
        /// <param name="action">The action.</param>
        public void BeginInvokeOnMediaPlayerDispatcher(Delegate action)
        {
            MediaPlayer.Dispatcher.BeginInvoke(action);
        }

        #endregion

        #region Private methods

        private void EnsurePlayerThread()
        {
            MediaPlayer.EnsureThread(DefaultApartmentState);
        }

        /// <summary>
        /// Occurs when the media player is being initialized.  Here
        /// the method is overridden as to attach to media seeking
        /// related functionality
        /// </summary>
        private void OnInitializeMediaPlayer()
        {
            if (MediaPlayer != null)
                return;

            MediaPlayer = new DxMediaPlayer();

            EnsurePlayerThread();

            MediaPlayer.FiltersConfiguration = FiltersConfiguration;

            MediaPlayer.Dispatcher.UnhandledException += new EventHandler<DirectShow.DispatcherUnhandledExceptionEventArgs>(Dispatcher_UnhandledException);

            MediaPlayer.MediaOpened += new EventHandler(MediaPlayer_MediaOpened);
            MediaPlayer.MediaClosed += new EventHandler(MediaPlayer_MediaClosed);
            MediaPlayer.MediaFailed += new EventHandler<MediaFailedEventArgs>(MediaPlayer_MediaFailed);
            MediaPlayer.MediaEnded += new EventHandler(MediaPlayer_MediaEnded);
            MediaPlayer.MediaPositionChanged += new EventHandler(MediaPlayer_MediaPositionChanged);
            MediaPlayer.IsLoadingChanged += new EventHandler(MediaPlayer_IsLoadingChanged);

            MediaPlayer.NewAllocatorFrame += new EventHandler(MediaPlayer_NewAllocatorFrame);
            MediaPlayer.NewAllocatorSurface += new EventHandler<NewAllocatorSurfaceEventArgs>(MediaPlayer_NewAllocatorSurface);
        }

        /// <summary>
        /// Initializes the D3DRenderer control
        /// </summary>
        private void OnInitializeD3DVideo()
        {
            if (_videoImage != null)
                return;

            /* Create our Image and it's D3DImage source */
            _videoImage = new Image();
            _d3dImage = new D3DImage();

            /* We hook into this event to handle when a D3D device is lost */
            _d3dImage.IsFrontBufferAvailableChanged += _d3dImage_IsFrontBufferAvailableChanged;

            /* Set our default stretch value of our video */
            _videoImage.Stretch = (Stretch)StretchProperty.DefaultMetadata.DefaultValue;
            _videoImage.StretchDirection = (StretchDirection)StretchProperty.DefaultMetadata.DefaultValue;

            /* Our source of the video image is the D3DImage */
            _videoImage.Source = _d3dImage;

            /* Register the Image as a visual child */
            AddVisualChild(_videoImage);

            /* Bind the horizontal alignment dp of this control to that of the video image */
            _videoImage.SetBinding(HorizontalAlignmentProperty, new Binding("HorizontalAlignment") { Source = this });

            /* Bind the vertical alignment dp of this control to that of the video image */
            _videoImage.SetBinding(VerticalAlignmentProperty, new Binding("VerticalAlignment") { Source = this });

            ToggleDeepColorEffect((bool)DeepColorProperty.DefaultMetadata.DefaultValue);
        }

        /// <summary>
        /// Executes the actions associated to a MediaState
        /// </summary>
        /// <param name="state">The MediaState to execute</param>
        private void ExecuteMediaState(MediaState state)
        {
            switch (state)
            {
                case MediaState.Manual:
                    break;
                case MediaState.Play:
                    Play();
                    break;
                case MediaState.Stop:
                    Stop();
                    break;
                case MediaState.Close:
                    Close();
                    break;
                case MediaState.Pause:
                    Pause();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RaiseEvent(new RoutedEventArgs(MediaStateChangedEvent));
        }

        private void ToggleDeepColorEffect(bool isEnabled)
        {
            //_videoImage.Effect = isEnabled ? new DeepColorEffect() : null;
        }

        /// <summary>
        /// Configures D3DImage with a new surface.  The back buffer is
        /// not set until we actually receive a frame, this way we
        /// can avoid a black flicker between media changes
        /// </summary>
        /// <param name="backBuffer">The unmanaged pointer to the Direct3D Surface</param>
        protected void SetBackBuffer(IntPtr backBuffer)
        {
            /* We only do this if target rendering is enabled because we must use an Invoke
             * instead of a BeginInvoke to keep the Surfaces in sync and Invoke could be dangerous
             * in other situations */
            if (RenderOnCompositionTargetRendering)
            {
                if (!_d3dImage.Dispatcher.CheckAccess())
                {
                    _d3dImage.Dispatcher.Invoke((Action)(() => SetBackBuffer(backBuffer)), DispatcherPriority.Render);
                    return;
                }
            }

            /* Flag a new surface */
            _newSurfaceAvailable = true;
            _backBuffer = backBuffer;

            /* Make a special case for target rendering */
            if (RenderOnCompositionTargetRendering || _backBuffer == IntPtr.Zero)
            {
                SetBackBufferInternal(_backBuffer);
            }

            SetBackBufferForClones();
        }

        /// <summary>
        /// Invalidates the entire Direct3D image, notifying WPF to redraw
        /// </summary>
        private void InvalidateVideoImage()
        {
            if (_renderOnCompositionTargetRendering)
                return;

            /* Ensure we run on the correct Dispatcher */
            if (!_d3dImage.Dispatcher.CheckAccess())
            {
                _d3dImage.Dispatcher.Invoke((Action)(() => InvalidateVideoImage()));
                return;
            }

            /* If there is a new Surface to set,
             * this method will do the trick */
            SetBackBufferInternal(_backBuffer);

            /* Only render the video image if possible, or if IsRenderingEnabled is true */
            if (_d3dImage.IsFrontBufferAvailable && IsRenderingEnabled && _backBuffer != IntPtr.Zero)
            {
                try
                {
                    /* Invalidate the entire image */
                    _d3dImage.Lock();
                    _d3dImage.AddDirtyRect(new Int32Rect(0, /* Left */
                                                        0, /* Top */
                                                        _d3dImage.PixelWidth, /* Width */
                                                        _d3dImage.PixelHeight /* Height */));
                    _d3dImage.Unlock();
                }
                catch (Exception)
                { }
            }

            /* Invalidate all of our cloned D3DRenderers */
            InvalidateClonedVideoImages();
        }

        /// <summary>
        /// Cleans up any dead references we may have to any cloned renderers
        /// </summary>
        private void CleanZombieRenderers()
        {
            lock (_clonedD3Drenderers)
            {
                var deadObjects = new List<WeakReference>();

                for (int i = 0; i < _clonedD3Drenderers.Count; i++)
                {
                    if (!_clonedD3Drenderers[i].IsAlive)
                        deadObjects.Add(_clonedD3Drenderers[i]);
                }

                foreach (var deadGuy in deadObjects)
                {
                    _clonedD3Drenderers.Remove(deadGuy);
                }
            }
        }

        /// <summary>
        /// Sets the backbuffer for any cloned D3DRenderers
        /// </summary>
        private void SetBackBufferForClones()
        {
            lock (_clonedD3Drenderers)
            {
                CleanZombieRenderers();

                foreach (var rendererRef in _clonedD3Drenderers)
                {
                    var renderer = rendererRef.Target as KMediaElement;

                    if (renderer != null)
                        renderer.SetBackBuffer(_backBuffer);
                }
            }
        }

        /// <summary>
        /// Configures D3DImage with a new surface.  This happens immediately
        /// </summary>
        private void SetBackBufferInternal(IntPtr backBuffer)
        {
            /* Do nothing if we don't have a new surface available */
            if (!_newSurfaceAvailable)
                return;

            if (!_d3dImage.Dispatcher.CheckAccess())
            {
                _d3dImage.Dispatcher.BeginInvoke((Action)(() => SetBackBufferInternal(backBuffer)));
                return;
            }

            /* We have this around a try/catch just in case we
             * lose the device and our Surface is invalid. The
             * try/catch may not be needed, but testing needs
             * to take place before it's removed */
            try
            {
                _d3dImage.Lock();
                _d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, backBuffer);
                _d3dImage.Unlock();

                SetNaturalWidthHeight();
            }
            catch (Exception)
            { }

            /* Clear our flag, so this won't be ran again
             * until a new surface is sent */
            _newSurfaceAvailable = false;
        }

        private void SetNaturalWidthHeight()
        {
            SetNaturalVideoHeight(_d3dImage.PixelHeight);
            SetNaturalVideoWidth(_d3dImage.PixelWidth);
        }

        /// <summary>
        /// Invalidates any possible cloned renderer we may have
        /// </summary>
        private void InvalidateClonedVideoImages()
        {
            lock (_clonedD3Drenderers)
            {
                CleanZombieRenderers();

                foreach (var rendererRef in _clonedD3Drenderers)
                {
                    var renderer = rendererRef.Target as KMediaElement;

                    if (renderer != null)
                        renderer.InvalidateVideoImage();
                }
            }
        }

        #endregion

        #region Private event handlers

        /// <summary>
        /// A private handler for the MediaPositionChanged event of the media player
        /// </summary>
        private void MediaPlayer_MediaPositionChanged(object sender, EventArgs e)
        {
            long position = MediaPlayer.MediaPosition;

            Dispatcher.BeginInvoke((Action)delegate
            {
                SetMediaPositionInternal(position);
            });
        }

        private void MediaPlayer_IsLoadingChanged(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                SetIsLoading(MediaPlayer.IsLoading);
            });
        }

        private void MediaPlayer_NewAllocatorSurface(object sender, NewAllocatorSurfaceEventArgs e)
        {
            SetBackBuffer(e.Surface);
        }

        private void MediaPlayer_NewAllocatorFrame(object sender, EventArgs e)
        {
            InvalidateVideoImage();
        }

        private void Dispatcher_UnhandledException(object sender, DirectShow.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                throw e.Exception;
            }));
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            long duration = MediaPlayer.Duration;
            bool hasVideo = MediaPlayer.HasVideo;
            int videoWidth = MediaPlayer.NaturalVideoWidth;
            int videoHeight = MediaPlayer.NaturalVideoHeight;

            long position = 0;
            double rate = 1;
            double volume = 1;
            double balance = 0;

            var positionFormat = MediaPlayer.CurrentPositionFormat;

            Dispatcher.BeginInvoke((Action)delegate
            {
                /* If we have no video just black out the video
                 * area by releasing the D3D surface */
                if (!hasVideo)
                    SetBackBuffer(IntPtr.Zero);

                SetNaturalVideoWidth(videoWidth);
                SetNaturalVideoHeight(videoHeight);

                /* Set our dp values to match the media player */
                SetHasVideo(hasVideo);

                balance = Balance;
                position = MediaPosition;
                rate = SpeedRatio;
                volume = Volume;

                /* Set our DP values */
                SetCurrentPositionFormat(positionFormat);
                SetMediaDuration(duration);

                MediaPlayer.Dispatcher.BeginInvoke((Action)delegate
                {
                    MediaPlayer.SpeedRatio = rate;
                    MediaPlayer.Volume = volume;
                    MediaPlayer.Balance = balance;
                });

                SetIsMediaOpened(true);
                this.PlayState = PlayState.Playing;
                RaiseEvent(new RoutedEventArgs(MediaOpenedEvent));
            });
        }

        private void MediaPlayer_MediaClosed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                SetIsMediaOpened(false);
                this.PlayState = PlayState.Stopped;
                RaiseEvent(new RoutedEventArgs(MediaClosedEvent));
            });
        }

        private void MediaPlayer_MediaFailed(object sender, MediaFailedEventArgs e)
        {
            /* Reset some values on a failure of the media */
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.PlayState = PlayState.Stopped;
                SetMediaDuration(0);
                MediaPosition = 0;

                RaiseEvent(new RoutedMediaFailedEventArgs(MediaFailedEvent, e.FileSource, e.Exception));
            });
        }

        private void MediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.ExecuteMediaState(MediaState.Pause);
                RaiseEvent(new RoutedEventArgs(MediaEndedEvent));
            });
        }

        private void KMediaElement_Unloaded(object sender, RoutedEventArgs e)
        {
            /* Remember what the property value was */
            _renderOnCompositionTargetRenderingTemp = RenderOnCompositionTargetRendering;

            /* Make sure to unhook the static event hook because we are unloading */
            RenderOnCompositionTargetRendering = false;

            ExecuteMediaState(UnloadedBehavior);

            if (Application.Current == null)
                return;

            _windowHooked = false;

            if (_currentWindow == null)
                return;

            _currentWindow.Closed -= WindowOwnerClosed;
            _currentWindow = null;
        }

        private void KMediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            /* Restore the property's value */
            RenderOnCompositionTargetRendering = _renderOnCompositionTargetRenderingTemp;

            _currentWindow = Window.GetWindow(this);

            if (_currentWindow != null && !_windowHooked)
            {
                _currentWindow.Closed += WindowOwnerClosed;
                _windowHooked = true;
            }

            ExecuteMediaState(LoadedBehavior);
        }

        private void WindowOwnerClosed(object sender, EventArgs e)
        {
            ExecuteMediaState(UnloadedBehavior);
        }

        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            InvalidateVideoImage();
        }

        /// <summary>
        /// This should only fire when a D3D device is lost
        /// </summary>
        private void _d3dImage_IsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_d3dImage.IsFrontBufferAvailable)
                return;

            /* Flag that we have a new surface, even
             * though we really don't */
            _newSurfaceAvailable = true;

            /* Force feed the D3DImage the Surface pointer */
            SetBackBufferInternal(_backBuffer);
        }

        private ExtensionFiltersSource GetExtensionFiltersSource()
        {
            if (Source != null)
            {
                var sourceFile = Source.OriginalString;
                var sourceFileExtension = System.IO.Path.GetExtension(sourceFile);
                var filterConfig = FiltersConfiguration[sourceFileExtension];
                return filterConfig;
            }
            else
                return null;
        }

        #endregion
    }
}

#pragma warning restore 1591