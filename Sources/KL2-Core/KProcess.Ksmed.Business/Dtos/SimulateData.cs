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
    public class SimulateData
    {

        /// <summary>
        /// Obtient ou définit les scénarios.
        /// </summary>
        public Scenario[] Scenarios { get; set; }

        /// <summary>
        /// Obtient ou définit les types d'actions.
        /// </summary>
        public ActionType[] ActionTypes { get; set; }

        /// <summary>
        /// Obtient ou définit les libellés des champs personnalisés.
        /// </summary>
        public CustomFieldsLabels CustomFieldsLabels { get; set; }

    }
}
