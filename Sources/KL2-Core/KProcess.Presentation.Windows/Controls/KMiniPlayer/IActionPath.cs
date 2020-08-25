namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Définit le comportement du chemin d'une action dans une lecture vidéo multi sources.
    /// </summary>
    public interface IActionPath
    {
        /// <summary>
        /// Obtient ou définit le début de l'action.
        /// </summary>
        long Start { get; set; }

        /// <summary>
        /// Obtient ou définit la fin de l'action.
        /// </summary>
        long Finish { get; set; }

        /// <summary>
        /// Obtient une valeur indiquant si l'action a une vidéo associée.
        /// </summary>
        bool HasVideo { get; }

        /// <summary>
        /// Obtient le début de l'action sur la vidéo.
        /// </summary>
        long VideoStart { get; }

        /// <summary>
        /// Obtient la fin de l'action sur la vidéo.
        /// </summary>
        long VideoFinish { get; }
    }
}
