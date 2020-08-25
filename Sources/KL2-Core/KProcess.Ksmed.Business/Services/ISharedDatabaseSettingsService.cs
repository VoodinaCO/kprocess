using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Définit le comportement des paramètres du mode de fonctionnement en base de données partagée.
    /// </summary>
    public interface ISharedDatabaseSettingsService
    {

        /// <summary>
        /// Obtient la durée à partir de laquelle un verrou n'est plus valide.
        /// </summary>
        TimeSpan LockTimeout { get; }

        /// <summary>
        /// Obtient la fréquence de mise à jour du verrou.
        /// </summary>
        TimeSpan LockUpdateFrequency { get; }

    }
}
