using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace Kprocess.KL2.Models
{
    public static class ConnectionStringsSecurity
    {
        /// <summary>
        /// Obtient la chaîne de connection avec un mot de passe correct.
        /// </summary>
        /// <param name="connectionStringName">Le nom de la chaîne de connexion.</param>
        /// <returns>La connexion à utiliser.</returns>
        public static EntityConnection GetConnectionString(string connectionStringName)
        {
            EntityConnection connection = new EntityConnection(connectionStringName);

            SqlConnectionStringBuilder sqlBuild = new SqlConnectionStringBuilder(connection.StoreConnection.ConnectionString);
            sqlBuild.Password = KProcess.KL2.ConnectionSecurity.ConnectionStringsSecurity.DecryptPassword(sqlBuild.Password);
            connection.StoreConnection.ConnectionString = sqlBuild.ConnectionString;

            return connection;
        }
    }
}
