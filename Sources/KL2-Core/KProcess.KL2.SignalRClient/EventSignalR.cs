﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace KProcess.KL2.SignalRClient
{
    [Export(typeof(IEventSignalR))]
    public class EventSignalR : IEventSignalR
    {
        readonly List<SignalRHandle> _handlers = new List<SignalRHandle>();

        /// <summary>
        /// Processing of handler results on publication thread.
        /// </summary>
        public static Action<object, object> HandlerResultProcessing = (target, result) => { };

        /// <summary>
        /// Searches the subscribed handlers to check if we have a handler for
        /// the message type supplied.
        /// </summary>
        /// <param name="messageType">The message type to check with</param>
        /// <returns>True if any handler is found, false if not.</returns>
        public bool HandlerExistsFor(Type messageType)
        {
            lock (_handlers)
            {
                return _handlers.Any(handler => handler.Handles(messageType) & !handler.IsDead);
            }
        }

        /// <summary>
        /// Subscribes an instance to all events declared through implementations of <see cref = "ISignalRHandle{T}" />
        /// </summary>
        /// <param name = "subscriber">The instance to subscribe for event publication.</param>
        public virtual void Subscribe(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(Subscribe));

            lock (_handlers)
            {
                if (_handlers.Any(x => x.Matches(subscriber)))
                    return;

                _handlers.Add(new SignalRHandle(subscriber));
            }
        }

        /// <summary>
        /// Unsubscribes the instance from all events.
        /// </summary>
        /// <param name = "subscriber">The instance to unsubscribe.</param>
        public virtual void Unsubscribe(object subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException(nameof(subscriber));

            lock (_handlers)
            {
                var found = _handlers.FirstOrDefault(x => x.Matches(subscriber));

                if (found != null)
                    _handlers.Remove(found);

            }
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <param name = "message">The message instance.</param>
        /// <param name = "marshal">Allows the publisher to provide a custom thread marshaller for the message publication.</param>
        public virtual void Publish(object message, Action<Action> marshal)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (marshal == null)
                throw new ArgumentNullException(nameof(marshal));

            SignalRHandle[] toNotify;
            lock (_handlers)
                toNotify = _handlers.ToArray();

            marshal(() =>
            {
                var messageType = message.GetType();

                var dead = toNotify.Where(handler => !handler.Handle(messageType, message))
                                   .ToList();

                if (dead.Any())
                {
                    lock (_handlers)
                    {
                        dead.ForEach(x =>
                            _handlers.Remove(x)
                        );
                    }
                }
            });
        }

        class SignalRHandle
        {
            readonly WeakReference _reference;
            readonly Dictionary<Type, MethodInfo> _supportedHandlers = new Dictionary<Type, MethodInfo>();

            public bool IsDead => _reference.Target == null;

            public SignalRHandle(object handler)
            {
                _reference = new WeakReference(handler);

                var interfaces = handler.GetType().GetInterfaces()
                    .Where(x => typeof(ISignalRHandle).IsAssignableFrom(x) && x.IsGenericType);

                foreach (var @interface in interfaces)
                {
                    var type = @interface.GetGenericArguments()[0];
                    var method = @interface.GetMethod("SignalRHandler", new[] { type });

                    if (method != null)
                        _supportedHandlers[type] = method;
                }
            }

            public bool Matches(object instance) => _reference.Target == instance;

            public bool Handle(Type messageType, object message)
            {
                var target = _reference.Target;
                if (target == null)
                    return false;

                foreach (var pair in _supportedHandlers)
                {
                    if (pair.Key.IsAssignableFrom(messageType))
                    {
                        var result = pair.Value.Invoke(target, new[] { message });
                        if (result != null)
                            HandlerResultProcessing(target, result);
                    }
                }

                return true;
            }

            public bool Handles(Type messageType) => _supportedHandlers.Any(pair => pair.Key.IsAssignableFrom(messageType));
        }
    }
}
