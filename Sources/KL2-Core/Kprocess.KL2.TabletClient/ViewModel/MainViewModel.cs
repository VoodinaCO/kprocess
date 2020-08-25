using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Kprocess.KL2.TabletClient.Converter;
using Kprocess.KL2.TabletClient.Dialog;
using Kprocess.KL2.TabletClient.Models;
using Kprocess.KL2.TabletClient.Services;
using Kprocess.KL2.TabletClient.Views;
using KProcess;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Helpers;
using KProcess.Ksmed.Security;
using KProcess.Ksmed.Security.Activation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, IDialogCoordinator
    {
        readonly IDialogCoordinator dialogCoordinator = DialogCoordinator.Instance;
        CustomDialog loginDialog;
        CustomDialog errorLoginDialog;
        CustomDialog errorSyncingDialog;
        CustomDialog errorPublicationDialog;
        ExitDialog exitDialog;
        static bool forceClose;

        BindingList<User> _users = new BindingList<User>();
        public BindingList<User> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    RaisePropertyChanged();
                }
            }
        }

        User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    RaisePropertyChanged();

                    SelectedLanguage = Languages.Single(_ => _.LanguageCode == (_selectedUser?.DefaultLanguageCode ?? "fr-FR"));
                }
            }
        }

        BindingList<Language> _languages = new BindingList<Language>();
        public BindingList<Language> Languages
        {
            get => _languages;
            set
            {
                if (_languages != value)
                {
                    _languages = value;
                    RaisePropertyChanged();
                }
            }
        }

        Language _selectedLanguage;
        public Language SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    RaisePropertyChanged();

                    Locator.LocalizationManager.CurrentCulture = new CultureInfo(_selectedLanguage.LanguageCode);
                    KProcess.Globalization.LocalizationManager.CurrentCulture = new CultureInfo(_selectedLanguage.LanguageCode);
                    KProcess.Presentation.Windows.LocalizationManagerExt.UpdateLocalizations();
                    RaisePropertyChanged("");
                }
                (ConnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string PublicationName
        {
            get
            {
                List<Publication> publications = new List<Publication>
                {
                    _trainingPublication,
                    _evaluationPublication,
                    _inspectionPublication
                };
                return publications.FirstOrDefault(_ => _ != null)?.Label;
            }
        }

        Publication _trainingPublication;
        public Publication TrainingPublication
        {
            get => _trainingPublication;
            set
            {
                if (_trainingPublication != value)
                {
                    _trainingPublication = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(PublicationName));
                }
            }
        }

        Publication _evaluationPublication;
        public Publication EvaluationPublication
        {
            get => _evaluationPublication;
            set
            {
                if (_evaluationPublication != value)
                {
                    _evaluationPublication = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(PublicationName));
                }
            }
        }

        Publication _inspectionPublication;
        public Publication InspectionPublication
        {
            get => _inspectionPublication;
            set
            {
                if (_inspectionPublication != value)
                {
                    _inspectionPublication = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(PublicationName));
                }
            }
        }

        KProcess.Ksmed.Models.Inspection _inspection;
        public KProcess.Ksmed.Models.Inspection Inspection
        {
            get => _inspection;
            set
            {
                if (_inspection != value)
                {
                    _inspection = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient un booléen indiquant si la page est en cours de chargement
        /// </summary>
        bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    if (!_isLoading)
                        _showDisconnectedMessage = false;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le texte à afficher lors du chargement
        /// </summary>
        string _loadingText;
        public string LoadingText
        {
            get => _loadingText;
            set
            {
                if (_loadingText != value)
                {
                    _loadingText = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le lecteur
        /// </summary>
        Unosquare.FFME.MediaElement _mediaElement;
        public Unosquare.FFME.MediaElement MediaElement
        {
            get => _mediaElement;
            set
            {
                if (_mediaElement != value)
                {
                    _mediaElement = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    RaisePropertyChanged();
                }
            }
        }

        bool _isConnecting;
        public bool IsConnecting
        {
            get => _isConnecting;
            private set
            {
                if (_isConnecting != value)
                {
                    _isConnecting = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _errorText;
        public string ErrorText
        {
            get => _errorText;
            private set
            {
                if (_errorText != value)
                {
                    _errorText = value;
                    RaisePropertyChanged();
                }
            }
        }

        string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                }
                (ConnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public bool ShowCamera =>
            _currentView != null && _currentView is Views.Inspection && _currentView.DataContext is InspectionViewModel vm && !vm.IsReadOnly;

        UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                if (_currentView != null)
                    _currentView.DataContext = null;

                if (_currentView != value)
                {
                    _currentView = value;
                    RaisePropertyChanged();
                }

                (NavigateToHome as RelayCommand)?.RaiseCanExecuteChanged();
                (DisconnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (NavigateToSnapshotCommand as RelayCommand)?.RaiseCanExecuteChanged();
                RaisePropertyChanged(nameof(ShowCamera));
            }
        }
        public BindingList<Flyout> Flyouts { get; } = new BindingList<Flyout>();

        bool _isOnlineIcon;
        public bool IsOnlineIcon
        {
            get => _isOnlineIcon;
            set
            {
                if (_isOnlineIcon != value)
                {
                    _isOnlineIcon = value;
                    RaisePropertyChanged();
                    (DisconnectCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        bool _showDisconnectedMessage;
        public bool ShowDisconnectedMessage
        {
            get => _showDisconnectedMessage;
            set
            {
                if (_showDisconnectedMessage != value)
                    _showDisconnectedMessage = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Affiche la vue permettant de sélectionner un dossier
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <returns>
        /// le dossier sélectionné, ou null si annulé
        /// </returns>
        public static string ShowOpenFolderDialog(string caption)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = caption,
            })
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    return dialog.SelectedPath;
                else
                    return null;
            }
        }

        ICommand _loadedCommand;
        public ICommand LoadedCommand
        {
            get
            {
                if (_loadedCommand == null)
                    _loadedCommand = new RelayCommand(() =>
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                        {
                            // Test Sync path before showing Authentication
                            while (!FilesHelper.TestSyncDirectoryRights())
                            {
                                var syncFolder = ShowOpenFolderDialog(Locator.LocalizationManager.GetString("Common_NoWritingRightsOnSyncPath"));
                                if (!string.IsNullOrEmpty(syncFolder))
                                {
                                    // Set SyncPath in config file and reload it
                                    FilesHelper.SetSyncFilesLocation(syncFolder);
                                    FilesHelper.SetSyncFilesLocation(syncFolder, Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Kprocess.KL2.SyncService.exe"));
                                }
                            }

                            IsConnecting = true;
                            IsOnlineIcon = Locator.APIManager.IsOnline ?? false;
                            Locator.TraceManager.TraceDebug($"{Locator.APIManager.IsOnline}");
                            if (Locator.APIManager.IsOnline == true) // Mode connecté
                            {
                                await ShowAppLogin();
                                try
                                {
                                    int last_userId = await OfflineFile.LastUser.GetFromJson<int>();
                                    var last_user = Users.SingleOrDefault(u => u.UserId == last_userId);
                                    var last_user_token = await OfflineFile.Token.GetFromJson<string>();
                                    if (last_user != null && !string.IsNullOrEmpty(last_user_token))
                                    {
                                        if (await Locator.Resolve<IAPIHttpClient>().Relogon(last_user_token))
                                        {
                                            var previousTrainingPublication = await OfflineFile.Training.GetFromJson<Publication>();
                                            var previousEvaluationPublication = await OfflineFile.Evaluation.GetFromJson<Publication>();
                                            var previousInspectionPublication = await OfflineFile.Inspection.GetFromJson<Publication>();
                                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, previousTrainingPublication, false);
                                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, previousEvaluationPublication, false);
                                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, previousInspectionPublication, false);
                                            OfflineFile.LastUser.DeleteJson();
                                            OfflineFile.Training.DeleteJson();
                                            OfflineFile.Evaluation.DeleteJson();
                                            OfflineFile.Inspection.DeleteJson();
                                            OfflineFile.Token.DeleteJson();
                                            Locator.Resolve<IAPIHttpClient>().Token = null;
                                        }
                                        IsConnected = false;
                                    }
                                }
                                catch (Exception e)
                                {
                                    TraceManager.TraceDebug(e, e.Message);
                                }
                            }
                            else // Mode hors ligne, on doit récupérer les infos de connexion ultérieure, si inexistante, message d'erreur
                            {
                                try
                                {
                                    Languages = new BindingList<Language>(await OfflineFile.Languages.GetFromJson<BindingList<Language>>());
                                    Users = new BindingList<User>((await OfflineFile.Users.GetFromJson<BindingList<User>>())
                                        .Where(u => u.IsActive
                                            && u.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator }.Contains(r.RoleCode)))
                                        .OrderBy(u => u.FullName)
                                        .ToList());
                                    int last_userId = await OfflineFile.LastUser.GetFromJson<int>();
                                    User lastUser = Users.SingleOrDefault(u => u.UserId == last_userId);
                                    //await ConnectOffline(lastUser);
                                    await ShowAppLoginOffline(lastUser);
                                }
                                catch (Exception ex)
                                {
                                    await ShowErrorLogin(ex);
                                }
                            }
                            SelectedLanguage = Languages.SingleOrDefault(_ => _.LanguageCode == (SelectedUser?.DefaultLanguageCode ?? "fr-FR"));

                            IsConnecting = false;
                        });
                    });
                return _loadedCommand;
            }
        }

        ICommand _connectCommand;
        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                    _connectCommand = new RelayCommand(async () =>
                    {
                        IsConnecting = true;
                        var _apiHttpClient = Locator.Resolve<IAPIHttpClient>();
                        try
                        {
                            if (IsOnlineIcon)
                            {
                                // Check license
                                var currentLicense = await Locator.Resolve<IAPIHttpClient>().ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");
                                if (currentLicense.Status == WebLicenseStatus.NotFound)
                                {
                                    DisconnectCommand.Execute(null);
                                    await dialogCoordinator.ShowMessageAsync(this, Locator.LocalizationManager.GetString("Common_Error"),
                                        Locator.LocalizationManager.GetString("VM_Authentication_Message_NoLicenseFound"),
                                        MessageDialogStyle.Affirmative,
                                        new MetroDialogSettings
                                        {
                                            AffirmativeButtonText = Locator.LocalizationManager.GetString("Common_OK")
                                        });
                                    IsConnecting = false;
                                    return;
                                }
                                else if (currentLicense.Status == WebLicenseStatus.Expired)
                                {
                                    DisconnectCommand.Execute(null);
                                    await dialogCoordinator.ShowMessageAsync(this, Locator.LocalizationManager.GetString("Common_Error"),
                                        Locator.LocalizationManager.GetString("VM_Authentication_Message_NotAuthorized"),
                                        MessageDialogStyle.Affirmative,
                                        new MetroDialogSettings
                                        {
                                            AffirmativeButtonText = Locator.LocalizationManager.GetString("Common_OK")
                                        });
                                    IsConnecting = false;
                                    return;
                                }

                                if (ValidatePassword(_selectedUser, _password))
                                {
                                    Locator.TraceManager.TraceDebug("Connexion réussie. Récupération du token.");
                                    await Locator.Resolve<IAPIHttpClient>().Logon(_selectedUser.Username, _password, _selectedLanguage.LanguageCode);

                                    // Save user
                                    OfflineFile.LastUser.SaveToJson(_selectedUser.UserId);
                                    _apiHttpClient.SetUserId(KL2_Server.API, _selectedUser.UserId);
                                    OfflineFile.Token.SaveToJson(_apiHttpClient.Token);
                                    IsConnected = true;

                                    var reasonList = await Locator.GetService<IPrepareService>().GetQualificationReasons();
                                    OfflineFile.QualificationReasons.SaveToJson(new BindingList<QualificationReason>(reasonList));
                                }
                                else
                                {
                                    ErrorText = "Les données d'authentification sont incorrectes.";
                                    Locator.TraceManager.TraceDebug(ErrorText);
                                }

                                if (_apiHttpClient.Token != null)
                                {
                                    // On charge les données déjà téléchargées
                                    _apiHttpClient.RefreshDownloadedFiles();
                                    // On synchronise le fichier de publication si besoin
                                    if (File.Exists(Path.Combine(OfflineManager.GetOfflinePath, OfflineManager.OfflineFiles[OfflineFile.Training].File)))
                                    {
                                        Locator.TraceManager.TraceDebug($"Synchronisation du fichier {OfflineManager.OfflineFiles[OfflineFile.Training].File}");
                                        Publication trainingPublication = await OfflineFile.Training.GetFromJson<Publication>();
                                        trainingPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, trainingPublication, false);
                                    }
                                    if (File.Exists(Path.Combine(OfflineManager.GetOfflinePath, OfflineManager.OfflineFiles[OfflineFile.Evaluation].File)))
                                    {
                                        Locator.TraceManager.TraceDebug($"Synchronisation du fichier {OfflineManager.OfflineFiles[OfflineFile.Evaluation].File}");
                                        Publication evaluationPublication = await OfflineFile.Evaluation.GetFromJson<Publication>();
                                        evaluationPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, evaluationPublication, false);
                                    }
                                    if (File.Exists(Path.Combine(OfflineManager.GetOfflinePath, OfflineManager.OfflineFiles[OfflineFile.Inspection].File)))
                                    {
                                        Locator.TraceManager.TraceDebug($"Synchronisation du fichier {OfflineManager.OfflineFiles[OfflineFile.Inspection].File}");
                                        Publication inspectionPublication = await OfflineFile.Inspection.GetFromJson<Publication>();
                                        inspectionPublication = await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, inspectionPublication, false);
                                    }
                                    await dialogCoordinator.HideMetroDialogAsync(this, loginDialog);
                                    loginDialog = null;
                                    // On change d'écran
                                    await Locator.Navigation.Push<ActivityChoice, ActivityChoiceViewModel>(new ActivityChoiceViewModel());
                                }
                            }
                            else
                            {
                                if (ValidatePassword(_selectedUser, _password))
                                {
                                    _apiHttpClient.SetUserId(KL2_Server.API, _selectedUser.UserId);
                                    _apiHttpClient.Token = await OfflineFile.Token.GetFromJson<string>(); ;
                                    IsConnected = true;
                                    // On charge les données déjà téléchargées
                                    TrainingPublication = await OfflineFile.Training.GetFromJson<Publication>();
                                    EvaluationPublication = await OfflineFile.Evaluation.GetFromJson<Publication>();
                                    InspectionPublication = await OfflineFile.Inspection.GetFromJson<Publication>();
                                    _apiHttpClient.RefreshDownloadedFiles();
                                    await dialogCoordinator.HideMetroDialogAsync(this, loginDialog);
                                    loginDialog = null;
                                    // On change d'écran
                                    await Locator.Navigation.Push<ActivityChoice, ActivityChoiceViewModel>(new ActivityChoiceViewModel());
                                }
                                else
                                {
                                    ErrorText = "Les données d'authentification sont incorrectes.";
                                    Locator.TraceManager.TraceDebug(ErrorText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorText = "Une erreur a été rencontrée lors de l'authentification.";
                            Locator.TraceManager.TraceError(ex, ErrorText);
                        }
                        IsConnecting = false;
                    }, () =>
                    {
                        return SelectedUser != null && !string.IsNullOrEmpty(_password);
                    });
                return _connectCommand;
            }
        }

        public bool ValidatePassword(User user, string password)
        {
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            {
                byte[] passwordHash = sha.ComputeHash(Encoding.Default.GetBytes(password));
                Locator.TraceManager.TraceDebug($"Tentative de connexion de l'utilisateur {user.Username}");
                return Users.Any(_ => _.UserId == user.UserId && _.Password.SequenceEqual(passwordHash));
            }
        }

        public async Task ConnectOffline(User lastUser)
        {
            IsLoading = true;
            ShowDisconnectedMessage = false;
            var _apiHttpClient = Locator.Resolve<IAPIHttpClient>();

            SelectedUser = lastUser;
            _apiHttpClient.SetUserId(KL2_Server.API, _selectedUser.UserId);
            IsConnected = true;
            // On charge les données déjà téléchargées
            TrainingPublication = await OfflineFile.Training.GetFromJson<Publication>();
            EvaluationPublication = await OfflineFile.Evaluation.GetFromJson<Publication>();
            InspectionPublication = await OfflineFile.Inspection.GetFromJson<Publication>();
            _apiHttpClient.RefreshDownloadedFiles();
            // On change d'écran
            await Locator.Navigation.Push<ActivityChoice, ActivityChoiceViewModel>(new ActivityChoiceViewModel());

            IsLoading = false;
        }

        ICommand _navigateToHome;
        public ICommand NavigateToHome
        {
            get
            {
                if (_navigateToHome == null)
                    _navigateToHome = new RelayCommand(async () =>
                    {
                        IsLoading = true;

                        try
                        {
                            if (Locator.APIManager.IsOnline == true)
                            {
                                ShowDisconnectedMessage = true;
                                await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, TrainingPublication);
                                await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, EvaluationPublication);
                                await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, InspectionPublication);
                            }
                            else
                            {
                                ShowDisconnectedMessage = false;
                                await Locator.APIManager.SyncPublicationOffline(OfflineFile.Training, TrainingPublication);
                                await Locator.APIManager.SyncPublicationOffline(OfflineFile.Evaluation, EvaluationPublication);
                                await Locator.APIManager.SyncPublicationOffline(OfflineFile.Inspection, InspectionPublication);
                            }
                        }
                        catch
                        {
                        }

                        Flyouts.Clear();
                        await Locator.Navigation.ToHome();
                        if(Locator.DownloadManager.IsDownloading == true)
                            await Locator.DownloadManager.StopDownload();
                        LoadingText = null;

                        IsLoading = false;
                    }, () =>
                    {
                        return CurrentView != null && !(CurrentView is ActivityChoice);
                    });
                return _navigateToHome;
            }
        }

        ICommand _navigateToSnapshotCommand;
        public ICommand NavigateToSnapshotCommand
        {
            get
            {
                if (_navigateToSnapshotCommand == null)
                    _navigateToSnapshotCommand = new RelayCommand(async () =>
                    {
                        await Locator.Navigation.Push<Snapshot, SnapshotViewModel>(new SnapshotViewModel());
                    }, () =>
                    {
                        return CurrentView != null && CurrentView is Views.Inspection && CurrentView.DataContext is InspectionViewModel vm && vm.Inspection != null;
                    });
                return _navigateToSnapshotCommand;
            }
        }

        ICommand _confirmExitCommand;
        public ICommand ConfirmExitCommand
        {
            get
            {
                if (_confirmExitCommand == null)
                    _confirmExitCommand = new RelayCommand<CancelEventArgs>(async (e) =>
                    {
                        if (forceClose)
                            return;
                        e.Cancel = true;
                        exitDialog = new ExitDialog
                        {
                            DataContext = this
                        };
                        await Locator.GetInstance<MainViewModel>().ShowMetroDialogAsync(null, exitDialog);
                    });
                return _confirmExitCommand;
            }
        }

        ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new RelayCommand(async () =>
                    {
                        forceClose = true;
                        await CloseApp();
                    });
                return _exitCommand;
            }
        }

        ICommand _forceExitCommand;
        public ICommand ForceExitCommand
        {
            get
            {
                if (_forceExitCommand == null)
                    _forceExitCommand = new RelayCommand(async () =>
                    {
                        if (exitDialog != null)
                            await dialogCoordinator.HideMetroDialogAsync(this, exitDialog);
                        forceClose = true;
                        await CloseApp();
                    });
                return _forceExitCommand;
            }
        }

        ICommand _cancelExitCommand;
        public ICommand CancelExitCommand
        {
            get
            {
                if (_cancelExitCommand == null)
                    _cancelExitCommand = new RelayCommand(async () =>
                    {
                        await dialogCoordinator.HideMetroDialogAsync(this, exitDialog);
                        exitDialog = null;
                    });
                return _cancelExitCommand;
            }
        }

        ICommand _disconnectCommand;
        public ICommand DisconnectCommand
        {
            get
            {
                if (_disconnectCommand == null)
                    _disconnectCommand = new RelayCommand(async () =>
                    {
                        try
                        {
                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Training, TrainingPublication);
                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Evaluation, EvaluationPublication);
                            await Locator.APIManager.SyncPublicationOnline(OfflineFile.Inspection, InspectionPublication);
                            Flyouts.Clear();
                            OfflineFile.Token.DeleteJson();
                            Locator.Resolve<IAPIHttpClient>().Token = null;
                            IsConnected = false;
                            if (CurrentView != null)
                            {
                                if (CurrentView.DataContext is ViewModelBase vm)
                                    vm.Cleanup();
                                CurrentView.DataContext = null;
                            }
                            CurrentView = null;
                            TrainingPublication = null;
                            EvaluationPublication = null;
                            InspectionPublication = null;
                            IsConnecting = true;
                            await ShowAppLogin();
                        }
                        catch (Exception e)
                        {
                            await ShowErrorSyncing(e);
                        }
                        IsConnecting = false;
                    }, () =>
                    {
                        return CurrentView != null && IsOnlineIcon;
                    });
                return _disconnectCommand;
            }
        }

        ICommand _cancelDisconnectCommand;
        public ICommand CancelDisconnectCommand
        {
            get
            {
                if (_cancelDisconnectCommand == null)
                    _cancelDisconnectCommand = new RelayCommand(async () =>
                    {
                        await dialogCoordinator.HideMetroDialogAsync(this, errorSyncingDialog);
                        errorSyncingDialog = null;
                    });
                return _cancelDisconnectCommand;
            }
        }

        ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new RelayCommand(async () =>
                    {
                        if (!(CurrentView.DataContext is ITempPublication iTempPublication))
                            return;
                        var cancellingAction = CancelDownloadToLabelConverter.GetCancellingAction(IsOnlineIcon, iTempPublication.PublicationIsNull);
                        await Locator.DownloadManager.StopDownload();
                        if (cancellingAction == CancellingAction.ExitApplication)
                            ExitCommand.Execute(null);
                    });
                return _cancelCommand;
            }
        }

        ICommand _closePublicationErrorDialogCommand;
        public ICommand ClosePublicationErrorDialogCommand
        {
            get
            {
                if (_closePublicationErrorDialogCommand == null)
                    _closePublicationErrorDialogCommand = new RelayCommand(async () =>
                    {
                        await dialogCoordinator.HideMetroDialogAsync(this, errorPublicationDialog);
                        errorPublicationDialog = null;
                    });
                return _closePublicationErrorDialogCommand;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDialogCoordinator instance)
        {
            Locator.TraceManager.TraceDebug("L'application a été démarrée.");
            dialogCoordinator = instance;
            Locator.APIManager.APIStatusChangedHandler += APIStatusChangedHandler;
        }

        void APIStatusChangedHandler(object sender, OnlineEventArgs e)
        {
            IsOnlineIcon = e.NewValue == true;

            if (e.OldValue != e.NewValue && e.NewValue == true && _selectedUser != null)
            {
                Locator.Resolve<IAPIHttpClient>().Relogon();
            }
        }

        public Task ShowSplashScreen() =>
            Task.CompletedTask;

        async Task CloseApp()
        {
            if (loginDialog != null)
                await dialogCoordinator.HideMetroDialogAsync(this, loginDialog);
            if (errorLoginDialog != null)
                await dialogCoordinator.HideMetroDialogAsync(this, errorLoginDialog);
            Locator.TraceManager.TraceDebug("L'application a été quittée.");
            System.Windows.Application.Current.MainWindow.Close();
        }

        public async Task ShowAppLogin()
        {
            // Définition de la culture par défaut à Français
            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-FR");

            try
            {
                ErrorText = null;
                Password = null;
                loginDialog = new AppLoginDialog();
                await dialogCoordinator.ShowMetroDialogAsync(this, loginDialog);
                // On charge la liste des utilisateurs pouvant se connecter
                var currentLicense = await Locator.Resolve<IAPIHttpClient>().ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");
                (User[] users, Role[] roles, Language[] languages, Team[] teams) = await Locator.GetService<IApplicationUsersService>().GetUsersAndRolesAndLanguages();
                foreach (var user in users)
                    user.IsActive = currentLicense?.UsersPool?.Contains(user.UserId) == true ? true : false;
                Languages = new BindingList<Language>(languages);
                Users = new BindingList<User>(users
                    .Where(u => u.IsActive
                        && u.Roles.Any(r => new[] { KnownRoles.Administrator, KnownRoles.Supervisor, KnownRoles.Evaluator, KnownRoles.Trainer, KnownRoles.Technician, KnownRoles.Operator }.Contains(r.RoleCode)))
                    .OrderBy(u => u.FullName)
                    .ToList());

                OfflineFile.Languages.SaveToJson(Languages);
                OfflineFile.Users.SaveToJson(Users);
            }
            catch (Exception e)
            {
                Locator.TraceManager.TraceError(e, "Impossible d'afficher la fenêtre d'authentification.");
            }
        }

        public async Task ShowAppLoginOffline(User lastUser)
        {
            try
            {
                ErrorText = null;
                Password = null;
                loginDialog = new AppLoginDialog();
                await dialogCoordinator.ShowMetroDialogAsync(this, loginDialog);

                SelectedUser = lastUser;
            }
            catch (Exception e)
            {
                Locator.TraceManager.TraceError(e, "Impossible d'afficher la fenêtre d'authentification.");
            }
        }

        public Task ShowErrorLogin(Exception ex)
        {
            Locator.TraceManager.TraceError(ex, "Error on offline login");
            errorLoginDialog = new ErrorLoginDialog();
            return dialogCoordinator.ShowMetroDialogAsync(this, errorLoginDialog);
        }

        public Task ShowErrorSyncing(Exception ex)
        {
            Locator.TraceManager.TraceError(ex, "Error on disconnection");
            errorSyncingDialog = new ErrorSyncingDialog();
            return dialogCoordinator.ShowMetroDialogAsync(this, errorSyncingDialog);
        }

        public Task ShowErrorPublicationChange(Exception ex)
        {
            Locator.TraceManager.TraceError(ex, "Error on publication change");
            errorPublicationDialog = new ErrorPublicationDialog();
            return dialogCoordinator.ShowMetroDialogAsync(this, errorPublicationDialog);
        }

        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }


        #region IDialogCoordinator

        public Task<string> ShowInputAsync(object context, string title, string message, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowInputAsync(this, title, message, settings);

        public string ShowModalInputExternal(object context, string title, string message, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowModalInputExternal(this, title, message, settings);

        public Task<LoginDialogData> ShowLoginAsync(object context, string title, string message, LoginDialogSettings settings = null) =>
            dialogCoordinator.ShowLoginAsync(this, title, message, settings);

        public LoginDialogData ShowModalLoginExternal(object context, string title, string message, LoginDialogSettings settings = null) =>
            dialogCoordinator.ShowModalLoginExternal(this, title, message, settings);

        public Task<MessageDialogResult> ShowMessageAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowMessageAsync(this, title, message, style, settings);

        public MessageDialogResult ShowModalMessageExternal(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowModalMessageExternal(this, title, message, style, settings);

        public Task<ProgressDialogController> ShowProgressAsync(object context, string title, string message, bool isCancelable = false, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowProgressAsync(this, title, message, isCancelable, settings);

        public Task ShowMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null) =>
            dialogCoordinator.ShowMetroDialogAsync(this, dialog, settings);

        public Task HideMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings settings = null) =>
            dialogCoordinator.HideMetroDialogAsync(this, dialog, settings);

        public Task<TDialog> GetCurrentDialogAsync<TDialog>(object context) where TDialog : BaseMetroDialog =>
            dialogCoordinator.GetCurrentDialogAsync<TDialog>(this);

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou définit la liste des utilisateurs suivant la formation
        /// </summary>
        List<UIUser> _selectedFormationUsers = new List<UIUser>();
        public List<UIUser> SelectedFormationUsers
        {
            get => _selectedFormationUsers;
            set
            {
                if (_selectedFormationUsers != value)
                {
                    _selectedFormationUsers = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion
    }
}