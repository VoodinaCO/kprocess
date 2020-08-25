﻿using System.Net;
using System.Text;
using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Constants;

namespace TusDotNet.Extensions
{
    internal static class ResponseAdapterExtensions
    {
        internal static bool NotFound(this ResponseAdapter response)
        {
            response.SetHeader(HeaderConstants.TusResumable, HeaderConstants.TusResumableValue);
            response.SetHeader(HeaderConstants.CacheControl, HeaderConstants.NoStore);
            response.SetStatus((int)HttpStatusCode.NotFound);
            return true;
        }

        internal static async Task<bool> Error(this ResponseAdapter response, HttpStatusCode statusCode, string message)
        {
            response.SetHeader(HeaderConstants.ContentType, "text/plain");
            response.SetHeader(HeaderConstants.TusResumable, HeaderConstants.TusResumableValue);
            response.SetStatus((int)statusCode);
            var buffer = new UTF8Encoding().GetBytes(message);
            await response.Body.WriteAsync(buffer, 0, buffer.Length);
            return true;
        }
    }
}
