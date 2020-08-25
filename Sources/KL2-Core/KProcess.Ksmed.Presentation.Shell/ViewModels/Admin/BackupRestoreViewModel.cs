using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de gestion des backup/restore.
    /// </summary>
    public partial class BackupRestoreViewModel : FrameContentExtensibleViewModelBase<BackupRestoreViewModel, IBackupRestoreViewModel>, IBackupRestoreViewModel
    {
        #region Propriétés

        private ObservableCollection<SqlOutput> _sqlMessages;
        /// <summary>
        /// Obtient les messages à afficher dans la console
        /// </summary>
        public ObservableCollection<SqlOutput> SqlMessages
        {
            get { return _sqlMessages; }
            private set
            {
                if (_sqlMessages != value)
                {
                    _sqlMessages = value;
                    OnPropertyChanged("SqlMessages");
                }
            }
        }

        private bool _sharedDbModeEnabled = true; // Par défaut, on considère le mode le plus restrictif
        /// <summary>
        /// Determine si la connection Sql est en mode Shared Db
        /// </summary>
        public bool SharedDbModeEnabled
        {
            get { return _sharedDbModeEnabled; }
            private set
            {
                if (_sharedDbModeEnabled != value)
                {
                    _sharedDbModeEnabled = value;
                    OnPropertyChanged("SharedDbModeEnabled");
                }
            }
        }
        #endregion

        #region Surcharges
        protected override async void Initialize()
        {
            base.Initialize();

            SqlMessages = new ObservableCollection<SqlOutput>();
            try
            {
                bool isLocked = await IoC.Resolve<IServiceBus>().Get<ISharedDatabasePresentationService>().IsLocked(Security.SecurityContext.CurrentUser.Username);
                await App.Current.Dispatcher.BeginInvoke(new Action(() => SharedDbModeEnabled = isLocked));
            }
            catch { }
        }

        protected override Task OnInitializeDesigner()
        {
            SqlMessages = new ObservableCollection<SqlOutput>
            {
                new SqlOutput {Message = "A sample Sql message" },
                new SqlOutput {Message = "Another sample server Sql message" },
                new SqlOutput {Message = "Another sample info Sql message", Kind = SqlOutputKind.Info },
                new SqlOutput {Message = "Another sample success Sql message", Kind = SqlOutputKind.Success },
                new SqlOutput {Message = "Another sample error Sql message", Kind = SqlOutputKind.Error },
                new SqlOutput {Message = "Another sample exception Sql message", Kind = SqlOutputKind.Exception }
            };
            return Task.CompletedTask;
        }
        #endregion

        #region Commands

        private Command _backupCommand;
        /// <summary>
        /// Obtient la commande permettant de créer un backup de la base de données.
        /// </summary>
        public ICommand BackupCommand
        {
            get
            {
                if (_backupCommand == null)
                    _backupCommand = new Command(async () =>
                    {
                        string[] result = DialogFactory.GetDialogView<IOpenFileDialog>()
                            .Show(LocalizationManager.GetString("View_BackupRestore_SelectABackup"),
                            filter: FileExtensionsDialogHelper.GetSqlServerBackupFileDialogFilter(),
                            checkFileExists: false);

                        if (result?.Any() == true)
                        {
                            ShowSpinner();

                            SqlMessages.Clear();
                            SqlMessages.Add(new SqlOutput("Starting backup operation..."));

                            var destination = result.First();
                            if (!destination.EndsWith("3"))
                                destination += "3";
                            string source = await DoBackup();
                            SqlMessages.Add(new SqlOutput("Finalizing..."));
                            if (TryFinalizeBackup(source, destination))
                                DialogFactory.GetDialogView<IMessageDialog>().Show(LocalizationManager.GetString("VM_BackupRestore_BackupSuccessMessage"), LocalizationManager.GetString("VM_BackupRestore_BackupSuccessMessageTitle"), MessageDialogButton.OK, MessageDialogImage.Information);
                            else
                                DialogFactory.GetDialogView<IMessageDialog>().Show(LocalizationManager.GetString("VM_BackupRestore_BackupFailureMessage"), LocalizationManager.GetString("VM_BackupRestore_BackupFailureMessageTitle"), MessageDialogButton.OK, MessageDialogImage.Error);
                            HideSpinner();
                        }

                    }, () => !SharedDbModeEnabled);

                return _backupCommand;
            }
        }

        private Command _restoreCommand;
        /// <summary>
        /// Obtient la commande permettant de restaurer un backup de la base de données.
        /// </summary>
        public ICommand RestoreCommand
        {
            get
            {
                if (_restoreCommand == null)
                    _restoreCommand = new Command(async () =>
                    {
                        string[] result = DialogFactory.GetDialogView<IOpenFileDialog>()
                            .Show(LocalizationManager.GetString("View_BackupRestore_SelectABackup"),
                            filter: FileExtensionsDialogHelper.GetSqlServerBackupFileDialogFilter(),
                            checkFileExists: true);

                        if (result?.Any() == true
                            && DialogFactory.GetDialogView<IMessageDialog>()
                            .Show(LocalizationManager.GetString("View_BackupRestore_ConfirmRestore"),
                            LocalizationManager.GetString("View_BackupRestore_ConfirmRestoreTitle"),
                            MessageDialogButton.YesNoCancel,
                            MessageDialogImage.Question) == MessageDialogResult.Yes)
                        {
                            ShowSpinner();
                            SqlMessages.Clear();
                            SqlMessages.Add(new SqlOutput("Starting restore operation..."));
                            this.TraceDebug("Début de l'opération de restauration.");

                            try
                            {
                                string backupFilePath = await BackupBeforeRestoreAsync();

                                this.TraceInfo($"Le backup avant restauration est contenu dans le fichier: {backupFilePath}");

                                if (TryPrepareRestore(result.First(), out string sourceFile))
                                {
                                    this.TraceDebug("Le backup avant l'operation de restauration s'est déroulée correctement");
                                    int version = 2;
                                    if (result.First().EndsWith("3"))
                                        version = 3;
                                    await DoRestore(sourceFile, version);
                                }
                                else
                                    this.TraceDebug("La restauration n'a pas eu lieu.");

                                HideSpinner();
                            }
                            catch (Exception e)
                            {
                                this.TraceError("Une erreur s'est produite lors de la creation du backup avant restauration", e);
                                SqlMessages.Add(new SqlOutput(e.InnerException ?? e));
                                SqlMessages.Add(new SqlOutput("The preliminary backup has failed, restore operation is aborted", SqlOutputKind.Error));
                            }
                        }
                        else
                        {
                            this.TraceDebug("La restauration a été abandonée par l'utilisateur.");
                            SqlMessages.Add(new SqlOutput("Restore process aborted by user."));
                        }

                    }, () => !SharedDbModeEnabled);

                return _restoreCommand;
            }
        }

        #endregion

        #region Backup

        private bool TryFinalizeBackup(string source, string destination)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(destination))
            {
                this.TraceDebug("La finalisation du backup est annulée");
                SqlMessages.Add(new SqlOutput("Backup finalization is skipped"));
                return false;
            }

            this.TraceDebug("Finalisation du backup");
            SqlMessages.Add(new SqlOutput("Copying buffered file in destination folder..."));

            try
            {
                this.TraceDebug("Copie des fichiers {0}, -> {1}".Format(new[] { source, destination }));
                File.Copy(source, destination, true);
                File.Delete(source);
                SqlMessages.Add(new SqlOutput("Backup file created successfuly:", SqlOutputKind.Success));
                SqlMessages.Add(new SqlOutput(destination, SqlOutputKind.Path));
            }
            catch (UnauthorizedAccessException e)
            {
                this.TraceError(e, "Interception de l'erreur de droits sur fichier");
                SqlMessages.Add(new SqlOutput(e));
                SqlMessages.Add(new SqlOutput("Creation of the backup is not allowed in this folder. Check folder security:", SqlOutputKind.Error));
                SqlMessages.Add(new SqlOutput(destination, SqlOutputKind.Path));

                if (File.Exists(source))
                {
                    try
                    {
                        File.Delete(source);
                    }
                    catch (IOException eDelete)
                    {
                        this.TraceError(e, "Erreur lors de la suppression du fichier dans le buffer SQL");
                        SqlMessages.Add(new SqlOutput(eDelete));
                        SqlMessages.Add(new SqlOutput("An error occured while trying to clean up the SQL buffer folder", SqlOutputKind.Error));
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                this.TraceError(e, "Interception d'une erreur générique");
                SqlMessages.Add(new SqlOutput(e));
                SqlMessages.Add(new SqlOutput("A not handled error occured during the backup process.", SqlOutputKind.Error));

                try
                {
                    if (File.Exists(source))
                        File.Delete(source);
                }
                catch (IOException eDelete)
                {
                    this.TraceError(e, "Erreur lors de la suppression du fichier dans le buffer SQL");
                    SqlMessages.Add(new SqlOutput(eDelete));
                    SqlMessages.Add(new SqlOutput("An error occured while trying to clean up the SQL buffer folder", SqlOutputKind.Error));
                }

                return false;
            }

            return true;
        }

        private async Task<string> DoBackup()
        {
            SqlExecutionResult<string> sqlResult = ServiceBus.Get<IDataBaseService>().Backup(true);
            sqlResult.SqlMessageSent += SqlResultSqlMessageSent;
            try
            {
                string source = await sqlResult.Start();
                sqlResult.SqlMessageSent -= SqlResultSqlMessageSent;
                return source;
            }
            catch (Exception e)
            {
                this.TraceError(e, "Une erreur s'est produite durant le processus de backup de base de donnée");
                SqlMessages.Add(new SqlOutput(e.InnerException ?? e));
                SqlMessages.Add(new SqlOutput("Backup failed and rolled back.", SqlOutputKind.Error));

                return null;
            }
        }

        #endregion

        #region Restore

        private async Task<string> BackupBeforeRestoreAsync()
        {
            this.TraceDebug("nettoyage du dossier SqlDir...");
            SqlMessages.Add(new SqlOutput("Cleaning up the buffer directory..."));

            foreach (string file in Directory.GetFiles(ServiceBus.Get<IDataBaseService>().GeBackupDir()))
            {
                try
                {
                    this.TraceDebug(file);
                    SqlMessages.Add(new SqlOutput(file));
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    this.TraceError(e, file);
                    SqlMessages.Add(new SqlOutput(e));
                    SqlMessages.Add(new SqlOutput("This error has no consequence for next restore steps. Continuing..."));
                }
            }

            this.TraceDebug("Fin du nettoyage");
            string backupFilePath = await DoBackup();
            SqlMessages.Add(new SqlOutput("The backup before restore process has succeded. Backup file is store at location:"));
            SqlMessages.Add(new SqlOutput(backupFilePath, SqlOutputKind.Path));

            return backupFilePath;
        }

        private bool TryPrepareRestore(string source, out string sqlDirSourceFile)
        {
            this.TraceDebug("Préparation de la restauration");
            IDataBaseService dataBaseService = ServiceBus.Get<IDataBaseService>();

            string finalSourcePath = Path.Combine(dataBaseService.GeBackupDir(), "tempRestorationFile");

            try
            {
                if (File.Exists(source))
                {
                    SqlMessages.Add(new SqlOutput("Importing backup file..."));
                    File.Copy(source, finalSourcePath, true);
                }
                else
                    SqlMessages.Add(new SqlOutput("Restore process has failed: The specified file has not been found", SqlOutputKind.Error));
            }
            catch (UnauthorizedAccessException e)
            {
                this.TraceError(e, "Une erreur s'est produite durant le processus de restauration de base de donnée (import du fichier de backup)");
                SqlMessages.Add(new SqlOutput(e));
                SqlMessages.Add(new SqlOutput("Backup file copy has failed. Check files authorization.", SqlOutputKind.Error));
                SqlMessages.Add(new SqlOutput("Restore process is rolled backed and aborted now."));

                sqlDirSourceFile = null;
                return false;
            }
            catch (Exception e)
            {
                this.TraceError(e, "Une erreur non gérée s'est produite durant le processus de restauration de base de donnée (import du fichier de backup)");
                SqlMessages.Add(new SqlOutput(e));
                SqlMessages.Add(new SqlOutput("A not handled error occured during the importation.", SqlOutputKind.Error));
                SqlMessages.Add(new SqlOutput("Restore process is aborted now."));

                if (File.Exists(finalSourcePath))
                    File.Delete(finalSourcePath);

                sqlDirSourceFile = null;
                return false;
            }

            sqlDirSourceFile = finalSourcePath;
            return true;
        }

        private async Task DoRestore(string source, int version = 3)
        {
            SqlExecutionResult<string> sqlResult = ServiceBus.Get<IDataBaseService>().Restore(source, version, true);
            sqlResult.SqlMessageSent += SqlResultSqlMessageSent;
            try
            {
                await sqlResult.Start();
                sqlResult.SqlMessageSent -= SqlResultSqlMessageSent;
                this.TraceInfo("Le processus de restauration de la base de données s'est déroulé avec succés");
                SqlMessages.Add(new SqlOutput("restore process of the backup file has succeeded.", SqlOutputKind.Success));
                try
                {
                    if (File.Exists(source))
                        File.Delete(source);
                }
                catch (Exception ex)
                {
                    SqlMessages.Add(new SqlOutput("An error occured while trying to remove the imported backup file.", SqlOutputKind.Error));
                    this.TraceError("Suppression du fichier temporaire importé pour le processus de restauration", ex);
                }
                RestartApplication();
            }
            catch (Exception e)
            {
                sqlResult.SqlMessageSent -= SqlResultSqlMessageSent;
                this.TraceError(e, "Une erreur s'est produite durant le processus de restauration de base de donnée");
                SqlMessages.Add(new SqlOutput(e.InnerException ?? e));
                SqlMessages.Add(new SqlOutput("Restoration failed and rolled back.", SqlOutputKind.Error));
                try
                {
                    if (File.Exists(source))
                        File.Delete(source);
                }
                catch (Exception ex)
                {
                    SqlMessages.Add(new SqlOutput("An error occured while trying to remove the imported backup file.", SqlOutputKind.Error));
                    this.TraceError("Suppression du fichier temporaire importé pour le processus de restauration", ex);
                }
                RestartApplicationWithError();
            }
        }

        private void RestartApplicationWithError()
        {
            this.TraceDebug("Redemarage de l'application en erreur");
            SqlMessages.Add(new SqlOutput("Restarting the application..."));
            DialogFactory.GetDialogView<IMessageDialog>().Show(
                "The database restoration failed. Please contact K-process support at KL2Suite@k-process.com",
                "Restoration failed",
                MessageDialogButton.OK, MessageDialogImage.Error);
            Application.Current.Restart();
        }

        private void RestartApplication()
        {
            this.TraceDebug("Redemarage de l'application");
            SqlMessages.Add(new SqlOutput("Restarting the application..."));
            DialogFactory.GetDialogView<IMessageDialog>().Show(
                LocalizationManager.GetString("View_BackupRestore_Restart"),
                LocalizationManager.GetString("View_BackupRestore_RestartTitle"),
                MessageDialogButton.OK, MessageDialogImage.Information);
            Application.Current.Restart();
        }

        private void SqlResultSqlMessageSent(object sender, SqlInfoMessageEventArgs e)
        {
            this.TraceDebug(e.Message);
            Application.Current.Dispatcher.BeginInvoke(new Action(() => SqlMessages.Add(new SqlOutput(e))), DispatcherPriority.Normal);
        }
        #endregion
    }
}