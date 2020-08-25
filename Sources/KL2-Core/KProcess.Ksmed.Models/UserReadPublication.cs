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
    /// <summary>
    /// 
    /// </summary>
    public partial class UserReadPublication : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.UserReadPublication";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="UserReadPublication"/>.
        /// </summary>
    	public UserReadPublication()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'UserId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublicationId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
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
        
        private Nullable<System.DateTime> _readDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> ReadDate
        {
            get { return _readDate; }
            set
            {
                if (_readDate != value)
                {
                    ChangeTracker.RecordValue("ReadDate", _readDate, value);
                    _readDate = value;
                    OnEntityPropertyChanged("ReadDate");
                }
            }
        }
        
        private Nullable<System.Guid> _previousVersionPublicationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> PreviousVersionPublicationId
        {
            get { return _previousVersionPublicationId; }
            set
            {
                if (_previousVersionPublicationId != value)
                {
                    ChangeTracker.RecordValue("PreviousVersionPublicationId", _previousVersionPublicationId, value);
                    if (!IsDeserializing)
                    {
                        if (PreviousVersionPublication != null && PreviousVersionPublication.PublicationId != value)
                        {
                            PreviousVersionPublication = null;
                        }
                    }
                    _previousVersionPublicationId = value;
                    OnEntityPropertyChanged("PreviousVersionPublicationId");
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (PublicationId != value.PublicationId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (UserId != value.UserId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
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
        public Publication PreviousVersionPublication
        {
            get { return _previousVersionPublication; }
            set
            {
                if (!ReferenceEquals(_previousVersionPublication, value))
                {
                    var previousValue = _previousVersionPublication;
                    _previousVersionPublication = value;
                    FixupPreviousVersionPublication(previousValue);
                    OnNavigationPropertyChanged("PreviousVersionPublication");
                }
            }
        }
        private Publication _previousVersionPublication;
    				

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
            Publication = null;
            User = null;
            PreviousVersionPublication = null;
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
    
            if (previousValue != null && previousValue.Readers.Contains(this))
            {
                previousValue.Readers.Remove(this);
            }
    
            if (Publication != null)
            {
                if (!Publication.Readers.Contains(this))
                {
                    Publication.Readers.Add(this);
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
    
            if (previousValue != null && previousValue.ReadPublications.Contains(this))
            {
                previousValue.ReadPublications.Remove(this);
            }
    
            if (User != null)
            {
                if (!User.ReadPublications.Contains(this))
                {
                    User.ReadPublications.Add(this);
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
        /// Corrige l'état de la propriété de navigation PreviousVersionPublication.
        /// </summary>
        private void FixupPreviousVersionPublication(Publication previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.NextVersionUserReadPublications.Contains(this))
            {
                previousValue.NextVersionUserReadPublications.Remove(this);
            }
    
            if (PreviousVersionPublication != null)
            {
                if (!PreviousVersionPublication.NextVersionUserReadPublications.Contains(this))
                {
                    PreviousVersionPublication.NextVersionUserReadPublications.Add(this);
                }
    
                PreviousVersionPublicationId = PreviousVersionPublication.PublicationId;
            }
            else if (!skipKeys)
            {
                PreviousVersionPublicationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PreviousVersionPublication", previousValue, PreviousVersionPublication);
                if (PreviousVersionPublication != null && !PreviousVersionPublication.ChangeTracker.ChangeTrackingEnabled)
                {
                    PreviousVersionPublication.StartTracking();
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
    			case "UserId":
    				this.UserId = Convert.ToInt32(value);
    				break;
    			case "PublicationId":
    				this.PublicationId = (System.Guid)value;
    				break;
    			case "ReadDate":
    				this.ReadDate = (Nullable<System.DateTime>)value;
    				break;
    			case "PreviousVersionPublicationId":
    				this.PreviousVersionPublicationId = (Nullable<System.Guid>)value;
    				break;
    			case "Publication":
    				this.Publication = (Publication)value;
    				break;
    			case "User":
    				this.User = (User)value;
    				break;
    			case "PreviousVersionPublication":
    				this.PreviousVersionPublication = (Publication)value;
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
    		values.Add("UserId", this.UserId);
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("ReadDate", this.ReadDate);
    		values.Add("PreviousVersionPublicationId", this.PreviousVersionPublicationId);
    		values.Add("Publication", this.Publication);
    		values.Add("User", this.User);
    		values.Add("PreviousVersionPublication", this.PreviousVersionPublication);
    
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
    		values.Add("PreviousVersionPublication", this.PreviousVersionPublication);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
