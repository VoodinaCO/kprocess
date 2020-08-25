using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using KProcess.KL2.ConnectionSecurity;

namespace ResExport
{
    class Program
    {
        private static CultureInfo _fr = CultureInfo.GetCultureInfo("fr-FR");
        private static CultureInfo _en = CultureInfo.GetCultureInfo("en-US");

        private static CultureInfo[] _appCultures = new CultureInfo[]
        {
            _fr,
            _en,
            CultureInfo.GetCultureInfo("pt-BR"),
            CultureInfo.GetCultureInfo("es-ES"),
            CultureInfo.GetCultureInfo("de-DE"),
            CultureInfo.GetCultureInfo("pl-PL")
        };

        private static CultureInfo[] _installerCultures = new CultureInfo[]
        {
            _fr,
            _en,
        };

        private const string xmlNamespace = "http://www.kp.com";
        private const string xsi = "http://www.w3.org/2001/XMLSchema-instance";

        private const string resourceNames = "LocalizationResources";

        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine("[1] Exporter les ressources");
            Console.WriteLine("[2] Importer les ressources");
            Console.WriteLine(@"[3] Vérifier que les trads en base contiennent bien les mêmes éléments ({*}) ou le même nombre d'éléments (\r, \n, \t) dans toutes les langues");
            Console.WriteLine("[4] Comparer les ressources de deux BDD et générer un script d'update");

            var read = Console.ReadLine();

            if (read.StartsWith("1"))
            {
                Export();
            }
            else if (read.StartsWith("2"))
            {
                Import();
            }
            else if (read.StartsWith("3"))
            {
                CheckValues();
            }
            else if (read.StartsWith("4"))
            {
                CompareDatabases();
            }
        }

        #region EXPORT

        private static void Export()
        {
            ExportApplicationDB();
            ExportApplicationRes();
            ExportExtWeber();
            ExportExtPG();
            ExportBootstrapper();
            ExportInstallerRes();
            ExportInstallerErrors();
            FixUpMissingValues();
            WriteResources();
        }

