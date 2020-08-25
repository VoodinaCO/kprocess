using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using DlhSoft.Windows.Controls;
using KProcess.Globalization;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de simulation dans la phase de construction.
    /// </summary>
    abstract class SimulateViewModelBase : FrameContentViewModelBase, ISimulateViewModel
    {

        #region Champs privés

        private Scenario[] _allScenarios;
        private Scenario _selectedOriginalScenario;
        private Scenario[] _availableOriginalScenarios;
        private BulkObservableCollection<GanttChartItem> _selectedOriginalActionItems;
        private Scenario _selectedTargetScenario;
        private BulkObservableCollection<GanttChartItem> _selectedTargetActionItems;
        private long _currentTime;
        private double _ganttHorizontalScrollOffset;
        private INavigationService _navigationService;
        private IESFilter _selectedIESFilter;
        private IESFilter[] _iESFilters;
        private CustomFieldsLabels _customFieldsLabels;

        private GanttActionsManager _originalActionsManager;
        private GanttActionsManager _targetActionsManager;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            this.IESFilters = CreateIESFilters();
            this.SelectedIESFilter = this.IESFilters.First();
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override async Task OnLoading()
        {
            _navigationService = ServiceBus.Get<INavigationService>();
            EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario != null)
                    TryLoadScenario(e.Scenario.Id);
            });
            var projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;
            await LoadData(projectId);

            EventBus?.Publish(new GanttAutoScaleEvent(this));
        }

        /// <summary>
        /// Charge les données en interne.
        /// </summary>
        /// <param name="data">les données.</param>
        protected void LoadDataInternal(Business.Dtos.SimulateData data)
        {
            _allScenarios = data.Scenarios;

            CustomFieldsLabels = new CustomFieldsLabels(data.CustomFieldsLabels);

            var projectService = ServiceBus.Get<IProjectManagerService>();
            if (projectService.CurrentScenario == null || !ScenarioNaturesFilter.Contains(projectService.CurrentScenario.NatureCode))
                projectService.SelectScenario(_allScenarios.First(s => ScenarioNaturesFilter.Contains(s.NatureCode)));
            else
                TryLoadScenario(projectService.CurrentScenario.Id);

            HideSpinner();
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            IESFilters = IESFilter.CreateDefault();
            SelectedIESFilter = IESFilters.First();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Obtient une valeur indiquant si le sélectionneur de scénario est à afficher.
        /// </summary>
        public override bool ShowScenarioPicker
        {
            get { return true; }
        }

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter
        {
            get { return new string[] { KnownScenarioNatures.Initial, KnownScenarioNatures.Target }; }
        }

        #endregion

        #region Propriétés

        public bool IsReadOnly =>
            IsReadOnlyForCurrentUser;

        /// <summary>
        /// Obtient ou définit le scénario d'origine sélectionné.
        /// </summary>
        public Scenario SelectedOriginalScenario
        {
            get { return _selectedOriginalScenario; }
            set
            {
                if (_selectedOriginalScenario != value)
                {
                    var old = _selectedOriginalScenario;
                    _selectedOriginalScenario = value.MapApprovedReduction();
                    OnPropertyChanged("SelectedOriginalScenario");
                    OnSelectedOriginalScenarioChanged(old, value);
                }
            }
        }

        /// <summary>
        /// Obtient les éléments du gantt du scénario original sélectionné.
        /// </summary>
        public BulkObservableCollection<GanttChartItem> SelectedOriginalActionItems
        {
            get { return _selectedOriginalActionItems; }
            private set
            {
                if (_selectedOriginalActionItems != value)
                {
                    _selectedOriginalActionItems = value;
                    OnPropertyChanged("SelectedOriginalActionItems");
                }
            }
        }

        private GanttChartItem _currentTargetGridItem;
        /// <summary>
        /// Obtient ou définit l'élément courant de la grille.
        /// </summary>
        public GanttChartItem CurrentTargetGridItem
        {
            get { return _currentTargetGridItem; }
            set
            {
                if (_currentTargetGridItem != value)
                {
                    var previous = _currentTargetGridItem;
                    _currentTargetGridItem = value;
                    OnCurrentTargetGridItemChanged(previous, _currentTargetGridItem);
                    OnPropertyChanged("CurrentTargetGridItem");
                }
            }
        }

        /// <summary>
        /// Obtient les scénarios originaux disponibles.
        /// </summary>
        public Scenario[] AvailableOriginalScenarios
        {
            get { return _availableOriginalScenarios; }
            private set
            {
                if (_availableOriginalScenarios != value)
                {
                    _availableOriginalScenarios = value;
                    OnPropertyChanged("AvailableOriginalScenarios");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le scénario cible sélectionné.
        /// </summary>
        public Scenario SelectedTargetScenario
        {
            get { return _selectedTargetScenario; }
            set
            {
                if (_selectedTargetScenario != value)
                {
                    var old = _selectedTargetScenario;
                    _selectedTargetScenario = value.MapApprovedReduction();
                    OnPropertyChanged("SelectedTargetScenario");
                    OnSelectedTargetScenarioChanged(old, value);
                }
            }
        }

        /// <summary>
        /// Obtient les éléments d'actions de la cible sélectionnée.
        /// </summary>
        public BulkObservableCollection<GanttChartItem> SelectedTargetActionItems
        {
            get { return _selectedTargetActionItems; }
            private set
            {
                if (_selectedTargetActionItems != value)
                {
                    _selectedTargetActionItems = value;
                    OnPropertyChanged("SelectedTargetActionItems");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le temps actuel.
        /// </summary>
        public long CurrentTimelinePosition
        {
            get { return _currentTime; }
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged("CurrentTimelinePosition");
                    OnCurrentTimelinePositionChanged();
                }
            }
        }

        /// <summary>
        /// Obtient le décalage dans les scroll viewers.
        /// </summary>
        public double GanttHorizontalScrollOffset
        {
            get { return _ganttHorizontalScrollOffset; }
            set
            {
                if (_ganttHorizontalScrollOffset != value)
                {
                    _ganttHorizontalScrollOffset = value;
                    OnPropertyChanged("GanttHorizontalScrollOffset");
                }
            }
        }

        /// <summary>
        /// Obtient les filtres IES disponibles.
        /// </summary>
        public IESFilter[] IESFilters
        {
            get { return _iESFilters; }
            private set
            {
                if (_iESFilters != value)
                {
                    _iESFilters = value;
                    OnPropertyChanged("IESFilters");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le filtre IES défini.
        /// </summary>
        public IESFilter SelectedIESFilter
        {
            get { return _selectedIESFilter; }
            set
            {
                if (_selectedIESFilter != value)
                {
                    _selectedIESFilter = value;
                    OnPropertyChanged("SelectedIESFilter");
                    OnSelectedIESFilterChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les libellés des champs libres texte.
        /// </summary>
        public CustomFieldsLabels CustomFieldsLabels
        {
            get { return _customFieldsLabels; }
            protected set
            {
                if (_customFieldsLabels != value)
                {
                    _customFieldsLabels = value;
                    OnPropertyChanged("CustomFieldsLabels");
                }
            }
        }

        #endregion

        #region Commandes

        #endregion

        #region Méthodes protégées

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected abstract IESFilter[] CreateIESFilters();

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected abstract Task LoadData(int projectId);

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Tente de charger le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        private void TryLoadScenario(int scenarioId)
        {
            var old = _selectedOriginalScenario;
            var sc = _allScenarios.FirstOrDefault(s => s.ScenarioId == scenarioId);
            if (sc == null)
            {
                sc = _allScenarios.First();
                base.ServiceBus.Get<IProjectManagerService>().SelectScenario(_allScenarios.First());
            }

            this.SelectedTargetScenario = sc;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedOriginalScenario"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancien scénario.</param>
        /// <param name="newValue">Le nouveau scénario.</param>
        private void OnSelectedOriginalScenarioChanged(Scenario oldValue, Scenario newValue)
        {
            LoadOriginalScenario(oldValue, newValue);
            EventBus?.Publish(new GanttAutoScaleEvent(this));
        }

        /// <summary>
        /// Met à jour les ratios d'amélioration.
        /// </summary>
        private void UpdateOriginalRatio()
        {
            if (this.SelectedOriginalScenario != null && this.SelectedTargetScenario != null)
            {
                _originalActionsManager.UpdateActionsOriginalRatio(this.SelectedOriginalScenario.Actions, this.SelectedTargetScenario.Actions);
                _targetActionsManager.UpdateActionsHeaderWithNoOriginal(this.SelectedTargetScenario.Actions);
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedTargetScenario"/> a changé.
        /// </summary>
        /// <param name="oldValue">L'ancien scénario.</param>
        /// <param name="newValue">Le nouveau scénario.</param>
        private void OnSelectedTargetScenarioChanged(Scenario oldValue, Scenario newValue)
        {
            LoadTargetScenario(oldValue, newValue);
            EventBus?.Publish(new GanttAutoScaleEvent(this));
        }

        /// <summary>
        /// Restaure la sélection de l'action à partir des préférences de navigation.
        /// </summary>
        protected void RestoreActionSelection()
        {
            if (_navigationService != null && _navigationService.Preferences.ActionId.HasValue)
            {
                var firstMatch = this.SelectedTargetActionItems.OfType<ActionGanttItem>().FirstOrDefault(a => a.Action.ActionId == _navigationService.Preferences.ActionId);
                if (firstMatch != null)
                    this.CurrentTargetGridItem = firstMatch;
            }
        }

        /// <summary>
        /// Appelé lorsque l'élément courant de la grille a changé.
        /// </summary>
        /// <param name="previousValue">La valeur précédante.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        private void OnCurrentTargetGridItemChanged(GanttChartItem previousValue, GanttChartItem newValue)
        {
            var actionitem = newValue as ActionGanttItem;

            _navigationService.Preferences.ActionId =
                actionitem != null && actionitem.Action.ActionId != default(int) ?
                actionitem.Action.ActionId :
                (int?)null;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentTimelinePosition"/> a changé.
        /// </summary>
        private void OnCurrentTimelinePositionChanged()
        {
            _navigationService.Preferences.TimelinePosition = this.CurrentTimelinePosition;
        }

        /// <summary>
        /// Charge le scénario d'origine.
        /// </summary>
        protected virtual void LoadOriginalScenario(Scenario oldValue, Scenario newValue)
        {
            if (this.SelectedOriginalScenario != null)
            {
                this.SelectedOriginalActionItems = new BulkObservableCollection<GanttChartItem>();
                _originalActionsManager = new GanttActionsManager(this.SelectedOriginalActionItems, null, null);
                _originalActionsManager.ChangeView(GanttGridView.WBS, null);

                _originalActionsManager.EnableReducedPercentageRefresh = false;

                _originalActionsManager.RegisterInitialActions(this.SelectedOriginalScenario.Actions, this.SelectedIESFilter.Value);

                UpdateOriginalRatio();
            }
            else
            {
                if (this.SelectedOriginalActionItems != null)
                    this.SelectedOriginalActionItems.Clear();
                if (_originalActionsManager != null)
                    _originalActionsManager.Clear();
            }
        }

        /// <summary>
        /// Charge le scénario cible.
        /// </summary>
        protected virtual void LoadTargetScenario(Scenario oldValue, Scenario newValue)
        {
            if (this.SelectedTargetScenario != null)
            {
                if (this.SelectedTargetActionItems != null)
                    this.SelectedTargetActionItems.Clear();
                else
                    this.SelectedTargetActionItems = new BulkObservableCollection<GanttChartItem>();

                if (_targetActionsManager != null)
                    _targetActionsManager.Clear();
                else
                {
                    _targetActionsManager = new GanttActionsManager(this.SelectedTargetActionItems, null, null);
                    _targetActionsManager.ChangeView(GanttGridView.WBS, null);
                }

                var oldOriginalScenario = this.SelectedOriginalScenario;
                this.SelectedOriginalScenario = null;
                this.AvailableOriginalScenarios = null;

                var originals = new List<Scenario>();
                var original = this.SelectedTargetScenario.Original;
                while (original != null)
                {
                    originals.Add(original);
                    original = original.Original;
                }

                this.AvailableOriginalScenarios = originals.ToArray();

                this.SelectedOriginalScenario = this.AvailableOriginalScenarios.FirstOrDefault();

                _targetActionsManager.EnableReducedPercentageRefresh = false;

                _targetActionsManager.RegisterInitialActions(this.SelectedTargetScenario.Actions, this.SelectedIESFilter.Value);

                RestoreActionSelection();

                if (_navigationService != null && _navigationService.Preferences.TimelinePosition.HasValue)
                    this.CurrentTimelinePosition = _navigationService.Preferences.TimelinePosition.Value;

                UpdateOriginalRatio();
            }
            else
            {
                if (this.SelectedTargetActionItems != null)
                    this.SelectedTargetActionItems.Clear();
                if (_targetActionsManager != null)
                    _targetActionsManager.Clear();
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedIESFilter"/> a changé.
        /// </summary>
        private void OnSelectedIESFilterChanged()
        {
            this.LoadOriginalScenario(this.SelectedOriginalScenario, this.SelectedOriginalScenario);
            this.LoadTargetScenario(this.SelectedTargetScenario, this.SelectedTargetScenario);

        }

        #endregion

    }
}