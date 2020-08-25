using System;
using System.IO;
using System.Threading.Tasks;

namespace MigrationTool
{
    public static class Logger
    {
        const string logFile = "MigrationTool.log";

        public static void DeleteLogFile() =>
            File.Delete(logFile);

        public static void Write(string content, bool withTime = false)
        {
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                writer.Write($"{(withTime ? $"{DateTime.Now} : " : string.Empty)}{content}");
            }
        }

        public static async Task WriteAsync(string content, bool withTime = false)
        {
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                await writer.WriteAsync($"{(withTime ? $"{DateTime.Now} : " : string.Empty)}{content}");
            }
        }

        public static void WriteLine(string content, bool withTime = false)
        {
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                writer.WriteLine($"{(withTime ? $"{DateTime.Now} : " : string.Empty)}{content}");
            }
        }

        public static async Task WriteLineAsync(string content, bool withTime = false)
        {
            using (StreamWriter writer = new StreamWriter(logFile, true))
            {
                await writer.WriteLineAsync($"{(withTime ? $"{DateTime.Now} : " : string.Empty)}{content}");
            }
        }
    }
}
