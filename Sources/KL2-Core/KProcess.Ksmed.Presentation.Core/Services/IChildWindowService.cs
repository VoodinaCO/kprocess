using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service capable d'afficher une fenêtre enfant.
    /// </summary>
    public interface IChildWindowService : IPresentationService
    {
        /// <summary>
        /// Afficher une fenêtre enfant.
        /// </summary>
        /// <param name="source">L'objet source. Il doit appartenir à une fenêtre affichée.</param>
        /// <param name="childWindow">La fenêtre enfant à afficher.</param>
        /// <param name="onClosed">Un délégué appelé lorsque la fenêtre est sur le point d'être fermée. Définir la valeur de retour à <c>true</c> pour annuler la fermeture.</param>
        /// <param name="onClosed">Un délégué appelé lorsque la fenêtre est fermée.</param>
        void ShowDialog(DependencyObject source, IChildWindow childWindow, Func<bool> onClosing = null, Action<bool?> onClosed = null);
    }

    /// <summary>
    /// Définit le comportement d'une fenêtre enfant.
    /// </summary>
    public interface IChildWindow
    {
        /// <summary>
        /// Obtient ou définit le résultat.
        /// </summary>
        bool? DialogResult { get; set; }

        /// <summary>
        /// Appelé lorsque la fenêtre est affichée.
        /// </summary>
        void OnShown();

        /// <summary>
        /// Ferme la fenêtre.
        /// </summary>
        void Close();

        /// <summary>
        /// Survient lorsque la fenêtre est fermée.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Survient lorsque la fenêtre est en train d'être fermée.
        /// </summary>
        event CancelEventHandler Closing;

    }
}
