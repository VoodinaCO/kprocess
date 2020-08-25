using KProcess.Ksmed.Models;
using KProcess.Ksmed.Models.Interfaces;
using System.Collections.Generic;

namespace KProcess.Ksmed.Business.Dtos.Prepare
{
    /// <summary>
    /// Contient les données pour Préparer - Projets.
    /// </summary>
    public class ProjectsData
    {
        /// <summary>
        /// Obtient ou définit les projets.
        /// </summary>
        public Project[] Projects { get; set; }

        /// <summary>
        /// Obtient ou définit les objectifs.
        /// </summary>
        public Objective[] Objectives { get; set; }

        /// <summary>
        /// Obtient ou définit l'arborescence de projets.
        /// </summary>
        public INode[] ProjectsTree { get; set; }

        /// <summary>
        /// Obtient ou définit la synthèse de chaque projet.
        /// </summary>
        public List<KeyValuePair<Project, ScenarioCriticalPath[]>> Summary { get; set; }
    }
}
