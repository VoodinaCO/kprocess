using KProcess.Presentation.Windows;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{

    /// <summary>
    /// Définit le comportement du VM de la fenêtre principale.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IMainWindowViewModel : IViewModel
    {
        /// <summary>
        /// Appelé lorsque la vue associée a été chargée
        /// </summary>
        Task OnViewLoaded();

    }

}
