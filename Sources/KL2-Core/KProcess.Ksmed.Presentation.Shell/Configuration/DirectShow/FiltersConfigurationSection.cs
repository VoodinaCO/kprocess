using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow
{
    /// <summary>
    /// Représente la configuration des filtres DirectShow.
    /// </summary>
    public class FiltersConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Obtient la configuration depuis le fichier de configuration.
        /// </summary>
        /// <returns>La configuration.</returns>
        public static FiltersConfigurationSection GetConfiguration()
        {
            var config = (FiltersConfigurationSection)System.Configuration.ConfigurationManager.GetSection(
        "directShowFilters");

            return config;
        }

        /// <summary>
        /// Obtient ou définit les extensions.
        /// </summary>
        [ConfigurationProperty("", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
        public ExtensionsCollection Extensions
        {
            get { return ((ExtensionsCollection)(base[""])); }
            set { base[""] = value; }
        }

    }
}
