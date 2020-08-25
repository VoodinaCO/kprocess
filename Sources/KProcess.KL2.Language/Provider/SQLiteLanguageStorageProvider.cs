using KProcess.KL2.Languages.Model;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace KProcess.KL2.Languages.Provider
{
    public class SQLiteLanguageStorageProvider : ILanguageStorageProvider
    {
        #region SQLite constants

        public const string kConnectionString = "Data Source=Localization.sqlite;";

        //Dirga's Localization.sqlite connection string for test
        //public const string wConnectionString = "Data Source=" + "C:\\Users\\dirgayudha\\Documents\\KL2\\Sources" + "\\KProcess.KL2.WebAdmin\\App_Data\\" + "Localization.sqlite;";

        //For Test
        public const string wConnectionString = "Data Source=" + @"Resources\" + "Localization.sqlite;";

        const string kCreateLocalizedStringSqlQuery = "CREATE TABLE  IF NOT EXISTS LocalizedStrings (Id INTEGER PRIMARY KEY AUTOINCREMENT, Key  VARCHAR(255), Culture VARCHAR(10), Value TEXT)";

        const string kDropLocalizatedStringSqlQuery = "DROP TABLE IF EXISTS LocalizedStrings";

        const string kSelectLocalizationStringSqlQuery = "SELECT * FROM LocalizedStrings";


        #endregion

        private readonly string _connectionString;

        /// <summary>
        /// Constructor without parameter
        /// In that case, we fall back to default connection string that will use the db file in the current folder of executing assemblt
        /// </summary>
        public SQLiteLanguageStorageProvider()
        {
            _connectionString = kConnectionString;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteLanguageStorageProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
     

        /// <summary>
        /// Create the database and the table asociated if not yet exists
        /// </summary>
        public void CreateDatabase()
        {
            var connBuilder = new SqliteConnectionStringBuilder(_connectionString);
            File.Create(connBuilder.DataSource).Close();
            using (var connection = new SqliteConnection(connBuilder.ToString()))
            {
                connection.Open();
                using (var command = new SqliteCommand(kCreateLocalizedStringSqlQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public string DatabaseFile
        {
            get
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    return connection.DataSource;
                }
            }
        }

        /// <summary>
        /// Retrieve all the localized string from sql db
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, string>> LoadLocalizedStrings()
        {
            ConcurrentDictionary<string, IDictionary<string, string>> values = new ConcurrentDictionary<string, IDictionary<string, string>>();
            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqliteCommand(kSelectLocalizationStringSqlQuery, connection))
                    {
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string key = reader["Key"] as string;
                            string culture = reader["Culture"] as string;
                            string value = reader["Value"] as string;

                            var cultureDict = values.GetOrAdd(culture, new ConcurrentDictionary<string, string>()) as ConcurrentDictionary<string, string>;
                            if (!cultureDict.ContainsKey(key))
                                cultureDict.TryAdd(key, value);
                        }
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                var currentException = ex;
                do
                {
                    Console.WriteLine(currentException.Message);
                    currentException = currentException.InnerException;
                } while (currentException != null);
            }
            return values;
        }

        /// <summary>
        /// Drop current table and recreate localized string value with up-to-date information
        /// </summary>
        /// <param name="values"></param>
        public void Save(IDictionary<string, IList<LocalizedStringValue>> values)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = kDropLocalizatedStringSqlQuery;
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = kCreateLocalizedStringSqlQuery;
                        cmd.ExecuteNonQuery();
                    }

                    foreach (var value in values)
                    {
                        foreach (var localizatedString in value.Value)
                        {
                            using (var cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = "INSERT INTO LocalizedStrings(Key, Culture, Value) VALUES (@key, @culture, @value)";
                                cmd.Parameters.AddWithValue("@key", localizatedString.Key);
                                cmd.Parameters.AddWithValue("@culture", localizatedString.Culture);
                                cmd.Parameters.AddWithValue("@value", localizatedString.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    connection.Close();
                }
            }
        }
    }
}
