using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Generic mini player
    /// </summary>
    [TemplatePart(Name = PART_MediaElement, Type = typeof(KMediaElement))]
    [TemplatePart(Name = PART_ThumbnailView, Type = typeof(Image))]
    [TemplatePart(Name = PART_ControlsMenu, Type = typeof(Grid))]
    [TemplatePart(Name = PART_Play, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Pause, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_Forward, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ResetSpeed, Type = typeof(ButtonBase))]
    [TemplateVisualState(GroupName = VSM_GRP_State, Name = VSM_State_IsPlaying)]
    [TemplateVisualState(GroupName = VSM_GRP_State, Name = VSM_State_IsPaused)]
    public class KMiniPlayer : Control, IMediaPlayer
    {
        #region Constants

        const string PART_MediaElement = "PART_MediaElement";
        const string PART_ThumbnailView = "PART_ThumbnailView";
        const string PART_ControlsMenu = "PART_ControlsMenu";
        const string PART_Play = "PART_Play";
        const string PART_Pause = "PART_Pause";
        const string PART_Backward = "PART_Backward";
        const string PART_Forward = "PART_Forward";
        const string PART_ResetSpeed = "PART_ResetSpeed";

        const string VSM_GRP_State = "State";
        const string VSM_State_IsPlaying = "IsPlaying";
        const string VSM_State_IsPaused = "IsPaused";

        readonly System.Windows.Media.DoubleCollection _speedRatios =
            (System.Windows.Media.DoubleCollection)new System.Windows.Media.DoubleCollectionConverter().ConvertFrom(KMediaPlayer.DefaultSpeedRatios);

        #endregion

        #region Attributes

        List<IAction> _actions = new List<IAction>();
        bool _internalCurrentActionFlag;
        IAction _currentAction;
        Grid _controlsMenu;
        Timer controlMenuTimer;
        Point lastMousePos;
        Image _thumbnailView;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="KMiniPlayer"/> class.
        /// </summary>
        static KMiniPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KMiniPlayer), new FrameworkPropertyMetadata(typeof(KMiniPlayer)));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Gets and sets up the mediaelement
            MediaElement = GetTemplateChild<KMediaElement>(PART_MediaElement);
            _thumbnailView = GetTemplateChild<Image>(PART_ThumbnailView);
            MediaElement.MediaFailed += new RoutedMediaFailedEventHandler(MediaElement_MediaFailed);

            // Get Control Menu and setup autohide
            _controlsMenu = GetTemplateChild<Grid>(PART_ControlsMenu);
            var videoSurface = GetTemplateChild<Grid>("VideoSurfaceGrid");
            videoSurface.MouseEnter += ShowControlMenu;
            videoSurface.MouseMove += (sender, e) =>
            {
                if (lastMousePos == null)
                {
                    lastMousePos = e.GetPosition(null);
                    ShowControlMenu(sender, e);
                }
                else
                {
                    Point mousePos = e.GetPosition(null);
                    Vector diff = lastMousePos - mousePos;
                    if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        ShowControlMenu(sender, e);
                        lastMousePos = mousePos;
                    }
                }
            };
            videoSurface.MouseLeave += HideControlMenu;

            // Subscribe to player's buttons
            GetTemplateChild<ButtonBase>(PART_Play).Click += (sender, e) => { Play(); };
            GetTemplateChild<ButtonBase>(PART_Pause).Click += (sender, e) => { Stop(); };

            GetTemplateChild<ButtonBase>(PART_Pause).Click += (sender, e) => { Stop(); };
            GetTemplateChild<ButtonBase>(PART_Pause).Click += (sender, e) => { Stop(); };

            GetTemplateChild<ButtonBase>(PART_ResetSpeed).Click += (sender, e) =>
            {
                ResetSpeedRatio();
            };

            GetTemplateChild<ButtonBase>(PART_Forward).Click += (sender, e) =>
            {
                IncreaseSpeedRatio();
            };
            GetTemplateChild<ButtonBase>(PART_Backward).Click += (sender, e) =>
            {
                DecreaseSpeedRatio();
            };

            if (IsMuted)
                Mute();
            else
                Unmute();

            // Starts in Pause mode
            VisualStateManager.GoToState(this, VSM_State_IsPaused, false);

            Unloaded += new RoutedEventHandler(KMiniPlayer_Unloaded);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the media element.
        /// </summary>
        /// <value>
        /// The media element.
        /// </value>
        public KMediaElement MediaElement
        {
            get;
            set;
        }

        long _internalPosition;
        /// <summary>
        /// Obtient ou définit la position interne.
        /// </summary>
        public long InternalPosition
        {
            get { return _internalPosition; }
            set
            {
                _internalPosition = value;
                Position = value;
            }
        }

        #endregion

        #region DPs

        /// <summary>
        /// Obtient une valeur indiquant si la lecture est en cours.
        /// </summary>
        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            private set { SetValue(IsPlayingPropertyKey, value); }
        }
        /// <summary>
        /// Identifies the <see cref="IsPlaying"/> dependency property key.
        /// </summary>
        static readonly DependencyPropertyKey IsPlayingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPlaying", typeof(bool), typeof(KMiniPlayer), new UIPropertyMetadata(false));
        /// <summary>
        /// Identifies the <see cref="IsPlaying"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPlayingProperty = IsPlayingPropertyKey.DependencyProperty;

        /// <summary>
        /// Obtient une valeur indiquant si la lecture doit boucler.
        /// </summary>
        public bool LoopPlaying
        {
            get { return (bool)GetValue(LoopPlayingProperty); }
            set { SetValue(LoopPlayingProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LoopPlaying"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoopPlayingProperty =
            DependencyProperty.Register("LoopPlaying", typeof(bool), typeof(KMiniPlayer), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        internal object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has source.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has source; otherwise, <c>false</c>.
        /// </value>
        public bool HasSource
        {
            get { return (bool)GetValue(HasSourceProperty); }
        }

        /// <summary>
        /// Gets or sets the source file binding path.
        /// </summary>
        /// <value>
        /// The source file binding path.
        /// </value>
        public string SourceFileBindingPath
        {
            get { return (string)GetValue(SourceFileBindingPathProperty); }
            set { SetValue(SourceFileBindingPathProperty, value); }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public long Position
        {
            get { return (long)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>
        /// The current item.
        /// </value>
        public IActionPath CurrentItem
        {
            get { return (IActionPath)GetValue(CurrentItemProperty); }
            set { SetValue(CurrentItemProperty, value); }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>
        /// The items source.
        /// </value>
        public IEnumerable<IActionPath> ItemsSource
        {
            get { return (IEnumerable<IActionPath>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the play button visibility.
        /// </summary>
        public Visibility PlayButtonVisibility
        {
            get { return (Visibility)GetValue(PlayButtonVisibilityProperty); }
            set { SetValue(PlayButtonVisibilityProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="PlayButtonVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlayButtonVisibilityProperty =
            DependencyProperty.Register("PlayButtonVisibility", typeof(Visibility), typeof(KMiniPlayer), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le son est assourdi.
        /// </summary>
        public bool IsMuted
        {
            get { return (bool)GetValue(IsMutedProperty); }
            set { SetValue(IsMutedProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsMuted"/>.
        /// </summary>
        public static readonly DependencyProperty IsMutedProperty =
            DependencyProperty.Register("IsMuted", typeof(bool), typeof(KMiniPlayer),
            new UIPropertyMetadata(false, OnIsMutedChanged));

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="IsMuted"/> a changé.
        /// </summary>
        /// <param name="d">La source.</param>
        /// <param name="e">Les <see cref="DependencyPropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        static void OnIsMutedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = (KMiniPlayer)d;
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
            get { return (long)GetValue(MoveStepDurationProperty); }
            set { SetValue(MoveStepDurationProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MoveStepDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MoveStepDurationProperty =
            DependencyProperty.Register("MoveStepDuration", typeof(long), typeof(KMiniPlayer),
            new UIPropertyMetadata(TimeSpan.FromSeconds(1).Ticks));

        /// <summary>
        /// Defines the DP SourceProperty
        /// </summary>
        internal static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(KMiniPlayer), new UIPropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// Defines the key HasSourcePropertyKey
        /// </summary>
        static readonly DependencyPropertyKey HasSourcePropertyKey
           = DependencyProperty.RegisterReadOnly("HasSource", typeof(bool), typeof(KMiniPlayer),
                                                 new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Defines the DP HasSourceProperty
        /// </summary>
        public static readonly DependencyProperty HasSourceProperty
            = HasSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// Defines the DP SourceFileBindingPathProperty
        /// </summary>
        public static readonly DependencyProperty SourceFileBindingPathProperty =
            DependencyProperty.Register("SourceFileBindingPath", typeof(string), typeof(KMiniPlayer), new UIPropertyMetadata($"{nameof(ActionPath.Action)}.{nameof(KAction.Video)}.{nameof(Video.Source)}"));

        /// <summary>
        /// Defines the DP Position
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(long), typeof(KMiniPlayer), new UIPropertyMetadata(0L, OnPositionPropertyChanged));

        /// <summary>
        /// Defines the DP CurrentItem
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register("CurrentItem", typeof(IActionPath), typeof(KMiniPlayer), new UIPropertyMetadata(OnCurrentItemPropertyChanged));

        /// <summary>
        /// Defines the DP ItemsSourceProperty
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<IActionPath>), typeof(KMiniPlayer), new UIPropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// Gets or sets the global speed ratio.
        /// </summary>
        public double SpeedRatio
        {
            get { return (double)GetValue(SpeedRatioProperty); }
            set { SetValue(SpeedRatioProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SpeedRatio"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SpeedRatioProperty =
            DependencyProperty.Register("SpeedRatio", typeof(double), typeof(KMiniPlayer),
            new UIPropertyMetadata(1.0, OnSpeedRatioChanged));

        #endregion

        #region Events

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
            "SpeedRatioChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(KMiniPlayer));

        #endregion

        #region CurrentActionChanged

        /// <summary>
        /// Occurs when the current action has changed.
        /// </summary>
        public event RoutedEventHandler CurrentActionChanged
        {
            add { AddHandler(CurrentActionChangedEvent, value); }
            remove { RemoveHandler(CurrentActionChangedEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CurrentActionChanged"/> event.
        /// </summary>
        public static readonly RoutedEvent CurrentActionChangedEvent = EventManager.RegisterRoutedEvent(
            "CurrentActionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(KMiniPlayer));

        #endregion

        #region Stopped

        /// <summary>
        /// Occurs when the play has stopped.
        /// </summary>
        public event RoutedEventHandler Stopped
        {
            add { AddHandler(StoppedEvent, value); }
            remove { RemoveHandler(StoppedEvent, value); }
        }
        /// <summary>
        /// identifies the <see cref="Stopped"/> event.
        /// </summary>
        public static readonly RoutedEvent StoppedEvent = EventManager.RegisterRoutedEvent(
            "Stopped", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(KMiniPlayer));



        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Plays the path
        /// </summary>
        public void Play()
        {
            // Try to set the current action if none
            if (CurrentAction == null)
                SetCurrentAction(true, true);

            // If none exists, get out
            if (CurrentAction == null)
                return;

            if (CurrentAction.IsLast && Position >= CurrentAction.End)
            {
                // Sets the first action to play if we were at the end
                var a = _actions.First();
                SeekAndResync(a.Start);
                CurrentAction = a;
            }

            IsPlaying = true;

            // Runs the action
            CurrentAction.LoadAndSync();
            RaiseCurrentActionChanged();
            CurrentAction.Run();

            // Goes to playing mode
            VisualStateManager.GoToState(this, VSM_State_IsPlaying, false);
        }

        /// <summary>
        /// Plays the media.
        /// </summary>
        public void Pause()
        {
            IsPlaying = false;

            if (CurrentAction != null)
                CurrentAction.Freeze();

            // Goes to pause mode
            VisualStateManager.GoToState(this, VSM_State_IsPaused, false);
        }

        /// <summary>
        /// Stops the player
        /// </summary>
        public void Stop()
        {
            IsPlaying = false;

            RaiseStopped();

            // Stops the internal player
            if (MediaElement != null)
                MediaElement.Stop();

            // Goes to pause mode
            VisualStateManager.GoToState(this, VSM_State_IsPaused, false);
        }

        /// <summary>
        /// Moves to a step forward.
        /// </summary>
        public void StepForward()
        {
            SeekAndResync(Position + MoveStepDuration);
        }

        /// <summary>
        /// Moves to a step backward.
        /// </summary>
        public void StepBackward()
        {
            SeekAndResync(Position - MoveStepDuration);
        }

        public void SetVideoVisibility(bool vis) =>
            MediaElement.VideoImage.Visibility = vis ? Visibility.Visible : Visibility.Hidden;

        public void ShowThumbnailView(KAction action)
        {
            if (action?.Thumbnail == null)
            {
                _thumbnailView.Source = null;
                _thumbnailView.Visibility = Visibility.Collapsed;
                SetVideoVisibility(action?.VideoId != null);
            }
            else
            {
                BindingOperations.SetBinding(_thumbnailView, Image.SourceProperty, new Binding(nameof(CloudFile.Uri)) { Source = action.Thumbnail });
                _thumbnailView.Visibility = Visibility.Visible;
                SetVideoVisibility(false);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Mutes the sound.
        /// </summary>
        public void Mute()
        {
            if (MediaElement != null)
                MediaElement.Volume = 0d;

            IsMuted = true;
        }

        /// <summary>
        /// Unmutes the sound.
        /// </summary>
        public void Unmute()
        {
            if (MediaElement != null)
                MediaElement.Volume = 1d;

            IsMuted = false;
        }

        #endregion

        #region Private methods

        void ShowControlMenu(object sender, MouseEventArgs e)
        {
            _controlsMenu.Visibility = Visibility.Visible;
            controlMenuTimer = new Timer(ControlMenuTimerCallback, null, 3000, Timeout.Infinite);
        }

        void HideControlMenu(object sender, MouseEventArgs e)
        {
            _controlsMenu.Visibility = Visibility.Collapsed;
            controlMenuTimer = null;
        }

        void ControlMenuTimerCallback(object state)
        {
            controlMenuTimer = null;
            Application.Current.Dispatcher.Invoke(() => HideControlMenu(null, null));
        }

        static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var current = (KMiniPlayer)d;

            // When the critical path changes, stop the player and rebuild the internal action tree
            current.Stop();
            current.BuildInternalActions();
            current.CurrentAction = null;
            current.Source = null;
        }

        static void OnCurrentItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var current = (KMiniPlayer)d;

            if (!current._internalCurrentActionFlag)
            {
                // When the current action (business wise speaking) is changed from outside the component, stops the player and set the current action (component wise speaking)
                current.Stop();
                current.CurrentAction = current._actions.FirstOrDefault(a => a.AssociatedItem == current.CurrentAction);
            }
        }

        static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var current = (KMiniPlayer)d;

            // Updates the source of the mediaelement
            current.MediaElement.SetBinding(KMediaElement.SourceProperty, new Binding(current.SourceFileBindingPath) { Source = current.Source, Mode = BindingMode.OneWay });

            // Updates the HasSource property
            d.SetValue(HasSourcePropertyKey, o.NewValue != null);
        }

        static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs o)
        {
            var player = (KMiniPlayer)d;
            player.SyncVideoWithPosition();

            if (player.IsPlaying)
            {
                var currentKAction = (player.CurrentAction.AssociatedItem as ActionPath)?.Action;
                if (currentKAction?.VideoId != null) //Si vidéo, on cache la vignette
                {
                    player._thumbnailView.Visibility = Visibility.Hidden;
                    player._thumbnailView.Source = null;
                    player.SetVideoVisibility(true);
                }
                else if (currentKAction?.Thumbnail != null) // Pas de vidéo et une vignette, on affiche la vignette
                {
                    BindingOperations.SetBinding(player._thumbnailView, Image.SourceProperty, new Binding(nameof(CloudFile.Uri)) { Source = currentKAction.Thumbnail });
                    player._thumbnailView.Visibility = Visibility.Visible;
                    player.SetVideoVisibility(false);
                }
                else // Pas de vidéo et pas de vignette, on affiche la vignette
                {
                    player._thumbnailView.Source = null;
                    player._thumbnailView.Visibility = Visibility.Visible;
                    player.SetVideoVisibility(false);
                }
            }
        }

        static void OnSpeedRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var current = (KMiniPlayer)d;
            current.RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, SpeedRatioChangedEvent));
        }

        /// <summary>
        /// Change la position de lecture et re-synchronise l'action actuellement lue.
        /// </summary>
        /// <param name="position">La nouvelle position.</param>
        void SeekAndResync(long position)
        {
            Position = position;
        }

        void SyncVideoWithPosition()
        {
            // Checks if the position change comes from a new
            if (Position != InternalPosition)
            {
                _internalPosition = Position;

                // Checks if the current action has changed
                SetCurrentAction(false, false);

                if (CurrentAction == null)
                {
                    // If no action (outside boundaries or else)
                    Stop();
                    Source = null;
                    return;
                }
                CurrentAction.LoadAndSync();
                RaiseCurrentActionChanged();

                if (IsPlaying && MediaElement != null)
                    CurrentAction.Run();
            }
        }

        void CurrentAction_Completed(object sender, EventArgs e)
        {
            var action = (IAction)sender;
            if (action.IsLast)
            {
                if (LoopPlaying && _actions.Count == 1)
                {
                    InternalPosition = CurrentAction.AssociatedItem.Start;
                    Play();
                }
                else
                {
                    // If the critical path is over, stops the player
                    Stop(); // Stops playback
                    InternalPosition = CurrentAction.AssociatedItem.Finish - 1; // Force selection to be the current one instead of the next one, if any
                }
                return;
            }
            // Checks if the current action has changed
            SetCurrentAction(false, true);
        }

        void KMiniPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= new RoutedEventHandler(KMiniPlayer_Unloaded);

            if (MediaElement != null)
            {
                // Unsubsribes and clean up
                MediaElement.MediaFailed -= new RoutedMediaFailedEventHandler(MediaElement_MediaFailed);
                MediaElement.Close();
            }
        }

        void MediaElement_MediaFailed(object sender, RoutedMediaFailedEventArgs e)
        {
        }

        TControl GetTemplateChild<TControl>(string name)
            where TControl : FrameworkElement
        {
            if (!(GetTemplateChild(name) is TControl control))
                throw new NotSupportedException($"{name} control cannot be found !");

            return control;
        }

        internal double GetCurrentActionActualSpeedRatio()
        {
            if (CurrentAction == null)
                return 1;
            var targetSpeedRatio = SpeedRatio * CurrentAction.InitialSpeedRatio;
            return targetSpeedRatio;
        }

        void BuildInternalActions()
        {
            _actions.Clear();

            if (ItemsSource?.Any() == true)
            {
                IAction lastAction = null;
                foreach (var item in ItemsSource)
                {
                    IAction current = null;

                    // Inserts a blank action if a hole exists between two actions
                    if (lastAction != null && item.Start != lastAction.AssociatedItem.Finish)
                        _actions.Add(new BlankAction(this, lastAction.AssociatedItem.Finish, item.Start));

                    // Inserts the action with or without video
                    if (item.HasVideo)
                        current = new PlayVideoAction(this, item);
                    else
                        current = new BlankAction(this, item);

                    _actions.Add(current);

                    lastAction = current;
                }

                // Sets the last one as the last one
                _actions.Last().IsLast = true;
            }
        }

        internal IAction CurrentAction
        {
            get { return _currentAction; }
            set
            {
                if (_currentAction != value)
                {
                    var old = _currentAction;
                    _currentAction = value;
                    _internalCurrentActionFlag = true;
                    OnCurrentActionChanged(old, value);
                    _internalCurrentActionFlag = false;
                }
            }
        }

        void OnCurrentActionChanged(IAction oldValue, IAction newValue)
        {
            if (oldValue != null)
            {
                oldValue.Completed -= CurrentAction_Completed;
                oldValue.Release();
            }

            if (CurrentAction != null)
            {
                CurrentAction.Completed += CurrentAction_Completed;
                // Sets the current action (business wise speaking)
                CurrentItem = CurrentAction.AssociatedItem;

                // Runs the action if the player is playing
                if (IsPlaying)
                {
                    CurrentAction.LoadAndSync();
                    RaiseCurrentActionChanged();
                    CurrentAction.Run();
                }
            }
            else
            {
                // Updates the current item if no action
                CurrentItem = null;
            }

        }

        void RaiseCurrentActionChanged()
        {
            Dispatcher.Invoke(() =>
            {
                RaiseEvent(new RoutedEventArgs(CurrentActionChangedEvent));
            }, DispatcherPriority.DataBind);
        }

        void RaiseStopped()
        {
            Dispatcher.Invoke(() =>
            {
                RaiseEvent(new RoutedEventArgs(StoppedEvent));
            }, DispatcherPriority.DataBind);
        }

        void SetCurrentAction(bool returnFirstAsDefault, bool skipEnding)
        {
            // Gets the action matching the position
            IAction matchingAction;

            if (skipEnding)
                matchingAction = _actions.FirstOrDefault(a => Position >= a.Start && Position < a.End);
            else
                matchingAction = _actions.FirstOrDefault(a => Position >= a.Start && Position <= a.End);

            if (matchingAction == null)
            {
                if (returnFirstAsDefault)
                {
                    // Returns the first and sets the position as its start
                    matchingAction = _actions.FirstOrDefault();
                    if (matchingAction != null)
                        SeekAndResync(matchingAction.Start);
                }
            }

            CurrentAction = matchingAction;
        }

        void ResetSpeedRatio()
        {
            SpeedRatio = 1;
        }

        void IncreaseSpeedRatio()
        {
            var currentSpeedRatio = SpeedRatio;

            var matchingTick = _speedRatios.FirstOrDefault(sr => sr == currentSpeedRatio);
            if (matchingTick != 0)
            {
                var index = _speedRatios.IndexOf(matchingTick);
                if (index != -1 && index < _speedRatios.Count - 1)
                    SpeedRatio = _speedRatios[index + 1];
            }
            else
                SpeedRatio += .5;
        }

        void DecreaseSpeedRatio()
        {
            var currentSpeedRatio = SpeedRatio;

            var matchingTick = _speedRatios.FirstOrDefault(sr => sr == currentSpeedRatio);
            if (matchingTick != 0)
            {
                var index = _speedRatios.IndexOf(matchingTick);
                if (index != -1 && index >= 1)
                    SpeedRatio = _speedRatios[index - 1];
            }
            else if (currentSpeedRatio > .5)
                SpeedRatio -= .5;
        }

        #endregion
    }
}