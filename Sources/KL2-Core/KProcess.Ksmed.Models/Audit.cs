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
    [KnownType(typeof(Survey))]
    [KnownType(typeof(User))]
    [KnownType(typeof(AuditItem))]
    [KnownType(typeof(Inspection))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Audit : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Audit";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Audit"/>.
        /// </summary>
    	public Audit()
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
        
        private int _surveyId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int SurveyId
        {
            get { return _surveyId; }
            set
            {
                if (_surveyId != value)
                {
                    ChangeTracker.RecordValue("SurveyId", _surveyId, value);
                    if (!IsDeserializing)
                    {
                        if (Survey != null && Survey.Id != value)
                        {
                            Survey = null;
                        }
                    }
                    _surveyId = value;
                    OnEntityPropertyChanged("SurveyId");
                }
            }
        }
        
        private int _auditorId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int AuditorId
        {
            get { return _auditorId; }
            set
            {
                if (_auditorId != value)
                {
                    ChangeTracker.RecordValue("AuditorId", _auditorId, value);
                    if (!IsDeserializing)
                    {
                        if (Auditor != null && Auditor.UserId != value)
                        {
                            Auditor = null;
                        }
                    }
                    _auditorId = value;
                    OnEntityPropertyChanged("AuditorId");
                }
            }
        }
        
        private System.DateTime _startDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
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
        
        private Nullable<System.DateTime> _endDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> EndDate
        {
            get { return _endDate; }
            set
            {
                if (_endDate != value)
                {
                    ChangeTracker.RecordValue("EndDate", _endDate, value);
                    _endDate = value;
                    OnEntityPropertyChanged("EndDate");
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
        /// 
        /// </summary>
        [DataMember]
        public Survey Survey
        {
            get { return _survey; }
            set
            {
                if (!ReferenceEquals(_survey, value))
                {
                    var previousValue = _survey;
                    _survey = value;
                    FixupSurvey(previousValue);
                    OnNavigationPropertyChanged("Survey");
                }
            }
        }
        private Survey _survey;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Auditor
        {
            get { return _auditor; }
            set
            {
                if (!ReferenceEquals(_auditor, value))
                {
                    var previousValue = _auditor;
                    _auditor = value;
                    FixupAuditor(previousValue);
                    OnNavigationPropertyChanged("Auditor");
                }
            }
        }
        private User _auditor;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<AuditItem> AuditItems
        {
            get
            {
                if (_auditItems == null)
                {
                    _auditItems = new TrackableCollection<AuditItem>();
                    _auditItems.CollectionChanged += FixupAuditItems;
                }
                return _auditItems;
            }
            set
            {
                if (!ReferenceEquals(_auditItems, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_auditItems != null)
                    {
                        _auditItems.CollectionChanged -= FixupAuditItems;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (AuditItem item in _auditItems)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _auditItems = value;
                    if (_auditItems != null)
                    {
                        _auditItems.CollectionChanged += FixupAuditItems;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (AuditItem item in _auditItems)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("AuditItems");
                }
            }
        }
        private TrackableCollection<AuditItem> _auditItems;
    
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
            Survey = null;
            Auditor = null;
            AuditItems.Clear();
            Inspection = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Survey.
        /// </summary>
        private void FixupSurvey(Survey previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Audits.Contains(this))
            {
                previousValue.Audits.Remove(this);
            }
    
            if (Survey != null)
            {
                if (!Survey.Audits.Contains(this))
                {
                    Survey.Audits.Add(this);
                }
    
                SurveyId = Survey.Id;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Survey", previousValue, Survey);
                if (Survey != null && !Survey.ChangeTracker.ChangeTrackingEnabled)
                {
                    Survey.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Auditor.
        /// </summary>
        private void FixupAuditor(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Audits.Contains(this))
            {
                previousValue.Audits.Remove(this);
            }
    
            if (Auditor != null)
            {
                if (!Auditor.Audits.Contains(this))
                {
                    Auditor.Audits.Add(this);
                }
    
                AuditorId = Auditor.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Auditor", previousValue, Auditor);
                if (Auditor != null && !Auditor.ChangeTracker.ChangeTrackingEnabled)
                {
                    Auditor.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Inspection.
        /// </summary>
        private void FixupInspection(Inspection previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Audits.Contains(this))
            {
                previousValue.Audits.Remove(this);
            }
    
            if (Inspection != null)
            {
                if (!Inspection.Audits.Contains(this))
                {
                    Inspection.Audits.Add(this);
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
        /// Corrige l'état de la propriété AuditItems.
        /// </summary>
        private void FixupAuditItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (AuditItem item in e.NewItems)
                {
                    item.Audit = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("AuditItems", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AuditItem item in e.OldItems)
                {
                    if (ReferenceEquals(item.Audit, this))
                    {
                        item.Audit = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("AuditItems", item);
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
    			case "Id":
    				this.Id = Convert.ToInt32(value);
    				break;
    			case "SurveyId":
    				this.SurveyId = Convert.ToInt32(value);
    				break;
    			case "AuditorId":
    				this.AuditorId = Convert.ToInt32(value);
    				break;
    			case "StartDate":
    				this.StartDate = (System.DateTime)value;
    				break;
    			case "EndDate":
    				this.EndDate = (Nullable<System.DateTime>)value;
    				break;
    			case "InspectionId":
    				this.InspectionId = Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "Survey":
    				this.Survey = (Survey)value;
    				break;
    			case "Auditor":
    				this.Auditor = (User)value;
    				break;
    			case "Inspection":
    				this.Inspection = (Inspection)value;
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
    			case "AuditItems":
    				this.AuditItems.Add((AuditItem)value);
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
    			case "AuditItems":
    				this.AuditItems.Remove((AuditItem)value);
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
    		values.Add("SurveyId", this.SurveyId);
    		values.Add("AuditorId", this.AuditorId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("EndDate", this.EndDate);
    		values.Add("InspectionId", this.InspectionId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Survey", this.Survey);
    		values.Add("Auditor", this.Auditor);
    		values.Add("Inspection", this.Inspection);
    
    		values.Add("AuditItems", GetHashCodes(this.AuditItems));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Survey", this.Survey);
    		values.Add("Auditor", this.Auditor);
    		values.Add("Inspection", this.Inspection);
    
    		values.Add("AuditItems", this.AuditItems);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
