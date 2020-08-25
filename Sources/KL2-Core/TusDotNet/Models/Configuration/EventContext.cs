﻿using System;
using System.Threading;
using TusDotNet.Adapters;
using TusDotNet.Extensions;
using TusDotNet.Interfaces;

namespace TusDotNet.Models.Configuration
{
    /// <summary>
    /// Base context for all events in tusdotnet
    /// </summary>
    /// <typeparam name="TSelf">The type of the derived class inheriting the EventContext</typeparam>
    public abstract class EventContext<TSelf> where TSelf : EventContext<TSelf>, new()
    {
        /// <summary>
        /// The id of the file that was completed
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// The store that was used when completing the upload
        /// </summary>
        public ITusStore Store { get; set; }

        /// <summary>
        /// The request's cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        internal static TSelf Create(ContextAdapter context, Action<TSelf> configure = null)
        {
            var fileId = context.GetFileId();
            if (string.IsNullOrEmpty(fileId))
            {
                fileId = null;
            }

            var eventContext = new TSelf
            {
                Store = context.Configuration.Store,
                CancellationToken = context.CancellationToken,
                FileId = fileId
            };

            configure?.Invoke(eventContext);

            return eventContext;
        }
    }
}
