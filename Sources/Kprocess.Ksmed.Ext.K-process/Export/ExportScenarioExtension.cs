using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Ext.Kprocess.Export
{
    /// <summary>
    /// Permet d'exporter un scénario sous le format 3D-Plus CV.
    /// </summary>
    class ExportScenarioExtension : IExtBarAction<IPrepareScenariosViewModel>
    {
        private const string _NO_MODE_SELECTED = "_NO_MODE_SELECTED";
        private const string _NO_EXPORT_SELECTED = "_NO_EXPORT_SELECTED";
        private const string _MODEL_FILE_NOT_EXISTS = "_MODEL_FILE_NOT_EXISTS";
        private int _errorCount = 0;
        private static readonly Uri _iconUri = new Uri(Path.Combine(KprocessExtension.AssemblyDirectory,
            "Resources/exportexcelKp.png"), UriKind.Absolute);
        //private const string modelRelativePath = @"Resources\3D-Plus modele procedure KL2 v2.xltm";

        public Guid ExtensionId => KprocessExtension.Id;

        /// <summary>
        /// Obtient le libellé de l'action.
        /// </summary>
        public string Label =>
            LocalizationManager.GetString("ExtKp_Export_Label", ResourceExport.Id);

        /// <summary>
        /// Obtient l'uri de l'icône en petite taille.
        /// </summary>
        public Uri SmallIconUri =>
            _iconUri;

        /// <summary>
        /// Obtient l'uri de l'icône en grande taille.
        /// </summary>
        public Uri LargeIconUri =>
            _iconUri;

        /// <summary>
        /// Définit le view model associé.
        /// </summary>
        public IPrepareScenariosViewModel ViewModel { private get; set; }

        /// <summary>
        /// Obtient la fabrique de gestion des interfaces de dialogue
        /// </summary>
        [Import]
        private IDialogFactory DialogFactory { get; set; }

        /// <summary>
        /// Obtient ou définit le bus de services.
        /// </summary>
        [Import]
        private IServiceBus ServiceBus { get; set; }

        private Command _action;
        /// <summary>
        /// Obtient l'action à exécuter.
        /// </summary>
        public ICommand Action
        {
            get
            {
                if (_action == null)
                    _action = new Command(ExecuteExport, CanExecuteExport);

                return _action;
            }
        }

        private bool CanExecuteExport() =>
            ViewModel != null && ViewModel.CurrentScenario != null && ViewModel.CanCurrentUserWrite && ViewModel.CanChange;

        private void ExecuteExport()
        {
            ServiceBus.Get<ISpinnerService>().ShowIncrement(null);
            if (!VerifySettings())
            {
                ServiceBus.Get<ISpinnerService>().HideDecrement();
                return;
            }

            var service = ServiceBus.Get<IAnalyzeService>();
            service.GetFullProjectDetails(ViewModel.CurrentScenario.ProjectId,
                async data =>
                {
                    var rawFilePath = $"{Path.GetTempFileName()}.xlsm";// "d:/Test.xlsm";
                    var tempExcelExportPath = $"{Path.GetTempFileName()}.xlsm";

                    try
                    {
                        this.TraceDebug("Début export Kp");

                        if (string.IsNullOrWhiteSpace(data.Project.Label))
                            throw new Exception(LocalizationManager.GetString("ExtKp_Error_MustContainDash"));

                        var exporterAssembly = Assembly.GetEntryAssembly();
                        var ExcelExporterType = exporterAssembly.GetTypes().First(type => type.FullName == "KProcess.Ksmed.Presentation.ViewModels.Restitution.ExportProjectToExcel");
                        var exporter = ExcelExporterType
                            .GetConstructor(new[] { typeof(RestitutionData), typeof(ExportResult) })
                            .Invoke(new object[] { data, new ExportResult { Filename = rawFilePath, Accepts = true, OpenWhenCreated = false } });

                        exporter.GetType().GetMethod("Export", new Type[] { }).Invoke(exporter, new object[] { });

                        var packagePath = await DoExport(data, tempExcelExportPath);
                        string calculatedPackagePath = string.Empty;
                        if (!string.IsNullOrEmpty(packagePath.packagePath_Archive))
                            calculatedPackagePath = $"\"{packagePath.packagePath_Archive}\"";
                        if (!string.IsNullOrEmpty(packagePath.packagePath_Redistribuable))
                            calculatedPackagePath += $"{(!string.IsNullOrEmpty(packagePath.packagePath_Redistribuable) && !string.IsNullOrEmpty(calculatedPackagePath) ? " and " : string.Empty)}\"{packagePath.packagePath_Redistribuable}\"";

                        if (_errorCount > 0)
                            DialogFactory.GetDialogView<IMessageDialog>().Show(
                                string.Format(LocalizationManager.GetString("ExtKp_Dlg_PackageExportedWithErrors"), calculatedPackagePath),
                                string.Empty,
                                MessageDialogButton.OK,
                                MessageDialogImage.Warning);
                        else
                            DialogFactory.GetDialogView<IMessageDialog>().Show(
                                string.Format(LocalizationManager.GetString("ExtKp_Dlg_PackageExportedWithSuccess"), calculatedPackagePath),//"Votre package contenant la procédure et toutes les séquences vidéo a été créé dans le répertoire {0}"
                                string.Empty,
                                MessageDialogButton.OK,
                                MessageDialogImage.Information);

                        ServiceBus.Get<ISpinnerService>().HideDecrement();
                        this.TraceDebug("Fin export SmartExport");
                    }
                    catch (OperationCanceledException e)
                    {
                        this.TraceInfo(e, "L'opération d'export a été annulé");
                        switch (e.Message)
                        {
                            case _MODEL_FILE_NOT_EXISTS:
                                DialogFactory.GetDialogView<IMessageDialog>().Show(
                                    e.InnerException.Message,
                                    string.Empty,
                                    MessageDialogButton.OK,
                                    MessageDialogImage.Error);
                                break;
                            case _NO_MODE_SELECTED:
                                DialogFactory.GetDialogView<IMessageDialog>().Show(
                                    LocalizationManager.GetString("ExtKp_Dlg_No_Mode_Selected"),
                                    string.Empty,
                                    MessageDialogButton.OK,
                                    MessageDialogImage.Information);
                                break;
                            case _NO_EXPORT_SELECTED:
                                DialogFactory.GetDialogView<IMessageDialog>().Show(
                                    LocalizationManager.GetString("ExtKp_Dlg_No_Export_Selected"),
                                    string.Empty,
                                    MessageDialogButton.OK,
                                    MessageDialogImage.Information);
                                break;
                            default:
                                DialogFactory.GetDialogView<IMessageDialog>().Show(
                                    LocalizationManager.GetString("ExtKp_Dlg_ExportCanceled"),//"L'opération d'export a été arrêtée par l'utilisateur",
                                    string.Empty,
                                    MessageDialogButton.OK,
                                    MessageDialogImage.Information);
                                break;
                        }

                        ServiceBus.Get<ISpinnerService>().HideDecrement();
                    }
                    catch (Exception e)
                    {
                        DialogFactory.GetDialogView<IErrorDialog>().Show(
                            e.Message,
                            LocalizationManager.GetString("Common_Error"),
                            e);

                        ServiceBus.Get<ISpinnerService>().HideDecrement();
                    }
                },
                OnError);
        }

        private async Task<(string packagePath_Archive, string packagePath_Redistribuable)> DoExport(RestitutionData data, string tempExcelExportPath)
        {
            try
            {
                var dialog1 = DialogFactory.GetDialogView<IMessageDialog>();
                if (dialog1.Show(
                   LocalizationManager.GetString("ExtKp_Dlg_DoYouWantToLaunchExport"),//"Etes-vous sûr de vouloir lancer l'export ?"
                   LocalizationManager.GetString("ExtKp_Dlg_LaunchExport"),//"Lancement de l'export",
                   MessageDialogButton.YesNo,
                   MessageDialogImage.Question) == MessageDialogResult.Yes)
                {
                    var currentScenario = data.Scenarios.First(scenario => scenario.ScenarioId == ViewModel.CurrentScenario.ScenarioId);
                    var referentials = ServiceBus.Get<IReferentialsService>().GetApplicationReferentials();

                    KprocessExportWindow.StartNew();

                    var criticalPath = currentScenario.Project.ScenariosCriticalPath.SingleOrDefault(s => s.Id == currentScenario.ScenarioId);

                    var settingsService = ServiceBus.Get<ISettingsService>();
                    Settings settings = settingsService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id);

                    // On vérifie qu'il y a au moins un type d'export actif
                    if (!(settings?.ArchivageIsEnabled == true || settings?.RedistribuableIsEnabled == true))
                        throw new OperationCanceledException(_NO_MODE_SELECTED);

                    // On vérifie qu'il y a au moins un export actif
                    if (!(settings?.ExcelExportIsEnabled == true || settings?.VideoExportIsEnabled == true))
                        throw new OperationCanceledException(_NO_EXPORT_SELECTED);

                    #region Repertoire Export Excel

                    string videoPath = currentScenario.Actions
                                  .Where(a => a.Video != null)
                                  .Select(a => a.Video.FilePath)
                                  .FirstOrDefault(File.Exists);

                    string exportDefaultPath;
                    if (settings?.ArchivageIsEnabled == true)
                        exportDefaultPath = settings?.DefaultExportDirectory_Archivage;
                    else
                        exportDefaultPath = settings?.DefaultExportDirectory_Redistribuable;

                    string ExcelExportDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (!string.IsNullOrWhiteSpace(exportDefaultPath))
                        ExcelExportDirectory = exportDefaultPath;
                    else if (!string.IsNullOrWhiteSpace(videoPath))
                        ExcelExportDirectory = Path.GetDirectoryName(videoPath);

                    #endregion

                    #region Repertoire Export video

                    string VideoExportDirectory = settings?.ArchivageIsEnabled == true ?
                        settings?.DefaultExportVideoDirectory :
                        null;

                    #endregion

                    string modelFilePath = null;
                    bool multiThreadVideoEncoding = false;
                    try
                    {
                        modelFilePath = settings?.ExportExcelModelPath;
                    }
                    catch { }

                    try
                    {
                        multiThreadVideoEncoding = (settings?.MultiThreading).GetValueOrDefault();
                    }
                    catch { }

                    if (!string.IsNullOrWhiteSpace(modelFilePath) && File.Exists(modelFilePath))
                    {
                        this.TraceDebug(string.Format("Lancement de l'export avec le modèle '{0}'", modelFilePath));
                        using (var exporter = new Exporter(modelFilePath))
                        {
                            string previousVersionFile = null;
                            this.TraceDebug("exporter.Open()");
                            exporter.Open(currentScenario, referentials, settings.ExcelExportIsEnabled);
                            if (settings?.ExcelExportIsEnabled == true)
                            {
                                CheckCancellation();
                                var dialog = new Microsoft.Win32.OpenFileDialog()
                                {
                                    Title = LocalizationManager.GetString("ExtKp_Dlg_SelectPreviousVersionFile"),
                                    Filter = "(*.xlsm)|*.xlsm",
                                    RestoreDirectory = true,
                                    InitialDirectory = exportDefaultPath
                                };

                                if (dialog.ShowDialog().GetValueOrDefault())
                                    previousVersionFile = dialog.FileNames.FirstOrDefault();
                            }

                            this.TraceDebug("exporter.Export()");

                            var nomFichierFinal_FileVersion = exporter.Export(tempExcelExportPath, previousVersionFile, ExcelExportDirectory, settingsService, VideoExportDirectory);
                            CheckCancellation();

                            var dossierFinal = nomFichierFinal_FileVersion.nomFichierFinal.Split('\\')[0];
                            var excelFileName = $"{nomFichierFinal_FileVersion.nomFichierFinal.Split('\\')[1]}";

                            ExcelExportDirectory = Path.Combine(ExcelExportDirectory, dossierFinal);

                            if (settings?.ArchivageIsEnabled == true)
                            {
                                if (string.IsNullOrWhiteSpace(VideoExportDirectory))
                                    VideoExportDirectory = Path.Combine(
                                        ExcelExportDirectory,
                                        $"{(settings?.ExcelExportIsEnabled == true ? "v" : string.Empty)}{nomFichierFinal_FileVersion.FileVersion}",
                                        Exporter._VIDEOS_DIRECTORY_NAME);
                                else
                                    VideoExportDirectory = Path.Combine(
                                        VideoExportDirectory,
                                        dossierFinal,
                                        $"{(settings?.ExcelExportIsEnabled == true ? "v" : string.Empty)}{nomFichierFinal_FileVersion.FileVersion}",
                                        Exporter._VIDEOS_DIRECTORY_NAME);
                            }
                            else
                                VideoExportDirectory = Path.Combine(ExcelExportDirectory, Exporter._VIDEOS_DIRECTORY_NAME);

                            this.TraceDebug("Finalizing excel process...");
                            //exporter.FinalizeExcel();
                            CheckCancellation();

                            this.TraceDebug("exporter.Package()");
                            //zipFilePath = Path.Combine(zipFileDirectory, string.Format("{0}.zip", fileName));
                            exporter.Package(ExcelExportDirectory, excelFileName, VideoExportDirectory, multiThreadVideoEncoding);

                            _errorCount = exporter.ErrorCount;

                            if (settings?.ArchivageIsEnabled == true && settings?.RedistribuableIsEnabled == true)
                            {
                                string archiveFolder = ExcelExportDirectory;
                                if (settings?.ExcelExportIsEnabled == false && !string.IsNullOrEmpty(settings?.DefaultExportVideoDirectory))
                                {
                                    var paths = VideoExportDirectory.Split('\\').ToList();
                                    paths.RemoveAt(paths.Count - 1);
                                    paths.RemoveAt(paths.Count - 1);
                                    archiveFolder = Path.Combine(paths.ToArray());
                                }
                                return (archiveFolder, Path.Combine(settings?.DefaultExportDirectory_Redistribuable, $"{dossierFinal} - {(settings?.ExcelExportIsEnabled == true ? "v" : string.Empty)}{nomFichierFinal_FileVersion.FileVersion}"));
                            }
                            else if (settings?.ArchivageIsEnabled == true)
                                return (ExcelExportDirectory, null);
                            else
                                return (null, ExcelExportDirectory);
                        }
                    }
                    else
                        throw new OperationCanceledException(_MODEL_FILE_NOT_EXISTS,
                            new Exception(string.Format(LocalizationManager.GetString("ExtKp_Error_ExcelModelPath"), modelFilePath ?? LocalizationManager.GetString("ExtKp_Error_Undefined"))));
                }
                else
                    throw new OperationCanceledException();
            }
            catch (Exception e)
            {
                if (e.InnerException is OperationCanceledException)
                    throw e.InnerException;
                else
                    throw;
            }
            finally
            {
                KprocessExportWindow.StopCurrent();
            }
        }

        private void CheckCancellation()
        {
            if (KprocessExportWindow.CancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();
        }

        /// <summary>
        /// Appelé lorsqu'une erreur a lieu.
        /// </summary>
        /// <param name="ex">L'exception.</param>
        private void OnError(Exception ex)
        {
            ServiceBus.Get<ISpinnerService>().HideDecrement();

            this.TraceError(ex, ex.Message);

            if (ex is OperationCanceledException
                || ex?.InnerException is OperationCanceledException
                || (ex is AggregateException && ((AggregateException)ex).InnerExceptions
                        .Union(((AggregateException)ex).InnerExceptions.SelectMany(_ => (_ as AggregateException)?.InnerExceptions))
                        .OfType<OperationCanceledException>().Any()))
                DialogFactory.GetDialogView<IMessageDialog>().Show(LocalizationManager.GetString("ExtKp_Dlg_ExportCanceled"), string.Empty);
            else
                DialogFactory.GetDialogView<IErrorDialog>().Show(LocalizationManager.GetString("Common_Error_GenericMessage"), LocalizationManager.GetString("Common_Error"), ex);
        }

        /// <summary>
        /// Vérifie que l'extension soit bien paramétrée.
        /// </summary>
        /// <returns><c>true</c> si l'extension est bien paramétrée.</returns>
        private bool VerifySettings()
        {
            var settingsService = ServiceBus.Get<ISettingsService>();
            var settings = settingsService.LoadExtensionApplicationSettings<Settings>(KprocessExtension.Id) ?? Settings.GetDefault();

            var hasSetModelFilePath = !string.IsNullOrEmpty(settings.ExportExcelModelPath);

            if (!hasSetModelFilePath)
            {
                DialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("ExtKp_Error_MustConfigure"),
                    LocalizationManager.GetString("Common_Error"),
                    image: MessageDialogImage.Error);

                ServiceBus.Get<INavigationService>().TryNavigate(KnownMenus.Extensions, KnownMenus.ExtensionsConfiguration);

                return false;
            }

            return true;
        }
    }
}
