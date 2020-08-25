using KProcess.Globalization;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Behaviors;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente un VM de base pour les fenêtre de restitution centrées autour de l'exploitation de référentiels par des ressources.
    /// </summary>
    /// <typeparam name="TData">Le type des données.</typeparam>
    /// <typeparam name="TDataItem">Le type des sous éléments des données.</typeparam>
    /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
    abstract class RestitutionViewByResourceViewModelBase<TData, TDataItem, TReferential> : ViewModelBase, IRestitutionViewByResourceViewModel<TReferential>
        where TReferential : IActionReferential
        where TData : ReferentialBase<TDataItem>, new()
        where TDataItem : ReferentialItemBase, new()
    {

        #region Champs privés

        private string _descriptionStringFormatPercentages = "{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2:F}%";
        private string _descriptionStringFormatOccurences = "{0}" + Environment.NewLine + "{1}" + Environment.NewLine + "{2}";

        private int _selectedViewIndex = -1;

        private TData[] _itemsLeft;
        private TData[] _leftTotal;
        private TData[] _itemsRight;
        private TData[] _rightTotal;

        private string _titleLeft;
        private string _titleRight;

        private Visibility _rightPickerVisibility;
        private Resource _selectedResource;
        private Resource[] _resources;
        private RestitutionValueMode _selectedValueMode;

        private ITimeTicksFormatService _timeService;


        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.ValueModes = RestitutionValueModeViewModel.GetViewModels().ToArray();

            this.Views = new string[]
            {
                LocalizationManager.GetString("ViewModel_Restitution_View_Global"),
                LocalizationManager.GetStringFormat("ViewModel_Restitution_View_Per", ReferentialsUse.Operator),
                LocalizationManager.GetStringFormat("ViewModel_Restitution_View_Per", ReferentialsUse.Equipment),
            };

            this.AllOperatorsLabel = ReferentialsUse.AllOperators;
            this.AllEquipmentsLabel = ReferentialsUse.AllEquipments;
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading()
        {
            _timeService = ServiceBus.Get<ITimeTicksFormatService>();
            IProjectManagerService pms = ServiceBus.Get<IProjectManagerService>();
            RestitutionState restitutionState = pms.RestitutionState[pms.CurrentProject.ProjectId];

            switch (restitutionState.ViewMode.Value)
            {
                case RestitutionStateViewMode.Global:
                    SelectedViewIndex = 0;
                    break;
                case RestitutionStateViewMode.PerOperator:
                    SelectedViewIndex = 1;
                    break;
                case RestitutionStateViewMode.PerEquipment:
                    SelectedViewIndex = 2;
                    break;
            }

            SelectedValueMode = (RestitutionValueMode)restitutionState.RestitutionValueMode.Value;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode appelée lors du rafraîchissement
        /// </summary>
        protected override Task OnRefreshing()
        {
            LoadCurrentView(true);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Views = new string[]
                {
                    "Global",
                    "Par opérateur",
                    "Par équipement",
                };

            AllOperatorsLabel = "Opérateur - Tous";
            AllEquipmentsLabel = "Equipements - Tous";

            return Task.CompletedTask;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le VM parent.
        /// </summary>
        public IRestitutionViewModel ParentViewModel { get; set; }

        /// <summary>
        /// Obtient ou définit les vues disponibles.
        /// </summary>
        public string[] Views { get; set; }

        /// <summary>
        /// Obtient ou définit l'index de la vue sélectionnée.
        /// </summary>
        public int SelectedViewIndex
        {
            get { return _selectedViewIndex; }
            set
            {
                if (_selectedViewIndex != value)
                {
                    _selectedViewIndex = value;
                    OnPropertyChanged("SelectedViewIndex");
                    OnSelectedViewIndexChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les données de la gauche.
        /// </summary>
        public TData[] ItemsLeft
        {
            get { return _itemsLeft; }
            private set
            {
                if (_itemsLeft != value)
                {
                    _itemsLeft = value;
                    OnPropertyChanged("ItemsLeft");
                }
            }
        }

        public TData[] LeftTotal
        {
            get { return _leftTotal; }
            private set
            {
                if (_leftTotal != value)
                {
                    _leftTotal = value;
                    OnPropertyChanged("LeftTotal");
                }
            }
        }

        /// <summary>
        /// Obtient les données de la droite.
        /// </summary>
        public TData[] ItemsRight
        {
            get { return _itemsRight; }
            private set
            {
                if (_itemsRight != value)
                {
                    _itemsRight = value;
                    OnPropertyChanged("ItemsRight");
                }
            }
        }

        public TData[] RightTotal
        {
            get { return _rightTotal; }
            private set
            {
                if (_rightTotal != value)
                {
                    _rightTotal = value;
                    OnPropertyChanged("RightTotal");
                }
            }
        }

        /// <summary>
        /// Obtient le titre de la gauche.
        /// </summary>
        public string TitleLeft
        {
            get { return _titleLeft; }
            private set
            {
                if (_titleLeft != value)
                {
                    _titleLeft = value;
                    OnPropertyChanged("TitleLeft");
                }
            }
        }

        /// <summary>
        /// Obtient le titre de la droite.
        /// </summary>
        public string TitleRight
        {
            get { return _titleRight; }
            private set
            {
                if (_titleRight != value)
                {
                    _titleRight = value;
                    OnPropertyChanged("TitleRight");
                }
            }
        }

        /// <summary>
        /// Obtient La visibilité du sélectionneur de ressource à droite.
        /// </summary>
        public Visibility RightPickerVisibility
        {
            get { return _rightPickerVisibility; }
            private set
            {
                if (_rightPickerVisibility != value)
                {
                    _rightPickerVisibility = value;
                    OnPropertyChanged("RightPickerVisibility");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la ressource sélectionnée.
        /// </summary>
        public Resource SelectedResource
        {
            get { return _selectedResource; }
            set
            {
                if (_selectedResource != value)
                {
                    _selectedResource = value;
                    OnPropertyChanged("SelectedResource");
                    OnSelectedResourceChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les ressources disponibles à la sélection.
        /// </summary>
        public Resource[] Resources
        {
            get { return _resources; }
            private set
            {
                if (_resources != value)
                {
                    _resources = value;
                    OnPropertyChanged("Resources");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la vue et les données sont relatives.
        /// </summary>
        public RestitutionValueMode SelectedValueMode
        {
            get { return _selectedValueMode; }
            set
            {
                if (_selectedValueMode != value)
                {
                    _selectedValueMode = value;
                    OnPropertyChanged("SelectedValueMode");
                    OnRestitutionValueModeChanged();
                }
            }
        }

        private RestitutionValueModeViewModel[] _valueModes;
        /// <summary>
        /// Obtient les modes de valeurs disponibles.
        /// </summary>
        public RestitutionValueModeViewModel[] ValueModes
        {
            get { return _valueModes; }
            private set
            {
                if (_valueModes != value)
                {
                    _valueModes = value;
                    OnPropertyChanged("ValueModes");
                }
            }
        }

        /// <summary>
        /// Obtient le libellé pour tous les opérateurs.
        /// </summary>
        public string AllOperatorsLabel { get; private set; }

        /// <summary>
        /// Obtient le libellé pour tous les équipements.
        /// </summary>
        public string AllEquipmentsLabel { get; private set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public bool OnNavigatingAway()
        {
            return true;
        }

        /// <summary>
        /// Appelé lorsque la sélection des scénarios a changé.
        /// </summary>
        public void OnScenariosSelectionChanged()
        {
            LoadCurrentView();
        }

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Obtient un tableau des différents référentiels utilisés.
        /// </summary>
        /// <param name="leftActionFilter">Le filtre sur les actions de gauche.</param>
        /// <param name="rightActionFilter">Le filtre sur les actions de droite.</param>
        /// <returns>Les référentiels</returns>
        protected abstract TReferential[] GetDistinctReferentials(Func<KAction, bool> leftActionFilter, Func<KAction, bool> rightActionFilter);

        /// <summary>
        /// Détermine si l'action spécifiée utilise le référentiel spécifié.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <param name="referential">Le référentiel.</param>
        /// <returns><c>true</c> si l'action spécifiée utilise le référentiel spécifié.</returns>
        protected abstract bool MatchesReferential(KAction action, TReferential referential);

        /// <summary>
        /// Initialise un nouvel élément.
        /// </summary>
        /// <param name="item">L'élément.</param>
        /// <param name="referential">Le référentiel associé.</param>
        protected virtual void InitializeItem(TDataItem item, TReferential referential)
        {
            string color;

            if (referential == null)
                color = null;
            else
                color = referential.Color;

            Brush fill;
            Brush stroke;
            BrushesHelper.GetBrush(color, false, out fill, out stroke);

            item.FillBrush = fill;
            item.StrokeBrush = stroke;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedViewIndex"/> a changé.
        /// </summary>
        private void OnSelectedViewIndexChanged()
        {
            if (!this.IsLoading)
                this.ParentViewModel.UpdateRestitutionState(this);
            LoadCurrentView();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedResource"/> a changé.
        /// </summary>
        private void OnSelectedResourceChanged()
        {
            if (!this.IsLoading)
                this.ParentViewModel.UpdateRestitutionState(this);
            LoadCurrentView(false);
        }

        /// <summary>
        /// Charge la vue courante
        /// </summary>
        /// <param name="refreshLeftSide"><c>true</c> pour rafraichir la partie gauche.</param>
        private void LoadCurrentView(bool refreshLeftSide = true)
        {
            Func<KAction, bool> leftFilter, rightFilter;
            string titleLeft, titleRight;
            bool showPicker;
            Resource[] resources;

            switch (SelectedViewIndex)
            {
                case 0: // Vue Globale
                    leftFilter = a => a.GetApprovedResource() is Operator;
                    rightFilter = a => a.GetApprovedResource() is Equipment;
                    titleLeft = AllOperatorsLabel;
                    titleRight = AllEquipmentsLabel;
                    showPicker = false;
                    resources = null;
                    break;

                case 1: // Vue par opérateur
                    leftFilter = a => a.GetApprovedResource() is Operator;
                    rightFilter = a => a.GetApprovedResource() == this.SelectedResource;
                    titleLeft = AllOperatorsLabel;
                    titleRight = string.Empty;
                    showPicker = true;
                    resources = GetDistinctResources<Operator>().ToArray();
                    break;

                case 2: // Vue par équipement
                    leftFilter = a => a.GetApprovedResource() is Equipment;
                    rightFilter = a => a.GetApprovedResource() == this.SelectedResource;
                    titleLeft = AllEquipmentsLabel;
                    titleRight = string.Empty;
                    showPicker = true;
                    resources = GetDistinctResources<Equipment>().ToArray();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (refreshLeftSide)
                this.TitleLeft = titleLeft;
            this.TitleRight = titleRight;

            if (refreshLeftSide)
            {
                this.RightPickerVisibility = showPicker ? Visibility.Visible : Visibility.Collapsed;

                this.Resources = resources;
                if (resources != null && !resources.Contains(this.SelectedResource))
                {
                    var pms = base.ServiceBus.Get<IProjectManagerService>();
                    var resourceId = pms.RestitutionState[pms.CurrentProject.ProjectId].ResourceId;
                    if (!resourceId.HasValue)
                        this.SelectedResource = resources.FirstOrDefault();
                    else
                    {
                        this.SelectedResource = resources.FirstOrDefault(r => r.ResourceId == resourceId.Value);
                        if (this.SelectedResource == null)
                            this.SelectedResource = resources.FirstOrDefault();
                    }
                }
            }

            var distinctReferentials = GetDistinctReferentials(leftFilter, rightFilter);

            if (refreshLeftSide)
            {
                TData[] total;
                this.LeftTotal = null;
                this.ItemsLeft = GenerateItems(leftFilter, distinctReferentials, out total).ToArray();
                this.LeftTotal = total;
                UpdateDescriptions(this.ItemsLeft.Concat(this.LeftTotal));
            }
            TData[] totalRight;
            this.RightTotal = null;
            this.ItemsRight = GenerateItems(rightFilter, distinctReferentials, out totalRight).ToArray();
            this.RightTotal = totalRight;
            UpdateDescriptions(this.ItemsRight.Concat(this.RightTotal));
        }

        /// <summary>
        /// Calcule les données pour un côté.
        /// </summary>
        /// <param name="actionFilter">Le filtre sur les actions.</param>
        /// <param name="distinctReferentials">The distinct referentials.</param>
        /// <returns>
        /// Les données créées.
        /// </returns>
        private IEnumerable<TData> GenerateItems(Func<KAction, bool> actionFilter, TReferential[] distinctReferentials, out TData[] total)
        {
            var scenarios = ParentViewModel.ScenariosToShow;

            var dataCol = new List<TData>();

            foreach (var sc in scenarios)
            {
                var data = new TData()
                {
                    Scenario = sc.Label
                };

                var dataItems = new List<TDataItem>();

                foreach (var referential in distinctReferentials)
                {
                    var actions = sc.Actions.
                        Where(actionFilter)
                        .Where(a => MatchesReferential(a, referential) && !a.IsGroup);

                    var actionsNotDeletedCount = actions.Where(a => !ActionsTimingsMoveManagement.IsActionDeleted(a)).Count();

                    var item = new TDataItem()
                    {
                        ReferentialName = referential != null ? referential.Label : LocalizationManager.GetString("ViewModel_Restitution_EmptyReferential"),
                        ReferentialDuration = actions.Any() ? actions.Sum(a => a.BuildDuration) : 0,
                        ReferentialOccurrences = actionsNotDeletedCount,
                        ValueMode = SelectedValueMode,
                        IsStandard = referential is IActionReferentialProcess actionRef && actionRef.ProcessId == null
                    };

                    this.InitializeItem(item, referential);

                    dataItems.Add(item);
                }

                data.Items = dataItems.ToArray();
                dataCol.Add(data);
            }

            // Une fois les items définis, les trier par ordre décroissant de valeurs
            Scenario referenceScenario = scenarios.FirstOrDefault();
            if (referenceScenario != null)
            {
                var data = dataCol.First(d => d.Scenario == referenceScenario.Label);
                data.Items = data.Items.OrderByDescending(d => d.ReferentialDuration).ToArray();

                var comparer = new ReferenceArrayComparer<string>(data.Items.Select(d => d.ReferentialName).ToArray());

                foreach (var d in dataCol.Except(data))
                    d.Items = d.Items.OrderBy(i => i.ReferentialName, comparer).ToArray();
            }

            switch (this.SelectedValueMode)
            {
                case RestitutionValueMode.Absolute:
                    {
                        // Calculer la durée totale du scénario initial
                        long totalDuration = 0;
                        foreach (var data in dataCol)
                        {
                            totalDuration = data.Items.Sum(i => i.ReferentialDuration);
                            if (totalDuration != 0)
                                break;
                        }

                        if (totalDuration != 0)
                            foreach (var data in dataCol)
                            {
                                foreach (var item in data.Items)
                                    item.ReferentialPercentage = Convert.ToDouble(item.ReferentialDuration) / Convert.ToDouble(totalDuration) * 100.0;
                            }
                    }
                    break;

                case RestitutionValueMode.Relative:
                    {
                        foreach (var data in dataCol)
                        {
                            var totalDuration = data.Items.Sum(i => i.ReferentialDuration);

                            if (totalDuration != 0)
                                foreach (var item in data.Items)
                                    item.ReferentialPercentage = Convert.ToDouble(item.ReferentialDuration) / Convert.ToDouble(totalDuration) * 100.0;
                        }
                    }
                    break;

                case RestitutionValueMode.Occurences:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("ValueMode");
            }

            // Calculer les totaux
            total = new TData[scenarios.Length];
            int ii = 0;
            foreach (var sc in scenarios)
            {
                var data = new TData()
                {
                    Scenario = sc.Label
                };

                var item = new TDataItem()
                {
                    ReferentialName = LocalizationManager.GetString("ViewModel_Restitution_Total"),
                    ReferentialDuration = dataCol[ii].Items.Sum(i => i.ReferentialDuration),
                    ReferentialPercentage = dataCol[ii].Items.Sum(i => i.ReferentialPercentage),
                    ReferentialOccurrences = dataCol[ii].Items.Sum(i => i.ReferentialOccurrences),
                    ValueMode = this.SelectedValueMode,
                };

                data.Items = new TDataItem[] { item };
                total[ii] = data;
                ii++;
            }

            return dataCol;
        }

        /// <summary>
        /// Comparateur permettant de trier une collection en utilisant l'odre d'une autre collection contenant les mêmes éléments.
        /// </summary>
        private class ReferenceArrayComparer<T> : IComparer<T>
        {
            private T[] _referenceValues;
            public ReferenceArrayComparer(T[] referenceValues)
            {
                _referenceValues = referenceValues;
            }

            public int Compare(T x, T y)
            {
                if (!_referenceValues.Contains(x) || !_referenceValues.Contains(y))
                    throw new ArgumentException("Les éléments doivent se trouver dans la collection initiale");


                var comp = Array.IndexOf(_referenceValues, x) - Array.IndexOf(_referenceValues, y);
                if (comp < 0)
                    return -1;
                else if (comp > 0)
                    return 1;
                else
                    return 0;
            }
        }


        /// <summary>
        /// Met à jour les descriptions des éléments.
        /// </summary>
        /// <param name="items">Les éléments.</param>
        private void UpdateDescriptions(IEnumerable<TData> items)
        {
            foreach (var item in items)
                foreach (var i in item.Items)
                {
                    i.ReferentialDurationFormatted = _timeService.TicksToString(i.ReferentialDuration);
                    i.ReferentialPercentageFormatted = string.Format("{0:F}%", i.ReferentialPercentage);
                    if (i.ValueMode == RestitutionValueMode.Occurences)
                    {
                        i.Description = string.Format(_descriptionStringFormatOccurences,
                            i.ReferentialName, i.ReferentialDurationFormatted, i.ReferentialOccurrences);
                    }
                    else
                    {
                        i.Description = string.Format(_descriptionStringFormatPercentages,
                            i.ReferentialName, i.ReferentialDurationFormatted, i.ReferentialPercentage);
                    }
                }
        }

        /// <summary>
        /// Obtient les ressources distinctes du type spécifié.
        /// </summary>
        /// <typeparam name="TResource">le type de la ressources.</typeparam>
        /// <returns>Les ressources</returns>
        private IEnumerable<TResource> GetDistinctResources<TResource>()
            where TResource : Resource
        {
            return this.ParentViewModel.ScenariosToShow.SelectMany(a => a.Actions)
                .Where(a => a.Resource != null)
                .Select(a => a.Resource)
                .OfType<TResource>()
                .Distinct();
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="ViewRelative"/> a changé.
        /// </summary>
        private void OnRestitutionValueModeChanged()
        {
            if (!this.IsLoading)
                this.ParentViewModel.UpdateRestitutionState(this);
            this.LoadCurrentView(true);
        }

        #endregion

    }
}
