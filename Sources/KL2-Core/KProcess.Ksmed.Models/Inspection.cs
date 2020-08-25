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
    [KnownType(typeof(InspectionStep))]
    [KnownType(typeof(Publication))]
    [KnownType(typeof(Anomaly))]
    [KnownType(typeof(Audit))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Inspection : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Inspection";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Inspection"/>.
        /// </summary>
    	public Inspection()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'InspectionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _inspectionId = value;
                    OnEntityPropertyChanged("InspectionId");
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
                    ChangeTracker.RecordValue("PublicationId", _publicationId, value);
                    if (!IsDeserializing)
                    {
                        if (Publication != null && Publication.PublicationId != value)
                        {
                            Publication = null;
                        }
                    }
                    _publicationId = value;
                    OnEntityPropertyChanged("PublicationId");
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
        
        private Nullable<bool> _isScheduled;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> IsScheduled
        {
            get { return _isScheduled; }
            set
            {
                if (_isScheduled != value)
                {
                    ChangeTracker.RecordValue("IsScheduled", _isScheduled, value);
                    _isScheduled = value;
                    OnEntityPropertyChanged("IsScheduled");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
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
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Publication Publication
        {
            get { return _publication; }
            set
            {
                if (!ReferenceEquals(_publication, value))
                {
                    var previousValue = _publication;
                    _publication = value;
                    FixupPublication(previousValue);
                    OnNavigationPropertyChanged("Publication");
                }
            }
        }
        private Publication _publication;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Anomaly> Anomalies
        {
            get
            {
                if (_anomalies == null)
                {
                    _anomalies = new TrackableCollection<Anomaly>();
                    _anomalies.CollectionChanged += FixupAnomalies;
                }
                return _anomalies;
            }
            set
            {
                if (!ReferenceEquals(_anomalies, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_anomalies != null)
                    {
                        _anomalies.CollectionChanged -= FixupAnomalies;
                    }
                    _anomalies = value;
                    if (_anomalies != null)
                    {
                        _anomalies.CollectionChanged += FixupAnomalies;
                    }
                    OnNavigationPropertyChanged("Anomalies");
                }
            }
        }
        private TrackableCollection<Anomaly> _anomalies;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Audit> Audits
        {
            get
            {
                if (_audits == null)
                {
                    _audits = new TrackableCollection<Audit>();
                    _audits.CollectionChanged += FixupAudits;
                }
                return _audits;
            }
            set
            {
                if (!ReferenceEquals(_audits, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_audits != null)
                    {
                        _audits.CollectionChanged -= FixupAudits;
                    }
                    _audits = value;
                    if (_audits != null)
                    {
                        _audits.CollectionChanged += FixupAudits;
                    }
                    OnNavigationPropertyChanged("Audits");
                }
            }
        }
        private TrackableCollection<Audit> _audits;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<InspectionStep> LinkedInspectionSteps
        {
            get
            {
                if (_linkedInspectionSteps == null)
                {
                    _linkedInspectionSteps = new TrackableCollection<InspectionStep>();
                    _linkedInspectionSteps.CollectionChanged += FixupLinkedInspectionSteps;
                }
                return _linkedInspectionSteps;
            }
            set
            {
                if (!ReferenceEquals(_linkedInspectionSteps, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_linkedInspectionSteps != null)
                    {
                        _linkedInspectionSteps.CollectionChanged -= FixupLinkedInspectionSteps;
                    }
                    _linkedInspectionSteps = value;
                    if (_linkedInspectionSteps != null)
                    {
                        _linkedInspectionSteps.CollectionChanged += FixupLinkedInspectionSteps;
                    }
                    OnNavigationPropertyChanged("LinkedInspectionSteps");
                }
            }
        }
        private TrackableCollection<InspectionStep> _linkedInspectionSteps;

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
            InspectionSteps.Clear();
            Publication = null;
            Anomalies.Clear();
            Audits.Clear();
            LinkedInspectionSteps.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Publication.
        /// </summary>
        private void FixupPublication(Publication previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Inspections.Contains(this))
            {
                previousValue.Inspections.Remove(this);
            }
    
            if (Publication != null)
            {
                if (!Publication.Inspections.Contains(this))
                {
                    Publication.Inspections.Add(this);
                }
    
                PublicationId = Publication.PublicationId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Publication", previousValue, Publication);
                if (Publication != null && !Publication.ChangeTracker.ChangeTrackingEnabled)
                {
                    Publication.StartTracking();
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
                    item.Inspection = this;
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
                    if (ReferenceEquals(item.Inspection, this))
                    {
                        item.Inspection = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionSteps", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Anomalies.
        /// </summary>
        private void FixupAnomalies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Anomaly item in e.NewItems)
                {
                    item.Inspection = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Anomalies", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Anomaly item in e.OldItems)
                {
                    if (ReferenceEquals(item.Inspection, this))
                    {
                        item.Inspection = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Anomalies", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Audits.
        /// </summary>
        private void FixupAudits(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Audit item in e.NewItems)
                {
                    item.Inspection = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Audits", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Audit item in e.OldItems)
                {
                    if (ReferenceEquals(item.Inspection, this))
                    {
                        item.Inspection = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Audits", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LinkedInspectionSteps.
        /// </summary>
        private void FixupLinkedInspectionSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (InspectionStep item in e.NewItems)
                {
                    item.LinkedInspection = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LinkedInspectionSteps", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (InspectionStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.LinkedInspection, this))
                    {
                        item.LinkedInspection = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LinkedInspectionSteps", item);
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
    			case "InspectionId":
    				this.InspectionId = Convert.ToInt32(value);
    				break;
    			case "StartDate":
    				this.StartDate = (System.DateTime)value;
    				break;
    			case "EndDate":
    				this.EndDate = (Nullable<System.DateTime>)value;
    				break;
    			case "PublicationId":
    				this.PublicationId = (System.Guid)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "IsScheduled":
    				this.IsScheduled = (Nullable<bool>)value;
    				break;
    			case "Publication":
    				this.Publication = (Publication)value;
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
    			case "Anomalies":
    				this.Anomalies.Add((Anomaly)value);
    				break;
    			case "Audits":
    				this.Audits.Add((Audit)value);
    				break;
    			case "LinkedInspectionSteps":
    				this.LinkedInspectionSteps.Add((InspectionStep)value);
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
    			case "Anomalies":
    				this.Anomalies.Remove((Anomaly)value);
    				break;
    			case "Audits":
    				this.Audits.Remove((Audit)value);
    				break;
    			case "LinkedInspectionSteps":
    				this.LinkedInspectionSteps.Remove((InspectionStep)value);
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
    		values.Add("InspectionId", this.InspectionId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("EndDate", this.EndDate);
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("IsScheduled", this.IsScheduled);
    		values.Add("Publication", this.Publication);
    
    		values.Add("InspectionSteps", GetHashCodes(this.InspectionSteps));
    		values.Add("Anomalies", GetHashCodes(this.Anomalies));
    		values.Add("Audits", GetHashCodes(this.Audits));
    		values.Add("LinkedInspectionSteps", GetHashCodes(this.LinkedInspectionSteps));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Publication", this.Publication);
    
    		values.Add("InspectionSteps", this.InspectionSteps);
    		values.Add("Anomalies", this.Anomalies);
    		values.Add("Audits", this.Audits);
    		values.Add("LinkedInspectionSteps", this.LinkedInspectionSteps);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
