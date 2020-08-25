using KProcess.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comportement d'un service de gestion de la partie fonctionnelle "Valider".
    /// </summary>
    public interface IValidateService : IBusinessService
    {

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<AcquireData> GetAcquireData(int projectId);

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        Task SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario);

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<BuildData> GetBuildData(int projectId);

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        Task SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario);



        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<SimulateData> GetSimulateData(int projectId);

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        Task SaveSimulateData(Scenario updatedScenario);



        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<RestitutionData> GetRestitutionData(int projectId);

    }
}