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
    [KnownType(typeof(PublishedAction))]
    [KnownType(typeof(Training))]
    [KnownType(typeof(User))]
    /// <summary>
    /// 
    /// </summary>
    public partial class ValidationTraining : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.ValidationTraining";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ValidationTraining"/>.
        /// </summary>
    	public ValidationTraining()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _validationTrainingId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ValidationTrainingId
        {
            get { return _validationTrainingId; }
            set
            {
                if (_validationTrainingId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ValidationTrainingId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _validationTrainingId = value;
                    OnEntityPropertyChanged("ValidationTrainingId");
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
                    ChangeTracker.RecordValue("TrainingId", _trainingId, value);
                    if (!IsDeserializing)
                    {
                        if (Training != null && Training.TrainingId != value)
                        {
                            Training = null;
                        }
                    }
                    _trainingId = value;
                    OnEntityPropertyChanged("TrainingId");
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
        
        private int _trainerId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TrainerId
        {
            get { return _trainerId; }
            set
            {
                if (_trainerId != value)
                {
                    ChangeTracker.RecordValue("TrainerId", _trainerId, value);
                    if (!IsDeserializing)
                    {
                        if (User != null && User.UserId != value)
                        {
                            User = null;
                        }
                    }
                    _trainerId = value;
                    OnEntityPropertyChanged("TrainerId");
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
        public Training Training
        {
            get { return _training; }
            set
            {
                if (!ReferenceEquals(_training, value))
                {
                    var previousValue = _training;
                    _training = value;
                    FixupTraining(previousValue);
                    OnNavigationPropertyChanged("Training");
                }
            }
        }
        private Training _training;
    				
    
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
            PublishedAction = null;
            Training = null;
            User = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation PublishedAction.
        /// </summary>
        private void FixupPublishedAction(PublishedAction previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ValidationTrainings.Contains(this))
            {
                previousValue.ValidationTrainings.Remove(this);
            }
    
            if (PublishedAction != null)
            {
                if (!PublishedAction.ValidationTrainings.Contains(this))
                {
                    PublishedAction.ValidationTrainings.Add(this);
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
        /// Corrige l'état de la propriété de navigation Training.
        /// </summary>
        private void FixupTraining(Training previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ValidationTrainings.Contains(this))
            {
                previousValue.ValidationTrainings.Remove(this);
            }
    
            if (Training != null)
            {
                if (!Training.ValidationTrainings.Contains(this))
                {
                    Training.ValidationTrainings.Add(this);
                }
    
                TrainingId = Training.TrainingId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Training", previousValue, Training);
                if (Training != null && !Training.ChangeTracker.ChangeTrackingEnabled)
                {
                    Training.StartTracking();
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
    
            if (previousValue != null && previousValue.ValidationTrainings.Contains(this))
            {
                previousValue.ValidationTrainings.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.ValidationTrainings.Contains(this))
                {
                    User.ValidationTrainings.Add(this);
                }
    
                TrainerId = User.UserId;
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
    			case "ValidationTrainingId":
    				this.ValidationTrainingId = Convert.ToInt32(value);
    				break;
    			case "StartDate":
    				this.StartDate = (System.DateTime)value;
    				break;
    			case "EndDate":
    				this.EndDate = (Nullable<System.DateTime>)value;
    				break;
    			case "TrainingId":
    				this.TrainingId = Convert.ToInt32(value);
    				break;
    			case "PublishedActionId":
    				this.PublishedActionId = Convert.ToInt32(value);
    				break;
    			case "TrainerId":
    				this.TrainerId = Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "PublishedAction":
    				this.PublishedAction = (PublishedAction)value;
    				break;
    			case "Training":
    				this.Training = (Training)value;
    				break;
    			case "User":
    				this.User = (User)value;
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
    		values.Add("ValidationTrainingId", this.ValidationTrainingId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("EndDate", this.EndDate);
    		values.Add("TrainingId", this.TrainingId);
    		values.Add("PublishedActionId", this.PublishedActionId);
    		values.Add("TrainerId", this.TrainerId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("Training", this.Training);
    		values.Add("User", this.User);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("Training", this.Training);
    		values.Add("User", this.User);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
