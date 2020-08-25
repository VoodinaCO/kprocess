using KProcess.KL2.ConnectionSecurity;
using Murmur;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MigrateOldFiles
{
    class Program
    {
        const int BufferSize = 81920; // 80KB

        const string DataSourceKey = "DataSource";
        const string InitialCatalogKey = "InitialCatalog";

        const string SFTP_ServerKey = "SFTP_Server";
        const string SFTP_PortKey = "SFTP_Port";
        const string SFTP_UserKey = "SFTP_User";
        const string SFTP_PasswordKey = "SFTP_Password";
        const string SFTP_PublishedFilesDirectoryKey = "SFTP_PublishedFilesDirectory";
        const string SFTP_UploadedFilesDirectoryKey = "SFTP_UploadedFilesDirectory";

        public static string SFTP_Server { get; private set; } = @"127.0.0.1";
        public static int SFTP_Port { get; private set; } = 22;
        public static string SFTP_User { get; private set; }
        public static string SFTP_Password { get; private set; }
        public static NetworkCredential SFTP_Credentials { get; private set; }
        public static string SFTP_PublishedFilesDirectory { get; private set; } = @"/PublishedFiles";
        public static string SFTP_UploadedFilesDirectory { get; private set; } = @"/UploadedFiles";
        static SftpClient _sftpClient;

        static List<ResourceFile> ResourceFiles = new List<ResourceFile>();
        static List<string> CloudFiles = new List<string>();

        static void Main(string[] args)
        {
            ConsoleInterop.DisableQuickEdit();
            Console.CursorVisible = false;

            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_ServerKey))
                SFTP_Server = ConfigurationManager.AppSettings[SFTP_ServerKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_PortKey))
                SFTP_Port = int.Parse(ConfigurationManager.AppSettings[SFTP_PortKey]);
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_UserKey))
                SFTP_User = ConfigurationManager.AppSettings[SFTP_UserKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_PasswordKey))
                SFTP_Password = ConfigurationManager.AppSettings[SFTP_PasswordKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_PublishedFilesDirectoryKey))
                SFTP_PublishedFilesDirectory = ConfigurationManager.AppSettings[SFTP_PublishedFilesDirectoryKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(SFTP_UploadedFilesDirectoryKey))
                SFTP_UploadedFilesDirectory = ConfigurationManager.AppSettings[SFTP_UploadedFilesDirectoryKey];
            SFTP_Credentials = new NetworkCredential(SFTP_User, SFTP_Password);
            _sftpClient = new SftpClient(SFTP_Server, SFTP_Port, SFTP_Credentials.UserName, SFTP_Credentials.Password);

            Console.Write("List CloudFile from database : ");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ReadCloudFileFromDataBase(sqlConn);
            }
            Console.WriteLine("OK");

            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                Console.Write("List files from database : ");
                ReadUriFromDataBase(sqlConn, "RefResource", "ResourceId");
                ReadUriFromDataBase(sqlConn, "RefActionCategory", "ActionCategoryId");
                ReadUriFromDataBase(sqlConn, "Ref1", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref2", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref3", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref4", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref5", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref6", "RefId");
                ReadUriFromDataBase(sqlConn, "Ref7", "RefId");
                ReadUriFromDataBase(sqlConn, "Video", "VideoId", "FilePath");
                Console.WriteLine("OK");
            }

            Console.Write("Test if files exist : ");
            int totalFiles = ResourceFiles.Count;
            int managedFiles = 0;
            ConsoleProgress.Write($"({managedFiles}/{totalFiles})");
            foreach (var file in ResourceFiles)
            {
                if (File.Exists(file.Uri))
                    file.LocalFileExists = true;
                else
                    Log.WriteLine($"{file.TableName} file '{file.Uri}' doesn't exist.");
                ConsoleProgress.Write($"({++managedFiles}/{totalFiles})");
            }
            ConsoleProgress.Finish("OK");

            Console.Write("Compute hashes : ");
            totalFiles = ResourceFiles.Count(_ => _.LocalFileExists);
            managedFiles = 0;
            ConsoleProgress.Write($"({managedFiles}/{totalFiles})");
            HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
            foreach (var file in ResourceFiles.Where(_ => _.LocalFileExists))
            {
                using (var fileStream = File.OpenRead(file.Uri))
                {
                    file.Hash = ToHashString(murmur128.ComputeHash(fileStream));
                }
                ConsoleProgress.Write($"({++managedFiles}/{totalFiles})");
            }
            ConsoleProgress.Finish("OK");

            Console.Write("Copy files with hash name and update hash into tables : ");
            long totalSize = ResourceFiles.Where(_ => _.LocalFileExists && _.Hash != null).Sum(_ => (new FileInfo(_.Uri)).Length);
            long alreadyCopied = 0;
            totalFiles = ResourceFiles.Count(_ => _.LocalFileExists && _.Hash != null);
            int copiedFiles = 0;
            byte[] buffer = new byte[BufferSize];
            ConsoleProgress.Write("({1}/{2}) {0}%", 0, copiedFiles, totalFiles);
            try
            {
                _sftpClient.Connect();
                foreach (var file in ResourceFiles.Where(_ => _.LocalFileExists && _.Hash != null))
                {
                    string sftpFilePath = $"{SFTP_PublishedFilesDirectory}/{file.Hash}{Path.GetExtension(file.Uri)}";
                    if (_sftpClient.Exists(sftpFilePath))
                    {
                        alreadyCopied += (new FileInfo(file.Uri)).Length;
                        copiedFiles++;
                        file.Copied = true;
                        ConsoleProgress.Write("({1}/{2}) {0}%", alreadyCopied * 100 / totalSize, copiedFiles, totalFiles);
                    }
                    else
                    {
                        using (var localFileStream = File.OpenRead(file.Uri))
                        using (var remoteStream = _sftpClient.OpenWrite(sftpFilePath))
                        {
                            int readBytes = localFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                remoteStream.Write(buffer, 0, readBytes);
                                alreadyCopied += readBytes;
                                ConsoleProgress.Write("({1}/{2}) {0}%", alreadyCopied * 100 / totalSize, copiedFiles, totalFiles);
                                readBytes = localFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        copiedFiles++;
                        file.Copied = true;
                    }
                    using (var sqlConn = new SqlConnection(GetConnectionString()))
                    {
                        if (CloudFiles.Contains(file.Hash))
                            file.AlreadyInBase = true;
                        else if (file.TableName != "Video")
                        {
                            ExecuteCommandFormat<int>(sqlConn, "INSERT INTO [dbo].[CloudFile] ([Hash],[Extension]) VALUES ('{0}',{1})", file.Hash, string.IsNullOrEmpty(Path.GetExtension(file.Uri)) ? "NULL" : $"'{Path.GetExtension(file.Uri)}'");
                            CloudFiles.Add(file.Hash);
                        }
                        else if (file.TableName == "Video")
                        {
                            ExecuteCommandFormat<int>(sqlConn, "UPDATE [dbo].[Video] SET [OriginalHash] = '{1}' WHERE [VideoId] = {0};", file.Id, file.Hash);
                            // If video file is used in a project, set Sync to true
                            ExecuteCommandFormat<int>(sqlConn, "IF EXISTS (SELECT * FROM [dbo].[Action] WHERE [VideoId] = {0}) BEGIN UPDATE [dbo].[Video] SET [Sync] = 1 WHERE [VideoId] = {0}; END", file.Id);
                        }
                        ExecuteCommandFormat<int>(sqlConn, $"UPDATE [dbo].[{file.TableName}] SET [{ResourceFile.HashPropertyName}] = '{file.Hash}'{(file.TableName == "Video" ? $", Extension = '{Path.GetExtension(file.Uri)}', OnServer = 1" : string.Empty)} WHERE [{file.IdPropertyName}] = {file.Id};");
                    }
                }
                _sftpClient.Disconnect();
                ConsoleProgress.Finish("OK\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                Console.WriteLine("\nFinish.\nPress a key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nFinish.\nPress a key to exit...");
            Console.ReadKey();
        }

        static void ReadUriFromDataBase(SqlConnection sqlConn, string tableName, string idName, string uriName = ResourceFile.UriPropertyName)
        {
            SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT [{idName}],[{uriName}] FROM [dbo].[{tableName}] WHERE [{uriName}] IS NOT NULL;");
            if (reader.HasRows)
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var uri = reader.GetString(1);
                    ResourceFiles.Add(new ResourceFile
                    {
                        TableName = tableName,
                        IdPropertyName = idName,
                        Id = id,
                        Uri = uri
                    });
                }
            reader.Close();
            sqlConn.Close();
        }

        static void ReadCloudFileFromDataBase(SqlConnection sqlConn)
        {
            SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT [{ResourceFile.HashPropertyName}] FROM [dbo].[CloudFile];");
            if (reader.HasRows)
                while (reader.Read())
                    CloudFiles.Add(reader.GetString(0));
            reader.Close();
            sqlConn.Close();
        }

        public static string GetConnectionString()
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

        public static void ExecuteScript(SqlConnection conn, string scriptName, bool useDatabase = false, string databaseName = Const.DataBaseName_v3)
        {
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MigrateOldFiles.Scripts.{scriptName}.sql");
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
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                try { conn.Close(); } catch { }
            }
        }

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

        public static string ToHashString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (byte hashed in hash)
                builder.AppendFormat("{0:X2}", hashed);
            return builder.ToString();
        }
    }
}
