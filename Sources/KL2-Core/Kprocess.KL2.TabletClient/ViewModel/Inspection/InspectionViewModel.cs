using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Extensions;
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

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class InspectionViewModel : ViewModelBase, IRefreshViewModel, IMediaElementViewModel, ISfDataGridViewModel<PublishedAction>
    {
        #region Attributs

        bool _detailsIsOpened;
        bool _reopenDetails;
        bool _reopenParent;

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
        public Publication Publication =>
            Locator.Main.InspectionPublication;

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
        /// Méthode permettant de récupérer la date de début de l'inspection
        /// </summary>
        public string StartDate =>
            Inspection != null ?
                Inspection.StartDate.ToShortDateString() :
                DateTime.Now.ToShortDateString();

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher ou non l'action permettant de mettre en pause l'inspection
        /// </summary>
        public bool ShowSleepAction =>
            !IsReadOnly && !ShowValidateAction;

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

        /// <summary>
        /// Obtient la dernière inspection
        /// </summary>
        public Inspection Inspection =>
            Locator.Main.Inspection;

        public TrackableCollection<Anomaly> LinkedAnomalies =>
            Inspection.GetLinkedAnomalies();

        public TrackableCollection<Anomaly> OutsidedAnomalies =>
            Inspection.GetOutsidedAnomalies();

        public TrackableCollection<Anomaly> AllAnomalies =>
            new TrackableCollection<Anomaly>(LinkedAnomalies.Concat(OutsidedAnomalies));

        public bool ShowValidateAction =>
            Inspection != null &&
                !IsReadOnly &&
                Publication?.PublishedActions != null &&
                Publication.PublishedActions.Where(_ => !_.IsGroup).All(_ => _.IsOk.HasValue) &&
                Publication.PublishedActions.Where(_ => _.LinkedPublication != null).All(pa => pa.LinkedPublication.PublishedActions.Where(_ => !_.IsGroup).All(_ => _.IsOk.HasValue));

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

        #endregion

        #region Commands

        ICommand _addAnomalyCommand;
        public ICommand AddAnomalyCommand
        {
            get
            {
                if (_addAnomalyCommand == null)
                    _addAnomalyCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.Push<Views.Snapshot, SnapshotViewModel>(new SnapshotViewModel());
                    }, () =>
                    {
                        return Inspection != null;
                    });
                return _addAnomalyCommand;
            }
        }

        ICommand _showAnomaliesCommand;
        public ICommand ShowAnomaliesCommand
        {
            get
            {
                if (_showAnomaliesCommand == null)
                    _showAnomaliesCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.PushDialog<ShowAnomaliesDialog, InspectionViewModel>(this);
                    }, () =>
                    {
                        return AllAnomalies != null && AllAnomalies.Any();
                    });
                return _showAnomaliesCommand;
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

                            await Locator.Navigation.PushDialog<InspectionActionDetailsDialog, InspectionViewModel>(this);
                            _detailsIsOpened = true;
                            _reopenDetails = true;
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans l'inspection.");
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

        ICommand _showDecisionCommand;
        public ICommand ShowDecisionCommand
        {
            get
            {
                if (_showDecisionCommand == null)
                    _showDecisionCommand = new RelayCommand<PublishedAction>(async (publishedAction) =>
                    {
                        try
                        {
                            PublishedAction = publishedAction;

                            var vm = new AddInspectionAnomalyViewModel()
                            {
                                PublishedAction = publishedAction,
                                IsReadOnly = IsReadOnly
                            };

                            await Locator.Navigation.Push<Views.AddInspectionAnomaly, AddInspectionAnomalyViewModel>(vm);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de l'affichage de la popup permettant de saisir une raison.");
                        }
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
                    _returnCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;

                        if (!IsFromAudit)
                        {
                            try
                            {
                                if (Locator.APIManager.IsOnline == true)
                                {
                                    Locator.Main.ShowDisconnectedMessage = true;
                                    Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                                }
                                else
                                {
                                    Locator.Main.ShowDisconnectedMessage = false;
                                    Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                                }
                            }
                            catch (Exception e)
                            {
                                Locator.TraceManager.TraceDebug(e, e.Message);
                            }
                        }

                        await Locator.Navigation.Pop();

                        Locator.Main.IsLoading = false;
                    });

                return _returnCommand;
            }
        }

        /// <summary>
        /// Obtient la commande permettant de mettre en pause l'inspection
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
                                Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                            }
                        }
                        catch (Exception e)
                        {
                            Locator.TraceManager.TraceDebug(e, e.Message);
                        }
                        await Locator.Navigation.Pop();

                        Locator.Main.IsLoading = false;
                    });

                return _sleepCommand;
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
                        try
                        {   
                            Inspection.EndDate = DateTime.Now;
                            // Validation de l'inspection de l'ensemble des sous process
                            Publication.PublishedActions
                                    .Where(pa => pa.LinkedPublication != null)
                                    .Select(pa => pa.LinkedPublication.Inspections
                                        .Where(i => !i.IsDeleted && i.EndDate == null)
                                        .MaxBy(i => i.StartDate)
                                        .FirstOrDefault())
                                    .ForEach(lastOpenedInnerInspection =>
                                    {
                                        if (lastOpenedInnerInspection != null)
                                            lastOpenedInnerInspection.EndDate = Inspection.EndDate;
                                    });

                            try
                            {
                                if (Locator.APIManager.IsOnline == true)
                                {
                                    Locator.Main.ShowDisconnectedMessage = true;
                                    Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                                }
                                else
                                {
                                    Locator.Main.ShowDisconnectedMessage = false;
                                    Locator.Main.InspectionPublication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                                }
                            }
                            catch (Exception e)
                            {
                                Locator.TraceManager.TraceDebug(e, e.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la validation de l'inspection.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }

                        await Locator.Navigation.Pop();
                    });

                return _validateCommand;
            }
        }

        ICommand _changeOkStateCommand;
        public ICommand ChangeOkStateCommand
        {
            get
            {
                if (_changeOkStateCommand == null)
                    _changeOkStateCommand = new RelayCommand<PublishedAction>(publishedAction =>
                    {
                        PublishedAction = publishedAction;
                        if (IsReadOnly || publishedAction.LinkedPublicationId != null)
                            return;
                        UpdateInspection(true);
                        ScrollTo(PublishedAction);
                        RaisePropertyChanged(nameof(LinkedAnomalies));
                    });

                return _changeOkStateCommand;
            }
        }

        ICommand _changeNotOkStateCommand;
        public ICommand ChangeNotOkStateCommand
        {
            get
            {
                if (_changeNotOkStateCommand == null)
                    _changeNotOkStateCommand = new RelayCommand<PublishedAction>(async publishedAction =>
                    {
                        PublishedAction = publishedAction;
                        if (publishedAction.LinkedPublicationId != null)
                            return;
                        if (IsReadOnly && publishedAction.IsOk == false)
                        {
                            // Fermeture de la dialog de détails si elle est ouverte
                            if (_detailsIsOpened)
                            {
                                await Locator.Navigation.PopModal();
                                _detailsIsOpened = false;
                            }
                            // Ouverture de la popup pour le choix de la raison
                            ShowDecisionCommand.Execute(publishedAction);
                        }
                        else if (!IsReadOnly)
                        {
                            // Fermeture de la dialog de détails si elle est ouverte
                            if (_detailsIsOpened)
                            {
                                await Locator.Navigation.PopModal();
                                _detailsIsOpened = false;
                            }
                            if (publishedAction.IsOk != false)
                            {
                                // Ouverture de la popup pour le choix de la raison
                                try
                                {
                                    var vm = new AddInspectionAnomalyViewModel()
                                    {
                                        PublishedAction = PublishedAction,
                                        Title = Locator.LocalizationManager.GetString("Tablet_View_AddAnomaly")
                                    };

                                    await Locator.Navigation.Push<Views.AddInspectionAnomaly, AddInspectionAnomalyViewModel>(vm);
                                }
                                catch (Exception ex)
                                {
                                    Locator.TraceManager.TraceError(ex, "Erreur lors de l'affichage de la popup permettant de saisir une raison.");
                                }
                            }
                            else
                            {
                                // Ouverture de la popup pour le choix de la raison
                                ShowDecisionCommand.Execute(publishedAction);
                            }
                        }
                    });

                return _changeNotOkStateCommand;
            }
        }

        ICommand _showAnomalyCommand;
        public ICommand ShowAnomalyCommand
        {
            get
            {
                if (_showAnomalyCommand == null)
                    _showAnomalyCommand = new RelayCommand<PublishedAction>(async publishedAction =>
                    {
                        PublishedAction = publishedAction;
                        if (publishedAction.LinkedPublicationId != null)
                            return;
                        if (publishedAction.IsOk != false)
                            return;
                        // Fermeture de la dialog de détails si elle est ouverte
                        if (_detailsIsOpened)
                        {
                            await Locator.Navigation.PopModal();
                            _detailsIsOpened = false;
                        }
                        // Ouverture de la popup pour le choix de la raison
                        ShowDecisionCommand.Execute(publishedAction);
                    });

                return _showAnomalyCommand;
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
                        await Locator.Navigation.PopModal();
                        _detailsIsOpened = false;
                        _reopenDetails = false;

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

        ICommand _showAddAnomalyCommand;
        public ICommand ShowAddAnomalyCommand
        {
            get
            {
                if (_showAddAnomalyCommand == null)
                    _showAddAnomalyCommand = new RelayCommand<Anomaly>(async anomaly =>
                    {
                        try
                        {
                            await Locator.Navigation.PopModal();
                            _detailsIsOpened = false;
                            _reopenDetails = false;

                            var vm = new AddInspectionAnomalyViewModel()
                            {
                                ModifyAnomalyFromDialog = true,
                                Anomaly = anomaly,
                                IsReadOnly = IsReadOnly,
                            };

                            vm.ClosedEventHandler += (sender, args) =>
                            {
                                if (ShowAnomaliesCommand.CanExecute(null))
                                    ShowAnomaliesCommand.Execute(null);
                            };

                            await Locator.Navigation.Push<Views.AddInspectionAnomaly, AddInspectionAnomalyViewModel>(vm);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de l'affichage de la popup permettant de saisir une raison.");
                        }
                    });
                return _showAddAnomalyCommand;
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Public Methods

        /// <summary>
        /// Méthode permettant de rafraichir les données si besoin
        /// </summary>
        public Task Refresh()
        {
            UpdateInspectionParts();

            RaisePropertyChanged(nameof(LinkedAnomalies));
            RaisePropertyChanged(nameof(OutsidedAnomalies));
            RaisePropertyChanged(nameof(AllAnomalies));

            RaisePropertyChanged(nameof(ShowValidateAction));
            RaisePropertyChanged(nameof(ShowSleepAction));

            // Réouverture de la dialog de détails si elle était ouverte et qu'on revient du module étiquette
            if (!_detailsIsOpened && _reopenDetails)
                ShowStepCommand.Execute(PublishedAction);

            return Task.CompletedTask;
        }

        #endregion

        #region Private Methods

        public void UpdateInspectionParts()
        {
            Publication.PublishedActions
                .Where(_ => !_.IsGroup)
                .ForEach(publishedAction =>
            {
                InspectionStep inspectionStepForCurrentAction = Inspection.InspectionSteps.SingleOrDefault(_ => _.PublishedActionId == publishedAction.PublishedActionId);
                publishedAction.InspectionStep = inspectionStepForCurrentAction;
                publishedAction.IsOk = inspectionStepForCurrentAction.IsOk;
                publishedAction.InspectionDate = !publishedAction.IsOk.HasValue ? null : inspectionStepForCurrentAction.Date.ToShortDateString();
                publishedAction.InspectBy = !publishedAction.IsOk.HasValue ? string.Empty : Locator.Main.Users.Single(_ => _.UserId == inspectionStepForCurrentAction.InspectorId).ToString();

                if (publishedAction.LinkedPublication != null)
                {
                    Inspection currentInnerInspection = inspectionStepForCurrentAction.LinkedInspection;
                    publishedAction.LinkedPublication.PublishedActions
                        .Where(_ => !_.IsGroup)
                        .ForEach(innerPublishedAction =>
                    {
                        InspectionStep inspectionStepForCurrentInnerAction = currentInnerInspection.InspectionSteps.SingleOrDefault(_ => _.PublishedActionId == innerPublishedAction.PublishedActionId);
                        innerPublishedAction.InspectionStep = inspectionStepForCurrentInnerAction;
                        innerPublishedAction.IsOk = inspectionStepForCurrentInnerAction.IsOk;
                        innerPublishedAction.InspectionDate = !innerPublishedAction.IsOk.HasValue ? null : inspectionStepForCurrentInnerAction.Date.ToShortDateString();
                        innerPublishedAction.InspectBy = !innerPublishedAction.IsOk.HasValue ? string.Empty : Locator.Main.Users.Single(_ => _.UserId == inspectionStepForCurrentInnerAction.InspectorId).ToString();
                    });
                }
            });
        }

        void UpdateInspection(bool? isOk)
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;

            try
            {
                // Inspection de l'etape courante
                if (PublishedAction.InspectionStep != null)
                {
                    // Il existe déjà une inspection on la modifie
                    if (isOk == true && PublishedAction.InspectionStep.Anomaly != null)
                    {
                        PublishedAction.InspectionStep.Anomaly.MarkAsDeleted();
                        PublishedAction.InspectionStep.Anomaly = null;
                    }
                    PublishedAction.InspectionStep.IsOk = isOk;
                    PublishedAction.InspectionStep.Date = DateTime.Now;
                    PublishedAction.InspectionStep.InspectorId = Locator.Main.SelectedUser.UserId;

                    PublishedAction.IsOk = PublishedAction.InspectionStep.IsOk;
                    PublishedAction.InspectionDate = !PublishedAction.InspectionStep.IsOk.HasValue ? null : PublishedAction.InspectionStep.Date.ToShortDateString();
                    PublishedAction.InspectBy = !PublishedAction.InspectionStep.IsOk.HasValue ? string.Empty : Locator.Main.Users.Single(_ => _.UserId == PublishedAction.InspectionStep.InspectorId).ToString();

                    // S'agit-il d'une tâche d'un sous-process, on met à jour la PublishedAction et l'InspectionStep parente
                    if (PublishedAction.PublicationId != Publication.PublicationId)
                    {
                        var parentInspectionStep = Inspection.InspectionSteps.SingleOrDefault(_ => _.LinkedInspection == PublishedAction.InspectionStep.Inspection);
                        var parentAction = Publication.PublishedActions.SingleOrDefault(_ => _.InspectionStep == parentInspectionStep);
                        if (isOk == true)
                            parentInspectionStep.IsOk = PublishedAction.InspectionStep.Inspection.InspectionSteps.All(_ => _.IsOk == true);
                        else if (isOk == false)
                            parentInspectionStep.IsOk = false;
                        else
                            parentInspectionStep.IsOk = PublishedAction.InspectionStep.Inspection.InspectionSteps.Any(_ => _.IsOk.HasValue);
                        parentInspectionStep.Date = PublishedAction.InspectionStep.Date;
                        parentInspectionStep.InspectorId = PublishedAction.InspectionStep.InspectorId;
                        parentAction.IsOk = parentInspectionStep.IsOk;
                        parentAction.InspectionDate = !parentAction.IsOk.HasValue ? null : parentInspectionStep.Date.ToShortDateString();
                        parentAction.InspectBy = !parentAction.IsOk.HasValue ? string.Empty : Locator.Main.Users.Single(_ => _.UserId == parentInspectionStep.InspectorId).ToString();
                    }
                }

                OfflineFile.Inspection.SaveToJson(Publication);
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la mise à jour de l'inspection.");
            }
            finally
            {
                RaisePropertyChanged(nameof(ShowValidateAction));
                RaisePropertyChanged(nameof(ShowSleepAction));
                if (_detailsIsOpened)
                {
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
                }

                RaisePropertyChanged(nameof(PublishedAction));
                RaisePropertyChanged(nameof(NextVisibility));
                RaisePropertyChanged(nameof(PreviousVisibility));

                Locator.Main.IsLoading = false;
            }
        }

        #endregion
    }
}