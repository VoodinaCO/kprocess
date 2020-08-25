using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Définit le comportement du service de gestion de verrous en base de données partagée.
    /// </summary>
    public interface ISharedDatabaseService : IService
    {

        /// <summary>
        /// Détermine si la base de données est vérouillée pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        Task<bool> IsLocked(string username);

        /// <summary>
        /// Met à jour le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        Task UpdateLock(string username);

        /// <summary>
        /// Libère le verrou pour l'utilisateur spécifié.
        /// </summary>
        /// <param name="username">le nom d'utilisateur</param>
        Task ReleaseLock(string username);

    }
}