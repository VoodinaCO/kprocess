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
    [KnownType(typeof(ActionType))]
    /// <summary>
    /// Représente une action réduite.
    /// </summary>
    public partial class KActionReduced : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.KActionReduced";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="KActionReduced"/>.
        /// </summary>
    	public KActionReduced()
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
                    if (!IsDeserializing)
                    {
                        if (Action != null && Action.ActionId != value)
                        {
                            Action = null;
                        }
                    }
                    _actionId = value;
                    OnEntityPropertyChanged("ActionId");
                }
            }
        }
        
        private string _actionTypeCode;
        /// <summary>
        /// Obtient ou définit le code du type de l'action.
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
        
        private string _solution;
        /// <summary>
        /// Obtient ou définit la solution de l'amélioration.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(SolutionMaxLength, ErrorMessageResourceName = "Validation_KActionReduced_Solution_StringLength")]
        public string Solution
        {
            get { return _solution; }
            set
            {
                if (_solution != value)
                {
                    ChangeTracker.RecordValue("Solution", _solution, value);
    				var oldValue = _solution;
                    _solution = value;
                    OnEntityPropertyChanged("Solution");
    				OnSolutionChanged(oldValue, value);
    				OnSolutionChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Solution"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnSolutionChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> SolutionChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Solution"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnSolutionChanged(string oldValue, string newValue)
    	{
    		if (SolutionChanged != null)
    			SolutionChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private double _reductionRatio;
        /// <summary>
        /// Obtient ou définit le ratio de réduction.
        /// </summary>
        [DataMember]
        public double ReductionRatio
        {
            get { return _reductionRatio; }
            set
            {
                if (_reductionRatio != value)
                {
                    ChangeTracker.RecordValue("ReductionRatio", _reductionRatio, value);
    				var oldValue = _reductionRatio;
                    _reductionRatio = value;
                    OnEntityPropertyChanged("ReductionRatio");
    				OnReductionRatioChanged(oldValue, value);
    				OnReductionRatioChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ReductionRatio"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnReductionRatioChangedPartial(double oldValue, double newValue);
    	public event EventHandler<PropertyChangedEventArgs<double>> ReductionRatioChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ReductionRatio"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnReductionRatioChanged(double oldValue, double newValue)
    	{
    		if (ReductionRatioChanged != null)
    			ReductionRatioChanged(this, new PropertyChangedEventArgs<double>(oldValue, newValue));
    	}
        
        private long _originalBuildDuration;
        /// <summary>
        /// Obtient ou définit la durée de l'action d'origine.
        /// </summary>
        [DataMember]
        public long OriginalBuildDuration
        {
            get { return _originalBuildDuration; }
            set
            {
                if (_originalBuildDuration != value)
                {
                    ChangeTracker.RecordValue("OriginalBuildDuration", _originalBuildDuration, value);
                    _originalBuildDuration = value;
                    OnEntityPropertyChanged("OriginalBuildDuration");
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
        /// Obtient ou définit l'action associée.
        /// </summary>
        [DataMember]
        public KAction Action
        {
            get { return _action; }
            set
            {
                if (!ReferenceEquals(_action, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added && value != null)
                    {
                        // This the dependent end of an identifying relationship, so the principal end cannot be changed if it is already set,
                        // otherwise it can only be set to an entity with a primary key that is the same value as the dependent's foreign key.
                        if (ActionId != value.ActionId)
                        {
                            throw new InvalidOperationException("The principal end of an identifying relationship can only be changed when the dependent end is in the Added state.");
                        }
                    }
                    var previousValue = _action;
                    _action = value;
                    FixupAction(previousValue);
                    OnNavigationPropertyChanged("Action");
                }
            }
        }
        private KAction _action;
    				
    
        /// <summary>
        /// Obtient ou définit le type de l'action associée.
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
    				OnActionTypeChanged(previousValue, value);
    				OnActionTypeChangedPartial(previousValue, value);
                }
            }
        }
        private ActionType _actionType;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ActionType"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnActionTypeChangedPartial(ActionType oldValue, ActionType newValue);
    	public event EventHandler<PropertyChangedEventArgs<ActionType>> ActionTypeChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ActionType"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnActionTypeChanged(ActionType oldValue, ActionType newValue)
    	{
    		if (ActionTypeChanged != null)
    			ActionTypeChanged(this, new PropertyChangedEventArgs<ActionType>(oldValue, newValue));
    	}

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
            Action = null;
            ActionType = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Action.
        /// </summary>
        private void FixupAction(KAction previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && ReferenceEquals(previousValue.Reduced, this))
            {
                previousValue.Reduced = null;
            }
    
            if (Action != null)
            {
                Action.Reduced = this;
                ActionId = Action.ActionId;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Action", previousValue, Action);
                if (Action != null && !Action.ChangeTracker.ChangeTrackingEnabled)
                {
                    Action.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation ActionType.
        /// </summary>
        private void FixupActionType(ActionType previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.ActionsReduced.Contains(this))
            {
                previousValue.ActionsReduced.Remove(this);
            }
    
            if (ActionType != null)
            {
                if (!ActionType.ActionsReduced.Contains(this))
                {
                    ActionType.ActionsReduced.Add(this);
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
    			case "ActionTypeCode":
    				this.ActionTypeCode = (string)value;
    				break;
    			case "Solution":
    				this.Solution = (string)value;
    				break;
    			case "ReductionRatio":
    				this.ReductionRatio = (double)value;
    				break;
    			case "OriginalBuildDuration":
    				this.OriginalBuildDuration = (long)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "Action":
    				this.Action = (KAction)value;
    				break;
    			case "ActionType":
    				this.ActionType = (ActionType)value;
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
    		values.Add("ActionId", this.ActionId);
    		values.Add("ActionTypeCode", this.ActionTypeCode);
    		values.Add("Solution", this.Solution);
    		values.Add("ReductionRatio", this.ReductionRatio);
    		values.Add("OriginalBuildDuration", this.OriginalBuildDuration);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("Action", this.Action);
    		values.Add("ActionType", this.ActionType);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Action", this.Action);
    		values.Add("ActionType", this.ActionType);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Solution.
        /// </summary>
    	public const int SolutionMaxLength = 100;

        #endregion

    }
}
