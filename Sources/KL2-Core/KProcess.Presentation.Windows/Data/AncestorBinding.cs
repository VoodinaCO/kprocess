using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace KProcess.Presentation.Windows.Data
{
    /// <summary>
    /// Représente un Binding relatif à une ancêtre. Le premier argument est le Path, le deuxième est l'AncestorType
    /// </summary>
    public class AncestorBinding : Binding
    {
        /// <summary>
        /// Crée une instance de <see cref="AncestorBinding"/>
        /// </summary>
        public AncestorBinding()
        {
            this.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
        }

        /// <summary>
        /// Crée une instance de <see cref="AncestorBinding"/>
        /// </summary>
        /// <param name="path">le chemin du binding</param>
        public AncestorBinding(string path)
            : base(path)
        {
            this.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
        }

        /// <summary>
        /// Crée une instance de <see cref="AncestorBinding"/>
        /// </summary>
        /// <param name="path">le chemin du binding</param>
        /// <param name="ancestorType">le type de l'ancêtre</param>
        public AncestorBinding(string path, Type ancestorType)
            : base(path)
        {
            this.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, ancestorType, 1);
        }
    }
}
