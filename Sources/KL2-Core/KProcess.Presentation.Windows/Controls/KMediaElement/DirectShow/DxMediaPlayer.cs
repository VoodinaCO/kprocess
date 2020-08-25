using DirectShowLib;
using MoreLinq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows.Controls.DirectShow
{

    /// <summary>
    /// The MediaUriPlayer plays media files from a given Uri.
    /// </summary>
    public class DxMediaPlayer : WorkDispatcherObject
    {
        #region Interop

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        #endregion

        #region Constants

        /// <summary>
        /// The name of the default audio render.  This is the
        /// same on all versions of windows
        /// </summary>
        const string DEFAULT_AUDIO_RENDERER_NAME = "Default DirectSound Device";

        /// <summary>
        /// The custom windows message constant for graph events
        /// </summary>
        const int W_GRAPH_NOTIFY = 0x0400 + 13;

        /// <summary>
        /// One second in 100ns units
        /// </summary>
        const long DSHOW_ONE_SECOND_UNIT = 10000000;

        /// <summary>
        /// The IBasicAudio volume value for silence
        /// </summary>
        const int DSHOW_VOLUME_SILENCE = -10000;

        /// <summary>
        /// The IBasicAudio volume value for full volume
        /// </summary>
        const int DSHOW_VOLUME_MAX = 0;

        /// <summary>
        /// The IBasicAudio balance max absolute value
        /// </summary>
        const int DSHOW_BALACE_MAX_ABS = 10000;

        /// <summary>
        /// Rate which our DispatcherTimer polls the graph
        /// </summary>
        const int DSHOW_TIMER_POLL_MS = 10;

        public static readonly LibraryModule LAVSplitter;
        public static readonly LibraryModule LAVAudio;
        public static readonly LibraryModule LAVVideo;

        #endregion

        #region Attributes

        /// <summary>
        /// Flag for the Dispose pattern
        /// </summary>
        bool _disposed;

        /// <summary>
        /// Flag for the playing state
        /// </summary>
        bool _isPlaying;

        /// <summary>
        /// The DirectShow graph interface.  In this example
        /// We keep reference to this so we can dispose 
        /// of it later.
        /// </summary>
        IGraphBuilder _graph;

        /// <summary>
        /// The media Uri
        /// </summary>
        Uri _sourceUri;

        /// <summary>
        /// UserId value for the VMR9 Allocator - Not entirely useful
        /// for this application of the VMR
        /// </summary>
        readonly IntPtr _userId = new IntPtr(unchecked((int)0xDEADBEEF));

        /// <summary>
        /// Static lock.  Seems multiple EVR controls instantiated at the same time crash
        /// </summary>
        static readonly object _videoRendererInitLock = new object();

        /// <summary>
        /// DirectShow interface for controlling audio
        /// functions such as volume and balance
        /// </summary>
        IBasicAudio _basicAudio;

        /// <summary>
        /// The custom DirectShow allocator
        /// </summary>
        ICustomAllocator _customAllocator;

        /// <summary>
        /// The DirectShow interface for controlling the
        /// filter graph.  This provides, Play, Pause, Stop, etc
        /// functionality.
        /// </summary>
        IMediaControl _mediaControl;

        /// <summary>
        /// The DirectShow interface for getting events
        /// that occur in the FilterGraph.
        /// </summary>
        IMediaEventEx _mediaEvent;

        /// <summary>
        /// The DirectShow interface for getting events
        /// that occur in the FilterGraph.
        /// </summary>
        IPropertyBag _mediaProperties;

        /// <summary>
        /// The natural video pixel height, if applicable
        /// </summary>
        int _naturalVideoHeight;

        /// <summary>
        /// The natural video pixel width, if applicable
        /// </summary>
        int _naturalVideoWidth;

        /// <summary>
        /// Our Win32 timer to poll the DirectShow graph
        /// </summary>
        DispatcherTimer _timer;

        /// <summary>
        /// The DirectShow media seeking interface
        /// </summary>
        IMediaSeeking _mediaSeeking;

        /// <summary>
        /// Local cache of the ending position
        /// </summary>
        long _endingPosition = -1;

        /// <summary>
        /// The vailable maximum position for current video.
        /// </summary>
        long _maxPosition = -1;

        /// <summary>
        /// Local cache of the starting position
        /// </summary>
        long _startingPosition;

        /// <summary>
        /// The prefered position format to use with the media
        /// </summary>
        MediaPositionFormat _preferedPositionFormat;

        /// <summary>
        /// The current media positioning format
        /// </summary>
        public MediaPositionFormat CurrentPositionFormat { get; private set; }

        /// <summary>
        /// The prefered media positioning format
        /// </summary>
        public MediaPositionFormat PreferedPositionFormat
        {
            get => _preferedPositionFormat;
            set
            {
                _preferedPositionFormat = value;
                SetMediaSeekingInterface(_mediaSeeking);
            }
        }

        /// <summary>
        /// Set the default audio renderer property backing
        /// </summary>
        string _audioRenderer = DEFAULT_AUDIO_RENDERER_NAME;

        IBaseFilter audioRenderer;
        IBaseFilter _parserFilter;

        /// <summary>
        /// Indicates if the player is loading
        /// </summary>
        bool _isLoading;

        readonly string _graphLogLocation;
        double _volume;

        #endregion

        #region Constructors

        public DxMediaPlayer()
        {
            _graphLogLocation = GraphHelper.GetGraphLogLocation("MediaPlayer");
        }

        static DxMediaPlayer()
        {
            var lavFiltersDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "LavFilters");
            LAVSplitter = LibraryModule.LoadModule(Path.Combine(lavFiltersDir, "LAVSplitter.ax"));
            LAVAudio = LibraryModule.LoadModule(Path.Combine(lavFiltersDir, "LAVAudio.ax"));
            LAVVideo = LibraryModule.LoadModule(Path.Combine(lavFiltersDir, "LAVVideo.ax"));
        }

        #endregion

        #region Events

        /// <summary>
        /// Event notifies when there is a new video frame
        /// to be rendered
        /// </summary>
        public event EventHandler NewAllocatorFrame;

        /// <summary>
        /// Event notifies when there is a new surface allocated
        /// </summary>
        public event EventHandler<NewAllocatorSurfaceEventArgs> NewAllocatorSurface;

        /// <summary>
        /// Notifies when the media has successfully been opened
        /// </summary>
        public event EventHandler MediaOpened;

        /// <summary>
        /// Notifies when the media has been closed
        /// </summary>
        public event EventHandler MediaClosed;

        /// <summary>
        /// Notifies when the media has failed and produced an exception
        /// </summary>
        public event EventHandler<MediaFailedEventArgs> MediaFailed;

        /// <summary>
        /// Notifies when the media has completed
        /// </summary>
        public event EventHandler MediaEnded;

        /// <summary>
        /// Notifies when the position of the media has changed
        /// </summary>
        public event EventHandler MediaPositionChanged;

        /// <summary>
        /// Notifies when loading status changed
        /// </summary>
        public event EventHandler IsLoadingChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Is true if the media contains renderable video
        /// </summary>
        public bool HasVideo { get; private set; }

        /// <summary>
        /// Is true if the media contains renderable audio
        /// </summary>
        public bool HasAudio { get; private set; }

        /// <summary>
        /// Gets the natural pixel width of the current media.
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoWidth
        {
            get
            {
                VerifyAccess();
                return _naturalVideoWidth;
            }
            private set
            {
                VerifyAccess();
                _naturalVideoWidth = value;
            }
        }

        /// <summary>
        /// Gets the natural pixel height of the current media.  
        /// The value will be 0 if there is no video in the media.
        /// </summary>
        public int NaturalVideoHeight
        {
            get
            {
                VerifyAccess();
                return _naturalVideoHeight;
            }
            private set
            {
                VerifyAccess();
                _naturalVideoHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the audio volume.  Specifies the volume, as a 
        /// number from 0 to 1.  Full volume is 1, and 0 is silence.
        /// </summary>
        public double Volume
        {
            get
            {
                VerifyAccess();

                /* Check if we even have an 
                 * audio interface */
                if (_basicAudio == null)
                    return 0;

                /* Get the current volume value from the interface */
                int hr = _basicAudio.get_Volume(out int dShowVolume);
                DsError.ThrowExceptionForHR(hr);

                /* Do calulations to convert to a base of 0 for silence */
                dShowVolume -= DSHOW_VOLUME_SILENCE;

                return (double)dShowVolume / -DSHOW_VOLUME_SILENCE;
            }
            set
            {
                VerifyAccess();

                /* Check if we even have an
                 * audio interface */
                if (_basicAudio == null)
                    return;

                _volume = value;
                ApplyVolume();
            }
        }

        /// <summary>
        /// Gets or sets the balance on the audio.
        /// The value can range from -1 to 1. The value -1 means the right channel is attenuated by 100 dB 
        /// and is effectively silent. The value 1 means the left channel is silent. The neutral value is 0, 
        /// which means that both channels are at full volume. When one channel is attenuated, the other 
        /// remains at full volume.
        /// </summary>
        public double Balance
        {
            get
            {
                VerifyAccess();

                /* Check if we even have an 
                 * audio interface */
                if (_basicAudio == null)
                    return 0;

                /* Get the interface supplied balance value */
                int hr = _basicAudio.get_Balance(out int balance);
                DsError.ThrowExceptionForHR(hr);

                /* Calc and return the balance based on 0 == silence */
                return (double)balance / DSHOW_BALACE_MAX_ABS;
            }
            set
            {
                VerifyAccess();

                /* Check if we even have an 
                 * audio interface */
                if (_basicAudio == null)
                    return;

                /* Calc the dshow balance value */
                int balance = (int)value * DSHOW_BALACE_MAX_ABS;

                int hr = _basicAudio.put_Balance(balance);
                // Permet de faire fonctionner sans le son
                //DsError.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Gets the duration in miliseconds, of the media that is opened
        /// </summary>
        public long Duration { get; private set; }

        /// <summary>
        /// Sets the rate at which the media plays back
        /// </summary>
        public double SpeedRatio
        {
            get
            {
                if (_graph == null)
                    return 1;

                int hr = _mediaSeeking.GetRate(out double rate);
                DsError.ThrowExceptionForHR(hr);

                return rate;
            }
            set
            {
                try
                {
                    IsLoading = true;

                    if (_graph == null)
                        return;

                    double rate = value;

                    int hr;

                    if (HasAudio)
                    {
                        // DirectSound only supports rate between .5 and 2
                        if (rate < 0.5 || rate > 2.0)
                        {
                            // Is the audio filter there ?
                            if (audioRenderer != null)
                            {
                                // Gets the audio renderer and WMAudio DMO pins
                                var audioRendererPinInput = DsFindPin.ByDirection(audioRenderer, PinDirection.Input, 0);

                                hr = audioRendererPinInput.ConnectedTo(out IPin audioRendererPinInputConnectedTo);

                                audioRendererPinInputConnectedTo.QueryPinInfo(out PinInfo pinf);
                                var sourceFilter = pinf.filter;
                                bool isAudioDecoderPresent = sourceFilter != _parserFilter;

                                var sourceFilterPinInput = DsFindPin.ByDirection(sourceFilter, PinDirection.Input, 0);
                                hr = sourceFilterPinInput.ConnectedTo(out IPin sourceFilterPinInputConnectedTo);

                                // Get the current state to be able to restore it later.
                                hr = _mediaControl.GetState(0, out FilterState filterState);
                                DsError.ThrowExceptionForHR(hr);

                                // Stops the graph
                                hr = _mediaControl.Stop();

                                // Disconnects the pins
                                hr = _graph.Disconnect(audioRendererPinInputConnectedTo);
                                hr = _graph.Disconnect(audioRendererPinInput);
                                if (isAudioDecoderPresent)
                                {
                                    hr = _graph.Disconnect(sourceFilterPinInputConnectedTo);
                                    hr = _graph.Disconnect(sourceFilterPinInput);
                                }

                                // Releases the pins
                                GraphHelper.SafeRelease(audioRendererPinInput);
                                GraphHelper.SafeRelease(audioRendererPinInputConnectedTo);
                                GraphHelper.SafeRelease(sourceFilterPinInput);
                                GraphHelper.SafeRelease(sourceFilterPinInputConnectedTo);

                                // Removes the filters
                                var filterGraph = (IFilterGraph)_graph;
                                hr = filterGraph.RemoveFilter(audioRenderer);
                                if (isAudioDecoderPresent)
                                    hr = filterGraph.RemoveFilter(sourceFilter);

                                // Releases the filters
                                GraphHelper.SafeRelease(audioRenderer);
                                if (isAudioDecoderPresent)
                                    GraphHelper.SafeRelease(sourceFilter);

                                audioRenderer = null;

                                if (filterState == FilterState.Paused)
                                    hr = _mediaControl.Pause();
                                else if (filterState == FilterState.Running)
                                    hr = _mediaControl.Run();
                            }
                        }
                        else
                        {
                            // Has it been removed ?
                            if (audioRenderer == null)
                            {
                                // Get the current state to be able to restore it later.
                                hr = _mediaControl.GetState(0, out FilterState filterState);
                                DsError.ThrowExceptionForHR(hr);

                                // Stops the graph
                                hr = _mediaControl.Stop();

                                var parserAudioOutputPin = DsFindPin.ByConnectionStatus(_parserFilter, PinConnectedStatus.Unconnected, 0);

                                CreateLAVAudio(parserAudioOutputPin);

                                // Restore the original state
                                ApplyVolume();
                                if (filterState == FilterState.Paused)
                                    hr = _mediaControl.Pause();
                                else if (filterState == FilterState.Running)
                                    hr = _mediaControl.Run();
                            }
                        }
                    }

                    hr = _mediaSeeking.SetRate(rate);

#if DEBUG
                    if (hr != 0)
                        System.Diagnostics.Debug.WriteLine($"Unable to set rate : {DsError.GetErrorText(hr)}");
#endif

                }
                catch (Exception e)
                {
                    this.TraceError(e, e.Message);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        void CreateAudio(ExtensionFiltersSource extensionFiltersSources, IPin splitterOutAudioPin)
        {
            int hr = 0;
            Type filterType = null;
            audioRenderer = InsertAudioRenderer(AudioRenderer);
            if (audioRenderer == null) // Cannot connect audio pin. Just ignore Audio
                HasAudio = false;
            else
            {
                var audioRendererInputPin = DsFindPin.ByDirection(audioRenderer, PinDirection.Input, 0);
                var audioFilterSource = extensionFiltersSources.AudioDecoder;

                switch (audioFilterSource.SourceType)
                {
                    case FilterSourceTypeEnum.Auto:

                        hr = _graph.Connect(splitterOutAudioPin, audioRendererInputPin);
                        DsError.ThrowExceptionForHR(hr);

                        GraphHelper.SafeRelease(audioRendererInputPin);

                        break;

                    case FilterSourceTypeEnum.External:

                        IBaseFilter externalFilter = null;

                        GraphHelper.CreateFilter(audioFilterSource.ExternalCLSID, audioFilterSource.Name, ref filterType, ref externalFilter);

                        hr = _graph.AddFilter(externalFilter, audioFilterSource.Name);
                        DsError.ThrowExceptionForHR(hr);

                        IPin audioDecoderInputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Input, 0);
                        hr = _graph.ConnectDirect(splitterOutAudioPin, audioDecoderInputPin, null);
                        DsError.ThrowExceptionForHR(hr);

                        IPin audioDecoderOutputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Output, 0);
                        hr = _graph.ConnectDirect(audioDecoderOutputPin, audioRendererInputPin, null);
                        DsError.ThrowExceptionForHR(hr);

                        GraphHelper.SafeRelease(audioDecoderInputPin);
                        GraphHelper.SafeRelease(audioDecoderOutputPin);


                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"{nameof(audioFilterSource)}.{nameof(FilterSource.SourceType)}");
                }
            }
        }

        void CreateLAVAudio(IPin splitterOutAudioPin)
        {
            int hr = 0;
            Type filterType = null;
            audioRenderer = InsertAudioRenderer(AudioRenderer);
            if (audioRenderer == null) // Cannot connect audio pin. Just ignore Audio
                HasAudio = false;
            else
            {
                var audioRendererInputPin = DsFindPin.ByDirection(audioRenderer, PinDirection.Input, 0);
                var audioFilterSource = GraphHelper.LAVFilters[GraphHelper.LAVFilterObject.AudioDecoder];

                IBaseFilter externalFilter = null;

                GraphHelper.CreateFilter(LAVAudio, audioFilterSource.CLSID, audioFilterSource.Name, ref filterType, ref externalFilter);

                hr = _graph.AddFilter(externalFilter, audioFilterSource.Name);
                DsError.ThrowExceptionForHR(hr);

                ConfigureAllCodecsForLAVAudio(externalFilter);

                IPin audioDecoderInputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Input, 0);
                hr = _graph.ConnectDirect(splitterOutAudioPin, audioDecoderInputPin, null);
                DsError.ThrowExceptionForHR(hr);

                IPin audioDecoderOutputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Output, 0);
                hr = _graph.ConnectDirect(audioDecoderOutputPin, audioRendererInputPin, null);
                DsError.ThrowExceptionForHR(hr);

                GraphHelper.SafeRelease(audioDecoderInputPin);
                GraphHelper.SafeRelease(audioDecoderOutputPin);
            }
        }

        void ConfigureAllCodecsForLAVAudio(IBaseFilter lavAudioFilter)
        {
            ILAVAudioSettings lavAudioSettings = lavAudioFilter as ILAVAudioSettings;
            lavAudioSettings.SetFormatConfiguration(LAVAudioCodec.Codec_WMA2, true);
            lavAudioSettings.SetFormatConfiguration(LAVAudioCodec.Codec_WMALL, true);
            lavAudioSettings.SetFormatConfiguration(LAVAudioCodec.Codec_WMAPRO, true);
        }

        /// <summary>
        /// Gets the position in miliseconds of the media.
        /// </summary>
        /// <remarks>Doesn't check if the thread is current.</remarks>
        internal long InternalMediaPosition { get; private set; }

        /// <summary>
        /// Gets or sets the position in miliseconds of the media
        /// </summary>
        public long MediaPosition
        {
            get
            {
                VerifyAccess();
                return InternalMediaPosition;
            }
            set
            {
                VerifyAccess();

                try
                {
                    IsLoading = true;

                    InternalMediaPosition = value;

                    SetPositions(InternalMediaPosition);
                    if (_mediaSeeking != null && !_isPlaying && _mediaControl != null)
                    {
                        try
                        {
                            int hr = _mediaControl.Pause();
                            DsError.ThrowExceptionForHR(hr);
                        }
                        catch (Exception e)
                        {
                            TraceManager.TraceError(e.Message);
                        }
                    }
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the ending position in miliseconds of the media
        /// </summary>
        public long MediaEndingPosition
        {
            get
            {
                VerifyAccess();
                return _endingPosition;
            }
            set
            {
                VerifyAccess();

                try
                {
                    IsLoading = true;
                    _endingPosition = value;

                    SetPositions(InternalMediaPosition);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the starting position in miliseconds of the media
        /// </summary>
        public long MediaStartingPosition
        {
            get
            {
                VerifyAccess();
                return _startingPosition;
            }
            set
            {
                VerifyAccess();

                try
                {
                    IsLoading = true;
                    _startingPosition = value;

                    SetPositions(InternalMediaPosition);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Uri source of the media
        /// </summary>
        public Uri Source
        {
            get => _sourceUri;
            set
            {
                VerifyAccess();
                _sourceUri = value;

                OpenSource();
            }
        }

        /// <summary>
        /// The renderer type to use when
        /// rendering video
        /// </summary>
        public VideoRendererType VideoRenderer { get; set; }

        /// <summary>
        /// The name of the audio renderer device
        /// </summary>
        public string AudioRenderer
        {
            get => _audioRenderer;
            set => _audioRenderer = string.IsNullOrEmpty(value) ? DEFAULT_AUDIO_RENDERER_NAME : value;
        }

        /// <summary>
        /// Gets or sets if the media should play in loop
        /// or if it should just stop when the media is complete
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    IsLoadingChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the filters configuration.
        /// </summary>
        public FiltersConfiguration FiltersConfiguration { get; set; }

        #endregion

        #region Dispose

        /// <summary>
        /// Frees any remaining memory
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Part of the dispose pattern
        /// </summary>
        void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return;

            _timer?.Stop();
            _timer = null;

            if (CheckAccess())
            {
                FreeResources();
                Dispatcher.BeginInvokeShutdown();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    FreeResources();
                    Dispatcher.BeginInvokeShutdown();
                });
            }

            _disposed = true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Polls the graph for various data about the media that is playing
        /// </summary>
        void OnGraphTimerTick()
        {
            /* Polls the current position */
            if (_mediaSeeking != null)
            {
                if (_mediaSeeking.GetCurrentPosition(out long lCurrentPos) == 0)
                {
                    // Parfois certains filtres fournissent des positions erronées en fin de vidéo.
                    // Cela permet d'éviter ce dépassement
                    if (_maxPosition != -1)
                        lCurrentPos = Math.Min(_maxPosition, lCurrentPos);

                    if (lCurrentPos != InternalMediaPosition)
                    {
                        InternalMediaPosition = lCurrentPos;
                        InvokeMediaPositionChanged(null);
                    }
                }
            }
        }

        /// <summary>
        /// Is called when a new media event code occurs on the graph
        /// </summary>
        /// <param name="code">The event code that occured</param>
        /// <param name="param1">The first parameter sent by the graph</param>
        /// <param name="param2">The second parameter sent by the graph</param>
        void OnMediaEvent(EventCode code, IntPtr param1, IntPtr param2)
        {
            if (code == EventCode.Complete && Loop)
                MediaPosition = 0;
            else if (code == EventCode.Complete && !Loop)
            {
                /* Only run the base when we don't loop
                 * otherwise the default behavior is to
                 * fire a media ended event */
                Pause();
                //StopInternal();
                InvokeMediaEnded(null);
                //StopGraphPollTimer();
            }
        }

        /// <summary>
        /// Starts the graph polling timer to update possibly needed
        /// things like the media position
        /// </summary>
        void StartGraphPollTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Render)
                {
                    Interval = TimeSpan.FromMilliseconds(DSHOW_TIMER_POLL_MS)
                };
                _timer.Tick += TimerElapsed;
            }

            _timer.Start();
        }

        void ProcessGraphEvents()
        {
            if (_mediaEvent != null)
            {
                /* Get all the queued events from the interface */
                while (_mediaEvent.GetEvent(out EventCode code, out IntPtr param1, out IntPtr param2, 0) == 0)
                {
                    /* Handle anything for this event code */
                    OnMediaEvent(code, param1, param2);

                    /* Free everything..we only need the code */
                    _mediaEvent.FreeEventParams(code, param1, param2);
                }
            }
        }

        void TimerElapsed(object sender, EventArgs e)
        {
            ProcessGraphEvents();
            OnGraphTimerTick();
        }

        /// <summary>
        /// Stops the graph polling timer
        /// </summary>
        void StopGraphPollTimer()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= TimerElapsed;
                _timer = null;
            }
        }

        /// <summary>
        /// Receives windows messages.  This is primarily used to get
        /// events that happen on our graph
        /// </summary>
        /// <param name="hwnd">The window handle</param>
        /// <param name="msg">The message Id</param>
        /// <param name="wParam">The message's wParam value</param>
        /// <param name="lParam">The message's lParam value</param>
        /// <param name="handled">A value that indicates whether the message was handled. Set the value to true if the message was handled; otherwise, false. </param>
        IntPtr WndProcHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            ProcessGraphEvents();

            return IntPtr.Zero;
        }

        /// <summary>
        /// Configures all general DirectShow interfaces that the
        /// FilterGraph supplies.
        /// </summary>
        void SetupFilterGraph()
        {
            SetMediaSeekingInterface(_graph as IMediaSeeking);
            _basicAudio = _graph as IBasicAudio;
            _mediaControl = _graph as IMediaControl;
            _mediaEvent = _graph as IMediaEventEx;
        }

        /// <summary>
        /// Registers the custom allocator and hooks into it's supplied events
        /// </summary>
        void RegisterCustomAllocator(ICustomAllocator allocator)
        {
            FreeCustomAllocator();

            if (allocator == null)
                return;

            _customAllocator = allocator;

            _customAllocator.NewAllocatorFrame += CustomAllocatorNewAllocatorFrame;
            _customAllocator.NewAllocatorSurface += CustomAllocatorNewAllocatorSurface;
        }

        /// <summary>
        /// Local event handler for the custom allocator's new surface event
        /// </summary>
        void CustomAllocatorNewAllocatorSurface(object sender, IntPtr pSurface) =>
            InvokeNewAllocatorSurface(pSurface);

        /// <summary>
        /// Local event handler for the custom allocator's new frame event
        /// </summary>
        void CustomAllocatorNewAllocatorFrame() =>
            InvokeNewAllocatorFrame();

        /// <summary>
        /// Disposes of the current allocator
        /// </summary>
        void FreeCustomAllocator()
        {
            if (_customAllocator == null)
                return;

            _customAllocator.NewAllocatorFrame -= CustomAllocatorNewAllocatorFrame;
            _customAllocator.NewAllocatorSurface -= CustomAllocatorNewAllocatorSurface;
            _customAllocator.Dispose();

            if (Marshal.IsComObject(_customAllocator))
                GraphHelper.SafeRelease(_customAllocator);

            _customAllocator = null;
        }

        /// <summary>
        /// Frees any allocated or unmanaged resources
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        void FreeResources()
        {
            try
            {
                /* We run the StopInternal() to avoid any 
                 * Dispatcher VeryifyAccess() issues because
                 * this may be called from the GC */
                StopInternal();

                StopGraphPollTimer();
                FreeCustomAllocator();

                //if (_basicAudio != null)
                //    Marshal.FinalReleaseComObject(_basicAudio);
                _basicAudio = null;

                //if (_mediaSeeking != null)
                //    Marshal.FinalReleaseComObject(_mediaSeeking);

                _mediaSeeking = null;

                //if (_mediaEvent != null)
                //    Marshal.FinalReleaseComObject(_mediaEvent);

                _mediaEvent = null;

                //if (_mediaControl != null)
                //    Marshal.FinalReleaseComObject(_mediaControl);

                _mediaControl = null;

                InternalMediaPosition = 0;
                _endingPosition = -1;
                _maxPosition = -1;

                if (_graph != null)
                {
                    // Causes COMException
                    GraphHelper.RemoveAllFilters(_graph);

                    GraphHelper.SafeRelease(_graph);
                    _graph = null;

                    /* Only run the media closed if we have an
                     * initialized filter graph */
                    InvokeMediaClosed(EventArgs.Empty);
                }
            }
            catch (InvalidComObjectException ex)
            {
                this.TraceDebug(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new renderer and configures it with a custom allocator
        /// </summary>
        /// <param name="rendererType">The type of renderer we wish to choose</param>
        /// <param name="graph">The DirectShow graph to add the renderer to</param>
        /// <param name="streamCount">Number of input pins for the renderer</param>
        /// <returns>An initialized DirectShow renderer</returns>
        IBaseFilter CreateVideoRenderer(VideoRendererType rendererType, IGraphBuilder graph, int streamCount = 2)
        {
            IBaseFilter renderer;

            switch (rendererType)
            {
                case VideoRendererType.VideoMixingRenderer9:
                    renderer = CreateVideoMixingRenderer9(graph, streamCount);
                    break;
                case VideoRendererType.EnhancedVideoRenderer:
                    renderer = CreateEnhancedVideoRenderer(graph, streamCount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rendererType));
            }

            return renderer;
        }

        /// <summary>
        /// Creates a new VMR9 renderer and configures it with an allocator
        /// </summary>
        /// <returns>An initialized DirectShow VMR9 renderer</returns>
        IBaseFilter CreateVideoMixingRenderer9(IGraphBuilder graph, int streamCount)
        {
            var vmr9 = new VideoMixingRenderer9() as IBaseFilter;

            if (!(vmr9 is IVMRFilterConfig9 filterConfig))
                throw new Exception("Could not query filter configuration.");

            /* We will only have one video stream connected to the filter */
            int hr = filterConfig.SetNumberOfStreams(streamCount);
            DsError.ThrowExceptionForHR(hr);

            /* Setting the renderer to "Renderless" mode
             * sounds counter productive, but its what we
             * need to do for setting up a custom allocator */
            hr = filterConfig.SetRenderingMode(VMR9Mode.Renderless);
            DsError.ThrowExceptionForHR(hr);

            /* Query the allocator interface */
            if (!(vmr9 is IVMRSurfaceAllocatorNotify9 vmrSurfAllocNotify))
                throw new Exception("Could not query the VMR surface allocator.");

            var allocator = new VMR9Allocator();

            /* We supply our custom allocator to the renderer */
            hr = vmrSurfAllocNotify.AdviseSurfaceAllocator(_userId, allocator);
            DsError.ThrowExceptionForHR(hr);

            hr = allocator.AdviseNotify(vmrSurfAllocNotify);
            DsError.ThrowExceptionForHR(hr);

            RegisterCustomAllocator(allocator);

            if (vmr9 is IVMRMixerControl9 mixer)
            {
                VMR9MixerPrefs dwPrefs;
                //mixer.GetMixingPrefs(out dwPrefs);
                //dwPrefs &= ~VMR9MixerPrefs.RenderTargetMask; // ATI compatibility issue ?
                //dwPrefs |= VMR9MixerPrefs.RenderTargetYUV;
                dwPrefs = VMR9MixerPrefs.RenderTargetRGB | VMR9MixerPrefs.NoDecimation | VMR9MixerPrefs.ARAdjustXorY | VMR9MixerPrefs.BiLinearFiltering;
                mixer.SetMixingPrefs(dwPrefs);
            }

            hr = graph.AddFilter(vmr9, "Video Mixing Renderer 9");
            DsError.ThrowExceptionForHR(hr);

            return vmr9;
        }

        /// <summary>
        /// Creates an instance of the EVR
        /// </summary>
        IBaseFilter CreateEnhancedVideoRenderer(IGraphBuilder graph, int streamCount)
        {
            EvrPresenter presenter;
            IBaseFilter filter;

            lock (_videoRendererInitLock)
            {
                var evr = new EnhancedVideoRenderer();
                filter = evr as IBaseFilter;

                int hr = graph.AddFilter(filter, $"Renderer: {VideoRendererType.EnhancedVideoRenderer}");
                DsError.ThrowExceptionForHR(hr);

                /* QueryInterface for the IMFVideoRenderer */
                if (!(filter is IMFVideoRenderer videoRenderer))
                    throw new Exception("Could not QueryInterface for the IMFVideoRenderer");

                /* Create a new EVR presenter */
                presenter = EvrPresenter.CreateNew();

                /* Initialize the EVR renderer with the custom video presenter */
                hr = videoRenderer.InitializeRenderer(null, presenter.VideoPresenter);
                DsError.ThrowExceptionForHR(hr);

                if (!(presenter.VideoPresenter is IEVRPresenterSettings presenterSettings))
                    throw new Exception("Could not QueryInterface for the IEVRPresenterSettings");

                presenterSettings.SetBufferCount(1);

                /* Use our interop hWnd */
                IntPtr handle = GetDesktopWindow();

                /* QueryInterface the IMFVideoDisplayControl */
                if (!(presenter.VideoPresenter is IMFVideoDisplayControl displayControl))
                    throw new Exception("Could not QueryInterface the IMFVideoDisplayControl");

                /* Configure the presenter with our hWnd */
                hr = displayControl.SetVideoWindow(handle);
                DsError.ThrowExceptionForHR(hr);

                if (filter is IEVRFilterConfig filterConfig)
                    filterConfig.SetNumberOfStreams(streamCount);
            }

            RegisterCustomAllocator(presenter);

            return filter;
        }

        /// <summary>
        /// Stops the media, but does not VerifyAccess() on
        /// the Dispatcher.  This can be used by destructors
        /// because it happens on another thread and our 
        /// DirectShow graph and COM run in MTA
        /// </summary>
        void StopInternal()
        {
            if (_mediaControl != null)
            {
                try
                {
                    PauseResyncPosition();
                }
                catch (Exception) { }
                _mediaControl.Stop();

                int hr = _mediaControl.GetState(0, out FilterState filterState);
                DsError.ThrowExceptionForHR(hr);

                while (filterState != FilterState.Stopped)
                    _mediaControl.GetState(0, out filterState);
            }
        }

        /// <summary>
        /// Inserts the audio renderer by the name of
        /// the audio renderer that is passed
        /// </summary>
        IBaseFilter InsertAudioRenderer(string audioDeviceName) =>
            _graph == null ? null : AddFilterByName(_graph, FilterCategory.AudioRendererCategory, audioDeviceName, false);

        void SetDuration()
        {
            if (_mediaSeeking == null)
                return;

            if (_endingPosition > -1)
            {
                Duration = _endingPosition - _startingPosition;
                return;
            }

            /* Get the duration of the media.  This value will
             * be in whatever format that was set. ie Frame, MediaTime */
            int hr = _mediaSeeking.GetDuration(out long duration);
            DsError.ThrowExceptionForHR(hr);

            Duration = duration;
            _maxPosition = duration;
        }

        /// <summary>
        /// Setup the IMediaSeeking interface
        /// </summary>
        void SetMediaSeekingInterface(IMediaSeeking mediaSeeking)
        {
            _mediaSeeking = mediaSeeking;

            if (mediaSeeking == null)
            {
                CurrentPositionFormat = MediaPositionFormat.None;
                Duration = 0;
                return;
            }

            /* Get our prefered DirectShow TimeFormat */
            Guid preferedFormat = ConvertPositionFormat(PreferedPositionFormat);

            /* Attempt to set the time format */
            int hr = mediaSeeking.SetTimeFormat(preferedFormat);
            DsError.ThrowExceptionForHR(hr);

            /* Gets the current time format
             * we may not have been successful
             * setting our prefered format */
            hr = mediaSeeking.GetTimeFormat(out Guid currentFormat);
            DsError.ThrowExceptionForHR(hr);

            /* Set our property up with the right format */
            CurrentPositionFormat = ConvertPositionFormat(currentFormat);

            SetDuration();
        }

        /// <summary>
        /// Opens the media by initializing the DirectShow graph
        /// </summary>
        void OpenSource()
        {
            if (FiltersConfiguration == null)
                throw new InvalidOperationException("The filters are not configured");

            FileStream fs = null;

            try
            {
                IsLoading = true;

                /* Make sure we clean up any remaining mess */
                FreeResources();

                string fileSource = _sourceUri?.OriginalString;

                if (string.IsNullOrEmpty(fileSource))
                    return;

                var extensionFiltersSources = GraphHelper.GetExtensionFiltersSource(FiltersConfiguration, fileSource);

                try
                {
                    /* Creates the GraphBuilder COM object */
                    _graph = new FilterGraphNoThread() as IGraphBuilder;

                    if (_graph == null)
                        throw new Exception("Could not create a graph");

                    if (_graphLogLocation != null)
                    {
                        fs = File.Create(_graphLogLocation);
                        int r = _graph.SetLogFile(fs.SafeFileHandle.DangerousGetHandle());
                    }

                    // 
                    // Creating FileSource filter
                    // 
                    IBaseFilter sourceFilter = GraphHelper.CreateLAVSplitterSourceFilter(_graph, fileSource, out IPin parserVideoOutputPin, out IPin parserAudioOutputPin);
                    _parserFilter = sourceFilter;
                    _mediaProperties = sourceFilter as IPropertyBag;

                    // 
                    // Creating renderer
                    // 
                    IBaseFilter videoRenderer = CreateVideoRenderer(VideoRenderer, _graph, 2);
                    IPin videoRendererInputPin = DsFindPin.ByDirection(videoRenderer, PinDirection.Input, 0);

                    // 
                    // Creating Video filter
                    // 
                    if (parserVideoOutputPin != null)
                        GraphHelper.ConnectLAVSplitterAndRendererWithLAVDecoder(_graph, parserVideoOutputPin, videoRendererInputPin);

                    // Runs the graph to make it choose a reference clock other than the one from the audio renderer
                    var mc = (IMediaControl)_graph;
                    mc.Pause();
                    mc.Stop();

                    HasAudio = parserAudioOutputPin != null;

                    // 
                    // Creating Audio filter
                    // 

                    if (HasAudio)
                        CreateLAVAudio(parserAudioOutputPin);

                    GraphHelper.SafeRelease(parserAudioOutputPin);
                    GraphHelper.SafeRelease(parserVideoOutputPin);
                    GraphHelper.SafeRelease(videoRendererInputPin);

                    /* Configure the graph in the base class */
                    SetupFilterGraph();

                    HasVideo = true;

                    /* Sets the NaturalVideoWidth/Height */
                    SetNativePixelSizes(videoRenderer);

                    /* Applies the current volume */
                    ApplyVolume();
                }
                catch (Exception ex)
                {
                    this.TraceError(ex, ex.Message);
                    /* This exection will happen usually if the media does
                     * not exist or could not open due to not having the
                     * proper filters installed */
                    FreeResources();

                    /* Fire our failed event */
                    InvokeMediaFailed(new MediaFailedEventArgs(ex.Message, fileSource, ex));
                }

                InvokeMediaOpened();
            }
            finally
            {
                IsLoading = false;

                if (_graphLogLocation != null && fs != null)
                    fs.Close();
            }
        }

        /// <summary>
        /// Applies the specified volume.
        /// </summary>
        private void ApplyVolume()
        {
            if (_volume <= 0)
            {
                /* Value should not be negative or else we treat as silence */
                int hr = _basicAudio.put_Volume(DSHOW_VOLUME_SILENCE);

                // Permet de ne pas planter pour les vidéos sans son
                //DsError.ThrowExceptionForHR(hr);
            }
            else if (_volume >= 1)
            {
                /* Value should not be greater than one or else we treat as maximum volume */
                int hr = _basicAudio.put_Volume(DSHOW_VOLUME_MAX);

                // Permet de ne pas planter pour les vidéos sans son
                //DsError.ThrowExceptionForHR(hr);
            }
            else
            {
                /* With the IBasicAudio interface, sound is DSHOW_VOLUME_SILENCE
                 * for silence and DSHOW_VOLUME_MAX for full volume
                 * so we calculate that here based off an input of 0 of silence and 1.0
                 * for full audio */
                int dShowVolume = (int)((1 - _volume) * DSHOW_VOLUME_SILENCE);
                _basicAudio.put_Volume(dShowVolume);
            }
        }

        /// <summary>
        /// Met la vidéo en pause et resynchronise les filtres à la position courante, si c'est possibles.
        /// </summary>
        void PauseResyncPosition()
        {
            _mediaControl.GetState(0, out FilterState currentState);

            // A la position 0, il faut quand même faire Pause pour afficher la première frame.
            if (currentState == FilterState.Stopped && InternalMediaPosition != 0)
                return;

            long previousPosition = InternalMediaPosition;
            long currentPosition = -1;

            if (_mediaSeeking != null)
            {
                int hr = _mediaSeeking.GetCurrentPosition(out currentPosition);
                DsError.ThrowExceptionForHR(hr);
            }

            _mediaControl.Pause();

            if (previousPosition != currentPosition)
                SetPositions(currentPosition);
        }

        /// <summary>
        /// Met à jour la position courante en fonction des divers paramètres de positionnement actuels.
        /// </summary>
        /// <param name="currentPosition">La position courante.</param>
        void SetPositions(long currentPosition)
        {
            if (_mediaSeeking != null)
            {
                long s = currentPosition > _startingPosition ? currentPosition : _startingPosition;

                if (_endingPosition > -1)
                {
                    /* Try to set the media time */
                    int hr = _mediaSeeking.SetPositions(s,
                                                        AMSeekingSeekingFlags.AbsolutePositioning,
                                                        _endingPosition,
                                                        AMSeekingSeekingFlags.AbsolutePositioning);
                    DsError.ThrowExceptionForHR(hr);
                }
                else
                {
                    /* Try to set the media time */
                    int hr = _mediaSeeking.SetPositions(s,
                                                        AMSeekingSeekingFlags.AbsolutePositioning,
                                                        null,
                                                        AMSeekingSeekingFlags.NoPositioning);
                    DsError.ThrowExceptionForHR(hr);
                }
            }
        }

        #endregion

        #region Pubic methods

        /// <summary>
        /// Plays the media
        /// </summary>
        public void Play()
        {
            VerifyAccess();

            _isPlaying = true;

            // Ensure the timer is alive
            StartGraphPollTimer();

            _mediaControl?.Run();
        }

        /// <summary>
        /// Stops the media
        /// </summary>
        public void Stop()
        {
            VerifyAccess();

            _isPlaying = false;

            StopInternal();
        }

        /// <summary>
        /// Closes the media and frees its resources
        /// </summary>
        public void Close()
        {
            VerifyAccess();

            _isPlaying = false;

            StopInternal();
            FreeResources();
        }

        /// <summary>
        /// Pauses the media
        /// </summary>
        public void Pause()
        {
            VerifyAccess();

            _isPlaying = false;

            if (_mediaControl != null)
                PauseResyncPosition();
        }

        #endregion

        #region Event Invokes

        /// <summary>
        /// Invokes the MediaEnded event, notifying any subscriber that
        /// media has reached the end
        /// </summary>
        void InvokeMediaEnded(EventArgs e) =>
            MediaEnded?.Invoke(this, e);

        /// <summary>
        /// Invokes the MediaOpened event, notifying any subscriber that
        /// media has successfully been opened
        /// </summary>
        void InvokeMediaOpened()
        {
            /* This is generally a good place to start
             * our polling timer */
            StartGraphPollTimer();

            MediaOpened?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the MediaClosed event, notifying any subscriber that
        /// the opened media has been closed
        /// </summary>
        void InvokeMediaClosed(EventArgs e)
        {
            StopGraphPollTimer();

            MediaClosed?.Invoke(this, e);
        }

        /// <summary>
        /// Invokes the MediaFailed event, notifying any subscriber that there was
        /// a media exception.
        /// </summary>
        /// <param name="e">The MediaFailedEventArgs contains the exception that caused this event to fire</param>
        void InvokeMediaFailed(MediaFailedEventArgs e) =>
            MediaFailed?.Invoke(this, e);

        /// <summary>
        /// Invokes the NewAllocatorFrame event, notifying any subscriber that new frame
        /// is ready to be presented.
        /// </summary>
        void InvokeNewAllocatorFrame() =>
            NewAllocatorFrame?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Invokes the NewAllocatorSurface event, notifying any subscriber of a new surface
        /// </summary>
        /// <param name="pSurface">The COM pointer to the D3D surface</param>
        void InvokeNewAllocatorSurface(IntPtr pSurface) =>
            NewAllocatorSurface?.Invoke(this, new NewAllocatorSurfaceEventArgs(pSurface));

        void InvokeMediaPositionChanged(EventArgs e) =>
            MediaPositionChanged?.Invoke(this, e);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets the natural pixel resolution the video in the graph
        /// </summary>
        /// <param name="renderer">The video renderer</param>
        void SetNativePixelSizes(IBaseFilter renderer)
        {
            Size size = GetVideoSize(renderer, PinDirection.Input, 0);

            NaturalVideoHeight = size.Height;
            NaturalVideoWidth = size.Width;

            HasVideo = true;
        }

        /// <summary>
        /// Gets the video resolution of a pin on a renderer.
        /// </summary>
        /// <param name="renderer">The renderer to inspect</param>
        /// <param name="direction">The direction the pin is</param>
        /// <param name="pinIndex">The zero based index of the pin to inspect</param>
        /// <returns>If successful a video resolution is returned.  If not, a 0x0 size is returned</returns>
        static Size GetVideoSize(IBaseFilter renderer, PinDirection direction, int pinIndex)
        {
            var size = new Size();

            var mediaType = new AMMediaType();
            IPin pin = DsFindPin.ByDirection(renderer, direction, pinIndex);

            try
            {
                if (pin?.ConnectionMediaType(mediaType) == 0)
                {
                    /* Check to see if its a video media type */
                    if (mediaType.formatType == FormatType.VideoInfo2 || mediaType.formatType == FormatType.VideoInfo)
                    {
                        var videoInfo = new VideoInfoHeader();

                        /* Read the video info header struct from the native pointer */
                        Marshal.PtrToStructure(mediaType.formatPtr, videoInfo);

                        Rectangle rect = videoInfo.SrcRect.ToRectangle();
                        size = new Size(rect.Width, rect.Height);
                    }
                }
            }
            finally
            {
                DsUtils.FreeAMMediaType(mediaType);

                GraphHelper.SafeRelease(pin);
            }

            return size;
        }

        /// <summary>
        /// Adds a filter to a DirectShow graph based on it's name and filter category
        /// </summary>
        /// <param name="graphBuilder">The graph builder to add the filter to</param>
        /// <param name="deviceCategory">The category the filter belongs to</param>
        /// <param name="friendlyName">The friendly name of the filter</param>
        /// <param name="throwIfFailed">Missing doc</param>
        /// <returns>Reference to the IBaseFilter that was added to the graph or returns null if unsuccessful</returns>
        static IBaseFilter AddFilterByName(IGraphBuilder graphBuilder, Guid deviceCategory, string friendlyName, bool throwIfFailed)
        {
            var devices = DsDevice.GetDevicesOfCat(deviceCategory);

            var deviceList = devices.Where(d => d.Name == friendlyName).ToArray();
            DsDevice device = deviceList.DefaultIfEmpty(null).FirstOrDefault();

            deviceList.ForEach(item =>
            {
                if (item != device)
                    item.Dispose();
            });

            return AddFilterByDevice(graphBuilder, device, throwIfFailed);
        }

        static IBaseFilter AddFilterByDevice(IGraphBuilder graphBuilder, DsDevice device, bool throwIfFailed)
        {
            if (graphBuilder == null)
                throw new ArgumentNullException(nameof(graphBuilder));

            if (!(graphBuilder is IFilterGraph2 filterGraph))
                return null;

            IBaseFilter filter = null;
            if (device != null)
            {
                int hr = filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out filter);
                if (throwIfFailed)
                    DsError.ThrowExceptionForHR(hr);
                else
                {
                    try
                    {
                        DsError.ThrowExceptionForHR(hr);
                    }
                    catch (Exception e)
                    {
                        TraceManager.TraceError($"Cannot add filter {device.Name} {device.ClassID}", e);
                        return null;
                    }

                }
            }
            return filter;
        }

        /// <summary>
        /// Converts a MediaPositionFormat enum to a DShow TimeFormat GUID
        /// </summary>
        static Guid ConvertPositionFormat(MediaPositionFormat positionFormat)
        {
            Guid timeFormat;

            switch (positionFormat)
            {
                case MediaPositionFormat.MediaTime:
                    timeFormat = TimeFormat.MediaTime;
                    break;
                case MediaPositionFormat.Frame:
                    timeFormat = TimeFormat.Frame;
                    break;
                case MediaPositionFormat.Byte:
                    timeFormat = TimeFormat.Byte;
                    break;
                case MediaPositionFormat.Field:
                    timeFormat = TimeFormat.Field;
                    break;
                case MediaPositionFormat.Sample:
                    timeFormat = TimeFormat.Sample;
                    break;
                default:
                    timeFormat = TimeFormat.None;
                    break;
            }

            return timeFormat;
        }

        /// <summary>
        /// Converts a DirectShow TimeFormat GUID to a MediaPositionFormat enum
        /// </summary>
        static MediaPositionFormat ConvertPositionFormat(Guid positionFormat)
        {
            if (positionFormat == TimeFormat.Byte)
                return MediaPositionFormat.Byte;
            if (positionFormat == TimeFormat.Field)
                return MediaPositionFormat.Field;
            if (positionFormat == TimeFormat.Frame)
                return MediaPositionFormat.Frame;
            if (positionFormat == TimeFormat.MediaTime)
                return MediaPositionFormat.MediaTime;
            if (positionFormat == TimeFormat.Sample)
                return MediaPositionFormat.Sample;
            return MediaPositionFormat.None;
        }

        #endregion
    }
}