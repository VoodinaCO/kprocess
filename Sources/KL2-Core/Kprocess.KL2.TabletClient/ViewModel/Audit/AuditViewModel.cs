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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class AuditViewModel : ViewModelBase, IMediaElementViewModel, ISfDataGridViewModel<AuditItem>, IRefreshViewModel
    {
        #region Attributs

        bool _detailsIsOpened;

        #endregion

        #region Properties

        public double TextDialogHeight { get; } = 300;

        public double TextDialogWidth { get; } = 600;

        public string TextDialogTitle { get; } = "Commentaire";

        public string TextDialogResult { get; set; }

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
            get => Locator.Main.InspectionPublication;
            set
            {
                if (Locator.Main.InspectionPublication != value)
                {
                    Locator.Main.InspectionPublication = value;
                    RaisePropertyChanged();
                }
                Audit = Locator.Main.InspectionPublication.Inspections.SelectMany(_ => _.Audits)
                    .Where(_ => !_.IsDeleted && _.AuditorId == Locator.Main.SelectedUser.UserId && _.EndDate == null)
                    .MaxBy(_ => _.StartDate)
                    .FirstOrDefault();
                Locator.Main.Inspection = Audit?.Inspection;
                CreateAuditItems();
            }
        }

        /// <summary>
        /// Obtient ou définit l'item courant
        /// </summary>
        AuditItem _auditItem;
        public AuditItem AuditItem
        {
            get => _auditItem;
            set
            {
                if (value != null)
                {
                    if (_auditItem != value)
                    {
                        _auditItem = value;
                        RaisePropertyChanged();
                    }

                    RaisePropertyChanged(nameof(NextVisibility));
                    RaisePropertyChanged(nameof(PreviousVisibility));
                }
            }
        }

        public SfDataGrid DataGrid { get; set; }

        public Task ScrollTo(AuditItem auditItem)
        {
            if (auditItem == null)
                auditItem = AuditItem;
            if (DataGrid == null || auditItem == null)
                return Task.CompletedTask;
            // On récupère l'index de l'élément
            auditItem = Audit?.AuditItems.FirstOrDefault(_ => _.Number == auditItem.Number);
            if (auditItem == null)
                return Task.CompletedTask;
            int index = Audit.AuditItems.IndexOf(auditItem) + 1;
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
        /// Méthode permettant de récupérer la date de début de l'audit
        /// </summary>
        public string StartDate =>
            Audit != null ?
                Audit.StartDate.ToShortDateString() :
                DateTime.Now.ToShortDateString();

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher ou non l'action permettant de mettre en pause l'inspection
        /// </summary>
        public bool ShowSleepAction =>
            !ShowValidateAction;

        Audit _audit;
        /// <summary>
        /// Obtient l'audit en cours
        /// </summary>
        public Audit Audit
        {
            get => _audit;
            private set
            {
                if (_audit != value)
                {
                    _audit = value;
                    RaisePropertyChanged();
                }
                RaisePropertyChanged(nameof(LinkedAnomalies));
                RaisePropertyChanged(nameof(OutsidedAnomalies));
                RaisePropertyChanged(nameof(AllAnomalies));
            }
        }

        public TrackableCollection<Anomaly> LinkedAnomalies =>
            _audit?.Inspection.GetLinkedAnomalies() ?? new TrackableCollection<Anomaly>();

        public TrackableCollection<Anomaly> OutsidedAnomalies =>
            _audit?.Inspection.GetOutsidedAnomalies() ?? new TrackableCollection<Anomaly>();

        public TrackableCollection<Anomaly> AllAnomalies =>
            new TrackableCollection<Anomaly>(LinkedAnomalies.Concat(OutsidedAnomalies));

        public bool ShowValidateAction =>
            Audit.AuditItems.All(_ => _.IsOk.HasValue);

        /// <summary>
        /// Obtient si on doit afficher ou non le bouton next
        /// </summary>
        public Visibility NextVisibility
        {
            get
            {
                return _index < Audit.AuditItems.Count - 1 ? Visibility.Visible : Visibility.Collapsed;
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
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// Obtient le booléen indiquant si la vidéo est fullscreen ou non
        /// </summary>
        bool _videoIsMaximized;
        public bool VideoIsMaximized
        {
            get => _videoIsMaximized;
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
                        return Audit != null;
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
                        await Locator.Navigation.PushDialog<ShowAnomaliesDialog, AuditViewModel>(this);
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
                    _showStepCommand = new RelayCommand<AuditItem>((auditItem) =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            AuditItem item = Audit.AuditItems.FirstOrDefault(a => a.Number == auditItem.Number);
                            _index = Audit.AuditItems.IndexOf(item);

                            AuditItem = item;

                            /*await Locator.Navigation.PushDialog<InspectionActionDetailsDialog, AuditViewModel>(this);
                            _detailsIsOpened = true;*/
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la sélection d'une action dans l'audit.");
                        }
                        finally
                        {
                            Locator.Main.IsLoading = false;
                        }
                    });

                return _showStepCommand;
            }
        }

        ICommand _showInspectionCommand;
        public ICommand ShowInspectionCommand
        {
            get
            {
                if (_showInspectionCommand == null)
                    _showInspectionCommand = new RelayCommand(async () =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            var viewModel = new InspectionViewModel
                            {
                                IsReadOnly = true,
                                IsFromAudit = true,
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
                    });

                return _showInspectionCommand;
            }
        }

        ICommand _showCommentCommand;
        public ICommand ShowCommentCommand
        {
            get
            {
                if (_showCommentCommand == null)
                    _showCommentCommand = new RelayCommand<AuditItem>(async (auditItem) =>
                    {
                        try
                        {
                            AuditItem = auditItem;
                            TextDialogResult = AuditItem.Comment;

                            await Locator.Navigation.PushDialog<TextDialog, AuditViewModel>(this);
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de l'affichage de la popup permettant de saisir un commentaire.");
                        }
                    });

                return _showCommentCommand;
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
                        Locator.Main.ShowDisconnectedMessage = false;

                        try
                        {
                            if (Locator.APIManager.IsOnline == true)
                            {
                                Locator.Main.ShowDisconnectedMessage = true;
                                Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
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
        /// Obtient la commande permettant de mettre en pause l'audit
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
                                Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                            }
                        }
                        catch { }
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
                            Audit.EndDate = DateTime.Now;

                            if (Locator.APIManager.IsOnline == true)
                            {
                                Locator.Main.ShowDisconnectedMessage = true;
                                Publication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, Publication);
                            }
                            else
                            {
                                Locator.Main.ShowDisconnectedMessage = false;
                                Publication = await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, Publication);
                            }
                        }
                        catch (Exception ex)
                        {
                            Locator.TraceManager.TraceError(ex, "Erreur lors de la validation de l'audit.");
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
                    _changeOkStateCommand = new RelayCommand<AuditItem>(async auditItem =>
                    {
                        AuditItem = auditItem;
                        await UpdateAudit(true);
                        var scrollTask = ScrollTo(AuditItem);
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
                    _changeNotOkStateCommand = new RelayCommand<AuditItem>(auditItem =>
                    {
                        AuditItem = auditItem;
                        // Ouverture de la popup pour le commentaire
                        ShowCommentCommand.Execute(auditItem);
                    });

                return _changeNotOkStateCommand;
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
                    });
                return _closeDetailsDialogCommand;
            }
        }

        /// <summary>
        /// Obtient la commande validant le changement de commentaire
        /// </summary>
        ICommand _textDialogValidateCommand;
        public ICommand TextDialogValidateCommand
        {
            get
            {
                if (_textDialogValidateCommand == null)
                    _textDialogValidateCommand = new RelayCommand<string>(async comment =>
                    {
                        AuditItem.IsOk = false;
                        if (AuditItem != null && AuditItem.Comment != TextDialogResult)
                            AuditItem.Comment = TextDialogResult;
                        await Locator.Navigation.PopModal();
                    });
                return _textDialogValidateCommand;
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
                        AuditItem = Audit.AuditItems[++_index];
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
                        AuditItem = Audit.AuditItems[--_index];
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
                if(_showAddAnomalyCommand == null)
                    _showAddAnomalyCommand = new RelayCommand<Anomaly>(async anomaly =>
                    {
                        try
                        {
                            await Locator.Navigation.PopModal();
                            _detailsIsOpened = false;

                            var vm = new AddInspectionAnomalyViewModel()
                            {
                                Anomaly = anomaly,
                                IsReadOnly = true,
                            };

                            vm.ClosedEventHandler += (sender, args) =>
                            {
                                if(ShowAnomaliesCommand.CanExecute(null))
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

        #region Private Methods

        void CreateAuditItems()
        {
            try
            {
                if (Publication != null && Audit != null)
                {
                    foreach (SurveyItem surveyItem in Audit.Survey.SurveyItems)
                    {
                        AuditItem auditItem = Audit.AuditItems.SingleOrDefault(_ => _.Number == surveyItem.Number);
                        if (auditItem == null)
                            Audit.AuditItems.Add(new AuditItem(surveyItem));
                        else
                            auditItem.SurveyItem = surveyItem;
                    }

                    RaisePropertyChanged(nameof(ShowValidateAction));
                    RaisePropertyChanged(nameof(ShowSleepAction));
                }
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de création de l'affichage de l'inspection.");
            }
        }

        Task UpdateAudit(bool? isOk, string comment = null)
        {
            Locator.Main.IsLoading = true;
            Locator.Main.ShowDisconnectedMessage = false;

            try
            {
                if (isOk == null)
                {
                    AuditItem.IsOk = null;
                    AuditItem.Comment = null;
                    AuditItem.Photo = null;
                }
                else if (isOk == true)
                {
                    AuditItem.IsOk = true;
                    AuditItem.Comment = null;
                    AuditItem.Photo = null;
                }
                else
                {
                    // TODO Ajouter la photo
                    AuditItem.IsOk = false;
                    AuditItem.Comment = comment;
                    AuditItem.Photo = null;
                }

                OfflineFile.Inspection.SaveToJson(Publication);
            }
            catch (Exception ex)
            {
                Locator.TraceManager.TraceError(ex, "Erreur lors de la mise à jour de l'audit.");
            }
            finally
            {
                RaisePropertyChanged(nameof(ShowValidateAction));
                RaisePropertyChanged(nameof(ShowSleepAction));

                if (_detailsIsOpened)
                    _index = Audit.AuditItems.IndexOf(AuditItem);

                CreateAuditItems();
                RaisePropertyChanged(nameof(AuditItem));
                RaisePropertyChanged(nameof(NextVisibility));
                RaisePropertyChanged(nameof(PreviousVisibility));

                Locator.Main.IsLoading = false;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode permettant de rafraichir les données si besoin
        /// </summary>
        public Task Refresh()
        {
            RaisePropertyChanged(nameof(LinkedAnomalies));
            RaisePropertyChanged(nameof(OutsidedAnomalies));
            RaisePropertyChanged(nameof(AllAnomalies));

            RaisePropertyChanged(nameof(ShowValidateAction));
            RaisePropertyChanged(nameof(ShowSleepAction));

            return Task.CompletedTask;
        }

        #endregion
    }
}