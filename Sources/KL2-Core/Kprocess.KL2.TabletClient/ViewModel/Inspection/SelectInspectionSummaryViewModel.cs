using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Converter;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Models.CriticalPathMethod;
using KProcess.Ksmed.Models;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using Unosquare.FFME.Common;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;

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
    public class SelectInspectionSummaryViewModel : ViewModelBase, IRefreshViewModel, IMediaElementViewModel, ISfDataGridViewModel<PublishedAction>
    {
        static TimeSpan thumbnailTimerDuration = TimeSpan.FromSeconds(5);
        static TimeSpan thumbnailUITimerInterval = TimeSpan.FromMilliseconds(10);

        bool _reopenParent;
        Stopwatch thumbnailTimer;
        DispatcherTimer thumbnailUITimer;

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

        public Publication Publication
        {
            get => Locator.Main.InspectionPublication;
            set
            {
                Locator.Main.IsLoading = true;
                Locator.Main.ShowDisconnectedMessage = false;

                if (Locator.Main.InspectionPublication != value)
                {
                    Locator.Main.InspectionPublication = value;
                    RaisePropertyChanged();
                }

                // TODO : Faire le calcul du chemin critique en incluant les tâches des sous-process
                var tempCriticalPath = Publication.PublishedActions.CriticalPath(p => p.Successors, p => p.BuildFinish - p.BuildStart);
                CriticalPath = new BindingList<PublishedAction>();
                /*for (int i = tempCriticalPath.Count(); i > 0; i--)
                    CriticalPath.Add(tempCriticalPath.ElementAt(i - 1));*/
                if (Locator.Main.InspectionPublication != null)
                    foreach (PublishedAction pAction in Publication.PublishedActions.Where(_ => !_.IsGroup))
                    {
                        if (pAction.LinkedPublication == null)
                            CriticalPath.Add(pAction);
                        else
                        {
                            foreach (PublishedAction pInnerAction in pAction.LinkedPublication.PublishedActions.Where(_ => !_.IsGroup))
                                CriticalPath.Add(pInnerAction);
                        }
                    }

                ((RelayCommand)PlayCriticalPathCommand).RaiseCanExecuteChanged();
                ShowDataGrid = true;

                Locator.Main.IsLoading = false;
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

        BindingList<PublishedAction> _criticalPath;
        public BindingList<PublishedAction> CriticalPath
        {
            get => _criticalPath;
            private set
            {
                if (_criticalPath != null)
                    _criticalPath.ListChanged -= RaisePlayCriticalPathCommandCanExecuteChanged;
                if (_criticalPath != value)
                {
                    _criticalPath = value;
                    RaisePropertyChanged();
                }
                if (_criticalPath != null)
                    _criticalPath.ListChanged += RaisePlayCriticalPathCommandCanExecuteChanged;
                RaisePlayCriticalPathCommandCanExecuteChanged(null, null);
            }
        }

        void RaisePlayCriticalPathCommandCanExecuteChanged(object sender, ListChangedEventArgs e) =>
            (PlayCriticalPathCommand as RelayCommand).RaiseCanExecuteChanged();

        PublishedAction _criticalPathCurrentPublishedAction;
        public PublishedAction CriticalPathCurrentPublishedAction
        {
            get { return _criticalPathCurrentPublishedAction; }
            set
            {
                if (value?.IsGroup == true)
                {
                    if (_criticalPathCurrentPublishedAction != null)
                    {
                        _criticalPathCurrentPublishedAction = null;
                        RaisePropertyChanged();
                    }
                }
                else
                {
                    if (_criticalPathCurrentPublishedAction != value)
                    {
                        _criticalPathCurrentPublishedAction = value;
                        RaisePropertyChanged();
                    }
                }
                OnCriticalPathCurrentPublishedActionChanged();
            }
        }
        async void OnCriticalPathCurrentPublishedActionChanged()
        {
            if (_criticalPathCurrentPublishedAction != null)
            {
                // On affiche le détails de la tâche si ce n'est pas déjà fait
                var criticalPathDialog = await Locator.Main.GetCurrentDialogAsync<CriticalPathDialog>(this);
                if (criticalPathDialog == null)
                {
                    await Locator.Navigation.PushDialog<CriticalPathDialog, SelectInspectionSummaryViewModel>(this);
                    if (string.IsNullOrEmpty(CriticalPathCurrentPublishedAction.CutVideoHash))
                    {
                        MediaElement.MediaEnded -= Player_MediaEnded;
                        MediaElement.Source = null;
                        StartThumbnailTimer();
                    }
                    else
                    {
                        MediaElement.MediaEnded -= Player_MediaEnded;
                        MediaElement.Source = (Uri)(new HashToUriConverter()).Convert(CriticalPathCurrentPublishedAction.CutVideoHash, typeof(Uri), null, null);
                        MediaElement.MediaEnded += Player_MediaEnded;
                    }
                }
            }
        }

        void Player_MediaEnded(object sender, EventArgs e)
        {
            ((RelayCommand)NextCriticalActionCommand).Execute(null);
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

        bool _videoIsMaximized;
        public bool VideoIsMaximized
        {
            get => _videoIsMaximized || PublishedAction?.IsGroup == true;
            set
            {
                if (_videoIsMaximized != value)
                {
                    _videoIsMaximized = value;
                    RaisePropertyChanged();
                }
            }
        }

        TimeSpan _thumbnailPosition;
        public TimeSpan ThumbnailPosition
        {
            get => _thumbnailPosition;
            set
            {
                if (_thumbnailPosition != value)
                {
                    _thumbnailPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        MediaPlaybackState _thumbnailMediaState;
        public MediaPlaybackState ThumbnailMediaState
        {
            get => _thumbnailMediaState;
            set
            {
                if (_thumbnailMediaState != value)
                {
                    _thumbnailMediaState = value;
                    RaisePropertyChanged();
                }
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

        ICommand _playCriticalPathCommand;
        public ICommand PlayCriticalPathCommand
        {
            get
            {
                if (_playCriticalPathCommand == null)
                    _playCriticalPathCommand = new RelayCommand(() =>
                    {
                        CriticalPathCurrentPublishedAction = CriticalPath.First();
                    }, () =>
                    {
                        return CriticalPath?.Any() == true && Publication != null;
                    });
                return _playCriticalPathCommand;
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
                        if (_criticalPathCurrentPublishedAction != null) // Si on est en cours de lecture du chemin critique, on met en pause
                        {
                            if (MediaElement.IsPlaying)
                                MediaElement.Pause();
                            else if (thumbnailTimer?.IsRunning == true)
                                ((RelayCommand)PlayPauseThumbnailCommand).Execute(null);
                        }
                        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    });
                return _hyperlinkRequestNavigateCommand;
            }
        }

        ICommand _closeDetailsDialogCommand;
        public ICommand CloseDetailsDialogCommand
        {
            get
            {
                if (_closeDetailsDialogCommand == null)
                    _closeDetailsDialogCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.PopModal();

                        if (_reopenParent)
                        {
                            PublishedAction nextPublishedAction = Publication.PublishedActions[_indexParent.Value];
                            ShowStepCommand.Execute(nextPublishedAction);
                            _reopenParent = false;
                        }
                        else
                        {
                            var scrollTask = ScrollTo(PublishedAction);
                        }
                    });
                return _closeDetailsDialogCommand;
            }
        }

        ICommand _closeCriticalPathDialogCommand;
        public ICommand CloseCriticalPathDialogCommand
        {
            get
            {
                if (_closeCriticalPathDialogCommand == null)
                    _closeCriticalPathDialogCommand = new RelayCommand(async () =>
                    {
                        if (MediaElement != null && MediaElement.MediaState != MediaPlaybackState.Close)
                            await MediaElement.Stop();
                        StopThumbnailTimer();
                        MediaElement.MediaEnded -= Player_MediaEnded;
                        await Locator.Navigation.PopModal();
                        CriticalPathCurrentPublishedAction = null;
                    });
                return _closeCriticalPathDialogCommand;
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

                            await Locator.Navigation.PushDialog<InspectionSummaryDetailsDialog, SelectInspectionSummaryViewModel>(this);
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
                    }, () =>
                    {
                        if (Publication == null)
                            return false;
                        if (IndexParent.HasValue && Publication.PublishedActions[IndexParent.Value].LinkedPublication != null)
                            return Publication.PublishedActions[IndexParent.Value].LinkedPublication.PublishedActions.IndexOf(PublishedAction) > 0;
                        return Publication.PublishedActions.IndexOf(PublishedAction) > 0;
                    });
                return _previousActionDetailsCommand;
            }
        }

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
                    }, () =>
                    {
                        if (Publication == null)
                            return false;
                        if (_indexParent != null)
                            return Publication.PublishedActions[IndexParent.Value].LinkedPublication.PublishedActions.IndexOf(PublishedAction) + 1 < Publication.PublishedActions[IndexParent.Value].LinkedPublication.PublishedActions.Count;
                        return Publication.PublishedActions.IndexOf(PublishedAction) + 1 < Publication.PublishedActions.Count;
                    });
                return _nextActionDetailsCommand;
            }
        }

        ICommand _previousCriticalActionCommand;
        public ICommand PreviousCriticalActionCommand
        {
            get
            {
                if (_previousCriticalActionCommand == null)
                    _previousCriticalActionCommand = new RelayCommand(() =>
                    {
                        StopThumbnailTimer();
                        CriticalPathCurrentPublishedAction = CriticalPath[CriticalPath.IndexOf(CriticalPathCurrentPublishedAction) - 1];
                        (PreviousCriticalActionCommand as RelayCommand).RaiseCanExecuteChanged();
                        (NextCriticalActionCommand as RelayCommand).RaiseCanExecuteChanged();
                        if (string.IsNullOrEmpty(CriticalPathCurrentPublishedAction.CutVideoHash))
                        {
                            MediaElement.Source = null;
                            StartThumbnailTimer();
                        }
                        else
                            MediaElement.Source = (Uri)(new HashToUriConverter()).Convert(CriticalPathCurrentPublishedAction.CutVideoHash, typeof(Uri), null, null);
                    }, () =>
                    {
                        int index = CriticalPath.IndexOf(CriticalPathCurrentPublishedAction);
                        return index > 0;
                    });
                return _previousCriticalActionCommand;
            }
        }

        ICommand _nextCriticalActionCommand;
        public ICommand NextCriticalActionCommand
        {
            get
            {
                if (_nextCriticalActionCommand == null)
                    _nextCriticalActionCommand = new RelayCommand(() =>
                    {
                        StopThumbnailTimer();
                        if (((RelayCommand)NextCriticalActionCommand).CanExecute(null))
                        {
                            CriticalPathCurrentPublishedAction = CriticalPath[CriticalPath.IndexOf(CriticalPathCurrentPublishedAction) + 1];
                            (PreviousCriticalActionCommand as RelayCommand).RaiseCanExecuteChanged();
                            (NextCriticalActionCommand as RelayCommand).RaiseCanExecuteChanged();
                            if (string.IsNullOrEmpty(CriticalPathCurrentPublishedAction.CutVideoHash))
                            {
                                MediaElement.MediaEnded -= Player_MediaEnded;
                                MediaElement.Source = null;
                                StartThumbnailTimer();
                            }
                            else
                            {
                                MediaElement.MediaEnded -= Player_MediaEnded;
                                MediaElement.Source = (Uri)(new HashToUriConverter()).Convert(CriticalPathCurrentPublishedAction.CutVideoHash, typeof(Uri), null, null);
                                MediaElement.MediaEnded += Player_MediaEnded;
                            }
                        }
                        else
                            (CloseCriticalPathDialogCommand as RelayCommand).Execute(null);
                    }, () =>
                    {
                        int index = CriticalPath.IndexOf(CriticalPathCurrentPublishedAction);
                        return index + 1 < CriticalPath.Count;
                    });
                return _nextCriticalActionCommand;
            }
        }

        ICommand _playPauseThumbnailCommand;
        public ICommand PlayPauseThumbnailCommand
        {
            get
            {
                if (_playPauseThumbnailCommand == null)
                    _playPauseThumbnailCommand = new RelayCommand(() =>
                    {
                        if (ThumbnailMediaState == MediaPlaybackState.Pause)
                        {
                            thumbnailTimer?.Start();
                            ThumbnailMediaState = MediaPlaybackState.Play;
                        }
                        else if (ThumbnailMediaState == MediaPlaybackState.Play)
                        {
                            thumbnailTimer?.Stop();
                            ThumbnailMediaState = MediaPlaybackState.Pause;
                        }
                    });
                return _playPauseThumbnailCommand;
            }
        }

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

        void StartThumbnailTimer()
        {
            thumbnailTimer = new Stopwatch();
            thumbnailUITimer = new DispatcherTimer
            {
                Interval = thumbnailUITimerInterval
            };
            thumbnailUITimer.Tick += (o, e) =>
            {
                ThumbnailPosition = TimeSpan.FromMilliseconds(thumbnailTimer.ElapsedMilliseconds);
                if (_thumbnailPosition >= thumbnailTimerDuration)
                {
                    StopThumbnailTimer();
                    ((RelayCommand)NextCriticalActionCommand).Execute(null);
                }
            };
            ThumbnailPosition = TimeSpan.FromTicks(0);
            thumbnailTimer.Start();
            thumbnailUITimer.Start();
            ThumbnailMediaState = MediaPlaybackState.Play;
        }

        void StopThumbnailTimer()
        {
            thumbnailUITimer?.Stop();
            thumbnailTimer?.Stop();
            thumbnailUITimer = null;
            thumbnailTimer = null;
            ThumbnailMediaState = MediaPlaybackState.Stop;
        }

        #region Properties

        /// <summary>
        /// Obtient un booléen indiquant si on doit afficher ou non la grille
        /// </summary>
        bool _showDataGrid;
        public bool ShowDataGrid
        {
            get => _showDataGrid;
            private set
            {
                if (_showDataGrid != value)
                {
                    _showDataGrid = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Implement IRefreshViewModel 

        public Task Refresh()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}