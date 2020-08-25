using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using MoreLinq;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class FormationViewModel : ViewModelBase, ISfDataGridViewModel<PublishedAction>, IMediaElementViewModel, IRefreshViewModel
    {
        #region Attributs

        PublishedAction _publishedAction;
        readonly DateTime _startValidationDate;

        List<UIUser> _operators = new List<UIUser>();

        bool _showSleepAction = true;
        bool _reopenParent;

        #endregion

        #region Constructor

        public FormationViewModel()
        {
            _startValidationDate = DateTime.Now;
        }

        #endregion

        #region Properties

        int? _indexParent;
        public int? IndexParent
        {
            get => _indexParent;
            set
            {
                if (_indexParent != value)
                {
                    _indexParent = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la commande permettant d'afficher les personnes sélectionnées si elles sont plusieures
        /// </summary>
        ICommand _showOperatorCommand;
        public ICommand ShowOperatorCommand
        {
            get
            {
                if (_showOperatorCommand == null)
                    _showOperatorCommand = new RelayCommand(async () =>
                    {
                        var viewModel = new OperatorsDialogViewModel
                        {
                            Publication = Publication,
                            Operators = Operators
                        };
                        await Locator.Navigation.PushDialog<OperatorsDialog, OperatorsDialogViewModel>(viewModel);
                    });

                return _showOperatorCommand;
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
                if (_returnCommand == null)
                    _returnCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;

                        try
                        {
                            if (Locator.APIManager.IsOnline == true)
                            {
                                Locator.Main.ShowDisconnectedMessage = true;
                                Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Training, Publication);
                            }
                        }
                        catch { }

                        await Locator.Navigation.Pop();

                        Locator.Main.IsLoading = false;
                    });

                return _returnCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de mettre en pause la formation
        /// </summary>
        ICommand _sleepCommand;
        public ICommand SleepCommand
        {
            get
            {
                if (_sleepCommand == null)
                    _sleepCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;

                        try
                        {
                            if (Locator.APIManager.IsOnline == true)
                            {
                                Locator.Main.ShowDisconnectedMessage = true;
                                Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Training, Publication);
                            }
                        }
                        catch { }

                        await Locator.Navigation.Pop();

                        Locator.Main.IsLoading = false;
                    });

                return _sleepCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de valider la formation
        /// </summary>
        ICommand _validateCommand;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                    _validateCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            CreateTraining();
                            OfflineFile.Training.SaveToJson(Publication);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la validation d'une étape.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }

                        RaisePropertyChanged(nameof(IsValidated));
                        RaisePropertyChanged(nameof(ShowValidateCommand));

                        if (NextVisibility != Visibility.Visible)
                        {
                            await Locator.Navigation.PopModal();
                            if (_indexParent != null)
                            {
                                _reopenParent = false;
                                PublishedAction = Publication.PublishedActions[_indexParent.Value];
                                ((RelayCommand<PublishedAction>)ShowStepCommand).Execute(PublishedAction);
                            }
                        }
                        else if (((RelayCommand)NextActionDetailsCommand).CanExecute(null))
                            ((RelayCommand)NextActionDetailsCommand).Execute(null);
                    });

                return _validateCommand;
            }
        }

        ICommand _hyperlinkRequestNavigateCommand;
        public ICommand HyperlinkRequestNavigateCommand
        {
            get
            {
                if (_hyperlinkRequestNavigateCommand == null)
                    _hyperlinkRequestNavigateCommand = new RelayCommand<RequestNavigateEventArgs>(e =>
                    {
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    });
                return _hyperlinkRequestNavigateCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de revenir en arrière
        /// </summary>
        ICommand _closeDetailsDialogCommand;
        public ICommand CloseDetailsDialogCommand
        {
            get
            {
                if (_closeDetailsDialogCommand == null)
                    _closeDetailsDialogCommand = new RelayCommand(async () =>
                    {
                        if (MediaElement != null)
                            await MediaElement.Stop();

                        await Locator.Navigation.PopModal();

                        if (_reopenParent)
                        {
                            PublishedAction nextPublishedAction = Publication.PublishedActions[_indexParent.Value];
                            ShowStepCommand.Execute(nextPublishedAction);
                            _reopenParent = false;
                        }
                        else
                        {
                            await ScrollTo(PublishedAction);
                        }
                    });
                return _closeDetailsDialogCommand;
            }
        }

        ICommand _showStepCommand;
        public ICommand ShowStepCommand
        {
            get
            {
                if (_showStepCommand == null)
                    _showStepCommand = new RelayCommand<PublishedAction>(async (publishedAction) =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            PublishedAction action = Publication.PublishedActions.FirstOrDefault(p => p.PublishedActionId == publishedAction.PublishedActionId);
                            if (action == null) // C'est un sous process
                            {
                                PublishedAction parent = Publication.PublishedActions.FirstOrDefault(p => p.LinkedPublicationId == publishedAction.PublicationId);
                                if (parent != null)
                                {
                                    action = parent.LinkedPublication.PublishedActions.FirstOrDefault(p => p.PublishedActionId == publishedAction.PublishedActionId);
                                    _indexParent = Publication.PublishedActions.IndexOf(parent);
                                    _index = parent.LinkedPublication.PublishedActions.IndexOf(action);
                                }
                            }
                            else
                            {
                                _indexParent = null;
                                _index = Publication.PublishedActions.IndexOf(action);
                            }

                            PublishedAction = action;

                            await Locator.Navigation.PushDialog<FormationActionDetailsDialog, FormationViewModel>(this);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans la formation.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }
                    });

                return _showStepCommand;
            }
        }

        ICommand _showInnerStepCommand;
        public ICommand ShowInnerStepCommand
        {
            get
            {
                if (_showInnerStepCommand == null)
                    _showInnerStepCommand = new RelayCommand(async () =>
                    {
                        int innerIndex = 0;
                        PublishedAction nextPublishedAction = Publication.PublishedActions[_index].LinkedPublication.PublishedActions[innerIndex];
                        while (nextPublishedAction.IsGroup) // L'action est un groupe
                            nextPublishedAction = Publication.PublishedActions[_index].LinkedPublication.PublishedActions[++innerIndex];

                        _reopenParent = true;
                        await Locator.Navigation.PopModal();
                        ShowStepCommand.Execute(nextPublishedAction);
                    });

                return _showInnerStepCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de passer à l'action suivante
        /// </summary>
        ICommand _nextActionDetailsCommand;
        public ICommand NextActionDetailsCommand
        {
            get
            {
                if (_nextActionDetailsCommand == null)
                    _nextActionDetailsCommand = new RelayCommand(() =>
                    {
                        PublishedAction nextPublishedAction = null;
                        if (_indexParent == null) // On est dans un process
                            nextPublishedAction = Publication.PublishedActions[++_index];
                        else // On est dans un sous-process
                            nextPublishedAction = Publication.PublishedActions[_indexParent.Value].LinkedPublication.PublishedActions[++_index];
                        // L'action est une action classique
                        PublishedAction = nextPublishedAction;
                    });
                return _nextActionDetailsCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de passer à l'action précédente
        /// </summary>
        ICommand _previousActionDetailsCommand;
        public ICommand PreviousActionDetailsCommand
        {
            get
            {
                if (_previousActionDetailsCommand == null)
                    _previousActionDetailsCommand = new RelayCommand(() =>
                    {
                        PublishedAction previousPublishedAction = null;
                        if (_indexParent == null) // On est dans un process
                            previousPublishedAction = Publication.PublishedActions[--_index];
                        else // On est dans un sous-process
                            previousPublishedAction = Publication.PublishedActions[_indexParent.Value].LinkedPublication.PublishedActions[--_index];
                        // L'action est une action classique
                        PublishedAction = previousPublishedAction;
                    });
                return _previousActionDetailsCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de lire une vidéo
        /// </summary>
        ICommand _playPauseCommand;
        public ICommand PlayPauseCommand
        {
            get
            {
                if (_playPauseCommand == null)
                    _playPauseCommand = new RelayCommand<Unosquare.FFME.MediaElement>(mediaElement =>
                    {
                        if (mediaElement?.IsPlaying == true)
                            mediaElement?.Pause();
                        else
                            mediaElement?.Play();
                    });
                return _playPauseCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de couper le son de la vidéo
        /// </summary>
        ICommand _muteCommand;
        public ICommand MuteCommand
        {
            get
            {
                if (_muteCommand == null)
                    _muteCommand = new RelayCommand<Unosquare.FFME.MediaElement>(mediaElement =>
                    {
                        if (mediaElement != null)
                            mediaElement.IsMuted = !mediaElement.IsMuted;
                    });
                return _muteCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de maximizer ou minimiser la vidéo
        /// </summary>
        ICommand _maximizeCommand;
        public ICommand MaximizeCommand
        {
            get
            {
                if (_maximizeCommand == null)
                    _maximizeCommand = new RelayCommand(() =>
                    {
                        VideoIsMaximized = !_videoIsMaximized;
                    });
                return _maximizeCommand;
            }
        }

        /// <summary>
        /// Obtient ou définit la publication courante
        /// </summary>
        public Publication Publication
        {
            get => Locator.Main.TrainingPublication;
            set
            {
                if (Locator.Main.TrainingPublication != value)
                {
                    Locator.Main.TrainingPublication = value;
                    RaisePropertyChanged();
                }
                CreateUIPublishedActions();
            }
        }

        /// <summary>
        /// Obtient ou définit la tâche courante
        /// </summary>
        public PublishedAction PublishedAction
        {
            get => _publishedAction;
            set
            {
                if (value != null)
                {
                    if (_publishedAction != value)
                    {
                        _publishedAction = value;
                        RaisePropertyChanged();
                    }

                    RaisePropertyChanged(nameof(NextVisibility));
                    RaisePropertyChanged(nameof(PreviousVisibility));
                    RaisePropertyChanged(nameof(VideoIsMaximized));
                    RaisePropertyChanged(nameof(IsValidated));
                    RaisePropertyChanged(nameof(ShowValidateCommand));
                }
            }
        }

        /// <summary>
        /// Méthode permettant de récupérer la date de début de la formation
        /// </summary>
        public string StartDate
        {
            get
            {
                var lastTraining = Publication?.Trainings
                    .Where(t => !t.IsDeleted && t.EndDate != null
                                && Operators.Select(o => o.UserId).Contains(t.UserId))
                    .MaxBy(t => t.StartDate)
                    .FirstOrDefault();

                return lastTraining?.StartDate.ToShortDateString() ?? DateTime.Now.ToShortDateString();
            }
        }

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher ou non l'action permettant de mettre en pause la formation
        /// </summary>
        public bool ShowSleepAction
        {
            get => _showSleepAction;
            private set
            {
                if (_showSleepAction != value)
                {
                    _showSleepAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient si on doit afficher ou non le bouton next
        /// </summary>
        public Visibility NextVisibility
        {
            get
            {
                TrackableCollection<PublishedAction> publishedActions = _indexParent == null ? Publication.PublishedActions : Publication.PublishedActions[_indexParent.Value].LinkedPublication.PublishedActions;
                return _index < publishedActions.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Obtient si on doit afficher ou non le bouton précédent
        /// </summary>
        public Visibility PreviousVisibility
        {
            get
            {
                if (_index == 0)
                    return Visibility.Collapsed;
                if (_index - 1 >= 0)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Obtient si on doit afficher le bouton permettant de valider une étape
        /// </summary>
        public bool ShowValidateCommand
        {
            get
            {
                if (_publishedAction != null && Operators?.Any() == true && _publishedAction.LinkedPublication == null)
                    return !IsValidated;
                return false;
            }
        }

        /// <summary>
        /// Obtient si une étape a été validée
        /// </summary>
        public bool IsValidated
        {
            get
            {
                bool isValidated;
                if (_publishedAction == null || Operators?.Any() != true)
                    return false;
                if (_publishedAction.LinkedPublication == null)
                {
                    // On récupère uniquement les ValidationTrainings du premier utilisateur, car ils sont tous au même niveau
                    isValidated = _publishedAction.Publication.Trainings
                        .Where(t => !t.IsDeleted && t.UserId == Operators.First().UserId)
                        .MaxBy(t => t.StartDate)
                        .FirstOrDefault()
                        ?.ValidationTrainings
                        .Any(vt => !vt.IsDeleted && vt.PublishedActionId == _publishedAction.PublishedActionId && vt.EndDate != null) == true;
                }
                else
                {
                    isValidated = _publishedAction.LinkedPublication.Trainings
                                      .Where(t => !t.IsDeleted && t.UserId == Operators.First().UserId)
                                      .MaxBy(_ => _.StartDate)
                                      .FirstOrDefault()
                                      ?.EndDate != null;
                }
                return isValidated;
            }
        }

        /// <summary>
        /// Obtient le booléen indiquant si la vidéo est fullscreen ou non
        /// </summary>
        bool _videoIsMaximized;
        public bool VideoIsMaximized
        {
            get => _videoIsMaximized || PublishedAction?.IsGroup == true;
            private set
            {
                if (_videoIsMaximized != value)
                {
                    _videoIsMaximized = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la liste des personnes sélectionnées
        /// </summary>
        public List<UIUser> Operators
        {
            get => _operators;
            set
            {
                if (_operators != value)
                {
                    _operators = value;
                    RaisePropertyChanged();
                }
                RaisePropertyChanged(nameof(NbOperators));
                RaisePropertyChanged(nameof(NbOperatorsLabel));
                RaisePropertyChanged(nameof(OnlyOneOperator));

                CreateUIPublishedActions();
            }
        }

        public SfDataGrid DataGrid { get; set; }

        public Task ScrollTo(PublishedAction publishedAction)
        {
            if (publishedAction == null)
                publishedAction = PublishedAction;
            if (DataGrid == null || publishedAction == null)
                return Task.CompletedTask;
            // On récupère la liste des PublishedActions
            List<PublishedAction> publishedActions = new List<PublishedAction>();
            foreach (PublishedAction pAction in Publication.PublishedActions)
            {
                publishedActions.Add(pAction);
                if (pAction.LinkedPublication != null)
                    foreach (PublishedAction pInnerAction in pAction.LinkedPublication.PublishedActions)
                        publishedActions.Add(pInnerAction);
            }
            // On récupère l'index de l'élément
            publishedAction = publishedActions.FirstOrDefault(_ => _.PublishedActionId == publishedAction.PublishedActionId);
            if (publishedAction == null)
                return Task.CompletedTask;
            int index = publishedActions.IndexOf(publishedAction) + 1;
            if (index == 0)
                return Task.CompletedTask;

            var visualContainer = DataGrid.GetVisualContainer();
            var scrollRows = visualContainer.ScrollRows;
            var scrollOwner = visualContainer.ScrollOwner;
            LineSizeCollection lineCount = visualContainer.RowHeights as LineSizeCollection;
            List<double> heights = new List<double>();
            for (int count = 0; count < lineCount.LineCount; count++)
            {
                var nestedDistances = lineCount.GetDistances(count);
                if (nestedDistances == null)
                {
                    if (lineCount[count] != 0)
                        heights.Add(lineCount[count]);
                }
                else
                {
                    for (int innerCount = 0; innerCount < nestedDistances.Count; innerCount++)
                    {
                        if (nestedDistances[innerCount] != 0)
                            heights.Add(nestedDistances[innerCount]);
                    }
                }
            }
            double vOffset = 0;
            for (int count = scrollRows.HeaderLineCount; count < index && count < heights.Count; count++)
                vOffset += heights[count];
            scrollOwner.ScrollToVerticalOffset(vOffset);
            return Task.CompletedTask;
        }

        public Unosquare.FFME.MediaElement MediaElement
        {
            get => Locator.Main.MediaElement;
            set
            {
                if (Locator.Main.MediaElement != value)
                {
                    Locator.Main.MediaElement = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le nombre de personnes selectionnées
        /// </summary>
        public int NbOperators =>
            _operators.Count;

        /// <summary>
        /// Obtient le nombre de personnes selectionnées en texte
        /// </summary>
        public string NbOperatorsLabel =>
            Locator.LocalizationManager.GetStringFormat("Person_Label", NbOperators);

        /// <summary>
        /// Obtient un booléen indiquant s'il n'y a qu'une seule personne selectionnée
        /// </summary>
        public bool OnlyOneOperator =>
            NbOperators == 1;

        #endregion

        #region Private Methods

        void CreateTraining()
        {
            try
            {
                var trainings = Publication.Trainings
                    .Where(t => !t.IsDeleted
                                && t.EndDate == null
                                && Operators.Select(o => o.UserId).Contains(t.UserId));
                if (IndexParent == null) // Validation d'une action dans le process
                {
                    foreach (var training in trainings)
                    {
                        var currentValidationTraining = training.ValidationTrainings
                            .SingleOrDefault(vt => !vt.IsDeleted
                                                   && vt.EndDate == null
                                                   && vt.PublishedActionId == PublishedAction.PublishedActionId);
                        if (currentValidationTraining == null)
                            training.ValidationTrainings.Add(new ValidationTraining()
                            {
                                StartDate = _startValidationDate,
                                EndDate = DateTime.Now,
                                TrainingId = training.TrainingId,
                                PublishedActionId = PublishedAction.PublishedActionId,
                                TrainerId = Locator.Main.SelectedUser.UserId
                            });
                        else
                        {
                            currentValidationTraining.EndDate = DateTime.Now;
                            currentValidationTraining.TrainerId = Locator.Main.SelectedUser.UserId;
                        }

                        // Si on a validé toutes les actions du process et éventuellement des sous process, on valide la formation
                        if (FormationIsValidated(Publication, training.UserId)
                            && Publication.PublishedActions
                                .Where(_ => _.LinkedPublication != null)
                                .All(_ => FormationIsValidated(_.LinkedPublication, training.UserId)))
                        {
                            training.EndDate = DateTime.Now;
                            var userReadPublication = Publication.Readers.SingleOrDefault(_ => _.UserId == training.UserId);
                            if (userReadPublication == null)
                                Publication.Readers.Add(new UserReadPublication
                                {
                                    UserId = training.UserId,
                                    ReadDate = training.EndDate.Value
                                });
                            else if (userReadPublication.ReadDate == null)
                                userReadPublication.ReadDate = training.EndDate.Value;
                        }
                    }
                    UpdateFormationParts(PublishedAction, Publication);

                }
                else // Validation d'une action dans un sous-process
                {
                    var subTrainings = Publication.PublishedActions[IndexParent.Value].LinkedPublication.Trainings
                        .Where(t => !t.IsDeleted
                                    && t.EndDate == null
                                    && Operators.Select(o => o.UserId).Contains(t.UserId));
                    foreach (var subTraining in subTrainings)
                    {
                        var currentInnerValidationTraining = subTraining.ValidationTrainings
                            .SingleOrDefault(vt => !vt.IsDeleted
                                                   && vt.EndDate == null
                                                   && vt.PublishedActionId == PublishedAction.PublishedActionId);
                        if (currentInnerValidationTraining == null)
                            subTraining.ValidationTrainings.Add(new ValidationTraining()
                            {
                                StartDate = _startValidationDate,
                                EndDate = DateTime.Now,
                                TrainingId = subTraining.TrainingId,
                                PublishedActionId = PublishedAction.PublishedActionId,
                                TrainerId = Locator.Main.SelectedUser.UserId
                            });
                        else
                        {
                            currentInnerValidationTraining.EndDate = DateTime.Now;
                            currentInnerValidationTraining.TrainerId = Locator.Main.SelectedUser.UserId;
                        }

                        // Si on a validé toutes les actions du sous-process, on valide la formation du sous-process
                        if (FormationIsValidated(subTraining.Publication, subTraining.UserId))
                        {
                            subTraining.EndDate = DateTime.Now;
                            var userReadPublication = subTraining.Publication.Readers.SingleOrDefault(_ => _.UserId == subTraining.UserId);
                            if (userReadPublication == null)
                                subTraining.Publication.Readers.Add(new UserReadPublication
                                {
                                    UserId = subTraining.UserId,
                                    ReadDate = subTraining.EndDate.Value
                                });
                            else if (userReadPublication.ReadDate == null)
                                userReadPublication.ReadDate = subTraining.EndDate.Value;
                            UpdateFormationParts(Publication.PublishedActions.FirstOrDefault(_ => _.LinkedPublicationId == PublishedAction.PublicationId), Publication);
                        }

                        // Si on a validé toutes les actions du process et éventuellement des sous process, on valide la formation
                        if (FormationIsValidated(Publication, subTraining.UserId)
                            && Publication.PublishedActions
                                .Where(_ => _.LinkedPublication != null)
                                .All(_ => FormationIsValidated(_.LinkedPublication, subTraining.UserId)))
                        {
                            var training = trainings.Single(_ => _.UserId == subTraining.UserId);
                            training.EndDate = DateTime.Now;
                            var userReadPublication = Publication.Readers.SingleOrDefault(_ => _.UserId == subTraining.UserId);
                            if (userReadPublication == null)
                                Publication.Readers.Add(new UserReadPublication
                                {
                                    UserId = subTraining.UserId,
                                    ReadDate = training.EndDate.Value
                                });
                            else if (userReadPublication.ReadDate == null)
                                userReadPublication.ReadDate = training.EndDate.Value;
                        }
                    }
                    UpdateFormationParts(PublishedAction,
                        PublishedAction.Publication,
                        Publication.PublishedActions.FirstOrDefault(_ => _.LinkedPublicationId == PublishedAction.PublicationId));
                }
                ShowSleepAction = !trainings.All(_ => _.EndDate.HasValue);
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la création d'une étape de validation de la formation.");
            }
        }

        // Permet de savoir si toutes les actions d'un process (hormis les groupes et les sous-process) ont été validées pour un utilisateur
        bool FormationIsValidated(Publication publication, int userId)
        {
            var training = publication.Trainings
                .FirstOrDefault(t => !t.IsDeleted && t.EndDate == null && t.UserId == userId);
            if (training == null)
                return true;
            return publication.PublishedActions
                .Where(_ => !_.IsGroup && _.LinkedPublication == null)
                .All(_ => training.ValidationTrainings.Any(v => v.PublishedActionId == _.PublishedActionId
                                                                && !v.IsDeleted
                                                                && v.EndDate != null));
        }

        void CreateUIPublishedActions()
        {
            try
            {
                if (Publication != null && Operators?.Any() == true)
                {
                    Publication.PublishedActions.ForEach(p => UpdateFormationParts(p, Publication));

                    ShowSleepAction = !Publication.Trainings
                        .Where(t => !t.IsDeleted && Operators.Select(o => o.UserId).Contains(t.UserId))
                        .All(_ => _.EndDate.HasValue);
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de création de l'affichage de la formation.");
            }
        }

        void UpdateFormationParts(PublishedAction publishedAction, Publication publication, PublishedAction parentPublishedAction = null)
        {
            var trainings = publication.Trainings
                .Where(t => !t.IsDeleted
                            && Operators.Any(o => o.UserId == t.UserId)
                            && t.PublicationId == publication.PublicationId);
            int index = publication.PublishedActions.IndexOf(publishedAction);
            if (parentPublishedAction == null) // Action d'un process
            {
                if (index == 0)
                {
                    publishedAction.CanValidateTrainingStep = true; // On peut toujours valider la première étape
                    publishedAction.FormationDate = null;
                    publishedAction.TrainedBy = null;
                    // Si c'est un sous-process et que toutes ses actions ont fait object d'une formation, on définit les infos
                    if (publishedAction.LinkedPublication != null
                        && publishedAction.LinkedPublication.Trainings
                            .Any(t => !t.IsDeleted
                                      && Operators.Any(o => o.UserId == t.UserId))
                        && publishedAction.LinkedPublication.Trainings
                            .Where(t => !t.IsDeleted
                                        && Operators.Any(o => o.UserId == t.UserId))
                            .All(_ => _.EndDate.HasValue)) // L'action est lié à un sous-process et toutes les sous-tâches ont fait l'objet d'une formation
                    {
                        publishedAction.FormationDate = publishedAction.LinkedPublication.Trainings
                            .Where(t => !t.IsDeleted
                                        && Operators.Any(o => o.UserId == t.UserId))
                            .MaxBy(_ => _.EndDate)
                            .First()
                            .EndDate.Value.ToShortDateString();
                        publishedAction.TrainedBy = Locator.Main.Users
                            .Single(u => u.UserId == publishedAction.LinkedPublication.Trainings
                                             .Where(t => !t.IsDeleted
                                                         && Operators.Any(o => o.UserId == t.UserId))
                                             .SelectMany(_ => _.ValidationTrainings)
                                             .Where(_ => !_.IsDeleted)
                                             .MaxBy(_ => _.EndDate)
                                             .First()
                                             .TrainerId)
                            .FullName;
                    }
                    else if (!publishedAction.IsGroup) // Si ce n'est pas un groupe, on définit les infos de formation si elle existe
                    {
                        var validationTrainings = trainings
                            .SelectMany(_ => _.ValidationTrainings)
                            .Where(_ => !_.IsDeleted
                                        && _.PublishedActionId == publishedAction.PublishedActionId);
                        if (validationTrainings.Any() && validationTrainings.All(_ => _.EndDate.HasValue))
                        {
                            var lastValidationTraining = validationTrainings
                                .MaxBy(vt => vt.EndDate)
                                .First();
                            publishedAction.FormationDate = lastValidationTraining.EndDate.Value.ToShortDateString();
                            publishedAction.TrainedBy = Locator.Main.Users.SingleOrDefault(u => u.UserId == lastValidationTraining.TrainerId)?.FullName;
                        }
                    }
                }
                else
                {
                    var previousAction = publication.PublishedActions[index - 1];

                    if (previousAction.IsGroup) // L'action précédente est un groupe
                        publishedAction.CanValidateTrainingStep = true;// previousAction.CanValidateTrainingStep;
                    else
                        publishedAction.CanValidateTrainingStep = true;// !string.IsNullOrEmpty(previousAction.FormationDate);

                    if (publishedAction.LinkedPublication != null
                        && publishedAction.LinkedPublication.Trainings
                            .Any(t => !t.IsDeleted && Operators.Any(o => o.UserId == t.UserId))
                        && publishedAction.LinkedPublication.Trainings
                            .Where(t => !t.IsDeleted && Operators.Any(o => o.UserId == t.UserId))
                            .All(_ => _.EndDate.HasValue)) // L'action est li� � un sous-process et toutes les sous-tâches ont fait l'objet d'une formation
                    {
                        publishedAction.FormationDate = publishedAction.LinkedPublication.Trainings
                            .Where(t => !t.IsDeleted && Operators.Any(o => o.UserId == t.UserId))
                            .MaxBy(_ => _.EndDate)
                            .First()
                            .EndDate.Value.ToShortDateString();
                        publishedAction.TrainedBy = Locator.Main.Users
                            .Single(u => u.UserId == publishedAction.LinkedPublication.Trainings
                                             .Where(t => !t.IsDeleted && Operators.Any(o => o.UserId == t.UserId))
                                             .SelectMany(_ => _.ValidationTrainings)
                                             .Where(_ => !_.IsDeleted)
                                             .MaxBy(_ => _.EndDate)
                                             .First()
                                             .TrainerId)
                            .FullName;
                    }
                    else if (publishedAction.LinkedPublication == null
                             && !publishedAction.IsGroup
                             && trainings.SelectMany(_ => _.ValidationTrainings)
                                 .Any(_ => !_.IsDeleted
                                           && _.PublishedActionId == publishedAction.PublishedActionId)
                             && trainings.SelectMany(_ => _.ValidationTrainings)
                                 .Where(_ => !_.IsDeleted
                                             && _.PublishedActionId == publishedAction.PublishedActionId)
                                 .All(_ => _.EndDate.HasValue))
                    {
                        var lastValidationTraining = trainings
                            .SelectMany(_ => _.ValidationTrainings)
                            .Where(_ => !_.IsDeleted && _.PublishedActionId == publishedAction.PublishedActionId)
                            .MaxBy(_ => _.EndDate)
                            .First();
                        publishedAction.FormationDate = lastValidationTraining.EndDate.Value.ToShortDateString();
                        publishedAction.TrainedBy = Locator.Main.Users.Single(u => u.UserId == lastValidationTraining.TrainerId).FullName;
                    }
                }

                if (publishedAction.LinkedPublication != null)
                    publishedAction.LinkedPublication.PublishedActions.ForEach(_ => UpdateFormationParts(_, publishedAction.LinkedPublication, publishedAction));
            }
            else // Action d'un sous-process
            {
                if (index == 0)
                {
                    publishedAction.CanValidateTrainingStep = true;//parentPublishedAction.CanValidateTrainingStep;
                    publishedAction.FormationDate = null;
                    publishedAction.TrainedBy = null;
                    if (!publishedAction.IsGroup) // Si ce n'est pas un groupe, on définit les infos de formation si elle existe
                    {
                        var validationTrainings = trainings.SelectMany(_ => _.ValidationTrainings)
                            .Where(_ => !_.IsDeleted
                                        && _.PublishedActionId == publishedAction.PublishedActionId);
                        if (validationTrainings.Any()
                            && validationTrainings.All(_ => _.EndDate.HasValue))
                        {
                            publishedAction.FormationDate = validationTrainings
                                .MaxBy(_ => _.EndDate)
                                .First()
                                .EndDate.Value.ToShortDateString();
                            publishedAction.TrainedBy = Locator.Main.Users
                                .Single(_ => _.UserId == validationTrainings.MaxBy(vt => vt.EndDate)
                                                 .First().TrainerId)
                                .FullName;
                        }
                    }
                }
                else
                {
                    var previousAction = publication.PublishedActions[index - 1];

                    if (previousAction.IsGroup) // L'action précédente est un groupe
                        publishedAction.CanValidateTrainingStep = true;// previousAction.CanValidateTrainingStep;
                    else
                        publishedAction.CanValidateTrainingStep = true;// !string.IsNullOrEmpty(previousAction.FormationDate);

                    if (trainings.SelectMany(_ => _.ValidationTrainings)
                            .Any(_ => !_.IsDeleted
                                      && _.PublishedActionId == publishedAction.PublishedActionId)
                        && trainings.SelectMany(_ => _.ValidationTrainings)
                            .Where(_ => !_.IsDeleted
                                        && _.PublishedActionId == publishedAction.PublishedActionId)
                            .All(_ => _.EndDate.HasValue))
                    {
                        var lastValidationTraining = trainings.SelectMany(_ => _.ValidationTrainings)
                            .Where(_ => !_.IsDeleted
                                        && _.PublishedActionId == publishedAction.PublishedActionId)
                            .MaxBy(_ => _.EndDate)
                            .First();
                        publishedAction.FormationDate = lastValidationTraining.EndDate.Value.ToShortDateString();
                        publishedAction.TrainedBy = Locator.Main.Users.Single(u => u.UserId == lastValidationTraining.TrainerId).FullName;
                    }
                }
            }
        }

        public async Task Refresh()
        {
            var dialog = await Locator.Main.GetCurrentDialogAsync<FormationActionDetailsDialog>(this);
            if (dialog != null)
            {
                RaisePropertyChanged(nameof(PublishedAction));
            }
        }

        #endregion
    }
}