        private static void ExportApplicationDB(string connectionString = "db")
        {
            _resources.Clear();
            var cs = ConfigurationManager.ConnectionStrings[connectionString];
            var csb = new SqlConnectionStringBuilder(cs.ConnectionString);
            csb.Password = ConnectionStringsSecurity.DecryptPassword(csb.Password);
            using (var connection = new SqlConnection(csb.ToString()))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    string query = @"SELECT k.ResourceKey, LanguageCode, v.Value, v.Comment 
FROM AppResourceValue v LEFT JOIN AppResourceKey k ON v.ResourceId = k.ResourceId WHERE LanguageCode IN ({0})";

                    var whereQUery = new StringBuilder();
                    foreach (var culture in _appCultures)
                    {
                        whereQUery.Append("'");
                        whereQUery.Append(culture.ToString());
                        whereQUery.Append("',");
                    }
                    whereQUery.Remove(whereQUery.Length - 1, 1);

                    query = string.Format(query, whereQUery.ToString());

                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AddResource(
                                Source.ApplicationDB,
                                reader.GetString(0),
                                CultureInfo.GetCultureInfo(reader.GetString(1)),
                                reader.GetString(2),
                                reader.IsDBNull(3) ? null : reader.GetString(3)
                                );
                        }
                    }
                }
            }



        }

        private static void ExportApplicationRes()
        {
            foreach (var culture in _appCultures)
            {
                string filePath = string.Format(@"..\..\..\KProcess.Ksmed.Presentation.Shell\Resources\{0}.{1}.resx",
                    resourceNames, culture.ToString());

                ExportResx(filePath, Source.ApplicationRes, culture);
            }
        }

        private static void ExportExtWeber()
        {
            string filePath = @"..\..\..\Ext\Weber\KProcess.Ksmed.Ext.Weber\Resources\Localization.resx";

            ExportResx(filePath, Source.ExtWeber, _fr);
        }

        private static void ExportExtPG()
        {
            string filePath = @"..\..\..\Ext\PG\KProcess.Ksmed.Ext.PG\Resources\Localization.resx";

            ExportResx(filePath, Source.ExtPG, _en);
        }

        private static void ExportBootstrapper()
        {
            string filePath = @"..\..\..\KProcess.Ksmed.Setup.Bootstrapper\configuration.xml";

            var xdoc = XDocument.Load(filePath);

            var root = xdoc.Element("configurations");

            foreach (var configuration in root.Elements("configuration"))
            {
                var language = CultureInfo.GetCultureInfo(int.Parse(configuration.Attribute("lcid_filter").Value));

                var attributes = new string[]
                {
                    "dialog_caption",
                    "dialog_message",
                    "skip_caption",
                    "install_caption",
                    "uninstall_caption",
                    "cancel_caption",
                    "status_installed",
                    "failed_exec_command_continue",
                    "installation_completed",
                    "uninstallation_completed",
                    "installation_none",
                    "uninstallation_none",
                    "installing_component_wait",
                    "uninstalling_component_wait",
                    "reboot_required",
                    "administrator_required_message",
                };

                foreach (var att in attributes)
                {
                    AddResource(Source.Bootstrapper, att, language, configuration.Attribute(att).Value, null);
                }
            }

        }

        private static void ExportInstallerRes()
        {
            foreach (var culture in _installerCultures)
            {
                string filepath = string.Format(@"..\..\..\KProcess.Ksmed.Setup\Loc\{0}.wxl", culture);

                if (File.Exists(filepath))
                {
                    var ns = XNamespace.Get("http://schemas.microsoft.com/wix/2006/localization");

                    var doc = XDocument.Load(filepath);
                    var root = doc.Element(ns + "WixLocalization");
                    foreach (var element in root.Elements(ns + "String"))
                    {
                        string name = element.Attribute("Id").Value;
                        string value = element.Value;

                        if (value != null)
                        {
                            AddResource(Source.InstallerRes, name, culture, value, null);
                        }
                    }
                }
            }
        }

        private static void ExportInstallerErrors()
        {
            var mapping = new Dictionary<string, CultureInfo>
            {
                {"1", _fr},
                {"2", _en},
            };

            string filepath = string.Format(@"..\..\..\KProcess.Ksmed.Setup\UI\ErrorMessages.wxs");

            var ns = XNamespace.Get("http://schemas.microsoft.com/wix/2006/wi");

            var doc = XDocument.Load(filepath);
            var root = doc.Element(ns + "Wix");
            var ui = root.Element(ns + "Fragment").Element(ns + "UI");
            foreach (var element in ui.Elements(ns + "Error"))
            {
                string name = element.Attribute("Id").Value;

                var kvpp = mapping.First(kvp => name.StartsWith(kvp.Key));

                name = name.Substring(kvpp.Key.Length);

                string value = element.Value;

                AddResource(Source.InstallerErrors, name, kvpp.Value, value, null);
            }
        }

        private static void ExportResx(string filePath, Source source, CultureInfo culture)
        {
            if (File.Exists(filePath))
            {
                var xdoc = XDocument.Load(filePath);
                var root = xdoc.Element("root");
                var datas = root.Elements("data");

                foreach (var data in datas)
                {
                    var name = data.Attribute("name").Value;
                    var value = data.Element("value").Value;

                    string comment = null;
                    if (data.Element("comment") != null)
                        comment = data.Element("comment").Value;

                    AddResource(source, name, culture, value, comment);
                }
            }
        }

        private static List<Resource> _resources = new List<Resource>();
        private static void AddResource(Source source, string key, CultureInfo culture, string value, string comments)
        {
            var res = _resources.FirstOrDefault(r => r.Source == source && r.Key == key);
            if (res != null)
                res.Values[culture] = value;
            else
            {
                res = new Resource()
                {
                    Source = source,
                    Key = key,
                    Comments = comments,
                };
                res.Values = new Dictionary<CultureInfo, string>()
                {
                    { culture, value },
                };
                _resources.Add(res);
            }
        }

        private static void FixUpMissingValues()
        {
            foreach (var resource in _resources)
            {
                bool isInInstaller;
                switch (resource.Source)
                {
                    case Source.ApplicationDB:
                    case Source.ApplicationRes:
                    case Source.ExtWeber:
                    case Source.ExtPG:
                        isInInstaller = false;
                        break;
                    case Source.Bootstrapper:
                    case Source.InstallerRes:
                    case Source.InstallerErrors:
                        isInInstaller = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var cultures = isInInstaller ? _installerCultures : _appCultures;

                foreach (var culture in cultures)
                {
                    if (!resource.Values.ContainsKey(culture))
                        AddResource(resource.Source, resource.Key, culture, null, null);
                }
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            throw new ApplicationException("xml mal formé");
        }

        private static void WriteResources()
        {
            var doc = new XDocument();
            var ns = XNamespace.Get(xmlNamespace);
            var xsiNs = XNamespace.Get(xsi);
            //doc.xmlns

            var root = new XElement(ns + "resources",
                new XAttribute(XNamespace.Xmlns + "xsi", xsiNs),
                new XAttribute(xsiNs + "schemaLocation", "http://www.kp.com ExportRes.xsd"));

            doc.Add(root);

            foreach (var res in _resources)
            {
                var node = new XElement(ns + "resource");
                node.SetAttributeValue("key", res.Key);
                root.Add(node);

                var source = new XElement(ns + "source");
                source.SetValue(res.Source.ToString());
                node.Add(source);

                foreach (var kvp in res.Values.OrderBy(k => k.Key.ToString()))
                {
                    var value = new XElement(ns + kvp.Key.ToString());
                    if (kvp.Value != null)
                        value.SetValue(kvp.Value);
                    node.Add(value);
                }

                var comment = new XElement(ns + "comment");
                if (res.Comments != null)
                    comment.SetValue(res.Comments);
                node.Add(comment);
            }

            var schemas = new XmlSchemaSet();
            using (var xsd = File.OpenRead("ExportRes.xsd"))
            {
                schemas.Add("http://www.kp.com", XmlReader.Create(xsd));
            }
            doc.Save(string.Format("ExportRessources_{0}.xml",
                DateTimeOffset.Now.ToString("yyyyMMddHHmmss")));
            doc.Validate(schemas, ValidationEventHandler, true);

            Console.WriteLine("{0} ressources exportées", _resources.Count);
            Console.ReadLine();
        }

        #endregion

        #region Import

        private static void Import()
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var resources = ReadResources(dialog.FileName);

                GenerateApplicationDBScript(resources);
            }
        }

        private static void GenerateApplicationDBScript(Resource[] resources)
        {
            using (var file = File.CreateText(string.Format("AppResources_{0}.sql",
                DateTimeOffset.Now.ToString("yyyyMMddHHmmss"))))
            {

                foreach (var res in resources
                    .Where(r => r.Source == Source.ApplicationDB)
                    .OrderBy(r => r.Key)
                    )
                {

                    foreach (var value in res.Values)
                    {
                        //if (!string.IsNullOrEmpty(value.Value)) // Pour le mode ou on ne veut pas combler les manques
                        //{
                            file.WriteLine("EXEC #InsertResource '{0}', '{1}', N'{2}', {3};",
                                res.Key,
                                value.Key.ToString(),
                                FormatSqlString(value.Value, true),
                                !string.IsNullOrEmpty(res.Comments) ? string.Format("N'{0}'", FormatSqlString(res.Comments, true)) : "null"
                                );
                        //}
                    }


                }

                foreach (var culture in _appCultures)
                {
                    if (culture != _fr)
                    {

                        file.WriteLine(@"
INSERT INTO [dbo].[AppResourceValue]
		([ResourceId]
		,[LanguageCode]
		,[Value]
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate])
		
	SELECT [ResourceId]
		,'{0}'
		,[Value] + '¤'
		,[Comment]
		,[CreatedByUserId]
		,[CreationDate]
		,[ModifiedByUserId]
		,[LastModificationDate]
		 FROM [AppResourceValue] v
		 WHERE [LanguageCode] = 'fr-FR' AND
		 (SELECT COUNT([ResourceId]) FROM [AppResourceValue] WHERE [AppResourceValue].[ResourceId] = v.[ResourceId] AND [AppResourceValue].[LanguageCode] = '{0}') = 0;
                ", culture.Name);

                    }
                }

                file.WriteLine("DROP PROC #InsertResource");
            }
        }

        private static string FormatSqlString(string input, bool updateLineBreaks)
        {
            input = input.Replace("'", "''");
            if (updateLineBreaks)
                input = input.Replace("\n", "\r\n");
            return input;
        }

        private static Resource[] ReadResources(string filepath)
        {
            var doc = XDocument.Load(filepath);
            var ns = XNamespace.Get(xmlNamespace);
            var xsiNs = XNamespace.Get(xsi);

            var root = doc.Root;
            var resourcesNodes = root.Elements(ns + "resource");
            var resources = new List<Resource>();

            foreach (var resNode in resourcesNodes)
            {
                var res = new Resource();

                res.Key = resNode.Attribute("key").Value;

                var sourceNode = resNode.Element(ns + "source");

                res.Source = (Source)Enum.Parse(typeof(Source), sourceNode.Value);

                foreach (var culture in _appCultures)
                {
                    var valNode = resNode.Element(ns + culture.ToString());
                    if (valNode != null)
                    {
                        if (res.Values == null)
                            res.Values = new Dictionary<CultureInfo, string>();

                        res.Values[culture] = valNode.Value;
                    }
                }

                var commentNode = resNode.Element(ns + "comment");
                res.Comments = commentNode != null ? commentNode.Value : string.Empty;
                resources.Add(res);
            }

            return resources.ToArray();
        }

        #endregion

        #region Tests

        private static void CheckValues()
        {
            ExportApplicationDB();

            var patterns = new Regex[]
            {
                new Regex(@"\{.+?\}"),
                new Regex("\r"),
                new Regex("\n"),
                new Regex("\t"),
            };

            (string ResourceId, string Culture)[] exclusions =
            {
                ("View_ActivationView_ActivationExplanation", "de-DE"),
            };

            int i = 0;

            foreach (var resource in _resources)
            {

                foreach (var pattern in patterns)
                {
                    var occurences = new Dictionary<CultureInfo, Match[]>();

                    foreach (var culture in _appCultures)
                    {
                        var thisCultureValue = resource.Values[culture];
                        var matches = pattern.Matches(thisCultureValue);

                        if (!exclusions.Any(e => e.ResourceId == resource.Key && e.Culture == culture.IetfLanguageTag))
                        {
                            occurences[culture] = matches.Cast<Match>().ToArray();
                        }

                        i++;
                    }

                    var differentCount = occurences.Values.Select(m => m.Count()).Distinct().Count() != 1;
                    if (differentCount)
                        throw new ApplicationException(string.Format("Une erreur est présente pour la clé {0}", resource.Key));

                    var values = occurences.Values.Select(matches => matches.Select(m => m.Value));

                    if (!SequenceEqual(values))
                        throw new ApplicationException(string.Format("Une erreur est présente pour la clé {0}", resource.Key));
                }

            }

            WriteAndLog("{0} chaînes vérifiées et OK", i);

            WriteAndLog("Vérifications chaînes identiques", i);

            foreach (var resource in _resources)
            {
                var groups = resource.Values.GroupBy(v => v.Value);
                foreach (var group in groups)
                {
                    if (group.Count() > 1)
                    {
                        WriteAndLog("{0}", resource.Key);
                        foreach (var val in group)
                            WriteAndLog("  {0} // {1}", val.Key, val.Value);
                    }
                }
            }

            WriteAndLog("Vérifications présence commentaires", i);

            if (!_resources.Any(r => !string.IsNullOrEmpty(r.Comments)))
                throw new ApplicationException("Il manque les commentaires");

            WriteAndLog("Done", i);

            if (_fs != null)
            {
                _sw.Dispose();
                _fs.Dispose();
            }

            Console.ReadLine();
        }

        private const string LogFilePath = "check.log";
        private static FileStream _fs;
        private static StreamWriter _sw;

        private static bool SequenceEqual<T>(IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<T> pred = null;
            foreach (var s in sequences)
            {
                if (pred != null)
                {
                    if (!s.SequenceEqual(pred))
                        return false;
                }

                pred = s;
            }

            return true;
        }

        private static void WriteAndLog(string format, params object[] args)
        {
            WriteAndLog(string.Format(format, args));
        }

        private static void WriteAndLog(string line)
        {
            if (_fs == null)
            {
                _fs = File.Create(LogFilePath);
                _sw = new StreamWriter(_fs);
            }

            _sw.WriteLine(line);
            Console.WriteLine(line);
        }

        #endregion

        #region Compare

        private static string UpdateScriptUpdateResource = "EXEC #InsertOrUpdateResource '{0}', '{1}', N'{2}', {3};"; // Clé, Langue, Valeur, Commentaires
        private static string UpdateScriptDeleteResource = "EXEC #DeleteResource '{0}';"; // Clé

        private static void CompareDatabases()
        {
            var dbSource = ConfigurationManager.ConnectionStrings["dbUpdateSource"];
            var dbTarget = ConfigurationManager.ConnectionStrings["dbUpdateTarget"];

            var csSource = new SqlConnectionStringBuilder(dbSource.ConnectionString);
            var csTarget = new SqlConnectionStringBuilder(dbTarget.ConnectionString);

            Console.WriteLine("DB Source : DataSource = {0}, InitialCatalog = {1}", csSource.DataSource, csSource.InitialCatalog);
            Console.WriteLine("DB Target : DataSource = {0}, InitialCatalog = {1}", csTarget.DataSource, csTarget.InitialCatalog);

            // REset des droits
            var css = new SqlConnectionStringBuilder[] { csSource, csTarget };
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["master"].ConnectionString))
            {
                connection.Open();
                foreach (var cs in css)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = string.Format("USE [{0}]", cs.InitialCatalog);
                        command.ExecuteNonQuery();

                        try
                        {
                            command.CommandText = $"DROP USER [{Const.DataBaseAdminUser}]";
                            command.ExecuteNonQuery();
                        }
                        catch { }

                        try
                        {
                            command.CommandText = $"DROP USER [{Const.DataBaseUser}]";
                            command.ExecuteNonQuery();
                        }
                        catch { }

                        command.CommandText = $"CREATE USER [{Const.DataBaseAdminUser}] FOR LOGIN [{Const.DataBaseAdminUser}]";
                        command.ExecuteNonQuery();

                        command.CommandText = $"EXEC sp_addrolemember N'db_owner', N'{Const.DataBaseAdminUser}'";
                        command.ExecuteNonQuery();

                        command.CommandText = $"CREATE USER [{Const.DataBaseUser}] FOR LOGIN [{Const.DataBaseUser}]";
                        command.ExecuteNonQuery();

                        command.CommandText = $"EXEC sp_addrolemember N'db_owner', N'{Const.DataBaseUser}'";
                        command.ExecuteNonQuery();
                    }
                }
            }


            ExportApplicationDB("dbUpdateSource");
            var resourcesSource = _resources.ToArray();

            ExportApplicationDB("dbUpdateTarget");
            var resourcesTarget = _resources.ToArray();

            var updateScript = new StringBuilder();
            var comparer = new ResourceKeyComparer();

            // Ressources à supprimer
            var deletedResources = resourcesSource.Except(resourcesTarget, comparer).ToArray();
            foreach (var deletedResource in deletedResources)
            {
                updateScript.AppendLine(string.Format(UpdateScriptDeleteResource, deletedResource.Key));
                Console.WriteLine("A supprimer: {0}", deletedResource.Key);
            }


            // Nouvelles ressources
            var newResources = resourcesTarget.Except(resourcesSource, comparer).ToArray();
            foreach (var newResource in newResources)
            {
                foreach (var culture in _appCultures)
                    GenerateUpdateResource(updateScript, newResource, culture);

                Console.WriteLine("Nouveau: {0}", newResource.Key);
            }

            // Ressources mises à jour
            var otherResources = resourcesSource.Intersect(resourcesTarget, comparer).ToArray();
            foreach (var source in otherResources)
            {
                var target = resourcesTarget.First(r => r.Key == source.Key);

                if (source.Values.Count > target.Values.Count)
                {
                    // Perte d'une langue, impossible pour le moment
                    throw new InvalidOperationException(string.Format("La resource {0} a perdu une langue", source.Values.Count));
                }
                else if (source.Values.Count != target.Values.Count)
                {
                    // De fortes chances que ce soit une nouvelle langue

                    var newCultures = target.Values.Keys.Except(source.Values.Keys);
                    foreach (var newCulture in newCultures)
                    {
                        GenerateUpdateResource(updateScript, target, newCulture);
                        Console.WriteLine("Nouvelle Langue: {0} {1}", target.Key, newCulture);
                    }
                }

                if (source.Comments != target.Comments)
                {
                    foreach (var culture in _appCultures)
                        GenerateUpdateResource(updateScript, target, culture);

                    Console.WriteLine("Nouveaux commentaires (et potentiellement nouvelles trads): {0}", target.Key);
                }
                else
                {
                    foreach (var culture in _appCultures)
                    {
                        if (source.Values.ContainsKey(culture)) // Gestion d'un nouvelle langue
                        {
                            var rSource = source.Values[culture];
                            var rTarget = target.Values[culture];

                            if (string.Compare(rSource, rTarget) != 0)
                            {
                                GenerateUpdateResource(updateScript, target, culture);
                                Console.WriteLine("Nouvelles valeur: {0} {1}", source.Key, culture);
                                continue;
                            }
                        }
                    }
                }
            }

            if (updateScript.Length > 0)
                File.WriteAllText(string.Format("UpdateScript-{0}.sql", DateTimeOffset.Now.ToString("yyyyMMddHHmmss")), updateScript.ToString());

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static void GenerateUpdateResource(StringBuilder updateScript, Resource newResource, CultureInfo language)
        {
            updateScript.AppendLine(string.Format(UpdateScriptUpdateResource, newResource.Key, language,
                FormatSqlString(newResource.Values[language], false),
                !string.IsNullOrEmpty(newResource.Comments) ? string.Format("N'{0}'", FormatSqlString(newResource.Comments, false)) : "null"));
        }

        #endregion

        private class Resource
        {
            public Source Source { get; set; }
            public string Key { get; set; }
            public Dictionary<CultureInfo, string> Values { get; set; }
            public string Comments { get; set; }
        }

        private class ResourceKeyComparer : IEqualityComparer<Resource>
        {
            public bool Equals(Resource x, Resource y)
            {
                return string.Equals(x.Key, y.Key);
            }

            public int GetHashCode(Resource obj)
            {
                return obj.Key.GetHashCode();
            }
        }

        private enum Source
        {
            ApplicationDB,
            ApplicationRes,
            Bootstrapper,
            InstallerRes,
            InstallerErrors,
            ExtWeber,
            ExtPG
        }
    }
}
