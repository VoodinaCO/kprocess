﻿using System.Net;
using System.Threading.Tasks;
using TusDotNet.Adapters;

namespace TusDotNet.Validation
{
    internal abstract class Requirement
    {
        public HttpStatusCode StatusCode { get; protected set; }

        public string ErrorMessage { get; protected set; }

        public abstract Task Validate(ContextAdapter context);

        public void Reset()
        {
            StatusCode = 0;
            ErrorMessage = null;
        }

        protected Task Conflict(string errorMessage)
        {
            return Error(HttpStatusCode.Conflict, errorMessage);
        }

        protected Task BadRequest(string errorMessage)
        {
            return Error(HttpStatusCode.BadRequest, errorMessage);
        }

        protected Task RequestEntityTooLarge(string errorMessage)
        {
            return Error(HttpStatusCode.RequestEntityTooLarge, errorMessage);
        }

        protected Task Forbidden(string errorMessage)
        {
            return Error(HttpStatusCode.Forbidden, errorMessage);
        }

        protected Task NotFound()
        {
            return Error(HttpStatusCode.NotFound, null);
        }

        private Task Error(HttpStatusCode status, string errorMessage)
        {
            StatusCode = status;
            ErrorMessage = errorMessage;
            return Done;
        }

        protected Task Done => Task.FromResult(true);
    }
}
