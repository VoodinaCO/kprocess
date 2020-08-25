using DirectShowLib;
using KProcess.Presentation.Windows.Controls.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KProcess.Presentation.Windows.Controls.Helpers
{
    /// <summary>
    /// Permet de récupérer une capture image de la vidéo à un temps donné.
    /// </summary>
    /// <remarks>
    /// Le grabber propose une version synchronisée et une version qui ne l'est pas.
    /// Le problème est que lors de l'utilisation des composants DirectShow, effectuer des appels simultanés entraîne des erreurs de connexion de PINs, 
    /// même lorsqu'il s'agit de deux graphes différents dans des threads différents.
    /// Pour résoudre ce problème, le composant est disponible en version synchronisée, qui met tous les appels en file d'attente.
    /// </remarks>
    public class SnapshotGrabber
    {
        #region Constantes

        const string SAMPLE_GRABBER = "C1F400A0-3F08-11d3-9F0B-006008039E37";
        const string NULL_RENDERER = "C1F400A4-3F08-11D3-9F0B-006008039E37";

        /// <summary>
        /// Rate which our DispatcherTimer polls the graph
        /// </summary>
        const int DSHOW_TIMER_POLL_MS = 10;

        const int TimeoutMs = 5000;

        #endregion

        #region Champs privés

        //private readonly string _graphLogLocation;
        readonly string _sourceFilePath;

        static readonly AutoResetEvent _autoResetEventSynchronization = new AutoResetEvent(true);

        ISampleGrabber _sampleGrabber;
        IGraphBuilder _graph;
        IMediaControl _mediaControl;
        IMediaSeeking _mediaSeeking;
        IMediaEventEx _mediaEvent;

        Timer _timer;
        ManualResetEvent _mre;

        int _videoWidth;
        int _videoHeight;
        int _videoImageSize;
        readonly long _position;

        BitmapData _latestBitmapData;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sourceFilePath">Le chemin vers le fichier vidéo source.</param>
        /// <param name="position">La position dans la vidéo.</param>
        SnapshotGrabber(string sourceFilePath, long position)
        {
            _sourceFilePath = sourceFilePath;
            _position = position;

            // Désactivé pour l'instant car crée des problèmes d'accès concurrent.
            //_graphLogLocation = GraphHelper.GetGraphLogLocation("MediaPlayer");
        }

        #endregion

        #region Propriétés


        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Obtient en asynchrone l'image à la position spécifiée.
        /// Pour plus d'informations sur la synchronisation, consulter la documentation de la classe.
        /// </summary>
        /// <param name="sourceFilePath">Le chemin vers le fichier vidéo source.</param>
        /// <param name="position">La position dans la vidéo.</param>
        /// <returns>L'opération asynchrone.</returns>
        public static Task<BitmapSource> GetSnapshotAsync(string sourceFilePath, long position) =>
            new SnapshotGrabber(sourceFilePath, position).GetSnapshotAsync(false);

        /// <summary>
        /// Obtient en asynchrone l'image à la position spécifiée, de façon synchronisée.
        /// Pour plus d'informations sur la synchronisation, consulter la documentation de la classe.
        /// </summary>
        /// <param name="sourceFilePath">Le chemin vers le fichier vidéo source.</param>
        /// <param name="position">La position dans la vidéo.</param>
        /// <returns>L'opération asynchrone.</returns>
        public static Task<BitmapSource> GetSnapshotSynchronizedAsync(string sourceFilePath, long position) =>
            new SnapshotGrabber(sourceFilePath, position).GetSnapshotAsync(true);

        async Task<BitmapSource> GetSnapshotAsync(bool synchronized)
        {

            BitmapData bitmapData = await Task.Factory.StartNew(() =>
            {
                if (synchronized)
                    _autoResetEventSynchronization.WaitOne();
                _mre = new ManualResetEvent(false);

                try
                {
                    CreateSampleGrabber();
                    SetupLAVGraph();
                    SeekToPosition(_position);
                    QueryVideoDimensions();
                    RunGraph();

                    if (_mre.WaitOne(TimeoutMs))
                    {
                        BitmapData data = _latestBitmapData;
                        _latestBitmapData = null;
                        return data;
                    }
                    return null;
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Erreur lors de la création de la vignette");
                    return null;
                }
                finally
                {
                    FreeResources();
                    _mre.Close();
                }
            });

            try
            {
                if (bitmapData != null)
                {
                    try
                    {
                        BitmapSource bitmapSource = CreateBitmapSource(bitmapData);
                        return bitmapSource;
                    }
                    catch (Exception e)
                    {
                        this.TraceError(e, "Erreur lors de la création de la vignette - création du Bitmap Source");
                        return null;
                    }
                }
                return null;
            }
            finally
            {
                if (synchronized)
                    _autoResetEventSynchronization.Set();
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crée le filtre SampleGrabber
        /// </summary>
        void CreateSampleGrabber()
        {
            Type comType = Type.GetTypeFromCLSID(new Guid(SAMPLE_GRABBER));
            _sampleGrabber = (ISampleGrabber)Activator.CreateInstance(comType);

            AMMediaType mediaType = new AMMediaType
            {
                majorType = MediaType.Video,
                subType = MediaSubType.RGB32,
                formatType = FormatType.VideoInfo
            };
            _sampleGrabber.SetMediaType(mediaType);

            DsUtils.FreeAMMediaType(mediaType);

            int hr = _sampleGrabber.SetOneShot(true);
            DsError.ThrowExceptionForHR(hr);

            hr = _sampleGrabber.SetBufferSamples(true);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Crée le graphe DirectShow
        /// </summary>
        void SetupGraph()
        {
            FiltersConfiguration filtersConfiguration = GraphHelper.GetFiltersConfiguration();
            if (filtersConfiguration == null)
                throw new InvalidOperationException("The filters are not configured");

            //System.IO.FileStream fs = null;

            try
            {
                ExtensionFiltersSource extensionFiltersSources = GraphHelper.GetExtensionFiltersSource(filtersConfiguration, _sourceFilePath);

                try
                {
                    /* Creates the GraphBuilder COM object */
                    _graph = new FilterGraphNoThread() as IGraphBuilder;

                    if (_graph == null)
                        throw new Exception("Could not create a graph");

                    //if (_graphLogLocation != null)
                    //{
                    //    fs = System.IO.File.Create(_graphLogLocation);
                    //    int r = _graph.SetLogFile(fs.Handle);
                    //}

                    // 
                    // Creating FileSource filter
                    // 
                    IBaseFilter sourceFilter = GraphHelper.CreateLAVSplitterSourceFilter(_graph, _sourceFilePath, out IPin parserAudioOutputPin, out IPin parserVideoOutputPin);

                    // 
                    // Creating renderer
                    // 
                    Type videoRendererFilterType = null;
                    IBaseFilter videoRenderer = null;
                    GraphHelper.CreateFilter(NULL_RENDERER, "Null Renderer", ref videoRendererFilterType, ref videoRenderer);
                    int hr = _graph.AddFilter(videoRenderer, "Null Renderer");
                    DsError.ThrowExceptionForHR(hr);

                    IPin videoRendererInputPin = DsFindPin.ByDirection(videoRenderer, PinDirection.Input, 0);

                    // 
                    // Creating Video filter
                    // 

                    // Connection du sample Grabber
                    IBaseFilter sampleGrabberFilter = (IBaseFilter)_sampleGrabber;
                    hr = _graph.AddFilter(sampleGrabberFilter, "Sample Grabber");
                    DsError.ThrowExceptionForHR(hr);
                    IPin sampleGrabberInputPin = DsFindPin.ByDirection(sampleGrabberFilter, PinDirection.Input, 0);
                    IPin sampleGrabberOuputPin = DsFindPin.ByDirection(sampleGrabberFilter, PinDirection.Output, 0);

                    //Insertion du Color Space Converter
                    Type filterType = null;
                    IBaseFilter colorSpaceConverter = null;
                    GraphHelper.CreateFilter(GraphHelper.CLSID_COLOR_SPACE_CONVERTER, GraphHelper.COLOR_SPACE_CONVERTER_FRIENDLYNAME, ref filterType, ref colorSpaceConverter);
                    hr = _graph.AddFilter(colorSpaceConverter, GraphHelper.COLOR_SPACE_CONVERTER_FRIENDLYNAME);
                    DsError.ThrowExceptionForHR(hr);
                    IPin colorSpaceConverterInputPin = DsFindPin.ByDirection(colorSpaceConverter, PinDirection.Input, 0);
                    GraphHelper.ConnectSplitterAndRendererWithDecoder(_graph, extensionFiltersSources, parserVideoOutputPin, colorSpaceConverterInputPin);
                    IPin colorSpaceConverterOutputPin = DsFindPin.ByDirection(colorSpaceConverter, PinDirection.Output, 0);
                    hr = _graph.ConnectDirect(colorSpaceConverterOutputPin, sampleGrabberInputPin, null);
                    DsError.ThrowExceptionForHR(hr);

                    hr = _graph.Connect(sampleGrabberOuputPin, videoRendererInputPin);
                    DsError.ThrowExceptionForHR(hr);

                    // Removes the clock to run the graph as fast as possible
                    ((IMediaFilter)_graph).SetSyncSource(null);

                    GraphHelper.SafeRelease(parserAudioOutputPin);
                    GraphHelper.SafeRelease(parserVideoOutputPin);
                    GraphHelper.SafeRelease(videoRendererInputPin);
                    GraphHelper.SafeRelease(sampleGrabberInputPin);
                    GraphHelper.SafeRelease(sampleGrabberOuputPin);
                    GraphHelper.SafeRelease(colorSpaceConverterInputPin);
                    GraphHelper.SafeRelease(colorSpaceConverterOutputPin);

                    _mediaSeeking = _graph as IMediaSeeking;
                    _mediaControl = _graph as IMediaControl;
                    _mediaEvent = _graph as IMediaEventEx;

                    /* Attempt to set the time format */
                    hr = _mediaSeeking.SetTimeFormat(TimeFormat.MediaTime);
                    DsError.ThrowExceptionForHR(hr);

                }
                catch (Exception ex)
                {
                    this.TraceError(ex, ex.Message);
                    /* This exection will happen usually if the media does
                     * not exist or could not open due to not having the
                     * proper filters installed */
                    FreeResources();
                }
            }
            finally
            {
                /*if (_graphLogLocation != null && fs != null)
                    fs.Close();*/
            }
        }

        /// <summary>
        /// Crée le graphe DirectShow avec LAVFilters
        /// </summary>
        void SetupLAVGraph()
        {
            //System.IO.FileStream fs = null;

            try
            {
                try
                {
                    /* Creates the GraphBuilder COM object */
                    _graph = new FilterGraphNoThread() as IGraphBuilder;

                    if (_graph == null)
                        throw new Exception("Could not create a graph");

                    //if (_graphLogLocation != null)
                    //{
                    //    fs = System.IO.File.Create(_graphLogLocation);
                    //    int r = _graph.SetLogFile(fs.Handle);
                    //}

                    // 
                    // Creating FileSource filter
                    // 
                    IBaseFilter sourceFilter = GraphHelper.CreateLAVSplitterSourceFilter(_graph, _sourceFilePath, out IPin parserVideoOutputPin, out IPin parserAudioOutputPin);

                    // 
                    // Creating renderer
                    // 
                    Type videoRendererFilterType = null;
                    IBaseFilter videoRenderer = null;
                    GraphHelper.CreateFilter(NULL_RENDERER, "Null Renderer", ref videoRendererFilterType, ref videoRenderer);
                    int hr = _graph.AddFilter(videoRenderer, "Null Renderer");
                    DsError.ThrowExceptionForHR(hr);

                    IPin videoRendererInputPin = DsFindPin.ByDirection(videoRenderer, PinDirection.Input, 0);

                    // 
                    // Creating Video filter
                    // 

                    // Connection du sample Grabber
                    var sampleGrabberFilter = (IBaseFilter)_sampleGrabber;
                    hr = _graph.AddFilter(sampleGrabberFilter, "Sample Grabber");
                    DsError.ThrowExceptionForHR(hr);
                    IPin sampleGrabberInputPin = DsFindPin.ByDirection(sampleGrabberFilter, PinDirection.Input, 0);
                    IPin sampleGrabberOuputPin = DsFindPin.ByDirection(sampleGrabberFilter, PinDirection.Output, 0);

                    //Insertion du Color Space Converter
                    Type filterType = null;
                    IBaseFilter colorSpaceConverter = null;
                    GraphHelper.CreateFilter(GraphHelper.CLSID_COLOR_SPACE_CONVERTER, GraphHelper.COLOR_SPACE_CONVERTER_FRIENDLYNAME, ref filterType, ref colorSpaceConverter);
                    hr = _graph.AddFilter(colorSpaceConverter, GraphHelper.COLOR_SPACE_CONVERTER_FRIENDLYNAME);
                    DsError.ThrowExceptionForHR(hr);
                    IPin colorSpaceConverterInputPin = DsFindPin.ByDirection(colorSpaceConverter, PinDirection.Input, 0);
                    GraphHelper.ConnectLAVSplitterAndRendererWithLAVDecoder(_graph, parserVideoOutputPin, colorSpaceConverterInputPin);
                    IPin colorSpaceConverterOutputPin = DsFindPin.ByDirection(colorSpaceConverter, PinDirection.Output, 0);
                    hr = _graph.ConnectDirect(colorSpaceConverterOutputPin, sampleGrabberInputPin, null);
                    DsError.ThrowExceptionForHR(hr);

                    hr = _graph.Connect(sampleGrabberOuputPin, videoRendererInputPin);
                    DsError.ThrowExceptionForHR(hr);

                    // Removes the clock to run the graph as fast as possible
                    ((IMediaFilter)_graph).SetSyncSource(null);

                    GraphHelper.SafeRelease(parserAudioOutputPin);
                    GraphHelper.SafeRelease(parserVideoOutputPin);
                    GraphHelper.SafeRelease(videoRendererInputPin);
                    GraphHelper.SafeRelease(sampleGrabberInputPin);
                    GraphHelper.SafeRelease(sampleGrabberOuputPin);
                    GraphHelper.SafeRelease(colorSpaceConverterInputPin);
                    GraphHelper.SafeRelease(colorSpaceConverterOutputPin);

                    _mediaSeeking = _graph as IMediaSeeking;
                    _mediaControl = _graph as IMediaControl;
                    _mediaEvent = _graph as IMediaEventEx;

                    /* Attempt to set the time format */
                    hr = _mediaSeeking.SetTimeFormat(TimeFormat.MediaTime);
                    DsError.ThrowExceptionForHR(hr);

                }
                catch (Exception ex)
                {
                    this.TraceError(ex, ex.Message);
                    /* This exection will happen usually if the media does
                     * not exist or could not open due to not having the
                     * proper filters installed */
                    FreeResources();
                }
            }
            finally
            {
                //if (_graphLogLocation != null && fs != null)
                //    fs.Close();
            }
        }

        /// <summary>
        /// Se déplace à la position spécifiée dans la vidéo.
        /// </summary>
        /// <param name="pos">La position</param>
        void SeekToPosition(long pos)
        {
            // Définition de la position
            int hr = _mediaSeeking.SetPositions(pos, AMSeekingSeekingFlags.AbsolutePositioning, null, AMSeekingSeekingFlags.NoPositioning);
            DsError.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Interroge le filtre pour obtenir les dimensions de la vidéo.
        /// </summary>
        void QueryVideoDimensions()
        {
            AMMediaType media = new AMMediaType();
            VideoInfoHeader videoInfoHeader;

            int hr = _sampleGrabber.GetConnectedMediaType(media);
            Marshal.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            _videoWidth = videoInfoHeader.BmiHeader.Width;
            _videoHeight = videoInfoHeader.BmiHeader.Height;
            _videoImageSize = videoInfoHeader.BmiHeader.ImageSize;
            Marshal.FreeCoTaskMem(media.formatPtr);
            media.formatPtr = IntPtr.Zero;
        }

        /// <summary>
        /// Démarre le graphe.
        /// </summary>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions] // Permet de catcher AccessViolationException survenant dans un composant COM. Auquel cas le composant COM est entièrement disposé et l'état corrompu est donc correctement géré.
        void RunGraph()
        {
            try
            {
                StartGraphPollTimer();

                int hr = _mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Frees any allocated or unmanaged resources
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        void FreeResources()
        {
            try
            {
                StopInternal();

                StopGraphPollTimer();

                _mediaSeeking = null;
                _mediaEvent = null;
                _mediaControl = null;

                if (_graph != null)
                {
                    // Causes COMException
                    GraphHelper.RemoveAllFilters(_graph);

                    GraphHelper.SafeRelease(_graph);
                    _graph = null;
                }
            }
            catch (InvalidComObjectException ex)
            {
                this.TraceDebug(ex.Message);
            }
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
                _mediaControl.Stop();
                int hr = _mediaControl.GetState(0, out FilterState filterState);
                DsError.ThrowExceptionForHR(hr);

                while (filterState != FilterState.Stopped)
                    _mediaControl.GetState(0, out filterState);
            }
        }

        /// <summary>
        /// Génère une image à partir de ce que contient le buffer du SampleGrabber.
        /// </summary>
        void GenerateImage()
        {
            int pBufferSize = 0;

            int hr = _sampleGrabber.GetCurrentBuffer(ref pBufferSize, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            byte[] byteArray = new byte[pBufferSize];
            GCHandle pinnedArray = GCHandle.Alloc(byteArray, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            hr = _sampleGrabber.GetCurrentBuffer(ref pBufferSize, pointer);
            DsError.ThrowExceptionForHR(hr);

            int stride = _videoWidth * 4;
            int scan0 = (int)pointer;
            scan0 += (_videoHeight - 1) * stride;


            Bitmap bitmapOriginalSize = new Bitmap(_videoWidth, _videoHeight, -stride, System.Drawing.Imaging.PixelFormat.Format32bppRgb, (IntPtr)scan0);

            bitmapOriginalSize = (Bitmap)bitmapOriginalSize.Clone(); // Permet d'avoir la même image mais avec un stride positif, qui est requis pour la conversion en System.Media.imaging.*

            _latestBitmapData = bitmapOriginalSize.LockBits(new Rectangle(0, 0, _videoWidth, _videoHeight),
                                        ImageLockMode.ReadOnly,
                                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            pinnedArray.Free();

            _mre.Set();
        }

        /// <summary>
        /// Génère un <see cref="BitmapSource"/> à partir d'un Bitmap GDI. 
        /// Le Bitmap source créé ne sera disponible qu'à partir du thread appelant cette méthode.
        /// </summary>
        /// <param name="bitmapData">Les données.</param>
        /// <returns>Le <see cref="BitmapSource"/>.</returns>
        BitmapSource CreateBitmapSource(BitmapData bitmapData) =>
            BitmapSource.Create(bitmapData.Width, bitmapData.Height,
                96, 96, PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

        #endregion

        #region Gestion de l'horloge

        /// <summary>
        /// Starts the graph polling timer to update possibly needed
        /// things like the media position
        /// </summary>
        void StartGraphPollTimer()
        {
            if (_timer == null)
                _timer = new Timer(TimerElapsed, null, 0, DSHOW_TIMER_POLL_MS);
        }

        /// <summary>
        /// Stops the graph polling timer
        /// </summary>
        void StopGraphPollTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }


        void ProcessGraphEvents()
        {
            if (_mediaEvent != null)
            {
                IMediaEventEx me = _mediaEvent;

                /* Get all the queued events from the interface */
                while (me.GetEvent(out EventCode code, out IntPtr param1, out IntPtr param2, 0) == 0)
                {
                    /* Handle anything for this event code */
                    OnMediaEvent(code, param1, param2);

                    /* Free everything..we only need the code */
                    me.FreeEventParams(code, param1, param2);
                }
            }
        }

        void OnMediaEvent(EventCode code, IntPtr param1, IntPtr param2)
        {
            //_sampleGrabber.
            /* Only run the base when we don't loop
             * otherwise the default behavior is to
             * fire a media ended event */
            switch (code)
            {
                case EventCode.Complete:
                    _mediaControl.Stop();
                    GenerateImage();
                    //StopInternal();
                    //StopGraphPollTimer();
                    break;
                case EventCode.Paused:
                    break;
            }
        }

        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions] // Permet de catcher AccessViolationException survenant dans un composant COM. Auquel cas le composant COM est entièrement disposé et l'état corrompu est donc correctement géré.
        [MethodImpl(MethodImplOptions.Synchronized)]
        void TimerElapsed(object state)
        {
            try
            {
                ProcessGraphEvents();
            }
            catch (Exception e)
            {
                this.TraceError(e, "Erreur lors de la création de la vignette");
                _mre.Set();
            }
        }

        #endregion
    }
}
