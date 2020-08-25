using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Valider - Acquérir.
    /// </summary>
    class ValidateAcquireViewModel : AcquireViewModelBase<ValidateAcquireViewModel, IValidateAcquireViewModel>, IValidateAcquireViewModel
    {
        /// <summary>
        /// Initialise le VM en vue du chargement
        /// </summary>
        protected override void Initialize() =>
            base.Initialize();

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter =>
            new string[] { KnownScenarioNatures.Realized };

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected override async Task LoadData(int projectId)
        {
            ShowSpinner();
            try
            {
                AcquireData data = await ServiceBus.Get<IValidateService>().GetAcquireData(projectId);
                await LoadDataInternal(data);
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Enregistre les actions modifiées.
        /// </summary>
        /// <param name="refreshSelectionWhenDone"><c>true</c> pour rafraichir la sélection une fois la sauvegarde effectuée.</param>
        protected override async Task<bool> SaveActions(bool refreshSelectionWhenDone)
        {
            ShowSpinner();

            // Ne pas afficher le prompt puisqu'aucun scénario ne devrait être impacté
            try
            {
                await SaveActionsWithoutPrompt(refreshSelectionWhenDone);
                return true;
            }
            catch(Exception e)
            {
                base.OnError(e);
            }
            return false;
        }

        /// <summary>
        /// Sauvegarde les actions.
        /// </summary>
        /// <param name="scenarios">Les scénarios à sauvegarder.</param>
        /// <param name="scenario">Le scénario à sauvegarder.</param>
        protected override async Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario)
        {
            return await ServiceBus.Get<IAnalyzeService>().SaveAcquireData(scenarios, scenario);
        }

        /// <summary>
        /// Appelé lorsque la valeur de le propriété <see cref="CurrentGridItem"/> a changé.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCurrentGridItemChanged(DlhSoft.Windows.Controls.DataTreeGridItem previousValue, DlhSoft.Windows.Controls.DataTreeGridItem newValue)
        {
            base.OnCurrentGridItemChanged(previousValue, newValue);
            OnPropertyChanged("CanChangeActionVideo");
        }

        /// <summary>
        /// Obtient le temps de début pour une action donc la vidéo en cours a changé.
        /// </summary>
        /// <param name="action">L'action.</param>
        /// <returns>Le début.</returns>
        protected override long? GetActionNewVideoStart(ActionGridItem action)
        {
            if (action.Action.Video == null)
                return base.GetActionNewVideoStart(action);
            // Sinon, elle commence après le predecesseur ayant une vidéo associée
            var predecessorsWithVideo = action.Action.Predecessors.Where(p => p.Video == action.Action.Video);
            if (predecessorsWithVideo.Any())
            {
                //var directPredecessors = action.Action.Predecessors;
                return predecessorsWithVideo.Max(p => p.Finish) + Math.Max(0, action.Action.BuildStart - predecessorsWithVideo.Max(p => p.BuildFinish));
            }
            else
            {
                return action.Action.BuildStart;
            }
        }

    }
}