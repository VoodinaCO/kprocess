using KProcess.Business;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Definit le comprotement d'un service de gestion de la partie fonctionnelle "Préparer".
    /// </summary>
    public interface IPrepareService : IBusinessService
    {
        /// <summary>
        /// True si une publication existe pour le process.
        /// </summary>
        Task<bool> PublicationExistsForProcess(int processId);

        /// <summary>
        /// True si une publication existe pour le process en sync.
        /// </summary>
        bool PublicationExistsForProcessSync(int processId);

        Task<Publication> GetPublication(Guid publicationId);

        Task<Publication> GetTrainingPublication(Guid evaluationPublicationId);

        Task<Publication> GetEvaluationPublication(Guid trainingPublicationId);

        Task<IEnumerable<Publication>> GetTrainingPublications(Guid[] evaluationPublicationIds);

        Task<IEnumerable<Publication>> GetEvaluationPublications(Guid[] trainingPublicationIds);

        Task<Publication> GetLightPublication(Guid publicationId);


        /// <summary>
        /// Obtient la dernière publication d'un process.
        /// </summary>
        Task<Publication> GetLastPublication(int processId);

        /// <summary>
        /// Obtient la dernière publication d'un process. with filter by publish mode
        /// </summary>
        Task<Publication> GetLastPublicationFiltered(int processId, int publishModeFilter);

        /// <summary>
        /// Obtient la publication d'un process avec un audit ouvert pour l'utilisateur donné.
        /// </summary>
        Task<Publication> GetPublicationToAudit(int auditorId);

        /// <summary>
        /// Obtient les dernières publications d'un process.
        /// </summary>
        Task<IEnumerable<Publication>> GetLastPublications(int publishModeFilter);

        Task<IEnumerable<Publication>> GetLastPublicationsForFilter(int publishModeFilter);

        /// <summary>
        /// Obtient les dernières publications d'un process ainsi que toutes les publications de la version courante
        /// </summary>
        Task<IEnumerable<Publication>> GetLastSameMajorPublications(int publishModeFilter);

        Task<IEnumerable<Publication>> GetLastPublicationsPerMajor(int publishModeFilter);

        /// <summary>
        /// Obtient les dernières publications d'un process. with IsSkill true
        /// </summary>
        Task<IEnumerable<Publication>> GetLastPublicationSkills();

        /// <summary>
        /// Get all audits or audits of inspections
        /// </summary>
        Task<IEnumerable<Audit>> GetAudits(int? auditId = null);

        /// <summary>
        /// Get active audit 
        /// </summary>
        Task<Audit> GetActiveAudit(int? auditorId = null, Guid? publicationId = null);

        /// <summary>
        /// Get all surveys or spesific survey
        /// </summary>
        Task<IEnumerable<Survey>> GetSurveys(int? surveyId = null);

        /// <summary>
        /// Save surveys
        /// </summary>
        /// <param name="surveys">List of surveys to be saved</param>
        /// <returns>List of surveys that want to be saved</returns>
        Task<Survey[]> SaveSurveys(Survey[] surveys);

        /// <summary>
        /// Check if auditor already have active audit 
        /// </summary>
        Task<bool> CheckAuditorHaveActiveAudit(int? auditorId = null, Guid? publicationId = null);

        /// <summary>
        /// Save audit
        /// </summary>
        /// <returns>Audit</returns>
        Task<Audit> SaveAudit(Audit audit);

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés.
        /// </summary>
        Task<(bool Result, Procedure[] NonPublishedProcesses)> AllLinkedProcessArePublished(int scenarioId);

        /// <summary>
        /// Obtient un booléen qui indique si tous les process liés d'un scénario (si ils existent) ont été publiés. (SYNC)
        /// </summary>
        (bool Result, Procedure[] NonPublishedProcesses) AllLinkedProcessArePublishedSync(int scenarioId);

        Task<Publication> SetReadPublication(Guid publicationId, int UserId, DateTime? ReadingDate);


        /// <summary>
        /// Obtient un projet.
        /// </summary>
        Task <Project> GetProject(int projectId);

        /// <summary>
        /// Obtient un projet en sync.
        /// </summary>
        Project GetProjectSync(int projectId);

        /// <summary>
        /// Obtient l'arborescence des process ayant une publication
        /// </summary>
        Task<INode[]> GetPublicationsTree(PublishModeEnum filter);

        /// <summary>
        /// Get process information by id
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="includeAllInformations">Include all the informations (longer to execute)</param>
        /// <returns></returns>
        Task<Procedure> GetProcess(int processId, bool includeAllInformations = true);

        Task<Procedure> GetProcessForPublishFormat(int processId);

        Task<INode[]> GetProcessTreeWithScenario();

        Task<string> GetProcessName(int processId);

        Task<string> GetProjectName(int projectId);

        Task<string> GetScenarioName(int scenarioId);

        /// <summary>
        /// Retrieve action information for a specific scenario
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        Task<IEnumerable<KAction>> GetActionsByScenario(int scenarioId);

        /// <summary>
        /// Obtient les projets et les objectifs.
        /// </summary>
        Task<ProjectsData> GetProjects();

        /// <summary>
        /// Sync all videos.
        /// </summary>
        Task SyncVideos(params int[] processIds);

        /// <summary>
        /// List all videos to sync.
        /// </summary>
        Task<Dictionary<VideoSyncTask, List<Video>>> ListAllVideosToSync(params int[] processIds);

        /// <summary>
        /// Get if a video can be unsynced.
        /// </summary>
        Task<bool> CanBeUnSync(string videoHash);

        /// <summary>
        /// Obtient les dossiers
        /// </summary>
        Task<ProjectDir[]> GetProjectDirs();

        /// <summary>
        /// Obtient les process.
        /// </summary>
        Task<Procedure[]> GetProcesses();

        Task<Procedure[]> GetPublishedProcessesForInspection();

        /// <summary>
        /// Obtient si un process est lié à une tâche.
        /// </summary>
        Task<bool> ProcessIsLinkedToATask(int processId);

        /// <summary>
        /// Obtient les noms avec extension d'une liste de fichiers.
        /// </summary>
        Task<string[]> GetFullName(IEnumerable<string> fileHashes);

        /// <summary>
        /// Sauvegarde le projet.
        /// </summary>
        /// <param name="project">Le projet.</param>
        Task<Project> SaveProject(Project project);

        /// <summary>
        /// Sauvegarde le dossier.
        /// </summary>
        /// <param name="folder">Le dossier.</param>
        Task<ProjectDir> SaveFolder(ProjectDir folder);

        Task<AppSetting[]> SaveAppSettings(AppSetting[] settings);

        /// <summary>
        /// Sauvegarde le process.
        /// </summary>
        /// <param name="process">Le process.</param>
        Task<Procedure> SaveProcess(Procedure process, bool notifyChanges = true);

        /// <summary>
        /// Obtient les raisons possibles d'une non qualification.
        /// </summary>
        Task<List<QualificationReason>> GetQualificationReasons();

        /// <summary>
        /// Sauvegarde les raisons possibles d'une non qualification.
        /// </summary>
        /// <param name="reasons">Les raisons.</param>
        Task<List<QualificationReason>> SaveQualificationReasons(IEnumerable<QualificationReason> reasons);

        /// <summary>
        /// Obtient les rôles des utilisateurs dans le process spécifié, tous les utilisateurs et tous les rôles disponibles.
        /// </summary>
        /// <param name="projectId">L'identifiant du process.</param>
        Task<(User[] Users, Role[] Roles)> GetMembers(int processId);

        /// <summary>
        /// Sauvegarde le membre d'un process.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        /// <param name="member">Le membre.</param>
        Task<User> SaveMember(int processId, User member);

        /// <summary>
        /// Obtient les référentiels des projets.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<Project> GetReferentials(int projectId);

        /// <summary>
        /// Sauvegarde les référentiels.
        /// </summary>
        /// <param name="project">Les projet et les référentiels à sauvegarder.</param>
        Task<Project> SaveReferentials(Project project);

        /// <summary>
        /// Obtient toutes les ressources liées au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        Task<Resource[]> GetAllResources(int processId);

        /// <summary>
        /// Obtient une vidéo ayant la même vidéo d'origine si elle existe.
        /// </summary>
        /// <param name="originalHash">Le hash de la vidéo d'origine.</param>
        Task<Video> GetSameOriginalVideo(string originalHash);

        /// <summary>
        /// Obtient la vidéo.
        /// </summary>
        /// <param name="videoId">L'identifiant de la vidéo.</param>
        Task<Video> GetVideo(int videoId);

        /// <summary>
        /// Get all app settings.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        Task<AppSetting[]> GetAllAppSettings();

        /// <summary>
        /// Obtient les vidéos et tous les éléments liés au chargement de Prepare - Videos, liés au process spécifié.
        /// </summary>
        /// <param name="processId">L'identifiant du process.</param>
        Task<VideoLoad> GetVideos(int processId);

        /// <summary>
        /// Sauvegarde la vidéo d'un process.
        /// </summary>
        /// <param name="video">La vidéo.</param>
        Task<Video> SaveVideo(Video video);

        /// <summary>
        /// Obtient les scénarios liés au projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<ScenariosData> GetScenarios(int projectId);

        /// <summary>
        /// Obtient le scénario.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scenario.</param>
        Task<Scenario> GetScenario(int scenarioId);

        /// <summary>
        /// Obtient le scénario pour publication.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scenario.</param>
        Task<Scenario> GetScenarioForPublish(int scenarioId);

        /// <summary>
        /// Crée un nouveau scénario initial.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        Task<Scenario> CreateInitialScenario(int projectId);

        /// <summary>
        /// Crée un nouveau scénario.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="sourceScenario">Le scénario source.</param>
        /// <param name="keepVideoForUnchanged">Détermine si les séquences initiales doivent être gardées.</param>
        Task<Scenario> CreateScenario(int projectId, Scenario sourceScenario, bool keepVideoForUnchanged);

        /// <summary>
        /// Deletes the scenario.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <param name="scenario">le scénario a supprimer.</param>
        Task<(bool Result, ScenarioCriticalPath[] Summary)> DeleteScenario(int projectId, Scenario scenario);

        /// <summary>
        /// Sauvegarde le scénario spécifiés.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="scenario">Le scénario.</param>
        Task<ScenariosData> SaveScenario(int projectId, Scenario scenario);

        /// <summary>
        /// Crée un nouveau projet à partir d'un scénario de validation.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        /// <param name="validatedScenario">Le scénario de validation.</param>
        Task<int> CreateNewProjectFromValidatedScenario(int projectId, Scenario validatedScenario);

        /// <summary>
        /// Met à jour l'identifiant de publication web.
        /// </summary>
        /// <param name="scenarioId">L'identifiant du scénario.</param>
        /// <param name="publicationGuid">L'identifiant de publication.</param>
        /// <returns>La tâche asynchrone.</returns>
        Task UpdateScenarioPublicationGuid(int scenarioId, Guid? publicationGuid);

        #region Publication
        /// <summary>
        /// Sauvegarde la publication
        /// </summary>
        /// <param name="publication">Publication à sauvegarder</param>
        /// <returns>La publication sauvegardée</returns>
        Task<Publication> SavePublication(Publication publication);
        #endregion

        #region Documentation

        Task<bool> HasDocumentationDraft(int scenarioId);
        bool HasDocumentationDraftSync(int scenarioId);
        Task<DocumentationDraft> GetLastDocumentationDraft(int processId, int scenarioId);
        Task<DocumentationDraft> GetDocumentationDraft(int documentationDraftId);
        Task<IEnumerable<ProjectReferential>> GetUsedReferentials(int projectId);
        Task<long> GetProjectTimeScale(int scenarioId);
        Task<DocumentationDraft> SaveDocumentationDraft(DocumentationDraft documentationDraft);
        Task SaveDocumentationDraft(DocumentationDraft documentation, List<DocumentationActionDraft> actions, List<DocumentationActionDraftWBS> actionsWbs, int projectId, int scenarioId);

        Task SaveDocumentationVideos(int documentationDraftId,
            bool activeVideoExport,
            bool slowMotion,
            double slowMotionDuration,
            bool waterMarking,
            string waterMarkingText,
            EVerticalAlign waterMarkingVAlign,
            EHorizontalAlign waterMarkingHAlign);

        #endregion

        /// <summary>
        /// Recupere un fiicher publie via le guid
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<PublishedFile> GetPublishedFile(string hash);

        /// <summary>
        /// Ajoute un fichier
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        Task CreatePublishedFile(string hash, string extension);

        /// <summary>
        /// Retrieve video by hash
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<CutVideo> GetCutVideo(string hash);


        #region Formation
        /// <summary>
        /// Méthode permettant de récupérer les formations d'un utilisateur pour une publication
        /// </summary>
        /// <param name="publicationId">Identifiant de la publication</param>
        /// <param name="userId">Identifiant de l'utilisateur</param>
        /// <returns>La formation de l'utilisateur ou null s'il n'en existe pas avec les paramètres spécifié</returns>
        Task<Training> GetTraining(Guid publicationId, int userId);

        /// <summary>
        /// Get all trainings
        /// </summary>
        Task<IEnumerable<Training>> GetTrainings();

        /// <summary>
        /// Get all trainings
        /// </summary>
        Task<IEnumerable<PublishedAction>> GetPublishedActions();

        /// <summary>
        /// Get a published action
        /// </summary>
        Task<PublishedAction> GetPublishedAction(int id);
        /// <summary>
        /// Get an action
        /// </summary>
        Task<KAction> GetAction(int id);

        /// <summary>
        /// Sauvegarde les formations
        /// </summary>
        /// <param name="trainings">Liste des formations à sauvegarder</param>
        /// <returns>La liste des formations sauvegardé</returns>
        Task<Training[]> SaveTrainings(Training[] trainings);

        Task<Timeslot[]> SaveTimeslots(Timeslot[] timeslots);

        /// <summary>
        /// Retrouve une qualification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Qualification> GetQualification(int id);


        #endregion

        #region Inspection

        /// <summary>
        /// Méthode permettant de récupérer la dernière inspection d'une publication
        /// </summary>
        /// <param name="publicationId">Identifiant de la publication</param>
        /// <returns>La dernière inspection ou null s'il n'en existe pas avec les paramètres spécifié</returns>
        Task<Inspection> GetLastInspection(Guid publicationId);

        Task<Inspection> GetInspection(int InspectionId);

        /// <summary>
        /// Get all inspections
        /// </summary>
        Task<IEnumerable<Inspection>> GetInspections(Guid? publicationId = null);

        /// <summary>
        /// Get all timeslots
        /// </summary>
        Task<IEnumerable<Timeslot>> GetTimeslots(int? timeslotId = null);

        /// <summary>
        /// Get all inspections schedule exclude inspections completed
        /// </summary>
        Task<IEnumerable<InspectionSchedule>> GetInspectionSchedules(int? InspectionScheduleId = null);

        /// <summary>
        /// Get all inspections schedule
        /// </summary>
        Task<IEnumerable<InspectionSchedule>> GetInspectionSchedulesNonFilter(int? InspectionScheduleId = null);

        /// <summary>
        /// Get all inspections schedule for timeslot
        /// </summary>
        Task<IEnumerable<InspectionSchedule>> GetInspectionsScheduleForTimeslot(int timeslotId);

        /// <summary>
        /// Sauvegarde les inspections
        /// </summary>
        /// <param name="inspections">Liste des inspections à sauvegarder</param>
        /// <returns>La liste des inspections sauvegardées</returns>
        Task<Inspection[]> SaveInspections(Inspection[] inspections);

        /// <summary>
        /// Save inspection schedule
        /// </summary>
        /// <param name="schedule">Schedule of Inspection</param>
        /// <returns>Saved inspection schedule</returns>
        Task<InspectionSchedule> SaveInspectionSchedule(InspectionSchedule schedule);

        Task<Anomaly[]> GetAnomalies(int inspectionId);

        Task<Anomaly> GetAnomaly(int AnomalyId);

        #endregion
    }
}
