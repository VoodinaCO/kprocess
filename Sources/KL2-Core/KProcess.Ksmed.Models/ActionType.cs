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
    [KnownType(typeof(KActionReduced))]
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(AppResourceKey))]
    [KnownType(typeof(PublishedActionCategory))]
    /// <summary>
    /// Représente le type d'une action (tâche).
    /// </summary>
    public partial class ActionType : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged, ILocalizedLabels
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.ActionType";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionType"/>.
        /// </summary>
    	public ActionType()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private string _actionTypeCode;
        /// <summary>
        /// Obtient ou définit le code du type d'action.
        /// </summary>
        [DataMember]
        public string ActionTypeCode
        {
            get { return _actionTypeCode; }
            set
            {
                if (_actionTypeCode != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ActionTypeCode' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _actionTypeCode = value;
                    OnEntityPropertyChanged("ActionTypeCode");
                }
            }
        }
        
        private int _shortLabelResourceId;
        /// <summary>
        /// Obtient ou définit l'identifiant du libellé court.
        /// </summary>
        [DataMember]
        public int ShortLabelResourceId
        {
            get { return _shortLabelResourceId; }
            set
            {
                if (_shortLabelResourceId != value)
                {
                    ChangeTracker.RecordValue("ShortLabelResourceId", _shortLabelResourceId, value);
                    if (!IsDeserializing)
                    {
                        if (ShortLabelResource != null && ShortLabelResource.ResourceId != value)
                        {
                            ShortLabelResource = null;
                        }
                    }
                    _shortLabelResourceId = value;
                    OnEntityPropertyChanged("ShortLabelResourceId");
                }
            }
        }
        
        private int _longLabelResourceId;
        /// <summary>
        /// Obtient ou définit l'identifiant du libellé long.
        /// </summary>
        [DataMember]
        public int LongLabelResourceId
        {
            get { return _longLabelResourceId; }
            set
            {
                if (_longLabelResourceId != value)
                {
                    ChangeTracker.RecordValue("LongLabelResourceId", _longLabelResourceId, value);
                    if (!IsDeserializing)
                    {
                        if (LongLabelResource != null && LongLabelResource.ResourceId != value)
                        {
                            LongLabelResource = null;
                        }
                    }
                    _longLabelResourceId = value;
                    OnEntityPropertyChanged("LongLabelResourceId");
                }
            }
        }
        
        private byte[] _rowVersion;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public byte[] RowVersion
        {
            get { return _rowVersion; }
            set
            {
                if (_rowVersion != value)
                {
                    ChangeTracker.RecordValue("RowVersion", _rowVersion, value);
                    _rowVersion = value;
                    OnEntityPropertyChanged("RowVersion");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<KActionReduced> ActionsReduced
        {
            get
            {
                if (_actionsReduced == null)
                {
                    _actionsReduced = new TrackableCollection<KActionReduced>();
                    _actionsReduced.CollectionChanged += FixupActionsReduced;
                }
                return _actionsReduced;
            }
            set
            {
                if (!ReferenceEquals(_actionsReduced, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionsReduced != null)
                    {
                        _actionsReduced.CollectionChanged -= FixupActionsReduced;
                    }
                    _actionsReduced = value;
                    if (_actionsReduced != null)
                    {
                        _actionsReduced.CollectionChanged += FixupActionsReduced;
                    }
                    OnNavigationPropertyChanged("ActionsReduced");
                }
            }
        }
        private TrackableCollection<KActionReduced> _actionsReduced;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionCategory> ActionCategories
        {
            get
            {
                if (_actionCategories == null)
                {
                    _actionCategories = new TrackableCollection<ActionCategory>();
                    _actionCategories.CollectionChanged += FixupActionCategories;
                }
                return _actionCategories;
            }
            set
            {
                if (!ReferenceEquals(_actionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionCategories != null)
                    {
                        _actionCategories.CollectionChanged -= FixupActionCategories;
                    }
                    _actionCategories = value;
                    if (_actionCategories != null)
                    {
                        _actionCategories.CollectionChanged += FixupActionCategories;
                    }
                    OnNavigationPropertyChanged("ActionCategories");
                }
            }
        }
        private TrackableCollection<ActionCategory> _actionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public AppResourceKey LongLabelResource
        {
            get { return _longLabelResource; }
            set
            {
                if (!ReferenceEquals(_longLabelResource, value))
                {
                    var previousValue = _longLabelResource;
                    _longLabelResource = value;
                    FixupLongLabelResource(previousValue);
                    OnNavigationPropertyChanged("LongLabelResource");
                }
            }
        }
        private AppResourceKey _longLabelResource;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public AppResourceKey ShortLabelResource
        {
            get { return _shortLabelResource; }
            set
            {
                if (!ReferenceEquals(_shortLabelResource, value))
                {
                    var previousValue = _shortLabelResource;
                    _shortLabelResource = value;
                    FixupShortLabelResource(previousValue);
                    OnNavigationPropertyChanged("ShortLabelResource");
                }
            }
        }
        private AppResourceKey _shortLabelResource;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedActionCategory> PublishedActionCategory
        {
            get
            {
                if (_publishedActionCategory == null)
                {
                    _publishedActionCategory = new TrackableCollection<PublishedActionCategory>();
                    _publishedActionCategory.CollectionChanged += FixupPublishedActionCategory;
                }
                return _publishedActionCategory;
            }
            set
            {
                if (!ReferenceEquals(_publishedActionCategory, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedActionCategory != null)
                    {
                        _publishedActionCategory.CollectionChanged -= FixupPublishedActionCategory;
                    }
                    _publishedActionCategory = value;
                    if (_publishedActionCategory != null)
                    {
                        _publishedActionCategory.CollectionChanged += FixupPublishedActionCategory;
                    }
                    OnNavigationPropertyChanged("PublishedActionCategory");
                }
            }
        }
        private TrackableCollection<PublishedActionCategory> _publishedActionCategory;

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
            ActionsReduced.Clear();
            ActionCategories.Clear();
            LongLabelResource = null;
            ShortLabelResource = null;
            PublishedActionCategory.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation LongLabelResource.
        /// </summary>
        private void FixupLongLabelResource(AppResourceKey previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionTypesForLongLabel.Contains(this))
            {
                previousValue.ActionTypesForLongLabel.Remove(this);
            }
    
            if (LongLabelResource != null)
            {
                if (!LongLabelResource.ActionTypesForLongLabel.Contains(this))
                {
                    LongLabelResource.ActionTypesForLongLabel.Add(this);
                }
    
                LongLabelResourceId = LongLabelResource.ResourceId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LongLabelResource", previousValue, LongLabelResource);
                if (LongLabelResource != null && !LongLabelResource.ChangeTracker.ChangeTrackingEnabled)
                {
                    LongLabelResource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ShortLabelResource.
        /// </summary>
        private void FixupShortLabelResource(AppResourceKey previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionTypesForShortLabel.Contains(this))
            {
                previousValue.ActionTypesForShortLabel.Remove(this);
            }
    
            if (ShortLabelResource != null)
            {
                if (!ShortLabelResource.ActionTypesForShortLabel.Contains(this))
                {
                    ShortLabelResource.ActionTypesForShortLabel.Add(this);
                }
    
                ShortLabelResourceId = ShortLabelResource.ResourceId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("ShortLabelResource", previousValue, ShortLabelResource);
                if (ShortLabelResource != null && !ShortLabelResource.ChangeTracker.ChangeTrackingEnabled)
                {
                    ShortLabelResource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionsReduced.
        /// </summary>
        private void FixupActionsReduced(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KActionReduced item in e.NewItems)
                {
                    item.ActionType = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionsReduced", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KActionReduced item in e.OldItems)
                {
                    if (ReferenceEquals(item.ActionType, this))
                    {
                        item.ActionType = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionsReduced", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionCategories.
        /// </summary>
        private void FixupActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionCategory item in e.NewItems)
                {
                    item.Type = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.Type, this))
                    {
                        item.Type = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedActionCategory.
        /// </summary>
        private void FixupPublishedActionCategory(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedActionCategory item in e.NewItems)
                {
                    item.ActionType = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedActionCategory", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.ActionType, this))
                    {
                        item.ActionType = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedActionCategory", item);
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
    			case "ActionTypeCode":
    				this.ActionTypeCode = (string)value;
    				break;
    			case "ShortLabelResourceId":
    				this.ShortLabelResourceId = Convert.ToInt32(value);
    				break;
    			case "LongLabelResourceId":
    				this.LongLabelResourceId = Convert.ToInt32(value);
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "LongLabelResource":
    				this.LongLabelResource = (AppResourceKey)value;
    				break;
    			case "ShortLabelResource":
    				this.ShortLabelResource = (AppResourceKey)value;
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
    			case "ActionsReduced":
    				this.ActionsReduced.Add((KActionReduced)value);
    				break;
    			case "ActionCategories":
    				this.ActionCategories.Add((ActionCategory)value);
    				break;
    			case "PublishedActionCategory":
    				this.PublishedActionCategory.Add((PublishedActionCategory)value);
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
    			case "ActionsReduced":
    				this.ActionsReduced.Remove((KActionReduced)value);
    				break;
    			case "ActionCategories":
    				this.ActionCategories.Remove((ActionCategory)value);
    				break;
    			case "PublishedActionCategory":
    				this.PublishedActionCategory.Remove((PublishedActionCategory)value);
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
    		values.Add("ActionTypeCode", this.ActionTypeCode);
    		values.Add("ShortLabelResourceId", this.ShortLabelResourceId);
    		values.Add("LongLabelResourceId", this.LongLabelResourceId);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("LongLabelResource", this.LongLabelResource);
    		values.Add("ShortLabelResource", this.ShortLabelResource);
    
    		values.Add("ActionsReduced", GetHashCodes(this.ActionsReduced));
    		values.Add("ActionCategories", GetHashCodes(this.ActionCategories));
    		values.Add("PublishedActionCategory", GetHashCodes(this.PublishedActionCategory));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("LongLabelResource", this.LongLabelResource);
    		values.Add("ShortLabelResource", this.ShortLabelResource);
    
    		values.Add("ActionsReduced", this.ActionsReduced);
    		values.Add("ActionCategories", this.ActionCategories);
    		values.Add("PublishedActionCategory", this.PublishedActionCategory);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region ILocalizedLabels
    
        private string _shortLabel;
        private string _longLabel;
    
        /// <summary>
        /// Obtient ou définit le libellé court.
        /// </summary>
        [DataMember]
        public string ShortLabel
        {
            get { return _shortLabel; }
            set
            {
                if (_shortLabel != value)
                {
                    _shortLabel = value;
                    OnPropertyChanged("ShortLabel");
                }
            }
        }
    
        /// <summary>
        /// Obtient ou définit le libellé long.
        /// </summary>
        [DataMember]
        public string LongLabel
        {
            get { return _longLabel; }
            set
            {
                if (_longLabel != value)
                {
                    _longLabel = value;
                    OnPropertyChanged("LongLabel");
                }
            }
        }
    
        #endregion
    }
}
