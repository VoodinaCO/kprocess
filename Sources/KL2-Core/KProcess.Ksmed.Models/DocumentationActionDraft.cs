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
    [KnownType(typeof(Skill))]
    [KnownType(typeof(DocumentationActionDraftWBS))]
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(Resource))]
    [KnownType(typeof(ReferentialDocumentationActionDraft))]
    [KnownType(typeof(CloudFile))]
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentationActionDraft : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.DocumentationActionDraft";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DocumentationActionDraft"/>.
        /// </summary>
    	public DocumentationActionDraft()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _documentationActionDraftId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int DocumentationActionDraftId
        {
            get { return _documentationActionDraftId; }
            set
            {
                if (_documentationActionDraftId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'DocumentationActionDraftId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _documentationActionDraftId = value;
                    OnEntityPropertyChanged("DocumentationActionDraftId");
                }
            }
        }
        
        private string _label;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(LabelMaxLength, ErrorMessageResourceName = "Validation_KAction_Label_StringLength")]
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
        
        private long _duration;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long Duration
        {
            get { return _duration; }
            set
            {
                if (_duration != value)
                {
                    ChangeTracker.RecordValue("Duration", _duration, value);
                    _duration = value;
                    OnEntityPropertyChanged("Duration");
                }
            }
        }
        
        private string _thumbnailHash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ThumbnailHash
        {
            get { return _thumbnailHash; }
            set
            {
                if (_thumbnailHash != value)
                {
                    ChangeTracker.RecordValue("ThumbnailHash", _thumbnailHash, value);
                    if (!IsDeserializing)
                    {
                        if (Thumbnail != null && Thumbnail.Hash != value)
                        {
                            Thumbnail = null;
                        }
                    }
                    _thumbnailHash = value;
                    OnEntityPropertyChanged("ThumbnailHash");
                }
            }
        }
        
        private bool _isKeyTask;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsKeyTask
        {
            get { return _isKeyTask; }
            set
            {
                if (_isKeyTask != value)
                {
                    ChangeTracker.RecordValue("IsKeyTask", _isKeyTask, value);
                    _isKeyTask = value;
                    OnEntityPropertyChanged("IsKeyTask");
                }
            }
        }
        
        private Nullable<int> _skillId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> SkillId
        {
            get { return _skillId; }
            set
            {
                if (_skillId != value)
                {
                    ChangeTracker.RecordValue("SkillId", _skillId, value);
                    if (!IsDeserializing)
                    {
                        if (Skill != null && Skill.SkillId != value)
                        {
                            Skill = null;
                        }
                    }
                    _skillId = value;
                    OnEntityPropertyChanged("SkillId");
                }
            }
        }
        
        private Nullable<double> _customNumericValue;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<double> CustomNumericValue
        {
            get { return _customNumericValue; }
            set
            {
                if (_customNumericValue != value)
                {
                    ChangeTracker.RecordValue("CustomNumericValue", _customNumericValue, value);
                    _customNumericValue = value;
                    OnEntityPropertyChanged("CustomNumericValue");
                }
            }
        }
        
        private Nullable<double> _customNumericValue2;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<double> CustomNumericValue2
        {
            get { return _customNumericValue2; }
            set
            {
                if (_customNumericValue2 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericValue2", _customNumericValue2, value);
                    _customNumericValue2 = value;
                    OnEntityPropertyChanged("CustomNumericValue2");
                }
            }
        }
        
        private Nullable<double> _customNumericValue3;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<double> CustomNumericValue3
        {
            get { return _customNumericValue3; }
            set
            {
                if (_customNumericValue3 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericValue3", _customNumericValue3, value);
                    _customNumericValue3 = value;
                    OnEntityPropertyChanged("CustomNumericValue3");
                }
            }
        }
        
        private Nullable<double> _customNumericValue4;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<double> CustomNumericValue4
        {
            get { return _customNumericValue4; }
            set
            {
                if (_customNumericValue4 != value)
                {
                    ChangeTracker.RecordValue("CustomNumericValue4", _customNumericValue4, value);
                    _customNumericValue4 = value;
                    OnEntityPropertyChanged("CustomNumericValue4");
                }
            }
        }
        
        private string _customTextValue;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustomTextValue
        {
            get { return _customTextValue; }
            set
            {
                if (_customTextValue != value)
                {
                    ChangeTracker.RecordValue("CustomTextValue", _customTextValue, value);
                    _customTextValue = value;
                    OnEntityPropertyChanged("CustomTextValue");
                }
            }
        }
        
        private string _customTextValue2;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustomTextValue2
        {
            get { return _customTextValue2; }
            set
            {
                if (_customTextValue2 != value)
                {
                    ChangeTracker.RecordValue("CustomTextValue2", _customTextValue2, value);
                    _customTextValue2 = value;
                    OnEntityPropertyChanged("CustomTextValue2");
                }
            }
        }
        
        private string _customTextValue3;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustomTextValue3
        {
            get { return _customTextValue3; }
            set
            {
                if (_customTextValue3 != value)
                {
                    ChangeTracker.RecordValue("CustomTextValue3", _customTextValue3, value);
                    _customTextValue3 = value;
                    OnEntityPropertyChanged("CustomTextValue3");
                }
            }
        }
        
        private string _customTextValue4;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CustomTextValue4
        {
            get { return _customTextValue4; }
            set
            {
                if (_customTextValue4 != value)
                {
                    ChangeTracker.RecordValue("CustomTextValue4", _customTextValue4, value);
                    _customTextValue4 = value;
                    OnEntityPropertyChanged("CustomTextValue4");
                }
            }
        }
        
        private Nullable<int> _resourceId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ResourceId
        {
            get { return _resourceId; }
            set
            {
                if (_resourceId != value)
                {
                    ChangeTracker.RecordValue("ResourceId", _resourceId, value);
                    if (!IsDeserializing)
                    {
                        if (DocumentationResource != null && DocumentationResource.ResourceId != value)
                        {
                            DocumentationResource = null;
                        }
                    }
                    _resourceId = value;
                    OnEntityPropertyChanged("ResourceId");
                }
            }
        }
        
        private Nullable<int> _actionCategoryId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ActionCategoryId
        {
            get { return _actionCategoryId; }
            set
            {
                if (_actionCategoryId != value)
                {
                    ChangeTracker.RecordValue("ActionCategoryId", _actionCategoryId, value);
                    if (!IsDeserializing)
                    {
                        if (DocumentationCategory != null && DocumentationCategory.ActionCategoryId != value)
                        {
                            DocumentationCategory = null;
                        }
                    }
                    _actionCategoryId = value;
                    OnEntityPropertyChanged("ActionCategoryId");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Skill Skill
        {
            get { return _skill; }
            set
            {
                if (!ReferenceEquals(_skill, value))
                {
                    var previousValue = _skill;
                    _skill = value;
                    FixupSkill(previousValue);
                    OnNavigationPropertyChanged("Skill");
                }
            }
        }
        private Skill _skill;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<DocumentationActionDraftWBS> DocumentationActionDraftWBS
        {
            get
            {
                if (_documentationActionDraftWBS == null)
                {
                    _documentationActionDraftWBS = new TrackableCollection<DocumentationActionDraftWBS>();
                    _documentationActionDraftWBS.CollectionChanged += FixupDocumentationActionDraftWBS;
                }
                return _documentationActionDraftWBS;
            }
            set
            {
                if (!ReferenceEquals(_documentationActionDraftWBS, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_documentationActionDraftWBS != null)
                    {
                        _documentationActionDraftWBS.CollectionChanged -= FixupDocumentationActionDraftWBS;
                    }
                    _documentationActionDraftWBS = value;
                    if (_documentationActionDraftWBS != null)
                    {
                        _documentationActionDraftWBS.CollectionChanged += FixupDocumentationActionDraftWBS;
                    }
                    OnNavigationPropertyChanged("DocumentationActionDraftWBS");
                }
            }
        }
        private TrackableCollection<DocumentationActionDraftWBS> _documentationActionDraftWBS;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public ActionCategory DocumentationCategory
        {
            get { return _documentationCategory; }
            set
            {
                if (!ReferenceEquals(_documentationCategory, value))
                {
                    var previousValue = _documentationCategory;
                    _documentationCategory = value;
                    FixupDocumentationCategory(previousValue);
                    OnNavigationPropertyChanged("DocumentationCategory");
                }
            }
        }
        private ActionCategory _documentationCategory;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Resource DocumentationResource
        {
            get { return _documentationResource; }
            set
            {
                if (!ReferenceEquals(_documentationResource, value))
                {
                    var previousValue = _documentationResource;
                    _documentationResource = value;
                    FixupDocumentationResource(previousValue);
                    OnNavigationPropertyChanged("DocumentationResource");
                }
            }
        }
        private Resource _documentationResource;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ReferentialDocumentationActionDraft> ReferentialDocumentations
        {
            get
            {
                if (_referentialDocumentations == null)
                {
                    _referentialDocumentations = new TrackableCollection<ReferentialDocumentationActionDraft>();
                    _referentialDocumentations.CollectionChanged += FixupReferentialDocumentations;
                }
                return _referentialDocumentations;
            }
            set
            {
                if (!ReferenceEquals(_referentialDocumentations, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_referentialDocumentations != null)
                    {
                        _referentialDocumentations.CollectionChanged -= FixupReferentialDocumentations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (ReferentialDocumentationActionDraft item in _referentialDocumentations)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _referentialDocumentations = value;
                    if (_referentialDocumentations != null)
                    {
                        _referentialDocumentations.CollectionChanged += FixupReferentialDocumentations;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (ReferentialDocumentationActionDraft item in _referentialDocumentations)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("ReferentialDocumentations");
                }
            }
        }
        private TrackableCollection<ReferentialDocumentationActionDraft> _referentialDocumentations;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public CloudFile Thumbnail
        {
            get { return _thumbnail; }
            set
            {
                if (!ReferenceEquals(_thumbnail, value))
                {
                    var previousValue = _thumbnail;
                    _thumbnail = value;
                    FixupThumbnail(previousValue);
                    OnNavigationPropertyChanged("Thumbnail");
                }
            }
        }
        private CloudFile _thumbnail;
    				

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
            Skill = null;
            DocumentationActionDraftWBS.Clear();
            DocumentationCategory = null;
            DocumentationResource = null;
            ReferentialDocumentations.Clear();
            Thumbnail = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Skill.
        /// </summary>
        private void FixupSkill(Skill previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDrafts.Contains(this))
            {
                previousValue.DocumentationActionDrafts.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.DocumentationActionDrafts.Contains(this))
                {
                    Skill.DocumentationActionDrafts.Add(this);
                }
    
                SkillId = Skill.SkillId;
            }
            else if (!skipKeys)
            {
                SkillId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Skill", previousValue, Skill);
                if (Skill != null && !Skill.ChangeTracker.ChangeTrackingEnabled)
                {
                    Skill.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation DocumentationCategory.
        /// </summary>
        private void FixupDocumentationCategory(ActionCategory previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDrafts.Contains(this))
            {
                previousValue.DocumentationActionDrafts.Remove(this);
            }
    
            if (DocumentationCategory != null)
            {
                if (!DocumentationCategory.DocumentationActionDrafts.Contains(this))
                {
                    DocumentationCategory.DocumentationActionDrafts.Add(this);
                }
    
                ActionCategoryId = DocumentationCategory.ActionCategoryId;
            }
            else if (!skipKeys)
            {
                ActionCategoryId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DocumentationCategory", previousValue, DocumentationCategory);
                if (DocumentationCategory != null && !DocumentationCategory.ChangeTracker.ChangeTrackingEnabled)
                {
                    DocumentationCategory.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation DocumentationResource.
        /// </summary>
        private void FixupDocumentationResource(Resource previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDrafts.Contains(this))
            {
                previousValue.DocumentationActionDrafts.Remove(this);
            }
    
            if (DocumentationResource != null)
            {
                if (!DocumentationResource.DocumentationActionDrafts.Contains(this))
                {
                    DocumentationResource.DocumentationActionDrafts.Add(this);
                }
    
                ResourceId = DocumentationResource.ResourceId;
            }
            else if (!skipKeys)
            {
                ResourceId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DocumentationResource", previousValue, DocumentationResource);
                if (DocumentationResource != null && !DocumentationResource.ChangeTracker.ChangeTrackingEnabled)
                {
                    DocumentationResource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Thumbnail.
        /// </summary>
        private void FixupThumbnail(CloudFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDrafts.Contains(this))
            {
                previousValue.DocumentationActionDrafts.Remove(this);
            }
    
            if (Thumbnail != null)
            {
                if (!Thumbnail.DocumentationActionDrafts.Contains(this))
                {
                    Thumbnail.DocumentationActionDrafts.Add(this);
                }
    
                ThumbnailHash = Thumbnail.Hash;
            }
            else if (!skipKeys)
            {
                ThumbnailHash = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Thumbnail", previousValue, Thumbnail);
                if (Thumbnail != null && !Thumbnail.ChangeTracker.ChangeTrackingEnabled)
                {
                    Thumbnail.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété DocumentationActionDraftWBS.
        /// </summary>
        private void FixupDocumentationActionDraftWBS(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (DocumentationActionDraftWBS item in e.NewItems)
                {
                    item.DocumentationActionDraft = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("DocumentationActionDraftWBS", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DocumentationActionDraftWBS item in e.OldItems)
                {
                    if (ReferenceEquals(item.DocumentationActionDraft, this))
                    {
                        item.DocumentationActionDraft = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationActionDraftWBS", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ReferentialDocumentations.
        /// </summary>
        private void FixupReferentialDocumentations(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ReferentialDocumentationActionDraft item in e.NewItems)
                {
                    item.DocumentationActionDraft = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ReferentialDocumentations", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ReferentialDocumentationActionDraft item in e.OldItems)
                {
                    if (ReferenceEquals(item.DocumentationActionDraft, this))
                    {
                        item.DocumentationActionDraft = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ReferentialDocumentations", item);
                        // Delete the dependent end of this identifying association. If the current state is Added,
                        // allow the relationship to be changed without causing the dependent to be deleted.
                        if (item.ChangeTracker.State != ObjectState.Added)
                        {
                            item.MarkAsDeleted();
                        }
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Remove the previous dependent from the event listener.
                    ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
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
    			case "DocumentationActionDraftId":
    				this.DocumentationActionDraftId = Convert.ToInt32(value);
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Duration":
    				this.Duration = (long)value;
    				break;
    			case "ThumbnailHash":
    				this.ThumbnailHash = (string)value;
    				break;
    			case "IsKeyTask":
    				this.IsKeyTask = (bool)value;
    				break;
    			case "SkillId":
    				this.SkillId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "CustomNumericValue":
    				this.CustomNumericValue = (Nullable<double>)value;
    				break;
    			case "CustomNumericValue2":
    				this.CustomNumericValue2 = (Nullable<double>)value;
    				break;
    			case "CustomNumericValue3":
    				this.CustomNumericValue3 = (Nullable<double>)value;
    				break;
    			case "CustomNumericValue4":
    				this.CustomNumericValue4 = (Nullable<double>)value;
    				break;
    			case "CustomTextValue":
    				this.CustomTextValue = (string)value;
    				break;
    			case "CustomTextValue2":
    				this.CustomTextValue2 = (string)value;
    				break;
    			case "CustomTextValue3":
    				this.CustomTextValue3 = (string)value;
    				break;
    			case "CustomTextValue4":
    				this.CustomTextValue4 = (string)value;
    				break;
    			case "ResourceId":
    				this.ResourceId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "ActionCategoryId":
    				this.ActionCategoryId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Skill":
    				this.Skill = (Skill)value;
    				break;
    			case "DocumentationCategory":
    				this.DocumentationCategory = (ActionCategory)value;
    				break;
    			case "DocumentationResource":
    				this.DocumentationResource = (Resource)value;
    				break;
    			case "Thumbnail":
    				this.Thumbnail = (CloudFile)value;
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
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Add((DocumentationActionDraftWBS)value);
    				break;
    			case "ReferentialDocumentations":
    				this.ReferentialDocumentations.Add((ReferentialDocumentationActionDraft)value);
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
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Remove((DocumentationActionDraftWBS)value);
    				break;
    			case "ReferentialDocumentations":
    				this.ReferentialDocumentations.Remove((ReferentialDocumentationActionDraft)value);
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
    		values.Add("DocumentationActionDraftId", this.DocumentationActionDraftId);
    		values.Add("Label", this.Label);
    		values.Add("Duration", this.Duration);
    		values.Add("ThumbnailHash", this.ThumbnailHash);
    		values.Add("IsKeyTask", this.IsKeyTask);
    		values.Add("SkillId", this.SkillId);
    		values.Add("CustomNumericValue", this.CustomNumericValue);
    		values.Add("CustomNumericValue2", this.CustomNumericValue2);
    		values.Add("CustomNumericValue3", this.CustomNumericValue3);
    		values.Add("CustomNumericValue4", this.CustomNumericValue4);
    		values.Add("CustomTextValue", this.CustomTextValue);
    		values.Add("CustomTextValue2", this.CustomTextValue2);
    		values.Add("CustomTextValue3", this.CustomTextValue3);
    		values.Add("CustomTextValue4", this.CustomTextValue4);
    		values.Add("ResourceId", this.ResourceId);
    		values.Add("ActionCategoryId", this.ActionCategoryId);
    		values.Add("Skill", this.Skill);
    		values.Add("DocumentationCategory", this.DocumentationCategory);
    		values.Add("DocumentationResource", this.DocumentationResource);
    		values.Add("Thumbnail", this.Thumbnail);
    
    		values.Add("DocumentationActionDraftWBS", GetHashCodes(this.DocumentationActionDraftWBS));
    		values.Add("ReferentialDocumentations", GetHashCodes(this.ReferentialDocumentations));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Skill", this.Skill);
    		values.Add("DocumentationCategory", this.DocumentationCategory);
    		values.Add("DocumentationResource", this.DocumentationResource);
    		values.Add("Thumbnail", this.Thumbnail);
    
    		values.Add("DocumentationActionDraftWBS", this.DocumentationActionDraftWBS);
    		values.Add("ReferentialDocumentations", this.ReferentialDocumentations);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Label.
        /// </summary>
    	public const int LabelMaxLength = 100;

        #endregion

    }
}
