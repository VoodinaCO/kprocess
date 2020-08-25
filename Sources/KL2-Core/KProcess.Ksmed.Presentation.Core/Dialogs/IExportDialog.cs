using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement de l'afficheur de boîtes de dialogue pour les exports.
    /// </summary>
    [InheritedExport]
    public interface IExportDialog : IDialogView
    {

        /// <summary>
        /// Afficher la boîte de dialogue d'export excel.
        /// </summary>
        /// <param name="format">Le format.</param>
        /// <returns>
        /// le résultat de la boite de dialogue
        /// </returns>
        ExportResult ShowExportToExcel(ExcelFormat format);

        /// <summary>
        /// Afficher la boîte de dialogue d'export de projet.
        /// </summary>
        /// <returns>Le chemin vers le fichier.</returns>
        string ExportProject();

        /// <summary>
        /// Afficher la boîte de dialogue d'import de projet.
        /// </summary>
        /// <returns>Les paramètres de l'importation.</returns>
        ImportWithVideoFolderResult ImportProject();

        /// <summary>
        /// Afficher la boîte de dialogue d'export de décomposition vidéo.
        /// </summary>
        /// <param name="videos">Les vidéos que l'utilisateur peut sélectionner.</param>
        /// <returns>
        /// Le résultat.
        /// </returns>
        ExportVideoDecompositionResult ExportVideoDecomposition(Models.Video[] videos);

        /// <summary>
        /// Afficher la boîte de dialogue d'import de décomposition vidéo.
        /// </summary>
        /// <returns>Les paramètres de l'importation.</returns>
        ImportWithVideoFolderResult ImportVideoDecomposition();

    }

    /// <summary>
    /// Le format des exports excel.
    /// </summary>
    public enum ExcelFormat
    {
        Xlsx,
        Xlsm
    }

    /// <summary>
    /// Le résultat de la boite de dialogue d'export.
    /// </summary>
    public class ExportResult
    {
        /// <summary>
        /// Obtient ou définit l'acceptation de l'utilisateur.
        /// </summary>
        public bool Accepts { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de fichier final.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le fichier doit être ouvert une fois l'export créé.
        /// </summary>
        public bool OpenWhenCreated { get; set; }
    }

    /// <summary>
    /// Le résultat de la boite de dialogue d'import contenant le dossier vidéo.
    /// </summary>
    public class ImportWithVideoFolderResult
    {
        /// <summary>
        /// Obtient ou définit l'acceptation de l'utilisateur.
        /// </summary>
        public bool Accepts { get; set; }

        /// <summary>
        /// Obtient ou définit le nom de fichier final.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Obtient ou définit le dossier où se trouvent les vidéos.
        /// </summary>
        public string VideosFolder { get; set; }

    }

    /// <summary>
    /// Le résultat d'une boite de dialogue d'export de décomposition vidéo
    /// </summary>
    public class ExportVideoDecompositionResult
    {
        /// <summary>
        /// Obtient ou définit le nom du fichier.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de la vidéo.
        /// </summary>
        public int VideoId { get; set; }
    }
}
