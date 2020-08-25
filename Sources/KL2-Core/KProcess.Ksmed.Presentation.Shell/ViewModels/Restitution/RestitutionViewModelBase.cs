using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Analyser - Restituer.
    /// </summary>
    abstract class RestitutionViewModelBase : FrameContentViewModelBase, IRestitutionViewModel
    {
        #region Champs privés

        ISubRestitutionViewModel _currentViewModel;
        IView _currentView;
        Dictionary<string, Action> _viewsFactory;
        string[] _views;
        string _selectedView;
        readonly string _solutionsViewKey = LocalizationManagerExt.GetSafeDesignerString("View_AnalyzeRestitution_Solution_Title");
        Scenario[] _allScenarios;
        RestitutionState _restitutionState;
        int _currentProjectId;

        #endregion

        #region Surcharges

        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            CreateMenu();
        }

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading()
        {
            IProjectManagerService pms = ServiceBus.Get<IProjectManagerService>();
            _currentProjectId = pms.CurrentProject.ProjectId;

            if (!pms.RestitutionState.TryGetValue(_currentProjectId, out _restitutionState))
            {
                _restitutionState = new RestitutionState
                {
                    Referential = ProcessReferentialIdentifier.Category,
                    RestitutionValueMode = (int)Core.Behaviors.RestitutionValueMode.Absolute,
                    ViewMode = RestitutionStateViewMode.Global,
                };
                pms.RestitutionState[_currentProjectId] = _restitutionState;
            }

            return LoadData(true);
        }

        /// <summary>
        /// Appelé afin de charger les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected abstract Task<RestitutionData> OnLoadData(int projectId);

        /// <summary>
        /// Charge les données.
        /// </summary>
        async Task LoadData(bool firstLoad)
        {
            int projectId = ServiceBus.Get<IProjectManagerService>().CurrentProject.ProjectId;

            ScenarioDescription currentScenario = ServiceBus.Get<IProjectManagerService>().CurrentScenario;

            ShowSpinner();

            try
            {
                RestitutionData data = await OnLoadData(projectId);

                _allScenarios = data.Scenarios;
                Scenarios = data.Scenarios
                    .Where(s => !s.IsDeleted)
                    .Select(s => new ScenarioSelection(s, s.IsShownInSummary, OnScenarioIsShownInSummaryChanged)).ToArray();

                UdpateScenariosToShow();

                if (SelectedView == null)
                {
                    if (_restitutionState.Solutions)
                        SelectedView = _solutionsViewKey;
                    else
                    {
                        string key = ReferentialsUse.GetLabel(_restitutionState.Referential.Value);
                        SelectedView = key;
                    }
                }
                else
                    await _currentViewModel?.Refresh();

                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Views = new string[] { "Occupation", "Lieux" };
            SelectedView = Views[0];

            return Task.CompletedTask;
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            _currentViewModel?.Shutdown();
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient la vue courante.
        /// </summary>
        public IView CurrentView
        {
            get { return _currentView; }
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
            }
        }

        ScenarioSelection[] _scenarios;
        /// <summary>
        /// Obtient les scénarios à afficher.
        /// </summary>
        public ScenarioSelection[] Scenarios
        {
            get { return _scenarios; }
            private set
            {
                if (_scenarios != value)
                {
                    _scenarios = value;
                    OnPropertyChanged();
                }
            }
        }


        Scenario[] _scenariosToShow;
        /// <summary>
        /// Obtient les scénarios à afficher.
        /// </summary>
        public Scenario[] ScenariosToShow
        {
            get { return _scenariosToShow; }
            private set
            {
                if (_scenariosToShow != value)
                {
                    _scenariosToShow = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient les catégories d'actions.
        /// </summary>
        public ActionCategory ActionCategories { get; private set; }

        /// <summary>
        /// Obtient les vues.
        /// </summary>
        public string[] Views
        {
            get { return _views; }
            private set
            {
                if (_views != value)
                {
                    _views = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée
        /// </summary>
        public string SelectedView
        {
            get { return _selectedView; }
            set
            {
                if (_selectedView != value)
                {
                    _selectedView = value;
                    OnPropertyChanged();
                    OnSelectedViewChanged();
                }
            }
        }


        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Rafraichît l'affichage des erreurs de validation pour les objets spécifiés.
        /// </summary>
        /// <param name="models">Les modèles.</param>
        public new void RefreshValidationErrors(IEnumerable<ValidatableObject> models) =>
            base.RefreshValidationErrors(models);

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token) =>
            Task.FromResult(_currentViewModel.OnNavigatingAway());

        /// <summary>
        /// Met à jour l'état de la synthèse.
        /// </summary>
        /// <typeparam name="TReferential">Le type de référentiel.</typeparam>
        /// <param name="vm">Le viewModel.</param>
        public void UpdateRestitutionState<TReferential>(IRestitutionViewByResourceViewModel<TReferential> vm)
            where TReferential : IActionReferential
        {
            ProcessReferentialIdentifier id = ReferentialsHelper.GetIdentifier<TReferential>();

            _restitutionState.Referential = id;
            _restitutionState.Solutions = false;
            _restitutionState.RestitutionValueMode = (int)vm.SelectedValueMode;

            switch (vm.SelectedViewIndex)
            {
                case 0: // Vue Globale
                    _restitutionState.ViewMode = RestitutionStateViewMode.Global;
                    _restitutionState.ResourceId = null;
                    break;

                case 1: // Vue par opérateur
                    _restitutionState.ViewMode = RestitutionStateViewMode.PerOperator;
                    _restitutionState.ResourceId = vm.SelectedResource != null ? (int?)vm.SelectedResource.Id : null;
                    break;

                case 2: // Vue par équipement
                    _restitutionState.ViewMode = RestitutionStateViewMode.PerEquipment;
                    _restitutionState.ResourceId = vm.SelectedResource != null ? (int?)vm.SelectedResource.Id : null;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(vm));
            }

            ServiceBus.Get<IProjectManagerService>().RestitutionState[_currentProjectId] = _restitutionState;
        }

        /// <summary>
        /// Met à jour l'état de la synthèse.
        /// </summary>
        /// <param name="solutionsVm">Le viewModel</param>
        public void UpdateRestitutionState(IRestitutionSolutionsViewModel solutionsVm)
        {
            _restitutionState.Referential = null;
            _restitutionState.Solutions = true;

            _restitutionState.ViewMode = null;
            _restitutionState.ResourceId = null;

            ServiceBus.Get<IProjectManagerService>().RestitutionState[_currentProjectId] = _restitutionState;
        }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute() =>
            _currentViewModel.CancelCommand.CanExecute(null);

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute() =>
            _currentViewModel.CancelCommand.Execute(null);

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute() =>
            _currentViewModel.ValidateCommand.CanExecute(null);

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override void OnValidateCommandExecute() =>
            _currentViewModel.ValidateCommand.Execute(null);

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crées le menu
        /// </summary>
        void CreateMenu()
        {
            if (!DesignMode.IsInDesignMode)
            {
                _viewsFactory = new Dictionary<string, Action>();

                IReferentialsUseService refUseService = IoC.Resolve<IReferentialsUseService>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Category))
                    _viewsFactory[ReferentialsUse.Category] = () => ShowSubMenuByResource<IRestitutionOccupationViewModel, ActionCategory>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref1))
                    _viewsFactory[ReferentialsUse.Ref1] = () => ShowSubMenuByResource<IRestitutionRef1ViewModel, Ref1>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref2))
                    _viewsFactory[ReferentialsUse.Ref2] = () => ShowSubMenuByResource<IRestitutionRef2ViewModel, Ref2>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref3))
                    _viewsFactory[ReferentialsUse.Ref3] = () => ShowSubMenuByResource<IRestitutionRef3ViewModel, Ref3>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref4))
                    _viewsFactory[ReferentialsUse.Ref4] = () => ShowSubMenuByResource<IRestitutionRef4ViewModel, Ref4>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref5))
                    _viewsFactory[ReferentialsUse.Ref5] = () => ShowSubMenuByResource<IRestitutionRef5ViewModel, Ref5>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref6))
                    _viewsFactory[ReferentialsUse.Ref6] = () => ShowSubMenuByResource<IRestitutionRef6ViewModel, Ref6>();

                if (refUseService.IsReferentialEnabled(ProcessReferentialIdentifier.Ref7))
                    _viewsFactory[ReferentialsUse.Ref7] = () => ShowSubMenuByResource<IRestitutionRef7ViewModel, Ref7>();

                _viewsFactory[_solutionsViewKey] = () =>
                {
                    IView view = CreateViewModel<IRestitutionSolutionsViewModel>();

                    _restitutionState.Referential = null;
                    _restitutionState.ResourceId = null;
                    _restitutionState.Solutions = true;
                    _restitutionState.ViewMode = null;

                    ServiceBus.Get<IProjectManagerService>().RestitutionState[_currentProjectId] = _restitutionState;

                    _currentViewModel.Load();
                    CurrentView = view;
                };

                Views = _viewsFactory.Keys.ToArray();
            }
        }

        /// <summary>
        /// Affiche le sous menu de vue par ressource spécifié.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TReferential"></typeparam>
        void ShowSubMenuByResource<TViewModel, TReferential>()
            where TViewModel : IRestitutionViewByResourceViewModel<TReferential>
            where TReferential : IActionReferential
        {
            IView view = CreateViewModel<TViewModel>();

            ProcessReferentialIdentifier id = ReferentialsHelper.GetIdentifier<TReferential>();
            if (_restitutionState.Referential != id)
            {
                _restitutionState.Referential = id;
                _restitutionState.ResourceId = null;
                _restitutionState.Solutions = false;
                _restitutionState.ViewMode = RestitutionStateViewMode.Global;

                ServiceBus.Get<IProjectManagerService>().RestitutionState[_currentProjectId] = _restitutionState;
            }

            _currentViewModel.Load();
            CurrentView = view;
        }

        /// <summary>
        /// Crée le VM spécifié.
        /// </summary>
        /// <typeparam name="TViewModel">Le type du VM.</typeparam>
        IView CreateViewModel<TViewModel>()
            where TViewModel : ISubRestitutionViewModel
        {
            _currentViewModel?.Shutdown();

            IView view = UXFactory.GetView(out TViewModel vm);
            _currentViewModel = vm;
            vm.ParentViewModel = this;
            return view;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="SelectedView"/> a changé.
        /// </summary>
        void OnSelectedViewChanged()
        {
            if (!DesignMode.IsInDesignMode)
                _viewsFactory[SelectedView]();
        }

        /// <summary>
        /// Appelé lorsque l'affichage dans la synthèse d'un scénario a changé.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="isShownInSummary"><c>true</c> pour l'afficher dans la synthèse.</param>
        async void OnScenarioIsShownInSummaryChanged(ScenarioSelection scenario, bool isShownInSummary)
        {
            UdpateScenariosToShow();

            ShowSpinner();
            try
            {
                var updatedScenario = await ServiceBus.Get<IAnalyzeService>().UpdateScenarioIsShownInSummary(scenario.Scenario.ScenarioId, isShownInSummary);
                Scenarios.Single(_ => _.Scenario.ScenarioId == updatedScenario.ScenarioId).Scenario.AcceptChanges();
                HideSpinner();
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Met à jour la liste des scénarios à afficher.
        /// </summary>
        void UdpateScenariosToShow()
        {
            ScenariosToShow = Scenarios.Where(ss => ss.IsSelected).Select(ss => ss.Scenario.MapApprovedReduction()).ToArray();

            _currentViewModel?.OnScenariosSelectionChanged();
        }

        #endregion

    }
}