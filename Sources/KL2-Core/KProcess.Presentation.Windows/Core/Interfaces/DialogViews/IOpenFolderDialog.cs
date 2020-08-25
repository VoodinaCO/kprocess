using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface des vues de dialogue d'ouverture de dossier
    /// </summary>
    [InheritedExport]
    public interface IOpenFolderDialog : IDialogView
    {

        /// <summary>
        /// Affiche la vue permettant de sélectionner un dossier
        /// </summary>
        /// <param name="caption">titre de la vue</param>
        /// <returns>
        /// le dossier sélectionné, ou null si annulé
        /// </returns>
        string Show(string caption);

    }
}
