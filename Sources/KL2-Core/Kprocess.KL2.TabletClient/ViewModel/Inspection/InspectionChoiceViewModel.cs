using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Kprocess.KL2.FileTransfer;
using Kprocess.KL2.TabletClient.Extensions;
using Kprocess.KL2.TabletClient.Flyouts;
using Kprocess.KL2.TabletClient.Models;
using KProcess;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Security;
using MoreLinq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
    public class InspectionChoiceViewModel : ViewModelBase, ITempPublication
    {
        public InspectionChoiceViewModel()
        {
            if (Locator.Main.TrainingPublication != null && Locator.Main.TrainingPublication.Formation_Disposition == null)
                Locator.Main.TrainingPublication = null;
            if (Locator.Main.EvaluationPublication != null && Locator.Main.EvaluationPublication.Evaluation_Disposition == null)
                Locator.Main.EvaluationPublication = null;
            if (Locator.Main.InspectionPublication != null && Locator.Main.InspectionPublication.Inspection_Disposition == null)
                Locator.Main.InspectionPublication = null;
            Application.Current.Dispatcher.Invoke(async () =>
            {
                TreeviewIsLoading = true;
                Locator.Main.ShowDisconnectedMessage = false;
                try
                {
                    Nodes = await PublishModeEnum.Inspection.ReloadAllPublications();
                }
                catch (Exception ex)
                {
                    Locator.TraceManager.TraceError(ex, "Erreur lors du chargement des publications");
                    // TODO : Une icone à la place du treeview pour retenter le chargement
                }
                finally
                {
                    TreeviewIsLoading = false;
                }
            });
        }

        public bool IsOnline =>
            Locator.APIManager.IsOnline ?? false;

        public Inspection LastClosedInspection =>
            InspectionPublication?.Inspections
                .Where(_ => _.IsScheduled != true && _.EndDate != null)
                .MaxBy(_ => _.EndDate)
                .FirstOrDefault();

        public Inspection LastOpenedInspection =>
            InspectionPublication?.Inspections
                    .Where(_ => _.IsScheduled != true && _.EndDate == null)
                    .MaxBy(_ => _.StartDate)
                    .FirstOrDefault();

        BulkObservableCollection<INode> _nodes;
        public BulkObservableCollection<INode> Nodes
        {
            get => _nodes;
            set
            {
                if (_nodes != value)
                {
                    _nodes = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool CurrentNodeIsChanging = false;

        INode _currentNode;
        public INode CurrentNode
        {
            get { return _currentNode; }
            set
            {
                if (_currentNode == value)
                    return;
                _currentNode = value;
                RaisePropertyChanged();
            }
        }

        public async Task<TaskResult> OnCurrentNodeChanged(Procedure process)
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;
            TaskResult loadingResult = TaskResult.Nok;

            if (Locator.APIManager.IsOnline == true)
            {
                try
                {
                    // Sauvegarde de la précédente publication  s'il n'y avait pas de réseau
                    var lastTraining = await OfflineFile.Training.GetFromJson<Publication>();
                    var lastEvaluation = await OfflineFile.Evaluation.GetFromJson<Publication>();
                    var lastInspection = await OfflineFile.Inspection.GetFromJson<Publication>();
                    await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, lastTraining, false);
                    await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, lastEvaluation, false);
                    await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, lastInspection, false);
                    loadingResult = await SetPublications(await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(process.ProcessId, (int)PublishModeEnum.Formation),
                        await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(process.ProcessId, (int)PublishModeEnum.Evaluation),
                        await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(process.ProcessId, (int)PublishModeEnum.Inspection));

                    if (loadingResult == TaskResult.Ok)
                    {
                        OfflineFile.Training.SaveToJson(TrainingPublication);
                        OfflineFile.Evaluation.SaveToJson(EvaluationPublication);
                        OfflineFile.Inspection.SaveToJson(InspectionPublication);
                    }

                    FlyoutIsOpen = false;
                }
                catch (Exception e)
                {
                    //TODO : Afficher un message d'erreur indiquant que la publication n'a pu être synchronisée ou chargée
                    Locator.TraceManager.TraceError(e, "Une erreur s'est produite lors de la récupération de la dernière publication");
                    await Locator.Main.ShowErrorPublicationChange(e);
                    Locator.Main.IsLoading = false;
                }
            }
            else
            {
                loadingResult = await SetPublications(await OfflineFile.Training.GetFromJson<Publication>(),
                    await OfflineFile.Evaluation.GetFromJson<Publication>(),
                    await OfflineFile.Inspection.GetFromJson<Publication>());
            }
            CurrentNodeIsChanging = false;
            return loadingResult;
        }

        Task<TaskResult> SetPublications(Publication trainingPublication, Publication evaluationPublication, Publication inspectionPublication)
        {
            Publication oldTrainingPublication = _tempCurrentTrainingPublication;
            Publication oldEvaluationPublication = _tempCurrentEvaluationPublication;
            Publication oldInspectionPublication = _tempCurrentInspectionPublication;
            TempCurrentTrainingPublication = trainingPublication;
            TempCurrentEvaluationPublication = evaluationPublication;
            TempCurrentInspectionPublication = inspectionPublication;
            if (oldTrainingPublication != _tempCurrentTrainingPublication
                || oldEvaluationPublication != _tempCurrentEvaluationPublication
                || oldInspectionPublication != _tempCurrentInspectionPublication)
                return OnTempCurrentPublicationsChanged(oldTrainingPublication, _tempCurrentTrainingPublication,
                    oldEvaluationPublication, _tempCurrentEvaluationPublication,
                    oldInspectionPublication, _tempCurrentInspectionPublication);
            return Task.FromResult(TaskResult.Ok);
        }

        Publication _tempCurrentTrainingPublication;
        public Publication TempCurrentTrainingPublication
        {
            get => _tempCurrentTrainingPublication;
            set
            {
                if (_tempCurrentTrainingPublication == value)
                    return;
                _tempCurrentTrainingPublication = value;
                RaisePropertyChanged();
            }
        }

        Publication _tempCurrentEvaluationPublication;
        public Publication TempCurrentEvaluationPublication
        {
            get => _tempCurrentEvaluationPublication;
            set
            {
                if (_tempCurrentEvaluationPublication == value)
                    return;
                _tempCurrentEvaluationPublication = value;
                RaisePropertyChanged();
            }
        }

        Publication _tempCurrentInspectionPublication;
        public Publication TempCurrentInspectionPublication
        {
            get => _tempCurrentInspectionPublication;
            set
            {
                if (_tempCurrentInspectionPublication == value)
                    return;
                _tempCurrentInspectionPublication = value;
                RaisePropertyChanged();
            }
        }

        public bool PublicationIsNull =>
            InspectionPublication == null;

        async Task<TaskResult> OnTempCurrentPublicationsChanged(Publication oldTrainingPublication, Publication newTrainingPublication,
            Publication oldEvaluationPublication, Publication newEvaluationPublication,
            Publication oldInspectionPublication, Publication newInspectionPublication)
        {
            var dummyTraining = oldTrainingPublication;
            var dummyEvaluation = oldEvaluationPublication;
            var dummyInspection = oldInspectionPublication;
            if (newInspectionPublication == null)
                return TaskResult.Nok;

            var updatePublicationsResult = await PublicationExtensions.UpdatePublications(new[] { newTrainingPublication, newEvaluationPublication, newInspectionPublication }, true);
            if (updatePublicationsResult == TaskResult.Cancelled)
            {
                Locator.Main.IsLoading = false;
                if (PublicationIsNull)
                {
                    FlyoutIsOpen = false;
                    await Locator.Navigation.Pop();
                }
                return updatePublicationsResult;
            }
            else if (updatePublicationsResult == TaskResult.Nok)
            {
                Locator.Main.LoadingText = "Une erreur s'est produite lors du téléchargement des fichiers";
                await Task.Delay(TimeSpan.FromSeconds(2));
                Locator.Main.IsLoading = false;
                return updatePublicationsResult;
            }

            TrainingPublication = newTrainingPublication;
            EvaluationPublication = newEvaluationPublication;
            InspectionPublication = newInspectionPublication;

            return updatePublicationsResult;
        }

        bool _treeviewIsLoading;
        public bool TreeviewIsLoading
        {
            get => _treeviewIsLoading;
            private set
            {
                if (_treeviewIsLoading != value)
                {
                    _treeviewIsLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher un message à l'utilisateur pour lui indiquer qu'il doit se connecter pour télécharger un process
        /// </summary>
        bool _showMessageDownloadProcess;
        public bool ShowMessageDownloadProcess
        {
            get => _showMessageDownloadProcess;
            private set
            {
                if (_showMessageDownloadProcess != value)
                {
                    _showMessageDownloadProcess = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _flyoutIsOpen;
        public bool FlyoutIsOpen
        {
            get => _flyoutIsOpen;
            set
            {
                if (_flyoutIsOpen != value)
                {
                    _flyoutIsOpen = value;
                    if (_flyoutIsOpen)
                    {

                    }
                    else
                    {
                        Locator.Main.Flyouts.Clear();
                    }
                    DispatcherHelper.CheckBeginInvokeOnUI(() => RaisePropertyChanged(nameof(FlyoutIsOpen)));
                }
            }
        }

        public Publication TrainingPublication
        {
            get => Locator.Main.TrainingPublication;
            set
            {
                if (Locator.Main.TrainingPublication != value)
                {
                    Locator.Main.TrainingPublication = value;
                    RaisePropertyChanged();
                }

                ((RelayCommand<Type>)NavigateTo).RaiseCanExecuteChanged();

                Locator.Main.IsLoading = false;
            }
        }

        public Publication EvaluationPublication
        {
            get => Locator.Main.EvaluationPublication;
            set
            {
                if (Locator.Main.EvaluationPublication != value)
                {
                    Locator.Main.EvaluationPublication = value;
                    RaisePropertyChanged();
                }

                ((RelayCommand<Type>)NavigateTo).RaiseCanExecuteChanged();

                Locator.Main.IsLoading = false;
            }
        }

        public Publication InspectionPublication
        {
            get => Locator.Main.InspectionPublication;
            set
            {
                if (Locator.Main.InspectionPublication != value)
                {
                    Locator.Main.InspectionPublication = value;
                    RaisePropertyChanged();
                }

                RaisePropertyChanged(nameof(LastClosedInspection));
                RaisePropertyChanged(nameof(LastOpenedInspection));

                ((RelayCommand<Type>)NavigateTo).RaiseCanExecuteChanged();
                ((RelayCommand)NavigateToLastInspection).RaiseCanExecuteChanged();

                Locator.Main.IsLoading = false;
            }
        }

        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommand<object>(async (o) =>
                    {
                        if (Locator.APIManager.IsOnline == true)
                        {
                            ShowMessageDownloadProcess = false;

                            if (InspectionPublication == null)
                                ((RelayCommand)ShowSelectPublicationFlyout).Execute(null);
                        }
                        else
                        {
                            CurrentNode = (await OfflineFile.Inspection.GetFromJson<Publication>())?.Process;

                            if (CurrentNode == null)
                            {
                                Locator.Main.IsLoading = false;

                                // On est hors ligne et aucun process n'est présent dans le fichier
                                ShowMessageDownloadProcess = true;
                            }
                        }
                        /*Locator.Main.IsLoading = true;

                        try
                        {
                            Nodes = await PublishModeEnum.Inspection.ReloadAllPublications();
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors du chargement des publications");
                            // TODO : Une icone à la place du treeview pour retenter le chargement
                        }

                        if (Publication != null && ((Publication.PublishMode & PublishModeEnum.Inspection) != PublishModeEnum.Inspection || Publication.Inspection_Disposition == null))
                        {
                            IoC.Resolve<FileTransferManager>().CleanSyncFiles();
                            // Save publication if we weren't online
                            try
                            {
                                if (Locator.APIManager.IsOnline == true)
                                    await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                                else
                                    await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                            }
                            catch (Exception e)
                            {
                                Locator.TraceManager.TraceDebug(e, e.Message);
                            }
                            Publication = null;
                            OfflineFile.Inspection.SaveToJson<Publication>(null);
                            CurrentNode = null;
                        }

                        if (Locator.APIManager.IsOnline == true)
                        {
                            ShowMessageDownloadProcess = false;

                            if (Publication == null)
                            {
                                Locator.Main.IsLoading = false;
                                ((RelayCommand) ShowSelectPublicationFlyout).Execute(null);
                            }
                            else
                            {
                                CurrentNode = TreeViewHelper.FindProcess(Nodes.FirstOrDefault(), Publication.ProcessId);
                                CurrentNode.IsSelected = true;
                                ExpandToProcess(CurrentNode as Procedure);
                            }
                        }
                        else
                        {
                            var currentPublicationProcessId = (await OfflineFile.Inspection.GetFromJson<Publication>())?.ProcessId;
                            CurrentNode = currentPublicationProcessId.HasValue ? TreeViewHelper.FindProcess(Nodes.FirstOrDefault(), currentPublicationProcessId.Value) : null;

                            // We are offline and no process is present in JSON publication file
                            ShowMessageDownloadProcess = CurrentNode == null;
                        }
                        LastClosedInspection = Publication?.Inspections.Where(_ => _.IsScheduled != true && _.EndDate != null).MaxBy(_ => _.EndDate).FirstOrDefault();
                        LastOpenedInspection = Publication?.Inspections.Where(_ => _.IsScheduled != true && _.EndDate == null).MaxBy(_ => _.StartDate).FirstOrDefault();*/
                    });
                return _loadedCommand;
            }
        }

        ICommand _showSelectPublicationFlyout;
        public ICommand ShowSelectPublicationFlyout
        {
            get
            {
                if (_showSelectPublicationFlyout == null)
                    _showSelectPublicationFlyout = new RelayCommand(() =>
                    {
                        Locator.GetFlyout<SelectInspection>(this);
                        FlyoutIsOpen = true;
                    }, () =>
                    {
                        return true;
                    });
                return _showSelectPublicationFlyout;
            }
        }

        ICommand _navigateToHome;
        public ICommand NavigateToHome
        {
            get
            {
                if (_navigateToHome == null)
                    _navigateToHome = new RelayCommand(() =>
                    {
                        FlyoutIsOpen = false;
                        if (Locator.Main.NavigateToHome.CanExecute(null))
                            Locator.Main.NavigateToHome.Execute(null);
                    });
                return _navigateToHome;
            }
        }

        ICommand _navigateTo;
        public ICommand NavigateTo
        {
            get
            {
                if (_navigateTo == null)
                    _navigateTo = new RelayCommand<Type>(async vmType =>
                    {
                        if (vmType == typeof(SelectInspectionSummaryViewModel))
                        {
                            var viewModel = new SelectInspectionSummaryViewModel
                            {
                                Publication = InspectionPublication
                            };
                            await Locator.Navigation.Push<Views.SelectInspectionSummary, SelectInspectionSummaryViewModel>(viewModel);
                        }
                        else if (vmType == typeof(InspectionViewModel))
                        {
                            Locator.Main.IsLoading = true;
                            Locator.Main.ShowDisconnectedMessage = false;

                            try
                            {
                                //Publication = await Locator.APIManager.SyncPublication(Publication);

                                Locator.Main.Inspection = ManageInspection(InspectionPublication);

                                var viewModel = new InspectionViewModel
                                {
                                    IndexParent = null,
                                    Index = 0
                                };
                                viewModel.UpdateInspectionParts();
                                await Locator.Navigation.Push<Views.Inspection, InspectionViewModel>(viewModel);

                                // Start directly on first task if no task are already inspected
                                var lastOpenedInspection = InspectionPublication?.Inspections.Where(_ => _.IsScheduled != true && _.EndDate == null).MaxBy(_ => _.StartDate).FirstOrDefault();
                                if (lastOpenedInspection?.InspectionSteps.Where(_ => _.LinkedInspection == null).All(_ => _.IsOk == null) == true &&
                                    lastOpenedInspection?.InspectionSteps.Where(_ => _.LinkedInspection != null).All(_ => _.LinkedInspection.InspectionSteps.All(i => i.IsOk == null)) == true)
                                {
                                    PublishedAction nextPublishedAction = InspectionPublication.PublishedActions[viewModel.Index];
                                    while (nextPublishedAction.IsGroup) // The action is a group
                                        nextPublishedAction = InspectionPublication.PublishedActions[++viewModel.Index];
                                    if (nextPublishedAction.LinkedPublication != null) // The action is a sub process
                                    {
                                        viewModel.IndexParent = viewModel.Index;
                                        viewModel.Index = 0;
                                        nextPublishedAction = InspectionPublication.PublishedActions[viewModel.IndexParent.Value].LinkedPublication.PublishedActions[viewModel.Index];
                                        while (nextPublishedAction.IsGroup) // The action is a group
                                            nextPublishedAction = InspectionPublication.PublishedActions[viewModel.IndexParent.Value].LinkedPublication.PublishedActions[++viewModel.Index];
                                    }
                                    viewModel.ShowStepCommand.Execute(nextPublishedAction);
                                }
                            }
                            catch (Exception ex)
                            {
                                Locator.TraceManager.TraceError(ex, "Erreur lors de la création de l'inspection.");
                            }
                            finally
                            {
                                Locator.Main.IsLoading = false;
                            }
                        }
                    }, vmType =>
                    {
                        if (Locator.Main.InspectionPublication == null)
                            return false;
                        if (vmType == typeof(SelectInspectionSummaryViewModel))
                            return Screen.Inspection.CanBeReadBy(Locator.Main.SelectedUser);
                        if (vmType == typeof(InspectionViewModel))
                            return Screen.Inspection.CanBeWrittenBy(Locator.Main.SelectedUser);
                        return false;
                    });
                return _navigateTo;
            }
        }

        ICommand _navigateToLastInspection;
        public ICommand NavigateToLastInspection
        {
            get
            {
                if (_navigateToLastInspection == null)
                    _navigateToLastInspection = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            Locator.Main.Inspection = InspectionPublication.Inspections
                                .Where(_ => _.EndDate.HasValue)
                                .MaxBy(_ => _.EndDate)
                                .First();

                            var viewModel = new InspectionViewModel
                            {
                                IsReadOnly = true,
                                IndexParent = null,
                                Index = 0
                            };
                            viewModel.UpdateInspectionParts();
                            await Locator.Navigation.Push<Views.Inspection, InspectionViewModel>(viewModel);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la création de l'inspection.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }
                    }, () =>
                    {
                        return InspectionPublication != null && LastClosedInspection != null;
                    });
                return _navigateToLastInspection;
            }
        }

        ICommand _returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.Pop();
                    });
                return _returnCommand;
            }
        }

        /// <summary>
        /// Crée les inspections si elles n'existent pas
        /// </summary>
        /// <param name="publication"></param>
        /// <returns>True si des modifications ont été faites</returns>
        Inspection ManageInspection(Publication publication)
        {
            Inspection lastOpenedInspection = publication.Inspections
                .Where(q => !q.IsDeleted && q.EndDate == null && q.IsScheduled != true)
                .MaxBy(i => i.StartDate)
                .FirstOrDefault();
            if (lastOpenedInspection == null)
            {
                // On crée l'inspection
                lastOpenedInspection = new Inspection()
                {
                    PublicationId = InspectionPublication.PublicationId,
                    StartDate = DateTime.Now
                };
                // On crée les InspectionSteps
                publication.PublishedActions
                    .Where(_ => !_.IsGroup)
                    .ForEach(publishedAction =>
                    {
                        var inspectionStep = new InspectionStep
                        {
                            Date = lastOpenedInspection.StartDate,
                            PublishedActionId = publishedAction.PublishedActionId,
                            InspectorId = Locator.Main.SelectedUser.UserId
                        };
                        if (publishedAction.LinkedPublication != null)
                        {
                            // S'il existe une inspection qui est encore ouverte, on la ferme
                            var lastOpenedInnerInspection = publishedAction.LinkedPublication.Inspections
                                .Where(i => !i.IsDeleted && i.EndDate == null && i.IsScheduled != true)
                                .MaxBy(i => i.StartDate)
                                .FirstOrDefault();
                            if (lastOpenedInnerInspection != null)
                                lastOpenedInnerInspection.EndDate = lastOpenedInspection.StartDate;

                            lastOpenedInnerInspection = new Inspection()
                            {
                                PublicationId = publishedAction.LinkedPublication.PublicationId,
                                StartDate = lastOpenedInspection.StartDate
                            };
                            publishedAction.LinkedPublication.PublishedActions
                                .Where(_ => !_.IsGroup)
                                .ForEach(innerPublishedAction =>
                                {
                                    var innerInspectionStep = new InspectionStep
                                    {
                                        Date = lastOpenedInspection.StartDate,
                                        PublishedActionId = innerPublishedAction.PublishedActionId,
                                        InspectorId = Locator.Main.SelectedUser.UserId
                                    };
                                    lastOpenedInnerInspection.InspectionSteps.Add(innerInspectionStep);
                                });
                            inspectionStep.LinkedInspection = lastOpenedInnerInspection;
                            publishedAction.LinkedPublication.Inspections.Add(lastOpenedInnerInspection);
                        }
                        lastOpenedInspection.InspectionSteps.Add(inspectionStep);
                    });

                publication.Inspections.Add(lastOpenedInspection);
            }
            return lastOpenedInspection;
        }

        void ExpandToProcess(Procedure process)
        {
            ProjectDir parentFolder = process.ProjectDir;
            while (parentFolder != null)
            {
                parentFolder.IsExpanded = true;
                parentFolder = parentFolder.Parent;
            }
        }
    }
}