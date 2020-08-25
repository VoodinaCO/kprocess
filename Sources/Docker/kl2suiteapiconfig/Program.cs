using PowerArgs;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace kl2suiteapiconfig
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parsed = Args.Parse<CustomArgs>(args);

                var configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), parsed.ConfigFile);
                var doc = XDocument.Load(configFile);

                var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
                elt.Attribute("connectionString").SetValue(System.Net.WebUtility.HtmlDecode($@"metadata=          res://KProcess.Ksmed.Data/KL2.csdl|          res://KProcess.Ksmed.Data/KL2.ssdl|          res://KProcess.Ksmed.Data/KL2.msl;          provider=System.Data.SqlClient;          provider connection string=&quot;Data Source={parsed.DataSource};Initial Catalog={parsed.Database};User ID=KL2User;Password=B4A60FC44BCBEDC6267B80118219D235;MultipleActiveResultSets=True&quot;"));

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "ApplicationUrl");
                elt.Attribute("value").SetValue($"{parsed.ApplicationScheme}://*:{parsed.ApplicationPort}");

                elt = doc.Root.Element("appSettings").Elements("add").Single(x => x.Attribute("key").Value == "FileServerUri");
                elt.Attribute("value").SetValue($"{parsed.FileServerScheme}://{parsed.FileServer}:{parsed.FileServerPort}");

                doc.Save(configFile);
            }
            catch (Exception e)
            {
                var currentException = e;
                while (currentException != null)
                {
                    Console.WriteLine(currentException.Message);
                    currentException = currentException.InnerException;
                }
            }
        }
    }

    public class CustomArgs
    {
        [ArgRequired]
        public string ConfigFile { get; set; }

        [ArgRequired]
        public string DataSource { get; set; }

        [ArgRequired]
        public string Database { get; set; }

        [ArgRequired]
        public string ApplicationScheme { get; set; }

        [ArgRequired]
        public int ApplicationPort { get; set; }

        [ArgRequired]
        public string FileServerScheme { get; set; }

        [ArgRequired]
        public string FileServer { get; set; }

        [ArgRequired]
        public int FileServerPort { get; set; }
    }
}
