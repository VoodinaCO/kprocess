using KProcess.KL2.ConnectionSecurity;
using KProcess.KL2.Database;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace MigrationTool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private static MainWindow _instance = null;

        private bool _IsLoading = false;
        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _ProgressValue = 0;
        public double ProgressValue
        {
            get { return _ProgressValue; }
            set
            {
                if (_ProgressValue != value)
                {
                    _ProgressValue = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _ProgressText = string.Empty;
        public string ProgressText
        {
            get { return _ProgressText; }
            set
            {
                if (_ProgressText != value)
                {
                    _ProgressText = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _Source_IsLoading = false;
        public bool Source_IsLoading
        {
            get { return _Source_IsLoading; }
            set
            {
                if (_Source_IsLoading != value)
                {
                    _Source_IsLoading = value;

                    RaisePropertyChanged();
                    Source_TestInstanceConnectionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _Source_ServerName;
        public string Source_ServerName
        {
            get { return _Source_ServerName; }
            set
            {
                if (_Source_ServerName != value)
                {
                    _Source_ServerName = value;
                    RaisePropertyChanged();
                    Source_InstanceTestOK = false;
                }
            }
        }

        private string _Source_InstanceName;
        public string Source_InstanceName
        {
            get { return _Source_InstanceName; }
            set
            {
                if (_Source_InstanceName != value)
                {
                    _Source_InstanceName = value;
                    RaisePropertyChanged();
                    Source_InstanceTestOK = false;
                }
            }
        }

        private string _Source_UserName;
        public string Source_UserName
        {
            get { return _Source_UserName; }
            set
            {
                if (_Source_UserName != value)
                {
                    _Source_UserName = value;
                    RaisePropertyChanged();
                    Source_InstanceTestOK = false;
                }
            }
        }

        public SecureString Source_Password { get; private set; }
        private void Source_DatabasePasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Source_DatabasePasswordBox.SecurePassword != null && Source_DatabasePasswordBox.SecurePassword.Length > 0)
                Source_Password = Source_DatabasePasswordBox.SecurePassword.Copy();
            else
                Source_Password = null;
            Source_InstanceTestOK = false;
        }

        private bool _Source_InstanceTestOK = false;
        public bool Source_InstanceTestOK
        {
            get { return _Source_InstanceTestOK; }
            private set
            {
                _Source_InstanceTestOK = value;
                RaisePropertyChanged();
                MigrateCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _Target_IsLoading = false;
        public bool Target_IsLoading
        {
            get { return _Target_IsLoading; }
            set
            {
                if (_Target_IsLoading != value)
                {
                    _Target_IsLoading = value;

                    RaisePropertyChanged();
                    Target_TestInstanceConnectionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _Target_ServerName;
        public string Target_ServerName
        {
            get { return _Target_ServerName; }
            set
            {
                if (_Target_ServerName != value)
                {
                    _Target_ServerName = value;
                    RaisePropertyChanged();
                    Target_InstanceTestOK = false;
                }
            }
        }

        private string _Target_InstanceName;
        public string Target_InstanceName
        {
            get { return _Target_InstanceName; }
            set
            {
                if (_Target_InstanceName != value)
                {
                    _Target_InstanceName = value;
                    RaisePropertyChanged();
                    Target_InstanceTestOK = false;
                }
            }
        }

        private string _Target_UserName;
        public string Target_UserName
        {
            get { return _Target_UserName; }
            set
            {
                if (_Target_UserName != value)
                {
                    _Target_UserName = value;
                    RaisePropertyChanged();
                    Target_InstanceTestOK = false;
                }
            }
        }

        public SecureString Target_Password { get; private set; }
        private void Target_DatabasePasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Target_DatabasePasswordBox.SecurePassword != null && Target_DatabasePasswordBox.SecurePassword.Length > 0)
                Target_Password = Target_DatabasePasswordBox.SecurePassword.Copy();
            else
                Target_Password = null;
            Target_InstanceTestOK = false;
        }

        private bool _Target_InstanceTestOK = false;
        public bool Target_InstanceTestOK
        {
            get { return _Target_InstanceTestOK; }
            private set
            {
                _Target_InstanceTestOK = value;
                RaisePropertyChanged();
                MigrateCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _HasOldDb = false;
        public bool HasOldDb
        {
            get { return _HasOldDb; }
            private set
            {
                _HasOldDb = value;
                RaisePropertyChanged();
                MigrateCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _HasNewdDb = false;
        public bool HasNewDb
        {
            get { return _HasNewdDb; }
            private set
            {
                _HasNewdDb = value;
                RaisePropertyChanged();
                MigrateCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand<object> Source_TestInstanceConnectionCommand { get; } = new DelegateCommand<object>(async e =>
        {
            _instance.Source_IsLoading = true;
            await Logger.WriteLineAsync("START : Test source instance connection", true);

            _instance.Source_InstanceTestOK = false;
            _instance.HasOldDb = false;

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{_instance.Source_ServerName}\{_instance.Source_InstanceName}"
            };
            SecureString password = _instance.Source_Password.Copy();
            password.MakeReadOnly();
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString, new SqlCredential(_instance.Source_UserName, password)))
            {
                try
                {
                    await sqlConn.OpenAsync();
                    _instance.Source_InstanceTestOK = true;
                    await Logger.WriteLineAsync("DEBUG : Connexion à l'instance source réussie", true);
                }
                catch (Exception ex)
                {
                    _instance.Source_IsLoading = false;
                    await Logger.WriteLineAsync("ERROR : Erreur de connexion à l'instance source", true);
                    await Logger.WriteLineAsync(ExceptionToStringWithInner(ex), false);
                    await Logger.WriteLineAsync("END : Test source instance connection", true);
                    return;
                }
                // On teste si une ancienne base existe
                SqlCommand cmd = new SqlCommand($"IF (DB_ID(N'{Const.DataBaseName_v2}') IS NOT NULL) SELECT 1 ELSE SELECT 0", sqlConn);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        sqlConn.Close();
                        _instance.Source_IsLoading = false;
                        await Logger.WriteLineAsync("ERROR : Impossible de savoir si une ancienne base existe.", true);
                        await Logger.WriteLineAsync("END : Test source instance connection", true);
                        return;
                    }
                    else
                        _instance.HasOldDb = (int)reader[0] == 1;
                }
                await Logger.WriteLineAsync($"DEBUG : HasOldDb = {_instance.HasOldDb}", true);
                sqlConn.Close();
            }

            _instance.Source_IsLoading = false;
            await Logger.WriteLineAsync("END : Test source instance connection", true);
        }, e => _instance != null && !_instance.Source_IsLoading);

        public DelegateCommand<object> Target_TestInstanceConnectionCommand { get; } = new DelegateCommand<object>(async e =>
        {
            _instance.Target_IsLoading = true;
            await Logger.WriteLineAsync("START : Test target instance connection", true);

            _instance.Target_InstanceTestOK = false;
            _instance.HasNewDb = false;

            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{_instance.Target_ServerName}\{_instance.Target_InstanceName}"
            };
            SecureString password = _instance.Target_Password.Copy();
            password.MakeReadOnly();
            using (SqlConnection sqlConn = new SqlConnection(connectionStringBuilder.ConnectionString, new SqlCredential(_instance.Target_UserName, password)))
            {
                try
                {
                    await sqlConn.OpenAsync();
                    _instance.Target_InstanceTestOK = true;
                    await Logger.WriteLineAsync("DEBUG : Connexion à l'instance cible réussie", true);
                }
                catch (Exception ex)
                {
                    _instance.Target_IsLoading = false;
                    await Logger.WriteLineAsync("ERROR : Erreur de connexion à l'instance cible", true);
                    await Logger.WriteLineAsync(ExceptionToStringWithInner(ex), false);
                    await Logger.WriteLineAsync("END : Test target instance connection", true);
                    return;
                }
                // On teste si une nouvelle base existe
                SqlCommand cmd = new SqlCommand($"IF (DB_ID(N'{Const.DataBaseName_v3}') IS NOT NULL) SELECT 1 ELSE SELECT 0", sqlConn);
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        sqlConn.Close();
                        _instance.Target_IsLoading = false;
                        await Logger.WriteLineAsync("ERROR : Impossible de savoir si une nouvelle base existe.", true);
                        await Logger.WriteLineAsync("END : Test target instance connection", true);
                        return;
                    }
                    else
                        _instance.HasNewDb = (int)reader[0] == 1;
                }
                await Logger.WriteLineAsync($"DEBUG : HasNewDb = {_instance.HasNewDb}", true);
                sqlConn.Close();
            }

            _instance.Target_IsLoading = false;
            await Logger.WriteLineAsync("END : Test target instance connection", true);
        }, e => _instance != null && !_instance.Target_IsLoading);

        public DelegateCommand<object> MigrateCommand { get; } = new DelegateCommand<object>(async e =>
        {
            if (_instance.HasNewDb)
            {
                MessageBoxResult msgResult = MessageBox.Show("Une nouvelle base est déjà présente.\nÊtes-vous sûr de vouloir l'écraser ?", "Base existante", MessageBoxButton.YesNoCancel);
                switch (msgResult)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.No:
                    case MessageBoxResult.None:
                    case MessageBoxResult.OK:
                    default:
                        return;
                }
            }
            _instance.IsLoading = true;
            await Logger.WriteLineAsync("START : Migration", true);

            _instance.ProgressText = "Création du dossier temporaire pour la sauvegarde de la base source";
            _instance.ProgressValue = 0;
            await Logger.WriteLineAsync("Create temp folder for backup", true);
            SecurityIdentifier authUsers = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            DirectoryInfo tempDir = Directory.CreateDirectory(@"C:\Temp");
            string backupPath = tempDir.FullName;
            DirectorySecurity dirSecurity = new DirectorySecurity(backupPath, AccessControlSections.All);
            dirSecurity.AddAccessRule(new FileSystemAccessRule(authUsers, FileSystemRights.FullControl, AccessControlType.Allow));

            var Source_connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{_instance.Source_ServerName}\{_instance.Source_InstanceName}"
            };
            SecureString Source_password = _instance.Source_Password.Copy();
            Source_password.MakeReadOnly();

            var Target_connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = $@"{_instance.Target_ServerName}\{_instance.Target_InstanceName}"
            };
            SecureString Target_password = _instance.Target_Password.Copy();
            Target_password.MakeReadOnly();

            using (SqlConnection Source_sqlConn = new SqlConnection(Source_connectionStringBuilder.ConnectionString, new SqlCredential(_instance.Source_UserName, Source_password)))
            {
                using (SqlConnection Target_sqlConn = new SqlConnection(Target_connectionStringBuilder.ConnectionString, new SqlCredential(_instance.Target_UserName, Target_password)))
                {
                    try
                    {
                        SqlCommand cmd_v2;
                        SqlCommand cmd_v3;

                        // On crée la base
                        _instance.ProgressText = "Création de la base cible";
                        _instance.ProgressValue = 5;
                        await Logger.WriteLineAsync("EXECUTE InitialTables.sql on target instance", true);
                        await Task.Run(() => Scripts.Execute(Target_sqlConn, "InitialTables"));
                        await Logger.WriteLineAsync("EXECUTE StoredProcedures.sql on target database", true);
                        await Task.Run(() => Scripts.Execute(Target_sqlConn, "StoredProcedures", true));
                        await Logger.WriteLineAsync("EXECUTE InitialData.sql on target database", true);
                        await Task.Run(() => Scripts.Execute(Target_sqlConn, "InitialData", true));

                        // On fait un backup de la base de donnée de l'ancienne instance
                        _instance.ProgressText = "Sauvegarde de la base source";
                        _instance.ProgressValue = 25;
                        var backupCmdText = $@"BACKUP DATABASE [{Const.DataBaseName_v2}] TO DISK='{backupPath}\KL2.bak'";
                        await Logger.WriteLineAsync("Backup source database with command :", true);
                        await Logger.WriteLineAsync(backupCmdText);
                        cmd_v2 = new SqlCommand(backupCmdText, Source_sqlConn);
                        await Source_sqlConn.OpenAsync();
                        await cmd_v2.ExecuteNonQueryAsync();
                        Source_sqlConn.Close();

                        // On retrouve l'emplacement de la nouvelle base
                        _instance.ProgressText = "Recherche de l'emplacement de la base cible";
                        _instance.ProgressValue = 50;
                        await Logger.WriteLineAsync("Search new database path", true);
                        cmd_v3 = new SqlCommand($@"SELECT physical_name FROM sys.master_files WHERE physical_name LIKE '%\{Const.DataBaseName_v3}.mdf'", Target_sqlConn);
                        await Target_sqlConn.OpenAsync();
                        string newDbPath = null;
                        using (SqlDataReader reader = await cmd_v3.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                await Logger.WriteLineAsync("ERROR : Impossible de retrouver l'emplacement de la nouvelle base de donnée.", true);
                                _instance.IsLoading = false;
                                await Logger.WriteLineAsync("END : Migration", true);
                                return;
                            }
                            newDbPath = Directory.GetParent((string)reader[0]).FullName;
                        }
                        Target_sqlConn.Close();

                        // On récupère la version de la nouvelle base
                        _instance.ProgressText = "Recherche de la version de la base cible";
                        _instance.ProgressValue = 55;
                        await Logger.WriteLineAsync("Search new database path", true);
                        cmd_v3 = new SqlCommand($"USE [{Const.DataBaseName_v3}]", Target_sqlConn);
                        await Target_sqlConn.OpenAsync();
                        cmd_v3.ExecuteNonQuery();
                        cmd_v3 = new SqlCommand("GetDatabaseVersion", Target_sqlConn) { CommandType = System.Data.CommandType.StoredProcedure };
                        Version newDbVersion = null;
                        using (SqlDataReader reader = await cmd_v3.ExecuteReaderAsync())
                        {
                            if (!await reader.ReadAsync())
                            {
                                await Logger.WriteLineAsync("ERROR : Impossible de retrouver la version de la nouvelle base de donnée.", true);
                                _instance.IsLoading = false;
                                await Logger.WriteLineAsync("END : Migration", true);
                                return;
                            }
                            newDbVersion = Version.Parse((string)reader[0]);
                        }
                        Target_sqlConn.Close();

                        // On supprime la nouvelle base
                        _instance.ProgressText = "Suppression de la base cible";
                        _instance.ProgressValue = 60;
                        var deleteText = $@"IF EXISTS(SELECT * FROM sysdatabases WHERE name='{Const.DataBaseName_v3}') DROP DATABASE [{Const.DataBaseName_v3}]";
                        await Logger.WriteLineAsync("Delete target database with command :", true);
                        await Logger.WriteLineAsync(deleteText);
                        cmd_v3 = new SqlCommand(deleteText, Target_sqlConn);
                        await Target_sqlConn.OpenAsync();
                        await cmd_v3.ExecuteNonQueryAsync();
                        Target_sqlConn.Close();

                        // On restaure le backup dans la nouvelle instance
                        _instance.ProgressText = "Restauration de la base source à l'emplacement de la base cible";
                        _instance.ProgressValue = 65;
                        var restoreCmdText = $@"RESTORE DATABASE [{Const.DataBaseName_v3}] FROM DISK=N'{backupPath}\KL2.bak' WITH MOVE '{Const.DataBaseName_v2}' TO '{newDbPath}\{Const.DataBaseName_v3}.mdf', MOVE '{Const.DataBaseName_v2}_log' TO '{newDbPath}\{Const.DataBaseName_v3}.LDF'";
                        await Logger.WriteLineAsync("Restore database with command :", true);
                        await Logger.WriteLineAsync(restoreCmdText);
                        cmd_v3 = new SqlCommand(restoreCmdText, Target_sqlConn);
                        await Target_sqlConn.OpenAsync();
                        await cmd_v3.ExecuteNonQueryAsync();
                        Target_sqlConn.Close();

                        // On effectue l'upgrade jusqu'à la version de l'installeur
                        _instance.ProgressText = "Update de la base cible";
                        _instance.ProgressValue = 85;
                        await Logger.WriteLineAsync($"Update database to {newDbVersion}", true);
                        Scripts.UpgradeTo(Target_sqlConn, newDbVersion);

                        // On supprime le fichier de backup
                        _instance.ProgressText = "Suppression du fichier de sauvegarde de la base source";
                        _instance.ProgressValue = 90;
                        await Logger.WriteLineAsync($@"Delete backup file {backupPath}\KL2.bak", true);
                        File.Delete($@"{backupPath}\KL2.bak");

                        _instance.IsLoading = false;
                        _instance.ProgressText = "Migration terminée";
                        _instance.ProgressValue = 100;
                        await Logger.WriteLineAsync("END : Migration", true);
                        MessageBox.Show("La migration a réussi.\nL'outil de migration va être fermé.", "Résultat de la migration");
                        Application.Current.Shutdown();
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            // On supprime le fichier de backup
                            await Logger.WriteLineAsync($@"Delete backup file {backupPath}\KL2.bak", true);
                            File.Delete($@"{backupPath}\KL2.bak");
                        }
                        catch { }
                        _instance.IsLoading = false;
                        _instance.ProgressText = string.Empty;
                        _instance.ProgressValue = 0;
                        await Logger.WriteLineAsync("END : Migration", true);
                        MessageBox.Show($"La migration a échoué.\nException :\n{ExceptionToStringWithInner(ex)}", "Résultat de la migration");
                    }
                }
            }
        }, e => _instance != null && _instance.Source_InstanceTestOK && _instance.Target_InstanceTestOK && _instance.HasOldDb);

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;
            Source_TestInstanceConnectionCommand.RaiseCanExecuteChanged();
            Target_TestInstanceConnectionCommand.RaiseCanExecuteChanged();
            Logger.DeleteLogFile();
            Logger.WriteLine("Start Migration Tool", true);
        }

        static string ExceptionToStringWithInner(Exception e)
        {
            string result = e.Message;
            if (e.InnerException != null)
                result += $"\n{ExceptionToStringWithInner(e.InnerException)}";
            return result;
        }
    }
}
