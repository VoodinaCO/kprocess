using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    class ExportRanges
    {
        public const string Owner = "Owner";
        public const string ExportDate = "Date_redac";
        public const string MinI = "min_I";
        public const string MaxI = "max_I";
        public const string ProcessI = "Scenario.ProcessDurationI";
        public const string ProcessIE = "Scenario.ProcessDurationIE";

        public const string VideoDirectory = "Rep_video";

        public const string RepVideoRelatif = "Rep_video_relatif";       

        public const string ScenarioSheetOldName = "Scenario";
        public const string ActionReductionRatio = "KAction.ReductionPerCent";
        public const string ActionSaving = "KAction.Saving";
        public const string ActionSolution = "KAction.Solution";
        public const string ActionApproved = "KAction.Approved";

        public const string ExcelFolderName = "ExcelFolderName";

        public const string Category = "Category";
        public const string CategoryList = "CategoryList";

        public const string Operator = "Operator";
        public const string OperatorList = "OperatorList";

        public const string Equipment = "Equipment";
        public const string EquipmentList = "EquipmentList";

        public const string VideoList = "VideoList";
        public const string SplittedVideoList = "SplittedVideoList";
        public const string VideoApplicable = "Video.Applicable";

        public const string KActionVideoWithExtension = "KAction.VideoWithExtension";
        public const string KActionVideoPath = "KAction.VideoPath";
        public const string KActionHasSnapshot = "KAction.HasSnapshot";
        public const string KActionThumbnailPath = "KAction.ThumbnailPath";
        public const string KActionThumbnailWithExtension = "KAction.ThumbnailWithExtension";

        private static Dictionary<int, Dictionary<string, string>> _refDictionary;
        public static Dictionary<int, Dictionary<string, string>> RefDictionary
        {
            get
            {
                if (_refDictionary == null)
                {
                    _refDictionary = new Dictionary<int, Dictionary<string, string>>();
                    for(int i = 1; i <= 7; i++)
                        _refDictionary.Add(i, RefConstructDictionary.ToDictionary(kvp => kvp.Key, kvp => string.Format(kvp.Value, i)));
                }
                return _refDictionary;
            }
        }

        private static Dictionary<string, string> RefConstructDictionary = new Dictionary<string, string>
        {
            ["Ref"] = "Ref{0}",
            ["KActionNbImages"] = "KAction.NbImagesRef{0}",
            ["List"] = "Ref{0}List",
            ["Label"] = "Ref{0}Label",
            ["ScenarioLabelsText"] = "Scenario.Ref{0}LabelsText",
            ["ScenarioImages"] = "Scenario.Ref{0}Images",
            ["ScenarioLabelsLinks"] = "Scenario.Ref{0}LabelsLink",
            ["ScenarioLabelsLinksList"] = "Scenario.Ref{0}LabelsLinkList",
            ["KAction"] = "KAction.Ref{0}",
            ["KActionImage"] = "KAction.Ref{0}Image",
            ["ImageHeight"] = "ImageHeightRef{0}",
            ["ImageHeightLine"] = "ImageHeightLineRef{0}",
            ["NbImagesPerLine"] = "NbImagesPerLineRef{0}",
            ["NbImagesPerLineLine"] = "NbImagesPerLineLineRef{0}"
        };
        
        public const string KActionGantt = "KAction.Gantt";
        
        public const string ActionList = "ActionList";

        public const string KActionVideo = "KAction.Video";

        public const string KActionHeightRow = "KAction.HeightRow";

        public const string ImportantTaskColor = "ImportantTaskColor";

        public const string ThumbnailHeight = "ThumbnailHeight";

        public const string MEFKActionLabel = "MEF.KAction.Label";

        public const string TrackabilityList = "TrackabilityList";
        public const string TrackabilityModification = "Trackability.Modification";
        public const string TrackabilityDate = "Trackability.Date";
        public const string TrackabilityVisa = "Trackability.Visa";
        public const string TrackabilityIndice = "Trackability.Indice";

        public const string ScenarioSheetName = "ScenarioSheetName";
        public const string ExcelFinalFileName = "ExcelFinalFileName";
        public const string FileVersion = "FileVersion";
    }
}
