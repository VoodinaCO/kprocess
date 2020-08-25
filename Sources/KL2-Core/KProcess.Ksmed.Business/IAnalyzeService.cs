using KProcess.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Models;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion de la partie fonctionnelle "Analyser".
    /// </summary>
    public interface IAnalyzeService : IBusinessService
    {

        /// <summary>
        /// Prédit les scénarios qui seront impactés par les modifications en attente de sauvegarde.
        /// </summary>
        /// <param name="sourceModifiedScenario">Le scenério source modifié.</param>
        /// <param name="allScenarios">Tous les scénarios.</param>
        /// <param name="actionsToDelete">Les actions à supprimer.</param>
        /// <param name="actionsWithUpdatedWBS">Les actions qui auront une mise à jour de WBS.</param>
        Task<Scenario[]> PredictImpactedScenarios(Scenario sourceModifiedScenario, Scenario[] allScenarios, KAction[] actionsToDelete, KAction[] actionsWithUpdatedWBS);

        /// <summary>
        /// Obtient les données pour l'écran Acquérir.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<AcquireData> GetAcquireData(int projectId, bool getSyncedVideos = true);

        /// <summary>
        /// Sauvegarde les actions spécifiées.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        Task<Scenario> SaveAcquireData(Scenario[] allScenarios, Scenario updatedScenario);

        /// <summary>
        /// Obtient les données pour l'écran Construire.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<BuildData> GetBuildData(int projectId, bool getSyncedVideos = true);

        /// <summary>
        /// Sauvegarde le scénario spécifié.
        /// </summary>
        /// <param name="allScenarios">Tous les scénarios liés.</param>
        /// <param name="updatedScenario">Le scénario qui a été mis à jour.</param>
        Task<Scenario> SaveBuildScenario(Scenario[] allScenarios, Scenario updatedScenario);

        /// <summary>
        /// Obtient les données pour l'écran Simuler.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<SimulateData> GetSimulateData(int projectId);

        /// <summary>
        /// Obtient les données pour l'écran Restituer.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<RestitutionData> GetRestitutionData(int projectId);

        /// <summary>
        /// Obtient toutes les données du projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<RestitutionData> GetFullProjectDetails(int projectId);

        /// <summary>
        /// Met à jour l'affichage dans la synthèse pour le scénario spécifié.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="isShownInSummary"><c>true</c> pour l'afficher dans le scénario.</param>
        Task<Scenario> UpdateScenarioIsShownInSummary(int scenarioId, bool isShownInSummary);

    }
}