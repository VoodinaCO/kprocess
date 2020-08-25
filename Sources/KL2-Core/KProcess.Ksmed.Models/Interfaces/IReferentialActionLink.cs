namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Représente un lien entre un référentiel et une action.
    /// </summary>
    public interface IReferentialActionLink
    {
        /// <summary>
        /// Obtient ou définit l'identifiant de l'action.
        /// </summary>
        int ActionId { get; set; }
        
        /// <summary>
        /// Obtient ou définit l'action.
        /// </summary>
        KAction Action { get; set; }

        /// <summary>
        /// Obtient ou définit l'identifiant du référentiel.
        /// </summary>
        int ReferentialId { get; set; }

        /// <summary>
        /// Obtient ou définit le référentiel associé.
        /// </summary>
        IMultipleActionReferential Referential { get; set; }

        /// <summary>
        /// Obtient ou définit la quantité.
        /// </summary>
        int Quantity { get; set; }
    }
}
