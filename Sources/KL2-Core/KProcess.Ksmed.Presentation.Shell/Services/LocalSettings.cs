using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Xml.Serialization;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Contient les settings utilisateur et de l'application.
    /// </summary>
    public class LocalSettings : NotifiableObject, ISettingsService, ILayoutPersistanceService, IDecoderInfoService, IVideoSpeedRatioPersistanceService, IVideoColorPersistanceService
    {
        private const string ApplicationSettingsFileName = "ApplicationSettings";
        private static ISettingsService _service;

        private static object _syncRoot = new object();

        /// <summary>
        /// Obtient l'instance en cours.
        /// </summary>
        public static ISettingsService Instance
        {
            get
            {
                if (_service == null)
                    if (KProcess.Presentation.Windows.DesignMode.IsInDesignMode)
                        _service = new LocalSettings();
                    else
                        _service = IoC.Resolve<IServiceBus>().Get<ISettingsService>();
                return _service;
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Settings"/>.
        /// </summary>
        internal LocalSettings()
        {
        }

        /// <summary>
        /// Initialise la gestion des paramèters..
        /// </summary>
        internal static void Initialize()
        {
            if (!KProcess.Presentation.Windows.DesignMode.IsInDesignMode)
            {
                var instance = new LocalSettings();
                IoC.Resolve<IServiceBus>().Register<ISettingsService>(instance);
                IoC.Resolve<IServiceBus>().Register<ILayoutPersistanceService>(instance);
                IoC.Resolve<IServiceBus>().Register<IVideoColorPersistanceService>(instance);
                IoC.Resolve<IServiceBus>().Register<IDecoderInfoService>(instance);
                IoC.Resolve<IServiceBus>().Register<IVideoSpeedRatioPersistanceService>(instance);
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le son est assourdi.
        /// </summary>
        public bool Mute
        {
            get { return Settings.Default.Mute; }
            set
            {
                if (Settings.Default.Mute != value)
                {
                    Settings.Default.Mute = value;
                    OnPropertyChanged("Mute");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'envoi de rapport est autorisé.
        /// </summary>
        public bool SendReport
        {
            get { return Settings.Default.SendReport; }
            set
            {
                if (Settings.Default.SendReport != value)
                {
                    Settings.Default.SendReport = value;
                    OnPropertyChanged("SendReport");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant la dernière langue utilisée.
        /// </summary>
        public string LastCulture
        {
            get { return Settings.Default.LastCulture; }
            set
            {
                if (Settings.Default.LastCulture != value)
                {
                    Settings.Default.LastCulture = value;
                    OnPropertyChanged("LastCulture");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant le dernier identifiant utilisateur utilisé.
        /// </summary>
        public string LastUserName
        {
            get { return Settings.Default.LastUserName; }
            set
            {
                if (Settings.Default.LastUserName != value)
                {
                    Settings.Default.LastUserName = value;
                    OnPropertyChanged("LastUserName");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit les extensions désactivées.
        /// </summary>
        public List<Guid> DisabledExtensions
        {
            get
            {
                if (ApplicationSettings != null)
                    return ApplicationSettings.DisabledExtensions;
                else
                    return null;
            }
            set
            {
                if (ApplicationSettings == null)
                    ApplicationSettings = new Shell.ApplicationSettings();
                ApplicationSettings.DisabledExtensions = value;
                OnPropertyChanged("DisabledExtensions");
            }
        }

        /// <summary>
        /// Met à jour
        /// </summary>
        public void Upgrade()
        {
            if (Settings.Default.IsNewVersion)
            {
                Settings.Default.Upgrade();
                Settings.Default.IsNewVersion = false;

                Settings.Default.GridLayoutPersistance = null;
                Settings.Default.DataGridLayoutPersistance = null;

                Save();
            }
        }

        /// <summary>
        /// Sauvegarde
        /// </summary>
        public void Save()
        {
            Settings.Default.Save();

            if (_appSettings != null)
            {
                SaveIsolatedMachineStore(_appSettings, ApplicationSettingsFileName);
            }
        }

        /// <summary>
        /// Recharge les données.
        /// </summary>
        public void Reload()
        {
            Settings.Default.Reload();
            _appSettings = null;
            var settings = ApplicationSettings;

            _dataGridLayout = null;
            _gridLayout = null;
        }

        private IDictionary<(int ProjectId, int VideoId), string> _videoColors;
        /// <summary>
        /// Obtient ou définit les couleurs des vidéos.
        /// </summary>
        public IDictionary<(int ProjectId, int VideoId), string> VideoColors
        {
            get
            {
                if (_videoColors == null)
                    _videoColors = DeserializeJson<VideoColorPersistance>(Settings.Default.VideoColorPersistance);
                return _videoColors;
            }
            set
            {
                Settings.Default.VideoColorPersistance = SerializeJson(value);
                _videoColors = value;
            }
        }

        [CollectionDataContract]
        public class VideoColorPersistance : Dictionary<(int ProjectId, int VideoId), string>
        {
            public VideoColorPersistance()
            {
            }

            public VideoColorPersistance(IDictionary<(int ProjectId, int VideoId), string> dictionary)
                : base(dictionary)
            {
            }
        }

        /// <summary>
        /// Charge des préférences au niveau de l'application pour une extension en particulier.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à stocker</typeparam>
        /// <param name="extensionId">L'identifiant unique de l'extension.</param>
        public T LoadExtensionApplicationSettings<T>(Guid extensionId)
        {
            var fileName = MakeExtIsolatedStorageFileName(extensionId, typeof(T));

            try
            {
                return LoadIsolatedMachineStore<T>(fileName);
            }
            catch (Exception e)
            {
                TraceManager.TraceError(e, "Erreur lors du chargement des settings de l'extension {0}", extensionId);
                return default(T);
            }
        }

        /// <summary>
        /// Sauvegarde les préférences au niveau de l'applicaiton pour une extension en particulier.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à stocker</typeparam>
        /// <param name="extensionId">L'identifiant unique de l'extension.</param>
        /// <param name="settings">Les préférences à sauvegarder.</param>
        public void SaveExtensionApplicationSettings<T>(Guid extensionId, T settings)
        {
            var fileName = MakeExtIsolatedStorageFileName(extensionId, typeof(T));

            try
            {
                SaveIsolatedMachineStore<T>(settings, fileName);
            }
            catch (Exception e)
            {
                TraceManager.TraceError(e, "Erreur lors de la sauvegarde des settings de l'extension {0}", extensionId);
            }
        }

        /// <summary>
        /// Crée un nom de fichier pour des préférences d'une extension.
        /// </summary>
        /// <param name="extensionId">L'identifiant de l'extension.</param>
        /// <param name="fileType">Le type du fichier à stocker.</param>
        /// <returns>Le nom du fichier.</returns>
        private string MakeExtIsolatedStorageFileName(Guid extensionId, Type fileType)
        {
            string raw = string.Format("{0}{1}", extensionId, fileType.FullName);

            // En faire un hash et prendre les 8 derniers caractères car la taille du nom de fichier est limitée.

            var hash = new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(raw))
                .Skip(12).Take(8);

            return StringExtensions.BytesToString(hash);
        }

        /// <summary>
        /// Sauvegarde des données dans l'isolated storage.
        /// </summary>
        /// <typeparam name="T">Le type de données à sauvegarder</typeparam>
        /// <param name="data">Les données.</param>
        /// <param name="fileName">Le nom du fichier.</param>
        private void SaveIsolatedMachineStore<T>(T data, string fileName)
        {
            lock (_syncRoot)
            {
                using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
                {
                    if (machineStore != null)
                    {
                        IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName
                            , System.IO.FileMode.Create, machineStore);

                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            serializer.Serialize(writer, data);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Charge des données depuis l'isolated storage.
        /// </summary>
        /// <typeparam name="T">Le type de données à charger</typeparam>
        /// <param name="fileName">Le nom du fichier.</param>
        /// <returns>Les données.</returns>
        private T LoadIsolatedMachineStore<T>(string fileName)
        {
            using (var machineStore = IsolatedStorageFile.GetMachineStoreForAssembly())
            {
                if (machineStore != null)
                {
                    if (machineStore.FileExists(fileName))
                    {
                        IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName
                            , System.IO.FileMode.Open, FileAccess.Read, FileShare.ReadWrite, machineStore);

                        using (StreamReader reader = new StreamReader(stream))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(T));
                            return (T)serializer.Deserialize(reader);
                        }
                    }
                }
            }

            return default(T);
        }

        private ApplicationSettings _appSettings;
        /// <summary>
        /// Obtient ou définit les préférences de l'application.
        /// </summary>
        private ApplicationSettings ApplicationSettings
        {
            get
            {
                if (_appSettings == null)
                {
                    try
                    {
                        _appSettings = LoadIsolatedMachineStore<ApplicationSettings>(ApplicationSettingsFileName);
                    }
                    catch (Exception)
                    {
                        _appSettings = new ApplicationSettings();
                    }
                }
                return _appSettings;
            }
            set
            {
                _appSettings = value;
            }
        }

        #region ILayoutPersistanceService Members

        private GridLayoutPersistance _gridLayout;
        /// <summary>
        /// Obtient ou définit l'agencement des grilles.
        /// </summary>
        private GridLayoutPersistance GridLayout
        {
            get
            {
                if (_gridLayout == null)
                    _gridLayout = DeserializeJson<GridLayoutPersistance>(Settings.Default.GridLayoutPersistance);
                return _gridLayout;
            }
            set
            {
                Settings.Default.GridLayoutPersistance = SerializeJson(value);
                _gridLayout = value;
            }
        }


        private DataGridColumnsProjectPersistance _dataGridLayout;
        /// <summary>
        /// Obtient ou définit l'agencement des grilles de données.
        /// </summary>
        private DataGridColumnsProjectPersistance DataGridLayout
        {
            get
            {
                if (_dataGridLayout == null)
                    _dataGridLayout = DeserializeJson<DataGridColumnsProjectPersistance>(Settings.Default.DataGridLayoutPersistance);
                return _dataGridLayout;
            }
            set
            {
                Settings.Default.DataGridLayoutPersistance = SerializeJson(value);
                _dataGridLayout = value;
            }
        }

        /// <summary>
        /// Tente de récupérer le layout sauvegardé.
        /// </summary>
        /// <param name="gridName">Le nom de la grille.</param>
        /// <param name="columns">Les colonnes de la grille.</param>
        /// <param name="rows">Les lignes de la grilles.</param>
        /// <returns>
        ///   <c>true</c> si la récupération a réussi.
        /// </returns>
        public bool GridTryRetrieve(string gridName, out GridLength[] columns, out GridLength[] rows)
        {
            if (GridLayout != null)
            {
                var converter = new GridLengthConverter();

                var data = GridLayout;
                if (data != null)
                {
                    if (data.ContainsKey(gridName) && data[gridName].Length == 2)
                    {
                        try
                        {
                            columns = data[gridName][0].Select(s => (GridLength)converter.ConvertFromString(s)).ToArray();
                            rows = data[gridName][1].Select(s => (GridLength)converter.ConvertFromString(s)).ToArray();
                            return columns != null && rows != null;
                        }
                        catch (Exception e)
                        {
                            // Laisser sortir avec return false
                            Console.WriteLine($"Erreur LocalSettings - GridTryRetrieve : {e.Message}");
                        }
                    }
                }
            }

            columns = null;
            rows = null;
            return false;

        }

        /// <summary>
        /// Persiste le layout pour la grille spécifiée
        /// </summary>
        /// <param name="gridName">Le nom de la grille.</param>
        /// <param name="columns">Les colonnes de la grille.</param>
        /// <param name="rows">Les lignes de la grilles.</param>
        public void GridPersist(string gridName, IEnumerable<GridLength> columns, IEnumerable<GridLength> rows)
        {
            var data = this.GridLayout;
            if (data == null)
                data = new GridLayoutPersistance();

            var converter = new GridLengthConverter();

            data[gridName] =
                new string[][]
                {
                    columns.Select(c => converter.ConvertToString(c)).ToArray(),
                    rows.Select(r => converter.ConvertToString(r)).ToArray(),
                };

            this.GridLayout = data;
        }

        /// <summary>
        /// Tente de récupérer le layout sauvegardé pour le DataGrid spécifié.
        /// </summary>
        /// <returns>
        ///   Le layout, ou <c>null</c> s'il n'est pas défini.
        /// </returns>
        public IDictionary<GanttGridView, DataGridLayout> DataGridTryRetrieve()
        {
            var currentProject = IoC.Resolve<IServiceBus>().Get<IProjectManagerService>().CurrentProject;
            if (currentProject != null)
            {
                var data = DataGridLayout;
                if (data != null)
                {
                    if (data.ContainsKey(currentProject.ProjectId))
                    {
                        var layout = data[currentProject.ProjectId].ToDictionary(
                            kvp => kvp.Key,
                            kvp => new DataGridLayout()
                            {
                                ColumnsVisibilities = kvp.Value.Visibilities ?? new Dictionary<string, bool>(),
                                ColumnsOrder = kvp.Value.Order
                            });

                        return layout;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Persiste le layout pour le datagrid spécifiée
        /// </summary>
        /// <param name="layout">Le layout.</param>
        public void DataGridPersist(IDictionary<GanttGridView, DataGridLayout> layout)
        {
            var currentProject = IoC.Resolve<IServiceBus>().Get<IProjectManagerService>().CurrentProject;
            if (currentProject != null)
            {
                var projects = DataGridLayout;
                if (projects == null)
                    projects = new DataGridColumnsProjectPersistance();

                DataGridColumnsViewPersistance projectViews;
                if (projects.ContainsKey(currentProject.ProjectId))
                    projectViews = projects[currentProject.ProjectId];
                else
                {
                    projectViews = new DataGridColumnsViewPersistance();
                    projects[currentProject.ProjectId] = projectViews;
                }

                foreach (var kvp in layout)
                {
                    DataGridColumnsPersistance columns;
                    if (projectViews.ContainsKey(kvp.Key))
                        columns = projectViews[kvp.Key];
                    else
                    {
                        columns = new DataGridColumnsPersistance();
                        projectViews[kvp.Key] = columns;
                    }

                    if (columns.Visibilities == null)
                        columns.Visibilities = new Dictionary<string, bool>();

                    columns.Visibilities = new Dictionary<string, bool>(kvp.Value.ColumnsVisibilities);
                    columns.Order = OrderColumns(columns.Order, kvp.Value.ColumnsOrder);
                }

                DataGridLayout = projects;
            }
        }

        /// <summary>
        /// Merge l'ordre des colonnes entre l'ancien ordre et le nouvel.
        /// </summary>
        /// <param name="original">L'ordre original.</param>
        /// <param name="newOrder">le nouvel ordre.</param>
        /// <returns></returns>
        internal string[] OrderColumns(string[] original, string[] newOrder)
        {
            if (original == null)
                return newOrder;

            var notPresentInNew = original.Except(newOrder).ToArray();

            var ret = new List<string>(newOrder.Length + notPresentInNew.Length);
            foreach (var c in newOrder)
                ret.Add(c);

            foreach (var c in notPresentInNew)
            {
                var index = original.IndexOf(c);
                if (index == 0)
                    ret.Insert(0, c);
                else
                {
                    var previous = original[index - 1];
                    var previousIndexInNew = ret.IndexOf(previous);
                    if (previousIndexInNew != -1)
                    {
                        ret.Insert(previousIndexInNew + 1, c);
                    }
                    else
                        ret.Insert(index, c);
                }
            }

            return ret.ToArray();

        }

        /// <summary>
        /// Désérialise la propriété définie en Json.
        /// </summary>
        /// <typeparam name="T">Le type de la donnée.</typeparam>
        /// <param name="content">le contenu sérialisé.</param>
        /// <returns>L'objet désérialisé.</returns>
        private T DeserializeJson<T>(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    TypeDescriptor.AddAttributes(typeof((Int32, Int32)), new TypeConverterAttribute(typeof(ValueTupleConverter<Int32, Int32>)));
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch (Exception e)
                {
                    this.TraceDebug("Impossible de désérialiser", e);
                    return default(T);
                }
            }
            else
                return default(T);
        }

        /// <summary>
        /// Sérialise l'objet spéficié en JSON.
        /// </summary>
        /// <typeparam name="T">Le type de la donnée.</typeparam>
        /// <param name="data">la donnée.</param>
        /// <returns>Le contenu sérialisé</returns>
        private string SerializeJson<T>(T data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception e)
            {
                this.TraceError("Impossible de sérialiser", e);
                return null;
            }

        }

        /// <summary>
        /// Contient les données de la persistence du layout des grilles.
        /// </summary>
        [CollectionDataContract]
        public class GridLayoutPersistance : Dictionary<string, string[][]>
        {
        }

        [CollectionDataContract]
        public class DataGridColumnsProjectPersistance : Dictionary<int, DataGridColumnsViewPersistance>
        {
            public DataGridColumnsProjectPersistance()
            {
            }

            public DataGridColumnsProjectPersistance(IDictionary<int, DataGridColumnsViewPersistance> dictionary)
                : base(dictionary)
            {

            }
        }

        [CollectionDataContract]
        public class DataGridColumnsViewPersistance : Dictionary<GanttGridView, DataGridColumnsPersistance>
        {
            public DataGridColumnsViewPersistance()
            {
            }

            public DataGridColumnsViewPersistance(IDictionary<GanttGridView, DataGridColumnsPersistance> dictionary)
                : base(dictionary)
            {

            }
        }

        [DataContract]
        public class DataGridColumnsPersistance
        {
            public DataGridColumnsPersistance()
            {
            }

            public DataGridColumnsPersistance(IDictionary<string, bool> dictionary)
            {
                Visibilities = new Dictionary<string, bool>(dictionary);
            }

            /// <summary>
            /// Obtient ou définit l'ordre des colonnes.
            /// </summary>
            [DataMember]
            public string[] Order { get; set; }

            /// <summary>
            /// Obtient ou définit la visibilité des colonnes.
            /// </summary>
            [DataMember]
            public Dictionary<string, bool> Visibilities { get; set; }
        }

        #endregion

        #region IVideoSpeedRatioPersistanceService Members

        private VideoSpeedRatiosProjectPersistance _videoSpeedRatios;
        /// <summary>
        /// Obtient ou définit la vitesse de lecture des vidéos.
        /// </summary>
        private VideoSpeedRatiosProjectPersistance VideoSpeedRatios
        {
            get
            {
                if (_videoSpeedRatios == null)
                    _videoSpeedRatios = DeserializeJson<VideoSpeedRatiosProjectPersistance>(Settings.Default.VideosSpeedRatioPersistance);
                return _videoSpeedRatios;
            }
            set
            {
                Settings.Default.VideosSpeedRatioPersistance = SerializeJson(value);
                _videoSpeedRatios = value;
            }
        }

        /// <summary>
        /// Obtient le coefficient vidéo pour le fichier spécifié.
        /// </summary>
        /// <param name="file">Le fichier.</param>
        /// <returns>
        /// La valeur
        /// </returns>
        public double? GetSpeedRatio(Uri file)
        {
            var currentProject = IoC.Resolve<IServiceBus>().Get<IProjectManagerService>().CurrentProject;
            if (currentProject != null)
            {
                var projects = VideoSpeedRatios;
                if (projects != null && projects.ContainsKey(currentProject.ProjectId))
                {
                    var project = projects[currentProject.ProjectId];

                    if (project.ContainsKey(file))
                        return project[file];
                }
            }
            return null;
        }

        /// <summary>
        /// Persiste la vitesse pour le fichier spécifié.
        /// </summary>
        /// <param name="file">Le fichier.</param>
        /// <param name="ratio">La vitesse.</param>
        public void PersistSpeedRatio(Uri file, double ratio)
        {
            var currentProject = IoC.Resolve<IServiceBus>().Get<IProjectManagerService>().CurrentProject;
            if (currentProject != null)
            {
                var projects = VideoSpeedRatios;
                if (projects == null)
                    projects = new VideoSpeedRatiosProjectPersistance();

                VideoSpeedRatiosPersistance project;
                if (projects.ContainsKey(currentProject.ProjectId))
                    project = projects[currentProject.ProjectId];
                else
                {
                    project = new VideoSpeedRatiosPersistance();
                    projects[currentProject.ProjectId] = project;
                }

                project[file] = ratio;

                VideoSpeedRatios = projects;
            }
        }

        [CollectionDataContract]
        public class VideoSpeedRatiosProjectPersistance : Dictionary<int, VideoSpeedRatiosPersistance>
        {
            public VideoSpeedRatiosProjectPersistance()
            {
            }

            public VideoSpeedRatiosProjectPersistance(IDictionary<int, VideoSpeedRatiosPersistance> dictionary)
                : base(dictionary)
            {
            }
        }

        [CollectionDataContract]
        public class VideoSpeedRatiosPersistance : Dictionary<Uri, double>
        {
            public VideoSpeedRatiosPersistance()
            {
            }

            public VideoSpeedRatiosPersistance(IDictionary<Uri, double> dictionary)
                : base(dictionary)
            {
            }
        }

        #endregion

        #region IDecoderInfoService Members

        /// <summary>
        /// Obtient ou définit les profils de décodage.
        /// </summary>
        public FiltersConfiguration FiltersConfiguration { get; set; }

        /// <inheritdoc />
        public bool InitializeFiltersConfiguration()
        {
            var config = Configuration.DirectShow.ConfigurationParser.Parse();
            this.FiltersConfiguration = config;
            return config != null;
        }

        #endregion
    }

    /// <summary>
    /// Contient les préférences de l'application.
    /// </summary>
    [DataContract]
    public class ApplicationSettings
    {

        /// <summary>
        /// Obtient ou définit les extensions désactivées.
        /// </summary>
        [DataMember]
        public List<Guid> DisabledExtensions { get; set; }
    }

    public class ValueTupleConverter<T1, T2> : TypeConverter
        where T1 : struct where T2 : struct
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var elements = ((String)value).Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            return (JsonConvert.DeserializeObject<T1>(elements.First()), JsonConvert.DeserializeObject<T2>(elements.Last()));
        }
    }
}
