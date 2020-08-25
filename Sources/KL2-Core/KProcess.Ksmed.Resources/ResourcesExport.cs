using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;

namespace KProcess.Ksmed.Resources
{
    /// <summary>
    /// Identifie l'export des ressources
    /// </summary>
    public class ResourcesExport : IResourceProviderExport
    {
        /// <summary>
        /// Obtient le fournisseur de ressources.
        /// </summary>
        /// <returns>Le fournisseur de ressources.</returns>
        public ILocalizedResourceProvider GetProvider()
        {
            return new ResourceFileProvider("Main", typeof(MainResources));
        }
    }
}
