using KProcess.KL2.APIClient;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Security.Business;
using KProcess.Presentation.Windows;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LocalizationManager = KProcess.Globalization.LocalizationManager;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente du VM d'authentification.
    /// </summary>
    class AuthenticationViewModel : KsmedViewModelBase, IAuthenticationViewModel
    {

        #region Champs privés

        string _userName;
        string _password;
        string _domain;
        Visibility _domainVisibility;
        Language[] _languages;
        Language _selectedLanguage;

        #endregion

        #region Surcharges

        /// <inheritdoc />
        protected override void Initialize()
        {
            base.Initialize();
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Obtient le titre
        /// </summary>
        public override string Title =>
            LocalizationManagerExt.GetSafeDesignerString("VM_Authentication_Title");

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            DomainVisibility = SecurityContext.CurrentAuthenticationModeNeedsDomain() ? Visibility.Visible : Visibility.Collapsed;

            ShowSpinner();
            try
            {
                var settingsService = IoC.Resolve<IServiceBus>().Get<ISettingsService>();
                Username = settingsService.LastUserName;

                Languages = await ServiceBus.Get<IAppResourceService>().GetLanguages();

                var osCulture = CultureInfo.InstalledUICulture.Name;
                var osLanguage = Languages.DefaultIfEmpty(null).SingleOrDefault(l => l.LanguageCode == settingsService.LastCulture);
                if (osLanguage == null)
                    osLanguage = Languages.DefaultIfEmpty(null).SingleOrDefault(l => l.LanguageCode == osCulture);
                if (osLanguage == null)
                    osLanguage = Languages.Single(l => l.LanguageCode == "fr-FR");
                SelectedLanguage = osLanguage;

                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }

#if DEBUG
            Username = "admin"; //che
            Password = "admin"; //pass
#endif
        }

        protected override Task OnInitializeDesigner()
        {
            DomainVisibility = Visibility.Visible;

            Languages = DesignData.GenerateLanguages().ToArray();

            SelectedLanguage = Languages.FirstOrDefault(l => l.LanguageCode == LocalizationManager.CurrentCulture.Name);

            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le login.
        /// </summary>
        public string Username
        {
            get { return _userName; }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le mot de passe.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le domaine.
        /// </summary>
        public string Domain
        {
            get { return _domain; }
            set
            {
                if (_domain != value)
                {
                    _domain = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient la visibilité du domaine.
        /// </summary>
        public Visibility DomainVisibility
        {
            get { return _domainVisibility; }
            private set
            {
                if (_domainVisibility != value)
                {
                    _domainVisibility = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la langue sélectionnée.
        /// </summary>
        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    OnPropertyChanged();

                    SwitchCulture(_selectedLanguage.LanguageCode);
                }
            }
        }

        /// <summary>
        /// Obtient la version.
        /// </summary>
        public string Version { get; private set; }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns></returns>
        protected override bool OnValidateCommandCanExecute() =>
            !string.IsNullOrWhiteSpace(Username);

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            ShowSpinner();
            try
            {
                var logon = await SecurityContext.TryLogonUser(Username, Password, SelectedLanguage.LanguageCode);

                if (!logon.Result)
                {
                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        IoC.Resolve<ILocalizationManager>().GetString("VM_Authentication_Message_InvalidCredentials"),
                        IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                        image: MessageDialogImage.Error);
                    return;
                }

                // Vérifier qu'il s'agit bien d'un administrateur ou un analyste, car eux seuls ont accès à KL²
                if (!SecurityContext.HasCurrentUserRole(KnownRoles.Administrator)
                    && !SecurityContext.HasCurrentUserRole(KnownRoles.Analyst))
                {
                    SecurityContext.DisconnectCurrentUser();
                    DialogFactory.GetDialogView<IMessageDialog>().Show(
                        IoC.Resolve<ILocalizationManager>().GetString("VM_Authentication_Message_InvalidRoles"),
                        IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                        image: MessageDialogImage.Error);
                    return;
                }

                // Vérifier si la BDD est accessible en mode partagé
                try
                {
                    bool isLocked = await ServiceBus.Get<ISharedDatabasePresentationService>()
                        .IsLocked(logon.User.Username);

                    if (isLocked)
                    {
                        SecurityContext.DisconnectCurrentUser();
                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                            IoC.Resolve<ILocalizationManager>()
                                .GetString("VM_Authentication_Message_SharedDatabaseLocked"),
                            IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                            image: MessageDialogImage.Error);
                        return;
                    }

                    SecurityContext.CurrentProductLicense = await IoC.Resolve<IAPIHttpClient>().ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");

                    if (SecurityContext.CurrentProductLicense.Status == WebLicenseStatus.NotFound)
                    {
                        SecurityContext.DisconnectCurrentUser();
                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                            IoC.Resolve<ILocalizationManager>()
                                .GetString("VM_Authentication_Message_NoLicenseFound"),
                            IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                            image: MessageDialogImage.Error);
                        return;
                    }
                    else if (SecurityContext.CurrentProductLicense.Status == WebLicenseStatus.OverageOfUsers
                        || SecurityContext.CurrentProductLicense.Status == WebLicenseStatus.Expired
                        || !SecurityContext.CurrentProductLicense.UsersPool.Contains(logon.User.UserId))
                    {
                        SecurityContext.DisconnectCurrentUser();
                        DialogFactory.GetDialogView<IMessageDialog>().Show(
                            IoC.Resolve<ILocalizationManager>()
                                .GetString("VM_Authentication_Message_NotAuthorized"),
                            IoC.Resolve<ILocalizationManager>().GetString("Common_Error"),
                            image: MessageDialogImage.Error);
                        return;
                    }

                    ServiceBus.Get<ISharedDatabasePresentationService>().Initialize();

                    await SaveUserSetting(logon.User);

                    // On sauvegarde aussi la dernière langue dans le fichier config, ainsi que le dernier identifiant utilisateur
                    var settingsService = IoC.Resolve<IServiceBus>().Get<ISettingsService>();
                    settingsService.LastCulture = SelectedLanguage.LanguageCode;
                    settingsService.LastUserName = logon.User.Username;

                    Language languageToApply = SelectedLanguage;

                    // On applique la langue sélectionnée
                    if (languageToApply != null)
                        IoC.Resolve<ILocalizationManager>().CurrentCulture =
                            CultureInfo.GetCultureInfo(languageToApply.LanguageCode);
                    AnnotationsLib.LocalizationExt.CurrentCulture = IoC.Resolve<ILocalizationManager>().CurrentCulture;
                    DlhSoft.LocalizationExt.CurrentCulture = IoC.Resolve<ILocalizationManager>().CurrentCulture;

                    await Task.Factory.StartNew(() => LocalizationManager.CurrentCulture = IoC.Resolve<ILocalizationManager>().CurrentCulture);

                    EventBus.Publish(new CultureChangedEvent(this));

                    Shutdown();
                }
                catch (Exception ex)
                {
                    TraceManager.TraceDebug("AuthenticationViewModel.OnValidateCommandExecute :");
                    TraceManager.TraceDebug($"Exception :\n{ex.Message}");
                    if (ex.InnerException != null)
                        TraceManager.TraceDebug($"InnerException :\n{ex.InnerException.Message}");

                    if (ex.InnerException is ServerNotReacheableException)
                        OnError(ex.InnerException);
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
            finally
            {
                HideSpinner();
            }
        }

        async Task SaveUserSetting(User user)
        {
            if (user.DefaultLanguageCode != SelectedLanguage.LanguageCode)
            {
                user.StartTracking();
                user.DefaultLanguage = SelectedLanguage;
                try
                {
                    await ServiceBus.Get<IAuthenticationService>().SaveUser(user);
                }
                catch (Exception ex)
                {
                    TraceManager.TraceDebug($"AuthenticationService.SaveUser : Erreur lors de l'enregistrement de l'utilisateur {user.FullName}");
                    TraceManager.TraceDebug($"Exception :\n{ex.Message}");
                    if (ex.InnerException != null)
                        TraceManager.TraceDebug($"InnerException :\n{ex.InnerException.Message}");
                }
            }
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute() =>
            Shutdown();

        #endregion

        #region Methods

        void SwitchCulture(string culture)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            try
            {
                ci = new CultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
                try
                {
                    // Try language without region
                    ci = new CultureInfo("en-GB");
                }
                catch (Exception)
                {
                    ci = CultureInfo.InvariantCulture;
                }
            }
            finally
            {
                var localizationManager = IoC.Resolve<ILocalizationManager>();
                localizationManager.CurrentCulture = ci;
                LocalizationManager.CurrentCulture = ci;
                LocalizationManagerExt.UpdateLocalizations();
            }
        }

        #endregion
    }
}