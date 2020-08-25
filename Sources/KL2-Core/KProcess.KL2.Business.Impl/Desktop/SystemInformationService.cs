using KProcess.Business;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente le service de gestion des informations système.
    /// </summary>
    public class SystemInformationService : IBusinessService, ISystemInformationService
    {
        readonly ITraceManager _traceManager;

        private const string ComputerSystem = "Win32_ComputerSystem";
        private const string OperatingSystem = "Win32_OperatingSystem";
        private const string Processor = "Win32_Processor";
        private const string VideoController = "Win32_VideoController";

        public SystemInformationService(
            ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        /// <inheritdoc />
        public SystemBasicInformation GetBasicInformation()
        {
            // Misc
            var si = new SystemBasicInformation
            {
                MachineName = GetSafeWMIValue<string>(ComputerSystem, "Name"),

                OperatingSystem = GetSafeWMIValue<string>(OperatingSystem, "Name"),
                OperatingSystemArchitecture = GetSafeWMIValue<string>(OperatingSystem, "OSArchitecture"),
                OperatingSystemVersion = GetSafeWMIValue<string>(OperatingSystem, "Version"),

                Manufacturer = GetSafeWMIValue<string>(ComputerSystem, "Manufacturer"),
                Model = GetSafeWMIValue<string>(ComputerSystem, "Model"),

                Memory = GetSafeWMIValue<ulong>(ComputerSystem, "TotalPhysicalMemory"),
                OSVisibleMemory = GetSafeWMIValue<ulong>(OperatingSystem, "TotalVisibleMemorySize"),
            };

            // Processeurs
            try
            {
                var processors = ExecuteWMI("SELECT Name FROM Win32_Processor").ToArray();
                si.Processors = processors.Select(dic => GetSafeDictionaryValue<string>(dic, "Name")).ToArray();
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Erreur lors de la récupération des informations sur les processeurs");
            }

            // Cartes graphiques
            try
            {
                var videoControllers = ExecuteWMI("SELECT Name, VideoModeDescription FROM Win32_VideoController").ToArray();
                si.VideoControllers = videoControllers.Select(dic => new VideoController
                {
                    Name = GetSafeDictionaryValue<string>(dic, "Name"),
                    Resolution = GetSafeDictionaryValue<string>(dic, "VideoModeDescription"),
                }).ToArray();
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Erreur lors de la récupération des informations sur les cartes graphiques");
            }

            // Languages
            var language = GetSafeWMIValue<uint>(OperatingSystem, "OSLanguage");
            if (language != default(uint))
            {
                try
                {
                    si.OperatingSystemLanguage = System.Globalization.CultureInfo.GetCultureInfo((int)language);
                }
                catch (ArgumentException e)
                {
                    _traceManager.TraceError(e, "Erreur lors de la récupération des informations sur les langues");
                }
            }

            return si;
        }

        /// <summary>
        /// Obtient la valeur de la clé spécifiée sur la première ligne du tableau spécifié.
        /// </summary>
        /// <typeparam name="T">Le type de la donnée.</typeparam>
        /// <param name="table">Le nom du tableau.</param>
        /// <param name="key">Le nom de la clé.</param>
        /// <returns>La donnée ou sa valeur par défaut en cas de problème.</returns>
        private T GetSafeWMIValue<T>(string table, string key)
        {
            string query = $"SELECT {key} FROM {table}";
            try
            {
                var searcher = new ManagementObjectSearcher(query);

                var enumerator = searcher.Get().GetEnumerator();
                enumerator.MoveNext();
                var obj = enumerator.Current;
                var value = obj.Properties[key];

                return GetSafeValue<T>(value.Value);
            }
            catch (Exception e)
            {
                _traceManager.TraceError(e, "Erreur lors de l'exécution de la requête : {0}", query);
                return default(T);
            }
        }
        /// <summary>
        /// Obtient la  valeur de la clé spécifiée dans le dictionnaire.
        /// </summary>
        /// <typeparam name="T">Le type de la donnée.</typeparam>
        /// <param name="data">Le dictionnaire.</param>
        /// <param name="key">Le nom de la clé.</param>
        /// <returns>La donnée ou sa valeur par défaut en cas de problème.</returns>
        private T GetSafeDictionaryValue<T>(Dictionary<string, object> data, string key)
        {
            if (!data.ContainsKey(key))
                return default(T);

            object value = data[key];
            return GetSafeValue<T>(value);
        }

        /// <summary>
        /// Obtient la  valeur spécifiée de manière sûre.
        /// </summary>
        /// <typeparam name="T">Le type de la donnée.</typeparam>
        /// <param name="value">La valeur en object.</param>
        /// <returns>La donnée ou sa valeur par défaut en cas de problème.</returns>
        private T GetSafeValue<T>(object value)
        {
            if (value == null)
                return default(T);

            try
            {
                return (T)value;
            }
            catch (InvalidCastException e)
            {
                _traceManager.TraceError(e, "Erreur lors du cast de la valeur : {0} en {1}", value, typeof(T));
                return default(T);
            }
        }

        /// <summary>
        /// Exécute une requête WMI.
        /// </summary>
        /// <param name="query">La requête.</param>
        /// <returns>Les résultats de la requête.</returns>
        private IEnumerable<Dictionary<string, object>> ExecuteWMI(string query)
        {
            var searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject obj in searcher.Get())
                yield return obj.Properties.Cast<PropertyData>().ToDictionary(pd => pd.Name, pd => pd.Value);
        }

    }
}
