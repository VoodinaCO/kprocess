using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace KProcess.Presentation.Windows.Data
{

    /// <summary>
    /// Représente un Binding sur le <see cref="System.Windows.Controls.UserControl"/> parent le plus proche
    /// </summary>
    public class UCBinding : AncestorBinding
    {

        /// <summary>
        /// Crée une instance de <see cref="UCBinding"/>
        /// </summary>
        public UCBinding()
        {
            this.RelativeSource.AncestorType = typeof(UserControl);
        }

        /// <summary>
        /// Crée une instance de <see cref="UCBinding"/>
        /// </summary>
        /// <param name="path">le chemin vers le binding</param>
        public UCBinding(string path)
            : base(path, typeof(UserControl))
        {
        }

    }
}
