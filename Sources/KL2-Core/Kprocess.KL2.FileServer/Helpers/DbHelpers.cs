using KProcess.KL2.ConnectionSecurity;
using System;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;

namespace Kprocess.KL2.FileServer
{
    public class DbHelpers : IDisposable
    {
        readonly SqlConnection _connection;

        public DbHelpers()
        {
            _connection = new SqlConnection(GetConnectionString());
        }

        string GetConnectionString()
        {
            var connection = new EntityConnection("name=KsmedEntities");
            var sqlBuild = new SqlConnectionStringBuilder(connection.StoreConnection.ConnectionString);
            sqlBuild.Password = ConnectionStringsSecurity.DecryptPassword(sqlBuild.Password);
            return sqlBuild.ConnectionString;
        }

        public async Task<T> ExecuteCommandFormatAsync<T>(string command, params object[] args)
        {
            try
            {
                T result;
                await _connection.OpenAsync();
                SqlCommand cmd = new SqlCommand(string.Format(command, args), _connection);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)await cmd.ExecuteNonQueryAsync();
                    _connection.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)await cmd.ExecuteReaderAsync();
                }
                else if (typeof(T) == typeof(XmlReader))
                {
                    result = (T)(object)await cmd.ExecuteXmlReaderAsync();
                }
                else
                {
                    result = (T)await cmd.ExecuteScalarAsync();
                    _connection.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public T ExecuteCommandFormat<T>(string command, params object[] args)
        {
            try
            {
                T result;
                _connection.Open();
                SqlCommand cmd = new SqlCommand(string.Format(command, args), _connection);
                if (typeof(T) == typeof(int))
                {
                    result = (T)(object)cmd.ExecuteNonQuery();
                    _connection.Close();
                }
                else if (typeof(T) == typeof(SqlDataReader))
                {
                    result = (T)(object)cmd.ExecuteReader();
                }
                else if (typeof(T) == typeof(XmlReader))
                {
                    result = (T)(object)cmd.ExecuteXmlReader();
                }
                else
                {
                    result = (T)cmd.ExecuteScalar();
                    _connection.Close();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
