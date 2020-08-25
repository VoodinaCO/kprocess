using System.Collections.Generic;
using System.Globalization;

namespace KProcess.KL2.Languages
{
    /// <summary>
    /// Gestionnaire de localisation
    /// </summary>
    public interface ILocalizationManager
    {
        #region Propriétés

        /// <summary>
        /// Obtient la liste des cultures supportées
        /// </summary>
        IList<CultureInfo> SupportedCultures { get; }

        /// <summary>
        /// Obtient ou définit la culture courante
        /// </summary>
        CultureInfo CurrentCulture { get; set; }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        object GetValue(string key, string providerKey = null, object defaultValue = null);

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        string GetString(string key, string providerKey = null, string defaultValue = null);

        /// <summary>
        /// Retourne une resource formattée en fonction de sa clé et de ses valeurs
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="args">Les arguments de formattage</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        string GetStringFormat(string key, params object[] args);

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="id">L'identifiant de la ressource.</param>
        /// <param name="providerKey">la clé du fournisseur.</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>
        /// la valeur associée à la clé ou la valeur par défaut
        /// </returns>
        string GetString(int id, string providerKey = null, string defaultValue = null);

        /// <summary>
        /// Obtient le nom complet d'une personne de manière localisée.
        /// </summary>
        /// <param name="firstName">Le prénom.</param>
        /// <param name="lastName">le nom.</param>
        /// <returns></returns>
        string GetLocalizedFullName(string firstName, string lastName);

        #endregion
    }
}
