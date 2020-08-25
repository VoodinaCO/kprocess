// -----------------------------------------------------------------------
// <copyright file="DispatcherHelper.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Propose des méthodes utilitaires concernant le dispatcher
    /// </summary>
    public static class DispatcherHelper
    {
        #region Attributs

        private static Dispatcher _dispatcher;

        #endregion

        #region Constructeur

        static DispatcherHelper()
        {
#if SILVERLIGHT
            _dispatcher = System.Windows.Deployment.Current.Dispatcher;
#else
            _dispatcher = Dispatcher.CurrentDispatcher;
#endif
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <param name="action">le délégué à exécuter</param>
        public static void SafeInvoke(Action action)
        {
            SafeInvoke(action, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <param name="action">le délégué à exécuter</param>
        /// <param name="priorityWhenInvoking">définit la priorité à appliquer lorsque le délégué est invoqué sur le dispatcher</param>
        public static void SafeInvoke(Action action, DispatcherPriority priorityWhenInvoking)
        {
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(action, priorityWhenInvoking);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <typeparam name="T">type du paramètre</typeparam>
        /// <param name="action">le délégué à exécuter</param>
        /// <param name="obj">paramètre du délégué</param>
        public static void SafeInvoke<T>(Action<T> action, T obj)
        {
            SafeInvoke<T>(action, obj, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <typeparam name="T">type du paramètre</typeparam>
        /// <param name="action">le délégué à exécuter</param>
        /// <param name="obj">paramètre du délégué</param>
        /// <param name="priorityWhenInvoking">définit la priorité à appliquer lorsque le délégué est invoqué sur le dispatcher</param>
        public static void SafeInvoke<T>(Action<T> action, T obj, DispatcherPriority priorityWhenInvoking)
        {
            if (_dispatcher.CheckAccess())
                action(obj);
            else
                _dispatcher.BeginInvoke(action, priorityWhenInvoking, obj);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <param name="d">délégué à invoquer</param>
        /// <param name="args">liste d'arguments du délégué</param>
        public static void SafeInvoke(Delegate d, params object[] args)
        {
            SafeInvoke(d, DispatcherPriority.Normal, args);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI de façon certaine
        /// </summary>
        /// <param name="d">délégué à invoquer</param>
        /// <param name="args">liste d'arguments du délégué</param>
        /// <param name="priorityWhenInvoking">définit la priorité à appliquer lorsque le délégué est invoqué sur le dispatcher</param>
        public static void SafeInvoke(Delegate d, DispatcherPriority priorityWhenInvoking, params object[] args)
        {
            if (_dispatcher.CheckAccess())
                d.DynamicInvoke(args);
            else
                _dispatcher.BeginInvoke(d, priorityWhenInvoking, args);
        }

        /// <summary>
        /// Invoque un délégué sur le thread UI.
        /// </summary>
        /// <param name="d">délégué à invoquer</param>
        /// <param name="args">liste d'arguments du délégué</param>
        /// <param name="priorityWhenInvoking">définit la priorité à appliquer lorsque le délégué est invoqué sur le dispatcher</param>
        public static void BeginInvoke(Action d, DispatcherPriority priorityWhenInvoking, params object[] args)
        {
            _dispatcher.BeginInvoke(d, priorityWhenInvoking, args);
        }

        #endregion
    }
}
