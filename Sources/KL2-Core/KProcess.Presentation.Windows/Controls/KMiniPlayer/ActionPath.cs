using KProcess.Ksmed.Models;

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Représente une partie d'une action..
    /// </summary>
    public class ActionPath : IActionPath
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ActionPath"/>.
        /// </summary>
        /// <param name="action">L'action.</param>
        public ActionPath(KAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Obtient l'action associée.
        /// </summary>
        public KAction Action { get; private set; }

        /// <summary>
        /// Obtient une valeur indiquant si l'action a une vidéo associée.
        /// </summary>
        public bool HasVideo =>
            Action.Video != null;

        /// <summary>
        /// Obtient le début de l'action sur la vidéo.
        /// </summary>
        public long VideoStart =>
            Action.Start;

        /// <summary>
        /// Obtient la fin de l'action sur la vidéo.
        /// </summary>
        public long VideoFinish =>
            Action.Finish;

        /// <summary>
        /// Obtient ou définit le début de l'action sur le chemin critique.
        /// </summary>
        public long Start { get; set; }

        /// <summary>
        /// Obtient ou définit la fin de l'action sur le chemin critique.
        /// </summary>
        public long Finish { get; set; }
    }
}
