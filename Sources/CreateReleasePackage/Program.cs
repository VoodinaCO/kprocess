using PowerArgs;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace CreateReleasePackage
{
    class Program
    {
        static int Main(string[] args)
        {
            var parsed = Args.Parse<CustomArgs>(args);

            var workingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var releasePath = Path.Combine(workingPath, "..", "Release");

            Console.WriteLine("Remove all release files...");
            if (Directory.Exists(releasePath))
                Directory.Delete(releasePath, true);

            try
            {
                Console.WriteLine("Create release folders...");
                Directory.CreateDirectory(Path.Combine(releasePath, "Clients"));
                Directory.CreateDirectory(Path.Combine(releasePath, "Server", "Scripts"));
                Directory.CreateDirectory(Path.Combine(workingPath, "..", "ReleasePackages"));

                Console.WriteLine("Copy release files...");
                File.Copy(Path.Combine(workingPath, "KProcess.KL2.Setup", "KL²VideoAnalyst_Setup.exe"),
                    Path.Combine(releasePath, "Clients", "KL²VideoAnalyst_Setup.exe"));
                File.Copy(Path.Combine(workingPath, "KProcess.KL2.Tablet.Setup", "KL²FieldServices_Setup.exe"),
                    Path.Combine(releasePath, "Clients", "KL²FieldServices_Setup.exe"));
                File.Copy(Path.Combine(workingPath, "KProcess.KL2.Server.Setup", "KL²Server_Setup.exe"),
                    Path.Combine(releasePath, "Server", "KL²Server_Setup.exe"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "InitialTables.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "1 - InitialTables.sql"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "StoredProcedures.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "2 - StoredProcedures.sql"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "InitialData.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "3 - InitialData.sql"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "InitialData_en-US.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "4 - InitialData_en - US.sql"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "InitialData_fr-FR.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "4 - InitialData_fr - FR.sql"));
                File.Copy(Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.Database", "Scripts", "RestoreUserRights.sql"),
                    Path.Combine(releasePath, "Server", "Scripts", "5 - RestoreUserRights.sql"));

                Console.WriteLine("Compress release files...");
                var releasePackageName = Path.Combine(workingPath, "..", "ReleasePackages", $"KL2_Suite_v{parsed.Version}.zip");
                if (File.Exists(releasePackageName))
                    File.Delete(releasePackageName);
                while (File.Exists(releasePackageName))
                    Thread.Sleep(500);
                ZipFile.CreateFromDirectory(releasePath, releasePackageName);
            }
            catch (Exception ex)
            {
                var currentException = ex;
                while (currentException != null)
                {
                    Console.WriteLine(currentException.Message);
                    currentException = currentException.InnerException;
                }
                return -1;
            }

            return 0;
        }

        public class CustomArgs
        {
            [ArgRequired]
            public Version Version { get; set; }
        }
    }
}
