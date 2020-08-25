using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Valider- Simuler/comparer.
    /// </summary>
    class ValidateSimulateViewModel : SimulateViewModelBase, IValidateSimulateViewModel
    {

        #region Champs privés

        private bool _isReadOnly;
        private bool _canChange = true;

        /// <summary>
        /// Si définit, une navigation se produit à l'issue d'une sauvegarde
        /// </summary>
        private IFrameNavigationToken _navigationToken = null;

        #endregion

        #region Surcharges

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter
        {
            get { return new string[] { KnownScenarioNatures.Realized }; }
        }

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected override async Task LoadData(int projectId)
        {
            ShowSpinner();
            try
            {
                SimulateData data = await ServiceBus.Get<IValidateService>().GetSimulateData(projectId);
                LoadDataInternal(data);
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected override void OnEntityStateChanged(ObjectState newState)
        {
            CanChange &= newState == ObjectState.Unchanged;
        }

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la navigation est acceptée.
        /// </returns>
        public override Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            if (!CanChangeAndHasNoPendingChanges())
            {
                MessageDialogResult answer = this.AskNavigatingAwayValidationConfirmation(() => _navigationToken = token);
                return Task.FromResult(answer == MessageDialogResult.No);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected override IESFilter[] CreateIESFilters()
        {
            return IESFilter.CreateWithoutIES();
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();
            UnregisterScenario(this.SelectedOriginalScenario);
            UnregisterScenario(this.SelectedTargetScenario);
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient une valeur indiquant si la sélection peut être changée.
        /// </summary>
        public virtual bool CanChange
        {
            get { return _canChange; }
            protected set
            {
                if (_canChange != value)
                {
                    _canChange = value;
                    OnPropertyChanged("CanChange");
                    OnCanChangeChanged();
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant est en lecture seule.
        /// </summary>
        public new bool IsReadOnly
        {
            get { return _isReadOnly; }
            protected set
            {
                if (_isReadOnly != value)
                {
                    _isReadOnly = value;
                    OnPropertyChanged("IsReadOnly");
                    OnPropertyChanged("IsNotReadOnly");
                }
            }
        }

        /// <summary>
        /// Obtient une valeur indiquant si le scénario courant n'est pas en lecture seule.
        /// </summary>
        public bool IsNotReadOnly
        {
            get { return !IsReadOnly; }
        }

        #endregion

        #region Commandes

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande ValidateCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnValidateCommandCanExecute()
        {
            return this.IsNotReadOnly;
        }

        /// <summary>
        /// Callback utilisé pour vérifier la capacité d'exécution de la commande CancelCommand
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la commande peut être executée.
        /// </returns>
        protected override bool OnCancelCommandCanExecute()
        {
            return !this.CanChange;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande CancelCommand
        /// </summary>
        protected override void OnCancelCommandExecute()
        {
            this.HideValidationErrors();

            var t = this.SelectedTargetScenario;
            this.SelectedTargetScenario = null;

            var o = this.SelectedOriginalScenario;
            this.SelectedOriginalScenario = null;

            // Détecter les entités nouvelles et les supprimer

            ObjectWithChangeTrackerExtensions.CancelChanges(
                new Scenario[] { t },
                t.Actions
                );

            this.SelectedTargetScenario = t;
            this.SelectedOriginalScenario = o;

            this.CanChange = true;
        }

        /// <summary>
        /// Callback utilisé lors de l'exécution de la commande ValidateCommand
        /// </summary>
        protected override async void OnValidateCommandExecute()
        {
            // Valider l'objet
            if (!ValidateData())
                return;

            if (await SaveActions(true))
            {
                CanChange = true;
                if (_navigationToken?.IsValid == true)
                    _navigationToken.Navigate();
            }
        }

        #endregion

        #region Méthodes privés

        /// <summary>
        /// Valide les actions qui ont changé.
        /// </summary>
        /// <returns><c>true</c> si les actions sont valides.</returns>
        private bool ValidateData()
        {
            var changedActions = this.SelectedTargetScenario.Actions
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            foreach (var action in changedActions)
                action.Validate();

            var allChanged = new List<ValidatableObject>(changedActions);

            base.RefreshValidationErrors(allChanged);

            if (allChanged.Any(a => !a.IsValid.GetValueOrDefault()))
            {
                // Active la validation auto
                foreach (var item in allChanged)
                    item.EnableAutoValidation = true;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Enregistre les actions modifiées.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        private async Task<bool> SaveActions(bool refreshSelectionWhenDone)
        {
            ShowSpinner();

            try
            {
                await ServiceBus.Get<IValidateService>().SaveSimulateData(SelectedTargetScenario);
                if (refreshSelectionWhenDone)
                    await OnLoading();

                foreach (var action in SelectedTargetScenario.Actions)
                    action.StartTracking();

                HideSpinner();
                return true;
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Détermine si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde.
        /// </summary>
        /// <returns>
        ///   <c>true</c> si la sélection peut être changée et s'il n'y pas de modifications en attente de sauvegarde; sinon, <c>false</c>.
        /// </returns>
        protected virtual bool CanChangeAndHasNoPendingChanges()
        {
            if (this.IsReadOnly)
                return true;

            var changedActions = this.SelectedTargetScenario.Actions
                .Where(a => a.IsNotMarkedAsUnchanged)
                .ToArray();

            if (changedActions.Any())
                return false;
            else
                return true;
        }

        /// <summary>
        /// Charge le scénario d'origine.
        /// </summary>
        protected override void LoadOriginalScenario(Scenario oldScenario, Scenario newScenario)
        {
            this.RegisterScenarios(oldScenario);

            base.LoadOriginalScenario(oldScenario, newScenario);
        }

        /// <summary>
        /// Charge le scénario cible.
        /// </summary>
        protected override void LoadTargetScenario(Scenario oldScenario, Scenario newScenario)
        {
            UpdateIsReadOnly();

            this.RegisterScenarios(oldScenario);

            base.LoadTargetScenario(oldScenario, newScenario);
        }

        /// <summary>
        /// S'abonne aux changements sur les scénarios chargés.
        /// </summary>
        /// <param name="oldScenario">L'ancien scénario.</param>
        private void RegisterScenarios(Scenario oldScenario)
        {
            this.UnregisterScenario(this.SelectedTargetScenario);
            this.UnregisterScenario(this.SelectedOriginalScenario);

            UpdateDifferenceReasons();

            if (this.SelectedTargetScenario != null && this.SelectedOriginalScenario != null)
                foreach (var action in this.SelectedTargetScenario.Actions.Concat(this.SelectedOriginalScenario.Actions))
                {
                    action.StartTracking();
                    base.RegisterToStateChanged(action);
                    action.DifferenceReasonManagedChanged += new EventHandler(OnActionDifferenceReasonManagedChanged);
                }

        }

        /// <summary>
        /// Se désabonne aux changements sur le scénario spécifié.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        private void UnregisterScenario(Scenario scenario)
        {
            if (scenario == null)
                return;

            foreach (var action in scenario.Actions)
            {
                action.StopTracking();
                base.UnregisterToStateChanged(action);
                action.DifferenceReasonManagedChanged -= new EventHandler(OnActionDifferenceReasonManagedChanged);
            }
        }

        /// <summary>
        /// Obtient la source de la cause des écarts.
        /// </summary>
        /// <param name="action">L'action où la cause des écarts a changé.</param>
        /// <param name="isFromValidationScenario"><c>true</c> si l'action provient du scénario de validation. <c>false</c> si elle provient du scénario cible.</param>
        /// <param name="targetAction">L'action du scénario cible.</param>
        /// <param name="validationAction">L'action du scénario de validation.</param>
        /// <returns></returns>
        private DifferenceReasonSource GetDifferenceReasonSource(KAction action, out bool isFromValidationScenario,
            out KAction targetAction, out KAction validationAction)
        {
            isFromValidationScenario = this.SelectedTargetScenario != null && this.SelectedTargetScenario.Actions.Contains(action);

            if (isFromValidationScenario)
            {
                validationAction = action;
                targetAction = ScenarioActionHierarchyHelper.GetAncestorAction(action, this.SelectedOriginalScenario);
                if (targetAction == null)
                    return DifferenceReasonSource.FromSourceScenarioNew;
                else
                    return DifferenceReasonSource.FromBoth;
            }
            else
            {
                targetAction = action;
                validationAction = ScenarioActionHierarchyHelper.GetDerivedAction(action, this.SelectedTargetScenario);
                if (validationAction == null)
                    return DifferenceReasonSource.FromTargetScenarioDeleted;
                else
                    return DifferenceReasonSource.FromBoth;
            }
        }

        /// <summary>
        /// Met à jour l'état lecture seule du scénario.
        /// </summary>
        private void UpdateIsReadOnly()
        {
            this.IsReadOnly =
                (this.SelectedTargetScenario != null &&
                    base.ServiceBus.Get<IProjectManagerService>().Scenarios.First(sc => sc.Id == base.SelectedTargetScenario.ScenarioId).IsLocked) ||
                !base.CanCurrentUserWrite;
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CanChange"/> a changé.
        /// </summary>
        private void OnCanChangeChanged()
        {
            base.IsScenarioPickerEnabled = this.CanChange;
        }

        /// <summary>
        /// Met à jour les libellés des causes d'écarts sur les actions.
        /// </summary>
        private void UpdateDifferenceReasons()
        {
            if (this.SelectedOriginalScenario != null && this.SelectedTargetScenario != null)
            {
                foreach (var action in this.SelectedOriginalScenario.Actions.Concat(
                    this.SelectedTargetScenario.Actions))
                    action.DifferenceReasonManaged = action.DifferenceReason;

                foreach (var action in this.SelectedOriginalScenario.Actions)
                {
                    var derived = ScenarioActionHierarchyHelper.GetDerivedAction(action, this.SelectedTargetScenario);
                    if (derived != null && action.DifferenceReason != null)
                        derived.DifferenceReasonManaged = action.DifferenceReason;
                }

                foreach (var action in this.SelectedTargetScenario.Actions)
                {
                    var ancestor = ScenarioActionHierarchyHelper.GetAncestorAction(action, this.SelectedOriginalScenario);
                    if (ancestor != null && action.DifferenceReason != null)
                        ancestor.DifferenceReasonManaged = action.DifferenceReason;
                }
            }
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="DifferenceReasonManaged"/> a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void OnActionDifferenceReasonManagedChanged(object sender, EventArgs e)
        {
            var action = (KAction)sender;

            bool isFromValidationScenario;
            KAction targetAction, validationAction;
            var source = GetDifferenceReasonSource(action, out isFromValidationScenario, out targetAction, out validationAction);

            switch (source)
            {
                case DifferenceReasonSource.FromTargetScenarioDeleted:
                    targetAction.DifferenceReason = action.DifferenceReasonManaged;
                    break;
                case DifferenceReasonSource.FromSourceScenarioNew:
                    validationAction.DifferenceReason = action.DifferenceReasonManaged;
                    break;
                case DifferenceReasonSource.FromBoth:
                    targetAction.DifferenceReasonManaged = action.DifferenceReasonManaged;
                    validationAction.DifferenceReason = action.DifferenceReasonManaged;
                    validationAction.DifferenceReasonManaged = action.DifferenceReasonManaged;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("source");
            }
        }

        #endregion

        #region Types


        /// <summary>
        /// La source de la cause des écarts.
        /// </summary>
        private enum DifferenceReasonSource
        {
            /// <summary>
            /// Provient d'une tâches supprimée dans le scénario de validation qui provient du scénario cible.
            /// </summary>
            FromTargetScenarioDeleted,

            /// <summary>
            /// Provient d'une nouvelle tâche dans le scénario de validation.
            /// </summary>
            FromSourceScenarioNew,

            /// <summary>
            /// La tâche est dans les deux scénarios.
            /// </summary>
            FromBoth,
        }

        #endregion

    }
}