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
    [KnownType(typeof(ProjectDir))]
    [KnownType(typeof(Ref1))]
    [KnownType(typeof(Ref2))]
    [KnownType(typeof(Ref3))]
    [KnownType(typeof(Ref4))]
    [KnownType(typeof(Ref5))]
    [KnownType(typeof(Ref6))]
    [KnownType(typeof(Ref7))]
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(Equipment))]
    [KnownType(typeof(Operator))]
    [KnownType(typeof(Publication))]
    [KnownType(typeof(KAction))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(VideoSync))]
    [KnownType(typeof(InspectionSchedule))]
    [KnownType(typeof(User))]
    [KnownType(typeof(UserRoleProcess))]
    [KnownType(typeof(PublicationHistory))]
    [KnownType(typeof(DocumentationReferential))]
    [KnownType(typeof(DocumentationDraftLocalization))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Procedure : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Procedure";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Procedure"/>.
        /// </summary>
    	public Procedure()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _processId;
        /// <summary>
        /// Obtient ou définit l'identifiant d'un process.
        /// </summary>
        [DataMember]
        public int ProcessId
        {
            get { return _processId; }
            set
            {
                if (_processId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ProcessId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _processId = value;
                    OnEntityPropertyChanged("ProcessId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé du process.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Process_Label_Required")]
    	[LocalizableStringLength(LabelMaxLength, ErrorMessageResourceName = "Validation_Procedure_Label_StringLength")]
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
        
        private Nullable<int> _projectDirId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ProjectDirId
        {
            get { return _projectDirId; }
            set
            {
                if (_projectDirId != value)
                {
                    ChangeTracker.RecordValue("ProjectDirId", _projectDirId, value);
                    if (!IsDeserializing)
                    {
                        if (ProjectDir != null && ProjectDir.Id != value)
                        {
                            ProjectDir = null;
                        }
                    }
                    _projectDirId = value;
                    OnEntityPropertyChanged("ProjectDirId");
                }
            }
        }
        
        private string _description;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(DescriptionMaxLength, ErrorMessageResourceName = "Validation_Procedure_Description_StringLength")]
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
        
        private bool _isDeleted;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (_isDeleted != value)
                {
                    ChangeTracker.RecordValue("IsDeleted", _isDeleted, value);
                    _isDeleted = value;
                    OnEntityPropertyChanged("IsDeleted");
                }
            }
        }
        
        private int _ownerId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int OwnerId
        {
            get { return _ownerId; }
            set
            {
                if (_ownerId != value)
                {
                    ChangeTracker.RecordValue("OwnerId", _ownerId, value);
                    if (!IsDeserializing)
                    {
                        if (Owner != null && Owner.UserId != value)
                        {
                            Owner = null;
                        }
                    }
                    _ownerId = value;
                    OnEntityPropertyChanged("OwnerId");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit les projets associés.
        /// </summary>
        [DataMember]
        public TrackableCollection<Project> Projects
        {
            get
            {
                if (_projects == null)
                {
                    _projects = new TrackableCollection<Project>();
                    _projects.CollectionChanged += FixupProjects;
                }
                return _projects;
            }
            set
            {
                if (!ReferenceEquals(_projects, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_projects != null)
                    {
                        _projects.CollectionChanged -= FixupProjects;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Project item in _projects)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _projects = value;
                    if (_projects != null)
                    {
                        _projects.CollectionChanged += FixupProjects;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Project item in _projects)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Projects");
                }
            }
        }
        private TrackableCollection<Project> _projects;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ProjectDir ProjectDir
        {
            get { return _projectDir; }
            set
            {
                if (!ReferenceEquals(_projectDir, value))
                {
                    var previousValue = _projectDir;
                    _projectDir = value;
                    FixupProjectDir(previousValue);
                    OnNavigationPropertyChanged("ProjectDir");
                }
            }
        }
        private ProjectDir _projectDir;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref1> Refs1
        {
            get
            {
                if (_refs1 == null)
                {
                    _refs1 = new TrackableCollection<Ref1>();
                    _refs1.CollectionChanged += FixupRefs1;
                }
                return _refs1;
            }
            set
            {
                if (!ReferenceEquals(_refs1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs1 != null)
                    {
                        _refs1.CollectionChanged -= FixupRefs1;
                    }
                    _refs1 = value;
                    if (_refs1 != null)
                    {
                        _refs1.CollectionChanged += FixupRefs1;
                    }
                    OnNavigationPropertyChanged("Refs1");
                }
            }
        }
        private TrackableCollection<Ref1> _refs1;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref2> Refs2
        {
            get
            {
                if (_refs2 == null)
                {
                    _refs2 = new TrackableCollection<Ref2>();
                    _refs2.CollectionChanged += FixupRefs2;
                }
                return _refs2;
            }
            set
            {
                if (!ReferenceEquals(_refs2, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs2 != null)
                    {
                        _refs2.CollectionChanged -= FixupRefs2;
                    }
                    _refs2 = value;
                    if (_refs2 != null)
                    {
                        _refs2.CollectionChanged += FixupRefs2;
                    }
                    OnNavigationPropertyChanged("Refs2");
                }
            }
        }
        private TrackableCollection<Ref2> _refs2;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref3> Refs3
        {
            get
            {
                if (_refs3 == null)
                {
                    _refs3 = new TrackableCollection<Ref3>();
                    _refs3.CollectionChanged += FixupRefs3;
                }
                return _refs3;
            }
            set
            {
                if (!ReferenceEquals(_refs3, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs3 != null)
                    {
                        _refs3.CollectionChanged -= FixupRefs3;
                    }
                    _refs3 = value;
                    if (_refs3 != null)
                    {
                        _refs3.CollectionChanged += FixupRefs3;
                    }
                    OnNavigationPropertyChanged("Refs3");
                }
            }
        }
        private TrackableCollection<Ref3> _refs3;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref4> Refs4
        {
            get
            {
                if (_refs4 == null)
                {
                    _refs4 = new TrackableCollection<Ref4>();
                    _refs4.CollectionChanged += FixupRefs4;
                }
                return _refs4;
            }
            set
            {
                if (!ReferenceEquals(_refs4, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs4 != null)
                    {
                        _refs4.CollectionChanged -= FixupRefs4;
                    }
                    _refs4 = value;
                    if (_refs4 != null)
                    {
                        _refs4.CollectionChanged += FixupRefs4;
                    }
                    OnNavigationPropertyChanged("Refs4");
                }
            }
        }
        private TrackableCollection<Ref4> _refs4;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref5> Refs5
        {
            get
            {
                if (_refs5 == null)
                {
                    _refs5 = new TrackableCollection<Ref5>();
                    _refs5.CollectionChanged += FixupRefs5;
                }
                return _refs5;
            }
            set
            {
                if (!ReferenceEquals(_refs5, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs5 != null)
                    {
                        _refs5.CollectionChanged -= FixupRefs5;
                    }
                    _refs5 = value;
                    if (_refs5 != null)
                    {
                        _refs5.CollectionChanged += FixupRefs5;
                    }
                    OnNavigationPropertyChanged("Refs5");
                }
            }
        }
        private TrackableCollection<Ref5> _refs5;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref6> Refs6
        {
            get
            {
                if (_refs6 == null)
                {
                    _refs6 = new TrackableCollection<Ref6>();
                    _refs6.CollectionChanged += FixupRefs6;
                }
                return _refs6;
            }
            set
            {
                if (!ReferenceEquals(_refs6, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs6 != null)
                    {
                        _refs6.CollectionChanged -= FixupRefs6;
                    }
                    _refs6 = value;
                    if (_refs6 != null)
                    {
                        _refs6.CollectionChanged += FixupRefs6;
                    }
                    OnNavigationPropertyChanged("Refs6");
                }
            }
        }
        private TrackableCollection<Ref6> _refs6;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref7> Refs7
        {
            get
            {
                if (_refs7 == null)
                {
                    _refs7 = new TrackableCollection<Ref7>();
                    _refs7.CollectionChanged += FixupRefs7;
                }
                return _refs7;
            }
            set
            {
                if (!ReferenceEquals(_refs7, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refs7 != null)
                    {
                        _refs7.CollectionChanged -= FixupRefs7;
                    }
                    _refs7 = value;
                    if (_refs7 != null)
                    {
                        _refs7.CollectionChanged += FixupRefs7;
                    }
                    OnNavigationPropertyChanged("Refs7");
                }
            }
        }
        private TrackableCollection<Ref7> _refs7;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionCategory> ActionCategories
        {
            get
            {
                if (_actionCategories == null)
                {
                    _actionCategories = new TrackableCollection<ActionCategory>();
                    _actionCategories.CollectionChanged += FixupActionCategories;
                }
                return _actionCategories;
            }
            set
            {
                if (!ReferenceEquals(_actionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionCategories != null)
                    {
                        _actionCategories.CollectionChanged -= FixupActionCategories;
                    }
                    _actionCategories = value;
                    if (_actionCategories != null)
                    {
                        _actionCategories.CollectionChanged += FixupActionCategories;
                    }
                    OnNavigationPropertyChanged("ActionCategories");
                }
            }
        }
        private TrackableCollection<ActionCategory> _actionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Equipment> Equipments
        {
            get
            {
                if (_equipments == null)
                {
                    _equipments = new TrackableCollection<Equipment>();
                    _equipments.CollectionChanged += FixupEquipments;
                }
                return _equipments;
            }
            set
            {
                if (!ReferenceEquals(_equipments, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_equipments != null)
                    {
                        _equipments.CollectionChanged -= FixupEquipments;
                    }
                    _equipments = value;
                    if (_equipments != null)
                    {
                        _equipments.CollectionChanged += FixupEquipments;
                    }
                    OnNavigationPropertyChanged("Equipments");
                }
            }
        }
        private TrackableCollection<Equipment> _equipments;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Operator> Operators
        {
            get
            {
                if (_operators == null)
                {
                    _operators = new TrackableCollection<Operator>();
                    _operators.CollectionChanged += FixupOperators;
                }
                return _operators;
            }
            set
            {
                if (!ReferenceEquals(_operators, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_operators != null)
                    {
                        _operators.CollectionChanged -= FixupOperators;
                    }
                    _operators = value;
                    if (_operators != null)
                    {
                        _operators.CollectionChanged += FixupOperators;
                    }
                    OnNavigationPropertyChanged("Operators");
                }
            }
        }
        private TrackableCollection<Operator> _operators;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Publication> Publications
        {
            get
            {
                if (_publications == null)
                {
                    _publications = new TrackableCollection<Publication>();
                    _publications.CollectionChanged += FixupPublications;
                }
                return _publications;
            }
            set
            {
                if (!ReferenceEquals(_publications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publications != null)
                    {
                        _publications.CollectionChanged -= FixupPublications;
                    }
                    _publications = value;
                    if (_publications != null)
                    {
                        _publications.CollectionChanged += FixupPublications;
                    }
                    OnNavigationPropertyChanged("Publications");
                }
            }
        }
        private TrackableCollection<Publication> _publications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> LinkedActions
        {
            get
            {
                if (_linkedActions == null)
                {
                    _linkedActions = new TrackableCollection<KAction>();
                    _linkedActions.CollectionChanged += FixupLinkedActions;
                }
                return _linkedActions;
            }
            set
            {
                if (!ReferenceEquals(_linkedActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_linkedActions != null)
                    {
                        _linkedActions.CollectionChanged -= FixupLinkedActions;
                    }
                    _linkedActions = value;
                    if (_linkedActions != null)
                    {
                        _linkedActions.CollectionChanged += FixupLinkedActions;
                    }
                    OnNavigationPropertyChanged("LinkedActions");
                }
            }
        }
        private TrackableCollection<KAction> _linkedActions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Video> Videos
        {
            get
            {
                if (_videos == null)
                {
                    _videos = new TrackableCollection<Video>();
                    _videos.CollectionChanged += FixupVideos;
                }
                return _videos;
            }
            set
            {
                if (!ReferenceEquals(_videos, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_videos != null)
                    {
                        _videos.CollectionChanged -= FixupVideos;
                    }
                    _videos = value;
                    if (_videos != null)
                    {
                        _videos.CollectionChanged += FixupVideos;
                    }
                    OnNavigationPropertyChanged("Videos");
                }
            }
        }
        private TrackableCollection<Video> _videos;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<VideoSync> VideoSyncs
        {
            get
            {
                if (_videoSyncs == null)
                {
                    _videoSyncs = new TrackableCollection<VideoSync>();
                    _videoSyncs.CollectionChanged += FixupVideoSyncs;
                }
                return _videoSyncs;
            }
            set
            {
                if (!ReferenceEquals(_videoSyncs, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_videoSyncs != null)
                    {
                        _videoSyncs.CollectionChanged -= FixupVideoSyncs;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (VideoSync item in _videoSyncs)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _videoSyncs = value;
                    if (_videoSyncs != null)
                    {
                        _videoSyncs.CollectionChanged += FixupVideoSyncs;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (VideoSync item in _videoSyncs)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("VideoSyncs");
                }
            }
        }
        private TrackableCollection<VideoSync> _videoSyncs;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<InspectionSchedule> InspectionSchedules
        {
            get
            {
                if (_inspectionSchedules == null)
                {
                    _inspectionSchedules = new TrackableCollection<InspectionSchedule>();
                    _inspectionSchedules.CollectionChanged += FixupInspectionSchedules;
                }
                return _inspectionSchedules;
            }
            set
            {
                if (!ReferenceEquals(_inspectionSchedules, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inspectionSchedules != null)
                    {
                        _inspectionSchedules.CollectionChanged -= FixupInspectionSchedules;
                    }
                    _inspectionSchedules = value;
                    if (_inspectionSchedules != null)
                    {
                        _inspectionSchedules.CollectionChanged += FixupInspectionSchedules;
                    }
                    OnNavigationPropertyChanged("InspectionSchedules");
                }
            }
        }
        private TrackableCollection<InspectionSchedule> _inspectionSchedules;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Owner
        {
            get { return _owner; }
            set
            {
                if (!ReferenceEquals(_owner, value))
                {
                    var previousValue = _owner;
                    _owner = value;
                    FixupOwner(previousValue);
                    OnNavigationPropertyChanged("Owner");
                }
            }
        }
        private User _owner;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<UserRoleProcess> UserRoleProcesses
        {
            get
            {
                if (_userRoleProcesses == null)
                {
                    _userRoleProcesses = new TrackableCollection<UserRoleProcess>();
                    _userRoleProcesses.CollectionChanged += FixupUserRoleProcesses;
                }
                return _userRoleProcesses;
            }
            set
            {
                if (!ReferenceEquals(_userRoleProcesses, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_userRoleProcesses != null)
                    {
                        _userRoleProcesses.CollectionChanged -= FixupUserRoleProcesses;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (UserRoleProcess item in _userRoleProcesses)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _userRoleProcesses = value;
                    if (_userRoleProcesses != null)
                    {
                        _userRoleProcesses.CollectionChanged += FixupUserRoleProcesses;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (UserRoleProcess item in _userRoleProcesses)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("UserRoleProcesses");
                }
            }
        }
        private TrackableCollection<UserRoleProcess> _userRoleProcesses;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationHistory> PublicationHistories
        {
            get
            {
                if (_publicationHistories == null)
                {
                    _publicationHistories = new TrackableCollection<PublicationHistory>();
                    _publicationHistories.CollectionChanged += FixupPublicationHistories;
                }
                return _publicationHistories;
            }
            set
            {
                if (!ReferenceEquals(_publicationHistories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publicationHistories != null)
                    {
                        _publicationHistories.CollectionChanged -= FixupPublicationHistories;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (PublicationHistory item in _publicationHistories)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _publicationHistories = value;
                    if (_publicationHistories != null)
                    {
                        _publicationHistories.CollectionChanged += FixupPublicationHistories;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (PublicationHistory item in _publicationHistories)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("PublicationHistories");
                }
            }
        }
        private TrackableCollection<PublicationHistory> _publicationHistories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<DocumentationReferential> DocumentationReferentials
        {
            get
            {
                if (_documentationReferentials == null)
                {
                    _documentationReferentials = new TrackableCollection<DocumentationReferential>();
                    _documentationReferentials.CollectionChanged += FixupDocumentationReferentials;
                }
                return _documentationReferentials;
            }
            set
            {
                if (!ReferenceEquals(_documentationReferentials, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationReferentials != null)
                    {
                        _documentationReferentials.CollectionChanged -= FixupDocumentationReferentials;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (DocumentationReferential item in _documentationReferentials)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _documentationReferentials = value;
                    if (_documentationReferentials != null)
                    {
                        _documentationReferentials.CollectionChanged += FixupDocumentationReferentials;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (DocumentationReferential item in _documentationReferentials)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("DocumentationReferentials");
                }
            }
        }
        private TrackableCollection<DocumentationReferential> _documentationReferentials;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<DocumentationDraftLocalization> DocumentationDraftLocalizations
        {
            get
            {
                if (_documentationDraftLocalizations == null)
                {
                    _documentationDraftLocalizations = new TrackableCollection<DocumentationDraftLocalization>();
                    _documentationDraftLocalizations.CollectionChanged += FixupDocumentationDraftLocalizations;
                }
                return _documentationDraftLocalizations;
            }
            set
            {
                if (!ReferenceEquals(_documentationDraftLocalizations, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationDraftLocalizations != null)
                    {
                        _documentationDraftLocalizations.CollectionChanged -= FixupDocumentationDraftLocalizations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (DocumentationDraftLocalization item in _documentationDraftLocalizations)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _documentationDraftLocalizations = value;
                    if (_documentationDraftLocalizations != null)
                    {
                        _documentationDraftLocalizations.CollectionChanged += FixupDocumentationDraftLocalizations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (DocumentationDraftLocalization item in _documentationDraftLocalizations)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("DocumentationDraftLocalizations");
                }
            }
        }
        private TrackableCollection<DocumentationDraftLocalization> _documentationDraftLocalizations;

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
            Projects.Clear();
            ProjectDir = null;
            Refs1.Clear();
            Refs2.Clear();
            Refs3.Clear();
            Refs4.Clear();
            Refs5.Clear();
            Refs6.Clear();
            Refs7.Clear();
            ActionCategories.Clear();
            Equipments.Clear();
            Operators.Clear();
            Publications.Clear();
            LinkedActions.Clear();
            Videos.Clear();
            VideoSyncs.Clear();
            InspectionSchedules.Clear();
            Owner = null;
            UserRoleProcesses.Clear();
            PublicationHistories.Clear();
            DocumentationReferentials.Clear();
            DocumentationDraftLocalizations.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ProjectDir.
        /// </summary>
        private void FixupProjectDir(ProjectDir previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Processes.Contains(this))
            {
                previousValue.Processes.Remove(this);
            }
    
            if (ProjectDir != null)
            {
                if (!ProjectDir.Processes.Contains(this))
                {
                    ProjectDir.Processes.Add(this);
                }
    
                ProjectDirId = ProjectDir.Id;
            }
            else if (!skipKeys)
            {
                ProjectDirId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("ProjectDir", previousValue, ProjectDir);
                if (ProjectDir != null && !ProjectDir.ChangeTracker.ChangeTrackingEnabled)
                {
                    ProjectDir.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Owner.
        /// </summary>
        private void FixupOwner(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Procedures.Contains(this))
            {
                previousValue.Procedures.Remove(this);
            }
    
            if (Owner != null)
            {
                if (!Owner.Procedures.Contains(this))
                {
                    Owner.Procedures.Add(this);
                }
    
                OwnerId = Owner.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Owner", previousValue, Owner);
                if (Owner != null && !Owner.ChangeTracker.ChangeTrackingEnabled)
                {
                    Owner.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Projects.
        /// </summary>
        private void FixupProjects(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Project item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Projects", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Project item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Projects", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs1.
        /// </summary>
        private void FixupRefs1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref1 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref1 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs1", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs2.
        /// </summary>
        private void FixupRefs2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref2 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs2", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref2 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs2", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs3.
        /// </summary>
        private void FixupRefs3(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref3 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs3", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref3 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs3", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs4.
        /// </summary>
        private void FixupRefs4(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref4 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs4", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref4 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs4", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs5.
        /// </summary>
        private void FixupRefs5(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref5 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs5", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref5 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs5", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs6.
        /// </summary>
        private void FixupRefs6(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref6 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs6", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref6 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs6", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Refs7.
        /// </summary>
        private void FixupRefs7(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref7 item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Refs7", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref7 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Refs7", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionCategories.
        /// </summary>
        private void FixupActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionCategory item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Equipments.
        /// </summary>
        private void FixupEquipments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Equipment item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Equipments", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Equipment item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Equipments", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Operators.
        /// </summary>
        private void FixupOperators(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Operator item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Operators", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Operator item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Operators", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Publications.
        /// </summary>
        private void FixupPublications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Publication item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Publications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Publication item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Publications", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LinkedActions.
        /// </summary>
        private void FixupLinkedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KAction item in e.NewItems)
                {
                    item.LinkedProcess = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LinkedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.LinkedProcess, this))
                    {
                        item.LinkedProcess = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LinkedActions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Videos.
        /// </summary>
        private void FixupVideos(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Video item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Videos", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Video item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Videos", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété VideoSyncs.
        /// </summary>
        private void FixupVideoSyncs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (VideoSync item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("VideoSyncs", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (VideoSync item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("VideoSyncs", item);
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
        /// Corrige l'état de la propriété InspectionSchedules.
        /// </summary>
        private void FixupInspectionSchedules(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (InspectionSchedule item in e.NewItems)
                {
                    item.Procedure = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("InspectionSchedules", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (InspectionSchedule item in e.OldItems)
                {
                    if (ReferenceEquals(item.Procedure, this))
                    {
                        item.Procedure = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionSchedules", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété UserRoleProcesses.
        /// </summary>
        private void FixupUserRoleProcesses(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (UserRoleProcess item in e.NewItems)
                {
                    item.Procedure = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("UserRoleProcesses", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserRoleProcess item in e.OldItems)
                {
                    if (ReferenceEquals(item.Procedure, this))
                    {
                        item.Procedure = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("UserRoleProcesses", item);
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
        /// Corrige l'état de la propriété PublicationHistories.
        /// </summary>
        private void FixupPublicationHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationHistory item in e.NewItems)
                {
                    item.PublishedProcess = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublicationHistories", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.PublishedProcess, this))
                    {
                        item.PublishedProcess = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublicationHistories", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété DocumentationReferentials.
        /// </summary>
        private void FixupDocumentationReferentials(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationReferential item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationReferentials", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationReferential item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationReferentials", item);
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
        /// Corrige l'état de la propriété DocumentationDraftLocalizations.
        /// </summary>
        private void FixupDocumentationDraftLocalizations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationDraftLocalization item in e.NewItems)
                {
                    item.Process = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationDraftLocalizations", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationDraftLocalization item in e.OldItems)
                {
                    if (ReferenceEquals(item.Process, this))
                    {
                        item.Process = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationDraftLocalizations", item);
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
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "ProjectDirId":
    				this.ProjectDirId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "IsSkill":
    				this.IsSkill = (bool)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "OwnerId":
    				this.OwnerId = Convert.ToInt32(value);
    				break;
    			case "ProjectDir":
    				this.ProjectDir = (ProjectDir)value;
    				break;
    			case "Owner":
    				this.Owner = (User)value;
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
    			case "Projects":
    				this.Projects.Add((Project)value);
    				break;
    			case "Refs1":
    				this.Refs1.Add((Ref1)value);
    				break;
    			case "Refs2":
    				this.Refs2.Add((Ref2)value);
    				break;
    			case "Refs3":
    				this.Refs3.Add((Ref3)value);
    				break;
    			case "Refs4":
    				this.Refs4.Add((Ref4)value);
    				break;
    			case "Refs5":
    				this.Refs5.Add((Ref5)value);
    				break;
    			case "Refs6":
    				this.Refs6.Add((Ref6)value);
    				break;
    			case "Refs7":
    				this.Refs7.Add((Ref7)value);
    				break;
    			case "ActionCategories":
    				this.ActionCategories.Add((ActionCategory)value);
    				break;
    			case "Equipments":
    				this.Equipments.Add((Equipment)value);
    				break;
    			case "Operators":
    				this.Operators.Add((Operator)value);
    				break;
    			case "Publications":
    				this.Publications.Add((Publication)value);
    				break;
    			case "LinkedActions":
    				this.LinkedActions.Add((KAction)value);
    				break;
    			case "Videos":
    				this.Videos.Add((Video)value);
    				break;
    			case "VideoSyncs":
    				this.VideoSyncs.Add((VideoSync)value);
    				break;
    			case "InspectionSchedules":
    				this.InspectionSchedules.Add((InspectionSchedule)value);
    				break;
    			case "UserRoleProcesses":
    				this.UserRoleProcesses.Add((UserRoleProcess)value);
    				break;
    			case "PublicationHistories":
    				this.PublicationHistories.Add((PublicationHistory)value);
    				break;
    			case "DocumentationReferentials":
    				this.DocumentationReferentials.Add((DocumentationReferential)value);
    				break;
    			case "DocumentationDraftLocalizations":
    				this.DocumentationDraftLocalizations.Add((DocumentationDraftLocalization)value);
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
    			case "Projects":
    				this.Projects.Remove((Project)value);
    				break;
    			case "Refs1":
    				this.Refs1.Remove((Ref1)value);
    				break;
    			case "Refs2":
    				this.Refs2.Remove((Ref2)value);
    				break;
    			case "Refs3":
    				this.Refs3.Remove((Ref3)value);
    				break;
    			case "Refs4":
    				this.Refs4.Remove((Ref4)value);
    				break;
    			case "Refs5":
    				this.Refs5.Remove((Ref5)value);
    				break;
    			case "Refs6":
    				this.Refs6.Remove((Ref6)value);
    				break;
    			case "Refs7":
    				this.Refs7.Remove((Ref7)value);
    				break;
    			case "ActionCategories":
    				this.ActionCategories.Remove((ActionCategory)value);
    				break;
    			case "Equipments":
    				this.Equipments.Remove((Equipment)value);
    				break;
    			case "Operators":
    				this.Operators.Remove((Operator)value);
    				break;
    			case "Publications":
    				this.Publications.Remove((Publication)value);
    				break;
    			case "LinkedActions":
    				this.LinkedActions.Remove((KAction)value);
    				break;
    			case "Videos":
    				this.Videos.Remove((Video)value);
    				break;
    			case "VideoSyncs":
    				this.VideoSyncs.Remove((VideoSync)value);
    				break;
    			case "InspectionSchedules":
    				this.InspectionSchedules.Remove((InspectionSchedule)value);
    				break;
    			case "UserRoleProcesses":
    				this.UserRoleProcesses.Remove((UserRoleProcess)value);
    				break;
    			case "PublicationHistories":
    				this.PublicationHistories.Remove((PublicationHistory)value);
    				break;
    			case "DocumentationReferentials":
    				this.DocumentationReferentials.Remove((DocumentationReferential)value);
    				break;
    			case "DocumentationDraftLocalizations":
    				this.DocumentationDraftLocalizations.Remove((DocumentationDraftLocalization)value);
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
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("Label", this.Label);
    		values.Add("ProjectDirId", this.ProjectDirId);
    		values.Add("Description", this.Description);
    		values.Add("IsSkill", this.IsSkill);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("OwnerId", this.OwnerId);
    		values.Add("ProjectDir", this.ProjectDir);
    		values.Add("Owner", this.Owner);
    
    		values.Add("Projects", GetHashCodes(this.Projects));
    		values.Add("Refs1", GetHashCodes(this.Refs1));
    		values.Add("Refs2", GetHashCodes(this.Refs2));
    		values.Add("Refs3", GetHashCodes(this.Refs3));
    		values.Add("Refs4", GetHashCodes(this.Refs4));
    		values.Add("Refs5", GetHashCodes(this.Refs5));
    		values.Add("Refs6", GetHashCodes(this.Refs6));
    		values.Add("Refs7", GetHashCodes(this.Refs7));
    		values.Add("ActionCategories", GetHashCodes(this.ActionCategories));
    		values.Add("Equipments", GetHashCodes(this.Equipments));
    		values.Add("Operators", GetHashCodes(this.Operators));
    		values.Add("Publications", GetHashCodes(this.Publications));
    		values.Add("LinkedActions", GetHashCodes(this.LinkedActions));
    		values.Add("Videos", GetHashCodes(this.Videos));
    		values.Add("VideoSyncs", GetHashCodes(this.VideoSyncs));
    		values.Add("InspectionSchedules", GetHashCodes(this.InspectionSchedules));
    		values.Add("UserRoleProcesses", GetHashCodes(this.UserRoleProcesses));
    		values.Add("PublicationHistories", GetHashCodes(this.PublicationHistories));
    		values.Add("DocumentationReferentials", GetHashCodes(this.DocumentationReferentials));
    		values.Add("DocumentationDraftLocalizations", GetHashCodes(this.DocumentationDraftLocalizations));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("ProjectDir", this.ProjectDir);
    		values.Add("Owner", this.Owner);
    
    		values.Add("Projects", this.Projects);
    		values.Add("Refs1", this.Refs1);
    		values.Add("Refs2", this.Refs2);
    		values.Add("Refs3", this.Refs3);
    		values.Add("Refs4", this.Refs4);
    		values.Add("Refs5", this.Refs5);
    		values.Add("Refs6", this.Refs6);
    		values.Add("Refs7", this.Refs7);
    		values.Add("ActionCategories", this.ActionCategories);
    		values.Add("Equipments", this.Equipments);
    		values.Add("Operators", this.Operators);
    		values.Add("Publications", this.Publications);
    		values.Add("LinkedActions", this.LinkedActions);
    		values.Add("Videos", this.Videos);
    		values.Add("VideoSyncs", this.VideoSyncs);
    		values.Add("InspectionSchedules", this.InspectionSchedules);
    		values.Add("UserRoleProcesses", this.UserRoleProcesses);
    		values.Add("PublicationHistories", this.PublicationHistories);
    		values.Add("DocumentationReferentials", this.DocumentationReferentials);
    		values.Add("DocumentationDraftLocalizations", this.DocumentationDraftLocalizations);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Label.
        /// </summary>
    	public const int LabelMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ Description.
        /// </summary>
    	public const int DescriptionMaxLength = 4000;

        #endregion

    }
}
