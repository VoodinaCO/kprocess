﻿using System;
using System.Collections.Generic;
using TusDotNet.Adapters;
using TusDotNet.Constants;
using TusDotNet.Interfaces;

namespace TusDotNet.Extensions
{
    internal static class ContextAdapterExtensions
    {
        public static string GetFileId(this ContextAdapter context)
        {
            var startIndex =
                context.Request.RequestUri.LocalPath.IndexOf(context.Configuration.UrlPath,
                    StringComparison.OrdinalIgnoreCase) + context.Configuration.UrlPath.Length;

            return context.Request
                .RequestUri
                .LocalPath
                .Substring(startIndex)
                .Trim('/');
        }

        public static bool UrlMatchesUrlPath(this ContextAdapter context)
        {
            return context.Request.RequestUri.LocalPath.TrimEnd('/') == context.Configuration.UrlPath.TrimEnd('/');
        }

        public static bool UrlMatchesFileIdUrl(this ContextAdapter context)
        {
            return !context.UrlMatchesUrlPath()
                   && context.Request.RequestUri.LocalPath.StartsWith(context.Configuration.UrlPath,
                       StringComparison.OrdinalIgnoreCase);
        }

        internal static List<string> DetectExtensions(this ContextAdapter context)
        {
            var extensions = new List<string>(6);
            if (context.Configuration.Store is ITusCreationStore)
            {
                extensions.Add(ExtensionConstants.Creation);
            }

            if (context.Configuration.Store is ITusTerminationStore)
            {
                extensions.Add(ExtensionConstants.Termination);
            }

            if (context.Configuration.Store is ITusChecksumStore)
            {
                extensions.Add(ExtensionConstants.Checksum);
            }

            if (context.Configuration.Store is ITusConcatenationStore)
            {
                extensions.Add(ExtensionConstants.Concatenation);
            }

            if (context.Configuration.Store is ITusExpirationStore)
            {
                extensions.Add(ExtensionConstants.Expiration);
            }

            if (context.Configuration.Store is ITusCreationDeferLengthStore)
            {
                extensions.Add(ExtensionConstants.CreationDeferLength);
            }

            return extensions;
        }
    }
}