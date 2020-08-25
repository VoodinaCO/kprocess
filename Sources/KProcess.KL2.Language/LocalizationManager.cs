using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace KProcess.KL2.Languages
{
    public class LocalizationManager : ILocalizationManager
    {
        private readonly ILocalizedStrings _localizedStrings;
        private readonly List<CultureChangedSubscription> _subscriptions = new List<CultureChangedSubscription>();

        /// <summary>
        /// Initialise le LocalizationManager.
        /// </summary>
        public LocalizationManager(ILocalizedStrings localizedStrings)
        {
            _localizedStrings = localizedStrings;
            SupportedCultures = _localizedStrings.GetSupportedLanguages();
        }

        /// <summary>
        /// Obtient la liste des cultures supportées
        /// </summary>
        public IList<CultureInfo> SupportedCultures { get; private set; }

        /// <summary>
        /// Obtient ou définit la culture courante
        /// </summary>
        private CultureInfo _currentCulture = new CultureInfo("fr-FR");
        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value;
                Thread.CurrentThread.CurrentCulture = value;
                Thread.CurrentThread.CurrentUICulture = value;

                NotifyCultureChanged();
            }
        }

        /// <summary>
        /// Retourne une resource en fonction de sa clé
        /// </summary>
        /// <param name="key">clé de la resource</param>
        /// <param name="providerKey">clé du provider</param>
        /// <param name="defaultValue">valeur par défaut si la resource n'est pas trouvée</param>
        /// <returns>la valeur de la resource dans la culture courante</returns>
        public object GetValue(string key, string providerKey = null, object defaultValue = null)
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
        public string GetString(string key, string providerKey = null, string defaultValue = null)
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
        public string GetStringFormat(string key, params object[] args)
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
        public string GetString(int id, string providerKey = null, string defaultValue = null)
        {
            throw new NotImplementedException();
            /*using (var context = ContextFactory.GetNewContext())
            {
                var resource = context.AppResourceKeys.SingleOrDefault(_ => _.ResourceId == id);
                return GetString(resource?.ResourceKey);
            }*/
        }

        /// <summary>
        /// Obtient le nom complet d'une personne de manière localisée.
        /// </summary>
        /// <param name="firstName">Le prénom.</param>
        /// <param name="lastName">le nom.</param>
        /// <returns></returns>
        public string GetLocalizedFullName(string firstName, string lastName)
        {
            string loc = GetString("Common_UserFullName");
            if (!string.IsNullOrEmpty(loc))
                return string.Format(
                    loc,
                    firstName ?? string.Empty,
                    lastName ?? string.Empty);
            return $"{(firstName ?? string.Empty)} {(lastName ?? string.Empty)}";
        }

        /// <summary>
        /// Déclenché lorsque la culture courante change
        /// </summary>
        public event EventHandler CultureChanged
        {
            add { AddHandler(value); }
            remove { RemoveHandler(value); }
        }

        /// <summary>
        /// Ajoute un gestionnaire d'événements
        /// </summary>
        /// <param name="handler">Gestionnaire d'événements à ajouter</param>
        /// <returns>une instance disposable de l'abonnement</returns>
        public void AddHandler(EventHandler handler)
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
        public void RemoveHandler(EventHandler handler)
        {
            CleanupSubscribers();

            _subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.Method);
        }

        /// <summary>
        /// Ajoute une souscription à l'évènement <see cref="CultureChanged"/>.
        /// </summary>
        /// <param name="subscription">Une souscription à l'évènement <see cref="CultureChanged"/>.</param>
        private void AddSubscription(CultureChangedSubscription subscription)
        {
            CleanupSubscribers();
            _subscriptions.Add(subscription);
        }

        /// <summary>
        /// Nettoie la liste des souscripteurs.
        /// </summary>
        private void CleanupSubscribers()
        {
            _subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        /// <summary>
        /// Lève l'évènement <see cref="CultureChanged"/>.
        /// </summary>
        public void NotifyCultureChanged()
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
    }
}
