using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Représente la configuration des filtres DirectShow.
    /// </summary>
    public class FiltersConfiguration : IEnumerable<ExtensionFiltersSource>
    {

        private Dictionary<string, ExtensionFiltersSource> _extensions;

        /// <summary>
        /// Obtient ou définit le <see cref="KProcess.Presentation.Windows.Controls.ExtensionFiltersSource"/> pour l'extension spécifiée.
        /// </summary>
        public ExtensionFiltersSource this[string extension]
        {
            get
            {
                var ext = extension != null ? extension.ToLower() : null;
                if (_extensions == null || !_extensions.ContainsKey(ext))
                    return null;
                else
                    return _extensions[ext];
            }
            set
            {
                if (_extensions == null)
                    _extensions = new Dictionary<string, ExtensionFiltersSource>();

                var ext = extension != null ? extension.ToLower() : null;
                _extensions[ext] = value;

            }
        }

        #region IEnumerable<ExtensionFiltersSource> Members

        /// <summary>
        /// Obtient l'énumérateur.
        /// </summary>
        /// <returns>L'énumérateur</returns>
        public IEnumerator<ExtensionFiltersSource> GetEnumerator()
        {
            return _extensions.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Obtient l'énumérateur.
        /// </summary>
        /// <returns>L'énumérateur</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _extensions.Values.GetEnumerator();
        }

        #endregion
    }
}
