using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.Dtos.Prepare
{
    /// <summary>
    /// Représente les données de Préparer - Scénarios.
    /// </summary>
    public class ScenariosData
    {
        /// <summary>
        /// Obtient ou définit les scénarios.
        /// </summary>
        public Scenario[] Scenarios { get; set; }

        /// <summary>
        /// Obtient ou définit les états de scénarios.
        /// </summary>
        public ScenarioState[] ScenarioStates { get; set; }

        /// <summary>
        /// Obtient ou définit les natures de scénarios.
        /// </summary>
        public ScenarioNature[] ScenarioNatures { get; set; }
        
        /// <summary>
        /// Obtient ou définit la synthèse.
        /// </summary>
        public ScenarioCriticalPath[] Summary { get; set; }

    }
}
