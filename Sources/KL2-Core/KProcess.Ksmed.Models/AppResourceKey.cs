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
    [KnownType(typeof(AppResourceValue))]
    [KnownType(typeof(Objective))]
    [KnownType(typeof(Role))]
    [KnownType(typeof(ScenarioNature))]
    [KnownType(typeof(ScenarioState))]
    /// <summary>
    /// Représente la clé d'une ressource de l'application.
    /// </summary>
    public partial class AppResourceKey : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.AppResourceKey";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AppResourceKey"/>.
        /// </summary>
    	public AppResourceKey()
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
        
        private string _resourceKey;
        /// <summary>
        /// Obtient ou définit la clé de la ressource.
        /// </summary>
        [DataMember]
        public string ResourceKey
        {
            get { return _resourceKey; }
            set
            {
                if (_resourceKey != value)
                {
                    ChangeTracker.RecordValue("ResourceKey", _resourceKey, value);
                    _resourceKey = value;
                    OnEntityPropertyChanged("ResourceKey");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionType> ActionTypesForLongLabel
        {
            get
            {
                if (_actionTypesForLongLabel == null)
                {
                    _actionTypesForLongLabel = new TrackableCollection<ActionType>();
                    _actionTypesForLongLabel.CollectionChanged += FixupActionTypesForLongLabel;
                }
                return _actionTypesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_actionTypesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionTypesForLongLabel != null)
                    {
                        _actionTypesForLongLabel.CollectionChanged -= FixupActionTypesForLongLabel;
                    }
                    _actionTypesForLongLabel = value;
                    if (_actionTypesForLongLabel != null)
                    {
                        _actionTypesForLongLabel.CollectionChanged += FixupActionTypesForLongLabel;
                    }
                    OnNavigationPropertyChanged("ActionTypesForLongLabel");
                }
            }
        }
        private TrackableCollection<ActionType> _actionTypesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionType> ActionTypesForShortLabel
        {
            get
            {
                if (_actionTypesForShortLabel == null)
                {
                    _actionTypesForShortLabel = new TrackableCollection<ActionType>();
                    _actionTypesForShortLabel.CollectionChanged += FixupActionTypesForShortLabel;
                }
                return _actionTypesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_actionTypesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionTypesForShortLabel != null)
                    {
                        _actionTypesForShortLabel.CollectionChanged -= FixupActionTypesForShortLabel;
                    }
                    _actionTypesForShortLabel = value;
                    if (_actionTypesForShortLabel != null)
                    {
                        _actionTypesForShortLabel.CollectionChanged += FixupActionTypesForShortLabel;
                    }
                    OnNavigationPropertyChanged("ActionTypesForShortLabel");
                }
            }
        }
        private TrackableCollection<ActionType> _actionTypesForShortLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionValue> ActionValuesForLongLabel
        {
            get
            {
                if (_actionValuesForLongLabel == null)
                {
                    _actionValuesForLongLabel = new TrackableCollection<ActionValue>();
                    _actionValuesForLongLabel.CollectionChanged += FixupActionValuesForLongLabel;
                }
                return _actionValuesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_actionValuesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionValuesForLongLabel != null)
                    {
                        _actionValuesForLongLabel.CollectionChanged -= FixupActionValuesForLongLabel;
                    }
                    _actionValuesForLongLabel = value;
                    if (_actionValuesForLongLabel != null)
                    {
                        _actionValuesForLongLabel.CollectionChanged += FixupActionValuesForLongLabel;
                    }
                    OnNavigationPropertyChanged("ActionValuesForLongLabel");
                }
            }
        }
        private TrackableCollection<ActionValue> _actionValuesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionValue> ActionValuesForShortLabel
        {
            get
            {
                if (_actionValuesForShortLabel == null)
                {
                    _actionValuesForShortLabel = new TrackableCollection<ActionValue>();
                    _actionValuesForShortLabel.CollectionChanged += FixupActionValuesForShortLabel;
                }
                return _actionValuesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_actionValuesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_actionValuesForShortLabel != null)
                    {
                        _actionValuesForShortLabel.CollectionChanged -= FixupActionValuesForShortLabel;
                    }
                    _actionValuesForShortLabel = value;
                    if (_actionValuesForShortLabel != null)
                    {
                        _actionValuesForShortLabel.CollectionChanged += FixupActionValuesForShortLabel;
                    }
                    OnNavigationPropertyChanged("ActionValuesForShortLabel");
                }
            }
        }
        private TrackableCollection<ActionValue> _actionValuesForShortLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<AppResourceValue> AppResourceValues
        {
            get
            {
                if (_appResourceValues == null)
                {
                    _appResourceValues = new TrackableCollection<AppResourceValue>();
                    _appResourceValues.CollectionChanged += FixupAppResourceValues;
                }
                return _appResourceValues;
            }
            set
            {
                if (!ReferenceEquals(_appResourceValues, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_appResourceValues != null)
                    {
                        _appResourceValues.CollectionChanged -= FixupAppResourceValues;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (AppResourceValue item in _appResourceValues)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _appResourceValues = value;
                    if (_appResourceValues != null)
                    {
                        _appResourceValues.CollectionChanged += FixupAppResourceValues;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (AppResourceValue item in _appResourceValues)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("AppResourceValues");
                }
            }
        }
        private TrackableCollection<AppResourceValue> _appResourceValues;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Objective> ObjectivesForLongLabel
        {
            get
            {
                if (_objectivesForLongLabel == null)
                {
                    _objectivesForLongLabel = new TrackableCollection<Objective>();
                    _objectivesForLongLabel.CollectionChanged += FixupObjectivesForLongLabel;
                }
                return _objectivesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_objectivesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_objectivesForLongLabel != null)
                    {
                        _objectivesForLongLabel.CollectionChanged -= FixupObjectivesForLongLabel;
                    }
                    _objectivesForLongLabel = value;
                    if (_objectivesForLongLabel != null)
                    {
                        _objectivesForLongLabel.CollectionChanged += FixupObjectivesForLongLabel;
                    }
                    OnNavigationPropertyChanged("ObjectivesForLongLabel");
                }
            }
        }
        private TrackableCollection<Objective> _objectivesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Objective> ObjectivesForShortLabel
        {
            get
            {
                if (_objectivesForShortLabel == null)
                {
                    _objectivesForShortLabel = new TrackableCollection<Objective>();
                    _objectivesForShortLabel.CollectionChanged += FixupObjectivesForShortLabel;
                }
                return _objectivesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_objectivesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_objectivesForShortLabel != null)
                    {
                        _objectivesForShortLabel.CollectionChanged -= FixupObjectivesForShortLabel;
                    }
                    _objectivesForShortLabel = value;
                    if (_objectivesForShortLabel != null)
                    {
                        _objectivesForShortLabel.CollectionChanged += FixupObjectivesForShortLabel;
                    }
                    OnNavigationPropertyChanged("ObjectivesForShortLabel");
                }
            }
        }
        private TrackableCollection<Objective> _objectivesForShortLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Role> RolesForLongLabel
        {
            get
            {
                if (_rolesForLongLabel == null)
                {
                    _rolesForLongLabel = new TrackableCollection<Role>();
                    _rolesForLongLabel.CollectionChanged += FixupRolesForLongLabel;
                }
                return _rolesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_rolesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_rolesForLongLabel != null)
                    {
                        _rolesForLongLabel.CollectionChanged -= FixupRolesForLongLabel;
                    }
                    _rolesForLongLabel = value;
                    if (_rolesForLongLabel != null)
                    {
                        _rolesForLongLabel.CollectionChanged += FixupRolesForLongLabel;
                    }
                    OnNavigationPropertyChanged("RolesForLongLabel");
                }
            }
        }
        private TrackableCollection<Role> _rolesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Role> RolesForShortLabel
        {
            get
            {
                if (_rolesForShortLabel == null)
                {
                    _rolesForShortLabel = new TrackableCollection<Role>();
                    _rolesForShortLabel.CollectionChanged += FixupRolesForShortLabel;
                }
                return _rolesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_rolesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_rolesForShortLabel != null)
                    {
                        _rolesForShortLabel.CollectionChanged -= FixupRolesForShortLabel;
                    }
                    _rolesForShortLabel = value;
                    if (_rolesForShortLabel != null)
                    {
                        _rolesForShortLabel.CollectionChanged += FixupRolesForShortLabel;
                    }
                    OnNavigationPropertyChanged("RolesForShortLabel");
                }
            }
        }
        private TrackableCollection<Role> _rolesForShortLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ScenarioNature> ScenarioNaturesForLongLabel
        {
            get
            {
                if (_scenarioNaturesForLongLabel == null)
                {
                    _scenarioNaturesForLongLabel = new TrackableCollection<ScenarioNature>();
                    _scenarioNaturesForLongLabel.CollectionChanged += FixupScenarioNaturesForLongLabel;
                }
                return _scenarioNaturesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_scenarioNaturesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_scenarioNaturesForLongLabel != null)
                    {
                        _scenarioNaturesForLongLabel.CollectionChanged -= FixupScenarioNaturesForLongLabel;
                    }
                    _scenarioNaturesForLongLabel = value;
                    if (_scenarioNaturesForLongLabel != null)
                    {
                        _scenarioNaturesForLongLabel.CollectionChanged += FixupScenarioNaturesForLongLabel;
                    }
                    OnNavigationPropertyChanged("ScenarioNaturesForLongLabel");
                }
            }
        }
        private TrackableCollection<ScenarioNature> _scenarioNaturesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ScenarioNature> ScenarioNaturesForShortLabel
        {
            get
            {
                if (_scenarioNaturesForShortLabel == null)
                {
                    _scenarioNaturesForShortLabel = new TrackableCollection<ScenarioNature>();
                    _scenarioNaturesForShortLabel.CollectionChanged += FixupScenarioNaturesForShortLabel;
                }
                return _scenarioNaturesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_scenarioNaturesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_scenarioNaturesForShortLabel != null)
                    {
                        _scenarioNaturesForShortLabel.CollectionChanged -= FixupScenarioNaturesForShortLabel;
                    }
                    _scenarioNaturesForShortLabel = value;
                    if (_scenarioNaturesForShortLabel != null)
                    {
                        _scenarioNaturesForShortLabel.CollectionChanged += FixupScenarioNaturesForShortLabel;
                    }
                    OnNavigationPropertyChanged("ScenarioNaturesForShortLabel");
                }
            }
        }
        private TrackableCollection<ScenarioNature> _scenarioNaturesForShortLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ScenarioState> ScenarioStatesForLongLabel
        {
            get
            {
                if (_scenarioStatesForLongLabel == null)
                {
                    _scenarioStatesForLongLabel = new TrackableCollection<ScenarioState>();
                    _scenarioStatesForLongLabel.CollectionChanged += FixupScenarioStatesForLongLabel;
                }
                return _scenarioStatesForLongLabel;
            }
            set
            {
                if (!ReferenceEquals(_scenarioStatesForLongLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_scenarioStatesForLongLabel != null)
                    {
                        _scenarioStatesForLongLabel.CollectionChanged -= FixupScenarioStatesForLongLabel;
                    }
                    _scenarioStatesForLongLabel = value;
                    if (_scenarioStatesForLongLabel != null)
                    {
                        _scenarioStatesForLongLabel.CollectionChanged += FixupScenarioStatesForLongLabel;
                    }
                    OnNavigationPropertyChanged("ScenarioStatesForLongLabel");
                }
            }
        }
        private TrackableCollection<ScenarioState> _scenarioStatesForLongLabel;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ScenarioState> ScenarioStatesForShortLabel
        {
            get
            {
                if (_scenarioStatesForShortLabel == null)
                {
                    _scenarioStatesForShortLabel = new TrackableCollection<ScenarioState>();
                    _scenarioStatesForShortLabel.CollectionChanged += FixupScenarioStatesForShortLabel;
                }
                return _scenarioStatesForShortLabel;
            }
            set
            {
                if (!ReferenceEquals(_scenarioStatesForShortLabel, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_scenarioStatesForShortLabel != null)
                    {
                        _scenarioStatesForShortLabel.CollectionChanged -= FixupScenarioStatesForShortLabel;
                    }
                    _scenarioStatesForShortLabel = value;
                    if (_scenarioStatesForShortLabel != null)
                    {
                        _scenarioStatesForShortLabel.CollectionChanged += FixupScenarioStatesForShortLabel;
                    }
                    OnNavigationPropertyChanged("ScenarioStatesForShortLabel");
                }
            }
        }
        private TrackableCollection<ScenarioState> _scenarioStatesForShortLabel;

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
            ActionTypesForLongLabel.Clear();
            ActionTypesForShortLabel.Clear();
            ActionValuesForLongLabel.Clear();
            ActionValuesForShortLabel.Clear();
            AppResourceValues.Clear();
            ObjectivesForLongLabel.Clear();
            ObjectivesForShortLabel.Clear();
            RolesForLongLabel.Clear();
            RolesForShortLabel.Clear();
            ScenarioNaturesForLongLabel.Clear();
            ScenarioNaturesForShortLabel.Clear();
            ScenarioStatesForLongLabel.Clear();
            ScenarioStatesForShortLabel.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété ActionTypesForLongLabel.
        /// </summary>
        private void FixupActionTypesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionType item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionTypesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionType item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionTypesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionTypesForShortLabel.
        /// </summary>
        private void FixupActionTypesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionType item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionTypesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionType item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionTypesForShortLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionValuesForLongLabel.
        /// </summary>
        private void FixupActionValuesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionValue item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionValuesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionValuesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ActionValuesForShortLabel.
        /// </summary>
        private void FixupActionValuesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionValue item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ActionValuesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ActionValuesForShortLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété AppResourceValues.
        /// </summary>
        private void FixupAppResourceValues(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (AppResourceValue item in e.NewItems)
                {
                    item.AppResourceKey = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("AppResourceValues", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AppResourceValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.AppResourceKey, this))
                    {
                        item.AppResourceKey = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("AppResourceValues", item);
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
        /// Corrige l'état de la propriété ObjectivesForLongLabel.
        /// </summary>
        private void FixupObjectivesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Objective item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ObjectivesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Objective item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ObjectivesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ObjectivesForShortLabel.
        /// </summary>
        private void FixupObjectivesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Objective item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ObjectivesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Objective item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ObjectivesForShortLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété RolesForLongLabel.
        /// </summary>
        private void FixupRolesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Role item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("RolesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Role item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("RolesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété RolesForShortLabel.
        /// </summary>
        private void FixupRolesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Role item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("RolesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Role item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("RolesForShortLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ScenarioNaturesForLongLabel.
        /// </summary>
        private void FixupScenarioNaturesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ScenarioNature item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ScenarioNaturesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ScenarioNature item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ScenarioNaturesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ScenarioNaturesForShortLabel.
        /// </summary>
        private void FixupScenarioNaturesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ScenarioNature item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ScenarioNaturesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ScenarioNature item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ScenarioNaturesForShortLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ScenarioStatesForLongLabel.
        /// </summary>
        private void FixupScenarioStatesForLongLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ScenarioState item in e.NewItems)
                {
                    item.LongLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ScenarioStatesForLongLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ScenarioState item in e.OldItems)
                {
                    if (ReferenceEquals(item.LongLabelResource, this))
                    {
                        item.LongLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ScenarioStatesForLongLabel", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété ScenarioStatesForShortLabel.
        /// </summary>
        private void FixupScenarioStatesForShortLabel(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ScenarioState item in e.NewItems)
                {
                    item.ShortLabelResource = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ScenarioStatesForShortLabel", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ScenarioState item in e.OldItems)
                {
                    if (ReferenceEquals(item.ShortLabelResource, this))
                    {
                        item.ShortLabelResource = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ScenarioStatesForShortLabel", item);
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
    			case "ResourceKey":
    				this.ResourceKey = (string)value;
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
    			case "ActionTypesForLongLabel":
    				this.ActionTypesForLongLabel.Add((ActionType)value);
    				break;
    			case "ActionTypesForShortLabel":
    				this.ActionTypesForShortLabel.Add((ActionType)value);
    				break;
    			case "ActionValuesForLongLabel":
    				this.ActionValuesForLongLabel.Add((ActionValue)value);
    				break;
    			case "ActionValuesForShortLabel":
    				this.ActionValuesForShortLabel.Add((ActionValue)value);
    				break;
    			case "AppResourceValues":
    				this.AppResourceValues.Add((AppResourceValue)value);
    				break;
    			case "ObjectivesForLongLabel":
    				this.ObjectivesForLongLabel.Add((Objective)value);
    				break;
    			case "ObjectivesForShortLabel":
    				this.ObjectivesForShortLabel.Add((Objective)value);
    				break;
    			case "RolesForLongLabel":
    				this.RolesForLongLabel.Add((Role)value);
    				break;
    			case "RolesForShortLabel":
    				this.RolesForShortLabel.Add((Role)value);
    				break;
    			case "ScenarioNaturesForLongLabel":
    				this.ScenarioNaturesForLongLabel.Add((ScenarioNature)value);
    				break;
    			case "ScenarioNaturesForShortLabel":
    				this.ScenarioNaturesForShortLabel.Add((ScenarioNature)value);
    				break;
    			case "ScenarioStatesForLongLabel":
    				this.ScenarioStatesForLongLabel.Add((ScenarioState)value);
    				break;
    			case "ScenarioStatesForShortLabel":
    				this.ScenarioStatesForShortLabel.Add((ScenarioState)value);
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
    			case "ActionTypesForLongLabel":
    				this.ActionTypesForLongLabel.Remove((ActionType)value);
    				break;
    			case "ActionTypesForShortLabel":
    				this.ActionTypesForShortLabel.Remove((ActionType)value);
    				break;
    			case "ActionValuesForLongLabel":
    				this.ActionValuesForLongLabel.Remove((ActionValue)value);
    				break;
    			case "ActionValuesForShortLabel":
    				this.ActionValuesForShortLabel.Remove((ActionValue)value);
    				break;
    			case "AppResourceValues":
    				this.AppResourceValues.Remove((AppResourceValue)value);
    				break;
    			case "ObjectivesForLongLabel":
    				this.ObjectivesForLongLabel.Remove((Objective)value);
    				break;
    			case "ObjectivesForShortLabel":
    				this.ObjectivesForShortLabel.Remove((Objective)value);
    				break;
    			case "RolesForLongLabel":
    				this.RolesForLongLabel.Remove((Role)value);
    				break;
    			case "RolesForShortLabel":
    				this.RolesForShortLabel.Remove((Role)value);
    				break;
    			case "ScenarioNaturesForLongLabel":
    				this.ScenarioNaturesForLongLabel.Remove((ScenarioNature)value);
    				break;
    			case "ScenarioNaturesForShortLabel":
    				this.ScenarioNaturesForShortLabel.Remove((ScenarioNature)value);
    				break;
    			case "ScenarioStatesForLongLabel":
    				this.ScenarioStatesForLongLabel.Remove((ScenarioState)value);
    				break;
    			case "ScenarioStatesForShortLabel":
    				this.ScenarioStatesForShortLabel.Remove((ScenarioState)value);
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
    		values.Add("ResourceKey", this.ResourceKey);
    
    		values.Add("ActionTypesForLongLabel", GetHashCodes(this.ActionTypesForLongLabel));
    		values.Add("ActionTypesForShortLabel", GetHashCodes(this.ActionTypesForShortLabel));
    		values.Add("ActionValuesForLongLabel", GetHashCodes(this.ActionValuesForLongLabel));
    		values.Add("ActionValuesForShortLabel", GetHashCodes(this.ActionValuesForShortLabel));
    		values.Add("AppResourceValues", GetHashCodes(this.AppResourceValues));
    		values.Add("ObjectivesForLongLabel", GetHashCodes(this.ObjectivesForLongLabel));
    		values.Add("ObjectivesForShortLabel", GetHashCodes(this.ObjectivesForShortLabel));
    		values.Add("RolesForLongLabel", GetHashCodes(this.RolesForLongLabel));
    		values.Add("RolesForShortLabel", GetHashCodes(this.RolesForShortLabel));
    		values.Add("ScenarioNaturesForLongLabel", GetHashCodes(this.ScenarioNaturesForLongLabel));
    		values.Add("ScenarioNaturesForShortLabel", GetHashCodes(this.ScenarioNaturesForShortLabel));
    		values.Add("ScenarioStatesForLongLabel", GetHashCodes(this.ScenarioStatesForLongLabel));
    		values.Add("ScenarioStatesForShortLabel", GetHashCodes(this.ScenarioStatesForShortLabel));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    
    		values.Add("ActionTypesForLongLabel", this.ActionTypesForLongLabel);
    		values.Add("ActionTypesForShortLabel", this.ActionTypesForShortLabel);
    		values.Add("ActionValuesForLongLabel", this.ActionValuesForLongLabel);
    		values.Add("ActionValuesForShortLabel", this.ActionValuesForShortLabel);
    		values.Add("AppResourceValues", this.AppResourceValues);
    		values.Add("ObjectivesForLongLabel", this.ObjectivesForLongLabel);
    		values.Add("ObjectivesForShortLabel", this.ObjectivesForShortLabel);
    		values.Add("RolesForLongLabel", this.RolesForLongLabel);
    		values.Add("RolesForShortLabel", this.RolesForShortLabel);
    		values.Add("ScenarioNaturesForLongLabel", this.ScenarioNaturesForLongLabel);
    		values.Add("ScenarioNaturesForShortLabel", this.ScenarioNaturesForShortLabel);
    		values.Add("ScenarioStatesForLongLabel", this.ScenarioStatesForLongLabel);
    		values.Add("ScenarioStatesForShortLabel", this.ScenarioStatesForShortLabel);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
