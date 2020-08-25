using System;
using System.IO;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared.ProjectImportMigration
{
    /// <summary>
    /// Décrit une migration d'export de projet d'une version à une autre
    /// </summary>
    public interface IExportedProjectMigration
    {

        /// <summary>
        /// Obtient la version vers laquelle le projet va être migré.
        /// </summary>
        Version NewVersion { get; }

        /// <summary>
        /// Migre le projet.
        /// </summary>
        /// <param name="stream">Le flux de données de l'export projet.</param>
        /// <param name="streamExportVersion">La version contenu dans le stream.</param>
        /// <returns>Le flux de données contenant le projet migré.</returns>
        Task<Stream> Migrate(Stream stream, Version streamExportVersion);

    }
}
