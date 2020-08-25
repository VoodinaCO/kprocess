using KProcess.KL2.ConnectionSecurity;
using KProcess.KL2.Languages.Provider;
using KProcess.KL2.Languages.Service;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ExportTranslations
{
    class Program
    {
        const string DataSourceKey = "DataSource";
        const string InitialCatalogKey = "InitialCatalog";
        const string SQLiteDB = "Localization.sqlite";
        const string kCreateLocalizedStringSqlQuery = "CREATE TABLE  IF NOT EXISTS LocalizedStrings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Key  VARCHAR(255), Culture VARCHAR(10), Value TEXT)";

        static void Main(string[] args)
        {
            Dictionary<string, Dictionary<CultureInfo, string>> translations;
            using (var sqlConn = new SqlConnection(GetSqlConnectionString()))
            {
                translations = ReadTranslationsFromDataBase(sqlConn);
            }

            CreateSQLiteDB();
            using (var connection = new SqliteConnection(GetSQLiteConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.Transaction = transaction;

                        foreach (var translation in translations)
                        {
                            foreach (var localizedTranslation in translation.Value)
                            {
                                cmd.CommandText = "INSERT INTO LocalizedStrings(Key, Culture, Value) VALUES (@key, @culture, @value)";
                                cmd.Parameters.AddWithValue("key", translation.Key);
                                cmd.Parameters.AddWithValue("culture", localizedTranslation.Key.Name);
                                cmd.Parameters.AddWithValue("value", localizedTranslation.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        transaction.Commit();
                    }
                }
            }

            File.Copy(SQLiteDB, $@"../../../KProcess.Ksmed.Presentation.Shell/Resources/{SQLiteDB}", true);

            var languageProvider = new SQLiteLanguageStorageProvider(GetSQLiteConnectionString());
            ILocalizationService localizationService = new LocalizationService(languageProvider);
            var excelContentBytes = localizationService.ExportLocalizedStringsToExcel();
            File.WriteAllBytes("Localization.xlsx", excelContentBytes);
        }

        static Dictionary<string, Dictionary<CultureInfo, string>> ReadTranslationsFromDataBase(SqlConnection sqlConn)
        {
            var result = new Dictionary<string, Dictionary<CultureInfo, string>>();
            var reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT k.[ResourceKey], v.[LanguageCode], v.[Value] FROM[dbo].[AppResourceKey] AS k INNER JOIN[dbo].[AppResourceValue] AS v ON k.[ResourceId] = v.[ResourceId]; ");
            if (reader.HasRows)
                while (reader.Read())
                {
                    var resourceKey = reader.GetString(0);
                    var culture = CultureInfo.GetCultureInfoByIetfLanguageTag(reader.GetString(1));
                    var translation = reader.GetString(2);

                    if (!result.ContainsKey(resourceKey))
                        result.Add(resourceKey, new Dictionary<CultureInfo, string>());

                    if (!result[resourceKey].ContainsKey(culture))
                        result[resourceKey].Add(culture, translation);
                }
            reader.Close();
            sqlConn.Close();

            // Check number of exported translations
            int count = result.Sum(_ => _.Value.Count);

            return result;
        }

        static void CreateSQLiteDB()
        {
            File.Create(SQLiteDB);
            using (var sqliteConn = new SqliteConnection(GetSQLiteConnectionString()))
            {
                ExecuteCommandFormat<int>(sqliteConn, kCreateLocalizedStringSqlQuery);
            }
        }

        public static string GetSqlConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings[DataSourceKey],
                InitialCatalog = ConfigurationManager.AppSettings[InitialCatalogKey],
                UserID = Const.DataBaseAdminUser,
                Password = ConnectionStringsSecurity.DecryptPassword(Const.DataBaseAdminCryptedPassword)
            };
            return connectionStringBuilder.ConnectionString;
        }

        public static string GetSQLiteConnectionString() =>
            $"Data Source={SQLiteDB};Version=3;";

        public static async Task<T> ExecuteCommandFormatAsync<T>(SqlConnection conn, string command, params object[] args)
        {
            try
            {
                T result;
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand(string.Format(command, args), conn);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)await cmd.ExecuteReaderAsync();
                }
                else if (typeof(T) == typeof(XmlReader))
                {
                    result = (T)(object)await cmd.ExecuteXmlReaderAsync();
                }
                else
                {
                    result = (T)await cmd.ExecuteScalarAsync();
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static T ExecuteCommandFormat<T>(SqlConnection conn, string command, params object[] args)
        {
            try
            {
                T result;
                conn.Open();
                SqlCommand cmd = new SqlCommand(string.Format(command, args), conn);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)cmd.ExecuteNonQuery();
                    conn.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)cmd.ExecuteReader();
                }
                else if (typeof(T) == typeof(XmlReader))
                {
                    result = (T)(object)cmd.ExecuteXmlReader();
                }
                else
                {
                    result = (T)cmd.ExecuteScalar();
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<T> ExecuteCommandFormatAsync<T>(SqliteConnection conn, string command, params object[] args)
        {
            try
            {
                T result;
                await conn.OpenAsync();
                SqliteCommand cmd = new SqliteCommand(string.Format(command, args), conn);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)await cmd.ExecuteReaderAsync();
                }
                else
                {
                    result = (T)await cmd.ExecuteScalarAsync();
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static T ExecuteCommandFormat<T>(SqliteConnection conn, string command, params object[] args)
        {
            try
            {
                T result;
                conn.Open();
                SqliteCommand cmd = new SqliteCommand(string.Format(command, args), conn);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)cmd.ExecuteNonQuery();
                    conn.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)cmd.ExecuteReader();
                }
                else
                {
                    result = (T)cmd.ExecuteScalar();
                    conn.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
