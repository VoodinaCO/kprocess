using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit un menu.
    /// </summary>
    [InheritedExport]
    public interface IMenuDefinition
    {

        /// <summary>
        /// Initialise le menu.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Obtient le code identifiant le menu.
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Obtient la barre de menu à laquelle appartient ce menu;
        /// </summary>
        MenuStrip Strip { get; }

        /// <summary>
        /// Obtient le titre du menu.
        /// </summary>
        string TitleResourceKey { get; }

        /// <summary>
        /// Survient lorsque le titre a changé.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Obtient le délégué déterminant si ce menu est activé.
        /// </summary>
        Func<bool> IsEnabledDelegate { get; }

        /// <summary>
        /// Survient lorsque la propriété IsEnabled doit être rafraîchie.
        /// </summary>
        event EventHandler IsEnabledInvalidated;

    }
}
