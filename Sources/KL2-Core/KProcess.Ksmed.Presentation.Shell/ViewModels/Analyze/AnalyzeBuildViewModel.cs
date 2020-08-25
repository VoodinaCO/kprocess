using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de construction du scénario initial.
    /// </summary>
    class AnalyzeBuildViewModel : BuildViewModelBase<AnalyzeBuildViewModel, IAnalyzeBuildViewModel>, IAnalyzeBuildViewModel
    {

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter =>
            new string[] { KnownScenarioNatures.Initial, KnownScenarioNatures.Target };

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected override async Task LoadData(int projectId)
        {
            ShowSpinner();
            try
            {
                BuildData data = await ServiceBus.Get<IAnalyzeService>().GetBuildData(projectId);
                LoadDataInternal(data);
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }


        /// <summary>
        /// Sauvegarde les actions.
        /// </summary>
        /// <param name="scenarios">Les scénarios à sauvegarder.</param>
        /// <param name="scenario">Le scénario à sauvegarder.</param>
        protected override async Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario)
        {
            return await ServiceBus.Get<IAnalyzeService>().SaveBuildScenario(scenarios, scenario);            
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si les solutions sont à gérer.
        /// </summary>
        protected override bool EnableSolutions =>
            CurrentScenarioInternal != null && CurrentScenarioInternal.NatureCode != KnownScenarioNatures.Initial;

        /// <summary>
        /// Obtient une valeur indiquant si seuls les champs de sélection I ou E sont accessibles.
        /// </summary>
        public override bool ShowPastInternalisationOrExternalisation =>
            CurrentScenarioInternal != null && CurrentScenarioInternal.NatureCode == KnownScenarioNatures.Initial;

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected override IESFilter[] GetIESFilters()
        {
            if (CurrentScenarioInternal != null && CurrentScenarioInternal.NatureCode == KnownScenarioNatures.Initial)
                return IESFilter.CreateWithoutIESReplacingIEWithIES();
            return IESFilter.CreateDefault();
        }
    }
}