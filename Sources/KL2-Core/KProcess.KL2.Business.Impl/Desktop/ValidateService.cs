using KProcess.Business;
using KProcess.KL2.Languages;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Migration;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Security;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.Desktop
{
    /// <summary>
    /// Représente un service de gestion de la partie fonctionnelle "Valider".
    /// </summary>
    public class ValidateService : IBusinessService, IValidateService
    {
        private readonly ITraceManager _traceManager;
        private readonly ISecurityContext _securityContext;
        private readonly ILocalizationManager _localizationManager;
        private readonly ISharedScenarioActionsOperations _sharedScenarioActionsOperations;


        public ValidateService(
                ISecurityContext securityContext,
                ILocalizationManager localizationManager,
                ITraceManager traceManager,
                ISharedScenarioActionsOperations sharedScenarioActionsOperations)
        {
            _securityContext = securityContext;
            _localizationManager = localizationManager;
            _traceManager = traceManager;
            _sharedScenarioActionsOperations = sharedScenarioActionsOperations;
        }


        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<AcquireData> GetAcquireData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetAcquireData(context, projectId, GetDataScenarioNatures.Realized);
                }
            });

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    await _sharedScenarioActionsOperations.SaveAcquireData(context, allScenarios, updatedScenario, false);
                }
            });

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<BuildData> GetBuildData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetBuildData(context, projectId, GetDataScenarioNatures.Realized);
                }
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    await _sharedScenarioActionsOperations.SaveBuildScenario(context, allScenarios, updatedScenario, false);
                }
            });



        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<SimulateData> GetSimulateData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetSimulateData(context, projectId, GetDataScenarioNatures.All);
                }
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveSimulateData(Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    foreach (var action in updatedScenario.Actions)
                        context.KActions.ApplyChanges(action);

                    await context.SaveChangesAsync();
                }
            });

        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetRestitutionData(int projectId) =>
            await Task.Run(async () =>
            {
                using (var context = ContextFactory.GetNewContext(_securityContext.CurrentUser, _localizationManager))
                {
                    return await _sharedScenarioActionsOperations.GetRestitutionData(context, projectId);
                }
            });

    }
}