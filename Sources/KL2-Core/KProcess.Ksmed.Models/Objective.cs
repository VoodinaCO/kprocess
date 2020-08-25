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
    [KnownType(typeof(AppResourceKey))]
    [KnownType(typeof(Project))]
    /// <summary>
    /// Représente un objectif pour un projet.
    /// </summary>
    public partial class Objective : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged, ILocalizedLabels
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Objective";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Objective"/>.
        /// </summary>
    	public Objective()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private string _objectiveCode;
        /// <summary>
        /// Obtient ou définit le code identifiant l'objectif.
        /// </summary>
        [DataMember]
        public string ObjectiveCode
        {
            get { return _objectiveCode; }
            set
            {
                if (_objectiveCode != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ObjectiveCode' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _objectiveCode = value;
                    OnEntityPropertyChanged("ObjectiveCode");
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
        public TrackableCollection<Project> Projects
        {
            get
            {
                if (_projects == null)
                {
                    _projects = new TrackableCollection<Project>();
                    _projects.CollectionChanged += FixupProjects;
                }
                return _projects;
            }
            set
            {
                if (!ReferenceEquals(_projects, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_projects != null)
                    {
                        _projects.CollectionChanged -= FixupProjects;
                    }
                    _projects = value;
                    if (_projects != null)
                    {
                        _projects.CollectionChanged += FixupProjects;
                    }
                    OnNavigationPropertyChanged("Projects");
                }
            }
        }
        private TrackableCollection<Project> _projects;

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
            LongLabelResource = null;
            ShortLabelResource = null;
            Projects.Clear();
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
    
            if (previousValue != null && previousValue.ObjectivesForLongLabel.Contains(this))
            {
                previousValue.ObjectivesForLongLabel.Remove(this);
            }
    
            if (LongLabelResource != null)
            {
                if (!LongLabelResource.ObjectivesForLongLabel.Contains(this))
                {
                    LongLabelResource.ObjectivesForLongLabel.Add(this);
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
    
            if (previousValue != null && previousValue.ObjectivesForShortLabel.Contains(this))
            {
                previousValue.ObjectivesForShortLabel.Remove(this);
            }
    
            if (ShortLabelResource != null)
            {
                if (!ShortLabelResource.ObjectivesForShortLabel.Contains(this))
                {
                    ShortLabelResource.ObjectivesForShortLabel.Add(this);
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
        /// Corrige l'état de la propriété Projects.
        /// </summary>
        private void FixupProjects(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Project item in e.NewItems)
                {
                    item.Objective = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Projects", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Project item in e.OldItems)
                {
                    if (ReferenceEquals(item.Objective, this))
                    {
                        item.Objective = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Projects", item);
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
    			case "ObjectiveCode":
    				this.ObjectiveCode = (string)value;
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
    			case "Projects":
    				this.Projects.Add((Project)value);
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
    			case "Projects":
    				this.Projects.Remove((Project)value);
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
    		values.Add("ObjectiveCode", this.ObjectiveCode);
    		values.Add("ShortLabelResourceId", this.ShortLabelResourceId);
    		values.Add("LongLabelResourceId", this.LongLabelResourceId);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("LongLabelResource", this.LongLabelResource);
    		values.Add("ShortLabelResource", this.ShortLabelResource);
    
    		values.Add("Projects", GetHashCodes(this.Projects));
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
    
    		values.Add("Projects", this.Projects);
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
