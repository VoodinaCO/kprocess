using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;

namespace InstallDb
{
    class Program
    {

        const string instanceName = "KL2";
        const string logFile = "log.txt";

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF7;
            Console.OutputEncoding = Encoding.UTF7;

            try
            {
                if (File.Exists(logFile))
                    File.Delete(logFile);
            }
            catch { }

            using (var log = File.OpenWrite(logFile))
            using (var writer = new StreamWriter(log))
            {
                Log.WriteLine(writer, "The app has been started.", false);

                try
                {
                    // Récupération des paramètres
                    Log.WriteLine(writer, "Getting parameters");
                    string DataSource = ConfigurationManager.AppSettings["DataSource"];
                    Log.WriteLine(writer, $"DataSource : {DataSource}");
                    string DataBase = ConfigurationManager.AppSettings["DataBase"];
                    //Log.WriteLine(writer, $"DataBase : {DataBase}");
                    bool UseWindowsAuthentication = bool.Parse(ConfigurationManager.AppSettings["UseWindowsAuthentication"]);
                    Log.WriteLine(writer, $"UseWindowsAuthentication : {UseWindowsAuthentication}");
                    string UserId = ConfigurationManager.AppSettings["UserId"];
                    if (!UseWindowsAuthentication)
                        Log.WriteLine(writer, $"UserId : {UserId}");
                    string Password = ConfigurationManager.AppSettings["Password"];
                    if (!UseWindowsAuthentication)
                        Log.WriteLine(writer, $"Password : {Password}");
                    SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder()
                    {
                        DataSource = DataSource,
                        InitialCatalog = DataBase,
                        IntegratedSecurity = UseWindowsAuthentication
                    };
                    if (!UseWindowsAuthentication)
                    {
                        connBuilder.UserID = UserId;
                        connBuilder.Password = Password;
                    }

                    // Tentative de connexion
                    Log.WriteLine(writer, "Trying to connect to the instance");
                    using (var conn = new SqlConnection(connBuilder.ToString()))
                    {
                        conn.Open();
                    }

                    // Execution des scripts
                    using (var conn = new SqlConnection(connBuilder.ToString()))
                    {
                        Log.WriteLine(writer, "Creating database, tables and users");
                        ExecuteScript(conn, "InitialTables");
                        Log.WriteLine(writer, "Creating stocked procedures");
                        ExecuteScript(conn, "StoredProcedures");
                        Log.WriteLine(writer, "Inserting data");
                        ExecuteScript(conn, "InitialData");
                        Log.WriteLine(writer, "Inserting localized data");
                        ExecuteScript(conn, "InitialData_fr-FR");
                    }

                    Log.WriteLine(writer, "Successfully install database.");
                    Log.WriteLine(writer, "Press a key to exit...", false);
                    Console.ReadKey();
                }
                catch (Exception e)
                {
                    Log.WriteLine(writer, $"Exception : {e.Message}");
                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        Log.WriteLine(writer, $"Inner Exception : {inner.Message}");
                        inner = inner.InnerException;
                    }
                    Log.WriteLine(writer, "Press a key to exit...", false);
                    Console.ReadKey();
                }

                Log.WriteLine(writer, "The app has been exited.", false);
            }
        }

        static void ExecuteScript(SqlConnection conn, string scriptName)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"InstallDb.Scripts.{scriptName}.sql");
                string script;
                using (StreamReader reader = new StreamReader(stream))
                    script = reader.ReadToEnd();

                conn.Open();
                SqlCommand cmd = new SqlCommand { Connection = conn };
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
        }

        static class Log
        {
            public static void WriteLine(StreamWriter writer, string value, bool inFileToo = true)
            {
                if (inFileToo)
                {
                    try
                    {
                        writer.WriteLine(value);
                    }
                    catch { }
                }
                Console.WriteLine(value);
            }
        }
    }
}
