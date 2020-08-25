// -----------------------------------------------------------------------
// <copyright file="LocalizationManager.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using KProcess.KL2.Languages;
using KProcess.KL2.Languages.Provider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace KProcess.Globalization
{
    /// <summary>
    /// Gestionnaire de localisation
    /// </summary>
    public static class LocalizationManager
    {

        #region Champs statiques

        private readonly static ILocalizedStrings _localizedStrings;
        private static CultureInfo _currentCulture;
        private static List<CultureChangedSubscription> _subscriptions = new List<CultureChangedSubscription>();

        #endregion

        #region Constructeur

        static LocalizationManager()
        {
            _localizedStrings = new LocalizedStrings(new SQLiteLanguageStorageProvider("Data Source=Resources\\Localization.sqlite;"));
            SupportedCultures = _localizedStrings.GetSupportedLanguages();
            CurrentCulture = Thread.CurrentThread.CurrentCulture;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit le fournisseur de resources localisées
        /// </summary>
        public static ILocalizedResourceProvider ResourceProvider { get; set; }

        /// <summary>
        /// Obtient la liste des cultures supportées
        /// </summary>
        public static IList<CultureInfo> SupportedCultures { get; private set; }

        /// <summary>
        /// Obtient ou définit la culture courante
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                if (_currentCulture == null)
                {
                    _currentCulture = CultureInfo.CurrentCulture;
                    if(IoC.IsRegistered<ILocalizationManager>())
                        IoC.Resolve<ILocalizationManager>().CurrentCulture = _currentCulture;
                }
                return _currentCulture;
            }
            set
            {
                _currentCulture = value;
                if (IoC.IsRegistered<ILocalizationManager>())
                    IoC.Resolve<ILocalizationManager>().CurrentCulture = _currentCulture;
                Thread.CurrentThread.CurrentCulture = value;
                Thread.CurrentThread.CurrentUICulture = value;

                NotifyCultureChanged();
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public static object GetValue(string key, string providerKey = null, object defaultValue = null)
        {
            var language = CurrentCulture.ToString();
            return _localizedStrings.GetLanguageValueFormat(key, language);
        }

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public static string GetString(string key, string providerKey = null, string defaultValue = null)
        {
            var language = CurrentCulture.ToString();
            return _localizedStrings.GetLanguageValue(key, language);
        }

        /// <summary>
        /// Retourne une resource formattée en fonction de sa clé et de ses valeurs
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="args">Les arguments de formattage</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public static string GetStringFormat(string key, params object[] args)
        {
            var language = CurrentCulture.ToString();
            return _localizedStrings.GetLanguageValueFormat(key, language, args?.Select(_ => _?.ToString()).ToArray());
        }

        /// <summary>
        /// Récupère la valeur de la ressource sous forme de chaîne de caractères
        /// </summary>
        /// <param name="id">L'identifiant de la ressource.</param>
        /// <param name="providerKey">la clé du fournisseur.</param>
        /// <param name="defaultValue">la valeur par défaut à retourner</param>
        /// <returns>
        /// la valeur associée à la clé ou la valeur par défaut
        /// </returns>
        public static string GetString(int id, string providerKey = null, string defaultValue = null)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Gestion CultureChanged

        /// <summary>
        /// Déclenché lorsque la culture courante change
        /// </summary>
        public static event EventHandler CultureChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }
        /// <summary>
        /// Ajoute un gestionnaire d'événements
        /// </summary>
        /// <param name="handler">Gestionnaire d'événements à ajouter</param>
        /// <returns>une instance disposable de l'abonnement</returns>
        public static void AddHandler(EventHandler handler)
        {
            AddSubscription(new CultureChangedSubscription()
            {
                IsStatic = (handler.Target == null),
                SubscriberReference = new WeakReference(handler.Target),
                MethodCallback = handler.Method
            });
        }

        /// <summary>
        /// Supprime un gestionnaire d'événements
        /// </summary>
        /// <param name="handler">Gestionnaire d'événements à supprimer.</param>
        public static void RemoveHandler(EventHandler handler)
        {
            CleanupSubscribers();

            _subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.Method);
        }

        /// <summary>
        /// Ajoute une souscription à l'évènement <see cref="CultureChanged"/>.
        /// </summary>
        /// <param name="subscription">Une souscription à l'évènement <see cref="CultureChanged"/>.</param>
        private static void AddSubscription(CultureChangedSubscription subscription)
        {
            CleanupSubscribers();
            _subscriptions.Add(subscription);
        }

        /// <summary>
        /// Nettoie la liste des souscripteurs.
        /// </summary>
        private static void CleanupSubscribers()
        {
            _subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        /// <summary>
        /// Lève l'évènement <see cref="CultureChanged"/>.
        /// </summary>
        public static void NotifyCultureChanged()
        {
            CleanupSubscribers();

            foreach (var subscription in _subscriptions)
            {
                if (subscription.SubscriberReference.IsAlive)
                    subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { null, EventArgs.Empty });
            }
        }

        /// <summary>
        /// Représente une souscription à l'évènement CultureChanged
        /// </summary>
        private class CultureChangedSubscription
        {
            internal bool IsStatic { get; set; }
            internal WeakReference SubscriberReference { get; set; }
            internal MethodInfo MethodCallback { get; set; }
        }

        #endregion
    }
}
