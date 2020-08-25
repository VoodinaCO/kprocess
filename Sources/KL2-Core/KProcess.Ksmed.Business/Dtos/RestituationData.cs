using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.Dtos
{
    /// <summary>
    /// Contient les données pour la phase de simulation.
    /// </summary>
    public class RestitutionData
    {
        /// <summary>
        /// Obtient ou définit le projet.
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Obtient ou définit l'utilisateur ayant créé le projet.
        /// </summary>
        public User ProjectCreatedByUser { get; set; }

        /// <summary>
        /// Obtient ou définit les scénarios.
        /// </summary>
        public Scenario[] Scenarios { get; set; }

        /// <summary>
        /// Obtient ou tous les définit les scénarios.
        /// </summary>
        public Scenario[] AllScenarios { get; set; }

        /// <summary>
        /// Obtient ou définit les catégories d'actions.
        /// </summary>
        public ActionCategory[] ActionCategories { get; set; }

        /// <summary>
        /// Obtient ou définit des détails sur les utilisateurs ayant fait les modifications.
        /// </summary>
        public ModificationsUsers ModificationsUsers { get; set; }

        /// <summary>
        /// Obtient ou définit les règles d'utilisation des référentiels.
        /// </summary>
        public ProjectReferential[] ReferentialsUse { get; set; }
    }
}
