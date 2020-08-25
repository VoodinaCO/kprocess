using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CheckServerConfig
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
                Console.WriteLine("**** API ****");
                var doc = XDocument.Load(@"C:\K-process\KL² Suite\API\KProcess.KL2.API.exe.config");
                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                Console.WriteLine($"DataSource : {elt.Attribute("connectionString").Value.Substring(elt.Attribute("connectionString").Value.IndexOf("Data Source")).Split(';').Single(_ => _.StartsWith("Data Source")).Split('=')[1]}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                Console.WriteLine($"Application Url : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                Console.WriteLine($"File Server Location : {elt.Attribute("value").Value}");

                Console.WriteLine("**** FILE SERVER ****");
                doc = XDocument.Load(@"C:\K-process\KL² Suite\FileServer\Kprocess.KL2.FileServer.exe.config");
                elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                Console.WriteLine($"DataSource : {elt.Attribute("connectionString").Value.Substring(elt.Attribute("connectionString").Value.IndexOf("Data Source")).Split(';').Single(_ => _.StartsWith("Data Source")).Split('=')[1]}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                Console.WriteLine($"Application Url : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileProvider");
                var fileProvider = elt.Attribute("value").Value;
                Console.WriteLine($"File Provider : {fileProvider}");
                if (fileProvider == "SFtp")
                {
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Server");
                    Console.WriteLine($"Sftp Server : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Port");
                    Console.WriteLine($"Sftp Port : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_User");
                    Console.WriteLine($"Sftp User : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_Password");
                    Console.WriteLine($"Sftp Password : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_PublishedFilesDirectory");
                    Console.WriteLine($"Sftp PublishedFiles Path : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SFtp_UploadedFilesDirectory");
                    Console.WriteLine($"Sftp UploadedFiles Path : {elt.Attribute("value").Value}");
                }
                else if (fileProvider == "Local")
                {
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_PublishedFilesDirectory");
                    Console.WriteLine($"Local PublishedFiles Path : {elt.Attribute("value").Value}");
                    elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "Local_UploadedFilesDirectory");
                    Console.WriteLine($"Local UploadedFiles Path : {elt.Attribute("value").Value}");
                }

                Console.WriteLine("**** NOTIFICATION ****");
                doc = XDocument.Load(@"C:\K-process\KL² Suite\Notification\KProcess.KL2.Notification.exe.config");
                elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                Console.WriteLine($"DataSource : {elt.Attribute("connectionString").Value.Substring(elt.Attribute("connectionString").Value.IndexOf("Data Source")).Split(';').Single(_ => _.StartsWith("Data Source")).Split('=')[1]}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                Console.WriteLine($"Application Url : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                Console.WriteLine($"File Server Location : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "SendNotificationInterval");
                Console.WriteLine($"Notification Interval Checking : {elt.Attribute("value").Value}");

                Console.WriteLine("**** WEB ****");
                doc = XDocument.Load(@"C:\inetpub\KL2 Web Services\Web.config");
                elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                Console.WriteLine($"DataSource : {elt.Attribute("connectionString").Value.Substring(elt.Attribute("connectionString").Value.IndexOf("Data Source")).Split(';').Single(_ => _.StartsWith("Data Source")).Split('=')[1]}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApiServerUri");
                Console.WriteLine($"API Server Location : {elt.Attribute("value").Value}");
                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                Console.WriteLine($"File Server Location : {elt.Attribute("value").Value}");

                Console.WriteLine("**** API FILES INTEGRITY ****");
                CheckIntegrity("CheckServerConfig.API_FileList.txt", @"C:\K-process\KL² Suite\API");

                Console.WriteLine("**** FILE SERVER FILES INTEGRITY ****");
                CheckIntegrity("CheckServerConfig.FileServer_FileList.txt", @"C:\K-process\KL² Suite\FileServer");

                Console.WriteLine("**** NOTIFICATION FILES INTEGRITY ****");
                CheckIntegrity("CheckServerConfig.Notification_FileList.txt", @"C:\K-process\KL² Suite\Notification");

                Console.WriteLine("**** WEB FILES INTEGRITY ****");
                CheckIntegrity("CheckServerConfig.WebAdmin_FileList.txt", @"C:\inetpub\KL2 Web Services");

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
