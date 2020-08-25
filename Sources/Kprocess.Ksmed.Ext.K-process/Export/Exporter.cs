using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Ext.Kprocess.Converters;
using KProcess.Ksmed.Ext.Kprocess.Enums;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    public class Exporter : IDisposable
    {
        public const string _VIDEOS_DIRECTORY_NAME = "Videos";
        private const string _THUMBNAILS_DIRECTORY_NAME = "Thumbnails";
        private const string _THUMBNAIL_NAME_PART = "thumbnail";
        private const string _VIDEO_NAME_PART = "video";
        private const string _ACTION_REF_IMAGE_NAME_PART = "actionRef";
        private const string _REF_IMAGE_NAME_PART = "ref";
        private const string _ERROR_SHEET_NAME = "ERRORS";
        private const int _MARKING_PADDING = 10;

        List<string> _excelNames = new List<string>();
        Workbook New_wb;
        private readonly string _modelFilePath;

        private IActionReferential[] _allReferentials;
        private Models.Scenario _scenario;
        private string _outputExcelPath;

        private const int _MARGIN_TOP = 2;
        private const int _MARGIN_LEFT = 2;
        private const int _CAPTION_MARGIN_CORRECTION = 6;
        private ISettingsService _settingService = null;

        public int ErrorCount { get; protected set; } = 0;

#if DEBUG
        private const bool EXPORT_VIDEO = true;
        private const bool EXPORT_EXCEL = true;
#else
        private const bool EXPORT_VIDEO = true;
        private const bool EXPORT_EXCEL = true;
#endif

        private Application _application;
        private List<Referential> _referentials = new List<Referential>();
        private List<ActionCategory> _categories = new List<ActionCategory>();
        private List<Operator> _operators = new List<Operator>();
        private List<Equipment> _equipments = new List<Equipment>();
        private List<Video> _videosApplicables = new List<Video>();
        private Dictionary<int, List<IActionReferential>> ListRefDictionary = new Dictionary<int, List<IActionReferential>>
        {
            [1] = new List<IActionReferential>(),
            [2] = new List<IActionReferential>(),
            [3] = new List<IActionReferential>(),
            [4] = new List<IActionReferential>(),
            [5] = new List<IActionReferential>(),
            [6] = new List<IActionReferential>(),
            [7] = new List<IActionReferential>()
        };
        Dictionary<int, bool> AddCaptionToRef, AddCaptionToRefAction;
        Dictionary<int, int> CaptionSizeRef, CaptionSizeRefAction;
        long _minI = long.MaxValue, _maxI = 0;
        float currentCellLeft, currentCellTop, left, top;
        Dictionary<int, int> MaxNumberRefImagesPerCell;
        Dictionary<int, int> HeightRefImages;
        Dictionary<int, int> MaxNumberActionRefImagePerLine;
        Dictionary<int, int> HeightActionRefImage;
        double excelImageStepMargin = 3.25;
        //ExportAction
        int maxLineCount, currentRowIndex;
        Range currentRow;

        public Exporter(string modelFilePath)
        {
            _modelFilePath = modelFilePath;
        }

        public void Open(Models.Scenario scenario, Referential[] referentials, bool excelExport = true)
        {
            _scenario = scenario;
            _referentials.AddRange(referentials);
            if (excelExport)
            {
                _application = new Application
                {
#if DEBUG
                    ScreenUpdating = true,
                    Visible = true,
#else
                    ScreenUpdating = false,
                    Visible = false,
#endif
                    DisplayAlerts = false,
                    AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable
                };
            }

            _allReferentials = ReferentialsHelper.GetAllReferentialsStandardUsed(scenario.Project)
                .Cast<IActionReferential>()
                .Concat(ReferentialsHelper.GetAllReferentialsProject(scenario.Project))
                .ToArray();
        }


        /// <summary>
        /// Closes the excel file instances
        /// </summary>
        public void FinalizeExcel()
        {
            try
            {
                //    Raw_wb?.Close(SaveChanges: false);
                //    Raw_wb = null;
                New_wb?.Close(SaveChanges: false);
                New_wb = null;
                _application?.Quit();
                _application = null;
            }
            catch { }
        }

        /// <summary>
        /// Cleans up temporary excel files
        /// </summary>
        public void CleanupExcelFiles()
        {
            if (File.Exists(_outputExcelPath))
                try { File.Delete(_outputExcelPath); } catch { }
        }

        private void LogExcelError(string error)
        {
            ErrorCount++;

            //On crée l'onglet Error s'il n'existe pas
            if (!SheetExists(_ERROR_SHEET_NAME))
                CreateSheet(_ERROR_SHEET_NAME);
            _application.Range[$"{_ERROR_SHEET_NAME}!A:B"][1][ErrorCount].Value = ErrorCount;
            _application.Range[$"{_ERROR_SHEET_NAME}!A:B"][2][ErrorCount].Value = error;
            _application.Range[$"{_ERROR_SHEET_NAME}!A:B"][2][ErrorCount].Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
        }

        /// <summary>
        /// Cleans up temporary video files
        /// </summary>
        public void CleanupVideoFiles()
        {
            var directoryInfo = new DirectoryInfo(VideoExtensions.GetInVideoBufferFolder(""));
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                try
                {
                    fileInfo.Delete();
                }
                catch (Exception e)
                {
                    this.TraceError(e, $"An error occured while cleaning file in video buffer : {fileInfo.Name}");
                }
            }
        }

        public void Dispose()
        {
            FinalizeExcel();
            CleanupExcelFiles();
            CleanupVideoFiles();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputPath">The path of the folder to create</param>
        public void Package(string excelOuputDirectory, string excelFileName, string videoOutPutDirectory, bool multiThread)
        {
            var settings = _settingService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);

            this.TraceDebug("=> Packaging");
            this.TraceDebug("outputPath");

            var packageDirectoryPath = excelOuputDirectory;
            var packagedExcelPath = Path.Combine(packageDirectoryPath, $"{excelFileName}.xlsm");
            this.TraceDebug($"package directory output is {packageDirectoryPath}");

            if (!Directory.Exists(packageDirectoryPath) && settings.ExcelExportIsEnabled)
            {
                this.TraceInfo($"package directory {packageDirectoryPath} doesn't exist. Creating it...");
                Directory.CreateDirectory(packageDirectoryPath);
            }

            if (!Directory.Exists(videoOutPutDirectory) && settings.VideoExportIsEnabled)
            {
                this.TraceInfo($"package video directory {videoOutPutDirectory} doesn't exist. Creating it...");
                Directory.CreateDirectory(videoOutPutDirectory);
            }

            if (settings.ExcelExportIsEnabled)
            {
                this.TraceInfo($"Copying temporary excel file {_outputExcelPath} at {packagedExcelPath}");
                if (File.Exists(packagedExcelPath))
                    File.Delete(packagedExcelPath);
                File.Copy(_outputExcelPath, packagedExcelPath);
            }

            // Si on exporte en Archivage ET Redistribuable, on réouvre le fichier Excel, on modifie les chemins pour Redistribuable, et on l'enregistre dans le bon répertoire
            if (settings.ExcelExportIsEnabled && settings.ArchivageIsEnabled && settings.RedistribuableIsEnabled)
            {
                string ExcelFolderName = GetRangeValue(ExportRanges.ExcelFolderName).FormatForWindows();
                string FileVersion = GetRangeValue(ExportRanges.FileVersion);
                string ExcelFinalFileName = GetRangeValue(ExportRanges.ExcelFinalFileName).FormatForWindows();

                ExportToRange(ExportRanges.VideoDirectory, Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{ExcelFolderName} - v{FileVersion}"));
                ExportToRange(ExportRanges.RepVideoRelatif, "1");

                if (settings.VideoExportIsEnabled && RangeExists(ExportRanges.KActionVideoWithExtension))
                {
                    int rowsCount = _application.Range[ExportRanges.KActionVideoWithExtension].Rows.Count;
                    for (int i = 1; i <= rowsCount; i++)
                    {
                        if (!string.IsNullOrEmpty(GetRangeValue(ExportRanges.KActionVideoWithExtension, i)))
                        {
                            string rangeAddress = GetRangeAddress(ExportRanges.KActionVideoWithExtension, i);
                            string formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"{_VIDEOS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                            ExportFormulaToRange(ExportRanges.KActionVideoPath, formula, i);
                        }
                    }
                }

                if (RangeExists(ExportRanges.KActionThumbnailWithExtension))
                {
                    int rowsCount = _application.Range[ExportRanges.KActionThumbnailWithExtension].Rows.Count;
                    for (int i = 1; i <= rowsCount; i++)
                    {
                        if (!string.IsNullOrEmpty(GetRangeValue(ExportRanges.KActionThumbnailWithExtension, i)))
                        {
                            string rangeAddress = GetRangeAddress(ExportRanges.KActionThumbnailWithExtension, i);
                            string formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"{_THUMBNAILS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                            ExportFormulaToRange(ExportRanges.KActionThumbnailPath, formula, i);
                        }
                    }
                }

                string redistribuableFilePath = Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{ExcelFolderName} - v{FileVersion}", $"{ExcelFinalFileName}.xlsm");
                New_wb.SaveAs(
                    Filename: redistribuableFilePath,
                    FileFormat: XlFileFormat.xlOpenXMLWorkbookMacroEnabled,
                    ReadOnlyRecommended: settings.IsReadOnlyEnabled_Redistribuable,
                    CreateBackup: false);
            }

            this.TraceInfo($"Cleaning up temporary excel files");
            FinalizeExcel();
            CleanupExcelFiles();

            if (EXPORT_VIDEO && settings.VideoExportIsEnabled)
            {
                this.TraceInfo("Génération des videos à packager");

                List<KAction> actionsToExport = new List<KAction>();
                if (settings.ExportOnlyKeyTasksVideos)
                    actionsToExport = _scenario.Actions.Where(a => a.Video != null && a.BuildDuration != 0 && IsActionImportant(a, settings)).ToList();
                else
                    actionsToExport = _scenario.Actions.Where(a => a.Video != null && a.BuildDuration != 0).ToList();

                //var nbVideos = 0; // pas besoin de lock, ce n'est pas grave si elle est incrémentée plusieurs fois avant de pouvoir être affichée
                var nbActions = actionsToExport.Count;

                var degree = Math.Min(Math.Max(Environment.ProcessorCount - 1, 1), 3);

                KprocessExportWindow.nbVideos = 0;
                var videoPathsParallel = actionsToExport.AsParallel();
                if (!multiThread)
                    videoPathsParallel = videoPathsParallel.WithDegreeOfParallelism(1);
                var videoPaths = videoPathsParallel.Select(action =>
                {
                    string markings = string.Empty;

                    markings = settings.OverlayTextVideo;
                    if (settings.ExcelFileNameMarkingIsEnabled)
                        markings += $" - {excelFileName}";
                    if (settings.WBSMarkingIsEnabled)
                        markings += $" - {action.WBS}";

                    KprocessExportWindow.IncrLogVideoProgress(nbActions);

                    string videoPath = action.Video
                        .CutForAction(action)
                        .Save(Path.Combine(videoOutPutDirectory, $"{_VIDEO_NAME_PART} - {action.WBS}"), settings.VideoMarkingIsEnabled ? markings : null, settings.HorizontalAlignement, settings.VerticalAlignement, settings.SlowMotionIsEnabled ? settings.DurationMini : (double?)null);

                    // Si on exporte en Archivage ET Redistribuable, on copie la vidéo aussi dans le répertoire Redistribuable
                    if (settings.ArchivageIsEnabled && settings.RedistribuableIsEnabled)
                    {
                        string[] folders = videoOutPutDirectory.Split(Path.DirectorySeparatorChar);
                        string fileVersion = folders[folders.Length - 2];
                        string excelFolderName = folders[folders.Length - 3];
                        if (!Directory.Exists(Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {fileVersion}", _VIDEOS_DIRECTORY_NAME)))
                            Directory.CreateDirectory(Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {fileVersion}", _VIDEOS_DIRECTORY_NAME));
                        if (File.Exists(Path.Combine(videoOutPutDirectory, $"{_VIDEO_NAME_PART} - {action.WBS}.mpeg")))
                            File.Copy(
                                Path.Combine(videoOutPutDirectory, $"{_VIDEO_NAME_PART} - {action.WBS}.mpeg"),
                                Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {fileVersion}", _VIDEOS_DIRECTORY_NAME, $"{_VIDEO_NAME_PART} - {action.WBS}.mpeg"));
                    }

                    return videoPath;
                })
                .ToList();                
            }
        }

        private void CheckCancellation()
        {
            if (KprocessExportWindow.CancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();
        }

        private System.Windows.Size GetImageSize(byte[] image)
        {
            using (var ms = new MemoryStream(image))
            {
                var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                return new System.Windows.Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
            }
        }

        private void SaveImageToDisk(string directoryToSave, string fileName, byte[] image)
        {
            if (!Directory.Exists(directoryToSave))
                Directory.CreateDirectory(directoryToSave);

            var pathToSave = Path.Combine(directoryToSave, fileName);

            using (var ms = new MemoryStream(image))
            {
                var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(ms, System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                var size = new System.Windows.Size(decoder.Frames[0].PixelWidth, decoder.Frames[0].PixelHeight);
                var mimeTypes = decoder.CodecInfo.MimeTypes;

                /*ImagePartType imageType;
                if (mimeTypes.Contains("image/jpeg"))
                    imageType = ImagePartType.Jpeg;
                else if (mimeTypes.Contains("image/bmp"))
                    imageType = ImagePartType.Bmp;
                else if (mimeTypes.Contains("image/png"))
                    imageType = ImagePartType.Png;
                else
                {
                    this.TraceWarning($"Imposible de sauvegarder l'image: {pathToSave}");
                    return;
                }*/
                if (!mimeTypes.Contains("image/jpeg") && !mimeTypes.Contains("image/bmp") && !mimeTypes.Contains("image/png"))
                {
                    this.TraceWarning($"Imposible de sauvegarder l'image: {pathToSave}");
                    return;
                }

                var img = System.Drawing.Image.FromStream(ms);
                try
                {
                    img.Save(pathToSave);
                }
                catch (Exception e)
                {
                    this.TraceError(e, $"Erreur lors de la sauvegarder l'image: {pathToSave}");
                }
            }
        }

        private void SetBackgroundColorToRange(string range, string colorCodeHexa, int rowIndex = 1, int columnIndex = 1)
        {
            if (RangeExists(range) && colorCodeHexa != null)
            {
                var mediaColor = (Color)ColorConverter.ConvertFromString(colorCodeHexa);

                _application.Range[range][rowIndex][columnIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B));
                //_application.Range[range][rowIndex][columnIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);
            }
        }

        private void SetTextSizeToRange(string range, int size, int rowIndex = 1, int columnIndex = 1)
        {
            //La taille du texte doit être entre 1 et 409 points !!
            if (RangeExists(range))
            {
                if (size > 0 && size < 409)
                    _application.Range[range][rowIndex][columnIndex].Font.Size = size;
                else
                    LogExcelError("The task title font size must be between 1 and 409.");
            }
        }

        private void SetCommentToRange(string range, string comment, int rowIndex = 1, int columnIndex = 1)
        {
            if (RangeExists(range))
            {
                _application.Range[range][rowIndex][columnIndex].AddComment(comment);
                _application.Range[range][rowIndex][columnIndex].Comment.Shape.TextFrame.AutoSize = true;
                ((Range)_application.Range[range][rowIndex][columnIndex]).Comment.Shape.TextFrame.Characters().Font.Size = 14;
            }
        }

        private string GetRangeNameFromProperty<T>(Expression<Func<T>> property, object value)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            Type type = value.GetType();
            if (propertyInfo == null)
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            else
                return $"{type.Name.ToString()}.{propertyInfo.Name}";
        }

        private bool SheetExists(string name)
        {
            foreach (Worksheet sheet in _application.Sheets)
            {
                // Check the name of the current sheet
                if (sheet.Name == name)
                    return true;
            }

            return false;
        }

        private string CheckSheetName(string sheetName)
        {
            //Attention aux caractères interdits dans le sheet name    => ' : / \ * ? [ ]
            sheetName = sheetName
                .Replace(':', '¤')
                .Replace('/', '¤')
                .Replace('*', '¤')
                .Replace('?', '¤')
                .Replace('[', '¤')
                .Replace(']', '¤')
                .Replace('\'', '¤')
                .Replace('\\', '¤');

            //31 Caractères max! On tronque !
            if (sheetName.Length > 31)
                sheetName = sheetName.Substring(0, 31);

            return sheetName;
        }
        private void CreateSheet(string sheetName)
        {
            sheetName = CheckSheetName(sheetName);
            var newSheet = _application.Worksheets.Add(After: _application.Sheets[_application.Sheets.Count]);
            newSheet.Name = sheetName;
        }

        private void SetSheetName(string oldSheetName, string newSheetName)
        {
            newSheetName = CheckSheetName(newSheetName);

            _application.Worksheets[oldSheetName].Name = newSheetName;
        }

        private Shape ExportImageToSheet(string sheetName, string uri, float left, float top, float width, float height, string shapeName, int rowIndex = 1, int columnIndex = 1, string caption = null, int fontSize = 11)
        {
            if (File.Exists(uri))
            {
                Shape shape = _application.Sheets[sheetName].Shapes.AddPicture(uri,
                    Microsoft.Office.Core.MsoTriState.msoFalse,
                    Microsoft.Office.Core.MsoTriState.msoTrue,
                    left,
                    top,
                    width,
                    height);

                shape.Name = shapeName;
                shape.Placement = XlPlacement.xlMoveAndSize;

                if (!string.IsNullOrEmpty(caption))
                {
                    dynamic textbox = _application.Sheets[sheetName].Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal,
                        left,
                        top + height,
                        0,
                        0);
                    textbox.TextFrame.AutoSize = true;
                    textbox.TextFrame.MarginLeft = 0;
                    textbox.TextFrame.MarginRight = 0;
                    textbox.TextFrame.MarginTop = 0;
                    textbox.TextFrame.MarginBottom = 0;
                    textbox.TextFrame.Characters.Font.Size = fontSize;
                    textbox.TextFrame.Characters.Text = caption;
                    textbox.Name = $"{shape.Name}.Caption";
                    // On centre la légende
                    if (textbox.Width > shape.Width)
                        shape.IncrementLeft((textbox.Width - shape.Width) / 2);
                    else
                        textbox.Left = shape.Left + (shape.Width - textbox.Width) / 2;

                    // On groupe les deux formes
                    var finalShape = _application.Sheets[sheetName].Shapes.Range(new[] { shape.Name, textbox.Name }).Group();
                    // On corrige le placement de la forme groupée
                    finalShape.IncrementLeft(_CAPTION_MARGIN_CORRECTION);
                    finalShape.IncrementTop(_CAPTION_MARGIN_CORRECTION);
                    finalShape.Placement = XlPlacement.xlMoveAndSize;
                    return finalShape;
                }

                return shape;
            }
            else
            {
                this.TraceWarning($"L'image n'a pas été trouvée: {uri}");
                LogExcelError($"Image not found: {uri}");
                return null;
            }
        }

        private void ExportProperty<T>(Expression<Func<T>> property, object value, int rowIndex = 1, int columnIndex = 1)
        {
            var propertyInfo = (property.Body as MemberExpression).Member as PropertyInfo;
            Type type = value.GetType();
            if (propertyInfo == null)
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");

            if (propertyInfo.GetValue(value, null) != null)
                ExportToRange($"{type.Name.ToString()}.{propertyInfo.Name}", propertyInfo.GetValue(value, null), rowIndex, columnIndex);
        }

        private void AddNewLineInRange(string range, int rowIndex = 1)
        {
            if (RangeExists(range))
                ((Range)_application.Range[range].Rows[rowIndex]).Insert();
        }

        private void SetHyperlink(string range, int rowIndex = 1, string address = "")
        {
            if (RangeExists(range))
            {
                double previousFontSize = _application.Range[range][rowIndex].Font.Size;
                _application.Range[range][rowIndex].Hyperlinks.Add(Anchor: _application.Range[range][rowIndex], Address: address);
                _application.Range[range][rowIndex].Font.Size = previousFontSize;
            }
        }

        private void ExportProject()
        {
            ExportProperty(() => _scenario.Project.Label, _scenario.Project);
            ExportProperty(() => _scenario.Project.Workshop, _scenario.Project);
            ExportProperty(() => _scenario.Project.Description, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomTextLabel, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomTextLabel2, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomTextLabel3, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomTextLabel4, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomNumericLabel, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomNumericLabel2, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomNumericLabel3, _scenario.Project);
            ExportProperty(() => _scenario.Project.CustomNumericLabel4, _scenario.Project);
            ExportProperty(() => _scenario.Project.OtherObjectiveLabel, _scenario.Project);
            _scenario.Project.TimeScale = _scenario.Project.TimeScale / 10000;
            ExportProperty(() => _scenario.Project.TimeScale, _scenario.Project);
        }

        private void ExportScenario()
        {
            ExportProperty(() => _scenario.Label, _scenario);
            ExportProperty(() => _scenario.Description, _scenario);
            ExportProperty(() => _scenario.IsShownInSummary, _scenario);
        }

        private void ExportReferentials()
        {
            var settings = _settingService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);
            AddCaptionToRef = new Dictionary<int, bool>
            {
                [1] = settings.AddCaptionToRef1,
                [2] = settings.AddCaptionToRef2,
                [3] = settings.AddCaptionToRef3,
                [4] = settings.AddCaptionToRef4,
                [5] = settings.AddCaptionToRef5,
                [6] = settings.AddCaptionToRef6,
                [7] = settings.AddCaptionToRef7
            };
            CaptionSizeRef = new Dictionary<int, int>
            {
                [1] = settings.CaptionSizeRef1,
                [2] = settings.CaptionSizeRef2,
                [3] = settings.CaptionSizeRef3,
                [4] = settings.CaptionSizeRef4,
                [5] = settings.CaptionSizeRef5,
                [6] = settings.CaptionSizeRef6,
                [7] = settings.CaptionSizeRef7
            };
            HeightRefImages = new Dictionary<int, int>
            {
                [1] = settings.ImageHeightRef1,
                [2] = settings.ImageHeightRef2,
                [3] = settings.ImageHeightRef3,
                [4] = settings.ImageHeightRef4,
                [5] = settings.ImageHeightRef5,
                [6] = settings.ImageHeightRef6,
                [7] = settings.ImageHeightRef7
            };
            MaxNumberRefImagesPerCell = new Dictionary<int, int>
            {
                [1] = settings.NbImagesPerLineRef1,
                [2] = settings.NbImagesPerLineRef2,
                [3] = settings.NbImagesPerLineRef3,
                [4] = settings.NbImagesPerLineRef4,
                [5] = settings.NbImagesPerLineRef5,
                [6] = settings.NbImagesPerLineRef6,
                [7] = settings.NbImagesPerLineRef7
            };

            for (int i = 1; i <= 7; i++)
            {
                ExportToRange(ExportRanges.RefDictionary[i]["ImageHeight"], HeightRefImages[i].ToString());
                ExportToRange(ExportRanges.RefDictionary[i]["NbImagesPerLine"], MaxNumberRefImagesPerCell[i].ToString());
            }

            currentCellLeft = 1;
            currentCellTop = 1;

            for (int i = 1; i <= 7; i++)
                ExportReferential(i);
        }

        private void ExportReferential(int refNumber)
        {
            StringBuilder refToExport = new StringBuilder();
            int imageCount = 0;
            Shape lastImageInserted = null;
            int lineCount = 1;

            var refx = _scenario.Project.Referentials.FirstOrDefault(r => r.ReferentialId == refNumber + 3);

            if (refx != null && refx.IsEnabled)
            {
                var val = _referentials.FirstOrDefault(r => r.ReferentialId == refx.ReferentialId);
                ExportToRange(ExportRanges.RefDictionary[refNumber]["Label"], val.Label);

                int currentRowIndex = 1;
                foreach (var referential in ListRefDictionary[refNumber])
                {
                    #region Export tableau referentiels paramètres

                    if (currentRowIndex > 1)
                        AddNewLineInRange(ExportRanges.RefDictionary[refNumber]["List"], currentRowIndex + 1);

                    ExportToRange($"{ExportRanges.RefDictionary[refNumber]["Ref"]}.{GetRangeNameFromProperty(() => referential.Label, referential).Split('.')[1]}", referential.Label, currentRowIndex);
                    ExportToRange($"{ExportRanges.RefDictionary[refNumber]["Ref"]}.{GetRangeNameFromProperty(() => referential.Color, referential).Split('.')[1]}", referential.Color, currentRowIndex);
                    ExportToRange($"{ExportRanges.RefDictionary[refNumber]["Ref"]}.{GetRangeNameFromProperty(() => referential.Description, referential).Split('.')[1]}", referential.Description, currentRowIndex);
                    ExportToRange($"{ExportRanges.RefDictionary[refNumber]["Ref"]}.{GetRangeNameFromProperty(() => referential.Uri, referential).Split('.')[1]}", referential.Uri, currentRowIndex);


                    SetBackgroundColorToRange($"{ExportRanges.RefDictionary[refNumber]["Ref"]}.{GetRangeNameFromProperty(() => referential.Color, referential).Split('.')[1]}",
                        referential.Color,
                        currentRowIndex);

                    #endregion

                    #region Export des images des référentiels scénarios MEF

                    if (refToExport.Length > 0)
                        refToExport.AppendLine();

                    refToExport.Append(referential.Label);

                    //Export de l'image de la ref si elle existe
                    if (RangeExists(ExportRanges.RefDictionary[refNumber]["ScenarioImages"]) && !string.IsNullOrEmpty(referential.Uri))
                    {
                        currentCellLeft = (float)_application.Range[ExportRanges.RefDictionary[refNumber]["ScenarioImages"]].Left + _MARGIN_LEFT;
                        currentCellTop = (float)_application.Range[ExportRanges.RefDictionary[refNumber]["ScenarioImages"]].Top + _MARGIN_TOP;

                        var modulo = imageCount % MaxNumberRefImagesPerCell[refNumber];

                        //Première image
                        if (lastImageInserted == null)
                        {
                            left = currentCellLeft + _MARGIN_LEFT;
                            top = currentCellTop + _MARGIN_TOP;
                        }
                        //On a pas atteint le nb max d'image sur la ligne
                        else if (modulo != 0)
                        {
                            left = lastImageInserted.Left + lastImageInserted.Width + _MARGIN_LEFT;
                            top = AddCaptionToRef[refNumber] ? lastImageInserted.Top - _CAPTION_MARGIN_CORRECTION : lastImageInserted.Top;
                        }
                        else
                        {
                            left = currentCellLeft + _MARGIN_LEFT;
                            top = lastImageInserted.Top + lastImageInserted.Height + _MARGIN_TOP;
                            lineCount++;
                        }
                        Range currentRow = _application.Range[ExportRanges.RefDictionary[refNumber]["ScenarioImages"]];
                        double imageHeight = 50;
                        double.TryParse(GetRangeValue(ExportRanges.RefDictionary[refNumber]["ImageHeight"]), out imageHeight);
                        if (lineCount > 0)
                        {
                            var neededHeight = lineCount * (imageHeight + _MARGIN_TOP) + 3 * _MARGIN_TOP;
                            if (AddCaptionToRef[refNumber])
                                neededHeight += lineCount * (GetTextBoxHeight(CaptionSizeRef[refNumber]) + _CAPTION_MARGIN_CORRECTION) + _CAPTION_MARGIN_CORRECTION;
                            this.TraceInfo($"Vérification hauteur de ligne Ref{refNumber} scenario: currentHeight: {currentRow.RowHeight} // neededHeight: {neededHeight}");
                            if (currentRow.RowHeight < neededHeight)
                            {
                                var oldValue = currentRow.RowHeight;
                                currentRow.RowHeight = neededHeight;
                                this.TraceInfo($"Mise à jour hauteur de ligne {oldValue} => {currentRow.RowHeight}");
                            }
                        }

                        lastImageInserted = ExportImageToSheet(ExportRanges.ScenarioSheetOldName,
                            referential.Uri,
                            left: left,
                            top: top,
                            width: ConvertDoubleToFloat(imageHeight),
                            height: ConvertDoubleToFloat(imageHeight),
                            shapeName: $"{_REF_IMAGE_NAME_PART}{refNumber}.{imageCount}.{Guid.NewGuid()}",
                            rowIndex: currentRowIndex,
                            caption: AddCaptionToRef[refNumber] ? referential.Label : null,
                            fontSize: CaptionSizeRef[refNumber]);

                        imageCount++;
                    }

                    #endregion

                    #region Export des liens vers URi référentiels

                    if (currentRowIndex > 1)
                        AddNewLineInRange(ExportRanges.RefDictionary[refNumber]["ScenarioLabelsLinksList"], currentRowIndex);

                    ExportToRange(ExportRanges.RefDictionary[refNumber]["ScenarioLabelsLinks"], referential.Label, currentRowIndex);
                    SetHyperlink(ExportRanges.RefDictionary[refNumber]["ScenarioLabelsLinks"], currentRowIndex, referential.Uri);

                    #endregion

                    currentRowIndex++;
                }

                if (refToExport.Length > 0)
                    ExportToRange(ExportRanges.RefDictionary[refNumber]["ScenarioLabelsText"], refToExport.ToString());
            }
        }

        private void ExportCategories()
        {
            int currentRowIndex = 1;
            foreach (var category in _categories)
            {
                if (currentRowIndex > 1)
                    AddNewLineInRange(ExportRanges.CategoryList, currentRowIndex + 1);

                ExportToRange($"{ExportRanges.Category}.{GetRangeNameFromProperty(() => category.Label, category).Split('.')[1]}", category.Label, currentRowIndex);
                ExportToRange($"{ExportRanges.Category}.{GetRangeNameFromProperty(() => category.Color, category).Split('.')[1]}", category.Color, currentRowIndex);
                ExportToRange($"{ExportRanges.Category}.{GetRangeNameFromProperty(() => category.Description, category).Split('.')[1]}", category.Description, currentRowIndex);
                ExportToRange($"{ExportRanges.Category}.{GetRangeNameFromProperty(() => category.Uri, category).Split('.')[1]}", category.Uri, currentRowIndex);

                SetBackgroundColorToRange($"{ExportRanges.Category}.{GetRangeNameFromProperty(() => category.Color, category).Split('.')[1]}",
                    category.Color,
                    currentRowIndex);

                currentRowIndex++;
            }
        }

        private void ExportOperators()
        {
            int currentRowIndex = 1;
            foreach (var @operator in _operators)
            {
                if (currentRowIndex > 1)
                    AddNewLineInRange(ExportRanges.OperatorList, currentRowIndex + 1);

                ExportToRange($"{ExportRanges.Operator}.{GetRangeNameFromProperty(() => @operator.Label, @operator).Split('.')[1]}", @operator.Label, currentRowIndex);
                ExportToRange($"{ExportRanges.Operator}.{GetRangeNameFromProperty(() => @operator.Color, @operator).Split('.')[1]}", @operator.Color, currentRowIndex);
                ExportToRange($"{ExportRanges.Operator}.{GetRangeNameFromProperty(() => @operator.Description, @operator).Split('.')[1]}", @operator.Description, currentRowIndex);
                ExportToRange($"{ExportRanges.Operator}.{GetRangeNameFromProperty(() => @operator.Uri, @operator).Split('.')[1]}", @operator.Uri, currentRowIndex);

                if (@operator.Color != null)
                {
                    SetBackgroundColorToRange($"{ExportRanges.Operator}.{GetRangeNameFromProperty(() => @operator.Color, @operator).Split('.')[1]}",
                        @operator.Color,
                        currentRowIndex);
                }

                currentRowIndex++;
            }
        }

        private void ExportEquipments()
        {
            int currentRowIndex = 1;
            foreach (var @equipment in _equipments)
            {
                if (currentRowIndex > 1)
                    AddNewLineInRange(ExportRanges.EquipmentList, currentRowIndex + 1);

                ExportToRange($"{ExportRanges.Equipment}.{GetRangeNameFromProperty(() => @equipment.Label, @equipment).Split('.')[1]}", @equipment.Label, currentRowIndex);
                ExportToRange($"{ExportRanges.Equipment}.{GetRangeNameFromProperty(() => @equipment.Color, @equipment).Split('.')[1]}", @equipment.Color, currentRowIndex);
                ExportToRange($"{ExportRanges.Equipment}.{GetRangeNameFromProperty(() => @equipment.Description, @equipment).Split('.')[1]}", @equipment.Description, currentRowIndex);
                ExportToRange($"{ExportRanges.Equipment}.{GetRangeNameFromProperty(() => @equipment.Uri, @equipment).Split('.')[1]}", @equipment.Uri, currentRowIndex);

                if (@equipment.Color != null)
                {
                    SetBackgroundColorToRange($"{ExportRanges.Equipment}.{GetRangeNameFromProperty(() => @equipment.Color, @equipment).Split('.')[1]}",
                        @equipment.Color,
                        currentRowIndex);
                }

                currentRowIndex++;
            }
        }

        private void ExportVideosBase(Trackability previousTrackability)
        {
            Video videoModel = new Video();
            List<TrackabilityVideoItem> videoList = new List<TrackabilityVideoItem>();
            if (previousTrackability != null && previousTrackability.VideoList.Count > 0)
                videoList.AddRange(previousTrackability.VideoList);

            foreach (var video in _scenario.Project.Videos)
            {
                if (!videoList.Exists(v => v.Name == video.Name))
                    videoList.Add(new TrackabilityVideoItem
                    {
                        Name = video.Name,
                        Description = video.Description,
                        FileName = video.Filename,
                        FilePath = video.FilePath
                    });
            }

            int currentRowIndex = 1;
            foreach (var video in videoList)
            {
                if (currentRowIndex > 1)
                    AddNewLineInRange(ExportRanges.VideoList, currentRowIndex);

                ExportToRange(GetRangeNameFromProperty(() => videoModel.Name, videoModel), video.Name, currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => videoModel.Description, videoModel), video.Description, currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => videoModel.Filename, videoModel), video.FileName, currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => videoModel.FilePath, videoModel), video.FilePath, currentRowIndex);

                var applicable = "Non";
                if (_videosApplicables.Exists(v => v.Name == video.Name))
                    applicable = "Oui";
                ExportToRange(ExportRanges.VideoApplicable, applicable, currentRowIndex);

                currentRowIndex++;
            }
        }

        private void ExportTrackability(Trackability previousTrackability)
        {
            bool firstDoc = false;
            if (previousTrackability == null)
            {
                firstDoc = true;
                previousTrackability = new Trackability();
                previousTrackability.ModificationList.Add(new TrackabilityItem
                {
                    Modification = "Creation",
                    Visa = GetOwner().Username,
                    Date = DateTime.Now.ToString(),
                    Indice = 0
                });
            }

            if (previousTrackability != null)
            {
                if (!firstDoc)
                    previousTrackability.ModificationList.Add(new TrackabilityItem
                    {
                        Modification = _scenario.Description,
                        Visa = GetOwner().Username,
                        Date = DateTime.Now.ToString()
                    });

                int currentRowIndex = 1;
                foreach (var trackItem in previousTrackability.ModificationList)
                {
                    if (currentRowIndex > 1)
                        AddNewLineInRange(ExportRanges.TrackabilityList, currentRowIndex);

                    ExportToRange(ExportRanges.TrackabilityModification, trackItem.Modification, currentRowIndex);
                    DateTime date = new DateTime();
                    if (DateTime.TryParse(trackItem.Date, out date))
                        ExportToRange(ExportRanges.TrackabilityDate, date.ToString("MM/dd/yyyy"), currentRowIndex);
                    ExportToRange(ExportRanges.TrackabilityVisa, trackItem.Visa, currentRowIndex);
                    ExportToRange(ExportRanges.TrackabilityIndice, (currentRowIndex - 1).ToString(), currentRowIndex);

                    currentRowIndex++;
                }
            }
        }

        private float ConvertDoubleToFloat(double value)
        {
            float result = (float)value;
            if (float.IsPositiveInfinity(result))
                result = float.MaxValue;
            else if (float.IsNegativeInfinity(result))
                result = float.MinValue;

            return result;
        }

        private int GetPixelsFromCharacterNumber(int characterNumber)
        {
            System.Drawing.Font font = new System.Drawing.Font("Calibri", 11.0f, System.Drawing.FontStyle.Regular);
            string chars = new string('_', characterNumber);
            return System.Windows.Forms.TextRenderer.MeasureText(chars, font).Width;
        }

        private static String ConvertColorToHex(Color c) =>
            $"#{c.R.ToString("X2")}{c.G.ToString("X2")}{c.B.ToString("X2")}";

        private bool IsActionImportant(KAction action, Settings settings)
        {
            if (!settings.KeyTaskFreeTextField1 && !settings.KeyTaskFreeTextField2 && !settings.KeyTaskFreeTextField3 && !settings.KeyTaskFreeTextField4)
                return false;

            var converter = new ToggleAndOrButtonConverter();
            bool[] fields =
            {
                action.CustomTextValue.IsNotNullNorEmpty(),
                action.CustomTextValue2.IsNotNullNorEmpty(),
                action.CustomTextValue3.IsNotNullNorEmpty(),
                action.CustomTextValue4.IsNotNullNorEmpty()
            };
            bool[] operators = //false = AND, true = OR
            {
                (bool)converter.Convert(
                    new object[]{ settings.KeyTaskFreeTextField1, settings.KeyTaskFreeTextField2, settings.KeyTaskFreeTextField3, settings.KeyTaskFreeTextField4 },
                    typeof(bool), null, null) ? settings.KeyTaskOperator1 : true,
                (bool)converter.Convert(
                    new object[]{ settings.KeyTaskFreeTextField2, settings.KeyTaskFreeTextField3, settings.KeyTaskFreeTextField4 },
                    typeof(bool), null, null) ? settings.KeyTaskOperator2 : true,
                (bool)converter.Convert(
                    new object[]{ settings.KeyTaskFreeTextField3, settings.KeyTaskFreeTextField4 },
                    typeof(bool), null, null) ? settings.KeyTaskOperator3 : true
            };

            bool result = fields[0];
            if (operators[0])
                result = result || fields[1];
            else
                result = result && fields[1];
            if (operators[1])
                result = result || fields[2];
            else
                result = result && fields[2];
            if (operators[2])
                result = result || fields[3];
            else
                result = result && fields[3];

            return result;
        }

        private void ExportActions(string directoryToThumbnails, bool repRelatif)
        {
            var ExcelFileName = GetRangeValue(ExportRanges.ExcelFinalFileName);
            currentRowIndex = 1;
            var settings = _settingService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);
            string importantTaskColor = ConvertColorToHex(settings.ImportantTaskColor);

            AddCaptionToRefAction = new Dictionary<int, bool>
            {
                [1] = settings.AddCaptionToLineRef1Action,
                [2] = settings.AddCaptionToLineRef2Action,
                [3] = settings.AddCaptionToLineRef3Action,
                [4] = settings.AddCaptionToLineRef4Action,
                [5] = settings.AddCaptionToLineRef5Action,
                [6] = settings.AddCaptionToLineRef6Action,
                [7] = settings.AddCaptionToLineRef7Action
            };
            CaptionSizeRefAction = new Dictionary<int, int>
            {
                [1] = settings.CaptionSizeLineRef1,
                [2] = settings.CaptionSizeLineRef2,
                [3] = settings.CaptionSizeLineRef3,
                [4] = settings.CaptionSizeLineRef4,
                [5] = settings.CaptionSizeLineRef5,
                [6] = settings.CaptionSizeLineRef6,
                [7] = settings.CaptionSizeLineRef7
            };
            HeightActionRefImage = new Dictionary<int, int>
            {
                [1] = settings.ImageHeightLineRef1,
                [2] = settings.ImageHeightLineRef2,
                [3] = settings.ImageHeightLineRef3,
                [4] = settings.ImageHeightLineRef4,
                [5] = settings.ImageHeightLineRef5,
                [6] = settings.ImageHeightLineRef6,
                [7] = settings.ImageHeightLineRef7
            };
            MaxNumberActionRefImagePerLine = new Dictionary<int, int>
            {
                [1] = settings.NbImagesPerLineLineRef1,
                [2] = settings.NbImagesPerLineLineRef2,
                [3] = settings.NbImagesPerLineLineRef3,
                [4] = settings.NbImagesPerLineLineRef4,
                [5] = settings.NbImagesPerLineLineRef5,
                [6] = settings.NbImagesPerLineLineRef6,
                [7] = settings.NbImagesPerLineLineRef7
            };

            ExportToRange(ExportRanges.ThumbnailHeight, settings.ThumbnailHeight.ToString());
            for (int i = 1; i <= 7; i++)
            {
                ExportToRange(ExportRanges.RefDictionary[i]["ImageHeightLine"], HeightActionRefImage[i].ToString());
                ExportToRange(ExportRanges.RefDictionary[i]["NbImagesPerLineLine"], MaxNumberActionRefImagePerLine[i].ToString());
            }

            string GanttGroupColor = ConvertColorToHex(settings.GanttGroupColor); ;

            var criticalPathDurationIE = _scenario.Actions.Any() ? _scenario.Actions.Max(a => a.BuildFinish) - _scenario.Actions.Min(a => a.BuildStart) : 0;

            int nbColumnsInGanttRange = 0;
            long GanttStep = 0;
            if (RangeExists(ExportRanges.KActionGantt))
            {
                nbColumnsInGanttRange = _application.Range[ExportRanges.KActionGantt].Columns.Count;
                GanttStep = criticalPathDurationIE / nbColumnsInGanttRange;
            }

            var allActions = _scenario.Actions.Where(a => a.Duration > 0).OrderByWBS();

            foreach (var action in allActions)
            {
                if (action.BuildDuration == 0)
                    continue;

                if (allActions.IndexOf(action) < allActions.Count() - 1)
                {
                    AddNewLineInRange(ExportRanges.ActionList, currentRowIndex + 1);
                    //copie du contenu de la ligne
                    ((Range)_application.Range[ExportRanges.ActionList].Rows[currentRowIndex]).Copy(_application.Range[ExportRanges.ActionList].Rows[currentRowIndex + 1]);
                }

                #region Gestion du gantt

                if (GanttStep > 0)
                {
                    for (int currentColumnIndex = 1; currentColumnIndex <= nbColumnsInGanttRange; currentColumnIndex++)
                    {
                        long currentStep = currentColumnIndex * GanttStep;

                        if (action.IsGroup)
                        {
                            if (currentStep >= action.BuildStart && currentStep < action.BuildFinish)
                                SetBackgroundColorToRange(ExportRanges.KActionGantt, GanttGroupColor, currentColumnIndex, currentRowIndex);
                            else if (currentStep > action.BuildFinish)
                                break;
                        }
                        else
                        {
                            if (currentStep >= action.BuildStart && currentStep < action.BuildFinish)
                            {
                                string colorToApply = ConvertColorToHex(settings.GanttNoCategoryColor);
                                if (action.Category != null)
                                    colorToApply = action.Category.Color;
                                SetBackgroundColorToRange(ExportRanges.KActionGantt, colorToApply, currentColumnIndex, currentRowIndex);
                            }
                            else if (currentStep > action.BuildFinish)
                                break;
                        }
                    }
                }

                #endregion

                //Si c'est un groupe, pas de vidéo associée
                if (settings.VideoExportIsEnabled && !action.IsGroup && action.Video != null)
                {
                    if (!settings.ExportOnlyKeyTasksVideos ||
                        (settings.ExportOnlyKeyTasksVideos && IsActionImportant(action, settings)))
                    {
                        string splittedVideoName = $"{_VIDEO_NAME_PART} - {action.WBS}.mpeg";

                        ExportToRange(ExportRanges.KActionVideoWithExtension, splittedVideoName, currentRowIndex);

                        string rangeAddress = GetRangeAddress(ExportRanges.KActionVideoWithExtension, currentRowIndex);

                        string ExcelFolderName = GetRangeValue(ExportRanges.ExcelFolderName).FormatForWindows();
                        string ExcelFinalFileName = GetRangeValue(ExportRanges.ExcelFinalFileName).FormatForWindows();
                        string FileVersion = GetRangeValue(ExportRanges.FileVersion);

                        if (!ExcelFolderName.IsNotNullNorEmpty())
                            ExcelFolderName = ExcelFinalFileName;

                        string formula;
                        if (repRelatif)
                        {
                            if (settings.ArchivageIsEnabled)
                                formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"v{FileVersion}\" & CHAR(92) & \"{_VIDEOS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                            else
                                formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"{_VIDEOS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                        }
                        else
                            formula = $"= { ExportRanges.VideoDirectory} & CHAR(92) & { ExportRanges.ExcelFolderName} & CHAR(92) & \"v{FileVersion}\" & CHAR(92) & \"{_VIDEOS_DIRECTORY_NAME}\" & CHAR(92) & { rangeAddress}";
                        ExportFormulaToRange(ExportRanges.KActionVideoPath, formula, currentRowIndex);

                        ExportToRange(ExportRanges.KActionVideo, splittedVideoName, currentRowIndex);
                    }
                }

                currentRow = _application.Range[ExportRanges.ActionList][currentRowIndex];

                //Resizing de la ligne
                var rowHeight = settings.ThumbnailHeight + 2 * _MARGIN_TOP;
                currentRow.Rows[currentRowIndex].RowHeight = rowHeight;
                ExportToRange(ExportRanges.KActionHeightRow, rowHeight, currentRowIndex);

                ExportToRange(GetRangeNameFromProperty(() => action.Start, action), TimeSpan.FromTicks(action.Start).ToString(@"hh\:mm\:ss"), currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => action.Finish, action), TimeSpan.FromTicks(action.Finish).ToString(@"hh\:mm\:ss"), currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => action.Duration, action), TimeSpan.FromTicks(action.Duration).ToString(@"hh\:mm\:ss"), currentRowIndex);

                ExportToRange(GetRangeNameFromProperty(() => action.BuildStart, action), TimeSpan.FromTicks(action.BuildStart).ToString(@"hh\:mm\:ss"), currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => action.BuildFinish, action), TimeSpan.FromTicks(action.BuildFinish).ToString(@"hh\:mm\:ss"), currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => action.BuildDuration, action), TimeSpan.FromTicks(action.BuildDuration).ToString(@"hh\:mm\:ss"), currentRowIndex);
                
                ExportProperty(() => action.AmeliorationDescription, action, currentRowIndex);
                if (action.Category != null)
                    ExportToRange(GetRangeNameFromProperty(() => action.Category, action), action.Category.Label, currentRowIndex);
                ExportProperty(() => action.CustomNumericValue, action, currentRowIndex);
                ExportProperty(() => action.CustomNumericValue2, action, currentRowIndex);
                ExportProperty(() => action.CustomNumericValue3, action, currentRowIndex);
                ExportProperty(() => action.CustomNumericValue4, action, currentRowIndex);
                ExportProperty(() => action.CustomTextValue, action, currentRowIndex);
                ExportProperty(() => action.CustomTextValue2, action, currentRowIndex);
                ExportProperty(() => action.CustomTextValue3, action, currentRowIndex);
                ExportProperty(() => action.CustomTextValue4, action, currentRowIndex);
                ExportProperty(() => action.DifferenceReason, action, currentRowIndex);
                ExportProperty(() => action.DifferenceReasonManaged, action, currentRowIndex);
                ExportProperty(() => action.Label, action, currentRowIndex);
             
                SetTextSizeToRange(ExportRanges.MEFKActionLabel, settings.TaskTitleSize, currentRowIndex);

                ExportToRange(GetRangeNameFromProperty(() => action.IsGroup, action), action.IsGroup ? "1" : "0", currentRowIndex);
                ExportToRange(GetRangeNameFromProperty(() => action.IsRandom, action), action.IsRandom ? "1" : "0", currentRowIndex);

                if (action.IsReduced)
                {
                    ExportToRange(ExportRanges.ActionReductionRatio, action.Reduced.ReductionRatio.ToString(), currentRowIndex);
                    ExportToRange(ExportRanges.ActionSaving, action.Reduced.Saving.ToString(@"hh\:mm\:ss"), currentRowIndex);
                    ExportToRange(ExportRanges.ActionSolution, action.Reduced.Solution, currentRowIndex);
                    ExportToRange(ExportRanges.ActionApproved, action.Reduced.Approved.ToString(), currentRowIndex);
                }

                if (action.Operator != null)
                    ExportToRange(GetRangeNameFromProperty(() => action.Operator, action), action.Operator.Label, currentRowIndex);
                if (action.Equipment != null)
                    ExportToRange(GetRangeNameFromProperty(() => action.Equipment, action), action.Equipment.Label, currentRowIndex);
                if (action.Resource != null)
                    ExportToRange(GetRangeNameFromProperty(() => action.Resource, action), action.Resource.Label, currentRowIndex);
                ExportProperty(() => action.WBS, action, currentRowIndex);

                currentCellLeft = 1;
                currentCellTop = 1;
                maxLineCount = 1;

                for(int i = 1; i <= 7; i++)
                    ExportAction(action, i);

                #region Mise en forme des actions

                //Couleur de background si important
                if (IsActionImportant(action, settings))
                    SetBackgroundColorToRange(ExportRanges.MEFKActionLabel, importantTaskColor, currentRowIndex);

                //Hyperlink du label de la tache si vidéo
                if (settings.ExportOnlyKeyTasksVideos)
                {
                    if (action.IsGroup || (action.Video != null && IsActionImportant(action, settings)))
                        SetHyperlink(ExportRanges.MEFKActionLabel, currentRowIndex);
                }
                else
                {
                    if (action.IsGroup || action.Video != null)
                        SetHyperlink(ExportRanges.MEFKActionLabel, currentRowIndex);
                }

                if (action.Resource != null)
                {
                    if (action.Resource.Color != null)
                    {
                        //Couleur de fond de la colonne WBS en fonction de la couleur de la ressource
                        SetBackgroundColorToRange(GetRangeNameFromProperty(() => action.WBS, action), action.Resource.Color, currentRowIndex);
                    }

                    //Ajout du nom de la ressource en commentaire
                    SetCommentToRange(GetRangeNameFromProperty(() => action.WBS, action), action.Resource.Label, currentRowIndex);
                }

                #endregion

                try
                {
                    //Export de la thumbnail une fois que la ligne à sa taille fixe
                    if (action.Thumbnail != null)
                    {
                        ExportToRange(ExportRanges.KActionHasSnapshot, "1", currentRowIndex);
                        var thumbnailRange = _application.Range[GetRangeNameFromProperty(() => action.Thumbnail, action)];
                        Range currentThumbnailCell = thumbnailRange[currentRowIndex];
                        var imageSize = GetImageSize(action.Thumbnail);
                        var thumbnailNameWithExtension = $"{_THUMBNAIL_NAME_PART} - {action.WBS}.jpg";
                        var thumbnailPath = Path.Combine(directoryToThumbnails, thumbnailNameWithExtension);

                        var rangeAddress = GetRangeAddress(ExportRanges.KActionThumbnailWithExtension, currentRowIndex);

                        var ExcelFolderName = GetRangeValue(ExportRanges.ExcelFolderName).FormatForWindows();
                        var ExcelFinalFileName = GetRangeValue(ExportRanges.ExcelFinalFileName).FormatForWindows();
                        var FileVersion = GetRangeValue(ExportRanges.FileVersion);

                        if (!ExcelFolderName.IsNotNullNorEmpty())
                            ExcelFolderName = ExcelFinalFileName;

                        string formula;
                        if (repRelatif)
                        {
                            if (settings.ArchivageIsEnabled)
                                formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"v{FileVersion}\" & CHAR(92) & \"{_THUMBNAILS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                            else
                                formula = $"={ExportRanges.VideoDirectory} & CHAR(92) & \"{_THUMBNAILS_DIRECTORY_NAME}\" & CHAR(92) & {rangeAddress}";
                        }
                        else
                            formula = $"= { ExportRanges.VideoDirectory} & CHAR(92) & { ExportRanges.ExcelFolderName} & CHAR(92) & \"v{FileVersion}\" & CHAR(92) & \"{_THUMBNAILS_DIRECTORY_NAME}\" & CHAR(92) & { rangeAddress}";

                        ExportFormulaToRange(ExportRanges.KActionThumbnailPath, formula, currentRowIndex);
                        ExportToRange(ExportRanges.KActionThumbnailWithExtension, thumbnailNameWithExtension, currentRowIndex);

                        this.TraceInfo("Export de la thumnail action");
                        var lastImageExported = ExportImageToSheet("Scenario",
                           thumbnailPath,
                            left: (float)currentThumbnailCell.Left,
                            top: (float)currentThumbnailCell.Top,
                            width: ConvertDoubleToFloat(imageSize.Width * settings.ThumbnailHeight / imageSize.Height),
                            height: ConvertDoubleToFloat(settings.ThumbnailHeight),
                            shapeName: $"{_THUMBNAIL_NAME_PART} - {action.WBS}",
                            rowIndex: currentRowIndex);

                        //var xlToPxConverter = 100 / 75.0;
                        //var pxToXlConverter = 75.0 / 100;

                        this.TraceInfo("Centrage de la thumbnail dans la cellule");
                        var centeredImageLeft = (float)(currentThumbnailCell.Left + ((currentThumbnailCell.Width - (lastImageExported.Width)) / 2));
                        var centeredImageTop = (float)(currentThumbnailCell.Top + ((currentThumbnailCell.Height - (lastImageExported.Height)) / 2));
                        lastImageExported.Left = centeredImageLeft;
                        lastImageExported.Top = centeredImageTop;
                    }
                    else
                        ExportToRange(ExportRanges.KActionHasSnapshot, "0", currentRowIndex);
                }
                catch (Exception e)
                {
                    throw e;
                }

                currentRowIndex++;
            }

            #region Gestion des groupes

            List<RangeGroup> listGroup = new List<RangeGroup>();
            WBSTreeVirtualizer tree = new WBSTreeVirtualizer(allActions);

            foreach (var node in tree.ActionTree)
            {
                if (node.Children.Count > 0)
                {
                    var lastchild = GetLastChild(node, allActions.ToList(), tree);
                    listGroup.Add(new RangeGroup
                    {
                        RangeStart = _application.Range[ExportRanges.ActionList].Rows[allActions.IndexOf(node.Children[0].Action) + 1],
                        RangeFinish = _application.Range[ExportRanges.ActionList].Rows[allActions.IndexOf(lastchild.Action) + 1],
                        WBSStart = node.Children[0].Action.WBS,
                        WBSFinish = lastchild.Action.WBS,
                        StartLineNumber = allActions.IndexOf(node.Children[0].Action) + 1,
                        FinishLineNumber = allActions.IndexOf(lastchild.Action) + 1
                    });
                    AddChildRangeInGroup(listGroup, node, allActions.ToList(), tree);
                }
            }

            foreach (var group in listGroup.Where(g => g.WBSFinish != g.WBSStart))
            {
                var test = group.RangeStart.Row;
                var test2 = group.RangeStart.Column;
                var test3 = group.RangeFinish.Row;
                var test4 = group.RangeFinish.Column;

                _application.Range[group.RangeStart.EntireRow, group.RangeFinish.EntireRow].Group();
            }

            #endregion
        }

        private TrackableCollection<IReferentialActionLink> GetRefActions(KAction action, int refNumber)
        {
            switch(refNumber)
            {
                case 1:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref1.Cast<IReferentialActionLink>());
                case 2:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref2.Cast<IReferentialActionLink>());
                case 3:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref3.Cast<IReferentialActionLink>());
                case 4:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref4.Cast<IReferentialActionLink>());
                case 5:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref5.Cast<IReferentialActionLink>());
                case 6:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref6.Cast<IReferentialActionLink>());
                case 7:
                    return new TrackableCollection<IReferentialActionLink>(action.Ref7.Cast<IReferentialActionLink>());
                default:
                    return null;
            }
        }

        private void ExportAction(KAction action, int refNumber)
        {
            StringBuilder refToExport = new StringBuilder();
            Shape lastImageInserted = null;
            int lineCount = 1;
            int imageCount = 0;

            foreach (var reff in GetRefActions(action, refNumber))
            {
                if (refToExport.Length > 0)
                    refToExport.AppendLine();
                if (_scenario.Project.Referentials.FirstOrDefault(r => r.ReferentialId == 6).HasQuantity)
                    refToExport.Append($"{reff.Quantity}x");
                refToExport.Append(reff.Referential.Label);

                //Export de l'image de la ref si elle existe
                if (RangeExists(ExportRanges.RefDictionary[refNumber]["KActionImage"]) && !string.IsNullOrEmpty(reff.Referential.Uri))
                {
                    currentCellLeft = (float)_application.Range[ExportRanges.RefDictionary[refNumber]["KActionImage"]][currentRowIndex].Left + _MARGIN_LEFT;
                    currentCellTop = (float)_application.Range[ExportRanges.RefDictionary[refNumber]["KActionImage"]][currentRowIndex].Top + _MARGIN_TOP;

                    var modulo = imageCount % MaxNumberActionRefImagePerLine[refNumber];

                    //Première image
                    if (lastImageInserted == null)
                    {
                        left = currentCellLeft + _MARGIN_LEFT;
                        top = currentCellTop + _MARGIN_TOP;
                    }
                    //On a pas atteint le nb max d'image sur la ligne
                    else if (modulo != 0)
                    {
                        left = lastImageInserted.Left + lastImageInserted.Width + _MARGIN_LEFT;
                        top = AddCaptionToRefAction[refNumber] ? lastImageInserted.Top - _CAPTION_MARGIN_CORRECTION : lastImageInserted.Top;
                    }
                    else
                    {
                        left = currentCellLeft + _MARGIN_LEFT;
                        top = lastImageInserted.Top + lastImageInserted.Height + _MARGIN_TOP;
                        lineCount++;
                        if (lineCount > maxLineCount)
                            maxLineCount = lineCount;
                    }

                    double imageHeightLine = 50;
                    double.TryParse(GetRangeValue(ExportRanges.RefDictionary[refNumber]["ImageHeightLine"]), out imageHeightLine);
                    if (lineCount > 0)
                    {
                        var neededHeight = lineCount * (imageHeightLine + _MARGIN_TOP) + 3 * _MARGIN_TOP;
                        if (AddCaptionToRefAction[refNumber])
                            neededHeight += lineCount * (GetTextBoxHeight(CaptionSizeRefAction[refNumber]) + _CAPTION_MARGIN_CORRECTION) + _CAPTION_MARGIN_CORRECTION;
                        this.TraceInfo($"Vérification hauteur de ligne Ref{refNumber} action {action.WBS}: currentHeight: {currentRow.Rows[currentRowIndex].RowHeight} // neededHeight: {neededHeight}");
                        if (currentRow.Rows[currentRowIndex].RowHeight < neededHeight)
                        {
                            var oldValue = currentRow.Rows[currentRowIndex].RowHeight;
                            currentRow.Rows[currentRowIndex].RowHeight = neededHeight;
                            this.TraceInfo($"Mise à jour hauteur de ligne {oldValue} => {currentRow.Rows[currentRowIndex].RowHeight}");
                            ExportToRange(ExportRanges.KActionHeightRow, neededHeight, currentRowIndex);
                        }
                    }

                    lastImageInserted = ExportImageToSheet("Scenario",
                        reff.Referential.Uri,
                        left: left,
                        top: top,
                        width: ConvertDoubleToFloat(imageHeightLine),
                        height: ConvertDoubleToFloat(imageHeightLine),
                        shapeName: $"{_ACTION_REF_IMAGE_NAME_PART}{refNumber}.{imageCount}.{Guid.NewGuid()}",
                        rowIndex: currentRowIndex,
                        caption: AddCaptionToRefAction[refNumber] ? reff.Referential.Label : null,
                        fontSize: CaptionSizeRefAction[refNumber]);

                    imageCount++;
                }

                if (imageCount > 0)
                    ExportToRange(ExportRanges.RefDictionary[refNumber]["KActionNbImages"], imageCount.ToString(), currentRowIndex);

                //Resize de la colonne et de la ligne
                if (refToExport.Length > 0)
                    ExportToRange(ExportRanges.RefDictionary[refNumber]["KAction"], refToExport.ToString(), currentRowIndex);
            }
        }

        private void AddChildRangeInGroup(List<RangeGroup> listGroup, WBSTreeVirtualizer.Node node, List<KAction> allActions, WBSTreeVirtualizer tree)
        {
            foreach (var childNode in node.Children)
            {
                if (childNode.Children.Count > 0)
                {
                    var lastchild = GetLastChild(childNode, allActions.ToList(), tree);
                    listGroup.Add(new RangeGroup
                    {
                        RangeStart = _application.Range[ExportRanges.ActionList].Rows[allActions.IndexOf(childNode.Children[0].Action) + 1],
                        RangeFinish = _application.Range[ExportRanges.ActionList].Rows[allActions.IndexOf(lastchild.Action) + 1],
                        WBSStart = childNode.Children[0].Action.WBS,
                        WBSFinish = lastchild.Action.WBS,
                        StartLineNumber = allActions.IndexOf(childNode.Children[0].Action) + 1,
                        FinishLineNumber = allActions.IndexOf(lastchild.Action) + 1
                    });

                    AddChildRangeInGroup(listGroup, childNode, allActions, tree);
                }
            }
        }

        private WBSTreeVirtualizer.Node GetLastChild(WBSTreeVirtualizer.Node node, List<KAction> allActions, WBSTreeVirtualizer tree)
        {
            if (node.Children.Count > 0)
                return GetLastChild(tree.GetNode(WBSHelper.GetLastChild(node.Action, allActions)), allActions, tree);
            else
                return node;
        }


        private void ExportOther()
        {
            ExportToRange(ExportRanges.ExportDate, DateTime.Now.ToString("dd/MM/yyyy"));
            ExportToRange(ExportRanges.Owner, GetOwner().Username);

            ExportToRange(ExportRanges.MinI, TimeSpan.FromTicks(_minI).ToString(@"hh\:mm\:ss"));
            ExportToRange(ExportRanges.MaxI, TimeSpan.FromTicks(_maxI).ToString(@"hh\:mm\:ss"));
            var criticalPathDuration = _scenario.CriticalPathIDuration;
            var criticalPathDurationIE = _scenario.Actions.Any() ? _scenario.Actions.Max(a => a.BuildFinish) - _scenario.Actions.Min(a => a.BuildStart) : 0;
            ExportToRange(ExportRanges.ProcessI, TimeSpan.FromTicks(criticalPathDuration).ToString(@"hh\:mm\:ss"));
            ExportToRange(ExportRanges.ProcessIE, TimeSpan.FromTicks(criticalPathDurationIE).ToString(@"hh\:mm\:ss"));
        }

        private void GetAllExcelNames()
        {
            _excelNames = new List<string>();
            foreach (Name name in _application.Names)
                _excelNames.Add(name.Name);
        }

        public string GetRangeAddress(string range, int rowIndex = 1, int columnIndex = 1) =>
            RangeExists(range) ? _application.Range[range][rowIndex][columnIndex].AddressLocal[false, false, XlReferenceStyle.xlA1] : null;

        public string GetRangeValue(string range, int rowIndex = 1, int columnIndex = 1)
        {
            if (RangeExists(range))
            {
                object ret = _application.Range[range][rowIndex][columnIndex].Value;
                if (ret != null)
                    return ret.ToString();
            }
            return null;
        }

        private bool RangeExists(string range)
        {
            try
            {
                return (_excelNames.Contains(range) && !(_application.Range[range] is null));
            }
            catch
            {
                LogExcelError($"The range '{range}' may be uninitialized. Check #ref! presence with the command 'ctrl + F3'.");
                return false;
            }

        }

        //private void ExportToRange(string range, string value, int rowIndex = 1, int columnIndex = 1)
        //{
        //    ExportToRange(range, value, rowIndex, columnIndex);
        //}

        private void ExportToRange(string range, object value, int rowIndex = 1, int columnIndex = 1)
        {
            if (RangeExists(range))
                _application.Range[range][rowIndex][columnIndex].Value = value;
        }

        private void ExportFormulaToRange(string range, string formula, int rowIndex = 1, int columnIndex = 1)
        {
            if (RangeExists(range))
                ((Range)_application.Range[range][rowIndex][columnIndex]).Formula = formula;
        }

        private User GetOwner()
        {
            var service = IoC.Resolve<IServiceBus>().Get<IAuthenticationService>();
            return service.GetUser(Security.SecurityContext.CurrentUser.Username);
        }

        private void ExportThumbnails(string directoryToSaveThumbnails, Settings settings, string excelFileName, string excelFolderName, string fileVersion)
        {
            foreach (var action in _scenario.Actions)
            {
                //Save des thumbnails sur le disque
                if (action.Thumbnail != null)
                {
                    SaveImageToDisk(directoryToSaveThumbnails, $"{_THUMBNAIL_NAME_PART} - {action.WBS}.jpg", action.Thumbnail);

                    if (settings.VideoMarkingIsEnabled)
                        AddWatermarkToThumbnail(
                            Path.Combine(directoryToSaveThumbnails, $"{_THUMBNAIL_NAME_PART} - {action.WBS}.jpg"),
                            settings,
                            action.WBS,
                            excelFileName);

                    if (settings.ArchivageIsEnabled && settings.RedistribuableIsEnabled)
                    {
                        if (!Directory.Exists(Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{fileVersion}", _THUMBNAILS_DIRECTORY_NAME)))
                            Directory.CreateDirectory(Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{fileVersion}", _THUMBNAILS_DIRECTORY_NAME));
                        File.Copy(
                            Path.Combine(directoryToSaveThumbnails, $"{_THUMBNAIL_NAME_PART} - {action.WBS}.jpg"),
                            Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{excelFolderName} - {(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{fileVersion}", _THUMBNAILS_DIRECTORY_NAME, $"{_THUMBNAIL_NAME_PART} - {action.WBS}.jpg"));
                    }
                }
            }
        }

        private void AddWatermarkToThumbnail(string thumbnailFileName, Settings settings, string wbs, string excelFileName)
        {
            string markings = settings.OverlayTextVideo;
            if (settings.ExcelFileNameMarkingIsEnabled)
                markings += $" - {excelFileName}";
            if (settings.WBSMarkingIsEnabled)
                markings += $" - {wbs}";

            var exeFilePath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Video\",
                Environment.Is64BitOperatingSystem ? "videosplitter-64.exe" : "videosplitter-32.exe");
            var fontFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"Video\", "arial.ttf");
            var textFilePath = Path.GetTempFileName();
            if (!string.IsNullOrWhiteSpace(markings))
            {
                using (var stream = File.OpenWrite(textFilePath))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
                {
                    writer.Write(markings.Replace(@"\", @"\\"));
                }
            }

            string markingAlignment = string.Empty;

            switch (settings.HorizontalAlignement)
            {
                case EHorizontalAlign.Center:
                    markingAlignment = $"x=(w-tw)/2";
                    break;
                case EHorizontalAlign.Left:
                    markingAlignment = $"x={_MARKING_PADDING}";
                    break;
                case EHorizontalAlign.Right:
                    markingAlignment = $"x=w-tw-{_MARKING_PADDING}";
                    break;
            }

            switch (settings.VerticalAlignement)
            {
                case EVerticalAlign.Bottom:
                    markingAlignment += $":y=h-th-{_MARKING_PADDING}";
                    break;
                case EVerticalAlign.Center:
                    markingAlignment += $":y=(h-th)/2";
                    break;
                case EVerticalAlign.Top:
                    markingAlignment += $":y={_MARKING_PADDING}";
                    break;
            }

            var processArgumentsBuilder = new StringBuilder("-y -nostdin");
            processArgumentsBuilder.Append($" -i \"{thumbnailFileName}\"");
            if (!string.IsNullOrWhiteSpace(markings))
            {
                string formattedFontFilePath = fontFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                string formattedtextFilePath = textFilePath.Replace(@"\", @"\\").Replace(":", @"\:");
                processArgumentsBuilder.Append($" -vf \"drawtext=fontfile='{formattedFontFilePath}':textfile='{formattedtextFilePath}':fontcolor=white:fontsize=24:box=1:boxcolor=black@0.2:boxborderw=5:{markingAlignment}\"");
            }
            else
                return;
            processArgumentsBuilder.Append($" \"{thumbnailFileName}\"");

            using (var process = new Process())
            {
                process.StartInfo.FileName = exeFilePath;
                process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                this.TraceDebug($"{exeFilePath} {processArgumentsBuilder}");

                try
                {
                    string errorOutput = null;
                    string standardOutput = null;

                    process.Start();

                    var outputStreamTasks = new[]
                    {
                        Task.Factory.StartNew(() => errorOutput = process.StandardError.ReadToEnd()),
                        Task.Factory.StartNew(() => standardOutput = process.StandardOutput.ReadToEnd())
                    };

                    var timeout = TimeSpan.FromSeconds(10); // TODO add to configuration
                    process.WaitForExit((int)timeout.TotalMilliseconds);
                    Task.WaitAll(outputStreamTasks, (int)timeout.TotalMilliseconds, KprocessExportWindow.CancellationToken);

                    this.TraceDebug(standardOutput);
                    Debug.WriteLine(standardOutput);

                    if (!string.IsNullOrWhiteSpace(errorOutput))
                    {
                        this.TraceError("Des erreurs ont été levées par le processus permettant de créer les thumbnails:");
                        this.TraceError(errorOutput);
                        Debug.WriteLine(errorOutput);
                    }
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Une erreur non prévue s'est produite lors de la création des thumbnails");
                    throw;
                }
                finally
                {
                    try
                    {
                        File.Delete(textFilePath);
                    }
                    catch { }
                }
            }
        }

        private Trackability PreExport(string previousVersionFile)
        {
            foreach (var action in _scenario.Actions)
            {
                //On charge les catégories
                if (action.Category != null)
                    _categories.AddNew(action.Category);

                //On charge les opérateurs
                if (action.Operator != null)
                    _operators.AddNew(action.Operator);

                //On charge les equipements
                if (action.Equipment != null)
                    _equipments.AddNew(action.Equipment);

                //On charge les vidéos
                if (action.Video != null)
                    _videosApplicables.AddNew(action.Video);

                #region Referentials

                if (!action.Ref1.IsEmpty())
                    foreach (var ref1 in action.Ref1)
                    {
                        if (ref1 != null)
                            ListRefDictionary[1].AddNew(ref1.Ref1);
                    }

                if (!action.Ref2.IsEmpty())
                    foreach (var ref2 in action.Ref2)
                    {
                        if (ref2 != null)
                            ListRefDictionary[2].AddNew(ref2.Ref2);
                    }

                if (!action.Ref3.IsEmpty())
                    foreach (var ref3 in action.Ref3)
                    {
                        if (ref3 != null)
                            ListRefDictionary[3].AddNew(ref3.Ref3);
                    }

                if (!action.Ref4.IsEmpty())
                    foreach (var ref4 in action.Ref4)
                    {
                        if (ref4 != null)
                            ListRefDictionary[4].AddNew(ref4.Ref4);
                    }

                if (!action.Ref5.IsEmpty())
                    foreach (var ref5 in action.Ref5)
                    {
                        if (ref5 != null)
                            ListRefDictionary[5].AddNew(ref5.Ref5);
                    }

                if (!action.Ref6.IsEmpty())
                    foreach (var ref6 in action.Ref6)
                    {
                        if (ref6 != null)
                            ListRefDictionary[6].AddNew(ref6.Ref6);
                    }

                if (!action.Ref7.IsEmpty())
                    foreach (var ref7 in action.Ref7)
                    {
                        if (ref7 != null)
                            ListRefDictionary[7].AddNew(ref7.Ref7);
                    }

                #endregion

                //Min_I, Max_I
                if (!action.IsGroup &&
                    action.GetIES() != null &&
                    action.GetIES() == KProcess.Globalization.LocalizationManager.GetString("ViewModel_AnalyzeRestitution_Export_Action_Reduced_Internal"))
                {
                    _minI = Math.Min(action.BuildStart, _minI);
                    _maxI = Math.Max(action.BuildFinish, _maxI);
                }
            }

            //Si on a sélectionné un ancien fichier d'export, on récupère la tracabilité
            if (!string.IsNullOrWhiteSpace(previousVersionFile) && File.Exists(previousVersionFile))
            {
                //Récupération des infos tracabilité de l'ancien fichier sélectionné
                var previousWb = _application.Workbooks.Open(previousVersionFile);
                //On récupère les ranges name de ce WB
                GetAllExcelNames();
                var previousTrackability = new Trackability();

                int currentRowIndex = 1;
                int indice = -1;
                while (!string.IsNullOrWhiteSpace(GetRangeValue(ExportRanges.TrackabilityIndice, currentRowIndex)))
                {
                    var trackItem = new TrackabilityItem
                    {
                        Modification = GetRangeValue(ExportRanges.TrackabilityModification, currentRowIndex),
                        Date = GetRangeValue(ExportRanges.TrackabilityDate, currentRowIndex),
                        Visa = GetRangeValue(ExportRanges.TrackabilityVisa, currentRowIndex)
                    };
                    if (int.TryParse(GetRangeValue(ExportRanges.TrackabilityIndice, currentRowIndex), out indice))
                        trackItem.Indice = indice;

                    previousTrackability.ModificationList.Add(trackItem);

                    currentRowIndex++;
                }
                Video video = new Video();
                currentRowIndex = 1;
                while (!string.IsNullOrWhiteSpace(GetRangeValue(GetRangeNameFromProperty(() => video.Name, video), currentRowIndex)))
                {
                    var trackVideoItem = new TrackabilityVideoItem
                    {
                        Name = GetRangeValue(GetRangeNameFromProperty(() => video.Name, video), currentRowIndex),
                        Description = GetRangeValue(GetRangeNameFromProperty(() => video.Description, video), currentRowIndex),
                        FilePath = GetRangeValue(GetRangeNameFromProperty(() => video.FilePath, video), currentRowIndex),
                        FileName = GetRangeValue(GetRangeNameFromProperty(() => video.Filename, video), currentRowIndex),
                        Applicable = GetRangeValue(ExportRanges.VideoApplicable, currentRowIndex)
                    };

                    previousTrackability.VideoList.Add(trackVideoItem);

                    currentRowIndex++;
                }

                //On ferme le wb une fois les informations récupérées
                previousWb.Close(SaveChanges: false);
                previousWb = null;

                return previousTrackability;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputExcelPath"></param>
        /// <returns>The final excel name</returns>
        public (string nomFichierFinal, string FileVersion) Export(string outputExcelPath, string previousVersionFile, string directoryPath, ISettingsService settingService, string VideoExportDirectory)
        {
            _settingService = settingService;
            var settings = _settingService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);

            _outputExcelPath = outputExcelPath;

            if (settings.ExcelExportIsEnabled)
            {
                var previousTrackability = PreExport(previousVersionFile);

                New_wb = _application.Workbooks.Open(_modelFilePath, ReadOnly: false, IgnoreReadOnlyRecommended: true, Editable: true, Notify: false);

                //On récupère tous les ranges nommés dans excel
                GetAllExcelNames();

                //Export de la tracabilité
                ExportTrackability(previousTrackability);
                //Export des propriétés du projet
                ExportProject();
                //Export des propriétés du scenario
                ExportScenario();
                //Export des catégories
                ExportCategories();
                //Export des opérateurs
                ExportOperators();
                //Export des équipements
                ExportEquipments();
                //Export des vidéos
                ExportVideosBase(previousTrackability);
            }

            //On récupère les données stockées dans excel pour export directory
            string ExcelFolderName = settings.ExcelExportIsEnabled ? GetRangeValue(ExportRanges.ExcelFolderName).FormatForWindows().Trim() : _scenario.Project.Label.FormatForWindows().Trim();
            string FileVersion = settings.ExcelExportIsEnabled ? GetRangeValue(ExportRanges.FileVersion) : DateTime.Now.ToString("yyyyMMddHHmm");
            string ExcelFinalFileName = settings.ExcelExportIsEnabled ? GetRangeValue(ExportRanges.ExcelFinalFileName).FormatForWindows() : $"{ExcelFolderName} - {FileVersion}";

            if (!ExcelFolderName.IsNotNullNorEmpty())
                ExcelFolderName = ExcelFinalFileName;

            string thumbnailsDirectory;
            if (settings.ArchivageIsEnabled)
            {
                if (string.IsNullOrWhiteSpace(VideoExportDirectory))
                    thumbnailsDirectory = Path.Combine(directoryPath, ExcelFolderName, $"{(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{FileVersion}", _THUMBNAILS_DIRECTORY_NAME);
                else
                    thumbnailsDirectory = Path.Combine(VideoExportDirectory, ExcelFolderName, $"{(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{FileVersion}", _THUMBNAILS_DIRECTORY_NAME);
            }
            else
                thumbnailsDirectory = Path.Combine(settings.DefaultExportDirectory_Redistribuable, $"{ExcelFolderName} - {(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{FileVersion}", _THUMBNAILS_DIRECTORY_NAME);

            //Export des thumbnails
            ExportThumbnails(thumbnailsDirectory, settings, ExcelFinalFileName, ExcelFolderName, FileVersion);

            if (settings.ExcelExportIsEnabled)
            {
                //Export des actions
                ExportActions(thumbnailsDirectory, string.IsNullOrWhiteSpace(VideoExportDirectory));

                ExportReferentials();
                //Exporte les cas particuliers
                ExportOther();

                if (!string.IsNullOrWhiteSpace(VideoExportDirectory) && settings.ArchivageIsEnabled)
                {
                    ExportToRange(ExportRanges.VideoDirectory, VideoExportDirectory);
                    ExportToRange(ExportRanges.RepVideoRelatif, "0");
                }
                else
                    ExportToRange(ExportRanges.RepVideoRelatif, "1");

                //On renomme le sheet (à faire en dernier car on travail sur le l'ancien nom du sheet durant l'export)
                try
                {
                    SetSheetName(ExportRanges.ScenarioSheetOldName, GetRangeValue(ExportRanges.ScenarioSheetName).Trim());
                }
                catch
                {
                    LogExcelError($"Named range \"ScenarioSheetName\" was not found.");
                }

                // 'Sauvegarde automatique du nouveau fichier dans le répertoire de raw_wb
                New_wb.SaveAs(
                    Filename: _outputExcelPath,
                    FileFormat: XlFileFormat.xlOpenXMLWorkbookMacroEnabled,
                    ReadOnlyRecommended: settings.ArchivageIsEnabled ? settings.IsReadOnlyEnabled_Archivage : settings.IsReadOnlyEnabled_Redistribuable,
                    CreateBackup: false);
            }

            if (settings.ArchivageIsEnabled)
                return ($"{ExcelFolderName}\\{ExcelFinalFileName}", FileVersion);
            else
                return ($"{ExcelFolderName} - {(settings.ExcelExportIsEnabled ? "v" : string.Empty)}{FileVersion}\\{ExcelFinalFileName}", FileVersion);
        }

        /// <summary>
        /// Détermine la hauteur d'une textbox
        /// </summary>
        /// <param name="fontSize"></param>
        /// <returns>La hauteur de la textbox</returns>
        private double GetTextBoxHeight(int fontSize)
        {
            dynamic textbox = _application.Sheets[1].Shapes.AddTextbox(Microsoft.Office.Core.MsoTextOrientation.msoTextOrientationHorizontal, 0, 0, 0, 0);
            textbox.TextFrame.AutoSize = true;
            textbox.TextFrame.MarginLeft = 0;
            textbox.TextFrame.MarginRight = 0;
            textbox.TextFrame.MarginTop = 0;
            textbox.TextFrame.MarginBottom = 0;
            textbox.TextFrame.Characters.Font.Size = fontSize;
            textbox.TextFrame.Characters.Text = "TestTextBoxHeight";
            double result = textbox.Height;
            textbox.Delete();

            return result;
        }
    }
}