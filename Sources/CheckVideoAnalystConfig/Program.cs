using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CheckVideoAnalystConfig
{
    class Program
    {
        static void CheckIntegrity(string integrityFileName, string rootPath)
        {
            var resourcesList = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();
            if (!resourcesList.Contains(integrityFileName))
            {
                Console.WriteLine($"Can't find resource '{integrityFileName}'");
                return;
            }
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(integrityFileName))
            using (var reader = new StreamReader(stream))
            {
                bool fileMisses = false;
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var file = Path.Combine(rootPath, line);
                    if (!File.Exists(file))
                    {
                        fileMisses = true;
                        Console.WriteLine($"Can't find file '{file}'");
                    }
                }
                if (fileMisses == false)
                    Console.WriteLine("All files are present.");
            }
        }

        static void Main(string[] args)
        {
            try
            {
                // Get installation folder from registry
                var installPath = Registry.LocalMachine.OpenSubKey("SOFTWARE\\K-process\\KL²VideoAnalyst\\Install").GetValue("InstallLocation").ToString();

                Console.WriteLine("**** VIDEO ANALYST ****");
                var doc = XDocument.Load(Path.Combine(installPath, "KL².exe.config"));
                var elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                Console.WriteLine($"API Server Location : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                Console.WriteLine($"File Server Location : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SyncPath");
                Console.WriteLine($"Sync Path : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("userSettings").Element("KProcess.Ksmed.Presentation.Shell.Settings").Elements("setting").Single(x => x.Attribute("name").Value == "SendReport");
                Console.WriteLine($"Send Report : {elt.Element("value").Value}");

                Console.WriteLine("**** FILES INTEGRITY ****");
                CheckIntegrity("CheckVideoAnalystConfig.VideoAnalyst_FileList.txt", installPath);

                Console.WriteLine("Check all infos and press a key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                var currentException = ex;
                do
                {
                    Console.WriteLine(ex.Message);
                    currentException = currentException.InnerException;
                } while (currentException != null);
                Console.WriteLine("ERROR, press a key to exit...");
                Console.ReadKey();
            }
        }
    }
}
