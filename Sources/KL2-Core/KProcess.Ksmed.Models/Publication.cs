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
    [KnownType(typeof(Project))]
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(User))]
    [KnownType(typeof(PublishedAction))]
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(PublicationLocalization))]
    [KnownType(typeof(UserReadPublication))]
    [KnownType(typeof(Training))]
    [KnownType(typeof(Qualification))]
    [KnownType(typeof(Inspection))]
    [KnownType(typeof(PublicationHistory))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Publication : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Publication";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Publication"/>.
        /// </summary>
    	public Publication()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private System.Guid _publicationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.Guid PublicationId
        {
            get { return _publicationId; }
            set
            {
                if (_publicationId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublicationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _publicationId = value;
                    OnEntityPropertyChanged("PublicationId");
                }
            }
        }
        
        private int _projectId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (_projectId != value)
                {
                    ChangeTracker.RecordValue("ProjectId", _projectId, value);
                    if (!IsDeserializing)
                    {
                        if (Project != null && Project.ProjectId != value)
                        {
                            Project = null;
                        }
                    }
                    _projectId = value;
                    OnEntityPropertyChanged("ProjectId");
                }
            }
        }
        
        private int _scenarioId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ScenarioId
        {
            get { return _scenarioId; }
            set
            {
                if (_scenarioId != value)
                {
                    ChangeTracker.RecordValue("ScenarioId", _scenarioId, value);
                    if (!IsDeserializing)
                    {
                        if (Scenario != null && Scenario.ScenarioId != value)
                        {
                            Scenario = null;
                        }
                    }
                    _scenarioId = value;
                    OnEntityPropertyChanged("ScenarioId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    ChangeTracker.RecordValue("Label", _label, value);
                    _label = value;
                    OnEntityPropertyChanged("Label");
                }
            }
        }
        
        private string _description;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    ChangeTracker.RecordValue("Description", _description, value);
                    _description = value;
                    OnEntityPropertyChanged("Description");
                }
            }
        }
        
        private long _criticalPathIDuration;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long CriticalPathIDuration
        {
            get { return _criticalPathIDuration; }
            set
            {
                if (_criticalPathIDuration != value)
                {
                    ChangeTracker.RecordValue("CriticalPathIDuration", _criticalPathIDuration, value);
                    _criticalPathIDuration = value;
                    OnEntityPropertyChanged("CriticalPathIDuration");
                }
            }
        }
        
        private int _publishedByUserId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublishedByUserId
        {
            get { return _publishedByUserId; }
            set
            {
                if (_publishedByUserId != value)
                {
                    ChangeTracker.RecordValue("PublishedByUserId", _publishedByUserId, value);
                    if (!IsDeserializing)
                    {
                        if (Publisher != null && Publisher.UserId != value)
                        {
                            Publisher = null;
                        }
                    }
                    _publishedByUserId = value;
                    OnEntityPropertyChanged("PublishedByUserId");
                }
            }
        }
        
        private System.DateTime _publishedDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime PublishedDate
        {
            get { return _publishedDate; }
            set
            {
                if (_publishedDate != value)
                {
                    ChangeTracker.RecordValue("PublishedDate", _publishedDate, value);
                    _publishedDate = value;
                    OnEntityPropertyChanged("PublishedDate");
                }
            }
        }
        
        private string _watermark;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Watermark
        {
            get { return _watermark; }
            set
            {
                if (_watermark != value)
                {
                    ChangeTracker.RecordValue("Watermark", _watermark, value);
                    _watermark = value;
                    OnEntityPropertyChanged("Watermark");
                }
            }
        }
        
        private long _minDurationVideo;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long MinDurationVideo
        {
            get { return _minDurationVideo; }
            set
            {
                if (_minDurationVideo != value)
                {
                    ChangeTracker.RecordValue("MinDurationVideo", _minDurationVideo, value);
                    _minDurationVideo = value;
                    OnEntityPropertyChanged("MinDurationVideo");
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
        
        private long _timeScale;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long TimeScale
        {
            get { return _timeScale; }
            set
            {
                if (_timeScale != value)
                {
                    ChangeTracker.RecordValue("TimeScale", _timeScale, value);
                    _timeScale = value;
                    OnEntityPropertyChanged("TimeScale");
                }
            }
        }
        
        private bool _isSkill;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsSkill
        {
            get { return _isSkill; }
            set
            {
                if (_isSkill != value)
                {
                    ChangeTracker.RecordValue("IsSkill", _isSkill, value);
                    _isSkill = value;
                    OnEntityPropertyChanged("IsSkill");
                }
            }
        }
        
        private byte[] _formation_Disposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Formation_Disposition
        {
            get { return _formation_Disposition; }
            set
            {
                if (_formation_Disposition != value)
                {
                    ChangeTracker.RecordValue("Formation_Disposition", _formation_Disposition, value);
                    _formation_Disposition = value;
                    OnEntityPropertyChanged("Formation_Disposition");
                }
            }
        }
        
        private byte[] _inspection_Disposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Inspection_Disposition
        {
            get { return _inspection_Disposition; }
            set
            {
                if (_inspection_Disposition != value)
                {
                    ChangeTracker.RecordValue("Inspection_Disposition", _inspection_Disposition, value);
                    _inspection_Disposition = value;
                    OnEntityPropertyChanged("Inspection_Disposition");
                }
            }
        }
        
        private byte[] _audit_Disposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Audit_Disposition
        {
            get { return _audit_Disposition; }
            set
            {
                if (_audit_Disposition != value)
                {
                    ChangeTracker.RecordValue("Audit_Disposition", _audit_Disposition, value);
                    _audit_Disposition = value;
                    OnEntityPropertyChanged("Audit_Disposition");
                }
            }
        }
        
        private byte[] _evaluation_Disposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Evaluation_Disposition
        {
            get { return _evaluation_Disposition; }
            set
            {
                if (_evaluation_Disposition != value)
                {
                    ChangeTracker.RecordValue("Evaluation_Disposition", _evaluation_Disposition, value);
                    _evaluation_Disposition = value;
                    OnEntityPropertyChanged("Evaluation_Disposition");
                }
            }
        }
        
        private bool _isMajor;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsMajor
        {
            get { return _isMajor; }
            set
            {
                if (_isMajor != value)
                {
                    ChangeTracker.RecordValue("IsMajor", _isMajor, value);
                    _isMajor = value;
                    OnEntityPropertyChanged("IsMajor");
                }
            }
        }
        
        private string _releaseNote;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ReleaseNote
        {
            get { return _releaseNote; }
            set
            {
                if (_releaseNote != value)
                {
                    ChangeTracker.RecordValue("ReleaseNote", _releaseNote, value);
                    _releaseNote = value;
                    OnEntityPropertyChanged("ReleaseNote");
                }
            }
        }
        
        private string _version;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Version
        {
            get { return _version; }
            set
            {
                if (_version != value)
                {
                    ChangeTracker.RecordValue("Version", _version, value);
                    _version = value;
                    OnEntityPropertyChanged("Version");
                }
            }
        }
        
        private string _formation_ActionDisposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Formation_ActionDisposition
        {
            get { return _formation_ActionDisposition; }
            set
            {
                if (_formation_ActionDisposition != value)
                {
                    ChangeTracker.RecordValue("Formation_ActionDisposition", _formation_ActionDisposition, value);
                    _formation_ActionDisposition = value;
                    OnEntityPropertyChanged("Formation_ActionDisposition");
                }
            }
        }
        
        private string _inspection_ActionDisposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Inspection_ActionDisposition
        {
            get { return _inspection_ActionDisposition; }
            set
            {
                if (_inspection_ActionDisposition != value)
                {
                    ChangeTracker.RecordValue("Inspection_ActionDisposition", _inspection_ActionDisposition, value);
                    _inspection_ActionDisposition = value;
                    OnEntityPropertyChanged("Inspection_ActionDisposition");
                }
            }
        }
        
        private string _evaluation_ActionDisposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Evaluation_ActionDisposition
        {
            get { return _evaluation_ActionDisposition; }
            set
            {
                if (_evaluation_ActionDisposition != value)
                {
                    ChangeTracker.RecordValue("Evaluation_ActionDisposition", _evaluation_ActionDisposition, value);
                    _evaluation_ActionDisposition = value;
                    OnEntityPropertyChanged("Evaluation_ActionDisposition");
                }
            }
        }

        #endregion

        #region Propriétés Enum
        
        private PublishModeEnum _publishMode;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public PublishModeEnum PublishMode
        {
            get { return _publishMode; }
            set
            {
                if (_publishMode != value)
                {
                    ChangeTracker.RecordValue("PublishMode", _publishMode, value);
                    _publishMode = value;
                    OnEntityPropertyChanged("PublishMode");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Project Project
        {
            get { return _project; }
            set
            {
                if (!ReferenceEquals(_project, value))
                {
                    var previousValue = _project;
                    _project = value;
                    FixupProject(previousValue);
                    OnNavigationPropertyChanged("Project");
                }
            }
        }
        private Project _project;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Scenario Scenario
        {
            get { return _scenario; }
            set
            {
                if (!ReferenceEquals(_scenario, value))
                {
                    var previousValue = _scenario;
                    _scenario = value;
                    FixupScenario(previousValue);
                    OnNavigationPropertyChanged("Scenario");
                }
            }
        }
        private Scenario _scenario;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Publisher
        {
            get { return _publisher; }
            set
            {
                if (!ReferenceEquals(_publisher, value))
                {
                    var previousValue = _publisher;
                    _publisher = value;
                    FixupPublisher(previousValue);
                    OnNavigationPropertyChanged("Publisher");
                }
            }
        }
        private User _publisher;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedAction> PublishedActions
        {
            get
            {
                if (_publishedActions == null)
                {
                    _publishedActions = new TrackableCollection<PublishedAction>();
                    _publishedActions.CollectionChanged += FixupPublishedActions;
                }
                return _publishedActions;
            }
            set
            {
                if (!ReferenceEquals(_publishedActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedActions != null)
                    {
                        _publishedActions.CollectionChanged -= FixupPublishedActions;
                    }
                    _publishedActions = value;
                    if (_publishedActions != null)
                    {
                        _publishedActions.CollectionChanged += FixupPublishedActions;
                    }
                    OnNavigationPropertyChanged("PublishedActions");
                }
            }
        }
        private TrackableCollection<PublishedAction> _publishedActions;
    
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
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationLocalization> Localizations
        {
            get
            {
                if (_localizations == null)
                {
                    _localizations = new TrackableCollection<PublicationLocalization>();
                    _localizations.CollectionChanged += FixupLocalizations;
                }
                return _localizations;
            }
            set
            {
                if (!ReferenceEquals(_localizations, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_localizations != null)
                    {
                        _localizations.CollectionChanged -= FixupLocalizations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (PublicationLocalization item in _localizations)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _localizations = value;
                    if (_localizations != null)
                    {
                        _localizations.CollectionChanged += FixupLocalizations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (PublicationLocalization item in _localizations)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Localizations");
                }
            }
        }
        private TrackableCollection<PublicationLocalization> _localizations;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<UserReadPublication> Readers
        {
            get
            {
                if (_readers == null)
                {
                    _readers = new TrackableCollection<UserReadPublication>();
                    _readers.CollectionChanged += FixupReaders;
                }
                return _readers;
            }
            set
            {
                if (!ReferenceEquals(_readers, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_readers != null)
                    {
                        _readers.CollectionChanged -= FixupReaders;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (UserReadPublication item in _readers)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _readers = value;
                    if (_readers != null)
                    {
                        _readers.CollectionChanged += FixupReaders;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (UserReadPublication item in _readers)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Readers");
                }
            }
        }
        private TrackableCollection<UserReadPublication> _readers;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Training> Trainings
        {
            get
            {
                if (_trainings == null)
                {
                    _trainings = new TrackableCollection<Training>();
                    _trainings.CollectionChanged += FixupTrainings;
                }
                return _trainings;
            }
            set
            {
                if (!ReferenceEquals(_trainings, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_trainings != null)
                    {
                        _trainings.CollectionChanged -= FixupTrainings;
                    }
                    _trainings = value;
                    if (_trainings != null)
                    {
                        _trainings.CollectionChanged += FixupTrainings;
                    }
                    OnNavigationPropertyChanged("Trainings");
                }
            }
        }
        private TrackableCollection<Training> _trainings;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Qualification> Qualifications
        {
            get
            {
                if (_qualifications == null)
                {
                    _qualifications = new TrackableCollection<Qualification>();
                    _qualifications.CollectionChanged += FixupQualifications;
                }
                return _qualifications;
            }
            set
            {
                if (!ReferenceEquals(_qualifications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_qualifications != null)
                    {
                        _qualifications.CollectionChanged -= FixupQualifications;
                    }
                    _qualifications = value;
                    if (_qualifications != null)
                    {
                        _qualifications.CollectionChanged += FixupQualifications;
                    }
                    OnNavigationPropertyChanged("Qualifications");
                }
            }
        }
        private TrackableCollection<Qualification> _qualifications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedAction> LinkedPublishedActions
        {
            get
            {
                if (_linkedPublishedActions == null)
                {
                    _linkedPublishedActions = new TrackableCollection<PublishedAction>();
                    _linkedPublishedActions.CollectionChanged += FixupLinkedPublishedActions;
                }
                return _linkedPublishedActions;
            }
            set
            {
                if (!ReferenceEquals(_linkedPublishedActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_linkedPublishedActions != null)
                    {
                        _linkedPublishedActions.CollectionChanged -= FixupLinkedPublishedActions;
                    }
                    _linkedPublishedActions = value;
                    if (_linkedPublishedActions != null)
                    {
                        _linkedPublishedActions.CollectionChanged += FixupLinkedPublishedActions;
                    }
                    OnNavigationPropertyChanged("LinkedPublishedActions");
                }
            }
        }
        private TrackableCollection<PublishedAction> _linkedPublishedActions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Inspection> Inspections
        {
            get
            {
                if (_inspections == null)
                {
                    _inspections = new TrackableCollection<Inspection>();
                    _inspections.CollectionChanged += FixupInspections;
                }
                return _inspections;
            }
            set
            {
                if (!ReferenceEquals(_inspections, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inspections != null)
                    {
                        _inspections.CollectionChanged -= FixupInspections;
                    }
                    _inspections = value;
                    if (_inspections != null)
                    {
                        _inspections.CollectionChanged += FixupInspections;
                    }
                    OnNavigationPropertyChanged("Inspections");
                }
            }
        }
        private TrackableCollection<Inspection> _inspections;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<UserReadPublication> NextVersionUserReadPublications
        {
            get
            {
                if (_nextVersionUserReadPublications == null)
                {
                    _nextVersionUserReadPublications = new TrackableCollection<UserReadPublication>();
                    _nextVersionUserReadPublications.CollectionChanged += FixupNextVersionUserReadPublications;
                }
                return _nextVersionUserReadPublications;
            }
            set
            {
                if (!ReferenceEquals(_nextVersionUserReadPublications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nextVersionUserReadPublications != null)
                    {
                        _nextVersionUserReadPublications.CollectionChanged -= FixupNextVersionUserReadPublications;
                    }
                    _nextVersionUserReadPublications = value;
                    if (_nextVersionUserReadPublications != null)
                    {
                        _nextVersionUserReadPublications.CollectionChanged += FixupNextVersionUserReadPublications;
                    }
                    OnNavigationPropertyChanged("NextVersionUserReadPublications");
                }
            }
        }
        private TrackableCollection<UserReadPublication> _nextVersionUserReadPublications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationHistory> EvaluationPublicationHistories
        {
            get
            {
                if (_evaluationPublicationHistories == null)
                {
                    _evaluationPublicationHistories = new TrackableCollection<PublicationHistory>();
                    _evaluationPublicationHistories.CollectionChanged += FixupEvaluationPublicationHistories;
                }
                return _evaluationPublicationHistories;
            }
            set
            {
                if (!ReferenceEquals(_evaluationPublicationHistories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_evaluationPublicationHistories != null)
                    {
                        _evaluationPublicationHistories.CollectionChanged -= FixupEvaluationPublicationHistories;
                    }
                    _evaluationPublicationHistories = value;
                    if (_evaluationPublicationHistories != null)
                    {
                        _evaluationPublicationHistories.CollectionChanged += FixupEvaluationPublicationHistories;
                    }
                    OnNavigationPropertyChanged("EvaluationPublicationHistories");
                }
            }
        }
        private TrackableCollection<PublicationHistory> _evaluationPublicationHistories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationHistory> InspectionPublicationHistories
        {
            get
            {
                if (_inspectionPublicationHistories == null)
                {
                    _inspectionPublicationHistories = new TrackableCollection<PublicationHistory>();
                    _inspectionPublicationHistories.CollectionChanged += FixupInspectionPublicationHistories;
                }
                return _inspectionPublicationHistories;
            }
            set
            {
                if (!ReferenceEquals(_inspectionPublicationHistories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inspectionPublicationHistories != null)
                    {
                        _inspectionPublicationHistories.CollectionChanged -= FixupInspectionPublicationHistories;
                    }
                    _inspectionPublicationHistories = value;
                    if (_inspectionPublicationHistories != null)
                    {
                        _inspectionPublicationHistories.CollectionChanged += FixupInspectionPublicationHistories;
                    }
                    OnNavigationPropertyChanged("InspectionPublicationHistories");
                }
            }
        }
        private TrackableCollection<PublicationHistory> _inspectionPublicationHistories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationHistory> TrainingPublicationHistories
        {
            get
            {
                if (_trainingPublicationHistories == null)
                {
                    _trainingPublicationHistories = new TrackableCollection<PublicationHistory>();
                    _trainingPublicationHistories.CollectionChanged += FixupTrainingPublicationHistories;
                }
                return _trainingPublicationHistories;
            }
            set
            {
                if (!ReferenceEquals(_trainingPublicationHistories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_trainingPublicationHistories != null)
                    {
                        _trainingPublicationHistories.CollectionChanged -= FixupTrainingPublicationHistories;
                    }
                    _trainingPublicationHistories = value;
                    if (_trainingPublicationHistories != null)
                    {
                        _trainingPublicationHistories.CollectionChanged += FixupTrainingPublicationHistories;
                    }
                    OnNavigationPropertyChanged("TrainingPublicationHistories");
                }
            }
        }
        private TrackableCollection<PublicationHistory> _trainingPublicationHistories;

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
            Project = null;
            Scenario = null;
            Publisher = null;
            PublishedActions.Clear();
            Process = null;
            Localizations.Clear();
            Readers.Clear();
            Trainings.Clear();
            Qualifications.Clear();
            LinkedPublishedActions.Clear();
            Inspections.Clear();
            NextVersionUserReadPublications.Clear();
            EvaluationPublicationHistories.Clear();
            InspectionPublicationHistories.Clear();
            TrainingPublicationHistories.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Project.
        /// </summary>
        private void FixupProject(Project previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Publications.Contains(this))
            {
                previousValue.Publications.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.Publications.Contains(this))
                {
                    Project.Publications.Add(this);
                }
    
                ProjectId = Project.ProjectId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Project", previousValue, Project);
                if (Project != null && !Project.ChangeTracker.ChangeTrackingEnabled)
                {
                    Project.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Scenario.
        /// </summary>
        private void FixupScenario(Scenario previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Publications.Contains(this))
            {
                previousValue.Publications.Remove(this);
            }
    
            if (Scenario != null)
            {
                if (!Scenario.Publications.Contains(this))
                {
                    Scenario.Publications.Add(this);
                }
    
                ScenarioId = Scenario.ScenarioId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Scenario", previousValue, Scenario);
                if (Scenario != null && !Scenario.ChangeTracker.ChangeTrackingEnabled)
                {
                    Scenario.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Publisher.
        /// </summary>
        private void FixupPublisher(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Publication.Contains(this))
            {
                previousValue.Publication.Remove(this);
            }
    
            if (Publisher != null)
            {
                if (!Publisher.Publication.Contains(this))
                {
                    Publisher.Publication.Add(this);
                }
    
                PublishedByUserId = Publisher.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Publisher", previousValue, Publisher);
                if (Publisher != null && !Publisher.ChangeTracker.ChangeTrackingEnabled)
                {
                    Publisher.StartTracking();
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
    
            if (previousValue != null && previousValue.Publications.Contains(this))
            {
                previousValue.Publications.Remove(this);
            }
    
            if (Process != null)
            {
                if (!Process.Publications.Contains(this))
                {
                    Process.Publications.Add(this);
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
        /// Corrige l'état de la propriété PublishedActions.
        /// </summary>
        private void FixupPublishedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedAction item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedActions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Localizations.
        /// </summary>
        private void FixupLocalizations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationLocalization item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Localizations", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationLocalization item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Localizations", item);
                        // Delete the dependent end of this identifying association. If the current state is Added,
                        // allow the relationship to be changed without causing the dependent to be deleted.
                        if (item.ChangeTracker.State != ObjectState.Added)
                        {
                            item.MarkAsDeleted();
                        }
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Readers.
        /// </summary>
        private void FixupReaders(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (UserReadPublication item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Readers", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserReadPublication item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Readers", item);
                        // Delete the dependent end of this identifying association. If the current state is Added,
                        // allow the relationship to be changed without causing the dependent to be deleted.
                        if (item.ChangeTracker.State != ObjectState.Added)
                        {
                            item.MarkAsDeleted();
                        }
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Trainings.
        /// </summary>
        private void FixupTrainings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Training item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Trainings", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Training item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Trainings", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Qualifications.
        /// </summary>
        private void FixupQualifications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Qualification item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Qualifications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Qualification item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Qualifications", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LinkedPublishedActions.
        /// </summary>
        private void FixupLinkedPublishedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedAction item in e.NewItems)
                {
                    item.LinkedPublication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LinkedPublishedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.LinkedPublication, this))
                    {
                        item.LinkedPublication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LinkedPublishedActions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Inspections.
        /// </summary>
        private void FixupInspections(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Inspection item in e.NewItems)
                {
                    item.Publication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Inspections", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Inspection item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publication, this))
                    {
                        item.Publication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Inspections", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété NextVersionUserReadPublications.
        /// </summary>
        private void FixupNextVersionUserReadPublications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (UserReadPublication item in e.NewItems)
                {
                    item.PreviousVersionPublication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NextVersionUserReadPublications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserReadPublication item in e.OldItems)
                {
                    if (ReferenceEquals(item.PreviousVersionPublication, this))
                    {
                        item.PreviousVersionPublication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NextVersionUserReadPublications", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété EvaluationPublicationHistories.
        /// </summary>
        private void FixupEvaluationPublicationHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationHistory item in e.NewItems)
                {
                    item.EvaluationPublication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("EvaluationPublicationHistories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.EvaluationPublication, this))
                    {
                        item.EvaluationPublication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("EvaluationPublicationHistories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété InspectionPublicationHistories.
        /// </summary>
        private void FixupInspectionPublicationHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationHistory item in e.NewItems)
                {
                    item.InspectionPublication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("InspectionPublicationHistories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.InspectionPublication, this))
                    {
                        item.InspectionPublication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionPublicationHistories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété TrainingPublicationHistories.
        /// </summary>
        private void FixupTrainingPublicationHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationHistory item in e.NewItems)
                {
                    item.TrainingPublication = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("TrainingPublicationHistories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.TrainingPublication, this))
                    {
                        item.TrainingPublication = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("TrainingPublicationHistories", item);
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
    			case "PublicationId":
    				this.PublicationId = (System.Guid)value;
    				break;
    			case "ProjectId":
    				this.ProjectId = Convert.ToInt32(value);
    				break;
    			case "ScenarioId":
    				this.ScenarioId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "CriticalPathIDuration":
    				this.CriticalPathIDuration = (long)value;
    				break;
    			case "PublishedByUserId":
    				this.PublishedByUserId = Convert.ToInt32(value);
    				break;
    			case "PublishedDate":
    				this.PublishedDate = (System.DateTime)value;
    				break;
    			case "Watermark":
    				this.Watermark = (string)value;
    				break;
    			case "MinDurationVideo":
    				this.MinDurationVideo = (long)value;
    				break;
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "TimeScale":
    				this.TimeScale = (long)value;
    				break;
    			case "IsSkill":
    				this.IsSkill = (bool)value;
    				break;
    			case "Formation_Disposition":
    				this.Formation_Disposition = (byte[])value;
    				break;
    			case "Inspection_Disposition":
    				this.Inspection_Disposition = (byte[])value;
    				break;
    			case "Audit_Disposition":
    				this.Audit_Disposition = (byte[])value;
    				break;
    			case "Evaluation_Disposition":
    				this.Evaluation_Disposition = (byte[])value;
    				break;
    			case "IsMajor":
    				this.IsMajor = (bool)value;
    				break;
    			case "ReleaseNote":
    				this.ReleaseNote = (string)value;
    				break;
    			case "Version":
    				this.Version = (string)value;
    				break;
    			case "Formation_ActionDisposition":
    				this.Formation_ActionDisposition = (string)value;
    				break;
    			case "Inspection_ActionDisposition":
    				this.Inspection_ActionDisposition = (string)value;
    				break;
    			case "Evaluation_ActionDisposition":
    				this.Evaluation_ActionDisposition = (string)value;
    				break;
    			case "PublishMode":
    				this.PublishMode = (PublishModeEnum)Convert.ToInt32(value);
    				break;
    			case "Project":
    				this.Project = (Project)value;
    				break;
    			case "Scenario":
    				this.Scenario = (Scenario)value;
    				break;
    			case "Publisher":
    				this.Publisher = (User)value;
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
    			case "PublishedActions":
    				this.PublishedActions.Add((PublishedAction)value);
    				break;
    			case "Localizations":
    				this.Localizations.Add((PublicationLocalization)value);
    				break;
    			case "Readers":
    				this.Readers.Add((UserReadPublication)value);
    				break;
    			case "Trainings":
    				this.Trainings.Add((Training)value);
    				break;
    			case "Qualifications":
    				this.Qualifications.Add((Qualification)value);
    				break;
    			case "LinkedPublishedActions":
    				this.LinkedPublishedActions.Add((PublishedAction)value);
    				break;
    			case "Inspections":
    				this.Inspections.Add((Inspection)value);
    				break;
    			case "NextVersionUserReadPublications":
    				this.NextVersionUserReadPublications.Add((UserReadPublication)value);
    				break;
    			case "EvaluationPublicationHistories":
    				this.EvaluationPublicationHistories.Add((PublicationHistory)value);
    				break;
    			case "InspectionPublicationHistories":
    				this.InspectionPublicationHistories.Add((PublicationHistory)value);
    				break;
    			case "TrainingPublicationHistories":
    				this.TrainingPublicationHistories.Add((PublicationHistory)value);
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
    			case "PublishedActions":
    				this.PublishedActions.Remove((PublishedAction)value);
    				break;
    			case "Localizations":
    				this.Localizations.Remove((PublicationLocalization)value);
    				break;
    			case "Readers":
    				this.Readers.Remove((UserReadPublication)value);
    				break;
    			case "Trainings":
    				this.Trainings.Remove((Training)value);
    				break;
    			case "Qualifications":
    				this.Qualifications.Remove((Qualification)value);
    				break;
    			case "LinkedPublishedActions":
    				this.LinkedPublishedActions.Remove((PublishedAction)value);
    				break;
    			case "Inspections":
    				this.Inspections.Remove((Inspection)value);
    				break;
    			case "NextVersionUserReadPublications":
    				this.NextVersionUserReadPublications.Remove((UserReadPublication)value);
    				break;
    			case "EvaluationPublicationHistories":
    				this.EvaluationPublicationHistories.Remove((PublicationHistory)value);
    				break;
    			case "InspectionPublicationHistories":
    				this.InspectionPublicationHistories.Remove((PublicationHistory)value);
    				break;
    			case "TrainingPublicationHistories":
    				this.TrainingPublicationHistories.Remove((PublicationHistory)value);
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
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("ProjectId", this.ProjectId);
    		values.Add("ScenarioId", this.ScenarioId);
    		values.Add("Label", this.Label);
    		values.Add("Description", this.Description);
    		values.Add("CriticalPathIDuration", this.CriticalPathIDuration);
    		values.Add("PublishedByUserId", this.PublishedByUserId);
    		values.Add("PublishedDate", this.PublishedDate);
    		values.Add("Watermark", this.Watermark);
    		values.Add("MinDurationVideo", this.MinDurationVideo);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("TimeScale", this.TimeScale);
    		values.Add("IsSkill", this.IsSkill);
    		values.Add("Formation_Disposition", this.Formation_Disposition);
    		values.Add("Inspection_Disposition", this.Inspection_Disposition);
    		values.Add("Audit_Disposition", this.Audit_Disposition);
    		values.Add("Evaluation_Disposition", this.Evaluation_Disposition);
    		values.Add("IsMajor", this.IsMajor);
    		values.Add("ReleaseNote", this.ReleaseNote);
    		values.Add("Version", this.Version);
    		values.Add("Formation_ActionDisposition", this.Formation_ActionDisposition);
    		values.Add("Inspection_ActionDisposition", this.Inspection_ActionDisposition);
    		values.Add("Evaluation_ActionDisposition", this.Evaluation_ActionDisposition);
    		values.Add("Project", this.Project);
    		values.Add("Scenario", this.Scenario);
    		values.Add("Publisher", this.Publisher);
    		values.Add("Process", this.Process);
    
    		values.Add("PublishedActions", GetHashCodes(this.PublishedActions));
    		values.Add("Localizations", GetHashCodes(this.Localizations));
    		values.Add("Readers", GetHashCodes(this.Readers));
    		values.Add("Trainings", GetHashCodes(this.Trainings));
    		values.Add("Qualifications", GetHashCodes(this.Qualifications));
    		values.Add("LinkedPublishedActions", GetHashCodes(this.LinkedPublishedActions));
    		values.Add("Inspections", GetHashCodes(this.Inspections));
    		values.Add("NextVersionUserReadPublications", GetHashCodes(this.NextVersionUserReadPublications));
    		values.Add("EvaluationPublicationHistories", GetHashCodes(this.EvaluationPublicationHistories));
    		values.Add("InspectionPublicationHistories", GetHashCodes(this.InspectionPublicationHistories));
    		values.Add("TrainingPublicationHistories", GetHashCodes(this.TrainingPublicationHistories));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Project", this.Project);
    		values.Add("Scenario", this.Scenario);
    		values.Add("Publisher", this.Publisher);
    		values.Add("Process", this.Process);
    
    		values.Add("PublishedActions", this.PublishedActions);
    		values.Add("Localizations", this.Localizations);
    		values.Add("Readers", this.Readers);
    		values.Add("Trainings", this.Trainings);
    		values.Add("Qualifications", this.Qualifications);
    		values.Add("LinkedPublishedActions", this.LinkedPublishedActions);
    		values.Add("Inspections", this.Inspections);
    		values.Add("NextVersionUserReadPublications", this.NextVersionUserReadPublications);
    		values.Add("EvaluationPublicationHistories", this.EvaluationPublicationHistories);
    		values.Add("InspectionPublicationHistories", this.InspectionPublicationHistories);
    		values.Add("TrainingPublicationHistories", this.TrainingPublicationHistories);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
