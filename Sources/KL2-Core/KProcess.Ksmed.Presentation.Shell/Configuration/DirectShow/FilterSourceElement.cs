using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow
{
    /// <summary>
    /// Représente la configuration d'une source d'un filtre.
    /// </summary>
    public class FilterSourceElement : ConfigurationElement
    {

        /// <summary>
        /// Obtient ou définit le nom du filtre.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Obtient ou définit le type de source.
        /// </summary>
        [ConfigurationProperty("sourceType", IsRequired = true)]
        public FilterSourceTypeEnum SourceType
        {
            get { return (FilterSourceTypeEnum)this["sourceType"]; }
            set { this["sourceType"] = value; }
        }

        /// <summary>
        /// Obtient ou définit le CLSID du filtre externe.
        /// </summary>
        [ConfigurationProperty("externalCLSID", IsRequired = false)]
        public string ExternalCLSID
        {
            get { return (string)this["externalCLSID"]; }
            set { this["externalCLSID"] = value; }
        }

    }
}
