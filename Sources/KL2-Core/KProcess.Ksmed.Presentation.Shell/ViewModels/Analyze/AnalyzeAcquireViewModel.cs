using DlhSoft.Windows.Controls;
using KProcess.KL2.SignalRClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran d'acquisition du scénario principal.
    /// </summary>
    public class AnalyzeAcquireViewModel : AcquireViewModelBase<AnalyzeAcquireViewModel, IAnalyzeAcquireViewModel>, IAnalyzeAcquireViewModel, ISignalRHandle<AnalyzeEventArgs>
    {

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public override string[] ScenarioNaturesFilter =>
            new [] { KnownScenarioNatures.Initial, KnownScenarioNatures.Target };

        /// <summary>
        /// Charge les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected override async Task LoadData(int projectId)
        {
            ShowSpinner();
            try
            {
                AcquireData data = await ServiceBus.Get<IAnalyzeService>().GetAcquireData(projectId);
                await LoadDataInternal(data);
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
        protected override Task<Scenario> SaveActionsServiceCall(Scenario[] scenarios, Scenario scenario)
        {
            return ServiceBus.Get<IAnalyzeService>().SaveAcquireData(scenarios, scenario);
        }

        /// <summary>
        /// Appelé lorsque la valeur de la propriété <see cref="CurrentGridItem"/> a changé.
        /// </summary>
        /// <param name="previousValue">The previous value.</param>
        /// <param name="newValue">The new value.</param>
        protected override void OnCurrentGridItemChanged(DataTreeGridItem previousValue, DataTreeGridItem newValue)
        {
            base.OnCurrentGridItemChanged(previousValue, newValue);
            OnPropertyChanged(nameof(CanChangeActionVideo));
        }

        public Task SignalRHandler(AnalyzeEventArgs agrs)
        {
            return OnRefreshing();
        }

    }
}