using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'implémentation de base d'un controleur d'application.
    /// </summary>
    public abstract class ApplicationControllerBase : ControllerBase
    {
        /// <summary>
        /// Obtient la liste des contrôleurs de module
        /// </summary>
        [ImportMany(AllowRecomposition = true)]
        public IEnumerable<IModuleController> ModuleControllers { get; set; }

        /// <summary>
        /// Méthode invoquée lors de l'initialisation du module
        /// </summary>
        protected override async Task OnLoaded()
        {
            foreach (IModuleController mc in ModuleControllers)
                await mc.Start();
        }
    }
}
