// -----------------------------------------------------------------------
// <copyright file="IOpenFileDialog.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface des vues de dialogue d'ouverture de fichiers
    /// </summary>
    [InheritedExport]
    public interface IOpenFileDialog : IDialogView
    {
        /// <summary>
        /// Affiche la vue permettant de sélectionner un ou plusieurs fichiers
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <param name="defaultExtension">extension par défaut</param>
        /// <param name="multiSelect">indique si on peut ou non sélectionner plusieurs fichiers</param>
        /// <param name="filter">filtre de sélection</param>
        /// <param name="checkPathExists">indique si le chemin doit exister</param>
        /// <param name="checkFileExists">indique si le fichier doit exister</param>
        /// <param name="initialDirectory">Répertoire par défaut où va commencer la recherche</param>
        /// <returns>la liste des chemins complets des fichiers sélectionnés, null si aucun</returns>
        string[] Show(string caption, string defaultExtension = "", bool multiSelect = false, string filter = "All files (*.*)|*.*", bool checkPathExists = true, bool checkFileExists = true, string initialDirectory = "");
    }
}
