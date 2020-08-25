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
    [KnownType(typeof(DocumentationActionDraft))]
    [KnownType(typeof(DocumentationDraft))]
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentationActionDraftWBS : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.DocumentationActionDraftWBS";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="DocumentationActionDraftWBS"/>.
        /// </summary>
    	public DocumentationActionDraftWBS()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _documentationActionDraftWBSId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int DocumentationActionDraftWBSId
        {
            get { return _documentationActionDraftWBSId; }
            set
            {
                if (_documentationActionDraftWBSId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'DocumentationActionDraftWBSId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _documentationActionDraftWBSId = value;
                    OnEntityPropertyChanged("DocumentationActionDraftWBSId");
                }
            }
        }
        
        private Nullable<int> _actionId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> ActionId
        {
            get { return _actionId; }
            set
            {
                if (_actionId != value)
                {
                    ChangeTracker.RecordValue("ActionId", _actionId, value);
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
        
        private Nullable<int> _documentationActionDraftId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<int> DocumentationActionDraftId
        {
            get { return _documentationActionDraftId; }
            set
            {
                if (_documentationActionDraftId != value)
                {
                    ChangeTracker.RecordValue("DocumentationActionDraftId", _documentationActionDraftId, value);
                    if (!IsDeserializing)
                    {
                        if (DocumentationActionDraft != null && DocumentationActionDraft.DocumentationActionDraftId != value)
                        {
                            DocumentationActionDraft = null;
                        }
                    }
                    _documentationActionDraftId = value;
                    OnEntityPropertyChanged("DocumentationActionDraftId");
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
        
        private int _documentationDraftId;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int DocumentationDraftId
        {
            get { return _documentationDraftId; }
            set
            {
                if (_documentationDraftId != value)
                {
                    ChangeTracker.RecordValue("DocumentationDraftId", _documentationDraftId, value);
                    if (!IsDeserializing)
                    {
                        if (DocumentationDraft != null && DocumentationDraft.DocumentationDraftId != value)
                        {
                            DocumentationDraft = null;
                        }
                    }
                    _documentationDraftId = value;
                    OnEntityPropertyChanged("DocumentationDraftId");
                }
            }
        }
        
        private int _documentationPublishMode;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int DocumentationPublishMode
        {
            get { return _documentationPublishMode; }
            set
            {
                if (_documentationPublishMode != value)
                {
                    ChangeTracker.RecordValue("DocumentationPublishMode", _documentationPublishMode, value);
                    _documentationPublishMode = value;
                    OnEntityPropertyChanged("DocumentationPublishMode");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public KAction Action
        {
            get { return _action; }
            set
            {
                if (!ReferenceEquals(_action, value))
                {
                    var previousValue = _action;
                    _action = value;
                    FixupAction(previousValue);
                    OnNavigationPropertyChanged("Action");
                }
            }
        }
        private KAction _action;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DocumentationActionDraft DocumentationActionDraft
        {
            get { return _documentationActionDraft; }
            set
            {
                if (!ReferenceEquals(_documentationActionDraft, value))
                {
                    var previousValue = _documentationActionDraft;
                    _documentationActionDraft = value;
                    FixupDocumentationActionDraft(previousValue);
                    OnNavigationPropertyChanged("DocumentationActionDraft");
                }
            }
        }
        private DocumentationActionDraft _documentationActionDraft;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
    	[ScriptIgnore]
        public DocumentationDraft DocumentationDraft
        {
            get { return _documentationDraft; }
            set
            {
                if (!ReferenceEquals(_documentationDraft, value))
                {
                    var previousValue = _documentationDraft;
                    _documentationDraft = value;
                    FixupDocumentationDraft(previousValue);
                    OnNavigationPropertyChanged("DocumentationDraft");
                }
            }
        }
        private DocumentationDraft _documentationDraft;
    				

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
            DocumentationActionDraft = null;
            DocumentationDraft = null;
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Action.
        /// </summary>
        private void FixupAction(KAction previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDraftWBS.Contains(this))
            {
                previousValue.DocumentationActionDraftWBS.Remove(this);
            }
    
            if (Action != null)
            {
                if (!Action.DocumentationActionDraftWBS.Contains(this))
                {
                    Action.DocumentationActionDraftWBS.Add(this);
                }
    
                ActionId = Action.ActionId;
            }
            else if (!skipKeys)
            {
                ActionId = null;
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
        /// Corrige l'état de la propriété de navigation DocumentationActionDraft.
        /// </summary>
        private void FixupDocumentationActionDraft(DocumentationActionDraft previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDraftWBS.Contains(this))
            {
                previousValue.DocumentationActionDraftWBS.Remove(this);
            }
    
            if (DocumentationActionDraft != null)
            {
                if (!DocumentationActionDraft.DocumentationActionDraftWBS.Contains(this))
                {
                    DocumentationActionDraft.DocumentationActionDraftWBS.Add(this);
                }
    
                DocumentationActionDraftId = DocumentationActionDraft.DocumentationActionDraftId;
            }
            else if (!skipKeys)
            {
                DocumentationActionDraftId = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DocumentationActionDraft", previousValue, DocumentationActionDraft);
                if (DocumentationActionDraft != null && !DocumentationActionDraft.ChangeTracker.ChangeTrackingEnabled)
                {
                    DocumentationActionDraft.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation DocumentationDraft.
        /// </summary>
        private void FixupDocumentationDraft(DocumentationDraft previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.DocumentationActionDraftWBS.Contains(this))
            {
                previousValue.DocumentationActionDraftWBS.Remove(this);
            }
    
            if (DocumentationDraft != null)
            {
                if (!DocumentationDraft.DocumentationActionDraftWBS.Contains(this))
                {
                    DocumentationDraft.DocumentationActionDraftWBS.Add(this);
                }
    
                DocumentationDraftId = DocumentationDraft.DocumentationDraftId;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DocumentationDraft", previousValue, DocumentationDraft);
                if (DocumentationDraft != null && !DocumentationDraft.ChangeTracker.ChangeTrackingEnabled)
                {
                    DocumentationDraft.StartTracking();
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
    			case "DocumentationActionDraftWBSId":
    				this.DocumentationActionDraftWBSId = Convert.ToInt32(value);
    				break;
    			case "ActionId":
    				this.ActionId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "DocumentationActionDraftId":
    				this.DocumentationActionDraftId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "WBS":
    				this.WBS = (string)value;
    				break;
    			case "DocumentationDraftId":
    				this.DocumentationDraftId = Convert.ToInt32(value);
    				break;
    			case "DocumentationPublishMode":
    				this.DocumentationPublishMode = Convert.ToInt32(value);
    				break;
    			case "Action":
    				this.Action = (KAction)value;
    				break;
    			case "DocumentationActionDraft":
    				this.DocumentationActionDraft = (DocumentationActionDraft)value;
    				break;
    			case "DocumentationDraft":
    				this.DocumentationDraft = (DocumentationDraft)value;
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
    		values.Add("DocumentationActionDraftWBSId", this.DocumentationActionDraftWBSId);
    		values.Add("ActionId", this.ActionId);
    		values.Add("DocumentationActionDraftId", this.DocumentationActionDraftId);
    		values.Add("WBS", this.WBS);
    		values.Add("DocumentationDraftId", this.DocumentationDraftId);
    		values.Add("DocumentationPublishMode", this.DocumentationPublishMode);
    		values.Add("Action", this.Action);
    		values.Add("DocumentationActionDraft", this.DocumentationActionDraft);
    		values.Add("DocumentationDraft", this.DocumentationDraft);
    
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
    		values.Add("DocumentationActionDraft", this.DocumentationActionDraft);
    		values.Add("DocumentationDraft", this.DocumentationDraft);
    
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
    }
}
