using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kprocess.KL2.TabletClient.Controls;
using Kprocess.KL2.TabletClient.Models;
using KProcess.Ksmed.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    public class AddInspectionAnomalyViewModel : ViewModelBase, IWaitResultViewModel<MemoryStream>
    {
        public AddInspectionAnomalyViewModel()
        {
            ModifyAnomalyFromDialog = false;
        }

        Views.AddInspectionAnomaly _view;
        EtiquetteAnomaly _backupAnomaly;

        public EventHandler ClosedEventHandler;

        public double TextDialogHeight { get; } = 300;

        public double TextDialogWidth { get; } = 600;

        public string TextDialogTitle { get; } = "Description du problème";

        public string TextDialogResult { get; set; }

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

        string _title = string.Empty;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool ModifyAnomalyFromDialog { get; set; }

        /// <summary>
        /// Obtient ou définit la publication courante
        /// </summary>
        public Publication Publication =>
            Locator.Main.InspectionPublication;

        /// <summary>
        /// Obtient ou définit l'inspection concernée
        /// </summary>
        public Inspection Inspection =>
            Locator.Main.Inspection;

        /// <summary>
        /// Obtient ou définit la PublishedAction concernée
        /// </summary>
        PublishedAction _publishedAction;
        public PublishedAction PublishedAction
        {
            get => _publishedAction;
            set
            {
                if (_publishedAction != value)
                {
                    _publishedAction = value;
                    RaisePropertyChanged();
                }
            }
        }

        MemoryStream _captureStream;
        public MemoryStream CaptureStream
        {
            get => _captureStream;
            set
            {
                if (_captureStream != value)
                {
                    _captureStream = value;
                    RaisePropertyChanged();
                }
            }
        }

        AnomalyType _selectedKind;
        public AnomalyType SelectedKind
        {
            get => _selectedKind;
            set
            {
                if (_selectedKind != value)
                {
                    _selectedKind = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    RaisePropertyChanged();
                }
            }
        }

        Anomaly _anomaly;
        public Anomaly Anomaly
        {
            get => _anomaly;
            set
            {
                if (_anomaly != value)
                {
                    _anomaly = value;
                    RaisePropertyChanged();
                }
            }
        }

        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommand<RoutedEventArgs>(routedEventArgs =>
                    {
                        if (!(routedEventArgs.Source is Views.AddInspectionAnomaly view) || !(view.MyEtiquettesControl is EtiquettesControl etiquettesControl))
                            return;
                        _view = view;
                        EtiquetteAnomaly etiquetteToLoad = null;
                        if (Anomaly == null)
                            Anomaly = PublishedAction?.InspectionStep?.Anomaly;
                        if (Anomaly != null)
                        {
                            if (CaptureStream == null)
                                CaptureStream = Anomaly.Photo == null ? null : new MemoryStream(Anomaly.Photo.ToArray());
                            IAnomalyKindItem selectedAnomalyKindItem = null;
                            /*foreach (var editable in etiquettesControl.EtiquetteSecurity.PossibleAnomalies.OfType<AnomalyKindEditableItem>()
                                .Concat(etiquettesControl.EtiquetteMaintenance.PossibleAnomalies.OfType<AnomalyKindEditableItem>())
                                .Concat(etiquettesControl.EtiquetteOperator.PossibleAnomalies.OfType<AnomalyKindEditableItem>()))
                                editable.Value = null;*/
                            switch (Anomaly.Type)
                            {
                                case AnomalyType.Security:
                                    etiquetteToLoad = etiquettesControl.EtiquetteSecurity;
                                    break;
                                case AnomalyType.Maintenance:
                                    etiquetteToLoad = etiquettesControl.EtiquetteMaintenance;
                                    break;
                                case AnomalyType.Operator:
                                    etiquetteToLoad = etiquettesControl.EtiquetteOperator;
                                    break;
                            }
                            etiquetteToLoad.Description = Anomaly.Description;
                            etiquetteToLoad.Line = Anomaly.Line;
                            etiquetteToLoad.Machine = Anomaly.Machine;
                            etiquetteToLoad.SelectedPriority = Anomaly.Priority ?? AnomalyPriority.A;
                            selectedAnomalyKindItem = etiquetteToLoad.PossibleAnomalies.SingleOrDefault(_ => _.Category == Anomaly.Category && _.Label == Anomaly.Label);
                            etiquetteToLoad.SelectedAnomalyKindItem = selectedAnomalyKindItem;
                            etiquettesControl.SelectedKind = Anomaly.Type;
                        }
                        if (_backupAnomaly != null)
                        {
                            switch (_backupAnomaly.Kind)
                            {
                                case AnomalyType.Security:
                                    etiquetteToLoad = etiquettesControl.EtiquetteSecurity;
                                    break;
                                case AnomalyType.Maintenance:
                                    etiquetteToLoad = etiquettesControl.EtiquetteMaintenance;
                                    break;
                                case AnomalyType.Operator:
                                    etiquetteToLoad = etiquettesControl.EtiquetteOperator;
                                    break;
                            }
                            etiquetteToLoad.Description = _backupAnomaly.Description;
                            etiquetteToLoad.Line = _backupAnomaly.Line;
                            etiquetteToLoad.Machine = _backupAnomaly.Machine;
                            etiquetteToLoad.SelectedPriority = _backupAnomaly.SelectedPriority;
                            if (_backupAnomaly.SelectedAnomalyKindItem != null)
                                etiquetteToLoad.SelectedAnomalyKindItem = etiquetteToLoad.PossibleAnomalies[_backupAnomaly.PossibleAnomalies.IndexOf(_backupAnomaly.SelectedAnomalyKindItem)];
                            etiquettesControl.SelectedKind = _backupAnomaly.Kind;
                            _backupAnomaly = null;
                        }
                    });
                return _loadedCommand;
            }
        }

        ICommand _closeDialogCommand;
        public ICommand CloseDialogCommand
        {
            get
            {
                if (_closeDialogCommand == null)
                    _closeDialogCommand = new RelayCommand(async () =>
                    {
                        CaptureStream = null;
                        Description = null;
                        await Locator.Navigation.Pop();
                        ClosedEventHandler?.Invoke(this, EventArgs.Empty);
                    });
                return _closeDialogCommand;
            }
        }

        ICommand _retryPhotoCommand;
        public ICommand RetryPhotoCommand
        {
            get
            {
                if (_retryPhotoCommand == null)
                    _retryPhotoCommand = new RelayCommand(async () =>
                    {
                        if (IsReadOnly)
                            return;
                        if (_view != null)
                        {
                            switch (_view.MyEtiquettesControl.SelectedKind)
                            {
                                case AnomalyType.Security:
                                    _backupAnomaly = _view.MyEtiquettesControl.EtiquetteSecurity;
                                    break;
                                case AnomalyType.Maintenance:
                                    _backupAnomaly = _view.MyEtiquettesControl.EtiquetteMaintenance;
                                    break;
                                case AnomalyType.Operator:
                                    _backupAnomaly = _view.MyEtiquettesControl.EtiquetteOperator;
                                    break;
                                default:
                                    _backupAnomaly = null;
                                    break;
                            }
                        }

                        await Locator.Navigation.Push<Views.Snapshot, SnapshotViewModel>(new SnapshotViewModel() { ReturnAfterPhoto = true });
                    }, () =>
                    {
                        return !IsReadOnly;
                    });
                return _retryPhotoCommand;
            }
        }

        ICommand _validateCommand;
        public ICommand ValidateCommand
        {
            get
            {
                if (_validateCommand == null)
                    _validateCommand = new RelayCommand<EtiquettesControl>(async (etiquettesControl) =>
                    {
                        Locator.Main.IsLoading = true;
                        Locator.Main.ShowDisconnectedMessage = false;

                        ViewModelBase predecessor = Locator.Navigation.GetFirstPredecessor(typeof(InspectionViewModel), typeof(AuditViewModel));

                        if (!ModifyAnomalyFromDialog)
                        {
                            if (PublishedAction == null)
                                CreateAnomalyForPublishAction(etiquettesControl, predecessor);
                            else
                                UpdateAnomalyForPublishAction(etiquettesControl, predecessor);
                        }
                        else if (Anomaly != null && ModifyAnomalyFromDialog)
                            GetEtiquettesControlValue(etiquettesControl, Anomaly);


                        OfflineFile.Inspection.SaveToJson(Publication);

                        Locator.Main.IsLoading = false;

                        if (ClosedEventHandler != null)
                        {
                            await Locator.Navigation.Pop();
                            ClosedEventHandler.Invoke(this, EventArgs.Empty);
                        }
                        else
                            await Locator.Navigation.Pop(typeof(InspectionViewModel), typeof(AuditViewModel));
                    }, (etiquettesControl) =>
                    {
                        switch (etiquettesControl.SelectedKind)
                        {
                            case AnomalyType.Security:
                                //return (etiquettesControl.EtiquetteSecurity.SelectedAnomalyKindItem is AnomalyKindEditableItem editableS) ? !string.IsNullOrEmpty(editableS.Value) : etiquettesControl.EtiquetteSecurity.SelectedAnomalyKindItem != null;
                                return etiquettesControl.EtiquetteSecurity.SelectedAnomalyKindItem != null;
                            case AnomalyType.Maintenance:
                                //return (etiquettesControl.EtiquetteMaintenance.SelectedAnomalyKindItem is AnomalyKindEditableItem editableM) ? !string.IsNullOrEmpty(editableM.Value) : etiquettesControl.EtiquetteMaintenance.SelectedAnomalyKindItem != null;
                                return etiquettesControl.EtiquetteMaintenance.SelectedAnomalyKindItem != null;
                            case AnomalyType.Operator:
                                //return (etiquettesControl.EtiquetteOperator.SelectedAnomalyKindItem is AnomalyKindEditableItem editableO) ? !string.IsNullOrEmpty(editableO.Value) : etiquettesControl.EtiquetteOperator.SelectedAnomalyKindItem != null;
                                return etiquettesControl.EtiquetteOperator.SelectedAnomalyKindItem != null;
                        }
                        return false;
                    });
                return _validateCommand;
            }
        }



        void CreateAnomalyForPublishAction(EtiquettesControl etiquettesControl, ViewModelBase predecessor)
        {
            var anomaly = new Anomaly
            {
                Photo = CaptureStream?.ToArray(),
                InspectorId = Locator.Main.SelectedUser.UserId,
                Date = DateTime.Now,
                Type = etiquettesControl.SelectedKind,
                Origin = predecessor is AuditViewModel ? AnomalyOrigin.Audit : AnomalyOrigin.Inspection
            };
            GetEtiquettesControlValue(etiquettesControl, anomaly);
            Inspection.Anomalies.Add(anomaly);
        }

        void UpdateAnomalyForPublishAction(EtiquettesControl etiquettesControl, ViewModelBase predecessor)
        {
            if (PublishedAction.InspectionStep.Anomaly == null)
            {
                if (PublishedAction.InspectionStep.ChangeTracker.ModifiedValues.ContainsKey(nameof(InspectionStep.Anomaly))) // Une précédente anomalie est marquée comme supprimée, on doit la restaurer
                {
                    Anomaly deletedAnomaly = (Anomaly)PublishedAction.InspectionStep.ChangeTracker.OriginalValues[nameof(InspectionStep.Anomaly)];
                    deletedAnomaly.MarkAsUnchanged();
                    deletedAnomaly.InspectorId = Locator.Main.SelectedUser.UserId;
                    deletedAnomaly.Date = DateTime.Now;
                    deletedAnomaly.Type = etiquettesControl.SelectedKind;
                    PublishedAction.InspectionStep.Anomaly = deletedAnomaly;
                }
                else
                {
                    Anomaly newAnomaly = new Anomaly
                    {
                        InspectorId = Locator.Main.SelectedUser.UserId,
                        Date = DateTime.Now,
                        Type = etiquettesControl.SelectedKind,
                        InspectionId = PublishedAction.InspectionStep.Inspection.InspectionId,
                        Origin = predecessor is AuditViewModel ? AnomalyOrigin.Audit : AnomalyOrigin.Inspection
                    };
                    PublishedAction.InspectionStep.Inspection.Anomalies.Add(newAnomaly);
                    PublishedAction.InspectionStep.Anomaly = newAnomaly;
                }
            }
            else
                PublishedAction.InspectionStep.Anomaly.Type = etiquettesControl.SelectedKind;

            if (PublishedAction.InspectionStep.Anomaly.IsMarkedAsAdded || CaptureStream != null)
            {
                if (PublishedAction.InspectionStep.Anomaly.Photo == null || !PublishedAction.InspectionStep.Anomaly.Photo.SequenceEqual(CaptureStream?.ToArray()))
                    PublishedAction.InspectionStep.Anomaly.Photo = CaptureStream?.ToArray();
            }

            GetEtiquettesControlValue(etiquettesControl, PublishedAction.InspectionStep.Anomaly);

            PublishedAction.InspectionStep.IsOk = false;
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
                parentInspectionStep.IsOk = false;
                parentInspectionStep.Date = PublishedAction.InspectionStep.Date;
                parentInspectionStep.InspectorId = PublishedAction.InspectionStep.InspectorId;
                parentAction.IsOk = parentInspectionStep.IsOk;
                parentAction.InspectionDate = !parentAction.IsOk.HasValue ? null : parentInspectionStep.Date.ToShortDateString();
                parentAction.InspectBy = !parentAction.IsOk.HasValue ? string.Empty : Locator.Main.Users.Single(_ => _.UserId == parentInspectionStep.InspectorId).ToString();
            }
        }

        void GetEtiquettesControlValue(EtiquettesControl etiquettesControl, Anomaly currentAnomaly)
        {
            currentAnomaly.Type = etiquettesControl.SelectedKind;

            EtiquetteAnomaly etiquetteAnomaly = null;
            switch (etiquettesControl.SelectedKind)
            {
                case AnomalyType.Security:
                    etiquetteAnomaly = etiquettesControl.EtiquetteSecurity;
                    break;
                case AnomalyType.Maintenance:
                    etiquetteAnomaly = etiquettesControl.EtiquetteMaintenance;
                    break;
                case AnomalyType.Operator:
                    etiquetteAnomaly = etiquettesControl.EtiquetteOperator;
                    break;
            }

            if(etiquetteAnomaly == null)
                return;

            currentAnomaly.Description = etiquetteAnomaly.Description;
            currentAnomaly.Line = etiquetteAnomaly.Line;
            currentAnomaly.Machine = etiquetteAnomaly.Machine;
            currentAnomaly.Priority = etiquetteAnomaly.SelectedPriority;
            currentAnomaly.Label = etiquetteAnomaly.SelectedAnomalyKindItem?.Label;
            //currentAnomaly.Label = (etiquetteAnomaly.SelectedAnomalyKindItem as AnomalyKindEditableItem)?.Value ?? etiquetteAnomaly.SelectedAnomalyKindItem?.Label;
            currentAnomaly.Category = etiquetteAnomaly.SelectedAnomalyKindItem?.Category;
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
                        switch (_view.MyEtiquettesControl.SelectedKind)
                        {
                            case AnomalyType.Security:
                                _view.MyEtiquettesControl.EtiquetteSecurity.Description = TextDialogResult;
                                break;
                            case AnomalyType.Maintenance:
                                _view.MyEtiquettesControl.EtiquetteMaintenance.Description = TextDialogResult;
                                break;
                            case AnomalyType.Operator:
                                _view.MyEtiquettesControl.EtiquetteOperator.Description = TextDialogResult;
                                break;
                        }
                        await Locator.Navigation.PopModal();
                    });
                return _textDialogValidateCommand;
            }
        }

        public Task ComputeResult(MemoryStream result)
        {
            CaptureStream = result;
            return Task.CompletedTask;
        }
    }
}
