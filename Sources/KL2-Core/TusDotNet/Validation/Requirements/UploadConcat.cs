using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Constants;
using TusDotNet.Extensions;
using TusDotNet.Interfaces;
using TusDotNet.Models.Concatenation;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class UploadConcat : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            if (context.Request.GetMethod().Equals("post", StringComparison.OrdinalIgnoreCase))
            {
                await ValidateForPost(context);
            }
            else
            {
                await ValidateForPatch(context);
            }
        }

        private async Task ValidateForPatch(ContextAdapter context)
        {
            if (context.Configuration.Store is ITusConcatenationStore concatStore)
            {
                var uploadConcat = await concatStore.GetUploadConcatAsync(context.GetFileId(), context.CancellationToken);

                if (uploadConcat is FileConcatFinal)
                    await Forbidden("File with \"Upload-Concat: final\" cannot be patched");
            }
            else
                await Done;
        }

        private async Task ValidateForPost(ContextAdapter context)
        {
            ITusConcatenationStore concatenationStore = context.Configuration.Store as ITusConcatenationStore;
            if (concatenationStore != null
                && context.Request.Headers.ContainsKey(HeaderConstants.UploadConcat))
            {
                var uploadConcat = new Models.Concatenation.UploadConcat(
                    context.Request.GetHeader(HeaderConstants.UploadConcat),
                    context.Configuration.UrlPath);

                if (!uploadConcat.IsValid)
                {
                    await BadRequest(uploadConcat.ErrorMessage);
                    return;
                }

                if (uploadConcat.Type is FileConcatFinal finalConcat)
                    await ValidateFinalFileCreation(finalConcat, context, concatenationStore);
            }
            else
                await Done;
        }

        private async Task ValidateFinalFileCreation(FileConcatFinal finalConcat, ContextAdapter context, ITusConcatenationStore store)
        {
            var filesExist = await Task.WhenAll(finalConcat.Files.Select(f =>
                context.Configuration.Store.FileExistAsync(f, context.CancellationToken)));

            if (filesExist.Any(f => !f))
            {
                await BadRequest(
                    $"Could not find some of the files supplied for concatenation: {string.Join(", ", filesExist.Zip(finalConcat.Files, (b, s) => new {exist = b, name = s}).Where(f => !f.exist).Select(f => f.name))}");
                return;
            }

            var filesArePartial = await Task.WhenAll(
                finalConcat.Files.Select(f => store.GetUploadConcatAsync(f, context.CancellationToken)));

            if (filesArePartial.Any(f => !(f is FileConcatPartial)))
            {
                await BadRequest($"Some of the files supplied for concatenation are not marked as partial and can not be concatenated: {string.Join(", ", filesArePartial.Zip(finalConcat.Files, (s, s1) => new { partial = s is FileConcatPartial, name = s1 }).Where(f => !f.partial).Select(f => f.name))}");
                return;
            }

            var incompleteFiles = new List<string>(finalConcat.Files.Length);
            var totalSize = 0L;
            foreach (var file in finalConcat.Files)
            {
                var length = context.Configuration.Store.GetUploadLengthAsync(file, context.CancellationToken);
                var offset = context.Configuration.Store.GetUploadOffsetAsync(file, context.CancellationToken);
                await Task.WhenAll(length, offset);

                if (length.Result != null)
                {
                    totalSize += length.Result.Value;
                }

                if (length.Result != offset.Result)
                {
                    incompleteFiles.Add(file);
                }
            }

            if (incompleteFiles.Count > 0)
            {
                await BadRequest(
                    $"Some of the files supplied for concatenation are not finished and can not be concatenated: {string.Join(", ", incompleteFiles)}");
                return;
            }

            if (totalSize > context.Configuration.MaxAllowedUploadSizeInBytes)
            {
                await RequestEntityTooLarge("The concatenated file exceeds the server's max file size.");
            }
        }
    }
}
