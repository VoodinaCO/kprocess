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
    [KnownType(typeof(Inspection))]
    [KnownType(typeof(User))]
    [KnownType(typeof(InspectionStep))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Anomaly : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Anomaly";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Anomaly"/>.
        /// </summary>
    	public Anomaly()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _id;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Id' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _id = value;
                    OnEntityPropertyChanged("Id");
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
        
        private byte[] _photo;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] Photo
        {
            get { return _photo; }
            set
            {
                if (_photo != value)
                {
                    ChangeTracker.RecordValue("Photo", _photo, value);
                    _photo = value;
                    OnEntityPropertyChanged("Photo");
                }
            }
        }
        
        private int _inspectionId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int InspectionId
        {
            get { return _inspectionId; }
            set
            {
                if (_inspectionId != value)
                {
                    ChangeTracker.RecordValue("InspectionId", _inspectionId, value);
                    if (!IsDeserializing)
                    {
                        if (Inspection != null && Inspection.InspectionId != value)
                        {
                            Inspection = null;
                        }
                    }
                    _inspectionId = value;
                    OnEntityPropertyChanged("InspectionId");
                }
            }
        }
        
        private int _inspectorId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int InspectorId
        {
            get { return _inspectorId; }
            set
            {
                if (_inspectorId != value)
                {
                    ChangeTracker.RecordValue("InspectorId", _inspectorId, value);
                    if (!IsDeserializing)
                    {
                        if (Inspector != null && Inspector.UserId != value)
                        {
                            Inspector = null;
                        }
                    }
                    _inspectorId = value;
                    OnEntityPropertyChanged("InspectorId");
                }
            }
        }
        
        private System.DateTime _date;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    ChangeTracker.RecordValue("Date", _date, value);
                    _date = value;
                    OnEntityPropertyChanged("Date");
                }
            }
        }
        
        private string _line;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Line
        {
            get { return _line; }
            set
            {
                if (_line != value)
                {
                    ChangeTracker.RecordValue("Line", _line, value);
                    _line = value;
                    OnEntityPropertyChanged("Line");
                }
            }
        }
        
        private string _machine;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Machine
        {
            get { return _machine; }
            set
            {
                if (_machine != value)
                {
                    ChangeTracker.RecordValue("Machine", _machine, value);
                    _machine = value;
                    OnEntityPropertyChanged("Machine");
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
        
        private string _category;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Category
        {
            get { return _category; }
            set
            {
                if (_category != value)
                {
                    ChangeTracker.RecordValue("Category", _category, value);
                    _category = value;
                    OnEntityPropertyChanged("Category");
                }
            }
        }

        #endregion

        #region Propriétés Enum
        
        private AnomalyType _type;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public AnomalyType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    ChangeTracker.RecordValue("Type", _type, value);
                    _type = value;
                    OnEntityPropertyChanged("Type");
                }
            }
        }
        
        private Nullable<AnomalyPriority> _priority;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public Nullable<AnomalyPriority> Priority
        {
            get { return _priority; }
            set
            {
                if (_priority != value)
                {
                    ChangeTracker.RecordValue("Priority", _priority, value);
                    _priority = value;
                    OnEntityPropertyChanged("Priority");
                }
            }
        }
        
        private AnomalyOrigin _origin;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public AnomalyOrigin Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                {
                    ChangeTracker.RecordValue("Origin", _origin, value);
                    _origin = value;
                    OnEntityPropertyChanged("Origin");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Inspection Inspection
        {
            get { return _inspection; }
            set
            {
                if (!ReferenceEquals(_inspection, value))
                {
                    var previousValue = _inspection;
                    _inspection = value;
                    FixupInspection(previousValue);
                    OnNavigationPropertyChanged("Inspection");
                }
            }
        }
        private Inspection _inspection;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Inspector
        {
            get { return _inspector; }
            set
            {
                if (!ReferenceEquals(_inspector, value))
                {
                    var previousValue = _inspector;
                    _inspector = value;
                    FixupInspector(previousValue);
                    OnNavigationPropertyChanged("Inspector");
                }
            }
        }
        private User _inspector;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<InspectionStep> InspectionSteps
        {
            get
            {
                if (_inspectionSteps == null)
                {
                    _inspectionSteps = new TrackableCollection<InspectionStep>();
                    _inspectionSteps.CollectionChanged += FixupInspectionSteps;
                }
                return _inspectionSteps;
            }
            set
            {
                if (!ReferenceEquals(_inspectionSteps, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inspectionSteps != null)
                    {
                        _inspectionSteps.CollectionChanged -= FixupInspectionSteps;
                    }
                    _inspectionSteps = value;
                    if (_inspectionSteps != null)
                    {
                        _inspectionSteps.CollectionChanged += FixupInspectionSteps;
                    }
                    OnNavigationPropertyChanged("InspectionSteps");
                }
            }
        }
        private TrackableCollection<InspectionStep> _inspectionSteps;

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
            Inspection = null;
            Inspector = null;
            InspectionSteps.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Inspection.
        /// </summary>
        private void FixupInspection(Inspection previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Anomalies.Contains(this))
            {
                previousValue.Anomalies.Remove(this);
            }
    
            if (Inspection != null)
            {
                if (!Inspection.Anomalies.Contains(this))
                {
                    Inspection.Anomalies.Add(this);
                }
    
                InspectionId = Inspection.InspectionId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Inspection", previousValue, Inspection);
                if (Inspection != null && !Inspection.ChangeTracker.ChangeTrackingEnabled)
                {
                    Inspection.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Inspector.
        /// </summary>
        private void FixupInspector(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Anomalies.Contains(this))
            {
                previousValue.Anomalies.Remove(this);
            }
    
            if (Inspector != null)
            {
                if (!Inspector.Anomalies.Contains(this))
                {
                    Inspector.Anomalies.Add(this);
                }
    
                InspectorId = Inspector.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Inspector", previousValue, Inspector);
                if (Inspector != null && !Inspector.ChangeTracker.ChangeTrackingEnabled)
                {
                    Inspector.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété InspectionSteps.
        /// </summary>
        private void FixupInspectionSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (InspectionStep item in e.NewItems)
                {
                    item.Anomaly = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("InspectionSteps", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (InspectionStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.Anomaly, this))
                    {
                        item.Anomaly = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionSteps", item);
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
    			case "Id":
    				this.Id = Convert.ToInt32(value);
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "Photo":
    				this.Photo = (byte[])value;
    				break;
    			case "InspectionId":
    				this.InspectionId = Convert.ToInt32(value);
    				break;
    			case "InspectorId":
    				this.InspectorId = Convert.ToInt32(value);
    				break;
    			case "Date":
    				this.Date = (System.DateTime)value;
    				break;
    			case "Line":
    				this.Line = (string)value;
    				break;
    			case "Machine":
    				this.Machine = (string)value;
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Category":
    				this.Category = (string)value;
    				break;
    			case "Type":
    				this.Type = (AnomalyType)Convert.ToInt32(value);
    				break;
    			case "Priority":
    				this.Priority = value == null ? (AnomalyPriority?)null : (AnomalyPriority)Convert.ToInt32(value);
    				break;
    			case "Origin":
    				this.Origin = (AnomalyOrigin)Convert.ToInt32(value);
    				break;
    			case "Inspection":
    				this.Inspection = (Inspection)value;
    				break;
    			case "Inspector":
    				this.Inspector = (User)value;
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
    			case "InspectionSteps":
    				this.InspectionSteps.Add((InspectionStep)value);
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
    			case "InspectionSteps":
    				this.InspectionSteps.Remove((InspectionStep)value);
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
    		values.Add("Id", this.Id);
    		values.Add("Description", this.Description);
    		values.Add("Photo", this.Photo);
    		values.Add("InspectionId", this.InspectionId);
    		values.Add("InspectorId", this.InspectorId);
    		values.Add("Date", this.Date);
    		values.Add("Line", this.Line);
    		values.Add("Machine", this.Machine);
    		values.Add("Label", this.Label);
    		values.Add("Category", this.Category);
    		values.Add("Inspection", this.Inspection);
    		values.Add("Inspector", this.Inspector);
    
    		values.Add("InspectionSteps", GetHashCodes(this.InspectionSteps));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Inspection", this.Inspection);
    		values.Add("Inspector", this.Inspector);
    
    		values.Add("InspectionSteps", this.InspectionSteps);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
