namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'un modèle qui peut comporter un label localisé.
    /// </summary>
    public interface ILocalizedLabels
    {

        /// <summary>
        /// Obtient l'identifiant de la ressource pour le libellé court.
        /// </summary>
        int ShortLabelResourceId { get; }

        /// <summary>
        /// Obtient l'identifiant de la ressource pour le libellé long.
        /// </summary>
        int LongLabelResourceId { get; }


        /// <summary>
        /// Obtient ou définit le libellé court.
        /// </summary>
        string ShortLabel { get; set; }

        /// <summary>
        /// Obtient ou définit le libellé long.
        /// </summary>
        string LongLabel { get; set; }

    }
}
