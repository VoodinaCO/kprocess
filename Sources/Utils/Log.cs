using System.IO;

namespace Utils
{
    public static class Log
    {
        const string LogFilePath = @"C:\Utils\ftp_to_sftp.log";

        public static void Write(string format, params object[] args)
        {
            using (var logStream = File.Open(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(logStream))
            {
                writer.Write(string.Format(format, args));
            }
        }

        public static void WriteLine(string format, params object[] args)
        {
            using (var logStream = File.Open(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (StreamWriter writer = new StreamWriter(logStream))
            {
                writer.WriteLine(string.Format(format, args));
            }
        }
    }
}
