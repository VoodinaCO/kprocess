using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Presentation.Core;
using System.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente un conteneur autour d'une vue.
    /// </summary>
    public class GanttGridViewContainer : NotifiableObject
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="GanttGridViewContainer"/>.
        /// </summary>
        /// <param name="view">la vue.</param>
        /// <param name="label">Le libellé.</param>
        public GanttGridViewContainer(int view, string label)
        {
            this.View = view;
            this.Label = label;
        }

        /// <summary>
        /// Obtient la vue.
        /// </summary>
        public int View { get; private set; }

        /// <summary>
        /// Obtient le libellé
        /// </summary>
        public string Label { get; private set; }

        private Visibility _visibility = Visibility.Visible;
        /// <summary>
        /// Obtient ou définit la visibilité.
        /// </summary>
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }

    }
}
