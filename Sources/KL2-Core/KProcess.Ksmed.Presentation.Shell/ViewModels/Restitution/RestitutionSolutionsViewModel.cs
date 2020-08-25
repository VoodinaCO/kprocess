using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation.ViewModels.Restitution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Restituer - Solutions.
    /// </summary>
    class RestitutionSolutionsViewModel : KsmedViewModelBase, IRestitutionSolutionsViewModel
    {

        #region Champs privés

        private Scenario _scenario;
        private SolutionWrapper[] _solutions;

        #endregion

        #region Surcharges

        /// <summary>
        /// Méthode appelée lors du chargement
        /// </summary>
        protected override Task OnLoading()
        {
            ServiceBus.Get<IProjectManagerService>().ShowScenariosPicker();
            EventBus.Subscribe<ScenarioChangedEvent>(e =>
            {
                if (e.Scenario != null)
                    TryLoadScenario(e.Scenario.Id);
            });

            IProjectManagerService projectService = ServiceBus.Get<IProjectManagerService>();
            if (projectService.CurrentScenario == null)
                projectService.SelectScenario(ParentViewModel.Scenarios.First().Scenario);
            else
                TryLoadScenario(projectService.CurrentScenario.Id);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du viewModel en mode design
        /// </summary>
        protected override Task OnInitializeDesigner()
        {
            Solutions = new SolutionWrapper[]
            {
                new SolutionWrapper(new Solution { SolutionDescription = "Mettre une tablette", Investment = 5, Difficulty = 1, Cost = 1, Approved = true, Comments= "blabla" })
            };
            return Task.CompletedTask;
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();
            base.ServiceBus.Get<IProjectManagerService>().HideScenariosPicker();
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le VM parent.
        /// </summary>
        public IRestitutionViewModel ParentViewModel { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si le sélectionneur de scénario supplémentaire doit être affiché.
        /// </summary>
        public bool ShowAdditionalScenarios
        {
            get { return false; }
        }

        /// <summary>
        /// Obtient les solutions.
        /// </summary>
        public SolutionWrapper[] Solutions
        {
            get { return _solutions; }
            private set
            {
                if (_solutions != value)
                {
                    _solutions = value;
                    OnPropertyChanged("Solutions");
                }
            }
        }
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
            throw new NotSupportedException();
        }

        #endregion

        #region Commandes

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Tente de charger le scénario à l'identifiant spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        private void TryLoadScenario(int scenarioId)
        {
            var old = _scenario;
            var sc = this.ParentViewModel.Scenarios.FirstOrDefault(ss => ss.Scenario.ScenarioId == scenarioId);
            if (sc == null)
            {
                sc = this.ParentViewModel.Scenarios.First();
                base.ServiceBus.Get<IProjectManagerService>().SelectScenario(sc.Scenario);
            }

            LoadScenario(old, sc.Scenario);
        }

        /// <summary>
        /// Charge le scénario spécifié.
        /// </summary>
        private void LoadScenario(Scenario oldScenario, Scenario newScenario)
        {
            if (oldScenario == newScenario)
                return;

            _scenario = newScenario;

            if (oldScenario != null)
            {
                foreach (var solution in oldScenario.Solutions)
                {
                    base.UnregisterToStateChanged(solution);
                    solution.StopTracking();
                }
            }

            if (newScenario != null)
            {
                var solutions = new List<SolutionWrapper>();
                
                // On ne doit pas afficher les solutions du scénario de validation et du scénario initial
                if (newScenario.NatureCode != KnownScenarioNatures.Realized && newScenario.NatureCode != KnownScenarioNatures.Initial)
                {
                    foreach (var solution in newScenario.Solutions.OrderBy(s => s.SolutionDescription))
                    {
                        var w = CreateSolutionWrapper(newScenario.Actions, solution);
                        w.IsNotReadOnly = true;
                        solutions.Add(w);
                    }
                }

                var originalScenario = newScenario.Original;
                while (originalScenario != null)
                {
                    // On ne doit pas afficher le scénario initial
                    if (originalScenario.NatureCode == KnownScenarioNatures.Initial)
                        break;

                    // Déterminer les actions qui sont concernées
                    var originalActions = originalScenario.Actions.Where(originalAction =>
                        newScenario.Actions.Any(currentScenarioAction =>
                            ScenarioActionHierarchyHelper.IsAncestor(originalAction, currentScenarioAction)));

                    foreach (var solution in originalScenario.Solutions.OrderBy(s => s.SolutionDescription))
                    {
                        var wrapper = CreateSolutionWrapper(originalActions, solution);
                        // Ignorer les solutions qui n'apportent pas de gain. C'est un surplus d'infos inutile
                        if (wrapper.Saving != 0)
                            solutions.Add(wrapper);
                    }

                    originalScenario = originalScenario.Original;
                }

                // Définir l'index
                int i = 1;
                foreach (var wrapper in solutions)
                    wrapper.Index = i++;

                this.Solutions = solutions.ToArray();
            }
            else
                this.Solutions = null;
        }

        /// <summary>
        /// Crée un conteneur pour la solution.
        /// </summary>
        /// <param name="actions">Les actions qui peuvent être liées à la solution.</param>
        /// <param name="solution">La solution.</param>
        /// <returns>Le conteneur</returns>
        private SolutionWrapper CreateSolutionWrapper(IEnumerable<KAction> actions, Solution solution)
        {
            var wrapper = new SolutionWrapper(solution);
            wrapper.SetRelatedActions(actions);

            return wrapper;
        }

        #endregion
    }
}
