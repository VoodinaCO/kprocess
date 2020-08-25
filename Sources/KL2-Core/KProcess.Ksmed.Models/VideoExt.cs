using Kprocess.KL2.FileTransfer;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    [Serializable]
    partial class Video : IAuditableUserRequired
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>
        /// Une énumération des erreurs de validation, ou null s'il n'y en a pas.
        /// </returns>
        [SelfValidation]
        protected override IEnumerable<ValidationError> OnCustomValidate()
        {
            foreach (ValidationError error in base.OnCustomValidate())
                yield return error;

            if (DefaultResource != null)
            {
                DefaultResource.Validate();
                if (!DefaultResource.IsValid.GetValueOrDefault() && DefaultResource.Errors.Any())
                    yield return new ValidationError(nameof(DefaultResource), DefaultResource.Errors.First().Message, DefaultResource.Errors);
            }
        }

        // TODO : Changer l'uri pour pointer vers le dossier de téléchargement des fichiers
        /// <summary>
        /// Obtient le chemin vers le fichier sous forme d'Uri.
        /// </summary>
        public Uri Source
        {
            get
            {
                if (IsSync)
                    return new Uri(Path.Combine(Preferences.SyncDirectory, Filename));
                else if (OnServer == true)
                    return new Uri($"{Preferences.FileServerUri}/Stream/{Filename}");
                return File.Exists(_filePath) ? new Uri(_filePath) : null;
            }
        }

        /// <summary>
        /// Obtient le nom du fichier (sans le chemin).
        /// </summary>
        public string Filename =>
            $"{Hash}{Extension}";

        /// <summary>
        /// Obtient ou définit la vue de la ressource
        /// </summary>
        public ResourceView View
        {
            get => ResourceView.HasValue ? Enums.ResourceViews[ResourceView.Value] : null;
            set
            {
                ResourceView = value?.Id;
                OnPropertyChanged();
            }
        }

        public event TransferFinished OnTransferFinished;
        public delegate void TransferFinished(Video sender, JobType jobType);

        /// <summary>
        /// Obtient ou définit la progression du téléchargement ou de l'envoi de la vidéo
        /// </summary>
        ITransferOperation _transfer;
        public ITransferOperation Transfer
        {
            get => _transfer;
            set
            {
                if (_transfer != null)
                    _transfer.OnTransferFinished -= Transfer_OnTransferFinished;
                _transfer = value;
                if (_transfer != null)
                    _transfer.OnTransferFinished += Transfer_OnTransferFinished;
                OnPropertyChanged();
            }
        }

        void Transfer_OnTransferFinished(object sender, JobType e) =>
            OnTransferFinished?.Invoke(this, e);

        public event EventHandler OnTranscodingFinished;

        /// <summary>
        /// Obtient ou définit la progression du transcoding lors de l'envoi de la vidéo
        /// </summary>
        double? _transcodingProgress;
        public double? TranscodingProgress
        {
            get => _transcodingProgress;
            set
            {
                if (_transcodingProgress != value)
                {
                    bool raiseFinished = _transcodingProgress != null && value == null;
                    _transcodingProgress = value;
                    OnPropertyChanged();
                    if (_transcodingProgress != null)
                        Transfer = null;
                    if (raiseFinished)
                        OnTranscodingFinished?.Invoke(this, null);
                }
            }
        }

        /// <summary>
        /// Obtient si la vidéo est présente dans le répertoire de téléchargement
        /// </summary>
        public bool IsSync =>
            File.Exists(Path.Combine(Preferences.SyncDirectory, Filename));

        /// <summary>
        /// Obtient si la vidéo est présente dans le répertoire de téléchargement
        /// </summary>
        public bool DeleteLocal(IList<Video> videos)
        {
            try
            {
                File.Delete(Path.Combine(Preferences.SyncDirectory, Filename));
                OnPropertyChanged(nameof(IsSync));
                return true;
            }
            catch (IOException) // If file is in use, delete it later
            {
                var taskScheduler = TaskScheduler.Current;
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    File.Delete(Path.Combine(Preferences.SyncDirectory, Filename));
                }).ContinueWith(t => videos.SingleOrDefault(_ => _.VideoId == VideoId)?.PropertyChangedManager.NotifyPropertyChanged(nameof(IsSync)), taskScheduler);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SendVideo(Dictionary<string, double> transcodingProgresses)
        {
            if (Sync && OnServer != true)
            {
                var fileTransferManager = IoC.Resolve<FileTransferManager>();
                if (fileTransferManager.DownloadOperations.ContainsKey(Filename))
                    return;
                if (fileTransferManager.UploadOperations.ContainsKey(Filename))
                    return;
                if (transcodingProgresses?.ContainsKey($"TRANSCODED_{OriginalHash}{Extension}") == true)
                    return;
                var uploadOperation = fileTransferManager.CreateUpload($"{Filename}", $"{Preferences.FileServerUri}/{Filename}", FilePath);
                uploadOperation.Resume();
            }
        }

        public override string ToString() =>
            string.Format("{0}{1}{2}{3}",
                string.IsNullOrEmpty(CameraName) ? string.Empty : $"{CameraName} - ",
                DefaultResource == null ? string.Empty : $"{DefaultResource.Label} - ",
                View == null ? string.Empty : $"{View.Label} - ",
                NumSeq);

        [DataMember]
        public bool IsUsed { get; set; }

        bool _canChangeSync;
        public bool CanChangeSync
        {
            get => _canChangeSync;
            set
            {
                if (_canChangeSync != value)
                {
                    _canChangeSync = value;
                    OnPropertyChanged();
                }
            }
        }

        string _canChangeSyncTooltip;
        public string CanChangeSyncTooltip
        {
            get => _canChangeSyncTooltip;
            set
            {
                if (_canChangeSyncTooltip != value)
                {
                    _canChangeSyncTooltip = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SetNumSeqToNextAvailable(IList<Video> videos)
        {
            Func<Video, bool> where = _ => _.VideoId != VideoId
                                           && _.CameraName == CameraName
                                           && _.DefaultResourceId == DefaultResourceId;
            if (DefaultResourceId != null)
                where = _ => _.VideoId != VideoId
                             && _.CameraName == CameraName
                             && _.DefaultResourceId == DefaultResourceId
                             && _.ResourceView == ResourceView;
            var similaryVideosNums = videos?
                .Where(where)
                .Select(_ => _.NumSeq)
                .ToList();
            int currentNumSeq = 1;
            while (similaryVideosNums?.Contains(currentNumSeq) == true)
                currentNumSeq++;
            NumSeq = currentNumSeq;
        }
    }
}
