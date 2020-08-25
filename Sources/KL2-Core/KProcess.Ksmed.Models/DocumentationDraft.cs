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
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(DocumentationActionDraftWBS))]
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentationDraft : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.DocumentationDraft";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DocumentationDraft"/>.
        /// </summary>
    	public DocumentationDraft()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _documentationDraftId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int DocumentationDraftId
        {
            get { return _documentationDraftId; }
            set
            {
                if (_documentationDraftId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'DocumentationDraftId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _documentationDraftId = value;
                    OnEntityPropertyChanged("DocumentationDraftId");
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
        
        private string _formation_DispositionAsJson;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Formation_DispositionAsJson
        {
            get { return _formation_DispositionAsJson; }
            set
            {
                if (_formation_DispositionAsJson != value)
                {
                    ChangeTracker.RecordValue("Formation_DispositionAsJson", _formation_DispositionAsJson, value);
                    _formation_DispositionAsJson = value;
                    OnEntityPropertyChanged("Formation_DispositionAsJson");
                }
            }
        }
        
        private string _inspection_DispositionAsJson;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Inspection_DispositionAsJson
        {
            get { return _inspection_DispositionAsJson; }
            set
            {
                if (_inspection_DispositionAsJson != value)
                {
                    ChangeTracker.RecordValue("Inspection_DispositionAsJson", _inspection_DispositionAsJson, value);
                    _inspection_DispositionAsJson = value;
                    OnEntityPropertyChanged("Inspection_DispositionAsJson");
                }
            }
        }
        
        private string _evaluation_DispositionAsJson;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Evaluation_DispositionAsJson
        {
            get { return _evaluation_DispositionAsJson; }
            set
            {
                if (_evaluation_DispositionAsJson != value)
                {
                    ChangeTracker.RecordValue("Evaluation_DispositionAsJson", _evaluation_DispositionAsJson, value);
                    _evaluation_DispositionAsJson = value;
                    OnEntityPropertyChanged("Evaluation_DispositionAsJson");
                }
            }
        }
        
        private Nullable<bool> _activeVideoExport;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> ActiveVideoExport
        {
            get { return _activeVideoExport; }
            set
            {
                if (_activeVideoExport != value)
                {
                    ChangeTracker.RecordValue("ActiveVideoExport", _activeVideoExport, value);
                    _activeVideoExport = value;
                    OnEntityPropertyChanged("ActiveVideoExport");
                }
            }
        }
        
        private Nullable<bool> _slowMotion;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> SlowMotion
        {
            get { return _slowMotion; }
            set
            {
                if (_slowMotion != value)
                {
                    ChangeTracker.RecordValue("SlowMotion", _slowMotion, value);
                    _slowMotion = value;
                    OnEntityPropertyChanged("SlowMotion");
                }
            }
        }
        
        private Nullable<decimal> _slowMotionDuration;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<decimal> SlowMotionDuration
        {
            get { return _slowMotionDuration; }
            set
            {
                if (_slowMotionDuration != value)
                {
                    ChangeTracker.RecordValue("SlowMotionDuration", _slowMotionDuration, value);
                    _slowMotionDuration = value;
                    OnEntityPropertyChanged("SlowMotionDuration");
                }
            }
        }
        
        private Nullable<bool> _waterMarking;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> WaterMarking
        {
            get { return _waterMarking; }
            set
            {
                if (_waterMarking != value)
                {
                    ChangeTracker.RecordValue("WaterMarking", _waterMarking, value);
                    _waterMarking = value;
                    OnEntityPropertyChanged("WaterMarking");
                }
            }
        }
        
        private string _waterMarkingText;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string WaterMarkingText
        {
            get { return _waterMarkingText; }
            set
            {
                if (_waterMarkingText != value)
                {
                    ChangeTracker.RecordValue("WaterMarkingText", _waterMarkingText, value);
                    _waterMarkingText = value;
                    OnEntityPropertyChanged("WaterMarkingText");
                }
            }
        }
        
        private bool _formation_IsMajor;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Formation_IsMajor
        {
            get { return _formation_IsMajor; }
            set
            {
                if (_formation_IsMajor != value)
                {
                    ChangeTracker.RecordValue("Formation_IsMajor", _formation_IsMajor, value);
                    _formation_IsMajor = value;
                    OnEntityPropertyChanged("Formation_IsMajor");
                }
            }
        }
        
        private string _formation_ReleaseNote;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Formation_ReleaseNote
        {
            get { return _formation_ReleaseNote; }
            set
            {
                if (_formation_ReleaseNote != value)
                {
                    ChangeTracker.RecordValue("Formation_ReleaseNote", _formation_ReleaseNote, value);
                    _formation_ReleaseNote = value;
                    OnEntityPropertyChanged("Formation_ReleaseNote");
                }
            }
        }
        
        private bool _inspection_IsMajor;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Inspection_IsMajor
        {
            get { return _inspection_IsMajor; }
            set
            {
                if (_inspection_IsMajor != value)
                {
                    ChangeTracker.RecordValue("Inspection_IsMajor", _inspection_IsMajor, value);
                    _inspection_IsMajor = value;
                    OnEntityPropertyChanged("Inspection_IsMajor");
                }
            }
        }
        
        private string _inspection_ReleaseNote;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Inspection_ReleaseNote
        {
            get { return _inspection_ReleaseNote; }
            set
            {
                if (_inspection_ReleaseNote != value)
                {
                    ChangeTracker.RecordValue("Inspection_ReleaseNote", _inspection_ReleaseNote, value);
                    _inspection_ReleaseNote = value;
                    OnEntityPropertyChanged("Inspection_ReleaseNote");
                }
            }
        }
        
        private bool _evaluation_IsMajor;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool Evaluation_IsMajor
        {
            get { return _evaluation_IsMajor; }
            set
            {
                if (_evaluation_IsMajor != value)
                {
                    ChangeTracker.RecordValue("Evaluation_IsMajor", _evaluation_IsMajor, value);
                    _evaluation_IsMajor = value;
                    OnEntityPropertyChanged("Evaluation_IsMajor");
                }
            }
        }
        
        private string _evaluation_ReleaseNote;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Evaluation_ReleaseNote
        {
            get { return _evaluation_ReleaseNote; }
            set
            {
                if (_evaluation_ReleaseNote != value)
                {
                    ChangeTracker.RecordValue("Evaluation_ReleaseNote", _evaluation_ReleaseNote, value);
                    _evaluation_ReleaseNote = value;
                    OnEntityPropertyChanged("Evaluation_ReleaseNote");
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
        
        private Nullable<EVerticalAlign> _waterMarkingVAlign;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public Nullable<EVerticalAlign> WaterMarkingVAlign
        {
            get { return _waterMarkingVAlign; }
            set
            {
                if (_waterMarkingVAlign != value)
                {
                    ChangeTracker.RecordValue("WaterMarkingVAlign", _waterMarkingVAlign, value);
                    _waterMarkingVAlign = value;
                    OnEntityPropertyChanged("WaterMarkingVAlign");
                }
            }
        }
        
        private Nullable<EHorizontalAlign> _waterMarkingHAlign;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public Nullable<EHorizontalAlign> WaterMarkingHAlign
        {
            get { return _waterMarkingHAlign; }
            set
            {
                if (_waterMarkingHAlign != value)
                {
                    ChangeTracker.RecordValue("WaterMarkingHAlign", _waterMarkingHAlign, value);
                    _waterMarkingHAlign = value;
                    OnEntityPropertyChanged("WaterMarkingHAlign");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
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
        public TrackableCollection<DocumentationActionDraftWBS> DocumentationActionDraftWBS
        {
            get
            {
                if (_documentationActionDraftWBS == null)
                {
                    _documentationActionDraftWBS = new TrackableCollection<DocumentationActionDraftWBS>();
                    _documentationActionDraftWBS.CollectionChanged += FixupDocumentationActionDraftWBS;
                }
                return _documentationActionDraftWBS;
            }
            set
            {
                if (!ReferenceEquals(_documentationActionDraftWBS, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationActionDraftWBS != null)
                    {
                        _documentationActionDraftWBS.CollectionChanged -= FixupDocumentationActionDraftWBS;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (DocumentationActionDraftWBS item in _documentationActionDraftWBS)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _documentationActionDraftWBS = value;
                    if (_documentationActionDraftWBS != null)
                    {
                        _documentationActionDraftWBS.CollectionChanged += FixupDocumentationActionDraftWBS;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (DocumentationActionDraftWBS item in _documentationActionDraftWBS)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("DocumentationActionDraftWBS");
                }
            }
        }
        private TrackableCollection<DocumentationActionDraftWBS> _documentationActionDraftWBS;

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
            Scenario = null;
            DocumentationActionDraftWBS.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Scenario.
        /// </summary>
        private void FixupScenario(Scenario previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationDrafts.Contains(this))
            {
                previousValue.DocumentationDrafts.Remove(this);
            }
    
            if (Scenario != null)
            {
                if (!Scenario.DocumentationDrafts.Contains(this))
                {
                    Scenario.DocumentationDrafts.Add(this);
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
        /// Corrige l'état de la propriété DocumentationActionDraftWBS.
        /// </summary>
        private void FixupDocumentationActionDraftWBS(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationActionDraftWBS item in e.NewItems)
                {
                    item.DocumentationDraft = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationActionDraftWBS", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationActionDraftWBS item in e.OldItems)
                {
                    if (ReferenceEquals(item.DocumentationDraft, this))
                    {
                        item.DocumentationDraft = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationActionDraftWBS", item);
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
    			case "DocumentationDraftId":
    				this.DocumentationDraftId = Convert.ToInt32(value);
    				break;
    			case "ScenarioId":
    				this.ScenarioId = Convert.ToInt32(value);
    				break;
    			case "Formation_DispositionAsJson":
    				this.Formation_DispositionAsJson = (string)value;
    				break;
    			case "Inspection_DispositionAsJson":
    				this.Inspection_DispositionAsJson = (string)value;
    				break;
    			case "Evaluation_DispositionAsJson":
    				this.Evaluation_DispositionAsJson = (string)value;
    				break;
    			case "ActiveVideoExport":
    				this.ActiveVideoExport = (Nullable<bool>)value;
    				break;
    			case "SlowMotion":
    				this.SlowMotion = (Nullable<bool>)value;
    				break;
    			case "SlowMotionDuration":
    				this.SlowMotionDuration = (Nullable<decimal>)value;
    				break;
    			case "WaterMarking":
    				this.WaterMarking = (Nullable<bool>)value;
    				break;
    			case "WaterMarkingText":
    				this.WaterMarkingText = (string)value;
    				break;
    			case "Formation_IsMajor":
    				this.Formation_IsMajor = (bool)value;
    				break;
    			case "Formation_ReleaseNote":
    				this.Formation_ReleaseNote = (string)value;
    				break;
    			case "Inspection_IsMajor":
    				this.Inspection_IsMajor = (bool)value;
    				break;
    			case "Inspection_ReleaseNote":
    				this.Inspection_ReleaseNote = (string)value;
    				break;
    			case "Evaluation_IsMajor":
    				this.Evaluation_IsMajor = (bool)value;
    				break;
    			case "Evaluation_ReleaseNote":
    				this.Evaluation_ReleaseNote = (string)value;
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
    			case "WaterMarkingVAlign":
    				this.WaterMarkingVAlign = value == null ? (EVerticalAlign?)null : (EVerticalAlign)Convert.ToInt32(value);
    				break;
    			case "WaterMarkingHAlign":
    				this.WaterMarkingHAlign = value == null ? (EHorizontalAlign?)null : (EHorizontalAlign)Convert.ToInt32(value);
    				break;
    			case "Scenario":
    				this.Scenario = (Scenario)value;
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
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Add((DocumentationActionDraftWBS)value);
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
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Remove((DocumentationActionDraftWBS)value);
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
    		values.Add("DocumentationDraftId", this.DocumentationDraftId);
    		values.Add("ScenarioId", this.ScenarioId);
    		values.Add("Formation_DispositionAsJson", this.Formation_DispositionAsJson);
    		values.Add("Inspection_DispositionAsJson", this.Inspection_DispositionAsJson);
    		values.Add("Evaluation_DispositionAsJson", this.Evaluation_DispositionAsJson);
    		values.Add("ActiveVideoExport", this.ActiveVideoExport);
    		values.Add("SlowMotion", this.SlowMotion);
    		values.Add("SlowMotionDuration", this.SlowMotionDuration);
    		values.Add("WaterMarking", this.WaterMarking);
    		values.Add("WaterMarkingText", this.WaterMarkingText);
    		values.Add("Formation_IsMajor", this.Formation_IsMajor);
    		values.Add("Formation_ReleaseNote", this.Formation_ReleaseNote);
    		values.Add("Inspection_IsMajor", this.Inspection_IsMajor);
    		values.Add("Inspection_ReleaseNote", this.Inspection_ReleaseNote);
    		values.Add("Evaluation_IsMajor", this.Evaluation_IsMajor);
    		values.Add("Evaluation_ReleaseNote", this.Evaluation_ReleaseNote);
    		values.Add("Formation_ActionDisposition", this.Formation_ActionDisposition);
    		values.Add("Inspection_ActionDisposition", this.Inspection_ActionDisposition);
    		values.Add("Evaluation_ActionDisposition", this.Evaluation_ActionDisposition);
    		values.Add("Scenario", this.Scenario);
    
    		values.Add("DocumentationActionDraftWBS", GetHashCodes(this.DocumentationActionDraftWBS));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Scenario", this.Scenario);
    
    		values.Add("DocumentationActionDraftWBS", this.DocumentationActionDraftWBS);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
