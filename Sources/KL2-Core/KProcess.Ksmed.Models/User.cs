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
    [KnownType(typeof(AppResourceValue))]
    [KnownType(typeof(Language))]
    [KnownType(typeof(Project))]
    [KnownType(typeof(Ref1))]
    [KnownType(typeof(Ref2))]
    [KnownType(typeof(Ref3))]
    [KnownType(typeof(Ref4))]
    [KnownType(typeof(Ref5))]
    [KnownType(typeof(Ref6))]
    [KnownType(typeof(Ref7))]
    [KnownType(typeof(ActionCategory))]
    [KnownType(typeof(Resource))]
    [KnownType(typeof(Scenario))]
    [KnownType(typeof(User))]
    [KnownType(typeof(Video))]
    [KnownType(typeof(Role))]
    [KnownType(typeof(Publication))]
    [KnownType(typeof(Team))]
    [KnownType(typeof(UserReadPublication))]
    [KnownType(typeof(Training))]
    [KnownType(typeof(ValidationTraining))]
    [KnownType(typeof(Qualification))]
    [KnownType(typeof(QualificationStep))]
    [KnownType(typeof(Skill))]
    [KnownType(typeof(InspectionStep))]
    [KnownType(typeof(Anomaly))]
    [KnownType(typeof(Audit))]
    [KnownType(typeof(VideoSync))]
    [KnownType(typeof(Procedure))]
    [KnownType(typeof(UserRoleProcess))]
    [KnownType(typeof(PublicationHistory))]
    /// <summary>
    /// Représente un utilisateur.
    /// </summary>
    public partial class User : ModelBase, IObjectWithChangeTracker, INotifyPropertyChanged
    {
    
        #region Constantes
    
        public const string TypeFullName = "KProcess.Ksmed.Models.User";

        #endregion

        #region Constructeurs et initialisation
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="User"/>.
        /// </summary>
    	public User()
    	{
    		this.Initialize();
    	}
    	
        /// <summary>
        /// Initialise cette instance.
        /// </summary>
        partial void Initialize();

        #endregion

        #region Propriétés primitives
        
        private int _userId;
        /// <summary>
        /// Obtient ou définit l'identifiant d'un utilisateur.
        /// </summary>
        [DataMember]
        public int UserId
        {
            get { return _userId; }
            set
            {
                if (_userId != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("The property 'UserId' is part of the object's key and cannot be changed. Changes to key properties can only be made when the object is not being tracked or is in the Added state.");
                    }
                    _userId = value;
                    OnEntityPropertyChanged("UserId");
                }
            }
        }
        
        private string _defaultLanguageCode;
        /// <summary>
        /// Obtient ou définit la langue par défaut de cet utilisateur.
        /// </summary>
        [DataMember]
        public string DefaultLanguageCode
        {
            get { return _defaultLanguageCode; }
            set
            {
                if (_defaultLanguageCode != value)
                {
                    ChangeTracker.RecordValue("DefaultLanguageCode", _defaultLanguageCode, value);
                    if (!IsDeserializing)
                    {
                        if (DefaultLanguage != null && DefaultLanguage.LanguageCode != value)
                        {
                            DefaultLanguage = null;
                        }
                    }
                    _defaultLanguageCode = value;
                    OnEntityPropertyChanged("DefaultLanguageCode");
                }
            }
        }
        
        private string _username;
        /// <summary>
        /// Obtient ou définit le nom d'utilisateur (login).
        /// </summary>
        [DataMember]
    	[LocalizableRequired(ErrorMessageResourceName = "Validation_User_Username_Required")]
    	[LocalizableStringLength(UsernameMaxLength, ErrorMessageResourceName = "Validation_User_Username_StringLength")]
        public string Username
        {
            get { return _username; }
            set
            {
                if (_username != value)
                {
                    ChangeTracker.RecordValue("Username", _username, value);
    				var oldValue = _username;
                    _username = value;
                    OnEntityPropertyChanged("Username");
    				OnUsernameChanged(oldValue, value);
    				OnUsernameChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Username"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnUsernameChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> UsernameChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Username"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnUsernameChanged(string oldValue, string newValue)
    	{
    		if (UsernameChanged != null)
    			UsernameChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private byte[] _password;
        /// <summary>
        /// Obtient ou définit le mot de passe.
        /// </summary>
        [DataMember]
        public byte[] Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    ChangeTracker.RecordValue("Password", _password, value);
                    _password = value;
                    OnEntityPropertyChanged("Password");
                }
            }
        }
        
        private string _firstname;
        /// <summary>
        /// Obtient ou définit le prénom.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(FirstnameMaxLength, ErrorMessageResourceName = "Validation_User_Firstname_StringLength")]
        public string Firstname
        {
            get { return _firstname; }
            set
            {
                if (_firstname != value)
                {
                    ChangeTracker.RecordValue("Firstname", _firstname, value);
    				var oldValue = _firstname;
                    _firstname = value;
                    OnEntityPropertyChanged("Firstname");
    				OnFirstnameChanged(oldValue, value);
    				OnFirstnameChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Firstname"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnFirstnameChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> FirstnameChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Firstname"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnFirstnameChanged(string oldValue, string newValue)
    	{
    		if (FirstnameChanged != null)
    			FirstnameChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private string _name;
        /// <summary>
        /// Obtient ou définit le nom.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(NameMaxLength, ErrorMessageResourceName = "Validation_User_Name_StringLength")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    ChangeTracker.RecordValue("Name", _name, value);
    				var oldValue = _name;
                    _name = value;
                    OnEntityPropertyChanged("Name");
    				OnNameChanged(oldValue, value);
    				OnNameChangedPartial(oldValue, value);
                }
            }
        }
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Name"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	partial void OnNameChangedPartial(string oldValue, string newValue);
    	public event EventHandler<PropertyChangedEventArgs<string>> NameChanged;
        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="Name"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
    	protected virtual void OnNameChanged(string oldValue, string newValue)
    	{
    		if (NameChanged != null)
    			NameChanged(this, new PropertyChangedEventArgs<string>(oldValue, newValue));
    	}
        
        private string _email;
        /// <summary>
        /// Obtient ou définit l'addresse e-mail.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(EmailMaxLength, ErrorMessageResourceName = "Validation_User_Email_StringLength")]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    ChangeTracker.RecordValue("Email", _email, value);
                    _email = value;
                    OnEntityPropertyChanged("Email");
                }
            }
        }
        
        private string _phoneNumber;
        /// <summary>
        /// Obtient ou définit le numéro de téléphone.
        /// </summary>
        [DataMember]
    	[LocalizableStringLength(PhoneNumberMaxLength, ErrorMessageResourceName = "Validation_User_PhoneNumber_StringLength")]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (_phoneNumber != value)
                {
                    ChangeTracker.RecordValue("PhoneNumber", _phoneNumber, value);
                    _phoneNumber = value;
                    OnEntityPropertyChanged("PhoneNumber");
                }
            }
        }
        
        private bool _isDeleted;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur est supprimé.
        /// </summary>
        [DataMember]
        public bool IsDeleted
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
        
        private Nullable<int> _createdByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a créé l'entité.
        /// </summary>
        [DataMember]
        public Nullable<int> CreatedByUserId
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
        
        private Nullable<int> _modifiedByUserId;
        /// <summary>
        /// Obtient ou définit l'identifiant de l'utilisateur qui a en dernier modifié l'entité.
        /// </summary>
        [DataMember]
        public Nullable<int> ModifiedByUserId
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
        
        private Nullable<bool> _tenured;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public Nullable<bool> Tenured
        {
            get { return _tenured; }
            set
            {
                if (_tenured != value)
                {
                    ChangeTracker.RecordValue("Tenured", _tenured, value);
                    _tenured = value;
                    OnEntityPropertyChanged("Tenured");
                }
            }
        }

        #endregion

        #region Propriétés de navigation
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> CreatedActions
        {
            get
            {
                if (_createdActions == null)
                {
                    _createdActions = new TrackableCollection<KAction>();
                    _createdActions.CollectionChanged += FixupCreatedActions;
                }
                return _createdActions;
            }
            set
            {
                if (!ReferenceEquals(_createdActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdActions != null)
                    {
                        _createdActions.CollectionChanged -= FixupCreatedActions;
                    }
                    _createdActions = value;
                    if (_createdActions != null)
                    {
                        _createdActions.CollectionChanged += FixupCreatedActions;
                    }
                    OnNavigationPropertyChanged("CreatedActions");
                }
            }
        }
        private TrackableCollection<KAction> _createdActions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<KAction> LastModifiedActions
        {
            get
            {
                if (_lastModifiedActions == null)
                {
                    _lastModifiedActions = new TrackableCollection<KAction>();
                    _lastModifiedActions.CollectionChanged += FixupLastModifiedActions;
                }
                return _lastModifiedActions;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedActions, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedActions != null)
                    {
                        _lastModifiedActions.CollectionChanged -= FixupLastModifiedActions;
                    }
                    _lastModifiedActions = value;
                    if (_lastModifiedActions != null)
                    {
                        _lastModifiedActions.CollectionChanged += FixupLastModifiedActions;
                    }
                    OnNavigationPropertyChanged("LastModifiedActions");
                }
            }
        }
        private TrackableCollection<KAction> _lastModifiedActions;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<AppResourceValue> CreatedAppResourceValues
        {
            get
            {
                if (_createdAppResourceValues == null)
                {
                    _createdAppResourceValues = new TrackableCollection<AppResourceValue>();
                    _createdAppResourceValues.CollectionChanged += FixupCreatedAppResourceValues;
                }
                return _createdAppResourceValues;
            }
            set
            {
                if (!ReferenceEquals(_createdAppResourceValues, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdAppResourceValues != null)
                    {
                        _createdAppResourceValues.CollectionChanged -= FixupCreatedAppResourceValues;
                    }
                    _createdAppResourceValues = value;
                    if (_createdAppResourceValues != null)
                    {
                        _createdAppResourceValues.CollectionChanged += FixupCreatedAppResourceValues;
                    }
                    OnNavigationPropertyChanged("CreatedAppResourceValues");
                }
            }
        }
        private TrackableCollection<AppResourceValue> _createdAppResourceValues;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<AppResourceValue> LastModifiedAppResourceValues
        {
            get
            {
                if (_lastModifiedAppResourceValues == null)
                {
                    _lastModifiedAppResourceValues = new TrackableCollection<AppResourceValue>();
                    _lastModifiedAppResourceValues.CollectionChanged += FixupLastModifiedAppResourceValues;
                }
                return _lastModifiedAppResourceValues;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedAppResourceValues, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedAppResourceValues != null)
                    {
                        _lastModifiedAppResourceValues.CollectionChanged -= FixupLastModifiedAppResourceValues;
                    }
                    _lastModifiedAppResourceValues = value;
                    if (_lastModifiedAppResourceValues != null)
                    {
                        _lastModifiedAppResourceValues.CollectionChanged += FixupLastModifiedAppResourceValues;
                    }
                    OnNavigationPropertyChanged("LastModifiedAppResourceValues");
                }
            }
        }
        private TrackableCollection<AppResourceValue> _lastModifiedAppResourceValues;
    
        /// <summary>
        /// Obtient ou définit la langue par défaut associée.
        /// </summary>
        [DataMember]
        public Language DefaultLanguage
        {
            get { return _defaultLanguage; }
            set
            {
                if (!ReferenceEquals(_defaultLanguage, value))
                {
                    var previousValue = _defaultLanguage;
                    _defaultLanguage = value;
                    FixupDefaultLanguage(previousValue);
                    OnNavigationPropertyChanged("DefaultLanguage");
                }
            }
        }
        private Language _defaultLanguage;
    				
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Project> CreatedProjects
        {
            get
            {
                if (_createdProjects == null)
                {
                    _createdProjects = new TrackableCollection<Project>();
                    _createdProjects.CollectionChanged += FixupCreatedProjects;
                }
                return _createdProjects;
            }
            set
            {
                if (!ReferenceEquals(_createdProjects, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdProjects != null)
                    {
                        _createdProjects.CollectionChanged -= FixupCreatedProjects;
                    }
                    _createdProjects = value;
                    if (_createdProjects != null)
                    {
                        _createdProjects.CollectionChanged += FixupCreatedProjects;
                    }
                    OnNavigationPropertyChanged("CreatedProjects");
                }
            }
        }
        private TrackableCollection<Project> _createdProjects;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Project> LastModifiedProjects
        {
            get
            {
                if (_lastModifiedProjects == null)
                {
                    _lastModifiedProjects = new TrackableCollection<Project>();
                    _lastModifiedProjects.CollectionChanged += FixupLastModifiedProjects;
                }
                return _lastModifiedProjects;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedProjects, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedProjects != null)
                    {
                        _lastModifiedProjects.CollectionChanged -= FixupLastModifiedProjects;
                    }
                    _lastModifiedProjects = value;
                    if (_lastModifiedProjects != null)
                    {
                        _lastModifiedProjects.CollectionChanged += FixupLastModifiedProjects;
                    }
                    OnNavigationPropertyChanged("LastModifiedProjects");
                }
            }
        }
        private TrackableCollection<Project> _lastModifiedProjects;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref1> CreatedRefs1
        {
            get
            {
                if (_createdRefs1 == null)
                {
                    _createdRefs1 = new TrackableCollection<Ref1>();
                    _createdRefs1.CollectionChanged += FixupCreatedRefs1;
                }
                return _createdRefs1;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs1 != null)
                    {
                        _createdRefs1.CollectionChanged -= FixupCreatedRefs1;
                    }
                    _createdRefs1 = value;
                    if (_createdRefs1 != null)
                    {
                        _createdRefs1.CollectionChanged += FixupCreatedRefs1;
                    }
                    OnNavigationPropertyChanged("CreatedRefs1");
                }
            }
        }
        private TrackableCollection<Ref1> _createdRefs1;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref1> LastModifiedRefs1
        {
            get
            {
                if (_lastModifiedRefs1 == null)
                {
                    _lastModifiedRefs1 = new TrackableCollection<Ref1>();
                    _lastModifiedRefs1.CollectionChanged += FixupLastModifiedRefs1;
                }
                return _lastModifiedRefs1;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs1 != null)
                    {
                        _lastModifiedRefs1.CollectionChanged -= FixupLastModifiedRefs1;
                    }
                    _lastModifiedRefs1 = value;
                    if (_lastModifiedRefs1 != null)
                    {
                        _lastModifiedRefs1.CollectionChanged += FixupLastModifiedRefs1;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs1");
                }
            }
        }
        private TrackableCollection<Ref1> _lastModifiedRefs1;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref2> CreatedRefs2
        {
            get
            {
                if (_createdRefs2 == null)
                {
                    _createdRefs2 = new TrackableCollection<Ref2>();
                    _createdRefs2.CollectionChanged += FixupCreatedRefs2;
                }
                return _createdRefs2;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs2, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs2 != null)
                    {
                        _createdRefs2.CollectionChanged -= FixupCreatedRefs2;
                    }
                    _createdRefs2 = value;
                    if (_createdRefs2 != null)
                    {
                        _createdRefs2.CollectionChanged += FixupCreatedRefs2;
                    }
                    OnNavigationPropertyChanged("CreatedRefs2");
                }
            }
        }
        private TrackableCollection<Ref2> _createdRefs2;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref2> LastModifiedRefs2
        {
            get
            {
                if (_lastModifiedRefs2 == null)
                {
                    _lastModifiedRefs2 = new TrackableCollection<Ref2>();
                    _lastModifiedRefs2.CollectionChanged += FixupLastModifiedRefs2;
                }
                return _lastModifiedRefs2;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs2, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs2 != null)
                    {
                        _lastModifiedRefs2.CollectionChanged -= FixupLastModifiedRefs2;
                    }
                    _lastModifiedRefs2 = value;
                    if (_lastModifiedRefs2 != null)
                    {
                        _lastModifiedRefs2.CollectionChanged += FixupLastModifiedRefs2;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs2");
                }
            }
        }
        private TrackableCollection<Ref2> _lastModifiedRefs2;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref3> CreatedRefs3
        {
            get
            {
                if (_createdRefs3 == null)
                {
                    _createdRefs3 = new TrackableCollection<Ref3>();
                    _createdRefs3.CollectionChanged += FixupCreatedRefs3;
                }
                return _createdRefs3;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs3, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs3 != null)
                    {
                        _createdRefs3.CollectionChanged -= FixupCreatedRefs3;
                    }
                    _createdRefs3 = value;
                    if (_createdRefs3 != null)
                    {
                        _createdRefs3.CollectionChanged += FixupCreatedRefs3;
                    }
                    OnNavigationPropertyChanged("CreatedRefs3");
                }
            }
        }
        private TrackableCollection<Ref3> _createdRefs3;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref3> LastModifiedRefs3
        {
            get
            {
                if (_lastModifiedRefs3 == null)
                {
                    _lastModifiedRefs3 = new TrackableCollection<Ref3>();
                    _lastModifiedRefs3.CollectionChanged += FixupLastModifiedRefs3;
                }
                return _lastModifiedRefs3;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs3, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs3 != null)
                    {
                        _lastModifiedRefs3.CollectionChanged -= FixupLastModifiedRefs3;
                    }
                    _lastModifiedRefs3 = value;
                    if (_lastModifiedRefs3 != null)
                    {
                        _lastModifiedRefs3.CollectionChanged += FixupLastModifiedRefs3;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs3");
                }
            }
        }
        private TrackableCollection<Ref3> _lastModifiedRefs3;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref4> CreatedRefs4
        {
            get
            {
                if (_createdRefs4 == null)
                {
                    _createdRefs4 = new TrackableCollection<Ref4>();
                    _createdRefs4.CollectionChanged += FixupCreatedRefs4;
                }
                return _createdRefs4;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs4, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs4 != null)
                    {
                        _createdRefs4.CollectionChanged -= FixupCreatedRefs4;
                    }
                    _createdRefs4 = value;
                    if (_createdRefs4 != null)
                    {
                        _createdRefs4.CollectionChanged += FixupCreatedRefs4;
                    }
                    OnNavigationPropertyChanged("CreatedRefs4");
                }
            }
        }
        private TrackableCollection<Ref4> _createdRefs4;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref4> LastModifiedRefs4
        {
            get
            {
                if (_lastModifiedRefs4 == null)
                {
                    _lastModifiedRefs4 = new TrackableCollection<Ref4>();
                    _lastModifiedRefs4.CollectionChanged += FixupLastModifiedRefs4;
                }
                return _lastModifiedRefs4;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs4, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs4 != null)
                    {
                        _lastModifiedRefs4.CollectionChanged -= FixupLastModifiedRefs4;
                    }
                    _lastModifiedRefs4 = value;
                    if (_lastModifiedRefs4 != null)
                    {
                        _lastModifiedRefs4.CollectionChanged += FixupLastModifiedRefs4;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs4");
                }
            }
        }
        private TrackableCollection<Ref4> _lastModifiedRefs4;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref5> CreatedRefs5
        {
            get
            {
                if (_createdRefs5 == null)
                {
                    _createdRefs5 = new TrackableCollection<Ref5>();
                    _createdRefs5.CollectionChanged += FixupCreatedRefs5;
                }
                return _createdRefs5;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs5, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs5 != null)
                    {
                        _createdRefs5.CollectionChanged -= FixupCreatedRefs5;
                    }
                    _createdRefs5 = value;
                    if (_createdRefs5 != null)
                    {
                        _createdRefs5.CollectionChanged += FixupCreatedRefs5;
                    }
                    OnNavigationPropertyChanged("CreatedRefs5");
                }
            }
        }
        private TrackableCollection<Ref5> _createdRefs5;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref5> LastModifiedRefs5
        {
            get
            {
                if (_lastModifiedRefs5 == null)
                {
                    _lastModifiedRefs5 = new TrackableCollection<Ref5>();
                    _lastModifiedRefs5.CollectionChanged += FixupLastModifiedRefs5;
                }
                return _lastModifiedRefs5;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs5, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs5 != null)
                    {
                        _lastModifiedRefs5.CollectionChanged -= FixupLastModifiedRefs5;
                    }
                    _lastModifiedRefs5 = value;
                    if (_lastModifiedRefs5 != null)
                    {
                        _lastModifiedRefs5.CollectionChanged += FixupLastModifiedRefs5;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs5");
                }
            }
        }
        private TrackableCollection<Ref5> _lastModifiedRefs5;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref6> CreatedRefs6
        {
            get
            {
                if (_createdRefs6 == null)
                {
                    _createdRefs6 = new TrackableCollection<Ref6>();
                    _createdRefs6.CollectionChanged += FixupCreatedRefs6;
                }
                return _createdRefs6;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs6, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs6 != null)
                    {
                        _createdRefs6.CollectionChanged -= FixupCreatedRefs6;
                    }
                    _createdRefs6 = value;
                    if (_createdRefs6 != null)
                    {
                        _createdRefs6.CollectionChanged += FixupCreatedRefs6;
                    }
                    OnNavigationPropertyChanged("CreatedRefs6");
                }
            }
        }
        private TrackableCollection<Ref6> _createdRefs6;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref6> LastModifiedRefs6
        {
            get
            {
                if (_lastModifiedRefs6 == null)
                {
                    _lastModifiedRefs6 = new TrackableCollection<Ref6>();
                    _lastModifiedRefs6.CollectionChanged += FixupLastModifiedRefs6;
                }
                return _lastModifiedRefs6;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs6, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs6 != null)
                    {
                        _lastModifiedRefs6.CollectionChanged -= FixupLastModifiedRefs6;
                    }
                    _lastModifiedRefs6 = value;
                    if (_lastModifiedRefs6 != null)
                    {
                        _lastModifiedRefs6.CollectionChanged += FixupLastModifiedRefs6;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs6");
                }
            }
        }
        private TrackableCollection<Ref6> _lastModifiedRefs6;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref7> CreatedRefs7
        {
            get
            {
                if (_createdRefs7 == null)
                {
                    _createdRefs7 = new TrackableCollection<Ref7>();
                    _createdRefs7.CollectionChanged += FixupCreatedRefs7;
                }
                return _createdRefs7;
            }
            set
            {
                if (!ReferenceEquals(_createdRefs7, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdRefs7 != null)
                    {
                        _createdRefs7.CollectionChanged -= FixupCreatedRefs7;
                    }
                    _createdRefs7 = value;
                    if (_createdRefs7 != null)
                    {
                        _createdRefs7.CollectionChanged += FixupCreatedRefs7;
                    }
                    OnNavigationPropertyChanged("CreatedRefs7");
                }
            }
        }
        private TrackableCollection<Ref7> _createdRefs7;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Ref7> LastModifiedRefs7
        {
            get
            {
                if (_lastModifiedRefs7 == null)
                {
                    _lastModifiedRefs7 = new TrackableCollection<Ref7>();
                    _lastModifiedRefs7.CollectionChanged += FixupLastModifiedRefs7;
                }
                return _lastModifiedRefs7;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedRefs7, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedRefs7 != null)
                    {
                        _lastModifiedRefs7.CollectionChanged -= FixupLastModifiedRefs7;
                    }
                    _lastModifiedRefs7 = value;
                    if (_lastModifiedRefs7 != null)
                    {
                        _lastModifiedRefs7.CollectionChanged += FixupLastModifiedRefs7;
                    }
                    OnNavigationPropertyChanged("LastModifiedRefs7");
                }
            }
        }
        private TrackableCollection<Ref7> _lastModifiedRefs7;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionCategory> CreatedActionCategories
        {
            get
            {
                if (_createdActionCategories == null)
                {
                    _createdActionCategories = new TrackableCollection<ActionCategory>();
                    _createdActionCategories.CollectionChanged += FixupCreatedActionCategories;
                }
                return _createdActionCategories;
            }
            set
            {
                if (!ReferenceEquals(_createdActionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdActionCategories != null)
                    {
                        _createdActionCategories.CollectionChanged -= FixupCreatedActionCategories;
                    }
                    _createdActionCategories = value;
                    if (_createdActionCategories != null)
                    {
                        _createdActionCategories.CollectionChanged += FixupCreatedActionCategories;
                    }
                    OnNavigationPropertyChanged("CreatedActionCategories");
                }
            }
        }
        private TrackableCollection<ActionCategory> _createdActionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<ActionCategory> LastModifiedActionCategories
        {
            get
            {
                if (_lastModifiedActionCategories == null)
                {
                    _lastModifiedActionCategories = new TrackableCollection<ActionCategory>();
                    _lastModifiedActionCategories.CollectionChanged += FixupLastModifiedActionCategories;
                }
                return _lastModifiedActionCategories;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedActionCategories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedActionCategories != null)
                    {
                        _lastModifiedActionCategories.CollectionChanged -= FixupLastModifiedActionCategories;
                    }
                    _lastModifiedActionCategories = value;
                    if (_lastModifiedActionCategories != null)
                    {
                        _lastModifiedActionCategories.CollectionChanged += FixupLastModifiedActionCategories;
                    }
                    OnNavigationPropertyChanged("LastModifiedActionCategories");
                }
            }
        }
        private TrackableCollection<ActionCategory> _lastModifiedActionCategories;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Resource> CreatedResources
        {
            get
            {
                if (_createdResources == null)
                {
                    _createdResources = new TrackableCollection<Resource>();
                    _createdResources.CollectionChanged += FixupCreatedResources;
                }
                return _createdResources;
            }
            set
            {
                if (!ReferenceEquals(_createdResources, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdResources != null)
                    {
                        _createdResources.CollectionChanged -= FixupCreatedResources;
                    }
                    _createdResources = value;
                    if (_createdResources != null)
                    {
                        _createdResources.CollectionChanged += FixupCreatedResources;
                    }
                    OnNavigationPropertyChanged("CreatedResources");
                }
            }
        }
        private TrackableCollection<Resource> _createdResources;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Resource> LastModifiedResources
        {
            get
            {
                if (_lastModifiedResources == null)
                {
                    _lastModifiedResources = new TrackableCollection<Resource>();
                    _lastModifiedResources.CollectionChanged += FixupLastModifiedResources;
                }
                return _lastModifiedResources;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedResources, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedResources != null)
                    {
                        _lastModifiedResources.CollectionChanged -= FixupLastModifiedResources;
                    }
                    _lastModifiedResources = value;
                    if (_lastModifiedResources != null)
                    {
                        _lastModifiedResources.CollectionChanged += FixupLastModifiedResources;
                    }
                    OnNavigationPropertyChanged("LastModifiedResources");
                }
            }
        }
        private TrackableCollection<Resource> _lastModifiedResources;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Scenario> CreatedScenarios
        {
            get
            {
                if (_createdScenarios == null)
                {
                    _createdScenarios = new TrackableCollection<Scenario>();
                    _createdScenarios.CollectionChanged += FixupCreatedScenarios;
                }
                return _createdScenarios;
            }
            set
            {
                if (!ReferenceEquals(_createdScenarios, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdScenarios != null)
                    {
                        _createdScenarios.CollectionChanged -= FixupCreatedScenarios;
                    }
                    _createdScenarios = value;
                    if (_createdScenarios != null)
                    {
                        _createdScenarios.CollectionChanged += FixupCreatedScenarios;
                    }
                    OnNavigationPropertyChanged("CreatedScenarios");
                }
            }
        }
        private TrackableCollection<Scenario> _createdScenarios;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Scenario> LastModifiedScenarios
        {
            get
            {
                if (_lastModifiedScenarios == null)
                {
                    _lastModifiedScenarios = new TrackableCollection<Scenario>();
                    _lastModifiedScenarios.CollectionChanged += FixupLastModifiedScenarios;
                }
                return _lastModifiedScenarios;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedScenarios, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedScenarios != null)
                    {
                        _lastModifiedScenarios.CollectionChanged -= FixupLastModifiedScenarios;
                    }
                    _lastModifiedScenarios = value;
                    if (_lastModifiedScenarios != null)
                    {
                        _lastModifiedScenarios.CollectionChanged += FixupLastModifiedScenarios;
                    }
                    OnNavigationPropertyChanged("LastModifiedScenarios");
                }
            }
        }
        private TrackableCollection<Scenario> _lastModifiedScenarios;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<User> CreatedUsers
        {
            get
            {
                if (_createdUsers == null)
                {
                    _createdUsers = new TrackableCollection<User>();
                    _createdUsers.CollectionChanged += FixupCreatedUsers;
                }
                return _createdUsers;
            }
            set
            {
                if (!ReferenceEquals(_createdUsers, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdUsers != null)
                    {
                        _createdUsers.CollectionChanged -= FixupCreatedUsers;
                    }
                    _createdUsers = value;
                    if (_createdUsers != null)
                    {
                        _createdUsers.CollectionChanged += FixupCreatedUsers;
                    }
                    OnNavigationPropertyChanged("CreatedUsers");
                }
            }
        }
        private TrackableCollection<User> _createdUsers;
    
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
        public TrackableCollection<User> LastModifiedUsers
        {
            get
            {
                if (_lastModifiedUsers == null)
                {
                    _lastModifiedUsers = new TrackableCollection<User>();
                    _lastModifiedUsers.CollectionChanged += FixupLastModifiedUsers;
                }
                return _lastModifiedUsers;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedUsers, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedUsers != null)
                    {
                        _lastModifiedUsers.CollectionChanged -= FixupLastModifiedUsers;
                    }
                    _lastModifiedUsers = value;
                    if (_lastModifiedUsers != null)
                    {
                        _lastModifiedUsers.CollectionChanged += FixupLastModifiedUsers;
                    }
                    OnNavigationPropertyChanged("LastModifiedUsers");
                }
            }
        }
        private TrackableCollection<User> _lastModifiedUsers;
    
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
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Video> CreatedVideos
        {
            get
            {
                if (_createdVideos == null)
                {
                    _createdVideos = new TrackableCollection<Video>();
                    _createdVideos.CollectionChanged += FixupCreatedVideos;
                }
                return _createdVideos;
            }
            set
            {
                if (!ReferenceEquals(_createdVideos, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_createdVideos != null)
                    {
                        _createdVideos.CollectionChanged -= FixupCreatedVideos;
                    }
                    _createdVideos = value;
                    if (_createdVideos != null)
                    {
                        _createdVideos.CollectionChanged += FixupCreatedVideos;
                    }
                    OnNavigationPropertyChanged("CreatedVideos");
                }
            }
        }
        private TrackableCollection<Video> _createdVideos;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Video> LastModifiedVideos
        {
            get
            {
                if (_lastModifiedVideos == null)
                {
                    _lastModifiedVideos = new TrackableCollection<Video>();
                    _lastModifiedVideos.CollectionChanged += FixupLastModifiedVideos;
                }
                return _lastModifiedVideos;
            }
            set
            {
                if (!ReferenceEquals(_lastModifiedVideos, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_lastModifiedVideos != null)
                    {
                        _lastModifiedVideos.CollectionChanged -= FixupLastModifiedVideos;
                    }
                    _lastModifiedVideos = value;
                    if (_lastModifiedVideos != null)
                    {
                        _lastModifiedVideos.CollectionChanged += FixupLastModifiedVideos;
                    }
                    OnNavigationPropertyChanged("LastModifiedVideos");
                }
            }
        }
        private TrackableCollection<Video> _lastModifiedVideos;
    
        /// <summary>
        /// Obtient ou définit les rôles de cet utilisateur.
        /// </summary>
        [DataMember]
        public TrackableCollection<Role> Roles
        {
            get
            {
                if (_roles == null)
                {
                    _roles = new TrackableCollection<Role>();
                    _roles.CollectionChanged += FixupRoles;
                }
                return _roles;
            }
            set
            {
                if (!ReferenceEquals(_roles, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_roles != null)
                    {
                        _roles.CollectionChanged -= FixupRoles;
                    }
                    _roles = value;
                    if (_roles != null)
                    {
                        _roles.CollectionChanged += FixupRoles;
                    }
                    OnNavigationPropertyChanged("Roles");
                }
            }
        }
        private TrackableCollection<Role> _roles;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Publication> Publication
        {
            get
            {
                if (_publication == null)
                {
                    _publication = new TrackableCollection<Publication>();
                    _publication.CollectionChanged += FixupPublication;
                }
                return _publication;
            }
            set
            {
                if (!ReferenceEquals(_publication, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publication != null)
                    {
                        _publication.CollectionChanged -= FixupPublication;
                    }
                    _publication = value;
                    if (_publication != null)
                    {
                        _publication.CollectionChanged += FixupPublication;
                    }
                    OnNavigationPropertyChanged("Publication");
                }
            }
        }
        private TrackableCollection<Publication> _publication;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Team> Teams
        {
            get
            {
                if (_teams == null)
                {
                    _teams = new TrackableCollection<Team>();
                    _teams.CollectionChanged += FixupTeams;
                }
                return _teams;
            }
            set
            {
                if (!ReferenceEquals(_teams, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_teams != null)
                    {
                        _teams.CollectionChanged -= FixupTeams;
                    }
                    _teams = value;
                    if (_teams != null)
                    {
                        _teams.CollectionChanged += FixupTeams;
                    }
                    OnNavigationPropertyChanged("Teams");
                }
            }
        }
        private TrackableCollection<Team> _teams;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<UserReadPublication> ReadPublications
        {
            get
            {
                if (_readPublications == null)
                {
                    _readPublications = new TrackableCollection<UserReadPublication>();
                    _readPublications.CollectionChanged += FixupReadPublications;
                }
                return _readPublications;
            }
            set
            {
                if (!ReferenceEquals(_readPublications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_readPublications != null)
                    {
                        _readPublications.CollectionChanged -= FixupReadPublications;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (UserReadPublication item in _readPublications)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _readPublications = value;
                    if (_readPublications != null)
                    {
                        _readPublications.CollectionChanged += FixupReadPublications;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (UserReadPublication item in _readPublications)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("ReadPublications");
                }
            }
        }
        private TrackableCollection<UserReadPublication> _readPublications;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Training> Trainings
        {
            get
            {
                if (_trainings == null)
                {
                    _trainings = new TrackableCollection<Training>();
                    _trainings.CollectionChanged += FixupTrainings;
                }
                return _trainings;
            }
            set
            {
                if (!ReferenceEquals(_trainings, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_trainings != null)
                    {
                        _trainings.CollectionChanged -= FixupTrainings;
                    }
                    _trainings = value;
                    if (_trainings != null)
                    {
                        _trainings.CollectionChanged += FixupTrainings;
                    }
                    OnNavigationPropertyChanged("Trainings");
                }
            }
        }
        private TrackableCollection<Training> _trainings;
    
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
        public TrackableCollection<Qualification> Qualifications
        {
            get
            {
                if (_qualifications == null)
                {
                    _qualifications = new TrackableCollection<Qualification>();
                    _qualifications.CollectionChanged += FixupQualifications;
                }
                return _qualifications;
            }
            set
            {
                if (!ReferenceEquals(_qualifications, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_qualifications != null)
                    {
                        _qualifications.CollectionChanged -= FixupQualifications;
                    }
                    _qualifications = value;
                    if (_qualifications != null)
                    {
                        _qualifications.CollectionChanged += FixupQualifications;
                    }
                    OnNavigationPropertyChanged("Qualifications");
                }
            }
        }
        private TrackableCollection<Qualification> _qualifications;
    
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
        public TrackableCollection<Skill> Skills1
        {
            get
            {
                if (_skills1 == null)
                {
                    _skills1 = new TrackableCollection<Skill>();
                    _skills1.CollectionChanged += FixupSkills1;
                }
                return _skills1;
            }
            set
            {
                if (!ReferenceEquals(_skills1, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_skills1 != null)
                    {
                        _skills1.CollectionChanged -= FixupSkills1;
                    }
                    _skills1 = value;
                    if (_skills1 != null)
                    {
                        _skills1.CollectionChanged += FixupSkills1;
                    }
                    OnNavigationPropertyChanged("Skills1");
                }
            }
        }
        private TrackableCollection<Skill> _skills1;
    
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
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Anomaly> Anomalies
        {
            get
            {
                if (_anomalies == null)
                {
                    _anomalies = new TrackableCollection<Anomaly>();
                    _anomalies.CollectionChanged += FixupAnomalies;
                }
                return _anomalies;
            }
            set
            {
                if (!ReferenceEquals(_anomalies, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_anomalies != null)
                    {
                        _anomalies.CollectionChanged -= FixupAnomalies;
                    }
                    _anomalies = value;
                    if (_anomalies != null)
                    {
                        _anomalies.CollectionChanged += FixupAnomalies;
                    }
                    OnNavigationPropertyChanged("Anomalies");
                }
            }
        }
        private TrackableCollection<Anomaly> _anomalies;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Audit> Audits
        {
            get
            {
                if (_audits == null)
                {
                    _audits = new TrackableCollection<Audit>();
                    _audits.CollectionChanged += FixupAudits;
                }
                return _audits;
            }
            set
            {
                if (!ReferenceEquals(_audits, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_audits != null)
                    {
                        _audits.CollectionChanged -= FixupAudits;
                    }
                    _audits = value;
                    if (_audits != null)
                    {
                        _audits.CollectionChanged += FixupAudits;
                    }
                    OnNavigationPropertyChanged("Audits");
                }
            }
        }
        private TrackableCollection<Audit> _audits;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<VideoSync> VideoSyncs
        {
            get
            {
                if (_videoSyncs == null)
                {
                    _videoSyncs = new TrackableCollection<VideoSync>();
                    _videoSyncs.CollectionChanged += FixupVideoSyncs;
                }
                return _videoSyncs;
            }
            set
            {
                if (!ReferenceEquals(_videoSyncs, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_videoSyncs != null)
                    {
                        _videoSyncs.CollectionChanged -= FixupVideoSyncs;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (VideoSync item in _videoSyncs)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _videoSyncs = value;
                    if (_videoSyncs != null)
                    {
                        _videoSyncs.CollectionChanged += FixupVideoSyncs;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (VideoSync item in _videoSyncs)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("VideoSyncs");
                }
            }
        }
        private TrackableCollection<VideoSync> _videoSyncs;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<Procedure> Procedures
        {
            get
            {
                if (_procedures == null)
                {
                    _procedures = new TrackableCollection<Procedure>();
                    _procedures.CollectionChanged += FixupProcedures;
                }
                return _procedures;
            }
            set
            {
                if (!ReferenceEquals(_procedures, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_procedures != null)
                    {
                        _procedures.CollectionChanged -= FixupProcedures;
                    }
                    _procedures = value;
                    if (_procedures != null)
                    {
                        _procedures.CollectionChanged += FixupProcedures;
                    }
                    OnNavigationPropertyChanged("Procedures");
                }
            }
        }
        private TrackableCollection<Procedure> _procedures;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<UserRoleProcess> UserRoleProcesses
        {
            get
            {
                if (_userRoleProcesses == null)
                {
                    _userRoleProcesses = new TrackableCollection<UserRoleProcess>();
                    _userRoleProcesses.CollectionChanged += FixupUserRoleProcesses;
                }
                return _userRoleProcesses;
            }
            set
            {
                if (!ReferenceEquals(_userRoleProcesses, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_userRoleProcesses != null)
                    {
                        _userRoleProcesses.CollectionChanged -= FixupUserRoleProcesses;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (UserRoleProcess item in _userRoleProcesses)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _userRoleProcesses = value;
                    if (_userRoleProcesses != null)
                    {
                        _userRoleProcesses.CollectionChanged += FixupUserRoleProcesses;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (UserRoleProcess item in _userRoleProcesses)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("UserRoleProcesses");
                }
            }
        }
        private TrackableCollection<UserRoleProcess> _userRoleProcesses;
    
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public TrackableCollection<PublicationHistory> PublicationHistories
        {
            get
            {
                if (_publicationHistories == null)
                {
                    _publicationHistories = new TrackableCollection<PublicationHistory>();
                    _publicationHistories.CollectionChanged += FixupPublicationHistories;
                }
                return _publicationHistories;
            }
            set
            {
                if (!ReferenceEquals(_publicationHistories, value))
                {
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        throw new InvalidOperationException("Cannot set the FixupChangeTrackingCollection when ChangeTracking is enabled");
                    }
                    if (_publicationHistories != null)
                    {
                        _publicationHistories.CollectionChanged -= FixupPublicationHistories;
                        // This is the principal end in an association that performs cascade deletes.
                        // Remove the cascade delete event handler for any entities in the current collection.
                        foreach (PublicationHistory item in _publicationHistories)
                        {
                            ChangeTracker.ObjectStateChanging -= item.HandleCascadeDelete;
                        }
                    }
                    _publicationHistories = value;
                    if (_publicationHistories != null)
                    {
                        _publicationHistories.CollectionChanged += FixupPublicationHistories;
                        // This is the principal end in an association that performs cascade deletes.
                        // Add the cascade delete event handler for any entities that are already in the new collection.
                        foreach (PublicationHistory item in _publicationHistories)
                        {
                            ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                        }
                    }
                    OnNavigationPropertyChanged("PublicationHistories");
                }
            }
        }
        private TrackableCollection<PublicationHistory> _publicationHistories;

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
            CreatedActions.Clear();
            LastModifiedActions.Clear();
            CreatedAppResourceValues.Clear();
            LastModifiedAppResourceValues.Clear();
            DefaultLanguage = null;
            CreatedProjects.Clear();
            LastModifiedProjects.Clear();
            CreatedRefs1.Clear();
            LastModifiedRefs1.Clear();
            CreatedRefs2.Clear();
            LastModifiedRefs2.Clear();
            CreatedRefs3.Clear();
            LastModifiedRefs3.Clear();
            CreatedRefs4.Clear();
            LastModifiedRefs4.Clear();
            CreatedRefs5.Clear();
            LastModifiedRefs5.Clear();
            CreatedRefs6.Clear();
            LastModifiedRefs6.Clear();
            CreatedRefs7.Clear();
            LastModifiedRefs7.Clear();
            CreatedActionCategories.Clear();
            LastModifiedActionCategories.Clear();
            CreatedResources.Clear();
            LastModifiedResources.Clear();
            CreatedScenarios.Clear();
            LastModifiedScenarios.Clear();
            CreatedUsers.Clear();
            Creator = null;
            LastModifiedUsers.Clear();
            LastModifier = null;
            CreatedVideos.Clear();
            LastModifiedVideos.Clear();
            Roles.Clear();
            Publication.Clear();
            Teams.Clear();
            ReadPublications.Clear();
            Trainings.Clear();
            ValidationTrainings.Clear();
            Qualifications.Clear();
            QualificationSteps.Clear();
            Skills.Clear();
            Skills1.Clear();
            InspectionSteps.Clear();
            Anomalies.Clear();
            Audits.Clear();
            VideoSyncs.Clear();
            Procedures.Clear();
            UserRoleProcesses.Clear();
            PublicationHistories.Clear();
        }

        #endregion

        #region Correction de l'état des associations
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation DefaultLanguage.
        /// </summary>
        private void FixupDefaultLanguage(Language previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.Users.Contains(this))
            {
                previousValue.Users.Remove(this);
            }
    
            if (DefaultLanguage != null)
            {
                if (!DefaultLanguage.Users.Contains(this))
                {
                    DefaultLanguage.Users.Add(this);
                }
    
                DefaultLanguageCode = DefaultLanguage.LanguageCode;
            }
            else if (!skipKeys)
            {
                DefaultLanguageCode = null;
            }
    
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                ChangeTracker.RecordValue("DefaultLanguage", previousValue, DefaultLanguage);
                if (DefaultLanguage != null && !DefaultLanguage.ChangeTracker.ChangeTrackingEnabled)
                {
                    DefaultLanguage.StartTracking();
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété de navigation Creator.
        /// </summary>
        private void FixupCreator(User previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CreatedUsers.Contains(this))
            {
                previousValue.CreatedUsers.Remove(this);
            }
    
            if (Creator != null)
            {
                if (!Creator.CreatedUsers.Contains(this))
                {
                    Creator.CreatedUsers.Add(this);
                }
    
                CreatedByUserId = Creator.UserId;
            }
            else if (!skipKeys)
            {
                CreatedByUserId = null;
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
        private void FixupLastModifier(User previousValue, bool skipKeys = false)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.LastModifiedUsers.Contains(this))
            {
                previousValue.LastModifiedUsers.Remove(this);
            }
    
            if (LastModifier != null)
            {
                if (!LastModifier.LastModifiedUsers.Contains(this))
                {
                    LastModifier.LastModifiedUsers.Add(this);
                }
    
                ModifiedByUserId = LastModifier.UserId;
            }
            else if (!skipKeys)
            {
                ModifiedByUserId = null;
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
        /// Corrige l'état de la propriété CreatedActions.
        /// </summary>
        private void FixupCreatedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KAction item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedActions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedActions.
        /// </summary>
        private void FixupLastModifiedActions(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (KAction item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedActions", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (KAction item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedActions", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedAppResourceValues.
        /// </summary>
        private void FixupCreatedAppResourceValues(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (AppResourceValue item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedAppResourceValues", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AppResourceValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedAppResourceValues", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedAppResourceValues.
        /// </summary>
        private void FixupLastModifiedAppResourceValues(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (AppResourceValue item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedAppResourceValues", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (AppResourceValue item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedAppResourceValues", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedProjects.
        /// </summary>
        private void FixupCreatedProjects(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Project item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedProjects", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Project item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedProjects", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedProjects.
        /// </summary>
        private void FixupLastModifiedProjects(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Project item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedProjects", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Project item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedProjects", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs1.
        /// </summary>
        private void FixupCreatedRefs1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref1 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref1 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs1", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs1.
        /// </summary>
        private void FixupLastModifiedRefs1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref1 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref1 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs1", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs2.
        /// </summary>
        private void FixupCreatedRefs2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref2 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs2", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref2 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs2", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs2.
        /// </summary>
        private void FixupLastModifiedRefs2(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref2 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs2", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref2 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs2", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs3.
        /// </summary>
        private void FixupCreatedRefs3(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref3 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs3", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref3 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs3", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs3.
        /// </summary>
        private void FixupLastModifiedRefs3(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref3 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs3", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref3 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs3", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs4.
        /// </summary>
        private void FixupCreatedRefs4(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref4 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs4", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref4 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs4", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs4.
        /// </summary>
        private void FixupLastModifiedRefs4(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref4 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs4", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref4 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs4", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs5.
        /// </summary>
        private void FixupCreatedRefs5(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref5 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs5", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref5 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs5", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs5.
        /// </summary>
        private void FixupLastModifiedRefs5(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref5 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs5", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref5 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs5", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs6.
        /// </summary>
        private void FixupCreatedRefs6(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref6 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs6", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref6 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs6", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs6.
        /// </summary>
        private void FixupLastModifiedRefs6(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref6 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs6", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref6 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs6", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedRefs7.
        /// </summary>
        private void FixupCreatedRefs7(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref7 item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedRefs7", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref7 item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedRefs7", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedRefs7.
        /// </summary>
        private void FixupLastModifiedRefs7(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Ref7 item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedRefs7", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Ref7 item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedRefs7", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedActionCategories.
        /// </summary>
        private void FixupCreatedActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionCategory item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedActionCategories.
        /// </summary>
        private void FixupLastModifiedActionCategories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (ActionCategory item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedActionCategories", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ActionCategory item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedActionCategories", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedResources.
        /// </summary>
        private void FixupCreatedResources(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Resource item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedResources", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Resource item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedResources", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedResources.
        /// </summary>
        private void FixupLastModifiedResources(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Resource item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedResources", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Resource item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedResources", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedScenarios.
        /// </summary>
        private void FixupCreatedScenarios(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Scenario item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedScenarios", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Scenario item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedScenarios", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedScenarios.
        /// </summary>
        private void FixupLastModifiedScenarios(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Scenario item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedScenarios", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Scenario item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedScenarios", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedUsers.
        /// </summary>
        private void FixupCreatedUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (User item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedUsers", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (User item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedUsers", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedUsers.
        /// </summary>
        private void FixupLastModifiedUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (User item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedUsers", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (User item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedUsers", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété CreatedVideos.
        /// </summary>
        private void FixupCreatedVideos(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Video item in e.NewItems)
                {
                    item.Creator = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("CreatedVideos", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Video item in e.OldItems)
                {
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("CreatedVideos", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété LastModifiedVideos.
        /// </summary>
        private void FixupLastModifiedVideos(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Video item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("LastModifiedVideos", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Video item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("LastModifiedVideos", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Roles.
        /// </summary>
        private void FixupRoles(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Role item in e.NewItems)
                {
                    if (!item.Users.Contains(this))
                    {
                        item.Users.Add(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Roles", item);
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
                foreach (Role item in e.OldItems)
                {
                    if (item.Users.Contains(this))
                    {
                        item.Users.Remove(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Roles", item);
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
        /// Corrige l'état de la propriété Publication.
        /// </summary>
        private void FixupPublication(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Publication item in e.NewItems)
                {
                    item.Publisher = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Publication", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Publication item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publisher, this))
                    {
                        item.Publisher = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Publication", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Teams.
        /// </summary>
        private void FixupTeams(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Team item in e.NewItems)
                {
                    if (!item.Users.Contains(this))
                    {
                        item.Users.Add(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Teams", item);
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
                foreach (Team item in e.OldItems)
                {
                    if (item.Users.Contains(this))
                    {
                        item.Users.Remove(this);
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Teams", item);
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
        /// Corrige l'état de la propriété ReadPublications.
        /// </summary>
        private void FixupReadPublications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (UserReadPublication item in e.NewItems)
                {
                    item.User = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("ReadPublications", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserReadPublication item in e.OldItems)
                {
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ReadPublications", item);
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
        /// Corrige l'état de la propriété Trainings.
        /// </summary>
        private void FixupTrainings(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Training item in e.NewItems)
                {
                    item.User = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Trainings", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Training item in e.OldItems)
                {
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Trainings", item);
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
                    item.User = this;
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
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("ValidationTrainings", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Qualifications.
        /// </summary>
        private void FixupQualifications(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Qualification item in e.NewItems)
                {
                    item.User = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Qualifications", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Qualification item in e.OldItems)
                {
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Qualifications", item);
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
                    item.User = this;
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
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("QualificationSteps", item);
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
                    item.Creator = this;
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
                    if (ReferenceEquals(item.Creator, this))
                    {
                        item.Creator = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Skills", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Skills1.
        /// </summary>
        private void FixupSkills1(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Skill item in e.NewItems)
                {
                    item.LastModifier = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Skills1", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Skill item in e.OldItems)
                {
                    if (ReferenceEquals(item.LastModifier, this))
                    {
                        item.LastModifier = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Skills1", item);
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
                    item.Inspector = this;
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
                    if (ReferenceEquals(item.Inspector, this))
                    {
                        item.Inspector = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("InspectionSteps", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Anomalies.
        /// </summary>
        private void FixupAnomalies(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Anomaly item in e.NewItems)
                {
                    item.Inspector = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Anomalies", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Anomaly item in e.OldItems)
                {
                    if (ReferenceEquals(item.Inspector, this))
                    {
                        item.Inspector = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Anomalies", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété Audits.
        /// </summary>
        private void FixupAudits(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Audit item in e.NewItems)
                {
                    item.Auditor = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Audits", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Audit item in e.OldItems)
                {
                    if (ReferenceEquals(item.Auditor, this))
                    {
                        item.Auditor = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Audits", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété VideoSyncs.
        /// </summary>
        private void FixupVideoSyncs(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (VideoSync item in e.NewItems)
                {
                    item.User = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("VideoSyncs", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (VideoSync item in e.OldItems)
                {
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("VideoSyncs", item);
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
        /// Corrige l'état de la propriété Procedures.
        /// </summary>
        private void FixupProcedures(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (Procedure item in e.NewItems)
                {
                    item.Owner = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("Procedures", item);
                    }
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (Procedure item in e.OldItems)
                {
                    if (ReferenceEquals(item.Owner, this))
                    {
                        item.Owner = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("Procedures", item);
                    }
                }
            }
        }
    
        /// <summary>
        /// Corrige l'état de la propriété UserRoleProcesses.
        /// </summary>
        private void FixupUserRoleProcesses(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (UserRoleProcess item in e.NewItems)
                {
                    item.User = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("UserRoleProcesses", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (UserRoleProcess item in e.OldItems)
                {
                    if (ReferenceEquals(item.User, this))
                    {
                        item.User = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("UserRoleProcesses", item);
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
        /// Corrige l'état de la propriété PublicationHistories.
        /// </summary>
        private void FixupPublicationHistories(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (e.NewItems != null)
            {
                foreach (PublicationHistory item in e.NewItems)
                {
                    item.Publisher = this;
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        if (!item.ChangeTracker.ChangeTrackingEnabled)
                        {
                            item.StartTracking();
                        }
                        ChangeTracker.RecordAdditionToCollectionProperties("PublicationHistories", item);
                    }
                    // This is the principal end in an association that performs cascade deletes.
                    // Update the event listener to refer to the new dependent.
                    ChangeTracker.ObjectStateChanging += item.HandleCascadeDelete;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (PublicationHistory item in e.OldItems)
                {
                    if (ReferenceEquals(item.Publisher, this))
                    {
                        item.Publisher = null;
                    }
                    if (ChangeTracker.ChangeTrackingEnabled)
                    {
                        ChangeTracker.RecordRemovalFromCollectionProperties("PublicationHistories", item);
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
    			case "UserId":
    				this.UserId = Convert.ToInt32(value);
    				break;
    			case "DefaultLanguageCode":
    				this.DefaultLanguageCode = (string)value;
    				break;
    			case "Username":
    				this.Username = (string)value;
    				break;
    			case "Password":
    				this.Password = (byte[])value;
    				break;
    			case "Firstname":
    				this.Firstname = (string)value;
    				break;
    			case "Name":
    				this.Name = (string)value;
    				break;
    			case "Email":
    				this.Email = (string)value;
    				break;
    			case "PhoneNumber":
    				this.PhoneNumber = (string)value;
    				break;
    			case "IsDeleted":
    				this.IsDeleted = (bool)value;
    				break;
    			case "CreatedByUserId":
    				this.CreatedByUserId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "CreationDate":
    				this.CreationDate = (System.DateTime)value;
    				break;
    			case "ModifiedByUserId":
    				this.ModifiedByUserId = value == null ? (int?)null : Convert.ToInt32(value);
    				break;
    			case "LastModificationDate":
    				this.LastModificationDate = (System.DateTime)value;
    				break;
    			case "RowVersion":
    				this.RowVersion = (byte[])value;
    				break;
    			case "Tenured":
    				this.Tenured = (Nullable<bool>)value;
    				break;
    			case "DefaultLanguage":
    				this.DefaultLanguage = (Language)value;
    				break;
    			case "Creator":
    				this.Creator = (User)value;
    				break;
    			case "LastModifier":
    				this.LastModifier = (User)value;
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
    			case "CreatedActions":
    				this.CreatedActions.Add((KAction)value);
    				break;
    			case "LastModifiedActions":
    				this.LastModifiedActions.Add((KAction)value);
    				break;
    			case "CreatedAppResourceValues":
    				this.CreatedAppResourceValues.Add((AppResourceValue)value);
    				break;
    			case "LastModifiedAppResourceValues":
    				this.LastModifiedAppResourceValues.Add((AppResourceValue)value);
    				break;
    			case "CreatedProjects":
    				this.CreatedProjects.Add((Project)value);
    				break;
    			case "LastModifiedProjects":
    				this.LastModifiedProjects.Add((Project)value);
    				break;
    			case "CreatedRefs1":
    				this.CreatedRefs1.Add((Ref1)value);
    				break;
    			case "LastModifiedRefs1":
    				this.LastModifiedRefs1.Add((Ref1)value);
    				break;
    			case "CreatedRefs2":
    				this.CreatedRefs2.Add((Ref2)value);
    				break;
    			case "LastModifiedRefs2":
    				this.LastModifiedRefs2.Add((Ref2)value);
    				break;
    			case "CreatedRefs3":
    				this.CreatedRefs3.Add((Ref3)value);
    				break;
    			case "LastModifiedRefs3":
    				this.LastModifiedRefs3.Add((Ref3)value);
    				break;
    			case "CreatedRefs4":
    				this.CreatedRefs4.Add((Ref4)value);
    				break;
    			case "LastModifiedRefs4":
    				this.LastModifiedRefs4.Add((Ref4)value);
    				break;
    			case "CreatedRefs5":
    				this.CreatedRefs5.Add((Ref5)value);
    				break;
    			case "LastModifiedRefs5":
    				this.LastModifiedRefs5.Add((Ref5)value);
    				break;
    			case "CreatedRefs6":
    				this.CreatedRefs6.Add((Ref6)value);
    				break;
    			case "LastModifiedRefs6":
    				this.LastModifiedRefs6.Add((Ref6)value);
    				break;
    			case "CreatedRefs7":
    				this.CreatedRefs7.Add((Ref7)value);
    				break;
    			case "LastModifiedRefs7":
    				this.LastModifiedRefs7.Add((Ref7)value);
    				break;
    			case "CreatedActionCategories":
    				this.CreatedActionCategories.Add((ActionCategory)value);
    				break;
    			case "LastModifiedActionCategories":
    				this.LastModifiedActionCategories.Add((ActionCategory)value);
    				break;
    			case "CreatedResources":
    				this.CreatedResources.Add((Resource)value);
    				break;
    			case "LastModifiedResources":
    				this.LastModifiedResources.Add((Resource)value);
    				break;
    			case "CreatedScenarios":
    				this.CreatedScenarios.Add((Scenario)value);
    				break;
    			case "LastModifiedScenarios":
    				this.LastModifiedScenarios.Add((Scenario)value);
    				break;
    			case "CreatedUsers":
    				this.CreatedUsers.Add((User)value);
    				break;
    			case "LastModifiedUsers":
    				this.LastModifiedUsers.Add((User)value);
    				break;
    			case "CreatedVideos":
    				this.CreatedVideos.Add((Video)value);
    				break;
    			case "LastModifiedVideos":
    				this.LastModifiedVideos.Add((Video)value);
    				break;
    			case "Roles":
    				this.Roles.Add((Role)value);
    				break;
    			case "Publication":
    				this.Publication.Add((Publication)value);
    				break;
    			case "Teams":
    				this.Teams.Add((Team)value);
    				break;
    			case "ReadPublications":
    				this.ReadPublications.Add((UserReadPublication)value);
    				break;
    			case "Trainings":
    				this.Trainings.Add((Training)value);
    				break;
    			case "ValidationTrainings":
    				this.ValidationTrainings.Add((ValidationTraining)value);
    				break;
    			case "Qualifications":
    				this.Qualifications.Add((Qualification)value);
    				break;
    			case "QualificationSteps":
    				this.QualificationSteps.Add((QualificationStep)value);
    				break;
    			case "Skills":
    				this.Skills.Add((Skill)value);
    				break;
    			case "Skills1":
    				this.Skills1.Add((Skill)value);
    				break;
    			case "InspectionSteps":
    				this.InspectionSteps.Add((InspectionStep)value);
    				break;
    			case "Anomalies":
    				this.Anomalies.Add((Anomaly)value);
    				break;
    			case "Audits":
    				this.Audits.Add((Audit)value);
    				break;
    			case "VideoSyncs":
    				this.VideoSyncs.Add((VideoSync)value);
    				break;
    			case "Procedures":
    				this.Procedures.Add((Procedure)value);
    				break;
    			case "UserRoleProcesses":
    				this.UserRoleProcesses.Add((UserRoleProcess)value);
    				break;
    			case "PublicationHistories":
    				this.PublicationHistories.Add((PublicationHistory)value);
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
    			case "CreatedActions":
    				this.CreatedActions.Remove((KAction)value);
    				break;
    			case "LastModifiedActions":
    				this.LastModifiedActions.Remove((KAction)value);
    				break;
    			case "CreatedAppResourceValues":
    				this.CreatedAppResourceValues.Remove((AppResourceValue)value);
    				break;
    			case "LastModifiedAppResourceValues":
    				this.LastModifiedAppResourceValues.Remove((AppResourceValue)value);
    				break;
    			case "CreatedProjects":
    				this.CreatedProjects.Remove((Project)value);
    				break;
    			case "LastModifiedProjects":
    				this.LastModifiedProjects.Remove((Project)value);
    				break;
    			case "CreatedRefs1":
    				this.CreatedRefs1.Remove((Ref1)value);
    				break;
    			case "LastModifiedRefs1":
    				this.LastModifiedRefs1.Remove((Ref1)value);
    				break;
    			case "CreatedRefs2":
    				this.CreatedRefs2.Remove((Ref2)value);
    				break;
    			case "LastModifiedRefs2":
    				this.LastModifiedRefs2.Remove((Ref2)value);
    				break;
    			case "CreatedRefs3":
    				this.CreatedRefs3.Remove((Ref3)value);
    				break;
    			case "LastModifiedRefs3":
    				this.LastModifiedRefs3.Remove((Ref3)value);
    				break;
    			case "CreatedRefs4":
    				this.CreatedRefs4.Remove((Ref4)value);
    				break;
    			case "LastModifiedRefs4":
    				this.LastModifiedRefs4.Remove((Ref4)value);
    				break;
    			case "CreatedRefs5":
    				this.CreatedRefs5.Remove((Ref5)value);
    				break;
    			case "LastModifiedRefs5":
    				this.LastModifiedRefs5.Remove((Ref5)value);
    				break;
    			case "CreatedRefs6":
    				this.CreatedRefs6.Remove((Ref6)value);
    				break;
    			case "LastModifiedRefs6":
    				this.LastModifiedRefs6.Remove((Ref6)value);
    				break;
    			case "CreatedRefs7":
    				this.CreatedRefs7.Remove((Ref7)value);
    				break;
    			case "LastModifiedRefs7":
    				this.LastModifiedRefs7.Remove((Ref7)value);
    				break;
    			case "CreatedActionCategories":
    				this.CreatedActionCategories.Remove((ActionCategory)value);
    				break;
    			case "LastModifiedActionCategories":
    				this.LastModifiedActionCategories.Remove((ActionCategory)value);
    				break;
    			case "CreatedResources":
    				this.CreatedResources.Remove((Resource)value);
    				break;
    			case "LastModifiedResources":
    				this.LastModifiedResources.Remove((Resource)value);
    				break;
    			case "CreatedScenarios":
    				this.CreatedScenarios.Remove((Scenario)value);
    				break;
    			case "LastModifiedScenarios":
    				this.LastModifiedScenarios.Remove((Scenario)value);
    				break;
    			case "CreatedUsers":
    				this.CreatedUsers.Remove((User)value);
    				break;
    			case "LastModifiedUsers":
    				this.LastModifiedUsers.Remove((User)value);
    				break;
    			case "CreatedVideos":
    				this.CreatedVideos.Remove((Video)value);
    				break;
    			case "LastModifiedVideos":
    				this.LastModifiedVideos.Remove((Video)value);
    				break;
    			case "Roles":
    				this.Roles.Remove((Role)value);
    				break;
    			case "Publication":
    				this.Publication.Remove((Publication)value);
    				break;
    			case "Teams":
    				this.Teams.Remove((Team)value);
    				break;
    			case "ReadPublications":
    				this.ReadPublications.Remove((UserReadPublication)value);
    				break;
    			case "Trainings":
    				this.Trainings.Remove((Training)value);
    				break;
    			case "ValidationTrainings":
    				this.ValidationTrainings.Remove((ValidationTraining)value);
    				break;
    			case "Qualifications":
    				this.Qualifications.Remove((Qualification)value);
    				break;
    			case "QualificationSteps":
    				this.QualificationSteps.Remove((QualificationStep)value);
    				break;
    			case "Skills":
    				this.Skills.Remove((Skill)value);
    				break;
    			case "Skills1":
    				this.Skills1.Remove((Skill)value);
    				break;
    			case "InspectionSteps":
    				this.InspectionSteps.Remove((InspectionStep)value);
    				break;
    			case "Anomalies":
    				this.Anomalies.Remove((Anomaly)value);
    				break;
    			case "Audits":
    				this.Audits.Remove((Audit)value);
    				break;
    			case "VideoSyncs":
    				this.VideoSyncs.Remove((VideoSync)value);
    				break;
    			case "Procedures":
    				this.Procedures.Remove((Procedure)value);
    				break;
    			case "UserRoleProcesses":
    				this.UserRoleProcesses.Remove((UserRoleProcess)value);
    				break;
    			case "PublicationHistories":
    				this.PublicationHistories.Remove((PublicationHistory)value);
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
    		values.Add("UserId", this.UserId);
    		values.Add("DefaultLanguageCode", this.DefaultLanguageCode);
    		values.Add("Username", this.Username);
    		values.Add("Password", this.Password);
    		values.Add("Firstname", this.Firstname);
    		values.Add("Name", this.Name);
    		values.Add("Email", this.Email);
    		values.Add("PhoneNumber", this.PhoneNumber);
    		values.Add("IsDeleted", this.IsDeleted);
    		values.Add("CreatedByUserId", this.CreatedByUserId);
    		values.Add("CreationDate", this.CreationDate);
    		values.Add("ModifiedByUserId", this.ModifiedByUserId);
    		values.Add("LastModificationDate", this.LastModificationDate);
    		values.Add("RowVersion", this.RowVersion);
    		values.Add("Tenured", this.Tenured);
    		values.Add("DefaultLanguage", this.DefaultLanguage);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    
    		values.Add("CreatedActions", GetHashCodes(this.CreatedActions));
    		values.Add("LastModifiedActions", GetHashCodes(this.LastModifiedActions));
    		values.Add("CreatedAppResourceValues", GetHashCodes(this.CreatedAppResourceValues));
    		values.Add("LastModifiedAppResourceValues", GetHashCodes(this.LastModifiedAppResourceValues));
    		values.Add("CreatedProjects", GetHashCodes(this.CreatedProjects));
    		values.Add("LastModifiedProjects", GetHashCodes(this.LastModifiedProjects));
    		values.Add("CreatedRefs1", GetHashCodes(this.CreatedRefs1));
    		values.Add("LastModifiedRefs1", GetHashCodes(this.LastModifiedRefs1));
    		values.Add("CreatedRefs2", GetHashCodes(this.CreatedRefs2));
    		values.Add("LastModifiedRefs2", GetHashCodes(this.LastModifiedRefs2));
    		values.Add("CreatedRefs3", GetHashCodes(this.CreatedRefs3));
    		values.Add("LastModifiedRefs3", GetHashCodes(this.LastModifiedRefs3));
    		values.Add("CreatedRefs4", GetHashCodes(this.CreatedRefs4));
    		values.Add("LastModifiedRefs4", GetHashCodes(this.LastModifiedRefs4));
    		values.Add("CreatedRefs5", GetHashCodes(this.CreatedRefs5));
    		values.Add("LastModifiedRefs5", GetHashCodes(this.LastModifiedRefs5));
    		values.Add("CreatedRefs6", GetHashCodes(this.CreatedRefs6));
    		values.Add("LastModifiedRefs6", GetHashCodes(this.LastModifiedRefs6));
    		values.Add("CreatedRefs7", GetHashCodes(this.CreatedRefs7));
    		values.Add("LastModifiedRefs7", GetHashCodes(this.LastModifiedRefs7));
    		values.Add("CreatedActionCategories", GetHashCodes(this.CreatedActionCategories));
    		values.Add("LastModifiedActionCategories", GetHashCodes(this.LastModifiedActionCategories));
    		values.Add("CreatedResources", GetHashCodes(this.CreatedResources));
    		values.Add("LastModifiedResources", GetHashCodes(this.LastModifiedResources));
    		values.Add("CreatedScenarios", GetHashCodes(this.CreatedScenarios));
    		values.Add("LastModifiedScenarios", GetHashCodes(this.LastModifiedScenarios));
    		values.Add("CreatedUsers", GetHashCodes(this.CreatedUsers));
    		values.Add("LastModifiedUsers", GetHashCodes(this.LastModifiedUsers));
    		values.Add("CreatedVideos", GetHashCodes(this.CreatedVideos));
    		values.Add("LastModifiedVideos", GetHashCodes(this.LastModifiedVideos));
    		values.Add("Roles", GetHashCodes(this.Roles));
    		values.Add("Publication", GetHashCodes(this.Publication));
    		values.Add("Teams", GetHashCodes(this.Teams));
    		values.Add("ReadPublications", GetHashCodes(this.ReadPublications));
    		values.Add("Trainings", GetHashCodes(this.Trainings));
    		values.Add("ValidationTrainings", GetHashCodes(this.ValidationTrainings));
    		values.Add("Qualifications", GetHashCodes(this.Qualifications));
    		values.Add("QualificationSteps", GetHashCodes(this.QualificationSteps));
    		values.Add("Skills", GetHashCodes(this.Skills));
    		values.Add("Skills1", GetHashCodes(this.Skills1));
    		values.Add("InspectionSteps", GetHashCodes(this.InspectionSteps));
    		values.Add("Anomalies", GetHashCodes(this.Anomalies));
    		values.Add("Audits", GetHashCodes(this.Audits));
    		values.Add("VideoSyncs", GetHashCodes(this.VideoSyncs));
    		values.Add("Procedures", GetHashCodes(this.Procedures));
    		values.Add("UserRoleProcesses", GetHashCodes(this.UserRoleProcesses));
    		values.Add("PublicationHistories", GetHashCodes(this.PublicationHistories));
    		return values;
    	}	
    	
    	/// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	public virtual IDictionary<string,dynamic> GetGraphValues()
    	{
    		var values = new Dictionary<string,dynamic>();
    		values.Add("DefaultLanguage", this.DefaultLanguage);
    		values.Add("Creator", this.Creator);
    		values.Add("LastModifier", this.LastModifier);
    
    		values.Add("CreatedActions", this.CreatedActions);
    		values.Add("LastModifiedActions", this.LastModifiedActions);
    		values.Add("CreatedAppResourceValues", this.CreatedAppResourceValues);
    		values.Add("LastModifiedAppResourceValues", this.LastModifiedAppResourceValues);
    		values.Add("CreatedProjects", this.CreatedProjects);
    		values.Add("LastModifiedProjects", this.LastModifiedProjects);
    		values.Add("CreatedRefs1", this.CreatedRefs1);
    		values.Add("LastModifiedRefs1", this.LastModifiedRefs1);
    		values.Add("CreatedRefs2", this.CreatedRefs2);
    		values.Add("LastModifiedRefs2", this.LastModifiedRefs2);
    		values.Add("CreatedRefs3", this.CreatedRefs3);
    		values.Add("LastModifiedRefs3", this.LastModifiedRefs3);
    		values.Add("CreatedRefs4", this.CreatedRefs4);
    		values.Add("LastModifiedRefs4", this.LastModifiedRefs4);
    		values.Add("CreatedRefs5", this.CreatedRefs5);
    		values.Add("LastModifiedRefs5", this.LastModifiedRefs5);
    		values.Add("CreatedRefs6", this.CreatedRefs6);
    		values.Add("LastModifiedRefs6", this.LastModifiedRefs6);
    		values.Add("CreatedRefs7", this.CreatedRefs7);
    		values.Add("LastModifiedRefs7", this.LastModifiedRefs7);
    		values.Add("CreatedActionCategories", this.CreatedActionCategories);
    		values.Add("LastModifiedActionCategories", this.LastModifiedActionCategories);
    		values.Add("CreatedResources", this.CreatedResources);
    		values.Add("LastModifiedResources", this.LastModifiedResources);
    		values.Add("CreatedScenarios", this.CreatedScenarios);
    		values.Add("LastModifiedScenarios", this.LastModifiedScenarios);
    		values.Add("CreatedUsers", this.CreatedUsers);
    		values.Add("LastModifiedUsers", this.LastModifiedUsers);
    		values.Add("CreatedVideos", this.CreatedVideos);
    		values.Add("LastModifiedVideos", this.LastModifiedVideos);
    		values.Add("Roles", this.Roles);
    		values.Add("Publication", this.Publication);
    		values.Add("Teams", this.Teams);
    		values.Add("ReadPublications", this.ReadPublications);
    		values.Add("Trainings", this.Trainings);
    		values.Add("ValidationTrainings", this.ValidationTrainings);
    		values.Add("Qualifications", this.Qualifications);
    		values.Add("QualificationSteps", this.QualificationSteps);
    		values.Add("Skills", this.Skills);
    		values.Add("Skills1", this.Skills1);
    		values.Add("InspectionSteps", this.InspectionSteps);
    		values.Add("Anomalies", this.Anomalies);
    		values.Add("Audits", this.Audits);
    		values.Add("VideoSyncs", this.VideoSyncs);
    		values.Add("Procedures", this.Procedures);
    		values.Add("UserRoleProcesses", this.UserRoleProcesses);
    		values.Add("PublicationHistories", this.PublicationHistories);
    		return values;
    	}
    	
    	public int[] GetHashCodes(IEnumerable<object> entities)
    	{
    		return entities.Select(e => e.GetHashCode()).ToArray();
    	}
    	

        #endregion

    	
        #region Taille maximum des champs
    
        /// <summary>
        /// Taille maximum du champ Username.
        /// </summary>
    	public const int UsernameMaxLength = 50;
    
        /// <summary>
        /// Taille maximum du champ Firstname.
        /// </summary>
    	public const int FirstnameMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ Name.
        /// </summary>
    	public const int NameMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ Email.
        /// </summary>
    	public const int EmailMaxLength = 100;
    
        /// <summary>
        /// Taille maximum du champ PhoneNumber.
        /// </summary>
    	public const int PhoneNumberMaxLength = 20;

        #endregion

    }
}
