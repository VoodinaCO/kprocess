using ArxOne.Ftp;
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
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utils
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

            Console.Write("Add project dispositions if not exist : ");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteScript(sqlConn, "ProjectDispositionMemorized");
            }
            Console.WriteLine("OK\n");

            Console.WriteLine("Add IsDeleted to Audit if not exists");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteCommandFormat<int>(sqlConn, $"EXEC AddColumnIfNotExists 'Audit', 'IsDeleted', '[BIT] NOT NULL DEFAULT((0))';");
                sqlConn.Close();
            }
            Console.WriteLine("OK\n");

            Console.WriteLine("Add LinkedInspectionId to InspectionStep if not exists");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                bool exists = false;
                using (SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT column_id FROM sys.columns WHERE NAME = 'LinkedInspectionId' AND object_id = OBJECT_ID('InspectionStep');"))
                {
                    exists = reader.HasRows;
                    reader.Close();
                    sqlConn.Close();
                }
                Console.WriteLine($"LinkedInspectionId exists : {(exists ? "Yes\n" : "No")}");
                if (!exists)
                {
                    Console.Write("Clear all inspections : ");
                    ExecuteScript(sqlConn, "ClearAllInspections");
                    Console.WriteLine("OK");
                    Console.Write("Add LinkedInspectionId to InspectionStep : ");
                    ExecuteScript(sqlConn, "AddInspectionStep_LinkedInspectionStep");
                    Console.WriteLine("OK\n");
                }
            }

            Console.WriteLine("Add QualificationReason table if not exists");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                bool exists = false;
                using (SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT * FROM sys.tables WHERE NAME = 'QualificationReason';"))
                {
                    exists = reader.HasRows;
                    reader.Close();
                    sqlConn.Close();
                }
                Console.WriteLine($"QualificationReason table exists : {(exists ? "Yes\n" : "No")}");
                if (!exists)
                {
                    Console.Write("Add QualificationReason table : ");
                    ExecuteScript(sqlConn, "QualificationReason");
                    Console.WriteLine("OK\n");
                }
            }

            Console.WriteLine("Add IsDeleted and AnomalyOrigin if not exist");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteScript(sqlConn, "IsDeleted_AnomalyOrigin");
            }
            Console.WriteLine("OK\n");

            Console.WriteLine("Extract thumbnails");
            string thumbnailsDir = @"C:\Utils\Thumbnails";
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                Console.Write("Configure OLE : ");
                ExecuteScript(sqlConn, "ConfigureOle", true, "master");
                Console.WriteLine("OK");
                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                Console.Write($"Configure Role Server bulkadmin for {currentIdentity.Name} : ");
                ExecuteCommandFormat<int>(sqlConn, "ALTER SERVER ROLE [bulkadmin] ADD MEMBER [{0}];", currentIdentity.Name);
                Console.WriteLine("OK");
            }
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                Console.Write("Add ThumbnailHash to Action : ");
                ExecuteScript(sqlConn, "ActionThumbnailHash");
                Console.WriteLine("OK");
                Console.Write("Add Action_ExportThumbnail procedure : ");
                ExecuteScript(sqlConn, "Action_ExportThumbnail");
                Console.WriteLine("OK");
                Console.Write("Add Action_ExportAllThumbnail procedure : ");
                ExecuteScript(sqlConn, "Action_ExportAllThumbnail");
                Console.WriteLine("OK");
                Console.Write($"Create thumbnails directory ({thumbnailsDir}) : ");
                Directory.CreateDirectory(thumbnailsDir);
                Console.WriteLine("OK");
                Console.Write($"Extract thumbnails : ");
                ExecuteCommandFormat<int>(sqlConn, "EXECUTE dbo.[Action_ExportAllThumbnail] N'{0}', N'{1}';", thumbnailsDir, ".jpg");
                Console.WriteLine("OK");
                Console.Write($"List exported thumbnails : ");
                var thumbnails = Directory.EnumerateFiles(thumbnailsDir);
                Console.WriteLine("OK");
                Console.Write($"Compute hashes and rename thumbnails : ");
                Dictionary<int, (string hash, string extension)> hashThumbnailsDict = new Dictionary<int, (string hash, string extension)>();
                foreach (string thumbnail in thumbnails.Where(_ => int.TryParse(Path.GetFileNameWithoutExtension(_), out int tmpResult)))
                {
                    HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                    string newName;
                    using (var fileStream = File.OpenRead(thumbnail))
                    {
                        newName = ToHashString(murmur128.ComputeHash(fileStream));
                    }
                    hashThumbnailsDict.Add(int.Parse(Path.GetFileNameWithoutExtension(thumbnail)), (newName, Path.GetExtension(thumbnail)));
                    newName = $"{newName}{Path.GetExtension(thumbnail)}";
                    if (File.Exists(Path.Combine(thumbnailsDir, newName)))
                        File.Delete(thumbnail);
                    else
                        File.Move(thumbnail, Path.Combine(thumbnailsDir, newName));
                }
                Console.WriteLine("OK");
                Console.Write($"Insert thumbnails into CloudFile : ");
                var cloudFiles = new Dictionary<string, string>();
                foreach (var kv in hashThumbnailsDict)
                    if (!cloudFiles.ContainsKey(kv.Value.hash))
                        cloudFiles.Add(kv.Value.hash, kv.Value.extension);
                foreach (var kv in cloudFiles)
                    ExecuteCommandFormat<int>(sqlConn, "INSERT INTO [dbo].[CloudFile] ([Hash],[Extension]) VALUES ('{0}','{1}');",
                        kv.Key,
                        kv.Value);
                Console.WriteLine("OK");
                Console.Write($"Update CloudFile links in Action : ");
                foreach (var kv in hashThumbnailsDict)
                    ExecuteCommandFormat<int>(sqlConn, "UPDATE [dbo].[Action] SET [ThumbnailHash] = '{0}' WHERE [ActionId] = {1};",
                        kv.Value.hash,
                        kv.Key);
                Console.WriteLine("OK");
                Console.Write($"Delete Thumbnail from Action : ");
                ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[Action] DROP COLUMN [Thumbnail];");
                Console.WriteLine("OK");
                Console.Write($"Delete Action_ExportAllThumbnail procedure : ");
                ExecuteCommandFormat<int>(sqlConn, "DROP PROCEDURE [dbo].Action_ExportAllThumbnail;");
                Console.WriteLine("OK");
                Console.Write($"Delete Action_ExportThumbnail procedure : ");
                ExecuteCommandFormat<int>(sqlConn, "DROP PROCEDURE [dbo].Action_ExportThumbnail;");
                Console.WriteLine("OK\n");
            }
            Console.Write("Copy thumbnails to SFTP : ");
            var thumbnailsToCopy = Directory.EnumerateFiles(thumbnailsDir);
            long totalSize = thumbnailsToCopy.Sum(_ => (new FileInfo(_)).Length);
            long alreadyCopied = 0;
            byte[] buffer = new byte[BufferSize];
            try
            {
                _sftpClient.Connect();
                foreach (var thumbnailToCopy in thumbnailsToCopy)
                {
                    using (var localFileStream = File.OpenRead(thumbnailToCopy))
                    using (var remoteStream = _sftpClient.OpenWrite($"{SFTP_PublishedFilesDirectory}/{Path.GetFileName(thumbnailToCopy)}"))
                    {
                        int readBytes = localFileStream.Read(buffer, 0, buffer.Length);
                        while (readBytes > 0)
                        {
                            remoteStream.Write(buffer, 0, readBytes);
                            alreadyCopied += readBytes;
                            ConsoleProgress.Write("{0}%", alreadyCopied * 100 / totalSize);
                            readBytes = localFileStream.Read(buffer, 0, buffer.Length);
                        }
                    }
                }
                _sftpClient.Disconnect();
                ConsoleProgress.Finish("OK\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL");
                Console.WriteLine(e.Message);

                Console.WriteLine("\nFinish.\nPress a key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Add CloudFile links if not exist");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteScript(sqlConn, "CloudFile_Links");
            }
            Console.WriteLine("OK\n");

            Console.WriteLine("Migrate videos of projects to videos of processes");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                Console.Write("Insert video resources : ");
                ExecuteScript(sqlConn, "VideoResources");
                Console.WriteLine("OK");
                Console.Write("Add VideoSync table if not exists : ");
                ExecuteScript(sqlConn, "VideoSync");
                Console.WriteLine("OK");
                Console.Write("Update Video if necessary : ");
                ExecuteScript(sqlConn, "VideoProjectToProcess");
                Console.WriteLine("OK\n");
            }

            Console.WriteLine("Fix hashes");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                Console.Write("Part 1 : ");
                ExecuteScript(sqlConn, "FixHashes1");
                Console.WriteLine("OK");
                string constraintName = null;
                Console.Write("Update PublishedFile.Hash : ");
                using (SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT c.[name] FROM[sys].[key_constraints] c INNER JOIN[sys].[objects] o ON c.[parent_object_id] = o.[object_id] WHERE c.[type] = 'PK' AND o.[name] = 'PublishedFile';"))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        constraintName = reader.GetString(0);
                    }
                    reader.Close();
                    sqlConn.Close();
                }
                if (!string.IsNullOrEmpty(constraintName))
                    ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[PublishedFile] DROP CONSTRAINT {0};", constraintName);
                ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[PublishedFile] ALTER COLUMN [Hash] NCHAR(32) NOT NULL;");
                ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[PublishedFile] ADD CONSTRAINT PK_PublishedFile PRIMARY KEY (Hash);");
                Console.WriteLine("OK");
                constraintName = null;
                Console.Write("Update CutVideo.Hash : ");
                using (SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, $"SELECT c.[name] FROM[sys].[key_constraints] c INNER JOIN[sys].[objects] o ON c.[parent_object_id] = o.[object_id] WHERE c.[type] = 'PK' AND o.[name] = 'CutVideo';"))
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        constraintName = reader.GetString(0);
                    }
                    reader.Close();
                    sqlConn.Close();
                }
                if (!string.IsNullOrEmpty(constraintName))
                    ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[CutVideo] DROP CONSTRAINT {0};", constraintName);
                ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[CutVideo] ALTER COLUMN [Hash] NCHAR(32) NOT NULL;");
                ExecuteCommandFormat<int>(sqlConn, "ALTER TABLE [dbo].[CutVideo] ADD CONSTRAINT PK_CutVideo PRIMARY KEY (Hash);");
                Console.WriteLine("OK");
                Console.Write("Part 2 : ");
                ExecuteScript(sqlConn, "FixHashes2");
                Console.WriteLine("OK\n");
            }

            Console.WriteLine("Add Timeslot if not exist");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteScript(sqlConn, "Timeslot");
            }
            Console.WriteLine("OK\n");

            Console.WriteLine("Add InspectionSchedule if not exist");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                ExecuteScript(sqlConn, "InspectionSchedule");
            }
            Console.WriteLine("OK\n");

            Console.Write("Copy FTP files to SFTP : ");
            if (FTP_to_SFTP())
                ConsoleProgress.Finish("OK\n");
            else
                ConsoleProgress.Finish("FAIL\n");

            Console.WriteLine("\nFinish.\nPress a key to exit...");
            Console.ReadKey();
        }

        static bool FTP_to_SFTP()
        {
            long alreadyCopied = 0;
            byte[] buffer = new byte[BufferSize];
            try
            {
                using (var _ftpClient = new FtpClient(FtpProtocol.Ftp, FTP_Server, FTP_Port, FTP_Credentials))
                {
                    _sftpClient.Connect();
                    var filesToCopy = _ftpClient.ListEntries(FTP_PublishedFilesDirectory);
                    long totalSize = filesToCopy.Sum(_ => _.Size ?? 0);
                    int totalFiles = filesToCopy.Count();
                    int copiedFiles = 0;
                    foreach (var fileToCopy in filesToCopy)
                    {
                        var sftpFileName = $"{SFTP_PublishedFilesDirectory}/{Path.GetFileName(fileToCopy.Name.Replace("-", ""))}";

                        HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                        string ftpHash = null;
                        string sftpHash = null;
                        long ftpSize = fileToCopy.Size.Value;
                        long sftpSize = 0;
                        string tempFile = Path.GetTempFileName();

                        using (var ftpFileStream = _ftpClient.Retr(fileToCopy.Path))
                        using (var tempFileStream = File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            int readBytes = ftpFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                tempFileStream.Write(buffer, 0, readBytes);
                                readBytes = ftpFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        Log.WriteLine($"Copy {fileToCopy.Name} from FTP to {tempFile}");

                        using (var tempFileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            ftpHash = ToHashString(murmur128.ComputeHash(tempFileStream));
                        }
                        if (_sftpClient.Exists(sftpFileName))
                        {
                            sftpSize = _sftpClient.Get(sftpFileName).Length;
                            using (var sftpFileStream = _sftpClient.OpenRead(sftpFileName))
                            {
                                sftpHash = ToHashString(murmur128.ComputeHash(sftpFileStream));
                            }
                        }
                        if (ftpHash == sftpHash)
                        {
                            alreadyCopied += fileToCopy.Size.Value;
                            copiedFiles++;
                            ConsoleProgress.Write("({1}/{2}) {0}%", alreadyCopied * 100 / totalSize, copiedFiles, totalFiles);
                            File.Delete(tempFile);
                            continue;
                        }
                        Log.WriteLine($"{fileToCopy.Name} - {ftpHash} ({ftpSize}) - {sftpHash} ({(string.IsNullOrEmpty(sftpHash) ? 0 : sftpSize)})");

                        using (var tempFileStream = File.Open(tempFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var sftpFileStream = _sftpClient.Open(sftpFileName, FileMode.Create, FileAccess.Write))
                        {
                            int readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                sftpFileStream.Write(buffer, 0, readBytes);
                                alreadyCopied += readBytes;
                                ConsoleProgress.Write("({1}/{2}) {0}%", alreadyCopied * 100 / totalSize, copiedFiles, totalFiles);
                                readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        File.Delete(tempFile);
                        copiedFiles++;

                        sftpSize = _sftpClient.Get(sftpFileName).Length;
                        using (var sftpFileStream = _sftpClient.OpenRead(sftpFileName))
                        {
                            sftpHash = ToHashString(murmur128.ComputeHash(sftpFileStream));
                        }
                        Log.WriteLine($"Verify {fileToCopy.Name} - {ftpHash} ({ftpSize}) - {sftpHash} ({sftpSize})");
                    }
                    _sftpClient.Disconnect();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(e.Message);
                return false;
            }
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
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Utils.Scripts.{scriptName}.sql");
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
