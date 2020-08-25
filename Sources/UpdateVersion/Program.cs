using PowerArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UpdateVersion
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            var parsed = new CustomArgs
            {
                TextTransform = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TextTransform.exe"),
                Version = new Version("4.0.0.5000")
            };
#else
            var parsed = Args.Parse<CustomArgs>(args);
#endif

#if DEBUG
            var workingPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", ".."));
#else
            var workingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

            var ttFiles = new Dictionary<string, string>
            {
                ["KL2 Suite"] = Path.Combine(workingPath, "KL2-Core", "Version.tt"),
                ["Field Services"] = Path.Combine(workingPath, "KL2-Core", "Kprocess.KL2.TabletClient", "Properties", "Version.tt"),
                ["Video Analyst"] = Path.Combine(workingPath, "KL2-Core" , "KProcess.Ksmed.Presentation.Shell" , "Properties", "Version.tt"),
                ["API"] = Path.Combine(workingPath, "KL2-Core" , "KProcess.KL2.API" , "Properties", "Version.tt"),
                ["File Server"] = Path.Combine(workingPath, "KL2-Core" , "Kprocess.KL2.FileServer" , "Properties", "Version.tt"),
                ["Notification Service"] = Path.Combine(workingPath, "KProcess.KL2.Notification" , "Properties", "Version.tt"),
                ["WebAdmin"] = Path.Combine(workingPath, "KProcess.KL2.WebAdmin" , "Properties", "Version.tt")
            };

            var regexString = "(string versionNumber = \"\\w+.\\w+.\\w+.\\w+\";)";

            try
            {
                Console.WriteLine("-- Set version in Version.tt files --");
                foreach (var ttFile in ttFiles)
                {
                    Console.WriteLine($"Set version in '{ttFile.Value}'");
                    File.WriteAllText(ttFile.Value,
                        Regex.Replace(File.ReadAllText(ttFile.Value), regexString, $"string versionNumber = \"{ parsed.Version }\";"));
                    Console.WriteLine($"Run tt engine...");
                    var proc = new Process
                    {
                        StartInfo =
                        {
                            FileName = parsed.TextTransform,
                            Arguments = $"\"{ttFile.Value}\""
                        }
                    };
                    proc.Start();
                    proc.WaitForExit();
                    if (proc.ExitCode != 0)
                        throw new Exception("FAILED!!!");                    
                }
            }
            catch (Exception ex)
            {
                var currentException = ex;
                while (currentException != null)
                {
                    Console.WriteLine(currentException.Message);
                    currentException = currentException.InnerException;
                }
                return 1;
            }

            return 0;
        }
    }

    public class CustomArgs
    {
        [ArgRequired]
        public string TextTransform { get; set; }

        [ArgRequired]
        public Version Version { get; set; }
    }
}
