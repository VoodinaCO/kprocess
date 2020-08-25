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
    [KnownType(typeof(NotificationType))]
    [KnownType(typeof(NotificationAttachment))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Notification : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Notification";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Notification"/>.
        /// </summary>
    	public Notification()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _notificationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int NotificationId
        {
            get { return _notificationId; }
            set
            {
                if (_notificationId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'NotificationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _notificationId = value;
                    OnEntityPropertyChanged("NotificationId");
                }
            }
        }
        
        private Nullable<System.DateTime> _actualSendingDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> ActualSendingDate
        {
            get { return _actualSendingDate; }
            set
            {
                if (_actualSendingDate != value)
                {
                    ChangeTracker.RecordValue("ActualSendingDate", _actualSendingDate, value);
                    _actualSendingDate = value;
                    OnEntityPropertyChanged("ActualSendingDate");
                }
            }
        }
        
        private string _body;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    ChangeTracker.RecordValue("Body", _body, value);
                    _body = value;
                    OnEntityPropertyChanged("Body");
                }
            }
        }
        
        private System.DateTime _createdAt;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime CreatedAt
        {
            get { return _createdAt; }
            set
            {
                if (_createdAt != value)
                {
                    ChangeTracker.RecordValue("CreatedAt", _createdAt, value);
                    _createdAt = value;
                    OnEntityPropertyChanged("CreatedAt");
                }
            }
        }
        
        private bool _isProcessed;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsProcessed
        {
            get { return _isProcessed; }
            set
            {
                if (_isProcessed != value)
                {
                    ChangeTracker.RecordValue("IsProcessed", _isProcessed, value);
                    _isProcessed = value;
                    OnEntityPropertyChanged("IsProcessed");
                }
            }
        }
        
        private int _notificationTypeId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int NotificationTypeId
        {
            get { return _notificationTypeId; }
            set
            {
                if (_notificationTypeId != value)
                {
                    ChangeTracker.RecordValue("NotificationTypeId", _notificationTypeId, value);
                    if (!IsDeserializing)
                    {
                        if (NotificationType != null && NotificationType.Id != value)
                        {
                            NotificationType = null;
                        }
                    }
                    _notificationTypeId = value;
                    OnEntityPropertyChanged("NotificationTypeId");
                }
            }
        }
        
        private string _recipientBcc;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RecipientBcc
        {
            get { return _recipientBcc; }
            set
            {
                if (_recipientBcc != value)
                {
                    ChangeTracker.RecordValue("RecipientBcc", _recipientBcc, value);
                    _recipientBcc = value;
                    OnEntityPropertyChanged("RecipientBcc");
                }
            }
        }
        
        private string _recipientCc;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RecipientCc
        {
            get { return _recipientCc; }
            set
            {
                if (_recipientCc != value)
                {
                    ChangeTracker.RecordValue("RecipientCc", _recipientCc, value);
                    _recipientCc = value;
                    OnEntityPropertyChanged("RecipientCc");
                }
            }
        }
        
        private string _recipientTo;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RecipientTo
        {
            get { return _recipientTo; }
            set
            {
                if (_recipientTo != value)
                {
                    ChangeTracker.RecordValue("RecipientTo", _recipientTo, value);
                    _recipientTo = value;
                    OnEntityPropertyChanged("RecipientTo");
                }
            }
        }
        
        private System.DateTime _scheduledSendingDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime ScheduledSendingDate
        {
            get { return _scheduledSendingDate; }
            set
            {
                if (_scheduledSendingDate != value)
                {
                    ChangeTracker.RecordValue("ScheduledSendingDate", _scheduledSendingDate, value);
                    _scheduledSendingDate = value;
                    OnEntityPropertyChanged("ScheduledSendingDate");
                }
            }
        }
        
        private string _subject;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    ChangeTracker.RecordValue("Subject", _subject, value);
                    _subject = value;
                    OnEntityPropertyChanged("Subject");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public NotificationType NotificationType
        {
            get { return _notificationType; }
            set
            {
                if (!ReferenceEquals(_notificationType, value))
                {
                    var previousValue = _notificationType;
                    _notificationType = value;
                    FixupNotificationType(previousValue);
                    OnNavigationPropertyChanged("NotificationType");
                }
            }
        }
        private NotificationType _notificationType;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<NotificationAttachment> NotificationAttachments
        {
            get
            {
                if (_notificationAttachments == null)
                {
                    _notificationAttachments = new TrackableCollection<NotificationAttachment>();
                    _notificationAttachments.CollectionChanged += FixupNotificationAttachments;
                }
                return _notificationAttachments;
            }
            set
            {
                if (!ReferenceEquals(_notificationAttachments, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_notificationAttachments != null)
                    {
                        _notificationAttachments.CollectionChanged -= FixupNotificationAttachments;
                    }
                    _notificationAttachments = value;
                    if (_notificationAttachments != null)
                    {
                        _notificationAttachments.CollectionChanged += FixupNotificationAttachments;
                    }
                    OnNavigationPropertyChanged("NotificationAttachments");
                }
            }
        }
        private TrackableCollection<NotificationAttachment> _notificationAttachments;

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
            NotificationType = null;
            NotificationAttachments.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation NotificationType.
        /// </summary>
        private void FixupNotificationType(NotificationType previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Notifications.Contains(this))
            {
                previousValue.Notifications.Remove(this);
            }
    
            if (NotificationType != null)
            {
                if (!NotificationType.Notifications.Contains(this))
                {
                    NotificationType.Notifications.Add(this);
                }
    
                NotificationTypeId = NotificationType.Id;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("NotificationType", previousValue, NotificationType);
                if (NotificationType != null && !NotificationType.ChangeTracker.ChangeTrackingEnabled)
                {
                    NotificationType.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété NotificationAttachments.
        /// </summary>
        private void FixupNotificationAttachments(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (NotificationAttachment item in e.NewItems)
                {
                    item.Notification = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("NotificationAttachments", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (NotificationAttachment item in e.OldItems)
                {
                    if (ReferenceEquals(item.Notification, this))
                    {
                        item.Notification = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("NotificationAttachments", item);
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
    			case "NotificationId":
    				this.NotificationId = Convert.ToInt32(value);
    				break;
    			case "ActualSendingDate":
    				this.ActualSendingDate = (Nullable<System.DateTime>)value;
    				break;
    			case "Body":
    				this.Body = (string)value;
    				break;
    			case "CreatedAt":
    				this.CreatedAt = (System.DateTime)value;
    				break;
    			case "IsProcessed":
    				this.IsProcessed = (bool)value;
    				break;
    			case "NotificationTypeId":
    				this.NotificationTypeId = Convert.ToInt32(value);
    				break;
    			case "RecipientBcc":
    				this.RecipientBcc = (string)value;
    				break;
    			case "RecipientCc":
    				this.RecipientCc = (string)value;
    				break;
    			case "RecipientTo":
    				this.RecipientTo = (string)value;
    				break;
    			case "ScheduledSendingDate":
    				this.ScheduledSendingDate = (System.DateTime)value;
    				break;
    			case "Subject":
    				this.Subject = (string)value;
    				break;
    			case "NotificationType":
    				this.NotificationType = (NotificationType)value;
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
    			case "NotificationAttachments":
    				this.NotificationAttachments.Add((NotificationAttachment)value);
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
    			case "NotificationAttachments":
    				this.NotificationAttachments.Remove((NotificationAttachment)value);
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
    		values.Add("NotificationId", this.NotificationId);
    		values.Add("ActualSendingDate", this.ActualSendingDate);
    		values.Add("Body", this.Body);
    		values.Add("CreatedAt", this.CreatedAt);
    		values.Add("IsProcessed", this.IsProcessed);
    		values.Add("NotificationTypeId", this.NotificationTypeId);
    		values.Add("RecipientBcc", this.RecipientBcc);
    		values.Add("RecipientCc", this.RecipientCc);
    		values.Add("RecipientTo", this.RecipientTo);
    		values.Add("ScheduledSendingDate", this.ScheduledSendingDate);
    		values.Add("Subject", this.Subject);
    		values.Add("NotificationType", this.NotificationType);
    
    		values.Add("NotificationAttachments", GetHashCodes(this.NotificationAttachments));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("NotificationType", this.NotificationType);
    
    		values.Add("NotificationAttachments", this.NotificationAttachments);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
