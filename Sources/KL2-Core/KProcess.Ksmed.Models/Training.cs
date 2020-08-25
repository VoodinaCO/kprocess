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
    [KnownType(typeof(Publication))]
    [KnownType(typeof(User))]
    [KnownType(typeof(ValidationTraining))]
    [KnownType(typeof(Training))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Training : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Training";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Training"/>.
        /// </summary>
    	public Training()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _trainingId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TrainingId
        {
            get { return _trainingId; }
            set
            {
                if (_trainingId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'TrainingId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _trainingId = value;
                    OnEntityPropertyChanged("TrainingId");
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
        
        private int _userId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    ChangeTracker.RecordValue("UserId", _userId, value);
                    if (!IsDeserializing)
                    {
                        if (User != null && User.UserId != value)
                        {
                            User = null;
                        }
                    }
                    _userId = value;
                    OnEntityPropertyChanged("UserId");
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
        
        private Nullable<int> _previousVersionTrainingId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> PreviousVersionTrainingId
        {
            get { return _previousVersionTrainingId; }
            set
            {
                if (_previousVersionTrainingId != value)
                {
                    ChangeTracker.RecordValue("PreviousVersionTrainingId", _previousVersionTrainingId, value);
                    if (!IsDeserializing)
                    {
                        if (PreviousVersionTraining != null && PreviousVersionTraining.TrainingId != value)
                        {
                            PreviousVersionTraining = null;
                        }
                    }
                    _previousVersionTrainingId = value;
                    OnEntityPropertyChanged("PreviousVersionTrainingId");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
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
        public User User
        {
            get { return _user; }
            set
            {
                if (!ReferenceEquals(_user, value))
                {
                    var previousValue = _user;
                    _user = value;
                    FixupUser(previousValue);
                    OnNavigationPropertyChanged("User");
                }
            }
        }
        private User _user;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ValidationTraining> ValidationTrainings
        {
            get
            {
                if (_validationTrainings == null)
                {
                    _validationTrainings = new TrackableCollection<ValidationTraining>();
                    _validationTrainings.CollectionChanged += FixupValidationTrainings;
                }
                return _validationTrainings;
            }
            set
            {
                if (!ReferenceEquals(_validationTrainings, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_validationTrainings != null)
                    {
                        _validationTrainings.CollectionChanged -= FixupValidationTrainings;
                    }
                    _validationTrainings = value;
                    if (_validationTrainings != null)
                    {
                        _validationTrainings.CollectionChanged += FixupValidationTrainings;
                    }
                    OnNavigationPropertyChanged("ValidationTrainings");
                }
            }
        }
        private TrackableCollection<ValidationTraining> _validationTrainings;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Training> NextVersionTrainings
        {
            get
            {
                if (_nextVersionTrainings == null)
                {
                    _nextVersionTrainings = new TrackableCollection<Training>();
                    _nextVersionTrainings.CollectionChanged += FixupNextVersionTrainings;
                }
                return _nextVersionTrainings;
            }
            set
            {
                if (!ReferenceEquals(_nextVersionTrainings, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nextVersionTrainings != null)
                    {
                        _nextVersionTrainings.CollectionChanged -= FixupNextVersionTrainings;
                    }
                    _nextVersionTrainings = value;
                    if (_nextVersionTrainings != null)
                    {
                        _nextVersionTrainings.CollectionChanged += FixupNextVersionTrainings;
                    }
                    OnNavigationPropertyChanged("NextVersionTrainings");
                }
            }
        }
        private TrackableCollection<Training> _nextVersionTrainings;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Training PreviousVersionTraining
        {
            get { return _previousVersionTraining; }
            set
            {
                if (!ReferenceEquals(_previousVersionTraining, value))
                {
                    var previousValue = _previousVersionTraining;
                    _previousVersionTraining = value;
                    FixupPreviousVersionTraining(previousValue);
                    OnNavigationPropertyChanged("PreviousVersionTraining");
                }
            }
        }
        private Training _previousVersionTraining;
    				

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
            Publication = null;
            User = null;
            ValidationTrainings.Clear();
            NextVersionTrainings.Clear();
            PreviousVersionTraining = null;
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
    
            if (previousValue != null && previousValue.Trainings.Contains(this))
            {
                previousValue.Trainings.Remove(this);
            }
    
            if (Publication != null)
            {
                if (!Publication.Trainings.Contains(this))
                {
                    Publication.Trainings.Add(this);
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
        /// Corrige l'état de la propriété de navigation User.
        /// </summary>
        private void FixupUser(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Trainings.Contains(this))
            {
                previousValue.Trainings.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Trainings.Contains(this))
                {
                    User.Trainings.Add(this);
                }
    
                UserId = User.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("User", previousValue, User);
                if (User != null && !User.ChangeTracker.ChangeTrackingEnabled)
                {
                    User.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation PreviousVersionTraining.
        /// </summary>
        private void FixupPreviousVersionTraining(Training previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.NextVersionTrainings.Contains(this))
            {
                previousValue.NextVersionTrainings.Remove(this);
            }
    
            if (PreviousVersionTraining != null)
            {
                if (!PreviousVersionTraining.NextVersionTrainings.Contains(this))
                {
                    PreviousVersionTraining.NextVersionTrainings.Add(this);
                }
    
                PreviousVersionTrainingId = PreviousVersionTraining.TrainingId;
            }
            else if (!skipKeys)
            {
                PreviousVersionTrainingId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PreviousVersionTraining", previousValue, PreviousVersionTraining);
                if (PreviousVersionTraining != null && !PreviousVersionTraining.ChangeTracker.ChangeTrackingEnabled)
                {
                    PreviousVersionTraining.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ValidationTrainings.
        /// </summary>
        private void FixupValidationTrainings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ValidationTraining item in e.NewItems)
                {
                    item.Training = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ValidationTrainings", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ValidationTraining item in e.OldItems)
                {
                    if (ReferenceEquals(item.Training, this))
                    {
                        item.Training = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ValidationTrainings", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété NextVersionTrainings.
        /// </summary>
        private void FixupNextVersionTrainings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Training item in e.NewItems)
                {
                    item.PreviousVersionTraining = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NextVersionTrainings", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Training item in e.OldItems)
                {
                    if (ReferenceEquals(item.PreviousVersionTraining, this))
                    {
                        item.PreviousVersionTraining = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NextVersionTrainings", item);
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
    			case "TrainingId":
    				this.TrainingId = Convert.ToInt32(value);
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
    			case "UserId":
    				this.UserId = Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "PreviousVersionTrainingId":
    				this.PreviousVersionTrainingId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Publication":
    				this.Publication = (Publication)value;
    				break;
    			case "User":
    				this.User = (User)value;
    				break;
    			case "PreviousVersionTraining":
    				this.PreviousVersionTraining = (Training)value;
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
    			case "ValidationTrainings":
    				this.ValidationTrainings.Add((ValidationTraining)value);
    				break;
    			case "NextVersionTrainings":
    				this.NextVersionTrainings.Add((Training)value);
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
    			case "ValidationTrainings":
    				this.ValidationTrainings.Remove((ValidationTraining)value);
    				break;
    			case "NextVersionTrainings":
    				this.NextVersionTrainings.Remove((Training)value);
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
    		values.Add("TrainingId", this.TrainingId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("EndDate", this.EndDate);
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("UserId", this.UserId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("PreviousVersionTrainingId", this.PreviousVersionTrainingId);
    		values.Add("Publication", this.Publication);
    		values.Add("User", this.User);
    		values.Add("PreviousVersionTraining", this.PreviousVersionTraining);
    
    		values.Add("ValidationTrainings", GetHashCodes(this.ValidationTrainings));
    		values.Add("NextVersionTrainings", GetHashCodes(this.NextVersionTrainings));
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
    		values.Add("User", this.User);
    		values.Add("PreviousVersionTraining", this.PreviousVersionTraining);
    
    		values.Add("ValidationTrainings", this.ValidationTrainings);
    		values.Add("NextVersionTrainings", this.NextVersionTrainings);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
