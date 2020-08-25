// -----------------------------------------------------------------------
// <copyright file="IEventBus.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace KProcess.Presentation.Windows
{
    #region Déclaration des enums

    /// <summary>
    /// Détermine les options de threading lors de l'exécution du callback
    /// </summary>
    public enum ThreadingStrategy
    {
        /// <summary>
        /// le callback est déclenché sur le même thread que celui qui a publié l'événement
        /// </summary>
        Publisher,
        /// <summary>
        /// le callback est déclenché sur un thread d'arrière plan
        /// </summary>
        Background,
        /// <summary>
        /// le callback est déclenché sur le thread de présentation
        /// </summary>
        UI
    }

    #endregion

    /// <summary>
    /// Définit le contrat d'un bus d'événements
    /// </summary>
    [InheritedExport]
    public interface IEventBus
    {
        /// <summary>
        /// Publie un événement sur le bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement à publier</typeparam>
        /// <param name="item">événement à publier</param>
        /// <returns>l'instance du bus pour une écriture fluide</returns>
        IEventBus Publish<TEvent>(TEvent item) where TEvent : EventBase;

        /// <summary>
        /// S'abonne à un événement publié sur le bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est fait</typeparam>
        /// <param name="callback">handler utilisé lors de la levée de l'événement</param>
        /// <param name="throwOnce">indique si l'abonnement est maintenu ou non après le premier déclenchement</param>
        /// <param name="target">cible de l'abonnement</param>
        /// <returns>l'instance du bus pour une écriture fluide</returns>
        /// <remarks>Attention en cas d'utilisation de lambda à ce que ce ne soit pas un champs statique (=> la lambda doit faire référence à la classe qui la contient</remarks>
        IEventBus Subscribe<TEvent>(Action<TEvent> callback, bool throwOnce = false, object target = null) where TEvent : EventBase;

        /// <summary>
        /// S'abonne à un événement publié sur le bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est fait</typeparam>
        /// <param name="callback">handler utilisé lors de la levée de l'événement</param>
        /// <param name="filter">filtre permettant de savoir si le handler doit être exécuté ou non</param>
        /// <param name="throwOnce">indique si l'abonnement est maintenu ou non après le premier déclenchement</param>
        /// <param name="target">cible de l'abonnement</param>
        /// <returns>l'instance du bus pour une écriture fluide</returns>
        /// <remarks>Attention en cas d'utilisation de lambda à ce que ce ne soit pas un champs statique (=> la lambda doit faire référence à la classe qui la contient</remarks>
        IEventBus Subscribe<TEvent>(Action<TEvent> callback, Predicate<TEvent> filter, bool throwOnce = false, object target = null) where TEvent : EventBase;

        /// <summary>
        /// S'abonne à un événement publié sur le bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est fait</typeparam>
        /// <param name="callback">handler utilisé lors de la levée de l'événement</param>
        /// <param name="strategy">stratégie de threading lors de l'exécution du callback</param>
        /// <param name="throwOnce">indique si l'abonnement est maintenu ou non après le premier déclenchement</param>
        /// <param name="target">cible de l'abonnement</param>
        /// <returns>l'instance du bus pour une écriture fluide</returns>
        /// <remarks>Attention en cas d'utilisation de lambda à ce que ce ne soit pas un champs statique (=> la lambda doit faire référence à la classe qui la contient</remarks>
        IEventBus Subscribe<TEvent>(Action<TEvent> callback, ThreadingStrategy strategy, bool throwOnce = false, object target = null) where TEvent : EventBase;

        /// <summary>
        /// S'abonne à un événement publié sur le bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est fait</typeparam>
        /// <param name="callback">handler utilisé lors de la levée de l'événement</param>
        /// <param name="filter">filtre permettant de savoir si le handler doit être exécuté ou non</param>
        /// <param name="strategy">stratégie de threading lors de l'exécution du callback</param>
        /// <param name="throwOnce">indique si l'abonnement est maintenu ou non après le premier déclenchement</param>
        /// <param name="target">cible de l'abonnement</param>
        /// <returns>l'instance du bus pour une écriture fluide</returns>
        /// <remarks>Attention en cas d'utilisation de lambda à ce que ce ne soit pas un champs statique (=> la lambda doit faire référence à la classe qui la contient</remarks>
        IEventBus Subscribe<TEvent>(Action<TEvent> callback, Predicate<TEvent> filter, ThreadingStrategy strategy, bool throwOnce = false, object target = null) where TEvent : EventBase;

        /// <summary>
        /// Permet de se désabonner de tous les événements du bus
        /// </summary>
        /// <param name="target">cible à supprimer des listes d'invocation</param>
        void Unsubscribe(object target);

        /// <summary>
        /// Permet de se désabonner d'un événement particulier du bus
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est défait</typeparam>
        /// <param name="callback">callback à supprimer de la liste d'invocations du bus</param>
        void Unsubscribe<TEvent>(Action<TEvent> callback);

        /// <summary>
        /// Permet de se désabonner d'un événement particulier du bus pour une cible donnée
        /// </summary>
        /// <typeparam name="TEvent">type d'événement pour lequel l'abonnement est défait</typeparam>
        /// <param name="target">cible à supprimer de la liste d'invocations du bus</param>
        void Unsubscribe<TEvent>(object target);
    }
}
