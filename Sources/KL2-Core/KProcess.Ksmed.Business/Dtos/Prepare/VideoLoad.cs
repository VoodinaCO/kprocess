using KProcess.Ksmed.Models;

namespace KProcess.Ksmed.Business.Dtos.Prepare
{
    /// <summary>
    /// Contient les données liées au chargement de Prepare - Videos.
    /// </summary>
    public class VideoLoad
    {

        /// <summary>
        /// Obtient ou définit les resources du process.
        /// </summary>
        public Resource[] ProcessResources { get; set; }

        /// <summary>
        /// Obtient ou définit les vidéos du process.
        /// </summary>
        public Video[] ProcessVideos { get; set; }

        /// <summary>
        /// Get or set if videos have to be synced
        /// </summary>
        public bool Sync { get; set; }

    }
}
