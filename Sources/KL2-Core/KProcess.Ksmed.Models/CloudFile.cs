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
    [KnownType(typeof(KAction))]
    [KnownType(typeof(Resource))]
    [KnownType(typeof(Ref1))]
    [KnownType(typeof(Ref2))]
    [KnownType(typeof(Ref3))]
    [KnownType(typeof(Ref4))]
    [KnownType(typeof(Ref5))]
    [KnownType(typeof(Ref6))]
    [KnownType(typeof(Ref7))]
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(Skill))]
    [KnownType(typeof(DocumentationActionDraft))]
    /// <summary>
    /// 
    /// </summary>
    public partial class CloudFile : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.CloudFile";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CloudFile"/>.
        /// </summary>
    	public CloudFile()
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
    	[ScriptIgnore]
        public TrackableCollection<Resource> RefResources
        {
            get
            {
                if (_refResources == null)
                {
                    _refResources = new TrackableCollection<Resource>();
                    _refResources.CollectionChanged += FixupRefResources;
                }
                return _refResources;
            }
            set
            {
                if (!ReferenceEquals(_refResources, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refResources != null)
                    {
                        _refResources.CollectionChanged -= FixupRefResources;
                    }
                    _refResources = value;
                    if (_refResources != null)
                    {
                        _refResources.CollectionChanged += FixupRefResources;
                    }
                    OnNavigationPropertyChanged("RefResources");
                }
            }
        }
        private TrackableCollection<Resource> _refResources;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref1> Ref1
        {
            get
            {
                if (_ref1 == null)
                {
                    _ref1 = new TrackableCollection<Ref1>();
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
                    }
                    _ref1 = value;
                    if (_ref1 != null)
                    {
                        _ref1.CollectionChanged += FixupRef1;
                    }
                    OnNavigationPropertyChanged("Ref1");
                }
            }
        }
        private TrackableCollection<Ref1> _ref1;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref2> Ref2
        {
            get
            {
                if (_ref2 == null)
                {
                    _ref2 = new TrackableCollection<Ref2>();
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
                    }
                    _ref2 = value;
                    if (_ref2 != null)
                    {
                        _ref2.CollectionChanged += FixupRef2;
                    }
                    OnNavigationPropertyChanged("Ref2");
                }
            }
        }
        private TrackableCollection<Ref2> _ref2;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref3> Ref3
        {
            get
            {
                if (_ref3 == null)
                {
                    _ref3 = new TrackableCollection<Ref3>();
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
                    }
                    _ref3 = value;
                    if (_ref3 != null)
                    {
                        _ref3.CollectionChanged += FixupRef3;
                    }
                    OnNavigationPropertyChanged("Ref3");
                }
            }
        }
        private TrackableCollection<Ref3> _ref3;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref4> Ref4
        {
            get
            {
                if (_ref4 == null)
                {
                    _ref4 = new TrackableCollection<Ref4>();
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
                    }
                    _ref4 = value;
                    if (_ref4 != null)
                    {
                        _ref4.CollectionChanged += FixupRef4;
                    }
                    OnNavigationPropertyChanged("Ref4");
                }
            }
        }
        private TrackableCollection<Ref4> _ref4;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref5> Ref5
        {
            get
            {
                if (_ref5 == null)
                {
                    _ref5 = new TrackableCollection<Ref5>();
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
                    }
                    _ref5 = value;
                    if (_ref5 != null)
                    {
                        _ref5.CollectionChanged += FixupRef5;
                    }
                    OnNavigationPropertyChanged("Ref5");
                }
            }
        }
        private TrackableCollection<Ref5> _ref5;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref6> Ref6
        {
            get
            {
                if (_ref6 == null)
                {
                    _ref6 = new TrackableCollection<Ref6>();
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
                    }
                    _ref6 = value;
                    if (_ref6 != null)
                    {
                        _ref6.CollectionChanged += FixupRef6;
                    }
                    OnNavigationPropertyChanged("Ref6");
                }
            }
        }
        private TrackableCollection<Ref6> _ref6;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Ref7> Ref7
        {
            get
            {
                if (_ref7 == null)
                {
                    _ref7 = new TrackableCollection<Ref7>();
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
                    }
                    _ref7 = value;
                    if (_ref7 != null)
                    {
                        _ref7.CollectionChanged += FixupRef7;
                    }
                    OnNavigationPropertyChanged("Ref7");
                }
            }
        }
        private TrackableCollection<Ref7> _ref7;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<ActionCategory> RefActionCategories
        {
            get
            {
                if (_refActionCategories == null)
                {
                    _refActionCategories = new TrackableCollection<ActionCategory>();
                    _refActionCategories.CollectionChanged += FixupRefActionCategories;
                }
                return _refActionCategories;
            }
            set
            {
                if (!ReferenceEquals(_refActionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_refActionCategories != null)
                    {
                        _refActionCategories.CollectionChanged -= FixupRefActionCategories;
                    }
                    _refActionCategories = value;
                    if (_refActionCategories != null)
                    {
                        _refActionCategories.CollectionChanged += FixupRefActionCategories;
                    }
                    OnNavigationPropertyChanged("RefActionCategories");
                }
            }
        }
        private TrackableCollection<ActionCategory> _refActionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public TrackableCollection<Skill> Skills
        {
            get
            {
                if (_skills == null)
                {
                    _skills = new TrackableCollection<Skill>();
                    _skills.CollectionChanged += FixupSkills;
                }
                return _skills;
            }
            set
            {
                if (!ReferenceEquals(_skills, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_skills != null)
                    {
                        _skills.CollectionChanged -= FixupSkills;
                    }
                    _skills = value;
                    if (_skills != null)
                    {
                        _skills.CollectionChanged += FixupSkills;
                    }
                    OnNavigationPropertyChanged("Skills");
                }
            }
        }
        private TrackableCollection<Skill> _skills;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
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
            RefResources.Clear();
            Ref1.Clear();
            Ref2.Clear();
            Ref3.Clear();
            Ref4.Clear();
            Ref5.Clear();
            Ref6.Clear();
            Ref7.Clear();
            RefActionCategories.Clear();
            Skills.Clear();
            DocumentationActionDrafts.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
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
                    item.Thumbnail = this;
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
                    if (ReferenceEquals(item.Thumbnail, this))
                    {
                        item.Thumbnail = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Actions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété RefResources.
        /// </summary>
        private void FixupRefResources(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Resource item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("RefResources", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Resource item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("RefResources", item);
                    }
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
                foreach (Ref1 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref1 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref1", item);
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
                foreach (Ref2 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref2", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref2 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref2", item);
                    }
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
                foreach (Ref3 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref3", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref3 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref3", item);
                    }
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
                foreach (Ref4 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref4", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref4 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref4", item);
                    }
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
                foreach (Ref5 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref5", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref5 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref5", item);
                    }
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
                foreach (Ref6 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref6", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref6 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref6", item);
                    }
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
                foreach (Ref7 item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Ref7", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref7 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Ref7", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété RefActionCategories.
        /// </summary>
        private void FixupRefActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionCategory item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("RefActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("RefActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Skills.
        /// </summary>
        private void FixupSkills(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Skill item in e.NewItems)
                {
                    item.CloudFile = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Skills", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Skill item in e.OldItems)
                {
                    if (ReferenceEquals(item.CloudFile, this))
                    {
                        item.CloudFile = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Skills", item);
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
                    item.Thumbnail = this;
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
                    if (ReferenceEquals(item.Thumbnail, this))
                    {
                        item.Thumbnail = null;
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
    			case "Actions":
    				this.Actions.Add((KAction)value);
    				break;
    			case "RefResources":
    				this.RefResources.Add((Resource)value);
    				break;
    			case "Ref1":
    				this.Ref1.Add((Ref1)value);
    				break;
    			case "Ref2":
    				this.Ref2.Add((Ref2)value);
    				break;
    			case "Ref3":
    				this.Ref3.Add((Ref3)value);
    				break;
    			case "Ref4":
    				this.Ref4.Add((Ref4)value);
    				break;
    			case "Ref5":
    				this.Ref5.Add((Ref5)value);
    				break;
    			case "Ref6":
    				this.Ref6.Add((Ref6)value);
    				break;
    			case "Ref7":
    				this.Ref7.Add((Ref7)value);
    				break;
    			case "RefActionCategories":
    				this.RefActionCategories.Add((ActionCategory)value);
    				break;
    			case "Skills":
    				this.Skills.Add((Skill)value);
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
    			case "RefResources":
    				this.RefResources.Remove((Resource)value);
    				break;
    			case "Ref1":
    				this.Ref1.Remove((Ref1)value);
    				break;
    			case "Ref2":
    				this.Ref2.Remove((Ref2)value);
    				break;
    			case "Ref3":
    				this.Ref3.Remove((Ref3)value);
    				break;
    			case "Ref4":
    				this.Ref4.Remove((Ref4)value);
    				break;
    			case "Ref5":
    				this.Ref5.Remove((Ref5)value);
    				break;
    			case "Ref6":
    				this.Ref6.Remove((Ref6)value);
    				break;
    			case "Ref7":
    				this.Ref7.Remove((Ref7)value);
    				break;
    			case "RefActionCategories":
    				this.RefActionCategories.Remove((ActionCategory)value);
    				break;
    			case "Skills":
    				this.Skills.Remove((Skill)value);
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
    		values.Add("Hash", this.Hash);
    		values.Add("Extension", this.Extension);
    
    		values.Add("Actions", GetHashCodes(this.Actions));
    		values.Add("RefResources", GetHashCodes(this.RefResources));
    		values.Add("Ref1", GetHashCodes(this.Ref1));
    		values.Add("Ref2", GetHashCodes(this.Ref2));
    		values.Add("Ref3", GetHashCodes(this.Ref3));
    		values.Add("Ref4", GetHashCodes(this.Ref4));
    		values.Add("Ref5", GetHashCodes(this.Ref5));
    		values.Add("Ref6", GetHashCodes(this.Ref6));
    		values.Add("Ref7", GetHashCodes(this.Ref7));
    		values.Add("RefActionCategories", GetHashCodes(this.RefActionCategories));
    		values.Add("Skills", GetHashCodes(this.Skills));
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
    
    		values.Add("Actions", this.Actions);
    		values.Add("RefResources", this.RefResources);
    		values.Add("Ref1", this.Ref1);
    		values.Add("Ref2", this.Ref2);
    		values.Add("Ref3", this.Ref3);
    		values.Add("Ref4", this.Ref4);
    		values.Add("Ref5", this.Ref5);
    		values.Add("Ref6", this.Ref6);
    		values.Add("Ref7", this.Ref7);
    		values.Add("RefActionCategories", this.RefActionCategories);
    		values.Add("Skills", this.Skills);
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
