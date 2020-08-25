using KProcess.Ksmed.Business;
using KProcess.Ksmed.Ext.Kprocess.ViewModels.Interfaces;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Ksmed.Ext.Kprocess.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de configuration.
    /// </summary>
    public class ConfigurationViewModel : ExtensionConfigurationViewModelBase, IConfigurationViewModel
    {
        public Dictionary<byte, Referential> Referentials { get; private set; }

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override void OnLoading()
        {
            // Charger les référentiels
            var refService = IoC.Resolve<IServiceBus>().Get<IReferentialsService>();
            Referentials = refService.GetApplicationReferentials().ToDictionary(x => x.ReferentialId, x => x);

            // Charger les settings.
            var service = IoC.Resolve<IServiceBus>().Get<ISettingsService>(); ;

            var settings = service.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);
            if (settings == null)
            {
                // Charger les valeurs par défaut.
                settings = new Settings
                {
                    ExportExcelModelPath = string.Empty,
                    MultiThreading = false,
                    DefaultExportDirectory_Archivage = string.Empty,
                    DefaultExportDirectory_Redistribuable = string.Empty,
                    DefaultExportVideoDirectory = string.Empty,
                    ImportantTaskColor = new Color { A = 255, R = 255, G = 0, B = 0 }, //Rouge
                    ImageHeightRef1 = 50,
                    ImageHeightRef2 = 50,
                    ImageHeightRef3 = 50,
                    ImageHeightRef4 = 50,
                    ImageHeightRef5 = 50,
                    ImageHeightRef6 = 50,
                    ImageHeightRef7 = 50,
                    NbImagesPerLineRef1 = 8,
                    NbImagesPerLineRef2 = 8,
                    NbImagesPerLineRef3 = 8,
                    NbImagesPerLineRef4 = 8,
                    NbImagesPerLineRef5 = 8,
                    NbImagesPerLineRef6 = 8,
                    NbImagesPerLineRef7 = 8,
                    ImageHeightLineRef1 = 50,
                    ImageHeightLineRef2 = 50,
                    ImageHeightLineRef3 = 50,
                    ImageHeightLineRef4 = 50,
                    ImageHeightLineRef5 = 50,
                    ImageHeightLineRef6 = 50,
                    ImageHeightLineRef7 = 50,
                    NbImagesPerLineLineRef1 = 4,
                    NbImagesPerLineLineRef2 = 4,
                    NbImagesPerLineLineRef3 = 4,
                    NbImagesPerLineLineRef4 = 4,
                    NbImagesPerLineLineRef5 = 4,
                    NbImagesPerLineLineRef6 = 4,
                    NbImagesPerLineLineRef7 = 4,
                    GanttGroupColor = new Color { A = 255, R = 0, G = 0, B = 0 }, //Noir
                    GanttNoCategoryColor = new Color { A = 255, R = 160, G = 160, B = 160 },//Gris,
                    VideoMarkingIsEnabled = false,
                    SlowMotionIsEnabled = false,
                    OverlayTextVideo = "{COMPANY NAME} INTERNAL ONLY",
                    DurationMini = 5,
                    HorizontalAlignement = Enums.EHorizontalAlign.Center,
                    VerticalAlignement = Enums.EVerticalAlign.Top,
                    ArchivageIsEnabled = false,
                    RedistribuableIsEnabled = false,
                    ExcelExportIsEnabled = true,
                    ExcelFileNameMarkingIsEnabled = false,
                    VideoExportIsEnabled = true,
                    ExportOnlyKeyTasksVideos = false,
                    WBSMarkingIsEnabled = false,
                    IsReadOnlyEnabled_Archivage = true,
                    IsReadOnlyEnabled_Redistribuable = true,
                    TaskTitleSize = 14,
                    CaptionSizeRef1 = 11,
                    CaptionSizeRef2 = 11,
                    CaptionSizeRef3 = 11,
                    CaptionSizeRef4 = 11,
                    CaptionSizeRef5 = 11,
                    CaptionSizeRef6 = 11,
                    CaptionSizeRef7 = 11,
                    CaptionSizeLineRef1 = 11,
                    CaptionSizeLineRef2 = 11,
                    CaptionSizeLineRef3 = 11,
                    CaptionSizeLineRef4 = 11,
                    CaptionSizeLineRef5 = 11,
                    CaptionSizeLineRef6 = 11,
                    CaptionSizeLineRef7 = 11,
                    ThumbnailHeight = 150
                };
            }

            if (settings.TaskTitleSize == 0)
                settings.TaskTitleSize = 14;
            if (settings.CaptionSizeRef1 == 0)
                settings.CaptionSizeRef1 = 11;
            if (settings.CaptionSizeRef2 == 0)
                settings.CaptionSizeRef2 = 11;
            if (settings.CaptionSizeRef3 == 0)
                settings.CaptionSizeRef3 = 11;
            if (settings.CaptionSizeRef4 == 0)
                settings.CaptionSizeRef4 = 11;
            if (settings.CaptionSizeRef5 == 0)
                settings.CaptionSizeRef5 = 11;
            if (settings.CaptionSizeRef6 == 0)
                settings.CaptionSizeRef6 = 11;
            if (settings.CaptionSizeRef7 == 0)
                settings.CaptionSizeRef7 = 11;
            if (settings.CaptionSizeLineRef1 == 0)
                settings.CaptionSizeLineRef1 = 11;
            if (settings.CaptionSizeLineRef2 == 0)
                settings.CaptionSizeLineRef2 = 11;
            if (settings.CaptionSizeLineRef3 == 0)
                settings.CaptionSizeLineRef3 = 11;
            if (settings.CaptionSizeLineRef4 == 0)
                settings.CaptionSizeLineRef4 = 11;
            if (settings.CaptionSizeLineRef5 == 0)
                settings.CaptionSizeLineRef5 = 11;
            if (settings.CaptionSizeLineRef6 == 0)
                settings.CaptionSizeLineRef6 = 11;
            if (settings.CaptionSizeLineRef7 == 0)
                settings.CaptionSizeLineRef7 = 11;
            if (settings.ThumbnailHeight == 0)
                settings.ThumbnailHeight = 150;
            if (settings.ImageHeightRef1 == 0)
                settings.ImageHeightRef1 = 50;
            if (settings.ImageHeightRef2 == 0)
                settings.ImageHeightRef2 = 50;
            if (settings.ImageHeightRef3 == 0)
                settings.ImageHeightRef3 = 50;
            if (settings.ImageHeightRef4 == 0)
                settings.ImageHeightRef4 = 50;
            if (settings.ImageHeightRef5 == 0)
                settings.ImageHeightRef5 = 50;
            if (settings.ImageHeightRef6 == 0)
                settings.ImageHeightRef6 = 50;
            if (settings.ImageHeightRef7 == 0)
                settings.ImageHeightRef7 = 50;
            if (settings.NbImagesPerLineRef1 == 0)
                settings.NbImagesPerLineRef1 = 8;
            if (settings.NbImagesPerLineRef2 == 0)
                settings.NbImagesPerLineRef2 = 8;
            if (settings.NbImagesPerLineRef3 == 0)
                settings.NbImagesPerLineRef3 = 8;
            if (settings.NbImagesPerLineRef4 == 0)
                settings.NbImagesPerLineRef4 = 8;
            if (settings.NbImagesPerLineRef5 == 0)
                settings.NbImagesPerLineRef5 = 8;
            if (settings.NbImagesPerLineRef6 == 0)
                settings.NbImagesPerLineRef6 = 8;
            if (settings.NbImagesPerLineRef7 == 0)
                settings.NbImagesPerLineRef7 = 8;
            if (settings.ImageHeightLineRef1 == 0)
                settings.ImageHeightLineRef1 = 50;
            if (settings.ImageHeightLineRef2 == 0)
                settings.ImageHeightLineRef2 = 50;
            if (settings.ImageHeightLineRef3 == 0)
                settings.ImageHeightLineRef3 = 50;
            if (settings.ImageHeightLineRef4 == 0)
                settings.ImageHeightLineRef4 = 50;
            if (settings.ImageHeightLineRef5 == 0)
                settings.ImageHeightLineRef5 = 50;
            if (settings.ImageHeightLineRef6 == 0)
                settings.ImageHeightLineRef6 = 50;
            if (settings.ImageHeightLineRef7 == 0)
                settings.ImageHeightLineRef7 = 50;
            if (settings.NbImagesPerLineLineRef1 == 0)
                settings.NbImagesPerLineLineRef1 = 4;
            if (settings.NbImagesPerLineLineRef2 == 0)
                settings.NbImagesPerLineLineRef2 = 4;
            if (settings.NbImagesPerLineLineRef3 == 0)
                settings.NbImagesPerLineLineRef3 = 4;
            if (settings.NbImagesPerLineLineRef4 == 0)
                settings.NbImagesPerLineLineRef4 = 4;
            if (settings.NbImagesPerLineLineRef5 == 0)
                settings.NbImagesPerLineLineRef5 = 4;
            if (settings.NbImagesPerLineLineRef6 == 0)
                settings.NbImagesPerLineLineRef6 = 4;
            if (settings.NbImagesPerLineLineRef7 == 0)
                settings.NbImagesPerLineLineRef7 = 4;
            if (settings.DurationMini == 0)
                settings.DurationMini = 5;

            Settings = settings;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override void OnInitializeDesigner()
        {
            Settings = new Settings
            {
                ExportExcelModelPath = "c:/test"
            };
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        public override void OnNavigatingAway()
        {
            base.OnNavigatingAway();

            // Sauvegarder les settings
            var service = IoC.Resolve<IServiceBus>().Get<ISettingsService>();
            service.SaveExtensionApplicationSettings(KprocessExtension.Id, Settings);
        }

        #endregion

        #region Propriétés

        private Settings _settings;
        /// <summary>
        /// Obtient les préférences.
        /// </summary>
        public Settings Settings
        {
            get { return _settings; }
            private set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged(nameof(Settings));
                }
            }
        }

        #endregion

        #region Commandes


        private Command _browseModelFileCommand;
        /// <summary>
        /// Obtient une commande permettant de spécifier le chemin vers le modèle.
        /// </summary>
        public ICommand BrowseModelFileCommand
        {
            get
            {
                if (_browseModelFileCommand == null)
                    _browseModelFileCommand = new Command(() =>
                    {

                        var file = DialogFactory.GetDialogView<IOpenFileDialog>().Show(string.Empty, filter: "Excel 2007 (*.xltm)|*.xltm").FirstOrDefault();

                        if (!string.IsNullOrEmpty(file))
                        {
                            Settings.ExportExcelModelPath = file;
                            OnPropertyChanged(nameof(Settings));
                        }

                    });
                return _browseModelFileCommand;
            }
        }

        private Command _browseDefaultExportDirectoryArchivageCommand;
        /// <summary>
        /// Obtient une commande permettant de spécifier le chemin vers le modèle en mode archivage.
        /// </summary>
        public ICommand BrowseDefaultExportDirectoryArchivageCommand
        {
            get
            {
                if (_browseDefaultExportDirectoryArchivageCommand == null)
                    _browseDefaultExportDirectoryArchivageCommand = new Command(() =>
                    {

                        var folder = DialogFactory.GetDialogView<IOpenFolderDialog>().Show(string.Empty);

                        if (!string.IsNullOrEmpty(folder))
                        {
                            Settings.DefaultExportDirectory_Archivage = folder;
                            OnPropertyChanged(nameof(Settings));
                        }
                    });
                return _browseDefaultExportDirectoryArchivageCommand;
            }
        }

        private Command _browseDefaultExportDirectoryRedistribuableCommand;
        /// <summary>
        /// Obtient une commande permettant de spécifier le chemin vers le modèle en mode redistribuable.
        /// </summary>
        public ICommand BrowseDefaultExportDirectoryRedistribuableCommand
        {
            get
            {
                if (_browseDefaultExportDirectoryRedistribuableCommand == null)
                    _browseDefaultExportDirectoryRedistribuableCommand = new Command(() =>
                    {

                        var folder = DialogFactory.GetDialogView<IOpenFolderDialog>().Show(string.Empty);

                        if (!string.IsNullOrEmpty(folder))
                        {
                            Settings.DefaultExportDirectory_Redistribuable = folder;
                            OnPropertyChanged(nameof(Settings));
                        }
                    });
                return _browseDefaultExportDirectoryRedistribuableCommand;
            }
        }

        private Command _browseDefaultExportVideoDirectoryCommand;
        /// <summary>
        /// Obtient une commande permettant de spécifier le chemin vers le modèle.
        /// </summary>
        public ICommand BrowseDefaultExportVideoDirectoryCommand
        {
            get
            {
                if (_browseDefaultExportVideoDirectoryCommand == null)
                    _browseDefaultExportVideoDirectoryCommand = new Command(() =>
                    {
                        var folder = DialogFactory.GetDialogView<IOpenFolderDialog>().Show(string.Empty);

                        if (!string.IsNullOrEmpty(folder))
                        {
                            Settings.DefaultExportVideoDirectory = folder;
                            OnPropertyChanged(nameof(Settings));
                        }

                    });
                return _browseDefaultExportVideoDirectoryCommand;
            }
        }


        private Command<Enums.EHorizontalAlign> _setHorizontalAlign;
        public ICommand SetHorizontalAlign
        {
            get
            {
                if (_setHorizontalAlign == null)
                    _setHorizontalAlign = new Command<Enums.EHorizontalAlign>((e) =>
                    {                      
                            Settings.HorizontalAlignement = e;
                            OnPropertyChanged(nameof(Settings));    
                    });
                return _setHorizontalAlign;
            }
        }


        private Command<Enums.EVerticalAlign> _setVerticalAlign;
        public ICommand SetVerticalAlign
        {
            get
            {
                if (_setVerticalAlign == null)
                    _setVerticalAlign = new Command<Enums.EVerticalAlign>((e) =>
                    {
                        Settings.VerticalAlignement = e;
                        OnPropertyChanged(nameof(Settings));
                    });
                return _setVerticalAlign;
            }
        }

        #endregion
    }
}
