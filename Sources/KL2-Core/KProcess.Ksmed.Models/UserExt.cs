using KProcess.Globalization;
using KProcess.KL2.JWT;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    [HasSelfValidation]
    partial class User : IAuditableUserOptional
    {

        /// <summary>
        /// Obtient le nom complet de l'utilisateur
        /// </summary>
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(Firstname) && string.IsNullOrEmpty(Name))
                    return Username;

                // Commented, because it causes problem in WebAdmin, indeed, WebAdmin uses Lazy<IUnityContainer>,
                // so, no classes are registered correctly in IoC.Container
                /*if (IoC.IsRegistered<ILocalizationManager>())
                    return IoC.Resolve<ILocalizationManager>().GetLocalizedFullName(Firstname, Name).Trim();*/
                return $"{Firstname} {Name}".Trim();
            }
        }

        [DataMember]
        public string CurrentLanguageCode { get; set; }

        public override string ToString() =>
            FullName;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'audit ne doit pas être géré automatiquement.
        /// </summary>
        public bool CustomAudit =>
            false;

        /// <summary>
        /// Obtient une valeur indiquant si l'utilisateur a été validé et s'il est valide.
        /// </summary>
        public bool IsValidatedAndIsNotValid =>
            IsValid.HasValue && !IsValid.Value;

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ValidatableObject.IsValid"/> a changé.
        /// </summary>
        protected override void OnIsValidChanged()
        {
            base.OnIsValidChanged();
            base.OnPropertyChanged(nameof(IsValidatedAndIsNotValid), false);
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="FirstnameChanged"/> a changé.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        partial void OnFirstnameChangedPartial(string oldValue, string newValue) =>
            base.OnPropertyChanged(nameof(FullName), false);

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="NameChanged"/> a changé.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        partial void OnNameChangedPartial(string oldValue, string newValue) =>
            base.OnPropertyChanged(nameof(FullName), false);

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="UsernameChanged"/> a changé.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        partial void OnUsernameChangedPartial(string oldValue, string newValue) =>
            base.OnPropertyChanged(nameof(FullName), false);

        string _newPassword;
        /// <summary>
        /// Obtient ou définit le nouveau mot de passe.
        /// </summary>
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (_newPassword != value)
                {
                    _newPassword = value;
                    OnEntityPropertyChanged(nameof(NewPassword));
                }
            }
        }

        string _confirmNewPassword;
        /// <summary>
        /// Obtient ou définit la confirmation du nouveau mot de passe.
        /// </summary>
        public string ConfirmNewPassword
        {
            get { return _confirmNewPassword; }
            set
            {
                if (_confirmNewPassword != value)
                {
                    _confirmNewPassword = value;
                    OnEntityPropertyChanged(nameof(ConfirmNewPassword));
                }
            }
        }

        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Lorsque surchargé dans une classe fille, cette méthode sert à exécuter une validation personnalisée.
        /// </summary>
        /// <returns>
        /// Une énumération des erreurs de validation, ou null s'il n'y en a pas.
        /// </returns>
        [SelfValidation]
        protected override IEnumerable<ValidationError> OnCustomValidate()
        {
            foreach (var error in base.OnCustomValidate())
                yield return error;

            if (!ValidateRoleCodes)
            {
                if (IsMarkedAsAdded && string.IsNullOrEmpty(NewPassword) && string.IsNullOrEmpty(ConfirmNewPassword))
                {
                    yield return new ValidationError(nameof(NewPassword), LocalizationManager.GetString("Validation_User_NewPassword_Required"));
                    yield return new ValidationError(nameof(ConfirmNewPassword), LocalizationManager.GetString("Validation_User_NewPassword_Required"));

                }

                if (!string.IsNullOrEmpty(NewPassword) || !string.IsNullOrEmpty(ConfirmNewPassword))
                {
                    if (!string.Equals(NewPassword, ConfirmNewPassword))
                    {
                        yield return new ValidationError(nameof(NewPassword), LocalizationManager.GetString("Validation_User_NewPasswords_DontMatch"));
                    }
                }

                if (!Roles.Any())
                    yield return new ValidationError(nameof(Roles), LocalizationManager.GetString("Validation_User_Roles_MustHaveAtLeastOneRole"));
            }
            else if (!RoleCodes.Any())
            {
                yield return new ValidationError(nameof(RoleCodes), LocalizationManager.GetString("Validation_Member_Roles_MustHaveAtLeastOneRole"));
            }
        }

        [DataMember]
        public ICollection<UserRole> UserRoles { get; set; }

        List<string> _roleCodes;
        /// <summary>
        /// Obtient ou définit les codes des rôles attribués à cet utilisateur.
        /// </summary>
        [DataMember]
        public List<string> RoleCodes
        {
            get
            {
                if (_roleCodes == null)
                    _roleCodes = new List<string>();
                return _roleCodes;
            }
            set
            {
                _roleCodes = value;
            }
        }

        [DataMember]
        public bool ValidateRoleCodes { get; set; }

        bool _isUsernameReadOnly;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si is le nom d'utilisateur est en lecture seule.
        /// </summary>
        [DataMember]
        public bool IsUsernameReadOnly
        {
            get { return _isUsernameReadOnly; }
            set
            {
                if (_isUsernameReadOnly != value)
                {
                    _isUsernameReadOnly = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserModel ToUserModel()
        {
            return new UserModel
            {
                UserId = UserId,
                Username = Username,
                Firstname = Firstname,
                Name = Name,
                Email = Email,
                PhoneNumber = PhoneNumber,
                DefaultLanguageCode = DefaultLanguageCode,
                CurrentLanguageCode = CurrentLanguageCode ?? DefaultLanguageCode,
                Roles = Roles.Select(_ => _.RoleCode)
            };
        }

        public static User FromUserModel(UserModel usermodel)
        {
            return new User
            {
                UserId = usermodel.UserId,
                Username = usermodel.Username,
                Firstname = usermodel.Firstname,
                Name = usermodel.Name,
                Email = usermodel.Email,
                PhoneNumber = usermodel.PhoneNumber,
                DefaultLanguageCode = usermodel.DefaultLanguageCode,
                CurrentLanguageCode = usermodel.CurrentLanguageCode ?? usermodel.DefaultLanguageCode,
                RoleCodes = usermodel.Roles.ToList()
            };
        }

        public TrackableCollection<InspectionSchedule> InspectionSchedules { get; set; }
    }
}
