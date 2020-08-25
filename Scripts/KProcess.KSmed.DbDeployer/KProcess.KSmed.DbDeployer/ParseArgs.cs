using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace KProcess.KSmed.DbDeployer
{
    public static class ParseArgs
    {
        #region Constants

        private const string PropertiesElement = "Properties";
        private const string PropertyElement = "Property";
        private const string PropertyNameElement = "PropertyName";
        private const string PropertyValueElement = "PropertyValue";

        private const string SqlFilePathParameter = "SqlFilePath";
        private const string DefaultDataPathParameter = "DefaultDataPath";
        private const string DefaultLogPathParameter = "DefaultLogPath";
        private const string DatabaseNameParameter = "DatabaseName";
        private const string SqlvarsFilePathParameter = "SqlvarsFilePath";
        private const string LoginParameter = "U";
        private const string PasswordParameter = "P";
        private const string ServerParameter = "S";

        #endregion Constants

        public static int Parse(string[] args, Parameters parameters)
        {
            Arguments arguments = new Arguments(args);
            string sqlFilePath = arguments[SqlFilePathParameter];
            string sqlvarsFilePath = arguments[SqlvarsFilePathParameter];
            string defaultDataPath = arguments[DefaultDataPathParameter];
            string defaultLogPath = arguments[DefaultLogPathParameter];
            string databaseName = arguments[DatabaseNameParameter];
            string serverName = arguments[ServerParameter];
            string passwordParameter = arguments[PasswordParameter];
            string loginParameter = arguments[LoginParameter];

            if (String.IsNullOrEmpty(sqlFilePath))
                return 0;
            else
                parameters.SqlFilePath = sqlFilePath;

            ParseArgs.FillDefaultValues(parameters);

            if (!String.IsNullOrEmpty(defaultDataPath))
                parameters.DataPath = defaultDataPath;
            if (!String.IsNullOrEmpty(defaultLogPath))
                parameters.LogPath = defaultLogPath;
            if (!String.IsNullOrEmpty(databaseName))
                parameters.DatabaseName = databaseName;

            parameters.ServerName = serverName;
            parameters.UserLogin = loginParameter;
            parameters.UserPassword = passwordParameter;

            if (!String.IsNullOrEmpty(sqlvarsFilePath))
                ParseArgs.GetCustomParameterValues(parameters, sqlvarsFilePath);

            foreach (var param in arguments.OverridedParameters)
            {
                if (parameters.CustomsParameters.ContainsKey(param.Key))
                    parameters.CustomsParameters[param.Key] = String.Format("\"{0}\"", param.Value);
                else
                    parameters.CustomsParameters.Add(param.Key, String.Format("\"{0}\"", param.Value));
            }

            return 1;
        }

        public static void ShowHelp()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("________");
            Console.WriteLine();
            Console.WriteLine("KProcess.KSmed.DbDeployer.exe /{0}:[Path]", ParseArgs.SqlFilePathParameter);
            Console.WriteLine("Required arguments");
            Console.WriteLine("/{0} to specify the file path to the generated sql file", ParseArgs.SqlFilePathParameter);
            Console.WriteLine();
            Console.WriteLine("Default arguments:");
            Console.WriteLine("/{0} to define where database file are created by default", ParseArgs.DefaultDataPathParameter);
            Console.WriteLine("/{0} to define where database log file are created by default", ParseArgs.DefaultLogPathParameter);
            Console.WriteLine("/{0} to define the database name (default KProcess.Ksmed)", ParseArgs.DatabaseNameParameter);
            Console.WriteLine();
            Console.WriteLine("Connexion arguments");
            Console.WriteLine("/{0} to specify user name", ParseArgs.LoginParameter);
            Console.WriteLine("/{0} to specify user password", ParseArgs.PasswordParameter);
            Console.WriteLine("/{0} to specify server name and/or instance name", ParseArgs.ServerParameter);
            Console.WriteLine();
            Console.WriteLine("To load Sql Vars from file use /{0}:[Path]", ParseArgs.SqlvarsFilePathParameter);
            Console.WriteLine();
            Console.WriteLine("Overridable parameters");
            Console.WriteLine("/p:ParameterName=ParameterValue to specify that the ParameterName has the specify ParameterValue");
            Console.WriteLine("Use a new /p: switch foreach parameter to override");
            Console.WriteLine();
            Console.WriteLine("Exemple: KProcess.KSmed.DbDeployer.exe /{0}:[Path] /{1}:[Path] /p:LoadSampleData=1", ParseArgs.SqlFilePathParameter, ParseArgs.SqlvarsFilePathParameter);
        }

        public static void FillDefaultValues(Parameters parameters)
        {
            parameters.DatabaseName = "KProcess.Ksmed";
            parameters.DataPath = "\"C:\\Program Files\\Microsoft SQL Server\\MSSQL10_50.MSSQLSERVER\\MSSQL\\DATA\\\"";
            parameters.LogPath = "\"C:\\Program Files\\Microsoft SQL Server\\MSSQL10_50.MSSQLSERVER\\MSSQL\\DATA\\\"";
        }

        /// <summary>
        /// Gets the custom parameter values.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="fileUri">The file URI.</param>
        public static Parameters GetCustomParameterValues(Parameters parameters, string fileUri)
        {
            XDocument xDoc = XDocument.Load(fileUri);

            //var result = xDoc.Root.Attributes().
            //        Where(a => a.IsNamespaceDeclaration).
            //        GroupBy(a => a.Name.Namespace == XNamespace.None ? String.Empty : a.Name.LocalName,
            //                a => XNamespace.Get(a.Value)).
            //        ToDictionary(g => g.Key,
            //                     g => g.First());

            IEnumerable<XElement> props = xDoc.Descendants().Where(xe => xe.Name.LocalName == ParseArgs.PropertyElement);
            
            foreach (XElement prop in props)
            {
                parameters.CustomsParameters.Add(
                    prop.Elements().Where(xe => xe.Name.LocalName == ParseArgs.PropertyNameElement).Single().Value,
                    String.Format("\"{0}\"", prop.Elements().Where(xe => xe.Name.LocalName == ParseArgs.PropertyValueElement).Single().Value));
            }

            return parameters;
        }
    }
}
