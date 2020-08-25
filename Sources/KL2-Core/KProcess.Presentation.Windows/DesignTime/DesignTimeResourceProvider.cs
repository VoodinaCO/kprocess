using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;
using KProcess.Presentation.Windows;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.DesignTime
{
    /// <summary>
    /// Un fournisseur de ressources utilisé en mode design.
    /// </summary>
    public class DesignTimeResourceProvider : ILocalizedResourceProvider
    {
        public string UniqueID
        {
            get { return "Design"; }
        }

        public object GetValue(string key, string resourceProviderKey = null, object defaultValue = null)
        {
            return LocalizationManagerExt.GetShortKey(key);
        }

        public string GetString(string key, string resourceProviderKey = null, string defaultValue = null)
        {
            return LocalizationManagerExt.GetShortKey(key);
        }

        public string GetString(int id, string resourceProviderKey = null, string defaultValue = null)
        {
            return id.ToString();
        }

        public IEnumerable<string> GetAllKeys()
        {
            return null;
        }
    }
}

#pragma warning restore 1591