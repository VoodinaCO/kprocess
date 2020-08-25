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
    [KnownType(typeof(Project))]
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(ScenarioNature))]
    [KnownType(typeof(ScenarioState))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Solution))]
    [KnownType(typeof(Publication))]
    [KnownType(typeof(DocumentationDraft))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Scenario : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Scenario";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Scenario"/>.
        /// </summary>
    	public Scenario()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _scenarioId;
        /// <summary>
        /// Obtient ou définit l'identifiant d'un scénario.
        /// </summary>
        [DataMember]
        public int ScenarioId
        {
            get { return _scenarioId; }
            set
            {
                if (_scenarioId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ScenarioId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _scenarioId = value;
                    OnEntityPropertyChanged("ScenarioId");
                }
            }
        }
        
        private int _projectId;
        /// <summary>
        /// Obtient ou définit l'identifiant du projet associé.
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
        
        private string _stateCode;
        /// <summary>
        /// Obtient ou définit le code identifiant l'état du scénario.
        /// </summary>
        [DataMember]
        public string StateCode
        {
            get { return _stateCode; }
            set
            {
                if (_stateCode != value)
                {
                    ChangeTracker.RecordValue("StateCode", _stateCode, value);
                    if (!IsDeserializing)
                    {
                        if (State != null && State.ScenarioStateCode != value)
                        {
                            State = null;
                        }
                    }
    				var oldValue = _stateCode;
                    _stateCode = value;
                    OnEntityPropertyChanged("StateCode");
    				OnStateCodeChanged(oldValue, value);
    				OnStateCodeChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="StateCode"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnStateCodeChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> StateCodeChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="StateCode"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnStateCodeChanged(string oldValue, string newValue)
    	{
    		if (StateCodeChanged != null)
    			StateCodeChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private string _natureCode;
        /// <summary>
        /// Obtient ou définit le code identifiant la nature du scénario.
        /// </summary>
        [DataMember]
        public string NatureCode
        {
            get { return _natureCode; }
            set
            {
                if (_natureCode != value)
                {
                    ChangeTracker.RecordValue("NatureCode", _natureCode, value);
                    if (!IsDeserializing)
                    {
                        if (Nature != null && Nature.ScenarioNatureCode != value)
                        {
                            Nature = null;
                        }
                    }
                    _natureCode = value;
                    OnEntityPropertyChanged("NatureCode");
                }
            }
        }
        
        private Nullable<int> _originalScenarioId;
        /// <summary>
        /// Obtient ou définit l'identifiant du scénario d'origine.
        /// </summary>
        [DataMember]
        public Nullable<int> OriginalScenarioId
        {
            get { return _originalScenarioId; }
            set
            {
                if (_originalScenarioId != value)
                {
                    ChangeTracker.RecordValue("OriginalScenarioId", _originalScenarioId, value);
                    if (!IsDeserializing)
                    {
                        if (Original != null && Original.ScenarioId != value)
                        {
                            Original = null;
                        }
                    }
                    _originalScenarioId = value;
                    OnEntityPropertyChanged("OriginalScenarioId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé du scénario.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Scenario_Name_Required")]
    	[LocalizableStringLength(LabelMaxLength, ErrorMessageResourceName = "Validation_Scenario_Label_StringLength")]
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
        /// Obtient ou définit la description du scénario.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(DescriptionMaxLength, ErrorMessageResourceName = "Validation_Scenario_Description_StringLength")]
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
        
        private bool _isShownInSummary;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le scénario doit être affiché dans la synthèse.
        /// </summary>
        [DataMember]
        public bool IsShownInSummary
        {
            get { return _isShownInSummary; }
            set
            {
                if (_isShownInSummary != value)
                {
                    ChangeTracker.RecordValue("IsShownInSummary", _isShownInSummary, value);
                    _isShownInSummary = value;
                    OnEntityPropertyChanged("IsShownInSummary");
                }
            }
        }
        
        private long _criticalPathIDuration;
        /// <summary>
        /// Obtient ou définit la durée du chemin critique avec un filtre I.
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
        
        private Nullable<System.Guid> _webPublicationGuid;
        /// <summary>
        /// Obtient ou définit l'identifiant de publication sur le site web.
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> WebPublicationGuid
        {
            get { return _webPublicationGuid; }
            set
            {
                if (_webPublicationGuid != value)
                {
                    ChangeTracker.RecordValue("WebPublicationGuid", _webPublicationGuid, value);
                    _webPublicationGuid = value;
                    OnEntityPropertyChanged("WebPublicationGuid");
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

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit les actions associés à ce scénario.
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
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (KAction item in _actions)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _actions = value;
                    if (_actions != null)
                    {
                        _actions.CollectionChanged += FixupActions;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (KAction item in _actions)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Actions");
                }
            }
        }
        private TrackableCollection<KAction> _actions;
    
        /// <summary>
        /// Obtient ou définit le projet associé à ce scénario.
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
        public TrackableCollection<Scenario> InheritedScenarios
        {
            get
            {
                if (_inheritedScenarios == null)
                {
                    _inheritedScenarios = new TrackableCollection<Scenario>();
                    _inheritedScenarios.CollectionChanged += FixupInheritedScenarios;
                }
                return _inheritedScenarios;
            }
            set
            {
                if (!ReferenceEquals(_inheritedScenarios, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inheritedScenarios != null)
                    {
                        _inheritedScenarios.CollectionChanged -= FixupInheritedScenarios;
                    }
                    _inheritedScenarios = value;
                    if (_inheritedScenarios != null)
                    {
                        _inheritedScenarios.CollectionChanged += FixupInheritedScenarios;
                    }
                    OnNavigationPropertyChanged("InheritedScenarios");
                }
            }
        }
        private TrackableCollection<Scenario> _inheritedScenarios;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Scenario Original
        {
            get { return _original; }
            set
            {
                if (!ReferenceEquals(_original, value))
                {
                    var previousValue = _original;
                    _original = value;
                    FixupOriginal(previousValue);
                    OnNavigationPropertyChanged("Original");
                }
            }
        }
        private Scenario _original;
    				
    
        /// <summary>
        /// Obtient ou définit la nature associée à ce scénario.
        /// </summary>
        [DataMember]
        public ScenarioNature Nature
        {
            get { return _nature; }
            set
            {
                if (!ReferenceEquals(_nature, value))
                {
                    var previousValue = _nature;
                    _nature = value;
                    FixupNature(previousValue);
                    OnNavigationPropertyChanged("Nature");
                }
            }
        }
        private ScenarioNature _nature;
    				
    
        /// <summary>
        /// Obtient ou définit l'état associé à ce scénario.
        /// </summary>
        [DataMember]
        public ScenarioState State
        {
            get { return _state; }
            set
            {
                if (!ReferenceEquals(_state, value))
                {
                    var previousValue = _state;
                    _state = value;
                    FixupState(previousValue);
                    OnNavigationPropertyChanged("State");
    				OnStateChanged(previousValue, value);
    				OnStateChangedPartial(previousValue, value);
                }
            }
        }
        private ScenarioState _state;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="State"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnStateChangedPartial(ScenarioState oldValue, ScenarioState newValue);
    	public event EventHandler<PropertyChangedEventArgs<ScenarioState>> StateChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="State"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnStateChanged(ScenarioState oldValue, ScenarioState newValue)
    	{
    		if (StateChanged != null)
    			StateChanged(this, new PropertyChangedEventArgs<ScenarioState>(oldValue, newValue));
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
        /// Obtient ou définit les solutions associées.
        /// </summary>
        [DataMember]
        public TrackableCollection<Solution> Solutions
        {
            get
            {
                if (_solutions == null)
                {
                    _solutions = new TrackableCollection<Solution>();
                    _solutions.CollectionChanged += FixupSolutions;
                }
                return _solutions;
            }
            set
            {
                if (!ReferenceEquals(_solutions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_solutions != null)
                    {
                        _solutions.CollectionChanged -= FixupSolutions;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Solution item in _solutions)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _solutions = value;
                    if (_solutions != null)
                    {
                        _solutions.CollectionChanged += FixupSolutions;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Solution item in _solutions)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Solutions");
                }
            }
        }
        private TrackableCollection<Solution> _solutions;
    
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
        public TrackableCollection<DocumentationDraft> DocumentationDrafts
        {
            get
            {
                if (_documentationDrafts == null)
                {
                    _documentationDrafts = new TrackableCollection<DocumentationDraft>();
                    _documentationDrafts.CollectionChanged += FixupDocumentationDrafts;
                }
                return _documentationDrafts;
            }
            set
            {
                if (!ReferenceEquals(_documentationDrafts, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationDrafts != null)
                    {
                        _documentationDrafts.CollectionChanged -= FixupDocumentationDrafts;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (DocumentationDraft item in _documentationDrafts)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _documentationDrafts = value;
                    if (_documentationDrafts != null)
                    {
                        _documentationDrafts.CollectionChanged += FixupDocumentationDrafts;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (DocumentationDraft item in _documentationDrafts)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("DocumentationDrafts");
                }
            }
        }
        private TrackableCollection<DocumentationDraft> _documentationDrafts;

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
            Actions.Clear();
            Project = null;
            InheritedScenarios.Clear();
            Original = null;
            Nature = null;
            State = null;
            Creator = null;
            LastModifier = null;
            Solutions.Clear();
            Publications.Clear();
            DocumentationDrafts.Clear();
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
    
            if (previousValue != null && previousValue.Scenarios.Contains(this))
            {
                previousValue.Scenarios.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.Scenarios.Contains(this))
                {
                    Project.Scenarios.Add(this);
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
        /// Corrige l'état de la propriété de navigation Original.
        /// </summary>
        private void FixupOriginal(Scenario previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InheritedScenarios.Contains(this))
            {
                previousValue.InheritedScenarios.Remove(this);
            }
    
            if (Original != null)
            {
                if (!Original.InheritedScenarios.Contains(this))
                {
                    Original.InheritedScenarios.Add(this);
                }
    
                OriginalScenarioId = Original.ScenarioId;
            }
            else if (!skipKeys)
            {
                OriginalScenarioId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Original", previousValue, Original);
                if (Original != null && !Original.ChangeTracker.ChangeTrackingEnabled)
                {
                    Original.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Nature.
        /// </summary>
        private void FixupNature(ScenarioNature previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Scenarios.Contains(this))
            {
                previousValue.Scenarios.Remove(this);
            }
    
            if (Nature != null)
            {
                if (!Nature.Scenarios.Contains(this))
                {
                    Nature.Scenarios.Add(this);
                }
    
                NatureCode = Nature.ScenarioNatureCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Nature", previousValue, Nature);
                if (Nature != null && !Nature.ChangeTracker.ChangeTrackingEnabled)
                {
                    Nature.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation State.
        /// </summary>
        private void FixupState(ScenarioState previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Scenarios.Contains(this))
            {
                previousValue.Scenarios.Remove(this);
            }
    
            if (State != null)
            {
                if (!State.Scenarios.Contains(this))
                {
                    State.Scenarios.Add(this);
                }
    
                StateCode = State.ScenarioStateCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("State", previousValue, State);
                if (State != null && !State.ChangeTracker.ChangeTrackingEnabled)
                {
                    State.StartTracking();
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
    
            if (previousValue != null && previousValue.CreatedScenarios.Contains(this))
            {
                previousValue.CreatedScenarios.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedScenarios.Contains(this))
                {
                    Creator.CreatedScenarios.Add(this);
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
    
            if (previousValue != null && previousValue.LastModifiedScenarios.Contains(this))
            {
                previousValue.LastModifiedScenarios.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedScenarios.Contains(this))
                {
                    LastModifier.LastModifiedScenarios.Add(this);
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
                    item.Scenario = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Actions", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Scenario, this))
                    {
                        item.Scenario = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Actions", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété InheritedScenarios.
        /// </summary>
        private void FixupInheritedScenarios(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Scenario item in e.NewItems)
                {
                    item.Original = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("InheritedScenarios", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Scenario item in e.OldItems)
                {
                    if (ReferenceEquals(item.Original, this))
                    {
                        item.Original = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InheritedScenarios", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Solutions.
        /// </summary>
        private void FixupSolutions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Solution item in e.NewItems)
                {
                    item.Scenario = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Solutions", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Solution item in e.OldItems)
                {
                    if (ReferenceEquals(item.Scenario, this))
                    {
                        item.Scenario = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Solutions", item);
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
                    item.Scenario = this;
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
                    if (ReferenceEquals(item.Scenario, this))
                    {
                        item.Scenario = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Publications", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété DocumentationDrafts.
        /// </summary>
        private void FixupDocumentationDrafts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationDraft item in e.NewItems)
                {
                    item.Scenario = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationDrafts", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationDraft item in e.OldItems)
                {
                    if (ReferenceEquals(item.Scenario, this))
                    {
                        item.Scenario = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationDrafts", item);
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
    			case "ScenarioId":
    				this.ScenarioId = Convert.ToInt32(value);
    				break;
    			case "ProjectId":
    				this.ProjectId = Convert.ToInt32(value);
    				break;
    			case "StateCode":
    				this.StateCode = (string)value;
    				break;
    			case "NatureCode":
    				this.NatureCode = (string)value;
    				break;
    			case "OriginalScenarioId":
    				this.OriginalScenarioId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "IsShownInSummary":
    				this.IsShownInSummary = (bool)value;
    				break;
    			case "CriticalPathIDuration":
    				this.CriticalPathIDuration = (long)value;
    				break;
    			case "WebPublicationGuid":
    				this.WebPublicationGuid = (Nullable<System.Guid>)value;
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
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "Project":
    				this.Project = (Project)value;
    				break;
    			case "Original":
    				this.Original = (Scenario)value;
    				break;
    			case "Nature":
    				this.Nature = (ScenarioNature)value;
    				break;
    			case "State":
    				this.State = (ScenarioState)value;
    				break;
    			case "Creator":
    				this.Creator = (User)value;
    				break;
    			case "LastModifier":
    				this.LastModifier = (User)value;
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
    			case "InheritedScenarios":
    				this.InheritedScenarios.Add((Scenario)value);
    				break;
    			case "Solutions":
    				this.Solutions.Add((Solution)value);
    				break;
    			case "Publications":
    				this.Publications.Add((Publication)value);
    				break;
    			case "DocumentationDrafts":
    				this.DocumentationDrafts.Add((DocumentationDraft)value);
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
    			case "InheritedScenarios":
    				this.InheritedScenarios.Remove((Scenario)value);
    				break;
    			case "Solutions":
    				this.Solutions.Remove((Solution)value);
    				break;
    			case "Publications":
    				this.Publications.Remove((Publication)value);
    				break;
    			case "DocumentationDrafts":
    				this.DocumentationDrafts.Remove((DocumentationDraft)value);
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
    		values.Add("ScenarioId", this.ScenarioId);
    		values.Add("ProjectId", this.ProjectId);
    		values.Add("StateCode", this.StateCode);
    		values.Add("NatureCode", this.NatureCode);
    		values.Add("OriginalScenarioId", this.OriginalScenarioId);
    		values.Add("Label", this.Label);
    		values.Add("Description", this.Description);
    		values.Add("IsShownInSummary", this.IsShownInSummary);
    		values.Add("CriticalPathIDuration", this.CriticalPathIDuration);
    		values.Add("WebPublicationGuid", this.WebPublicationGuid);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Project", this.Project);
    		values.Add("Original", this.Original);
    		values.Add("Nature", this.Nature);
    		values.Add("State", this.State);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    
    		values.Add("Actions", GetHashCodes(this.Actions));
    		values.Add("InheritedScenarios", GetHashCodes(this.InheritedScenarios));
    		values.Add("Solutions", GetHashCodes(this.Solutions));
    		values.Add("Publications", GetHashCodes(this.Publications));
    		values.Add("DocumentationDrafts", GetHashCodes(this.DocumentationDrafts));
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
    		values.Add("Original", this.Original);
    		values.Add("Nature", this.Nature);
    		values.Add("State", this.State);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    
    		values.Add("Actions", this.Actions);
    		values.Add("InheritedScenarios", this.InheritedScenarios);
    		values.Add("Solutions", this.Solutions);
    		values.Add("Publications", this.Publications);
    		values.Add("DocumentationDrafts", this.DocumentationDrafts);
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
    	public const int LabelMaxLength = 50;
    
        /// <summary>
        /// Taille maximum du champ Description.
        /// </summary>
    	public const int DescriptionMaxLength = 4000;

        #endregion

    }
}
