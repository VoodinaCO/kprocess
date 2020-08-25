using KProcess.Business;
using KProcess.Ksmed.Business;
using System;
using System.IO;

namespace KProcess.KL2.Business.Impl.API
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

        public DataBaseService(ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        public string GeBackupDir() =>
            Path.Combine(Environment.CurrentDirectory, BackupAppRelativeFolderPath);

        public SqlExecutionResult<string> Backup(bool preventActionToBegin = false)
        {
            return null;
        }

        public SqlExecutionResult<string> Restore(string sourcePath, int version = 3, bool preventActionToBegin = false)
        {
            return null;
        }

        public SqlExecutionResult<Version> GetVersion()
        {
            return null;
        }

        public void Upgrade(Version to)
        {
        }

        public SqlExecutionResult<object> SetDataBaseVersion(Version version)
        {
            return null;
        }

        /// <summary>
        /// Détermine s'il la base est locale
        /// </summary>
        public bool IsLocalDb() => IsLocalDbStatic();
        public static bool IsLocalDbStatic()
        {
            return false;
        }
    }
}
