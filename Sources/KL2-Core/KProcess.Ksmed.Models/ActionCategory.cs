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
    [KnownType(typeof(ActionType))]
    [KnownType(typeof(ActionValue))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(CloudFile))]
    [KnownType(typeof(DocumentationActionDraft))]
    /// <summary>
    /// Représente la catégorie d'une action (tâche).
    /// </summary>
    public partial class ActionCategory : ReferentialBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.ActionCategory";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionCategory"/>.
        /// </summary>
    	public ActionCategory()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _actionCategoryId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la catégorie d'action.
        /// </summary>
        [DataMember]
        public int ActionCategoryId
        {
            get { return _actionCategoryId; }
            set
            {
                if (_actionCategoryId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ActionCategoryId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _actionCategoryId = value;
                    OnEntityPropertyChanged("ActionCategoryId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Referential_Label_Required")]
    	[LocalizableStringLength(ActionReferentialProperties.LabelMaxLength, ErrorMessageResourceName = "Validation_Referential_Label_StringLength")]
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
        
        private string _color;
        /// <summary>
        /// Obtient ou définit la couleur de cette catégorie.
        /// </summary>
        [DataMember]
    	[LocalizableRegularExpression(@"#([0-9]|[A-F]|[a-f]){6,8}", ErrorMessageResourceName = "Validation_Referential_Color_Syntax")]
        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    ChangeTracker.RecordValue("Color", _color, value);
                    _color = value;
                    OnEntityPropertyChanged("Color");
                }
            }
        }
        
        private string _actionTypeCode;
        /// <summary>
        /// Obtient ou définit le code du type d'action.
        /// </summary>
        [DataMember]
        public string ActionTypeCode
        {
            get { return _actionTypeCode; }
            set
            {
                if (_actionTypeCode != value)
                {
                    ChangeTracker.RecordValue("ActionTypeCode", _actionTypeCode, value);
                    if (!IsDeserializing)
                    {
                        if (Type != null && Type.ActionTypeCode != value)
                        {
                            Type = null;
                        }
                    }
                    _actionTypeCode = value;
                    OnEntityPropertyChanged("ActionTypeCode");
                }
            }
        }
        
        private string _actionValueCode;
        /// <summary>
        /// Obtient ou définit le code de la valorisation de l'action.
        /// </summary>
        [DataMember]
        public string ActionValueCode
        {
            get { return _actionValueCode; }
            set
            {
                if (_actionValueCode != value)
                {
                    ChangeTracker.RecordValue("ActionValueCode", _actionValueCode, value);
                    if (!IsDeserializing)
                    {
                        if (Value != null && Value.ActionValueCode != value)
                        {
                            Value = null;
                        }
                    }
                    _actionValueCode = value;
                    OnEntityPropertyChanged("ActionValueCode");
                }
            }
        }
        
        private string _description;
        /// <summary>
        /// Obtient ou définit la description.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(ActionReferentialProperties.DescriptionMaxLength, ErrorMessageResourceName = "Validation_Referential_Description_StringLength")]
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
        
        private Nullable<int> _processId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ProcessId
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
                    if (!IsDeserializing)
                    {
                        if (CloudFile != null && CloudFile.Hash != value)
                        {
                            CloudFile = null;
                        }
                    }
                    _hash = value;
                    OnEntityPropertyChanged("Hash");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
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
        /// Obtient ou définit le type d'action associé.
        /// </summary>
        [DataMember]
        public ActionType Type
        {
            get { return _type; }
            set
            {
                if (!ReferenceEquals(_type, value))
                {
                    var previousValue = _type;
                    _type = value;
                    FixupType(previousValue);
                    OnNavigationPropertyChanged("Type");
                }
            }
        }
        private ActionType _type;
    				
    
        /// <summary>
        /// Obtient ou définit la valorisation d'action associée.
        /// </summary>
        [DataMember]
        public ActionValue Value
        {
            get { return _value; }
            set
            {
                if (!ReferenceEquals(_value, value))
                {
                    var previousValue = _value;
                    _value = value;
                    FixupValue(previousValue);
                    OnNavigationPropertyChanged("Value");
                }
            }
        }
        private ActionValue _value;
    				
    
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
    				OnProcessChanged(previousValue, value);
    				OnProcessChangedPartial(previousValue, value);
                }
            }
        }
        private Procedure _process;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Process"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnProcessChangedPartial(Procedure oldValue, Procedure newValue);
    	public event EventHandler<PropertyChangedEventArgs<Procedure>> ProcessChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Process"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnProcessChanged(Procedure oldValue, Procedure newValue)
    	{
    		if (ProcessChanged != null)
    			ProcessChanged(this, new PropertyChangedEventArgs<Procedure>(oldValue, newValue));
    	}
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public CloudFile CloudFile
        {
            get { return _cloudFile; }
            set
            {
                if (!ReferenceEquals(_cloudFile, value))
                {
                    var previousValue = _cloudFile;
                    _cloudFile = value;
                    FixupCloudFile(previousValue);
                    OnNavigationPropertyChanged("CloudFile");
                }
            }
        }
        private CloudFile _cloudFile;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<DocumentationActionDraft> DocumentationActionDrafts
        {
            get
            {
                if (_documentationActionDrafts == null)
                {
                    _documentationActionDrafts = new TrackableCollection<DocumentationActionDraft>();
                    _documentationActionDrafts.CollectionChanged += FixupDocumentationActionDrafts;
                }
                return _documentationActionDrafts;
            }
            set
            {
                if (!ReferenceEquals(_documentationActionDrafts, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationActionDrafts != null)
                    {
                        _documentationActionDrafts.CollectionChanged -= FixupDocumentationActionDrafts;
                    }
                    _documentationActionDrafts = value;
                    if (_documentationActionDrafts != null)
                    {
                        _documentationActionDrafts.CollectionChanged += FixupDocumentationActionDrafts;
                    }
                    OnNavigationPropertyChanged("DocumentationActionDrafts");
                }
            }
        }
        private TrackableCollection<DocumentationActionDraft> _documentationActionDrafts;

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
            Type = null;
            Value = null;
            Creator = null;
            LastModifier = null;
            Process = null;
            CloudFile = null;
            DocumentationActionDrafts.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Type.
        /// </summary>
        private void FixupType(ActionType previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionCategories.Contains(this))
            {
                previousValue.ActionCategories.Remove(this);
            }
    
            if (Type != null)
            {
                if (!Type.ActionCategories.Contains(this))
                {
                    Type.ActionCategories.Add(this);
                }
    
                ActionTypeCode = Type.ActionTypeCode;
            }
            else if (!skipKeys)
            {
                ActionTypeCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Type", previousValue, Type);
                if (Type != null && !Type.ChangeTracker.ChangeTrackingEnabled)
                {
                    Type.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Value.
        /// </summary>
        private void FixupValue(ActionValue previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionCategories.Contains(this))
            {
                previousValue.ActionCategories.Remove(this);
            }
    
            if (Value != null)
            {
                if (!Value.ActionCategories.Contains(this))
                {
                    Value.ActionCategories.Add(this);
                }
    
                ActionValueCode = Value.ActionValueCode;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Value", previousValue, Value);
                if (Value != null && !Value.ChangeTracker.ChangeTrackingEnabled)
                {
                    Value.StartTracking();
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
    
            if (previousValue != null && previousValue.CreatedActionCategories.Contains(this))
            {
                previousValue.CreatedActionCategories.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedActionCategories.Contains(this))
                {
                    Creator.CreatedActionCategories.Add(this);
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
    
            if (previousValue != null && previousValue.LastModifiedActionCategories.Contains(this))
            {
                previousValue.LastModifiedActionCategories.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedActionCategories.Contains(this))
                {
                    LastModifier.LastModifiedActionCategories.Add(this);
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
        private void FixupProcess(Procedure previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionCategories.Contains(this))
            {
                previousValue.ActionCategories.Remove(this);
            }
    
            if (Process != null)
            {
                if (!Process.ActionCategories.Contains(this))
                {
                    Process.ActionCategories.Add(this);
                }
    
                ProcessId = Process.ProcessId;
            }
            else if (!skipKeys)
            {
                ProcessId = null;
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
        /// Corrige l'état de la propriété de navigation CloudFile.
        /// </summary>
        private void FixupCloudFile(CloudFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RefActionCategories.Contains(this))
            {
                previousValue.RefActionCategories.Remove(this);
            }
    
            if (CloudFile != null)
            {
                if (!CloudFile.RefActionCategories.Contains(this))
                {
                    CloudFile.RefActionCategories.Add(this);
                }
    
                Hash = CloudFile.Hash;
            }
            else if (!skipKeys)
            {
                Hash = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("CloudFile", previousValue, CloudFile);
                if (CloudFile != null && !CloudFile.ChangeTracker.ChangeTrackingEnabled)
                {
                    CloudFile.StartTracking();
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
                    item.Category = this;
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
                    if (ReferenceEquals(item.Category, this))
                    {
                        item.Category = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Actions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété DocumentationActionDrafts.
        /// </summary>
        private void FixupDocumentationActionDrafts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationActionDraft item in e.NewItems)
                {
                    item.DocumentationCategory = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationActionDrafts", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationActionDraft item in e.OldItems)
                {
                    if (ReferenceEquals(item.DocumentationCategory, this))
                    {
                        item.DocumentationCategory = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationActionDrafts", item);
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
    			case "ActionCategoryId":
    				this.ActionCategoryId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Color":
    				this.Color = (string)value;
    				break;
    			case "ActionTypeCode":
    				this.ActionTypeCode = (string)value;
    				break;
    			case "ActionValueCode":
    				this.ActionValueCode = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
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
    				this.ProcessId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "Hash":
    				this.Hash = (string)value;
    				break;
    			case "Type":
    				this.Type = (ActionType)value;
    				break;
    			case "Value":
    				this.Value = (ActionValue)value;
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
    			case "CloudFile":
    				this.CloudFile = (CloudFile)value;
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
    			case "DocumentationActionDrafts":
    				this.DocumentationActionDrafts.Add((DocumentationActionDraft)value);
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
    			case "DocumentationActionDrafts":
    				this.DocumentationActionDrafts.Remove((DocumentationActionDraft)value);
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
    		values.Add("ActionCategoryId", this.ActionCategoryId);
    		values.Add("Label", this.Label);
    		values.Add("Color", this.Color);
    		values.Add("ActionTypeCode", this.ActionTypeCode);
    		values.Add("ActionValueCode", this.ActionValueCode);
    		values.Add("Description", this.Description);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Hash", this.Hash);
    		values.Add("Type", this.Type);
    		values.Add("Value", this.Value);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    		values.Add("CloudFile", this.CloudFile);
    
    		values.Add("Actions", GetHashCodes(this.Actions));
    		values.Add("DocumentationActionDrafts", GetHashCodes(this.DocumentationActionDrafts));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Type", this.Type);
    		values.Add("Value", this.Value);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Process", this.Process);
    		values.Add("CloudFile", this.CloudFile);
    
    		values.Add("Actions", this.Actions);
    		values.Add("DocumentationActionDrafts", this.DocumentationActionDrafts);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
