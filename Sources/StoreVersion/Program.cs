using PowerArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace StoreVersion
{
    class Program
    {
        static int Main(string[] args)
        {
#if DEBUG
            var parsed = new CustomArgs
            {
                Version = new Version("4.0.0.5000"),
                Branch = "develop"
            };
#else
            var parsed = Args.Parse<CustomArgs>(args);
#endif

#if DEBUG
            var workingPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", "..", ".."));
#else
            var workingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif

            var versionFiles = new Dictionary<string, string>
            {
                ["KL2 Suite"] = Path.Combine(workingPath, "KL2-Core", "Versions.xml"),
                ["Field Services"] = Path.Combine(workingPath, "KL2-Core", "Kprocess.KL2.TabletClient", "Properties", "Versions.xml"),
                ["Video Analyst"] = Path.Combine(workingPath, "KL2-Core", "KProcess.Ksmed.Presentation.Shell", "Properties", "Versions.xml"),
                ["API"] = Path.Combine(workingPath, "KL2-Core", "KProcess.KL2.API", "Properties", "Versions.xml"),
                ["File Server"] = Path.Combine(workingPath, "KL2-Core", "Kprocess.KL2.FileServer", "Properties", "Versions.xml"),
                ["Notification Service"] = Path.Combine(workingPath, "KProcess.KL2.Notification", "Properties", "Versions.xml"),
                ["WebAdmin"] = Path.Combine(workingPath, "KProcess.KL2.WebAdmin", "Properties", "Versions.xml")
            };

            try
            {
                Console.WriteLine("-- Store version from Versions.xml to database --");
                foreach (var versionFile in versionFiles)
                {
                    Console.WriteLine($"Store GUID for version {parsed.Version} and file '{versionFile.Value}' in the database");
                    var doc = XDocument.Load(versionFile.Value);
                    var elt = doc.Root.Elements("version").Single(x => Version.Parse(x.Attribute("Id").Value) == parsed.Version);
                    var guid = Guid.Parse(elt.Attribute("Guid").Value);
                    using (var sqlConn = new System.Data.SqlClient.SqlConnection("Server=localhost; Initial Catalog=TeamCity; User ID=teamCityAgent; Password=K-process;"))
                    {
                        sqlConn.Open();
                        var sqlcmd = sqlConn.CreateCommand();
                        sqlcmd.CommandText = $"SELECT * FROM kl2GuidHistory WHERE Component=N'{versionFile.Key}' AND Version=N'{parsed.Version}'";
                        using (var dataReader = sqlcmd.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                sqlcmd.CommandText = $"UPDATE kl2GuidHistory SET Component='{versionFile.Key}', Version='{parsed.Version}', GUID='{guid}'";
                            }
                            else
                            {
                                sqlcmd.CommandText = $"INSERT INTO kl2GuidHistory (Component, Version, GUID) VALUES('{versionFile.Key}', '{parsed.Version}', '{guid}')";
                            }
                        }
                        sqlcmd.ExecuteNonQuery();
                    }
                }

                Console.WriteLine("-- Store version to database --");
                using (var sqlConn = new System.Data.SqlClient.SqlConnection("Server=localhost; Initial Catalog=TeamCity; User ID=teamCityAgent; Password=K-process;"))
                {
                    sqlConn.Open();
                    var sqlcmd = sqlConn.CreateCommand();
                    sqlcmd.CommandText = $"UPDATE kl2versioning SET Major={parsed.Version.Major}, Minor={parsed.Version.Minor}, Patch={parsed.Version.Build}, Revision={parsed.Version.Revision} WHERE Branch = N'{parsed.Branch.Replace("refs/heads/", "")}'";
                    sqlcmd.ExecuteNonQuery();
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
        public Version Version { get; set; }

        [ArgRequired]
        public string Branch { get; set; }
    }
}
