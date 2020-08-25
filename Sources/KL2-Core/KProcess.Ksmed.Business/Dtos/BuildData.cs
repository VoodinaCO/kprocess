using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.Dtos
{
    /// <summary>
    /// Contient les données pour la phase de construction.
    /// </summary>
    public class BuildData
    {

        /// <summary>
        /// Obtient ou définit les scénarios.
        /// </summary>
        public Scenario[] Scenarios { get; set; }

        /// <summary>
        /// Obtient ou définit les catégories.
        /// </summary>
        public ActionCategory[] Categories { get; set; }

        /// <summary>
        /// Obtient ou définit les compétences.
        /// </summary>
        public Skill[] Skills { get; set; }

        /// <summary>
        /// Obtient ou définit les ressources.
        /// </summary>
        public Resource[] Resources { get; set; }

        /// <summary>
        /// Obtient ou définit les vidéos.
        /// </summary>
        public Video[] Videos { get; set; }

        /// <summary>
        /// Obtient ou définit les types d'actions possibles.
        /// </summary>
        public ActionType[] ActionTypes { get; set; }

        /// <summary>
        /// Obtient ou définit les libellés des champs personnalisés.
        /// </summary>
        public CustomFieldsLabels CustomFieldsLabels { get; set; }

    }
}
