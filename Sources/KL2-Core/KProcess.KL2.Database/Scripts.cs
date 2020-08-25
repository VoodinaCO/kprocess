using KProcess.KL2.ConnectionSecurity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KProcess.KL2.Database
{
    public static class Scripts
    {
        public static void Execute(SqlConnection conn, string scriptName, bool useDatabase = false, string databaseName = Const.DataBaseName_v3, CultureInfo cultureInfo = null)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"KProcess.KL2.Database.Scripts.{scriptName}.sql");
                string script;
                using (StreamReader reader = new StreamReader(stream))
                    script = reader.ReadToEnd();

                conn.Open();
                SqlCommand cmd = new SqlCommand($"USE [{databaseName}];", conn);
                if (useDatabase)
                    cmd.ExecuteNonQuery();
                foreach (var query in script.Split(new[] { "\nGO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        cmd.CommandText = query;
                        int result = cmd.ExecuteNonQuery();
                    }
                    catch (Exception inner)
                    {
                        throw new Exception($"SQL ERROR - {inner.Message}\n{query}", inner);
                    }
                }
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            var scripts = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            if (cultureInfo != null && scripts.Contains($"KProcess.KL2.Database.Scripts.{scriptName}_{cultureInfo.ToString()}.sql"))
                Execute(conn, $"{scriptName}_{cultureInfo.ToString()}", useDatabase);
        }

        public static void UpgradeTo(SqlConnection conn, Version targetVersion, string databaseName = Const.DataBaseName_v3)
        {
            Version currentDatabaseVersion = InitializeOperation(conn, targetVersion, databaseName);
            // On teste si la version voulue est bien supérieure à celle actuelle
            if (currentDatabaseVersion == null)
                throw new ArgumentNullException("Impossible de récupérer la version actuelle de la base.");
            if (currentDatabaseVersion >= targetVersion)
                throw new ArgumentException("Impossible d'upgrader car la version actuelle est plus récente que la version voulue.");
            // On applique les patches Up jusqu'à la version voulue inclue
            var scripts = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var versionsList = Versions.List.Keys.ToList();
            int indexCurrent = versionsList.FindIndex(_ => _ == currentDatabaseVersion);
            Version upVersion = versionsList[++indexCurrent];
            while (upVersion <= targetVersion)
            {
                if (scripts.Contains($"KProcess.KL2.Database.Scripts.{upVersion}_Up.sql"))
                    Execute(conn, $"{upVersion}_Up", true, databaseName);
                conn.Open();
                SqlCommand SetVersion = new SqlCommand($"USE [{databaseName}]", conn);
                SetVersion.ExecuteNonQuery();
                SetVersion = new SqlCommand("InsertOrUpdateDatabaseVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                SetVersion.Parameters.Add(new SqlParameter("@version", upVersion.ToString()));
                SetVersion.ExecuteNonQuery();
                conn.Close();
                if (upVersion == targetVersion)
                    break;
                upVersion = versionsList[++indexCurrent];
            }
        }

        public static void DowngradeTo(SqlConnection conn, Version targetVersion, string databaseName = Const.DataBaseName_v3)
        {
            Version currentDatabaseVersion = InitializeOperation(conn, targetVersion, databaseName);
            // On teste si la version voulue est bien inférieure à celle actuelle
            if (currentDatabaseVersion == null)
                throw new ArgumentNullException("Impossible de récupérer la version actuelle de la base.");
            if (currentDatabaseVersion >= targetVersion)
                throw new ArgumentException("Impossible de downgrader car la version actuelle est plus ancienne que la version voulue.");
            // On applique les patches Down jusqu'à la version voulue exclue
            var scripts = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            var versionsList = Versions.List.Keys.ToList();
            int indexCurrent = versionsList.FindIndex(_ => _ == currentDatabaseVersion);
            Version downVersion = versionsList[indexCurrent];
            while (downVersion > targetVersion)
            {
                if (scripts.Contains($"KProcess.KL2.Database.Scripts.{downVersion}_Down.sql"))
                    Execute(conn, $"{downVersion}_Down", true, databaseName);
                downVersion = versionsList[--indexCurrent];
                conn.Open();
                SqlCommand SetVersion = new SqlCommand($"USE [{databaseName}]", conn);
                SetVersion.ExecuteNonQuery();
                SetVersion = new SqlCommand("InsertOrUpdateDatabaseVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                SetVersion.Parameters.Add(new SqlParameter("@version", downVersion.ToString()));
                SetVersion.ExecuteNonQuery();
                conn.Close();
            }
        }

        public static void GoTo(SqlConnection conn, Version targetVersion, string databaseName = Const.DataBaseName_v3)
        {
            Version currentDatabaseVersion = InitializeOperation(conn, targetVersion, databaseName);
            if (currentDatabaseVersion == null)
                throw new ArgumentNullException("Impossible de récupérer la version actuelle de la base.");
            if (currentDatabaseVersion < targetVersion)
                UpgradeTo(conn, targetVersion, databaseName);
            else if (currentDatabaseVersion > targetVersion)
                DowngradeTo(conn, targetVersion, databaseName);
        }

        static Version InitializeOperation(SqlConnection conn, Version targetVersion, string databaseName = Const.DataBaseName_v3)
        {
            // On teste si la version voulue existe
            if (!Versions.List.Keys.Any(_ => _ == targetVersion))
                throw new KeyNotFoundException("La version voulue n'existe pas.");
            // On insère les procédures stockées
            Execute(conn, "StoredProcedures", true, databaseName);
            // On récupère la version de la base actuelle
            conn.Open();
            SqlCommand GetDatabaseVersion = new SqlCommand($"USE [{databaseName}]", conn);
            GetDatabaseVersion.ExecuteNonQuery();
            GetDatabaseVersion = new SqlCommand("GetDatabaseVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure };
            SqlDataReader reader = GetDatabaseVersion.ExecuteReader();
            Version currentDatabaseVersion = null;
            if (reader.Read())
                currentDatabaseVersion = Version.Parse((string)reader[0]);
            conn.Close();
            return currentDatabaseVersion;
        }

        public static void Move(SqlConnection conn, Version targetVersion)
        {
            try
            {
                // On ne crée que la base, pas les tables (si une base existe déjà, elle est supprimée)
                Execute(conn, "InitialTablesRemote");
                // On copie les données de l'ancienne base vers la nouvelle
                Execute(conn, "MoveTablesRemote");
                // On crée les procédures stockées dans la nouvelle base
                Execute(conn, "StoredProcedures", true);
                // On récupère la version de l'ancienne base
                conn.Open();
                SqlCommand cmd = new SqlCommand($"USE [{Const.DataBaseName_v2}]", conn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("SELECT CONVERT(nvarchar(30),[Value]) AS 'Version' FROM SYS.EXTENDED_PROPERTIES WHERE class_desc='DATABASE' AND name='KL_Version'", conn);
                Version oldDbVersion = null;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        throw new Exception("Impossible de récupérer la version de l'ancienne base.");
                    oldDbVersion = Version.Parse((string)reader[0]);
                }
                // On définit la version dans la nouvelle base
                cmd = new SqlCommand($"USE [{Const.DataBaseName_v3}]", conn);
                cmd.ExecuteNonQuery();
                cmd = new SqlCommand("InsertOrUpdateDatabaseVersion", conn) { CommandType = System.Data.CommandType.StoredProcedure };
                cmd.Parameters.Add(new SqlParameter("@version", oldDbVersion.ToString()));
                cmd.ExecuteNonQuery();
                conn.Close();
                // On procède à l'upgrade de la nouvelle base vers la version désirée
                UpgradeTo(conn, targetVersion);
            }
            catch (Exception e)
            {
                try { conn.Close(); } catch { }
                throw new Exception("Une erreur s'est produite lors de la migration des données.", e);
            }
        }
    }
}
