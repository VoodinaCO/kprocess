using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Security.Extensions;
using KProcess.Presentation.Windows;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    [HasSelfValidation]
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des membres du projet.
    /// </summary>
    class PrepareMembersViewModel : FrameContentViewModelBase, IPrepareMembersViewModel
    {
        #region Champs privés

        private User _currentMember;
        private bool _canChange = true;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        private IFrameNavigationToken _navigationToken = null;

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            ShowSpinner();
            var projectInfo = ServiceBus.Get<IProjectManagerService>().CurrentProject;
            var prepareService = ServiceBus.Get<IPrepareService>();
            CurrentProcess = await prepareService.GetProcess(projectInfo.ProcessId, false);
            CurrentProject = prepareService.GetProjectSync(projectInfo.ProjectId);
            try
            {
                await Load(CurrentProcess.ProcessId);
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        private async Task Load(int processId)
        {
            var (users, roles) = await ServiceBus.Get<IPrepareService>().GetMembers(processId);
            Users = users.ToArray();
            ActivatedUsers = new List<User>(await FilterByActivatedUser(Users.Where(u => !u.IsDeleted && u.Roles.Any(r => r.RoleCode == KnownRoles.Administrator || r.RoleCode == KnownRoles.Analyst)), u => u.UserId));
            var owner = Users.Single(_ => _.UserId == CurrentProcess.OwnerId);
            ActivatedUsersAndOwner = new List<User>(ActivatedUsers);
            if (!ActivatedUsersAndOwner.Any(_ => _.UserId == owner.UserId))
            {
                ActivatedUsersAndOwner.Add(owner);
                ActivatedUsersAndOwner = ActivatedUsersAndOwner.OrderBy(_ => _.FullName).ToList();
            }
            OnPropertyChanged(nameof(Owner));
            OnPropertyChanged(nameof(CanUpdateRights));
        }

        public static async Task<IEnumerable<T>> FilterByActivatedUser<T>(IEnumerable<T> collection, Func<T, int> getUserId)
        {
            var apiClient = IoC.Resolve<IAPIHttpClient>();
            var license = await apiClient.ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");
            return collection.Where(_ => license.UsersPool.Any(u => u == getUserId(_)));
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Users.AddRange(DesignData.GenerateUsers());
            CurrentMember = Users.FirstOrDefault();
            OnPropertyChanged(nameof(Owner));

            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le membre courant.
        /// </summary>
        public User CurrentMember
        {
            get { return _currentMember; }
            set
            {
                if (_currentMember != value)
                {
                    _currentMember = value;
                    OnPropertyChanged();
                }
            }
        }

        Procedure _currentProcess;
        public Procedure CurrentProcess
        {
            get => _currentProcess;
            set
            {
                if (_currentProcess == value)
                    return;
                _currentProcess = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Owner));
            }
        }

        Project _currentProject;
        public Project CurrentProject
        {
            get => _currentProject;
            set
            {
                if (_currentProject == value)
                    return;
                _currentProject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le projet courant est en lecture seule.
        /// </summary>
        public bool IsReadOnly =>
            CurrentProject?.IsReadOnly ?? true;

        /// <summary>
        /// Obtient ou définit le propriétaire du projet.
        /// </summary>
        public User Owner
        {
            get => CurrentProcess == null ? null : Users?.SingleOrDefault(u => u.UserId == CurrentProcess.OwnerId);
            set
            {
                if (CurrentProcess == null || value == null || CurrentProcess.OwnerId == value.UserId)
                    return;
                CurrentProcess.OwnerId = value.UserId;
                OnPropertyChanged();
                CanChange = false;
            }
        }

        /// <summary>
        /// Obtient les utilisateurs.
        /// </summary>
        User[] _users;
        public User[] Users
        {
            get => _users;
            private set
            {
                if (_users == value)
                    return;
                _users = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Owner));
            }
        }

        /// <summary>
        /// Obtient les utilisateurs actifs.
        /// </summary>
        List<User> _activatedUsers;
        public List<User> ActivatedUsers
        {
            get => _activatedUsers;
            private set
            {
                if (_activatedUsers == value)
                    return;
                _activatedUsers = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obtient les utilisateurs actifs et le propriétaire.
        /// </summary>
        List<User> _activatedUsersAndOwner;
        public List<User> ActivatedUsersAndOwner
        {
            get => _activatedUsersAndOwner;
            private set
            {
                if (_activatedUsersAndOwner == value)
                    return;
                _activatedUsersAndOwner = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        public bool CanChange
        {
            get => _canChange;
            private set
            {
                if (_canChange == value)
                    return;
                _canChange = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si les droits peut être modifiés.
        /// </summary>
        public bool CanUpdateRights =>
            CurrentProcess != null
            && SecurityContext.CurrentUser?.User != null
            && (SecurityContext.HasCurrentUserRole(KnownRoles.Administrator)
                || CurrentProcess.OwnerId == SecurityContext.CurrentUser.User.UserId);

        #endregion

        #region Commandes

        Command<User> _checkCanWriteCommand;
        /// <summary>
        /// Obtient la commande permettant de cocher/décocher les droits d'écriture.
        /// </summary>
        public ICommand CheckCanWriteCommand
        {
            get
            {
                if (_checkCanWriteCommand == null)
                    _checkCanWriteCommand = new Command<User>(async user =>
                    {
                        if (CurrentProcess.CanWrite(user)) // L'utilisateur a les droits d'écriture, donc on doit les enlever
                        {
                            CurrentProcess.UserRoleProcesses.Single(urp => urp.UserId == user.UserId
                                                                          && urp.RoleCode == KnownRoles.Analyst)
                                .MarkAsDeleted();
                            CurrentProcess.UserRoleProcesses.Add(new UserRoleProcess
                            {
                                UserId = user.UserId,
                                RoleCode = KnownRoles.Contributor
                            });
                        }
                        else // L'utilisateur n'a pas les droits d'écriture, donc on doit les ajouter
                        {
                            CurrentProcess.UserRoleProcesses.Add(new UserRoleProcess
                            {
                                UserId = user.UserId,
                                RoleCode = KnownRoles.Analyst
                            });
                            CurrentProcess.UserRoleProcesses.SingleOrDefault(urp => urp.UserId == user.UserId
                                                                          && urp.RoleCode == KnownRoles.Contributor)
                                ?.MarkAsDeleted();
                        }
                        CurrentProcess.MarkAsModified();
                        ValidateCommand.Execute(null);

                        await SignalRFactory
                                .GetSignalR<IKL2AnalyzeHubConnect>()
                                .RefreshProcess(new AnalyzeEventArgs(nameof(CheckCanWriteCommand)));

                    }, user =>
                    {
                        if (IsReadOnly)
                        {
                            return false;
                        }
                        if (user == null || CurrentProcess == null)
                            return false;
                        if (user.Roles.Any(r => r.RoleCode == KnownRoles.Administrator))
                            return false;
                        if (user.UserId == CurrentProcess.OwnerId)
                            return false;
                        return CanUpdateRights;
                    });
                return _checkCanWriteCommand;
            }
        }

        Command<User> _checkCanReadCommand;
        /// <summary>
        /// Obtient la commande permettant de cocher/décocher les droits de lecture.
        /// </summary>
        public ICommand CheckCanReadCommand
        {
            get
            {
                if (_checkCanReadCommand == null)
                    _checkCanReadCommand = new Command<User>(async user =>
                    {
                        if (CurrentProcess.CanRead(user)) // L'utilisateur a les droits de lecture, donc on doit les enlever
                        {
                            CurrentProcess.UserRoleProcesses.Single(urp => urp.UserId == user.UserId
                                                                          && urp.RoleCode == KnownRoles.Contributor)
                                .MarkAsDeleted();
                        }
                        else // L'utilisateur n'a pas les droits de lecture, donc on doit les ajouter
                        {
                            CurrentProcess.UserRoleProcesses.Add(new UserRoleProcess
                            {
                                UserId = user.UserId,
                                RoleCode = KnownRoles.Contributor
                            });
                        }
                        CurrentProcess.MarkAsModified();
                        ValidateCommand.Execute(null);

                        await SignalRFactory
                                .GetSignalR<IKL2AnalyzeHubConnect>()
                                .RefreshProcess(new AnalyzeEventArgs(nameof(CheckCanReadCommand)));

                    }, user =>
                    {
                        if (IsReadOnly)
                        {
                            return false;
                        }
                        if (user == null)
                            return false;
                        if (user.Roles.Any(r => r.RoleCode == KnownRoles.Administrator))
                            return false;
                        if (user.UserId == CurrentProcess.OwnerId)
                            return false;
                        if (CurrentProcess.CanWrite(user))
                            return false;
                        return CanUpdateRights;
                    });
                return _checkCanReadCommand;
            }
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'execution de la commande RemoveCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        /*protected override bool OnRemoveCommandCanExecute()
        {
            return CurrentMember != null && RemoveMemberCommand.CanExecute(CurrentMember);
        }*/

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande RemoveCommand
        /// </summary>
        /*protected override void OnRemoveCommandExecute()
        {
            RemoveMemberCommand.Execute(CurrentMember);
        }*/

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            !CanChange;

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            !CanChange;


        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            base.OnValidateCommandExecute();

            // Vérifier si l'objet a changé
            if (CurrentProcess.IsNotMarkedAsUnchanged)
            {
                if (!CurrentProcess.CanWrite(Owner)) // On change le propriétaire, mais il n'avait pas les droits d'écriture
                {
                    CurrentProcess.UserRoleProcesses.Add(new UserRoleProcess
                    {
                        UserId = Owner.UserId,
                        RoleCode = KnownRoles.Analyst
                    });
                    CurrentProcess.UserRoleProcesses.SingleOrDefault(urp => urp.UserId == Owner.UserId
                                                                  && urp.RoleCode == KnownRoles.Contributor)
                        ?.MarkAsDeleted();
                }
                if (await SaveCurrentProcess())
                {
                    await SignalRFactory
                        .GetSignalR<IKL2AnalyzeHubConnect>()
                        .RefreshProcess(new AnalyzeEventArgs(nameof(Owner)));
                    // On recrée les users
                    ActivatedUsersAndOwner = new List<User>(ActivatedUsers);
                    if (!ActivatedUsersAndOwner.Any(_ => _.UserId == Owner.UserId))
                    {
                        ActivatedUsersAndOwner.Add(Owner);
                        ActivatedUsersAndOwner = ActivatedUsersAndOwner.OrderBy(_ => _.FullName).ToList();
                    }
                    OnPropertyChanged(nameof(CanUpdateRights));
                    CanChange = true;
                    if (_navigationToken?.IsValid == true)
                        _navigationToken.Navigate();
                }
            }
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            CurrentProcess.CancelChanges();
            OnPropertyChanged(nameof(Owner));
            CanChange = true;
            HideValidationErrors();
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (CurrentProcess != null &&
                (!CanChange || CurrentProcess.IsNotMarkedAsUnchanged && !CurrentProcess.IsValid.GetValueOrDefault()))
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            if (newState != ObjectState.Unchanged)
                CanChange = false;
        }

        /// <summary>
        /// Sauvegarde les rôles de l'utilisateur spécifié.
        /// </summary>
        private async Task<bool> SaveCurrentProcess()
        {
            ShowSpinner();

            // Si on désire changer de propriétaire
            // que l'utilisateur courant est le propriétaire du process
            // qu'il ne s'agit pas d'un administrateur
            // on doit avertir l'utilisateur qu'il ne pourra plus modifier les droits
            if (CurrentProcess.ChangeTracker.ModifiedValues.Any(kpv => kpv.Key == nameof(Procedure.OwnerId))
                && SecurityContext.CurrentUser.User.UserId == (int)CurrentProcess.ChangeTracker.OriginalValues.Single(kpv => kpv.Key == nameof(Procedure.OwnerId)).Value
                && !SecurityContext.HasCurrentUserRole(KnownRoles.Administrator))
            {
                var changeProprietary = IoC.Resolve<IDialogFactory>().GetDialogView<IMessageDialog>().Show(
                    IoC.Resolve<ILocalizationManager>().GetString("View_PrepareMembers_AskChangeOwner"),
                    IoC.Resolve<ILocalizationManager>().GetString("Common_Confirmation"),
                    MessageDialogButton.YesNoCancel,
                    MessageDialogImage.Question);
                if (changeProprietary != MessageDialogResult.Yes)
                {
                    HideSpinner();
                    return false;
                }
            }

            try
            {
                await ServiceBus.Get<IPrepareService>().SaveProcess(CurrentProcess);
                foreach (var urp in CurrentProcess.UserRoleProcesses)
                    urp.AcceptChanges();
                CurrentProcess.AcceptChanges();
                OnPropertyChanged(nameof(CurrentProcess));
                HideSpinner();
                return true;
            }
            catch (Exception e)
            {
                base.OnError(e);
                return false;
            }
        }

        #endregion

    }
}