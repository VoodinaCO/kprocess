using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow
{
    /// <summary>
    /// Représente une collection contenant la configuration des filtres DirectShow.
    /// </summary>
    [ConfigurationCollection(typeof(ExtensionElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class ExtensionsCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        protected override string ElementName
        {
            get { return "extension"; }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName == "extension";
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ExtensionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ExtensionElement)element).Extension;
        }
    }
}
