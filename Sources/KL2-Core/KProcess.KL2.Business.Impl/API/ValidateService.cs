using KProcess.Business;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using System.Dynamic;
using System.Threading.Tasks;

namespace KProcess.KL2.Business.Impl.API
{
    /// <summary>
    /// Représente un service de gestion de la partie fonctionnelle "Valider".
    /// </summary>
    public class ValidateService : IBusinessService, IValidateService
    {
        readonly IAPIHttpClient _apiHttpClient;

        public ValidateService(IAPIHttpClient apiHttpClient)
        {
            _apiHttpClient = apiHttpClient;
        }

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<AcquireData> GetAcquireData(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<AcquireData>(KL2_Server.API, nameof(ValidateService), nameof(GetAcquireData), param);
            });

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.allScenarios = allScenarios;
                param.updatedScenario = updatedScenario;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(ValidateService), nameof(SaveAcquireData), param);
            });

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<BuildData> GetBuildData(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<BuildData>(KL2_Server.API, nameof(ValidateService), nameof(GetBuildData), param);
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.allScenarios = allScenarios;
                param.updatedScenario = updatedScenario;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(ValidateService), nameof(SaveBuildScenario), param);
            });



        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<SimulateData> GetSimulateData(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<SimulateData>(KL2_Server.API, nameof(ValidateService), nameof(GetSimulateData), param);
            });

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        public virtual async Task SaveSimulateData(Scenario updatedScenario) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.updatedScenario = updatedScenario;
                await _apiHttpClient.ServiceAsync(KL2_Server.API, nameof(ValidateService), nameof(SaveSimulateData), param);
            });

        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        public virtual async Task<RestitutionData> GetRestitutionData(int projectId) =>
            await Task.Run(async () =>
            {
                dynamic param = new ExpandoObject();
                param.projectId = projectId;
                return await _apiHttpClient.ServiceAsync<RestitutionData>(KL2_Server.API, nameof(ValidateService), nameof(GetRestitutionData), param);
            });

    }
}