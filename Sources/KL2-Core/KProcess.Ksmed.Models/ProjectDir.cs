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
    [KnownType(typeof(ProjectDir))]
    [KnownType(typeof(Procedure))]
    /// <summary>
    /// 
    /// </summary>
    public partial class ProjectDir : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.ProjectDir";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ProjectDir"/>.
        /// </summary>
    	public ProjectDir()
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
        /// Obtient ou définit l'identifiant du dossier.
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
        /// Obtient ou définit le nom du dossier.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(NameMaxLength, ErrorMessageResourceName = "Validation_ProjectDir_Name_StringLength")]
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
        
        private Nullable<int> _parentId;
        /// <summary>
        /// Obtient ou définit l'identifiant du dossier parent.
        /// </summary>
        [DataMember]
        public Nullable<int> ParentId
        {
            get { return _parentId; }
            set
            {
                if (_parentId != value)
                {
                    ChangeTracker.RecordValue("ParentId", _parentId, value);
                    if (!IsDeserializing)
                    {
                        if (Parent != null && Parent.Id != value)
                        {
                            Parent = null;
                        }
                    }
                    _parentId = value;
                    OnEntityPropertyChanged("ParentId");
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

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit les sous-dossiers du dossier.
        /// </summary>
        [DataMember]
        public TrackableCollection<ProjectDir> Childs
        {
            get
            {
                if (_childs == null)
                {
                    _childs = new TrackableCollection<ProjectDir>();
                    _childs.CollectionChanged += FixupChilds;
                }
                return _childs;
            }
            set
            {
                if (!ReferenceEquals(_childs, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_childs != null)
                    {
                        _childs.CollectionChanged -= FixupChilds;
                    }
                    _childs = value;
                    if (_childs != null)
                    {
                        _childs.CollectionChanged += FixupChilds;
                    }
                    OnNavigationPropertyChanged("Childs");
                }
            }
        }
        private TrackableCollection<ProjectDir> _childs;
    
        /// <summary>
        /// Obtient ou définit le dossier parent.
        /// </summary>
        [DataMember]
        public ProjectDir Parent
        {
            get { return _parent; }
            set
            {
                if (!ReferenceEquals(_parent, value))
                {
                    var previousValue = _parent;
                    _parent = value;
                    FixupParent(previousValue);
                    OnNavigationPropertyChanged("Parent");
                }
            }
        }
        private ProjectDir _parent;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Procedure> Processes
        {
            get
            {
                if (_processes == null)
                {
                    _processes = new TrackableCollection<Procedure>();
                    _processes.CollectionChanged += FixupProcesses;
                }
                return _processes;
            }
            set
            {
                if (!ReferenceEquals(_processes, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_processes != null)
                    {
                        _processes.CollectionChanged -= FixupProcesses;
                    }
                    _processes = value;
                    if (_processes != null)
                    {
                        _processes.CollectionChanged += FixupProcesses;
                    }
                    OnNavigationPropertyChanged("Processes");
                }
            }
        }
        private TrackableCollection<Procedure> _processes;

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
            Childs.Clear();
            Parent = null;
            Processes.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Parent.
        /// </summary>
        private void FixupParent(ProjectDir previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Childs.Contains(this))
            {
                previousValue.Childs.Remove(this);
            }
    
            if (Parent != null)
            {
                if (!Parent.Childs.Contains(this))
                {
                    Parent.Childs.Add(this);
                }
    
                ParentId = Parent.Id;
            }
            else if (!skipKeys)
            {
                ParentId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Parent", previousValue, Parent);
                if (Parent != null && !Parent.ChangeTracker.ChangeTrackingEnabled)
                {
                    Parent.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Childs.
        /// </summary>
        private void FixupChilds(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ProjectDir item in e.NewItems)
                {
                    item.Parent = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Childs", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ProjectDir item in e.OldItems)
                {
                    if (ReferenceEquals(item.Parent, this))
                    {
                        item.Parent = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Childs", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Processes.
        /// </summary>
        private void FixupProcesses(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Procedure item in e.NewItems)
                {
                    item.ProjectDir = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Processes", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Procedure item in e.OldItems)
                {
                    if (ReferenceEquals(item.ProjectDir, this))
                    {
                        item.ProjectDir = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Processes", item);
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
    			case "ParentId":
    				this.ParentId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "Parent":
    				this.Parent = (ProjectDir)value;
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
    			case "Childs":
    				this.Childs.Add((ProjectDir)value);
    				break;
    			case "Processes":
    				this.Processes.Add((Procedure)value);
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
    			case "Childs":
    				this.Childs.Remove((ProjectDir)value);
    				break;
    			case "Processes":
    				this.Processes.Remove((Procedure)value);
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
    		values.Add("ParentId", this.ParentId);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Parent", this.Parent);
    
    		values.Add("Childs", GetHashCodes(this.Childs));
    		values.Add("Processes", GetHashCodes(this.Processes));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Parent", this.Parent);
    
    		values.Add("Childs", this.Childs);
    		values.Add("Processes", this.Processes);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Name.
        /// </summary>
    	public const int NameMaxLength = 50;

        #endregion

    }
}
