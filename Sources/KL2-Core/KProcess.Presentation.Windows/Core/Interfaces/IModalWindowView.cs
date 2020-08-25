using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente une fênetre modale
    /// </summary>
    public interface IModalWindowView : IWindowView
    {

        /// <summary>
        /// Affiche la fenêtre en modal et retourne son résultat
        /// </summary>
        /// <returns>le résultat de la fenêtre</returns>
        bool? ShowDialog();

        /// <summary>
        /// Obtient ou définit le résultat de la fenêtre
        /// </summary>
        bool? DialogResult { get; set; }

    }
}
