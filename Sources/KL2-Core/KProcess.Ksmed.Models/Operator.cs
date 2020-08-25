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
    /// <summary>
    /// Représente un opérateur.
    /// </summary>
    public partial class Operator : Resource, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public new const string TypeFullName = "KProcess.Ksmed.Models.Operator";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Operator"/>.
        /// </summary>
    	public Operator()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private Nullable<int> _processId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ProcessId
        {
            get { return _processId; }
            set
            {
                if (_processId != value)
                {
                    ChangeTracker.RecordValue("ProcessId", _processId, value);
                    if (!IsDeserializing)
                    {
                        if (Process != null && Process.ProcessId != value)
                        {
                            Process = null;
                        }
                    }
                    _processId = value;
                    OnEntityPropertyChanged("ProcessId");
                }
            }
        }
        
        private bool _isDeleted;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public new bool IsDeleted
        {
            get { return _isDeleted; }
            set
            {
                if (_isDeleted != value)
                {
                    ChangeTracker.RecordValue("IsDeleted", _isDeleted, value);
                    _isDeleted = value;
                    OnEntityPropertyChanged("IsDeleted");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Procedure Process
        {
            get { return _process; }
            set
            {
                if (!ReferenceEquals(_process, value))
                {
                    var previousValue = _process;
                    _process = value;
                    FixupProcess(previousValue);
                    OnNavigationPropertyChanged("Process");
    				OnProcessChanged(previousValue, value);
    				OnProcessChangedPartial(previousValue, value);
                }
            }
        }
        private Procedure _process;
    				
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Process"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnProcessChangedPartial(Procedure oldValue, Procedure newValue);
    	public event EventHandler<PropertyChangedEventArgs<Procedure>> ProcessChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Process"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnProcessChanged(Procedure oldValue, Procedure newValue)
    	{
    		if (ProcessChanged != null)
    			ProcessChanged(this, new PropertyChangedEventArgs<Procedure>(oldValue, newValue));
    	}

        #endregion

        #region Suivi des changements
    
        /// <summary>
        /// Vide les propriétés de navigation.
        /// </summary>
        protected override void ClearNavigationProperties()
        {
            base.ClearNavigationProperties();
            Process = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Process.
        /// </summary>
        private void FixupProcess(Procedure previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Operators.Contains(this))
            {
                previousValue.Operators.Remove(this);
            }
    
            if (Process != null)
            {
                if (!Process.Operators.Contains(this))
                {
                    Process.Operators.Add(this);
                }
    
                ProcessId = Process.ProcessId;
            }
            else if (!skipKeys)
            {
                ProcessId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("Process", previousValue, Process);
                if (Process != null && !Process.ChangeTracker.ChangeTrackingEnabled)
                {
                    Process.StartTracking();
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
    	public override void SetPropertyValue(string propertyName, object value)
    	{
    		base.SetPropertyValue(propertyName, value);
    		switch (propertyName)
    		{
    			case "ProcessId":
    				this.ProcessId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "Process":
    				this.Process = (Procedure)value;
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
    	public override void AddItemToCollection(string propertyName, object value)
    	{
    		base.AddItemToCollection(propertyName, value);
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
    	public override void RemoveItemFromCollection(string propertyName, object value)
    	{
    		base.RemoveItemFromCollection(propertyName, value);
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
    	public override IDictionary<string,object> GetCurrentValues()
    	{
    		var values = new Dictionary<string,object>();
    		foreach	(var kvp in base.GetCurrentValues())
    			values.Add(kvp.Key, kvp.Value);
    		values.Add("ProcessId", this.ProcessId);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("Process", this.Process);
    
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public override IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		foreach	(var kvp in base.GetGraphValues())
    			values.Add(kvp.Key, kvp.Value);
    		values.Add("Process", this.Process);
    
    		return values;
    	}
    	
    	

        #endregion

    	
    }
}
