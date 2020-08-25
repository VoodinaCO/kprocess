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
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(Timeslot))]
    /// <summary>
    /// 
    /// </summary>
    public partial class InspectionSchedule : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.InspectionSchedule";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="InspectionSchedule"/>.
        /// </summary>
    	public InspectionSchedule()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _inspectionScheduleId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int InspectionScheduleId
        {
            get { return _inspectionScheduleId; }
            set
            {
                if (_inspectionScheduleId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'InspectionScheduleId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _inspectionScheduleId = value;
                    OnEntityPropertyChanged("InspectionScheduleId");
                }
            }
        }
        
        private System.DateTime _startDate;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public System.DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (_startDate != value)
                {
                    ChangeTracker.RecordValue("StartDate", _startDate, value);
                    _startDate = value;
                    OnEntityPropertyChanged("StartDate");
                }
            }
        }
        
        private Nullable<int> _recurrenceId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> RecurrenceId
        {
            get { return _recurrenceId; }
            set
            {
                if (_recurrenceId != value)
                {
                    ChangeTracker.RecordValue("RecurrenceId", _recurrenceId, value);
                    _recurrenceId = value;
                    OnEntityPropertyChanged("RecurrenceId");
                }
            }
        }
        
        private string _recurrenceRule;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RecurrenceRule
        {
            get { return _recurrenceRule; }
            set
            {
                if (_recurrenceRule != value)
                {
                    ChangeTracker.RecordValue("RecurrenceRule", _recurrenceRule, value);
                    _recurrenceRule = value;
                    OnEntityPropertyChanged("RecurrenceRule");
                }
            }
        }
        
        private string _recurrenceException;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string RecurrenceException
        {
            get { return _recurrenceException; }
            set
            {
                if (_recurrenceException != value)
                {
                    ChangeTracker.RecordValue("RecurrenceException", _recurrenceException, value);
                    _recurrenceException = value;
                    OnEntityPropertyChanged("RecurrenceException");
                }
            }
        }
        
        private int _processId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int ProcessId
        {
            get { return _processId; }
            set
            {
                if (_processId != value)
                {
                    ChangeTracker.RecordValue("ProcessId", _processId, value);
                    if (!IsDeserializing)
                    {
                        if (Procedure != null && Procedure.ProcessId != value)
                        {
                            Procedure = null;
                        }
                    }
                    _processId = value;
                    OnEntityPropertyChanged("ProcessId");
                }
            }
        }
        
        private int _timeslotId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int TimeslotId
        {
            get { return _timeslotId; }
            set
            {
                if (_timeslotId != value)
                {
                    ChangeTracker.RecordValue("TimeslotId", _timeslotId, value);
                    if (!IsDeserializing)
                    {
                        if (Timeslot != null && Timeslot.TimeslotId != value)
                        {
                            Timeslot = null;
                        }
                    }
                    _timeslotId = value;
                    OnEntityPropertyChanged("TimeslotId");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Procedure Procedure
        {
            get { return _procedure; }
            set
            {
                if (!ReferenceEquals(_procedure, value))
                {
                    var previousValue = _procedure;
                    _procedure = value;
                    FixupProcedure(previousValue);
                    OnNavigationPropertyChanged("Procedure");
                }
            }
        }
        private Procedure _procedure;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Timeslot Timeslot
        {
            get { return _timeslot; }
            set
            {
                if (!ReferenceEquals(_timeslot, value))
                {
                    var previousValue = _timeslot;
                    _timeslot = value;
                    FixupTimeslot(previousValue);
                    OnNavigationPropertyChanged("Timeslot");
                }
            }
        }
        private Timeslot _timeslot;
    				

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
            Procedure = null;
            Timeslot = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Procedure.
        /// </summary>
        private void FixupProcedure(Procedure previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InspectionSchedules.Contains(this))
            {
                previousValue.InspectionSchedules.Remove(this);
            }
    
            if (Procedure != null)
            {
                if (!Procedure.InspectionSchedules.Contains(this))
                {
                    Procedure.InspectionSchedules.Add(this);
                }
    
                ProcessId = Procedure.ProcessId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Procedure", previousValue, Procedure);
                if (Procedure != null && !Procedure.ChangeTracker.ChangeTrackingEnabled)
                {
                    Procedure.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Timeslot.
        /// </summary>
        private void FixupTimeslot(Timeslot previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.InspectionSchedules.Contains(this))
            {
                previousValue.InspectionSchedules.Remove(this);
            }
    
            if (Timeslot != null)
            {
                if (!Timeslot.InspectionSchedules.Contains(this))
                {
                    Timeslot.InspectionSchedules.Add(this);
                }
    
                TimeslotId = Timeslot.TimeslotId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Timeslot", previousValue, Timeslot);
                if (Timeslot != null && !Timeslot.ChangeTracker.ChangeTrackingEnabled)
                {
                    Timeslot.StartTracking();
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
    			case "InspectionScheduleId":
    				this.InspectionScheduleId = Convert.ToInt32(value);
    				break;
    			case "StartDate":
    				this.StartDate = (System.DateTime)value;
    				break;
    			case "RecurrenceId":
    				this.RecurrenceId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "RecurrenceRule":
    				this.RecurrenceRule = (string)value;
    				break;
    			case "RecurrenceException":
    				this.RecurrenceException = (string)value;
    				break;
    			case "ProcessId":
    				this.ProcessId = Convert.ToInt32(value);
    				break;
    			case "TimeslotId":
    				this.TimeslotId = Convert.ToInt32(value);
    				break;
    			case "Procedure":
    				this.Procedure = (Procedure)value;
    				break;
    			case "Timeslot":
    				this.Timeslot = (Timeslot)value;
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
    		values.Add("InspectionScheduleId", this.InspectionScheduleId);
    		values.Add("StartDate", this.StartDate);
    		values.Add("RecurrenceId", this.RecurrenceId);
    		values.Add("RecurrenceRule", this.RecurrenceRule);
    		values.Add("RecurrenceException", this.RecurrenceException);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("TimeslotId", this.TimeslotId);
    		values.Add("Procedure", this.Procedure);
    		values.Add("Timeslot", this.Timeslot);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("Procedure", this.Procedure);
    		values.Add("Timeslot", this.Timeslot);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
