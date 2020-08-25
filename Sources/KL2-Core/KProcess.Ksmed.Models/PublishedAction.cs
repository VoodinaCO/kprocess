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
    [KnownType(typeof(PublishedActionCategory))]
    [KnownType(typeof(PublishedResource))]
    [KnownType(typeof(PublishedReferentialAction))]
    [KnownType(typeof(CutVideo))]
    [KnownType(typeof(PublishedFile))]
    [KnownType(typeof(PublishedAction))]
    [KnownType(typeof(ValidationTraining))]
    [KnownType(typeof(QualificationStep))]
    [KnownType(typeof(Skill))]
    [KnownType(typeof(InspectionStep))]
    /// <summary>
    /// 
    /// </summary>
    public partial class PublishedAction : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.PublishedAction";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PublishedAction"/>.
        /// </summary>
    	public PublishedAction()
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
                    _publishedActionId = value;
                    OnEntityPropertyChanged("PublishedActionId");
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
                    ChangeTracker.RecordValue("PublicationId", _publicationId, value);
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
                        if (PublishedResource != null && PublishedResource.PublishedResourceId != value)
                        {
                            PublishedResource = null;
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
                        if (PublishedActionCategory != null && PublishedActionCategory.PublishedActionCategoryId != value)
                        {
                            PublishedActionCategory = null;
                        }
                    }
                    _actionCategoryId = value;
                    OnEntityPropertyChanged("ActionCategoryId");
                }
            }
        }
        
        private string _wBS;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string WBS
        {
            get { return _wBS; }
            set
            {
                if (_wBS != value)
                {
                    ChangeTracker.RecordValue("WBS", _wBS, value);
                    _wBS = value;
                    OnEntityPropertyChanged("WBS");
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
        
        private long _start;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long Start
        {
            get { return _start; }
            set
            {
                if (_start != value)
                {
                    ChangeTracker.RecordValue("Start", _start, value);
                    _start = value;
                    OnEntityPropertyChanged("Start");
                }
            }
        }
        
        private long _finish;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long Finish
        {
            get { return _finish; }
            set
            {
                if (_finish != value)
                {
                    ChangeTracker.RecordValue("Finish", _finish, value);
                    _finish = value;
                    OnEntityPropertyChanged("Finish");
                }
            }
        }
        
        private long _buildStart;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long BuildStart
        {
            get { return _buildStart; }
            set
            {
                if (_buildStart != value)
                {
                    ChangeTracker.RecordValue("BuildStart", _buildStart, value);
                    _buildStart = value;
                    OnEntityPropertyChanged("BuildStart");
                }
            }
        }
        
        private long _buildFinish;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public long BuildFinish
        {
            get { return _buildFinish; }
            set
            {
                if (_buildFinish != value)
                {
                    ChangeTracker.RecordValue("BuildFinish", _buildFinish, value);
                    _buildFinish = value;
                    OnEntityPropertyChanged("BuildFinish");
                }
            }
        }
        
        private bool _isRandom;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public bool IsRandom
        {
            get { return _isRandom; }
            set
            {
                if (_isRandom != value)
                {
                    ChangeTracker.RecordValue("IsRandom", _isRandom, value);
                    _isRandom = value;
                    OnEntityPropertyChanged("IsRandom");
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
        
        private string _differenceReason;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string DifferenceReason
        {
            get { return _differenceReason; }
            set
            {
                if (_differenceReason != value)
                {
                    ChangeTracker.RecordValue("DifferenceReason", _differenceReason, value);
                    _differenceReason = value;
                    OnEntityPropertyChanged("DifferenceReason");
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
        
        private string _cutVideoHash;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string CutVideoHash
        {
            get { return _cutVideoHash; }
            set
            {
                if (_cutVideoHash != value)
                {
                    ChangeTracker.RecordValue("CutVideoHash", _cutVideoHash, value);
                    if (!IsDeserializing)
                    {
                        if (CutVideo != null && CutVideo.Hash != value)
                        {
                            CutVideo = null;
                        }
                    }
                    _cutVideoHash = value;
                    OnEntityPropertyChanged("CutVideoHash");
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
        
        private Nullable<System.Guid> _linkedPublicationId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.Guid> LinkedPublicationId
        {
            get { return _linkedPublicationId; }
            set
            {
                if (_linkedPublicationId != value)
                {
                    ChangeTracker.RecordValue("LinkedPublicationId", _linkedPublicationId, value);
                    if (!IsDeserializing)
                    {
                        if (LinkedPublication != null && LinkedPublication.PublicationId != value)
                        {
                            LinkedPublication = null;
                        }
                    }
                    _linkedPublicationId = value;
                    OnEntityPropertyChanged("LinkedPublicationId");
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
        public PublishedActionCategory PublishedActionCategory
        {
            get { return _publishedActionCategory; }
            set
            {
                if (!ReferenceEquals(_publishedActionCategory, value))
                {
                    var previousValue = _publishedActionCategory;
                    _publishedActionCategory = value;
                    FixupPublishedActionCategory(previousValue);
                    OnNavigationPropertyChanged("PublishedActionCategory");
                }
            }
        }
        private PublishedActionCategory _publishedActionCategory;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PublishedResource PublishedResource
        {
            get { return _publishedResource; }
            set
            {
                if (!ReferenceEquals(_publishedResource, value))
                {
                    var previousValue = _publishedResource;
                    _publishedResource = value;
                    FixupPublishedResource(previousValue);
                    OnNavigationPropertyChanged("PublishedResource");
                }
            }
        }
        private PublishedResource _publishedResource;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedReferentialAction> PublishedReferentialActions
        {
            get
            {
                if (_publishedReferentialActions == null)
                {
                    _publishedReferentialActions = new TrackableCollection<PublishedReferentialAction>();
                    _publishedReferentialActions.CollectionChanged += FixupPublishedReferentialActions;
                }
                return _publishedReferentialActions;
            }
            set
            {
                if (!ReferenceEquals(_publishedReferentialActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publishedReferentialActions != null)
                    {
                        _publishedReferentialActions.CollectionChanged -= FixupPublishedReferentialActions;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (PublishedReferentialAction item in _publishedReferentialActions)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _publishedReferentialActions = value;
                    if (_publishedReferentialActions != null)
                    {
                        _publishedReferentialActions.CollectionChanged += FixupPublishedReferentialActions;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (PublishedReferentialAction item in _publishedReferentialActions)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("PublishedReferentialActions");
                }
            }
        }
        private TrackableCollection<PublishedReferentialAction> _publishedReferentialActions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public CutVideo CutVideo
        {
            get { return _cutVideo; }
            set
            {
                if (!ReferenceEquals(_cutVideo, value))
                {
                    var previousValue = _cutVideo;
                    _cutVideo = value;
                    FixupCutVideo(previousValue);
                    OnNavigationPropertyChanged("CutVideo");
                }
            }
        }
        private CutVideo _cutVideo;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public PublishedFile Thumbnail
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
        private PublishedFile _thumbnail;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedAction> Successors
        {
            get
            {
                if (_successors == null)
                {
                    _successors = new TrackableCollection<PublishedAction>();
                    _successors.CollectionChanged += FixupSuccessors;
                }
                return _successors;
            }
            set
            {
                if (!ReferenceEquals(_successors, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_successors != null)
                    {
                        _successors.CollectionChanged -= FixupSuccessors;
                    }
                    _successors = value;
                    if (_successors != null)
                    {
                        _successors.CollectionChanged += FixupSuccessors;
                    }
                    OnNavigationPropertyChanged("Successors");
                }
            }
        }
        private TrackableCollection<PublishedAction> _successors;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublishedAction> Predecessors
        {
            get
            {
                if (_predecessors == null)
                {
                    _predecessors = new TrackableCollection<PublishedAction>();
                    _predecessors.CollectionChanged += FixupPredecessors;
                }
                return _predecessors;
            }
            set
            {
                if (!ReferenceEquals(_predecessors, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_predecessors != null)
                    {
                        _predecessors.CollectionChanged -= FixupPredecessors;
                    }
                    _predecessors = value;
                    if (_predecessors != null)
                    {
                        _predecessors.CollectionChanged += FixupPredecessors;
                    }
                    OnNavigationPropertyChanged("Predecessors");
                }
            }
        }
        private TrackableCollection<PublishedAction> _predecessors;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ValidationTraining> ValidationTrainings
        {
            get
            {
                if (_validationTrainings == null)
                {
                    _validationTrainings = new TrackableCollection<ValidationTraining>();
                    _validationTrainings.CollectionChanged += FixupValidationTrainings;
                }
                return _validationTrainings;
            }
            set
            {
                if (!ReferenceEquals(_validationTrainings, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_validationTrainings != null)
                    {
                        _validationTrainings.CollectionChanged -= FixupValidationTrainings;
                    }
                    _validationTrainings = value;
                    if (_validationTrainings != null)
                    {
                        _validationTrainings.CollectionChanged += FixupValidationTrainings;
                    }
                    OnNavigationPropertyChanged("ValidationTrainings");
                }
            }
        }
        private TrackableCollection<ValidationTraining> _validationTrainings;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<QualificationStep> QualificationSteps
        {
            get
            {
                if (_qualificationSteps == null)
                {
                    _qualificationSteps = new TrackableCollection<QualificationStep>();
                    _qualificationSteps.CollectionChanged += FixupQualificationSteps;
                }
                return _qualificationSteps;
            }
            set
            {
                if (!ReferenceEquals(_qualificationSteps, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_qualificationSteps != null)
                    {
                        _qualificationSteps.CollectionChanged -= FixupQualificationSteps;
                    }
                    _qualificationSteps = value;
                    if (_qualificationSteps != null)
                    {
                        _qualificationSteps.CollectionChanged += FixupQualificationSteps;
                    }
                    OnNavigationPropertyChanged("QualificationSteps");
                }
            }
        }
        private TrackableCollection<QualificationStep> _qualificationSteps;
    
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
        public Publication LinkedPublication
        {
            get { return _linkedPublication; }
            set
            {
                if (!ReferenceEquals(_linkedPublication, value))
                {
                    var previousValue = _linkedPublication;
                    _linkedPublication = value;
                    FixupLinkedPublication(previousValue);
                    OnNavigationPropertyChanged("LinkedPublication");
                }
            }
        }
        private Publication _linkedPublication;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<InspectionStep> InspectionSteps
        {
            get
            {
                if (_inspectionSteps == null)
                {
                    _inspectionSteps = new TrackableCollection<InspectionStep>();
                    _inspectionSteps.CollectionChanged += FixupInspectionSteps;
                }
                return _inspectionSteps;
            }
            set
            {
                if (!ReferenceEquals(_inspectionSteps, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_inspectionSteps != null)
                    {
                        _inspectionSteps.CollectionChanged -= FixupInspectionSteps;
                    }
                    _inspectionSteps = value;
                    if (_inspectionSteps != null)
                    {
                        _inspectionSteps.CollectionChanged += FixupInspectionSteps;
                    }
                    OnNavigationPropertyChanged("InspectionSteps");
                }
            }
        }
        private TrackableCollection<InspectionStep> _inspectionSteps;

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
            Publication = null;
            PublishedActionCategory = null;
            PublishedResource = null;
            PublishedReferentialActions.Clear();
            CutVideo = null;
            Thumbnail = null;
            Successors.Clear();
            Predecessors.Clear();
            ValidationTrainings.Clear();
            QualificationSteps.Clear();
            Skill = null;
            LinkedPublication = null;
            InspectionSteps.Clear();
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
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (Publication != null)
            {
                if (!Publication.PublishedActions.Contains(this))
                {
                    Publication.PublishedActions.Add(this);
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
        /// Corrige l'état de la propriété de navigation PublishedActionCategory.
        /// </summary>
        private void FixupPublishedActionCategory(PublishedActionCategory previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (PublishedActionCategory != null)
            {
                if (!PublishedActionCategory.PublishedActions.Contains(this))
                {
                    PublishedActionCategory.PublishedActions.Add(this);
                }
    
                ActionCategoryId = PublishedActionCategory.PublishedActionCategoryId;
            }
            else if (!skipKeys)
            {
                ActionCategoryId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PublishedActionCategory", previousValue, PublishedActionCategory);
                if (PublishedActionCategory != null && !PublishedActionCategory.ChangeTracker.ChangeTrackingEnabled)
                {
                    PublishedActionCategory.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation PublishedResource.
        /// </summary>
        private void FixupPublishedResource(PublishedResource previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (PublishedResource != null)
            {
                if (!PublishedResource.PublishedActions.Contains(this))
                {
                    PublishedResource.PublishedActions.Add(this);
                }
    
                ResourceId = PublishedResource.PublishedResourceId;
            }
            else if (!skipKeys)
            {
                ResourceId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("PublishedResource", previousValue, PublishedResource);
                if (PublishedResource != null && !PublishedResource.ChangeTracker.ChangeTrackingEnabled)
                {
                    PublishedResource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation CutVideo.
        /// </summary>
        private void FixupCutVideo(CutVideo previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (CutVideo != null)
            {
                if (!CutVideo.PublishedActions.Contains(this))
                {
                    CutVideo.PublishedActions.Add(this);
                }
    
                CutVideoHash = CutVideo.Hash;
            }
            else if (!skipKeys)
            {
                CutVideoHash = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("CutVideo", previousValue, CutVideo);
                if (CutVideo != null && !CutVideo.ChangeTracker.ChangeTrackingEnabled)
                {
                    CutVideo.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Thumbnail.
        /// </summary>
        private void FixupThumbnail(PublishedFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (Thumbnail != null)
            {
                if (!Thumbnail.PublishedActions.Contains(this))
                {
                    Thumbnail.PublishedActions.Add(this);
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
        /// Corrige l'état de la propriété de navigation Skill.
        /// </summary>
        private void FixupSkill(Skill previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.PublishedActions.Contains(this))
            {
                previousValue.PublishedActions.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.PublishedActions.Contains(this))
                {
                    Skill.PublishedActions.Add(this);
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
        /// Corrige l'état de la propriété de navigation LinkedPublication.
        /// </summary>
        private void FixupLinkedPublication(Publication previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LinkedPublishedActions.Contains(this))
            {
                previousValue.LinkedPublishedActions.Remove(this);
            }
    
            if (LinkedPublication != null)
            {
                if (!LinkedPublication.LinkedPublishedActions.Contains(this))
                {
                    LinkedPublication.LinkedPublishedActions.Add(this);
                }
    
                LinkedPublicationId = LinkedPublication.PublicationId;
            }
            else if (!skipKeys)
            {
                LinkedPublicationId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LinkedPublication", previousValue, LinkedPublication);
                if (LinkedPublication != null && !LinkedPublication.ChangeTracker.ChangeTrackingEnabled)
                {
                    LinkedPublication.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété PublishedReferentialActions.
        /// </summary>
        private void FixupPublishedReferentialActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedReferentialAction item in e.NewItems)
                {
                    item.PublishedAction = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublishedReferentialActions", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedReferentialAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.PublishedAction, this))
                    {
                        item.PublishedAction = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublishedReferentialActions", item);
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
    
        /// <summary>
        /// Corrige l'état de la propriété Successors.
        /// </summary>
        private void FixupSuccessors(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedAction item in e.NewItems)
                {
                    if (!item.Predecessors.Contains(this))
                    {
                        item.Predecessors.Add(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Successors", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedAction item in e.OldItems)
                {
                    if (item.Predecessors.Contains(this))
                    {
                        item.Predecessors.Remove(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Successors", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Predecessors.
        /// </summary>
        private void FixupPredecessors(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublishedAction item in e.NewItems)
                {
                    if (!item.Successors.Contains(this))
                    {
                        item.Successors.Add(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Predecessors", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublishedAction item in e.OldItems)
                {
                    if (item.Successors.Contains(this))
                    {
                        item.Successors.Remove(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Predecessors", item);
    		            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
    		            {
    		                ChangeTracker.State = ObjectState.Modified;
    		                NotifyMarkedAsPropertyChanged();
    		            }
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ValidationTrainings.
        /// </summary>
        private void FixupValidationTrainings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ValidationTraining item in e.NewItems)
                {
                    item.PublishedAction = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ValidationTrainings", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ValidationTraining item in e.OldItems)
                {
                    if (ReferenceEquals(item.PublishedAction, this))
                    {
                        item.PublishedAction = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ValidationTrainings", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété QualificationSteps.
        /// </summary>
        private void FixupQualificationSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (QualificationStep item in e.NewItems)
                {
                    item.PublishedAction = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("QualificationSteps", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (QualificationStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.PublishedAction, this))
                    {
                        item.PublishedAction = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("QualificationSteps", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété InspectionSteps.
        /// </summary>
        private void FixupInspectionSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (InspectionStep item in e.NewItems)
                {
                    item.PublishedAction = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("InspectionSteps", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (InspectionStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.PublishedAction, this))
                    {
                        item.PublishedAction = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionSteps", item);
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
    			case "PublishedActionId":
    				this.PublishedActionId = Convert.ToInt32(value);
    				break;
    			case "PublicationId":
    				this.PublicationId = (System.Guid)value;
    				break;
    			case "ResourceId":
    				this.ResourceId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "ActionCategoryId":
    				this.ActionCategoryId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "WBS":
    				this.WBS = (string)value;
    				break;
    			case "Label":
    				this.Label = (string)value;
    				break;
    			case "Start":
    				this.Start = (long)value;
    				break;
    			case "Finish":
    				this.Finish = (long)value;
    				break;
    			case "BuildStart":
    				this.BuildStart = (long)value;
    				break;
    			case "BuildFinish":
    				this.BuildFinish = (long)value;
    				break;
    			case "IsRandom":
    				this.IsRandom = (bool)value;
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
    			case "DifferenceReason":
    				this.DifferenceReason = (string)value;
    				break;
    			case "ThumbnailHash":
    				this.ThumbnailHash = (string)value;
    				break;
    			case "CutVideoHash":
    				this.CutVideoHash = (string)value;
    				break;
    			case "IsKeyTask":
    				this.IsKeyTask = (bool)value;
    				break;
    			case "SkillId":
    				this.SkillId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "LinkedPublicationId":
    				this.LinkedPublicationId = (Nullable<System.Guid>)value;
    				break;
    			case "Publication":
    				this.Publication = (Publication)value;
    				break;
    			case "PublishedActionCategory":
    				this.PublishedActionCategory = (PublishedActionCategory)value;
    				break;
    			case "PublishedResource":
    				this.PublishedResource = (PublishedResource)value;
    				break;
    			case "CutVideo":
    				this.CutVideo = (CutVideo)value;
    				break;
    			case "Thumbnail":
    				this.Thumbnail = (PublishedFile)value;
    				break;
    			case "Skill":
    				this.Skill = (Skill)value;
    				break;
    			case "LinkedPublication":
    				this.LinkedPublication = (Publication)value;
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
    			case "PublishedReferentialActions":
    				this.PublishedReferentialActions.Add((PublishedReferentialAction)value);
    				break;
    			case "Successors":
    				this.Successors.Add((PublishedAction)value);
    				break;
    			case "Predecessors":
    				this.Predecessors.Add((PublishedAction)value);
    				break;
    			case "ValidationTrainings":
    				this.ValidationTrainings.Add((ValidationTraining)value);
    				break;
    			case "QualificationSteps":
    				this.QualificationSteps.Add((QualificationStep)value);
    				break;
    			case "InspectionSteps":
    				this.InspectionSteps.Add((InspectionStep)value);
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
    			case "PublishedReferentialActions":
    				this.PublishedReferentialActions.Remove((PublishedReferentialAction)value);
    				break;
    			case "Successors":
    				this.Successors.Remove((PublishedAction)value);
    				break;
    			case "Predecessors":
    				this.Predecessors.Remove((PublishedAction)value);
    				break;
    			case "ValidationTrainings":
    				this.ValidationTrainings.Remove((ValidationTraining)value);
    				break;
    			case "QualificationSteps":
    				this.QualificationSteps.Remove((QualificationStep)value);
    				break;
    			case "InspectionSteps":
    				this.InspectionSteps.Remove((InspectionStep)value);
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
    		values.Add("PublishedActionId", this.PublishedActionId);
    		values.Add("PublicationId", this.PublicationId);
    		values.Add("ResourceId", this.ResourceId);
    		values.Add("ActionCategoryId", this.ActionCategoryId);
    		values.Add("WBS", this.WBS);
    		values.Add("Label", this.Label);
    		values.Add("Start", this.Start);
    		values.Add("Finish", this.Finish);
    		values.Add("BuildStart", this.BuildStart);
    		values.Add("BuildFinish", this.BuildFinish);
    		values.Add("IsRandom", this.IsRandom);
    		values.Add("CustomNumericValue", this.CustomNumericValue);
    		values.Add("CustomNumericValue2", this.CustomNumericValue2);
    		values.Add("CustomNumericValue3", this.CustomNumericValue3);
    		values.Add("CustomNumericValue4", this.CustomNumericValue4);
    		values.Add("CustomTextValue", this.CustomTextValue);
    		values.Add("CustomTextValue2", this.CustomTextValue2);
    		values.Add("CustomTextValue3", this.CustomTextValue3);
    		values.Add("CustomTextValue4", this.CustomTextValue4);
    		values.Add("DifferenceReason", this.DifferenceReason);
    		values.Add("ThumbnailHash", this.ThumbnailHash);
    		values.Add("CutVideoHash", this.CutVideoHash);
    		values.Add("IsKeyTask", this.IsKeyTask);
    		values.Add("SkillId", this.SkillId);
    		values.Add("LinkedPublicationId", this.LinkedPublicationId);
    		values.Add("Publication", this.Publication);
    		values.Add("PublishedActionCategory", this.PublishedActionCategory);
    		values.Add("PublishedResource", this.PublishedResource);
    		values.Add("CutVideo", this.CutVideo);
    		values.Add("Thumbnail", this.Thumbnail);
    		values.Add("Skill", this.Skill);
    		values.Add("LinkedPublication", this.LinkedPublication);
    
    		values.Add("PublishedReferentialActions", GetHashCodes(this.PublishedReferentialActions));
    		values.Add("Successors", GetHashCodes(this.Successors));
    		values.Add("Predecessors", GetHashCodes(this.Predecessors));
    		values.Add("ValidationTrainings", GetHashCodes(this.ValidationTrainings));
    		values.Add("QualificationSteps", GetHashCodes(this.QualificationSteps));
    		values.Add("InspectionSteps", GetHashCodes(this.InspectionSteps));
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
    		values.Add("PublishedActionCategory", this.PublishedActionCategory);
    		values.Add("PublishedResource", this.PublishedResource);
    		values.Add("CutVideo", this.CutVideo);
    		values.Add("Thumbnail", this.Thumbnail);
    		values.Add("Skill", this.Skill);
    		values.Add("LinkedPublication", this.LinkedPublication);
    
    		values.Add("PublishedReferentialActions", this.PublishedReferentialActions);
    		values.Add("Successors", this.Successors);
    		values.Add("Predecessors", this.Predecessors);
    		values.Add("ValidationTrainings", this.ValidationTrainings);
    		values.Add("QualificationSteps", this.QualificationSteps);
    		values.Add("InspectionSteps", this.InspectionSteps);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
