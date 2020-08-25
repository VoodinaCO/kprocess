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
    [KnownType(typeof(Objective))]
    [KnownType(typeof(Project))]
    [KnownType(typeof(User))]
    [KnownType(typeof(ProjectReferential))]
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(Publication))]
    /// <summary>
    /// Représente un projet.
    /// </summary>
    public partial class Project : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Project";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Project"/>.
        /// </summary>
    	public Project()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _projectId;
        /// <summary>
        /// Obtient ou définit l'identifiant du projet.
        /// </summary>
        [DataMember]
        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (_projectId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ProjectId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _projectId = value;
                    OnEntityPropertyChanged("ProjectId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé du projet.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Referential_Label_Required")]
    	[LocalizableStringLength(LabelMaxLength, ErrorMessageResourceName = "Validation_Project_Label_StringLength")]
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
        /// Obtient ou définit la description du projet.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(DescriptionMaxLength, ErrorMessageResourceName = "Validation_Project_Description_StringLength")]
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
        
        private string _objectiveCode;
        /// <summary>
        /// Obtient ou définit le code de l'objectif.
        /// </summary>
        [DataMember]
        public string ObjectiveCode
        {
            get { return _objectiveCode; }
            set
            {
                if (_objectiveCode != value)
                {
                    ChangeTracker.RecordValue("ObjectiveCode", _objectiveCode, value);
                    if (!IsDeserializing)
                    {
                        if (Objective != null && Objective.ObjectiveCode != value)
                        {
                            Objective = null;
                        }
                    }
                    _objectiveCode = value;
                    OnEntityPropertyChanged("ObjectiveCode");
                }
            }
        }
        
        private string _otherObjectiveLabel;
        /// <summary>
        /// Obtient ou définit le libellé d'un autre objectif.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(OtherObjectiveLabelMaxLength, ErrorMessageResourceName = "Validation_Project_OtherObjectiveLabel_StringLength")]
        public string OtherObjectiveLabel
        {
            get { return _otherObjectiveLabel; }
            set
            {
                if (_otherObjectiveLabel != value)
                {
                    ChangeTracker.RecordValue("OtherObjectiveLabel", _otherObjectiveLabel, value);
                    _otherObjectiveLabel = value;
                    OnEntityPropertyChanged("OtherObjectiveLabel");
                }
            }
        }
        
        private string _customTextLabel;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir du texte n° 1.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextLabelMaxLength, ErrorMessageResourceName = "Validation_Project_CustomTextLabel_StringLength")]
        public string CustomTextLabel
        {
            get { return _customTextLabel; }
            set
            {
                if (_customTextLabel != value)
                {
                    ChangeTracker.RecordValue("CustomTextLabel", _customTextLabel, value);
                    _customTextLabel = value;
                    OnEntityPropertyChanged("CustomTextLabel");
                }
            }
        }
        
        private string _customTextLabel2;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir du texte n° 2.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextLabel2MaxLength, ErrorMessageResourceName = "Validation_Project_CustomTextLabel2_StringLength")]
        public string CustomTextLabel2
        {
            get { return _customTextLabel2; }
            set
            {
                if (_customTextLabel2 != value)
                {
                    ChangeTracker.RecordValue("CustomTextLabel2", _customTextLabel2, value);
                    _customTextLabel2 = value;
                    OnEntityPropertyChanged("CustomTextLabel2");
                }
            }
        }
        
        private string _customTextLabel3;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir du texte n° 3.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextLabel3MaxLength, ErrorMessageResourceName = "Validation_Project_CustomTextLabel3_StringLength")]
        public string CustomTextLabel3
        {
            get { return _customTextLabel3; }
            set
            {
                if (_customTextLabel3 != value)
                {
                    ChangeTracker.RecordValue("CustomTextLabel3", _customTextLabel3, value);
                    _customTextLabel3 = value;
                    OnEntityPropertyChanged("CustomTextLabel3");
                }
            }
        }
        
        private string _customTextLabel4;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir du texte n°4.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextLabel4MaxLength, ErrorMessageResourceName = "Validation_Project_CustomTextLabel4_StringLength")]
        public string CustomTextLabel4
        {
            get { return _customTextLabel4; }
            set
            {
                if (_customTextLabel4 != value)
                {
                    ChangeTracker.RecordValue("CustomTextLabel4", _customTextLabel4, value);
                    _customTextLabel4 = value;
                    OnEntityPropertyChanged("CustomTextLabel4");
                }
            }
        }
        
        private string _customNumericLabel;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir une valeur numérique n° 1.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomNumericLabelMaxLength, ErrorMessageResourceName = "Validation_Project_CustomNumericLabel_StringLength")]
        public string CustomNumericLabel
        {
            get { return _customNumericLabel; }
            set
            {
                if (_customNumericLabel != value)
                {
                    ChangeTracker.RecordValue("CustomNumericLabel", _customNumericLabel, value);
                    _customNumericLabel = value;
                    OnEntityPropertyChanged("CustomNumericLabel");
                }
            }
        }
        
        private string _customNumericLabel2;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir une valeur numérique n° 2.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomNumericLabel2MaxLength, ErrorMessageResourceName = "Validation_Project_CustomNumericLabel2_StringLength")]
        public string CustomNumericLabel2
        {
            get { return _customNumericLabel2; }
            set
            {
                if (_customNumericLabel2 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericLabel2", _customNumericLabel2, value);
                    _customNumericLabel2 = value;
                    OnEntityPropertyChanged("CustomNumericLabel2");
                }
            }
        }
        
        private string _customNumericLabel3;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir une valeur numérique n° 3.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomNumericLabel3MaxLength, ErrorMessageResourceName = "Validation_Project_CustomNumericLabel3_StringLength")]
        public string CustomNumericLabel3
        {
            get { return _customNumericLabel3; }
            set
            {
                if (_customNumericLabel3 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericLabel3", _customNumericLabel3, value);
                    _customNumericLabel3 = value;
                    OnEntityPropertyChanged("CustomNumericLabel3");
                }
            }
        }
        
        private string _customNumericLabel4;
        /// <summary>
        /// Obtient ou définit le libellé du champ libre pouvant contenir une valeur numérique n° 4.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomNumericLabel4MaxLength, ErrorMessageResourceName = "Validation_Project_CustomNumericLabel4_StringLength")]
        public string CustomNumericLabel4
        {
            get { return _customNumericLabel4; }
            set
            {
                if (_customNumericLabel4 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericLabel4", _customNumericLabel4, value);
                    _customNumericLabel4 = value;
                    OnEntityPropertyChanged("CustomNumericLabel4");
                }
            }
        }
        
        private long _timeScale;
        /// <summary>
        /// Obtient ou définit l'échelle de temps.
        /// </summary>
        [DataMember]
    	[LocalizableRange(1, long.MaxValue, ErrorMessageResourceName = "Validation_Project_TimeScale_Required")]
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
        
        private Nullable<int> _parentProjectId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ParentProjectId
        {
            get { return _parentProjectId; }
            set
            {
                if (_parentProjectId != value)
                {
                    ChangeTracker.RecordValue("ParentProjectId", _parentProjectId, value);
                    if (!IsDeserializing)
                    {
                        if (ProjectParent != null && ProjectParent.ProjectId != value)
                        {
                            ProjectParent = null;
                        }
                    }
                    _parentProjectId = value;
                    OnEntityPropertyChanged("ParentProjectId");
                }
            }
        }
        
        private int _processId;
        /// <summary>
        /// Obtient ou définit l'identifiant du process associé à ce projet.
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
        
        private string _workshop;
        /// <summary>
        /// Obtient ou définit l'atelier sur lequel porte le projet.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(WorkshopMaxLength, ErrorMessageResourceName = "Validation_Project_Workshop_StringLength")]
        public string Workshop
        {
            get { return _workshop; }
            set
            {
                if (_workshop != value)
                {
                    ChangeTracker.RecordValue("Workshop", _workshop, value);
                    _workshop = value;
                    OnEntityPropertyChanged("Workshop");
                }
            }
        }
        
        private System.DateTime _startDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Project_StartDate_Required")]
        public System.DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    ChangeTracker.RecordValue("StartDate", _startDate, value);
                    _startDate = value;
                    OnEntityPropertyChanged("StartDate");
                }
            }
        }
        
        private Nullable<System.DateTime> _forecastEndDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> ForecastEndDate
        {
            get { return _forecastEndDate; }
            set
            {
                if (_forecastEndDate != value)
                {
                    ChangeTracker.RecordValue("ForecastEndDate", _forecastEndDate, value);
                    _forecastEndDate = value;
                    OnEntityPropertyChanged("ForecastEndDate");
                }
            }
        }
        
        private Nullable<System.DateTime> _realEndDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> RealEndDate
        {
            get { return _realEndDate; }
            set
            {
                if (_realEndDate != value)
                {
                    ChangeTracker.RecordValue("RealEndDate", _realEndDate, value);
                    _realEndDate = value;
                    OnEntityPropertyChanged("RealEndDate");
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
        
        private bool _isAbandoned;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsAbandoned
        {
            get { return _isAbandoned; }
            set
            {
                if (_isAbandoned != value)
                {
                    ChangeTracker.RecordValue("IsAbandoned", _isAbandoned, value);
                    _isAbandoned = value;
                    OnEntityPropertyChanged("IsAbandoned");
                }
            }
        }
        
        private byte[] _other_Disposition;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Other_Disposition
        {
            get { return _other_Disposition; }
            set
            {
                if (_other_Disposition != value)
                {
                    ChangeTracker.RecordValue("Other_Disposition", _other_Disposition, value);
                    _other_Disposition = value;
                    OnEntityPropertyChanged("Other_Disposition");
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

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit l'objectif de ce projet.
        /// </summary>
        [DataMember]
        public Objective Objective
        {
            get { return _objective; }
            set
            {
                if (!ReferenceEquals(_objective, value))
                {
                    var previousValue = _objective;
                    _objective = value;
                    FixupObjective(previousValue);
                    OnNavigationPropertyChanged("Objective");
                }
            }
        }
        private Objective _objective;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Project> ProjectChilds
        {
            get
            {
                if (_projectChilds == null)
                {
                    _projectChilds = new TrackableCollection<Project>();
                    _projectChilds.CollectionChanged += FixupProjectChilds;
                }
                return _projectChilds;
            }
            set
            {
                if (!ReferenceEquals(_projectChilds, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_projectChilds != null)
                    {
                        _projectChilds.CollectionChanged -= FixupProjectChilds;
                    }
                    _projectChilds = value;
                    if (_projectChilds != null)
                    {
                        _projectChilds.CollectionChanged += FixupProjectChilds;
                    }
                    OnNavigationPropertyChanged("ProjectChilds");
                }
            }
        }
        private TrackableCollection<Project> _projectChilds;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Project ProjectParent
        {
            get { return _projectParent; }
            set
            {
                if (!ReferenceEquals(_projectParent, value))
                {
                    var previousValue = _projectParent;
                    _projectParent = value;
                    FixupProjectParent(previousValue);
                    OnNavigationPropertyChanged("ProjectParent");
                }
            }
        }
        private Project _projectParent;
    				
    
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
        /// Obtient ou définit les définitions d'utilisation des référentiels associés à ce projet.
        /// </summary>
        [DataMember]
        public TrackableCollection<ProjectReferential> Referentials
        {
            get
            {
                if (_referentials == null)
                {
                    _referentials = new TrackableCollection<ProjectReferential>();
                    _referentials.CollectionChanged += FixupReferentials;
                }
                return _referentials;
            }
            set
            {
                if (!ReferenceEquals(_referentials, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_referentials != null)
                    {
                        _referentials.CollectionChanged -= FixupReferentials;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (ProjectReferential item in _referentials)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _referentials = value;
                    if (_referentials != null)
                    {
                        _referentials.CollectionChanged += FixupReferentials;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (ProjectReferential item in _referentials)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Referentials");
                }
            }
        }
        private TrackableCollection<ProjectReferential> _referentials;
    
        /// <summary>
        /// Obtient ou définit le process associé à ce projet.
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
        /// Obtient ou définit les scénarios de ce projet.
        /// </summary>
        [DataMember]
        public TrackableCollection<Scenario> Scenarios
        {
            get
            {
                if (_scenarios == null)
                {
                    _scenarios = new TrackableCollection<Scenario>();
                    _scenarios.CollectionChanged += FixupScenarios;
                }
                return _scenarios;
            }
            set
            {
                if (!ReferenceEquals(_scenarios, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_scenarios != null)
                    {
                        _scenarios.CollectionChanged -= FixupScenarios;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Scenario item in _scenarios)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _scenarios = value;
                    if (_scenarios != null)
                    {
                        _scenarios.CollectionChanged += FixupScenarios;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Scenario item in _scenarios)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Scenarios");
                }
            }
        }
        private TrackableCollection<Scenario> _scenarios;
    
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
        /// Gère la suppression en cascade.
        /// This entity type is the dependent end in at least one association that performs cascade deletes.
        /// This event handler will process notifications that occur when the principal end is deleted.
        /// </summary>
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
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
            Objective = null;
            ProjectChilds.Clear();
            ProjectParent = null;
            Creator = null;
            LastModifier = null;
            Referentials.Clear();
            Process = null;
            Scenarios.Clear();
            Publications.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Objective.
        /// </summary>
        private void FixupObjective(Objective previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Projects.Contains(this))
            {
                previousValue.Projects.Remove(this);
            }
    
            if (Objective != null)
            {
                if (!Objective.Projects.Contains(this))
                {
                    Objective.Projects.Add(this);
                }
    
                ObjectiveCode = Objective.ObjectiveCode;
            }
            else if (!skipKeys)
            {
                ObjectiveCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Objective", previousValue, Objective);
                if (Objective != null && !Objective.ChangeTracker.ChangeTrackingEnabled)
                {
                    Objective.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ProjectParent.
        /// </summary>
        private void FixupProjectParent(Project previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ProjectChilds.Contains(this))
            {
                previousValue.ProjectChilds.Remove(this);
            }
    
            if (ProjectParent != null)
            {
                if (!ProjectParent.ProjectChilds.Contains(this))
                {
                    ProjectParent.ProjectChilds.Add(this);
                }
    
                ParentProjectId = ProjectParent.ProjectId;
            }
            else if (!skipKeys)
            {
                ParentProjectId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("ProjectParent", previousValue, ProjectParent);
                if (ProjectParent != null && !ProjectParent.ChangeTracker.ChangeTrackingEnabled)
                {
                    ProjectParent.StartTracking();
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
    
            if (previousValue != null && previousValue.CreatedProjects.Contains(this))
            {
                previousValue.CreatedProjects.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedProjects.Contains(this))
                {
                    Creator.CreatedProjects.Add(this);
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
    
            if (previousValue != null && previousValue.LastModifiedProjects.Contains(this))
            {
                previousValue.LastModifiedProjects.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedProjects.Contains(this))
                {
                    LastModifier.LastModifiedProjects.Add(this);
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
    
            if (previousValue != null && previousValue.Projects.Contains(this))
            {
                previousValue.Projects.Remove(this);
            }
    
            if (Process != null)
            {
                if (!Process.Projects.Contains(this))
                {
                    Process.Projects.Add(this);
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
        /// Corrige l'état de la propriété ProjectChilds.
        /// </summary>
        private void FixupProjectChilds(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Project item in e.NewItems)
                {
                    item.ProjectParent = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ProjectChilds", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Project item in e.OldItems)
                {
                    if (ReferenceEquals(item.ProjectParent, this))
                    {
                        item.ProjectParent = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ProjectChilds", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Referentials.
        /// </summary>
        private void FixupReferentials(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ProjectReferential item in e.NewItems)
                {
                    item.Project = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Referentials", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProjectReferential item in e.OldItems)
                {
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Referentials", item);
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
        /// Corrige l'état de la propriété Scenarios.
        /// </summary>
        private void FixupScenarios(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Scenario item in e.NewItems)
                {
                    item.Project = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Scenarios", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Scenario item in e.OldItems)
                {
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Scenarios", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
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
                    item.Project = this;
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
                    if (ReferenceEquals(item.Project, this))
                    {
                        item.Project = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Publications", item);
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
    			case "ProjectId":
    				this.ProjectId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "ObjectiveCode":
    				this.ObjectiveCode = (string)value;
    				break;
    			case "OtherObjectiveLabel":
    				this.OtherObjectiveLabel = (string)value;
    				break;
    			case "CustomTextLabel":
    				this.CustomTextLabel = (string)value;
    				break;
    			case "CustomTextLabel2":
    				this.CustomTextLabel2 = (string)value;
    				break;
    			case "CustomTextLabel3":
    				this.CustomTextLabel3 = (string)value;
    				break;
    			case "CustomTextLabel4":
    				this.CustomTextLabel4 = (string)value;
    				break;
    			case "CustomNumericLabel":
    				this.CustomNumericLabel = (string)value;
    				break;
    			case "CustomNumericLabel2":
    				this.CustomNumericLabel2 = (string)value;
    				break;
    			case "CustomNumericLabel3":
    				this.CustomNumericLabel3 = (string)value;
    				break;
    			case "CustomNumericLabel4":
    				this.CustomNumericLabel4 = (string)value;
    				break;
    			case "TimeScale":
    				this.TimeScale = (long)value;
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
    			case "ParentProjectId":
    				this.ParentProjectId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "Workshop":
    				this.Workshop = (string)value;
    				break;
    			case "StartDate":
    				this.StartDate = (System.DateTime)value;
    				break;
    			case "ForecastEndDate":
    				this.ForecastEndDate = (Nullable<System.DateTime>)value;
    				break;
    			case "RealEndDate":
    				this.RealEndDate = (Nullable<System.DateTime>)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
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
    			case "IsAbandoned":
    				this.IsAbandoned = (bool)value;
    				break;
    			case "Other_Disposition":
    				this.Other_Disposition = (byte[])value;
    				break;
    			case "Evaluation_Disposition":
    				this.Evaluation_Disposition = (byte[])value;
    				break;
    			case "Objective":
    				this.Objective = (Objective)value;
    				break;
    			case "ProjectParent":
    				this.ProjectParent = (Project)value;
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
    			case "ProjectChilds":
    				this.ProjectChilds.Add((Project)value);
    				break;
    			case "Referentials":
    				this.Referentials.Add((ProjectReferential)value);
    				break;
    			case "Scenarios":
    				this.Scenarios.Add((Scenario)value);
    				break;
    			case "Publications":
    				this.Publications.Add((Publication)value);
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
    			case "ProjectChilds":
    				this.ProjectChilds.Remove((Project)value);
    				break;
    			case "Referentials":
    				this.Referentials.Remove((ProjectReferential)value);
    				break;
    			case "Scenarios":
    				this.Scenarios.Remove((Scenario)value);
    				break;
    			case "Publications":
    				this.Publications.Remove((Publication)value);
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
    		values.Add("ProjectId", this.ProjectId);
    		values.Add("Label", this.Label);
    		values.Add("Description", this.Description);
    		values.Add("ObjectiveCode", this.ObjectiveCode);
    		values.Add("OtherObjectiveLabel", this.OtherObjectiveLabel);
    		values.Add("CustomTextLabel", this.CustomTextLabel);
    		values.Add("CustomTextLabel2", this.CustomTextLabel2);
    		values.Add("CustomTextLabel3", this.CustomTextLabel3);
    		values.Add("CustomTextLabel4", this.CustomTextLabel4);
    		values.Add("CustomNumericLabel", this.CustomNumericLabel);
    		values.Add("CustomNumericLabel2", this.CustomNumericLabel2);
    		values.Add("CustomNumericLabel3", this.CustomNumericLabel3);
    		values.Add("CustomNumericLabel4", this.CustomNumericLabel4);
    		values.Add("TimeScale", this.TimeScale);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("ParentProjectId", this.ParentProjectId);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("Workshop", this.Workshop);
    		values.Add("StartDate", this.StartDate);
    		values.Add("ForecastEndDate", this.ForecastEndDate);
    		values.Add("RealEndDate", this.RealEndDate);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Formation_Disposition", this.Formation_Disposition);
    		values.Add("Inspection_Disposition", this.Inspection_Disposition);
    		values.Add("Audit_Disposition", this.Audit_Disposition);
    		values.Add("IsAbandoned", this.IsAbandoned);
    		values.Add("Other_Disposition", this.Other_Disposition);
    		values.Add("Evaluation_Disposition", this.Evaluation_Disposition);
    		values.Add("Objective", this.Objective);
    		values.Add("ProjectParent", this.ProjectParent);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    
    		values.Add("ProjectChilds", GetHashCodes(this.ProjectChilds));
    		values.Add("Referentials", GetHashCodes(this.Referentials));
    		values.Add("Scenarios", GetHashCodes(this.Scenarios));
    		values.Add("Publications", GetHashCodes(this.Publications));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Objective", this.Objective);
    		values.Add("ProjectParent", this.ProjectParent);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    
    		values.Add("ProjectChilds", this.ProjectChilds);
    		values.Add("Referentials", this.Referentials);
    		values.Add("Scenarios", this.Scenarios);
    		values.Add("Publications", this.Publications);
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
    
        /// <summary>
        /// Taille maximum du champ OtherObjectiveLabel.
        /// </summary>
    	public const int OtherObjectiveLabelMaxLength = 50;
    
        /// <summary>
        /// Taille maximum du champ CustomTextLabel.
        /// </summary>
    	public const int CustomTextLabelMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomTextLabel2.
        /// </summary>
    	public const int CustomTextLabel2MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomTextLabel3.
        /// </summary>
    	public const int CustomTextLabel3MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomTextLabel4.
        /// </summary>
    	public const int CustomTextLabel4MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomNumericLabel.
        /// </summary>
    	public const int CustomNumericLabelMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomNumericLabel2.
        /// </summary>
    	public const int CustomNumericLabel2MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomNumericLabel3.
        /// </summary>
    	public const int CustomNumericLabel3MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ CustomNumericLabel4.
        /// </summary>
    	public const int CustomNumericLabel4MaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ Workshop.
        /// </summary>
    	public const int WorkshopMaxLength = 4000;

        #endregion

    }
}
