using ArxOne.Ftp;
using KProcess.KL2.ConnectionSecurity;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;

namespace Prerequisites_Server
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

        public static string FTP_Server { get; private set; } = @"127.0.0.1";
        public static int FTP_Port { get; private set; } = 21;
        public static string FTP_User { get; private set; }
        public static string FTP_Password { get; private set; }
        public static NetworkCredential FTP_Credentials { get; private set; }
        public static string FTP_PublishedFilesDirectory { get; private set; } = @"/PublishedFiles";
        public static string FTP_UploadedFilesDirectory { get; private set; } = @"/UploadedFiles";

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

            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_ServerKey))
                FTP_Server = ConfigurationManager.AppSettings[FTP_ServerKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_PortKey))
                FTP_Port = int.Parse(ConfigurationManager.AppSettings[FTP_PortKey]);
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_UserKey))
                FTP_User = ConfigurationManager.AppSettings[FTP_UserKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_PasswordKey))
                FTP_Password = ConfigurationManager.AppSettings[FTP_PasswordKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_PublishedFilesDirectoryKey))
                FTP_PublishedFilesDirectory = ConfigurationManager.AppSettings[FTP_PublishedFilesDirectoryKey];
            if (ConfigurationManager.AppSettings.AllKeys.Contains(FTP_UploadedFilesDirectoryKey))
                FTP_UploadedFilesDirectory = ConfigurationManager.AppSettings[FTP_UploadedFilesDirectoryKey];
            FTP_Credentials = new NetworkCredential(FTP_User, FTP_Password);

            try
            {
                string rootDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if !DEBUG
                Console.WriteLine($"Test executable directory : ");
                if (rootDir == @"C:\Utils")
                    Console.WriteLine("OK\n");
                else
                    throw new Exception(@"Executable must be in 'C:\Utils' folder.");
#endif

                Console.WriteLine($"Test file creation rights : ");
                using (var fileStream = File.Create(Path.Combine(rootDir, "test.file")))
                { }
                File.Delete(Path.Combine(rootDir, "test.file"));
                Console.WriteLine("OK\n");

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

                Console.WriteLine($"Test FTP connection : ");
                using (var _ftpClient = new FtpClient(FtpProtocol.Ftp, FTP_Server, FTP_Port, FTP_Credentials))
                {
                    try
                    {
                        FtpEntry entry = _ftpClient.GetEntry(FTP_PublishedFilesDirectory);
                        if (entry != null && entry.Type == FtpEntryType.Directory)
                            Console.WriteLine("Folder 'PublishedFiles' exists on FTP server.");
                        else
                            throw new Exception("Folder 'PublishedFiles' doesn't exist on FTP server.");
                    }
                    catch
                    {
                        throw new Exception("Folder 'PublishedFiles' doesn't exist on FTP server.");
                    }
                    try
                    {
                        FtpEntry entry = _ftpClient.GetEntry(FTP_UploadedFilesDirectory);
                        if (entry != null && entry.Type == FtpEntryType.Directory)
                            Console.WriteLine("Folder 'UploadedFiles' exists on FTP server.");
                        else
                            throw new Exception("Folder 'UploadedFiles' doesn't exist on FTP server.");
                    }
                    catch
                    {
                        throw new Exception("Folder 'UploadedFiles' doesn't exist on FTP server.");
                    }
                }
                Console.WriteLine("OK\n");
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
