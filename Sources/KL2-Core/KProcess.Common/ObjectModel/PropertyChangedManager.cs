//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : PropertyChangedManager.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Permet de gérer l'événement PropertyChanged via des événements faibles.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace KProcess
{
    /// <summary>
    /// Permet de gérer l'événement PropertyChanged d'une classe via des événements faibles.
    /// </summary>
    public class PropertyChangedManager : IDisposable
    {
        #region Attributs

        private List<Subscription> _subscriptions = new List<Subscription>();
        private WeakReference _source;
        private bool _isDisposed = false;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="source">Source à surveiller.</param>
        public PropertyChangedManager(INotifyPropertyChanged source)
        {
            _source = new WeakReference(source);
        }

        #endregion

        #region Destructeur

        /// <summary>
        /// Destructeur.
        /// </summary>
        ~PropertyChangedManager()
        {
            Dispose(false);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Permet de s'abonner à l'événement PropertyChanged
        /// </summary>
        /// <typeparam name="T">Type de l'objet à surveiller</typeparam>
        /// <param name="propertyName">Nom de la propriété.</param>
        /// <param name="callback">Délégué à appelé lorsque l'événement est déclenché.</param>
        /// <returns>une instance disposable de l'abonnement</returns>
        public IDisposable SubscribeToPropertyChanged<T>(string propertyName, Action<T> callback)
            where T : INotifyPropertyChanged
        {
            return AddSubscription(new Subscription
            {
                IsStatic = (callback.Target == null),
                PropertyName = propertyName,
                SubscriberReference = new WeakReference(callback.Target),
                MethodCallback = callback.Method
            });
        }

        /// <summary>
        /// Permet de se désabonner de l'événement PropertyChanged
        /// </summary>
        /// <typeparam name="T">Type de l'objet surveillé</typeparam>
        /// <param name="propertyName">Nom de la propriété.</param>
        /// <param name="callback">Délégué à appelé lorsque l'événement est déclenché.</param>
        public void UnsubscribeToPropertyChanged<T>(string propertyName, Action<T> callback)
            where T : INotifyPropertyChanged
        {
            _subscriptions.RemoveWhere(s => s.PropertyName == propertyName && s.MethodCallback == callback.Method);
        }

        /// <summary>
        /// Ajoute un gestionnaire d'événements
        /// </summary>
        /// <param name="handler">Gestionnaire d'événements à ajouter</param>
        /// <returns>une instance disposable de l'abonnement</returns>
        public IDisposable AddHandler(PropertyChangedEventHandler handler)
        {
            return AddSubscription(new Subscription
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
        public void RemoveHandler(PropertyChangedEventHandler handler)
        {
            CleanupSubscribers();

            _subscriptions.RemoveAll(s => s.SubscriberReference.Target == handler.Target && s.MethodCallback == handler.Method);
        }

        /// <summary>
        /// Notifie d'un changement de valeur de la propriété
        /// </summary>
        /// <param name="propertyName">Nom de la propriété</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            CleanupSubscribers();

            var subscriptions = _subscriptions.Where(s => s.PropertyName == propertyName).ToArray();
            foreach (var subscription in subscriptions)
                subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { _source.Target });

            subscriptions = _subscriptions.Where(s => s.PropertyName == null).ToArray();
            foreach (var subscription in subscriptions)
                subscription.MethodCallback.Invoke(subscription.SubscriberReference.Target, new object[] { _source.Target, new PropertyChangedEventArgs(propertyName) });
        }

        #endregion

        #region Méthodes privées

        private IDisposable AddSubscription(Subscription subscription)
        {
            CleanupSubscribers();

            _subscriptions.Add(subscription);

            return new SubscriptionReference(_subscriptions, subscription);
        }

        private void CleanupSubscribers()
        {
            _subscriptions.RemoveAll(s => !s.IsStatic && !s.SubscriberReference.IsAlive);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Nettoie les resources utilisées
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Nettoie les resources utilisées
        /// </summary>
        /// <param name="disposing">indique si le dispose est en cours</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if (disposing)
                {
                    if (_subscriptions != null)
                        _subscriptions.Clear();
                }
            }
        }

        #endregion

        #region Classes internes

        private sealed class SubscriptionReference : IDisposable
        {
            private List<Subscription> subscriptions;
            private Subscription entry;

            public SubscriptionReference(List<Subscription> subscriptions, Subscription entry)
            {
                this.subscriptions = subscriptions;
                this.entry = entry;
            }

            public void Dispose()
            {
                this.subscriptions.Remove(this.entry);
            }
        }

        private class Subscription
        {
            public bool IsStatic { get; set; }
            public string PropertyName { get; set; }
            public WeakReference SubscriberReference { get; set; }
            public MethodInfo MethodCallback { get; set; }
        }

        #endregion
    }
}
