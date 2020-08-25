using KProcess.KL2.ConnectionSecurity;
using Murmur;
using Newtonsoft.Json.Linq;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Reencode
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

        static readonly string BufferDir =  @"C:\Utils\Buffer";

        static readonly List<(string Hash, string Extension)> Videos = new List<(string Hash, string Extension)>();

        static string ffmpegPath;
        static string ffprobePath;

        static readonly string transcodeExt = ".mp4";
        static readonly int maxAudioBitRate = 96000; // 96k
        static readonly int maxVideoBitRate = 1500000; // 1500k
        static readonly int maxResolution = 720;
        static TimeSpan videoDuration = new TimeSpan(0);
        static TimeSpan progressDuration = new TimeSpan(0);

        static void Main()
        {
            ConsoleInterop.DisableQuickEdit();
            Console.CursorVisible = false;

            string assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

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

            Directory.CreateDirectory(BufferDir);
            ffmpegPath = Path.Combine(assemblyLocation ?? string.Empty, "ffme", "ffmpeg.exe");
            ffprobePath = Path.Combine(assemblyLocation ?? string.Empty, "ffme", "ffprobe.exe");

            Console.WriteLine("Manage original videos");
            Console.Write("List original videos : ");
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                using (var reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, "SELECT DISTINCT [Hash], [Extension] FROM [dbo].[Video] WHERE [Hash] IS NOT NULL AND [Hash] = [OriginalHash];"))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            var hash = reader.GetString(0);
                            var ext = reader.GetString(1);
                            Videos.Add((hash, ext));
                        }
                    reader.Close();
                    sqlConn.Close();
                }
            }
            Console.WriteLine("OK");
            try
            {
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                byte[] buffer = new byte[BufferSize];
                int totalFiles = Videos.Count;
                int managedFiles = 0;
                foreach ((string Hash, string Extension) in Videos)
                {
                    Console.Write($"Manage {Hash}{Extension} ({managedFiles + 1}/{totalFiles}) : ");
                    ConsoleProgress.Write("Copy file to local...");
                    string sftpInFilePath = $"{SFTP_PublishedFilesDirectory}/{Hash}{Extension}";
                    string tempInFilePath = Path.Combine(BufferDir, $"{Hash}{Extension}");
                    if (!File.Exists(tempInFilePath))
                    {
                        _sftpClient.Connect();
                        using (var sftpFileStream = _sftpClient.OpenRead(sftpInFilePath))
                        using (var tempFileStream = File.Open(tempInFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            int readBytes = sftpFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                tempFileStream.Write(buffer, 0, readBytes);
                                readBytes = sftpFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        _sftpClient.Disconnect();
                    }
                    ConsoleProgress.Write("Reencode 0%");
                    string tempOutFilePath = Path.Combine(BufferDir, $"out{transcodeExt}");
                    var mediaInfo = GetMediaInfo(new FileInfo(tempInFilePath));
                    var processArgumentsBuilder = new StringBuilder($"-hide_banner -y -nostdin -i \"{tempInFilePath}\"");
                    if (mediaInfo.HasAudio)
                        processArgumentsBuilder.Append($" -acodec mp3 -b:a {Math.Min(mediaInfo.AudioBitrate ?? maxAudioBitRate, maxAudioBitRate)}");
                    if (mediaInfo.HasVideo)
                    {
                        if (Math.Min(mediaInfo.Width, mediaInfo.Height) > maxResolution) // Needs downscaling
                            processArgumentsBuilder.Append($" -vf \"scale={(mediaInfo.Width > mediaInfo.Height ? $"-2:{maxResolution}" : $"{maxResolution}:-2")}\"");
                        else if (mediaInfo.Width % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                        {
                            mediaInfo.Width++;
                            processArgumentsBuilder.Append($" -vf \"scale={mediaInfo.Width}:-2\"");
                        }
                        else if (mediaInfo.Height % 2 != 0) // Needs rescaling (MP4 resolutions must be multiple of 2)
                        {
                            mediaInfo.Height++;
                            processArgumentsBuilder.Append($" -vf \"scale=-2:{mediaInfo.Height}\"");
                        }
                        processArgumentsBuilder.Append($" -vcodec libx264 -preset fast -b:v {Math.Min(mediaInfo.VideoBitrate ?? maxVideoBitRate, maxVideoBitRate)}");
                    }
                    processArgumentsBuilder.Append($" -f mp4 -movflags faststart \"{tempOutFilePath}\"");
                    using (var process = new Process())
                    {
                        videoDuration = new TimeSpan(0);
                        progressDuration = new TimeSpan(0);
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.RedirectStandardOutput = true;

                        process.OutputDataReceived += (s, e) => Log.WriteLine(e.Data);
                        process.ErrorDataReceived += (s, e) =>
                        {
                            if (e.Data != null && e.Data.Trim().StartsWith("Duration:"))
                            {
                                string duration = e.Data.Trim().Split(',').First().Split(' ')[1].Trim();
                                TimeSpan.TryParse(duration, out videoDuration);
                            }
                            if (e.Data != null && e.Data.Trim().StartsWith("frame="))
                            {
                                string duration = e.Data.Trim().Split(' ').Single(_ => _.StartsWith("time=")).Split('=')[1];
                                TimeSpan.TryParse(duration, out progressDuration);
                                double progressValue = videoDuration.Ticks == 0 ? 0 : Math.Round(progressDuration.TotalMilliseconds * 100 / videoDuration.TotalMilliseconds);
                                ConsoleProgress.Write($"Reencode {progressValue}%");
                            }
                            Log.WriteLine(e.Data);
                        };

                        try
                        {
                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            process.WaitForExit();
                            if (process.ExitCode != 0)
                                throw new Exception();
                        }
                        catch (Win32Exception e)
                        {
                            Log.WriteLine($"Le splitter de video n'a pas été trouvé :\n{e.Message}");
                            throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                        }
                        catch (OperationCanceledException e)
                        {
                            Log.WriteLine($"L'export a été annulé lors du split des vidéos :\n{e.Message}");
                            throw;
                        }
                        catch (Exception e)
                        {
                            Log.WriteLine($"Une erreur non prévue s'est produite lors du split de la video :\n{e.Message}");
                            throw;
                        }
                    }
                    ConsoleProgress.Write("Compute new hash...");
                    string newVideoHash;
                    using (var fileStream = File.OpenRead(tempOutFilePath))
                    {
                        newVideoHash = ToHashString(murmur128.ComputeHash(fileStream));
                    }
                    ConsoleProgress.Write("Rename file with new hash...");
                    string newVideoFilePath = Path.Combine(BufferDir, $"{newVideoHash}{Path.GetExtension(tempOutFilePath)}");
                    File.Move(tempOutFilePath, newVideoFilePath);
                    ConsoleProgress.Write("Delete old file...");
                    File.Delete(tempInFilePath);
                    ConsoleProgress.Write("Copy new file to SFTP...");
                    string sftpOutFilePath = $"{SFTP_PublishedFilesDirectory}/{Path.GetFileName(newVideoFilePath)}";
                    _sftpClient.Connect();
                    if (!_sftpClient.Exists(sftpOutFilePath))
                    {
                        using (var tempFileStream = File.Open(newVideoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var sftpFileStream = _sftpClient.Open(sftpOutFilePath, FileMode.Create, FileAccess.Write))
                        {
                            int readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                sftpFileStream.Write(buffer, 0, readBytes);
                                readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    _sftpClient.Disconnect();
                    ConsoleProgress.Write("Update Video hash and links into database...");
                    using (var sqlConn = new SqlConnection(GetConnectionString()))
                    {
                        ExecuteCommandFormat<int>(sqlConn, $"UPDATE [dbo].[Video] SET [Hash] = '{newVideoHash}', [Extension] = '.mp4' WHERE [Hash] = '{Hash}';");
                        ExecuteCommandFormat<int>(sqlConn, $"UPDATE [dbo].[CutVideo] SET [HashOriginalVideo] = '{newVideoHash}' WHERE [HashOriginalVideo] = '{Hash}';");
                    }
                    ConsoleProgress.Write("Delete old file from SFTP...");
                    _sftpClient.Connect();
                    _sftpClient.Delete(sftpInFilePath);
                    _sftpClient.Disconnect();
                    ConsoleProgress.Finish("OK");
                    managedFiles++;
                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Console.WriteLine("\nFinish.\nPress a key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Manage cut videos");
            Console.Write("List cut videos : ");
            Videos.Clear();
            using (var sqlConn = new SqlConnection(GetConnectionString()))
            {
                using (SqlDataReader reader = ExecuteCommandFormat<SqlDataReader>(sqlConn, "SELECT [Hash], [Extension] FROM [dbo].[CutVideo];"))
                {
                    if (reader.HasRows)
                        while (reader.Read())
                        {
                            var hash = reader.GetString(0);
                            var ext = reader.GetString(1);
                            Videos.Add((hash, ext));
                        }
                    reader.Close();
                    sqlConn.Close();
                }
            }
            Console.WriteLine("OK");
            try
            {
                HashAlgorithm murmur128 = MurmurHash.Create128(managed: false);
                byte[] buffer = new byte[BufferSize];
                int totalFiles = Videos.Count;
                int managedFiles = 0;
                foreach ((string Hash, string Extension) in Videos)
                {
                    Console.Write($"Manage {Hash}{Extension} ({managedFiles + 1}/{totalFiles}) : ");
                    ConsoleProgress.Write("Copy file to local...");
                    string sftpInFilePath = $"{SFTP_PublishedFilesDirectory}/{Hash}{Extension}";
                    string tempInFilePath = Path.Combine(BufferDir, $"{Hash}{Extension}");
                    if (!File.Exists(tempInFilePath))
                    {
                        _sftpClient.Connect();
                        using (var sftpFileStream = _sftpClient.OpenRead(sftpInFilePath))
                        using (var tempFileStream = File.Open(tempInFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            int readBytes = sftpFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                tempFileStream.Write(buffer, 0, readBytes);
                                readBytes = sftpFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                        _sftpClient.Disconnect();
                    }
                    ConsoleProgress.Write("Reencode 0%");
                    string tempOutFilePath = Path.Combine(BufferDir, $"out{transcodeExt}");
                    var mediaInfo = GetMediaInfo(new FileInfo(tempInFilePath));
                    var processArgumentsBuilder = new StringBuilder($"-hide_banner -y -nostdin -i \"{tempInFilePath}\"");
                    if (mediaInfo.HasAudio)
                        processArgumentsBuilder.Append($" -acodec mp3 -b:a {Math.Min(mediaInfo.AudioBitrate ?? maxAudioBitRate, maxAudioBitRate)}");
                    if (mediaInfo.HasVideo)
                        processArgumentsBuilder.Append($" -vcodec libx264 -preset fast -b:v {Math.Min(mediaInfo.VideoBitrate ?? maxVideoBitRate, maxVideoBitRate)}");
                    processArgumentsBuilder.Append($" -f mp4 -movflags faststart \"{tempOutFilePath}\"");
                    using (var process = new Process())
                    {
                        videoDuration = new TimeSpan(0);
                        progressDuration = new TimeSpan(0);
                        process.StartInfo.FileName = ffmpegPath;
                        process.StartInfo.Arguments = processArgumentsBuilder.ToString();
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.RedirectStandardOutput = true;

                        process.OutputDataReceived += (s, e) => Log.WriteLine(e.Data);
                        process.ErrorDataReceived += (s, e) =>
                        {
                            if (e.Data != null && e.Data.Trim().StartsWith("Duration:"))
                            {
                                string duration = e.Data.Trim().Split(',').First().Split(' ')[1].Trim();
                                TimeSpan.TryParse(duration, out videoDuration);
                            }
                            if (e.Data != null && e.Data.Trim().StartsWith("frame="))
                            {
                                string duration = e.Data.Trim().Split(' ').Single(_ => _.StartsWith("time=")).Split('=')[1];
                                TimeSpan.TryParse(duration, out progressDuration);
                                double progressValue = videoDuration.Ticks == 0 ? 0 : Math.Round(progressDuration.TotalMilliseconds * 100 / videoDuration.TotalMilliseconds);
                                ConsoleProgress.Write($"Reencode {progressValue}%");
                            }
                            Log.WriteLine(e.Data);
                        };

                        try
                        {
                            process.Start();
                            process.BeginOutputReadLine();
                            process.BeginErrorReadLine();

                            process.WaitForExit();
                            if (process.ExitCode != 0)
                                throw new Exception();
                        }
                        catch (Win32Exception e)
                        {
                            Log.WriteLine($"Le splitter de video n'a pas été trouvé :\n{e.Message}");
                            throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                        }
                        catch (OperationCanceledException e)
                        {
                            Log.WriteLine($"L'export a été annulé lors du split des vidéos :\n{e.Message}");
                            throw;
                        }
                        catch (Exception e)
                        {
                            Log.WriteLine($"Une erreur non prévue s'est produite lors du split de la video :\n{e.Message}");
                            throw;
                        }
                    }
                    ConsoleProgress.Write("Compute new hash...");
                    string newVideoHash;
                    using (var fileStream = File.OpenRead(tempOutFilePath))
                    {
                        newVideoHash = ToHashString(murmur128.ComputeHash(fileStream));
                    }
                    ConsoleProgress.Write("Rename file with new hash...");
                    string newVideoFilePath = Path.Combine(BufferDir, $"{newVideoHash}{Path.GetExtension(tempOutFilePath)}");
                    File.Move(tempOutFilePath, newVideoFilePath);
                    ConsoleProgress.Write("Delete old file...");
                    File.Delete(tempInFilePath);
                    ConsoleProgress.Write("Copy new file to SFTP...");
                    string sftpOutFilePath = $"{SFTP_PublishedFilesDirectory}/{Path.GetFileName(newVideoFilePath)}";
                    _sftpClient.Connect();
                    if (!_sftpClient.Exists(sftpOutFilePath))
                    {
                        using (var tempFileStream = File.Open(newVideoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var sftpFileStream = _sftpClient.OpenWrite(sftpOutFilePath))
                        {
                            int readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            while (readBytes > 0)
                            {
                                sftpFileStream.Write(buffer, 0, readBytes);
                                readBytes = tempFileStream.Read(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    _sftpClient.Disconnect();
                    ConsoleProgress.Write("Update CutVideo hash and links into database...");
                    using (var sqlConn = new SqlConnection(GetConnectionString()))
                    {
                        ExecuteCommandFormat<int>(sqlConn, $"INSERT INTO [dbo].[CutVideo] ([Hash], [HashOriginalVideo], [Start], [End], [Watermark], [MinDuration], [Extension]) SELECT '{newVideoHash}', [HashOriginalVideo], [Start], [End], [Watermark], [MinDuration], '.mp4' FROM [dbo].[CutVideo] WHERE [Hash] = '{Hash}'; ");
                        ExecuteCommandFormat<int>(sqlConn, $"UPDATE [dbo].[PublishedAction] SET [CutVideoHash] = '{newVideoHash}' WHERE [CutVideoHash] = '{Hash}';");
                        ExecuteCommandFormat<int>(sqlConn, $"DELETE FROM [dbo].[CutVideo] WHERE [Hash] = '{Hash}';");
                    }
                    ConsoleProgress.Write("Delete old file from SFTP...");
                    _sftpClient.Connect();
                    _sftpClient.Delete(sftpInFilePath);
                    _sftpClient.Disconnect();
                    ConsoleProgress.Finish("OK");
                    managedFiles++;
                }
                Console.WriteLine("");
            }
            catch (Exception e)
            {
                Console.WriteLine("FAIL");
                Console.WriteLine(e.Message);

                Console.WriteLine("\nFinish.\nPress a key to exit...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nFinish.\nPress a key to exit...");
            Console.ReadKey();
        }

        public static string GetConnectionString()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder
            {
                DataSource = ConfigurationManager.AppSettings[DataSourceKey],
                InitialCatalog = ConfigurationManager.AppSettings[InitialCatalogKey],
                UserID = Const.DataBaseAdminUser,
                Password = ConnectionStringsSecurity.DecryptPassword(Const.DataBaseAdminCryptedPassword),
            };
            return connectionStringBuilder.ConnectionString;
        }

        public static void ExecuteScript(SqlConnection conn, string scriptName, bool useDatabase = false, string databaseName = Const.DataBaseName_v3)
        {
            Exception ex = null;
            try
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Reencode.Scripts.{scriptName}.sql");
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                string script;
                using (var reader = new StreamReader(stream))
                    script = reader.ReadToEnd();

                conn.Open();
                var cmd = new SqlCommand($"USE [{databaseName}];", conn);
                if (useDatabase)
                    cmd.ExecuteNonQuery();
                foreach (var query in script.Split(new[] { "\nGO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception inner)
                    {
                        throw new Exception($"SQL ERROR - {inner.Message}\n{query}", inner);
                    }
                }
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                try
                {
                    conn.Close();
                }
                catch
                {
                    Debug.WriteLine("Can't close connection.");
                }
            }

            if (ex != null)
                throw ex;
        }

        public static async Task<T> ExecuteCommandFormatAsync<T>(SqlConnection conn, string command, params object[] args)
        {
            T result;
            await conn.OpenAsync();
            var cmd = new SqlCommand(string.Format(command, args), conn);
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

        public static T ExecuteCommandFormat<T>(SqlConnection conn, string command, params object[] args)
        {
            T result;
            conn.Open();
            var cmd = new SqlCommand(string.Format(command, args), conn);
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

        public static string ToHashString(byte[] hash)
        {
            var builder = new StringBuilder();
            foreach (byte hashed in hash)
                builder.AppendFormat("{0:X2}", hashed);
            return builder.ToString();
        }

        static MediaInfo GetMediaInfo(FileInfo file)
        {
            MediaInfo result = null;
            string json = "";
            using (var process = new Process())
            {
                process.StartInfo.FileName = ffprobePath;
                process.StartInfo.Arguments = $"-v quiet -print_format json -show_format -show_streams \"{file.FullName}\"";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;

                process.OutputDataReceived += (s, e) =>
                {
                    json += e.Data;
                };

                try
                {
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
                catch (Exception e)
                {
                    throw new Exception("An issue has occured during video packaging. It seems that some components are missing.", e);
                }
            }
            using (var fileStream = File.Open(@"C:\Utils\fileInfos.json", FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(json);
            }
            JObject jsonRoot = JObject.Parse(json);
            if (jsonRoot.ContainsKey("streams"))
            {
                result = new MediaInfo();
                var streams = jsonRoot["streams"];
                var audio = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "audio");
                var video = streams.FirstOrDefault(_ => (string)_.SelectToken("codec_type") == "video");
                if (audio != null)
                {
                    result.HasAudio = true;
                    if (int.TryParse((string)audio.SelectToken("bit_rate"), out int audioBitrate))
                        result.AudioBitrate = audioBitrate;
                    result.AudioCodec = (string)audio.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string) audio.SelectToken("duration")))
                    {
                        if (double.TryParse((string)audio.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                        else
                            Log.WriteLine($"ERROR : Can't parse audio duration from '{(string)audio.SelectToken("duration")}'");
                    }
                }
                if (video != null)
                {
                    result.HasVideo = true;
                    if (int.TryParse((string)video.SelectToken("bit_rate"), out int videoBitrate))
                        result.VideoBitrate = videoBitrate;
                    result.VideoCodec = (string)video.SelectToken("codec_name");
                    if (!string.IsNullOrEmpty((string)video.SelectToken("duration")))
                    {
                        if (double.TryParse((string)video.SelectToken("duration"), NumberStyles.Any, CultureInfo.InvariantCulture, out double durationValue))
                            result.Duration = TimeSpan.FromSeconds(durationValue);
                        else
                            Log.WriteLine($"ERROR : Can't parse video duration from '{(string)video.SelectToken("duration")}'");
                    }
                    var rotate = (string)video["tags"]?.SelectToken("rotate");
                    double? NullableRotateValue = null;
                    if (!string.IsNullOrEmpty(rotate))
                    {
                        if (double.TryParse(rotate, out double rotateValue))
                            NullableRotateValue = rotateValue;
                        else
                            Log.WriteLine($"ERROR : Can't parse double from '{rotate}'");
                    }
                    bool invertWidthHeight = NullableRotateValue.HasValue && NullableRotateValue != 0 && NullableRotateValue.Value % 90 == 0;
                    result.Width = invertWidthHeight ? int.Parse((string)video.SelectToken("height")) : int.Parse((string)video.SelectToken("width"));
                    result.Height = invertWidthHeight ? int.Parse((string)video.SelectToken("width")) : int.Parse((string)video.SelectToken("height"));
                }
            }

            return result;
        }
    }
}
