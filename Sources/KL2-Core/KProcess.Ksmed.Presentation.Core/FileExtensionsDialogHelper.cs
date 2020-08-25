using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;
using KProcess.Presentation.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Contient les extensions connues et acceptées pour le lecteur vidéo.
    /// </summary>
    public static class FileExtensionsDialogHelper
    {

        private const string _allFilesResourceKey = "FileDescription_AllFiles";
        private const string _allVideoFilesResourceKey = "FileDescription_AllVideoFiles";
        private const string _allImageFilesResourceKey = "FileDescription_AllImageFiles";
        private const string _allSqlBackupFilesResourceKey = "FileDescription_AllSqlBackupFiles";

        private static readonly string[] _sqlBackupExtensions = new string[] { "bak", "bak3" };

        private static readonly Dictionary<string, string[]> _imageExtensions = new Dictionary<string, string[]>
        {
            { "FileDescription_BMP", new[] { "bmp" }},
            { "FileDescription_JPEG", new[] { "jpg", "jpeg", "jpe" }},
            { "FileDescription_PNG", new[] { "png" }},
        };


        /// <summary>
        /// Obtient une chaîne pouvant être utilisée en tant que filtre d'un FileDialog pour les vidéos.
        /// </summary>
        /// <returns>Une chaîne pouvant être utilisée en tant que filtre d'un FileDialog.</returns>
        public static string GetVideosFileDialogFilter()
        {
            var filtersConfiguration = IoC.Resolve<IServiceBus>().Get<IDecoderInfoService>().FiltersConfiguration;
            var computedVideoExtensions = filtersConfiguration.Select(f => f.Extension.TrimStart('.')).OrderBy(ext => ext).ToArray();
            return GetFileDialogFilter(computedVideoExtensions, _allVideoFilesResourceKey, false);
        }


        /// <summary>
        /// Obtient une chaîne pouvant être utilisée en tant que filtre d'un FileDialog pour les images.
        /// </summary>
        /// <returns>Une chaîne pouvant être utilisée en tant que filtre d'un FileDialog.</returns>
        public static string GetImagesFileDialogFilter()
        {
            return GetFileDialogFilter(_imageExtensions, _allImageFilesResourceKey);
        }

        /// <summary>
        /// Obtient une chaîne pouvant être utilisée en tant que filtre d'un FileDialog pour les backup de base de données.
        /// </summary>
        /// <returns>Une chaîne pouvant être utilisée en tant que filtre d'un FileDialog.</returns>
        public static string GetSqlServerBackupFileDialogFilter()
        {
            return GetFileDialogFilter(_sqlBackupExtensions, _allSqlBackupFilesResourceKey);
        }

        private static string GetFileDialogFilter(string[] extensions, string allFilesLabelResourceKey, bool withAllFiles = true)
        {
            var sb = new StringBuilder();

            AppendMultipleExtenionsFilter(sb, LocalizationManager.GetString(allFilesLabelResourceKey), extensions);

            if (withAllFiles)
                AppendMultipleExtenionsFilter(sb, LocalizationManager.GetString(_allFilesResourceKey), new[] { "*" });

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        private static string GetFileDialogFilter(Dictionary<string, string[]> dic, string allFilesLabelResourceKey)
        {
            var sb = new StringBuilder();

            AppendMultipleExtenionsFilter(sb, LocalizationManager.GetString(allFilesLabelResourceKey), dic.Values.SelectMany(v => v));

            foreach (var kvp in dic)
                AppendMultipleExtenionsFilter(sb, LocalizationManager.GetString(kvp.Key), kvp.Value);

            AppendMultipleExtenionsFilter(sb, LocalizationManager.GetString(_allFilesResourceKey), new[] { "*" });

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();

        }

        private static void AppendMultipleExtenionsFilter(StringBuilder sb, string name, IEnumerable<string> extensions)
        {
            sb.Append(name);
            sb.Append(" (");

            foreach (var extension in extensions)
            {
                sb.Append("*.");
                sb.Append(extension);
                sb.Append(";");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append(")|");

            foreach (var extension in extensions)
            {
                sb.Append("*.");
                sb.Append(extension);
                sb.Append(";");
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("|");
        }

    }
}
