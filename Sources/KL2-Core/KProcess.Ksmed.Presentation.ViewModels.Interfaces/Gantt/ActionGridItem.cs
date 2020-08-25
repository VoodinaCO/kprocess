using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente une action dans une grille.
    /// </summary>
    public class ActionGridItem : DataTreeGridItem, IActionItem
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionGridItem"/>.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="level">The level.</param>
        public ActionGridItem(KAction action, int level)
        {
            Action = action;
            Indentation = level;
            UpdateContent();
        }
        public ActionGridItem(KAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Obtient l'action.
        /// </summary>
        public KAction Action { get; private set; }

        /// <summary>
        /// Obtient ou définit l'élément de resource auquel appartient l'élément d'action.
        /// </summary>
        public IReferentialItem ParentReferentialItem { get; set; }

        bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'item est sélectionné.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isEnabled = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si is l'item est activé.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _areMarkersEnabled = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si les curseurs associés à l'action doivent être activés.
        /// </summary>
        public bool AreMarkersEnabled
        {
            get => _areMarkersEnabled;
            set
            {
                if (_areMarkersEnabled != value)
                {
                    _areMarkersEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le prédecesseur à la création de l'élément.
        /// </summary>
        public IActionItem CreationPredecessor { get; set; }

        string _predecessorsString;
        /// <summary>
        /// Obtient ou définit les prédécesseurs sous forme de chaîne de caractères.
        /// </summary>
        public string PredecessorsString
        {
            get => _predecessorsString;
            set
            {
                if (_predecessorsString != value)
                {
                    _predecessorsString = value;
                    OnPropertyChanged();

                    PredecessorsStringChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Survient lorsque la chaîne de prédécesseurs a changé.
        /// </summary>
        public event EventHandler PredecessorsStringChanged;

        bool? _isGroup;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'élément est un groupe.
        /// </summary>
        public bool? IsGroup
        {
            get => _isGroup;
            set
            {
                if (_isGroup != value)
                {
                    _isGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        Brush _videoColor;
        /// <summary>
        /// Obtient la couleur de la vidéo.
        /// </summary>
        public Brush VideoColor
        {
            get => _videoColor;
            set
            {
                if (_videoColor != value)
                {
                    _videoColor = value;
                    OnPropertyChanged();
                }
            }
        }

        string _videoName;
        /// <summary>
        /// Obtient ou définit le nom de la vidéo.
        /// </summary>
        public string VideoName
        {
            get => _videoName;
            set
            {
                if (_videoName != value)
                {
                    _videoName = value;
                    OnPropertyChanged();
                }
            }
        }

        bool _isCritical;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'action fait partie du chemin critique.
        /// </summary>
        public bool IsCritical
        {
            get => _isCritical;
            set
            {
                if (_isCritical != value)
                {
                    _isCritical = value;
                    OnPropertyChanged();
                }
            }
        }

        double? _originalGain;
        /// <summary>
        /// Obtient ou définit le gain de temps par rapport à l'original en pourcentage.
        /// </summary>
        public double? OriginalGainPercentage
        {
            get => _originalGain;
            set
            {
                if (_originalGain != value)
                {
                    _originalGain = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la vignette de la vidéo.
        /// </summary>
        public CloudFile Thumbnail
        {
            get => Action?.Thumbnail;
            set
            {
                if (Action?.Thumbnail != value)
                {
                    if (Action != null)
                        Action.Thumbnail = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des outils.
        /// </summary>
        string _ref1Labels;
        public string Ref1Labels
        {
            get => _ref1Labels;
            set
            {
                if (_ref1Labels != value)
                {
                    _ref1Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des consommables.
        /// </summary>
        string _ref2Labels;
        public string Ref2Labels
        {
            get => _ref2Labels;
            set
            {
                if (_ref2Labels != value)
                {
                    _ref2Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des lieux.
        /// </summary>
        string _ref3Labels;
        public string Ref3Labels
        {
            get => _ref3Labels;
            set
            {
                if (_ref3Labels != value)
                {
                    _ref3Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des documents.
        /// </summary>
        string _ref4Labels;
        public string Ref4Labels
        {
            get => _ref4Labels;
            set
            {
                if (_ref4Labels != value)
                {
                    _ref4Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels supplémentaires 1.
        /// </summary>
        string _ref5Labels;
        public string Ref5Labels
        {
            get => _ref5Labels;
            set
            {
                if (_ref5Labels != value)
                {
                    _ref5Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels supplémentaires 2.
        /// </summary>
        string _ref6Labels;
        public string Ref6Labels
        {
            get => _ref6Labels;
            set
            {
                if (_ref6Labels != value)
                {
                    _ref6Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés concaténés des référentiels supplémentaires 3.
        /// </summary>
        string _ref7Labels;
        public string Ref7Labels
        {
            get => _ref7Labels;
            set
            {
                if (_ref7Labels != value)
                {
                    _ref7Labels = value;
                    OnPropertyChanged();
                }
            }
        }

        string _labelOrWBs;
        /// <summary>
        /// Obtient le libellé ou le WBS de l'action associée.
        /// </summary>
        public string LabelOrWBS
        {
            get => _labelOrWBs;
            set
            {
                if (_labelOrWBs != value)
                {
                    _labelOrWBs = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Met à jour le contenu à partir de l'élément associé.
        /// </summary>
        public void UpdateContent() =>
            UpdateContent(this);

        /// <summary>
        /// Met à jour le contenu à partir de l'élément associé.
        /// </summary>
        internal static void UpdateContent(IActionItem item)
        {
            if (item.Action != null)
            {
                string labelOrWBS;
                if (string.IsNullOrEmpty(item.Action.Label))
                {
                    if (DesignMode.IsInDesignMode)
                        labelOrWBS = item.Action.WBS;
                    else
                        labelOrWBS = LocalizationManager.GetStringFormat("VM_GridGanttItem_WBSTooltip", item.Action.WBS);
                }
                else
                    labelOrWBS = item.Action.Label;

                item.LabelOrWBS = labelOrWBS;
                item.Content = item.Action.Label;
            }
            else
            {
                item.LabelOrWBS = null;
                item.Content = null;
            }
        }
    }
}
