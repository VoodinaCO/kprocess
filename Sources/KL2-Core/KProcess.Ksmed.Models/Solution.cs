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
    [KnownType(typeof(Scenario))]
    /// <summary>
    /// Représente une solution à une action améliorée.
    /// </summary>
    public partial class Solution : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.Solution";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Solution"/>.
        /// </summary>
    	public Solution()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _solutionId;
        /// <summary>
        /// Obtient ou définit l'identifiant de la solution.
        /// </summary>
        [DataMember]
        public int SolutionId
        {
            get { return _solutionId; }
            set
            {
                if (_solutionId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'SolutionId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _solutionId = value;
                    OnEntityPropertyChanged("SolutionId");
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
        
        private string _solutionDescription;
        /// <summary>
        /// Obtient ou définit la solution de l'amélioration.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(SolutionDescriptionMaxLength, ErrorMessageResourceName = "Validation_Solution_SolutionDescription_StringLength")]
        public string SolutionDescription
        {
            get { return _solutionDescription; }
            set
            {
                if (_solutionDescription != value)
                {
                    ChangeTracker.RecordValue("SolutionDescription", _solutionDescription, value);
                    _solutionDescription = value;
                    OnEntityPropertyChanged("SolutionDescription");
                }
            }
        }
        
        private bool _approved;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la solution a été approuvée.
        /// </summary>
        [DataMember]
        public bool Approved
        {
            get { return _approved; }
            set
            {
                if (_approved != value)
                {
                    ChangeTracker.RecordValue("Approved", _approved, value);
    				var oldValue = _approved;
                    _approved = value;
                    OnEntityPropertyChanged("Approved");
    				OnApprovedChanged(oldValue, value);
    				OnApprovedChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Approved"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnApprovedChangedPartial(bool oldValue, bool newValue);
    	public event EventHandler<PropertyChangedEventArgs<bool>> ApprovedChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Approved"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnApprovedChanged(bool oldValue, bool newValue)
    	{
    		if (ApprovedChanged != null)
    			ApprovedChanged(this, new PropertyChangedEventArgs<bool>(oldValue, newValue));
    	}
        
        private Nullable<short> _cost;
        /// <summary>
        /// Obtient ou définit le coût.
        /// </summary>
        [DataMember]
        public Nullable<short> Cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    ChangeTracker.RecordValue("Cost", _cost, value);
                    _cost = value;
                    OnEntityPropertyChanged("Cost");
                }
            }
        }
        
        private Nullable<short> _difficulty;
        /// <summary>
        /// Obtient ou définit la difficulté.
        /// </summary>
        [DataMember]
        public Nullable<short> Difficulty
        {
            get { return _difficulty; }
            set
            {
                if (_difficulty != value)
                {
                    ChangeTracker.RecordValue("Difficulty", _difficulty, value);
                    _difficulty = value;
                    OnEntityPropertyChanged("Difficulty");
                }
            }
        }
        
        private Nullable<double> _investment;
        /// <summary>
        /// Obtient ou définit l'investissement.
        /// </summary>
        [DataMember]
        public Nullable<double> Investment
        {
            get { return _investment; }
            set
            {
                if (_investment != value)
                {
                    ChangeTracker.RecordValue("Investment", _investment, value);
                    _investment = value;
                    OnEntityPropertyChanged("Investment");
                }
            }
        }
        
        private string _comments;
        /// <summary>
        /// Obtient ou définit des commentaires sur la soltion.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(CommentsMaxLength, ErrorMessageResourceName = "Validation_Solution_Comments_StringLength")]
        public string Comments
        {
            get { return _comments; }
            set
            {
                if (_comments != value)
                {
                    ChangeTracker.RecordValue("Comments", _comments, value);
                    _comments = value;
                    OnEntityPropertyChanged("Comments");
                }
            }
        }
        
        private bool _isEmpty;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si la solution correspond à la solution vide du projet.
        /// </summary>
        [DataMember]
        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                if (_isEmpty != value)
                {
                    ChangeTracker.RecordValue("IsEmpty", _isEmpty, value);
                    _isEmpty = value;
                    OnEntityPropertyChanged("IsEmpty");
                }
            }
        }
        
        private string _who;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string Who
        {
            get { return _who; }
            set
            {
                if (_who != value)
                {
                    ChangeTracker.RecordValue("Who", _who, value);
                    _who = value;
                    OnEntityPropertyChanged("Who");
                }
            }
        }
        
        private Nullable<System.DateTime> _when;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<System.DateTime> When
        {
            get { return _when; }
            set
            {
                if (_when != value)
                {
                    ChangeTracker.RecordValue("When", _when, value);
                    _when = value;
                    OnEntityPropertyChanged("When");
                }
            }
        }
        
        private decimal _p;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal P
        {
            get { return _p; }
            set
            {
                if (_p != value)
                {
                    ChangeTracker.RecordValue("P", _p, value);
                    _p = value;
                    OnEntityPropertyChanged("P");
                }
            }
        }
        
        private decimal _d;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal D
        {
            get { return _d; }
            set
            {
                if (_d != value)
                {
                    ChangeTracker.RecordValue("D", _d, value);
                    _d = value;
                    OnEntityPropertyChanged("D");
                }
            }
        }
        
        private decimal _c;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal C
        {
            get { return _c; }
            set
            {
                if (_c != value)
                {
                    ChangeTracker.RecordValue("C", _c, value);
                    _c = value;
                    OnEntityPropertyChanged("C");
                }
            }
        }
        
        private decimal _a;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public decimal A
        {
            get { return _a; }
            set
            {
                if (_a != value)
                {
                    ChangeTracker.RecordValue("A", _a, value);
                    _a = value;
                    OnEntityPropertyChanged("A");
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
            Scenario = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Scenario.
        /// </summary>
        private void FixupScenario(Scenario previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Solutions.Contains(this))
            {
                previousValue.Solutions.Remove(this);
            }
    
            if (Scenario != null)
            {
                if (!Scenario.Solutions.Contains(this))
                {
                    Scenario.Solutions.Add(this);
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
    			case "SolutionId":
    				this.SolutionId = Convert.ToInt32(value);
    				break;
    			case "ScenarioId":
    				this.ScenarioId = Convert.ToInt32(value);
    				break;
    			case "SolutionDescription":
    				this.SolutionDescription = (string)value;
    				break;
    			case "Approved":
    				this.Approved = (bool)value;
    				break;
    			case "Cost":
    				this.Cost = (Nullable<short>)value;
    				break;
    			case "Difficulty":
    				this.Difficulty = (Nullable<short>)value;
    				break;
    			case "Investment":
    				this.Investment = (Nullable<double>)value;
    				break;
    			case "Comments":
    				this.Comments = (string)value;
    				break;
    			case "IsEmpty":
    				this.IsEmpty = (bool)value;
    				break;
    			case "Who":
    				this.Who = (string)value;
    				break;
    			case "When":
    				this.When = (Nullable<System.DateTime>)value;
    				break;
    			case "P":
    				this.P = (decimal)value;
    				break;
    			case "D":
    				this.D = (decimal)value;
    				break;
    			case "C":
    				this.C = (decimal)value;
    				break;
    			case "A":
    				this.A = (decimal)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "Scenario":
    				this.Scenario = (Scenario)value;
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
    		values.Add("SolutionId", this.SolutionId);
    		values.Add("ScenarioId", this.ScenarioId);
    		values.Add("SolutionDescription", this.SolutionDescription);
    		values.Add("Approved", this.Approved);
    		values.Add("Cost", this.Cost);
    		values.Add("Difficulty", this.Difficulty);
    		values.Add("Investment", this.Investment);
    		values.Add("Comments", this.Comments);
    		values.Add("IsEmpty", this.IsEmpty);
    		values.Add("Who", this.Who);
    		values.Add("When", this.When);
    		values.Add("P", this.P);
    		values.Add("D", this.D);
    		values.Add("C", this.C);
    		values.Add("A", this.A);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("Scenario", this.Scenario);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Scenario", this.Scenario);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ SolutionDescription.
        /// </summary>
    	public const int SolutionDescriptionMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ Comments.
        /// </summary>
    	public const int CommentsMaxLength = 4000;

        #endregion

    }
}
