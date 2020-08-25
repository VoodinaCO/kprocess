using KProcess.KL2.ConnectionSecurity;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.SqlLocalDb;
using System.Linq;
using System.Management;
using System.Threading.Tasks;

namespace TestConnectionTool
{
    public static class SQLServerDetection
    {
        /// <summary>
        /// Method returns the correct SQL namespace to use to detect SQL Server instances.
        /// </summary>
        /// <param name="serverVersion">10(SQL Server 2008), 11(SQL Server 2012), 12(SQL Server 2014), 13(SQL Server 2016)</param>
        /// <returns>namespace to use to detect SQL Server instances</returns>
        static string GetCorrectWmiNameSpace()
        {
            string wmiNamespaceToUse = "root\\Microsoft\\sqlserver";
            List<string> namespaces = new List<string>();
            try
            {
                // Enumerate all WMI instances of
                // __namespace WMI class.
                ManagementClass nsClass = new ManagementClass(new ManagementScope(wmiNamespaceToUse), new ManagementPath("__namespace"), null);
                namespaces = nsClass.GetInstances().Cast<ManagementObject>().Select(_ => _["Name"].ToString()).Where(_ => _.StartsWith("ComputerManagement")).ToList();
            }
            catch (ManagementException e)
            {
                Console.WriteLine("Exception = " + e.Message);
            }
            if (namespaces.Count == 0)
                return null;
            int maxVersion = namespaces.Select(_ => int.Parse(_.Remove(0, 18))).Max();
            return $"{wmiNamespaceToUse}\\ComputerManagement{maxVersion}";
        }
        
        /// <summary>
        /// method extracts the instance name from the service name
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        static string GetInstanceNameFromServiceName(string serviceName)
        {
            if (!string.IsNullOrEmpty(serviceName))
                return string.Equals(serviceName, "MSSQLSERVER", StringComparison.OrdinalIgnoreCase) ? serviceName : serviceName.Split('$')[1];
            else
                return string.Empty;
        }

        /// <summary>
        /// Returns the WMI property value for a given property name for a particular SQL Server service Name
        /// </summary>
        /// <param name="serviceName">The service name for the SQL Server engine service to query for</param>
        /// <param name="wmiNamespace">The wmi namespace to connect to </param>
        /// <param name="propertyName">The property name whose value is required</param>
        /// <returns></returns>
        static string GetWmiPropertyValueForEngineService(string serviceName, string wmiNamespace, string propertyName)
        {
            string query = $"select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = '{propertyName}' and ServiceName = '{serviceName}'";
            ManagementObjectSearcher propertySearcher = new ManagementObjectSearcher(wmiNamespace, query);
            return propertySearcher.Get().Cast<ManagementObject>().First()["PropertyStrValue"].ToString();
        }

        /// <summary>
        /// Enumerates all SQL Server instances on the machine.
        /// </summary>
        /// <returns></returns>
        public static List<Models.DataBase> EnumerateSQLInstances()
        {
            List<Models.DataBase> result = new List<Models.DataBase>();
            try
            {
                string correctNamespace = GetCorrectWmiNameSpace();
                if (correctNamespace == null)
                    return result;
                ManagementObjectSearcher getSqlEngine = new ManagementObjectSearcher(correctNamespace, "select * from SqlServiceAdvancedProperty where SQLServiceType = 1 and PropertyName = 'instanceID'");
                IEnumerable<ManagementObject> collection = getSqlEngine.Get().OfType<ManagementObject>();
                return collection.Count() == 0 ? result : collection.Select(_ => new Models.DataBase
                {
                    ServerName = Environment.MachineName,
                    InstanceName = GetInstanceNameFromServiceName(_["ServiceName"].ToString()),
                    Version = Version.Parse(GetWmiPropertyValueForEngineService(_["ServiceName"].ToString(), correctNamespace, "Version"))
                }).ToList();
            }
            catch
            {
                return result;
            }
        }

        /// <summary>
        /// Teste l'existence de SQL Server 2014 LocalDB (12.0)
        /// </summary>
        /// <returns></returns>
        public static bool LocalDB_IsInstalled()
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey($@"SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\12.0") != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Teste l'existence d'une instance LocalDB de KL2
        /// </summary>
        /// <returns></returns>
        public static bool LocalDB_KL2Instance_Exists()
        {
            try
            {
                return (new SqlLocalDbProvider()).GetInstance(Const.InstanceName_v3) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Teste l'existence d'une base de donnée de KL2 dans l'instance par défault
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> LocalDB_KL2DataBase_Exists()
        {
            try
            {
                ISqlLocalDbInstance instance = (new SqlLocalDbProvider()).GetInstance(Const.InstanceName_v3);
                using (var conn = instance.CreateConnection())
                {
                    await conn.OpenAsync();
                    conn.ChangeDatabase(Const.DataBaseName_v3);
                    conn.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
