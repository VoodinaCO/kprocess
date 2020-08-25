using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    partial class PrepareReferentialsViewModel
    {
        /// <summary>
        /// Définit un objet de type projectReferential encapsulé
        /// </summary>
        /// <remarks>
        /// Le choix d'encapsuler une entité métier vient du fait que le model ne prend pas en comte laa catégorie d'une ressource.
        /// Ce raccourci qui engendre du traitement arbitraire a été fait en connaissance de cause par le mandataire de cette fonctionalité
        /// </remarks>
        public class ExtendedProjectReferential
        {
            /// <summary>
            /// Obtient ou définit un référentiel de projet
            /// </summary>
            public ProjectReferential Referential { get; set; }

            /// <summary>
            /// Détermine s'il s'agit d'une référence de type projet
            /// </summary>
            public bool IsResource { get; set; }
        }
    }
}
