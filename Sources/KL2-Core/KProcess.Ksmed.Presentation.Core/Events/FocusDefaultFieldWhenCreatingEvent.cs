using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Demande la sélection et le focus du champ par défaut lors de la création d'une nouvel élément.
    /// </summary>
    public class FocusDefaultFieldWhenCreatingEvent : EventBase
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="FocusDefaultFieldWhenCreatingEvent"/>.
        /// </summary>
        /// <param name="sender">objet à l'origine de l'événement</param>
        public FocusDefaultFieldWhenCreatingEvent(object sender)
            :base(sender)
        {
        }

    }
}
