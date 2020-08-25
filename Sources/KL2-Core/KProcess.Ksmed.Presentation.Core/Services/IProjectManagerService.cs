using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un service de gestion du projet en cours.
    /// </summary>
    public interface IProjectManagerService : IPresentationService
    {

        /// <summary>
        /// Obtient ou définit le projet courant.
        /// </summary>
        ProjectInfo CurrentProject { get; }

        /// <summary>
        /// Définit le projet courant.
        /// </summary>
        /// <param name="p">Le projet.</param>
        void SetCurrentProject(Project p);

        /// <summary>
        /// Synchronize le contexte relatif aux objectifs
        /// </summary>
        /// <param name="project"></param>
        void SynchronizeProjectObjectivesInfo(Project project);

        /// <summary>
        /// Obtient les scenarii.
        /// </summary>
        ReadOnlyObservableCollection<ScenarioDescription> Scenarios { get; }

        /// <summary>
        /// Obtient ou définit le scénario courant.
        /// </summary>
        ScenarioDescription CurrentScenario { get; set; }

        /// <summary>
        /// Ajoute les scénarios spécifiés dans le sélectionneur s'ils n'existent pas.
        /// </summary>
        /// <param name="scenarios">Les scénarios.</param>
        void SyncScenarios(IEnumerable<Scenario> scenarios);

        /// <summary>
        /// Ajoute les scénarios spécifiés dans le sélectionneur s'ils n'existent pas.
        /// </summary>
        /// <param name="scenarios">Les scénarios.</param>
        void SyncScenarios(IEnumerable<ScenarioDescription> scenarios);

        /// <summary>
        /// Supprime un scénario du sélectionneur.
        /// </summary>
        /// <param name="scenarioId">The scenario id.</param>
        void RemoveScenario(int scenarioId);

        /// <summary>
        /// Sélectionne un scénario dans le sélectionneur.
        /// </summary>
        /// <param name="scenario">Le scénario.</param>
        void SelectScenario(Scenario scenario);

        /// <summary>
        /// Cache le sélectionneur de scénarios.
        /// </summary>
        void HideScenariosPicker();

        /// <summary>
        /// Affiche le sélectionneur de scénarios.
        /// </summary>
        void ShowScenariosPicker();

        /// <summary>
        /// Obtient ou définit une valeur indiquant si le sélectionneur de scénarios est activé.
        /// </summary>
        bool IsScenarioPickerEnabled { get; set; }

        /// <summary>
        /// Obtient si le type de projet active le fait que le mode délier les marqueur soit activé d'office
        /// </summary>
        bool IsUnlinkMarkerEnabledAndLocked { get; }

        /// <summary>
        /// Filtre les scenarios par nature. Désactive ceux qui ne passent pas le filtre.
        /// Si null ou tableau vide, désactive le filtre.
        /// </summary>
        /// <param name="natureCodes">Les codes des natures.</param>
        void FilterScenarioNatures(string[] natureCodes);

        /// <summary>
        /// Obtient l'état de la synthèse.
        /// </summary>
        IDictionary<int, RestitutionState> RestitutionState { get; }

    }

    /// <summary>
    /// Levé lorsque le scénario courant a changé.
    /// </summary>
    public class ScenarioChangedEvent : EventBase
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ScenarioChangedEvent"/>.
        /// </summary>
        /// <param name="sender">L'appelant.</param>
        /// <param name="sd">Le scénario.</param>
        public ScenarioChangedEvent(object sender, ScenarioDescription sd)
            : base(sender)
        {
            this.Scenario = sd;
        }

        /// <summary>
        /// Obtient le scénario.
        /// </summary>
        public ScenarioDescription Scenario { get; private set; }
    }


    /// <summary>
    /// Levé lorsque le scénario courant a changé.
    /// </summary>
    public class ScenariosCollectionChangedEvent : EventBase
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ScenarioChangedEvent"/>.
        /// </summary>
        /// <param name="sender">L'appelant.</param>
        /// <param name="sd">Le scénario.</param>
        public ScenariosCollectionChangedEvent(object sender)
            : base(sender)
        {
        }
    }

    /// <summary>
    /// Décrit un projet.
    /// </summary>
    public class ProjectInfo
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ProjectInfo"/>.
        /// </summary>
        /// <param name="p">Le projet initial.</param>
        public ProjectInfo(Project p)
        {
            ProjectId = p.ProjectId;
            Label = p.Label;
            ProcessId = p.ProcessId;
            ProcessLabel = p.Process.Label;
            Roles = p.Process?.UserRoleProcesses.GroupBy(urp => urp.User).ToDictionary(key => key.Key.Username, value => value.Select(urp => urp.RoleCode).ToArray())
                ?? new Dictionary<string, string[]>();
            TimeScale = p.TimeScale;
        }

        /// <summary>
        /// Obtient ou définit l'identifiant du projet.
        /// </summary>
        public int ProjectId { get; private set; }

        /// <summary>
        /// Obtient ou définit le nom du projet.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du process.
        /// </summary>
        public int ProcessId { get; private set; }

        /// <summary>
        /// Obtient ou définit le nom du process.
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// Obtient ou définit l'échelle de temps.
        /// </summary>
        public long TimeScale { get; set; }

        /// <summary>
        /// Obtient les rôles attribués aux utilisateurs.
        /// </summary>
        public Dictionary<string, string[]> Roles { get; private set; }

    }

    /// <summary>
    /// Représente l'état de la synthèse.
    /// </summary>
    public class RestitutionState
    {
        /// <summary>
        /// Obtient ou définit le référentiel.
        /// </summary>
        public ProcessReferentialIdentifier? Referential { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'écran est celui des solutions.
        /// </summary>
        public bool Solutions { get; set; }

        /// <summary>
        /// Obtient ou définit le mode de restitution des valeurs.
        /// </summary>
        public int? RestitutionValueMode { get; set; }

        /// <summary>
        /// Obtient ou définit le mode de visualisation.
        /// </summary>
        public RestitutionStateViewMode? ViewMode { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant de la resource?
        /// </summary>
        public int? ResourceId { get; set; }
    }

    /// <summary>
    /// Représente le mode de visualisation.
    /// </summary>
    public enum RestitutionStateViewMode
    {
        /// <summary>
        /// Vue globale.
        /// </summary>
        Global,

        /// <summary>
        /// Vue par opérateur.
        /// </summary>
        PerOperator,

        /// <summary>
        /// Vue par équipement.
        /// </summary>
        PerEquipment,
    }
}
