using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow
{
    /// <summary>
    /// Représente la configuration des filtres définis pour une extension de fichier.
    /// </summary>
    public class ExtensionElement : ConfigurationElement
    {
        /// <summary>
        /// Obtient ou définit l'extension de fichier (avec le point).
        /// </summary>
        [ConfigurationProperty("extension", IsRequired = true)]
        public string Extension
        {
            get { return (string)this["extension"]; }
            set { this["extension"] = value; }
        }

        /// <summary>
        /// Obtient ou définit la vitesse de lecture minimale supportée.
        /// </summary>
        [ConfigurationProperty("minSpeedRatio", IsRequired = false, DefaultValue = null)]
        public double? MinSpeedRatio
        {
            get { return (double?)this["minSpeedRatio"]; }
            set { this["minSpeedRatio"] = value; }
        }

        /// <summary>
        /// Obtient ou définit la vitesse de lecture maximale supportée.
        /// </summary>
        [ConfigurationProperty("maxSpeedRatio", IsRequired = false, DefaultValue = null)]
        public double? MaxSpeedRatio
        {
            get { return (double?)this["maxSpeedRatio"]; }
            set { this["maxSpeedRatio"] = value; }
        }

        /// <summary>
        /// Obtient ou définit le Splitter.
        /// </summary>
        [ConfigurationProperty("splitter", IsRequired = true)]
        public FilterSourceElement Splitter
        {
            get { return (FilterSourceElement)this["splitter"]; }
            set { this["splitter"] = value; }
        }

        /// <summary>
        /// Obtient ou définit le décodeur vidéo.
        /// </summary>
        [ConfigurationProperty("videoDecoder", IsRequired = true)]
        public FilterSourceElement VideoDecoder
        {
            get { return (FilterSourceElement)this["videoDecoder"]; }
            set { this["videoDecoder"] = value; }
        }

        /// <summary>
        /// Obtient ou définit le décodeur audio.
        /// </summary>
        [ConfigurationProperty("audioDecoder", IsRequired = true)]
        public FilterSourceElement AudioDecoder
        {
            get { return (FilterSourceElement)this["audioDecoder"]; }
            set { this["audioDecoder"] = value; }
        }

    }
}
