using System.IO;

namespace KProcess.Common
{
    /// <summary>
    /// Fournit des méthodes d'aide à l'IO.
    /// </summary>
    public static class IOHelper
    {

        /// <summary>
        /// Remplace les caractères invalides dans le nom de fichier spécifié.
        /// </summary>
        /// <param name="filename">Le nom de fichier.</param>
        /// <param name="replacement">Le caractère de remplacement. Defaut: _</param>
        /// <returns>Le nom de fichier valide.</returns>
        public static string ReplaceInvalidFileNameChars(string filename, char replacement = '_')
        {
#if SILVERLIGHT
            var invalidChars = Path.GetInvalidPathChars();
#else
            var invalidChars = Path.GetInvalidFileNameChars();
#endif

            foreach (char ch in invalidChars)
                filename = filename.Replace(ch, '_');

            return filename;
        }

        /// <summary>
        /// Change le dossier d'un chemin vers un fichier
        /// </summary>
        /// <param name="filePath">Le chemin d'un fichier.</param>
        /// <param name="newDirectory">Le nouveau répertoire.</param>
        /// <returns>Le chemin avec fichier dans le dossier spécifié.</returns>
        public static string ChangeDirectory(string filePath, string newDirectory)
        {
            var fileName = Path.GetFileName(filePath);
            return Path.Combine(newDirectory, fileName);
        }

    }
}
