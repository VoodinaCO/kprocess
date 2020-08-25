using KProcess.Ksmed.Ext.Kprocess.Enums;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace KProcess.Ksmed.Ext.Kprocess
{
    /// <summary>
    /// Représente les préférences de l'extension 3D-Plus.
    /// </summary>
    [DataContract]
    public class Settings
    {
        //#if DEBUG
        //        private const string modelRelativePath = @"Resources\Procedure KL2 LISI AERO 3.0 32 bits.xltm";
        //        public Settings()
        //        {
        //            this.ExportExcelModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Extensions\\Lisi\\", modelRelativePath);
        //        }
        //#endif

        /// <summary>
        /// Obtient ou définit pour l'export Excel le chemin vers le fichier modèle.
        /// </summary>
        [DataMember]
        public string ExportExcelModelPath { get; set; }

        /// <summary>
        /// Obtient ou définit le chemin par défaut vers les exports en mode archivage.
        /// </summary>
        [DataMember]
        public string DefaultExportDirectory_Archivage { get; set; }

        /// <summary>
        /// Obtient ou définit le chemin par défaut vers les exports en mode redistribuable.
        /// </summary>
        [DataMember]
        public string DefaultExportDirectory_Redistribuable { get; set; }

        [DataMember]
        public string DefaultExportVideoDirectory { get; set; }

        [DataMember]
        public EHorizontalAlign HorizontalAlignement { get; set; }

        [DataMember]
        public EVerticalAlign VerticalAlignement { get; set; }

        /// <summary>
        /// Obtient ou définit pour l'export Excel l'utilisation ou non du multithreading.
        /// </summary>
        [DataMember]
        public bool MultiThreading { get; set; }

        [DataMember]
        public bool VideoExportIsEnabled{get;set; }

        [DataMember]
        public bool ExportOnlyKeyTasksVideos { get; set; }

        [DataMember]
        public bool ArchivageIsEnabled { get; set; }

        [DataMember]
        public bool RedistribuableIsEnabled { get; set; }

        [DataMember]
        public bool ExcelExportIsEnabled { get; set; }

        [DataMember]
        public bool KeyTaskFreeTextField1 { get; set; }

        [DataMember]
        public bool KeyTaskFreeTextField2 { get; set; }

        [DataMember]
        public bool KeyTaskFreeTextField3 { get; set; }

        [DataMember]
        public bool KeyTaskFreeTextField4 { get; set; }

        [DataMember]
        public bool KeyTaskOperator1 { get; set; }

        [DataMember]
        public bool KeyTaskOperator2 { get; set; }

        [DataMember]
        public bool KeyTaskOperator3 { get; set; }

        [DataMember]
        public Color ImportantTaskColor { get; set; }

        [DataMember]
        public bool AddCaptionToRef1 { get; set; }

        [DataMember]
        public bool AddCaptionToRef2 { get; set; }

        [DataMember]
        public bool AddCaptionToRef3 { get; set; }

        [DataMember]
        public bool AddCaptionToRef4 { get; set; }

        [DataMember]
        public bool AddCaptionToRef5 { get; set; }

        [DataMember]
        public bool AddCaptionToRef6 { get; set; }

        [DataMember]
        public bool AddCaptionToRef7 { get; set; }

        [DataMember]
        public int CaptionSizeRef1 { get; set; }

        [DataMember]
        public int CaptionSizeRef2 { get; set; }

        [DataMember]
        public int CaptionSizeRef3 { get; set; }

        [DataMember]
        public int CaptionSizeRef4 { get; set; }

        [DataMember]
        public int CaptionSizeRef5 { get; set; }

        [DataMember]
        public int CaptionSizeRef6 { get; set; }

        [DataMember]
        public int CaptionSizeRef7 { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef1Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef2Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef3Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef4Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef5Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef6Action { get; set; }

        [DataMember]
        public bool AddCaptionToLineRef7Action { get; set; }

        [DataMember]
        public int CaptionSizeLineRef1 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef2 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef3 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef4 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef5 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef6 { get; set; }

        [DataMember]
        public int CaptionSizeLineRef7 { get; set; }

        [DataMember]
        public int ImageHeightRef1 { get; set; }

        [DataMember]
        public int ImageHeightRef2 { get; set; }

        [DataMember]
        public int ImageHeightRef3 { get; set; }

        [DataMember]
        public int ImageHeightRef4 { get; set; }

        [DataMember]
        public int ImageHeightRef5 { get; set; }

        [DataMember]
        public int ImageHeightRef6 { get; set; }

        [DataMember]
        public int ImageHeightRef7 { get; set; }

        [DataMember]
        public int ImageHeightLineRef1 { get; set; }

        [DataMember]
        public int ImageHeightLineRef2 { get; set; }

        [DataMember]
        public int ImageHeightLineRef3 { get; set; }

        [DataMember]
        public int ImageHeightLineRef4 { get; set; }

        [DataMember]
        public int ImageHeightLineRef5 { get; set; }

        [DataMember]
        public int ImageHeightLineRef6 { get; set; }

        [DataMember]
        public int ImageHeightLineRef7 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef1 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef2 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef3 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef4 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef5 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef6 { get; set; }

        [DataMember]
        public int NbImagesPerLineRef7 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef1 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef2 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef3 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef4 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef5 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef6 { get; set; }

        [DataMember]
        public int NbImagesPerLineLineRef7 { get; set; }

        [DataMember]
        public bool VideoMarkingIsEnabled { get; set; }

        [DataMember]
        public bool SlowMotionIsEnabled { get; set; }

        [DataMember]
        public string OverlayTextVideo { get; set; }

        [DataMember]
        public double DurationMini { get; set; }

        [DataMember]
        public bool WBSMarkingIsEnabled { get; set; }

        [DataMember]
        public bool ExcelFileNameMarkingIsEnabled { get; set; }

        [DataMember]
        public Color GanttGroupColor { get; set; }

        [DataMember]
        public Color GanttNoCategoryColor { get; set; }

        [DataMember]
        public int TaskTitleSize { get; set; }

        [DataMember]
        public int ThumbnailHeight { get; set; }

        [DataMember]
        public bool IsReadOnlyEnabled_Archivage { get; set; }

        [DataMember]
        public bool IsReadOnlyEnabled_Redistribuable { get; set; }

        public static Settings GetDefault() =>
            new Settings();
    }
}
