using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit le comportement d'un ViewModel pouvant être affiché dans la frame principale de l'application.
    /// </summary>
    public interface IFrameContentViewModel : IKsmedViewModelBase
    {

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <param name="token">Représente le jeton de navigation permettant d'effectuer une navigation asynchrone</param>
        /// <returns><c>true</c> si la navigation est acceptée.</returns>
        Task<bool> OnNavigatingAway(IFrameNavigationToken token);

        /// <summary>
        /// Obtient une valeur indiquant si le sélectionneur de scénario est à afficher.
        /// </summary>
        bool ShowScenarioPicker { get; }

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        string[] ScenarioNaturesFilter { get; }

    }
}
