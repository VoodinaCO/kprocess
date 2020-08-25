using KProcess.Business;
using KProcess.KL2.ConnectionSecurity;
using KProcess.KL2.Database;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Data;
using System;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion de base de données
    /// </summary>
    public class DataBaseService : IBusinessService, IDataBaseService
    {
        readonly ITraceManager _traceManager;

        public static readonly string BackupAppRelativeFolderPath = @"ExportBuffer\SQL";
        public static readonly string PatchAppRelativeFolderPath = @"DbMigrations";
        public static readonly string PatchFileVersionExtractionRegex = @"(?<=Patch.*?)\d[\d\.]+(?=\.sql)";

        private const string KLVersionExtendedPropertyKey = "KL_Version";

        private string _DataBaseName;

        #region Public methods

        public DataBaseService(
            ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        public string GeBackupDir() =>
            Path.Combine(Environment.CurrentDirectory, BackupAppRelativeFolderPath);

        public SqlExecutionResult<string> Backup(bool preventActionToBegin = false)
        {
            this.TraceDebug("Création d'un backup de base de données...");

            CreateSqlDirForDebugPurpose();
            var sqlConnection = CreateAdminSqlConnection();
            var destinationPath = Path.Combine(GeBackupDir(), string.Format("DbBackup_{0}.bak3", DateTime.Now.Ticks));

            var task = new Task<string>(() =>
            {
                ExecuteBackup(sqlConnection, destinationPath);
                return destinationPath;
            });

            if (!preventActionToBegin)
            {
                task.Start();
            }

            return SqlExecutionResult.New(sqlConnection, task);
        }

        public SqlExecutionResult<string> Restore(string sourcePath, int version = 3, bool preventActionToBegin = false)
        {
            this.TraceDebug("Restauration d'un backup de base de données...");

            CreateSqlDirForDebugPurpose();

            SqlConnection sqlConn = CreateAdminSqlConnection(true);
            SqlCommand sqlCmd;

            var task = new Task<string>(() =>
            {
                try
                {
                    // On retrouve l'emplacement de la nouvelle base
                    SqlDataReader reader;
                    sqlCmd = new SqlCommand($@"SELECT physical_name FROM sys.master_files WHERE physical_name LIKE '%\{Const.DataBaseName_v3}.mdf'", sqlConn);
                    sqlConn.Open();
                    reader = sqlCmd.ExecuteReader();
                    if (!reader.Read())
                        throw new Exception("Impossible de retrouver l'emplacement de la nouvelle base de donnée.");
                    string newDbPath = Directory.GetParent((string)reader[0]).FullName;
                    sqlConn.Close();

                    // On récupère la version de la nouvelle base
                    sqlCmd = new SqlCommand($"USE [{Const.DataBaseName_v3}]", sqlConn);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlCmd = new SqlCommand("GetDatabaseVersion", sqlConn) { CommandType = System.Data.CommandType.StoredProcedure };
                    reader = sqlCmd.ExecuteReader();
                    if (!reader.Read())
                        throw new Exception("Impossible de retrouver la version de la nouvelle base de donnée.");
                    Version newDbVersion = Version.Parse((string)reader[0]);
                    sqlConn.Close();

                    //On coupe toutes les connexions à la base en la passant en single user
                    sqlCmd = new SqlCommand($"ALTER DATABASE [{Const.DataBaseName_v3}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", sqlConn);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();

                    // On supprime la nouvelle base
                    var deleteText = $@"IF EXISTS(SELECT * FROM sysdatabases WHERE name='{Const.DataBaseName_v3}') DROP DATABASE [{Const.DataBaseName_v3}]";
                    sqlCmd = new SqlCommand(deleteText, sqlConn);
                    this.TraceDebug("Suppression de la base");
                    this.TraceDebug(deleteText);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();

                    // On restaure le backup dans la nouvelle instance
                    string restoreCmdText;
                    // On récupère les noms logiques du backup
                    restoreCmdText = $@"RESTORE FILELISTONLY FROM DISK=N'{sourcePath}'";
                    sqlCmd = new SqlCommand(restoreCmdText, sqlConn);
                    this.TraceDebug("Restauration de la base (récupération des noms logiques)");
                    this.TraceDebug(restoreCmdText);
                    sqlConn.Open();
                    var dataReader = sqlCmd.ExecuteReader();
                    string LogicalNameDb = "";
                    string LogicalNameLog = "";
                    while (dataReader.Read())
                    {
                        string logicalName = (string)dataReader["LogicalName"];
                        string type = (string)dataReader["Type"];
                        if (type == "D")
                            LogicalNameDb = logicalName;
                        else if (type == "L")
                            LogicalNameLog = logicalName;
                    }
                    sqlConn.Close();
                    this.TraceDebug($"Db : {LogicalNameDb}, Log : {LogicalNameLog}");
                    restoreCmdText = $@"RESTORE DATABASE [{Const.DataBaseName_v3}] FROM DISK=N'{sourcePath}' WITH MOVE '{LogicalNameDb}' TO '{newDbPath}\{Const.DataBaseName_v3}.mdf', MOVE '{LogicalNameLog}' TO '{newDbPath}\{Const.DataBaseName_v3}.LDF'";
                    sqlCmd = new SqlCommand(restoreCmdText, sqlConn);
                    this.TraceDebug("Restauration de la base");
                    this.TraceDebug(restoreCmdText);
                    sqlConn.Open();
                    sqlCmd.ExecuteNonQuery();
                    sqlConn.Close();

                    // On effectue l'upgrade jusqu'à la version de l'installeur
                    this.TraceDebug($"Update de la base vers {newDbVersion}");
                    Scripts.GoTo(sqlConn, newDbVersion);

                    // On restore les droits utilisateurs sur la base
                    this.TraceDebug($"Restauration des droits utilisateurs");
                    Scripts.Execute(sqlConn, "RestoreUserRights");
                }
                catch (Exception e)
                {
                    this.TraceError("Erreur pendant l'execution du script sql de restauration", e);
                    throw e;
                }

                return sourcePath;
            });

            if (!preventActionToBegin)
                task.Start();

            return SqlExecutionResult.New(sqlConn, task);
        }

        public SqlExecutionResult<Version> GetVersion()
        {
            this.TraceDebug("Récupération de la version de la base de données...");
            var sqlConnection = CreateAdminSqlConnection();

            var task = new Task<Version>(() => ExecuteGetVersion(sqlConnection));
            task.Start();
            return SqlExecutionResult.New(sqlConnection, task);
        }

        public void Upgrade(Version to)
        {
            this.TraceDebug("Upgrade de la base de donnée vers la version {0}".Format(to));
            var sqlConnection = CreateAdminSqlConnection();

            try
            {
                Scripts.UpgradeTo(sqlConnection, to);
            }
            catch (Exception e)
            {
                this.TraceError(e, "Une erreur s'est produite lors de la mise à niveau de la base de données");

                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }

        public SqlExecutionResult<object> SetDataBaseVersion(Version version)
        {
            this.TraceDebug("Mise à jour de la version de la base de données");
            var sqlConnection = CreateAdminSqlConnection();
            var taskCompletionSource = new TaskCompletionSource<object>();

            Task.Factory.StartNew(() =>
            {
                try
                {
                    sqlConnection.Open();
                    ExecuteSetDataBaseVersion(sqlConnection, version);
                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Une erreur s'est produite lors de la mise à jour de la verion de la base de données");
                    taskCompletionSource.SetException(e);
                }
                finally
                {
                    sqlConnection.Close();
                }
            });

            return SqlExecutionResult.New(sqlConnection, taskCompletionSource.Task);
        }

        private static string MachineDataSource()
        {
            var connection = ConfigurationManager.ConnectionStrings["KsmedEntities"].ConnectionString;
            int startPos = connection.IndexOf("Data Source=");
            int endPos = connection.IndexOf(';', startPos);
            string dataSource = connection.Substring(startPos + 12, endPos - startPos - 12);
            var dataSourceElts = dataSource.Split('\\');
            return dataSourceElts[0];
        }

        /// <summary>
        /// Détermine s'il la base est locale
        /// </summary>
        public bool IsLocalDb() => IsLocalDbStatic();
        public static bool IsLocalDbStatic()
        {
            string machineDataSource = MachineDataSource();
            return (machineDataSource == "." || machineDataSource == "(LocalDb)");
        }

        #endregion

        #region Debug methods
        [Conditional("DEBUG")]
        private void CreateSqlDirForDebugPurpose()
        {
            var sqlDir = Path.Combine(Environment.CurrentDirectory, BackupAppRelativeFolderPath);
            if (!Directory.Exists(sqlDir))
            {
                Directory.CreateDirectory(sqlDir); // Ce repertoire est créé par l'installeur avec les droits appropriés
            }
        }

        [Conditional("DEBUG")]
        private void CreatePatchsDirForDebugPurpose()
        {
            var patchDir = Path.Combine(Environment.CurrentDirectory, PatchAppRelativeFolderPath);
            if (!Directory.Exists(patchDir))
            {
                Directory.CreateDirectory(patchDir); // Ce repertoire est créé par l'installeur avec les droits appropriés
            }
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Essait de remapper les utilisateurs sans SMO qui peut peut être échouer à cause de problèmes de compatibilité avec l'assembly qui est prévue pour le framework 2.0
        /// </summary>
        /// <remarks>
        /// Cette 2eme approche est plus sûre en terme de stabilité, mais moins désirée car sans transaction
        /// </remarks>
        /// <param name="sqlConnection">La connexion sql à utiliser pour effectuer le changement d'utilisateur</param>
        /// <returns></returns>
        private bool TryRepairUserMapping(SqlConnection sqlConnection)
        {
            this.TraceDebug("Reset du mapping des utilisateurs - mode 2nd chance avec ADO à la place de SMO");
            using (sqlConnection)
            {
                try
                {
                    sqlConnection.Open();

                    try
                    {
                        using (var cmd = new SqlCommand($@"DROP USER {Const.DataBaseUser}", sqlConnection)) cmd.ExecuteNonQuery();
                    }
                    catch { } // La suppression peut échoué si l'utilisateur n'existe pas. Ce qui devrait ne pas se produire mais en mode seconde chance on autorise ce cas.

                    using (var cmd = new SqlCommand($@"CREATE USER [{Const.DataBaseUser}] FOR LOGIN [{Const.DataBaseUser}]", sqlConnection)) cmd.ExecuteNonQuery();
                    using (var cmd = new SqlCommand($@"EXEC sp_addrolemember N'db_datareader', N'{Const.DataBaseUser}'", sqlConnection)) cmd.ExecuteNonQuery();
                    using (var cmd = new SqlCommand($@"EXEC sp_addrolemember N'db_datawriter', N'{Const.DataBaseUser}'", sqlConnection)) cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    this.TraceError(e, "Une erreur s'est produite lors du reset du mapping des utilisateurs - mode ADO");
                    return false;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

            return true;
        }

        /// <summary>
        /// Execute la commande SQL mettant à jour la version de la base de données
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <param name="version"></param>
        private void ExecuteSetDataBaseVersion(SqlConnection sqlConnection, Version version)
        {
            using (var cmd = new SqlCommand(string.Format(@"EXEC sys.sp_updateextendedproperty @name = N'{1}', @value =  '{0}';",
                version,
                KLVersionExtendedPropertyKey), sqlConnection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Requete la version pour laquelle la base de données est prévue
        /// </summary>
        /// <param name="sqlConnection"></param>
        /// <returns></returns>
        private Version ExecuteGetVersion(SqlConnection sqlConnection)
        {
            Version version = null;
            using (sqlConnection)
            {
                sqlConnection.Open();
                var query = string.Format("SELECT Value FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='{0}'", KLVersionExtendedPropertyKey);
                using (var cmd = new SqlCommand(query, sqlConnection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        var rawVersion = reader.GetString(0);
                        this.TraceDebug(string.Format("La version de la base de donnée a été identifée: {0}", rawVersion));
                        var parsedVersion = new Version(rawVersion);
                        version = new Version(
                            Math.Max(parsedVersion.Major, 0),
                            Math.Max(parsedVersion.Minor, 0),
                            Math.Max(parsedVersion.Build, 0),
                            Math.Max(parsedVersion.Revision, 0));
                    }
                    else
                    {
                        this.TraceDebug("La version de la base de donnée n'a pas été trouvée");
                    }

                    reader.Close();
                }

                sqlConnection.Close();
            }

            return version;
        }

        private void ExecuteBackup(SqlConnection sqlConnection, string destination)
        {
            using (sqlConnection)
            {
                sqlConnection.Open();
                using (var cmd = new SqlCommand(string.Format(
                    "backup database {0} to disk = {1} with description = {2}, name = {3}, STATS = 20",
                    QuoteIdentifier(_DataBaseName),
                    QuoteString(destination),
                    QuoteString($"Database name : {_DataBaseName}"),
                    QuoteString("backupName")), sqlConnection))
                {
                    cmd.ExecuteNonQuery();
                }

                sqlConnection.Close();
            }
        }

        private SqlConnection CreateAdminSqlConnection(bool useMaster = false)
        {
            EntityConnectionStringBuilder efConnectionStringBuilder = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings[KsmedEntities.ContainerName].ConnectionString);
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(efConnectionStringBuilder.ProviderConnectionString)
            {
                UserID = Const.DataBaseAdminUser,
                Password = ConnectionSecurity.ConnectionStringsSecurity.DecryptPassword(Const.DataBaseAdminCryptedPassword)
            };

            if (useMaster)
            {
                _DataBaseName = connectionStringBuilder.InitialCatalog;
                connectionStringBuilder.InitialCatalog = "master";
            }
            else
            {
                _DataBaseName = connectionStringBuilder.InitialCatalog;
            }

            var connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            return connection;
        }

        private static string QuoteIdentifier(string name) =>
            "[" + name.Replace("]", "]]") + "]";

        private static string QuoteString(string text) =>
            "'" + text.Replace("'", "''") + "'";
        #endregion
    }
}
