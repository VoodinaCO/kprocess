using KProcess.Ksmed.Data;
using KProcess.Ksmed.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Business.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business.Tests;
using KProcess.Ksmed.Tests.Helper;
using KProcess.KL2.APIClient;

namespace KProcess.Ksmed.Business.Tests
{
    public static class SampleData
    {
        public static void ClearDatabase()
        {
            LegacyMode.UseLegacy();
            Initialization.SetCurrentUser();
            ExecuteScript(ReadStream("KProcess.Ksmed.Business.Tests.Resources.SQL.CleanDatabase.sql"), false);
        }

        public static void ClearDatabaseThenImportDefaultProject()
        {
            ClearDatabaseThenImportProject(GetDefaultProject());
        }

        public static Stream GetDefaultProject()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.Ksmed.Business.Tests.Resources.ProjBase.ksp");
        }

        public static void ImportProject(Stream kspStream)
        {
            Exception e = null;
            var mre = new System.Threading.ManualResetEvent(false);

            byte[] importData;
            using (var memoryStream = new MemoryStream()) { kspStream.CopyTo(memoryStream, StreamExtensions.BufferSize); importData = memoryStream.ToArray(); }

            // Ouvrir le fichier
            Project project = null;
            new ImportExportService().PredictMergedReferentialsProject(importData, pi =>
            {
                new ImportExportService().ImportProject(pi, false, @"D:\Videos\Sample Videos", p =>
                {
                    project = p;
                    mre.Set();
                }, ex =>
                {
                    e = ex;
                    mre.Set();
                });
            }, ex =>
            {
                e = ex;
                mre.Set();
            });


            mre.WaitOne();
            AssertExt.IsExceptionNull(e);
            Assert.IsNotNull(project);
        }

        public static void ClearDatabaseThenImportProject(Stream kspStream)
        {
            LegacyMode.UseLegacy();
            ClearDatabase();

            ImportProject(kspStream);

            ExecuteScript(ReadStream("KProcess.Ksmed.Business.Tests.Resources.SQL.Users.sql"), false);
        }

        public static void ImportDatabaseBackup(string backupPath)
        {
            LegacyMode.UseLegacy();

            var backupDir = ExecuteScalar<string>(ReadStream("KProcess.Ksmed.Business.Tests.Resources.SQL.GetBackupPath.sql"), true);
            var destBackupPath = Path.Combine(backupDir, "KProcess.Ksmed.bak");
            File.Copy(backupPath, destBackupPath, true);

            ExecuteScript(ReadStream("KProcess.Ksmed.Business.Tests.Resources.SQL.RestoreDatabase.sql"), true);
        }

        private static string ReadStream(string fileName)
        {
            var sqlStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
            string sql;
            using (var reader = new StreamReader(sqlStream))
            {
                sql = reader.ReadToEnd();
            }
            return sql;
        }

        private static void ExecuteScript(string sql, bool admin)
        {
            using (var connection = GetConnection(admin))
            {
                Server server = new Server(new ServerConnection(connection));
                server.ConnectionContext.ExecuteNonQuery(sql);
            }
        }

        private static T ExecuteScalar<T>(string sql, bool admin)
        {
            using (var connection = GetConnection(admin))
            {
                Server server = new Server(new ServerConnection(connection));
                return (T)server.ConnectionContext.ExecuteScalar(sql);
            }
        }

        private static SqlConnection GetConnection(bool admin)
        {
            var connectionString = ConnectionStringsSecurity.GetConnectionString(admin ? "name=KsmedEntitiesAdmin" : KsmedEntities.ConnectionString);
            return new SqlConnection(connectionString.StoreConnection.ConnectionString);
        }


        public static int GetProjectId()
        {
            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                return ctx.Projects.First().ProjectId;
            }
        }

    }
}
