using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows.Converters;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Generic media player
    /// </summary>
    [TemplatePart(Name = PART_MediaElement, Type = typeof(KMediaElement))]
    [TemplatePart(Name = PART_VideosSelector, Type = typeof(Selector))]
    [TemplatePart(Name = PART_TimeLine, Type = typeof(RangeBase))]
    [TemplatePart(Name = PART_DetailPresenterSelected, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_DetailPresenterOvered, Type = typeof(ContentControl))]
    [TemplatePart(Name = PART_Volume, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Mute, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Previous, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Backward, Type = typeof(ButtonBase))]
    //[TemplatePart(Name = PART_Stop, Type = typeof(ButtonBase))] User Story 4371:Modifier la strategie de selection des videos
    [TemplatePart(Name = PART_Play, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Pause, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Forward, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Next, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_MarkersSelector, Type = typeof(Selector))]
    [TemplatePart(Name = PART_TimelineScrollViewer, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = PART_Position, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_Thumbnail, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_ThumbnailView, Type = typeof(Image))]
    [TemplateVisualState(GroupName = VSM_GRP_State, Name = VSM_State_IsPlaying)]
    [TemplateVisualState(GroupName = VSM_GRP_State, Name = VSM_State_IsPaused)]
    [TemplateVisualState(GroupName = VSM_GRP_State, Name = VSM_State_IsStopped)]
    [TemplateVisualState(GroupName = VSM_GRP_Volume, Name = VSM_Volume_IsOn)]
    [TemplateVisualState(GroupName = VSM_GRP_Volume, Name = VSM_Volume_IsOff)]
    [StyleTypedProperty(Property = "VideosSelectorStyle", StyleTargetType = typeof(ListBox))]
    public class KMediaPlayer : Control, IMediaPlayer
    {
        #region Constants

        const string PART_MediaElement = "PART_MediaElement";
        const string PART_VideosSelector = "PART_VideosSelector";
        const string PART_TimeLine = "PART_TimeLine";
        const string PART_DetailPresenterSelected = "PART_DetailPresenterSelected";
        const string PART_DetailPresenterOvered = "PART_DetailPresenterOvered";
        const string PART_Volume = "PART_Volume";
        const string PART_Mute = "PART_Mute";
        const string PART_Previous = "PART_Previous";
        const string PART_Backward = "PART_Backward";
        //const string PART_Stop = "PART_Stop"; User Story 4371:Modifier la strategie de selection des videos
        const string PART_Play = "PART_Play";
        const string PART_Pause = "PART_Pause";
        const string PART_Forward = "PART_Forward";
        const string PART_Next = "PART_Next";
        const string PART_MarkersSelector = "PART_MarkersSelector";
        const string PART_TimelineScrollViewer = "PART_TimelineScrollViewer";
        const string PART_Position = "PART_Position";
        const string PART_Thumbnail = "PART_Thumbnail";
        const string PART_ThumbnailView = "PART_ThumbnailView";

        const string VSM_GRP_State = "State";
        const string VSM_State_IsPlaying = "IsPlaying";
        const string VSM_State_IsPaused = "IsPaused";
        const string VSM_State_IsStopped = "IsStopped";
        const string VSM_GRP_Volume = "Volume";
        const string VSM_Volume_IsOn = "IsOn";
        const string VSM_Volume_IsOff = "IsOff";

        const string DefaultMediaPositionStringFormatProperty = @"{0:hh\:mm\:ss\.f} / {1:hh\:mm\:ss\.f}";

        /// <summary>
        /// Les valeurs par défaut pour les speed ratios.
        /// </summary>
        public const string DefaultSpeedRatios = "0.1 0.25 0.5 0.75 1 1.5 2 3 4 5 6 7 8";

        readonly DoubleCollection _speedRatios =
            (DoubleCollection)new DoubleCollectionConverter().ConvertFrom(DefaultSpeedRatios);

        const double TimelineMinimumOffset = 10;

        #endregion

        #region Fields

        Selector _videosSelector;
        RangeBase _timelineRangeBase;
        ScrollViewer _timelineScrollViewer;
        FrameworkElement _thumbnailButton;
        Image _thumbnailView;

        double _timelineComputedWidthNotAnimated;
        double _timelineHorizontalOffsetNotAnimated;

        Storyboard _moveTimeLineScrollViewerStoryBoard;

        DispatcherTimer _thumbnailClickTimer;

        #endregion

        #region Events

        /// <summary>
        /// Définit l'évènement <see cref="StartMarkerClick"/>.
        /// </summary>
        public static readonly RoutedEvent StartMarkerClickEvent = EventManager.RegisterRoutedEvent(nameof(StartMarkerClick), RoutingStrategy.Bubble, typeof(MarkerRoutedEventHandler), typeof(KMediaPlayer));

        /// <summary>
        /// Survient lorsque le marqueur inférieur est cliqué.
        /// </summary>
        public event MarkerRoutedEventHandler StartMarkerClick
        {
            add { AddHandler(StartMarkerClickEvent, value); }
            remove { RemoveHandler(StartMarkerClickEvent, value); }
        }

        /// <summary>
        /// Définit l'évènement <see cref="EndMarkerClick"/>.
        /// </summary>
        public static readonly RoutedEvent EndMarkerClickEvent = EventManager.RegisterRoutedEvent(nameof(EndMarkerClick), RoutingStrategy.Bubble, typeof(MarkerRoutedEventHandler), typeof(KMediaPlayer));


        /// <summary>
        /// Survient lorsque le marqueur est cliqué.
        /// </summary>
        public event MarkerRoutedEventHandler EndMarkerClick
        {
            add { AddHandler(EndMarkerClickEvent, value); }
            remove { RemoveHandler(EndMarkerClickEvent, value); }
        }

        /// <summary>
        /// Délégué représentant une intéraction avec un marqueur.
        /// </summary>
        /// <param name="sender">La source</param>
        /// <param name="e">Les données</param>
        public delegate void MarkerRoutedEventHandler(object sender, RoutedEventArgs e);

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
        public static readonly RoutedEvent SpeedRatioChangedEvent = KMediaElement.SpeedRatioChangedEvent.AddOwner(typeof(KMediaPlayer));

        #endregion

        #region MediaOpened

        /// <summary>
        /// Fires when media has successfully been opened
        /// </summary>
        public event RoutedEventHandler MediaOpened
        {
            add { AddHandler(MediaOpenedEvent, value); }
            remove { RemoveHandler(MediaOpenedEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MediaOpened"/> event.
        /// </summary>
        public static readonly RoutedEvent MediaOpenedEvent = KMediaElement.MediaOpenedEvent.AddOwner(typeof(KMediaPlayer));

        #endregion

        #endregion

        #region Commands

        Command _changeVideoCommand;
        /// <summary>
        /// Obtient la commande permettant de changer la video courante.
        /// </summary>
        public ICommand ChangeVideoCommand
        {
            get
            {
                if (_changeVideoCommand == null)
                    _changeVideoCommand = new Command(() =>
                    {
                        ShowThumbnailView(null);
                        ChangeVideo();
                    },
                    () => !IsPlaying);
                return _changeVideoCommand;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="KMediaPlayer"/> class.
        /// </summary>
        static KMediaPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KMediaPlayer), new FrameworkPropertyMetadata(typeof(KMediaPlayer)));
        }

        /// <summary>
        /// Constructeur.
        /// </summary>
        public KMediaPlayer()
        {
            _thumbnailClickTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(GetDoubleClickTime()),
                DispatcherPriority.Background,
                ThumbnailClickTimer_Tick,
                Dispatcher.CurrentDispatcher)
            {
                IsEnabled = false
            };
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateTimelineScroll();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            MediaElement = GetTemplateChild<KMediaElement>(PART_MediaElement);
            _thumbnailView = GetTemplateChild<Image>(PART_ThumbnailView);

            SetBinding(CurrentPositionProperty, new Binding(nameof(KMediaElement.MediaPosition)) { Source = MediaElement, Mode = BindingMode.TwoWay });

            MediaElement.MediaFailed += MediaElement_MediaFailed;
            MediaElement.MediaOpened += MediaElement_MediaOpened;
            MediaElement.MediaStateChanged += MediaElement_MediaStateChanged;
            MediaElement.MediaEnded += MediaElement_MediaEnded;
            MediaElement.MediaPositionChanged += MediaElement_MediaPositionChanged;

            if (_videosSelector != null)
                _videosSelector.SelectionChanged -= _videosSelector_SelectionChanged;
            _videosSelector = GetTemplateChild<Selector>(PART_VideosSelector);
            if (_videosSelector != null)
            {
                _videosSelector.SelectionChanged += _videosSelector_SelectionChanged;
                if (Source != null)
                    _videosSelector.SelectedValue = Source;
                else if (SelectedSource != null)
                    _videosSelector.SelectedItem = SelectedSource;
            }

            _timelineRangeBase = GetTemplateChild<RangeBase>(PART_TimeLine);
            _timelineRangeBase.ValueChanged += OnTimelineValueChanged;

            var volume = GetTemplateChild<ButtonBase>(PART_Volume);
            if (volume != null)
                volume.Click += (sender, e) => Mute();
            var mute = GetTemplateChild<ButtonBase>(PART_Mute);
            if (mute != null)
                mute.Click += (sender, e) => Unmute();

            GetTemplateChild<ButtonBase>(PART_Play).Click += (sender, e) => Play();
            GetTemplateChild<ButtonBase>(PART_Pause).Click += (sender, e) => Pause();
            //GetTemplateChild<ButtonBase>(PART_Stop).Click += (sender, e) => ChangeVideo(); User Story 4371:Modifier la strategie de selection des videos

            GetTemplateChild<ButtonBase>(PART_Next).Click += (sender, e) => StepForward();
            GetTemplateChild<ButtonBase>(PART_Previous).Click += (sender, e) => StepBackward();

            GetTemplateChild<ButtonBase>(PART_Forward).Click += (sender, e) =>
            {
                var matchingTick = _speedRatios.FirstOrDefault(sr => sr == MediaElement.SpeedRatio);
                if (matchingTick != 0)
                {
                    var index = _speedRatios.IndexOf(matchingTick);
                    if (index != -1 && index < _speedRatios.Count - 1)
                        MediaElement.SpeedRatio = _speedRatios[index + 1];
                }
                else
                    MediaElement.SpeedRatio += .5;
            };
            GetTemplateChild<ButtonBase>(PART_Backward).Click += (sender, e) =>
            {
                var matchingTick = _speedRatios.FirstOrDefault(sr => sr == MediaElement.SpeedRatio);
                if (matchingTick != 0)
                {
                    var index = _speedRatios.IndexOf(matchingTick);
                    if (index != -1 && index >= 1)
                        MediaElement.SpeedRatio = _speedRatios[index - 1];
                }
                else if (MediaElement.SpeedRatio > .5)
                    MediaElement.SpeedRatio -= .5;
            };


            if (_thumbnailButton != null)
                _thumbnailButton.MouseLeftButtonDown -= ThumbnailButton_MouseLeftButtonUp;

            _thumbnailButton = GetTemplateChild(PART_Thumbnail) as FrameworkElement;
            if (_thumbnailButton != null)
                _thumbnailButton.MouseLeftButtonDown += ThumbnailButton_MouseLeftButtonUp;

            UpdatePositionStringFormat();

            if (IsMuted)
                Mute();
            else
                Unmute();

            var selector = GetTemplateChild<Selector>(PART_MarkersSelector);

            _timelineScrollViewer = GetTemplateChild<ScrollViewer>(PART_TimelineScrollViewer);
            _timelineScrollViewer.PreviewMouseWheel += TimelineScrollViewer_PreviewMouseWheel;

            UpdateTimelineScroll();
            UpdateVisualStates(false);

            Unloaded += KMediaPlayer_Unloaded;
        }

        void ChangeVideo()
        {
            switch (MediaElement.PlayState)
            {
                case PlayState.Playing:
                case PlayState.Paused:
                    Stop();
                    break;
                case PlayState.Stopped:
                    Pause();
                    break;
            }
        }

        void ThumbnailClickTimer_Tick(object sender, EventArgs e)
        {
            lock (_thumbnailClickTimer)
            {
                if (!_thumbnailClickTimer.IsEnabled)
                    return;
                _thumbnailClickTimer.Stop();
            }

            if (HasSource)
            {
                if (ThumbnailCommand.CanExecute(null))
                    ThumbnailCommand.Execute(null);
            }
            else
            {
                if (ThumbnailExternalCommand.CanExecute(null))
                    ThumbnailExternalCommand.Execute(null);
            }
        }

        void ThumbnailButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (HasThumbnail)
            {
                ThumbnailExternalCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.ClickCount == 1)
            {
                lock (_thumbnailClickTimer)
                    _thumbnailClickTimer.Start();
            }
            else if (e.ClickCount == 2)
            {
                lock (_thumbnailClickTimer)
                {
                    _thumbnailClickTimer.Stop();
                    if (ThumbnailExternalCommand.CanExecute(null))
                        ThumbnailExternalCommand.Execute(null);
                }
            }
        }

        bool _ignoreNextMediaPositionValueChanged;

        void MediaElement_MediaPositionChanged(object sender, RoutedEventArgs e)
        {
            if (_timelineRangeBase != null)
            {
                _ignoreNextMediaPositionValueChanged = true;
                _timelineRangeBase.Value = MediaElement.MediaPosition;
                _ignoreNextMediaPositionValueChanged = false;
            }
        }

        void OnTimelineValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_ignoreNextMediaPositionValueChanged)
            {
                if (SeekCommand != null && SeekCommand.CanExecute(null))
                    SeekCommand.Execute(null);

                MediaElement.MediaPosition = Convert.ToInt64(e.NewValue);
            }
        }

        void UpdatePositionStringFormat()
        {
            if (GetTemplateChild(PART_Position) is TextBlock tb)
            {
                MultiBinding binding;

                if (TimeToStringFormatter != null)
                {
                    binding = new MultiBinding()
                    {
                        Converter = TimeToStringFormatter.CurrentPositionConverter,
                    };
                    binding.Bindings.Add(new Binding(nameof(KMediaElement.MediaPosition)) { Source = MediaElement });
                    binding.Bindings.Add(new Binding(nameof(KMediaElement.MediaDuration)) { Source = MediaElement });
                }
                else
                {
                    binding = new MultiBinding()
                    {
                        StringFormat = DefaultMediaPositionStringFormatProperty,
                    };
                    binding.Bindings.Add(new Binding(nameof(KMediaElement.Position)) { Source = MediaElement });
                    binding.Bindings.Add(new Binding(nameof(KMediaElement.Duration)) { Source = MediaElement });
                }

                BindingOperations.SetBinding(tb, TextBlock.TextProperty, binding);
            }
        }

        internal void OnMarkerSliderValueChanged(KTimelineSlider slider, double newValue)
        {
            if (slider.IsMouseCaptureWithin) // Drag & drop en cours
                SetCurrentValue(CurrentPositionProperty, (long)newValue);
        }

        internal void OnLowerMarkerSliderMouseLeftButtonDown(object datacontext)
        {
            RaiseEvent(new RoutedEventArgs(StartMarkerClickEvent, datacontext));
            SelectedMarker = datacontext;
        }

        internal void OnUpperMarkerSliderMouseLeftButtonDown(object datacontext)
        {
            RaiseEvent(new RoutedEventArgs(EndMarkerClickEvent, datacontext));
            SelectedMarker = datacontext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the media element.
        /// </summary>
        /// <value>
        /// The media element.
        /// </value>
        public KMediaElement MediaElement { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si la lecture est en cours.
        /// </summary>
        public bool IsPlaying =>
            MediaElement?.PlayState == PlayState.Playing;

        /// <summary>
        /// Gets or sets the sources.
        /// </summary>
        /// <value>
        /// The sources.
        /// </value>
        public IEnumerable Sources
        {
            get => (IEnumerable)GetValue(SourcesProperty);
            set => SetValue(SourcesProperty, value);
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance has source.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has source; otherwise, <c>false</c>.
        /// </value>
        public bool HasSource =>
            (bool)GetValue(HasSourceProperty);

        /// <summary>
        /// Gets or sets the detail template.
        /// </summary>
        /// <value>
        /// The detail template.
        /// </value>
        public DataTemplate DetailTemplate
        {
            get => (DataTemplate)GetValue(DetailTemplateProperty);
            set => SetValue(DetailTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the next command.
        /// </summary>
        /// <value>
        /// The next command.
        /// </value>
        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the previous command.
        /// </summary>
        /// <value>
        /// The previous command.
        /// </value>
        public ICommand PreviousCommand
        {
            get => (ICommand)GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        public long CurrentPosition
        {
            get => (long)GetValue(CurrentPositionProperty);
            set => SetValue(CurrentPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the sources path.
        /// </summary>
        /// <value>
        /// The sources path.
        /// </value>
        public string SourcesPath
        {
            get => (string)GetValue(SourcesPathProperty);
            set => SetValue(SourcesPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the sources template.
        /// </summary>
        /// <value>
        /// The sources template.
        /// </value>
        public DataTemplate SourcesTemplate
        {
            get => (DataTemplate)GetValue(SourcesTemplateProperty);
            set => SetValue(SourcesTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected source.
        /// </summary>
        public object SelectedSource
        {
            get => GetValue(SelectedSourceProperty);
            set => SetValue(SelectedSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the current media info.
        /// </summary>
        /// <value>
        /// The current media info.
        /// </value>
        public MediaInfo CurrentMediaInfo
        {
            get => (MediaInfo)GetValue(CurrentMediaInfoProperty);
            set => SetValue(CurrentMediaInfoProperty, value);
        }

        /// <summary>
        /// Gets or sets the end binding path.
        /// </summary>
        /// <value>
        /// The end binding path.
        /// </value>
        public string EndBindingPath
        {
            get => (string)GetValue(EndBindingPathProperty);
            set => SetValue(EndBindingPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the start binding path.
        /// </summary>
        /// <value>
        /// The start binding path.
        /// </value>
        public string StartBindingPath
        {
            get => (string)GetValue(StartBindingPathProperty);
            set => SetValue(StartBindingPathProperty, value);
        }

        /// <summary>
        /// Gets or sets the markers.
        /// </summary>
        /// <value>
        /// The markers.
        /// </value>
        public IEnumerable Markers
        {
            get => (IEnumerable)GetValue(MarkersProperty);
            set => SetValue(MarkersProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected marker.
        /// </summary>
        /// <value>
        /// The selected marker.
        /// </value>
        public object SelectedMarker
        {
            get => GetValue(SelectedMarkerProperty);
            set => SetValue(SelectedMarkerProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has selected marker.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has selected marker; otherwise, <c>false</c>.
        /// </value>
        public bool HasSelectedMarker
        {
            get => (bool)GetValue(HasSelectedMarkerProperty);
            set => SetValue(HasSelectedMarkerProperty, value);
        }

        /// <summary>
        /// Gets or sets the HasAnyMarker.
        /// </summary>
        public bool HasAnyMarker
        {
            get => (bool)GetValue(HasAnyMarkerProperty);
            set => SetValue(HasAnyMarkerProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="HasAnyMarker"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasAnyMarkerProperty =
            DependencyProperty.Register(nameof(HasAnyMarker), typeof(bool), typeof(KMediaPlayer), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public long Duration
        {
            get => (long)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        /// <summary>
        /// Gets or sets the Jump buttons visibility.
        /// </summary>
        public Visibility JumpButtonsVisibility
        {
            get => (Visibility)GetValue(JumpButtonsVisibilityProperty);
            set => SetValue(JumpButtonsVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="JumpButtonsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty JumpButtonsVisibilityProperty =
            DependencyProperty.Register(nameof(JumpButtonsVisibility), typeof(Visibility), typeof(KMediaPlayer), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the markers list item container style (default type ListBoxItem).
        /// </summary>
        public Style MarkerItemContainerStyle
        {
            get => (Style)GetValue(MarkerItemContainerStyleProperty);
            set => SetValue(MarkerItemContainerStyleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="MarkerItemContainerStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerItemContainerStyleProperty =
            DependencyProperty.Register(nameof(MarkerItemContainerStyle), typeof(Style), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for the videos selector.
        /// </summary>
        public Style VideosSelectorStyle
        {
            get => (Style)GetValue(VideosSelectorStyleProperty);
            set => SetValue(VideosSelectorStyleProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="VideosSelectorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VideosSelectorStyleProperty =
            DependencyProperty.Register(nameof(VideosSelectorStyle), typeof(Style), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the visibility for the videos selector.
        /// </summary>
        public Visibility VideosSelectorVisibility
        {
            get => (Visibility)GetValue(VideosSelectorVisibilityProperty);
            set => SetValue(VideosSelectorVisibilityProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="VideosSelectorVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VideosSelectorVisibilityProperty =
            DependencyProperty.Register(nameof(VideosSelectorVisibility), typeof(Visibility), typeof(KMediaPlayer), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Gets or sets the TimelineZoom.
        /// </summary>
        public double TimelineZoom
        {
            get => (double)GetValue(TimelineZoomProperty);
            set => SetValue(TimelineZoomProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="TimelineZoom"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimelineZoomProperty =
            DependencyProperty.Register(nameof(TimelineZoom), typeof(double), typeof(KMediaPlayer), new UIPropertyMetadata(1.0d, OnTimelineZoomChanged, OnCoerceTimelineZoom));


        /// <summary>
        /// Gets or sets the TimelineOffset.
        /// </summary>
        public long TimelineOffset
        {
            get => (long)GetValue(TimelineOffsetProperty);
            set => SetValue(TimelineOffsetProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="TimelineOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimelineOffsetProperty =
            DependencyProperty.Register(nameof(TimelineOffset), typeof(long), typeof(KMediaPlayer), new UIPropertyMetadata(DefaultTimelineOffset, OnTimelineOffsetChanged, OnCoerceTimelineOffset));
        /// <summary>
        /// Le décalage par défaut pour la propriété de dépendance <see cref="TimelineOffsetProperty"/>.
        /// </summary>
        const long DefaultTimelineOffset = 0L;

        /// <summary>
        /// Gets the TimelineComputedWidth.
        /// </summary>
        public double TimelineComputedWidth
        {
            get => (double)GetValue(TimelineComputedWidthProperty);
            set => SetValue(TimelineComputedWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TimelineComputedWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimelineComputedWidthProperty =
            DependencyProperty.Register(nameof(TimelineComputedWidth), typeof(double), typeof(KMediaPlayer), new UIPropertyMetadata(0.0d));

        /// <summary>
        /// Gets or sets the AreMarkersLinked.
        /// </summary>
        public bool AreMarkersLinked
        {
            get => (bool)GetValue(AreMarkersLinkedProperty);
            set => SetValue(AreMarkersLinkedProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="AreMarkersLinked"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AreMarkersLinkedProperty =
            DependencyProperty.Register(nameof(AreMarkersLinked), typeof(bool), typeof(KMediaPlayer),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or sets the MarkersLinkedVisibility.
        /// </summary>
        public Visibility MarkersLinkedVisibility
        {
            get => (Visibility)GetValue(MarkersLinkedVisibilityProperty);
            set => SetValue(MarkersLinkedVisibilityProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="MarkersLinkedVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkersLinkedVisibilityProperty =
            DependencyProperty.Register(nameof(MarkersLinkedVisibility), typeof(Visibility), typeof(KMediaPlayer), new UIPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le son est assourdi.
        /// </summary>
        public bool IsMuted
        {
            get => (bool)GetValue(IsMutedProperty);
            set => SetValue(IsMutedProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsMuted"/>.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register(nameof(IsMuted), typeof(bool), typeof(KMediaPlayer),
            new UIPropertyMetadata(false, OnIsMutedChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsMuted"/> a changé.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="e">Les <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMediaPlayer)d;
            var newValue = (bool)e.NewValue;
            if (newValue)
                source.Mute();
            else
                source.Unmute();
        }

        /// <summary>
        /// Gets or sets the duration used when moving by steps.
        /// </summary>
        public long MoveStepDuration
        {
            get => (long)GetValue(MoveStepDurationProperty);
            set => SetValue(MoveStepDurationProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="MoveStepDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveStepDurationProperty =
            DependencyProperty.Register(nameof(MoveStepDuration), typeof(long), typeof(KMediaPlayer),
            new UIPropertyMetadata(TimeSpan.FromSeconds(1).Ticks));

        /// <summary>
        /// Gets or sets the TimeToStringFormatter.
        /// </summary>
        public ITimeToStringFormatter TimeToStringFormatter
        {
            get { return (ITimeToStringFormatter)GetValue(TimeToStringFormatterProperty); }
            set { SetValue(TimeToStringFormatterProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TimeToStringFormatter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TimeToStringFormatterProperty =
            DependencyProperty.Register(nameof(TimeToStringFormatter), typeof(ITimeToStringFormatter), typeof(KMediaPlayer), new UIPropertyMetadata(OnTimeToStringFormatterChanged));

        static void OnTimeToStringFormatterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMediaPlayer)d;
            source.UpdatePositionStringFormat();
        }

        /// <summary>
        /// Obtient ou définit la visibilité du bouton Stop.
        /// </summary>
        public Visibility StopVisibility
        {
            get => (Visibility)GetValue(StopVisibilityProperty);
            set => SetValue(StopVisibilityProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="StopVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty StopVisibilityProperty =
            DependencyProperty.Register(nameof(StopVisibility), typeof(Visibility), typeof(KMediaPlayer), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Obtient le temps du point le plus à gauche de la timeline.
        /// </summary>
        public long TimelineLeftTime
        {
            get => (long)GetValue(TimelineLeftTimeProperty);
            private set => SetValue(TimelineLeftTimePropertyKey, value);
        }
        /// <summary>
        /// Identifie la clé propriété de dépendance <see cref="TimelineLeftTime"/>.
        /// </summary>
        static readonly DependencyPropertyKey TimelineLeftTimePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TimelineLeftTime), typeof(long), typeof(KMediaPlayer), new UIPropertyMetadata(0L));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineLeftTime"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineLeftTimeProperty = TimelineLeftTimePropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient le temps de gauche de la timeline une fois formatté.
        /// </summary>
        public string TimelineLeftTimeFormatted
        {
            get => (string)GetValue(TimelineLeftTimeFormattedProperty);
            private set => SetValue(TimelineLeftTimeFormattedPropertyKey, value);
        }
        /// <summary>
        /// Identifie la clé de propriété de dépendance <see cref="TimelineLeftTimeFormatted"/>.
        /// </summary>
        static readonly DependencyPropertyKey TimelineLeftTimeFormattedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TimelineLeftTimeFormatted), typeof(string), typeof(KMediaPlayer), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineLeftTimeFormatted"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineLeftTimeFormattedProperty = TimelineLeftTimeFormattedPropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient le temps du point le plus à droite de la timeline.
        /// </summary>
        public long TimelineRightTime
        {
            get => (long)GetValue(TimelineRightTimeProperty);
            private set => SetValue(TimelineRightTimePropertyKey, value);
        }
        /// <summary>
        /// Identifie la clé propriété de dépendance <see cref="TimelineRightTime"/>.
        /// </summary>
        static readonly DependencyPropertyKey TimelineRightTimePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TimelineRightTime), typeof(long), typeof(KMediaPlayer), new UIPropertyMetadata(0L));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineRightTime"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineRightTimeProperty = TimelineRightTimePropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient le temps de droite de la timeline une fois formatté.
        /// </summary>
        public string TimelineRightTimeFormatted
        {
            get => (string)GetValue(TimelineRightTimeFormattedProperty);
            private set => SetValue(TimelineRightTimeFormattedPropertyKey, value);
        }
        /// <summary>
        /// Identifie la clé de propriété de dépendance <see cref="TimelineRightTimeFormatted"/>.
        /// </summary>
        static readonly DependencyPropertyKey TimelineRightTimeFormattedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TimelineRightTimeFormatted), typeof(string), typeof(KMediaPlayer), new UIPropertyMetadata(null));
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineRightTimeFormatted"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineRightTimeFormattedProperty = TimelineRightTimeFormattedPropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient ou définit le décalage de scrolling de la timeline (horizontal).
        /// </summary>
        public double TimelineScrollOffset
        {
            get => (double)GetValue(TimelineScrollOffsetProperty);
            set => SetValue(TimelineScrollOffsetProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="TimelineScrollOffset"/>.
        /// </summary>
        public static readonly DependencyProperty TimelineScrollOffsetProperty =
            DependencyProperty.Register(nameof(TimelineScrollOffset), typeof(double), typeof(KMediaPlayer),
            new UIPropertyMetadata(0.0, OnTimelineScrollOffsetChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="TimelineScrollOffset"/> a changé.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">Les <see cref="DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        static void OnTimelineScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMediaPlayer)d;
            source._timelineScrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        /// <summary>
        /// Obtient ou définit la commande associée au bouton vignette.
        /// </summary>
        public ICommand ThumbnailCommand
        {
            get => (ICommand)GetValue(ThumbnailCommandProperty);
            set => SetValue(ThumbnailCommandProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ThumbnailCommand"/>.
        /// </summary>
        public static readonly DependencyProperty ThumbnailCommandProperty =
            DependencyProperty.Register(nameof(ThumbnailCommand), typeof(ICommand), typeof(KMediaPlayer),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit .
        /// </summary>
        public ICommand ThumbnailExternalCommand
        {
            get => (ICommand)GetValue(ThumbnailExternalCommandProperty);
            set => SetValue(ThumbnailExternalCommandProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ThumbnailExternalCommand"/>.
        /// </summary>
        public static readonly DependencyProperty ThumbnailExternalCommandProperty =
            DependencyProperty.Register(nameof(ThumbnailExternalCommand), typeof(ICommand), typeof(KMediaPlayer),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Obtient ou définit la visibilité du bouton vignette.
        /// </summary>
        public Visibility ThumbnailVisibility
        {
            get => (Visibility)GetValue(ThumbnailVisibilityProperty);
            set => SetValue(ThumbnailVisibilityProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ThumbnailVisibility"/>.
        /// </summary>
        public static readonly DependencyProperty ThumbnailVisibilityProperty =
            DependencyProperty.Register(nameof(ThumbnailVisibility), typeof(Visibility), typeof(KMediaPlayer),
            new UIPropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Obtient ou définit uen valeur indiquant si le marker actuel a une image..
        /// </summary>
        public bool HasThumbnail
        {
            get => (bool)GetValue(HasThumbnailProperty);
            set => SetValue(HasThumbnailProperty, value);
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HasThumbnail"/>.
        /// </summary>
        public static readonly DependencyProperty HasThumbnailProperty =
            DependencyProperty.Register(nameof(HasThumbnail), typeof(bool), typeof(KMediaPlayer),
            new UIPropertyMetadata(false));


        #endregion

        #region DPs

        /// <summary>
        /// Defines the DP SourcesProperty
        /// </summary>
        public static readonly DependencyProperty SourcesProperty =
            DependencyProperty.Register(nameof(Sources), typeof(IEnumerable), typeof(KMediaPlayer), new UIPropertyMetadata(null, OnSourcesPropertyChanged));

        /// <summary>
        /// Defines the DP SourceProperty
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(Uri), typeof(KMediaPlayer), new UIPropertyMetadata(OnSourcePropertyChanged));

        /// <summary>
        /// Defines the key HasSourcePropertyKey
        /// </summary>
        static readonly DependencyPropertyKey HasSourcePropertyKey
           = DependencyProperty.RegisterReadOnly(nameof(HasSource), typeof(bool), typeof(KMediaPlayer),
                                                 new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines the DP HasSourceProperty
        /// </summary>
        public static readonly DependencyProperty HasSourceProperty
            = HasSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the key CurrentPositionPropertyKey
        /// </summary>
        static readonly DependencyProperty CurrentPositionProperty
           = DependencyProperty.Register(nameof(CurrentPosition), typeof(long), typeof(KMediaPlayer),
                                                 new FrameworkPropertyMetadata(0L, OnCurrentPositionChanged));

        static void OnCurrentPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = (KMediaPlayer)d;
            player.BringTickIntoTimelineView((long)e.NewValue);
            var view = player._thumbnailView;
            if (view.Visibility == Visibility.Visible)
            {
                view.Visibility = Visibility.Collapsed;
                view.Source = null;
                player.SetVideoVisibility(true);
            }
        }

        /// <summary>
        /// Defines the DP DetailTemplateProperty.
        /// </summary>
        public static readonly DependencyProperty DetailTemplateProperty =
            DependencyProperty.Register(nameof(DetailTemplate), typeof(DataTemplate), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP NextCommandProperty.
        /// </summary>
        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP PreviousCommandProperty.
        /// </summary>
        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register(nameof(PreviousCommand), typeof(ICommand), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP SourcesPathProperty
        /// </summary>
        public static readonly DependencyProperty SourcesPathProperty =
            DependencyProperty.Register(nameof(SourcesPath), typeof(string), typeof(KMediaPlayer), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Identifies the <see cref="SelectedSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedSourceProperty =
            DependencyProperty.Register(nameof(SelectedSource), typeof(object), typeof(KMediaPlayer),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedSourceChanged));

        /// <summary>
        /// Defines the DP SourcesTemplateProperty
        /// </summary>
        public static readonly DependencyProperty SourcesTemplateProperty =
            DependencyProperty.Register(nameof(SourcesTemplate), typeof(DataTemplate), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP CurrentMediaInfoProperty
        /// </summary>
        public static readonly DependencyProperty CurrentMediaInfoProperty =
            DependencyProperty.Register(nameof(CurrentMediaInfo), typeof(MediaInfo), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP StartBindingPathProperty
        /// </summary>
        public static readonly DependencyProperty StartBindingPathProperty =
            DependencyProperty.Register(nameof(StartBindingPath), typeof(string), typeof(KMediaPlayer), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Defines the DP EndBindingPathProperty
        /// </summary>
        public static readonly DependencyProperty EndBindingPathProperty =
            DependencyProperty.Register(nameof(EndBindingPath), typeof(string), typeof(KMediaPlayer), new UIPropertyMetadata(String.Empty));

        /// <summary>
        /// Defines the DP MarkersProperty
        /// </summary>
        public static readonly DependencyProperty MarkersProperty =
            DependencyProperty.Register(nameof(Markers), typeof(IEnumerable), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Defines the DP SelectedMarkerProperty
        /// </summary>
        public static readonly DependencyProperty SelectedMarkerProperty =
            DependencyProperty.Register(nameof(SelectedMarker), typeof(object), typeof(KMediaPlayer), new UIPropertyMetadata(null, OnSelectedMarkerPropertyChanged));

        static void OnSelectedMarkerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            d.SetValue(HasSelectedMarkerProperty, o.NewValue != null);
        }

        /// <summary>
        /// Defines the DP HasSelectedMarkerProperty
        /// </summary>
        public static readonly DependencyProperty HasSelectedMarkerProperty =
            DependencyProperty.Register(nameof(HasSelectedMarker), typeof(bool), typeof(KMediaPlayer), new UIPropertyMetadata(false));

        /// <summary>
        /// Defines the DP DurationProperty
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register(nameof(Duration), typeof(long), typeof(KMediaPlayer), new UIPropertyMetadata(0L));

        /// <summary>
        /// Defines the DP IsReadOnlyProperty
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(KMediaPlayer), new UIPropertyMetadata(false, OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// Gets or sets the command executed when the player starts playing.
        /// </summary>
        public ICommand PlayCommand
        {
            get => (ICommand)GetValue(PlayCommandProperty);
            set => SetValue(PlayCommandProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="PlayCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlayCommandProperty =
            DependencyProperty.Register(nameof(PlayCommand), typeof(ICommand), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command executed when the position in the video has been seeked.
        /// </summary>
        public ICommand SeekCommand
        {
            get => (ICommand)GetValue(SeekCommandProperty);
            set => SetValue(SeekCommandProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="SeekCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeekCommandProperty =
            DependencyProperty.Register(nameof(SeekCommand), typeof(ICommand), typeof(KMediaPlayer), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command executed when a screen mode change is requested.
        /// </summary>
        public ICommand ScreenModeCommand
        {
            get => (ICommand)GetValue(ScreenModeCommandProperty);
            set => SetValue(ScreenModeCommandProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="ScreenModeCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScreenModeCommandProperty =
            DependencyProperty.Register(nameof(ScreenModeCommand), typeof(ICommand), typeof(KMediaPlayer),
            new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the screen mode visibility.
        /// </summary>
        public Visibility ScreenModeVisibility
        {
            get => (Visibility)GetValue(ScreenModeVisibilityProperty);
            set => SetValue(ScreenModeVisibilityProperty, value);
        }
        /// <summary>
        /// Identifies the <see cref="ScreenModeVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScreenModeVisibilityProperty =
            DependencyProperty.Register(nameof(ScreenModeVisibility), typeof(Visibility), typeof(KMediaPlayer),
            new UIPropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Public methods

        /// <summary>
        /// Plays the media
        /// </summary>
        public void Play()
        {
            if (MediaElement != null)
            {
                if (PlayCommand != null && PlayCommand.CanExecute(null))
                    PlayCommand.Execute(null);

                MediaElement.Play();
            }

            UpdateVisualStates(true);
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        public void Pause()
        {
            if (MediaElement != null)
                MediaElement.Pause();

            UpdateVisualStates(true);
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        public void Stop()
        {
            if (MediaElement != null)
                MediaElement.Stop();

            UpdateVisualStates(true);
        }

        /// <summary>
        /// Mutes the sound.
        /// </summary>
        public void Mute()
        {
            if (MediaElement != null)
                MediaElement.Volume = 0d;

            IsMuted = true;

            UpdateVisualStates(true);
        }

        /// <summary>
        /// Unmutes the sound.
        /// </summary>
        public void Unmute()
        {
            if (MediaElement != null)
                MediaElement.Volume = 1d;

            IsMuted = false;

            UpdateVisualStates(true);
        }

        /// <summary>
        /// Switches between muted and unmuted states.
        /// </summary>
        public void ToggleMute()
        {
            if (MediaElement != null)
            {
                if (MediaElement.Volume > 0d)
                    Unmute();
                else
                    Mute();
            }
        }

        public void SetVideoVisibility(bool vis) =>
            MediaElement.VideoImage.Visibility = vis ? Visibility.Visible : Visibility.Hidden;

        public void ShowThumbnailView(KAction action)
        {
            if (action?.Thumbnail == null)
            {
                _thumbnailView.Source = null;
                _thumbnailView.Visibility = Visibility.Collapsed;
                SetVideoVisibility(true);
                _videosSelector.Visibility = action?.VideoId == null ? Visibility.Visible : Visibility.Hidden;
            }
            else
            {
                if (action.Thumbnail.IsNotMarkedAsUnchanged)
                    BindingOperations.SetBinding(_thumbnailView, Image.SourceProperty, new Binding(nameof(CloudFile.LocalUri))
                    {
                        Source = action.Thumbnail,
                        Converter = UriToCachedImageConverter.Instance
                    });
                else
                    BindingOperations.SetBinding(_thumbnailView, Image.SourceProperty, new Binding(nameof(CloudFile.Uri)) { Source = action.Thumbnail});
                _thumbnailView.Visibility = Visibility.Visible;
                SetVideoVisibility(false);
                _videosSelector.Visibility = Visibility.Hidden;
            }
        }
        public void HideVideoSelector()
        {
            _thumbnailView.Visibility = Visibility.Collapsed;
            SetVideoVisibility(true);
            _videosSelector.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Met à jour la propriété <see cref="CurrentMediaInfo"/> en fonction de la vidéo actuelle.
        /// </summary>
        public void UpdateMediaInfo()
        {
            if (Source != null)
            {
                var uri = Source.Scheme == "file" ? Source.LocalPath : Source.AbsoluteUri;
                CurrentMediaInfo = MediaDetector.GetMediaInfo(uri);
            }
            else
                CurrentMediaInfo = null;
        }

        /// <summary>
        /// Moves to a step forward.
        /// </summary>
        public void StepForward()
        {
            if (MediaElement.MediaDuration >= (MediaElement.MediaPosition + MoveStepDuration))
                MediaElement.MediaPosition += MoveStepDuration;
        }

        /// <summary>
        /// Moves to a step backward.
        /// </summary>
        public void StepBackward()
        {
            if (MediaElement.MediaPosition >= MoveStepDuration)
                MediaElement.MediaPosition -= MoveStepDuration;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Obtient la durée entre deux clics pour considérer un double clic.
        /// </summary>
        /// <returns>La durée en ms</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();

        static void OnSelectedSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMediaPlayer)d;
            if (source._videosSelector != null)
                source._videosSelector.SelectedItem = e.NewValue;
        }

        void _videosSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetCurrentValue(SelectedSourceProperty, _videosSelector.SelectedItem);
            Source = (Uri)_videosSelector.SelectedValue;
            _videosSelector.Visibility = Visibility.Hidden;
        }

        static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            d.SetValue(HasSourcePropertyKey, o.NewValue != null);

            var player = (KMediaPlayer)d;

            // If not bound, use source duration
            if (BindingOperations.GetBinding(d, DurationProperty) == null)
                player.SetBinding(DurationProperty, new Binding(nameof(KMediaElement.MediaDuration)) { Source = player.MediaElement, Mode = BindingMode.OneWay });

            player.UpdateVisualStates(true);
        }

        static void OnSourcesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            d.SetValue(SourceProperty, null);
        }

        static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
        }

        void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            UpdateTimelineScroll(true);
        }

        void MediaElement_MediaStateChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisualStates(true);
        }

        void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
        }

        void KMediaPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= KMediaPlayer_Unloaded;

            if (MediaElement != null)
            {
                MediaElement.MediaFailed -= MediaElement_MediaFailed;
                MediaElement.Close();
            }
        }

        void MediaElement_MediaFailed(object sender, RoutedMediaFailedEventArgs e)
        {
            UpdateVisualStates(true);
        }

        TControl GetTemplateChild<TControl>(string name)
            where TControl : FrameworkElement
        {
            var control = GetTemplateChild(name) as TControl;
            return control;
        }

        /// <summary>
        /// Met à jour les VisualStates.
        /// </summary>
        /// <param name="useTransitions"><c>true</c> pour utiliser les transitions.</param>
        void UpdateVisualStates(bool useTransitions)
        {
            if (MediaElement != null)
            {
                if (MediaElement.Volume == 0d)
                    VisualStateManager.GoToState(this, VSM_Volume_IsOff, useTransitions);
                else
                    VisualStateManager.GoToState(this, VSM_Volume_IsOn, useTransitions);

                if (Source == null || string.IsNullOrEmpty(Source.OriginalString))
                    VisualStateManager.GoToState(this, VSM_State_IsStopped, useTransitions);
                else
                {
                    switch (MediaElement.PlayState)
                    {
                        case PlayState.Playing:
                            VisualStateManager.GoToState(this, VSM_State_IsPlaying, useTransitions);
                            break;
                        case PlayState.Paused:
                            VisualStateManager.GoToState(this, VSM_State_IsPaused, useTransitions);
                            break;
                        case PlayState.Stopped:
                            VisualStateManager.GoToState(this, VSM_State_IsStopped, useTransitions);
                            break;
                    }
                }
            }
            else
            {
                VisualStateManager.GoToState(this, VSM_Volume_IsOn, useTransitions);
                VisualStateManager.GoToState(this, VSM_State_IsStopped, useTransitions);
            }
        }


        #endregion

        #region Timeline Scroll management

        bool _hasJustCoercedTimelineOffset;
        bool _coerceTimelineOffset;
        (long Start, long End, bool AddMargins)? _nextStartEndToApplyWhenLoaded;
        bool _ignoreZoomUpdate;

        /// <summary>
        /// Scales and offsets the timeline so the two values are shown at the start and the end.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="addMargins"><c>true</c> to add margins.</param>
        public void TimelineShow(long start, long end, bool addMargins)
        {
            if (end < start)
                throw new ArgumentException("end must be greated than start");

            if (!MediaElement.IsMediaOpened)
            {
                _nextStartEndToApplyWhenLoaded = (start, end, addMargins);
                return;
            }

            var totalDuration = GetDurationOrEmpty();

            if (totalDuration == 0)
                return;

            var offsetRatio = TimelineMinimumOffset / ActualWidth;
            var zoom = totalDuration / (end - start);

            var zoomWithMargins = addMargins ? zoom - zoom * offsetRatio * 2 : zoom;
            var startWithMargins = addMargins ? start - (long)(start * offsetRatio / zoomWithMargins) : start;

            if (TimelineOffset != startWithMargins)
                _ignoreZoomUpdate = true;

            TimelineZoom = zoomWithMargins;

            _ignoreZoomUpdate &= TimelineOffset == startWithMargins;

            TimelineOffset = startWithMargins;
        }

        /// <summary>
        /// Resets the timeline Scale and offset.
        /// </summary>
        public void TimelineReset()
        {
            _ignoreZoomUpdate = true; // Permet de n'effectuer le scroll qu'une fois
            ClearValue(TimelineZoomProperty);
            _ignoreZoomUpdate = false;

            if (TimelineOffset != DefaultTimelineOffset)
                ClearValue(TimelineOffsetProperty);
            else if (IsLoaded)
                UpdateTimelineScroll();
        }

        /// <summary>
        /// Coerces the value of the <see cref="TimelineZoom"/> property.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The coerced value.</returns>
        static object OnCoerceTimelineZoom(DependencyObject d, object baseValue)
        {
            var val = (double)baseValue;
            return val <= 1.0 ? 1.0 : val;
        }

        /// <summary>
        /// Called when the value of <see cref="TimelineZoom"/> has changed.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event args.</param>
        static void OnTimelineZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!((KMediaPlayer)d)._ignoreZoomUpdate)
                ((KMediaPlayer)d).UpdateTimelineScroll();
        }

        /// <summary>
        /// Coerces the value of the <see cref="TimelineOffset"/> property.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns>The coerced value.</returns>
        static object OnCoerceTimelineOffset(DependencyObject d, object baseValue)
        {
            var source = (KMediaPlayer)d;
            var baseVal = (long)baseValue;
            long newVal;

            if (source._coerceTimelineOffset)
            {
                newVal = source.GetCoercedTimelineOffset();
                source._coerceTimelineOffset = false;
            }
            else
                newVal = baseVal <= 0 ? 0 : baseVal;

            if (baseVal != newVal)
            {
                source._hasJustCoercedTimelineOffset = true;
                return newVal;
            }
            source._hasJustCoercedTimelineOffset = true;
            return baseValue;
        }

        /// <summary>
        /// Called when the value of <see cref="TimelineOffset"/> has changed..
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event args.</param>
        static void OnTimelineOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMediaPlayer)d;
            if (source._hasJustCoercedTimelineOffset)
            {
                source.UpdateTimelineScroll();
                source._hasJustCoercedTimelineOffset = false;
            }
        }

        /// <summary>
        /// Handles the PreviewMouseWheel event of the TimelineScrollViewer control.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        void TimelineScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double delta;

                if (e.Delta < 0)
                    delta = TimelineZoom / 1.2 - TimelineZoom;
                else
                    delta = TimelineZoom * 1.2 - TimelineZoom;

                TimelineZoom += delta;
                TimelineOffset += ScrollOffsetConvert((e.GetPosition(_timelineScrollViewer).X) * delta);

            }
            else
            {
                bool toRight = e.Delta < 0;

                var totalLength = MediaElement != null ? MediaElement.Duration.Ticks : 0;
                if (toRight)
                    TimelineOffset += Convert.ToInt64(Convert.ToDouble(totalLength) / TimelineZoom * .1);
                else
                    TimelineOffset -= Convert.ToInt64(Convert.ToDouble(totalLength) / TimelineZoom * .1);
            }
        }

        /// <summary>
        /// Updates the Timeline scroll computed values.
        /// </summary>
        void UpdateTimelineScroll(bool checkValuesOnLoaded = false)
        {
            if (!MediaElement.IsMediaOpened)
                return;

            if (checkValuesOnLoaded && _nextStartEndToApplyWhenLoaded.HasValue)
            {
                var start = _nextStartEndToApplyWhenLoaded.Value.Start;
                var end = _nextStartEndToApplyWhenLoaded.Value.End;
                var addMargins = _nextStartEndToApplyWhenLoaded.Value.AddMargins;
                _nextStartEndToApplyWhenLoaded = null;
                TimelineShow(start, end, addMargins);
            }

            var totalDuration = GetDurationOrEmpty();

            var lastTimelineHorizontalOffsetNotAnimated = _timelineHorizontalOffsetNotAnimated;
            var lastTimelineComputedWidthNotAnimated = _timelineComputedWidthNotAnimated;
            _timelineComputedWidthNotAnimated = _timelineScrollViewer.ViewportWidth * TimelineZoom;
            _timelineHorizontalOffsetNotAnimated = TimelineOffsetConvert(TimelineOffset);

            // Mettre à jour les temps de droite et de gauche
            TimelineLeftTime = TimelineOffset;
            TimelineRightTime = ScrollOffsetConvert(_timelineHorizontalOffsetNotAnimated + _timelineScrollViewer.ViewportWidth);

            if (TimeToStringFormatter != null)
            {
                TimelineLeftTimeFormatted = TimeToStringFormatter.FormatTime(TimelineLeftTime);
                TimelineRightTimeFormatted = TimeToStringFormatter.FormatTime(TimelineRightTime);
            }

            // Animer
            if (_moveTimeLineScrollViewerStoryBoard == null)
                _moveTimeLineScrollViewerStoryBoard = new Storyboard();
            else
                _moveTimeLineScrollViewerStoryBoard.SkipToFill(this);

            _moveTimeLineScrollViewerStoryBoard.Children.Clear();

            var _timelineOffsetAnimation = new DoubleAnimation
            {
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut },
                To = _timelineHorizontalOffsetNotAnimated
            };

            var _timelineZoomAnimation = new DoubleAnimation
            {
                EasingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut },
                To = _timelineComputedWidthNotAnimated
            };

            // Pour la durée, se baser sur le changement de zoom. Plus il est relativement petit, plus le temps sera court. Au max 500ms
            var relativeDurationChange = lastTimelineComputedWidthNotAnimated / _timelineComputedWidthNotAnimated;
            if (relativeDurationChange < 1)
                relativeDurationChange = 1 / relativeDurationChange;

            var durationMs = 100 * Math.Min(5, Math.Log(relativeDurationChange, 2));

            _timelineOffsetAnimation.Duration = TimeSpan.FromMilliseconds(durationMs);
            _timelineZoomAnimation.Duration = TimeSpan.FromMilliseconds(durationMs);

            Storyboard.SetTargetProperty(_timelineOffsetAnimation, new PropertyPath(nameof(TimelineScrollOffset)));
            _moveTimeLineScrollViewerStoryBoard.Children.Add(_timelineOffsetAnimation);
            Storyboard.SetTargetProperty(_timelineZoomAnimation, new PropertyPath(nameof(TimelineComputedWidth)));
            _moveTimeLineScrollViewerStoryBoard.Children.Add(_timelineZoomAnimation);

            _moveTimeLineScrollViewerStoryBoard.Begin(this, true);

            CoerceTimelineOffsetAsync();
        }

        /// <summary>
        /// Coerces the timeline offset asynchronously.
        /// </summary>
        void CoerceTimelineOffsetAsync()
        {
            _coerceTimelineOffset = true;
            Dispatcher.BeginInvoke((Action<DependencyProperty>)base.CoerceValue, DispatcherPriority.Loaded, TimelineOffsetProperty);
        }

        /// <summary>
        /// Get the coerced <see cref="TimelineOffset"/> value from the scrollviewer.
        /// </summary>
        /// <returns>The coerced value.</returns>
        long GetCoercedTimelineOffset()
        {
            var totalDuration = GetDurationOrEmpty();
            if (DoubleUtil.AreClose(_timelineComputedWidthNotAnimated, 0d))
                return 0;
            return ScrollOffsetConvert(_timelineHorizontalOffsetNotAnimated);
        }

        /// <summary>
        /// Gets the loaded media duration, or 0 if no media is loaded.
        /// </summary>
        /// <returns>The duration.</returns>
        long GetDurationOrEmpty() =>
            MediaElement != null ? MediaElement.MediaDuration : 0;

        /// <summary>
        /// Converts a tick offset to a WPF-offset.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>The offset</returns>
        double TimelineOffsetConvert(long tick)
        {
            var totalDuration = GetDurationOrEmpty();
            return totalDuration != 0 ?
                _timelineComputedWidthNotAnimated * Convert.ToDouble(tick) / Convert.ToDouble(totalDuration)
                : 0;
        }

        /// <summary>
        /// Converts a WPF-offset to a tick offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// The offset
        /// </returns>
        long ScrollOffsetConvert(double offset)
        {
            var totalDuration = GetDurationOrEmpty();
            return Convert.ToInt64(offset * totalDuration / (_timelineComputedWidthNotAnimated == 0 ? 1 : _timelineComputedWidthNotAnimated));
        }

        /// <summary>
        /// Brings the specified tick into the timeline view.
        /// </summary>
        /// <param name="tick">The tick.</param>
        void BringTickIntoTimelineView(long tick)
        {
            if (_timelineRangeBase != null)
            {
                if (tick < 0)
                    tick = 0;
                else if (tick > GetDurationOrEmpty())
                    tick = GetDurationOrEmpty();

                var leftOffset = _timelineHorizontalOffsetNotAnimated;
                var offsetToShow = TimelineOffsetConvert(tick);
                var rightOffset = _timelineHorizontalOffsetNotAnimated + _timelineScrollViewer.ViewportWidth;

                if (double.IsNaN(leftOffset) || double.IsNaN(rightOffset))
                    return;

                var offsetToShowAtLeft = offsetToShow;
                var offsetToShowAtRight = offsetToShow + TimelineMinimumOffset * 2;

                if (offsetToShowAtLeft < leftOffset)
                    TimelineOffset = ScrollOffsetConvert(offsetToShowAtLeft);
                else if (offsetToShowAtRight > rightOffset)
                    TimelineShow(ScrollOffsetConvert(leftOffset), ScrollOffsetConvert(offsetToShowAtRight), false);
            }
        }

        #endregion

        #region SecondsValueConverter nested class

        class TicksValueConverter : IValueConverter
        {

            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
                ((TimeSpan)value).Ticks;

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) =>
                throw new NotSupportedException();
        }

        #endregion
    }
}