using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KProcess.Globalization
{
    /// <summary>
    /// Représente un fournisseur de ressources qui agrège plusieurs autres fournisseurs de ressources.
    /// </summary>
    public class AggregateResourceProvider : ObservableCollection<ILocalizedResourceProvider>, ILocalizedResourceProvider
    {

        private SortedDictionary<string, ILocalizedResourceProvider> _resourcesSet;

        #region Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AggregateResourceProvider"/>.
        /// </summary>
        public AggregateResourceProvider()
        {
            Initialize(null);
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AggregateResourceProvider"/>.
        /// </summary>
        /// <param name="providers">Les fournisseurs à agréger.</param>
        public AggregateResourceProvider(params ILocalizedResourceProvider[] providers)
            : this(providers.AsEnumerable())
        {
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AggregateResourceProvider"/>.
        /// </summary>
        /// <param name="collection">Les fournisseurs à agréger.</param>
        public AggregateResourceProvider(IEnumerable<ILocalizedResourceProvider> collection)
        {
            Initialize(collection);
        }

        #endregion

        /// <summary>
        /// Initialise le fournisseur.
        /// </summary>
        /// <param name="collection">Les fournisseurs à agréger.</param>
        private void Initialize(IEnumerable<ILocalizedResourceProvider> collection)
        {
            _resourcesSet = new SortedDictionary<string, ILocalizedResourceProvider>();

            if (collection != null)
            {
                foreach (var item in collection)
                    base.Add(item);
            }
        }

        #region ObservableCollection overrides

        /// <summary>
        /// Insère un objet.
        /// </summary>
        /// <param name="index">L'index.</param>
        /// <param name="item">L'objet.</param>
        protected override void InsertItem(int index, ILocalizedResourceProvider item)
        {
            base.InsertItem(index, item);

            // Load all the resource names from the provider
            foreach (var key in item.GetAllKeys())
            {
                try
                {
                    _resourcesSet.Add(key, item);
                }
                catch (ArgumentException e)
                {
                    throw ExceptionManager.Wrap<FWTException>(e,
                        "The specified Resource key already exists. You may remove it from the provider. Key = '{0}'. ProviderID = '{1}')", key, item.UniqueID);
                }
            }
        }

        /// <summary>
        /// Supprimer l'objet.
        /// </summary>
        /// <param name="index">L'index.</param>
        protected override void RemoveItem(int index)
        {
            ILocalizedResourceProvider item = base[index];

            var keysToDelete = _resourcesSet.Where(kvp => kvp.Value == item).Select(kvp => kvp.Key).ToList();
            // Remove elements from the dictionary
            foreach (var key in keysToDelete)
                _resourcesSet.Remove(key);

            base.RemoveItem(index);
        }

        #endregion

        #region ILocalizedResourceProvider Implementation

        /// <summary>
        /// Correspond à l'identifiant du provider
        /// </summary>
        public string UniqueID { get; private set; }

        /// <summary>
        /// Récupère la valeur de la ressource
        /// </summary>
        /// <param name="key">la clé de la ressource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>la valeur associée à la clé ou la valeur par défaut</returns>
        public object GetValue(string key, string resourceProviderKey = null, object defaultValue = null)
        {
            Assertion.NotNull(key, "key");

            ILocalizedResourceProvider provider;

            if (resourceProviderKey != null)
            {
                // Get the right provider
                provider = this.FirstOrDefault(p => p.UniqueID.CompareTo(resourceProviderKey) == 0);
                if (provider == null)
                    throw new ArgumentException("The specified resourceProviderKey is not in the providers list");

            }
            else
            {
                // Get the right provider
                if (!_resourcesSet.TryGetValue(key, out provider))
                    return key;
            }

            return provider.GetValue(key, resourceProviderKey, defaultValue);
        }


        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="key">la clé de la ressource</param>
        /// <param name="resourceProviderKey">la clé du fournisseur</param>
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
            foreach (var provider in this)
            {
                var res = provider.GetString(id, resourceProviderKey, defaultValue);
                if (res != null)
                    return res;
            }

            return null;
        }

        /// <summary>
        /// Retourne toutes les clés présentes dans le fournisseur
        /// </summary>
        /// <returns>toutes les clés présentes dans le fournisseur</returns>
        public IEnumerable<string> GetAllKeys()
        {
            return this.SelectMany(provider => provider.GetAllKeys());
        }

        #endregion

    }
}
