using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Valider - Construire.
    /// </summary>
    class ValidateBuildViewModel : BuildViewModelBase<ValidateBuildViewModel, IValidateBuildViewModel>, IValidateBuildViewModel
    {

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
                BuildData data = await ServiceBus.Get<IValidateService>().GetBuildData(projectId);
                LoadDataInternal(data);
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
        protected override Task<bool> SaveActions(bool refreshSelectionWhenDone)
        {
            ShowSpinner();

            // Ne pas afficher le prompt puisqu'aucun scénario ne devrait être impacté
            return SaveActionsWithoutPrompt(refreshSelectionWhenDone);
        }

        /// <summary>
        /// Sauvegarde les actions.
        /// </summary>
        /// <param name="scenarios">Les scénarios à sauvegarder.</param>
        /// <param name="scenario">Le scénario à sauvegarder.</param>
        protected override Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario)
        {
            //// Dans le cas ou il n'y a pas de video associée, le start et finish de l'action suit celui du build start et build finish dans un contexte de validation.
            //// Cela permet lorsqu'on associe une nouvelle video qu'elle ait le bon emplacement
            //if (scenario != null)
            //{
            //    foreach (var action in scenario.Actions)
            //    {
            //        if (action.VideoId == null)
            //        {
            //            action.Start = action.BuildStart;
            //            action.Finish = action.BuildFinish;
            //        }
            //    }
            //}

            return ServiceBus.Get<IAnalyzeService>().SaveBuildScenario(scenarios, scenario);
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les solutions sont à gérer.
        /// </summary>
        protected override bool EnableSolutions => false;


        /// <summary>
        /// Obtient une valeur indiquant si seuls les champs de sélection I ou E sont accessibles.
        /// </summary>
        public override bool ShowPastInternalisationOrExternalisation => true;

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected override IESFilter[] GetIESFilters() =>
            IESFilter.CreateWithoutIESReplacingIEWithIES();
    }
}