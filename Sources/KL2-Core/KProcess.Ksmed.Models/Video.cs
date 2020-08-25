//------------------------------------------------------------------------------
// <automatiquement-généré>
//     Ce code a été généré depuis une template.
//
//     Les changements effectués directement sur ce fichier risquent d'être
//     perdus à la prochaine génération.
// </utomatiquement-généré>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using KProcess.Business;
using System.Linq;
using KProcess.Ksmed.Models.Validation;
using System.Web.Script.Serialization;

namespace KProcess.Ksmed.Models
{
    [DataContract(IsReference = true, Namespace = ModelsConstants.DataContractNamespace)]
    [KnownType(typeof(KAction))]
    [KnownType(typeof(Resource))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Procedure))]
    /// <summary>
    /// Représente une vidéo.
    /// </summary>
    public partial class Video : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Video";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Video"/>.
        /// </summary>
    	public Video()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _videoId;
        /// <summary>
        /// Obtient ou définit l'identifiant de cette vidéo.
        /// </summary>
        [DataMember]
        public int VideoId
        {
            get { return _videoId; }
            set
            {
                if (_videoId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'VideoId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _videoId = value;
                    OnEntityPropertyChanged("VideoId");
                }
            }
        }
        
        private Nullable<int> _defaultResourceId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la ressource par défaut de cette vidéo.
        /// </summary>
        [DataMember]
        public Nullable<int> DefaultResourceId
        {
            get { return _defaultResourceId; }
            set
            {
                if (_defaultResourceId != value)
                {
                    ChangeTracker.RecordValue("DefaultResourceId", _defaultResourceId, value);
                    if (!IsDeserializing)
                    {
                        if (DefaultResource != null && DefaultResource.ResourceId != value)
                        {
                            DefaultResource = null;
                        }
                    }
                    _defaultResourceId = value;
                    OnEntityPropertyChanged("DefaultResourceId");
                }
            }
        }
        
        private double _duration;
        /// <summary>
        /// Obtient ou définit la durée (en secondes).
        /// </summary>
        [DataMember]
        public double Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    ChangeTracker.RecordValue("Duration", _duration, value);
                    _duration = value;
                    OnEntityPropertyChanged("Duration");
                }
            }
        }
        
        private string _format;
        /// <summary>
        /// Obtient ou définit le format.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(FormatMaxLength, ErrorMessageResourceName = "Validation_Video_Format_StringLength")]
        public string Format
        {
            get { return _format; }
            set
            {
                if (_format != value)
                {
                    ChangeTracker.RecordValue("Format", _format, value);
                    _format = value;
                    OnEntityPropertyChanged("Format");
                }
            }
        }
        
        private string _filePath;
        /// <summary>
        /// Obtient ou définit le chemin vers le fichier.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Video_FilePath_Required")]
    	[LocalizableStringLength(FilePathMaxLength, ErrorMessageResourceName = "Validation_Video_FilePath_StringLength")]
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                if (_filePath != value)
                {
                    ChangeTracker.RecordValue("FilePath", _filePath, value);
    				var oldValue = _filePath;
                    _filePath = value;
                    OnEntityPropertyChanged("FilePath");
    				OnFilePathChanged(oldValue, value);
    				OnFilePathChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="FilePath"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnFilePathChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> FilePathChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="FilePath"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnFilePathChanged(string oldValue, string newValue)
    	{
    		if (FilePathChanged != null)
    			FilePathChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private System.DateTime _shootingDate;
        /// <summary>
        /// Obtient ou définit la date de prise de vue.
        /// </summary>
        [DataMember]
        public System.DateTime ShootingDate
        {
            get { return _shootingDate; }
            set
            {
                if (_shootingDate != value)
                {
                    ChangeTracker.RecordValue("ShootingDate", _shootingDate, value);
                    _shootingDate = value;
                    OnEntityPropertyChanged("ShootingDate");
                }
            }
        }
        
        private byte[] _thumbnail;
        /// <summary>
        /// Obtient ou définit la vignette.
        /// </summary>
        [DataMember]
        public byte[] Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (_thumbnail != value)
                {
                    ChangeTracker.RecordValue("Thumbnail", _thumbnail, value);
                    _thumbnail = value;
                    OnEntityPropertyChanged("Thumbnail");
                }
            }
        }
        
        private int _createdByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a créé l'entité.
        /// </summary>
        [DataMember]
        public int CreatedByUserId
        {
            get { return _createdByUserId; }
            set
            {
                if (_createdByUserId != value)
                {
                    ChangeTracker.RecordValue("CreatedByUserId", _createdByUserId, value);
                    if (!IsDeserializing)
                    {
                        if (Creator != null && Creator.UserId != value)
                        {
                            Creator = null;
                        }
                    }
                    _createdByUserId = value;
                    OnEntityPropertyChanged("CreatedByUserId");
                }
            }
        }
        
        private System.DateTime _creationDate;
        /// <summary>
        /// Obtient ou définit la date de création de l'entité.
        /// </summary>
        [DataMember]
        public System.DateTime CreationDate
        {
            get { return _creationDate; }
            set
            {
                if (_creationDate != value)
                {
                    ChangeTracker.RecordValue("CreationDate", _creationDate, value);
                    _creationDate = value;
                    OnEntityPropertyChanged("CreationDate");
                }
            }
        }
        
        private int _modifiedByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a en dernier modifié l'entité.
        /// </summary>
        [DataMember]
        public int ModifiedByUserId
        {
            get { return _modifiedByUserId; }
            set
            {
                if (_modifiedByUserId != value)
                {
                    ChangeTracker.RecordValue("ModifiedByUserId", _modifiedByUserId, value);
                    if (!IsDeserializing)
                    {
                        if (LastModifier != null && LastModifier.UserId != value)
                        {
                            LastModifier = null;
                        }
                    }
                    _modifiedByUserId = value;
                    OnEntityPropertyChanged("ModifiedByUserId");
                }
            }
        }
        
        private System.DateTime _lastModificationDate;
        /// <summary>
        /// Obtient ou définit la dernière date de modification de l'entité.
        /// </summary>
        [DataMember]
        public System.DateTime LastModificationDate
        {
            get { return _lastModificationDate; }
            set
            {
                if (_lastModificationDate != value)
                {
                    ChangeTracker.RecordValue("LastModificationDate", _lastModificationDate, value);
                    _lastModificationDate = value;
                    OnEntityPropertyChanged("LastModificationDate");
                }
            }
        }
        
        private byte[] _rowVersion;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] RowVersion
        {
            get { return _rowVersion; }
            set
            {
                if (_rowVersion != value)
                {
                    ChangeTracker.RecordValue("RowVersion", _rowVersion, value);
                    _rowVersion = value;
                    OnEntityPropertyChanged("RowVersion");
                }
            }
        }
        
        private int _processId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProcessId
        {
            get { return _processId; }
            set
            {
                if (_processId != value)
                {
                    ChangeTracker.RecordValue("ProcessId", _processId, value);
                    if (!IsDeserializing)
                    {
                        if (Process != null && Process.ProcessId != value)
                        {
                            Process = null;
                        }
                    }
                    _processId = value;
                    OnEntityPropertyChanged("ProcessId");
                }
            }
        }
        
        private string _cameraName;
        /// <summary>
        /// Obtient ou définit le nom de la caméra.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CameraNameMaxLength, ErrorMessageResourceName = "Validation_Video_CameraName_StringLength")]
        public string CameraName
        {
            get { return _cameraName; }
            set
            {
                if (_cameraName != value)
                {
                    ChangeTracker.RecordValue("CameraName", _cameraName, value);
                    _cameraName = value;
                    OnEntityPropertyChanged("CameraName");
                }
            }
        }
        
        private string _hash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Hash
        {
            get { return _hash; }
            set
            {
                if (_hash != value)
                {
                    ChangeTracker.RecordValue("Hash", _hash, value);
                    _hash = value;
                    OnEntityPropertyChanged("Hash");
                }
            }
        }
        
        private bool _onServer;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool OnServer
        {
            get { return _onServer; }
            set
            {
                if (_onServer != value)
                {
                    ChangeTracker.RecordValue("OnServer", _onServer, value);
                    _onServer = value;
                    OnEntityPropertyChanged("OnServer");
                }
            }
        }
        
        private string _extension;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    ChangeTracker.RecordValue("Extension", _extension, value);
                    _extension = value;
                    OnEntityPropertyChanged("Extension");
                }
            }
        }
        
        private bool _sync;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Sync
        {
            get { return _sync; }
            set
            {
                if (_sync != value)
                {
                    ChangeTracker.RecordValue("Sync", _sync, value);
                    _sync = value;
                    OnEntityPropertyChanged("Sync");
                }
            }
        }
        
        private string _originalHash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string OriginalHash
        {
            get { return _originalHash; }
            set
            {
                if (_originalHash != value)
                {
                    ChangeTracker.RecordValue("OriginalHash", _originalHash, value);
                    _originalHash = value;
                    OnEntityPropertyChanged("OriginalHash");
                }
            }
        }
        
        private int _numSeq;
        /// <summary>
        /// Obtient ou définit le numéro de séquence de la vidéo.
        /// </summary>
        [DataMember]
        public int NumSeq
        {
            get { return _numSeq; }
            set
            {
                if (_numSeq != value)
                {
                    ChangeTracker.RecordValue("NumSeq", _numSeq, value);
                    _numSeq = value;
                    OnEntityPropertyChanged("NumSeq");
                }
            }
        }

        #endregion

        #region Propriétés Enum
        
        private Nullable<ResourceViewEnum> _resourceView;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public Nullable<ResourceViewEnum> ResourceView
        {
            get { return _resourceView; }
            set
            {
                if (_resourceView != value)
                {
                    ChangeTracker.RecordValue("ResourceView", _resourceView, value);
                    _resourceView = value;
                    OnEntityPropertyChanged("ResourceView");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit les actions utilisant cette vidéo.
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new TrackableCollection<KAction>();
                    _actions.CollectionChanged += FixupActions;
                }
                return _actions;
            }
            set
            {
                if (!ReferenceEquals(_actions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actions != null)
                    {
                        _actions.CollectionChanged -= FixupActions;
                    }
                    _actions = value;
                    if (_actions != null)
                    {
                        _actions.CollectionChanged += FixupActions;
                    }
                    OnNavigationPropertyChanged("Actions");
                }
            }
        }
        private TrackableCollection<KAction> _actions;
    
        /// <summary>
        /// Obtient ou définit la ressource par défaut de cette vidéo.
        /// </summary>
        [DataMember]
        public Resource DefaultResource
        {
            get { return _defaultResource; }
            set
            {
                if (!ReferenceEquals(_defaultResource, value))
                {
                    var previousValue = _defaultResource;
                    _defaultResource = value;
                    FixupDefaultResource(previousValue);
                    OnNavigationPropertyChanged("DefaultResource");
    				OnDefaultResourceChanged(previousValue, value);
    				OnDefaultResourceChangedPartial(previousValue, value);
                }
            }
        }
        private Resource _defaultResource;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DefaultResource"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnDefaultResourceChangedPartial(Resource oldValue, Resource newValue);
    	public event EventHandler<PropertyChangedEventArgs<Resource>> DefaultResourceChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DefaultResource"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnDefaultResourceChanged(Resource oldValue, Resource newValue)
    	{
    		if (DefaultResourceChanged != null)
    			DefaultResourceChanged(this, new PropertyChangedEventArgs<Resource>(oldValue, newValue));
    	}
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Creator
        {
            get { return _creator; }
            set
            {
                if (!ReferenceEquals(_creator, value))
                {
                    var previousValue = _creator;
                    _creator = value;
                    FixupCreator(previousValue);
                    OnNavigationPropertyChanged("Creator");
                }
            }
        }
        private User _creator;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User LastModifier
        {
            get { return _lastModifier; }
            set
            {
                if (!ReferenceEquals(_lastModifier, value))
                {
                    var previousValue = _lastModifier;
                    _lastModifier = value;
                    FixupLastModifier(previousValue);
                    OnNavigationPropertyChanged("LastModifier");
                }
            }
        }
        private User _lastModifier;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Procedure Process
        {
            get { return _process; }
            set
            {
                if (!ReferenceEquals(_process, value))
                {
                    var previousValue = _process;
                    _process = value;
                    FixupProcess(previousValue);
                    OnNavigationPropertyChanged("Process");
                }
            }
        }
        private Procedure _process;
    				

        #endregion

        #region Propriétés de présentation
    
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est nouvelle.
        /// </summary>
        public bool IsMarkedAsAdded
        {
            get { return this.ChangeTracker.State == ObjectState.Added; }
        }
    
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est modifiée.
        /// </summary>
        public bool IsMarkedAsModified
        {
            get { return this.ChangeTracker.State == ObjectState.Modified; }
        }
    
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est inchangée.
        /// </summary>
        public bool IsMarkedAsUnchanged
        {
            get { return this.ChangeTracker.State == ObjectState.Unchanged; }
        }
    
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est supprimée.
        /// </summary>
        public bool IsMarkedAsDeleted
        {
            get { return this.ChangeTracker.State == ObjectState.Deleted; }
        }
    
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité n'est pas inchangée.
        /// </summary>
        public bool IsNotMarkedAsUnchanged
        {
            get { return this.ChangeTracker.State != ObjectState.Unchanged; }
        }

        #endregion

        #region Suivi des changements
    
        /// <summary>
        /// Appelé lorsqu'une propriété trackée de l'entité a changé.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété.</param>
        protected virtual void OnEntityPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
                NotifyMarkedAsPropertyChanged();
            }
    
            base.OnPropertyChanged(propertyName);
        }
        
        /// <summary>
        /// Appelé lorsqu'une propriété de navigation a changé.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété de navigation.</param>
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
                NotifyMarkedAsPropertyChanged();
            }
    
            base.OnPropertyChanged(propertyName);
        }
        
        /// <summary>
        /// Lève l'évènement PropertyChanged pour les champs de types "Marked as".
        /// </summary>
        public void NotifyMarkedAsPropertyChanged()
        {
            base.OnPropertyChanged("IsMarkedAsAdded");
            base.OnPropertyChanged("IsMarkedAsModified");
            base.OnPropertyChanged("IsMarkedAsUnchanged");
            base.OnPropertyChanged("IsMarkedAsDeleted");
            base.OnPropertyChanged("IsNotMarkedAsUnchanged");
        }
    
        private ObjectChangeTracker _changeTracker;
        
        /// <summary>
        /// Obtient ou définit le suivi des changements.
        /// </summary>
        [DataMember] //Désactivé pour export. Réactiver pour WCF
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        /// <summary>
        /// Gère le changement d'état de l'objet.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.Ksmed.Models.ObjectStateChangingEventArgs"/> contenant les données de l'évènement.</param>
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        /// <summary>
        /// Obtient une valeur indiquant si l'instance est en cours de désérialisation.
        /// </summary>
        protected bool IsDeserializing { get; private set; }
    
        /// <summary>
        /// Appelé lorsque l'instance est désérialisée.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        [OnDeserializing]
        private void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
        
        /// <summary>
        /// Appelé lorsque l'instance a été désérialisée.
        /// </summary>
        /// <param name="context">Le contexte.</param>
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        /// <summary>
        /// Vide les propriétés de navigation.
        /// </summary>
        protected virtual void ClearNavigationProperties()
        {
            Actions.Clear();
            DefaultResource = null;
            Creator = null;
            LastModifier = null;
            Process = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation DefaultResource.
        /// </summary>
        private void FixupDefaultResource(Resource previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Videos.Contains(this))
            {
                previousValue.Videos.Remove(this);
            }
    
            if (DefaultResource != null)
            {
                if (!DefaultResource.Videos.Contains(this))
                {
                    DefaultResource.Videos.Add(this);
                }
    
                DefaultResourceId = DefaultResource.ResourceId;
            }
            else if (!skipKeys)
            {
                DefaultResourceId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DefaultResource", previousValue, DefaultResource);
                if (DefaultResource != null && !DefaultResource.ChangeTracker.ChangeTrackingEnabled)
                {
                    DefaultResource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Creator.
        /// </summary>
        private void FixupCreator(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CreatedVideos.Contains(this))
            {
                previousValue.CreatedVideos.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedVideos.Contains(this))
                {
                    Creator.CreatedVideos.Add(this);
                }
    
                CreatedByUserId = Creator.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Creator", previousValue, Creator);
                if (Creator != null && !Creator.ChangeTracker.ChangeTrackingEnabled)
                {
                    Creator.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation LastModifier.
        /// </summary>
        private void FixupLastModifier(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LastModifiedVideos.Contains(this))
            {
                previousValue.LastModifiedVideos.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedVideos.Contains(this))
                {
                    LastModifier.LastModifiedVideos.Add(this);
                }
    
                ModifiedByUserId = LastModifier.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LastModifier", previousValue, LastModifier);
                if (LastModifier != null && !LastModifier.ChangeTracker.ChangeTrackingEnabled)
                {
                    LastModifier.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Process.
        /// </summary>
        private void FixupProcess(Procedure previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Videos.Contains(this))
            {
                previousValue.Videos.Remove(this);
            }
    
            if (Process != null)
            {
                if (!Process.Videos.Contains(this))
                {
                    Process.Videos.Add(this);
                }
    
                ProcessId = Process.ProcessId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Process", previousValue, Process);
                if (Process != null && !Process.ChangeTracker.ChangeTrackingEnabled)
                {
                    Process.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Actions.
        /// </summary>
        private void FixupActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KAction item in e.NewItems)
                {
                    item.Video = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Actions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Video, this))
                    {
                        item.Video = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Actions", item);
                    }
                }
            }
        }

        #endregion

        #region Assignation des valeurs pour les propriétés
    
    	/// <summary>
    	/// Assigne une valeur à la propriété spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	public virtual void SetPropertyValue(string propertyName, object value)
    	{
    		switch (propertyName)
    		{
    			case "VideoId":
    				this.VideoId = Convert.ToInt32(value);
    				break;
    			case "DefaultResourceId":
    				this.DefaultResourceId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Duration":
    				this.Duration = (double)value;
    				break;
    			case "Format":
    				this.Format = (string)value;
    				break;
    			case "FilePath":
    				this.FilePath = (string)value;
    				break;
    			case "ShootingDate":
    				this.ShootingDate = (System.DateTime)value;
    				break;
    			case "Thumbnail":
    				this.Thumbnail = (byte[])value;
    				break;
    			case "CreatedByUserId":
    				this.CreatedByUserId = Convert.ToInt32(value);
    				break;
    			case "CreationDate":
    				this.CreationDate = (System.DateTime)value;
    				break;
    			case "ModifiedByUserId":
    				this.ModifiedByUserId = Convert.ToInt32(value);
    				break;
    			case "LastModificationDate":
    				this.LastModificationDate = (System.DateTime)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "CameraName":
    				this.CameraName = (string)value;
    				break;
    			case "Hash":
    				this.Hash = (string)value;
    				break;
    			case "OnServer":
    				this.OnServer = (bool)value;
    				break;
    			case "Extension":
    				this.Extension = (string)value;
    				break;
    			case "Sync":
    				this.Sync = (bool)value;
    				break;
    			case "OriginalHash":
    				this.OriginalHash = (string)value;
    				break;
    			case "NumSeq":
    				this.NumSeq = Convert.ToInt32(value);
    				break;
    			case "ResourceView":
    				this.ResourceView = value == null ? (ResourceViewEnum?)null : (ResourceViewEnum)Convert.ToInt32(value);
    				break;
    			case "DefaultResource":
    				this.DefaultResource = (Resource)value;
    				break;
    			case "Creator":
    				this.Creator = (User)value;
    				break;
    			case "LastModifier":
    				this.LastModifier = (User)value;
    				break;
    			case "Process":
    				this.Process = (Procedure)value;
    				break;
    			default:
    				break;
    		}
    	}
    	
    	/// <summary>
    	/// Ajoute un élément à la collection spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	public virtual void AddItemToCollection(string propertyName, object value)
    	{
    		switch (propertyName)
    		{
    			case "Actions":
    				this.Actions.Add((KAction)value);
    				break;
    			default:
    				break;
    		}
    	}
    	
    	/// <summary>
    	/// Ajoute un élément à la collection spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	public virtual void RemoveItemFromCollection(string propertyName, object value)
    	{
    		switch (propertyName)
    		{
    			case "Actions":
    				this.Actions.Remove((KAction)value);
    				break;
    			default:
    				break;
    		}
    	}
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés.</returns>
    	public virtual IDictionary<string,object> GetCurrentValues()
    	{
    		var values = new Dictionary<string,object>();
    		values.Add("VideoId", this.VideoId);
    		values.Add("DefaultResourceId", this.DefaultResourceId);
    		values.Add("Duration", this.Duration);
    		values.Add("Format", this.Format);
    		values.Add("FilePath", this.FilePath);
    		values.Add("ShootingDate", this.ShootingDate);
    		values.Add("Thumbnail", this.Thumbnail);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("CameraName", this.CameraName);
    		values.Add("Hash", this.Hash);
    		values.Add("OnServer", this.OnServer);
    		values.Add("Extension", this.Extension);
    		values.Add("Sync", this.Sync);
    		values.Add("OriginalHash", this.OriginalHash);
    		values.Add("NumSeq", this.NumSeq);
    		values.Add("DefaultResource", this.DefaultResource);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    
    		values.Add("Actions", GetHashCodes(this.Actions));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("DefaultResource", this.DefaultResource);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    
    		values.Add("Actions", this.Actions);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Format.
        /// </summary>
    	public const int FormatMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ FilePath.
        /// </summary>
    	public const int FilePathMaxLength = 255;
    
        /// <summary>
        /// Taille maximum du champ CameraName.
        /// </summary>
    	public const int CameraNameMaxLength = 50;

        #endregion

    }
}
