using System.Threading.Tasks;
using TusDotNet.Adapters;
using TusDotNet.Extensions;

namespace TusDotNet.Validation.Requirements
{
    internal sealed class FileIsNotCompleted : Requirement
    {
        public override async Task Validate(ContextAdapter context)
        {
            var fileId = context.GetFileId();

            var fileUploadLength = context.Configuration.Store.GetUploadLengthAsync(fileId, context.CancellationToken);
            var fileOffset =
                context.Configuration.Store.GetUploadOffsetAsync(context.GetFileId(), context.CancellationToken);

            await Task.WhenAll(fileUploadLength, fileOffset);

            if (fileUploadLength != null && fileOffset.Result == fileUploadLength.Result)
            {
                await BadRequest("Upload is already complete.");
            }
        }
    }
}
