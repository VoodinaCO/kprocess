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
    [KnownType(typeof(Equipment))]
    [KnownType(typeof(Operator))]
    [KnownType(typeof(KAction))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(CloudFile))]
    [KnownType(typeof(DocumentationActionDraft))]
    /// <summary>
    /// Représente une ressource d'un projet.
    /// </summary>
    public abstract partial class Resource : ReferentialBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Resource";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Resource"/>.
        /// </summary>
    	public Resource()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _resourceId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la ressource.
        /// </summary>
        [DataMember]
        public int ResourceId
        {
            get { return _resourceId; }
            set
            {
                if (_resourceId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ResourceId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _resourceId = value;
                    OnEntityPropertyChanged("ResourceId");
                }
            }
        }
        
        private double _paceRating;
        /// <summary>
        /// Obtient ou définit le jugement de valeur de la ressource.
        /// </summary>
        [DataMember]
    	//[LocalizableRange(0.01d, 100.0d, ErrorMessageResourceName = "Validation_Resource_PaceRating_Range")]
        public double PaceRating
        {
            get { return _paceRating; }
            set
            {
                if (_paceRating != value)
                {
                    ChangeTracker.RecordValue("PaceRating", _paceRating, value);
                    _paceRating = value;
                    OnEntityPropertyChanged("PaceRating");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé de la ressource.
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_Referential_Label_Required")]
    	[LocalizableStringLength(ActionReferentialProperties.LabelMaxLength, ErrorMessageResourceName = "Validation_Referential_Label_StringLength")]
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
        
        private string _color;
        /// <summary>
        /// Obtient ou définit la couleur de ce référentiel.
        /// </summary>
        [DataMember]
    	[LocalizableRegularExpression(@"#([0-9]|[A-F]|[a-f]){6,8}", ErrorMessageResourceName = "Validation_Referential_Color_Syntax")]
        public string Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    ChangeTracker.RecordValue("Color", _color, value);
                    _color = value;
                    OnEntityPropertyChanged("Color");
                }
            }
        }
        
        private string _description;
        /// <summary>
        /// Obtient ou définit la description.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(ActionReferentialProperties.DescriptionMaxLength, ErrorMessageResourceName = "Validation_Referential_Description_StringLength")]
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
        
        private int _createdByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a créé l'entité.
        /// </summary>
        [DataMember]
        public int CreatedByUserId
        {
            get { return _createdByUserId; }
            set
            {
                if (_createdByUserId != value)
                {
                    ChangeTracker.RecordValue("CreatedByUserId", _createdByUserId, value);
                    if (!IsDeserializing)
                    {
                        if (Creator != null && Creator.UserId != value)
                        {
                            Creator = null;
                        }
                    }
                    _createdByUserId = value;
                    OnEntityPropertyChanged("CreatedByUserId");
                }
            }
        }
        
        private System.DateTime _creationDate;
        /// <summary>
        /// Obtient ou définit la date de création de l'entité.
        /// </summary>
        [DataMember]
        public System.DateTime CreationDate
        {
            get { return _creationDate; }
            set
            {
                if (_creationDate != value)
                {
                    ChangeTracker.RecordValue("CreationDate", _creationDate, value);
                    _creationDate = value;
                    OnEntityPropertyChanged("CreationDate");
                }
            }
        }
        
        private int _modifiedByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a en dernier modifié l'entité.
        /// </summary>
        [DataMember]
        public int ModifiedByUserId
        {
            get { return _modifiedByUserId; }
            set
            {
                if (_modifiedByUserId != value)
                {
                    ChangeTracker.RecordValue("ModifiedByUserId", _modifiedByUserId, value);
                    if (!IsDeserializing)
                    {
                        if (LastModifier != null && LastModifier.UserId != value)
                        {
                            LastModifier = null;
                        }
                    }
                    _modifiedByUserId = value;
                    OnEntityPropertyChanged("ModifiedByUserId");
                }
            }
        }
        
        private System.DateTime _lastModificationDate;
        /// <summary>
        /// Obtient ou définit la dernière date de modification de l'entité.
        /// </summary>
        [DataMember]
        public System.DateTime LastModificationDate
        {
            get { return _lastModificationDate; }
            set
            {
                if (_lastModificationDate != value)
                {
                    ChangeTracker.RecordValue("LastModificationDate", _lastModificationDate, value);
                    _lastModificationDate = value;
                    OnEntityPropertyChanged("LastModificationDate");
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
                    ChangeTracker.RecordValue("Hash", _hash, value);
                    if (!IsDeserializing)
                    {
                        if (CloudFile != null && CloudFile.Hash != value)
                        {
                            CloudFile = null;
                        }
                    }
                    _hash = value;
                    OnEntityPropertyChanged("Hash");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit les actions associées à cette ressource.
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<KAction> Actions
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new TrackableCollection<KAction>();
                    _actions.CollectionChanged += FixupActions;
                }
                return _actions;
            }
            set
            {
                if (!ReferenceEquals(_actions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actions != null)
                    {
                        _actions.CollectionChanged -= FixupActions;
                    }
                    _actions = value;
                    if (_actions != null)
                    {
                        _actions.CollectionChanged += FixupActions;
                    }
                    OnNavigationPropertyChanged("Actions");
                }
            }
        }
        private TrackableCollection<KAction> _actions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User Creator
        {
            get { return _creator; }
            set
            {
                if (!ReferenceEquals(_creator, value))
                {
                    var previousValue = _creator;
                    _creator = value;
                    FixupCreator(previousValue);
                    OnNavigationPropertyChanged("Creator");
                }
            }
        }
        private User _creator;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public User LastModifier
        {
            get { return _lastModifier; }
            set
            {
                if (!ReferenceEquals(_lastModifier, value))
                {
                    var previousValue = _lastModifier;
                    _lastModifier = value;
                    FixupLastModifier(previousValue);
                    OnNavigationPropertyChanged("LastModifier");
                }
            }
        }
        private User _lastModifier;
    				
    
        /// <summary>
        /// Obtient ou définit les resources associées à cette ressource.
        /// </summary>
        [DataMember]
        public TrackableCollection<Video> Videos
        {
            get
            {
                if (_videos == null)
                {
                    _videos = new TrackableCollection<Video>();
                    _videos.CollectionChanged += FixupVideos;
                }
                return _videos;
            }
            set
            {
                if (!ReferenceEquals(_videos, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_videos != null)
                    {
                        _videos.CollectionChanged -= FixupVideos;
                    }
                    _videos = value;
                    if (_videos != null)
                    {
                        _videos.CollectionChanged += FixupVideos;
                    }
                    OnNavigationPropertyChanged("Videos");
                }
            }
        }
        private TrackableCollection<Video> _videos;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public CloudFile CloudFile
        {
            get { return _cloudFile; }
            set
            {
                if (!ReferenceEquals(_cloudFile, value))
                {
                    var previousValue = _cloudFile;
                    _cloudFile = value;
                    FixupCloudFile(previousValue);
                    OnNavigationPropertyChanged("CloudFile");
                }
            }
        }
        private CloudFile _cloudFile;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<DocumentationActionDraft> DocumentationActionDrafts
        {
            get
            {
                if (_documentationActionDrafts == null)
                {
                    _documentationActionDrafts = new TrackableCollection<DocumentationActionDraft>();
                    _documentationActionDrafts.CollectionChanged += FixupDocumentationActionDrafts;
                }
                return _documentationActionDrafts;
            }
            set
            {
                if (!ReferenceEquals(_documentationActionDrafts, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationActionDrafts != null)
                    {
                        _documentationActionDrafts.CollectionChanged -= FixupDocumentationActionDrafts;
                    }
                    _documentationActionDrafts = value;
                    if (_documentationActionDrafts != null)
                    {
                        _documentationActionDrafts.CollectionChanged += FixupDocumentationActionDrafts;
                    }
                    OnNavigationPropertyChanged("DocumentationActionDrafts");
                }
            }
        }
        private TrackableCollection<DocumentationActionDraft> _documentationActionDrafts;

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
            Actions.Clear();
            Creator = null;
            LastModifier = null;
            Videos.Clear();
            CloudFile = null;
            DocumentationActionDrafts.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Creator.
        /// </summary>
        private void FixupCreator(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CreatedResources.Contains(this))
            {
                previousValue.CreatedResources.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedResources.Contains(this))
                {
                    Creator.CreatedResources.Add(this);
                }
    
                CreatedByUserId = Creator.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Creator", previousValue, Creator);
                if (Creator != null && !Creator.ChangeTracker.ChangeTrackingEnabled)
                {
                    Creator.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation LastModifier.
        /// </summary>
        private void FixupLastModifier(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LastModifiedResources.Contains(this))
            {
                previousValue.LastModifiedResources.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedResources.Contains(this))
                {
                    LastModifier.LastModifiedResources.Add(this);
                }
    
                ModifiedByUserId = LastModifier.UserId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LastModifier", previousValue, LastModifier);
                if (LastModifier != null && !LastModifier.ChangeTracker.ChangeTrackingEnabled)
                {
                    LastModifier.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation CloudFile.
        /// </summary>
        private void FixupCloudFile(CloudFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.RefResources.Contains(this))
            {
                previousValue.RefResources.Remove(this);
            }
    
            if (CloudFile != null)
            {
                if (!CloudFile.RefResources.Contains(this))
                {
                    CloudFile.RefResources.Add(this);
                }
    
                Hash = CloudFile.Hash;
            }
            else if (!skipKeys)
            {
                Hash = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("CloudFile", previousValue, CloudFile);
                if (CloudFile != null && !CloudFile.ChangeTracker.ChangeTrackingEnabled)
                {
                    CloudFile.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Actions.
        /// </summary>
        private void FixupActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KAction item in e.NewItems)
                {
                    item.Resource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Actions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Resource, this))
                    {
                        item.Resource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Actions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Videos.
        /// </summary>
        private void FixupVideos(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Video item in e.NewItems)
                {
                    item.DefaultResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Videos", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Video item in e.OldItems)
                {
                    if (ReferenceEquals(item.DefaultResource, this))
                    {
                        item.DefaultResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Videos", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété DocumentationActionDrafts.
        /// </summary>
        private void FixupDocumentationActionDrafts(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationActionDraft item in e.NewItems)
                {
                    item.DocumentationResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationActionDrafts", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationActionDraft item in e.OldItems)
                {
                    if (ReferenceEquals(item.DocumentationResource, this))
                    {
                        item.DocumentationResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationActionDrafts", item);
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
    			case "ResourceId":
    				this.ResourceId = Convert.ToInt32(value);
    				break;
    			case "PaceRating":
    				this.PaceRating = (double)value;
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Color":
    				this.Color = (string)value;
    				break;
    			case "Description":
    				this.Description = (string)value;
    				break;
    			case "CreatedByUserId":
    				this.CreatedByUserId = Convert.ToInt32(value);
    				break;
    			case "CreationDate":
    				this.CreationDate = (System.DateTime)value;
    				break;
    			case "ModifiedByUserId":
    				this.ModifiedByUserId = Convert.ToInt32(value);
    				break;
    			case "LastModificationDate":
    				this.LastModificationDate = (System.DateTime)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "Hash":
    				this.Hash = (string)value;
    				break;
    			case "Creator":
    				this.Creator = (User)value;
    				break;
    			case "LastModifier":
    				this.LastModifier = (User)value;
    				break;
    			case "CloudFile":
    				this.CloudFile = (CloudFile)value;
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
    			case "Actions":
    				this.Actions.Add((KAction)value);
    				break;
    			case "Videos":
    				this.Videos.Add((Video)value);
    				break;
    			case "DocumentationActionDrafts":
    				this.DocumentationActionDrafts.Add((DocumentationActionDraft)value);
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
    			case "Actions":
    				this.Actions.Remove((KAction)value);
    				break;
    			case "Videos":
    				this.Videos.Remove((Video)value);
    				break;
    			case "DocumentationActionDrafts":
    				this.DocumentationActionDrafts.Remove((DocumentationActionDraft)value);
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
    		values.Add("ResourceId", this.ResourceId);
    		values.Add("PaceRating", this.PaceRating);
    		values.Add("Label", this.Label);
    		values.Add("Color", this.Color);
    		values.Add("Description", this.Description);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("Hash", this.Hash);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("CloudFile", this.CloudFile);
    
    		values.Add("Actions", GetHashCodes(this.Actions));
    		values.Add("Videos", GetHashCodes(this.Videos));
    		values.Add("DocumentationActionDrafts", GetHashCodes(this.DocumentationActionDrafts));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("CloudFile", this.CloudFile);
    
    		values.Add("Actions", this.Actions);
    		values.Add("Videos", this.Videos);
    		values.Add("DocumentationActionDrafts", this.DocumentationActionDrafts);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
