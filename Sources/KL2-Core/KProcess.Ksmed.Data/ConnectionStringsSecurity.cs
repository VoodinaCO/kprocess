using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;

namespace KProcess.Ksmed.Data
{
    public static class ConnectionStringsSecurity
    {
        const string providerConst = "provider connection string=";

        /// <summary>
        /// Obtient la chaîne de connection avec un mot de passe correct.
        /// </summary>
        /// <param name="connectionStringName">Le nom de la chaîne de connexion.</param>
        /// <returns>La connexion à utiliser.</returns>
        public static EntityConnection GetConnectionString(ITraceManager traceManager)
        {
            var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var doc = XDocument.Load(configFile);
            var elt = doc.Root.Element("connectionStrings").Elements("add").Single(x => x.Attribute("name").Value == "KsmedEntities");
            var connectionString = elt.Attribute("connectionString").Value;
            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = connectionString.Substring(connectionString.IndexOf(providerConst) + providerConst.Length).Trim('\"'),
                Metadata = @"res://KProcess.Ksmed.Data/KL2.csdl|
                            res://KProcess.Ksmed.Data/KL2.ssdl|
                            res://KProcess.Ksmed.Data/KL2.msl"
            };
            var connection = new EntityConnection(entityBuilder.ToString());

            var sqlBuild = new SqlConnectionStringBuilder(connection.StoreConnection.ConnectionString);
            sqlBuild.Password = KL2.ConnectionSecurity.ConnectionStringsSecurity.DecryptPassword(sqlBuild.Password);
            connection.StoreConnection.ConnectionString = sqlBuild.ConnectionString;

            return connection;
        }
    }
}
