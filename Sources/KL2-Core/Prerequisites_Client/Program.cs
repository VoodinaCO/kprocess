using KProcess.KL2.ConnectionSecurity;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;

namespace Prerequisites_Client
{
    class Program
    {
        const string DataSourceKey = "DataSource";
        const string InitialCatalogKey = "InitialCatalog";

        const string SFTP_ServerKey = "SFTP_Server";
        const string SFTP_PortKey = "SFTP_Port";
        const string SFTP_UserKey = "SFTP_User";
        const string SFTP_PasswordKey = "SFTP_Password";
        const string SFTP_PublishedFilesDirectoryKey = "SFTP_PublishedFilesDirectory";
        const string SFTP_UploadedFilesDirectoryKey = "SFTP_UploadedFilesDirectory";

        const string FTP_ServerKey = "FTP_Server";
        const string FTP_PortKey = "FTP_Port";
        const string FTP_UserKey = "FTP_User";
        const string FTP_PasswordKey = "FTP_Password";
        const string FTP_PublishedFilesDirectoryKey = "FTP_PublishedFilesDirectory";
        const string FTP_UploadedFilesDirectoryKey = "FTP_UploadedFilesDirectory";

        public static string SFTP_Server { get; private set; } = @"127.0.0.1";
        public static int SFTP_Port { get; private set; } = 22;
        public static string SFTP_User { get; private set; }
        public static string SFTP_Password { get; private set; }
        public static NetworkCredential SFTP_Credentials { get; private set; }
        public static string SFTP_PublishedFilesDirectory { get; private set; } = @"/PublishedFiles";
        public static string SFTP_UploadedFilesDirectory { get; private set; } = @"/UploadedFiles";
        static SftpClient _sftpClient;

        static List<ResourceFile> ResourceFiles = new List<ResourceFile>();

        static void Main()
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

            try
            {
                Console.WriteLine($"Test connection to database with DataSource={ConfigurationManager.AppSettings[DataSourceKey]} and InitialCatalog={ConfigurationManager.AppSettings[InitialCatalogKey]} : ");
                using (var sqlConn = new SqlConnection(GetConnectionString()))
                {
                    ExecuteCommandFormat<int>(sqlConn, "EXECUTE [dbo].[GetDatabaseVersion];");
                }
                Console.WriteLine("OK\n");

                Console.WriteLine($"Test SFTP connection : ");
                _sftpClient.Connect();
                try
                {
                    SftpFile entry = _sftpClient.Get(SFTP_PublishedFilesDirectory);
                    if (entry != null && entry.IsDirectory)
                        Console.WriteLine("Folder 'PublishedFiles' exists on SFTP server.");
                    else
                        throw new Exception("Folder 'PublishedFiles' doesn't exist on SFTP server.");
                }
                catch
                {
                    throw new Exception("Folder 'PublishedFiles' doesn't exist on FTP server.");
                }
                try
                {
                    SftpFile entry = _sftpClient.Get(SFTP_UploadedFilesDirectory);
                    if (entry != null && entry.IsDirectory)
                        Console.WriteLine("Folder 'UploadedFiles' exists on SFTP server.");
                    else
                        throw new Exception("Folder 'UploadedFiles' doesn't exist on SFTP server.");
                }
                catch
                {
                    throw new Exception("Folder 'UploadedFiles' doesn't exist on FTP server.");
                }
                _sftpClient.Disconnect();
                Console.WriteLine("OK\n");

                Console.WriteLine("List files from database : ");
                using (var sqlConn = new SqlConnection(GetConnectionString()))
                {
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
                    Console.WriteLine("OK\n");
                }

                Console.WriteLine("Test if files exist : ");
                int totalFiles = ResourceFiles.Count;
                int managedFiles = 0;
                string notExisting = string.Empty;
                ConsoleProgress.Write($"({managedFiles}/{totalFiles})");
                foreach (var file in ResourceFiles)
                {
                    if (File.Exists(file.Uri))
                        file.LocalFileExists = true;
                    else
                        notExisting += $"\n{file.TableName} file '{file.Uri}' doesn't exist.";
                    ConsoleProgress.Write($"({++managedFiles}/{totalFiles})");
                }
                ConsoleProgress.Finish("OK\n");

                if (string.IsNullOrEmpty(notExisting))
                    Console.WriteLine("All file exist.\n");
                else
                    Console.WriteLine($"Notexistent files :{notExisting}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAIL\n");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Finish.\nPress a key to exit...");
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
    }
}
