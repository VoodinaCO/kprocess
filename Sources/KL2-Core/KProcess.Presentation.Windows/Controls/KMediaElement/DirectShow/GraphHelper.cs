using DirectShowLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace KProcess.Presentation.Windows.Controls.DirectShow
{
    /// <summary>
    /// Contient des méthodes aidant à la gsetion des graphes DirectShow.
    /// </summary>
    public static class GraphHelper
    {
        public enum LAVFilterObject
        {
            SplitterSource,
            SplitterInputFormats,
            Splitter,
            SplitterProperties,

            AudioDecoder,
            AudioMixer,
            AudioStatus,
            AudioFormatSettings,
            AudioProperties,

            VideoDecoder,
            VideoProperties,
            VideoFormatSettings
        }

        public class LAVFilterInfo
        {
            public string Name { get; }
            public string CLSID { get; }

            public LAVFilterInfo(string name, string clsid)
            {
                Name = name;
                CLSID = clsid;
            }
        }

        public static readonly Dictionary<LAVFilterObject, LAVFilterInfo> LAVFilters = new Dictionary<LAVFilterObject, LAVFilterInfo>()
        {
            [LAVFilterObject.SplitterSource] = new LAVFilterInfo("LAV Splitter Source", "B98D13E7-55DB-4385-A33D-09FD1BA26338"),
            [LAVFilterObject.SplitterInputFormats] = new LAVFilterInfo("LAV Splitter Input Formats", "56904B22-091C-4459-A2E6-B1F4F946B55F"),
            [LAVFilterObject.Splitter] = new LAVFilterInfo("LAV Splitter", "171252A0-8820-4AFE-9DF8-5C92B2D66B04"),
            [LAVFilterObject.SplitterProperties] = new LAVFilterInfo("LAV Splitter Properties", "A19DE2F2-2F74-4927-8436-61129D26C141"),

            [LAVFilterObject.AudioDecoder] = new LAVFilterInfo("LAV Audio Decoder", "E8E73B6B-4CB3-44A4-BE99-4F7BCB96E491"),
            [LAVFilterObject.AudioMixer] = new LAVFilterInfo("LAV Audio Mixer", "C89FC33C-E60A-4C97-BEF4-ACC5762B6404"),
            [LAVFilterObject.AudioStatus] = new LAVFilterInfo("LAV Audio Status", "20ED4A03-6AFD-4FD9-980B-2F6143AA0892"),
            [LAVFilterObject.AudioFormatSettings] = new LAVFilterInfo("LAV Audio Formats Settings", "BD72668E-6BFF-4CD1-8480-D465708B336B"),
            [LAVFilterObject.AudioProperties] = new LAVFilterInfo("LAV Audio Properties", "2D8F1801-A70D-48F4-B76B-7F5AE022AB54"),

            [LAVFilterObject.VideoDecoder] = new LAVFilterInfo("LAV Video Decoder", "EE30215D-164F-4A92-A4EB-9D4C13390F9F"),
            [LAVFilterObject.VideoProperties] = new LAVFilterInfo("LAV Video Properties", "278407C2-558C-4BED-83A0-B6FA454200BD"),
            [LAVFilterObject.VideoFormatSettings] = new LAVFilterInfo("LAV Video Format Settings", "2D4D6F88-8B41-40A2-B297-3D722816648B")
        };

        const string CLSID_FILESOURCE = "E436EBB5-524F-11CE-9F53-0020AF0BA770";
        const string CLSID_VIDEO_DECODER_DMO = "82D353DF-90BD-4382-8BC2-3F6192B76E34";

        public const string CLSID_COLOR_SPACE_CONVERTER = "1643E180-90F5-11CE-97D5-00AA0055595A";
        public const string COLOR_SPACE_CONVERTER_FRIENDLYNAME = "Color Space Converter";

        /// <summary>
        /// Obtient la localisation du fichier de log.
        /// </summary>
        /// <param name="suffix">Le suffixe à ajouter.</param>
        /// <returns>La localisation du fichier de log.</returns>
        public static string GetGraphLogLocation(string suffix)
        {
            Assertion.NotNull(suffix, nameof(suffix));
            try
            {
                string logLoc = System.Configuration.ConfigurationManager.AppSettings["GraphLogLocation"];
                if (logLoc != null)
                {
                    logLoc = logLoc.Replace("${APPDATA}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

                    string folder = Path.GetDirectoryName(logLoc);

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string file = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(logLoc)}.{suffix}{Path.GetExtension(logLoc)}");

                    return file;
                }
            }
            catch (Exception)
            {
                TraceManager.TraceDebug("Can't get graph log location.");
            }

            return null;
        }

        /// <summary>
        /// Obtient la configuration des filtres.
        /// </summary>
        /// <returns>La configuration des filtres.</returns>
        public static FiltersConfiguration GetFiltersConfiguration()
        {
            if (DesignMode.IsInDesignMode)
                return new FiltersConfiguration();
            return IoC.Resolve<IServiceBus>().Get<IDecoderInfoService>().FiltersConfiguration;
        }

        /// <summary>
        /// Libère les ressources.
        /// </summary>
        /// <param name="o">L'objet</param>
        public static void SafeRelease(object o)
        {
            if (o != null)
                Marshal.FinalReleaseComObject(o);
        }


        /// <summary>
        /// Obtient les paramètres pour un fichier.
        /// </summary>
        /// <param name="filtersConfiguration">Tous les paramètres.</param>
        /// <param name="fileSource">Le chemin vers le fichier vidéo.</param>
        /// <returns>Les paramètres de filtre pour le fichier.</returns>
        internal static ExtensionFiltersSource GetExtensionFiltersSource(FiltersConfiguration filtersConfiguration, string fileSource)
        {
            string fileExtension = Path.GetExtension(fileSource);
            ExtensionFiltersSource extensionFiltersSources = filtersConfiguration[fileExtension];

            if (extensionFiltersSources == null)
                throw new InvalidOperationException("There is no filter for this file type");
            return extensionFiltersSources;
        }

        /// <summary>
        /// Crée le filtre "File SOurce" et l'ajoute au graphe.
        /// </summary>
        /// <param name="graph">Le graphe.</param>
        /// <param name="fileSource">Le chemin vers le fichier vidéo.</param>
        /// <param name="sourceOutputPin">Le pin de sortie.</param>
        /// <returns>Le filtre créé.</returns>
        internal static IBaseFilter CreateFileSourceFilter(IGraphBuilder graph, string fileSource, out IPin sourceOutputPin)
        {
            Type filterType = null;
            IBaseFilter sourceFilter = null;

            CreateFilter(CLSID_FILESOURCE, "File Source (Async.)", ref filterType, ref sourceFilter);
            ((IFileSourceFilter)sourceFilter).Load(fileSource, null);

            sourceOutputPin = DsFindPin.ByDirection(sourceFilter, PinDirection.Output, 0);

            int hr = graph.AddFilter(sourceFilter, "File Source (Async.)");
            DsError.ThrowExceptionForHR(hr);

            return sourceFilter;
        }

        /// <summary>
        /// Crée le filtre "LAV Splitter Source" et l'ajoute au graphe.
        /// </summary>
        /// <param name="graph">Le graphe.</param>
        /// <param name="fileSource">Le chemin vers le fichier vidéo.</param>
        /// <param name="videoOutputPin">Le pin de sortie video.</param>
        /// <param name="audioOutputPin">Le pin de sortie audio.</param>
        /// <returns>Le filtre créé.</returns>
        internal static IBaseFilter CreateLAVSplitterSourceFilter(IGraphBuilder graph, string fileSource, out IPin videoOutputPin, out IPin audioOutputPin)
        {
            Type filterType = null;
            IBaseFilter sourceFilter = null;

            var lavFilterInfo = LAVFilters[LAVFilterObject.SplitterSource];
            CreateFilter(DxMediaPlayer.LAVSplitter, lavFilterInfo.CLSID, lavFilterInfo.Name, ref filterType, ref sourceFilter);
            ((IFileSourceFilter)sourceFilter).Load(fileSource, null);

            audioOutputPin = DsFindPin.ByDirection(sourceFilter, PinDirection.Output, 1);
            videoOutputPin = DsFindPin.ByDirection(sourceFilter, PinDirection.Output, 0);

            int hr = graph.AddFilter(sourceFilter, lavFilterInfo.Name);
            DsError.ThrowExceptionForHR(hr);

            return sourceFilter;
        }

        /// <summary>
        /// Crée le filtre "File Splitter (demuxer)" et l'ajoute au graphe.
        /// </summary>
        /// <param name="graph">Le graphe.</param>
        /// <param name="filtersConfig">La configuration pour ce fichier.</param>
        /// <param name="sourceOutputPin">Le pin de sortie de la source.</param>
        /// <param name="parserOutputAudioPin">Le pin de sortie audio.</param>
        /// <param name="parserOutputVideoPin">Le pin de sortie vidéo</param>
        /// <returns>Le filtre File SPlitter.</returns>
        internal static IBaseFilter CreateSplitterFilter(IGraphBuilder graph, ExtensionFiltersSource filtersConfig, IPin sourceOutputPin,
            out IPin parserOutputAudioPin, out IPin parserOutputVideoPin)
        {
            Type filterType = null;
            IBaseFilter parserFilter = null;

            FilterSource splitterSource = filtersConfig.Splitter;

            if (splitterSource.SourceType != FilterSourceTypeEnum.External)
                throw new InvalidOperationException("The Splitter Filter Source cannot be Automatic");

            CreateFilter(splitterSource.ExternalCLSID, splitterSource.Name, ref filterType, ref parserFilter);

            IPin parserInputPin = DsFindPin.ByDirection(parserFilter, PinDirection.Input, 0);

            int hr = graph.AddFilter(parserFilter, splitterSource.Name);
            DsError.ThrowExceptionForHR(hr);

            // Connectiong FileSource et Splitter
            hr = graph.Connect(sourceOutputPin, parserInputPin);
            DsError.ThrowExceptionForHR(hr);

            SafeRelease(sourceOutputPin);
            SafeRelease(parserInputPin);

            // Pins de sortie

            parserOutputAudioPin = DsFindPin.ByDirection(parserFilter, PinDirection.Output, 0);
            parserOutputVideoPin = DsFindPin.ByDirection(parserFilter, PinDirection.Output, 1);

            // Only video or audio
            if (parserOutputVideoPin == null)
            {
                parserOutputVideoPin = parserOutputAudioPin;
                parserOutputAudioPin = null;
            }

            parserOutputVideoPin.EnumMediaTypes(out IEnumMediaTypes enumMediaTypes);
            AMMediaType[] mts = { null };
            while (enumMediaTypes.Next(1, mts, IntPtr.Zero) == 0)
            {
                if (mts[0].majorType != MediaType.Video)
                {
                    // Invert in case it's not the appropriate media type
                    IPin temp = parserOutputAudioPin;
                    parserOutputAudioPin = parserOutputVideoPin;
                    parserOutputVideoPin = temp;
                    break;
                }
            }

            return parserFilter;
        }

        /// <summary>
        /// Connecte le File Splitter et le renderer vidéo en créant le décodeur vidéo.
        /// </summary>
        /// <param name="graph">Le graphe.</param>
        /// <param name="filtersConfig">La configuration pour ce fichier.</param>
        /// <param name="parserOutputVideoPin">Le pin de sortie vidéo</param>
        /// <param name="videoRendererInputPin">Le pin d'entrée du Renderer.</param>
        internal static void ConnectSplitterAndRendererWithDecoder(IGraphBuilder graph, ExtensionFiltersSource filtersConfig, 
            IPin parserOutputVideoPin, IPin videoRendererInputPin)
        {
            FilterSource videoFilterSource = filtersConfig.VideoDecoder;

            switch (videoFilterSource.SourceType)
            {
                case FilterSourceTypeEnum.Auto:

                    int hr = graph.Connect(parserOutputVideoPin, videoRendererInputPin);
                    DsError.ThrowExceptionForHR(hr);

                    break;

                case FilterSourceTypeEnum.External:
                    if (new Guid(videoFilterSource.ExternalCLSID) == new Guid(CLSID_VIDEO_DECODER_DMO))
                    {
                        // The DMO filter is handled differently
                        DMOWrapperFilter dmoFilter = new DMOWrapperFilter();
                        IDMOWrapperFilter wrapper = (IDMOWrapperFilter)dmoFilter;
                        hr = wrapper.Init(new Guid(CLSID_VIDEO_DECODER_DMO), DirectShowLib.DMO.DMOCategory.VideoDecoder);

                        DsError.ThrowExceptionForHR(hr);

                        if (dmoFilter is IBaseFilter decoderFilter)
                        {
                            hr = graph.AddFilter(decoderFilter, "WMVideo Decoder DMO");
                            DsError.ThrowExceptionForHR(hr);

                            IPin wmvDecoderInputPin = DsFindPin.ByDirection(decoderFilter, PinDirection.Input, 0);
                            hr = graph.ConnectDirect(parserOutputVideoPin, wmvDecoderInputPin, null);
                            DsError.ThrowExceptionForHR(hr);

                            IPin wmvDecoderOutputPin = DsFindPin.ByDirection(decoderFilter, PinDirection.Output, 0);
                            hr = graph.ConnectDirect(wmvDecoderOutputPin, videoRendererInputPin, null);
                            DsError.ThrowExceptionForHR(hr);

                            SafeRelease(wmvDecoderInputPin);
                            SafeRelease(wmvDecoderOutputPin);
                        }
                        else
                        {
                            wrapper = null;
                            SafeRelease(dmoFilter);
                            dmoFilter = null;
                        }
                    }
                    else
                    {
                        Type filterType = null;
                        IBaseFilter externalFilter = null;

                        CreateFilter(videoFilterSource.ExternalCLSID, videoFilterSource.Name, ref filterType, ref externalFilter);

                        hr = graph.AddFilter(externalFilter, videoFilterSource.Name);
                        DsError.ThrowExceptionForHR(hr);

                        IPin externalDecoderInputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Input, 0);
                        hr = graph.ConnectDirect(parserOutputVideoPin, externalDecoderInputPin, null);
                        DsError.ThrowExceptionForHR(hr);

                        IPin externalDecoderOutputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Output, 0);
                        hr = graph.ConnectDirect(externalDecoderOutputPin, videoRendererInputPin, null);
                        DsError.ThrowExceptionForHR(hr);

                        SafeRelease(externalDecoderInputPin);
                        SafeRelease(externalDecoderOutputPin);
                    }


                    break;

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(videoFilterSource)}.{nameof(FilterSource.SourceType)}");
            }
        }

        /// <summary>
        /// Connecte le LAV Splitter Source et le renderer vidéo en créant le LAV Video Decoder.
        /// </summary>
        /// <param name="graph">Le graphe.</param>
        /// <param name="parserOutputVideoPin">Le pin de sortie vidéo</param>
        /// <param name="videoRendererInputPin">Le pin d'entrée du Renderer.</param>
        internal static void ConnectLAVSplitterAndRendererWithLAVDecoder(IGraphBuilder graph,
            IPin parserOutputVideoPin, IPin videoRendererInputPin)
        {
            Type filterType = null;
            IBaseFilter externalFilter = null;

            var lavVideoDecoder = LAVFilters[LAVFilterObject.VideoDecoder];
            CreateFilter(DxMediaPlayer.LAVVideo, lavVideoDecoder.CLSID, lavVideoDecoder.Name, ref filterType, ref externalFilter);

            int hr = graph.AddFilter(externalFilter, lavVideoDecoder.Name);
            DsError.ThrowExceptionForHR(hr);

            IPin externalDecoderInputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Input, 0);
            hr = graph.ConnectDirect(parserOutputVideoPin, externalDecoderInputPin, null);
            DsError.ThrowExceptionForHR(hr);

            IPin externalDecoderOutputPin = DsFindPin.ByDirection(externalFilter, PinDirection.Output, 0);
            hr = graph.ConnectDirect(externalDecoderOutputPin, videoRendererInputPin, null);
            DsError.ThrowExceptionForHR(hr);

            SafeRelease(externalDecoderInputPin);
            SafeRelease(externalDecoderOutputPin);
        }

        /// <summary>
        /// Creates a new instance of the specified filter.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="clsid">filter The CLSID.</param>
        /// <param name="name">The name.</param>
        /// <param name="filterType">Type of the filter.</param>
        /// <param name="parserFilter">The parser filter.</param>
        internal static void CreateFilter<TFilter>(string clsid, string name, ref Type filterType, ref TFilter parserFilter)
        {
            TraceManager.TraceDebug($"Creating new instance of filter, Name = {name}, CLSID = {clsid}");
            try
            {
                filterType = Type.GetTypeFromCLSID(new Guid(clsid));
                parserFilter = (TFilter)Activator.CreateInstance(filterType);
            }
            catch (Exception ex)
            {
                TraceManager.TraceError(ex, ex.Message);
                throw new NotSupportedException($"Cannot create the filter: Name = {name}, CLSID = {clsid}", ex);
            }
        }

        /// <summary>
        /// Creates a new instance of the specified filter from a module.
        /// </summary>
        /// <typeparam name="TFilter">The type of the filter.</typeparam>
        /// <param name="clsid">filter The CLSID.</param>
        /// <param name="name">The name.</param>
        /// <param name="filterType">Type of the filter.</param>
        /// <param name="parserFilter">The parser filter.</param>
        internal static void CreateFilter<TFilter>(LibraryModule module, string clsid, string name, ref Type filterType, ref TFilter parserFilter)
        {
            TraceManager.TraceDebug($"Creating new instance of filter, Name = {name}, CLSID = {clsid}, Module = {module.FilePath}");
            try
            {
                filterType = Type.GetTypeFromCLSID(new Guid(clsid));
                parserFilter = (TFilter)ComHelper.CreateInstance(module, new Guid(clsid));
            }
            catch (Exception ex)
            {
                TraceManager.TraceError(ex, ex.Message);
                throw new NotSupportedException($"Cannot create the filter: Name = {name}, CLSID = {clsid}, Module = {module.FilePath}", ex);
            }
        }

        /// <summary>
        /// Removes all filters from a DirectShow graph
        /// </summary>
        /// <param name="graphBuilder">The DirectShow graph to remove all the filters from</param>
        internal static void RemoveAllFilters(IGraphBuilder graphBuilder)
        {
            if (graphBuilder == null)
                return;

            /* Gets the filter enumerator from the graph */
            int hr = graphBuilder.EnumFilters(out IEnumFilters enumFilters);
            DsError.ThrowExceptionForHR(hr);

            /* The list of filters from the DirectShow graph */
            List<IBaseFilter> filtersArray = new List<IBaseFilter>();

            try
            {
                /* This array is filled with reference to a filter */
                IBaseFilter[] filters = new IBaseFilter[1];
                IntPtr fetched = IntPtr.Zero;

                /* Get reference to all the filters */
                while (enumFilters.Next(filters.Length, filters, fetched) == 0)
                {
                    /* Add the filter to our array */
                    filtersArray.Add(filters[0]);
                }
            }
            finally
            {
                /* Enum filters is a COM, so release that */
                SafeRelease(enumFilters);
            }

            /* Loop over and release each COM */
            for (int i = 0; i < filtersArray.Count; i++)
            {
                hr = graphBuilder.RemoveFilter(filtersArray[i]);
                DsError.ThrowExceptionForHR(hr);

                SafeRelease(filtersArray[i]);
            }
        }

    }
}
