using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une classe de base pour un élément du menu.
    /// </summary>
    public abstract class ApplicationMenuItemBase : NotifiableObject
    {

        /// <summary>
        /// Obtient ou définit le code identifiant l'élément.
        /// </summary>
        public string Code { get; set; }

        private string _label;
        /// <summary>
        /// Obtient le libellé.
        /// </summary>
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged("Label");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la clé de la ressource du libellé.
        /// </summary>
        public string LabelResourceKey { get; set; }

        private bool _isEnabled = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est activé.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        private bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est sélectionné.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        private Visibility _separatorVisibility;
        /// <summary>
        /// Obtient ou définit la visibilité du séparateur.
        /// </summary>
        public Visibility SeparatorVisibility
        {
            get { return _separatorVisibility; }
            set
            {
                if (_separatorVisibility != value)
                {
                    _separatorVisibility = value;
                    OnPropertyChanged("SeparatorVisibility");
                }
            }
        }
    }
}
