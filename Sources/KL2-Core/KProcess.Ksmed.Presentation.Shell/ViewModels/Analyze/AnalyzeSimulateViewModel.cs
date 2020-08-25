using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran de simulation dans la phase de construction.
    /// </summary>
    class AnalyzeSimulateViewModel : SimulateViewModelBase, IAnalyzeSimulateViewModel
    {

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter
        {
            get { return new string[] { KnownScenarioNatures.Initial, KnownScenarioNatures.Target }; }
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
                SimulateData data = await ServiceBus.Get<IAnalyzeService>().GetSimulateData(projectId);
                LoadDataInternal(data);
            }
            catch (Exception e)
            {
                base.OnError(e);
            }
        }

        /// <summary>
        /// Crée les filtres IES.
        /// </summary>
        /// <returns>Les filtres à utiliser.</returns>
        protected override IESFilter[] CreateIESFilters()
        {
            return IESFilter.CreateDefault();
        }

    }
}