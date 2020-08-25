using System;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Définit le comportement d'un service de gestion de la base de données.
    /// </summary>
    public interface IDataBaseService: IService
    {
        /// <summary>
        /// Crée un fichier de backup de la base de données
        /// </summary>
        /// <param name="preventActionToBegin">Ne lance pas la tache de backup si True</param>
        /// <returns>Le resultat du backup. La chaine de characteres envoyée par la tache correpond au chemin vers le fichier créé</returns>
        SqlExecutionResult<string> Backup(bool preventActionToBegin = false);

        /// <summary>
        /// Restaure la base de donnée depuis un fichier de backup
        /// </summary>
        /// <param name="preventActionToBegin">Ne lance pas la tache de backup si True</param>
        /// <returns>Le resultat du backup. La chaine de characteres envoyée par la tache correpond au chemin vers le fichier créé</returns>
        SqlExecutionResult<string> Restore(string sourcePath, int version = 3, bool preventActionToBegin = false);

        /// <summary>
        /// Récupère le dossier courant reservé aux opérations de base de données
        /// </summary>
        /// <returns></returns>
        string GeBackupDir();

        /// <summary>
        /// Récupère la version pour laquelle la base de données est prévue
        /// </summary>
        /// <returns>La version de l'application associée à la base de données</returns>
        SqlExecutionResult<Version> GetVersion();

        /// <summary>
        /// Upgrade la version de la base de donnée
        /// </summary>
        /// <param name="to">La version ciblée</param>
        /// <returns></returns>
        void Upgrade(Version to);

        /// <summary>
        /// Update la version de la base de données
        /// </summary>
        /// <param name="version">La nouvelle version de la base de données</param>
        /// <returns></returns>
        SqlExecutionResult<object> SetDataBaseVersion(Version version);

        /// <summary>
        /// Détermine s'il la base est locale
        /// </summary>
        bool IsLocalDb();
    }
}
