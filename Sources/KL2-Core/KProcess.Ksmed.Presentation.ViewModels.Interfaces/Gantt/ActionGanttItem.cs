using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using System;
using System.Windows;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente une action dans une grille.
    /// </summary>
    public class ActionGanttItem : GanttChartItem, IActionItem
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionGanttItem"/>.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="level">Le niveau d'indentation.</param>
        public ActionGanttItem(KAction action, int level)
        {
            Action = action;
            Indentation = level;
            Start = GanttDates.ToDateTime(action.BuildStart);
            Finish = GanttDates.ToDateTime(action.BuildFinish);
            UpdateContent();
        }

        /// <summary>
        /// Obtient l'action.
        /// </summary>
        public KAction Action { get; }

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
                    OnIsGroupChanged();
                }
            }
        }

        void OnIsGroupChanged()
        {
            DependencyCreationThumbVisibility = IsGroup.GetValueOrDefault() ? Visibility.Collapsed : Visibility.Visible;
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

        Brush _strokeBrush;
        /// <summary>
        /// Obtient ou définit la couleur de l'élément.
        /// </summary>
        public Brush StrokeBrush
        {
            get => _strokeBrush;
            set
            {
                if (_strokeBrush != value)
                {
                    _strokeBrush = value;
                    OnPropertyChanged();
                }
            }
        }

        Brush _fillBrush;
        /// <summary>
        /// Obtient ou définit le pinceau définissant le contenu.
        /// </summary>
        public Brush FillBrush
        {
            get => _fillBrush;
            set
            {
                if (_fillBrush != value)
                {
                    _fillBrush = value;
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

        Visibility _orangeHeaderVisibility = Visibility.Collapsed;
        /// <summary>
        /// Obtient ou définit la visibilité de l'entête orange.
        /// </summary>
        public Visibility OrangeHeaderVisibility
        {
            get => _orangeHeaderVisibility;
            set
            {
                if (_orangeHeaderVisibility != value)
                {
                    _orangeHeaderVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        string _orangeHeaderToolTip;
        /// <summary>
        /// Obtient ou définit le ToolTip de l'entête orange.
        /// </summary>
        public string OrangeHeaderToolTip
        {
            get => _orangeHeaderToolTip;
            set
            {
                if (_orangeHeaderToolTip != value)
                {
                    _orangeHeaderToolTip = value;
                    OnPropertyChanged();
                }
            }
        }

        Visibility _greenHeaderVisibility = Visibility.Collapsed;
        /// <summary>
        /// Obtient ou définit la visibilité de l'entête Green.
        /// </summary>
        public Visibility GreenHeaderVisibility
        {
            get => _greenHeaderVisibility;
            set
            {
                if (_greenHeaderVisibility != value)
                {
                    _greenHeaderVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        string _greenHeaderToolTip;
        /// <summary>
        /// Obtient ou définit le ToolTip de l'entête Green.
        /// </summary>
        public string GreenHeaderToolTip
        {
            get => _greenHeaderToolTip;
            set
            {
                if (_greenHeaderToolTip != value)
                {
                    _greenHeaderToolTip = value;
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

        bool _canResize = true;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'item peut être redimensionné.
        /// </summary>
        public bool CanResize
        {
            get => _canResize;
            set
            {
                if (_canResize != value)
                {
                    _canResize = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override void OnTimingChanged()
        {
            base.OnTimingChanged();

            _managedDuration = (Finish - Start).Ticks;
            _managedStart = GanttDates.ToTicks(Start);
            _managedFinish = GanttDates.ToTicks(Finish);
            OnPropertyChanged(nameof(ManagedDuration));
            OnPropertyChanged(nameof(ManagedStart));
            OnPropertyChanged(nameof(ManagedFinish));
        }

        long _managedStart;
        /// <summary>
        /// Obtient ou définit le début managé de l'action.
        /// </summary>
        public long ManagedStart
        {
            get => _managedStart;
            set
            {
                if (_managedStart != value)
                {
                    _managedStart = value;
                    Action.BuildStart = value;
                    OnPropertyChanged();
                }
            }
        }

        long _managedDuration;
        /// <summary>
        /// Obtient ou définit la durée managée de l'action.
        /// </summary>
        public long ManagedDuration
        {
            get => _managedDuration;
            set
            {
                if (_managedDuration != value)
                {
                    _managedDuration = value;
                    Action.BuildDuration = value;
                    OnPropertyChanged();
                }
            }
        }

        long _managedFinish;
        /// <summary>
        /// Obtient ou définit la fin managée de l'action.
        /// </summary>
        public long ManagedFinish
        {
            get => _managedFinish;
            set
            {
                if (_managedFinish != value)
                {
                    _managedFinish = value;
                    Action.BuildFinish = value;
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
            ActionGridItem.UpdateContent(this);
    }
}
