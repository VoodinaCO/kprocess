using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Extensions;
using KProcess.KL2.Business.Impl.Helpers;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.FileTransfer;
using KProcess.Ksmed.Security;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class InspectionScheduledViewModel : ViewModelBase, ITempPublication
    {
        #region Properties

        TrackableCollection<InspectionSchedule> _inspectionScheduleCollection;
        public TrackableCollection<InspectionSchedule> InspectionScheduleCollection
        {
            get => _inspectionScheduleCollection;
            set
            {
                if (_inspectionScheduleCollection == value)
                    return;
                _inspectionScheduleCollection = value;
                RaisePropertyChanged();
            }
        }

        bool _isFromAudit;
        public bool IsFromAudit
        {
            get => _isFromAudit;
            set
            {
                if (_isFromAudit != value)
                {
                    _isFromAudit = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _isReadOnly;
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    RaisePropertyChanged();
                }
            }
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

                Locator.Main.IsLoading = false;
            }
        }

        #endregion

        #region Commands

        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(async () =>
                {
                    Locator.Main.IsLoading = true;

                    IEnumerable<InspectionSchedule> result;

                    if (Locator.APIManager.IsOnline == true)
                    {
                        Locator.Main.ShowDisconnectedMessage = true;
                        result = await Locator.GetService<IPrepareService>().GetInspectionSchedules();
                    }
                    else
                    {
                        Locator.Main.ShowDisconnectedMessage = false;
                        var tempResult = new ConcurrentBag<InspectionSchedule>();
                        var now = DateTime.Now;
                        var currentTime = now.TimeOfDay;
                        var currentDate = now.Date;

                        var inspectionSchedules = InspectionPublication?.Process?.InspectionSchedules
                            .Where(_ => !_.Timeslot.IsDeleted);

                        foreach (var inspectionSchedule in inspectionSchedules)
                        {
                            var recurrences = RecurrenceHelper.GetRecurrenceDateTimeCollection(inspectionSchedule, currentDate, currentDate.AddDays(1));
                            var timeSlots = recurrences.GetRecurrenceTimeSlotCollection(inspectionSchedule.Timeslot);
                            var currentTimeSlot = timeSlots.Contains(now);
                            if (currentTimeSlot != null)
                            {
                                var existentInspection = InspectionPublication.Inspections
                                    .SingleOrDefault(_ => InspectionPublication.ProcessId == inspectionSchedule.ProcessId
                                                          && _.IsScheduled == true
                                                          && currentTimeSlot.StartDateTime <= _.StartDate
                                                          && _.StartDate < currentTimeSlot.EndDateTime);
                                inspectionSchedule.IsClosed = existentInspection?.EndDate != null;
                                tempResult.Add(inspectionSchedule);
                            }
                        }

                        result = tempResult;
                    }

                    InspectionScheduleCollection = new TrackableCollection<InspectionSchedule>(result);

                    Locator.Main.IsLoading = false;
                }));
            }
        }

        /// <summary>
        /// Obtient la commande permettant de revenir en arrière
        /// </summary>
        ICommand _returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                return _returnCommand ?? (_returnCommand = new RelayCommand(async () =>
                {
                    await Locator.Navigation.Pop();
                }));
            }
        }

        ICommand _selectInspectionCommand;
        public ICommand SelectInspectionCommand =>
            _selectInspectionCommand ?? (_selectInspectionCommand = new RelayCommand<InspectionSchedule>(async item =>
            {
                Locator.Main.IsLoading = true;
                TaskResult loadingResult = TaskResult.Nok;

                if (Locator.APIManager.IsOnline == true)
                {
                    Locator.Main.ShowDisconnectedMessage = true;
                    try
                    {
                        // Sauvegarde de la précédente publication  s'il n'y avait pas de réseau
                        var lastTraining = await OfflineFile.Training.GetFromJson<Publication>();
                        var lastEvaluation = await OfflineFile.Evaluation.GetFromJson<Publication>();
                        var lastInspection = await OfflineFile.Inspection.GetFromJson<Publication>();
                        await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, lastTraining, false);
                        await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, lastEvaluation, false);
                        await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, lastInspection, false);
                        loadingResult = await SetPublications(await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(item.ProcessId, (int)PublishModeEnum.Formation),
                            await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(item.ProcessId, (int)PublishModeEnum.Evaluation),
                            await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(item.ProcessId, (int)PublishModeEnum.Inspection));

                        if (loadingResult == TaskResult.Ok)
                        {
                            await UpdatePublication(InspectionPublication, item);
                            OfflineFile.Training.SaveToJson(TrainingPublication);
                            OfflineFile.Evaluation.SaveToJson(EvaluationPublication);
                            OfflineFile.Inspection.SaveToJson(InspectionPublication);
                        }
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
                    Locator.Main.ShowDisconnectedMessage = false;
                    loadingResult = await SetPublications(await OfflineFile.Training.GetFromJson<Publication>(),
                        await OfflineFile.Evaluation.GetFromJson<Publication>(),
                        await OfflineFile.Inspection.GetFromJson<Publication>());
                }

                /*try
                {
                    var oldPublicationId = InspectionPublication?.PublicationId;
                    try
                    {
                        if (Locator.APIManager.IsOnline == true)
                        {
                            InspectionPublication = await Locator.GetService<IPrepareService>().GetLastPublicationFiltered(item.ProcessId, (int)PublishModeEnum.Inspection);
                            InspectionPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, InspectionPublication);
                        }
                        else
                        {
                            InspectionPublication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, InspectionPublication);
                        }
                    }
                    catch (Exception e)
                    {
                        Locator.TraceManager.TraceDebug(e, e.Message);
                    }

                    if (oldPublicationId != InspectionPublication?.PublicationId)
                    {
                        if (await InspectionPublication.UpdatePublication(true) == TaskResult.Nok)
                        {
                            Locator.Main.LoadingText = "Une erreur s'est produite lors du téléchargement des fichiers";
                            await Task.Delay(TimeSpan.FromSeconds(2));
                            Locator.Main.IsLoading = false;
                            return;
                        }
                    }

                    await UpdatePublication(InspectionPublication, item);
                }
                catch (Exception ex)
                {
                    Locator.TraceManager.TraceError(ex, "Erreur lors de la création de l'inspection.");
                }
                finally
                {
                    Locator.Main.IsLoading = false;
                }*/
            }, item => 
            {
                if (item == null)
                    return false;
                bool canInspect = true;
                var currentUser = Locator.Main.SelectedUser;
                if (currentUser.Roles.Any(r => r.RoleCode == KnownRoles.Administrator))
                    canInspect = true;
                else if (currentUser.Roles.Any(r => r.RoleCode == KnownRoles.Supervisor))
                    canInspect = false;
                return canInspect;
            }));

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
                    Locator.Main.Flyouts.Clear();
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

        #endregion

        #region Methods

        async Task UpdatePublication(Publication publication, InspectionSchedule inspectionSchedule)
        {
            Locator.Main.Inspection = ManageInspection(publication, inspectionSchedule);

            var viewModel = new InspectionViewModel
            {
                IndexParent = null,
                Index = 0
            };
            viewModel.UpdateInspectionParts();

            await Locator.Navigation.Push<Views.Inspection, InspectionViewModel>(viewModel);

            // On démarre directement sur la première tâche, si aucune tâche n'est encore inspectée
            if (Locator.Main.Inspection.InspectionSteps
                    .Where(_ => _.LinkedInspection == null)
                    .All(_ => _.IsOk == null) 
                && Locator.Main.Inspection.InspectionSteps
                    .Where(_ => _.LinkedInspection != null)
                    .All(_ => _.LinkedInspection.InspectionSteps.All(i => i.IsOk == null)))
            {
                var nextPublishedAction = publication.PublishedActions[viewModel.Index];
                while (nextPublishedAction.IsGroup) // L'action est un groupe
                    nextPublishedAction = publication.PublishedActions[++viewModel.Index];
                if (nextPublishedAction.LinkedPublication != null) // L'action est un sous-process
                {
                    viewModel.IndexParent = viewModel.Index;
                    viewModel.Index = 0;
                    nextPublishedAction = publication.PublishedActions[viewModel.IndexParent.Value].LinkedPublication.PublishedActions[viewModel.Index];
                    while (nextPublishedAction.IsGroup) // L'action est un groupe
                        nextPublishedAction = publication.PublishedActions[viewModel.IndexParent.Value].LinkedPublication.PublishedActions[++viewModel.Index];
                }
                viewModel.ShowStepCommand.Execute(nextPublishedAction);
            }
        }


        /// <summary>
        /// Crée les inspections si elles n'existent pas
        /// </summary>
        /// <param name="publication"></param>
        /// <returns>True si des modifications ont été faites</returns>
        Inspection ManageInspection(Publication publication, InspectionSchedule inspectionSchedule)
        {
            var now = DateTime.Now;
            var currentTime = now.TimeOfDay;
            var currentDate = now.Date;

            var recurrences = RecurrenceHelper.GetRecurrenceDateTimeCollection(inspectionSchedule, currentDate, currentDate.AddDays(1));
            var timeSlots = recurrences.GetRecurrenceTimeSlotCollection(inspectionSchedule.Timeslot);
            var currentTimeSlot = timeSlots.Contains(now);
            var scheduledInspections = publication.Inspections
                .Where(_ => publication.ProcessId == inspectionSchedule.ProcessId
                            && _.IsScheduled == true
                            && currentTimeSlot.StartDateTime <= _.StartDate
                            && _.StartDate < currentTimeSlot.EndDateTime);
            if (scheduledInspections.Count() > 1)
            {
                Locator.TraceManager.TraceError($"More than one scheduled inspections have been found for publication {publication.Label} and timeslot {inspectionSchedule.Timeslot.Label}");
                scheduledInspections = scheduledInspections.OrderByDescending(_ => _.InspectionId).ToList();
            }

            var scheduledInspection = scheduledInspections.FirstOrDefault();
            if (scheduledInspection != null)
                return scheduledInspection;

            // On crée l'inspection
            scheduledInspection = new Inspection
            {
                PublicationId = publication.PublicationId,
                StartDate = now,
                IsScheduled = true
            };
            // On crée les InspectionSteps
            publication.PublishedActions
                .Where(_ => !_.IsGroup)
                .ForEach(publishedAction =>
            {
                var inspectionStep = new InspectionStep
                {
                    Date = scheduledInspection.StartDate,
                    PublishedActionId = publishedAction.PublishedActionId,
                    InspectorId = Locator.Main.SelectedUser.UserId
                };
                // Don't take care about LinkedPublication for inspections
                /*if (publishedAction.LinkedPublication != null)
                {
                    // S'il existe une inspection qui est encore ouverte, on la ferme
                    var lastOpenedInnerInspection = publishedAction.LinkedPublication.Inspections
                        .Where(q => DateTime.Compare(q.StartDate.Date, inspectionSchedule.StartDate.Date) == 0
                                    && q.StartDate.TimeOfDay < inspectionSchedule.Timeslot.EndTime
                                    && q.StartDate.TimeOfDay >= inspectionSchedule.Timeslot.StartTime
                                    && !q.IsDeleted && q.EndDate == null && q.IsScheduled == true
                                    && q.Publication.ProcessId == inspectionSchedule.ProcessId)
                        .MaxBy(si => si.StartDate)
                        .FirstOrDefault();
                    if (lastOpenedInnerInspection != null)
                        lastOpenedInnerInspection.EndDate = scheduledInspection.StartDate;

                    lastOpenedInnerInspection = new Inspection()
                    {
                        PublicationId = publishedAction.LinkedPublication.PublicationId,
                        StartDate = scheduledInspection.StartDate
                    };
                    publishedAction.LinkedPublication.PublishedActions.Where(_ => !_.IsGroup).ForEach(innerPublishedAction =>
                    {
                        var innerInspectionStep = new InspectionStep
                        {
                            Date = scheduledInspection.StartDate,
                            PublishedActionId = innerPublishedAction.PublishedActionId,
                            InspectorId = Locator.Main.SelectedUser.UserId
                        };
                        lastOpenedInnerInspection.InspectionSteps.Add(innerInspectionStep);
                    });
                    inspectionStep.LinkedInspection = lastOpenedInnerInspection;
                    publishedAction.LinkedPublication.Inspections.Add(lastOpenedInnerInspection);
                }*/
                scheduledInspection.InspectionSteps.Add(inspectionStep);
            });

            publication.Inspections.Add(scheduledInspection);
            return scheduledInspection;
        }

        #endregion
    }
}
