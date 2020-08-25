using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using System.Collections.Generic;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran Analyser - Restituer.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IRestitutionViewModel : IViewModel, IFrameContentViewModel
    {

        /// <summary>
        /// Obtient ou définit les catégories d'actions.
        /// </summary>
        ActionCategory ActionCategories { get; }

        /// <summary>
        /// Obtient la vue courante.
        /// </summary>
        IView CurrentView { get; }

        /// <summary>
        /// Obtient les scénarios.
        /// </summary>
        ScenarioSelection[] Scenarios { get; }

        /// <summary>
        /// Obtient les scénarios à afficher.
        /// </summary>
        Scenario[] ScenariosToShow { get; }

        /// <summary>
        /// Obtient ou définit la vue sélectionnée
        /// </summary>
        string SelectedView { get; set; }

        /// <summary>
        /// Obtient les vues.
        /// </summary>
        string[] Views { get; }

        /// <summary>
        /// Rafraichît l'affichage des erreurs de validation pour les objets spécifiés.
        /// </summary>
        /// <param name="models">Les modèles.</param>
        void RefreshValidationErrors(IEnumerable<ValidatableObject> models);

        /// <summary>
        /// Met à jour l'état de la synthèse.
        /// </summary>
        /// <typeparam name="TReferential">Le type de référentiel.</typeparam>
        /// <param name="vm">Le viewModel.</param>
        void UpdateRestitutionState<TReferential>(IRestitutionViewByResourceViewModel<TReferential> vm)
            where TReferential : IActionReferential;

        /// <summary>
        /// Met à jour l'état de la synthèse.
        /// </summary>
        /// <param name="solutionsVm">Le viewModel</param>
        void UpdateRestitutionState(IRestitutionSolutionsViewModel solutionsVm);

    }

    /// <summary>
    /// Représente un scénario sélectionnable.
    /// </summary>
    public class ScenarioSelection : NotifiableObject
    {
        public delegate void OnIsSelectedChangedDelegate(ScenarioSelection scenario, bool isShownInSummary);

        readonly OnIsSelectedChangedDelegate _onIsSelectedChanged;

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        /// <param name="isSelected"><c>true</c> si le scénario est sélectionné.</param>
        /// <param name="onIsSelectedChanged">Un callback à appeler lorsque l'état sélectionné change.</param>
        public ScenarioSelection(Scenario scenario, bool isSelected, OnIsSelectedChangedDelegate onIsSelectedChanged)
        {
            Scenario = scenario;
            _isSelected = isSelected;
            OnPropertyChanged(nameof(IsSelected));

            _onIsSelectedChanged = onIsSelectedChanged;
        }

        /// <summary>
        /// Obtient le scénario.
        /// </summary>
        public Scenario Scenario { get; private set; }

        bool _isSelected;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le scénario est sélectionné.
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
                    _onIsSelectedChanged(this, value);
                }
            }
        }
    }
}