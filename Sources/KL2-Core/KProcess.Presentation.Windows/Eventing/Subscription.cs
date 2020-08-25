// -----------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit un abonnement à un événement du bus d'événements
    /// </summary>
    internal class Subscription
    {
        #region Attributs

        private WeakDelegate _callback;
        private WeakDelegate _filter;
        private ThreadingStrategy _threadingStrategy;
        private bool _throwOnce;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="callback">callback à invoquer</param>
        /// <param name="filter">filtre permettant de savoir si le callback doit être invoqué</param>
        /// <param name="threadingStrategy">stratégie de threading d'exécution du callback</param>
        /// <param name="throwOnce">Si définie à <c>true</c>, l'invocation du callback n'est réalisée qu'une fois.</param>
        /// <param name="target">La cible de de l'abonnement.</param>
        public Subscription(Delegate callback, Delegate filter, ThreadingStrategy threadingStrategy, bool throwOnce, object target)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _callback = new WeakDelegate(callback, target);

            if (filter != null)
                _filter = new WeakDelegate(filter, target);

            _threadingStrategy = threadingStrategy;

            _throwOnce = throwOnce;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient l'instance cible du callback
        /// </summary>
        public object Target
        {
            get { return _callback.Target; }
        }

        /// <summary>
        /// Obtient le délégué du callback
        /// </summary>
        public Delegate Action
        {
            get { return _callback.GetDelegate(); }
        }

        /// <summary>
        /// Obtient le délégué du filtre
        /// </summary>
        public Delegate Filter
        {
            get { return (_filter != null) ? _filter.GetDelegate() : null; }
        }

        /// <summary>
        /// Obtient la stratégie de threading
        /// </summary>
        public ThreadingStrategy ThreadingStrategy
        {
            get { return _threadingStrategy; }
        }

        /// <summary>
        /// Indique si l'abonnement est maintenu après le premier déclenchement
        /// </summary>
        public bool ThrowOnce
        {
            get { return _throwOnce; }
        }

        #endregion
    }
}
