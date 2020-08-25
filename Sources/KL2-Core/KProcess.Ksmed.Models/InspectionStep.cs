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
    [KnownType(typeof(PublishedAction))]
    [KnownType(typeof(Anomaly))]
    /// <summary>
    /// 
    /// </summary>
    public partial class InspectionStep : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.InspectionStep";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="InspectionStep"/>.
        /// </summary>
    	public InspectionStep()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
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
        
        private int _publishedActionId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublishedActionId
        {
            get { return _publishedActionId; }
            set
            {
                if (_publishedActionId != value)
                {
                    ChangeTracker.RecordValue("PublishedActionId", _publishedActionId, value);
                    if (!IsDeserializing)
                    {
                        if (PublishedAction != null && PublishedAction.PublishedActionId != value)
                        {
                            PublishedAction = null;
                        }
                    }
                    _publishedActionId = value;
                    OnEntityPropertyChanged("PublishedActionId");
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
        
        private int _inspectionStepId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int InspectionStepId
        {
            get { return _inspectionStepId; }
            set
            {
                if (_inspectionStepId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'InspectionStepId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _inspectionStepId = value;
                    OnEntityPropertyChanged("InspectionStepId");
                }
            }
        }
        
        private Nullable<bool> _isOk;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> IsOk
        {
            get { return _isOk; }
            set
            {
                if (_isOk != value)
                {
                    ChangeTracker.RecordValue("IsOk", _isOk, value);
                    _isOk = value;
                    OnEntityPropertyChanged("IsOk");
                }
            }
        }
        
        private Nullable<int> _anomalyId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> AnomalyId
        {
            get { return _anomalyId; }
            set
            {
                if (_anomalyId != value)
                {
                    ChangeTracker.RecordValue("AnomalyId", _anomalyId, value);
                    if (!IsDeserializing)
                    {
                        if (Anomaly != null && Anomaly.Id != value)
                        {
                            Anomaly = null;
                        }
                    }
                    _anomalyId = value;
                    OnEntityPropertyChanged("AnomalyId");
                }
            }
        }
        
        private Nullable<int> _linkedInspectionId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> LinkedInspectionId
        {
            get { return _linkedInspectionId; }
            set
            {
                if (_linkedInspectionId != value)
                {
                    ChangeTracker.RecordValue("LinkedInspectionId", _linkedInspectionId, value);
                    if (!IsDeserializing)
                    {
                        if (LinkedInspection != null && LinkedInspection.InspectionId != value)
                        {
                            LinkedInspection = null;
                        }
                    }
                    _linkedInspectionId = value;
                    OnEntityPropertyChanged("LinkedInspectionId");
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
        public PublishedAction PublishedAction
        {
            get { return _publishedAction; }
            set
            {
                if (!ReferenceEquals(_publishedAction, value))
                {
                    var previousValue = _publishedAction;
                    _publishedAction = value;
                    FixupPublishedAction(previousValue);
                    OnNavigationPropertyChanged("PublishedAction");
                }
            }
        }
        private PublishedAction _publishedAction;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Anomaly Anomaly
        {
            get { return _anomaly; }
            set
            {
                if (!ReferenceEquals(_anomaly, value))
                {
                    var previousValue = _anomaly;
                    _anomaly = value;
                    FixupAnomaly(previousValue);
                    OnNavigationPropertyChanged("Anomaly");
                }
            }
        }
        private Anomaly _anomaly;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Inspection LinkedInspection
        {
            get { return _linkedInspection; }
            set
            {
                if (!ReferenceEquals(_linkedInspection, value))
                {
                    var previousValue = _linkedInspection;
                    _linkedInspection = value;
                    FixupLinkedInspection(previousValue);
                    OnNavigationPropertyChanged("LinkedInspection");
                }
            }
        }
        private Inspection _linkedInspection;
    				

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
            PublishedAction = null;
            Anomaly = null;
            LinkedInspection = null;
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
    
            if (previousValue != null && previousValue.InspectionSteps.Contains(this))
            {
                previousValue.InspectionSteps.Remove(this);
            }
    
            if (Inspection != null)
            {
                if (!Inspection.InspectionSteps.Contains(this))
                {
                    Inspection.InspectionSteps.Add(this);
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
    
            if (previousValue != null && previousValue.InspectionSteps.Contains(this))
            {
                previousValue.InspectionSteps.Remove(this);
            }
    
            if (Inspector != null)
            {
                if (!Inspector.InspectionSteps.Contains(this))
                {
                    Inspector.InspectionSteps.Add(this);
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
        /// Corrige l'état de la propriété de navigation PublishedAction.
        /// </summary>
        private void FixupPublishedAction(PublishedAction previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InspectionSteps.Contains(this))
            {
                previousValue.InspectionSteps.Remove(this);
            }
    
            if (PublishedAction != null)
            {
                if (!PublishedAction.InspectionSteps.Contains(this))
                {
                    PublishedAction.InspectionSteps.Add(this);
                }
    
                PublishedActionId = PublishedAction.PublishedActionId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PublishedAction", previousValue, PublishedAction);
                if (PublishedAction != null && !PublishedAction.ChangeTracker.ChangeTrackingEnabled)
                {
                    PublishedAction.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Anomaly.
        /// </summary>
        private void FixupAnomaly(Anomaly previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InspectionSteps.Contains(this))
            {
                previousValue.InspectionSteps.Remove(this);
            }
    
            if (Anomaly != null)
            {
                if (!Anomaly.InspectionSteps.Contains(this))
                {
                    Anomaly.InspectionSteps.Add(this);
                }
    
                AnomalyId = Anomaly.Id;
            }
            else if (!skipKeys)
            {
                AnomalyId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Anomaly", previousValue, Anomaly);
                if (Anomaly != null && !Anomaly.ChangeTracker.ChangeTrackingEnabled)
                {
                    Anomaly.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation LinkedInspection.
        /// </summary>
        private void FixupLinkedInspection(Inspection previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LinkedInspectionSteps.Contains(this))
            {
                previousValue.LinkedInspectionSteps.Remove(this);
            }
    
            if (LinkedInspection != null)
            {
                if (!LinkedInspection.LinkedInspectionSteps.Contains(this))
                {
                    LinkedInspection.LinkedInspectionSteps.Add(this);
                }
    
                LinkedInspectionId = LinkedInspection.InspectionId;
            }
            else if (!skipKeys)
            {
                LinkedInspectionId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LinkedInspection", previousValue, LinkedInspection);
                if (LinkedInspection != null && !LinkedInspection.ChangeTracker.ChangeTrackingEnabled)
                {
                    LinkedInspection.StartTracking();
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
    			case "Date":
    				this.Date = (System.DateTime)value;
    				break;
    			case "InspectionId":
    				this.InspectionId = Convert.ToInt32(value);
    				break;
    			case "PublishedActionId":
    				this.PublishedActionId = Convert.ToInt32(value);
    				break;
    			case "InspectorId":
    				this.InspectorId = Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "InspectionStepId":
    				this.InspectionStepId = Convert.ToInt32(value);
    				break;
    			case "IsOk":
    				this.IsOk = (Nullable<bool>)value;
    				break;
    			case "AnomalyId":
    				this.AnomalyId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "LinkedInspectionId":
    				this.LinkedInspectionId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Inspection":
    				this.Inspection = (Inspection)value;
    				break;
    			case "Inspector":
    				this.Inspector = (User)value;
    				break;
    			case "PublishedAction":
    				this.PublishedAction = (PublishedAction)value;
    				break;
    			case "Anomaly":
    				this.Anomaly = (Anomaly)value;
    				break;
    			case "LinkedInspection":
    				this.LinkedInspection = (Inspection)value;
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
    		values.Add("Date", this.Date);
    		values.Add("InspectionId", this.InspectionId);
    		values.Add("PublishedActionId", this.PublishedActionId);
    		values.Add("InspectorId", this.InspectorId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("InspectionStepId", this.InspectionStepId);
    		values.Add("IsOk", this.IsOk);
    		values.Add("AnomalyId", this.AnomalyId);
    		values.Add("LinkedInspectionId", this.LinkedInspectionId);
    		values.Add("Inspection", this.Inspection);
    		values.Add("Inspector", this.Inspector);
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("Anomaly", this.Anomaly);
    		values.Add("LinkedInspection", this.LinkedInspection);
    
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
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("Anomaly", this.Anomaly);
    		values.Add("LinkedInspection", this.LinkedInspection);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
