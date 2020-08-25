// -----------------------------------------------------------------------
// <copyright file="ILocalizedResourceProvider.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System.Collections.Generic;
namespace KProcess.Globalization
{
    /// <summary>
    /// Définit le contrat d'un fournisseur de resources localisées
    /// </summary>
    public interface ILocalizedResourceProvider
    {

        /// <summary>
        /// L'identifiant de ce provider. Doit être unique pour chaque fournisseur utilisé dans une même application
        /// </summary>
        string UniqueID { get; }

        /// <summary>
        /// Récupère la valeur de la ressource
        /// </summary>
        /// <param name="key">la clé de la ressource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>la valeur associée à la clé ou la valeur par défaut</returns>
        object GetValue(string key, string resourceProviderKey = null, object defaultValue = null);

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="key">la clé de la ressource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>la valeur associée à la clé ou la valeur par défaut</returns>
        string GetString(string key, string resourceProviderKey = null, string defaultValue = null);

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="id">L'identifiant de la ressource.</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>
        /// la valeur associée à la clé ou la valeur par défaut
        /// </returns>
        string GetString(int id, string resourceProviderKey = null, string defaultValue = null);

        /// <summary>
        /// Retourne toutes les clés présentes dans le fournisseur
        /// </summary>
        /// <returns>toutes les clés présentes dans le fournisseur</returns>
        IEnumerable<string> GetAllKeys();

    }
}
