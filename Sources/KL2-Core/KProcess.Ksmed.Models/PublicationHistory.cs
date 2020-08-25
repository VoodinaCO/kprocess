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
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(Publication))]
    [KnownType(typeof(User))]
    /// <summary>
    /// 
    /// </summary>
    public partial class PublicationHistory : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.PublicationHistory";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PublicationHistory"/>.
        /// </summary>
    	public PublicationHistory()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _publicationHistoryId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublicationHistoryId
        {
            get { return _publicationHistoryId; }
            set
            {
                if (_publicationHistoryId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublicationHistoryId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _publicationHistoryId = value;
                    OnEntityPropertyChanged("PublicationHistoryId");
                }
            }
        }
        
        private int _processId;
        /// <summary>
        /// 
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
                        if (PublishedProcess != null && PublishedProcess.ProcessId != value)
                        {
                            PublishedProcess = null;
                        }
                    }
                    _processId = value;
                    OnEntityPropertyChanged("ProcessId");
                }
            }
        }
        
        private System.DateTime _timestamp;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    ChangeTracker.RecordValue("Timestamp", _timestamp, value);
                    _timestamp = value;
                    OnEntityPropertyChanged("Timestamp");
                }
            }
        }
        
        private Nullable<System.Guid> _trainingDocumentationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> TrainingDocumentationId
        {
            get { return _trainingDocumentationId; }
            set
            {
                if (_trainingDocumentationId != value)
                {
                    ChangeTracker.RecordValue("TrainingDocumentationId", _trainingDocumentationId, value);
                    if (!IsDeserializing)
                    {
                        if (TrainingPublication != null && TrainingPublication.PublicationId != value)
                        {
                            TrainingPublication = null;
                        }
                    }
                    _trainingDocumentationId = value;
                    OnEntityPropertyChanged("TrainingDocumentationId");
                }
            }
        }
        
        private Nullable<System.Guid> _evaluationDocumentationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> EvaluationDocumentationId
        {
            get { return _evaluationDocumentationId; }
            set
            {
                if (_evaluationDocumentationId != value)
                {
                    ChangeTracker.RecordValue("EvaluationDocumentationId", _evaluationDocumentationId, value);
                    if (!IsDeserializing)
                    {
                        if (EvaluationPublication != null && EvaluationPublication.PublicationId != value)
                        {
                            EvaluationPublication = null;
                        }
                    }
                    _evaluationDocumentationId = value;
                    OnEntityPropertyChanged("EvaluationDocumentationId");
                }
            }
        }
        
        private Nullable<System.Guid> _inspectionDocumentationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> InspectionDocumentationId
        {
            get { return _inspectionDocumentationId; }
            set
            {
                if (_inspectionDocumentationId != value)
                {
                    ChangeTracker.RecordValue("InspectionDocumentationId", _inspectionDocumentationId, value);
                    if (!IsDeserializing)
                    {
                        if (InspectionPublication != null && InspectionPublication.PublicationId != value)
                        {
                            InspectionPublication = null;
                        }
                    }
                    _inspectionDocumentationId = value;
                    OnEntityPropertyChanged("InspectionDocumentationId");
                }
            }
        }
        
        private string _errorMessage;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage != value)
                {
                    ChangeTracker.RecordValue("ErrorMessage", _errorMessage, value);
                    _errorMessage = value;
                    OnEntityPropertyChanged("ErrorMessage");
                }
            }
        }
        
        private int _publisherId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublisherId
        {
            get { return _publisherId; }
            set
            {
                if (_publisherId != value)
                {
                    ChangeTracker.RecordValue("PublisherId", _publisherId, value);
                    if (!IsDeserializing)
                    {
                        if (Publisher != null && Publisher.UserId != value)
                        {
                            Publisher = null;
                        }
                    }
                    _publisherId = value;
                    OnEntityPropertyChanged("PublisherId");
                }
            }
        }

        #endregion

        #region Propriétés Enum
        
        private PublicationStatus _state;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public PublicationStatus State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    ChangeTracker.RecordValue("State", _state, value);
                    _state = value;
                    OnEntityPropertyChanged("State");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Procedure PublishedProcess
        {
            get { return _publishedProcess; }
            set
            {
                if (!ReferenceEquals(_publishedProcess, value))
                {
                    var previousValue = _publishedProcess;
                    _publishedProcess = value;
                    FixupPublishedProcess(previousValue);
                    OnNavigationPropertyChanged("PublishedProcess");
                }
            }
        }
        private Procedure _publishedProcess;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Publication EvaluationPublication
        {
            get { return _evaluationPublication; }
            set
            {
                if (!ReferenceEquals(_evaluationPublication, value))
                {
                    var previousValue = _evaluationPublication;
                    _evaluationPublication = value;
                    FixupEvaluationPublication(previousValue);
                    OnNavigationPropertyChanged("EvaluationPublication");
                }
            }
        }
        private Publication _evaluationPublication;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Publication InspectionPublication
        {
            get { return _inspectionPublication; }
            set
            {
                if (!ReferenceEquals(_inspectionPublication, value))
                {
                    var previousValue = _inspectionPublication;
                    _inspectionPublication = value;
                    FixupInspectionPublication(previousValue);
                    OnNavigationPropertyChanged("InspectionPublication");
                }
            }
        }
        private Publication _inspectionPublication;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Publication TrainingPublication
        {
            get { return _trainingPublication; }
            set
            {
                if (!ReferenceEquals(_trainingPublication, value))
                {
                    var previousValue = _trainingPublication;
                    _trainingPublication = value;
                    FixupTrainingPublication(previousValue);
                    OnNavigationPropertyChanged("TrainingPublication");
                }
            }
        }
        private Publication _trainingPublication;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Publisher
        {
            get { return _publisher; }
            set
            {
                if (!ReferenceEquals(_publisher, value))
                {
                    var previousValue = _publisher;
                    _publisher = value;
                    FixupPublisher(previousValue);
                    OnNavigationPropertyChanged("Publisher");
                }
            }
        }
        private User _publisher;
    				

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
            PublishedProcess = null;
            EvaluationPublication = null;
            InspectionPublication = null;
            TrainingPublication = null;
            Publisher = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation PublishedProcess.
        /// </summary>
        private void FixupPublishedProcess(Procedure previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublicationHistories.Contains(this))
            {
                previousValue.PublicationHistories.Remove(this);
            }
    
            if (PublishedProcess != null)
            {
                if (!PublishedProcess.PublicationHistories.Contains(this))
                {
                    PublishedProcess.PublicationHistories.Add(this);
                }
    
                ProcessId = PublishedProcess.ProcessId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PublishedProcess", previousValue, PublishedProcess);
                if (PublishedProcess != null && !PublishedProcess.ChangeTracker.ChangeTrackingEnabled)
                {
                    PublishedProcess.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation EvaluationPublication.
        /// </summary>
        private void FixupEvaluationPublication(Publication previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.EvaluationPublicationHistories.Contains(this))
            {
                previousValue.EvaluationPublicationHistories.Remove(this);
            }
    
            if (EvaluationPublication != null)
            {
                if (!EvaluationPublication.EvaluationPublicationHistories.Contains(this))
                {
                    EvaluationPublication.EvaluationPublicationHistories.Add(this);
                }
    
                EvaluationDocumentationId = EvaluationPublication.PublicationId;
            }
            else if (!skipKeys)
            {
                EvaluationDocumentationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("EvaluationPublication", previousValue, EvaluationPublication);
                if (EvaluationPublication != null && !EvaluationPublication.ChangeTracker.ChangeTrackingEnabled)
                {
                    EvaluationPublication.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation InspectionPublication.
        /// </summary>
        private void FixupInspectionPublication(Publication previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InspectionPublicationHistories.Contains(this))
            {
                previousValue.InspectionPublicationHistories.Remove(this);
            }
    
            if (InspectionPublication != null)
            {
                if (!InspectionPublication.InspectionPublicationHistories.Contains(this))
                {
                    InspectionPublication.InspectionPublicationHistories.Add(this);
                }
    
                InspectionDocumentationId = InspectionPublication.PublicationId;
            }
            else if (!skipKeys)
            {
                InspectionDocumentationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("InspectionPublication", previousValue, InspectionPublication);
                if (InspectionPublication != null && !InspectionPublication.ChangeTracker.ChangeTrackingEnabled)
                {
                    InspectionPublication.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation TrainingPublication.
        /// </summary>
        private void FixupTrainingPublication(Publication previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.TrainingPublicationHistories.Contains(this))
            {
                previousValue.TrainingPublicationHistories.Remove(this);
            }
    
            if (TrainingPublication != null)
            {
                if (!TrainingPublication.TrainingPublicationHistories.Contains(this))
                {
                    TrainingPublication.TrainingPublicationHistories.Add(this);
                }
    
                TrainingDocumentationId = TrainingPublication.PublicationId;
            }
            else if (!skipKeys)
            {
                TrainingDocumentationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("TrainingPublication", previousValue, TrainingPublication);
                if (TrainingPublication != null && !TrainingPublication.ChangeTracker.ChangeTrackingEnabled)
                {
                    TrainingPublication.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Publisher.
        /// </summary>
        private void FixupPublisher(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublicationHistories.Contains(this))
            {
                previousValue.PublicationHistories.Remove(this);
            }
    
            if (Publisher != null)
            {
                if (!Publisher.PublicationHistories.Contains(this))
                {
                    Publisher.PublicationHistories.Add(this);
                }
    
                PublisherId = Publisher.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Publisher", previousValue, Publisher);
                if (Publisher != null && !Publisher.ChangeTracker.ChangeTrackingEnabled)
                {
                    Publisher.StartTracking();
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
    			case "PublicationHistoryId":
    				this.PublicationHistoryId = Convert.ToInt32(value);
    				break;
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "Timestamp":
    				this.Timestamp = (System.DateTime)value;
    				break;
    			case "TrainingDocumentationId":
    				this.TrainingDocumentationId = (Nullable<System.Guid>)value;
    				break;
    			case "EvaluationDocumentationId":
    				this.EvaluationDocumentationId = (Nullable<System.Guid>)value;
    				break;
    			case "InspectionDocumentationId":
    				this.InspectionDocumentationId = (Nullable<System.Guid>)value;
    				break;
    			case "ErrorMessage":
    				this.ErrorMessage = (string)value;
    				break;
    			case "PublisherId":
    				this.PublisherId = Convert.ToInt32(value);
    				break;
    			case "State":
    				this.State = (PublicationStatus)Convert.ToInt32(value);
    				break;
    			case "PublishedProcess":
    				this.PublishedProcess = (Procedure)value;
    				break;
    			case "EvaluationPublication":
    				this.EvaluationPublication = (Publication)value;
    				break;
    			case "InspectionPublication":
    				this.InspectionPublication = (Publication)value;
    				break;
    			case "TrainingPublication":
    				this.TrainingPublication = (Publication)value;
    				break;
    			case "Publisher":
    				this.Publisher = (User)value;
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
    		values.Add("PublicationHistoryId", this.PublicationHistoryId);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("Timestamp", this.Timestamp);
    		values.Add("TrainingDocumentationId", this.TrainingDocumentationId);
    		values.Add("EvaluationDocumentationId", this.EvaluationDocumentationId);
    		values.Add("InspectionDocumentationId", this.InspectionDocumentationId);
    		values.Add("ErrorMessage", this.ErrorMessage);
    		values.Add("PublisherId", this.PublisherId);
    		values.Add("PublishedProcess", this.PublishedProcess);
    		values.Add("EvaluationPublication", this.EvaluationPublication);
    		values.Add("InspectionPublication", this.InspectionPublication);
    		values.Add("TrainingPublication", this.TrainingPublication);
    		values.Add("Publisher", this.Publisher);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("PublishedProcess", this.PublishedProcess);
    		values.Add("EvaluationPublication", this.EvaluationPublication);
    		values.Add("InspectionPublication", this.InspectionPublication);
    		values.Add("TrainingPublication", this.TrainingPublication);
    		values.Add("Publisher", this.Publisher);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
