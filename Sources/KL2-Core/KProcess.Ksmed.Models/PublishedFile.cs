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
    [KnownType(typeof(PublishedActionCategory))]
    [KnownType(typeof(PublishedReferential))]
    [KnownType(typeof(PublishedResource))]
    [KnownType(typeof(PublishedAction))]
    /// <summary>
    /// 
    /// </summary>
    public partial class PublishedFile : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.PublishedFile";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PublishedFile"/>.
        /// </summary>
    	public PublishedFile()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private string _hash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Hash
        {
            get { return _hash; }
            set
            {
                if (_hash != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'Hash' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _hash = value;
                    OnEntityPropertyChanged("Hash");
                }
            }
        }
        
        private string _extension;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Extension
        {
            get { return _extension; }
            set
            {
                if (_extension != value)
                {
                    ChangeTracker.RecordValue("Extension", _extension, value);
                    _extension = value;
                    OnEntityPropertyChanged("Extension");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedActionCategory> PublishedActionCategories
        {
            get
            {
                if (_publishedActionCategories == null)
                {
                    _publishedActionCategories = new TrackableCollection<PublishedActionCategory>();
                    _publishedActionCategories.CollectionChanged += FixupPublishedActionCategories;
                }
                return _publishedActionCategories;
            }
            set
            {
                if (!ReferenceEquals(_publishedActionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedActionCategories != null)
                    {
                        _publishedActionCategories.CollectionChanged -= FixupPublishedActionCategories;
                    }
                    _publishedActionCategories = value;
                    if (_publishedActionCategories != null)
                    {
                        _publishedActionCategories.CollectionChanged += FixupPublishedActionCategories;
                    }
                    OnNavigationPropertyChanged("PublishedActionCategories");
                }
            }
        }
        private TrackableCollection<PublishedActionCategory> _publishedActionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedReferential> PublishedReferentials
        {
            get
            {
                if (_publishedReferentials == null)
                {
                    _publishedReferentials = new TrackableCollection<PublishedReferential>();
                    _publishedReferentials.CollectionChanged += FixupPublishedReferentials;
                }
                return _publishedReferentials;
            }
            set
            {
                if (!ReferenceEquals(_publishedReferentials, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedReferentials != null)
                    {
                        _publishedReferentials.CollectionChanged -= FixupPublishedReferentials;
                    }
                    _publishedReferentials = value;
                    if (_publishedReferentials != null)
                    {
                        _publishedReferentials.CollectionChanged += FixupPublishedReferentials;
                    }
                    OnNavigationPropertyChanged("PublishedReferentials");
                }
            }
        }
        private TrackableCollection<PublishedReferential> _publishedReferentials;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedResource> PublishedResources
        {
            get
            {
                if (_publishedResources == null)
                {
                    _publishedResources = new TrackableCollection<PublishedResource>();
                    _publishedResources.CollectionChanged += FixupPublishedResources;
                }
                return _publishedResources;
            }
            set
            {
                if (!ReferenceEquals(_publishedResources, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedResources != null)
                    {
                        _publishedResources.CollectionChanged -= FixupPublishedResources;
                    }
                    _publishedResources = value;
                    if (_publishedResources != null)
                    {
                        _publishedResources.CollectionChanged += FixupPublishedResources;
                    }
                    OnNavigationPropertyChanged("PublishedResources");
                }
            }
        }
        private TrackableCollection<PublishedResource> _publishedResources;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedAction> PublishedActions
        {
            get
            {
                if (_publishedActions == null)
                {
                    _publishedActions = new TrackableCollection<PublishedAction>();
                    _publishedActions.CollectionChanged += FixupPublishedActions;
                }
                return _publishedActions;
            }
            set
            {
                if (!ReferenceEquals(_publishedActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedActions != null)
                    {
                        _publishedActions.CollectionChanged -= FixupPublishedActions;
                    }
                    _publishedActions = value;
                    if (_publishedActions != null)
                    {
                        _publishedActions.CollectionChanged += FixupPublishedActions;
                    }
                    OnNavigationPropertyChanged("PublishedActions");
                }
            }
        }
        private TrackableCollection<PublishedAction> _publishedActions;

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
            PublishedActionCategories.Clear();
            PublishedReferentials.Clear();
            PublishedResources.Clear();
            PublishedActions.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedActionCategories.
        /// </summary>
        private void FixupPublishedActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedActionCategory item in e.NewItems)
                {
                    item.File = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.File, this))
                    {
                        item.File = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedReferentials.
        /// </summary>
        private void FixupPublishedReferentials(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedReferential item in e.NewItems)
                {
                    item.File = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedReferentials", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedReferential item in e.OldItems)
                {
                    if (ReferenceEquals(item.File, this))
                    {
                        item.File = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedReferentials", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedResources.
        /// </summary>
        private void FixupPublishedResources(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedResource item in e.NewItems)
                {
                    item.File = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedResources", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedResource item in e.OldItems)
                {
                    if (ReferenceEquals(item.File, this))
                    {
                        item.File = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedResources", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedActions.
        /// </summary>
        private void FixupPublishedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedAction item in e.NewItems)
                {
                    item.Thumbnail = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Thumbnail, this))
                    {
                        item.Thumbnail = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedActions", item);
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
    			case "Hash":
    				this.Hash = (string)value;
    				break;
    			case "Extension":
    				this.Extension = (string)value;
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
    			case "PublishedActionCategories":
    				this.PublishedActionCategories.Add((PublishedActionCategory)value);
    				break;
    			case "PublishedReferentials":
    				this.PublishedReferentials.Add((PublishedReferential)value);
    				break;
    			case "PublishedResources":
    				this.PublishedResources.Add((PublishedResource)value);
    				break;
    			case "PublishedActions":
    				this.PublishedActions.Add((PublishedAction)value);
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
    			case "PublishedActionCategories":
    				this.PublishedActionCategories.Remove((PublishedActionCategory)value);
    				break;
    			case "PublishedReferentials":
    				this.PublishedReferentials.Remove((PublishedReferential)value);
    				break;
    			case "PublishedResources":
    				this.PublishedResources.Remove((PublishedResource)value);
    				break;
    			case "PublishedActions":
    				this.PublishedActions.Remove((PublishedAction)value);
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
    		values.Add("Hash", this.Hash);
    		values.Add("Extension", this.Extension);
    
    		values.Add("PublishedActionCategories", GetHashCodes(this.PublishedActionCategories));
    		values.Add("PublishedReferentials", GetHashCodes(this.PublishedReferentials));
    		values.Add("PublishedResources", GetHashCodes(this.PublishedResources));
    		values.Add("PublishedActions", GetHashCodes(this.PublishedActions));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    
    		values.Add("PublishedActionCategories", this.PublishedActionCategories);
    		values.Add("PublishedReferentials", this.PublishedReferentials);
    		values.Add("PublishedResources", this.PublishedResources);
    		values.Add("PublishedActions", this.PublishedActions);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
