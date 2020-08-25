using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Représente le modèle de vue de l'écran Analyser - Restituer.
    /// </summary>
    class AnalyzeRestitutionViewModel : RestitutionViewModelBase, IAnalyzeRestitutionViewModel
    {

        /// <summary>
        /// Appelé afin de charger les données.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        protected override Task<RestitutionData> OnLoadData(int projectId) =>
            ServiceBus.Get<IAnalyzeService>().GetRestitutionData(projectId);
    }
}