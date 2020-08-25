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
    [KnownType(typeof(PublishedReferential))]
    /// <summary>
    /// 
    /// </summary>
    public partial class PublishedReferentialAction : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.PublishedReferentialAction";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PublishedReferentialAction"/>.
        /// </summary>
    	public PublishedReferentialAction()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublishedActionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
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
        
        private int _publishedReferentialId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublishedReferentialId
        {
            get { return _publishedReferentialId; }
            set
            {
                if (_publishedReferentialId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublishedReferentialId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (PublishedReferential != null && PublishedReferential.PublishedReferentialId != value)
                        {
                            PublishedReferential = null;
                        }
                    }
                    _publishedReferentialId = value;
                    OnEntityPropertyChanged("PublishedReferentialId");
                }
            }
        }
        
        private int _refNumber;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int RefNumber
        {
            get { return _refNumber; }
            set
            {
                if (_refNumber != value)
                {
                    ChangeTracker.RecordValue("RefNumber", _refNumber, value);
                    _refNumber = value;
                    OnEntityPropertyChanged("RefNumber");
                }
            }
        }
        
        private Nullable<int> _quantity;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    ChangeTracker.RecordValue("Quantity", _quantity, value);
                    _quantity = value;
                    OnEntityPropertyChanged("Quantity");
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
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (PublishedActionId != value.PublishedActionId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
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
        public PublishedReferential PublishedReferential
        {
            get { return _publishedReferential; }
            set
            {
                if (!ReferenceEquals(_publishedReferential, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (PublishedReferentialId != value.PublishedReferentialId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _publishedReferential;
                    _publishedReferential = value;
                    FixupPublishedReferential(previousValue);
                    OnNavigationPropertyChanged("PublishedReferential");
                }
            }
        }
        private PublishedReferential _publishedReferential;
    				

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
            PublishedAction = null;
            PublishedReferential = null;
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
    
            if (previousValue != null && previousValue.PublishedReferentialActions.Contains(this))
            {
                previousValue.PublishedReferentialActions.Remove(this);
            }
    
            if (PublishedAction != null)
            {
                if (!PublishedAction.PublishedReferentialActions.Contains(this))
                {
                    PublishedAction.PublishedReferentialActions.Add(this);
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
        /// Corrige l'état de la propriété de navigation PublishedReferential.
        /// </summary>
        private void FixupPublishedReferential(PublishedReferential previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedReferentialActions.Contains(this))
            {
                previousValue.PublishedReferentialActions.Remove(this);
            }
    
            if (PublishedReferential != null)
            {
                if (!PublishedReferential.PublishedReferentialActions.Contains(this))
                {
                    PublishedReferential.PublishedReferentialActions.Add(this);
                }
    
                PublishedReferentialId = PublishedReferential.PublishedReferentialId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PublishedReferential", previousValue, PublishedReferential);
                if (PublishedReferential != null && !PublishedReferential.ChangeTracker.ChangeTrackingEnabled)
                {
                    PublishedReferential.StartTracking();
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
    			case "PublishedActionId":
    				this.PublishedActionId = Convert.ToInt32(value);
    				break;
    			case "PublishedReferentialId":
    				this.PublishedReferentialId = Convert.ToInt32(value);
    				break;
    			case "RefNumber":
    				this.RefNumber = Convert.ToInt32(value);
    				break;
    			case "Quantity":
    				this.Quantity = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "PublishedAction":
    				this.PublishedAction = (PublishedAction)value;
    				break;
    			case "PublishedReferential":
    				this.PublishedReferential = (PublishedReferential)value;
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
    		values.Add("PublishedActionId", this.PublishedActionId);
    		values.Add("PublishedReferentialId", this.PublishedReferentialId);
    		values.Add("RefNumber", this.RefNumber);
    		values.Add("Quantity", this.Quantity);
    		values.Add("PublishedAction", this.PublishedAction);
    		values.Add("PublishedReferential", this.PublishedReferential);
    
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
    		values.Add("PublishedReferential", this.PublishedReferential);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
