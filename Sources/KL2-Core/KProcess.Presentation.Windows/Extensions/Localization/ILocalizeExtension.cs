using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows.Localization
{
    /// <summary>
    /// Définit le comportement d'un objet fournissant les informations nécessaires à la localisation.
    /// </summary>
    public interface ILocalizeExtension
    {
        /// <summary>
        /// Obtient ou définit la valeur par défaut si la resource n'est pas trouvée
        /// </summary>
        object DefaultValue { get; set; }

        /// <summary>
        /// Obtient ou définit la clé de la resource
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Obtient ou définit le fournisseur à utiliser
        /// </summary>
        string ProviderKey { get; set; }

        /// <summary>
        /// Obtient ou définit la chaîne de formatage de la resource
        /// </summary>
        string StringFormat { get; set; }
    }
}
