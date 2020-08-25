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
    [KnownType(typeof(Notification))]
    [KnownType(typeof(NotificationTypeSetting))]
    /// <summary>
    /// 
    /// </summary>
    public partial class NotificationType : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.NotificationType";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="NotificationType"/>.
        /// </summary>
    	public NotificationType()
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
        
        private int _notificationTypeSettingId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int NotificationTypeSettingId
        {
            get { return _notificationTypeSettingId; }
            set
            {
                if (_notificationTypeSettingId != value)
                {
                    ChangeTracker.RecordValue("NotificationTypeSettingId", _notificationTypeSettingId, value);
                    if (!IsDeserializing)
                    {
                        if (NotificationTypeSetting != null && NotificationTypeSetting.Id != value)
                        {
                            NotificationTypeSetting = null;
                        }
                    }
                    _notificationTypeSettingId = value;
                    OnEntityPropertyChanged("NotificationTypeSettingId");
                }
            }
        }

        #endregion

        #region Propriétés Enum
        
        private NotificationCategory _notificationCategory;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public NotificationCategory NotificationCategory
        {
            get { return _notificationCategory; }
            set
            {
                if (_notificationCategory != value)
                {
                    ChangeTracker.RecordValue("NotificationCategory", _notificationCategory, value);
                    _notificationCategory = value;
                    OnEntityPropertyChanged("NotificationCategory");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Notification> Notifications
        {
            get
            {
                if (_notifications == null)
                {
                    _notifications = new TrackableCollection<Notification>();
                    _notifications.CollectionChanged += FixupNotifications;
                }
                return _notifications;
            }
            set
            {
                if (!ReferenceEquals(_notifications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_notifications != null)
                    {
                        _notifications.CollectionChanged -= FixupNotifications;
                    }
                    _notifications = value;
                    if (_notifications != null)
                    {
                        _notifications.CollectionChanged += FixupNotifications;
                    }
                    OnNavigationPropertyChanged("Notifications");
                }
            }
        }
        private TrackableCollection<Notification> _notifications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public NotificationTypeSetting NotificationTypeSetting
        {
            get { return _notificationTypeSetting; }
            set
            {
                if (!ReferenceEquals(_notificationTypeSetting, value))
                {
                    var previousValue = _notificationTypeSetting;
                    _notificationTypeSetting = value;
                    FixupNotificationTypeSetting(previousValue);
                    OnNavigationPropertyChanged("NotificationTypeSetting");
                }
            }
        }
        private NotificationTypeSetting _notificationTypeSetting;
    				

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
            Notifications.Clear();
            NotificationTypeSetting = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation NotificationTypeSetting.
        /// </summary>
        private void FixupNotificationTypeSetting(NotificationTypeSetting previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.NotificationTypes.Contains(this))
            {
                previousValue.NotificationTypes.Remove(this);
            }
    
            if (NotificationTypeSetting != null)
            {
                if (!NotificationTypeSetting.NotificationTypes.Contains(this))
                {
                    NotificationTypeSetting.NotificationTypes.Add(this);
                }
    
                NotificationTypeSettingId = NotificationTypeSetting.Id;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("NotificationTypeSetting", previousValue, NotificationTypeSetting);
                if (NotificationTypeSetting != null && !NotificationTypeSetting.ChangeTracker.ChangeTrackingEnabled)
                {
                    NotificationTypeSetting.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Notifications.
        /// </summary>
        private void FixupNotifications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Notification item in e.NewItems)
                {
                    item.NotificationType = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Notifications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Notification item in e.OldItems)
                {
                    if (ReferenceEquals(item.NotificationType, this))
                    {
                        item.NotificationType = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Notifications", item);
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
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "NotificationTypeSettingId":
    				this.NotificationTypeSettingId = Convert.ToInt32(value);
    				break;
    			case "NotificationCategory":
    				this.NotificationCategory = (NotificationCategory)Convert.ToInt32(value);
    				break;
    			case "NotificationTypeSetting":
    				this.NotificationTypeSetting = (NotificationTypeSetting)value;
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
    			case "Notifications":
    				this.Notifications.Add((Notification)value);
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
    			case "Notifications":
    				this.Notifications.Remove((Notification)value);
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
    		values.Add("Label", this.Label);
    		values.Add("Description", this.Description);
    		values.Add("NotificationTypeSettingId", this.NotificationTypeSettingId);
    		values.Add("NotificationTypeSetting", this.NotificationTypeSetting);
    
    		values.Add("Notifications", GetHashCodes(this.Notifications));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("NotificationTypeSetting", this.NotificationTypeSetting);
    
    		values.Add("Notifications", this.Notifications);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
