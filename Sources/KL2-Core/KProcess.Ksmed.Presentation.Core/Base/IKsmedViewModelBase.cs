using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    public interface IKsmedViewModelBase : IViewModel
    {
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur courant peut écrire sur la fiche.
        /// </summary>
        bool CanCurrentUserWrite { get; set; }
    }
}
