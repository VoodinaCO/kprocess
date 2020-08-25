using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using MoreLinq;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class QualificationViewModel : ViewModelBase, IRefreshViewModel, IMediaElementViewModel, ISfDataGridViewModel<PublishedAction>
    {
        #region Attributs        

        ChooseReasonDialogViewModel _chooseReasonDialogViewModel;
        bool _reopenParent;
        bool CreateUIPublishedActions_IsRunning;

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
        /// Obtient ou définit la publication courante
        /// </summary>
        public Publication Publication
        {
            get => Locator.Main.EvaluationPublication;
            set
            {
                if (Locator.Main.EvaluationPublication != value)
                {
                    Locator.Main.EvaluationPublication = value;
                    RaisePropertyChanged();
                }
                var createTask = CreateUIPublishedActions();
            }
        }

        /// <summary>
        /// Obtient ou définit la tâche courante
        /// </summary>
        PublishedAction _publishedAction;
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
                }
            }
        }

        UIUser _operator;
        /// <summary>
        /// Obtient la personne sélectionnée
        /// </summary>
        public UIUser Operator
        {
            get => _operator;
            set
            {
                if (_operator != value)
                {
                    _operator = value;
                    RaisePropertyChanged();
                }
                var createTask = CreateUIPublishedActions();
            }
        }

        public SfDataGrid DataGrid { get; set; }

        public async Task ScrollTo(PublishedAction publishedAction)
        {
            try
            {
                if (publishedAction == null)
                    publishedAction = PublishedAction;
                if (DataGrid == null || publishedAction == null)
                    return;

                // Waiting for end of CreateUIPublishedActions
                while (CreateUIPublishedActions_IsRunning)
                    await Task.Delay(10);
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
                    return;
                int index = publishedActions.IndexOf(publishedAction) + 1;
                if (index == 0)
                    return;

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
            }
            catch
            {

            }
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
        /// Méthode permettant de récupérer la date de début de la formation
        /// </summary>
        public string StartDate =>
            Qualification != null ?
                Qualification.StartDate.ToShortDateString() :
                DateTime.Now.ToShortDateString();

        bool _showSleepAction = true;
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

        Qualification _qualification;
        /// <summary>
        /// Obtient la dernière évaluation de l'utilisateur selectionné
        /// </summary>
        public Qualification Qualification
        {
            get => _qualification;
            private set
            {
                if (_qualification != value)
                {
                    _qualification = value;
                    RaisePropertyChanged();
                }
                RaisePropertyChanged(nameof(ResultBackgroundColor));
                RaisePropertyChanged(nameof(DecisionBackgroundColor));
            }
        }

        /// <summary>
        /// Obtient la liste des décisions possible
        /// </summary>
        public Dictionary<bool, string> Decisions =>
            new Dictionary<bool, string>()
            {
                [true] = Locator.LocalizationManager.GetString("Common_Qualified"),
                [false] = Locator.LocalizationManager.GetString("Common_NotQualified")
            };

        /// <summary>
        /// Obtient la couleur de fond du résultat
        /// </summary>
        public Brush ResultBackgroundColor =>
            (Qualification == null || Qualification.Result < 80) ? Brushes.Red : Brushes.Green;

        /// <summary>
        /// Obtient la couleur de fond pour la décision
        /// </summary>
        public Brush DecisionBackgroundColor =>
            Qualification == null || (Qualification.IsQualified.HasValue && !Qualification.IsQualified.Value) ? Brushes.Red : Brushes.Green;

        bool _showValidateAction;
        public bool ShowValidateAction
        {
            get => _showValidateAction;
            private set
            {
                if (_showValidateAction != value)
                {
                    _showValidateAction = value;
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
        /// Obtient la liste des raisons
        /// </summary>
        BindingList<QualificationReason> _reasons;
        public BindingList<QualificationReason> Reasons
        {
            get => _reasons;
            private set
            {
                if (_reasons != value)
                {
                    _reasons = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Obtient la commande permettant d'afficher l'étape
        /// </summary>
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

                            await Locator.Navigation.PushDialog<QualificationActionDetailsDialog, QualificationViewModel>(this);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans l'évaluation.");
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
        /// Obtient la commande permettant d'afficher la décision
        /// </summary>
        ICommand _showDecisionCommand;
        public ICommand ShowDecisionCommand
        {
            get
            {
                if (_showDecisionCommand == null)
                    _showDecisionCommand = new RelayCommand<PublishedAction>(async (publishedAction) =>
                    {
                        PublishedAction = publishedAction;
                        await ShowChooseReasonDialog(publishedAction);
                    });

                return _showDecisionCommand;
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
                    _returnCommand = new RelayCommand(async () => await SavePublicationData());

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
                    _sleepCommand = new RelayCommand(async () => await SavePublicationData());

                return _sleepCommand;
            }
        }

        bool _isConnecting;
        public bool IsConnecting
        {
            get => _isConnecting;
            private set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _errorText;
        public string ErrorText
        {
            get => _errorText;
            private set
            {
                if (_errorText != value)
                {
                    _errorText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string RequiredPasswordText =>
            Locator.LocalizationManager.GetStringFormat("RequiredPasswordToSignEvaluation", Operator.FullName);

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
                (ValidateCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        ICommand _addSignatureCommand;
        public ICommand AddSignatureCommand
        {
            get
            {
                if (_addSignatureCommand == null)
                    _addSignatureCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        Password = string.Empty;
                        await Locator.Navigation.PushDialog<AddSignatureOperatorFormationDialog, QualificationViewModel>(this);

                        Locator.Main.IsLoading = false;
                    });
                return _addSignatureCommand;
            }
        }
        
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
                            if (await ValidatePassword() == false)
                                return;
                            
                            Qualification.EndDate = DateTime.Now;

                            // Validation de l'évaluation de l'ensemble des sous process
                            var innerQualifications = Publication.PublishedActions
                                .Where(pa => pa.LinkedPublication != null)
                                .Select(pa => pa.LinkedPublication)
                                .Select(lp => lp.Qualifications
                                    .Where(q => !q.IsDeleted && q.UserId == Operator.UserId)
                                    .MaxBy(q => q.StartDate)
                                    .FirstOrDefault());
                            foreach (var innerQualification in innerQualifications)
                            {
                                if (innerQualification != null)
                                    innerQualification.EndDate = Qualification.EndDate;
                            }

                            await SavePublicationData(false);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la validation de l'évaluation.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                            IsConnecting = false;
                        }
                       
                    }, () => !string.IsNullOrEmpty(Password) && Locator.Main.SelectedUser != null && Operator != null);

                return _validateCommand;
            }
        }

        ICommand _changeQualifiedStateCommand;
        public ICommand ChangeQualifiedStateCommand
        {
            get
            {
                if (_changeQualifiedStateCommand == null)
                    _changeQualifiedStateCommand = new RelayCommand<PublishedAction>(async publishedAction =>
                    {
                        PublishedAction = publishedAction;

                        await UpdateQualification(true, null);
                        var scrollTo = ScrollTo(PublishedAction);
                    });

                return _changeQualifiedStateCommand;
            }
        }

        ICommand _changeNotQualifiedStateCommand;
        public ICommand ChangeNotQualifiedStateCommand
        {
            get
            {
                if (_changeNotQualifiedStateCommand == null)
                    _changeNotQualifiedStateCommand = new RelayCommand<PublishedAction>(async publishedAction =>
                    {
                        PublishedAction = publishedAction;
                        // Ouverture de la popup pour le choix de la raison
                        await ShowChooseReasonDialog(publishedAction);
                    });

                return _changeNotQualifiedStateCommand;
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
                    });
                return _closeDetailsDialogCommand;
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
                    _playPauseCommand = new RelayCommand<MediaElement>(mediaElement =>
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
                    _muteCommand = new RelayCommand<MediaElement>(mediaElement =>
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

        #endregion

        #region Public Methods

        /// <summary>
        /// Méthode permettant de rafraichir les données si besoin
        /// </summary>
        public async Task Refresh()
        {
            Publication = await OfflineFile.Evaluation.GetFromJson<Publication>();
        }

        #endregion

        #region Private Methods

        async Task<bool> ValidatePassword()
        {
            IsConnecting = true;
            bool result = Locator.Main.ValidatePassword(Operator, _password);
            if (!result)
            {
                ErrorText = Locator.LocalizationManager.GetString("AuthenticationFailedMessage");
                Locator.TraceManager.TraceDebug(ErrorText);

                Password = string.Empty;
            }
            else
            {
                //Close AddSignature View
                await Locator.Navigation.Pop();
            }

            return result;
        }

        async Task SavePublicationData(bool setLoading = true)
        {
            Locator.Main.IsLoading |= setLoading;

            //AnhLe TODO : Please check again this one
            //do not need to re-assign publication because it raises some property change and function 
            //just pop view
            try
            {
                if (Locator.APIManager.IsOnline == true)
                    Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, Publication);
                else
                    Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Evaluation, Publication);
            }
            catch { }
            await Locator.Navigation.Pop();

            Locator.Main.IsLoading &= !setLoading;
        }

        (int? QualificationReasonId, string Comment) GetLastReason(PublishedAction publishedAction)
        {
            var step = publishedAction.QualificationStep;
            return step == null ? (null, null) : (step.QualificationReasonId, step.Comment);
        }

        async Task LoadReasonList()
        {
            if (Reasons == null || Reasons.Count == 0)
            {
                if (Locator.APIManager.IsOnline == true) // Mode connecté
                    Reasons = new BindingList<QualificationReason>(
                        await Locator.GetService<IPrepareService>().GetQualificationReasons());
                else
                    Reasons = await OfflineFile.QualificationReasons.GetFromJson<BindingList<QualificationReason>>();
            }
        }

        //Anh LE Todo : Should check reload this function call too much
        async Task CreateUIPublishedActions()
        {
            CreateUIPublishedActions_IsRunning = true;
            try
            {
                if (Publication == null || Operator == null)
                    return;

                await LoadReasonList();

                Publication.PublishedActions.ForEach(p => UpdateQualificationParts(p, new List<UIUser>
                {
                    Operator
                }, Publication));

                Qualification = Publication.Qualifications
                    .Where(q => !q.IsDeleted && q.UserId == Operator.UserId)
                    .MaxBy(q => q.StartDate)
                    .FirstOrDefault();
                if (Qualification != null)
                    Qualification.PropertyChanged += Qualification_PropertyChanged;

                ShowSleepAction = Qualification != null
                                  && Qualification.QualificationSteps.Count(q => !q.IsDeleted) < Publication.PublishedActions.Count(p => !p.IsGroup);
                ShowValidateAction = Qualification != null
                                     && Qualification.EndDate == null
                                     && Qualification.QualificationSteps.Count(q => !q.IsDeleted) >= Publication.PublishedActions.Count(p => !p.IsGroup);
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de création de l'affichage de la formation.");
            }
            finally
            {
                CreateUIPublishedActions_IsRunning = false;
            }
        }

        void UpdateQualificationParts(PublishedAction publishedAction, List<UIUser> operators, Publication publication, bool parentCanValidate = false, bool isNested = false)
        {
            var lastQualification = publication.Qualifications
                .Where(q => !q.IsDeleted && operators.Any(o => o.UserId == q.UserId))
                .MaxBy(q => q.StartDate)
                .FirstOrDefault();

            bool isCanValidate = lastQualification?.IsQualified != true
                                 && publishedAction.IsQualified != true
                                 || lastQualification?.EndDate.HasValue == false;

            publishedAction.CanValidateQualificationStep = isCanValidate;

            UpdateQualificationStep(publishedAction,
                publication.Qualifications
                    .Where(q => !q.IsDeleted && q.UserId == operators[0].UserId)
                    .MaxBy(q => q.StartDate)
                    .FirstOrDefault());

            // Update editable comment
            UpdateEditableComment(publishedAction);

            // Traitement des sous process
            var linkedPublication = publishedAction.LinkedPublication;
            if (linkedPublication != null)
                foreach (var p in linkedPublication.PublishedActions)
                    UpdateQualificationParts(p, operators, publishedAction.LinkedPublication, publishedAction.CanValidateQualificationStep, true);
        }

        void UpdateQualificationStep(PublishedAction publishedAction, Qualification qualification)
        {
            var lastQualification = qualification;

            if (lastQualification == null)
                return;

            var qualificationSteps = lastQualification.QualificationSteps
                .Where(_ => !_.IsDeleted && _.PublishedActionId == publishedAction.PublishedActionId);
            var lastQualificationStep = qualificationSteps
                .MaxBy(_ => _.Date)
                .FirstOrDefault();

            publishedAction.QualificationStep = lastQualificationStep;

            if (lastQualificationStep == null)
                return;

            publishedAction.IsQualified = publishedAction.QualificationStep.IsQualified;
            publishedAction.Qualifier = lastQualificationStep.User?.FullName ?? string.Empty;
        }

        void UpdateEditableComment(PublishedAction publishedAction)
        {
            var step = publishedAction.QualificationStep;
            if (step == null)
                return;

            if (step.IsQualified == true)
            {
                step.IsEditableComment = false;
                return;
            }

            if (Reasons != null)
                step.IsEditableComment = Reasons.FirstOrDefault(x => x.Id == step.QualificationReasonId)?.IsEditable ?? false;
            else
                step.IsEditableComment = step.QualificationReason?.IsEditable ?? false;
        }

        async Task UpdateQualification(bool? isQualified, QualificationReason reason)
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;

            Qualification.PropertyChanged -= Qualification_PropertyChanged;

            int result;

            try
            {
                PublishedAction.LinkedPublication?.PublishedActions.ForEach(publishedAction =>
                {
                    // On est sur une étape qui contient un process interne, l'évaluation de celle ci entraîne l'évaluation de l'ensemble des etapes du process interne
                    if (!publishedAction.IsGroup)
                        ManageQualification(publishedAction.Publication, publishedAction.PublishedActionId, reason, isQualified, out result);
                });

                // Evaluation de l'etape courante
                if (ManageQualification(PublishedAction.Publication, PublishedAction.PublishedActionId, reason, isQualified, out result))
                {
                    // On a évalué l'ensemble des étapes si c'est un process interne on valide l'étape du process parent
                    if (PublishedAction.PublicationId != Publication.PublicationId)
                    {
                        var parentPublishedAction = Publication.PublishedActions.FirstOrDefault(p => p.LinkedPublication != null && p.LinkedPublicationId == PublishedAction.PublicationId);

                        if (parentPublishedAction != null)
                            ManageQualification(parentPublishedAction.Publication, parentPublishedAction.PublishedActionId, null, result > 80, out result);
                    }
                }

                OfflineFile.Evaluation.SaveToJson(Publication);
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la mise à jour de l'évaluation.");
            }
            finally
            {
                Qualification.PropertyChanged += Qualification_PropertyChanged;

                if (PublishedAction.PublicationId != Publication.PublicationId) // C'est un sous process
                {
                    PublishedAction parent = Publication.PublishedActions.FirstOrDefault(p => p.LinkedPublicationId == PublishedAction.PublicationId);
                    if (parent != null)
                    {
                        _indexParent = Publication.PublishedActions.IndexOf(parent);
                        _index = parent.LinkedPublication.PublishedActions.IndexOf(PublishedAction);
                    }
                }
                else
                {
                    _indexParent = null;
                    _index = Publication.PublishedActions.IndexOf(PublishedAction);
                }

                await CreateUIPublishedActions();
                RaisePropertyChanged(nameof(PublishedAction));

                Locator.Main.IsLoading = false;
            }
        }

        bool ManageQualification(Publication publication, int publishedActionId, QualificationReason reason, bool? isQualified, out int result)
        {
            QualificationStep qualificationStep = null;

            Qualification lastQualification = publication.Qualifications
                .Where(q => !q.IsDeleted && Operator.UserId == q.UserId)
                .MaxBy(q => q.StartDate)
                .FirstOrDefault();
            if (lastQualification?.QualificationSteps.Any() == true)
                qualificationStep = lastQualification.QualificationSteps
                    .FirstOrDefault(qs => !qs.IsDeleted && qs.PublishedActionId == publishedActionId);

            if (qualificationStep == null)
            {
                // Pas de qualification pour cette action
                var newQualificationStep = new QualificationStep()
                {
                    Date = DateTime.Now,
                    Comment = reason?.Comment,
                    QualificationId = lastQualification.QualificationId,
                    QualifierId = Locator.Main.SelectedUser.UserId,
                    IsQualified = isQualified,
                    PublishedActionId = publishedActionId,
                    QualificationReasonId = reason?.Id,
                    IsEditableComment = reason?.IsEditable ?? false
                };
                lastQualification.QualificationSteps.Add(newQualificationStep);
                publication.PublishedActions.Single(_ => _.PublishedActionId == publishedActionId).QualificationStep = newQualificationStep;
                newQualificationStep.PublishedAction = publication.PublishedActions.Single(_ => _.PublishedActionId == newQualificationStep.PublishedActionId);
                qualificationStep = newQualificationStep;
            }
            else
            {
                // Il existe déjà une qualification on la modifie
                qualificationStep.QualificationReasonId = reason?.Id;
                qualificationStep.IsQualified = isQualified;
                qualificationStep.Date = DateTime.Now;
                qualificationStep.QualifierId = Locator.Main.SelectedUser.UserId;
                qualificationStep.IsEditableComment = reason?.IsEditable ?? false;
                qualificationStep.Comment = reason?.Comment;
            }

            // Calcul du resultat de l'évaluation
            var hasNokImportantStep = lastQualification.QualificationSteps
                .Any(_ => _.PublishedAction.IsKeyTask && _.IsQualified == false);
            var nbStep = publication.PublishedActions.Count(p => !p.IsGroup);
            var steps = lastQualification?.QualificationSteps.Where(q => !q.IsDeleted);
            var nbOK = Math.Max(0, steps?.Count(s => s.IsQualified == true) ?? 0);

            result = Math.Min((int)Math.Round((double)(nbOK * 100 / nbStep)), 100);
            lastQualification.Result = result;
            lastQualification.IsQualified = lastQualification.Result >= 80 && !hasNokImportantStep;

            return nbStep == (steps?.Count() ?? 0);
        }

        async Task ShowChooseReasonDialog(PublishedAction publishedAction)
        {
            try
            {
                var (QualificationReasonId, Comment) = GetLastReason(publishedAction);

                _chooseReasonDialogViewModel = new ChooseReasonDialogViewModel(QualificationReasonId, Comment, Reasons);
                await Locator.Navigation.PushDialog<ChooseReasonDialog, ChooseReasonDialogViewModel>(_chooseReasonDialogViewModel);
                _chooseReasonDialogViewModel.OnClose += ChooseReasonDialogViewModel_OnClose;
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de l'affichage de la popup permettant de saisir une raison.");
            }
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Qualification_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Qualification.IsQualified))
                RaisePropertyChanged(nameof(DecisionBackgroundColor));

            if (e.PropertyName == nameof(Qualification.IsQualified) || e.PropertyName == nameof(Qualification.Comment))
                OfflineFile.Evaluation.SaveToJson(Publication);
        }

        /// <summary>
        /// Méthode appellé lorsque l'utilisateur sélectionne une raison 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void ChooseReasonDialogViewModel_OnClose(object sender, QualificationReason e)
        {
            try
            {
                _chooseReasonDialogViewModel.OnClose -= ChooseReasonDialogViewModel_OnClose;

                if (!string.IsNullOrEmpty(e.Comment))
                    await UpdateQualification(false, e);
                else
                {
                    if (PublishedAction.QualificationStep?.QualificationReason != null)
                        await UpdateQualification(PublishedAction.QualificationStep.IsQualified == true, PublishedAction.QualificationStep.QualificationReason);
                    else
                        await UpdateQualification(null, e);
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la modification du commentaire.");
            }
            var scrollTo = ScrollTo(PublishedAction);
        }

        #endregion
    }
}