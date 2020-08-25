// -----------------------------------------------------------------------
// <copyright file="ResourceFileProvider.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace KProcess.Globalization
{
    /// <summary>
    /// Fournisseur de resources basé sur les .resx
    /// </summary>
    public class ResourceFileProvider : ResourceManager, ILocalizedResourceProvider
    {
        #region Attributs

        private ResourceSet _resourceSet;

        #endregion

        #region Constructeur


        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ResourceFileProvider"/>.
        /// </summary>
        /// <param name="id">L'identifiant du fournisseur.</param>
        /// <param name="resourceSource">Le type correspondant au fichier de ressources.</param>
        public ResourceFileProvider(string id, Type resourceSource)
            : base(resourceSource)
        {
            UniqueID = id;
            LoadResources();
            LocalizationManager.CultureChanged += (sender, e) => LoadResources();
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ResourceFileProvider"/>.
        /// </summary>
        /// <param name="id">L'identifiant du fournisseur.</param>
        /// <param name="baseName">racine du nom de la resources</param>
        /// <param name="assembly">assembly contenant la resource</param>
        public ResourceFileProvider(string id, string baseName, Assembly assembly)
            : base(baseName, assembly)
        {
            UniqueID = id;
            LoadResources();
            LocalizationManager.CultureChanged += (sender, e) => LoadResources();
        }

        #endregion

        #region ILocalizedResourceProvider Members

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur, inutile pour ce fournisseur</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public object GetValue(string key, string resourceProviderKey = null, object defaultValue = null)
        {
            try
            {
                // Effectue la recherche en ignorant la casse de la clé
                return _resourceSet.GetObject(key, true);
            }
            catch
            {
                if (defaultValue != null)
                    return defaultValue;
                else
                    throw new KeyNotFoundException($"Resource key '{key}' not found");
            }
        }

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="key">la clé de la ressource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur, inutile pour ce fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>la valeur associée à la clé ou la valeur par défaut</returns>
        public string GetString(string key, string resourceProviderKey = null, string defaultValue = null)
        {
            return (string)GetValue(key, resourceProviderKey, defaultValue);
        }

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="id">L'identifiant de la ressource.</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>
        /// la valeur associée à la clé ou la valeur par défaut
        /// </returns>
        public string GetString(int id, string resourceProviderKey = null, string defaultValue = null)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Obtient ou définit la clé du provider
        /// </summary>
        public string UniqueID { get; set; }

        /// <summary>
        /// Retourne toutes les clés du fichier
        /// </summary>
        /// <returns>toutes les clés du fichier</returns>
        public IEnumerable<string> GetAllKeys()
        {
            var enumerator = _resourceSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (string)enumerator.Key;
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Charge les resources
        /// </summary>
        private void LoadResources()
        {
            ReleaseAllResources();
            _resourceSet = GetResourceSet(CultureInfo.CurrentCulture, true, true);
        }

        #endregion
    }
}
