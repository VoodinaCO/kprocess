namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'une ressource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Obtient ou définit le jugement de valeur de la ressource.
        /// </summary>
        double PaceRating { get; set; }
    }
}
