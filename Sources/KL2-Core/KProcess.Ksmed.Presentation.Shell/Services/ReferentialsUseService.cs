using KProcess.Ksmed.Business;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Représente un service de récupération d'informations sur l'utilisation des référentiels dans un projet.
    /// </summary>
    public class ReferentialsUseService : IReferentialsUseService
    {
        /// <summary>
        /// Met à jour les référentiels.
        /// </summary>
        /// <param name="referentials">Les référentiels</param>
        public void UpdateProjectReferentials(IEnumerable<ProjectReferential> referentials)
        {
            if (referentials != null)
            {
                Referentials = referentials.Select(r => new ProjectReferentialInfo(r)).ToDictionary(r => r.Referential);
                ReferentialsEnabled = Referentials.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.IsEnabled);
            }
            else
            {
                Referentials = null;
                ReferentialsEnabled = null;
            }
        }

        /// <summary>
        /// Obtient les propriétés d'utilisation des référentiels.
        /// </summary>
        public Dictionary<ProcessReferentialIdentifier, ProjectReferentialInfo> Referentials { get; private set; }

        /// <summary>
        /// Obtient l'utilisation des référentiels.
        /// </summary>
        public IDictionary<ProcessReferentialIdentifier, bool> ReferentialsEnabled { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si le référentiel spécifié est activé.
        /// </summary>
        /// <param name="refe">Le référentiel.</param>
        /// <returns><c>true</c> si le référentiel est activé.</returns>
        public bool IsReferentialEnabled(ProcessReferentialIdentifier refe) =>
            ReferentialsEnabled[refe];

        /// <summary>
        /// Obtient le libellé pour un référentiel.
        /// </summary>
        /// <param name="refe">Le référentiel.</param>
        /// <returns>Le libellé.</returns>
        public string GetLabel(ProcessReferentialIdentifier refe) =>
            IoC.Resolve<IServiceBus>().Get<IReferentialsService>().GetLabel(refe);

        /// <summary>
        /// Obtient le libellé pour un référentiel, au pluriel.
        /// </summary>
        /// <param name="refe">Le référentiel.</param>
        /// <returns>Le libellé.</returns>
        public string GetLabelPlural(ProcessReferentialIdentifier refe) =>
            IoC.Resolve<IServiceBus>().Get<IReferentialsService>().GetLabelPlural(refe);
    }
}
