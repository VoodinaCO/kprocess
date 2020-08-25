// -----------------------------------------------------------------------
// <copyright file="ISaveFileDialog.cs" company="Tekigo">
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
    /// Définit l'interface des vues de dialogue de sauvegarde de fichiers
    /// </summary>
    [InheritedExport]
    public interface ISaveFileDialog : IDialogView
    {
        /// <summary>
        /// Affiche la vue permettant de sauvegarder un fichier
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <param name="defaultExtension">extension par défaut</param>        
        /// <param name="filter">filtre de sélection</param>
        /// <param name="checkPathExists">indique si le chemin doit exister</param>
        /// <param name="checkFileExists">indique si le fichier doit exister</param>
        /// <returns>le chemin complet du fichier sélectionné, null si aucun</returns>
        string Show(string caption, string defaultExtension = "", string filter = "All files (*.*)|*.*", bool checkPathExists = true, bool checkFileExists = false);
    }
}
