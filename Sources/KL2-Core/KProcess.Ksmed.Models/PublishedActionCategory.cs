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
    [KnownType(typeof(ActionType))]
    [KnownType(typeof(ActionValue))]
    [KnownType(typeof(PublishedAction))]
    [KnownType(typeof(PublishedFile))]
    /// <summary>
    /// 
    /// </summary>
    public partial class PublishedActionCategory : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.PublishedActionCategory";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PublishedActionCategory"/>.
        /// </summary>
    	public PublishedActionCategory()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _publishedActionCategoryId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int PublishedActionCategoryId
        {
            get { return _publishedActionCategoryId; }
            set
            {
                if (_publishedActionCategoryId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'PublishedActionCategoryId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _publishedActionCategoryId = value;
                    OnEntityPropertyChanged("PublishedActionCategoryId");
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
        
        private string _actionTypeCode;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ActionTypeCode
        {
            get { return _actionTypeCode; }
            set
            {
                if (_actionTypeCode != value)
                {
                    ChangeTracker.RecordValue("ActionTypeCode", _actionTypeCode, value);
                    if (!IsDeserializing)
                    {
                        if (ActionType != null && ActionType.ActionTypeCode != value)
                        {
                            ActionType = null;
                        }
                    }
                    _actionTypeCode = value;
                    OnEntityPropertyChanged("ActionTypeCode");
                }
            }
        }
        
        private string _actionValueCode;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ActionValueCode
        {
            get { return _actionValueCode; }
            set
            {
                if (_actionValueCode != value)
                {
                    ChangeTracker.RecordValue("ActionValueCode", _actionValueCode, value);
                    if (!IsDeserializing)
                    {
                        if (ActionValue != null && ActionValue.ActionValueCode != value)
                        {
                            ActionValue = null;
                        }
                    }
                    _actionValueCode = value;
                    OnEntityPropertyChanged("ActionValueCode");
                }
            }
        }
        
        private string _fileHash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string FileHash
        {
            get { return _fileHash; }
            set
            {
                if (_fileHash != value)
                {
                    ChangeTracker.RecordValue("FileHash", _fileHash, value);
                    if (!IsDeserializing)
                    {
                        if (File != null && File.Hash != value)
                        {
                            File = null;
                        }
                    }
                    _fileHash = value;
                    OnEntityPropertyChanged("FileHash");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ActionType ActionType
        {
            get { return _actionType; }
            set
            {
                if (!ReferenceEquals(_actionType, value))
                {
                    var previousValue = _actionType;
                    _actionType = value;
                    FixupActionType(previousValue);
                    OnNavigationPropertyChanged("ActionType");
                }
            }
        }
        private ActionType _actionType;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ActionValue ActionValue
        {
            get { return _actionValue; }
            set
            {
                if (!ReferenceEquals(_actionValue, value))
                {
                    var previousValue = _actionValue;
                    _actionValue = value;
                    FixupActionValue(previousValue);
                    OnNavigationPropertyChanged("ActionValue");
                }
            }
        }
        private ActionValue _actionValue;
    				
    
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
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PublishedFile File
        {
            get { return _file; }
            set
            {
                if (!ReferenceEquals(_file, value))
                {
                    var previousValue = _file;
                    _file = value;
                    FixupFile(previousValue);
                    OnNavigationPropertyChanged("File");
                }
            }
        }
        private PublishedFile _file;
    				

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
            ActionType = null;
            ActionValue = null;
            PublishedActions.Clear();
            File = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ActionType.
        /// </summary>
        private void FixupActionType(ActionType previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActionCategory.Contains(this))
            {
                previousValue.PublishedActionCategory.Remove(this);
            }
    
            if (ActionType != null)
            {
                if (!ActionType.PublishedActionCategory.Contains(this))
                {
                    ActionType.PublishedActionCategory.Add(this);
                }
    
                ActionTypeCode = ActionType.ActionTypeCode;
            }
            else if (!skipKeys)
            {
                ActionTypeCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("ActionType", previousValue, ActionType);
                if (ActionType != null && !ActionType.ChangeTracker.ChangeTrackingEnabled)
                {
                    ActionType.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ActionValue.
        /// </summary>
        private void FixupActionValue(ActionValue previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActionCategory.Contains(this))
            {
                previousValue.PublishedActionCategory.Remove(this);
            }
    
            if (ActionValue != null)
            {
                if (!ActionValue.PublishedActionCategory.Contains(this))
                {
                    ActionValue.PublishedActionCategory.Add(this);
                }
    
                ActionValueCode = ActionValue.ActionValueCode;
            }
            else if (!skipKeys)
            {
                ActionValueCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("ActionValue", previousValue, ActionValue);
                if (ActionValue != null && !ActionValue.ChangeTracker.ChangeTrackingEnabled)
                {
                    ActionValue.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation File.
        /// </summary>
        private void FixupFile(PublishedFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActionCategories.Contains(this))
            {
                previousValue.PublishedActionCategories.Remove(this);
            }
    
            if (File != null)
            {
                if (!File.PublishedActionCategories.Contains(this))
                {
                    File.PublishedActionCategories.Add(this);
                }
    
                FileHash = File.Hash;
            }
            else if (!skipKeys)
            {
                FileHash = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("File", previousValue, File);
                if (File != null && !File.ChangeTracker.ChangeTrackingEnabled)
                {
                    File.StartTracking();
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
                    item.PublishedActionCategory = this;
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
                    if (ReferenceEquals(item.PublishedActionCategory, this))
                    {
                        item.PublishedActionCategory = null;
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
    			case "PublishedActionCategoryId":
    				this.PublishedActionCategoryId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "ActionTypeCode":
    				this.ActionTypeCode = (string)value;
    				break;
    			case "ActionValueCode":
    				this.ActionValueCode = (string)value;
    				break;
    			case "FileHash":
    				this.FileHash = (string)value;
    				break;
    			case "ActionType":
    				this.ActionType = (ActionType)value;
    				break;
    			case "ActionValue":
    				this.ActionValue = (ActionValue)value;
    				break;
    			case "File":
    				this.File = (PublishedFile)value;
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
    		values.Add("PublishedActionCategoryId", this.PublishedActionCategoryId);
    		values.Add("Label", this.Label);
    		values.Add("Description", this.Description);
    		values.Add("ActionTypeCode", this.ActionTypeCode);
    		values.Add("ActionValueCode", this.ActionValueCode);
    		values.Add("FileHash", this.FileHash);
    		values.Add("ActionType", this.ActionType);
    		values.Add("ActionValue", this.ActionValue);
    		values.Add("File", this.File);
    
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
    		values.Add("ActionType", this.ActionType);
    		values.Add("ActionValue", this.ActionValue);
    		values.Add("File", this.File);
    
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
