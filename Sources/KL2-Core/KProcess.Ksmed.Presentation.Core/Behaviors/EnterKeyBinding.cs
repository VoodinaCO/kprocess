using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un binding qui se met à jour lors de l'appui sur la touche entrée.
    /// Cette fonctionnalité ne peut fonctionner qu'avec l'utilisation conjointe de <see cref="ControlEnterBindingUpdate"/>.
    /// </summary>
    public class EnterKeyBinding : Binding
    {

        private bool _validates;

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la validation doit être activée.
        /// </summary>
        [ConstructorArgument("Validates")]
        public bool Validates
        {
            get { return _validates; }
            set
            {
                _validates = value;
                this.ValidatesOnDataErrors = value;
            }
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EnterKeyBinding"/>.
        /// </summary>
        public EnterKeyBinding()
        {
            this.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor);
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EnterKeyBinding"/>.
        /// </summary>
        /// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path"/> for the binding.</param>
        public EnterKeyBinding(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="EnterKeyBinding"/>.
        /// </summary>
        /// <param name="path">The initial <see cref="P:System.Windows.Data.Binding.Path"/> for the binding.</param>
        /// <param name="validates"><c>true</c> pour activer la validation.</param>
        public EnterKeyBinding(string path, bool validates)
            : base(path)
        {
            this.Validates = validates;
        }

    }
}
