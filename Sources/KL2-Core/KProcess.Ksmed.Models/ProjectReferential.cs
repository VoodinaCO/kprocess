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
    [KnownType(typeof(Project))]
    /// <summary>
    /// 
    /// </summary>
    public partial class ProjectReferential : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.ProjectReferential";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ProjectReferential"/>.
        /// </summary>
    	public ProjectReferential()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _projectId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (_projectId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ProjectId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    if (!IsDeserializing)
                    {
                        if (Project != null && Project.ProjectId != value)
                        {
                            Project = null;
                        }
                    }
                    _projectId = value;
                    OnEntityPropertyChanged("ProjectId");
                }
            }
        }
        
        private bool _isEnabled;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    ChangeTracker.RecordValue("IsEnabled", _isEnabled, value);
                    _isEnabled = value;
                    OnEntityPropertyChanged("IsEnabled");
                }
            }
        }
        
        private bool _hasMultipleSelection;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool HasMultipleSelection
        {
            get { return _hasMultipleSelection; }
            set
            {
                if (_hasMultipleSelection != value)
                {
                    ChangeTracker.RecordValue("HasMultipleSelection", _hasMultipleSelection, value);
                    _hasMultipleSelection = value;
                    OnEntityPropertyChanged("HasMultipleSelection");
                }
            }
        }
        
        private bool _keepsSelection;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool KeepsSelection
        {
            get { return _keepsSelection; }
            set
            {
                if (_keepsSelection != value)
                {
                    ChangeTracker.RecordValue("KeepsSelection", _keepsSelection, value);
                    _keepsSelection = value;
                    OnEntityPropertyChanged("KeepsSelection");
                }
            }
        }
        
        private bool _hasQuantity;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool HasQuantity
        {
            get { return _hasQuantity; }
            set
            {
                if (_hasQuantity != value)
                {
                    ChangeTracker.RecordValue("HasQuantity", _hasQuantity, value);
                    _hasQuantity = value;
                    OnEntityPropertyChanged("HasQuantity");
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

        #region Propriétés Enum
        
        private ProcessReferentialIdentifier _referentialId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	public ProcessReferentialIdentifier ReferentialId
        {
            get { return _referentialId; }
            set
            {
                if (_referentialId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ReferentialId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _referentialId = value;
                    OnEntityPropertyChanged("ReferentialId");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Project Project
        {
            get { return _project; }
            set
            {
                if (!ReferenceEquals(_project, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (ProjectId != value.ProjectId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _project;
                    _project = value;
                    FixupProject(previousValue);
                    OnNavigationPropertyChanged("Project");
                }
            }
        }
        private Project _project;
    				

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
            Project = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Project.
        /// </summary>
        private void FixupProject(Project previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Referentials.Contains(this))
            {
                previousValue.Referentials.Remove(this);
            }
    
            if (Project != null)
            {
                if (!Project.Referentials.Contains(this))
                {
                    Project.Referentials.Add(this);
                }
    
                ProjectId = Project.ProjectId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Project", previousValue, Project);
                if (Project != null && !Project.ChangeTracker.ChangeTrackingEnabled)
                {
                    Project.StartTracking();
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
    			case "ProjectId":
    				this.ProjectId = Convert.ToInt32(value);
    				break;
    			case "IsEnabled":
    				this.IsEnabled = (bool)value;
    				break;
    			case "HasMultipleSelection":
    				this.HasMultipleSelection = (bool)value;
    				break;
    			case "KeepsSelection":
    				this.KeepsSelection = (bool)value;
    				break;
    			case "HasQuantity":
    				this.HasQuantity = (bool)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "ReferentialId":
    				this.ReferentialId = (ProcessReferentialIdentifier)value;
    				break;
    			case "Project":
    				this.Project = (Project)value;
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
    		values.Add("ProjectId", this.ProjectId);
    		values.Add("IsEnabled", this.IsEnabled);
    		values.Add("HasMultipleSelection", this.HasMultipleSelection);
    		values.Add("KeepsSelection", this.KeepsSelection);
    		values.Add("HasQuantity", this.HasQuantity);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("Project", this.Project);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Project", this.Project);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
