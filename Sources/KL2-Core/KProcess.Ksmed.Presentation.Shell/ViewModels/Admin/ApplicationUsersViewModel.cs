using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des utilisateurs de l'application.
    /// </summary>
    public class ApplicationUsersViewModel : FrameContentExtensibleViewModelBase<ApplicationUsersViewModel, IApplicationUsersViewModel>, IApplicationUsersViewModel
    {

        #region Champs privés

        private IPresentationRole[] _roles;
        private BulkObservableCollection<User> _users;
        private Language[] _languages;
        private User _currentUser;
        private List<User> _usersPendingForDelete;
        private bool _hasChanged;
        private bool _ignoreRoleCheck = false;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            this.Users = new BulkObservableCollection<User>(true);
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            Users.Clear();

            try
            {
                var (users, roles, languages, teams) = await ServiceBus.Get<IApplicationUsersService>().GetUsersAndRolesAndLanguages();

                Users.AddRange(users);
                foreach (var user in Users)
                {
                    if (user.Username == Security.SecurityContext.CurrentUser.Username)
                        user.IsUsernameReadOnly = true;
                    user.StartTracking();
                }

                var presRoles = roles.Select(r => new PresentationRole(this, r)).ToArray();
                Array.Sort(presRoles);
                Roles = presRoles;

                Languages = languages;
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Users = new BulkObservableCollection<User>(DesignData.GenerateUsers());
            Roles = DesignData.GenerateRoles().Select(r => new PresentationRole(this, r)).ToArray();
            DesignData.LinkUsersWithRoles(Users, Roles.Select(r => r.Role));
            Languages = DesignData.GenerateLanguages().ToArray();

            CurrentUser = Users.First();

            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient les roles.
        /// </summary>
        public IPresentationRole[] Roles
        {
            get { return _roles; }
            private set
            {
                if (_roles != value)
                {
                    _roles = value;
                    OnPropertyChanged("Roles");
                }
            }
        }

        /// <summary>
        /// Obtient les utilisateurs.
        /// </summary>
        public BulkObservableCollection<User> Users
        {
            get { return _users; }
            private set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged("Users");
                }
            }
        }

        /// <summary>
        /// Obtient les langues.
        /// </summary>
        public Language[] Languages
        {
            get { return _languages; }
            private set
            {
                if (_languages != value)
                {
                    _languages = value;
                    OnPropertyChanged("Languages");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit l'utilisateur courant.
        /// </summary>
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value)
                {
                    var previous = _currentUser;
                    _currentUser = value;
                    OnPropertyChanged("CurrentUser");
                    OnCurrentUserChanged(previous, _currentUser);
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si une des éléments de la liste a changé.
        /// </summary>
        public bool HasChanged
        {
            get { return _hasChanged; }
            private set
            {
                if (_hasChanged != value)
                {
                    _hasChanged = value;
                    OnPropertyChanged("HasChanged");
                }
            }
        }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande AddCommand
        /// </summary>
        protected override void OnAddCommandExecute()
        {
            this.CurrentUser = new User();
            this.Users.Add(this.CurrentUser);
            this.HasChanged = true;
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnRemoveCommandCanExecute()
        {
            return this.CurrentUser != null && KProcess.Ksmed.Security.SecurityContext.CurrentUser != null
                && this.CurrentUser.Username != KProcess.Ksmed.Security.SecurityContext.CurrentUser.Username
                && !KProcess.Ksmed.Security.SecurityContext.IsUserDefaultAdmin(this.CurrentUser);
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        protected override void OnRemoveCommandExecute()
        {
            if (this.CurrentUser.ChangeTracker.State == ObjectState.Added)
            {
                this.Users.Remove(this.CurrentUser);
                return;
            }

            if (_usersPendingForDelete == null)
                _usersPendingForDelete = new List<User>();

            _usersPendingForDelete.Add(this.CurrentUser);
            this.Users.Remove(this.CurrentUser);
            this.HasChanged = true;
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute()
        {
            return this.HasChanged;
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute()
        {
            return this.HasChanged;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            foreach (var user in this.Users)
                user.ExternalValidation = null;

            if (ValidateUsers())
                await Save();
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override async void OnCancelCommandExecute()
        {
            // rafraichir le contenu depuis la base de données
            await OnLoading();
            HideValidationErrors();
            HasChanged = false;
            _usersPendingForDelete?.Clear();

            foreach (var user in Users)
                user.ExternalValidation = null;
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override async Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (Users.Any(i => i.ChangeTracker.State != ObjectState.Unchanged) ||
                (_usersPendingForDelete != null && _usersPendingForDelete.Any()))
            {
                var result = base.DialogFactory.GetDialogView<IMessageDialog>().Show(
                    LocalizationManager.GetString("VM_ApplicationMembers_Message_WantToSave"),
                    LocalizationManager.GetString("Common_Confirmation"),
                    MessageDialogButton.YesNoCancel,
                    MessageDialogImage.Question);

                switch (result)
                {
                    case MessageDialogResult.Yes:

                        // Valider, sauvegarder, quitter
                        foreach (var user in this.Users)
                            user.ExternalValidation = null;
                        if (ValidateUsers())
                        {
                            await Save();
                            return true;
                        }
                        else
                            return false;

                    case MessageDialogResult.No:
                        // Quitter sans rien faire
                        return true;
                    default:
                        // Ne pas quitter
                        return false;
                }

            }
            else
                return true;

        }

        private Command<Role> _changeRoleCommand;
        /// <summary>
        /// Commande autorisant le changement de rôle
        /// </summary>
        public ICommand ChangeRoleCommand
        {
            get
            {
                if (_changeRoleCommand == null)
                    _changeRoleCommand = new Command<Role>(role =>
                    {
                        // Ne rien faire, le binding sera appliqué
                    },
                // Empêcher de décocher Admin pour l'admin par défaut
                role => CurrentUser == null || !Security.SecurityContext.IsUserDefaultAdmin(CurrentUser) || role.RoleCode != Security.KnownRoles.Administrator);
                return _changeRoleCommand;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque l'utilisateur courant a changé.
        /// </summary>
        /// <param name="previousUser">Le précédent utilisateur.</param>
        /// <param name="newUser">le nouvel utilisateur.</param>
        private void OnCurrentUserChanged(User previousUser, User newUser)
        {
            RefreshRolesCheckStates();
            SaveNewPassword(previousUser);
            base.RegisterToStateChanged(previousUser, newUser);
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            this.HasChanged |= newState != ObjectState.Unchanged;
        }

        /// <summary>
        /// Rafrichit l'état IsChecked des rôles.
        /// </summary>
        private void RefreshRolesCheckStates()
        {
            var rolesToCheck =
                CurrentUser != null ?
                CurrentUser.Roles.Select(r => r.RoleCode) :
                Enumerable.Empty<string>();

            _ignoreRoleCheck = true;
            foreach (var role in this.Roles)
                role.IsChecked = rolesToCheck.Contains(role.Role.RoleCode);
            _ignoreRoleCheck = false;
        }

        /// <summary>
        /// Appelé lorsqu'un rôle est coché ou décoché.
        /// </summary>
        /// <param name="role">Le rôle.</param>
        /// <param name="isChecked">L'état.</param>
        public void OnRoleChecked(Role role, bool isChecked)
        {
            if (!_ignoreRoleCheck && this.CurrentUser != null)
            {
                if (isChecked)
                    this.CurrentUser.Roles.Add(role);
                else
                    this.CurrentUser.Roles.Remove(role);

                if (this.CurrentUser.IsMarkedAsUnchanged)
                    this.CurrentUser.MarkAsModified();
            }
        }

        /// <summary>
        /// Sauvegarde le nouveau mot de passe de l'utilisateur.
        /// </summary>
        /// <param name="user">L'utilisateur.</param>
        private void SaveNewPassword(User user)
        {
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.NewPassword) || !string.IsNullOrEmpty(user.ConfirmNewPassword))
                {
                    if (string.Equals(user.NewPassword, user.ConfirmNewPassword))
                    {
                        user.Password = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
                            Encoding.Default.GetBytes(user.NewPassword));
                    }
                }
            }
        }

        /// <summary>
        /// Valide les utilisateurs.
        /// </summary>
        /// <returns><c>true</c> si tous les utilisateurs sont valides.</returns>
        private bool ValidateUsers()
        {
            // Sauvegarder les rôles de l'utilisateur actuel
            SaveNewPassword(this.CurrentUser);

            foreach (var user in this.Users)
                user.Validate();

            base.RefreshValidationErrors(this.Users);

            if (this.Users.Any(u => !u.IsValid.GetValueOrDefault()))
            {
                foreach (var user in this.Users)
                    user.EnableAutoValidation = true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected override void OnRefreshValidationErrorsRequested()
        {
            ValidateUsers();
        }

        /// <summary>
        /// Sauvegarde les utilisateurs.
        /// </summary>
        private async Task Save()
        {

            var usersToSave = Users.Where(u => u.ChangeTracker.State != ObjectState.Unchanged).ToList();

            if (_usersPendingForDelete != null)
            {
                foreach (var user in _usersPendingForDelete)
                    user.IsDeleted = true;
                usersToSave.AddRange(_usersPendingForDelete);
            }

            ShowSpinner();
            try
            {
                await ServiceBus.Get<IApplicationUsersService>().SaveUsers(usersToSave);

                _usersPendingForDelete?.Clear();

                foreach (var user in Users)
                {
                    user.NewPassword = null;
                    user.ConfirmNewPassword = null;
                    user.AcceptChanges();
                    user.ExternalValidation = null;
                }

                RefreshValidationErrors(Users);

                var loggedUser = usersToSave.FirstOrDefault(u => u.Username == Security.SecurityContext.CurrentUser.Username);
                if (loggedUser != null)
                {
                    Security.SecurityContext.CurrentUser.UpdateNames(loggedUser);
                    EventBus.Publish(new CurrentUserChangedEvent(this));
                }

                HideSpinner();
                HasChanged = false;
            }
            catch (BLLException e)
            {
                if (e.ErrorCode == KnownErrorCodes.CannotUseSameUserName)
                {
                    var users = (User[])e.InnerException.Data[KnownErrorCodes.CannotUseSameUserName_UsersKey];
                    var usersString = string.Join("\r\n", users
                        .Select(u => u.FullName));

                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        string.Format(
                            LocalizationManager.GetString("VM_ApplicationMembers_Message_CannotUseSameUserName"),
                            usersString
                        ),
                        LocalizationManager.GetString("Common_Error"),
                        MessageDialogButton.OK, MessageDialogImage.Error);

                    foreach (var user in users)
                    {
                        user.ExternalValidation = () =>
                        {
                            return new List<KeyValuePair<string, string>>()
                                {
                                    new KeyValuePair<string, string>("Username", LocalizationManager.GetString("VM_ApplicationMembers_Validation_CannotUseSameUserName")),
                                };
                        };
                        ValidateUsers();
                    }

                    HideSpinner();
                    return;
                }
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        #endregion

        #region Types imbriqués

        /// <summary>
        /// Représente un rôle.
        /// </summary>
        private class PresentationRole : NotifiableObject, IPresentationRole, IComparable<IPresentationRole>
        {
            private IApplicationUsersViewModel _vm;

            public PresentationRole(IApplicationUsersViewModel vm, Role role)
            {
                _vm = vm;
                Role = role;

                HasSeparator |= role.RoleCode == Security.KnownRoles.Exporter;
            }

            /// <summary>
            /// Obtient le rôle associé.
            /// </summary>
            public Role Role { get; private set; }

            private bool _isChecked;
            /// <summary>
            /// Obtient ou définit une valeur indiquant si le rôle est coché.
            /// </summary>
            public bool IsChecked
            {
                get { return _isChecked; }
                set
                {
                    if (_isChecked != value)
                    {
                        _isChecked = value;
                        OnPropertyChanged("IsChecked");
                        _vm.OnRoleChecked(this.Role, value);
                    }
                }
            }

            /// <summary>
            /// Obtient une valeur indiquant si un séparateur est présent avant cet élément.
            /// </summary>
            public bool HasSeparator { get; private set; }


            #region IComparable<IPresentationRole> Members

            /// <inheritdoc />
            public int CompareTo(IPresentationRole other)
            {

                if (Role.RoleCode == Security.KnownRoles.Exporter)
                    return 1;
                if (other.Role.RoleCode == Security.KnownRoles.Exporter)
                    return -1;
                return string.Compare(Role.ShortLabel, other.Role.ShortLabel);
            }

            #endregion
        }

        #endregion

    }
}