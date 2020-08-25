using KProcess.Presentation.Windows;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Définit le comportement du service de gestion de verrou en base de données partagée dans la couche présentation.
    /// </summary>
    interface ISharedDatabasePresentationService : KProcess.Ksmed.Business.ISharedDatabaseSettingsService, IPresentationService
    {
        /// <summary>
        /// Initialise la gestion du verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Obtient une valeur indiquant si la base de données est vérouillée pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur.</param>
        Task<bool> IsLocked(string username);

        /// <summary>
        /// Met à jour le verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        Task UpdateLock();

        /// <summary>
        /// Libère le verrou pour l'utilisateur actuellement connecté.
        /// </summary>
        Task ReleaseLock();

    }
}