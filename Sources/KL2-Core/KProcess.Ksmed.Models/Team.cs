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
    [KnownType(typeof(User))]
    /// <summary>
    /// 
    /// </summary>
    public partial class Team : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Team";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Team"/>.
        /// </summary>
    	public Team()
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
        
        private string _name;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Team_Name_Required")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    ChangeTracker.RecordValue("Name", _name, value);
                    _name = value;
                    OnEntityPropertyChanged("Name");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<User> Users
        {
            get
            {
                if (_users == null)
                {
                    _users = new TrackableCollection<User>();
                    _users.CollectionChanged += FixupUsers;
                }
                return _users;
            }
            set
            {
                if (!ReferenceEquals(_users, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_users != null)
                    {
                        _users.CollectionChanged -= FixupUsers;
                    }
                    _users = value;
                    if (_users != null)
                    {
                        _users.CollectionChanged += FixupUsers;
                    }
                    OnNavigationPropertyChanged("Users");
                }
            }
        }
        private TrackableCollection<User> _users;

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
            Users.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété Users.
        /// </summary>
        private void FixupUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (User item in e.NewItems)
                {
                    if (!item.Teams.Contains(this))
                    {
                        item.Teams.Add(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Users", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (User item in e.OldItems)
                {
                    if (item.Teams.Contains(this))
                    {
                        item.Teams.Remove(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Users", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
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
    			case "Name":
    				this.Name = (string)value;
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
    			case "Users":
    				this.Users.Add((User)value);
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
    			case "Users":
    				this.Users.Remove((User)value);
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
    		values.Add("Name", this.Name);
    
    		values.Add("Users", GetHashCodes(this.Users));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    
    		values.Add("Users", this.Users);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
