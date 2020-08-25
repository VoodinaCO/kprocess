namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Définit le comportement d'un référentiel de catégorie d'action.
    /// </summary>
    public interface IActionCategory :IActionReferentialProcess
    {
        /// <summary>
        /// Obtient ou définit le code du type d'action.
        /// </summary>
        string ActionTypeCode { get; set; }

        /// <summary>
        /// Obtient ou définit le code de la valorisation de l'action.
        /// </summary>
        string ActionValueCode { get; set; }
    }
}
