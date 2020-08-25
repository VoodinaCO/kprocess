using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente un référentiel dans une grille.
    /// </summary>
    public class ReferentialGridItem : DataTreeGridItem, IReferentialItem
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ReferentialGridItem"/>.
        /// </summary>
        /// <param name="referential">Le référentiel.</param>
        /// <param name="label">Le libellé du référentiel.</param>
        public ReferentialGridItem(IActionReferential referential, string label)
        {
            this.Referential = referential;
            this.Content = label;
        }

        /// <summary>
        /// Obtient le référentiel.
        /// </summary>
        public IActionReferential Referential { get; private set; }

        private Brush _videoColor;
        /// <summary>
        /// Obtient la couleur de la vidéo.
        /// </summary>
        public Brush VideoColor
        {
            get { return _videoColor; }
            set
            {
                if (_videoColor != value)
                {
                    _videoColor = value;
                    OnPropertyChanged("VideoColor");
                }
            }
        }

        private string _videoName;
        /// <summary>
        /// Obtient ou définit le nom de la vidéo.
        /// </summary>
        public string VideoName
        {
            get { return _videoName; }
            set
            {
                if (_videoName != value)
                {
                    _videoName = value;
                    OnPropertyChanged("VideoName");
                }
            }
        }

        private bool? _isGroup;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est un groupe.
        /// </summary>
        public bool? IsGroup
        {
            get { return _isGroup; }
            set
            {
                if (_isGroup != value)
                {
                    _isGroup = value;
                    OnPropertyChanged("IsGroup");
                }
            }
        }

        private bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'item est sélectionné.
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

        private bool _isEnabled = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si is l'item est activé.
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

    }
}
