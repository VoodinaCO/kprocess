using KProcess.Business;
using KProcess.Ksmed.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comportement d'un service de gestion des référentiels d'actions.
    /// </summary>
    public interface IReferentialsService : IBusinessService
    {
        string GetLabel(ProcessReferentialIdentifier refe);

        string GetLabelPlural(ProcessReferentialIdentifier refe);

        /// <summary>
        /// Obtient la configuration des réferentiels.
        /// </summary>
        /// <returns>Lse référentiels.</returns>
        Task<Referential[]> GetApplicationReferentials();

        /// <summary>
        /// Met à jour le libellé du référentiel spécifié.
        /// </summary>
        /// <param name="refId">L'identifiant du référentiel.</param>
        /// <param name="label">Le libellé.</param>
        Task UpdateReferentialLabel(ProcessReferentialIdentifier refId, string label);

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les catégories d'actions de tous les projets
        /// Tous les types d'actions 
        /// Toutes les valorisations d'actions.
        /// </summary>
        Task<(ActionCategory[] Categories, ActionValue[] ActionValues, ActionType[] ActionTypes, Procedure[] Processes)> LoadCategories();

        /// <summary>
        /// Obtient les compétences d'actions de tous les projets
        /// </summary>
        Task<Skill[]> LoadSkills(bool allInfos = false);

        /// <summary>
        /// Sauvegarde les catégories.
        /// </summary>
        /// <param name="categories">Les catégories.</param>
        Task<IEnumerable<ActionCategory>> SaveCategories(IEnumerable<ActionCategory> categories);

        /// <summary>
        /// Sauvegarde les compétences.
        /// </summary>
        /// <param name="skills">Les compétences.</param>
        Task<IEnumerable<Skill>> SaveSkills(IEnumerable<Skill> skills);

        /// <summary>
        /// Obtient les référentiels standards et projets du type spécifié.
        /// </summary>
        /// <param name="refId">L'identifiant du référentiel.</param>
        Task<(IMultipleActionReferential[] Referentials, Procedure[] Processes)> GetReferentials(ProcessReferentialIdentifier refId, int? processId = null);

        Task<(IActionReferential[] Referentials, Procedure[] Processes)> GetAllReferentials(ProcessReferentialIdentifier refId, int? processId = null);

        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        /// <typeparam name="TReferential">Le type du référentiel.</typeparam>
        /// <param name="referentials">Les référentiels.</param>
        Task<IEnumerable<TReferential>> SaveReferentials<TReferential>(IEnumerable<TReferential> referentials) where TReferential : class, IActionReferential;

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les équipements de tous les projets.
        /// </summary>
        Task<(Equipment[] Equipments, Procedure[] Processes)> LoadEquipments();

        /// <summary>
        /// Obtient dans l'ordre :
        /// Les opérateurs de tous les projets.
        /// </summary>
        Task<(Operator[] Operators, Procedure[] Processes)> LoadOperators();

        /// <summary>
        /// Sauvegarde les ressources.
        /// </summary>
        /// <param name="resources">Les ressources.</param>
        Task<IEnumerable<Resource>> SaveResources(IEnumerable<Resource> resources);



        /// <summary>
        /// Fusionne des référentiels
        /// </summary>
        /// <param name="master">Le référentiel maître.</param>
        /// <param name="slaves">Les référentiels esclaves.</param>
        Task MergeReferentials(IActionReferential master, IActionReferential[] slaves);

        /// <summary>
        /// Obtient un CloudFile
        /// <param name="hash">Hash généré comme identifiant</param>
        /// </summary>
        Task<CloudFile> GetCloudFile(string hash);

        /// <summary>
        /// Sauvegarde un CloudFile
        /// </summary>
        Task<CloudFile> SaveCloudFile(CloudFile file);

        /// <summary>
        /// Obtient les référentiels du projet spécifié.
        /// </summary>
        Task<List<ProjectReferential>> GetProjectReferentials(int projectId);

        Task<bool> ReferentialUsed(ProcessReferentialIdentifier processReferentialId, int referentialId);

        /// <summary>
        /// /// Obtient si le Custom Field est utilisé dans le scénario spécifié.
        /// /// </summary>
        Task<bool> CustomFieldIsUsed(int scenarioId, ProcessReferentialIdentifier customFieldId);

        /// <summary>
        /// Obtient les référentiels de documentation du process spécifié.
        /// </summary>
        Task<List<DocumentationReferential>> GetDocumentationReferentials(int processId);

        /// <summary>
        /// Sauvegarde les référentiels de documentation.
        /// </summary>
        Task<List<DocumentationReferential>> SaveDocumentationReferentials(DocumentationReferential[] documentationReferentials);

    }
}