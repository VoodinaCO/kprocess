using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement de l'afficheur de boîtes de dialogue pour KSmed.
    /// </summary>
    [InheritedExport]
    public interface ICommonDialog : IDialogView
    {

        /// <summary>
        /// Affiche une boîte demandant la confirmation de suppression d'un élément.
        /// </summary>
        /// <returns>Le choix de l'utilisateur.</returns>
        bool ShowSureToDelete();

    }
}
