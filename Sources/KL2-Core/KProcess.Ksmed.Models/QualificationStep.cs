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
    [KnownType(typeof(Qualification))]
    [KnownType(typeof(User))]
    [KnownType(typeof(QualificationReason))]
    /// <summary>
    /// 
    /// </summary>
    public partial class QualificationStep : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.QualificationStep";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="QualificationStep"/>.
        /// </summary>
    	public QualificationStep()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _qualificationStepId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int QualificationStepId
        {
            get { return _qualificationStepId; }
            set
            {
                if (_qualificationStepId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'QualificationStepId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _qualificationStepId = value;
                    OnEntityPropertyChanged("QualificationStepId");
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
                    ChangeTracker.RecordValue("QualificationId", _qualificationId, value);
                    if (!IsDeserializing)
                    {
                        if (Qualification != null && Qualification.QualificationId != value)
                        {
                            Qualification = null;
                        }
                    }
                    _qualificationId = value;
                    OnEntityPropertyChanged("QualificationId");
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
        
        private int _qualifierId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int QualifierId
        {
            get { return _qualifierId; }
            set
            {
                if (_qualifierId != value)
                {
                    ChangeTracker.RecordValue("QualifierId", _qualifierId, value);
                    if (!IsDeserializing)
                    {
                        if (User != null && User.UserId != value)
                        {
                            User = null;
                        }
                    }
                    _qualifierId = value;
                    OnEntityPropertyChanged("QualifierId");
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
        
        private Nullable<int> _qualificationReasonId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> QualificationReasonId
        {
            get { return _qualificationReasonId; }
            set
            {
                if (_qualificationReasonId != value)
                {
                    ChangeTracker.RecordValue("QualificationReasonId", _qualificationReasonId, value);
                    if (!IsDeserializing)
                    {
                        if (QualificationReason != null && QualificationReason.Id != value)
                        {
                            QualificationReason = null;
                        }
                    }
                    _qualificationReasonId = value;
                    OnEntityPropertyChanged("QualificationReasonId");
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
        public Qualification Qualification
        {
            get { return _qualification; }
            set
            {
                if (!ReferenceEquals(_qualification, value))
                {
                    var previousValue = _qualification;
                    _qualification = value;
                    FixupQualification(previousValue);
                    OnNavigationPropertyChanged("Qualification");
                }
            }
        }
        private Qualification _qualification;
    				
    
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
        public QualificationReason QualificationReason
        {
            get { return _qualificationReason; }
            set
            {
                if (!ReferenceEquals(_qualificationReason, value))
                {
                    var previousValue = _qualificationReason;
                    _qualificationReason = value;
                    FixupQualificationReason(previousValue);
                    OnNavigationPropertyChanged("QualificationReason");
                }
            }
        }
        private QualificationReason _qualificationReason;
    				

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
            Qualification = null;
            User = null;
            QualificationReason = null;
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
    
            if (previousValue != null && previousValue.QualificationSteps.Contains(this))
            {
                previousValue.QualificationSteps.Remove(this);
            }
    
            if (PublishedAction != null)
            {
                if (!PublishedAction.QualificationSteps.Contains(this))
                {
                    PublishedAction.QualificationSteps.Add(this);
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
        /// Corrige l'état de la propriété de navigation Qualification.
        /// </summary>
        private void FixupQualification(Qualification previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.QualificationSteps.Contains(this))
            {
                previousValue.QualificationSteps.Remove(this);
            }
    
            if (Qualification != null)
            {
                if (!Qualification.QualificationSteps.Contains(this))
                {
                    Qualification.QualificationSteps.Add(this);
                }
    
                QualificationId = Qualification.QualificationId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Qualification", previousValue, Qualification);
                if (Qualification != null && !Qualification.ChangeTracker.ChangeTrackingEnabled)
                {
                    Qualification.StartTracking();
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
    
            if (previousValue != null && previousValue.QualificationSteps.Contains(this))
            {
                previousValue.QualificationSteps.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.QualificationSteps.Contains(this))
                {
                    User.QualificationSteps.Add(this);
                }
    
                QualifierId = User.UserId;
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
        /// Corrige l'état de la propriété de navigation QualificationReason.
        /// </summary>
        private void FixupQualificationReason(QualificationReason previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.QualificationSteps.Contains(this))
            {
                previousValue.QualificationSteps.Remove(this);
            }
    
            if (QualificationReason != null)
            {
                if (!QualificationReason.QualificationSteps.Contains(this))
                {
                    QualificationReason.QualificationSteps.Add(this);
                }
    
                QualificationReasonId = QualificationReason.Id;
            }
            else if (!skipKeys)
            {
                QualificationReasonId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("QualificationReason", previousValue, QualificationReason);
                if (QualificationReason != null && !QualificationReason.ChangeTracker.ChangeTrackingEnabled)
                {
                    QualificationReason.StartTracking();
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
    			case "QualificationStepId":
    				this.QualificationStepId = Convert.ToInt32(value);
    				break;
    			case "Date":
    				this.Date = (System.DateTime)value;
    				break;
    			case "Comment":
    				this.Comment = (string)value;
    				break;
    			case "QualificationId":
    				this.QualificationId = Convert.ToInt32(value);
    				break;
    			case "PublishedActionId":
    				this.PublishedActionId = Convert.ToInt32(value);
    				break;
    			case "QualifierId":
    				this.QualifierId = Convert.ToInt32(value);
    				break;
    			case "IsQualified":
    				this.IsQualified = (Nullable<bool>)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "QualificationReasonId":
    				this.QualificationReasonId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "PublishedAction":
    				this.PublishedAction = (PublishedAction)value;
    				break;
    			case "Qualification":
    				this.Qualification = (Qualification)value;
    				break;
    			case "User":
    				this.User = (User)value;
    				break;
    			case "QualificationReason":
    				this.QualificationReason = (QualificationReason)value;
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
    		values.Add("QualificationStepId", this.QualificationStepId);
    		values.Add("Date", this.Date);
    		values.Add("Comment", this.Comment);
    		values.Add("QualificationId", this.QualificationId);
    		values.Add("PublishedActionId", this.PublishedActionId);
    		values.Add("QualifierId", this.QualifierId);
    		values.Add("IsQualified", this.IsQualified);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("QualificationReasonId", this.QualificationReasonId);
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("Qualification", this.Qualification);
    		values.Add("User", this.User);
    		values.Add("QualificationReason", this.QualificationReason);
    
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
    		values.Add("Qualification", this.Qualification);
    		values.Add("User", this.User);
    		values.Add("QualificationReason", this.QualificationReason);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
