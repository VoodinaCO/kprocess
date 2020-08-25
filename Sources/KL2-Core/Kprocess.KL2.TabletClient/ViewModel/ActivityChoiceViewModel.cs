using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.FileTransfer;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Extensions;
using Kprocess.KL2.TabletClient.Views;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using log4net;
using log4net.Appender;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ActivityChoiceViewModel : ViewModelBase
    {
        Task _hasAuditTask;
        Progress<bool> _hasAuditTask_Progress;
        readonly CancellationTokenSource cts = new CancellationTokenSource();

        public ActivityChoiceViewModel()
        {
            _hasAuditTask_Progress = new Progress<bool>(hasAudit =>
            {
                if (Locator.Main.InspectionPublication != null)
                {
                    Locator.Main.InspectionPublication.AuditorHaveActiveAudit = hasAudit;
                    ((RelayCommand<PublishModeEnum>)NavigateTo).RaiseCanExecuteChanged();
                }
            });
            _hasAuditTask = Task.Run(() => HasAuditTask_Work(_hasAuditTask_Progress), cts.Token);
        }

        async void HasAuditTask_Work(IProgress<bool> progress)
        {
            while(!cts.IsCancellationRequested)
            {
                try
                {
                    if (Locator.APIManager.IsOnline == true)
                    {
                        var result = Locator.Main.SelectedUser == null || !Locator.Main.IsConnected ? false : await Locator.GetService<IPrepareService>().CheckAuditorHaveActiveAudit(Locator.Main.SelectedUser.UserId);
                        progress.Report(result);
                    }
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
                catch { }
            }
        }

        public override void Cleanup()
        {
            base.Cleanup();
            cts.Cancel();
            _hasAuditTask = null;
        }

        ICommand _navigateTo;
        public ICommand NavigateTo
        {
            get
            {
                if (_navigateTo == null)
                    _navigateTo = new RelayCommand<PublishModeEnum>(async publishMode =>
                    {
                        switch (publishMode)
                        {
                            case PublishModeEnum.Formation:
                                await Locator.Navigation.Push<FormationChoice, FormationChoiceViewModel>(new FormationChoiceViewModel());
                                break;
                            case PublishModeEnum.Inspection:
                                await Locator.Navigation.Push<InspectionTypeChoice, InspectionTypeChoiceViewModel>(new InspectionTypeChoiceViewModel());
                                break;
                            case PublishModeEnum.Audit:
                                Locator.Main.IsLoading = true;
                                try
                                {
                                    // On doit tester si il existe un audit ouvert, message sinon
                                    if (Locator.APIManager.IsOnline == true)
                                    {
                                        Locator.Main.ShowDisconnectedMessage = true;
                                        if (!await Locator.GetService<IPrepareService>().CheckAuditorHaveActiveAudit(Locator.Main.SelectedUser.UserId))
                                        {
                                            await Locator.Navigation.PushDialog<NoOpenedAuditDialog, ActivityChoiceViewModel>(this);
                                            break;
                                        }
                                        Publication publication = await Locator.GetService<IPrepareService>().GetPublicationToAudit(Locator.Main.SelectedUser.UserId);
                                        if (publication.PublicationId != Locator.Main.InspectionPublication?.PublicationId)
                                        {
                                            Locator.Main.InspectionPublication = publication;
                                            if (await Locator.Main.InspectionPublication.UpdatePublication(true) == TaskResult.Nok)
                                            {
                                                Locator.Main.LoadingText = "Une erreur s'est produite lors du téléchargement des fichiers";
                                                await Task.Delay(TimeSpan.FromSeconds(2));
                                                Locator.Main.IsLoading = false;
                                                return;
                                            }
                                        }
                                        else if (publication.Inspections.SelectMany(_ => _.Audits).Count() != Locator.Main.InspectionPublication.Inspections.SelectMany(_ => _.Audits).Count())
                                        {
                                            Locator.Main.InspectionPublication = publication;
                                            await Locator.Main.InspectionPublication.UpdatePublication(false);
                                        }
                                    }
                                    else
                                    {
                                        Locator.Main.ShowDisconnectedMessage = false;
                                        if (Locator.Main.InspectionPublication == null || !Locator.Main.InspectionPublication.CheckAuditorHaveActiveAudit(Locator.Main.SelectedUser.UserId))
                                        {
                                            await Locator.Navigation.PushDialog<NoOpenedAuditDialog, ActivityChoiceViewModel>(this);
                                            break;
                                        }
                                    }
                                    var viewModel = new AuditViewModel
                                    {
                                        Publication = Locator.Main.InspectionPublication,
                                        Index = 0
                                    };
                                    // On crée les auditItems si ils n'existent pas
                                    foreach (SurveyItem surveyItem in viewModel.Audit.Survey.SurveyItems)
                                    {
                                        AuditItem auditItem = viewModel.Audit.AuditItems.SingleOrDefault(_ => _.Number == surveyItem.Number);
                                        if (auditItem == null)
                                            viewModel.Audit.AuditItems.Add(new AuditItem(surveyItem));
                                        else
                                            auditItem.SurveyItem = surveyItem;
                                    }
                                    await Locator.Navigation.Push<Views.Audit, AuditViewModel>(viewModel);
                                }
                                catch (Exception ex)
                                {
                                    Locator.TraceManager.TraceError(ex, $"Une erreur s'est produite lors du chargement de l'audit.");
                                }
                                finally
                                {
                                    Locator.Main.IsLoading = false;
                                }
                                break;
                        }
                    }, publishMode =>
                    {
                        bool canNavigate = false;
                        switch (publishMode)
                        {
                            case PublishModeEnum.Formation:
                                canNavigate = Screen.Training.CanBeReadBy(Locator.Main.SelectedUser);
                                if (Locator.APIManager.IsOnline == true)
                                    return canNavigate;
                                return canNavigate && Locator.Main.TrainingPublication?.Formation_Disposition != null;
                            case PublishModeEnum.Audit:
                                canNavigate = Screen.Audit.CanBeWrittenBy(Locator.Main.SelectedUser);
                                if (canNavigate && Locator.APIManager.IsOnline == true)
                                {
                                    if (Locator.Main.InspectionPublication != null)
                                    {
                                        return Locator.Main.InspectionPublication.AuditorHaveActiveAudit;
                                    }
                                    
                                    var t = Task.Run(async () => await Locator.GetService<IPrepareService>().CheckAuditorHaveActiveAudit(Locator.Main.SelectedUser.UserId));
                                    t.Wait();
                                    return t.Result;                                    
                                }
                                else if (canNavigate && Locator.APIManager.IsOnline != true)
                                {
                                    if (Locator.Main.InspectionPublication == null || !Locator.Main.InspectionPublication.CheckAuditorHaveActiveAudit(Locator.Main.SelectedUser.UserId))
                                        return false;
                                }
                                return canNavigate;
                            case PublishModeEnum.Inspection:
                                canNavigate = Screen.Inspection.CanBeReadBy(Locator.Main.SelectedUser);
                                if (Locator.APIManager.IsOnline == true)
                                    return canNavigate;
                                return canNavigate && Locator.Main.InspectionPublication?.Inspection_Disposition != null;
                            default:
                                return canNavigate;
                        }
                    });
                return _navigateTo;
            }
        }

        ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.PopModal();
                    });
                return _exitCommand;
            }
        }

        private (string fileName, byte[] data) GetLogInternal()
        {
            var logFileLocation = LogManager.GetRepository().GetAppenders().OfType<FileAppender>().FirstOrDefault()?.File;

            var tmpFileName = Path.GetTempFileName();
            System.IO.File.Copy(logFileLocation, tmpFileName, true);
            var fileBytes = System.IO.File.ReadAllBytes(tmpFileName);
            System.IO.File.Delete(tmpFileName);

            return (Path.GetFileName(logFileLocation), fileBytes);
        }

        ICommand _getAllLogsCommand;
        public ICommand GetAllLogsCommand
        {
            get
            {
                if (_getAllLogsCommand == null)
                {
                    _getAllLogsCommand = new RelayCommand(() =>
                    {
                        var version = Assembly.GetExecutingAssembly().FullName
                            .Split(',')
                            .Single(_ => _.Contains("Version="))
                            .Split('=')
                            .Last();
                        var dialog = new SaveFileDialog
                        {
                            CheckFileExists = false,
                            OverwritePrompt = true,
                            FileName = $"FS-log-v{version}.zip",
                            Filter = "Zip file|*.zip"
                        };

                        if (dialog.ShowDialog() ?? false)
                        {
                            var targetFileName = dialog.FileName;

                            var logFiles = new List<(string fileName, byte[] data)>
                            {
                                GetLogInternal() // FieldService
                            };
                            var apiClient = Locator.Resolve<IAPIHttpClient>();
                            var apiLog = apiClient.GetLog(KL2_Server.API);
                            if (!string.IsNullOrEmpty(apiLog.fileName) && apiLog.data != null)
                                logFiles.Add(apiLog);
                            var fileServerLog = apiClient.GetLog(KL2_Server.File);
                            if (!string.IsNullOrEmpty(fileServerLog.fileName) && fileServerLog.data != null)
                                logFiles.Add(fileServerLog);
                            //TODO : Get Logs from Sync service

                            byte[] compressedBytes;

                            using (var outStream = new MemoryStream())
                            {
                                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                                {
                                    foreach (var (fileName, data) in logFiles)
                                    {
                                        var fileInArchive = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                                        using (var entryStream = fileInArchive.Open())
                                        using (var fileToCompressStream = new MemoryStream(data))
                                        {
                                            fileToCompressStream.CopyTo(entryStream);
                                        }
                                    }
                                }
                                compressedBytes = outStream.ToArray();
                            }

                            File.WriteAllBytes(targetFileName, compressedBytes);
                        }
                    }, () => Locator.Main.SelectedUser.Roles.Any(r => new[] { KnownRoles.Administrator }.Contains(r.RoleCode)));
                }
                return _getAllLogsCommand;
            }
        }
    }
}