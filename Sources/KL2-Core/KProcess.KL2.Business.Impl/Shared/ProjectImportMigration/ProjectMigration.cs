using KProcess.Ksmed.Business.Dtos.Export;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Shared.ProjectImportMigration
{
    /// <summary>
    /// Permet d'effectuer une migration.
    /// </summary>
    internal class ProjectMigration
    {

        // Interne pour les TUs
        internal static IExportedProjectMigration[] _migrations =
        {
            // A remplir au fil des versions
        };

        private byte[] _inputBytes;
        private Stream _inputStream;
        private StreamReader _inputStreamReader;

        private static readonly Regex _versionRegex = new Regex(@"\<AppVersion[^>]*?\>(?<version>\d\.\d\.\d\.\d)\</AppVersion[^>]*?\>");

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="bytes">Les données sérialisées, sous la forme d'octets.</param>
        public ProjectMigration(byte[] bytes)
        {
            _inputBytes = bytes;
            _inputStream = new MemoryStream(bytes);
            _inputStreamReader = new StreamReader(_inputStream);
        }

        /// <summary>
        /// Migre les données.
        /// </summary>
        /// <returns>Les données migrées.</returns>
        public async Task<ProjectExport> Migrate()
        {
            this.TraceDebug("Début de migration");

            var currentVersion = typeof(ProjectMigration).Assembly.GetName().Version;

            _inputStream.Position = 0;

            Version version = await ReadVersion();
            if (version == null)
                throw new InvalidDataException("Version non trouvée");

            _inputStream.Position = 0;

            var migrations = GetMigrations(version);

            Stream currentData = _inputStream;

            foreach (var migration in migrations)
            {
                this.TraceDebug("Migration de l'export projet vers la version {0}", migration.NewVersion);

                Stream newData = await migration.Migrate(currentData, version);

                currentData.Dispose();
                currentData = newData;
                version = migration.NewVersion;

                this.TraceDebug("Migration vers la version {0} effectuée", migration.NewVersion);

                currentData.Position = 0;
            }

            this.TraceDebug("Désérialisation du projet vers la version actuelle");

            var projectImport = SerializationOperations.DeserializeWithNamespacesChange<ProjectExport>(currentData, currentVersion, true);

            currentData.Dispose();

            this.TraceDebug("Migration effectuée");

            return projectImport;
        }

        /// <summary>
        /// Obtient les migrations nécessaires à partir de la version spécifiée.
        /// </summary>
        /// <param name="importedVersion">La version à laquelle se trouvent les données importées.</param>
        /// <returns>Les migrations à effectuer.</returns>
        private IEnumerable<IExportedProjectMigration> GetMigrations(Version importedVersion) =>
            _migrations.Where(m => m.NewVersion > importedVersion);

        /// <summary>
        /// Lit la version dans la source.
        /// </summary>
        /// <returns>La version.</returns>
        private async Task<Version> ReadVersion()
        {
            string line;
            while ((line = await _inputStreamReader.ReadLineAsync()) != null)
            {
                var match = _versionRegex.Match(line);
                if (match.Success && match.Groups["version"].Success)
                {
                    string version = match.Groups["version"].Value;
                    this.TraceDebug("Version trouvée : {0}", version);
                    return Version.Parse(version);
                }
            }
            return null;
        }

    }
}