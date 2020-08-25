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
    [KnownType(typeof(QualificationStep))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Qualification))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Qualification : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Qualification";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Qualification"/>.
        /// </summary>
    	public Qualification()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _qualificationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int QualificationId
        {
            get { return _qualificationId; }
            set
            {
                if (_qualificationId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'QualificationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _qualificationId = value;
                    OnEntityPropertyChanged("QualificationId");
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
        
        private Nullable<int> _result;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> Result
        {
            get { return _result; }
            set
            {
                if (_result != value)
                {
                    ChangeTracker.RecordValue("Result", _result, value);
                    _result = value;
                    OnEntityPropertyChanged("Result");
                }
            }
        }
        
        private Nullable<bool> _isQualified;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> IsQualified
        {
            get { return _isQualified; }
            set
            {
                if (_isQualified != value)
                {
                    ChangeTracker.RecordValue("IsQualified", _isQualified, value);
                    _isQualified = value;
                    OnEntityPropertyChanged("IsQualified");
                }
            }
        }
        
        private string _comment;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (_comment != value)
                {
                    ChangeTracker.RecordValue("Comment", _comment, value);
                    _comment = value;
                    OnEntityPropertyChanged("Comment");
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
        
        private Nullable<int> _previousVersionQualificationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> PreviousVersionQualificationId
        {
            get { return _previousVersionQualificationId; }
            set
            {
                if (_previousVersionQualificationId != value)
                {
                    ChangeTracker.RecordValue("PreviousVersionQualificationId", _previousVersionQualificationId, value);
                    if (!IsDeserializing)
                    {
                        if (PreviousVersionQualification != null && PreviousVersionQualification.QualificationId != value)
                        {
                            PreviousVersionQualification = null;
                        }
                    }
                    _previousVersionQualificationId = value;
                    OnEntityPropertyChanged("PreviousVersionQualificationId");
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
        public TrackableCollection<QualificationStep> QualificationSteps
        {
            get
            {
                if (_qualificationSteps == null)
                {
                    _qualificationSteps = new TrackableCollection<QualificationStep>();
                    _qualificationSteps.CollectionChanged += FixupQualificationSteps;
                }
                return _qualificationSteps;
            }
            set
            {
                if (!ReferenceEquals(_qualificationSteps, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_qualificationSteps != null)
                    {
                        _qualificationSteps.CollectionChanged -= FixupQualificationSteps;
                    }
                    _qualificationSteps = value;
                    if (_qualificationSteps != null)
                    {
                        _qualificationSteps.CollectionChanged += FixupQualificationSteps;
                    }
                    OnNavigationPropertyChanged("QualificationSteps");
                }
            }
        }
        private TrackableCollection<QualificationStep> _qualificationSteps;
    
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
        public TrackableCollection<Qualification> NextVersionQualifications
        {
            get
            {
                if (_nextVersionQualifications == null)
                {
                    _nextVersionQualifications = new TrackableCollection<Qualification>();
                    _nextVersionQualifications.CollectionChanged += FixupNextVersionQualifications;
                }
                return _nextVersionQualifications;
            }
            set
            {
                if (!ReferenceEquals(_nextVersionQualifications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_nextVersionQualifications != null)
                    {
                        _nextVersionQualifications.CollectionChanged -= FixupNextVersionQualifications;
                    }
                    _nextVersionQualifications = value;
                    if (_nextVersionQualifications != null)
                    {
                        _nextVersionQualifications.CollectionChanged += FixupNextVersionQualifications;
                    }
                    OnNavigationPropertyChanged("NextVersionQualifications");
                }
            }
        }
        private TrackableCollection<Qualification> _nextVersionQualifications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Qualification PreviousVersionQualification
        {
            get { return _previousVersionQualification; }
            set
            {
                if (!ReferenceEquals(_previousVersionQualification, value))
                {
                    var previousValue = _previousVersionQualification;
                    _previousVersionQualification = value;
                    FixupPreviousVersionQualification(previousValue);
                    OnNavigationPropertyChanged("PreviousVersionQualification");
                }
            }
        }
        private Qualification _previousVersionQualification;
    				

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
            QualificationSteps.Clear();
            User = null;
            NextVersionQualifications.Clear();
            PreviousVersionQualification = null;
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
    
            if (previousValue != null && previousValue.Qualifications.Contains(this))
            {
                previousValue.Qualifications.Remove(this);
            }
    
            if (Publication != null)
            {
                if (!Publication.Qualifications.Contains(this))
                {
                    Publication.Qualifications.Add(this);
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
    
            if (previousValue != null && previousValue.Qualifications.Contains(this))
            {
                previousValue.Qualifications.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.Qualifications.Contains(this))
                {
                    User.Qualifications.Add(this);
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
        /// Corrige l'état de la propriété de navigation PreviousVersionQualification.
        /// </summary>
        private void FixupPreviousVersionQualification(Qualification previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.NextVersionQualifications.Contains(this))
            {
                previousValue.NextVersionQualifications.Remove(this);
            }
    
            if (PreviousVersionQualification != null)
            {
                if (!PreviousVersionQualification.NextVersionQualifications.Contains(this))
                {
                    PreviousVersionQualification.NextVersionQualifications.Add(this);
                }
    
                PreviousVersionQualificationId = PreviousVersionQualification.QualificationId;
            }
            else if (!skipKeys)
            {
                PreviousVersionQualificationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PreviousVersionQualification", previousValue, PreviousVersionQualification);
                if (PreviousVersionQualification != null && !PreviousVersionQualification.ChangeTracker.ChangeTrackingEnabled)
                {
                    PreviousVersionQualification.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété QualificationSteps.
        /// </summary>
        private void FixupQualificationSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (QualificationStep item in e.NewItems)
                {
                    item.Qualification = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("QualificationSteps", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (QualificationStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.Qualification, this))
                    {
                        item.Qualification = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("QualificationSteps", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété NextVersionQualifications.
        /// </summary>
        private void FixupNextVersionQualifications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Qualification item in e.NewItems)
                {
                    item.PreviousVersionQualification = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NextVersionQualifications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Qualification item in e.OldItems)
                {
                    if (ReferenceEquals(item.PreviousVersionQualification, this))
                    {
                        item.PreviousVersionQualification = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NextVersionQualifications", item);
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
    			case "QualificationId":
    				this.QualificationId = Convert.ToInt32(value);
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
    			case "Result":
    				this.Result = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "IsQualified":
    				this.IsQualified = (Nullable<bool>)value;
    				break;
    			case "Comment":
    				this.Comment = (string)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "PreviousVersionQualificationId":
    				this.PreviousVersionQualificationId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Publication":
    				this.Publication = (Publication)value;
    				break;
    			case "User":
    				this.User = (User)value;
    				break;
    			case "PreviousVersionQualification":
    				this.PreviousVersionQualification = (Qualification)value;
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
    			case "QualificationSteps":
    				this.QualificationSteps.Add((QualificationStep)value);
    				break;
    			case "NextVersionQualifications":
    				this.NextVersionQualifications.Add((Qualification)value);
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
    			case "QualificationSteps":
    				this.QualificationSteps.Remove((QualificationStep)value);
    				break;
    			case "NextVersionQualifications":
    				this.NextVersionQualifications.Remove((Qualification)value);
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
    		values.Add("QualificationId", this.QualificationId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("EndDate", this.EndDate);
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("UserId", this.UserId);
    		values.Add("Result", this.Result);
    		values.Add("IsQualified", this.IsQualified);
    		values.Add("Comment", this.Comment);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("PreviousVersionQualificationId", this.PreviousVersionQualificationId);
    		values.Add("Publication", this.Publication);
    		values.Add("User", this.User);
    		values.Add("PreviousVersionQualification", this.PreviousVersionQualification);
    
    		values.Add("QualificationSteps", GetHashCodes(this.QualificationSteps));
    		values.Add("NextVersionQualifications", GetHashCodes(this.NextVersionQualifications));
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
    		values.Add("PreviousVersionQualification", this.PreviousVersionQualification);
    
    		values.Add("QualificationSteps", this.QualificationSteps);
    		values.Add("NextVersionQualifications", this.NextVersionQualifications);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
