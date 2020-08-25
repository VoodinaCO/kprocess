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
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(KAction))]
    [KnownType(typeof(Resource))]
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(KActionReduced))]
    [KnownType(typeof(Ref2Action))]
    [KnownType(typeof(Ref4Action))]
    [KnownType(typeof(Ref5Action))]
    [KnownType(typeof(Ref6Action))]
    [KnownType(typeof(Ref7Action))]
    [KnownType(typeof(Ref3Action))]
    [KnownType(typeof(Ref1Action))]
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(Skill))]
    [KnownType(typeof(CloudFile))]
    [KnownType(typeof(DocumentationActionDraftWBS))]
    /// <summary>
    /// 
    /// </summary>
    public partial class KAction : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.KAction";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KAction"/>.
        /// </summary>
    	public KAction()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _actionId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'action.
        /// </summary>
        [DataMember]
        public int ActionId
        {
            get { return _actionId; }
            set
            {
                if (_actionId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'ActionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _actionId = value;
                    OnEntityPropertyChanged("ActionId");
                }
            }
        }
        
        private int _scenarioId;
        /// <summary>
        /// Obtient ou définit l'identifiant du scénario.
        /// </summary>
        [DataMember]
        public int ScenarioId
        {
            get { return _scenarioId; }
            set
            {
                if (_scenarioId != value)
                {
                    ChangeTracker.RecordValue("ScenarioId", _scenarioId, value);
                    if (!IsDeserializing)
                    {
                        if (Scenario != null && Scenario.ScenarioId != value)
                        {
                            Scenario = null;
                        }
                    }
                    _scenarioId = value;
                    OnEntityPropertyChanged("ScenarioId");
                }
            }
        }
        
        private Nullable<int> _resourceId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la ressource.
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
                        if (Resource != null && Resource.ResourceId != value)
                        {
                            Resource = null;
                        }
                    }
                    _resourceId = value;
                    OnEntityPropertyChanged("ResourceId");
                }
            }
        }
        
        private Nullable<int> _categoryId;
        /// <summary>
        /// Obtient ou définit l'identifiant de catégorie.
        /// </summary>
        [DataMember]
        public Nullable<int> CategoryId
        {
            get { return _categoryId; }
            set
            {
                if (_categoryId != value)
                {
                    ChangeTracker.RecordValue("CategoryId", _categoryId, value);
                    if (!IsDeserializing)
                    {
                        if (Category != null && Category.ActionCategoryId != value)
                        {
                            Category = null;
                        }
                    }
                    _categoryId = value;
                    OnEntityPropertyChanged("CategoryId");
                }
            }
        }
        
        private Nullable<int> _originalActionId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'action d'origine.
        /// </summary>
        [DataMember]
        public Nullable<int> OriginalActionId
        {
            get { return _originalActionId; }
            set
            {
                if (_originalActionId != value)
                {
                    ChangeTracker.RecordValue("OriginalActionId", _originalActionId, value);
                    if (!IsDeserializing)
                    {
                        if (Original != null && Original.ActionId != value)
                        {
                            Original = null;
                        }
                    }
                    _originalActionId = value;
                    OnEntityPropertyChanged("OriginalActionId");
                }
            }
        }
        
        private Nullable<int> _videoId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la vidéo.
        /// </summary>
        [DataMember]
        public Nullable<int> VideoId
        {
            get { return _videoId; }
            set
            {
                if (_videoId != value)
                {
                    ChangeTracker.RecordValue("VideoId", _videoId, value);
                    if (!IsDeserializing)
                    {
                        if (Video != null && Video.VideoId != value)
                        {
                            Video = null;
                        }
                    }
                    _videoId = value;
                    OnEntityPropertyChanged("VideoId");
                }
            }
        }
        
        private string _wBS;
        /// <summary>
        /// Obtient ou définit le WBS.
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
    				var oldValue = _wBS;
                    _wBS = value;
                    OnEntityPropertyChanged("WBS");
    				OnWBSChanged(oldValue, value);
    				OnWBSChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="WBS"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnWBSChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> WBSChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="WBS"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnWBSChanged(string oldValue, string newValue)
    	{
    		if (WBSChanged != null)
    			WBSChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private string _label;
        /// <summary>
        /// Obtient ou définit le libellé.
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
    				var oldValue = _label;
                    _label = value;
                    OnEntityPropertyChanged("Label");
    				OnLabelChanged(oldValue, value);
    				OnLabelChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Label"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnLabelChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> LabelChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Label"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnLabelChanged(string oldValue, string newValue)
    	{
    		if (LabelChanged != null)
    			LabelChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private long _start;
        /// <summary>
        /// Obtient ou définit le temps de début sur la vidéo associée.
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
    				var oldValue = _start;
                    _start = value;
                    OnEntityPropertyChanged("Start");
    				OnStartChanged(oldValue, value);
    				OnStartChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Start"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnStartChangedPartial(long oldValue, long newValue);
    	public event EventHandler<PropertyChangedEventArgs<long>> StartChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Start"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnStartChanged(long oldValue, long newValue)
    	{
    		if (StartChanged != null)
    			StartChanged(this, new PropertyChangedEventArgs<long>(oldValue, newValue));
    	}
        
        private long _finish;
        /// <summary>
        /// Obtient ou définit le temps de fin sur la vidéo associée.
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
    				var oldValue = _finish;
                    _finish = value;
                    OnEntityPropertyChanged("Finish");
    				OnFinishChanged(oldValue, value);
    				OnFinishChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Finish"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnFinishChangedPartial(long oldValue, long newValue);
    	public event EventHandler<PropertyChangedEventArgs<long>> FinishChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Finish"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnFinishChanged(long oldValue, long newValue)
    	{
    		if (FinishChanged != null)
    			FinishChanged(this, new PropertyChangedEventArgs<long>(oldValue, newValue));
    	}
        
        private long _buildStart;
        /// <summary>
        /// Obtient ou définit le temps de début sur le process.
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
    				var oldValue = _buildStart;
                    _buildStart = value;
                    OnEntityPropertyChanged("BuildStart");
    				OnBuildStartChanged(oldValue, value);
    				OnBuildStartChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildStart"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnBuildStartChangedPartial(long oldValue, long newValue);
    	public event EventHandler<PropertyChangedEventArgs<long>> BuildStartChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildStart"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnBuildStartChanged(long oldValue, long newValue)
    	{
    		if (BuildStartChanged != null)
    			BuildStartChanged(this, new PropertyChangedEventArgs<long>(oldValue, newValue));
    	}
        
        private long _buildFinish;
        /// <summary>
        /// Obtient ou définit le temps de fin sur le process.
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
    				var oldValue = _buildFinish;
                    _buildFinish = value;
                    OnEntityPropertyChanged("BuildFinish");
    				OnBuildFinishChanged(oldValue, value);
    				OnBuildFinishChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildFinish"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnBuildFinishChangedPartial(long oldValue, long newValue);
    	public event EventHandler<PropertyChangedEventArgs<long>> BuildFinishChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="BuildFinish"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnBuildFinishChanged(long oldValue, long newValue)
    	{
    		if (BuildFinishChanged != null)
    			BuildFinishChanged(this, new PropertyChangedEventArgs<long>(oldValue, newValue));
    	}
        
        private bool _isRandom;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action est aléatoire.
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
        
        private Nullable<double> _customNumericValue;
        /// <summary>
        /// Obtient ou définit une valeur numérique personnalisée n° 1.
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
        /// Obtient ou définit une valeur numérique personnalisée n° 2.
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
        /// Obtient ou définit une valeur numérique personnalisée n° 3.
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
        /// Obtient ou définit une valeur numérique personnalisée n° 4.
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
        /// Obtient ou définit un texte personnalisé n° 1.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextValueMaxLength, ErrorMessageResourceName = "Validation_KAction_CustomTextValue_StringLength")]
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
        /// Obtient ou définit un texte personnalisé n° 2.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextValue2MaxLength, ErrorMessageResourceName = "Validation_KAction_CustomTextValue2_StringLength")]
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
        /// Obtient ou définit un texte personnalisé n° 3.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextValue3MaxLength, ErrorMessageResourceName = "Validation_KAction_CustomTextValue3_StringLength")]
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
        /// Obtient ou définit un texte personnalisé n° 4.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CustomTextValue4MaxLength, ErrorMessageResourceName = "Validation_KAction_CustomTextValue4_StringLength")]
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
        /// Obtient ou définit la cause des écarts.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(DifferenceReasonMaxLength, ErrorMessageResourceName = "Validation_KAction_DifferenceReason_StringLength")]
        public string DifferenceReason
        {
            get { return _differenceReason; }
            set
            {
                if (_differenceReason != value)
                {
                    ChangeTracker.RecordValue("DifferenceReason", _differenceReason, value);
    				var oldValue = _differenceReason;
                    _differenceReason = value;
                    OnEntityPropertyChanged("DifferenceReason");
    				OnDifferenceReasonChanged(oldValue, value);
    				OnDifferenceReasonChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DifferenceReason"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnDifferenceReasonChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> DifferenceReasonChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DifferenceReason"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnDifferenceReasonChanged(string oldValue, string newValue)
    	{
    		if (DifferenceReasonChanged != null)
    			DifferenceReasonChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private string _thumbnailHash;
        /// <summary>
        /// Obtient ou définit le hash de la vignette
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
    				var oldValue = _thumbnailHash;
                    _thumbnailHash = value;
                    OnEntityPropertyChanged("ThumbnailHash");
    				OnThumbnailHashChanged(oldValue, value);
    				OnThumbnailHashChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ThumbnailHash"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnThumbnailHashChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> ThumbnailHashChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ThumbnailHash"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnThumbnailHashChanged(string oldValue, string newValue)
    	{
    		if (ThumbnailHashChanged != null)
    			ThumbnailHashChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private bool _isThumbnailSpecific;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la vignette a été définie spécifiquement.
        /// </summary>
        [DataMember]
        public bool IsThumbnailSpecific
        {
            get { return _isThumbnailSpecific; }
            set
            {
                if (_isThumbnailSpecific != value)
                {
                    ChangeTracker.RecordValue("IsThumbnailSpecific", _isThumbnailSpecific, value);
                    _isThumbnailSpecific = value;
                    OnEntityPropertyChanged("IsThumbnailSpecific");
                }
            }
        }
        
        private Nullable<long> _thumbnailPosition;
        /// <summary>
        /// Obtient ou définit la position dans la vidéo à laquelle la vignette a été prise.
        /// </summary>
        [DataMember]
        public Nullable<long> ThumbnailPosition
        {
            get { return _thumbnailPosition; }
            set
            {
                if (_thumbnailPosition != value)
                {
                    ChangeTracker.RecordValue("ThumbnailPosition", _thumbnailPosition, value);
                    _thumbnailPosition = value;
                    OnEntityPropertyChanged("ThumbnailPosition");
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
        
        private Nullable<int> _linkedProcessId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> LinkedProcessId
        {
            get { return _linkedProcessId; }
            set
            {
                if (_linkedProcessId != value)
                {
                    ChangeTracker.RecordValue("LinkedProcessId", _linkedProcessId, value);
                    if (!IsDeserializing)
                    {
                        if (LinkedProcess != null && LinkedProcess.ProcessId != value)
                        {
                            LinkedProcess = null;
                        }
                    }
                    _linkedProcessId = value;
                    OnEntityPropertyChanged("LinkedProcessId");
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

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// Obtient ou définit la catégorie associée.
        /// </summary>
        [DataMember]
        public ActionCategory Category
        {
            get { return _category; }
            set
            {
                if (!ReferenceEquals(_category, value))
                {
                    var previousValue = _category;
                    _category = value;
                    FixupCategory(previousValue);
                    OnNavigationPropertyChanged("Category");
    				OnCategoryChanged(previousValue, value);
    				OnCategoryChangedPartial(previousValue, value);
                }
            }
        }
        private ActionCategory _category;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Category"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnCategoryChangedPartial(ActionCategory oldValue, ActionCategory newValue);
    	public event EventHandler<PropertyChangedEventArgs<ActionCategory>> CategoryChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Category"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnCategoryChanged(ActionCategory oldValue, ActionCategory newValue)
    	{
    		if (CategoryChanged != null)
    			CategoryChanged(this, new PropertyChangedEventArgs<ActionCategory>(oldValue, newValue));
    	}
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> ActionsReduced
        {
            get
            {
                if (_actionsReduced == null)
                {
                    _actionsReduced = new TrackableCollection<KAction>();
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
        private TrackableCollection<KAction> _actionsReduced;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public KAction Original
        {
            get { return _original; }
            set
            {
                if (!ReferenceEquals(_original, value))
                {
                    var previousValue = _original;
                    _original = value;
                    FixupOriginal(previousValue);
                    OnNavigationPropertyChanged("Original");
                }
            }
        }
        private KAction _original;
    				
    
        /// <summary>
        /// Obtient ou définit la ressource qui effectue l'action.
        /// </summary>
        [DataMember]
        public Resource Resource
        {
            get { return _resource; }
            set
            {
                if (!ReferenceEquals(_resource, value))
                {
                    var previousValue = _resource;
                    _resource = value;
                    FixupResource(previousValue);
                    OnNavigationPropertyChanged("Resource");
    				OnResourceChanged(previousValue, value);
    				OnResourceChangedPartial(previousValue, value);
                }
            }
        }
        private Resource _resource;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Resource"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnResourceChangedPartial(Resource oldValue, Resource newValue);
    	public event EventHandler<PropertyChangedEventArgs<Resource>> ResourceChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Resource"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnResourceChanged(Resource oldValue, Resource newValue)
    	{
    		if (ResourceChanged != null)
    			ResourceChanged(this, new PropertyChangedEventArgs<Resource>(oldValue, newValue));
    	}
    
        /// <summary>
        /// Obtient ou définit le scénario associé.
        /// </summary>
        [DataMember]
        public Scenario Scenario
        {
            get { return _scenario; }
            set
            {
                if (!ReferenceEquals(_scenario, value))
                {
                    var previousValue = _scenario;
                    _scenario = value;
                    FixupScenario(previousValue);
                    OnNavigationPropertyChanged("Scenario");
                }
            }
        }
        private Scenario _scenario;
    				
    
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
        /// Obtient ou définit la vidéo qui illustre l'action.
        /// </summary>
        [DataMember]
        public Video Video
        {
            get { return _video; }
            set
            {
                if (!ReferenceEquals(_video, value))
                {
                    var previousValue = _video;
                    _video = value;
                    FixupVideo(previousValue);
                    OnNavigationPropertyChanged("Video");
    				OnVideoChanged(previousValue, value);
    				OnVideoChangedPartial(previousValue, value);
                }
            }
        }
        private Video _video;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Video"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnVideoChangedPartial(Video oldValue, Video newValue);
    	public event EventHandler<PropertyChangedEventArgs<Video>> VideoChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Video"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnVideoChanged(Video oldValue, Video newValue)
    	{
    		if (VideoChanged != null)
    			VideoChanged(this, new PropertyChangedEventArgs<Video>(oldValue, newValue));
    	}
    
        /// <summary>
        /// Obtient ou définit l'action réduite.
        /// </summary>
        [DataMember]
        public KActionReduced Reduced
        {
            get { return _reduced; }
            set
            {
                if (!ReferenceEquals(_reduced, value))
                {
                    var previousValue = _reduced;
                    _reduced = value;
                    FixupReduced(previousValue);
                    OnNavigationPropertyChanged("Reduced");
    				OnReducedChanged(previousValue, value);
    				OnReducedChangedPartial(previousValue, value);
                }
            }
        }
        private KActionReduced _reduced;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Reduced"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnReducedChangedPartial(KActionReduced oldValue, KActionReduced newValue);
    	public event EventHandler<PropertyChangedEventArgs<KActionReduced>> ReducedChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Reduced"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnReducedChanged(KActionReduced oldValue, KActionReduced newValue)
    	{
    		if (ReducedChanged != null)
    			ReducedChanged(this, new PropertyChangedEventArgs<KActionReduced>(oldValue, newValue));
    	}
    
        /// <summary>
        /// Obtient ou définit les consommables liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref2Action> Ref2
        {
            get
            {
                if (_ref2 == null)
                {
                    _ref2 = new TrackableCollection<Ref2Action>();
                    _ref2.CollectionChanged += FixupRef2;
                }
                return _ref2;
            }
            set
            {
                if (!ReferenceEquals(_ref2, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref2 != null)
                    {
                        _ref2.CollectionChanged -= FixupRef2;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref2Action item in _ref2)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref2 = value;
                    if (_ref2 != null)
                    {
                        _ref2.CollectionChanged += FixupRef2;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref2Action item in _ref2)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref2");
                }
            }
        }
        private TrackableCollection<Ref2Action> _ref2;
    
        /// <summary>
        /// Obtient ou définit les documents liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref4Action> Ref4
        {
            get
            {
                if (_ref4 == null)
                {
                    _ref4 = new TrackableCollection<Ref4Action>();
                    _ref4.CollectionChanged += FixupRef4;
                }
                return _ref4;
            }
            set
            {
                if (!ReferenceEquals(_ref4, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref4 != null)
                    {
                        _ref4.CollectionChanged -= FixupRef4;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref4Action item in _ref4)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref4 = value;
                    if (_ref4 != null)
                    {
                        _ref4.CollectionChanged += FixupRef4;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref4Action item in _ref4)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref4");
                }
            }
        }
        private TrackableCollection<Ref4Action> _ref4;
    
        /// <summary>
        /// Obtient ou définit les référentiels supplémentaires 1 liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref5Action> Ref5
        {
            get
            {
                if (_ref5 == null)
                {
                    _ref5 = new TrackableCollection<Ref5Action>();
                    _ref5.CollectionChanged += FixupRef5;
                }
                return _ref5;
            }
            set
            {
                if (!ReferenceEquals(_ref5, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref5 != null)
                    {
                        _ref5.CollectionChanged -= FixupRef5;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref5Action item in _ref5)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref5 = value;
                    if (_ref5 != null)
                    {
                        _ref5.CollectionChanged += FixupRef5;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref5Action item in _ref5)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref5");
                }
            }
        }
        private TrackableCollection<Ref5Action> _ref5;
    
        /// <summary>
        /// Obtient ou définit les référentiels supplémentaires 2 liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref6Action> Ref6
        {
            get
            {
                if (_ref6 == null)
                {
                    _ref6 = new TrackableCollection<Ref6Action>();
                    _ref6.CollectionChanged += FixupRef6;
                }
                return _ref6;
            }
            set
            {
                if (!ReferenceEquals(_ref6, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref6 != null)
                    {
                        _ref6.CollectionChanged -= FixupRef6;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref6Action item in _ref6)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref6 = value;
                    if (_ref6 != null)
                    {
                        _ref6.CollectionChanged += FixupRef6;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref6Action item in _ref6)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref6");
                }
            }
        }
        private TrackableCollection<Ref6Action> _ref6;
    
        /// <summary>
        /// Obtient ou définit les référentiels supplémentaires 3 liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref7Action> Ref7
        {
            get
            {
                if (_ref7 == null)
                {
                    _ref7 = new TrackableCollection<Ref7Action>();
                    _ref7.CollectionChanged += FixupRef7;
                }
                return _ref7;
            }
            set
            {
                if (!ReferenceEquals(_ref7, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref7 != null)
                    {
                        _ref7.CollectionChanged -= FixupRef7;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref7Action item in _ref7)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref7 = value;
                    if (_ref7 != null)
                    {
                        _ref7.CollectionChanged += FixupRef7;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref7Action item in _ref7)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref7");
                }
            }
        }
        private TrackableCollection<Ref7Action> _ref7;
    
        /// <summary>
        /// Obtient ou définit les lieux liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref3Action> Ref3
        {
            get
            {
                if (_ref3 == null)
                {
                    _ref3 = new TrackableCollection<Ref3Action>();
                    _ref3.CollectionChanged += FixupRef3;
                }
                return _ref3;
            }
            set
            {
                if (!ReferenceEquals(_ref3, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref3 != null)
                    {
                        _ref3.CollectionChanged -= FixupRef3;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref3Action item in _ref3)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref3 = value;
                    if (_ref3 != null)
                    {
                        _ref3.CollectionChanged += FixupRef3;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref3Action item in _ref3)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref3");
                }
            }
        }
        private TrackableCollection<Ref3Action> _ref3;
    
        /// <summary>
        /// Obtient ou définit les outils liés à l'action.
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref1Action> Ref1
        {
            get
            {
                if (_ref1 == null)
                {
                    _ref1 = new TrackableCollection<Ref1Action>();
                    _ref1.CollectionChanged += FixupRef1;
                }
                return _ref1;
            }
            set
            {
                if (!ReferenceEquals(_ref1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_ref1 != null)
                    {
                        _ref1.CollectionChanged -= FixupRef1;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (Ref1Action item in _ref1)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _ref1 = value;
                    if (_ref1 != null)
                    {
                        _ref1.CollectionChanged += FixupRef1;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (Ref1Action item in _ref1)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("Ref1");
                }
            }
        }
        private TrackableCollection<Ref1Action> _ref1;
    
        /// <summary>
        /// Obtient ou définit les successeurs.
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> Successors
        {
            get
            {
                if (_successors == null)
                {
                    _successors = new TrackableCollection<KAction>();
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
        private TrackableCollection<KAction> _successors;
    
        /// <summary>
        /// Obtient ou définit les prédécesseurs.
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> Predecessors
        {
            get
            {
                if (_predecessors == null)
                {
                    _predecessors = new TrackableCollection<KAction>();
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
        private TrackableCollection<KAction> _predecessors;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Procedure LinkedProcess
        {
            get { return _linkedProcess; }
            set
            {
                if (!ReferenceEquals(_linkedProcess, value))
                {
                    var previousValue = _linkedProcess;
                    _linkedProcess = value;
                    FixupLinkedProcess(previousValue);
                    OnNavigationPropertyChanged("LinkedProcess");
                }
            }
        }
        private Procedure _linkedProcess;
    				
    
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
            Category = null;
            ActionsReduced.Clear();
            Original = null;
            Resource = null;
            Scenario = null;
            Creator = null;
            LastModifier = null;
            Video = null;
            Reduced = null;
            Ref2.Clear();
            Ref4.Clear();
            Ref5.Clear();
            Ref6.Clear();
            Ref7.Clear();
            Ref3.Clear();
            Ref1.Clear();
            Successors.Clear();
            Predecessors.Clear();
            LinkedProcess = null;
            Skill = null;
            Thumbnail = null;
            DocumentationActionDraftWBS.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Category.
        /// </summary>
        private void FixupCategory(ActionCategory previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Category != null)
            {
                if (!Category.Actions.Contains(this))
                {
                    Category.Actions.Add(this);
                }
    
                CategoryId = Category.ActionCategoryId;
            }
            else if (!skipKeys)
            {
                CategoryId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Category", previousValue, Category);
                if (Category != null && !Category.ChangeTracker.ChangeTrackingEnabled)
                {
                    Category.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Original.
        /// </summary>
        private void FixupOriginal(KAction previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionsReduced.Contains(this))
            {
                previousValue.ActionsReduced.Remove(this);
            }
    
            if (Original != null)
            {
                if (!Original.ActionsReduced.Contains(this))
                {
                    Original.ActionsReduced.Add(this);
                }
    
                OriginalActionId = Original.ActionId;
            }
            else if (!skipKeys)
            {
                OriginalActionId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Original", previousValue, Original);
                if (Original != null && !Original.ChangeTracker.ChangeTrackingEnabled)
                {
                    Original.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Resource.
        /// </summary>
        private void FixupResource(Resource previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Resource != null)
            {
                if (!Resource.Actions.Contains(this))
                {
                    Resource.Actions.Add(this);
                }
    
                ResourceId = Resource.ResourceId;
            }
            else if (!skipKeys)
            {
                ResourceId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Resource", previousValue, Resource);
                if (Resource != null && !Resource.ChangeTracker.ChangeTrackingEnabled)
                {
                    Resource.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Scenario.
        /// </summary>
        private void FixupScenario(Scenario previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Scenario != null)
            {
                if (!Scenario.Actions.Contains(this))
                {
                    Scenario.Actions.Add(this);
                }
    
                ScenarioId = Scenario.ScenarioId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Scenario", previousValue, Scenario);
                if (Scenario != null && !Scenario.ChangeTracker.ChangeTrackingEnabled)
                {
                    Scenario.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Creator.
        /// </summary>
        private void FixupCreator(User previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CreatedActions.Contains(this))
            {
                previousValue.CreatedActions.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedActions.Contains(this))
                {
                    Creator.CreatedActions.Add(this);
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
    
            if (previousValue != null && previousValue.LastModifiedActions.Contains(this))
            {
                previousValue.LastModifiedActions.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedActions.Contains(this))
                {
                    LastModifier.LastModifiedActions.Add(this);
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
        /// Corrige l'état de la propriété de navigation Video.
        /// </summary>
        private void FixupVideo(Video previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Video != null)
            {
                if (!Video.Actions.Contains(this))
                {
                    Video.Actions.Add(this);
                }
    
                VideoId = Video.VideoId;
            }
            else if (!skipKeys)
            {
                VideoId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Video", previousValue, Video);
                if (Video != null && !Video.ChangeTracker.ChangeTrackingEnabled)
                {
                    Video.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Reduced.
        /// </summary>
        private void FixupReduced(KActionReduced previousValue)
        {
            // This is the principal end in an association that performs cascade deletes.
            // Update the event listener to refer to the new dependent.
            if (previousValue != null)
            {
                ChangeTracker.ObjectStateChanging -= previousValue.HandleCascadeDelete;
            }
    
            if (Reduced != null)
            {
                ChangeTracker.ObjectStateChanging += Reduced.HandleCascadeDelete;
            }
    
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && ReferenceEquals(previousValue.Action, this))
            {
                previousValue.Action = null;
            }
    
            if (Reduced != null)
            {
                Reduced.Action = this;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Reduced", previousValue, Reduced);
                    // This is the principal end of an identifying association, so the dependent must be deleted when the relationship is removed.
                    // If the current state of the dependent is Added, the relationship can be changed without causing the dependent to be deleted.
                    if (previousValue != null && previousValue.ChangeTracker.State != ObjectState.Added)
                    {
                        previousValue.MarkAsDeleted();
                    }
                if (Reduced != null && !Reduced.ChangeTracker.ChangeTrackingEnabled)
                {
                    Reduced.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation LinkedProcess.
        /// </summary>
        private void FixupLinkedProcess(Procedure previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LinkedActions.Contains(this))
            {
                previousValue.LinkedActions.Remove(this);
            }
    
            if (LinkedProcess != null)
            {
                if (!LinkedProcess.LinkedActions.Contains(this))
                {
                    LinkedProcess.LinkedActions.Add(this);
                }
    
                LinkedProcessId = LinkedProcess.ProcessId;
            }
            else if (!skipKeys)
            {
                LinkedProcessId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("LinkedProcess", previousValue, LinkedProcess);
                if (LinkedProcess != null && !LinkedProcess.ChangeTracker.ChangeTrackingEnabled)
                {
                    LinkedProcess.StartTracking();
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
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Skill != null)
            {
                if (!Skill.Actions.Contains(this))
                {
                    Skill.Actions.Add(this);
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
        /// Corrige l'état de la propriété de navigation Thumbnail.
        /// </summary>
        private void FixupThumbnail(CloudFile previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Actions.Contains(this))
            {
                previousValue.Actions.Remove(this);
            }
    
            if (Thumbnail != null)
            {
                if (!Thumbnail.Actions.Contains(this))
                {
                    Thumbnail.Actions.Add(this);
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
                foreach (KAction item in e.NewItems)
                {
                    item.Original = this;
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
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Original, this))
                    {
                        item.Original = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionsReduced", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Ref2.
        /// </summary>
        private void FixupRef2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref2Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref2", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref2Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref2", item);
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
        /// Corrige l'état de la propriété Ref4.
        /// </summary>
        private void FixupRef4(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref4Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref4", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref4Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref4", item);
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
        /// Corrige l'état de la propriété Ref5.
        /// </summary>
        private void FixupRef5(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref5Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref5", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref5Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref5", item);
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
        /// Corrige l'état de la propriété Ref6.
        /// </summary>
        private void FixupRef6(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref6Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref6", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref6Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref6", item);
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
        /// Corrige l'état de la propriété Ref7.
        /// </summary>
        private void FixupRef7(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref7Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref7", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref7Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref7", item);
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
        /// Corrige l'état de la propriété Ref3.
        /// </summary>
        private void FixupRef3(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref3Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref3", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref3Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref3", item);
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
        /// Corrige l'état de la propriété Ref1.
        /// </summary>
        private void FixupRef1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref1Action item in e.NewItems)
                {
                    item.Action = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref1", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref1Action item in e.OldItems)
                {
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref1", item);
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
                foreach (KAction item in e.NewItems)
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
                foreach (KAction item in e.OldItems)
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
                foreach (KAction item in e.NewItems)
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
                foreach (KAction item in e.OldItems)
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
                    item.Action = this;
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
                    if (ReferenceEquals(item.Action, this))
                    {
                        item.Action = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("DocumentationActionDraftWBS", item);
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
    			case "ActionId":
    				this.ActionId = Convert.ToInt32(value);
    				break;
    			case "ScenarioId":
    				this.ScenarioId = Convert.ToInt32(value);
    				break;
    			case "ResourceId":
    				this.ResourceId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "CategoryId":
    				this.CategoryId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "OriginalActionId":
    				this.OriginalActionId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "VideoId":
    				this.VideoId = value == null ? (int?)null : Convert.ToInt32(value);
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
    			case "IsThumbnailSpecific":
    				this.IsThumbnailSpecific = (bool)value;
    				break;
    			case "ThumbnailPosition":
    				this.ThumbnailPosition = (Nullable<long>)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "IsKeyTask":
    				this.IsKeyTask = (bool)value;
    				break;
    			case "LinkedProcessId":
    				this.LinkedProcessId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "SkillId":
    				this.SkillId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "Category":
    				this.Category = (ActionCategory)value;
    				break;
    			case "Original":
    				this.Original = (KAction)value;
    				break;
    			case "Resource":
    				this.Resource = (Resource)value;
    				break;
    			case "Scenario":
    				this.Scenario = (Scenario)value;
    				break;
    			case "Creator":
    				this.Creator = (User)value;
    				break;
    			case "LastModifier":
    				this.LastModifier = (User)value;
    				break;
    			case "Video":
    				this.Video = (Video)value;
    				break;
    			case "Reduced":
    				this.Reduced = (KActionReduced)value;
    				break;
    			case "LinkedProcess":
    				this.LinkedProcess = (Procedure)value;
    				break;
    			case "Skill":
    				this.Skill = (Skill)value;
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
    			case "ActionsReduced":
    				this.ActionsReduced.Add((KAction)value);
    				break;
    			case "Ref2":
    				this.Ref2.Add((Ref2Action)value);
    				break;
    			case "Ref4":
    				this.Ref4.Add((Ref4Action)value);
    				break;
    			case "Ref5":
    				this.Ref5.Add((Ref5Action)value);
    				break;
    			case "Ref6":
    				this.Ref6.Add((Ref6Action)value);
    				break;
    			case "Ref7":
    				this.Ref7.Add((Ref7Action)value);
    				break;
    			case "Ref3":
    				this.Ref3.Add((Ref3Action)value);
    				break;
    			case "Ref1":
    				this.Ref1.Add((Ref1Action)value);
    				break;
    			case "Successors":
    				this.Successors.Add((KAction)value);
    				break;
    			case "Predecessors":
    				this.Predecessors.Add((KAction)value);
    				break;
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Add((DocumentationActionDraftWBS)value);
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
    				this.ActionsReduced.Remove((KAction)value);
    				break;
    			case "Ref2":
    				this.Ref2.Remove((Ref2Action)value);
    				break;
    			case "Ref4":
    				this.Ref4.Remove((Ref4Action)value);
    				break;
    			case "Ref5":
    				this.Ref5.Remove((Ref5Action)value);
    				break;
    			case "Ref6":
    				this.Ref6.Remove((Ref6Action)value);
    				break;
    			case "Ref7":
    				this.Ref7.Remove((Ref7Action)value);
    				break;
    			case "Ref3":
    				this.Ref3.Remove((Ref3Action)value);
    				break;
    			case "Ref1":
    				this.Ref1.Remove((Ref1Action)value);
    				break;
    			case "Successors":
    				this.Successors.Remove((KAction)value);
    				break;
    			case "Predecessors":
    				this.Predecessors.Remove((KAction)value);
    				break;
    			case "DocumentationActionDraftWBS":
    				this.DocumentationActionDraftWBS.Remove((DocumentationActionDraftWBS)value);
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
    		values.Add("ActionId", this.ActionId);
    		values.Add("ScenarioId", this.ScenarioId);
    		values.Add("ResourceId", this.ResourceId);
    		values.Add("CategoryId", this.CategoryId);
    		values.Add("OriginalActionId", this.OriginalActionId);
    		values.Add("VideoId", this.VideoId);
    		values.Add("WBS", this.WBS);
    		values.Add("Label", this.Label);
    		values.Add("Start", this.Start);
    		values.Add("Finish", this.Finish);
    		values.Add("BuildStart", this.BuildStart);
    		values.Add("BuildFinish", this.BuildFinish);
    		values.Add("IsRandom", this.IsRandom);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
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
    		values.Add("IsThumbnailSpecific", this.IsThumbnailSpecific);
    		values.Add("ThumbnailPosition", this.ThumbnailPosition);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("IsKeyTask", this.IsKeyTask);
    		values.Add("LinkedProcessId", this.LinkedProcessId);
    		values.Add("SkillId", this.SkillId);
    		values.Add("Category", this.Category);
    		values.Add("Original", this.Original);
    		values.Add("Resource", this.Resource);
    		values.Add("Scenario", this.Scenario);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Video", this.Video);
    		values.Add("Reduced", this.Reduced);
    		values.Add("LinkedProcess", this.LinkedProcess);
    		values.Add("Skill", this.Skill);
    		values.Add("Thumbnail", this.Thumbnail);
    
    		values.Add("ActionsReduced", GetHashCodes(this.ActionsReduced));
    		values.Add("Ref2", GetHashCodes(this.Ref2));
    		values.Add("Ref4", GetHashCodes(this.Ref4));
    		values.Add("Ref5", GetHashCodes(this.Ref5));
    		values.Add("Ref6", GetHashCodes(this.Ref6));
    		values.Add("Ref7", GetHashCodes(this.Ref7));
    		values.Add("Ref3", GetHashCodes(this.Ref3));
    		values.Add("Ref1", GetHashCodes(this.Ref1));
    		values.Add("Successors", GetHashCodes(this.Successors));
    		values.Add("Predecessors", GetHashCodes(this.Predecessors));
    		values.Add("DocumentationActionDraftWBS", GetHashCodes(this.DocumentationActionDraftWBS));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Category", this.Category);
    		values.Add("Original", this.Original);
    		values.Add("Resource", this.Resource);
    		values.Add("Scenario", this.Scenario);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    		values.Add("Video", this.Video);
    		values.Add("Reduced", this.Reduced);
    		values.Add("LinkedProcess", this.LinkedProcess);
    		values.Add("Skill", this.Skill);
    		values.Add("Thumbnail", this.Thumbnail);
    
    		values.Add("ActionsReduced", this.ActionsReduced);
    		values.Add("Ref2", this.Ref2);
    		values.Add("Ref4", this.Ref4);
    		values.Add("Ref5", this.Ref5);
    		values.Add("Ref6", this.Ref6);
    		values.Add("Ref7", this.Ref7);
    		values.Add("Ref3", this.Ref3);
    		values.Add("Ref1", this.Ref1);
    		values.Add("Successors", this.Successors);
    		values.Add("Predecessors", this.Predecessors);
    		values.Add("DocumentationActionDraftWBS", this.DocumentationActionDraftWBS);
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
    
        /// <summary>
        /// Taille maximum du champ CustomTextValue.
        /// </summary>
    	public const int CustomTextValueMaxLength = 4000;
    
        /// <summary>
        /// Taille maximum du champ CustomTextValue2.
        /// </summary>
    	public const int CustomTextValue2MaxLength = 4000;
    
        /// <summary>
        /// Taille maximum du champ CustomTextValue3.
        /// </summary>
    	public const int CustomTextValue3MaxLength = 4000;
    
        /// <summary>
        /// Taille maximum du champ CustomTextValue4.
        /// </summary>
    	public const int CustomTextValue4MaxLength = 4000;
    
        /// <summary>
        /// Taille maximum du champ DifferenceReason.
        /// </summary>
    	public const int DifferenceReasonMaxLength = 100;

        #endregion

    }
}
