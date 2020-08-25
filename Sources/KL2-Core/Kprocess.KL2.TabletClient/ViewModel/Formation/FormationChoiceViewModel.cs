using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Kprocess.KL2.FileTransfer;
using Kprocess.KL2.TabletClient.Extensions;
using Kprocess.KL2.TabletClient.Flyouts;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Views;
using KProcess;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using KProcess.Ksmed.Security;
using System;
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
    public class FormationChoiceViewModel : ViewModelBase, ITempPublication
    {
        public FormationChoiceViewModel()
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
                try
                {
                    Nodes = await PublishModeEnum.Formation.ReloadAllPublications();
                }
                catch (Exception ex)
                {
                    Locator.TraceManager.TraceError(ex, "Erreur lors du chargement des publications");
                    // TODO : Une icone � la place du treeview pour retenter le chargement
                }
                finally
                {
                    TreeviewIsLoading = false;
                }
            });
        }

        public bool IsOnline =>
            Locator.APIManager.IsOnline ?? false;

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
            TaskResult loadingResult = TaskResult.Nok;

            if (Locator.APIManager.IsOnline == true)
            {
                Locator.Main.ShowDisconnectedMessage = true;
                try
                {
                    // Sauvegarde de la pr�c�dente publication  s'il n'y avait pas de r�seau
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

                    _currentNode = process;
                    FlyoutIsOpen = false;
                }
                catch (Exception e)
                {
                    //TODO : Afficher un message d'erreur indiquant que la publication n'a pu �tre synchronis�e ou charg�e
                    Locator.TraceManager.TraceError(e, "Une erreur s'est produite lors de la r�cup�ration de la derni�re publication");
                    await Locator.Main.ShowErrorPublicationChange(e);
                    Locator.Main.IsLoading = false;
                }
            }
            else
            {
                Locator.Main.ShowDisconnectedMessage = false;
                loadingResult = await SetPublications(await OfflineFile.Training.GetFromJson<Publication>(),
                    await OfflineFile.Evaluation.GetFromJson<Publication>(),
                    await OfflineFile.Inspection.GetFromJson<Publication>());
                _currentNode = process;
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
            TrainingPublication == null || EvaluationPublication == null;

        async Task<TaskResult> OnTempCurrentPublicationsChanged(Publication oldTrainingPublication, Publication newTrainingPublication,
            Publication oldEvaluationPublication, Publication newEvaluationPublication,
            Publication oldInspectionPublication, Publication newInspectionPublication)
        {
            var dummyTraining = oldTrainingPublication;
            var dummyEvaluation = oldEvaluationPublication;
            var dummyInspection = oldInspectionPublication;
            if (newTrainingPublication == null
                || newEvaluationPublication == null)
                return TaskResult.Nok;

            var updatePublicationsResult = await PublicationExtensions.UpdatePublications(new[] { newTrainingPublication, newEvaluationPublication, newInspectionPublication }, true);
            if (updatePublicationsResult == TaskResult.Cancelled)
            {
                Locator.Main.IsLoading = false;
                if (PublicationIsNull)
                    NavigateToHome.Execute(null);
                return updatePublicationsResult;
            }
            else if (updatePublicationsResult == TaskResult.Nok)
            {
                Locator.Main.LoadingText = "Une erreur s'est produite lors du t�l�chargement des fichiers";
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
        /// Obtient un bool�en indiquant si on doit afficher un message � l'utilisateur pour lui indiquer qu'il doit se connecter pour t�l�charger un process
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

                ((RelayCommand<Type>)NavigateTo).RaiseCanExecuteChanged();

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

                            if (TrainingPublication == null || EvaluationPublication == null)
                                ((RelayCommand)ShowSelectPublicationFlyout).Execute(null);
                        }
                        else
                        {
                            CurrentNode = (await OfflineFile.Training.GetFromJson<Publication>())?.Process;

                            if (CurrentNode == null)
                            {
                                Locator.Main.IsLoading = false;

                                // On est hors ligne et aucun process n'est pr�sent dans le fichier
                                ShowMessageDownloadProcess = true;
                            }
                        }
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
                        Locator.GetFlyout<SelectFormation>(this);
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
                        if (vmType == typeof(SelectFormationSummaryViewModel))
                        {
                            var viewModel = new SelectFormationSummaryViewModel
                            {
                                Publication = TrainingPublication
                            };
                            await Locator.Navigation.Push<SelectFormationSummary, SelectFormationSummaryViewModel>(viewModel);
                        }
                        else if (vmType == typeof(SelectFormationOperatorViewModel))
                        {
                            var viewModel = new SelectFormationOperatorViewModel
                            {
                                Publication = TrainingPublication
                            };
                            await Locator.Navigation.Push<SelectFormationOperators, SelectFormationOperatorViewModel>(viewModel);
                        }
                        else if (vmType == typeof(SelectQualificationOperatorViewModel))
                        {
                            var viewModel = new SelectQualificationOperatorViewModel
                            {
                                Publication = EvaluationPublication
                            };
                            await Locator.Navigation.Push<SelectQualificationOperators, SelectQualificationOperatorViewModel>(viewModel);
                        }
                    }, vmType =>
                    {
                        if (Locator.Main.TrainingPublication == null || Locator.Main.EvaluationPublication == null)
                            return false;
                        if (vmType == typeof(SelectFormationSummaryViewModel))
                            return Screen.Training.CanBeReadBy(Locator.Main.SelectedUser);
                        if (vmType == typeof(SelectFormationOperatorViewModel))
                            return Screen.Training.CanBeWrittenBy(Locator.Main.SelectedUser);
                        if (vmType == typeof(SelectQualificationOperatorViewModel))
                            return Screen.Qualification.CanBeWrittenBy(Locator.Main.SelectedUser);
                        return false;
                    });
                return _navigateTo;
            }
        }
    }
}