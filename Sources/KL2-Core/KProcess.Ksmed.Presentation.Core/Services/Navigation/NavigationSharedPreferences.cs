using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente les préférences de navigation.
    /// </summary>
    public class NavigationSharedPreferences
    {

        /// <summary>
        /// Obtient ou définit la vue actuelle de la grille.
        /// </summary>
        public GanttGridView? GanttGridView { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant actuel de l'action.
        /// </summary>
        public int? ActionId { get; set; }

        /// <summary>
        /// Obtient ou définit la position de la timeline.
        /// </summary>
        public long? TimelinePosition { get; set; }

        private Dictionary<int, bool> _actionsExpandedStates;
        private Dictionary<int, bool> _referentialsExpandedStates;

        /// <summary>
        /// Détermine si l'action identifiée par son id est dans l'état Expanded.
        /// </summary>
        /// <param name="actionId">L'identifiant de l'action.</param>
        /// <returns><c>true</c> si Expanded; <c>false</c> si Collapsed; <c>null</c> si non défini</returns>
        public bool? IsActionExpanded(int actionId)
        {
            return _actionsExpandedStates == null || !_actionsExpandedStates.ContainsKey(actionId) ?
                (bool?)null : _actionsExpandedStates[actionId];
        }

        /// <summary>
        /// Détermine si le référentiel identifiée par son id est dans l'état Expanded.
        /// </summary>
        /// <param name="referentialId">L'identifiant de du Referential.</param>
        /// <returns><c>true</c> si Expanded; <c>false</c> si Collapsed; <c>null</c> si non défini</returns>
        public bool? IsReferentialExpanded(int referentialId)
        {
            return _referentialsExpandedStates == null || !_referentialsExpandedStates.ContainsKey(referentialId) ?
                (bool?)null : _referentialsExpandedStates[referentialId];
        }

        /// <summary>
        /// Définit l'état Expanded d'une action.
        /// </summary>
        /// <param name="actionId">L'identifiant de l'action.</param>
        /// <param name="isExpanded">L'état.</param>
        public void SetActionExpanded(int actionId, bool isExpanded)
        {
            if (_actionsExpandedStates == null)
                _actionsExpandedStates = new Dictionary<int, bool>();

            _actionsExpandedStates[actionId] = isExpanded;
        }

        /// <summary>
        /// Définit l'état Expanded d'une ressource.
        /// </summary>
        /// <param name="referentialId">L'identifiant du référentiel.</param>
        /// <param name="isExpanded">L'état.</param>
        public void SetReferentialExpanded(int referentialId, bool isExpanded)
        {
            if (_referentialsExpandedStates == null)
                _referentialsExpandedStates = new Dictionary<int, bool>();

            _referentialsExpandedStates[referentialId] = isExpanded;
        }

        private Dictionary<int, (double X, double Y)> _ganttZooms = new Dictionary<int, (double X, double Y)>();
        /// <summary>
        /// Obtient le zoom à appliquer sur les gantts.
        /// </summary>
        public Dictionary<int, (double X, double Y)> GanttZooms
        {
            get
            {
                return _ganttZooms;
            }
        }

        private Dictionary<int, bool> _ganttDependencyLinesVisibilities = new Dictionary<int,bool>();
        /// <summary>
        /// Obtient les les visibilités des lingnes de dépendance, par projet.
        /// </summary>
        public Dictionary<int, bool> GanttDependencyLinesVisibilities
        {
            get
            {
                return _ganttDependencyLinesVisibilities;
            }
        }

    }

}
